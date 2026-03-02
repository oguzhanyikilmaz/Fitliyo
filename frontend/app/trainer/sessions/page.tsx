"use client";

import { useEffect, useState } from "react";
import { apiFetch, buildQuery, DEFAULT_LIST_PARAMS } from "@/lib/api";
import { ApiPaths } from "@/lib/api-paths";
import type { PagedResultDto, OrderDto, GetOrderListDto, SessionDto } from "@/lib/types";

const SESSION_STATUS_LABELS: Record<number, string> = {
  0: "Planlandı",
  1: "Devam ediyor",
  2: "Tamamlandı",
  3: "İptal",
};

function formatDateTime(s: string) {
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

export default function EgitmenSeanslarPage() {
  const [sessions, setSessions] = useState<SessionDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const params: GetOrderListDto = { ...DEFAULT_LIST_PARAMS, maxResultCount: 100 };
    const query = buildQuery(params as Record<string, string | number | boolean | undefined | null>);

    apiFetch<PagedResultDto<OrderDto>>(ApiPaths.Order.getTrainerOrdersAsync(query))
      .then(async (ordersRes) => {
        const orders = ordersRes.items ?? [];
        const allSessions: SessionDto[] = [];
        for (const order of orders) {
          try {
            const res = await apiFetch<PagedResultDto<SessionDto>>(
              ApiPaths.Order.getSessionsAsync(order.id)
            );
            const list = res.items ?? [];
            list.forEach((s) => allSessions.push(s));
          } catch {
            // skip order if sessions fail
          }
        }
        allSessions.sort(
          (a, b) =>
            new Date(b.scheduledStartTime).getTime() - new Date(a.scheduledStartTime).getTime()
        );
        setSessions(allSessions);
      })
      .catch((e) => setError(e instanceof Error ? e.message : "Seanslar yüklenemedi"))
      .finally(() => setLoading(false));
  }, []);

  if (loading) {
    return (
      <div className="flex min-h-[40vh] items-center justify-center">
        <p className="text-slate-600">Seanslar yükleniyor...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="space-y-4">
        <h1 className="text-2xl font-bold text-slate-800">Seanslarım</h1>
        <div className="rounded-xl border border-amber-200 bg-amber-50 p-4 text-amber-800">{error}</div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-slate-800">Seanslarım</h1>

      {sessions.length === 0 ? (
        <div className="rounded-xl border border-slate-200 bg-slate-50 p-8 text-center text-slate-600">
          Henüz seans kaydı yok.
        </div>
      ) : (
        <ul className="space-y-3">
          {sessions.map((s) => (
            <li
              key={s.id}
              className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm"
            >
              <div className="flex flex-wrap items-start justify-between gap-2">
                <div>
                  <p className="font-medium text-slate-800">
                    Sipariş seansı #{s.sequenceNumber}
                  </p>
                  <p className="mt-1 text-sm text-slate-600">
                    {formatDateTime(s.scheduledStartTime)} – {formatDateTime(s.scheduledEndTime)}
                  </p>
                  {s.meetingUrl && (
                    <a
                      href={s.meetingUrl}
                      target="_blank"
                      rel="noopener noreferrer"
                      className="mt-2 inline-block text-sm text-fitliyo-green hover:underline"
                    >
                      Toplantı linki
                    </a>
                  )}
                </div>
                <span className="rounded-full bg-slate-100 px-2 py-0.5 text-xs text-slate-600">
                  {SESSION_STATUS_LABELS[s.status] ?? s.status}
                </span>
              </div>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
