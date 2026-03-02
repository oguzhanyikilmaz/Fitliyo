"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { apiFetch, buildQuery, DEFAULT_LIST_PARAMS } from "@/lib/api";
import { ApiPaths } from "@/lib/api-paths";
import type { PagedResultDto, OrderDto, GetOrderListDto, SessionDto } from "@/lib/types";

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

export default function StudentSessionsPage() {
  const [sessions, setSessions] = useState<(SessionDto & { orderNumber?: string })[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const params: GetOrderListDto = {
      ...DEFAULT_LIST_PARAMS,
      maxResultCount: 20,
    };
    const query = buildQuery(params as Record<string, string | number | boolean | undefined | null>);
    apiFetch<PagedResultDto<OrderDto>>(ApiPaths.Order.getMyOrdersAsync(query))
      .then((res) => {
        const orders = res.items ?? [];
        if (orders.length === 0) return [];
        return Promise.all(
          orders.slice(0, 10).map((o) =>
            apiFetch<PagedResultDto<SessionDto>>(ApiPaths.Order.getSessionsAsync(o.id)).then((r) =>
              (r.items ?? []).map((s) => ({ ...s, orderId: o.id, orderNumber: o.orderNumber }))
            )
          )
        );
      })
      .then((chunks) => {
        const all = chunks.flat() as (SessionDto & { orderNumber?: string })[];
        all.sort((a, b) => new Date(a.scheduledStartTime).getTime() - new Date(b.scheduledStartTime).getTime());
        setSessions(all);
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

  const now = new Date();
  const upcoming = sessions.filter((s) => new Date(s.scheduledStartTime) >= now);
  const past = sessions.filter((s) => new Date(s.scheduledStartTime) < now);

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-slate-800">Seanslarım</h1>

      {sessions.length === 0 ? (
        <div className="rounded-xl border border-dashed border-slate-300 bg-slate-50 p-8 text-center text-slate-600">
          <p>Henüz seansınız yok. Bir paket satın alarak seans planlayabilirsiniz.</p>
          <p className="mt-2 text-sm">
            Seans içermeyen program paketleri (örn. 3 aylık antrenman+beslenme) satın aldıysanız, detaylar için{" "}
            <Link href="/student/orders" className="font-medium text-fitliyo-green hover:underline">
              Siparişlerim
            </Link>
            sayfasındaki ilgili siparişe tıklayın.
          </p>
        </div>
      ) : (
        <>
          {upcoming.length > 0 && (
            <div>
              <h2 className="font-semibold text-slate-800">Yaklaşan Seanslar</h2>
              <ul className="mt-2 space-y-2">
                {upcoming.map((s) => (
                  <li key={s.id} className="flex flex-wrap items-center justify-between gap-2 rounded-xl border bg-white p-4">
                    <div>
                      <p className="font-medium text-slate-800">{formatDate(s.scheduledStartTime)}</p>
                      <p className="text-sm text-slate-500">Sipariş #{s.orderNumber ?? s.orderId}</p>
                    </div>
                    <div className="flex items-center gap-2">
                      {s.meetingUrl && (
                        <a
                          href={s.meetingUrl}
                          target="_blank"
                          rel="noopener noreferrer"
                          className="rounded-lg bg-fitliyo-green px-3 py-1.5 text-sm font-medium text-white hover:bg-fitliyo-dark"
                        >
                          Bağlantıya Git
                        </a>
                      )}
                      <Link
                        href={`/student/orders/${s.orderId}`}
                        className="text-sm font-medium text-fitliyo-green hover:underline"
                      >
                        Sipariş detayı
                      </Link>
                    </div>
                  </li>
                ))}
              </ul>
            </div>
          )}
          {past.length > 0 && (
            <div>
              <h2 className="font-semibold text-slate-800">Geçmiş Seanslar</h2>
              <ul className="mt-2 space-y-2">
                {past.slice(0, 10).map((s) => (
                  <li key={s.id} className="rounded-xl border border-slate-100 bg-white p-3">
                    <p className="text-sm text-slate-700">{formatDate(s.scheduledStartTime)}</p>
                    <Link href={`/student/orders/${s.orderId}`} className="text-xs text-fitliyo-green hover:underline">
                      Sipariş detayı
                    </Link>
                  </li>
                ))}
              </ul>
              {past.length > 10 && (
                <p className="mt-2 text-sm text-slate-500">Son 10 geçmiş seans gösteriliyor.</p>
              )}
            </div>
          )}
        </>
      )}

      <Link href="/student/orders" className="inline-block text-sm font-medium text-fitliyo-green hover:underline">
        Tüm siparişlerim →
      </Link>
    </div>
  );
}
