# FoodOrder.Backend

Çok satıcılı yemek sipariş platformu. Müşteriler çevrelerindeki restoranları keşfedip sipariş verebilir. Her siparişte yalnızca 1 restoran bulunabilir. Platformun gelir modeli komisyon tabanlıdır — sipariş başına komisyon (oran + sabit ücret) alınır. Platform varsayılanı PlatformCommissionSchedule, restoran bazlı özel oran RestaurantCommission ile yönetilir. Aylık abonelik yoktur.

## Mimari

Clean Architecture — 6 katman:

| Katman | Açıklama |
|--------|----------|
| `Domain` | Entity'ler, DTO'lar, Enum'lar, ServiceResult |
| `Application` | Business logic (Manager pattern), AutoMapper, FluentValidation |
| `Persistence` | EF Core, Repository pattern, UnitOfWork |
| `Infrastructure` | Harici adaptörler (Iyzico, AWS S3, Getir, YemekSepeti) |
| `WebAPI` | Controller'lar, middleware, Program.cs |
| `Base` | Paylaşılan security, sabitler, enum'lar |

## Teknoloji Stack

- .NET 8, ASP.NET Core Web API
- MySQL (Pomelo EF Core 8)
- Redis (StackExchange.Redis)
- BCrypt.Net-Next (şifre hashing)
- AutoMapper 14
- FluentValidation
- Dapper (kompleks SQL sorgular için)
- NArchitecture.Core (repository base, security, exception handling)
- Iyzico (ödeme entegrasyonu)
- AWS S3 / Cloudflare R2 (dosya storage)

## Domain Modülleri

### Seller
- `Seller` → `Restaurant` → `Category` → `Menu` → `MenuOption` → `MenuOptionValue` → `MenuOptionValueOption` → `MenuOptionValueOptionValue`
- `RestaurantCommission` — restoran bazlı komisyon oranı geçmişi (EffectiveFrom/To)
- `SettlementPeriod` → `SettlementItem` — günlük hakediş dönemleri ve kalemleri

### Buyer
- `Basket` → `BasketItem` → `BasketItemValue` → `BasketItemValueItemValue`
- `Order` → `OrderItem` (siparişler, tek restoran per sipariş)

### Courier
- `CourierCompany` — kurye firması (legal bilgiler, vergi, IBAN)
- `Courier` — kurye profili (tip: RestaurantOwn/Individual/CompanyMember, araç, konum, rating)
- `RestaurantCourierAgreement` — restoran-kurye/firma anlaşması (ücret, strateji, öncelik)
- `DeliveryAssignment` — teslimat ataması (sipariş-kurye eşleşme, yaşam döngüsü)
- `CourierEarning` — kurye kazancı (teslimat ücreti, bahşiş, bonus)
- `CourierLocationHistory` — konum geçmişi (analitik)

### Common
- `User` (Admin/SellerAdmin/SellerUser/User rolleri)
- `Address`
- `Cuisine`
- `PlatformCommissionSchedule` — platform geneli varsayılan komisyon tarifesi

## Güvenlik Standartları

- **Şifreler**: BCrypt.Net-Next (workFactor: 12) — geri dönüşümsüz hash
- **Token**: Custom encrypted JSON (CryptoManagerV3/Rijndael) — Authorization header'da Bearer token
- **SQL**: Parameterized queries (Dapper `@param` syntax) — string interpolation YOK
- **Authorization**: `[AuthorizeAPIRequestAttribute]` action filter — 401/403 HTTP status codes döner
- **Rate Limiting**: 60 req/dk (genel), 10 req/dk (auth endpoints)
- **CORS**: Sadece `WebAPIConfiguration:AllowedOrigins`'daki domainlere izin verilir

## Komisyon İş Mantığı

1. Admin `PlatformCommissionSchedule` ile platform varsayılan oranını yönetir (tarih bazlı planlama)
2. Restoran bazlı özel oran `RestaurantCommission` ile tanımlanır; yoksa platform varsayılanı uygulanır
3. Sipariş tutarı üzerinden komisyon hesaplanır; günlük hakediş `SettlementPeriod`/`SettlementItem` ile satıcıya dağıtılır
4. Satıcı onayında (`ConfirmSeller`) İyzico alt üye işyeri (SubMerchant) kaydı yapılır — BLOCKING

## Sipariş İş Mantığı

1. Müşteri `POST /v1/order/place` ile sipariş verir
2. Kontroller: restoran aktif mi, açık mı, minimum tutar karşılandı mı
3. Sipariş `Pending` statüsünde oluşur
4. Satıcı `PUT /v1/order/{id}/status` ile statü günceller
5. Müşteri `POST /v1/order/{id}/cancel` ile `Pending`/`Confirmed` siparişi iptal edebilir
6. Sipariş `Preparing` statüsüne geçtiğinde kurye atama tetiklenir (RestaurantCourierAgreement'a göre)
7. Kurye kabul → `CourierAssigned(9)` → Teslim aldı → `CourierPickedUp(10)` → `OnTheWay(7)` → `Delivered(8)`
8. Kurye sistemi olmayan restoranlar eski akışla çalışmaya devam eder: Preparing → OnTheWay → Delivered

## Restoran Keşfi (Location)

- `Restaurant.Latitude`, `Restaurant.Longitude` — restoranın koordinatı
- `Restaurant.ServiceAreaPolygonWkt` — teslimat alanı (WKT polygon string)
- `GetRestaurantsByPolygon`: Dapper raw SQL + MySQL `ST_Contains(servis_alani_polygon, POINT(@lng, @lat))`
- Grid cache: Redis'te 15dk TTL, `grid_{latIdx}_{lngIdx}` key formatı

## Önemli Dosyalar

```
WebAPI/Program.cs                                           — startup, middleware, DI
WebAPI/appsettings.json                                     — config (secrets ENV'den gelmeli!)
WebAPI/Helpers/AuthorizationAPIRequest.cs                   — custom auth filter
Application/ApplicationServiceRegistration.cs               — uygulama DI kayıtları
Persistence/PersistenceServiceRegistration.cs               — persistence DI kayıtları
Persistence/IRepositories/IUnitOfWork.cs                    — UoW interface
Application/Services/Common/UserService/UserManager.cs      — login/register (BCrypt)
Application/Services/Seller/2_RestaurantService/            — restoran CRUD + lokasyon
Application/Services/Buyer/OrderService/OrderManager.cs     — sipariş iş mantığı
Application/Services/Seller/CommissionService/              — komisyon çözümleme iş mantığı
Domain/Entities/Seller/Restaurant.cs                        — IsActive, IsOpen, konum alanları
Domain/Entities/Seller/RestaurantCommission.cs              — restoran bazlı oran geçmişi
Domain/Entities/Common/PlatformCommissionSchedule.cs        — platform varsayılan tarifesi
Domain/Entities/Buyer/Order.cs                              — sipariş entity
Domain/Entities/Buyer/OrderItem.cs                          — sipariş kalemi entity
Application/Services/Courier/CourierService/                — kurye kayıt, profil, online/offline
Application/Services/Courier/DeliveryAssignmentService/     — teslimat atama, kabul/red, teslim
Application/Services/Courier/CourierCompanyService/          — firma kayıt, üye yönetimi
Application/Services/Courier/CourierEarningService/          — kazanç hesaplama, ödeme
WebAPI/Controllers/Courier/                                 — kurye ve firma API'leri
WebAPI/Controllers/Seller/SellerCourierController.cs        — satıcı kurye yönetimi
WebAPI/Controllers/Admin/AdminCourierController.cs          — admin kurye yönetimi
WebAPI/Hubs/CourierHub.cs                                   — kurye SignalR hub
```

## API Endpoint Özeti

| Metod | Endpoint | Yetki | Açıklama |
|-------|----------|-------|----------|
| POST | /v1/user/register | - | Kullanıcı kayıt |
| POST | /v1/user/login | - | Giriş + token |
| GET | /v1/seller/restaurant/list | SellerAdmin/SellerUser | Satıcının restoranları |
| POST | /v1/seller/restaurant/add | SellerAdmin | Restoran ekle |
| GET | /v1/buyer/restaurants | User | Konuma göre restoranlar |
| GET | /v1/buyer/restaurant/{id} | User | Restoran menüsü |
| POST | /v1/order/place | User | Sipariş ver |
| GET | /v1/order/history | User | Sipariş geçmişi |
| POST | /v1/order/{id}/cancel | User | Sipariş iptal |
| PUT | /v1/order/{id}/status | SellerAdmin/Admin | Statü güncelle |
| GET | /v1/order/restaurant/{id} | SellerAdmin/Admin | Restoran siparişleri |
| POST | /v1/seller/register | - | Restoran self-servis başvuru (anonim) |
| POST | /v1/admin/seller/confirm | Admin | Başvuru onayı → İyzico alt üye kaydı |
| POST | /v1/courier/register | User | Kurye kayıt (admin onayı gerekir) |
| POST | /v1/courier/go-online | Courier | Online ol |
| POST | /v1/courier/go-offline | Courier | Offline ol |
| PUT | /v1/courier/location | Courier | Konum güncelle |
| POST | /v1/courier/assignment/{id}/accept | Courier | Teslimatı kabul et |
| POST | /v1/courier/assignment/{id}/picked-up | Courier | Teslim aldım |
| POST | /v1/courier/assignment/{id}/delivered | Courier | Teslim ettim |
| POST | /v1/courier-company/register | User | Firma kayıt |
| GET | /v1/seller/courier/restaurant/{id}/agreements | SellerAdmin | Anlaşma listesi |
| POST | /v1/seller/courier/restaurant/{id}/assign/{orderId} | SellerAdmin | Manuel kurye ata |
| GET | /v1/admin/courier/companies | Admin | Firma listesi |
| PUT | /v1/admin/courier/couriers/{id}/approve | Admin | Kurye onayla |
| GET | /v1/customer/order/{id}/tracking | User | Canlı kurye takip |

## Geliştirme Notları

- Yeni entity eklerken: Entity → Configuration → IRepository → Repository → UnitOfWork → DI kayıt
- SQL injection riski: Raw SQL'de her zaman `@paramName` kullan, string interpolation YASAK
- Şifre asla plain text veya simetrik şifreleme ile saklanmaz — BCrypt zorunlu
- Exception detayları production'da client'a yansımamalı (`KeepRawException` = false)
- `appsettings.json` dosyasına credential girme — environment variable veya user-secrets kullan
