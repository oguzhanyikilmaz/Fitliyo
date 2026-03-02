"use client";

import { useEffect, useState } from "react";
import { apiFetch, buildQuery } from "@/lib/api";
import { ApiPaths } from "@/lib/api-paths";
import type {
  PagedResultDto,
  SupportTicketDto,
  GetSupportTicketListDto,
  CreateSupportTicketDto,
} from "@/lib/types";

const CATEGORY_LABELS: Record<number, string> = {
  0: "Genel",
  1: "Ödeme",
  2: "Sipariş",
  3: "Teknik",
  4: "Hesap",
};

const STATUS_LABELS: Record<number, string> = {
  0: "Açık",
  1: "İnceleniyor",
  2: "Müşteri Bekleniyor",
  3: "Çözüldü",
  4: "Kapatıldı",
};

function formatDate(s: string) {
  try {
    return new Date(s).toLocaleString("tr-TR", {
      day: "2-digit",
      month: "short",
      year: "numeric",
      hour: "2-digit",
      minute: "2-digit",
    });
  } catch {
    return s;
  }
}

export default function EgitmenDestekPage() {
  const [data, setData] = useState<PagedResultDto<SupportTicketDto> | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [showForm, setShowForm] = useState(false);
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);
  const [form, setForm] = useState<CreateSupportTicketDto>({
    subject: "",
    message: "",
    category: 0,
  });

  const load = () => {
    setLoading(true);
    const params: GetSupportTicketListDto = {
      skipCount: 0,
      maxResultCount: 50,
      sorting: "creationTime desc",
    };
    const query = buildQuery(params as Record<string, string | number | boolean | undefined | null>);
    apiFetch<PagedResultDto<SupportTicketDto>>(ApiPaths.SupportTicket.getMyTicketsAsync(query))
      .then(setData)
      .catch((e) => setError(e instanceof Error ? e.message : "Talepler yüklenemedi"))
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    load();
  }, []);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitError(null);
    if (!form.subject.trim() || !form.message.trim()) {
      setSubmitError("Konu ve mesaj zorunludur.");
      return;
    }
    setSubmitting(true);
    apiFetch<SupportTicketDto>(ApiPaths.SupportTicket.createAsync(), {
      method: "POST",
      body: JSON.stringify(form),
    })
      .then(() => {
        setForm({ subject: "", message: "", category: 0 });
        setShowForm(false);
        load();
      })
      .catch((e) => setSubmitError(e instanceof Error ? e.message : "Talep oluşturulamadı"))
      .finally(() => setSubmitting(false));
  };

  if (loading && !data) {
    return (
      <div className="flex min-h-[40vh] items-center justify-center">
        <p className="text-slate-600">Talepleriniz yükleniyor...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="space-y-4">
        <h1 className="text-2xl font-bold text-slate-800">Destek Taleplerim</h1>
        <div className="rounded-xl border border-amber-200 bg-amber-50 p-4 text-amber-800">{error}</div>
      </div>
    );
  }

  const items = data?.items ?? [];

  return (
    <div className="space-y-6">
      <div className="flex flex-wrap items-center justify-between gap-4">
        <h1 className="text-2xl font-bold text-slate-800">Destek Taleplerim</h1>
        <button
          type="button"
          onClick={() => setShowForm(!showForm)}
          className="rounded-lg bg-fitliyo-green px-4 py-2 text-sm font-medium text-white shadow-sm hover:bg-fitliyo-green/90"
        >
          {showForm ? "İptal" : "Yeni talep"}
        </button>
      </div>

      {showForm && (
        <form
          onSubmit={handleSubmit}
          className="rounded-xl border border-slate-200 bg-white p-6 shadow-sm"
        >
          <h2 className="mb-4 text-lg font-semibold text-slate-800">Yeni destek talebi</h2>
          {submitError && (
            <div className="mb-4 rounded-lg border border-amber-200 bg-amber-50 p-3 text-sm text-amber-800">
              {submitError}
            </div>
          )}
          <div className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-slate-700">Kategori</label>
              <select
                value={form.category}
                onChange={(e) => setForm((f) => ({ ...f, category: Number(e.target.value) }))}
                className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-800"
              >
                {Object.entries(CATEGORY_LABELS).map(([value, label]) => (
                  <option key={value} value={value}>
                    {label}
                  </option>
                ))}
              </select>
            </div>
            <div>
              <label className="block text-sm font-medium text-slate-700">Konu</label>
              <input
                type="text"
                value={form.subject}
                onChange={(e) => setForm((f) => ({ ...f, subject: e.target.value }))}
                required
                maxLength={256}
                className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-800"
                placeholder="Kısa başlık"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-slate-700">Mesaj</label>
              <textarea
                value={form.message}
                onChange={(e) => setForm((f) => ({ ...f, message: e.target.value }))}
                required
                rows={4}
                className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-800"
                placeholder="Sorununuzu detaylı yazın"
              />
            </div>
            <div className="flex gap-2">
              <button
                type="submit"
                disabled={submitting}
                className="rounded-lg bg-fitliyo-green px-4 py-2 text-sm font-medium text-white hover:bg-fitliyo-green/90 disabled:opacity-50"
              >
                {submitting ? "Gönderiliyor..." : "Gönder"}
              </button>
              <button
                type="button"
                onClick={() => setShowForm(false)}
                className="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50"
              >
                İptal
              </button>
            </div>
          </div>
        </form>
      )}

      {items.length === 0 && !showForm ? (
        <div className="rounded-xl border border-slate-200 bg-slate-50 p-8 text-center text-slate-600">
          Henüz destek talebiniz yok. &quot;Yeni talep&quot; ile oluşturabilirsiniz.
        </div>
      ) : (
        <ul className="space-y-3">
          {items.map((t) => (
            <li key={t.id} className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
              <div className="flex flex-wrap justify-between gap-2">
                <p className="font-medium text-slate-800">{t.subject}</p>
                <span className="rounded-full bg-slate-100 px-2 py-0.5 text-xs text-slate-600">
                  {STATUS_LABELS[t.status] ?? t.status}
                </span>
              </div>
              <p className="mt-1 text-xs text-slate-500">
                {CATEGORY_LABELS[t.category] ?? t.category} · {formatDate(t.creationTime)}
              </p>
              <p className="mt-2 text-sm text-slate-600 line-clamp-2">{t.message}</p>
              {t.adminReply && (
                <div className="mt-3 rounded-lg border border-slate-100 bg-slate-50 p-3 text-sm text-slate-700">
                  <span className="font-medium">Yanıt: </span>
                  {t.adminReply}
                  {t.adminReplyDate && (
                    <p className="mt-1 text-xs text-slate-500">{formatDate(t.adminReplyDate)}</p>
                  )}
                </div>
              )}
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
