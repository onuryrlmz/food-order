# Faz 1 Tasarım Dokümanı — Kurumsal Kurye, Kademeli Abonelik, iyzico Otomatik Kayıt, Navigasyon

**Tarih:** 2026-03-16
**Durum:** Onaylandı
**Kapsam:** Backend (.NET 8), front-admin (Next.js), front-seller (Next.js), front-courier (React Native), front-app (React Native)

---

## 1. Genel Bakış

Bu faz dört ana özelliği kapsar:

1. **Kurumsal Kurye Sistemi** — Vigo gibi firmalar sisteme üye olur, kuryelerini yönetir, restoranlarla anlaşır
2. **Kademeli Abonelik** — Sipariş sayısına göre paketler (Basic/Pro/Premium), limit takibi, paket yükseltme
3. **iyzico Alt Üye İşyeri Otomatik Kaydı** — Satıcı admin onayında otomatik iyzico kaydı
4. **Kurye Navigasyon ve Konum Takibi** — Uygulama içi harita + harici navigasyon + arka plan konum takibi

---

## 2. Kurumsal Kurye Sistemi

### 2.1 Yeni Entity'ler

**CourierCompany**
```
- Id (Guid)
- Name (string) — firma adı ("Vigo Kurye")
- LegalName (string)
- TaxCode (string)
- TaxArea (string)
- ContactEmail (string)
- ContactPhone (string)
- OwnerUserId (Guid, FK → User) — firmayı açan kişi
- StatusId (short) — PendingApproval=1, Active=2, Suspended=3, Banned=4
- CreatedAt (DateTime)
- UpdatedAt (DateTime)
```

**CourierCompanyMember**
```
- Id (Guid)
- CourierCompanyId (Guid, FK → CourierCompany)
- CourierId (Guid, FK → User, role=Courier)
- StatusId (short) — PendingApproval=1, Active=2, RemovedByCompany=3, LeftByChoice=4
- RequestedAt (DateTime)
- ApprovedAt (DateTime?)
```

**RestaurantCourier tablosuna ekleme:**
```
- CourierCompanyId (Guid?, nullable FK → CourierCompany)
```
null ise bireysel kurye anlaşması, doluysa kurumsal firma anlaşması.

**Order tablosuna ekleme:**
```
- CourierCompanyId (Guid?, nullable FK → CourierCompany)
- PickedUpByCourierId (Guid?, nullable FK → User)
- PickedUpAt (DateTime?)
- DeliveredAt (DateTime?)
```

### 2.2 Akışlar

#### 2.2.1 Firma Kaydı
1. Kurye app'ten "Kurumsal Firma Olarak Kayıt" seçeneği
2. Firma bilgileri girilir (ad, vergi no, iletişim)
3. CourierCompany oluşur (StatusId = PendingApproval)
4. Admin onaylarsa → StatusId = Active
5. Owner kullanıcının rolü `CourierCompanyAdmin` olarak güncellenir

#### 2.2.2 Kurye Sahiplenme
1. Firma admini app'ten kurye arar (email/telefon ile)
2. "Bu benim kuryem" talebi → CourierCompanyMember (PendingApproval)
3. Kurye app'te talebi görür → Onaylar veya Reddeder
4. Onaylarsa → Active, kurye artık o firmanın üyesi
5. Bir kurye aynı anda sadece bir firmaya bağlı olabilir

#### 2.2.3 Restoran ↔ Firma Anlaşması
1. Satıcı panelinden "Kurumsal Firma Ekle" → firma aranır, davet gönderilir
2. Mevcut RestaurantCourier sistemi genişler (CourierCompanyId dolu)
3. Firma admini app'ten onaylar → RestaurantCourier.StatusId = Active

#### 2.2.4 Sipariş Atama — Hibrit Teslim Alma
1. Restoran siparişi "Yola Çıktı" yapar
2. Dropdown'dan seçim: bireysel kurye VEYA kurumsal firma
3. Kurumsal firma seçilirse: Order.CourierCompanyId = firmaId, PickedUpByCourierId = null
4. Firmanın herhangi bir kuryesi restorana gelir
5. Kurye ekranında: o restoranın bekleyen siparişleri (CourierCompanyId eşleşen, PickedUpByCourierId null)
6. Kurye sipariş numarasını doğrular → "Teslim Aldım"
7. PickedUpByCourierId = kuryeId, PickedUpAt = now
8. Birden fazla sipariş alabilir

#### 2.2.5 Bireysel Kurye (mevcut akış korunur)
- Restoran bireysel kuryeye atar → Order.CourierId = kuryeId, CourierCompanyId = null
- Mevcut akış aynen devam eder

### 2.3 API Endpoint'leri

```
# Firma Yönetimi
POST   /v1/courier/company/register              — Firma kaydı
GET    /v1/courier/company/my                     — Firma bilgilerim
PUT    /v1/courier/company/my                     — Firma güncelle

# Firma ↔ Kurye
POST   /v1/courier/company/members/request        — Kurye sahiplenme talebi
GET    /v1/courier/company/members                — Firma kuryeleri listele
DELETE /v1/courier/company/members/{id}           — Kurye çıkar

# Kurye tarafı
GET    /v1/courier/company-invites                — Firma talepleri
PUT    /v1/courier/company-invites/{id}/accept
PUT    /v1/courier/company-invites/{id}/reject

# Restoran ↔ Firma
POST   /v1/seller/restaurant/{id}/courier-company/add    — Firma davet
GET    /v1/seller/restaurant/{id}/courier-companies      — Anlaşmalı firmalar

# Sipariş teslim alma
GET    /v1/courier/pickup/{restaurantId}/pending          — Bekleyen siparişler
PUT    /v1/courier/pickup/{orderId}/confirm                — Teslim aldım

# Admin
GET    /v1/admin/courier-companies                — Firma listesi
PUT    /v1/admin/courier-companies/{id}/approve   — Firma onay
```

---

## 3. Kademeli Abonelik Sistemi

### 3.1 SubscriptionPlan Tablosu Güncelleme

Mevcut alanlara eklenen:
```
- MaxOrdersPerMonth (int) — bu paketin sipariş limiti
- OverageAction (short) — Block=1, AutoUpgrade=2
```

Örnek paketler:
```
Basic:    0-100 sipariş/ay   → 500₺/ay,  MaxRestaurants=1
Pro:      0-500 sipariş/ay   → 1000₺/ay, MaxRestaurants=3
Premium:  0-2000 sipariş/ay  → 2000₺/ay, MaxRestaurants=10
```

### 3.2 Yeni Entity: SubscriptionUsage

```
- Id (Guid)
- SubscriptionId (Guid, FK → Subscription)
- Year (int)
- Month (int)
- OrderCount (int) — o aydaki toplam sipariş sayısı
- UpdatedAt (DateTime)
```

Unique constraint: (SubscriptionId, Year, Month)

### 3.3 Akışlar

#### 3.3.1 Paket Seçimi
- Satıcı paketleri görür (sipariş limitleri dahil)
- Paket seçer → mevcut ödeme akışı ile satın alır
- SubscriptionUsage o ay için OrderCount=0 ile oluşur

#### 3.3.2 Sipariş Sayacı
- Her başarılı sipariş → SubscriptionUsage.OrderCount++ (atomik increment)
- Satıcı panelinde gösterge: "Bu ay: 73/100 sipariş kullandınız"

#### 3.3.3 Limit Aşımı
- **Block**: Sipariş limiti dolunca restoran yeni sipariş alamaz. Müşteri app'te "Bu restoran şu an sipariş kabul etmiyor." Satıcıya "Limitiniz doldu, üst pakete geçin" uyarısı.
- **AutoUpgrade**: Otomatik üst pakete geçiş teklifi → satıcı onaylarsa upgrade, onaylamazsa block.

#### 3.3.4 Paket Yükseltme
- Satıcı panelinden "Paket Yükselt" → üst paketler listelenir
- Kalan günler için fark: `(üstPaketFiyat - mevcutFiyat) × (kalanGün / 30)`
- Ödeme alınır → Subscription.SubscriptionPlanId güncellenir
- SubscriptionUsage aynı kalır (sayaç sıfırlanmaz)

#### 3.3.5 Ay Sonu / Yenileme
- EndDate gelince → mevcut expire check devam eder
- Satıcı manuel yeniler (auto-renewal Faz 2'de)
- Yeni ay başında SubscriptionUsage yeni kayıt (OrderCount=0)

### 3.4 Order Flow Etkisi

`OrderManager.CreateOrder` içinde kontrol:
1. Satıcının aktif Subscription'ını bul
2. SubscriptionUsage.OrderCount < Plan.MaxOrdersPerMonth mı?
3. Aşıldıysa → OverageAction'a göre block veya upgrade teklifi
4. Bu kontrol sipariş oluşturulurken ve restoran listesinde görünürlük olarak yapılır

### 3.5 API Endpoint'leri

```
GET  /v1/subscription/plans              — MaxOrdersPerMonth bilgisi de döner
GET  /v1/subscription/usage              — bu ayki kullanım (73/100)
POST /v1/subscription/upgrade            — paket yükseltme
GET  /v1/subscription/upgrade/preview    — yükseltme fiyat önizleme
```

### 3.6 Seller Panel Değişiklikleri

- Dashboard'a "Sipariş Kullanımı" widget'ı: progress bar + kalan gün
- %80'de sarı uyarı, %100'de kırmızı uyarı + "Yükselt" butonu
- Abonelik sayfasına paket karşılaştırma tablosu

---

## 4. iyzico Alt Üye İşyeri Otomatik Kaydı

### 4.1 Seller Tablosuna Ekleme

```
- IdentityNumber (string?, nullable) — bireysel satıcılar için TC kimlik no
```

### 4.2 Akış

#### 4.2.1 Satıcı Kayıt (Eksik Bilgi Tamamlama)

Kayıt formuna eklenen validasyonlar:
- CompanyType seçimi: Bireysel / Limited / Anonim
- Bireysel → IdentityNumber zorunlu
- Limited/Anonim → TaxCode, TaxArea zorunlu (zaten var)
- IBAN zorunlu (zaten var)
- LegalName zorunlu (zaten var)

iyzico'nun gerektirdiği tüm alanlar dolmadan kayıt tamamlanamaz.

#### 4.2.2 Admin Onay Akışı

```
Admin "Onayla" butonuna basar
    ↓
Backend: Seller bilgileri iyzico formatına map edilir
    ↓
CompanyType'a göre:
  - Bireysel → subMerchantType = "PERSONAL"
    → identityNumber, iban, contactName, contactSurname, email
  - Limited → subMerchantType = "LIMITED_OR_JOINT_STOCK_COMPANY"
    → taxOffice, legalCompanyTitle, taxNumber, iban, email
  - Anonim → subMerchantType = "LIMITED_OR_JOINT_STOCK_COMPANY"
    → taxOffice, legalCompanyTitle, taxNumber, iban, email
    ↓
IyzicoServiceAdapter.CreateSeller() çağrılır
    ↓
Başarılı:
  → subMerchantKey UserExternalInfo'ya kaydedilir
  → Seller.CompanyStatus = Approved
  → Admin'e başarı mesajı
    ↓
Başarısız:
  → Seller.CompanyStatus değişmez (Pending kalır)
  → Hata mesajı admin'e gösterilir
  → Hata log'a yazılır
  → Admin tekrar deneyebilir
```

#### 4.2.3 Ödeme Akışında Kullanım

- Müşteri sipariş verince → PaymentManager satıcının subMerchantKey'ini UserExternalInfo'dan çeker
- iyzico'ya ödeme gönderilirken subMerchantKey ve subMerchantPrice (komisyon düşülmüş tutar) eklenir
- iyzico otomatik olarak satıcıya payout yapar

#### 4.2.4 Güncelleme Akışı

- Satıcı bilgilerini değiştirirse (IBAN, ticari ünvan vs.)
- Backend otomatik IyzicoServiceAdapter.UpdateSeller() çağırır
- subMerchantKey aynı kalır, bilgiler güncellenir

### 4.3 Admin Panel Değişiklikleri

- Satıcı detay sayfasında "iyzico Durumu" badge'i:
  - **Kayıtlı** (yeşil) — subMerchantKey mevcut
  - **Kayıtsız** (gri) — henüz onaylanmamış
  - **Hatalı** (kırmızı) — son deneme başarısız + hata mesajı
- "Onayla" butonu basıldığında loading state + sonuç feedback
- Hata durumunda "Tekrar Dene" butonu

### 4.4 API Endpoint'leri

```
PUT /v1/admin/sellers/{id}/approve   — iyzico kaydı da yapar
GET /v1/admin/sellers/{id}           — iyzico durumu bilgisi de döner
PUT /v1/seller/profile               — bilgi güncellenince iyzico'ya da sync
```

---

## 5. Kurye Navigasyon ve Konum Takibi

### 5.1 Kurye Teslim Sonrası Ekran

Teslim aldıktan sonra sipariş kartı genişler:
- Müşteri adresi
- Mesafe bilgisi
- **"Haritada Gör"** butonu → uygulama içi harita (react-native-maps)
- **"Navigasyonu Başlat"** butonu → harici harita deep link

### 5.2 Uygulama İçi Harita

- react-native-maps ile harita görünümü
- Teslimat noktası pin olarak gösterilir
- Kurye konumu canlı güncellenir

### 5.3 Harici Navigasyon Deep Link

- iOS: Apple Maps (`maps://`) veya Google Maps (tercih sorulur)
- Android: Google Maps (`google.navigation:q=lat,lng`)
- `Linking.openURL()` ile açılır

### 5.4 Arka Plan Konum Takibi

- react-native-geolocation-service ile arka planda konum alınır (zaten mevcut)
- Kurye aktif sipariş taşırken her 15 saniyede bir konum gönderilir
- `POST /v1/courier/location` → CourierLocation tablosuna upsert
- Sipariş teslim edilince konum takibi durur

### 5.5 Müşteri Tarafında Harita (front-app)

OrderDetail ekranında sipariş "Yola Çıktı" durumundayken:
- Harita görünür (react-native-maps)
- Kurye konumu: mavi pin (30 saniyede bir polling ile güncellenir)
- Teslimat adresi: kırmızı pin
- Tahmini süre: mesafe bazlı basit hesaplama
- Kurye bilgisi: isim + telefon (arama butonu)

> Not: Real-time WebSocket Faz 2'de gelecek. Faz 1'de polling ile çalışır.

### 5.6 Çoklu Sipariş Navigasyonu

- Aktif siparişler listesinde her sipariş ayrı kart
- Kurye istediği sırayla teslim edebilir
- Her sipariş için ayrı navigasyon butonu
- "Teslim Ettim" → sipariş listeden düşer
- Son sipariş teslim edilince konum takibi durur

### 5.7 Sipariş Teslim Onayı

```
Kurye "Teslim Ettim" → Order.StatusId = Delivered, DeliveredAt = now
→ Konum takibi durur (o orderId için)
→ Müşteri tarafında "Teslim Edildi" olarak güncellenir
```

### 5.8 API Endpoint'leri

```
POST /v1/courier/location                          — mevcut, değişiklik yok
PUT  /v1/courier/orders/{orderId}/deliver           — teslim ettim
GET  /v1/customer/order/{orderId}/courier-location  — kurye konumu (polling)
```

### 5.9 Mobile Kütüphaneler

- `react-native-maps` — uygulama içi harita
- `react-native-geolocation-service` — zaten mevcut
- `Linking.openURL()` — harici navigasyon

---

## 6. Veritabanı Değişiklikleri Özeti

### Yeni Tablolar
- `CourierCompanies`
- `CourierCompanyMembers`
- `SubscriptionUsages`

### Mevcut Tablo Güncellemeleri
- `Sellers` → + IdentityNumber
- `SubscriptionPlans` → + MaxOrdersPerMonth, OverageAction
- `RestaurantCouriers` → + CourierCompanyId
- `Orders` → + CourierCompanyId, PickedUpByCourierId, PickedUpAt, DeliveredAt

### Yeni Enum'lar
- `CourierCompanyStatusEnums`: PendingApproval=1, Active=2, Suspended=3, Banned=4
- `CourierCompanyMemberStatusEnums`: PendingApproval=1, Active=2, RemovedByCompany=3, LeftByChoice=4
- `OverageActionEnums`: Block=1, AutoUpgrade=2

### Yeni User Role
- `CourierCompanyAdmin` — firma yöneticisi rolü

---

## 7. Proje Tarama Sonuçları (Faz 2-3 için)

### Faz 2 — Kritik Altyapı
- Ödeme iadesi (Refund) implementasyonu
- Push bildirimler (Firebase Cloud Messaging)
- Real-time güncellemeler (SignalR)
- Refresh token mekanizması
- Şifre sıfırlama
- Background job scheduler (Hangfire)
- Seller payout/settlement sistemi
- Auto-renewal abonelik

### Faz 3 — UX Geliştirmeleri
- Yorum/değerlendirme sistemi
- Restoran arama/filtreleme (isim, mutfak, fiyat, puan)
- Menü görselleri upload
- Favori restoranlar
- Tekrar sipariş (reorder)
- Kurye kazanç takibi
- Kurye online/offline toggle
- Admin finansal raporlar
- Seller analytics dashboard
- Sipariş zaman aşımı
