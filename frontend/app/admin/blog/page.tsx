"use client";

import { useEffect, useState } from "react";
import { apiFetch, buildQuery, DEFAULT_LIST_PARAMS } from "@/lib/api";
import { ApiPaths } from "@/lib/api-paths";
import type {
  BlogPostDto,
  CreateUpdateBlogPostDto,
  GetBlogPostListDto,
  PagedResultDto,
} from "@/lib/types";

const BLOG_STATUS_LABELS: Record<number, string> = {
  0: "Taslak",
  1: "Yayında",
  2: "Arşiv",
};

type BlogFormState = {
  title: string;
  slug: string;
  summary: string;
  body: string;
  featuredImageUrl: string;
};

const defaultForm: BlogFormState = {
  title: "",
  slug: "",
  summary: "",
  body: "",
  featuredImageUrl: "",
};

function slugify(input: string): string {
  return input
    .toLocaleLowerCase("tr-TR")
    .replace(/ı/g, "i")
    .replace(/ğ/g, "g")
    .replace(/ü/g, "u")
    .replace(/ş/g, "s")
    .replace(/ö/g, "o")
    .replace(/ç/g, "c")
    .replace(/[^a-z0-9]+/g, "-")
    .replace(/^-+|-+$/g, "");
}

export default function AdminBlogPage() {
  const [data, setData] = useState<PagedResultDto<BlogPostDto> | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [form, setForm] = useState<BlogFormState>(defaultForm);
  const [submitting, setSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [publishingId, setPublishingId] = useState<string | null>(null);
  const [deletingId, setDeletingId] = useState<string | null>(null);

  const load = () => {
    setLoading(true);
    setError(null);
    const params: GetBlogPostListDto = { ...DEFAULT_LIST_PARAMS };
    const query = buildQuery(params as Record<string, string | number | boolean | undefined | null>);
    apiFetch<PagedResultDto<BlogPostDto>>(ApiPaths.BlogPost.getListAsync(query))
      .then(setData)
      .catch((e) => setError(e instanceof Error ? e.message : "Blog kayıtları yüklenemedi"))
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

  const startEdit = (post: BlogPostDto) => {
    setEditingId(post.id);
    setSubmitError(null);
    setForm({
      title: post.title,
      slug: post.slug,
      summary: post.summary ?? "",
      body: post.body,
      featuredImageUrl: post.featuredImageUrl ?? "",
    });
  };

  const submit = () => {
    if (!form.title.trim() || !form.body.trim()) {
      setSubmitError("Başlık ve içerik alanları zorunludur.");
      return;
    }
    if (!form.slug.trim()) {
      setSubmitError("Slug zorunludur.");
      return;
    }

    setSubmitting(true);
    setSubmitError(null);
    const body: CreateUpdateBlogPostDto = {
      title: form.title.trim(),
      slug: form.slug.trim(),
      summary: form.summary.trim() || null,
      body: form.body.trim(),
      featuredImageUrl: form.featuredImageUrl.trim() || null,
    };

    const request = editingId
      ? apiFetch<BlogPostDto>(ApiPaths.BlogPost.updateAsync(editingId), {
          method: "PUT",
          body: JSON.stringify(body),
        })
      : apiFetch<BlogPostDto>(ApiPaths.BlogPost.createAsync(), {
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

  const handlePublish = (id: string) => {
    setPublishingId(id);
    apiFetch<BlogPostDto>(ApiPaths.BlogPost.publishAsync(id), { method: "POST" })
      .then(() => load())
      .catch((e) => setSubmitError(e instanceof Error ? e.message : "Yayınlama başarısız"))
      .finally(() => setPublishingId(null));
  };

  const handleDelete = (id: string) => {
    if (!confirm("Blog yazısını silmek istediğinizden emin misiniz?")) return;
    setDeletingId(id);
    apiFetch<void>(ApiPaths.BlogPost.deleteAsync(id), { method: "DELETE" })
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
      <h1 className="text-2xl font-bold text-slate-800">Blog Yönetimi</h1>

      <section className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
        <h2 className="mb-3 text-sm font-semibold text-slate-700">
          {editingId ? "Blog yazısını düzenle" : "Yeni blog yazısı"}
        </h2>
        {submitError && (
          <div className="mb-3 rounded-lg border border-amber-200 bg-amber-50 p-3 text-sm text-amber-800">
            {submitError}
          </div>
        )}
        <div className="grid gap-3">
          <label className="text-sm">
            <span className="mb-1 block text-slate-600">Başlık</span>
            <input
              className="w-full rounded-lg border border-slate-300 px-3 py-2"
              value={form.title}
              onChange={(e) => {
                const title = e.target.value;
                setForm((prev) => ({
                  ...prev,
                  title,
                  slug: prev.slug ? prev.slug : slugify(title),
                }));
              }}
            />
          </label>
          <label className="text-sm">
            <span className="mb-1 block text-slate-600">Slug</span>
            <input
              className="w-full rounded-lg border border-slate-300 px-3 py-2"
              value={form.slug}
              onChange={(e) => setForm((prev) => ({ ...prev, slug: slugify(e.target.value) }))}
            />
          </label>
          <label className="text-sm">
            <span className="mb-1 block text-slate-600">Özet</span>
            <textarea
              rows={2}
              className="w-full rounded-lg border border-slate-300 px-3 py-2"
              value={form.summary}
              onChange={(e) => setForm((prev) => ({ ...prev, summary: e.target.value }))}
            />
          </label>
          <label className="text-sm">
            <span className="mb-1 block text-slate-600">İçerik</span>
            <textarea
              rows={6}
              className="w-full rounded-lg border border-slate-300 px-3 py-2"
              value={form.body}
              onChange={(e) => setForm((prev) => ({ ...prev, body: e.target.value }))}
            />
          </label>
          <label className="text-sm">
            <span className="mb-1 block text-slate-600">Kapak görsel URL (opsiyonel)</span>
            <input
              className="w-full rounded-lg border border-slate-300 px-3 py-2"
              value={form.featuredImageUrl}
              onChange={(e) => setForm((prev) => ({ ...prev, featuredImageUrl: e.target.value }))}
            />
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
          Blog kayıtları yükleniyor...
        </div>
      ) : error ? (
        <div className="rounded-xl border border-amber-200 bg-amber-50 p-4 text-amber-800">{error}</div>
      ) : items.length === 0 ? (
        <div className="rounded-xl border border-slate-200 bg-slate-50 p-8 text-center text-slate-600">
          Henüz blog yazısı yok.
        </div>
      ) : (
        <div className="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-sm">
          <ul className="divide-y divide-slate-100">
            {items.map((item) => (
              <li key={item.id} className="flex flex-wrap items-start justify-between gap-4 p-4">
                <div className="space-y-1">
                  <p className="font-semibold text-slate-800">{item.title}</p>
                  <p className="text-xs text-slate-500">/{item.slug}</p>
                  <p className="text-xs text-slate-500">
                    Durum: {BLOG_STATUS_LABELS[item.status] ?? `Durum ${item.status}`} · Oluşturulma:{" "}
                    {new Date(item.creationTime).toLocaleString("tr-TR")}
                  </p>
                  {item.summary && <p className="text-sm text-slate-700">{item.summary}</p>}
                </div>
                <div className="flex flex-wrap gap-2">
                  <button
                    type="button"
                    onClick={() => startEdit(item)}
                    className="rounded-lg border border-slate-300 px-3 py-1.5 text-sm font-medium text-slate-700 hover:bg-slate-50"
                  >
                    Düzenle
                  </button>
                  {item.status !== 1 && (
                    <button
                      type="button"
                      onClick={() => handlePublish(item.id)}
                      disabled={publishingId === item.id}
                      className="rounded-lg bg-fitliyo-green px-3 py-1.5 text-sm font-medium text-white hover:bg-fitliyo-green/90 disabled:opacity-50"
                    >
                      {publishingId === item.id ? "Yayınlanıyor..." : "Yayınla"}
                    </button>
                  )}
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
