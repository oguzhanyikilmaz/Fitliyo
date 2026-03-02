"use client";

import { useEffect, useState, useRef } from "react";
import { useSearchParams } from "next/navigation";
import { apiFetch, buildQuery } from "@/lib/api";
import { ApiPaths } from "@/lib/api-paths";
import { getStoredUser } from "@/lib/auth";
import type {
  ListResultDto,
  ConversationDto,
  PagedResultDto,
  MessageDto,
  SendMessageDto,
} from "@/lib/types";

function formatDate(s: string) {
  try {
    return new Date(s).toLocaleString("tr-TR", {
      day: "2-digit",
      month: "short",
      hour: "2-digit",
      minute: "2-digit",
    });
  } catch {
    return s;
  }
}

export default function TrainerMessagesPage() {
  const searchParams = useSearchParams();
  const user = getStoredUser();
  const currentUserId = user?.id ?? "";
  const [conversations, setConversations] = useState<ConversationDto[]>([]);
  const [selected, setSelected] = useState<ConversationDto | null>(null);
  const [messages, setMessages] = useState<MessageDto[]>([]);
  const [loadingConv, setLoadingConv] = useState(true);
  const [loadingMsg, setLoadingMsg] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [replyText, setReplyText] = useState("");
  const [sending, setSending] = useState(false);
  const messagesEndRef = useRef<HTMLDivElement>(null);

  const otherPartyId = selected
    ? currentUserId === selected.initiatorId
      ? selected.participantId
      : selected.initiatorId
    : "";

  useEffect(() => {
    apiFetch<ListResultDto<ConversationDto>>(ApiPaths.Messaging.getMyConversationsAsync())
      .then((res) => setConversations(res.items ?? []))
      .catch((e) => setError(e instanceof Error ? e.message : "Konuşmalar yüklenemedi"))
      .finally(() => setLoadingConv(false));
  }, []);

  const conversationIdFromUrl = searchParams.get("conversationId");
  useEffect(() => {
    if (!conversationIdFromUrl || conversations.length === 0) return;
    const conv = conversations.find((c) => c.id === conversationIdFromUrl);
    if (conv) setSelected(conv);
  }, [conversationIdFromUrl, conversations]);

  useEffect(() => {
    if (!selected) {
      setMessages([]);
      return;
    }
    setLoadingMsg(true);
    const query = buildQuery({
      skipCount: 0,
      maxResultCount: 100,
    });
    apiFetch<PagedResultDto<MessageDto>>(
      ApiPaths.Messaging.getMessagesAsync(selected.id, query)
    )
      .then((res) => {
        const list = res.items ?? [];
        list.sort(
          (a, b) => new Date(a.creationTime).getTime() - new Date(b.creationTime).getTime()
        );
        setMessages(list);
      })
      .catch(() => setMessages([]))
      .finally(() => setLoadingMsg(false));
  }, [selected]);

  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages]);

  const handleSend = () => {
    if (!replyText.trim() || !otherPartyId || sending) return;
    setSending(true);
    const body: SendMessageDto = { recipientId: otherPartyId, content: replyText.trim() };
    apiFetch<MessageDto>(ApiPaths.Messaging.sendMessageAsync(), {
      method: "POST",
      body: JSON.stringify(body),
    })
      .then((msg) => {
        setReplyText("");
        setMessages((prev) => [...prev, { ...msg, isMine: true }]);
      })
      .catch(() => setSending(false))
      .finally(() => setSending(false));
  };

  const handleMarkRead = () => {
    if (selected)
      apiFetch<void>(ApiPaths.Messaging.markAsReadAsync(selected.id), {
        method: "POST",
      }).catch(() => {});
  };

  useEffect(() => {
    if (selected) handleMarkRead();
  }, [selected?.id]);

  if (loadingConv) {
    return (
      <div className="flex min-h-[40vh] items-center justify-center">
        <p className="text-slate-600">Konuşmalar yükleniyor...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="space-y-4">
        <h1 className="text-2xl font-bold text-slate-800">Mesajlar</h1>
        <div className="rounded-xl border border-amber-200 bg-amber-50 p-4 text-amber-800">{error}</div>
      </div>
    );
  }

  return (
    <div className="flex h-[calc(100vh-8rem)] gap-4 overflow-hidden rounded-xl border border-slate-200 bg-white shadow-sm">
      <aside className="w-72 shrink-0 border-r border-slate-100 p-2">
        <h2 className="p-2 text-sm font-semibold text-slate-800">Konuşmalar</h2>
        {conversations.length === 0 ? (
          <p className="p-4 text-sm text-slate-500">Henüz konuşma yok.</p>
        ) : (
          <ul className="space-y-1">
            {conversations.map((c) => (
              <li key={c.id}>
                <button
                  type="button"
                  onClick={() => setSelected(c)}
                  className={`w-full rounded-lg p-3 text-left ${selected?.id === c.id ? "bg-fitliyo-green/10" : "hover:bg-slate-50"}`}
                >
                  <p className="font-medium text-slate-800">{c.otherPartyFullName ?? "Kullanıcı"}</p>
                  <p className="truncate text-xs text-slate-500">{c.lastMessagePreview ?? "—"}</p>
                  {c.orderId && (
                    <span className="mt-1 inline-block text-xs text-slate-400">Siparişe özel</span>
                  )}
                </button>
              </li>
            ))}
          </ul>
        )}
      </aside>
      <div className="flex min-w-0 flex-1 flex-col">
        {selected ? (
          <>
            <div className="border-b border-slate-100 p-3">
              <p className="font-medium text-slate-800">{selected.otherPartyFullName ?? "Kullanıcı"}</p>
            </div>
            <div className="min-h-0 flex-1 overflow-y-auto space-y-3 p-4">
              {loadingMsg ? (
                <p className="text-sm text-slate-500">Mesajlar yükleniyor...</p>
              ) : (
                messages.map((m) => (
                  <div
                    key={m.id}
                    className={`flex ${m.isMine ? "justify-end" : "justify-start"}`}
                  >
                    <div
                      className={`max-w-[80%] rounded-lg px-3 py-2 ${
                        m.isMine ? "bg-fitliyo-green text-white" : "bg-slate-100 text-slate-800"
                      }`}
                    >
                      <p className="text-sm">{m.content}</p>
                      <p className={`mt-1 text-xs ${m.isMine ? "text-white/80" : "text-slate-500"}`}>
                        {formatDate(m.creationTime)}
                      </p>
                    </div>
                  </div>
                ))
              )}
              <div ref={messagesEndRef} />
            </div>
            <div className="border-t border-slate-100 p-3">
              <div className="flex gap-2">
                <input
                  type="text"
                  value={replyText}
                  onChange={(e) => setReplyText(e.target.value)}
                  onKeyDown={(e) => e.key === "Enter" && !e.shiftKey && handleSend()}
                  placeholder="Mesaj yazın..."
                  className="min-w-0 flex-1 rounded-lg border border-slate-300 px-3 py-2 text-slate-800"
                />
                <button
                  type="button"
                  onClick={handleSend}
                  disabled={!replyText.trim() || sending}
                  className="rounded-lg bg-fitliyo-green px-4 py-2 text-sm font-medium text-white hover:bg-fitliyo-green/90 disabled:opacity-50"
                >
                  {sending ? "..." : "Gönder"}
                </button>
              </div>
            </div>
          </>
        ) : (
          <div className="flex flex-1 items-center justify-center text-slate-500">
            Bir konuşma seçin veya sipariş detayından &quot;Bu sipariş hakkında yazış&quot; ile başlayın.
          </div>
        )}
      </div>
    </div>
  );
}
