# Abonelik → Komisyon Sistemi Geçiş Tasarımı

## Amaç
Mevcut aylık abonelik gelir modelini kaldırıp, sipariş başına komisyon (%X) + sabit ücret (Y₺) modeline geçiş. Restoran bazlı özel oranlar + platform geneli varsayılan değerler. Günlük hakediş sistemi ile satıcı ödemeleri.

---

## 1. Kaldırılacaklar

### Entity'ler
- `SubscriptionPlan` — tamamen kaldır
- `Subscription` — tamamen kaldır
- `SubscriptionUsage` — tamamen kaldır

### Enum'lar
- `SubscriptionStatusEnums` — kaldır
- `SubscriptionPlanTypeEnums` — kaldır
- `OverageActionEnums` — kaldır

### Servisler
- `Application/Services/Seller/SubscriptionService/` — tüm klasör kaldır
- `Application/Services/Common/BackgroundJobs/SubscriptionJobService.cs` — kaldır
- `Application/Services/Common/BackgroundJobs/ISubscriptionJobService.cs` — kaldır

### Controller'lar
- `WebAPI/Controllers/Seller/SellerSubscriptionController.cs` — kaldır
- `WebAPI/Controllers/Admin/AdminSubscriptionController.cs` — kaldır

### Repository'ler
- `ISubscriptionRepository`, `SubscriptionRepository` — kaldır
- `ISubscriptionPlanRepository`, `SubscriptionPlanRepository` — kaldır
- `ISubscriptionUsageRepository`, `SubscriptionUsageRepository` — kaldır

### Entity Configuration'lar
- `SubscriptionPlanConfiguration.cs` — kaldır
- `SubscriptionConfiguration.cs` — kaldır
- `SubscriptionUsageConfiguration.cs` — kaldır

### Hangfire Job'lar (Program.cs)
- `check-expired` — kaldır
- `expiry-reminder` — kaldır
- `auto-renew` — kaldır
- `usage-warnings` — kaldır

### Frontend
- `front-seller/app/subscription/page.js` — kaldır
- `front-admin/app/subscriptions/page.js` — kaldır
- `front-admin/app/subscriptions/plans/page.js` — kaldır (varsa)
- İlgili API fonksiyonları (getSubscriptionUsage, upgradeSubscription, vb.)

### Seed Data
- `SubscriptionPlan` seed kayıtları — kaldır
- `Subscription` seed kayıtları — kaldır

### UnitOfWork, DbContext, DI
- Subscription ile ilgili tüm DbSet, repository property, DI kayıtları kaldır

---

## 2. Yeni Entity'ler

### 2.1 PlatformCommissionSchedule
Platform geneli varsayılan komisyon tarifesi. Birden fazla kayıt olabilir — tarih bazlı planlama.
Admin önceden "1 Kasım'da %10, 1 Aralık'ta %8" gibi girebilir. Aktif olan = `EffectiveFrom <= now` ve `EffectiveTo > now || EffectiveTo == null` olan en güncel kayıt.

```
PlatformCommissionSchedule : Entity<Guid>
├── CommissionRate       decimal    (ör: 0.10 = %10)
├── FixedFee             decimal    (ör: 5.00₺)
├── EffectiveFrom        DateTime   (bu tarihten itibaren geçerli)
├── EffectiveTo          DateTime?  (null = süresiz, yeni kayıt girilince set edilir)
├── SetByUserId          Guid?      (admin user)
└── Notes                string?    (ör: "Kış kampanyası indirimi")
```

Konum: `Domain/Entities/Common/PlatformCommissionSchedule.cs`

Aktif kaydı bulmak: `WHERE EffectiveFrom <= @now AND (EffectiveTo IS NULL OR EffectiveTo > @now) ORDER BY EffectiveFrom DESC LIMIT 1`

### 2.2 RestaurantCommission
Restoran bazlı komisyon geçmişi. Her değişiklik yeni kayıt oluşturur, eski kayıdın `EffectiveTo`'su set edilir. Böylece tam geçmiş tutulur.
Restoran için özel kayıt yoksa → PlatformCommissionSchedule varsayılanı uygulanır.

```
RestaurantCommission : Entity<Guid>
├── RestaurantId        Guid FK
├── CommissionRate      decimal     (ör: 0.08 = %8)
├── FixedFee            decimal     (ör: 3.50₺)
├── EffectiveFrom       DateTime    (bu tarihten itibaren geçerli)
├── EffectiveTo         DateTime?   (null = aktif, değiştirilince set edilir)
├── SetByUserId         Guid?       (admin user)
├── Reason              string?     (değişiklik sebebi, ör: "Özel anlaşma", "Kampanya")
└── Notes               string?
```

Konum: `Domain/Entities/Seller/RestaurantCommission.cs`

Aktif kaydı bulmak: `WHERE RestaurantId = @id AND EffectiveFrom <= @now AND (EffectiveTo IS NULL OR EffectiveTo > @now) ORDER BY EffectiveFrom DESC LIMIT 1`

**Komisyon çözümleme sırası:**
1. Restoran için aktif `RestaurantCommission` var mı? → Onu kullan
2. Yoksa → Aktif `PlatformCommissionSchedule` varsayılanını kullan

### 2.3 SettlementItem
Her tamamlanan sipariş için bir kayıt. Sipariş Delivered olduğunda otomatik oluşur.
Uygulanan komisyon oranı ve kaynağı snapshot olarak saklanır — sonradan oran değişse bile geçmiş etkilenmez.

```
SettlementItem : Entity<Guid>
├── OrderId                    Guid FK
├── SellerId                   Guid FK
├── RestaurantId               Guid FK
├── OrderAmount                decimal     (sipariş toplam tutarı)
├── CommissionRate             decimal     (uygulanan oran — snapshot)
├── CommissionAmount           decimal     (= OrderAmount × CommissionRate)
├── FixedFee                   decimal     (sabit ücret — snapshot)
├── NetAmount                  decimal     (= OrderAmount - CommissionAmount - FixedFee)
├── CommissionSourceType       short       (1=Platform, 2=RestaurantCustom)
├── CommissionSourceId         Guid        (PlatformCommissionSchedule veya RestaurantCommission Id)
├── PeriodDate                 DateTime    (sipariş tarihi, sadece date kısmı)
├── SettlementPeriodId         Guid? FK    (günlük dönem oluştuğunda set edilir)
└── Notes                      string?
```

Konum: `Domain/Entities/Seller/SettlementItem.cs`

### 2.4 SettlementPeriod
Günlük hakediş dönemi. Hangfire job ile otomatik oluşur.

```
SettlementPeriod : Entity<Guid>
├── SellerId              Guid FK
├── RestaurantId          Guid FK
├── PeriodDate            DateTime       (hangi gün)
├── TotalOrderCount       int
├── TotalOrderAmount      decimal
├── TotalCommission       decimal
├── TotalFixedFee         decimal
├── TotalNetAmount        decimal        (satıcıya ödenecek)
├── StatusId              short          (Pending=1, Approved=2, Paid=3, Cancelled=4)
├── IBAN                  string?        (ödeme yapılan IBAN)
├── BankTransferRef       string?        (EFT/havale referansı)
├── ApprovedAt            DateTime?
├── ApprovedByUserId      Guid?
├── PaidAt                DateTime?
└── Notes                 string?
```

Konum: `Domain/Entities/Seller/SettlementPeriod.cs`

### 2.5 PaymentLog
Tüm ödeme isteklerini/yanıtlarını tutan audit log.

```
PaymentLog : Entity<Guid>
├── OrderId              Guid? FK
├── PaymentId            Guid? FK
├── Action               string          (InitiatePayment, Callback, Refund, vb.)
├── RequestData          string          (JSON — gönderilen veri)
├── ResponseData         string          (JSON — gelen yanıt)
├── StatusCode           int?
├── IsSuccess            bool
├── ErrorMessage         string?
├── DurationMs           int?
└── IpAddress            string?
```

Konum: `Domain/Entities/Buyer/PaymentLog.cs`

---

## 3. Enum'lar

### Yeni
```
SettlementStatusEnums : short
├── Pending = 1
├── Approved = 2
├── Paid = 3
└── Cancelled = 4

CommissionSourceTypeEnums : short
├── Platform = 1
└── RestaurantCustom = 2
```

---

## 4. Restaurant Entity Güncelleme

```diff
Restaurant
+ ApprovedAt              DateTime?
+ ApprovedByUserId        Guid?
```

`IsActive` artık admin onayına bağlı:
- Admin restoran onayladığında: `IsActive = true`, `ApprovedAt = now`
- Abonelik kontrolü tamamen kaldırılıyor

---

## 5. Değişen Akışlar

### 5.1 Sipariş Verme (PlaceOrder)

**Eski:**
```
1. Restaurant.IsActive kontrol
2. HasActiveSubscription kontrol ← KALDIRILACAK
3. IncrementOrderCount ← KALDIRILACAK
4. Sipariş oluştur
```

**Yeni:**
```
1. Restaurant.IsActive kontrol (admin onayına bağlı)
2. Sipariş oluştur
```

### 5.2 Sipariş Teslim Edildiğinde

**Eski:** Payment kaydında CommissionAmount hesapla (subscription plan rate'i ile)

**Yeni:**
```
1. RestaurantCommission'dan oran al (yoksa PlatformSettings.Default)
2. SettlementItem oluştur:
   - CommissionAmount = OrderAmount × CommissionRate
   - FixedFee = RestaurantCommission.FixedFee (veya default)
   - NetAmount = OrderAmount - CommissionAmount - FixedFee
3. Payment kaydını da güncelle (CommissionAmount, SellerPayoutAmount)
```

### 5.3 Günlük Hakediş Job'ı (Hangfire, her gece 00:05)

```
1. Dünün tarihini al (PeriodDate = yesterday)
2. Tüm SettlementItem'ları grupla: GROUP BY SellerId, RestaurantId, PeriodDate
3. Her grup için SettlementPeriod oluştur (StatusId = Pending)
4. SettlementItem'ların SettlementPeriodId'sini set et
5. Seller'ın IBAN'ını SettlementPeriod.IBAN'a kopyala
```

### 5.4 Admin Hakediş Onay/Ödeme

```
Admin panel:
1. Pending hakedişleri listele
2. "Onayla" → StatusId = Approved, ApprovedAt = now
3. "Ödendi İşaretle" → StatusId = Paid, PaidAt = now, BankTransferRef = input
4. "İptal" → StatusId = Cancelled, Notes = sebep
```

### 5.5 Restoran Onay Akışı

```
1. SellerAdmin restoran oluşturur (IsActive = false)
2. Admin onaylar → IsActive = true, ApprovedAt = now, ApprovedByUserId = adminId
3. Varsayılan komisyon otomatik atanır (RestaurantCommission kaydı oluşur)
```

---

## 6. API Endpoint'ler

### Yeni Endpoint'ler

**Admin:**
| Method | Route | Açıklama |
|--------|-------|----------|
| GET | `/v1/admin/commission/settings` | Platform varsayılan komisyon ayarları |
| PUT | `/v1/admin/commission/settings` | Varsayılan güncelle |
| GET | `/v1/admin/commission/restaurant/{id}` | Restoran komisyon bilgisi |
| PUT | `/v1/admin/commission/restaurant/{id}` | Restoran komisyonu belirle |
| GET | `/v1/admin/settlement/periods` | Hakediş dönemleri listesi (filtre: status, tarih) |
| GET | `/v1/admin/settlement/period/{id}` | Dönem detay (item'lar dahil) |
| POST | `/v1/admin/settlement/period/{id}/approve` | Hakediş onayla |
| POST | `/v1/admin/settlement/period/{id}/pay` | Ödendi işaretle |
| POST | `/v1/admin/settlement/period/{id}/cancel` | İptal |
| POST | `/v1/admin/restaurant/{id}/approve` | Restoran onayla (mevcut toggle-active yerine) |

**Seller:**
| Method | Route | Açıklama |
|--------|-------|----------|
| GET | `/v1/seller/commission/my` | Restoranlarımın komisyon bilgisi |
| GET | `/v1/seller/settlement/periods` | Hakediş dönemlerim |
| GET | `/v1/seller/settlement/period/{id}` | Dönem detay |

### Kaldırılacak Endpoint'ler
- Tüm `/v1/seller/subscription/*`
- Tüm `/v1/admin/subscription/*`

### Değişen Endpoint'ler
- `PUT /v1/admin/finance/settings/commission-rate` → kaldır (yeni commission endpoint'leri ile değişiyor)

---

## 7. Frontend Değişiklikleri

### Seller Panel
- **Kaldır:** `subscription/page.js`
- **Yeni:** `commission/page.js` — Komisyon bilgisi (oran + sabit ücret, read-only)
- **Güncelle:** `finance/page.js` — Günlük hakediş dönemleri, sipariş detay, ödeme durumu

### Admin Panel
- **Kaldır:** `subscriptions/page.js`, `subscriptions/plans/page.js`
- **Yeni:** `commission/page.js` — Varsayılan ayarlar + restoran bazlı komisyon yönetimi
- **Yeni:** `settlements/page.js` — Hakediş listesi, onaylama, ödeme işaretleme
- **Yeni:** `settlements/[periodId]/page.js` — Dönem detay (sipariş satırları)
- **Güncelle:** `finance/page.js` — Abonelik geliri yerine komisyon geliri

### Sidebar Güncellemeleri
- Seller: "Abonelik" → "Komisyon Bilgisi"
- Admin: "Abonelikler" → "Hakedişler" + "Komisyon Ayarları"

---

## 8. Hangfire Job'lar

### Kaldır
- `check-expired` (abonelik süre kontrolü)
- `expiry-reminder` (süre hatırlatma)
- `auto-renew` (otomatik yenileme)
- `usage-warnings` (kullanım uyarısı)

### Yeni
- `generate-daily-settlements` — Her gece 00:05, günlük hakediş dönemlerini oluştur

---

## 9. Seed Data

### Kaldır
- SubscriptionPlan (3 adet)
- Subscription (2 adet)

### Yeni
- PlatformCommissionSchedule: EffectiveFrom=SeedDate, CommissionRate=0.10, FixedFee=5.00
- RestaurantCommission: Restaurant3 (Kebapçı) için özel oran (%8, 3₺) — diğerleri platform varsayılanı kullanır
- Restaurant: `ApprovedAt = SeedDate`, `ApprovedByUserId = AdminUserId` (3 restoran için)

---

## 10. Migration Stratejisi

1. Yeni tabloları oluştur (PlatformSettings, RestaurantCommission, SettlementItem, SettlementPeriod, PaymentLog)
2. Restaurant'a ApprovedAt, ApprovedByUserId ekle
3. Eski tabloları kaldır (SubscriptionPlan, Subscription, SubscriptionUsage)
4. Tek migration: `RemoveSubscriptionAddCommission`
