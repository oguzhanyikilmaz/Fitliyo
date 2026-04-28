"use client";

import Link from "next/link";
import { useCallback, useEffect, useState } from "react";
import { apiFetch, buildQuery } from "@/lib/api";
import { ApiPaths } from "@/lib/api-paths";
import type { CategoryDto, ListResultDto, PagedResultDto, TrainerProfileDto } from "@/lib/types";

const PAGE_SIZE = 6;

const SORT_OPTIONS: { value: string; label: string }[] = [
  { value: "TotalReviewCount desc", label: "En çok değerlendirilen" },
  { value: "AverageRating desc", label: "En yüksek puan" },
  { value: "Slug", label: "İsme göre (A–Z)" },
];

const MIN_RATING_OPTIONS = [
  { value: "", label: "Tüm puanlar" },
  { value: "3", label: "Bu sayfada ≥ 3 yıldız" },
  { value: "4", label: "Bu sayfada ≥ 4 yıldız" },
  { value: "4.5", label: "Bu sayfada ≥ 4,5 yıldız" },
];

export interface TrainerListProps {
  title?: string;
  subtitle?: string;
  backHref?: string;
  backLabel?: string;
}

/**
 * Eğitmen listesi — arama, şehir, online/yüz yüze/sertifika, sıralama, sayfalama.
 * Min. puan: sunucuda filtre yok; yalnızca gelen sayfa sonuçları üzerinde uygulanır.
 * Kategori: DTO’da alan var; backend join gelene kadar seçim API’ye gönderilir (yok sayılabilir).
 */
export function TrainerList({ title, subtitle, backHref, backLabel }: TrainerListProps) {
  const [items, setItems] = useState<TrainerProfileDto[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(0);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [categories, setCategories] = useState<CategoryDto[]>([]);

  const [filter, setFilter] = useState("");
  const [debouncedFilter, setDebouncedFilter] = useState("");
  const [categoryId, setCategoryId] = useState("");
  const [city, setCity] = useState("");
  const [isOnline, setIsOnline] = useState<string>("");
  const [isOnSite, setIsOnSite] = useState<string>("");
  const [isVerified, setIsVerified] = useState<string>("");
  const [sorting, setSorting] = useState("TotalReviewCount desc");
  const [minRating, setMinRating] = useState("");

  useEffect(() => {
    const t = setTimeout(() => setDebouncedFilter(filter.trim()), 400);
    return () => clearTimeout(t);
  }, [filter]);

  useEffect(() => {
    let cancelled = false;
    apiFetch<ListResultDto<CategoryDto>>(ApiPaths.Category.getListAsync())
      .then((res) => {
        if (!cancelled) setCategories(res.items ?? []);
      })
      .catch(() => {
        if (!cancelled) setCategories([]);
      });
    return () => {
      cancelled = true;
    };
  }, []);

  const minRatingNum = minRating ? parseFloat(minRating) : 0;

  const fetchList = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const query = buildQuery({
        filter: debouncedFilter || undefined,
        categoryId: categoryId || undefined,
        city: city || undefined,
        isOnlineAvailable: isOnline || undefined,
        isOnSiteAvailable: isOnSite || undefined,
        isVerified: isVerified || undefined,
        skipCount: page * PAGE_SIZE,
        maxResultCount: PAGE_SIZE,
        sorting,
      });
      const res = await apiFetch<PagedResultDto<TrainerProfileDto>>(ApiPaths.TrainerProfile.getListAsync(query));
      const raw = res.items ?? [];
      const filtered =
        minRatingNum > 0 ? raw.filter((t) => Number(t.averageRating) >= minRatingNum) : raw;
      setItems(filtered);
      setTotalCount(res.totalCount);
    } catch (e: unknown) {
      setError(e instanceof Error ? e.message : "Eğitmen listesi yüklenemedi.");
    } finally {
      setLoading(false);
    }
  }, [page, debouncedFilter, categoryId, city, isOnline, isOnSite, isVerified, sorting, minRatingNum]);

  useEffect(() => {
    void fetchList();
  }, [fetchList]);

  useEffect(() => {
    setPage(0);
  }, [debouncedFilter, categoryId, city, isOnline, isOnSite, isVerified, sorting, minRating]);

  const maxPage = Math.max(0, Math.ceil(totalCount / PAGE_SIZE) - 1);

  useEffect(() => {
    if (page > maxPage && maxPage >= 0) setPage(maxPage);
  }, [page, maxPage]);

  const tri = (value: string, onChange: (v: string) => void) => (
    <select
      value={value}
      onChange={(e) => onChange(e.target.value)}
      className="rounded-lg border border-slate-200 bg-white px-2 py-1.5 text-sm text-slate-800"
    >
      <option value="">Hepsi</option>
      <option value="true">Evet</option>
      <option value="false">Hayır</option>
    </select>
  );

  return (
    <div className="space-y-4">
      {backHref && (
        <Link href={backHref} className="inline-block text-sm font-medium text-fitliyo-green hover:underline">
          {backLabel ?? "← Geri"}
        </Link>
      )}
      {title && <h1 className="text-2xl font-bold text-slate-800">{title}</h1>}
      {subtitle && <p className="text-slate-600">{subtitle}</p>}

      {categoryId && (
        <p className="text-xs text-amber-800 bg-amber-50 border border-amber-200 rounded-lg px-3 py-2">
          Kategori filtresi API tarafında tanımlandığında tam etkin olacaktır; şimdilik isteğe ekleniyor.
        </p>
      )}

      <div className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm space-y-3">
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3">
          <input
            type="search"
            value={filter}
            onChange={(e) => setFilter(e.target.value)}
            placeholder="İsim, şehir, bio içinde ara..."
            className="rounded-lg border border-slate-200 px-3 py-2 text-sm sm:col-span-2"
          />
          <label className="flex flex-col gap-1 text-xs text-slate-600">
            Kategori
            <select
              value={categoryId}
              onChange={(e) => setCategoryId(e.target.value)}
              className="rounded-lg border border-slate-200 bg-white px-2 py-2 text-sm text-slate-800"
            >
              <option value="">Tüm kategoriler</option>
              {categories.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.name}
                </option>
              ))}
            </select>
          </label>
          <label className="flex flex-col gap-1 text-xs text-slate-600">
            Şehir
            <input
              value={city}
              onChange={(e) => setCity(e.target.value)}
              placeholder="Örn. İstanbul"
              className="rounded-lg border border-slate-200 px-3 py-2 text-sm"
            />
          </label>
          <label className="flex flex-col gap-1 text-xs text-slate-600">
            Sıralama
            <select
              value={sorting}
              onChange={(e) => setSorting(e.target.value)}
              className="rounded-lg border border-slate-200 bg-white px-2 py-2 text-sm"
            >
              {SORT_OPTIONS.map((o) => (
                <option key={o.value} value={o.value}>
                  {o.label}
                </option>
              ))}
            </select>
          </label>
          <label className="flex flex-col gap-1 text-xs text-slate-600">
            Min. puan (sayfa içi)
            <select
              value={minRating}
              onChange={(e) => setMinRating(e.target.value)}
              className="rounded-lg border border-slate-200 bg-white px-2 py-2 text-sm"
            >
              {MIN_RATING_OPTIONS.map((o) => (
                <option key={o.value || "all"} value={o.value}>
                  {o.label}
                </option>
              ))}
            </select>
          </label>
        </div>
        <div className="flex flex-wrap gap-4 items-end text-sm">
          <span className="text-slate-600">Online:</span>
          {tri(isOnline, setIsOnline)}
          <span className="text-slate-600">Yüz yüze:</span>
          {tri(isOnSite, setIsOnSite)}
          <span className="text-slate-600">Sertifikalı:</span>
          {tri(isVerified, setIsVerified)}
        </div>
      </div>

      {minRatingNum > 0 && (
        <p className="text-xs text-slate-500">
          Minimum puan filtresi yalnızca bu sayfadaki {PAGE_SIZE} kayıt üzerinde uygulanır; tüm liste için API
          genişlemesi gerekir.
        </p>
      )}

      {error && (
        <div className="rounded-xl border border-red-200 bg-red-50 p-3 text-sm text-red-800">{error}</div>
      )}
      {loading && <p className="text-sm text-slate-500">Yükleniyor…</p>}

      {!loading && !error && items.length === 0 && (
        <p className="text-slate-500">Kriterlere uygun eğitmen bulunamadı.</p>
      )}

      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {items.map((t) => (
          <Link
            key={t.id}
            href={`/trainers/${t.slug || t.id}`}
            className="block rounded-xl border border-slate-200 bg-white p-4 shadow-sm transition hover:shadow-md hover:border-fitliyo-green/30"
          >
            <div className="flex items-start justify-between gap-2 mb-2">
              <h3 className="font-semibold text-slate-800 line-clamp-2">{t.trainerFullName ?? t.slug}</h3>
              {t.isVerified && (
                <span className="text-fitliyo-green text-sm shrink-0" title="Doğrulanmış" aria-hidden>
                  ✓
                </span>
              )}
            </div>
            {t.bio && <p className="text-sm text-slate-600 line-clamp-2 mb-2">{t.bio}</p>}
            <div className="flex items-center justify-between text-xs text-slate-500">
              <span>
                {Number(t.averageRating) > 0
                  ? `★ ${Number(t.averageRating).toFixed(1)} (${t.totalReviewCount})`
                  : "—"}
              </span>
              {t.city && <span className="truncate max-w-[50%]">📍 {t.city}</span>}
            </div>
            {t.specialtyTags && (
              <p className="mt-2 text-[11px] text-fitliyo-green/90 line-clamp-1">{t.specialtyTags}</p>
            )}
          </Link>
        ))}
      </div>

      {totalCount > PAGE_SIZE && (
        <div className="flex items-center justify-center gap-3 pt-2">
          <button
            type="button"
            disabled={page <= 0}
            onClick={() => setPage((p) => Math.max(0, p - 1))}
            className="inline-flex items-center rounded-lg border border-slate-200 px-2 py-1 disabled:opacity-40"
            aria-label="Önceki"
          >
            ‹
          </button>
          <span className="text-sm text-slate-600 min-w-[10rem] text-center">
            Sayfa {page + 1} / {Math.max(1, maxPage + 1)} · {totalCount} kayıt
          </span>
          <button
            type="button"
            disabled={page >= maxPage}
            onClick={() => setPage((p) => p + 1)}
            className="inline-flex items-center rounded-lg border border-slate-200 px-2 py-1 disabled:opacity-40"
            aria-label="Sonraki"
          >
            ›
          </button>
        </div>
      )}
    </div>
  );
}
