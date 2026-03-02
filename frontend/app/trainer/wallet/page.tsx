"use client";

import { useEffect, useState } from "react";
import { apiFetch, buildQuery } from "@/lib/api";
import { ApiPaths } from "@/lib/api-paths";
import type {
  TrainerWalletDto,
  PagedResultDto,
  WalletTransactionDto,
  GetWalletTransactionListDto,
} from "@/lib/types";

const TX_TYPE_LABELS: Record<number, string> = {
  0: "Kredi",
  1: "Borç",
  2: "İade",
  3: "Ödeme",
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

export default function EgitmenCuzdanPage() {
  const [wallet, setWallet] = useState<TrainerWalletDto | null>(null);
  const [transactions, setTransactions] = useState<PagedResultDto<WalletTransactionDto> | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const params: GetWalletTransactionListDto = {
      skipCount: 0,
      maxResultCount: 50,
      sorting: "creationTime desc",
    };
    const query = buildQuery(params as Record<string, string | number | boolean | undefined | null>);

    Promise.all([
      apiFetch<TrainerWalletDto>(ApiPaths.Wallet.getMyWalletAsync()),
      apiFetch<PagedResultDto<WalletTransactionDto>>(
        ApiPaths.Wallet.getMyTransactionsAsync(query)
      ),
    ])
      .then(([w, t]) => {
        setWallet(w);
        setTransactions(t);
      })
      .catch((e) => setError(e instanceof Error ? e.message : "Cüzdan yüklenemedi"))
      .finally(() => setLoading(false));
  }, []);

  if (loading) {
    return (
      <div className="flex min-h-[40vh] items-center justify-center">
        <p className="text-slate-600">Cüzdan yükleniyor...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="space-y-4">
        <h1 className="text-2xl font-bold text-slate-800">Cüzdan</h1>
        <div className="rounded-xl border border-amber-200 bg-amber-50 p-4 text-amber-800">{error}</div>
      </div>
    );
  }

  const items = transactions?.items ?? [];

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-slate-800">Cüzdan</h1>

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
          <h2 className="text-sm font-medium text-slate-500">Toplam çekilen</h2>
          <p className="mt-1 text-2xl font-bold text-slate-800">
            ₺{wallet?.totalWithdrawn?.toLocaleString("tr-TR", { minimumFractionDigits: 2 }) ?? "0,00"}
          </p>
        </div>
      </div>

      <div className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
        <h2 className="text-lg font-semibold text-slate-800">Son işlemler</h2>
        {items.length === 0 ? (
          <p className="mt-4 text-sm text-slate-500">Henüz işlem yok.</p>
        ) : (
          <ul className="mt-4 divide-y divide-slate-100">
            {items.map((tx) => (
              <li key={tx.id} className="flex flex-wrap items-center justify-between gap-2 py-3">
                <div>
                  <p className="text-sm font-medium text-slate-800">{tx.description}</p>
                  <p className="text-xs text-slate-400">{formatDate(tx.creationTime)}</p>
                </div>
                <div className="flex items-center gap-2">
                  <span className="text-sm font-medium text-slate-800">
                    {tx.transactionType === 0 || tx.transactionType === 2 ? "+" : "-"}
                    ₺{Math.abs(tx.amount).toLocaleString("tr-TR", { minimumFractionDigits: 2 })}
                  </span>
                  <span className="rounded bg-slate-100 px-2 py-0.5 text-xs text-slate-600">
                    {TX_TYPE_LABELS[tx.transactionType] ?? tx.transactionType}
                  </span>
                </div>
              </li>
            ))}
          </ul>
        )}
      </div>
    </div>
  );
}
