"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { apiFetch, buildQuery } from "@/lib/api";
import { ApiPaths } from "@/lib/api-paths";
import type { PagedResultDto, OrderDto, GetOrderListDto, SessionDto } from "@/lib/types";

const ORDER_STATUS_LABELS: Record<number, string> = {
  0: "Beklemede",
  1: "Onaylandı",
  2: "Devam ediyor",
  3: "Tamamlandı",
  4: "İptal",
};

function formatDate(s: string) {
  try {
    return new Date(s).toLocaleDateString("tr-TR", { day: "2-digit", month: "short", year: "numeric" });
  } catch {
    return s;
  }
}

export default function OgrenciDashboardPage() {
  const [orders, setOrders] = useState<OrderDto[]>([]);
  const [sessions, setSessions] = useState<SessionDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const params: GetOrderListDto = {
      skipCount: 0,
      maxResultCount: 10,
      sorting: "creationTime desc",
    };
    const query = buildQuery(params as Record<string, string | number | boolean | undefined | null>);
    apiFetch<PagedResultDto<OrderDto>>(ApiPaths.Order.getMyOrdersAsync(query))
      .then((res) => {
        setOrders(res.items ?? []);
        const firstOrder = res.items?.[0];
        if (firstOrder?.id) {
          return apiFetch<PagedResultDto<SessionDto>>(ApiPaths.Order.getSessionsAsync(firstOrder.id));
        }
        return { items: [] as SessionDto[] };
      })
      .then((res) => setSessions(res.items ?? []))
      .catch((e) => setError(e instanceof Error ? e.message : "Veri yüklenemedi"))
      .finally(() => setLoading(false));
  }, []);

  const upcomingSessions = sessions.filter((s) => new Date(s.scheduledStartTime) >= new Date()).slice(0, 5);

  if (loading) {
    return (
      <div className="flex min-h-[40vh] items-center justify-center">
        <p className="text-slate-600">Yükleniyor...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="space-y-4">
        <h1 className="text-2xl font-bold text-slate-800">Öğrenci Dashboard</h1>
        <div className="rounded-xl border border-amber-200 bg-amber-50 p-4 text-amber-800">{error}</div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-slate-800">Öğrenci Dashboard</h1>

      <div className="grid gap-4 sm:grid-cols-2">
        <div className="rounded-xl border bg-white p-4 shadow-sm">
          <h2 className="font-semibold text-slate-800">Yaklaşan Seanslar</h2>
          {upcomingSessions.length === 0 ? (
            <p className="mt-2 text-sm text-slate-500">Yaklaşan seansınız yok.</p>
          ) : (
            <ul className="mt-2 space-y-2">
              {upcomingSessions.map((s) => (
                <li key={s.id} className="text-sm">
                  <span className="text-slate-600">{formatDate(s.scheduledStartTime)}</span>
                  {s.meetingUrl && (
                    <a href={s.meetingUrl} target="_blank" rel="noopener noreferrer" className="ml-2 text-fitliyo-green hover:underline">
                      Bağlantı
                    </a>
                  )}
                </li>
              ))}
            </ul>
          )}
          <Link href="/student/sessions" className="mt-2 inline-block text-sm font-medium text-fitliyo-green hover:underline">
            Tüm seanslar →
          </Link>
        </div>

        <div className="rounded-xl border bg-white p-4 shadow-sm">
          <h2 className="font-semibold text-slate-800">Son Siparişler</h2>
          {orders.length === 0 ? (
            <p className="mt-2 text-sm text-slate-500">Henüz siparişiniz yok.</p>
          ) : (
            <ul className="mt-2 space-y-2">
              {orders.slice(0, 5).map((o) => (
                <li key={o.id} className="flex items-center justify-between text-sm">
                  <span className="text-slate-700">{o.packageTitle ?? o.orderNumber}</span>
                  <span className="text-slate-500">{ORDER_STATUS_LABELS[o.status] ?? o.status}</span>
                </li>
              ))}
            </ul>
          )}
          <Link href="/student/orders" className="mt-2 inline-block text-sm font-medium text-fitliyo-green hover:underline">
            Tüm siparişler →
          </Link>
        </div>
      </div>

      <div className="flex gap-4">
        <Link href="/student/trainers" className="rounded-lg bg-fitliyo-green px-4 py-2 text-sm font-medium text-white hover:bg-fitliyo-dark">
          Eğitmenleri Keşfet
        </Link>
        <Link href="/student/packages" className="rounded-lg border border-fitliyo-green px-4 py-2 text-sm font-medium text-fitliyo-green hover:bg-fitliyo-green/10">
          Paketlere Göz At
        </Link>
      </div>
    </div>
  );
}
