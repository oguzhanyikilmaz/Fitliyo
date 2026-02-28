# Fitliyo Veritabanı Şeması

**Veritabanı:** PostgreSQL 16
**ORM:** Entity Framework Core (ABP Framework)
**Base Class:** `FullAuditedAggregateRoot<Guid>` — Tüm tablolarda CreationTime, CreatorId, LastModificationTime, DeletionTime, IsDeleted otomatik.
**Son Güncelleme:** 2026-02-28

---

## Kullanıcı Tabloları

### AppUser (ABP Identity extend)
| Alan | Tip | Kısıtlama | Açıklama |
|------|-----|-----------|----------|
| Id | Guid | PK | |
| UserName | string(256) | unique | |
| Email | string(256) | unique | |
| PhoneNumber | string(20) | | |
| FirstName | string(64) | | |
| LastName | string(64) | | |
| ProfilePhotoUrl | string(512) | | |
| UserType | enum | | Trainer, Student, Admin |
| IsActive | bool | | |
| LastLoginAt | DateTime? | | |
| LanguagePreference | string(10) | | |
| NotificationPreferences | JSON | | |

### TrainerProfiles
| Alan | Tip | Kısıtlama | Açıklama |
|------|-----|-----------|----------|
| Id | Guid | PK | |
| UserId | Guid | FK→AppUser, 1-1 | |
| Slug | string(100) | unique | SEO-friendly URL |
| Bio | string(2000) | | |
| ExperienceYears | int | | |
| TrainerType | enum | | PersonalTrainer, Dietitian, BasketballCoach, FootballCoach, TennisCoach, SwimmingCoach, YogaInstructor, Other |
| SpecialtyTags | JSON | | |
| City | string(100) | | |
| District | string(100) | | |
| IsOnlineAvailable | bool | | |
| IsOnSiteAvailable | bool | | |
| AverageRating | decimal(3,2) | | |
| TotalReviewCount | int | | |
| TotalStudentCount | int | | |
| IsVerified | bool | | |
| VerificationBadge | string(50) | | |
| SubscriptionTier | enum | | Free, Basic, Pro |
| SubscriptionExpiry | DateTime? | | |
| ProfileCompletionPct | int | 0-100 | |
| InstagramUrl | string(256) | | |
| YoutubeUrl | string(256) | | |
| WebsiteUrl | string(256) | | |
| BankAccountInfo | JSON | AES-256 şifreli | |

### TrainerCertificates
| Alan | Tip | Kısıtlama | Açıklama |
|------|-----|-----------|----------|
| Id | Guid | PK | |
| TrainerProfileId | Guid | FK | |
| CertificateName | string(256) | | |
| IssuingOrganization | string(256) | | |
| IssueDate | Date | | |
| ExpiryDate | Date? | | |
| DocumentUrl | string(512) | | |
| IsVerifiedByPlatform | bool | | |

### TrainerGallery
| Alan | Tip | Kısıtlama | Açıklama |
|------|-----|-----------|----------|
| Id | Guid | PK | |
| TrainerProfileId | Guid | FK | |
| MediaType | enum | | Image, Video |
| MediaUrl | string(512) | | |
| ThumbnailUrl | string(512) | | |
| Caption | string(256) | | |
| SortOrder | int | | |
| IsCoverImage | bool | | |

---

## Paket Tabloları

### ServicePackages
| Alan | Tip | Kısıtlama | Açıklama |
|------|-----|-----------|----------|
| Id | Guid | PK | |
| TrainerProfileId | Guid | FK | |
| PackageType | enum | | SingleSession, Training, Nutrition, Combined, GroupSession |
| Title | string(256) | | |
| Description | string(2000) | | |
| Price | decimal(10,2) | | |
| DiscountedPrice | decimal(10,2)? | | |
| Currency | string(3) | default TRY | |
| DurationDays | int? | | |
| SessionCount | int? | | |
| SessionDurationMinutes | int? | | |
| MaxStudents | int | tekli ders=1 | |
| IsActive | bool | | |
| IsOnline | bool | | |
| IsOnSite | bool | | |
| CancellationHours | int | | |
| CancellationPolicy | string(1000) | | |
| WhatIsIncluded | JSON | | |
| WhatIsNotIncluded | JSON | | |
| Tags | JSON | | |
| TotalSalesCount | int | | |
| AverageRating | decimal(3,2) | | |
| IsFeatured | bool | | |

### PackageAvailabilitySchedule
| Alan | Tip | Kısıtlama | Açıklama |
|------|-----|-----------|----------|
| Id | Guid | PK | |
| ServicePackageId | Guid | FK | |
| DayOfWeek | enum | | Monday..Sunday |
| StartTime | TimeSpan | | |
| EndTime | TimeSpan | | |
| IsAvailable | bool | | |
| SlotDurationMinutes | int | | |

### PackageUnavailableDates
| Alan | Tip | Kısıtlama | Açıklama |
|------|-----|-----------|----------|
| Id | Guid | PK | |
| TrainerProfileId | Guid | FK | |
| UnavailableDate | Date | | |
| Reason | string(256)? | | |

---

## Sipariş Tabloları

### Orders
| Alan | Tip | Kısıtlama | Açıklama |
|------|-----|-----------|----------|
| Id | Guid | PK | |
| OrderNumber | string(20) | unique | ORD-20250001 formatı |
| StudentUserId | Guid | FK→AppUser | |
| TrainerProfileId | Guid | FK | |
| ServicePackageId | Guid | FK | |
| Status | enum | | Pending, Confirmed, Active, Completed, Cancelled, Refunded |
| TotalAmount | decimal(10,2) | | |
| PlatformCommissionAmount | decimal(10,2) | | |
| TrainerEarningAmount | decimal(10,2) | | |
| CommissionRate | decimal(5,2) | | |
| PaymentId | Guid | FK→Payments | |
| StartDate | Date? | | |
| EndDate | Date? | | |
| RemainingSessionCount | int? | | |
| Notes | string(500)? | | |
| CancellationReason | string(500)? | | |
| CancelledAt | DateTime? | | |
| CompletedAt | DateTime? | | |

### Sessions
| Alan | Tip | Kısıtlama | Açıklama |
|------|-----|-----------|----------|
| Id | Guid | PK | |
| OrderId | Guid | FK | |
| SessionNumber | int | | |
| ScheduledAt | DateTime | | |
| ActualStartAt | DateTime? | | |
| ActualEndAt | DateTime? | | |
| Status | enum | | Scheduled, Completed, NoShow, Cancelled, Rescheduled |
| Location | string(256)? | | |
| MeetingLink | string(512)? | | |
| TrainerNotes | string(1000)? | | |
| StudentNotes | string(1000)? | | |
| RescheduledFrom | Guid? | | |

---

## Ödeme Tabloları

### Payments
| Alan | Tip | Kısıtlama | Açıklama |
|------|-----|-----------|----------|
| Id | Guid | PK | |
| OrderId | Guid | FK | |
| PaymentProvider | enum | | Stripe, iyzico, Manual |
| ProviderPaymentId | string(256) | | |
| Amount | decimal(10,2) | | |
| Currency | string(3) | | |
| Status | enum | | Pending, Completed, Failed, Refunded, PartialRefund |
| PaymentMethod | enum | | CreditCard, BankTransfer, Wallet |
| CardLastFour | string(4)? | | |
| ReceiptUrl | string(512)? | | |
| RefundAmount | decimal(10,2)? | | |
| RefundedAt | DateTime? | | |
| ProviderResponse | JSON | | |

### TrainerWallet
| Alan | Tip | Kısıtlama | Açıklama |
|------|-----|-----------|----------|
| Id | Guid | PK | |
| TrainerProfileId | Guid | FK, 1-1 | |
| AvailableBalance | decimal(10,2) | | |
| PendingBalance | decimal(10,2) | | |
| TotalEarned | decimal(10,2) | | |
| TotalWithdrawn | decimal(10,2) | | |
| LastPayoutAt | DateTime? | | |

### WalletTransactions
| Alan | Tip | Kısıtlama | Açıklama |
|------|-----|-----------|----------|
| Id | Guid | PK | |
| TrainerWalletId | Guid | FK | |
| TransactionType | enum | | Credit, Debit, Refund, Payout |
| Amount | decimal(10,2) | | |
| Description | string(256) | | |
| ReferenceId | Guid? | | |
| BalanceAfter | decimal(10,2) | | |

### WithdrawalRequests
| Alan | Tip | Kısıtlama | Açıklama |
|------|-----|-----------|----------|
| Id | Guid | PK | |
| TrainerWalletId | Guid | FK | |
| Amount | decimal(10,2) | | |
| Status | enum | | Pending, Approved, Rejected, Processed |
| IBAN | string(34) | | |
| AccountHolderName | string(256) | | |
| AdminNote | string(500)? | | |
| ProcessedAt | DateTime? | | |

---

## Abonelik Tabloları

### SubscriptionPlans
| Alan | Tip | Kısıtlama | Açıklama |
|------|-----|-----------|----------|
| Id | Guid | PK | |
| Name | string(100) | | |
| TargetUserType | enum | | Trainer, Student |
| MonthlyPrice | decimal(10,2) | | |
| AnnualPrice | decimal(10,2) | | |
| Features | JSON | | |
| MaxPackagesAllowed | int? | null=sınırsız | |
| CommissionRate | decimal(5,2) | | |
| IsActive | bool | | |
| SortOrder | int | | |
| HighlightBadge | string(50)? | | |

### UserSubscriptions
| Alan | Tip | Kısıtlama | Açıklama |
|------|-----|-----------|----------|
| Id | Guid | PK | |
| UserId | Guid | FK | |
| SubscriptionPlanId | Guid | FK | |
| Status | enum | | Active, Expired, Cancelled, PastDue |
| BillingCycle | enum | | Monthly, Annual |
| StartDate | DateTime | | |
| EndDate | DateTime | | |
| AutoRenew | bool | | |
| PaymentId | Guid? | FK | |
| CancelledAt | DateTime? | | |
| CancellationReason | string(256)? | | |

---

## Değerlendirme Tabloları

### Reviews
| Alan | Tip | Kısıtlama | Açıklama |
|------|-----|-----------|----------|
| Id | Guid | PK | |
| OrderId | Guid | FK, unique | Sipariş başına 1 yorum |
| ReviewerUserId | Guid | FK | |
| TrainerProfileId | Guid | FK | |
| ServicePackageId | Guid | FK | |
| OverallRating | decimal(3,2) | | |
| CommunicationRating | int | 1-5 | |
| ExpertiseRating | int | 1-5 | |
| ValueForMoneyRating | int | 1-5 | |
| PunctualityRating | int | 1-5 | |
| Comment | string(2000)? | | |
| TrainerReply | string(1000)? | | |
| TrainerRepliedAt | DateTime? | | |
| IsVerifiedPurchase | bool | | |
| IsPublished | bool | | |
| HelpfulCount | int | | |

### ReviewHelpfulVotes
| Alan | Tip | Kısıtlama | Açıklama |
|------|-----|-----------|----------|
| Id | Guid | PK | |
| ReviewId | Guid | FK | |
| VoterUserId | Guid | FK | |
| IsHelpful | bool | | |

---

## Mesajlaşma Tabloları

### Conversations
| Alan | Tip | Kısıtlama | Açıklama |
|------|-----|-----------|----------|
| Id | Guid | PK | |
| StudentUserId | Guid | FK | |
| TrainerProfileId | Guid | FK | |
| RelatedOrderId | Guid? | FK | |
| LastMessageAt | DateTime | | |
| StudentUnreadCount | int | | |
| TrainerUnreadCount | int | | |
| IsArchivedByStudent | bool | | |
| IsArchivedByTrainer | bool | | |
| IsBlockedByTrainer | bool | | |

### Messages
| Alan | Tip | Kısıtlama | Açıklama |
|------|-----|-----------|----------|
| Id | Guid | PK | |
| ConversationId | Guid | FK | |
| SenderUserId | Guid | FK | |
| MessageType | enum | | Text, File, Image, Video, System |
| Content | string(4000)? | | |
| FileUrl | string(512)? | | |
| FileName | string(256)? | | |
| FileSizeBytes | long? | | |
| IsRead | bool | | |
| ReadAt | DateTime? | | |
| IsDeleted | bool | | |

---

## Bildirim Tabloları

### Notifications
| Alan | Tip | Kısıtlama | Açıklama |
|------|-----|-----------|----------|
| Id | Guid | PK | |
| UserId | Guid | FK | |
| NotificationType | enum | | NewMessage, NewOrder, SessionReminder, ReviewReceived, PaymentReceived, SystemAlert, SubscriptionRenewing, WithdrawalProcessed |
| Title | string(256) | | |
| Body | string(1000) | | |
| ActionUrl | string(512)? | | |
| IsRead | bool | | |
| ReadAt | DateTime? | | |
| Channel | enum | | InApp, Email, Push |
| ReferenceId | Guid? | | |
| ReferenceType | string(50)? | | |

---

## İçerik Tabloları

### Categories
| Alan | Tip | Kısıtlama | Açıklama |
|------|-----|-----------|----------|
| Id | Guid | PK | |
| Name | string(100) | | |
| Slug | string(100) | unique | |
| ParentId | Guid? | FK self | Hiyerarşik |
| IconUrl | string(256)? | | |
| Description | string(500)? | | |
| SortOrder | int | | |
| IsActive | bool | | |

### TrainerCategoryMappings
| Alan | Tip | Kısıtlama | Açıklama |
|------|-----|-----------|----------|
| TrainerProfileId | Guid | PK, FK | |
| CategoryId | Guid | PK, FK | |

### BlogPosts
| Alan | Tip | Kısıtlama | Açıklama |
|------|-----|-----------|----------|
| Id | Guid | PK | |
| AuthorTrainerProfileId | Guid? | FK | |
| Title | string(512) | | |
| Slug | string(512) | unique | |
| Content | text | | |
| CoverImageUrl | string(512)? | | |
| Status | enum | | Draft, Published, Archived |
| PublishedAt | DateTime? | | |
| ViewCount | int | | |
| MetaDescription | string(300)? | | |
| Tags | JSON | | |

---

## Yönetim Tabloları

### SupportTickets
| Alan | Tip | Kısıtlama | Açıklama |
|------|-----|-----------|----------|
| Id | Guid | PK | |
| SubmitterUserId | Guid | FK | |
| Category | enum | | Payment, Technical, Complaint, Other |
| Subject | string(256) | | |
| Description | string(2000) | | |
| Status | enum | | Open, InProgress, Resolved, Closed |
| Priority | enum | | Low, Medium, High, Critical |
| AssignedAdminId | Guid? | FK | |
| ResolvedAt | DateTime? | | |
| RelatedOrderId | Guid? | FK | |

### FeaturedListings
| Alan | Tip | Kısıtlama | Açıklama |
|------|-----|-----------|----------|
| Id | Guid | PK | |
| TrainerProfileId | Guid | FK | |
| ServicePackageId | Guid? | FK | |
| StartDate | DateTime | | |
| EndDate | DateTime | | |
| Position | int | | |
| PageType | enum | | Homepage, CategoryPage, SearchResult |
| PaidAmount | decimal(10,2) | | |

### Disputes
| Alan | Tip | Kısıtlama | Açıklama |
|------|-----|-----------|----------|
| Id | Guid | PK | |
| OrderId | Guid | FK | |
| InitiatedByUserId | Guid | FK | |
| DisputeType | enum | | NoShow, QualityIssue, PaymentIssue, Other |
| Description | string(2000) | | |
| Status | enum | | Open, UnderReview, ResolvedForStudent, ResolvedForTrainer, Escalated |
| AdminDecision | string(1000)? | | |
| ResolvedAt | DateTime? | | |
| RefundAmount | decimal(10,2)? | | |

---

**Doküman Versiyonu:** v4.0
**Son Güncelleme:** 2026-02-28
