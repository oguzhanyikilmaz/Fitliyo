/**
 * Backend DTO'larına uyumlu TypeScript tipleri.
 * API path'leri tek kaynak: lib/api-paths.ts (Swagger ile uyumlu kebab-case: /api/app/order/get-my-orders-async).
 */

/** Sayfalı sonuç (ABP PagedResultDto) */
export interface PagedResultDto<T> {
  items: T[];
  totalCount: number;
}

/** Sayfalı istek (skipCount, maxResultCount, sorting) */
export interface PagedAndSortedRequest {
  skipCount?: number;
  maxResultCount?: number;
  sorting?: string;
}

/** Eğitmen tipi enum (backend TrainerType) */
export type TrainerType = number;

/** Eğitmen profili DTO */
export interface TrainerProfileDto {
  id: string;
  userId: string;
  slug: string;
  bio?: string;
  experienceYears: number;
  trainerType: TrainerType;
  specialtyTags?: string;
  city?: string;
  district?: string;
  isOnlineAvailable: boolean;
  isOnSiteAvailable: boolean;
  averageRating: number;
  totalReviewCount: number;
  totalStudentCount: number;
  isVerified: boolean;
  verificationBadge?: string;
  subscriptionTier?: number;
  profileCompletionPct: number;
  instagramUrl?: string;
  youtubeUrl?: string;
  websiteUrl?: string;
  isActive: boolean;
  trainerFullName?: string;
  profilePhotoUrl?: string;
}

/** Eğitmen listesi filtre isteği (GetTrainerListDto) */
export interface GetTrainerListDto extends PagedAndSortedRequest {
  filter?: string;
  trainerType?: TrainerType;
  city?: string;
  isOnlineAvailable?: boolean;
  isOnSiteAvailable?: boolean;
  isVerified?: boolean;
  categoryId?: string;
}

/** Kategori DTO */
export interface CategoryDto {
  id: string;
  name: string;
  slug?: string;
  parentId?: string;
  displayOrder?: number;
}

/** Öne çıkan liste (FeaturedListing) */
export type FeaturedListingPageType = number; // 0=Homepage, 1=Category, 2=Search
export interface FeaturedListingDto {
  id: string;
  pageType: FeaturedListingPageType;
  trainerProfileId?: string;
  servicePackageId?: string;
  sortOrder: number;
  startDate?: string;
  endDate?: string;
  isActive: boolean;
  adminNote?: string;
}
export interface GetFeaturedListingListDto extends PagedAndSortedRequest {
  pageType?: FeaturedListingPageType;
  isActive?: boolean;
}

/** Sipariş */
export type OrderStatus = number; // 0=Pending, 1=Confirmed, 2=InProgress, 3=Completed, 4=Cancelled
export type PaymentStatus = number; // 0=Pending, 1=Paid, 2=Escrow, 3=Released
export interface OrderDto {
  id: string;
  orderNumber: string;
  studentId: string;
  trainerProfileId: string;
  servicePackageId: string;
  status: OrderStatus;
  paymentStatus: PaymentStatus;
  unitPrice: number;
  quantity: number;
  totalAmount: number;
  discountAmount: number;
  netAmount: number;
  commissionAmount: number;
  trainerPayoutAmount: number;
  currency: string;
  paymentProvider?: string;
  paidAt?: string;
  completedAt?: string;
  cancelledAt?: string;
  cancellationReason?: string;
  creationTime: string;
  trainerFullName?: string;
  packageTitle?: string;
  /** Paketteki seans sayısı (null/0 = seans yok, öğrenci programı kendi uygular) */
  packageSessionCount?: number | null;
  /** Paket süresi (gün), örn. 90 = 3 ay */
  packageDurationDays?: number | null;
  /** Öğrencinin eğitmene ilettiği form (kan değerleri, hedefler vb.) */
  studentFormData?: string | null;
  studentFormSubmittedAt?: string | null;
  trainerProgramNotes?: string | null;
  programDeliveredAt?: string | null;
  programAttachmentUrl?: string | null;
}
export interface GetOrderListDto extends PagedAndSortedRequest {
  status?: OrderStatus;
  paymentStatus?: PaymentStatus;
}
export interface CreateOrderDto {
  servicePackageId: string;
  quantity?: number;
  notes?: string;
}

/** Öğrenci: sipariş için eğitmene ileteceği bilgiler */
export interface UpdateOrderStudentFormDto {
  formData?: string | null;
}

/** Eğitmen: siparişe program teslimi (not + link) */
export interface UpdateOrderDeliveryDto {
  trainerProgramNotes?: string | null;
  programAttachmentUrl?: string | null;
  markAsDelivered?: boolean;
}

/** Seans */
export type SessionStatus = number; // 0=Scheduled, 1=InProgress, 2=Completed, 3=Cancelled
export interface SessionDto {
  id: string;
  orderId: string;
  trainerProfileId: string;
  studentId: string;
  scheduledStartTime: string;
  scheduledEndTime: string;
  actualStartTime?: string;
  actualEndTime?: string;
  status: SessionStatus;
  meetingUrl?: string;
  trainerNotes?: string;
  studentNotes?: string;
  sequenceNumber: number;
}

/** Hizmet paketi */
export type PackageType = number; // 1=SingleSession, 2=Training, 3=Nutrition, 4=Combined
export interface ServicePackageDto {
  id: string;
  trainerProfileId: string;
  packageType: PackageType;
  title: string;
  description?: string;
  price: number;
  discountedPrice?: number;
  currency: string;
  durationDays?: number;
  sessionCount?: number;
  sessionDurationMinutes?: number;
  maxStudents: number;
  isActive: boolean;
  isOnline: boolean;
  isOnSite: boolean;
  cancellationHours: number;
  cancellationPolicy?: string;
  whatIsIncluded?: string;
  whatIsNotIncluded?: string;
  tags?: string;
  totalSalesCount: number;
  averageRating: number;
  isFeatured: boolean;
  trainerFullName?: string;
  trainerSlug?: string;
}
export interface GetPackageListDto extends PagedAndSortedRequest {
  filter?: string;
  trainerProfileId?: string;
  packageType?: PackageType;
  isOnline?: boolean;
  isOnSite?: boolean;
  minPrice?: number;
  maxPrice?: number;
  categoryId?: string;
}

/** Paket oluştur/güncelle (eğitmen) */
export interface CreateUpdateServicePackageDto {
  packageType: PackageType;
  title: string;
  description?: string | null;
  price: number;
  discountedPrice?: number | null;
  currency: string;
  durationDays?: number | null;
  sessionCount?: number | null;
  sessionDurationMinutes?: number | null;
  maxStudents: number;
  isOnline: boolean;
  isOnSite: boolean;
  cancellationHours: number;
  cancellationPolicy?: string | null;
  whatIsIncluded?: string | null;
  whatIsNotIncluded?: string | null;
  tags?: string | null;
}

/** Mesajlaşma */
export interface ListResultDto<T> {
  items: T[];
}
export interface ConversationDto {
  id: string;
  initiatorId: string;
  participantId: string;
  lastMessageAt?: string | null;
  isActive: boolean;
  unreadCount: number;
  otherPartyFullName?: string | null;
  otherPartyProfilePhotoUrl?: string | null;
  lastMessagePreview?: string | null;
  /** Bu konuşma belirli bir siparişe aitse */
  orderId?: string | null;
}
export interface MessageDto {
  id: string;
  conversationId: string;
  senderId: string;
  content: string;
  attachmentUrl?: string | null;
  isRead: boolean;
  readAt?: string | null;
  creationTime: string;
  senderFullName?: string | null;
  isMine: boolean;
}
export interface SendMessageDto {
  recipientId: string;
  content: string;
  attachmentUrl?: string | null;
}

/** Eğitmen profili oluştur/güncelle (bio, şehir, linkler vb.) */
export interface CreateUpdateTrainerProfileDto {
  slug: string;
  bio?: string | null;
  experienceYears: number;
  trainerType: TrainerType;
  specialtyTags?: string | null;
  city?: string | null;
  district?: string | null;
  isOnlineAvailable: boolean;
  isOnSiteAvailable: boolean;
  instagramUrl?: string | null;
  youtubeUrl?: string | null;
  websiteUrl?: string | null;
}

/** Kullanıcı profili / sağlık bilgisi (öğrenci ve eğitmen ortak) */
export const Gender = { NotSpecified: 0, Male: 1, Female: 2, Other: 3 } as const;
export const ActivityLevel = {
  NotSpecified: 0,
  Sedentary: 1,
  Light: 2,
  Moderate: 3,
  Active: 4,
  VeryActive: 5,
} as const;
export const FitnessGoal = {
  NotSpecified: 0,
  LoseWeight: 1,
  GainMuscle: 2,
  Maintain: 3,
  GeneralFitness: 4,
  Performance: 5,
} as const;

export interface UserProfileDto {
  id?: string;
  userId: string;
  birthDate?: string | null;
  gender: number;
  heightCm?: number | null;
  weightKg?: number | null;
  bloodType?: string | null;
  activityLevel: number;
  fitnessGoal: number;
  chronicConditions?: string | null;
  allergies?: string | null;
  medications?: string | null;
  injuries?: string | null;
  emergencyContact?: string | null;
  phone?: string | null;
  notes?: string | null;
  waistCm?: number | null;
  hipCm?: number | null;
  neckCm?: number | null;
  targetWeightKg?: number | null;
  sleepHoursPerNight?: number | null;
  smoking?: boolean | null;
  alcoholConsumption?: string | null;
  restingHeartRate?: number | null;
  /** Hesaplanan: yaş */
  age?: number | null;
  /** Hesaplanan: BMI */
  bmi?: number | null;
  /** Hesaplanan: bazal metabolizma (kcal/gün) */
  bmr?: number | null;
  /** Hesaplanan: günlük kalori ihtiyacı (kcal/gün) */
  tdee?: number | null;
  /** Hesaplanan: ideal min kilo (BMI 18.5) */
  idealWeightMinKg?: number | null;
  /** Hesaplanan: ideal max kilo (BMI 24.9) */
  idealWeightMaxKg?: number | null;
  /** Hesaplanan: vücut yağ % (Navy) */
  bodyFatPercentage?: number | null;
}

export interface CreateUpdateUserProfileDto {
  birthDate?: string | null;
  gender: number;
  heightCm?: number | null;
  weightKg?: number | null;
  bloodType?: string | null;
  activityLevel: number;
  fitnessGoal: number;
  chronicConditions?: string | null;
  allergies?: string | null;
  medications?: string | null;
  injuries?: string | null;
  emergencyContact?: string | null;
  phone?: string | null;
  notes?: string | null;
  waistCm?: number | null;
  hipCm?: number | null;
  neckCm?: number | null;
  targetWeightKg?: number | null;
  sleepHoursPerNight?: number | null;
  smoking?: boolean | null;
  alcoholConsumption?: string | null;
  restingHeartRate?: number | null;
}

/** Bildirim */
export interface NotificationDto {
  id: string;
  userId: string;
  notificationType: number;
  channel: number;
  title: string;
  body?: string | null;
  actionUrl?: string | null;
  isRead: boolean;
  readAt?: string | null;
  creationTime: string;
}
export interface GetNotificationListDto extends PagedAndSortedRequest {
  isRead?: boolean;
  notificationType?: number;
}

/** Destek talebi */
export type SupportTicketCategory = number; // 0=General, 1=Payment, 2=Order, 3=Technical, 4=Account
export type SupportTicketStatus = number; // 0=Open, 1=InProgress, 2=WaitingCustomer, 3=Resolved, 4=Closed
export type TicketPriority = number; // 0=Low, 1=Medium, 2=High, 3=Urgent
export interface SupportTicketDto {
  id: string;
  subject: string;
  message: string;
  userId?: string | null;
  orderId?: string | null;
  category: SupportTicketCategory;
  status: SupportTicketStatus;
  priority: TicketPriority;
  adminReply?: string | null;
  adminReplyDate?: string | null;
  assignedToUserId?: string | null;
  creationTime: string;
}
export interface GetSupportTicketListDto extends PagedAndSortedRequest {
  status?: SupportTicketStatus;
  category?: SupportTicketCategory;
}
export interface CreateSupportTicketDto {
  subject: string;
  message: string;
  category: SupportTicketCategory;
  orderId?: string | null;
}
export interface ReplySupportTicketDto {
  adminReply: string;
}

/** Cüzdan */
export interface TrainerWalletDto {
  id: string;
  trainerProfileId: string;
  availableBalance: number;
  pendingBalance: number;
  totalEarned: number;
  totalWithdrawn: number;
  lastPayoutAt?: string | null;
}
export type WalletTransactionType = number; // 0=Credit, 1=Debit, 2=Refund, 3=Payout
export interface WalletTransactionDto {
  id: string;
  trainerWalletId: string;
  transactionType: WalletTransactionType;
  amount: number;
  description: string;
  referenceId?: string | null;
  balanceAfter: number;
  creationTime: string;
}
export interface GetWalletTransactionListDto extends PagedAndSortedRequest {}

/** Para çekme talebi */
export type WithdrawalRequestStatus = number; // 0=Pending, 1=Approved, 2=Rejected, 3=Processed
export interface WithdrawalRequestDto {
  id: string;
  trainerWalletId: string;
  amount: number;
  status: WithdrawalRequestStatus;
  iban: string;
  accountHolderName: string;
  adminNote?: string | null;
  processedAt?: string | null;
  creationTime: string;
}
export interface GetWithdrawalRequestListDto extends PagedAndSortedRequest {
  status?: WithdrawalRequestStatus;
}
export interface CreateWithdrawalRequestDto {
  amount: number;
  iban: string;
  accountHolderName: string;
}

/** Admin dashboard */
export interface DashboardDto {
  totalTrainers: number;
  activeTrainers: number;
  verifiedTrainers: number;
  totalStudents: number;
  totalOrders: number;
  pendingOrders: number;
  completedOrders: number;
  totalRevenue: number;
  totalCommission: number;
  totalReviews: number;
  averagePlatformRating: number;
  totalActiveSubscriptions: number;
  totalPackages: number;
  totalCategories: number;
}

/** Uyuşmazlık */
export type DisputeType = number; // 0=Refund, 1=ServiceNotProvided, 2=Cancellation, 3=Other
export type DisputeStatus = number; // 0=Open, 1=UnderReview, 2=Resolved, 3=Closed
export interface DisputeDto {
  id: string;
  orderId: string;
  openedByUserId: string;
  disputeType: DisputeType;
  description: string;
  status: DisputeStatus;
  resolutionNote?: string | null;
  resolvedByUserId?: string | null;
  resolvedAt?: string | null;
  creationTime: string;
}
export interface GetDisputeListDto extends PagedAndSortedRequest {
  status?: DisputeStatus;
}
export interface ResolveDisputeDto {
  resolutionNote: string;
}
