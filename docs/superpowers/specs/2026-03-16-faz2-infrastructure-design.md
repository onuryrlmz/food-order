# Faz 2 Tasarım Dokümanı — Kritik Altyapı

**Tarih:** 2026-03-16
**Durum:** Onaylandı
**Kapsam:** Backend (.NET 8), front-admin, front-seller, front-courier, front-app

---

## 1. Genel Bakış

8 altyapı özelliği:
1. Ödeme İadesi (Refund) — sadece tam iade
2. Push Bildirimler — OneSignal
3. Real-time Güncellemeler — SignalR
4. Refresh Token
5. Şifre Sıfırlama — email + SMS
6. Background Jobs — Hangfire
7. Seller Payout/Settlement
8. Auto-renewal Abonelik — kayıtlı karttan otomatik çekim

---

## 2. Ödeme İadesi (Refund)

### Akış

```
Sipariş iptal edilir (müşteri veya restoran tarafından)
    ↓
Payment.StatusId == Completed ise → iade başlatılır
    ↓
IyzicoServiceAdapter.RefundPayment(paymentTransactionId, amount)
    ↓
Başarılı → Payment.StatusId = Refunded, RefundedAt = now
Başarısız → Payment.StatusId kalır, hata loglanır, admin bilgilendirilir
```

### İade Koşulları
- Sadece tam iade (kısmi iade Faz 2'de yok)
- Sadece `Payment.StatusId == Completed` olan ödemeler iade edilebilir
- Müşteri iptali: sadece `WaitingRestaurantApproval` durumundayken
- Restoran reddi (`RejectedByRestaurant`): otomatik iade tetiklenir
- Admin: herhangi bir completed ödemeyi manuel iade edebilir

### Entity Değişiklikleri

**Payment tablosuna ekleme:**
```
- RefundedAt (DateTime?, nullable)
- RefundTransactionId (string?, nullable) — iyzico refund transaction ID
- RefundReason (string?, nullable)
```

### iyzico Refund API

```csharp
// IyzicoServiceAdapter'a eklenen method:
public async Task<RefundResult> RefundPayment(string paymentTransactionId, decimal amount)
{
    // iyzipay Refund API: POST /refund
    // paymentTransactionId = Payment.ProviderTransactionId
    // price = Payment.Amount (tam iade)
}
```

### API Endpoint'leri

```
POST /v1/admin/orders/{orderId}/refund        — admin manuel iade
GET  /v1/admin/orders/{orderId}/refund-status  — iade durumu
```

Müşteri ve restoran iptali mevcut akışa entegre: iptal edildiğinde otomatik iade.

---

## 3. Push Bildirimler — OneSignal

### Kurulum

- OneSignal SDK: backend'de REST API, mobile'da native SDK
- Her kullanıcı login olduğunda OneSignal Player ID kaydedilir
- `UserExternalInfo` tablosunda: Provider="onesignal", Key="playerId", Value=playerId

### Bildirim Olayları

| Olay | Alıcı | Mesaj |
|------|-------|-------|
| Yeni sipariş | Satıcı | "Yeni sipariş #{orderNo}" |
| Sipariş onaylandı | Müşteri | "Siparişiniz onaylandı" |
| Sipariş hazırlanıyor | Müşteri | "Siparişiniz hazırlanıyor" |
| Sipariş yola çıktı | Müşteri + Kurye | "Sipariş yola çıktı" |
| Sipariş teslim edildi | Müşteri | "Siparişiniz teslim edildi" |
| Sipariş iptal | Müşteri/Satıcı | "Sipariş iptal edildi" |
| İade tamamlandı | Müşteri | "İadeniz tamamlandı" |
| Kurye atandı | Kurye | "Yeni teslimat atandı" |
| Firma daveti | Kurye | "Bir firma sizi sahiplenmek istiyor" |
| Restoran daveti | Firma admin | "Restoran anlaşma teklifi" |
| Abonelik dolmak üzere | Satıcı | "Aboneliğiniz 3 gün sonra bitiyor" |
| Sipariş limiti %80 | Satıcı | "Aylık sipariş limitinizin %80'ine ulaştınız" |

### Backend Servis

```csharp
public interface INotificationService
{
    Task SendToUserAsync(Guid userId, string title, string message, Dictionary<string, string>? data = null);
    Task SendToUsersAsync(List<Guid> userIds, string title, string message, Dictionary<string, string>? data = null);
}
```

OneSignal REST API kullanılır (`POST https://onesignal.com/api/v1/notifications`).

### Mobile Entegrasyon

- `react-native-onesignal` SDK
- App açılışında OneSignal.init(appId)
- Login sonrası setExternalUserId(userId)
- Bildirime tıklayınca ilgili ekrana navigate

### API Endpoint'leri

```
POST /v1/auth/register-device    — OneSignal playerId kaydı (login sonrası)
DELETE /v1/auth/unregister-device — logout'ta playerId silme
```

---

## 4. Real-time Güncellemeler — SignalR

### Hub'lar

**OrderHub** — sipariş durumu değişiklikleri
```csharp
public class OrderHub : Hub
{
    // Client joins order group
    public async Task JoinOrderGroup(string orderId)
        => await Groups.AddToGroupAsync(Context.ConnectionId, $"order-{orderId}");

    public async Task LeaveOrderGroup(string orderId)
        => await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"order-{orderId}");
}

// Server → Client events:
// OrderStatusChanged(orderId, newStatus, timestamp)
// CourierLocationUpdated(orderId, lat, lng, timestamp)
```

**RestaurantHub** — satıcıya yeni sipariş bildirimi
```csharp
public class RestaurantHub : Hub
{
    public async Task JoinRestaurantGroup(string restaurantId)
        => await Groups.AddToGroupAsync(Context.ConnectionId, $"restaurant-{restaurantId}");
}

// Server → Client events:
// NewOrder(orderDto)
// OrderCancelled(orderId)
```

### Entegrasyon

- Sipariş durumu her değiştiğinde → `OrderHub.Clients.Group($"order-{orderId}").OrderStatusChanged(...)`
- Kurye konum güncellendiğinde → `OrderHub.Clients.Group($"order-{orderId}").CourierLocationUpdated(...)`
- Yeni sipariş geldiğinde → `RestaurantHub.Clients.Group($"restaurant-{restaurantId}").NewOrder(...)`

### Frontend

- front-seller: `@microsoft/signalr` npm package, RestaurantHub'a bağlanır
- front-app: `@microsoft/signalr` (React Native uyumlu), OrderHub'a bağlanır
- front-courier: OrderHub'a bağlanır (atanan siparişler için)

### Faz 1'deki Polling'in Değiştirilmesi

- Müşteri kurye takibi: polling → SignalR CourierLocationUpdated event
- Satıcı sipariş listesi: 30s polling → SignalR NewOrder event
- Kurye aktif siparişler: polling → SignalR OrderStatusChanged event

---

## 5. Refresh Token

### Mekanizma

```
Login → access_token (15 dk) + refresh_token (30 gün)
Access token expire → client refresh_token ile yeni çift alır
Refresh token kullanıldığında → eski refresh_token invalidate, yeni çift döner (rotation)
```

### Entity

**RefreshToken (yeni tablo)**
```
- Id (Guid)
- UserId (Guid, FK → User)
- Token (string, unique) — random 64 byte base64
- ExpiresAt (DateTime)
- CreatedAt (DateTime)
- RevokedAt (DateTime?, nullable) — null ise aktif
- ReplacedByToken (string?, nullable) — rotation chain
```

### Akış

```
POST /v1/auth/login → { accessToken, refreshToken, expiresIn }
POST /v1/auth/refresh → { accessToken, refreshToken, expiresIn }
POST /v1/auth/logout → refresh token revoke
```

### Token Yapısı

- Access token: mevcut encrypted JSON token, expiry 15 dakikaya düşer (şu an 30 gün)
- Refresh token: random string, DB'de saklanır
- Client: access token expire olunca otomatik refresh (axios interceptor)

### Mobile/Web Entegrasyon

Axios response interceptor:
```javascript
// 401 alınca → /v1/auth/refresh çağır → yeni token al → isteği tekrarla
// Refresh de 401 dönerse → login'e yönlendir
```

---

## 6. Şifre Sıfırlama — Email + SMS

### Akış

```
Kullanıcı "Şifremi Unuttum" → email VEYA telefon girer
    ↓
Backend: 6 haneli kod üretir, 10 dakika geçerli
    ↓
Email girildiyse → email gönderilir (SMTP)
Telefon girildiyse → SMS gönderilir (mevcut SMS provider)
    ↓
Kullanıcı kodu girer + yeni şifre
    ↓
Backend: kod doğrulanır → şifre güncellenir (BCrypt)
```

### Entity

**PasswordResetToken (yeni tablo)**
```
- Id (Guid)
- UserId (Guid, FK → User)
- Code (string, 6 hane)
- Method (short) — Email=1, Sms=2
- ExpiresAt (DateTime) — CreatedAt + 10 min
- UsedAt (DateTime?, nullable)
- CreatedAt (DateTime)
```

### API Endpoint'leri

```
POST /v1/auth/forgot-password         — { emailOrPhone } → kod gönder
POST /v1/auth/verify-reset-code       — { emailOrPhone, code } → geçerli mi?
POST /v1/auth/reset-password           — { emailOrPhone, code, newPassword }
```

### Güvenlik

- Rate limit: 3 deneme/10 dk per email/phone
- Kod 10 dk sonra expire
- Kullanılan kod tekrar kullanılamaz (UsedAt set edilir)
- Brute force koruması: 5 yanlış kod → 30 dk bekleme

---

## 7. Background Jobs — Hangfire

### Kurulum

- NuGet: `Hangfire.Core`, `Hangfire.MySqlStorage`
- Dashboard: `/hangfire` (sadece Admin rolü)
- Storage: ayrı MySQL schema veya mevcut DB

### Scheduled Jobs

| Job | Sıklık | Açıklama |
|-----|--------|----------|
| CheckExpiredSubscriptions | Her saat | Süresi dolan abonelikleri expire et |
| SendSubscriptionExpiryReminder | Günlük | 3 gün kala satıcıya bildirim |
| AutoRenewSubscriptions | Günlük | Biten abonelikleri otomatik yenile (bkz. Section 8) |
| CleanupExpiredResetTokens | Günlük | Süresi geçmiş şifre sıfırlama kodlarını sil |
| CleanupExpiredRefreshTokens | Haftalık | Süresi geçmiş refresh token'ları sil |
| CheckSubscriptionUsageWarnings | Saatlik | %80 limit uyarısı gönder |

### Mevcut Manuel Endpoint'in Değiştirilmesi

`POST /v1/subscription/expire-check` → Hangfire recurring job'a taşınır. Endpoint kalır ama artık Hangfire otomatik tetikler.

---

## 8. Auto-renewal Abonelik

### Akış

```
Hangfire job: AutoRenewSubscriptions (günlük çalışır)
    ↓
Süresi yarın biten aktif abonelikleri bul
    ↓
Her biri için:
  1. Satıcının kayıtlı kartını bul (UserExternalInfo, provider=iyzico)
  2. Kayıtlı kart yoksa → satıcıya bildirim ("Aboneliğiniz yarın bitiyor, kart ekleyin")
  3. Kart varsa → iyzico ile ödeme al (stored card payment)
  4. Ödeme başarılı → yeni Subscription oluştur (EndDate += plan süresi)
  5. Ödeme başarısız → satıcıya bildirim, 3 gün retry window
  6. 3 gün sonra hala başarısız → abonelik expire, restoran deaktif
```

### Retry Mekanizması

```
Gün 0: İlk deneme başarısız → bildirim
Gün 1: İkinci deneme → bildirim
Gün 2: Üçüncü deneme → son uyarı
Gün 3: Abonelik expire → restoran deaktif
```

### Satıcı Kart Yönetimi

- Satıcı panelden kart ekleyebilir/güncelleyebilir (mevcut iyzico card storage kullanılır)
- Auto-renewal için en az 1 kayıtlı kart gerekli
- Satıcı panelde "Otomatik Yenileme: Açık/Kapalı" toggle

### Subscription Tablosuna Ekleme

```
- AutoRenew (bool, default true)
- RenewalAttempts (int, default 0) — retry sayacı
- LastRenewalAttemptAt (DateTime?, nullable)
```

### API Endpoint'leri

```
PUT /v1/subscription/auto-renew    — { restaurantId, enabled: true/false }
GET /v1/seller/cards               — kayıtlı kartlar (mevcut)
POST /v1/seller/cards              — kart ekle (mevcut)
```

---

## 9. Seller Payout/Settlement

### Mevcut Durum

`Payment` tablosunda `SellerPayoutAmount` ve `CommissionAmount` alanları var ama hiç hesaplanmıyor.

### Akış

```
Sipariş teslim edildi (Delivered)
    ↓
Payment.SellerPayoutAmount = Payment.Amount - CommissionAmount
Payment.CommissionAmount = Payment.Amount * commissionRate
    ↓
iyzico marketplace payout: alt üye işyerine otomatik ödeme
(iyzico bunu zaten subMerchantPrice ile yapıyor — sadece doğru hesaplanması lazım)
```

### Komisyon Hesaplama

- `commissionRate` admin panelden ayarlanabilir (global veya plan bazlı)
- Default: %10
- SubscriptionPlan tablosuna ekleme: `CommissionRate (decimal, default 0.10)`

### Admin Panel

- Finansal özet sayfası: toplam gelir, toplam komisyon, toplam payout
- Satıcı bazında payout geçmişi
- Komisyon oranı ayarlama

### API Endpoint'leri

```
GET  /v1/admin/finance/summary           — toplam gelir, komisyon, payout
GET  /v1/admin/finance/sellers/{id}      — satıcı payout geçmişi
PUT  /v1/admin/settings/commission-rate  — komisyon oranı güncelle
GET  /v1/seller/finance/summary          — satıcının kendi gelir özeti
GET  /v1/seller/finance/payments         — ödeme geçmişi (payout tutarları ile)
```

---

## 10. Veritabanı Değişiklikleri Özeti

### Yeni Tablolar
- `RefreshTokens`
- `PasswordResetTokens`

### Mevcut Tablo Güncellemeleri
- `Payments` → + RefundedAt, RefundTransactionId, RefundReason
- `Subscriptions` → + AutoRenew, RenewalAttempts, LastRenewalAttemptAt
- `SubscriptionPlans` → + CommissionRate

### Yeni NuGet Paketleri
- `Hangfire.Core`, `Hangfire.MySqlStorage`
- `Microsoft.AspNetCore.SignalR`

### Yeni NPM Paketleri
- `react-native-onesignal` (front-app, front-courier)
- `@microsoft/signalr` (front-seller, front-app, front-courier)

---

## 11. Hangfire Jobs Özeti

| Job | Trigger | Bağımlılık |
|-----|---------|------------|
| CheckExpiredSubscriptions | Recurring (1h) | Mevcut logic taşınır |
| SendSubscriptionExpiryReminder | Recurring (daily) | NotificationService |
| AutoRenewSubscriptions | Recurring (daily) | iyzico stored card payment |
| CleanupExpiredResetTokens | Recurring (daily) | PasswordResetToken |
| CleanupExpiredRefreshTokens | Recurring (weekly) | RefreshToken |
| CheckSubscriptionUsageWarnings | Recurring (1h) | NotificationService + SubscriptionUsage |
