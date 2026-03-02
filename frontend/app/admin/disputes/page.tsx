"use client";

import { useEffect, useState } from "react";
import { apiFetch, buildQuery } from "@/lib/api";
import { ApiPaths } from "@/lib/api-paths";
import type {
  PagedResultDto,
  DisputeDto,
  GetDisputeListDto,
  ResolveDisputeDto,
} from "@/lib/types";

const TYPE_LABELS: Record<number, string> = {
  0: "İade",
  1: "Hizmet verilmedi",
  2: "İptal",
  3: "Diğer",
};

const STATUS_LABELS: Record<number, string> = {
  0: "Açık",
  1: "İnceleniyor",
  2: "Çözüldü",
  3: "Kapatıldı",
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

export default function AdminDisputesPage() {
  const [data, setData] = useState<PagedResultDto<DisputeDto> | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [resolvingId, setResolvingId] = useState<string | null>(null);
  const [selectedId, setSelectedId] = useState<string | null>(null);
  const [resolutionNote, setResolutionNote] = useState("");
  const [resolveError, setResolveError] = useState<string | null>(null);

  const load = () => {
    setLoading(true);
    const params: GetDisputeListDto = {
      skipCount: 0,
      maxResultCount: 50,
      sorting: "creationTime desc",
    };
    const query = buildQuery(params as Record<string, string | number | boolean | undefined | null>);
    apiFetch<PagedResultDto<DisputeDto>>(ApiPaths.Dispute.getListAsync(query))
      .then(setData)
      .catch((e) => setError(e instanceof Error ? e.message : "Uyuşmazlıklar yüklenemedi"))
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    load();
  }, []);

  const handleResolve = (id: string) => {
    if (!resolutionNote.trim()) return;
    setResolveError(null);
    setResolvingId(id);
    const body: ResolveDisputeDto = { resolutionNote };
    apiFetch<DisputeDto>(ApiPaths.Dispute.resolveAsync(id), {
      method: "POST",
      body: JSON.stringify(body),
    })
      .then(() => {
        setResolutionNote("");
        setSelectedId(null);
        setResolvingId(null);
        load();
      })
      .catch((e) => {
        setResolveError(e instanceof Error ? e.message : "Çözüm kaydedilemedi");
        setResolvingId(null);
      });
  };

  if (loading && !data) {
    return (
      <div className="flex min-h-[40vh] items-center justify-center">
        <p className="text-slate-600">Uyuşmazlıklar yükleniyor...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="space-y-4">
        <h1 className="text-2xl font-bold text-slate-800">Uyuşmazlıklar</h1>
        <div className="rounded-xl border border-amber-200 bg-amber-50 p-4 text-amber-800">{error}</div>
      </div>
    );
  }

  const items = data?.items ?? [];

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-slate-800">Uyuşmazlıklar</h1>

      {resolveError && (
        <div className="rounded-lg border border-amber-200 bg-amber-50 p-3 text-sm text-amber-800">
          {resolveError}
        </div>
      )}

      {items.length === 0 ? (
        <div className="rounded-xl border border-slate-200 bg-slate-50 p-8 text-center text-slate-600">
          Henüz uyuşmazlık yok.
        </div>
      ) : (
        <ul className="space-y-4">
          {items.map((d) => (
            <li key={d.id} className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
              <div className="flex flex-wrap justify-between gap-2">
                <p className="font-medium text-slate-800">
                  {TYPE_LABELS[d.disputeType] ?? d.disputeType} · Sipariş: {d.orderId.slice(0, 8)}…
                </p>
                <span className="rounded-full bg-slate-100 px-2 py-0.5 text-xs text-slate-600">
                  {STATUS_LABELS[d.status] ?? d.status}
                </span>
              </div>
              <p className="mt-1 text-xs text-slate-500">{formatDate(d.creationTime)}</p>
              <p className="mt-2 text-sm text-slate-600">{d.description}</p>
              {d.resolutionNote && (
                <div className="mt-3 rounded-lg border border-slate-100 bg-slate-50 p-3 text-sm text-slate-700">
                  <span className="font-medium">Çözüm: </span>
                  {d.resolutionNote}
                  {d.resolvedAt && (
                    <p className="mt-1 text-xs text-slate-500">{formatDate(d.resolvedAt)}</p>
                  )}
                </div>
              )}
              {(d.status === 0 || d.status === 1) && (
                <div className="mt-4 border-t border-slate-100 pt-4">
                  {selectedId !== d.id ? (
                    <button
                      type="button"
                      onClick={() => setSelectedId(d.id)}
                      className="rounded-lg border border-slate-300 bg-white px-3 py-1.5 text-sm font-medium text-slate-700 hover:bg-slate-50"
                    >
                      Çözüm yaz
                    </button>
                  ) : (
                    <>
                      <textarea
                        value={resolutionNote}
                        placeholder="Çözüm notu..."
                        rows={3}
                        className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm text-slate-800"
                        onChange={(e) => setResolutionNote(e.target.value)}
                      />
                      <div className="mt-2 flex gap-2">
                        <button
                          type="button"
                          onClick={() => handleResolve(d.id)}
                          disabled={!resolutionNote.trim() || resolvingId === d.id}
                          className="rounded-lg bg-fitliyo-green px-3 py-1.5 text-sm font-medium text-white hover:bg-fitliyo-green/90 disabled:opacity-50"
                        >
                          {resolvingId === d.id ? "Kaydediliyor..." : "Çözüldü olarak kaydet"}
                        </button>
                        <button
                          type="button"
                          onClick={() => { setSelectedId(null); setResolutionNote(""); }}
                          className="rounded-lg border border-slate-300 px-3 py-1.5 text-sm font-medium text-slate-700 hover:bg-slate-50"
                        >
                          İptal
                        </button>
                      </div>
                    </>
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
