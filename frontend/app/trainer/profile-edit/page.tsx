"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { apiFetch } from "@/lib/api";
import { ApiPaths } from "@/lib/api-paths";
import type { TrainerProfileDto, CreateUpdateTrainerProfileDto } from "@/lib/types";

const TRAINER_TYPE_OPTIONS = [
  { value: 1, label: "Kişisel antrenör" },
  { value: 2, label: "Diyetisyen" },
  { value: 3, label: "Basketbol koçu" },
  { value: 4, label: "Futbol koçu" },
  { value: 5, label: "Tenis koçu" },
  { value: 6, label: "Yüzme koçu" },
  { value: 7, label: "Yoga eğitmeni" },
  { value: 99, label: "Diğer" },
];

export default function TrainerProfileEditPage() {
  const [profile, setProfile] = useState<TrainerProfileDto | null>(null);
  const [form, setForm] = useState<CreateUpdateTrainerProfileDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    apiFetch<TrainerProfileDto>(ApiPaths.TrainerProfile.getMyProfileAsync())
      .then((p) => {
        setProfile(p);
        setForm({
          slug: p.slug,
          bio: p.bio ?? undefined,
          experienceYears: p.experienceYears,
          trainerType: p.trainerType,
          specialtyTags: p.specialtyTags ?? undefined,
          city: p.city ?? undefined,
          district: p.district ?? undefined,
          isOnlineAvailable: p.isOnlineAvailable,
          isOnSiteAvailable: p.isOnSiteAvailable,
          instagramUrl: p.instagramUrl ?? undefined,
          youtubeUrl: p.youtubeUrl ?? undefined,
          websiteUrl: p.websiteUrl ?? undefined,
        });
      })
      .catch((e) => setError(e instanceof Error ? e.message : "Profil yüklenemedi"))
      .finally(() => setLoading(false));
  }, []);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!form || !profile) return;
    setError(null);
    setSubmitting(true);
    apiFetch<TrainerProfileDto>(ApiPaths.TrainerProfile.updateAsync(profile.id), {
      method: "PUT",
      body: JSON.stringify(form),
    })
      .then(setProfile)
      .catch((e) => setError(e instanceof Error ? e.message : "Güncellenemedi"))
      .finally(() => setSubmitting(false));
  };

  if (loading) {
    return (
      <div className="flex min-h-[40vh] items-center justify-center">
        <p className="text-slate-600">Profil yükleniyor...</p>
      </div>
    );
  }

  if (error && !profile) {
    return (
      <div className="space-y-4">
        <h1 className="text-2xl font-bold text-slate-800">Eğitmen profili</h1>
        <div className="rounded-xl border border-amber-200 bg-amber-50 p-4 text-amber-800">{error}</div>
        <Link href="/trainer" className="text-sm font-medium text-fitliyo-green hover:underline">
          ← Panele dön
        </Link>
      </div>
    );
  }

  if (!form) return null;

  return (
    <div className="mx-auto max-w-2xl space-y-6">
      <Link href="/trainer" className="inline-block text-sm font-medium text-fitliyo-green hover:underline">
        ← Panele dön
      </Link>
      <h1 className="text-2xl font-bold text-slate-800">Eğitmen profili düzenle</h1>

      {error && (
        <div className="rounded-lg border border-amber-200 bg-amber-50 p-3 text-sm text-amber-800">
          {error}
        </div>
      )}

      <form onSubmit={handleSubmit} className="space-y-4 rounded-xl border border-slate-200 bg-white p-6 shadow-sm">
        <div>
          <label className="block text-sm font-medium text-slate-700">Profil URL (slug) *</label>
          <input
            type="text"
            value={form.slug}
            onChange={(e) => setForm((f) => (f ? { ...f, slug: e.target.value } : f))}
            required
            className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-800"
            placeholder="ahmet-yilmaz"
          />
        </div>
        <div>
          <label className="block text-sm font-medium text-slate-700">Kısa bio</label>
          <textarea
            value={form.bio ?? ""}
            onChange={(e) => setForm((f) => (f ? { ...f, bio: e.target.value || undefined } : f))}
            rows={3}
            className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-800"
          />
        </div>
        <div>
          <label className="block text-sm font-medium text-slate-700">Deneyim (yıl)</label>
          <input
            type="number"
            min={0}
            max={50}
            value={form.experienceYears}
            onChange={(e) => setForm((f) => (f ? { ...f, experienceYears: Number(e.target.value) || 0 } : f))}
            className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-800"
          />
        </div>
        <div>
          <label className="block text-sm font-medium text-slate-700">Eğitmen tipi</label>
          <select
            value={form.trainerType}
            onChange={(e) => setForm((f) => (f ? { ...f, trainerType: Number(e.target.value) } : f))}
            className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-800"
          >
            {TRAINER_TYPE_OPTIONS.map((o) => (
              <option key={o.value} value={o.value}>{o.label}</option>
            ))}
          </select>
        </div>
        <div>
          <label className="block text-sm font-medium text-slate-700">Uzmanlık etiketleri</label>
          <input
            type="text"
            value={form.specialtyTags ?? ""}
            onChange={(e) => setForm((f) => (f ? { ...f, specialtyTags: e.target.value || undefined } : f))}
            className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-800"
            placeholder="Kilo verme, kas, esneklik"
          />
        </div>
        <div className="grid gap-4 sm:grid-cols-2">
          <div>
            <label className="block text-sm font-medium text-slate-700">Şehir</label>
            <input
              type="text"
              value={form.city ?? ""}
              onChange={(e) => setForm((f) => (f ? { ...f, city: e.target.value || undefined } : f))}
              className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-800"
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-slate-700">İlçe</label>
            <input
              type="text"
              value={form.district ?? ""}
              onChange={(e) => setForm((f) => (f ? { ...f, district: e.target.value || undefined } : f))}
              className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-800"
            />
          </div>
        </div>
        <div className="flex gap-4">
          <label className="flex items-center gap-2">
            <input
              type="checkbox"
              checked={form.isOnlineAvailable}
              onChange={(e) => setForm((f) => (f ? { ...f, isOnlineAvailable: e.target.checked } : f))}
              className="rounded border-slate-300"
            />
            <span className="text-sm text-slate-700">Online hizmet veriyorum</span>
          </label>
          <label className="flex items-center gap-2">
            <input
              type="checkbox"
              checked={form.isOnSiteAvailable}
              onChange={(e) => setForm((f) => (f ? { ...f, isOnSiteAvailable: e.target.checked } : f))}
              className="rounded border-slate-300"
            />
            <span className="text-sm text-slate-700">Yüz yüze hizmet veriyorum</span>
          </label>
        </div>
        <div>
          <label className="block text-sm font-medium text-slate-700">Instagram URL</label>
          <input
            type="url"
            value={form.instagramUrl ?? ""}
            onChange={(e) => setForm((f) => (f ? { ...f, instagramUrl: e.target.value || undefined } : f))}
            className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-800"
          />
        </div>
        <div>
          <label className="block text-sm font-medium text-slate-700">YouTube URL</label>
          <input
            type="url"
            value={form.youtubeUrl ?? ""}
            onChange={(e) => setForm((f) => (f ? { ...f, youtubeUrl: e.target.value || undefined } : f))}
            className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-800"
          />
        </div>
        <div>
          <label className="block text-sm font-medium text-slate-700">Web sitesi URL</label>
          <input
            type="url"
            value={form.websiteUrl ?? ""}
            onChange={(e) => setForm((f) => (f ? { ...f, websiteUrl: e.target.value || undefined } : f))}
            className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-800"
          />
        </div>
        <div className="flex gap-2">
          <button
            type="submit"
            disabled={submitting}
            className="rounded-lg bg-fitliyo-green px-4 py-2 text-sm font-medium text-white hover:bg-fitliyo-green/90 disabled:opacity-50"
          >
            {submitting ? "Kaydediliyor..." : "Kaydet"}
          </button>
          <Link
            href="/trainer"
            className="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50"
          >
            İptal
          </Link>
        </div>
      </form>
    </div>
  );
}
