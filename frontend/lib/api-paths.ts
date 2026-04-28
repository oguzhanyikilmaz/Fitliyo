/**
 * Backend path'leri — `docs/openapi/swagger.web.v1.full.json` (kebab-case) ile birebir.
 * Sorgu string'i için `lib/api.buildAbpQuery` kullan (PascalCase parametre isimleri).
 */

const BASE = "/api/app";

export const ApiPaths = {
  UserProfile: {
    myProfile: () => `${BASE}/user-profile/my-profile`,
  },
  Order: {
    getAsync: (id: string) => `${BASE}/order/${id}`,
    getMyOrdersAsync: (query = "") => `${BASE}/order/my-orders${query}`,
    getTrainerOrdersAsync: (query = "") => `${BASE}/order/trainer-orders${query}`,
    getSessionsAsync: (orderId: string) => `${BASE}/order/sessions/${orderId}`,
    createAsync: () => `${BASE}/order`,
    updateStudentFormAsync: (orderId: string) => `${BASE}/order/student-form/${orderId}`,
    updateOrderDeliveryAsync: (orderId: string) => `${BASE}/order/order-delivery/${orderId}`,
  },
  TrainerProfile: {
    getListAsync: (query = "") => `${BASE}/trainer-profile${query}`,
    getAsync: (id: string) => `${BASE}/trainer-profile/${id}`,
    getBySlugAsync: (slug: string) => `${BASE}/trainer-profile/by-slug?slug=${encodeURIComponent(slug)}`,
    getMyProfileAsync: () => `${BASE}/trainer-profile/my-profile`,
    updateAsync: (id: string) => `${BASE}/trainer-profile/${id}`,
  },
  ServicePackage: {
    getListAsync: (query = "") => `${BASE}/service-package${query}`,
    getAsync: (id: string) => `${BASE}/service-package/${id}`,
    createAsync: () => `${BASE}/service-package`,
    updateAsync: (id: string) => `${BASE}/service-package/${id}`,
    deleteAsync: (id: string) => `${BASE}/service-package/${id}`,
  },
  Review: {
    byTrainer: (query = "") => `${BASE}/review/by-trainer${query}`,
  },
  Category: {
    getAsync: (id: string) => `${BASE}/category/${id}`,
    getListAsync: () => `${BASE}/category`,
    getListByParentAsync: (parentId: string) => `${BASE}/category/by-parent/${parentId}`,
    createAsync: () => `${BASE}/category`,
    updateAsync: (id: string) => `${BASE}/category/${id}`,
    deleteAsync: (id: string) => `${BASE}/category/${id}`,
  },
  Notification: {
    getMyNotificationsAsync: (query = "") => `${BASE}/notification/my-notifications${query}`,
    markAsReadAsync: (id: string) => `${BASE}/notification/${id}/mark-as-read`,
    markAllAsReadAsync: () => `${BASE}/notification/mark-all-as-read`,
  },
  SupportTicket: {
    getAsync: (id: string) => `${BASE}/support-ticket/${id}`,
    getMyTicketsAsync: (query = "") => `${BASE}/support-ticket/my-tickets${query}`,
    getListAsync: (query = "") => `${BASE}/support-ticket${query}`,
    createAsync: () => `${BASE}/support-ticket`,
    replyAsync: (id: string) => `${BASE}/support-ticket/${id}/reply`,
    updateStatusAsync: (id: string, status: number) =>
      `${BASE}/support-ticket/${id}/status?status=${encodeURIComponent(String(status))}`,
  },
  Messaging: {
    getMyConversationsAsync: () => `${BASE}/messaging/my-conversations`,
    getMessagesAsync: (conversationId: string, query = "") =>
      `${BASE}/messaging/messages/${conversationId}${query}`,
    sendMessageAsync: () => `${BASE}/messaging/send-message`,
    markAsReadAsync: (conversationId: string) => `${BASE}/messaging/mark-as-read/${conversationId}`,
    getOrCreateConversationForOrderAsync: (orderId: string) =>
      `${BASE}/messaging/or-create-conversation-for-order/${orderId}`,
  },
  Wallet: {
    getMyWalletAsync: () => `${BASE}/wallet/my-wallet`,
    getMyTransactionsAsync: (query = "") => `${BASE}/wallet/my-transactions${query}`,
  },
  WithdrawalRequest: {
    getMyRequestsAsync: (query = "") => `${BASE}/withdrawal-request/my-requests${query}`,
    getListAsync: (query = "") => `${BASE}/withdrawal-request${query}`,
    createAsync: () => `${BASE}/withdrawal-request`,
    approveAsync: (id: string) => `${BASE}/withdrawal-request/${id}/approve`,
    rejectAsync: (id: string) => `${BASE}/withdrawal-request/${id}/reject`,
    markProcessedAsync: (id: string) => `${BASE}/withdrawal-request/${id}/mark-processed`,
  },
  Admin: {
    getDashboardAsync: () => `${BASE}/admin/dashboard`,
  },
  Subscription: {
    getPlansAsync: () => `${BASE}/subscription/plans`,
    createPlanAsync: () => `${BASE}/subscription/plan`,
    updatePlanAsync: (id: string) => `${BASE}/subscription/${id}/plan`,
    deletePlanAsync: (id: string) => `${BASE}/subscription/${id}/plan`,
  },
  Dispute: {
    getAsync: (id: string) => `${BASE}/dispute/${id}`,
    getListAsync: (query = "") => `${BASE}/dispute${query}`,
    resolveAsync: (id: string) => `${BASE}/dispute/${id}/resolve`,
  },
  FeaturedListing: {
    getAsync: (id: string) => `${BASE}/featured-listing/${id}`,
    getListAsync: (query = "") => `${BASE}/featured-listing${query}`,
    createAsync: () => `${BASE}/featured-listing`,
    updateAsync: (id: string) => `${BASE}/featured-listing/${id}`,
    deleteAsync: (id: string) => `${BASE}/featured-listing/${id}`,
  },
  BlogPost: {
    getAsync: (id: string) => `${BASE}/blog-post/${id}`,
    getListAsync: (query = "") => `${BASE}/blog-post${query}`,
    getPublishedListAsync: (query = "") => `${BASE}/blog-post/published-list${query}`,
    createAsync: () => `${BASE}/blog-post`,
    updateAsync: (id: string) => `${BASE}/blog-post/${id}`,
    deleteAsync: (id: string) => `${BASE}/blog-post/${id}`,
    publishAsync: (id: string) => `${BASE}/blog-post/${id}/publish`,
  },
} as const;
