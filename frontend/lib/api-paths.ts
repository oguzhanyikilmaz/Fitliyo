/**
 * Backend API path'leri — tek kaynak.
 * ABP conventional controller bu projede PascalCase route kullanıyor: /api/app/UserProfile/GetMyProfileAsync
 * Tüm apiFetch çağrıları bu sabitleri kullanmalı.
 */

const BASE = "/api/app";

/** Controller ve action ile path: /api/app/{Controller}/{Action} (PascalCase — backend ile aynı) */
export function apiPath(controller: string, action: string, query?: string): string {
  const path = `${BASE}/${controller}/${action}`;
  return query ? `${path}${query.startsWith("?") ? query : `?${query}`}` : path;
}

/** Sık kullanılan endpoint'ler — isimlendirme Swagger'daki path ile aynı */
export const ApiPaths = {
  // UserProfile — metod adları C# ile aynı (GetMyProfileAsync, UpdateMyProfileAsync)
  UserProfile: {
    getMyProfile: () => apiPath("UserProfile", "GetMyProfileAsync"),
    updateMyProfile: () => apiPath("UserProfile", "UpdateMyProfileAsync"),
  },
  // Order
  Order: {
    getAsync: (id: string) => apiPath("Order", "GetAsync", `id=${id}`),
    getMyOrdersAsync: (query?: string) => apiPath("Order", "GetMyOrdersAsync") + (query ?? ""),
    getTrainerOrdersAsync: (query?: string) => apiPath("Order", "GetTrainerOrdersAsync") + (query ?? ""),
    getSessionsAsync: (orderId: string) => apiPath("Order", "GetSessionsAsync", `orderId=${orderId}`),
    createAsync: () => apiPath("Order", "CreateAsync"),
    updateStudentFormAsync: (id: string) => apiPath("Order", "UpdateStudentFormAsync", `id=${id}`),
    updateOrderDeliveryAsync: (id: string) => apiPath("Order", "UpdateOrderDeliveryAsync", `id=${id}`),
  },
  // TrainerProfile
  TrainerProfile: {
    getListAsync: (query?: string) => apiPath("TrainerProfile", "GetListAsync") + (query ?? ""),
    getAsync: (id: string) => apiPath("TrainerProfile", "GetAsync", `id=${id}`),
    getBySlugAsync: (slug: string) => apiPath("TrainerProfile", "GetBySlugAsync", `slug=${encodeURIComponent(slug)}`),
    getMyProfileAsync: () => apiPath("TrainerProfile", "GetMyProfileAsync"),
    updateAsync: (id: string) => apiPath("TrainerProfile", "UpdateAsync", `id=${id}`),
  },
  // ServicePackage
  ServicePackage: {
    getListAsync: (query?: string) => apiPath("ServicePackage", "GetListAsync") + (query ?? ""),
    getAsync: (id: string) => apiPath("ServicePackage", "GetAsync", `id=${id}`),
    createAsync: () => apiPath("ServicePackage", "CreateAsync"),
    updateAsync: (id: string) => apiPath("ServicePackage", "UpdateAsync", `id=${id}`),
    deleteAsync: (id: string) => apiPath("ServicePackage", "DeleteAsync", `id=${id}`),
  },
  // Notification
  Notification: {
    getMyNotificationsAsync: (query?: string) => apiPath("Notification", "GetMyNotificationsAsync") + (query ?? ""),
    markAsReadAsync: (id: string) => apiPath("Notification", "MarkAsReadAsync", `id=${id}`),
    markAllAsReadAsync: () => apiPath("Notification", "MarkAllAsReadAsync"),
  },
  // SupportTicket
  SupportTicket: {
    getMyTicketsAsync: (query?: string) => apiPath("SupportTicket", "GetMyTicketsAsync") + (query ?? ""),
    getListAsync: (query?: string) => apiPath("SupportTicket", "GetListAsync") + (query ?? ""),
    createAsync: () => apiPath("SupportTicket", "CreateAsync"),
    replyAsync: (id: string) => apiPath("SupportTicket", "ReplyAsync", `id=${id}`),
  },
  // Messaging
  Messaging: {
    getMyConversationsAsync: () => apiPath("Messaging", "GetMyConversationsAsync"),
    getMessagesAsync: (conversationId: string, query?: string) =>
      apiPath("Messaging", "GetMessagesAsync", `conversationId=${conversationId}`) +
      (query ? (query.startsWith("?") ? `&${query.slice(1)}` : `&${query}`) : ""),
    sendMessageAsync: () => apiPath("Messaging", "SendMessageAsync"),
    markAsReadAsync: (conversationId: string) => apiPath("Messaging", "MarkAsReadAsync", `conversationId=${conversationId}`),
    getOrCreateConversationForOrderAsync: (orderId: string) =>
      apiPath("Messaging", "GetOrCreateConversationForOrderAsync", `orderId=${orderId}`),
  },
  // Wallet
  Wallet: {
    getMyWalletAsync: () => apiPath("Wallet", "GetMyWalletAsync"),
    getMyTransactionsAsync: (query?: string) => apiPath("Wallet", "GetMyTransactionsAsync") + (query ?? ""),
  },
  // WithdrawalRequest
  WithdrawalRequest: {
    getMyRequestsAsync: (query?: string) => apiPath("WithdrawalRequest", "GetMyRequestsAsync") + (query ?? ""),
    getListAsync: (query?: string) => apiPath("WithdrawalRequest", "GetListAsync") + (query ?? ""),
    createAsync: () => apiPath("WithdrawalRequest", "CreateAsync"),
    approveAsync: (id: string) => apiPath("WithdrawalRequest", "ApproveAsync", `id=${id}`),
    rejectAsync: (id: string) => apiPath("WithdrawalRequest", "RejectAsync", `id=${id}`),
    markProcessedAsync: (id: string) => apiPath("WithdrawalRequest", "MarkProcessedAsync", `id=${id}`),
  },
  // Admin
  Admin: {
    getDashboardAsync: () => apiPath("Admin", "GetDashboardAsync"),
  },
  // Dispute
  Dispute: {
    getListAsync: (query?: string) => apiPath("Dispute", "GetListAsync") + (query ?? ""),
    resolveAsync: (id: string) => apiPath("Dispute", "ResolveAsync", `id=${id}`),
  },
  // FeaturedListing
  FeaturedListing: {
    getListAsync: (query?: string) => apiPath("FeaturedListing", "GetListAsync") + (query ?? ""),
  },
} as const;
