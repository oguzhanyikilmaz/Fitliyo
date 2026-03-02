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
  UpdateOrderStudentFormDto,
} from "@/lib/types";

const ORDER_STATUS_LABELS: Record<number, string> = {
  0: "Beklemede",
  1: "Onaylandı",
  2: "Devam ediyor",
  3: "Tamamlandı",
  4: "İptal",
};

const SESSION_STATUS_LABELS: Record<number, string> = {
  0: "Planlandı",
  1: "Devam ediyor",
  2: "Tamamlandı",
  3: "İptal",
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

export default function StudentOrderDetailPage() {
  const params = useParams();
  const router = useRouter();
  const id = params?.id as string;
  const [order, setOrder] = useState<OrderDto | null>(null);
  const [sessions, setSessions] = useState<SessionDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [studentFormText, setStudentFormText] = useState("");
  const [submittingForm, setSubmittingForm] = useState(false);
  const [messageOrderLoading, setMessageOrderLoading] = useState(false);

  useEffect(() => {
    if (!id) return;
    apiFetch<OrderDto>(ApiPaths.Order.getAsync(id))
      .then((o) => {
        setOrder(o);
        return apiFetch<PagedResultDto<SessionDto>>(ApiPaths.Order.getSessionsAsync(id));
      })
      .then((res) => setSessions((res as PagedResultDto<SessionDto>).items ?? []))
      .catch((e) => setError(e instanceof Error ? e.message : "Sipariş yüklenemedi"))
      .finally(() => setLoading(false));
  }, [id]);

  useEffect(() => {
    if (order?.studentFormData != null) setStudentFormText(order.studentFormData);
  }, [order?.studentFormData]);

  const handleSubmitStudentForm = () => {
    if (!id || submittingForm) return;
    setSubmittingForm(true);
    const body: UpdateOrderStudentFormDto = { formData: studentFormText.trim() || undefined };
    apiFetch<OrderDto>(ApiPaths.Order.updateStudentFormAsync(id), {
      method: "PUT",
      body: JSON.stringify(body),
    })
      .then((updated) => setOrder(updated))
      .catch(() => setSubmittingForm(false))
      .finally(() => setSubmittingForm(false));
  };

  const handleMessageAboutOrder = () => {
    if (!id || messageOrderLoading) return;
    setMessageOrderLoading(true);
    apiFetch<ConversationDto>(ApiPaths.Messaging.getOrCreateConversationForOrderAsync(id))
      .then((conv) => router.push(`/student/messages?conversationId=${conv.id}`))
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
        <h1 className="text-2xl font-bold text-slate-800">Sipariş Detayı</h1>
        <div className="rounded-xl border border-amber-200 bg-amber-50 p-4 text-amber-800">
          {error ?? "Sipariş bulunamadı."}
        </div>
        <Link href="/student/orders" className="inline-block text-sm font-medium text-fitliyo-green hover:underline">
          ← Siparişlerime dön
        </Link>
      </div>
    );
  }

  return (
    <div className="mx-auto max-w-2xl space-y-6">
      <Link href="/student/orders" className="inline-block text-sm font-medium text-fitliyo-green hover:underline">
        ← Siparişlerime dön
      </Link>

      <div className="rounded-xl border bg-white p-6 shadow-sm">
        <h1 className="text-xl font-bold text-slate-800">Sipariş #{order.orderNumber}</h1>
        <p className="mt-1 text-slate-500">{order.packageTitle}</p>
        <p className="text-sm text-slate-600">{order.trainerFullName}</p>
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

      <div className="rounded-xl border bg-white p-6 shadow-sm">
        <h2 className="font-semibold text-slate-800">Eğitmene ileteceğin bilgiler</h2>
        <p className="mt-1 text-sm text-slate-500">
          Kan değerleri, hedefler, kronik rahatsızlıklar, diyet kısıtları vb. Eğitmen programını buna göre hazırlar.
        </p>
        <textarea
          value={studentFormText}
          onChange={(e) => setStudentFormText(e.target.value)}
          placeholder="Örn: Kan şekerim yüksek, 3 ay sonra yarış hedefim var..."
          rows={4}
          className="mt-2 w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-800"
        />
        <button
          type="button"
          onClick={handleSubmitStudentForm}
          disabled={submittingForm}
          className="mt-2 rounded-lg bg-fitliyo-green px-4 py-2 text-sm font-medium text-white hover:opacity-90 disabled:opacity-50"
        >
          {submittingForm ? "Gönderiliyor..." : order?.studentFormSubmittedAt ? "Güncelle" : "Eğitmene gönder"}
        </button>
        {order?.studentFormSubmittedAt && (
          <p className="mt-1 text-xs text-slate-500">
            Son gönderim: {formatDate(order.studentFormSubmittedAt)}
          </p>
        )}
      </div>

      {(order?.trainerProgramNotes != null && order.trainerProgramNotes !== "") ||
      (order?.programAttachmentUrl != null && order.programAttachmentUrl !== "") ? (
        <div className="rounded-xl border border-emerald-200 bg-emerald-50 p-6">
          <h2 className="font-semibold text-emerald-900">Eğitmenin program teslimi</h2>
          {order?.programDeliveredAt && (
            <p className="mt-1 text-xs text-emerald-700">
              Teslim tarihi: {formatDate(order.programDeliveredAt)}
            </p>
          )}
          {order?.trainerProgramNotes != null && order.trainerProgramNotes !== "" && (
            <div className="mt-2 whitespace-pre-wrap text-sm text-emerald-800">
              {order.trainerProgramNotes}
            </div>
          )}
          {order?.programAttachmentUrl != null && order.programAttachmentUrl !== "" && (
            <a
              href={order.programAttachmentUrl}
              target="_blank"
              rel="noopener noreferrer"
              className="mt-2 inline-block font-medium text-emerald-700 underline hover:text-emerald-900"
            >
              Program dosyası / link →
            </a>
          )}
        </div>
      ) : null}

      <div className="flex flex-wrap gap-3">
        <button
          type="button"
          onClick={handleMessageAboutOrder}
          disabled={messageOrderLoading}
          className="rounded-lg border border-slate-300 bg-white px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50 disabled:opacity-50"
        >
          {messageOrderLoading ? "Açılıyor..." : "Bu sipariş hakkında eğitmenle yazış"}
        </button>
      </div>

      <div className="rounded-xl border bg-white p-6 shadow-sm">
        <h2 className="font-semibold text-slate-800">Seanslar</h2>
        {sessions.length === 0 &&
        (order.packageSessionCount == null || order.packageSessionCount === 0) ? (
          <div className="mt-2 rounded-lg border border-emerald-100 bg-emerald-50 p-4 text-sm text-emerald-800">
            <p className="font-medium">Program paketi — seans yok</p>
            <p className="mt-1">
              Bu paket bire bir seans içermemektedir. Eğitmenin hazırladığı programı kendi günlük hayatınızda
              uygulayacaksınız.
              {order.packageDurationDays != null && order.packageDurationDays > 0 && (
                <span className="mt-1 block">
                  Program süresi: {order.packageDurationDays} gün
                  {order.packageDurationDays >= 30 &&
                    ` (${Math.round(order.packageDurationDays / 30)} ay)`}.
                </span>
              )}
            </p>
          </div>
        ) : sessions.length === 0 ? (
          <p className="mt-2 text-sm text-slate-500">Henüz seans planlanmamış.</p>
        ) : (
          <ul className="mt-3 space-y-2">
            {sessions.map((s) => (
              <li key={s.id} className="flex flex-wrap items-center justify-between gap-2 rounded-lg border border-slate-100 p-3">
                <div>
                  <p className="text-sm font-medium text-slate-800">Seans #{s.sequenceNumber}</p>
                  <p className="text-xs text-slate-500">{formatDate(s.scheduledStartTime)}</p>
                </div>
                <div className="flex items-center gap-2">
                  <span className="text-xs text-slate-600">{SESSION_STATUS_LABELS[s.status] ?? s.status}</span>
                  {s.meetingUrl && (
                    <a
                      href={s.meetingUrl}
                      target="_blank"
                      rel="noopener noreferrer"
                      className="text-sm font-medium text-fitliyo-green hover:underline"
                    >
                      Bağlantı
                    </a>
                  )}
                </div>
              </li>
            ))}
          </ul>
        )}
      </div>
    </div>
  );
}
