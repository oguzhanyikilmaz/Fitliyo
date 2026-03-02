"use client";

import { useEffect, useState } from "react";
import { apiFetch, buildQuery } from "@/lib/api";
import { ApiPaths } from "@/lib/api-paths";
import type {
  PagedResultDto,
  WithdrawalRequestDto,
  GetWithdrawalRequestListDto,
  CreateWithdrawalRequestDto,
  TrainerWalletDto,
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

export default function EgitmenParaCekmePage() {
  const [wallet, setWallet] = useState<TrainerWalletDto | null>(null);
  const [data, setData] = useState<PagedResultDto<WithdrawalRequestDto> | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [showForm, setShowForm] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [form, setForm] = useState<CreateWithdrawalRequestDto>({
    amount: 0,
    iban: "",
    accountHolderName: "",
  });

  const load = () => {
    setLoading(true);
    const params: GetWithdrawalRequestListDto = {
      skipCount: 0,
      maxResultCount: 50,
      sorting: "creationTime desc",
    };
    const query = buildQuery(params as Record<string, string | number | boolean | undefined | null>);

    Promise.all([
      apiFetch<TrainerWalletDto>(ApiPaths.Wallet.getMyWalletAsync()).catch(() => null),
      apiFetch<PagedResultDto<WithdrawalRequestDto>>(
        ApiPaths.WithdrawalRequest.getMyRequestsAsync(query)
      ),
    ])
      .then(([w, d]) => {
        setWallet(w ?? null);
        setData(d);
      })
      .catch((e) => setError(e instanceof Error ? e.message : "Talepler yüklenemedi"))
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    load();
  }, []);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitError(null);
    if (form.amount <= 0 || !form.iban.trim() || !form.accountHolderName.trim()) {
      setSubmitError("Tutar, IBAN ve hesap sahibi adı zorunludur.");
      return;
    }
    const available = wallet?.availableBalance ?? 0;
    if (form.amount > available) {
      setSubmitError(`Kullanılabilir bakiye: ₺${available.toLocaleString("tr-TR", { minimumFractionDigits: 2 })}`);
      return;
    }
    setSubmitting(true);
    apiFetch<WithdrawalRequestDto>(ApiPaths.WithdrawalRequest.createAsync(), {
      method: "POST",
      body: JSON.stringify(form),
    })
      .then(() => {
        setForm({ amount: 0, iban: "", accountHolderName: "" });
        setShowForm(false);
        load();
      })
      .catch((e) => setSubmitError(e instanceof Error ? e.message : "Talep oluşturulamadı"))
      .finally(() => setSubmitting(false));
  };

  if (loading && !data) {
    return (
      <div className="flex min-h-[40vh] items-center justify-center">
        <p className="text-slate-600">Yükleniyor...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="space-y-4">
        <h1 className="text-2xl font-bold text-slate-800">Para Çekme</h1>
        <div className="rounded-xl border border-amber-200 bg-amber-50 p-4 text-amber-800">{error}</div>
      </div>
    );
  }

  const items = data?.items ?? [];
  const available = wallet?.availableBalance ?? 0;

  return (
    <div className="space-y-6">
      <div className="flex flex-wrap items-center justify-between gap-4">
        <h1 className="text-2xl font-bold text-slate-800">Para Çekme</h1>
        <button
          type="button"
          onClick={() => setShowForm(!showForm)}
          disabled={available <= 0}
          className="rounded-lg bg-fitliyo-green px-4 py-2 text-sm font-medium text-white shadow-sm hover:bg-fitliyo-green/90 disabled:opacity-50"
        >
          {showForm ? "İptal" : "Yeni talep"}
        </button>
      </div>

      <div className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
        <p className="text-sm text-slate-500">Kullanılabilir bakiye</p>
        <p className="text-2xl font-bold text-slate-800">
          ₺{available.toLocaleString("tr-TR", { minimumFractionDigits: 2 })}
        </p>
      </div>

      {showForm && (
        <form
          onSubmit={handleSubmit}
          className="rounded-xl border border-slate-200 bg-white p-6 shadow-sm"
        >
          <h2 className="mb-4 text-lg font-semibold text-slate-800">Yeni para çekme talebi</h2>
          {submitError && (
            <div className="mb-4 rounded-lg border border-amber-200 bg-amber-50 p-3 text-sm text-amber-800">
              {submitError}
            </div>
          )}
          <div className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-slate-700">Tutar (₺)</label>
              <input
                type="number"
                step="0.01"
                min="0.01"
                value={form.amount || ""}
                onChange={(e) => setForm((f) => ({ ...f, amount: Number(e.target.value) || 0 }))}
                required
                className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-800"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-slate-700">IBAN</label>
              <input
                type="text"
                value={form.iban}
                onChange={(e) => setForm((f) => ({ ...f, iban: e.target.value }))}
                required
                maxLength={34}
                placeholder="TR00 0000 0000 0000 0000 0000 00"
                className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-800"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-slate-700">Hesap sahibi adı</label>
              <input
                type="text"
                value={form.accountHolderName}
                onChange={(e) => setForm((f) => ({ ...f, accountHolderName: e.target.value }))}
                required
                maxLength={256}
                className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-800"
              />
            </div>
            <div className="flex gap-2">
              <button
                type="submit"
                disabled={submitting}
                className="rounded-lg bg-fitliyo-green px-4 py-2 text-sm font-medium text-white hover:bg-fitliyo-green/90 disabled:opacity-50"
              >
                {submitting ? "Gönderiliyor..." : "Talep oluştur"}
              </button>
              <button
                type="button"
                onClick={() => setShowForm(false)}
                className="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50"
              >
                İptal
              </button>
            </div>
          </div>
        </form>
      )}

      <div className="rounded-xl border border-slate-200 bg-white shadow-sm">
        <h2 className="border-b border-slate-100 p-4 text-lg font-semibold text-slate-800">
          Taleplerim
        </h2>
        {items.length === 0 ? (
          <p className="p-6 text-sm text-slate-500">Henüz para çekme talebiniz yok.</p>
        ) : (
          <ul className="divide-y divide-slate-100">
            {items.map((r) => (
              <li key={r.id} className="flex flex-wrap items-center justify-between gap-2 p-4">
                <div>
                  <p className="font-medium text-slate-800">
                    ₺{r.amount.toLocaleString("tr-TR", { minimumFractionDigits: 2 })}
                  </p>
                  <p className="text-xs text-slate-500">{formatDate(r.creationTime)}</p>
                </div>
                <span className="rounded-full bg-slate-100 px-2 py-0.5 text-xs text-slate-600">
                  {STATUS_LABELS[r.status] ?? r.status}
                </span>
              </li>
            ))}
          </ul>
        )}
      </div>
    </div>
  );
}
