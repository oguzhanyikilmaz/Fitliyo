/**
 * Backend DTO'larına uyumlu TypeScript tipleri.
 * Conventional API path'leri: /api/app/trainerProfile, /api/app/category vb.
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
