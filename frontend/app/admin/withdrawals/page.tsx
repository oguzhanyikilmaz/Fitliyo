"use client";

import { useEffect, useState } from "react";
import { apiFetch, buildQuery } from "@/lib/api";
import { ApiPaths } from "@/lib/api-paths";
import type {
  PagedResultDto,
  WithdrawalRequestDto,
  GetWithdrawalRequestListDto,
} from "@/lib/types";

const STATUS_LABELS: Record<number, string> = {
  0: "Beklemede",
  1: "Onaylandı",
  2: "Reddedildi",
  3: "İşlendi",
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

export default function AdminWithdrawalsPage() {
  const [data, setData] = useState<PagedResultDto<WithdrawalRequestDto> | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [processingId, setProcessingId] = useState<string | null>(null);
  const [actionError, setActionError] = useState<string | null>(null);

  const load = () => {
    setLoading(true);
    const params: GetWithdrawalRequestListDto = {
      skipCount: 0,
      maxResultCount: 50,
      sorting: "creationTime desc",
    };
    const query = buildQuery(params as Record<string, string | number | boolean | undefined | null>);
    apiFetch<PagedResultDto<WithdrawalRequestDto>>(
      ApiPaths.WithdrawalRequest.getListAsync(query)
    )
      .then(setData)
      .catch((e) => setError(e instanceof Error ? e.message : "Talepler yüklenemedi"))
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    load();
  }, []);

  const handleApprove = (id: string) => {
    setActionError(null);
    setProcessingId(id);
    apiFetch<WithdrawalRequestDto>(ApiPaths.WithdrawalRequest.approveAsync(id), {
      method: "POST",
      body: JSON.stringify({}),
    })
      .then(() => load())
      .catch((e) => setActionError(e instanceof Error ? e.message : "Onaylanamadı"))
      .finally(() => setProcessingId(null));
  };

  const handleReject = (id: string) => {
    setActionError(null);
    setProcessingId(id);
    apiFetch<WithdrawalRequestDto>(ApiPaths.WithdrawalRequest.rejectAsync(id), {
      method: "POST",
      body: JSON.stringify({}),
    })
      .then(() => load())
      .catch((e) => setActionError(e instanceof Error ? e.message : "Reddedilemedi"))
      .finally(() => setProcessingId(null));
  };

  const handleMarkProcessed = (id: string) => {
    setActionError(null);
    setProcessingId(id);
    apiFetch<WithdrawalRequestDto>(ApiPaths.WithdrawalRequest.markProcessedAsync(id), {
      method: "POST",
    })
      .then(() => load())
      .catch((e) => setActionError(e instanceof Error ? e.message : "Güncellenemedi"))
      .finally(() => setProcessingId(null));
  };

  if (loading && !data) {
    return (
      <div className="flex min-h-[40vh] items-center justify-center">
        <p className="text-slate-600">Para çekme talepleri yükleniyor...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="space-y-4">
        <h1 className="text-2xl font-bold text-slate-800">Para Çekme Talepleri</h1>
        <div className="rounded-xl border border-amber-200 bg-amber-50 p-4 text-amber-800">{error}</div>
      </div>
    );
  }

  const items = data?.items ?? [];
  const pending = items.filter((r) => r.status === 0);

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-slate-800">Para Çekme Talepleri</h1>

      {actionError && (
        <div className="rounded-lg border border-amber-200 bg-amber-50 p-3 text-sm text-amber-800">
          {actionError}
        </div>
      )}

      {pending.length > 0 && (
        <p className="text-sm text-slate-600">
          {pending.length} adet beklemede talep var.
        </p>
      )}

      {items.length === 0 ? (
        <div className="rounded-xl border border-slate-200 bg-slate-50 p-8 text-center text-slate-600">
          Henüz para çekme talebi yok.
        </div>
      ) : (
        <div className="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-sm">
          <ul className="divide-y divide-slate-100">
            {items.map((r) => (
              <li key={r.id} className="flex flex-wrap items-center justify-between gap-4 p-4">
                <div>
                  <p className="font-medium text-slate-800">
                    ₺{r.amount.toLocaleString("tr-TR", { minimumFractionDigits: 2 })}
                  </p>
                  <p className="text-sm text-slate-600">{r.accountHolderName}</p>
                  <p className="text-xs text-slate-400">{r.iban}</p>
                  <p className="mt-1 text-xs text-slate-500">{formatDate(r.creationTime)}</p>
                </div>
                <div className="flex flex-wrap items-center gap-2">
                  <span className="rounded-full bg-slate-100 px-2 py-0.5 text-xs text-slate-600">
                    {STATUS_LABELS[r.status] ?? r.status}
                  </span>
                  {r.status === 0 && (
                    <>
                      <button
                        type="button"
                        onClick={() => handleApprove(r.id)}
                        disabled={processingId === r.id}
                        className="rounded-lg bg-fitliyo-green px-3 py-1.5 text-sm font-medium text-white hover:bg-fitliyo-green/90 disabled:opacity-50"
                      >
                        {processingId === r.id ? "..." : "Onayla"}
                      </button>
                      <button
                        type="button"
                        onClick={() => handleReject(r.id)}
                        disabled={processingId === r.id}
                        className="rounded-lg border border-red-300 bg-white px-3 py-1.5 text-sm font-medium text-red-700 hover:bg-red-50 disabled:opacity-50"
                      >
                        Reddet
                      </button>
                    </>
                  )}
                  {r.status === 1 && (
                    <button
                      type="button"
                      onClick={() => handleMarkProcessed(r.id)}
                      disabled={processingId === r.id}
                      className="rounded-lg border border-slate-300 bg-white px-3 py-1.5 text-sm font-medium text-slate-700 hover:bg-slate-50 disabled:opacity-50"
                    >
                      {processingId === r.id ? "..." : "İşlendi"}
                    </button>
                  )}
                </div>
              </li>
            ))}
          </ul>
        </div>
      )}
    </div>
  );
}
