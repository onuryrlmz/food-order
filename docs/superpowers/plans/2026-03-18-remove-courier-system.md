# Kurye Sistemi Kaldırma Planı

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Tüm kurye sistemini (backend entities, services, controllers, repositories, frontend courier app, seller/admin kurye sayfaları) tamamen kaldırıp, sipariş sistemindeki kurye atamasını pasif bırakmak.

**Architecture:** Silme işlemi katmanlı yapılacak: önce frontend uygulamaları (front-courier, front-seller courier sayfaları, front-admin courier sayfaları), sonra backend controller'lar, servisler, repository'ler, entity'ler ve son olarak DI registration, DbContext ve migration temizliği.

**Tech Stack:** .NET 8, Entity Framework Core (MySQL), React (Next.js), React Native

---

## Dosya Haritası

### Silinecek Dizinler (Komple)
- `front-courier/` — Tüm kurye mobil uygulaması
- `backend/Domain/Entities/Courier/` — Tüm kurye entity'leri
- `backend/Domain/Dto/Courier/` — Tüm kurye DTO'ları
- `backend/Domain/Dto/Seller/Courier/` — Seller kurye DTO'ları
- `backend/Domain/Dto/Admin/Courier/` — Admin kurye DTO'ları
- `backend/Application/Services/Courier/` — Tüm kurye servisleri
- `backend/Application/Services/Seller/CourierService/` — Seller kurye servisleri
- `backend/Application/Services/Admin/CourierService/` — Admin kurye servisleri
- `backend/WebAPI/Controllers/Courier/` — Tüm kurye controller'ları
- `backend/Persistence/IRepositories/Courier/` — Kurye repository interface'leri
- `backend/Persistence/Repositories/Courier/` — Kurye repository implementasyonları
- `backend/Persistence/EntityConfigurations/Courier/` — Kurye entity configuration'ları
- `front-seller/app/restaurants/[id]/couriers/` — Seller kurye sayfası
- `front-seller/app/restaurants/[id]/courier-companies/` — Seller kurye firma sayfası
- `front-admin/app/couriers/` — Admin kurye sayfası
- `front-admin/app/courier-companies/` — Admin kurye firma sayfası

### Silinecek Migration Dosyaları
- `backend/Persistence/Migrations/20260315075036_kurye2.cs` + `.Designer.cs`
- `backend/Persistence/Migrations/20260315094951_1249.cs` + `.Designer.cs`
- `backend/Persistence/Migrations/20260315140925_FixRestaurantCourierFKToUser.cs` + `.Designer.cs`
- `backend/Persistence/Migrations/20260315141402_UpdateRestaurantCourierStatusFields.cs` + `.Designer.cs`
- `backend/Persistence/Migrations/20260315143044_CleanupOldCourierEntities.cs` + `.Designer.cs`
- `backend/Persistence/Migrations/20260316211851_0018.cs` + `.Designer.cs`
- `backend/Persistence/Migrations/20260316214357_17030043.cs` + `.Designer.cs`
- `backend/Persistence/Migrations/20260317201708_AddCourierCompanyTypeId.cs` + `.Designer.cs`
- `backend/Persistence/Migrations/20260317202650_170320262326.cs` + `.Designer.cs`

### Düzenlenecek Dosyalar
- `backend/Base/Enums/AuthorizationServiceEnums.cs` — Kurye enum'larını sil
- `backend/Persistence/Contexts/BaseDbContext.cs` — Kurye DbSet'lerini sil
- `backend/Persistence/IRepositories/IUnitOfWork.cs` — Kurye property'lerini sil
- `backend/Persistence/Repositories/UnitOfWork.cs` — Kurye property ve constructor parametrelerini sil
- `backend/Application/ApplicationServiceRegistration.cs` — Kurye servis kayıtlarını sil
- `backend/Persistence/PersistenceServiceRegistration.cs` — Kurye repository kayıtlarını sil
- `backend/Domain/Entities/Buyer/Order.cs` — CourierId, CourierCompanyId, PickedUpByCourierId property'lerini ve CourierCompany navigation property'sini sil
- `backend/Domain/Entities/Common/User.cs` — CourierStatusId property'sini sil
- `backend/Base/Entities/TokenDto.cs` — CourierId property'sini sil
- `backend/WebAPI/Controllers/Customer/CustomerOrderController.cs` — GetCourierLocation endpoint'ini sil
- `backend/WebAPI/Controllers/Base/UserController.cs` — Courier role referanslarını temizle
- `backend/WebAPI/Services/SignalRRealtimeNotifier.cs` — NotifyCourierLocationUpdated metodunu sil
- `backend/Application/Services/Common/NotificationService/IRealtimeNotifier.cs` — NotifyCourierLocationUpdated interface metodunu sil
- `front-app/src/screens/Orders/OrderDetailScreen.js` — Kurye konum takibi UI kodunu sil
- `front-app/src/api/orderService.js` — getCourierLocation API çağrısını sil
- `front-seller/app/orders/page.js` — Kurye atama modalını sil
- `front-seller/app/restaurants/[id]/page.js` — Kuryeler sayfası linkini sil
- `backend/Application/Services/Buyer/OrderService/IOrderService.cs` — AssignCourier ve GetCourierLocation metodlarını kaldır
- `backend/Application/Services/Buyer/OrderService/OrderManager.cs` — AssignCourier ve GetCourierLocation implementasyonlarını kaldır, UpdateOrderStatus'tan courier parametrelerini kaldır
- `backend/WebAPI/Controllers/Seller/SellerOrderController.cs` — Courier parametrelerini kaldır
- `backend/WebAPI/Controllers/Seller/SellerCourierController.cs` — Tamamen sil
- `backend/WebAPI/Controllers/Seller/SellerCourierCompanyController.cs` — Tamamen sil
- `backend/WebAPI/Controllers/Admin/AdminCourierController.cs` — Tamamen sil
- `backend/WebAPI/Controllers/Admin/AdminCourierCompanyController.cs` — Tamamen sil
- `front-seller/lib/api.js` — Kurye API fonksiyonlarını sil
- `front-seller/components/layout/Sidebar.js` — Kurye menü linklerini sil
- `front-admin/lib/api.js` — Kurye API fonksiyonlarını sil
- `front-admin/components/layout/Sidebar.js` — Kurye menü linklerini sil
- `backend/Persistence/Migrations/BaseDbContextModelSnapshot.cs` — Kurye tablolarını sil (veya migration sonrası yeniden oluştur)

---

### Task 1: Frontend Courier Uygulamasını Sil

**Files:**
- Delete: `front-courier/` (tüm dizin)

- [ ] **Step 1: front-courier dizinini sil**

```bash
rm -rf front-courier/
```

- [ ] **Step 2: Commit**

```bash
git add -A front-courier/
git commit -m "chore: remove entire front-courier application"
```

---

### Task 2: Frontend Seller — Kurye Sayfalarını ve API'lerini Kaldır

**Files:**
- Delete: `front-seller/app/restaurants/[id]/couriers/page.js`
- Delete: `front-seller/app/restaurants/[id]/courier-companies/page.js`
- Modify: `front-seller/lib/api.js` — Kurye fonksiyonlarını sil
- Modify: `front-seller/components/layout/Sidebar.js` — Kurye menü linklerini sil
- Modify: `front-seller/app/orders/page.js` — Kurye atama modalını kaldır
- Modify: `front-seller/app/restaurants/[id]/page.js` — Kuryeler sayfası linkini kaldır

- [ ] **Step 1: Seller kurye sayfalarını sil**

```bash
rm -rf front-seller/app/restaurants/\[id\]/couriers/
rm -rf front-seller/app/restaurants/\[id\]/courier-companies/
```

- [ ] **Step 2: `front-seller/lib/api.js` dosyasından kurye fonksiyonlarını sil**

Silinecek fonksiyonlar:
- `addCourier`
- `getRestaurantCouriers`
- `removeCourier`
- `assignCourierToOrder`
- `getCourierLocation`
- `getCourierOrderHistory`
- `getRestaurantCourierCompanies`
- `inviteCourierCompany` / `addCourierCompany`
- `removeCourierCompany`
- `searchCourierCompanies`

- [ ] **Step 3: `front-seller/components/layout/Sidebar.js` dosyasından kurye menü linklerini sil**

"Kuryeler" ve "Kurye Firmaları" submenu öğelerini kaldır.

- [ ] **Step 4: `front-seller/app/orders/page.js` dosyasından kurye atama modalını kaldır**

Kurye seçim modalı, `openCourierModal`, `confirmCourierAndSend`, courier/company import'ları ve ilgili state'leri sil. Sipariş durum güncelleme akışı courier olmadan çalışmaya devam etmeli.

- [ ] **Step 5: `front-seller/app/restaurants/[id]/page.js` dosyasından kuryeler linkini kaldır**

Kuryeler sayfasına yönlendiren link'i sil.

- [ ] **Step 6: Seller uygulamasının build olduğunu doğrula**

```bash
cd front-seller && npm run build
```

- [ ] **Step 7: Commit**

```bash
git add front-seller/
git commit -m "chore: remove courier pages and API calls from seller app"
```

---

### Task 3: Frontend Admin — Kurye Sayfalarını ve API'lerini Kaldır

**Files:**
- Delete: `front-admin/app/couriers/page.js`
- Delete: `front-admin/app/courier-companies/page.js`
- Modify: `front-admin/lib/api.js` — Kurye fonksiyonlarını sil
- Modify: `front-admin/components/layout/Sidebar.js` — Kurye menü linklerini sil

- [ ] **Step 1: Admin kurye sayfalarını sil**

```bash
rm -rf front-admin/app/couriers/
rm -rf front-admin/app/courier-companies/
```

- [ ] **Step 2: `front-admin/lib/api.js` dosyasından kurye fonksiyonlarını sil**

Silinecek fonksiyonlar:
- `getCourierCompanies`
- `approveCourierCompany`
- `rejectCourierCompany`
- `suspendCourierCompany`
- `banCourierCompany`

- [ ] **Step 3: `front-admin/components/layout/Sidebar.js` dosyasından kurye menü linklerini sil**

"Kuryeler" ve "Kurye Firmaları" menü öğelerini kaldır.

- [ ] **Step 4: Admin uygulamasının build olduğunu doğrula**

```bash
cd front-admin && npm run build
```

- [ ] **Step 5: Commit**

```bash
git add front-admin/
git commit -m "chore: remove courier pages and API calls from admin app"
```

---

### Task 3b: Frontend Buyer App — Kurye Konum Takibini Kaldır

**Files:**
- Modify: `front-app/src/screens/Orders/OrderDetailScreen.js` — Kurye konum takibi UI kodunu sil (courier location state, SignalR `CourierLocationUpdated` listener, harita gösterimi, kurye ad/telefon gösterimi)
- Modify: `front-app/src/api/orderService.js` — `getCourierLocation` API çağrısını sil

- [ ] **Step 1: `OrderDetailScreen.js`'den kurye konum takibi kodunu sil**

Silinecek öğeler:
- Courier location state değişkenleri
- SignalR `CourierLocationUpdated` event listener
- Kurye harita gösterimi
- Kurye adı/telefon gösterimi
- İlgili import'lar

- [ ] **Step 2: `orderService.js`'den `getCourierLocation` fonksiyonunu sil**

- [ ] **Step 3: Commit**

```bash
git add front-app/
git commit -m "chore: remove courier location tracking from buyer app"
```

---

### Task 4: Backend Controller'ları Sil

**Files:**
- Delete: `backend/WebAPI/Controllers/Courier/` (tüm dizin — 7 controller)
- Delete: `backend/WebAPI/Controllers/Seller/SellerCourierController.cs`
- Delete: `backend/WebAPI/Controllers/Seller/SellerCourierCompanyController.cs`
- Delete: `backend/WebAPI/Controllers/Admin/AdminCourierController.cs`
- Delete: `backend/WebAPI/Controllers/Admin/AdminCourierCompanyController.cs`
- Modify: `backend/WebAPI/Controllers/Seller/SellerOrderController.cs` — courierId/courierCompanyId parametrelerini kaldır
- Modify: `backend/WebAPI/Controllers/Customer/CustomerOrderController.cs` — GetCourierLocation endpoint'ini sil
- Modify: `backend/WebAPI/Controllers/Base/UserController.cs` — Courier role referanslarını temizle

- [ ] **Step 1: Courier controller dizinini sil**

```bash
rm -rf backend/WebAPI/Controllers/Courier/
```

- [ ] **Step 2: Seller ve Admin courier controller'larını sil**

```bash
rm backend/WebAPI/Controllers/Seller/SellerCourierController.cs
rm backend/WebAPI/Controllers/Seller/SellerCourierCompanyController.cs
rm backend/WebAPI/Controllers/Admin/AdminCourierController.cs
rm backend/WebAPI/Controllers/Admin/AdminCourierCompanyController.cs
```

- [ ] **Step 3: SellerOrderController'dan courier parametrelerini kaldır**

`UpdateOrderStatus` endpoint'indeki `courierId` ve `courierCompanyId` query parametrelerini kaldır. Metod çağrısında bu parametreleri `null` olarak geç.

- [ ] **Step 4: CustomerOrderController'dan GetCourierLocation endpoint'ini sil**

`GetCourierLocation` metodunu ve `using Domain.Dto.Seller.Courier` import'unu sil.

- [ ] **Step 5: UserController'dan Courier role referanslarını temizle**

Authorize attribute'larındaki `Courier` ve `CourierCompanyAdmin` role referanslarını kaldır.

- [ ] **Step 6: Commit**

```bash
git add backend/WebAPI/
git commit -m "chore: remove all courier controllers from backend"
```

---

### Task 5: Backend Servisleri Sil

**Files:**
- Delete: `backend/Application/Services/Courier/` (tüm dizin)
- Delete: `backend/Application/Services/Seller/CourierService/` (tüm dizin)
- Delete: `backend/Application/Services/Admin/CourierService/` (tüm dizin)
- Modify: `backend/Application/ApplicationServiceRegistration.cs` — Kurye servis kayıtlarını sil

- [ ] **Step 1: Tüm courier servis dizinlerini sil**

```bash
rm -rf backend/Application/Services/Courier/
rm -rf backend/Application/Services/Seller/CourierService/
rm -rf backend/Application/Services/Admin/CourierService/
```

- [ ] **Step 2: `ApplicationServiceRegistration.cs` dosyasından kurye kayıtlarını sil**

Silinecek satırlar (//Courier bloğu):
```csharp
//Courier
services.AddScoped<ICourierAuthService, CourierAuthManager>();
services.AddScoped<ICourierOrderService, CourierOrderManager>();
services.AddScoped<ICourierLocationService, CourierLocationManager>();
services.AddScoped<ICourierRestaurantService, CourierRestaurantManager>();
services.AddScoped<IAdminCourierService, AdminCourierManager>();
services.AddScoped<ICourierCompanyService, CourierCompanyManager>();
services.AddScoped<ICourierEarningsService, CourierEarningsManager>();
```

Ayrıca seller courier servisi varsa onu da sil:
```csharp
services.AddScoped<ICourierService, CourierManager>();
```

- [ ] **Step 3: Commit**

```bash
git add backend/Application/
git commit -m "chore: remove all courier services from backend"
```

---

### Task 6: OrderService'den Kurye Referanslarını Kaldır

**Files:**
- Modify: `backend/Application/Services/Buyer/OrderService/IOrderService.cs` — AssignCourierAsync ve GetCourierLocationAsync metodlarını sil
- Modify: `backend/Application/Services/Buyer/OrderService/OrderManager.cs` — AssignCourierAsync ve GetCourierLocationAsync implementasyonlarını sil, UpdateOrderStatus'tan courier parametrelerini kaldır

- [ ] **Step 1: `IOrderService.cs`'den courier metodlarını sil**

Silinecek:
```csharp
Task<ServiceObjectResult<bool>> AssignCourierAsync(Guid orderId, Guid courierId);
Task<ServiceObjectResult<CourierLocationDto?>> GetCourierLocationAsync(Guid orderId);
```

`UpdateOrderStatus` imzasından `Guid? courierId = null, Guid? courierCompanyId = null` parametrelerini kaldır.

- [ ] **Step 2: `OrderManager.cs`'den courier metodlarını ve referanslarını sil**

- `AssignCourierAsync` metodu (satır ~1171-1248) tamamen sil
- `GetCourierLocationAsync` metodu (satır ~1250-1315) tamamen sil
- `UpdateOrderStatus` metodundaki courier parametrelerini kaldır
- Courier ile ilgili using statement'ları temizle
- `CourierLocationDto` import'unu sil

- [ ] **Step 3: Commit**

```bash
git add backend/Application/Services/Buyer/OrderService/
git commit -m "chore: remove courier assignment from order service"
```

---

### Task 7: Backend DTO'ları Sil

**Files:**
- Delete: `backend/Domain/Dto/Courier/` (tüm dizin)
- Delete: `backend/Domain/Dto/Seller/Courier/` (tüm dizin)
- Delete: `backend/Domain/Dto/Admin/Courier/` (tüm dizin)

- [ ] **Step 1: Tüm courier DTO dizinlerini sil**

```bash
rm -rf backend/Domain/Dto/Courier/
rm -rf backend/Domain/Dto/Seller/Courier/
rm -rf backend/Domain/Dto/Admin/Courier/
```

- [ ] **Step 2: Commit**

```bash
git add backend/Domain/Dto/
git commit -m "chore: remove all courier DTOs"
```

---

### Task 8: Backend Entity'leri ve Enum'ları Sil

**Files:**
- Delete: `backend/Domain/Entities/Courier/` (tüm dizin)
- Modify: `backend/Base/Enums/AuthorizationServiceEnums.cs` — Kurye enum'larını sil
- Modify: `backend/Domain/Entities/Buyer/Order.cs` — CourierId, CourierCompanyId, PickedUpByCourierId ve CourierCompany navigation property'sini sil
- Modify: `backend/Domain/Entities/Common/User.cs` — CourierStatusId property'sini sil
- Modify: `backend/Base/Entities/TokenDto.cs` — CourierId property'sini sil
- Modify: `backend/WebAPI/Services/SignalRRealtimeNotifier.cs` — NotifyCourierLocationUpdated metodunu sil
- Modify: `backend/Application/Services/Common/NotificationService/IRealtimeNotifier.cs` — NotifyCourierLocationUpdated metodunu sil

- [ ] **Step 1: Courier entity dizinini sil**

```bash
rm -rf backend/Domain/Entities/Courier/
```

- [ ] **Step 2: `AuthorizationServiceEnums.cs` dosyasından kurye enum'larını sil**

Silinecek enum'lar:
- `CourierStatusEnums`
- `RestaurantCourierStatusEnums`
- `CourierCompanyTypeEnums`
- `CourierCompanyStatusEnums`
- `CourierCompanyMemberStatusEnums`
- `RestaurantCourierCompanyStatusEnums`

`UserRoleEnums`'dan silinecek:
- `Courier = 6`
- `CourierCompanyAdmin = 7`

- [ ] **Step 3: `Order.cs`'den courier property'lerini sil**

Silinecek:
- `CourierId` (Guid?)
- `CourierCompanyId` (Guid?)
- `PickedUpByCourierId` (Guid?)
- `CourierCompany` navigation property
- İlgili using statement'ları

- [ ] **Step 4: `User.cs`'den `CourierStatusId` property'sini sil**

- [ ] **Step 5: `TokenDto.cs`'den `CourierId` property'sini sil**

- [ ] **Step 6: `SignalRRealtimeNotifier.cs`'den `NotifyCourierLocationUpdated` metodunu sil**

- [ ] **Step 7: `IRealtimeNotifier.cs`'den `NotifyCourierLocationUpdated` interface metodunu sil**

- [ ] **Step 8: Commit**

```bash
git add backend/Domain/ backend/Base/ backend/WebAPI/Services/ backend/Application/Services/Common/
git commit -m "chore: remove courier entities, enums, and related properties"
```

---

### Task 9: Backend Repository ve Persistence Katmanını Temizle

**Files:**
- Delete: `backend/Persistence/IRepositories/Courier/` (tüm dizin)
- Delete: `backend/Persistence/Repositories/Courier/` (tüm dizin)
- Delete: `backend/Persistence/EntityConfigurations/Courier/` (tüm dizin)
- Modify: `backend/Persistence/Contexts/BaseDbContext.cs` — Kurye DbSet'lerini sil
- Modify: `backend/Persistence/IRepositories/IUnitOfWork.cs` — Kurye property'lerini sil
- Modify: `backend/Persistence/Repositories/UnitOfWork.cs` — Kurye property ve constructor parametrelerini sil
- Modify: `backend/Persistence/PersistenceServiceRegistration.cs` — Kurye repository kayıtlarını sil

- [ ] **Step 1: Courier persistence dizinlerini sil**

```bash
rm -rf backend/Persistence/IRepositories/Courier/
rm -rf backend/Persistence/Repositories/Courier/
rm -rf backend/Persistence/EntityConfigurations/Courier/
```

- [ ] **Step 2: `BaseDbContext.cs`'den courier DbSet'lerini sil**

Silinecek:
```csharp
// Courier
public DbSet<RestaurantCourier> RestaurantCouriers { get; set; }
public DbSet<CourierLocation> CourierLocations { get; set; }
public DbSet<CourierCompany> CourierCompanies { get; set; }
public DbSet<CourierCompanyMember> CourierCompanyMembers { get; set; }
public DbSet<RestaurantCourierCompany> RestaurantCourierCompanies { get; set; }
```

- [ ] **Step 3: `IUnitOfWork.cs`'den courier property'lerini sil**

Silinecek:
```csharp
// Courier
IRestaurantCourierRepository RestaurantCourierRepository { get; }
ICourierLocationRepository CourierLocationRepository { get; }
ICourierCompanyRepository CourierCompanyRepository { get; }
ICourierCompanyMemberRepository CourierCompanyMemberRepository { get; }
IRestaurantCourierCompanyRepository RestaurantCourierCompanyRepository { get; }
```

- [ ] **Step 4: `UnitOfWork.cs`'den courier property ve constructor parametrelerini sil**

Silinecek property'ler:
```csharp
public IRestaurantCourierRepository RestaurantCourierRepository { get; }
public ICourierLocationRepository CourierLocationRepository { get; }
public ICourierCompanyRepository CourierCompanyRepository { get; }
public ICourierCompanyMemberRepository CourierCompanyMemberRepository { get; }
public IRestaurantCourierCompanyRepository RestaurantCourierCompanyRepository { get; }
```

Constructor'dan ilgili parametreleri ve atamaları sil.

- [ ] **Step 5: `PersistenceServiceRegistration.cs`'den courier repository kayıtlarını sil**

Silinecek:
```csharp
//Courier
services.AddScoped<IRestaurantCourierRepository, RestaurantCourierRepository>();
services.AddScoped<ICourierLocationRepository, CourierLocationRepository>();
services.AddScoped<ICourierCompanyRepository, CourierCompanyRepository>();
services.AddScoped<ICourierCompanyMemberRepository, CourierCompanyMemberRepository>();
services.AddScoped<IRestaurantCourierCompanyRepository, RestaurantCourierCompanyRepository>();
```

- [ ] **Step 6: Commit**

```bash
git add backend/Persistence/
git commit -m "chore: remove courier repositories, DbSets, and entity configurations"
```

---

### Task 10: Migration Temizliği

**Files:**
- Delete: Tüm courier migration dosyaları (9 migration × 2 dosya = 18 dosya)
- Delete/Regenerate: `backend/Persistence/Migrations/BaseDbContextModelSnapshot.cs`

- [ ] **Step 1: Tüm courier migration dosyalarını sil**

```bash
rm backend/Persistence/Migrations/20260315075036_kurye2.cs
rm backend/Persistence/Migrations/20260315075036_kurye2.Designer.cs
rm backend/Persistence/Migrations/20260315094951_1249.cs
rm backend/Persistence/Migrations/20260315094951_1249.Designer.cs
rm backend/Persistence/Migrations/20260315140925_FixRestaurantCourierFKToUser.cs
rm backend/Persistence/Migrations/20260315140925_FixRestaurantCourierFKToUser.Designer.cs
rm backend/Persistence/Migrations/20260315141402_UpdateRestaurantCourierStatusFields.cs
rm backend/Persistence/Migrations/20260315141402_UpdateRestaurantCourierStatusFields.Designer.cs
rm backend/Persistence/Migrations/20260315143044_CleanupOldCourierEntities.cs
rm backend/Persistence/Migrations/20260315143044_CleanupOldCourierEntities.Designer.cs
rm backend/Persistence/Migrations/20260316211851_0018.cs
rm backend/Persistence/Migrations/20260316211851_0018.Designer.cs
rm backend/Persistence/Migrations/20260316214357_17030043.cs
rm backend/Persistence/Migrations/20260316214357_17030043.Designer.cs
rm backend/Persistence/Migrations/20260317201708_AddCourierCompanyTypeId.cs
rm backend/Persistence/Migrations/20260317201708_AddCourierCompanyTypeId.Designer.cs
rm backend/Persistence/Migrations/20260317202650_170320262326.cs
rm backend/Persistence/Migrations/20260317202650_170320262326.Designer.cs
```

- [ ] **Step 2: BaseDbContextModelSnapshot.cs'den courier tablolarını temizle**

Snapshot dosyasından courier ile ilgili tüm entity tanımlarını (RestaurantCourier, CourierLocation, CourierCompany, CourierCompanyMember, RestaurantCourierCompany) sil.

- [ ] **Step 3: Commit**

```bash
git add backend/Persistence/Migrations/
git commit -m "chore: remove courier migration files and clean snapshot"
```

---

### Task 11: Build Doğrulaması

- [ ] **Step 1: Backend build kontrolü**

```bash
cd backend && dotnet build
```

Hata varsa: kalan courier referanslarını bul ve temizle.

- [ ] **Step 2: Kalan courier referanslarını ara**

```bash
grep -r "courier\|Courier" backend/ --include="*.cs" -l
```

Yalnızca yorum veya gereksiz referanslar kaldıysa temizle.

- [ ] **Step 3: Final commit**

```bash
git add -A
git commit -m "chore: complete courier system removal — clean build verified"
```
