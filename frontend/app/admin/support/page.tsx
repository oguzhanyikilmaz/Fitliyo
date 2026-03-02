"use client";

import { useEffect, useState } from "react";
import { apiFetch, buildQuery, DEFAULT_LIST_PARAMS } from "@/lib/api";
import { ApiPaths } from "@/lib/api-paths";
import type {
  PagedResultDto,
  SupportTicketDto,
  GetSupportTicketListDto,
  ReplySupportTicketDto,
} from "@/lib/types";

const CATEGORY_LABELS: Record<number, string> = {
  0: "Genel",
  1: "Ödeme",
  2: "Sipariş",
  3: "Teknik",
  4: "Hesap",
};

const STATUS_LABELS: Record<number, string> = {
  0: "Açık",
  1: "İnceleniyor",
  2: "Müşteri Bekleniyor",
  3: "Çözüldü",
  4: "Kapatıldı",
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

export default function AdminSupportPage() {
  const [data, setData] = useState<PagedResultDto<SupportTicketDto> | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [replyingId, setReplyingId] = useState<string | null>(null);
  const [selectedTicketId, setSelectedTicketId] = useState<string | null>(null);
  const [replyText, setReplyText] = useState("");
  const [replyError, setReplyError] = useState<string | null>(null);

  const load = () => {
    setLoading(true);
    const params: GetSupportTicketListDto = { ...DEFAULT_LIST_PARAMS };
    const query = buildQuery(params as Record<string, string | number | boolean | undefined | null>);
    apiFetch<PagedResultDto<SupportTicketDto>>(ApiPaths.SupportTicket.getListAsync(query))
      .then(setData)
      .catch((e) => setError(e instanceof Error ? e.message : "Talepler yüklenemedi"))
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    load();
  }, []);

  const handleReply = (id: string) => {
    if (!replyText.trim()) return;
    setReplyError(null);
    setReplyingId(id);
    const body: ReplySupportTicketDto = { adminReply: replyText };
    apiFetch<SupportTicketDto>(ApiPaths.SupportTicket.replyAsync(id), {
      method: "POST",
      body: JSON.stringify(body),
    })
      .then(() => {
        setReplyText("");
        setReplyingId(null);
        setSelectedTicketId(null);
        load();
      })
      .catch((e) => {
        setReplyError(e instanceof Error ? e.message : "Yanıt gönderilemedi");
        setReplyingId(null);
      });
  };

  if (loading && !data) {
    return (
      <div className="flex min-h-[40vh] items-center justify-center">
        <p className="text-slate-600">Destek talepleri yükleniyor...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="space-y-4">
        <h1 className="text-2xl font-bold text-slate-800">Destek Talepleri</h1>
        <div className="rounded-xl border border-amber-200 bg-amber-50 p-4 text-amber-800">{error}</div>
      </div>
    );
  }

  const items = data?.items ?? [];

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-slate-800">Destek Talepleri</h1>

      {replyError && (
        <div className="rounded-lg border border-amber-200 bg-amber-50 p-3 text-sm text-amber-800">
          {replyError}
        </div>
      )}

      {items.length === 0 ? (
        <div className="rounded-xl border border-slate-200 bg-slate-50 p-8 text-center text-slate-600">
          Henüz destek talebi yok.
        </div>
      ) : (
        <ul className="space-y-4">
          {items.map((t) => (
            <li key={t.id} className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
              <div className="flex flex-wrap justify-between gap-2">
                <p className="font-medium text-slate-800">{t.subject}</p>
                <span className="rounded-full bg-slate-100 px-2 py-0.5 text-xs text-slate-600">
                  {STATUS_LABELS[t.status] ?? t.status}
                </span>
              </div>
              <p className="mt-1 text-xs text-slate-500">
                {CATEGORY_LABELS[t.category] ?? t.category} · {formatDate(t.creationTime)}
              </p>
              <p className="mt-2 text-sm text-slate-600">{t.message}</p>
              {t.adminReply && (
                <div className="mt-3 rounded-lg border border-slate-100 bg-slate-50 p-3 text-sm text-slate-700">
                  <span className="font-medium">Yanıt: </span>
                  {t.adminReply}
                  {t.adminReplyDate && (
                    <p className="mt-1 text-xs text-slate-500">{formatDate(t.adminReplyDate)}</p>
                  )}
                </div>
              )}
              {(t.status === 0 || t.status === 1 || t.status === 2) && (
                <div className="mt-4 border-t border-slate-100 pt-4">
                  {selectedTicketId !== t.id ? (
                    <button
                      type="button"
                      onClick={() => setSelectedTicketId(t.id)}
                      className="rounded-lg border border-slate-300 bg-white px-3 py-1.5 text-sm font-medium text-slate-700 hover:bg-slate-50"
                    >
                      Yanıt yaz
                    </button>
                  ) : (
                    <>
                      <textarea
                        value={replyText}
                        placeholder="Admin yanıtı..."
                        rows={3}
                        className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm text-slate-800"
                        onChange={(e) => setReplyText(e.target.value)}
                      />
                      <div className="mt-2 flex gap-2">
                        <button
                          type="button"
                          onClick={() => handleReply(t.id)}
                          disabled={!replyText.trim() || replyingId === t.id}
                          className="rounded-lg bg-fitliyo-green px-3 py-1.5 text-sm font-medium text-white hover:bg-fitliyo-green/90 disabled:opacity-50"
                        >
                          {replyingId === t.id ? "Gönderiliyor..." : "Gönder"}
                        </button>
                        <button
                          type="button"
                          onClick={() => { setSelectedTicketId(null); setReplyText(""); }}
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
