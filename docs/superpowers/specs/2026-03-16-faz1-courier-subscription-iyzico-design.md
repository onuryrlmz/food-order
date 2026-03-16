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

**RestaurantCourierCompany (Yeni Tablo — Restoran ↔ Firma anlaşması)**
```
- Id (Guid)
- RestaurantId (Guid, FK → Restaurant)
- CourierCompanyId (Guid, FK → CourierCompany)
- StatusId (short) — PendingApproval=1, Active=2, TerminatedByRestaurant=3, TerminatedByCompany=4
- AgreementStartDate (DateTime)
- AgreementEndDate (DateTime?)
```

> Not: Mevcut `RestaurantCourier` tablosu sadece bireysel kuryeler için kullanılmaya devam eder. Kurumsal firma anlaşmaları ayrı `RestaurantCourierCompany` tablosunda tutulur. Bu sayede `RestaurantCourier.CourierId` nullable yapılmak zorunda kalmaz.

**Order tablosuna ekleme:**
```
- CourierCompanyId (Guid?, nullable FK → CourierCompany) — siparişin hangi firmaya verildiği
- PickedUpByCourierId (Guid?, nullable FK → User) — fiziksel olarak teslim alan kurye
- PickedUpAt (DateTime?) — teslim alma zamanı
- DeliveredAt (DateTime?) — teslim zamanı
```

### 2.2 Roller ve Yetkilendirme

**Yeni User Role:**
```
CourierCompanyAdmin = 7  (UserRoleEnums'a eklenir)
```

**Yetkilendirme kuralları:**
- `CourierCompanyAdmin` rolü, `Courier` rolünün tüm yetkilerini kapsar
- Tüm courier endpoint'lerindeki `[AuthorizeAPIRequest]` kontrolleri hem `Courier` hem `CourierCompanyAdmin` kabul edecek şekilde güncellenir
- Firma yönetim endpoint'leri sadece `CourierCompanyAdmin` rolünü kabul eder
- Firma owner'ı başka bir firma tarafından sahiplenemez (claim request reddedilir)

**Kurye app UX farklılığı:**
- Login sonrası rol kontrol edilir
- `Courier` → mevcut kurye ekranları
- `CourierCompanyAdmin` → ek olarak "Firma Yönetimi" tab'ı görünür (kurye listesi, talepler, restoran anlaşmaları)
- Her iki rol de sipariş teslim alma, navigasyon, konum takibi yapabilir

### 2.3 Akışlar

#### 2.3.1 Firma Kaydı
1. Kurye app'ten "Kurumsal Firma Olarak Kayıt" seçeneği
2. Firma bilgileri girilir (ad, vergi no, iletişim)
3. CourierCompany oluşur (StatusId = PendingApproval)
4. Admin onaylarsa → StatusId = Active
5. Owner kullanıcının rolü `CourierCompanyAdmin` olarak güncellenir

#### 2.3.2 Kurye Sahiplenme
1. Firma admini app'ten kurye arar (email/telefon ile)
2. Validasyon: hedef kullanıcı `Courier` rolünde olmalı, başka firmaya bağlı olmamalı, firma owner'ı olmamalı
3. "Bu benim kuryem" talebi → CourierCompanyMember (PendingApproval)
4. Kurye app'te talebi görür → Onaylar veya Reddeder
5. Onaylarsa → Active, kurye artık o firmanın üyesi
6. Bir kurye aynı anda sadece bir firmaya bağlı olabilir. MySQL filtered index desteklemediği için bu kısıt application-level'da enforce edilir: sahiplenme talebi gönderilmeden önce kurye'nin aktif veya bekleyen başka üyeliği var mı kontrol edilir (SELECT COUNT). Ayrıca INSERT öncesi tekrar kontrol yapılır (double-check pattern).

#### 2.3.3 Restoran ↔ Firma Anlaşması
1. Satıcı panelinden "Kurumsal Firma Ekle" → sadece Active firmalar aranabilir
2. RestaurantCourierCompany kaydı oluşur (StatusId = PendingApproval)
3. Firma admini app'ten onaylar → StatusId = Active

#### 2.3.4 Sipariş Atama — Hibrit Teslim Alma
1. Restoran siparişi "Yola Çıktı" yapar
2. Dropdown'dan seçim: bireysel kurye VEYA kurumsal firma (sadece Active anlaşmalı olanlar)
3. Kurumsal firma seçilirse: Order.CourierCompanyId = firmaId, PickedUpByCourierId = null
4. Firmanın herhangi bir kuryesi restorana gelir
5. Kurye ekranında: o restoranın bekleyen siparişleri (CourierCompanyId eşleşen, PickedUpByCourierId null)
6. Kurye sipariş numarasını doğrular → "Teslim Aldım"
7. PickedUpByCourierId = kuryeId, PickedUpAt = now
8. Birden fazla sipariş alabilir

#### 2.3.5 Bireysel Kurye (mevcut akış korunur)
- Restoran bireysel kuryeye atar → Order.CourierId = kuryeId, CourierCompanyId = null
- Bireysel kuryede de pickup onayı eklenir: kurye "Teslim Aldım" → PickedUpByCourierId = kuryeId, PickedUpAt = now
- Bu sayede her iki akışta da teslim alma zamanı kaydedilir

### 2.4 API Endpoint'leri

```
# Firma Yönetimi (CourierCompanyAdmin only)
POST   /v1/courier/company/register              — Firma kaydı
GET    /v1/courier/company/my                     — Firma bilgilerim
PUT    /v1/courier/company/my                     — Firma güncelle

# Firma ↔ Kurye (CourierCompanyAdmin only)
GET    /v1/courier/company/members/search?q=...   — Kurye arama (email/telefon)
POST   /v1/courier/company/members/request        — Kurye sahiplenme talebi
GET    /v1/courier/company/members                — Firma kuryeleri listele
DELETE /v1/courier/company/members/{id}           — Kurye çıkar

# Kurye tarafı (Courier role)
GET    /v1/courier/company-invites                — Firma talepleri
PUT    /v1/courier/company-invites/{id}/accept
PUT    /v1/courier/company-invites/{id}/reject
PUT    /v1/courier/company/leave                  — Firmadan ayrıl

# Restoran ↔ Firma (Seller role)
POST   /v1/seller/restaurant/{id}/courier-company/add       — Firma davet
GET    /v1/seller/restaurant/{id}/courier-companies         — Anlaşmalı firmalar
DELETE /v1/seller/restaurant/{id}/courier-company/{companyId} — Anlaşma sonlandır

# Restoran ↔ Firma onay (CourierCompanyAdmin role)
GET    /v1/courier/company/restaurant-invites               — Restoran davetleri
PUT    /v1/courier/company/restaurant-invites/{id}/accept
PUT    /v1/courier/company/restaurant-invites/{id}/reject

# Sipariş teslim alma (Courier + CourierCompanyAdmin)
GET    /v1/courier/pickup/{restaurantId}/pending?page=1&size=20  — Bekleyen siparişler (paginated, ORDER BY CreatedDate ASC)
PUT    /v1/courier/pickup/{orderId}/confirm                       — Teslim aldım

# Admin
GET    /v1/admin/courier-companies                     — Firma listesi
PUT    /v1/admin/courier-companies/{id}/approve        — Firma onay
PUT    /v1/admin/courier-companies/{id}/reject         — Firma red
PUT    /v1/admin/courier-companies/{id}/suspend        — Firma askıya al
PUT    /v1/admin/courier-companies/{id}/ban            — Firma yasakla
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

### 3.3 Sipariş Limiti Kapsamı

Sipariş limiti **restoran başına** sayılır. Her restoranın kendi Subscription'ı ve kendi SubscriptionUsage'ı vardır.

Örnek: Satıcı A, 3 restorana sahip, hepsi Pro pakette (500/ay):
- Restoran 1 → 320/500 sipariş
- Restoran 2 → 480/500 sipariş (uyarı!)
- Restoran 3 → 50/500 sipariş

Her restoranın sayacı bağımsızdır.

### 3.4 Akışlar

#### 3.4.1 Paket Seçimi
- Satıcı paketleri görür (sipariş limitleri dahil)
- Paket seçer → mevcut ödeme akışı ile satın alır
- SubscriptionUsage o ay için OrderCount=0 ile oluşur

#### 3.4.2 Sipariş Sayacı — Atomik Increment

Eşzamanlı sipariş durumunda race condition'ı önlemek için Dapper ile raw SQL kullanılır:

```sql
UPDATE SubscriptionUsages
SET OrderCount = OrderCount + 1, UpdatedAt = @now
WHERE SubscriptionId = @subscriptionId
  AND Year = @year AND Month = @month
  AND OrderCount < @maxOrders;
-- affected rows = 0 ise limit dolmuş demektir
```

Eğer ilgili ay için SubscriptionUsage kaydı yoksa (yeni ay başlangıcı), önce get-or-create pattern ile kayıt oluşturulur:

```sql
INSERT INTO SubscriptionUsages (Id, SubscriptionId, Year, Month, OrderCount, UpdatedAt)
VALUES (@id, @subscriptionId, @year, @month, 0, @now)
ON DUPLICATE KEY UPDATE OrderCount = OrderCount;
-- ON DUPLICATE KEY UPDATE: sadece duplicate key hatasini yutulur,
-- FK violation gibi gercek hatalar yine firlatilir (INSERT IGNORE'dan farki budur)
```

Ardından atomik increment çalıştırılır. Bu sayede background job scheduler olmadan da yeni ay kaydı lazy olarak oluşur.

#### 3.4.3 Limit Aşımı
- **Block**: Sipariş limiti dolunca restoran yeni sipariş alamaz. Müşteri app'te restoran listesinden gizlenmez ama sipariş verilmeye çalışıldığında "Bu restoran sipariş limitine ulaştı, lütfen daha sonra tekrar deneyin" mesajı gösterilir. Satıcıya "Limitiniz doldu, üst pakete geçin" uyarısı.
- **AutoUpgrade**: Otomatik üst pakete geçiş teklifi → satıcı onaylarsa upgrade, onaylamazsa block.

#### 3.4.4 Paket Yükseltme
- Satıcı panelinden "Paket Yükselt" → üst paketler listelenir
- Kalan günler için fark hesaplanır (gerçek gün sayısı): `(üstPaketFiyat - mevcutFiyat) × (kalanGün / toplamGünSayısı)`
  - `toplamGünSayısı` = Subscription.EndDate - Subscription.StartDate
- Ödeme alınır → Subscription.SubscriptionPlanId güncellenir
- SubscriptionUsage aynı kalır (sayaç sıfırlanmaz)
- Faz 1'de downgrade desteklenmez. Satıcı mevcut dönem bitince daha düşük paketi seçebilir.

#### 3.4.5 Ay Sonu / Yenileme
- EndDate gelince → mevcut expire check devam eder
- Satıcı manuel yeniler (auto-renewal Faz 2'de)
- Yeni ay SubscriptionUsage kaydı lazy olarak oluşur (ilk sipariş anında)

### 3.5 Order Flow Etkisi

`OrderManager.CreateOrder` içinde kontrol:
1. Siparişin RestaurantId'sine bağlı aktif Subscription'ı bul
2. Subscription → SubscriptionPlan.MaxOrdersPerMonth al
3. SubscriptionUsage atomik increment dene (Dapper raw SQL)
4. affected rows = 0 → limit dolmuş → OverageAction'a göre block veya upgrade teklifi
5. affected rows = 1 → sipariş devam eder

### 3.6 API Endpoint'leri

```
GET  /v1/subscription/plans                                      — MaxOrdersPerMonth bilgisi de döner
GET  /v1/subscription/usage?restaurantId={id}                    — bu ayki kullanım (73/100)
POST /v1/subscription/upgrade                                     — paket yükseltme (body: { restaurantId, targetPlanId })
GET  /v1/subscription/upgrade/preview?restaurantId={id}&planId={id} — yükseltme fiyat önizleme
```

### 3.7 Seller Panel Değişiklikleri

- Dashboard'a "Sipariş Kullanımı" widget'ı: progress bar + kalan gün (restoran başına)
- %80'de sarı uyarı, %100'de kırmızı uyarı + "Yükselt" butonu
- Abonelik sayfasına paket karşılaştırma tablosu

---

## 4. iyzico Alt Üye İşyeri Otomatik Kaydı

### 4.1 Mevcut Durum ve Değişiklik

**Mevcut davranış:** `SellerManager.ConfirmSeller()` zaten `IyzicoServiceAdapter.CreateSeller()` çağırıyor ama hata sessizce yutulup satıcı yine de onaylanıyor.

**Yeni davranış (breaking change):** iyzico kaydı başarısız olursa satıcı onaylanmaz. Bu, mevcut davranıştan farklıdır.

**Mevcut veriler için migration:** Halihazırda onaylı olup subMerchantKey'i olmayan satıcılar için admin panelinde "iyzico Kaydı Eksik" uyarısı gösterilir ve "iyzico Kaydını Oluştur" butonu eklenir.

### 4.2 Seller Tablosuna Ekleme

```
- IdentityNumber (string?, nullable) — bireysel satıcılar için TC kimlik no
```

### 4.3 CompanyType Enum Kullanımı

Mevcut `CompanyTypeEnums` korunur: `Individual = 1`, `Company = 2`.

iyzico mapping:
- `Individual (1)` → `subMerchantType = "PERSONAL"` → IdentityNumber kullanılır
- `Company (2)` → `subMerchantType = "LIMITED_OR_JOINT_STOCK_COMPANY"` → TaxCode kullanılır

> **Breaking change:** Mevcut kod Individual tipi için `PRIVATE_COMPANY` kullanıyor, bu `PERSONAL` olarak değiştirilecek. PERSONAL tipi için `taxOffice` ve `legalCompanyTitle` gönderilmez, sadece `identityNumber`, `iban`, `contactName`, `contactSurname`, `email` yeterlidir. iyzico sandbox'ta PERSONAL tipi test edilmelidir.

UI'da Limited/Anonim ayrımı gösterilmez çünkü iyzico'da ikisi de aynı tipe map edilir.

### 4.4 DTO Güncelleme

`CreateSubMerchantDto`'ya eklenen:
```
- IdentityNumber (string?) — bireysel satıcılar için TC kimlik no (TaxCode'dan ayrı)
```

Guncellenmesi gereken tum DTO'lar:
- `CreateSubMerchantDto` → + IdentityNumber
- Seller registration request DTO → + IdentityNumber (Individual secildiginde zorunlu)
- Seller update request DTO → + IdentityNumber
- Admin seller detail response DTO → + IdentityNumber (masked: "***12345678")

`IyzicoServiceAdapter` güncellenir:
- `Individual` tipi için `IdentityNumber` alanı kullanılır (mevcut kod TaxCode'u IdentityNumber olarak gönderiyor, bu düzeltilir)
- Seller address bilgisi: mevcut kodda hardcoded "Test Adres" gönderiliyor. Seller'ın kayıtlı adres bilgisi (User → Address tablosundan default adres) kullanılacak. Adres yoksa seller kayıt formunda adres zorunlu hale getirilecek.

### 4.5 Veri Saklama

subMerchantKey mevcut `SellerDetail` pattern'i ile saklanır (mevcut kodda zaten böyle):
```
- SellerDetail.Key1 = "PaymentSubMerchantKey"
- SellerDetail.Key2 = "Iyzico"
- SellerDetail.Value = subMerchantKey değeri
```

> Not: Spec'in önceki versiyonunda UserExternalInfo yazıyordu, bu yanlıştı. SellerDetail doğru lokasyondur.

### 4.6 Akış

#### 4.6.1 Satıcı Kayıt (Eksik Bilgi Tamamlama)

Kayıt formuna eklenen validasyonlar:
- CompanyType seçimi: Bireysel / Şirket
- Bireysel → IdentityNumber zorunlu (11 haneli TC kimlik)
- Şirket → TaxCode, TaxArea zorunlu (zaten var)
- IBAN zorunlu (zaten var)
- LegalName zorunlu (zaten var)

iyzico'nun gerektirdiği tüm alanlar dolmadan kayıt tamamlanamaz.

#### 4.6.2 Admin Onay Akışı

```
Admin "Onayla" butonuna basar
    ↓
Backend: Seller bilgileri iyzico formatına map edilir
    ↓
CompanyType'a göre:
  - Bireysel (1) → subMerchantType = "PERSONAL"
    → identityNumber (Seller.IdentityNumber), iban, contactName, contactSurname, email
  - Şirket (2) → subMerchantType = "LIMITED_OR_JOINT_STOCK_COMPANY"
    → taxOffice, legalCompanyTitle, taxNumber (Seller.TaxCode), iban, email
    ↓
IyzicoServiceAdapter.CreateSeller() çağrılır
    ↓
Başarılı:
  → subMerchantKey SellerDetail'a kaydedilir
  → Seller.CompanyStatus = Approved
  → Admin'e başarı mesajı
    ↓
Başarısız:
  → Seller.CompanyStatus değişmez (Pending kalır)
  → Hata mesajı admin'e gösterilir
  → Hata log'a yazılır
  → Admin tekrar deneyebilir
```

**Retry idempotency:** "Tekrar Dene" butonuna basıldığında, önce mevcut subMerchantKey kontrol edilir (SellerDetail'da var mı?). Varsa CreateSeller yerine UpdateSeller çağrılır. Yoksa CreateSeller çağrılır, ancak iyzico "already exists" hatası dönerse UpdateSeller'a fallback yapılır.

#### 4.6.3 Ödeme Akışında Kullanım

- Müşteri sipariş verince → PaymentManager satıcının subMerchantKey'ini SellerDetail'dan çeker
- iyzico'ya ödeme gönderilirken subMerchantKey ve subMerchantPrice (komisyon düşülmüş tutar) eklenir
- iyzico otomatik olarak satıcıya payout yapar

#### 4.6.4 Güncelleme Akışı

- Satıcı bilgilerini değiştirirse (IBAN, ticari ünvan vs.)
- Backend IyzicoServiceAdapter.UpdateSeller() çağırır
- subMerchantKey aynı kalır, bilgiler güncellenir
- iyzico güncelleme başarısız olursa: satıcı profili lokal olarak güncellenir, iyzico hatası loglanır, admin panelinde "iyzico Sync Gerekli" uyarısı gösterilir

### 4.7 Admin Panel Değişiklikleri

- Satıcı detay sayfasında "iyzico Durumu" badge'i:
  - **Kayıtlı** (yeşil) — subMerchantKey mevcut
  - **Kayıtsız** (gri) — henüz onaylanmamış
  - **Hatalı** (kırmızı) — son deneme başarısız + hata mesajı
  - **Sync Gerekli** (turuncu) — profil güncellendi ama iyzico sync edilemedi
- "Onayla" butonu basıldığında loading state + sonuç feedback
- Hata durumunda "Tekrar Dene" butonu
- Mevcut onaylı ama subMerchantKey'i olmayan satıcılar için "iyzico Kaydı Oluştur" butonu

### 4.8 API Endpoint'leri

```
PUT  /v1/admin/sellers/{id}/approve         — iyzico kaydı da yapar (blocking)
PUT  /v1/admin/sellers/{id}/retry-iyzico    — iyzico kaydı tekrar dene
GET  /v1/admin/sellers/{id}                 — iyzico durumu bilgisi de döner
PUT  /v1/seller/profile                     — bilgi güncellenince iyzico'ya da sync
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
- `POST /v1/courier/location` → CourierLocation tablosuna upsert (CourierId bazında, son konum)
- Sipariş teslim edilince konum takibi durur
- Konum endpoint'i rate limiting'den muaf tutulur (ayrı rate limit: 10 req/15s per courier)

### 5.5 Müşteri Tarafında Harita (front-app)

OrderDetail ekranında sipariş "Yola Çıktı" durumundayken:
- Harita görünür (react-native-maps)
- Kurye konumu: mavi pin (30 saniyede bir polling ile güncellenir)
- Teslimat adresi: kırmızı pin
- Tahmini süre: `mesafe_km / 25 * 60` dakika (25 km/h ortalama hız varsayımı)
- Kurye bilgisi: isim + telefon (arama butonu)

**Kurye tespiti:** Endpoint `Order.CourierId ?? Order.PickedUpByCourierId` ile kuryeyi bulur, ardından `CourierLocation` tablosundan son konumu döner. Her iki akış (bireysel + kurumsal) için çalışır.

> Not: Real-time WebSocket Faz 2'de gelecek. Faz 1'de polling ile çalışır. Worst-case konum gecikmesi ~45 saniye (15s gönderim + 30s polling).

### 5.6 Çoklu Sipariş Navigasyonu

- Aktif siparişler listesinde her sipariş ayrı kart
- Kurye istediği sırayla teslim edebilir (rota optimizasyonu Faz 1'de yok)
- Her sipariş için ayrı navigasyon butonu
- "Teslim Ettim" → sipariş listeden düşer
- Son sipariş teslim edilince konum takibi durur

### 5.7 Sipariş Teslim Onayı

```
Kurye "Teslim Ettim" → Order.StatusId = Delivered, DeliveredAt = now
→ Konum takibi durur (o orderId için)
→ Müşteri tarafında "Teslim Edildi" olarak güncellenir
```

**Yetki doğrulaması:** Deliver endpoint'i çağrıldığında, istekte bulunan kurye'nin userId'si `Order.CourierId` veya `Order.PickedUpByCourierId` ile eşleşmeli. Eşleşmezse 403 Forbidden döner.

> Not: Kurye teslim yetkisi yeni bir trust model değişikliğidir. Mevcut durumda sadece seller status değiştirebiliyordu. Faz 1'de sadece Delivered statüsü kurye tarafından set edilebilir, diğer tüm geçişler seller'da kalır.

### 5.8 API Endpoint'leri

```
POST /v1/courier/location                          — mevcut, değişiklik yok (rate limit ayrı)
PUT  /v1/courier/orders/{orderId}/deliver           — teslim ettim (sadece Courier/CourierCompanyAdmin)
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
- `RestaurantCourierCompanies`
- `SubscriptionUsages`

### Mevcut Tablo Güncellemeleri
- `Sellers` → + IdentityNumber (nullable)
- `SubscriptionPlans` → + MaxOrdersPerMonth, OverageAction
- `Orders` → + CourierCompanyId, PickedUpByCourierId, PickedUpAt, DeliveredAt

### Migration Stratejisi
- `SubscriptionPlans.MaxOrdersPerMonth` default = 2147483647 (int.MaxValue) → mevcut planlar sınırsız olarak başlar
- `SubscriptionPlans.OverageAction` default = 1 (Block)
- `Orders.PickedUpByCourierId`, `PickedUpAt`, `DeliveredAt` nullable → mevcut siparişler etkilenmez
- Mevcut delivered siparişler için `DeliveredAt` backfill yapılmaz (sadece yeni siparişler için)
- Migration sırası: 1) Yeni tablolar, 2) Seller.IdentityNumber, 3) SubscriptionPlan alanları, 4) Order alanları

### Yeni Enum'lar
- `CourierCompanyStatusEnums`: PendingApproval=1, Active=2, Suspended=3, Banned=4
- `CourierCompanyMemberStatusEnums`: PendingApproval=1, Active=2, RemovedByCompany=3, LeftByChoice=4
- `RestaurantCourierCompanyStatusEnums`: PendingApproval=1, Active=2, TerminatedByRestaurant=3, TerminatedByCompany=4
- `OverageActionEnums`: Block=1, AutoUpgrade=2

### Yeni User Role
- `CourierCompanyAdmin = 7` — firma yöneticisi rolü (Courier yetkilerini kapsar)

### Yetkilendirme Güncelleme Checklist
CourierCompanyAdmin rolünün kabul edilmesi gereken mevcut controller'lar:
- `CourierAuthController` — login/register/profile
- `CourierOrderController` — active orders, history
- `CourierLocationController` — location update
- `CourierRestaurantController` (mevcut) — restaurant invites, accept/reject
- Tüm bu controller'lardaki `[AuthorizeAPIRequest]` role check'leri `Courier || CourierCompanyAdmin` olarak güncellenir

---

## 7. Proje Tarama Sonuçları (Faz 2-3 için)

### Faz 2 — Kritik Altyapı
- Odeme iadesi (Refund) implementasyonu
- Push bildirimler (Firebase Cloud Messaging)
- Real-time guncellemeler (SignalR)
- Refresh token mekanizmasi
- Sifre sifirlama
- Background job scheduler (Hangfire)
- Seller payout/settlement sistemi
- Auto-renewal abonelik

### Faz 3 — UX Gelistirmeleri
- Yorum/degerlendirme sistemi
- Restoran arama/filtreleme (isim, mutfak, fiyat, puan)
- Menu gorselleri upload
- Favori restoranlar
- Tekrar siparis (reorder)
- Kurye kazanc takibi
- Kurye online/offline toggle
- Admin finansal raporlar
- Seller analytics dashboard
- Siparis zaman asimi
