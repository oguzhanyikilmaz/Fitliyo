"use client";

import { useEffect, useState } from "react";
import { apiFetch, buildQuery, DEFAULT_LIST_PARAMS } from "@/lib/api";
import { ApiPaths } from "@/lib/api-paths";
import type {
  CreateUpdateFeaturedListingDto,
  FeaturedListingDto,
  GetFeaturedListingListDto,
  PagedResultDto,
} from "@/lib/types";

const PAGE_TYPE_LABELS: Record<number, string> = {
  0: "Ana Sayfa",
  1: "Kategori",
  2: "Arama",
};

type FeaturedFormState = {
  pageType: number;
  trainerProfileId: string;
  servicePackageId: string;
  sortOrder: number;
  startDate: string;
  endDate: string;
  isActive: boolean;
  adminNote: string;
};

const defaultForm: FeaturedFormState = {
  pageType: 0,
  trainerProfileId: "",
  servicePackageId: "",
  sortOrder: 1,
  startDate: "",
  endDate: "",
  isActive: true,
  adminNote: "",
};

function toLocalDateInput(value?: string | null): string {
  if (!value) return "";
  return value.slice(0, 16);
}

export default function AdminFeaturedPage() {
  const [data, setData] = useState<PagedResultDto<FeaturedListingDto> | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [form, setForm] = useState<FeaturedFormState>(defaultForm);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [deletingId, setDeletingId] = useState<string | null>(null);

  const load = () => {
    setLoading(true);
    setError(null);
    const params: GetFeaturedListingListDto = { ...DEFAULT_LIST_PARAMS };
    const query = buildQuery(params as Record<string, string | number | boolean | undefined | null>);
    apiFetch<PagedResultDto<FeaturedListingDto>>(ApiPaths.FeaturedListing.getListAsync(query))
      .then(setData)
      .catch((e) => setError(e instanceof Error ? e.message : "Öne çıkanlar yüklenemedi"))
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    load();
  }, []);

  const resetForm = () => {
    setForm(defaultForm);
    setEditingId(null);
    setSubmitError(null);
  };

  const startEdit = (item: FeaturedListingDto) => {
    setEditingId(item.id);
    setSubmitError(null);
    setForm({
      pageType: item.pageType,
      trainerProfileId: item.trainerProfileId ?? "",
      servicePackageId: item.servicePackageId ?? "",
      sortOrder: item.sortOrder,
      startDate: toLocalDateInput(item.startDate),
      endDate: toLocalDateInput(item.endDate),
      isActive: item.isActive,
      adminNote: item.adminNote ?? "",
    });
  };

  const submit = () => {
    if (!form.trainerProfileId.trim() && !form.servicePackageId.trim()) {
      setSubmitError("Eğitmen veya paket ID alanlarından en az biri dolu olmalı.");
      return;
    }

    setSubmitting(true);
    setSubmitError(null);
    const body: CreateUpdateFeaturedListingDto = {
      pageType: form.pageType,
      trainerProfileId: form.trainerProfileId.trim() || null,
      servicePackageId: form.servicePackageId.trim() || null,
      sortOrder: Number(form.sortOrder),
      startDate: form.startDate ? new Date(form.startDate).toISOString() : null,
      endDate: form.endDate ? new Date(form.endDate).toISOString() : null,
      isActive: form.isActive,
      adminNote: form.adminNote.trim() || null,
    };

    const request = editingId
      ? apiFetch<FeaturedListingDto>(ApiPaths.FeaturedListing.updateAsync(editingId), {
          method: "PUT",
          body: JSON.stringify(body),
        })
      : apiFetch<FeaturedListingDto>(ApiPaths.FeaturedListing.createAsync(), {
          method: "POST",
          body: JSON.stringify(body),
        });

    request
      .then(() => {
        resetForm();
        load();
      })
      .catch((e) => setSubmitError(e instanceof Error ? e.message : "Kayıt başarısız"))
      .finally(() => setSubmitting(false));
  };

  const handleDelete = (id: string) => {
    if (!confirm("Bu kaydı silmek istediğinizden emin misiniz?")) return;
    setDeletingId(id);
    apiFetch<void>(ApiPaths.FeaturedListing.deleteAsync(id), { method: "DELETE" })
      .then(() => {
        if (editingId === id) resetForm();
        load();
      })
      .catch((e) => setSubmitError(e instanceof Error ? e.message : "Silme işlemi başarısız"))
      .finally(() => setDeletingId(null));
  };

  const items = data?.items ?? [];

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-slate-800">Öne Çıkanlar</h1>

      <section className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
        <h2 className="mb-3 text-sm font-semibold text-slate-700">
          {editingId ? "Öne çıkan kaydını düzenle" : "Yeni öne çıkan kaydı"}
        </h2>
        {submitError && (
          <div className="mb-3 rounded-lg border border-amber-200 bg-amber-50 p-3 text-sm text-amber-800">
            {submitError}
          </div>
        )}
        <div className="grid gap-3 md:grid-cols-2">
          <label className="text-sm">
            <span className="mb-1 block text-slate-600">Sayfa tipi</span>
            <select
              className="w-full rounded-lg border border-slate-300 px-3 py-2"
              value={form.pageType}
              onChange={(e) => setForm((prev) => ({ ...prev, pageType: Number(e.target.value) }))}
            >
              <option value={0}>Ana Sayfa</option>
              <option value={1}>Kategori</option>
              <option value={2}>Arama</option>
            </select>
          </label>
          <label className="text-sm">
            <span className="mb-1 block text-slate-600">Sıra</span>
            <input
              type="number"
              min={0}
              className="w-full rounded-lg border border-slate-300 px-3 py-2"
              value={form.sortOrder}
              onChange={(e) => setForm((prev) => ({ ...prev, sortOrder: Number(e.target.value) }))}
            />
          </label>
          <label className="text-sm">
            <span className="mb-1 block text-slate-600">Eğitmen Profil ID (opsiyonel)</span>
            <input
              className="w-full rounded-lg border border-slate-300 px-3 py-2"
              value={form.trainerProfileId}
              onChange={(e) => setForm((prev) => ({ ...prev, trainerProfileId: e.target.value }))}
            />
          </label>
          <label className="text-sm">
            <span className="mb-1 block text-slate-600">Paket ID (opsiyonel)</span>
            <input
              className="w-full rounded-lg border border-slate-300 px-3 py-2"
              value={form.servicePackageId}
              onChange={(e) => setForm((prev) => ({ ...prev, servicePackageId: e.target.value }))}
            />
          </label>
          <label className="text-sm">
            <span className="mb-1 block text-slate-600">Başlangıç tarihi (opsiyonel)</span>
            <input
              type="datetime-local"
              className="w-full rounded-lg border border-slate-300 px-3 py-2"
              value={form.startDate}
              onChange={(e) => setForm((prev) => ({ ...prev, startDate: e.target.value }))}
            />
          </label>
          <label className="text-sm">
            <span className="mb-1 block text-slate-600">Bitiş tarihi (opsiyonel)</span>
            <input
              type="datetime-local"
              className="w-full rounded-lg border border-slate-300 px-3 py-2"
              value={form.endDate}
              onChange={(e) => setForm((prev) => ({ ...prev, endDate: e.target.value }))}
            />
          </label>
          <label className="text-sm md:col-span-2">
            <span className="mb-1 block text-slate-600">Admin notu</span>
            <textarea
              rows={2}
              className="w-full rounded-lg border border-slate-300 px-3 py-2"
              value={form.adminNote}
              onChange={(e) => setForm((prev) => ({ ...prev, adminNote: e.target.value }))}
            />
          </label>
          <label className="flex items-center gap-2 text-sm text-slate-700">
            <input
              type="checkbox"
              checked={form.isActive}
              onChange={(e) => setForm((prev) => ({ ...prev, isActive: e.target.checked }))}
            />
            Aktif
          </label>
        </div>
        <div className="mt-4 flex gap-2">
          <button
            type="button"
            onClick={submit}
            disabled={submitting}
            className="rounded-lg bg-fitliyo-green px-4 py-2 text-sm font-medium text-white hover:bg-fitliyo-green/90 disabled:opacity-50"
          >
            {submitting ? "Kaydediliyor..." : editingId ? "Güncelle" : "Ekle"}
          </button>
          {editingId && (
            <button
              type="button"
              onClick={resetForm}
              className="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50"
            >
              Düzenlemeyi iptal et
            </button>
          )}
        </div>
      </section>

      {loading && !data ? (
        <div className="rounded-xl border border-slate-200 bg-slate-50 p-8 text-center text-slate-600">
          Öne çıkanlar yükleniyor...
        </div>
      ) : error ? (
        <div className="rounded-xl border border-amber-200 bg-amber-50 p-4 text-amber-800">{error}</div>
      ) : items.length === 0 ? (
        <div className="rounded-xl border border-slate-200 bg-slate-50 p-8 text-center text-slate-600">
          Kayıt bulunamadı.
        </div>
      ) : (
        <div className="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-sm">
          <ul className="divide-y divide-slate-100">
            {items.map((item) => (
              <li key={item.id} className="flex flex-wrap items-start justify-between gap-4 p-4">
                <div className="min-w-0 space-y-1">
                  <p className="text-sm font-semibold text-slate-800">
                    {PAGE_TYPE_LABELS[item.pageType] ?? `Tip ${item.pageType}`} · Sıra {item.sortOrder}
                  </p>
                  <p className="text-xs text-slate-500">
                    Eğitmen: {item.trainerProfileId ?? "—"} · Paket: {item.servicePackageId ?? "—"}
                  </p>
                  <p className="text-xs text-slate-500">
                    Başlangıç: {item.startDate ?? "—"} · Bitiş: {item.endDate ?? "—"}
                  </p>
                  <p className="text-xs text-slate-500">Durum: {item.isActive ? "Aktif" : "Pasif"}</p>
                  {item.adminNote && <p className="text-sm text-slate-700">{item.adminNote}</p>}
                </div>
                <div className="flex gap-2">
                  <button
                    type="button"
                    onClick={() => startEdit(item)}
                    className="rounded-lg border border-slate-300 px-3 py-1.5 text-sm font-medium text-slate-700 hover:bg-slate-50"
                  >
                    Düzenle
                  </button>
                  <button
                    type="button"
                    onClick={() => handleDelete(item.id)}
                    disabled={deletingId === item.id}
                    className="rounded-lg border border-red-300 px-3 py-1.5 text-sm font-medium text-red-700 hover:bg-red-50 disabled:opacity-50"
                  >
                    {deletingId === item.id ? "Siliniyor..." : "Sil"}
                  </button>
                </div>
              </li>
            ))}
          </ul>
        </div>
      )}
    </div>
  );
}
