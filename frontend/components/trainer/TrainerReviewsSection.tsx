"use client";

import { useCallback, useEffect, useState } from "react";
import { apiFetch, buildQuery } from "@/lib/api";
import { ApiPaths } from "@/lib/api-paths";
import type { PagedResultDto, ReviewDto } from "@/lib/types";

const PAGE = 5;

export function TrainerReviewsSection({ trainerProfileId }: { trainerProfileId: string }) {
  const [page, setPage] = useState(0);
  const [data, setData] = useState<PagedResultDto<ReviewDto> | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const q = buildQuery({
        trainerProfileId,
        skipCount: page * PAGE,
        maxResultCount: PAGE,
        sorting: "creationTime desc",
      });
      const res = await apiFetch<PagedResultDto<ReviewDto>>(ApiPaths.Review.byTrainer(q));
      setData(res);
    } catch (e: unknown) {
      setError(e instanceof Error ? e.message : "Yorumlar yüklenemedi.");
      setData(null);
    } finally {
      setLoading(false);
    }
  }, [trainerProfileId, page]);

  useEffect(() => {
    void load();
  }, [load]);

  const maxPage = data ? Math.max(0, Math.ceil(data.totalCount / PAGE) - 1) : 0;

  if (loading && !data) {
    return <p className="text-sm text-slate-500">Yorumlar yükleniyor…</p>;
  }
  if (error) {
    return <p className="text-sm text-red-700">{error}</p>;
  }

  const items = data?.items ?? [];
  if (items.length === 0) {
    return <p className="text-sm text-slate-500">Henüz yorum yok.</p>;
  }

  return (
    <div className="space-y-4">
      <ul className="space-y-3">
        {items.map((r) => (
          <li key={r.id} className="rounded-xl border border-slate-200 bg-white/80 p-4">
            <div className="flex items-center justify-between gap-2">
              <span className="font-medium text-slate-800">
                {r.studentFullName ?? "Öğrenci"}
              </span>
              <span className="text-amber-600 text-sm" title={`${r.rating}/5`}>
                {"★".repeat(r.rating)}
                <span className="text-slate-300">{"★".repeat(5 - r.rating)}</span>
              </span>
            </div>
            {r.comment && <p className="mt-2 text-sm text-slate-600 whitespace-pre-wrap">{r.comment}</p>}
            {r.trainerReply && (
              <div className="mt-2 rounded-lg bg-slate-50 p-2 text-sm text-slate-700">
                <span className="font-medium text-slate-800">Eğitmen yanıtı: </span>
                {r.trainerReply}
              </div>
            )}
            <p className="mt-1 text-xs text-slate-400">
              {new Date(r.creationTime).toLocaleString("tr-TR")}
            </p>
          </li>
        ))}
      </ul>
      {(data?.totalCount ?? 0) > PAGE && (
        <div className="flex items-center justify-center gap-3">
          <button
            type="button"
            disabled={page <= 0}
            onClick={() => setPage((p) => Math.max(0, p - 1))}
            className="inline-flex items-center rounded-lg border border-slate-200 px-2 py-1 disabled:opacity-40"
            aria-label="Önceki"
          >
            ‹
          </button>
          <span className="text-sm text-slate-600">
            {page + 1} / {Math.max(1, maxPage + 1)}
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
