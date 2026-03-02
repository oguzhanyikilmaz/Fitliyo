"use client";

import Link from "next/link";
import { useParams, useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import { apiFetch } from "@/lib/api";
import { ApiPaths } from "@/lib/api-paths";
import type {
  ConversationDto,
  OrderDto,
  PagedResultDto,
  SessionDto,
  UpdateOrderDeliveryDto,
} from "@/lib/types";

const ORDER_STATUS_LABELS: Record<number, string> = {
  0: "Beklemede",
  1: "Onaylandı",
  2: "Devam ediyor",
  3: "Tamamlandı",
  4: "İptal",
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

export default function TrainerOrderDetailPage() {
  const params = useParams();
  const router = useRouter();
  const id = params?.id as string;
  const [order, setOrder] = useState<OrderDto | null>(null);
  const [sessions, setSessions] = useState<SessionDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [deliveryNotes, setDeliveryNotes] = useState("");
  const [deliveryUrl, setDeliveryUrl] = useState("");
  const [submittingDelivery, setSubmittingDelivery] = useState(false);
  const [messageOrderLoading, setMessageOrderLoading] = useState(false);

  useEffect(() => {
    if (!id) return;
    apiFetch<OrderDto>(ApiPaths.Order.getAsync(id))
      .then((o) => {
        setOrder(o);
        setDeliveryNotes(o.trainerProgramNotes ?? "");
        setDeliveryUrl(o.programAttachmentUrl ?? "");
        return apiFetch<PagedResultDto<SessionDto>>(ApiPaths.Order.getSessionsAsync(id));
      })
      .then((res) => setSessions((res as PagedResultDto<SessionDto>).items ?? []))
      .catch((e) => setError(e instanceof Error ? e.message : "Sipariş yüklenemedi"))
      .finally(() => setLoading(false));
  }, [id]);

  const handleSubmitDelivery = (markAsDelivered: boolean) => {
    if (!id || submittingDelivery) return;
    setSubmittingDelivery(true);
    const body: UpdateOrderDeliveryDto = {
      trainerProgramNotes: deliveryNotes.trim() || undefined,
      programAttachmentUrl: deliveryUrl.trim() || undefined,
      markAsDelivered,
    };
    apiFetch<OrderDto>(ApiPaths.Order.updateOrderDeliveryAsync(id), {
      method: "PUT",
      body: JSON.stringify(body),
    })
      .then(setOrder)
      .finally(() => setSubmittingDelivery(false));
  };

  const handleMessageAboutOrder = () => {
    if (!id || messageOrderLoading) return;
    setMessageOrderLoading(true);
    apiFetch<ConversationDto>(ApiPaths.Messaging.getOrCreateConversationForOrderAsync(id))
      .then((conv) => router.push(`/trainer/messages?conversationId=${conv.id}`))
      .finally(() => setMessageOrderLoading(false));
  };

  if (loading) {
    return (
      <div className="flex min-h-[40vh] items-center justify-center">
        <p className="text-slate-600">Yükleniyor...</p>
      </div>
    );
  }

  if (error || !order) {
    return (
      <div className="space-y-4">
        <h1 className="text-2xl font-bold text-slate-800">Sipariş detayı</h1>
        <div className="rounded-xl border border-amber-200 bg-amber-50 p-4 text-amber-800">
          {error ?? "Sipariş bulunamadı."}
        </div>
        <Link href="/trainer/orders" className="inline-block text-sm font-medium text-fitliyo-green hover:underline">
          ← Siparişlerime dön
        </Link>
      </div>
    );
  }

  return (
    <div className="mx-auto max-w-2xl space-y-6">
      <Link href="/trainer/orders" className="inline-block text-sm font-medium text-fitliyo-green hover:underline">
        ← Siparişlerim
      </Link>

      <div className="rounded-xl border bg-white p-6 shadow-sm">
        <h1 className="text-xl font-bold text-slate-800">Sipariş #{order.orderNumber}</h1>
        <p className="mt-1 text-slate-500">{order.packageTitle}</p>
        <div className="mt-4 flex flex-wrap gap-3">
          <span className="rounded-full bg-slate-100 px-2 py-1 text-sm text-slate-700">
            {ORDER_STATUS_LABELS[order.status] ?? order.status}
          </span>
          <span className="text-sm font-medium text-slate-700">
            {order.netAmount} {order.currency}
          </span>
          <span className="text-sm text-slate-500">{formatDate(order.creationTime)}</span>
        </div>
      </div>

      {order.studentFormData != null && order.studentFormData !== "" && (
        <div className="rounded-xl border border-sky-200 bg-sky-50 p-6">
          <h2 className="font-semibold text-sky-900">Öğrencinin ilettiği bilgiler</h2>
          {order.studentFormSubmittedAt && (
            <p className="mt-1 text-xs text-sky-700">
              Gönderim: {formatDate(order.studentFormSubmittedAt)}
            </p>
          )}
          <div className="mt-2 whitespace-pre-wrap text-sm text-sky-800">{order.studentFormData}</div>
        </div>
      )}

      <div className="rounded-xl border bg-white p-6 shadow-sm">
        <h2 className="font-semibold text-slate-800">Program teslimi</h2>
        <p className="mt-1 text-sm text-slate-500">
          Programa dair notlarınız ve dosya/link (PDF vb.) — öğrenci sipariş detayında görecek.
        </p>
        <textarea
          value={deliveryNotes}
          onChange={(e) => setDeliveryNotes(e.target.value)}
          placeholder="Program notları, kullanım talimatı..."
          rows={3}
          className="mt-2 w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-800"
        />
        <input
          type="url"
          value={deliveryUrl}
          onChange={(e) => setDeliveryUrl(e.target.value)}
          placeholder="Program PDF/link URL"
          className="mt-2 w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-800"
        />
        <div className="mt-3 flex flex-wrap gap-2">
          <button
            type="button"
            onClick={() => handleSubmitDelivery(false)}
            disabled={submittingDelivery}
            className="rounded-lg bg-slate-200 px-4 py-2 text-sm font-medium text-slate-800 hover:bg-slate-300 disabled:opacity-50"
          >
            {submittingDelivery ? "Kaydediliyor..." : "Kaydet"}
          </button>
          <button
            type="button"
            onClick={() => handleSubmitDelivery(true)}
            disabled={submittingDelivery}
            className="rounded-lg bg-fitliyo-green px-4 py-2 text-sm font-medium text-white hover:opacity-90 disabled:opacity-50"
          >
            Kaydet ve teslim edildi işaretle
          </button>
        </div>
        {order.programDeliveredAt && (
          <p className="mt-2 text-xs text-slate-500">
            Teslim tarihi: {formatDate(order.programDeliveredAt)}
          </p>
        )}
      </div>

      <div className="flex flex-wrap gap-3">
        <button
          type="button"
          onClick={handleMessageAboutOrder}
          disabled={messageOrderLoading}
          className="rounded-lg border border-slate-300 bg-white px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50 disabled:opacity-50"
        >
          {messageOrderLoading ? "Açılıyor..." : "Bu sipariş hakkında öğrenciyle yazış"}
        </button>
      </div>

      {sessions.length > 0 && (
        <div className="rounded-xl border bg-white p-6 shadow-sm">
          <h2 className="font-semibold text-slate-800">Seanslar ({sessions.length})</h2>
          <ul className="mt-2 space-y-2">
            {sessions.map((s) => (
              <li key={s.id} className="flex justify-between rounded-lg border border-slate-100 p-2 text-sm">
                <span>Seans #{s.sequenceNumber}</span>
                <span className="text-slate-500">{formatDate(s.scheduledStartTime)}</span>
              </li>
            ))}
          </ul>
        </div>
      )}
    </div>
  );
}
