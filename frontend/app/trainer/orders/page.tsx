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

export default function EgitmenSiparislerPage() {
  const [data, setData] = useState<PagedResultDto<OrderDto> | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const params: GetOrderListDto = { ...DEFAULT_LIST_PARAMS };
    const query = buildQuery(params as Record<string, string | number | boolean | undefined | null>);
    apiFetch<PagedResultDto<OrderDto>>(ApiPaths.Order.getTrainerOrdersAsync(query))
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
        <div className="rounded-xl border border-slate-200 bg-slate-50 p-8 text-center text-slate-600">
          Henüz siparişiniz yok.
        </div>
      ) : (
        <div className="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-sm">
          <ul className="divide-y divide-slate-100">
            {items.map((o) => (
              <li key={o.id}>
                <Link
                  href={`/trainer/orders/${o.id}`}
                  className="block p-4 hover:bg-slate-50 sm:flex sm:flex-wrap sm:items-center sm:justify-between sm:gap-2"
                >
                  <div className="flex-1">
                    <span className="font-mono text-sm font-medium text-slate-800">{o.orderNumber}</span>
                    <p className="mt-0.5 text-sm text-slate-600">{o.packageTitle ?? "—"}</p>
                    <p className="text-xs text-slate-400">{formatDate(o.creationTime)}</p>
                  </div>
                  <div className="mt-2 flex items-center gap-2 sm:mt-0">
                    <span className="rounded-full bg-slate-100 px-2 py-0.5 text-xs text-slate-600">
                      {ORDER_STATUS_LABELS[o.status] ?? o.status}
                    </span>
                    <span className="text-sm font-medium text-slate-800">
                      ₺{o.netAmount?.toLocaleString("tr-TR", { minimumFractionDigits: 2 }) ?? "0,00"}
                    </span>
                  </div>
                </Link>
              </li>
            ))}
          </ul>
        </div>
      )}
    </div>
  );
}
