# FoodOrder.Backend

Çok satıcılı yemek sipariş platformu. Müşteriler çevrelerindeki restoranları keşfedip sipariş verebilir. Her siparişte yalnızca 1 restoran bulunabilir. Platformun gelir modeli abonelik tabanlıdır — restoran başına aylık ücret alınır, sipariş başına komisyon alınmaz.

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
- `SubscriptionPlan` — abonelik planları (Basic/Pro/Premium)
- `Subscription` — restaurant-plan ilişkisi, abonelik durumu ve tarihleri

### Buyer
- `Basket` → `BasketItem` → `BasketItemValue` → `BasketItemValueItemValue`
- `Order` → `OrderItem` (siparişler, tek restoran per sipariş)

### Common
- `User` (Admin/SellerAdmin/SellerUser/User rolleri)
- `Address`
- `Cuisine`

## Güvenlik Standartları

- **Şifreler**: BCrypt.Net-Next (workFactor: 12) — geri dönüşümsüz hash
- **Token**: Custom encrypted JSON (CryptoManagerV3/Rijndael) — Authorization header'da Bearer token
- **SQL**: Parameterized queries (Dapper `@param` syntax) — string interpolation YOK
- **Authorization**: `[AuthorizeAPIRequestAttribute]` action filter — 401/403 HTTP status codes döner
- **Rate Limiting**: 60 req/dk (genel), 10 req/dk (auth endpoints)
- **CORS**: Sadece `WebAPIConfiguration:AllowedOrigins`'daki domainlere izin verilir

## Abonelik İş Mantığı

1. Admin `SubscriptionPlan` oluşturur
2. SellerAdmin `POST /v1/subscription/subscribe` ile restoran için plan satın alır
3. Abonelik aktif olunca `Restaurant.IsActive = true` set edilir
4. Abonelik süresi dolunca (cron/endpoint: `POST /v1/subscription/expire-check`) `Restaurant.IsActive = false`
5. Aktif aboneliği olmayan restoranlar müşteri listesinde görünmez ve sipariş kabul etmez

## Sipariş İş Mantığı

1. Müşteri `POST /v1/order/place` ile sipariş verir
2. Kontroller: restoran aktif mi, açık mı, aktif aboneliği var mı, minimum tutar karşılandı mı
3. Sipariş `Pending` statüsünde oluşur
4. Satıcı `PUT /v1/order/{id}/status` ile statü günceller
5. Müşteri `POST /v1/order/{id}/cancel` ile `Pending`/`Confirmed` siparişi iptal edebilir

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
Application/Services/Seller/SubscriptionService/            — abonelik iş mantığı
Domain/Entities/Seller/Restaurant.cs                        — IsActive, IsOpen, konum alanları
Domain/Entities/Seller/SubscriptionPlan.cs                  — abonelik planı entity
Domain/Entities/Seller/Subscription.cs                      — abonelik entity
Domain/Entities/Buyer/Order.cs                              — sipariş entity
Domain/Entities/Buyer/OrderItem.cs                          — sipariş kalemi entity
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
| GET | /v1/subscription/plans | - | Planları listele |
| POST | /v1/subscription/plans | Admin | Plan oluştur |
| POST | /v1/subscription/subscribe | SellerAdmin | Abone ol |
| POST | /v1/subscription/{id}/cancel | SellerAdmin | Abonelik iptal |
| GET | /v1/subscription/my | SellerAdmin/SellerUser | Kendi abonelikleri |
| POST | /v1/subscription/expire-check | Admin | Süresi dolmuş abonelikleri kapat |

## Geliştirme Notları

- Yeni entity eklerken: Entity → Configuration → IRepository → Repository → UnitOfWork → DI kayıt
- SQL injection riski: Raw SQL'de her zaman `@paramName` kullan, string interpolation YASAK
- Şifre asla plain text veya simetrik şifreleme ile saklanmaz — BCrypt zorunlu
- Exception detayları production'da client'a yansımamalı (`KeepRawException` = false)
- `appsettings.json` dosyasına credential girme — environment variable veya user-secrets kullan
- Abonelik kontrolünü atlamak için `HasActiveSubscription` kontrolü mutlaka `PlaceOrder`'da kalmalı
