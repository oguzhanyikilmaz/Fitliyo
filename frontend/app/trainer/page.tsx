"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { apiFetch, buildQuery } from "@/lib/api";
import { ApiPaths } from "@/lib/api-paths";
import type { PagedResultDto, OrderDto, GetOrderListDto, TrainerWalletDto } from "@/lib/types";

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

export default function EgitmenDashboardPage() {
  const [wallet, setWallet] = useState<TrainerWalletDto | null>(null);
  const [orders, setOrders] = useState<PagedResultDto<OrderDto> | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const params: GetOrderListDto = {
      skipCount: 0,
      maxResultCount: 5,
      sorting: "creationTime desc",
    };
    const query = buildQuery(params as Record<string, string | number | boolean | undefined | null>);

    Promise.all([
      apiFetch<TrainerWalletDto>(ApiPaths.Wallet.getMyWalletAsync()).catch(() => null),
      apiFetch<PagedResultDto<OrderDto>>(ApiPaths.Order.getTrainerOrdersAsync(query)).catch(() => null),
    ])
      .then(([w, o]) => {
        setWallet(w ?? null);
        setOrders(o ?? null);
      })
      .catch((e) => setError(e instanceof Error ? e.message : "Veriler yüklenemedi"))
      .finally(() => setLoading(false));
  }, []);

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
        <h1 className="text-2xl font-bold text-slate-800">Eğitmen Dashboard</h1>
        <div className="rounded-xl border border-amber-200 bg-amber-50 p-4 text-amber-800">{error}</div>
      </div>
    );
  }

  const orderItems = orders?.items ?? [];
  const pendingCount = orderItems.filter((o) => o.status === 0).length;

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-slate-800">Eğitmen Dashboard</h1>

      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <div className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
          <h2 className="text-sm font-medium text-slate-500">Kullanılabilir bakiye</h2>
          <p className="mt-1 text-2xl font-bold text-slate-800">
            ₺{wallet?.availableBalance?.toLocaleString("tr-TR", { minimumFractionDigits: 2 }) ?? "0,00"}
          </p>
        </div>
        <div className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
          <h2 className="text-sm font-medium text-slate-500">Bekleyen bakiye</h2>
          <p className="mt-1 text-2xl font-bold text-slate-800">
            ₺{wallet?.pendingBalance?.toLocaleString("tr-TR", { minimumFractionDigits: 2 }) ?? "0,00"}
          </p>
        </div>
        <div className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
          <h2 className="text-sm font-medium text-slate-500">Toplam kazanç</h2>
          <p className="mt-1 text-2xl font-bold text-slate-800">
            ₺{wallet?.totalEarned?.toLocaleString("tr-TR", { minimumFractionDigits: 2 }) ?? "0,00"}
          </p>
        </div>
        <div className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
          <h2 className="text-sm font-medium text-slate-500">Bekleyen sipariş</h2>
          <p className="mt-1 text-2xl font-bold text-slate-800">{pendingCount}</p>
        </div>
      </div>

      <div className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
        <div className="flex items-center justify-between">
          <h2 className="text-lg font-semibold text-slate-800">Son siparişler</h2>
          <Link
            href="/trainer/orders"
            className="text-sm font-medium text-fitliyo-green hover:underline"
          >
            Tümü →
          </Link>
        </div>
        {orderItems.length === 0 ? (
          <p className="mt-4 text-sm text-slate-500">Henüz sipariş yok.</p>
        ) : (
          <ul className="mt-4 space-y-2">
            {orderItems.map((o) => (
              <li key={o.id} className="flex flex-wrap items-center justify-between gap-2 border-b border-slate-100 py-2 last:border-0">
                <span className="font-mono text-sm text-slate-600">{o.orderNumber}</span>
                <span className="text-sm text-slate-600">{o.packageTitle ?? "—"}</span>
                <span className="rounded bg-slate-100 px-2 py-0.5 text-xs text-slate-600">
                  {ORDER_STATUS_LABELS[o.status] ?? o.status}
                </span>
                <span className="text-xs text-slate-400">{formatDate(o.creationTime)}</span>
              </li>
            ))}
          </ul>
        )}
      </div>

      <div className="flex flex-wrap gap-4">
        <Link
          href="/trainer/sessions"
          className="rounded-lg bg-fitliyo-green px-4 py-2 text-sm font-medium text-white hover:bg-fitliyo-green/90"
        >
          Seanslarım
        </Link>
        <Link
          href="/trainer/wallet"
          className="rounded-lg border border-slate-300 bg-white px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50"
        >
          Cüzdan
        </Link>
        <Link
          href="/trainer/withdrawals"
          className="rounded-lg border border-slate-300 bg-white px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50"
        >
          Para çekme
        </Link>
      </div>
    </div>
  );
}
