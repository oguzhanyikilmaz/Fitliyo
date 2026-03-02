"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { apiFetch, buildQuery, DEFAULT_LIST_PARAMS } from "@/lib/api";
import { ApiPaths } from "@/lib/api-paths";
import type { PagedResultDto, OrderDto, GetOrderListDto } from "@/lib/types";

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

export default function OgrenciSiparislerPage() {
  const [data, setData] = useState<PagedResultDto<OrderDto> | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const params: GetOrderListDto = { ...DEFAULT_LIST_PARAMS };
    const query = buildQuery(params as Record<string, string | number | boolean | undefined | null>);
    apiFetch<PagedResultDto<OrderDto>>(ApiPaths.Order.getMyOrdersAsync(query))
      .then(setData)
      .catch((e) => setError(e instanceof Error ? e.message : "Siparişler yüklenemedi"))
      .finally(() => setLoading(false));
  }, []);

  if (loading) {
    return (
      <div className="flex min-h-[40vh] items-center justify-center">
        <p className="text-slate-600">Siparişler yükleniyor...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="space-y-4">
        <h1 className="text-2xl font-bold text-slate-800">Siparişlerim</h1>
        <div className="rounded-xl border border-amber-200 bg-amber-50 p-4 text-amber-800">{error}</div>
      </div>
    );
  }

  const items = data?.items ?? [];

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-slate-800">Siparişlerim</h1>

      {items.length === 0 ? (
        <div className="rounded-xl border border-dashed border-slate-300 bg-slate-50 p-8 text-center text-slate-600">
          Henüz siparişiniz yok.
        </div>
      ) : (
        <div className="space-y-3">
          {items.map((order) => (
            <Link
              key={order.id}
              href={`/student/orders/${order.id}`}
              className="block rounded-xl border bg-white p-4 shadow-sm transition hover:border-fitliyo-green hover:shadow-md"
            >
              <div className="flex flex-wrap items-center justify-between gap-2">
                <div>
                  <p className="font-medium text-slate-800">{order.packageTitle ?? order.orderNumber}</p>
                  <p className="text-sm text-slate-500">{order.trainerFullName} · {formatDate(order.creationTime)}</p>
                </div>
                <div className="flex items-center gap-2">
                  <span className="rounded-full bg-slate-100 px-2 py-0.5 text-sm text-slate-700">
                    {ORDER_STATUS_LABELS[order.status] ?? order.status}
                  </span>
                  <span className="text-sm font-medium text-slate-700">{order.netAmount} {order.currency}</span>
                </div>
              </div>
            </Link>
          ))}
        </div>
      )}

      <Link href="/student/trainers" className="inline-block text-sm font-medium text-fitliyo-green hover:underline">
        Eğitmenleri keşfet →
      </Link>
    </div>
  );
}
