# Faz 1 Implementation Plan — Kurumsal Kurye, Kademeli Abonelik, iyzico, Navigasyon

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Implement corporate courier system, tiered subscription with order limits, automatic iyzico sub-merchant registration, and courier navigation with location tracking.

**Architecture:** .NET 8 Clean Architecture backend (Domain → Application → Persistence → WebAPI), Next.js admin/seller panels, React Native courier/customer apps. All new features follow existing patterns: Entity<Guid>, Manager/Service, EfRepositoryBase, BaseController.

**Tech Stack:** .NET 8, EF Core 9 (Pomelo MySQL), Dapper, React Native 0.84, Next.js 15+, react-native-maps, SWR, Axios, Tailwind CSS

**Spec:** `docs/superpowers/specs/2026-03-16-faz1-courier-subscription-iyzico-design.md`

---

## Chunk 1: Backend Foundation — Enums, Entities, Configurations, Repositories, DI

### Task 1: Add New Enums

**Files:**
- Modify: `backend/Base/Enums/AuthorizationServiceEnums.cs`

- [ ] **Step 1: Add CourierCompanyAdmin role to UserRoleEnums**

```csharp
// Inside UserRoleEnums enum, add after Courier = 6:
CourierCompanyAdmin = 7
```

- [ ] **Step 2: Add CourierCompanyStatusEnums**

```csharp
public enum CourierCompanyStatusEnums : short
{
    PendingApproval = 0,
    Active = 1,
    Suspended = 2,
    Banned = 3
}
```

- [ ] **Step 3: Add CourierCompanyMemberStatusEnums**

```csharp
public enum CourierCompanyMemberStatusEnums : short
{
    PendingApproval = 0,
    Active = 1,
    RemovedByCompany = 2,
    LeftByChoice = 3
}
```

- [ ] **Step 4: Add RestaurantCourierCompanyStatusEnums**

```csharp
public enum RestaurantCourierCompanyStatusEnums : short
{
    PendingApproval = 0,
    Active = 1,
    TerminatedByRestaurant = 2,
    TerminatedByCompany = 3
}
```

- [ ] **Step 5: Add OverageActionEnums**

```csharp
public enum OverageActionEnums : short
{
    Block = 1,
    AutoUpgrade = 2
}
```

- [ ] **Step 6: Commit**

```bash
git add backend/Base/Enums/AuthorizationServiceEnums.cs
git commit -m "feat: add enums for courier company, subscription overage, and new role"
```

---

### Task 2: Create CourierCompany Entity

**Files:**
- Create: `backend/Domain/Entities/Courier/CourierCompany.cs`

- [ ] **Step 1: Create entity file**

```csharp
using Domain.Entities.Common;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Courier;

public class CourierCompany : Entity<Guid>
{
    public string Name { get; set; }
    public string? LegalName { get; set; }
    public string? TaxCode { get; set; }
    public string? TaxArea { get; set; }
    public string ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public Guid OwnerUserId { get; set; }
    public short StatusId { get; set; } = (short)Base.Enums.AuthorizationServiceEnums.CourierCompanyStatusEnums.PendingApproval;
    public virtual User OwnerUser { get; set; }
    public virtual ICollection<CourierCompanyMember> Members { get; set; }
}
```

- [ ] **Step 2: Commit**

```bash
git add backend/Domain/Entities/Courier/CourierCompany.cs
git commit -m "feat: add CourierCompany entity"
```

---

### Task 3: Create CourierCompanyMember Entity

**Files:**
- Create: `backend/Domain/Entities/Courier/CourierCompanyMember.cs`

- [ ] **Step 1: Create entity file**

```csharp
using Domain.Entities.Common;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Courier;

public class CourierCompanyMember : Entity<Guid>
{
    public Guid CourierCompanyId { get; set; }
    public Guid CourierId { get; set; }
    public short StatusId { get; set; } = (short)Base.Enums.AuthorizationServiceEnums.CourierCompanyMemberStatusEnums.PendingApproval;
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ApprovedAt { get; set; }
    public virtual CourierCompany CourierCompany { get; set; }
    public virtual User CourierUser { get; set; }
}
```

- [ ] **Step 2: Commit**

```bash
git add backend/Domain/Entities/Courier/CourierCompanyMember.cs
git commit -m "feat: add CourierCompanyMember entity"
```

---

### Task 4: Create RestaurantCourierCompany Entity

**Files:**
- Create: `backend/Domain/Entities/Courier/RestaurantCourierCompany.cs`

- [ ] **Step 1: Create entity file**

```csharp
using Domain.Entities.Common;
using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Courier;

public class RestaurantCourierCompany : Entity<Guid>
{
    public Guid RestaurantId { get; set; }
    public Guid CourierCompanyId { get; set; }
    public short StatusId { get; set; } = (short)Base.Enums.AuthorizationServiceEnums.RestaurantCourierCompanyStatusEnums.PendingApproval;
    public DateTime? AgreementStartDate { get; set; }
    public DateTime? AgreementEndDate { get; set; }
    public virtual Restaurant Restaurant { get; set; }
    public virtual CourierCompany CourierCompany { get; set; }
}
```

- [ ] **Step 2: Commit**

```bash
git add backend/Domain/Entities/Courier/RestaurantCourierCompany.cs
git commit -m "feat: add RestaurantCourierCompany entity"
```

---

### Task 5: Create SubscriptionUsage Entity

**Files:**
- Create: `backend/Domain/Entities/Seller/SubscriptionUsage.cs`

- [ ] **Step 1: Create entity file**

```csharp
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Seller;

public class SubscriptionUsage : Entity<Guid>
{
    public Guid SubscriptionId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public int OrderCount { get; set; } = 0;
    public virtual Subscription Subscription { get; set; }
}
```

- [ ] **Step 2: Commit**

```bash
git add backend/Domain/Entities/Seller/SubscriptionUsage.cs
git commit -m "feat: add SubscriptionUsage entity for order count tracking"
```

---

### Task 6: Modify Existing Entities

**Files:**
- Modify: `backend/Domain/Entities/Seller/SubscriptionPlan.cs`
- Modify: `backend/Domain/Entities/Seller/Seller.cs`
- Modify: `backend/Domain/Entities/Buyer/Order.cs`

- [ ] **Step 1: Add fields to SubscriptionPlan**

Add after existing properties:
```csharp
public int MaxOrdersPerMonth { get; set; } = int.MaxValue;
public short OverageAction { get; set; } = (short)Base.Enums.AuthorizationServiceEnums.OverageActionEnums.Block;
```

- [ ] **Step 2: Add IdentityNumber to Seller**

Add after existing properties:
```csharp
public string? IdentityNumber { get; set; }
```

- [ ] **Step 3: Add fields to Order**

Add after existing `CourierId` property:
```csharp
public Guid? CourierCompanyId { get; set; }
public Guid? PickedUpByCourierId { get; set; }
public DateTime? PickedUpAt { get; set; }
public DateTime? DeliveredAt { get; set; }
public virtual Courier.CourierCompany? CourierCompany { get; set; }
```

- [ ] **Step 4: Commit**

```bash
git add backend/Domain/Entities/Seller/SubscriptionPlan.cs backend/Domain/Entities/Seller/Seller.cs backend/Domain/Entities/Buyer/Order.cs
git commit -m "feat: add new fields to SubscriptionPlan, Seller, Order entities"
```

---

### Task 7: Create Entity Configurations

**Files:**
- Create: `backend/Persistence/EntityConfigurations/Courier/CourierCompanyConfiguration.cs`
- Create: `backend/Persistence/EntityConfigurations/Courier/CourierCompanyMemberConfiguration.cs`
- Create: `backend/Persistence/EntityConfigurations/Courier/RestaurantCourierCompanyConfiguration.cs`
- Create: `backend/Persistence/EntityConfigurations/Seller/SubscriptionUsageConfiguration.cs`

- [ ] **Step 1: CourierCompanyConfiguration**

```csharp
using Domain.Entities.Courier;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations.Courier;

public class CourierCompanyConfiguration : IEntityTypeConfiguration<CourierCompany>
{
    public void Configure(EntityTypeBuilder<CourierCompany> builder)
    {
        builder.ToTable("CourierCompany").HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.ContactEmail).IsRequired().HasMaxLength(200);
        builder.HasIndex(c => c.OwnerUserId).IsUnique();

        builder.HasOne(c => c.OwnerUser)
            .WithMany()
            .HasForeignKey(c => c.OwnerUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

- [ ] **Step 2: CourierCompanyMemberConfiguration**

```csharp
using Domain.Entities.Courier;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations.Courier;

public class CourierCompanyMemberConfiguration : IEntityTypeConfiguration<CourierCompanyMember>
{
    public void Configure(EntityTypeBuilder<CourierCompanyMember> builder)
    {
        builder.ToTable("CourierCompanyMember").HasKey(c => c.Id);
        builder.HasIndex(c => new { c.CourierCompanyId, c.CourierId, c.StatusId });

        builder.HasOne(c => c.CourierCompany)
            .WithMany(cc => cc.Members)
            .HasForeignKey(c => c.CourierCompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.CourierUser)
            .WithMany()
            .HasForeignKey(c => c.CourierId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

- [ ] **Step 3: RestaurantCourierCompanyConfiguration**

```csharp
using Domain.Entities.Courier;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations.Courier;

public class RestaurantCourierCompanyConfiguration : IEntityTypeConfiguration<RestaurantCourierCompany>
{
    public void Configure(EntityTypeBuilder<RestaurantCourierCompany> builder)
    {
        builder.ToTable("RestaurantCourierCompany").HasKey(c => c.Id);
        builder.HasIndex(c => new { c.RestaurantId, c.CourierCompanyId, c.StatusId });

        builder.HasOne(c => c.Restaurant)
            .WithMany()
            .HasForeignKey(c => c.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.CourierCompany)
            .WithMany()
            .HasForeignKey(c => c.CourierCompanyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

- [ ] **Step 4: SubscriptionUsageConfiguration**

```csharp
using Domain.Entities.Seller;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations.Seller;

public class SubscriptionUsageConfiguration : IEntityTypeConfiguration<SubscriptionUsage>
{
    public void Configure(EntityTypeBuilder<SubscriptionUsage> builder)
    {
        builder.ToTable("SubscriptionUsage").HasKey(c => c.Id);
        builder.HasIndex(c => new { c.SubscriptionId, c.Year, c.Month }).IsUnique();

        builder.HasOne(c => c.Subscription)
            .WithMany()
            .HasForeignKey(c => c.SubscriptionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

- [ ] **Step 5: Commit**

```bash
git add backend/Persistence/EntityConfigurations/Courier/ backend/Persistence/EntityConfigurations/Seller/SubscriptionUsageConfiguration.cs
git commit -m "feat: add entity configurations for new tables"
```

---

### Task 8: Register DbSets in BaseDbContext

**Files:**
- Modify: `backend/Persistence/Contexts/BaseDbContext.cs`

- [ ] **Step 1: Add DbSet properties**

Add alongside existing DbSets:
```csharp
public DbSet<CourierCompany> CourierCompanies { get; set; }
public DbSet<CourierCompanyMember> CourierCompanyMembers { get; set; }
public DbSet<RestaurantCourierCompany> RestaurantCourierCompanies { get; set; }
public DbSet<SubscriptionUsage> SubscriptionUsages { get; set; }
```

Add required using:
```csharp
using Domain.Entities.Courier;
using Domain.Entities.Seller;
```

- [ ] **Step 2: Commit**

```bash
git add backend/Persistence/Contexts/BaseDbContext.cs
git commit -m "feat: register new DbSets in BaseDbContext"
```

---

### Task 9: Create Repository Interfaces and Implementations

**Files:**
- Create: `backend/Persistence/IRepositories/Courier/ICourierCompanyRepository.cs`
- Create: `backend/Persistence/IRepositories/Courier/ICourierCompanyMemberRepository.cs`
- Create: `backend/Persistence/IRepositories/Courier/IRestaurantCourierCompanyRepository.cs`
- Create: `backend/Persistence/IRepositories/Seller/ISubscriptionUsageRepository.cs`
- Create: `backend/Persistence/Repositories/Courier/CourierCompanyRepository.cs`
- Create: `backend/Persistence/Repositories/Courier/CourierCompanyMemberRepository.cs`
- Create: `backend/Persistence/Repositories/Courier/RestaurantCourierCompanyRepository.cs`
- Create: `backend/Persistence/Repositories/Seller/SubscriptionUsageRepository.cs`

- [ ] **Step 1: ICourierCompanyRepository**

```csharp
using Domain.Entities.Courier;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Courier;

public interface ICourierCompanyRepository : IAsyncRepository<CourierCompany, Guid>, IRepository<CourierCompany, Guid>
{
}
```

- [ ] **Step 2: ICourierCompanyMemberRepository**

Same pattern, replacing `CourierCompany` with `CourierCompanyMember`.

- [ ] **Step 3: IRestaurantCourierCompanyRepository**

Same pattern with `RestaurantCourierCompany`.

- [ ] **Step 4: ISubscriptionUsageRepository**

```csharp
using Domain.Entities.Seller;
using NArchitecture.Core.Persistence.Repositories;

namespace Persistence.IRepositories.Seller;

public interface ISubscriptionUsageRepository : IAsyncRepository<SubscriptionUsage, Guid>, IRepository<SubscriptionUsage, Guid>
{
}
```

- [ ] **Step 5: CourierCompanyRepository**

```csharp
using Domain.Entities.Courier;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;
using Persistence.IRepositories.Courier;

namespace Persistence.Repositories.Courier;

public class CourierCompanyRepository : EfRepositoryBase<CourierCompany, Guid, BaseDbContext>, ICourierCompanyRepository
{
    public CourierCompanyRepository(BaseDbContext context) : base(context) { }
}
```

- [ ] **Step 6: Create remaining 3 repository implementations** (same pattern)

- [ ] **Step 7: Commit**

```bash
git add backend/Persistence/IRepositories/ backend/Persistence/Repositories/
git commit -m "feat: add repositories for courier company and subscription usage"
```

---

### Task 10: Register Repositories in DI

**Files:**
- Modify: `backend/Persistence/IRepositories/IUnitOfWork.cs`
- Modify: `backend/Persistence/PersistenceServiceRegistration.cs`

- [ ] **Step 1: Add to IUnitOfWork**

Add properties:
```csharp
ICourierCompanyRepository CourierCompanyRepository { get; }
ICourierCompanyMemberRepository CourierCompanyMemberRepository { get; }
IRestaurantCourierCompanyRepository RestaurantCourierCompanyRepository { get; }
ISubscriptionUsageRepository SubscriptionUsageRepository { get; }
```

- [ ] **Step 2: Add to UnitOfWork implementation** (if separate file, add the same properties with backing fields)

- [ ] **Step 3: Register in PersistenceServiceRegistration.cs**

Add in the `AddPersistenceServices` method:
```csharp
services.AddScoped<ICourierCompanyRepository, CourierCompanyRepository>();
services.AddScoped<ICourierCompanyMemberRepository, CourierCompanyMemberRepository>();
services.AddScoped<IRestaurantCourierCompanyRepository, RestaurantCourierCompanyRepository>();
services.AddScoped<ISubscriptionUsageRepository, SubscriptionUsageRepository>();
```

- [ ] **Step 4: Commit**

```bash
git add backend/Persistence/
git commit -m "feat: register new repositories in UnitOfWork and DI"
```

---

### Task 11: Create EF Core Migration

**Files:**
- Auto-generated migration in `backend/Persistence/Migrations/`

- [ ] **Step 1: Generate migration**

```bash
cd /Users/onuryorulmaz/Desktop/Github/food-order/backend
dotnet ef migrations add AddCourierCompanyAndSubscriptionUsage --project Persistence --startup-project WebAPI
```

- [ ] **Step 2: Review generated migration** — verify it creates:
  - `CourierCompany` table
  - `CourierCompanyMember` table
  - `RestaurantCourierCompany` table
  - `SubscriptionUsage` table with unique index on (SubscriptionId, Year, Month)
  - Adds `IdentityNumber` to `Seller`
  - Adds `MaxOrdersPerMonth`, `OverageAction` to `SubscriptionPlan`
  - Adds `CourierCompanyId`, `PickedUpByCourierId`, `PickedUpAt`, `DeliveredAt` to `Order`

- [ ] **Step 3: Apply migration**

```bash
dotnet ef database update --project Persistence --startup-project WebAPI
```

- [ ] **Step 4: Commit**

```bash
git add backend/Persistence/Migrations/
git commit -m "feat: add migration for courier company and subscription usage tables"
```

---

## Chunk 2: Backend Services — Courier Company Management

### Task 12: Create DTOs for Courier Company

**Files:**
- Create: `backend/Domain/Dto/Courier/Company/CourierCompanyDto.cs`
- Create: `backend/Domain/Dto/Courier/Company/RegisterCourierCompanyRequestDto.cs`
- Create: `backend/Domain/Dto/Courier/Company/UpdateCourierCompanyRequestDto.cs`
- Create: `backend/Domain/Dto/Courier/Company/CourierCompanyMemberDto.cs`
- Create: `backend/Domain/Dto/Courier/Company/SearchCourierRequestDto.cs`
- Create: `backend/Domain/Dto/Courier/Company/CompanyInviteDto.cs`
- Create: `backend/Domain/Dto/Courier/Company/RestaurantCourierCompanyDto.cs`
- Create: `backend/Domain/Dto/Courier/Company/PickupOrderDto.cs`

- [ ] **Step 1: CourierCompanyDto**

```csharp
using Base.Entities;

namespace Domain.Dto.Courier.Company;

public class CourierCompanyDto : IDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? LegalName { get; set; }
    public string ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public short StatusId { get; set; }
    public string StatusName { get; set; }
    public int MemberCount { get; set; }
    public DateTime CreatedDate { get; set; }
}
```

- [ ] **Step 2: RegisterCourierCompanyRequestDto**

```csharp
using Base.Entities;

namespace Domain.Dto.Courier.Company;

public class RegisterCourierCompanyRequestDto : IDto
{
    public string Name { get; set; }
    public string? LegalName { get; set; }
    public string? TaxCode { get; set; }
    public string? TaxArea { get; set; }
    public string ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
}
```

- [ ] **Step 3: UpdateCourierCompanyRequestDto** (same fields as Register minus ContactEmail)

- [ ] **Step 4: CourierCompanyMemberDto**

```csharp
using Base.Entities;

namespace Domain.Dto.Courier.Company;

public class CourierCompanyMemberDto : IDto
{
    public Guid Id { get; set; }
    public Guid CourierId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public short StatusId { get; set; }
    public string StatusName { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
}
```

- [ ] **Step 5: SearchCourierRequestDto**

```csharp
using Base.Entities;

namespace Domain.Dto.Courier.Company;

public class SearchCourierRequestDto : IDto
{
    public string Query { get; set; }
}
```

- [ ] **Step 6: CompanyInviteDto** (for courier's view of company invites)

```csharp
using Base.Entities;

namespace Domain.Dto.Courier.Company;

public class CompanyInviteDto : IDto
{
    public Guid Id { get; set; }
    public Guid CourierCompanyId { get; set; }
    public string CompanyName { get; set; }
    public string ContactEmail { get; set; }
    public DateTime RequestedAt { get; set; }
}
```

- [ ] **Step 7: RestaurantCourierCompanyDto**

```csharp
using Base.Entities;

namespace Domain.Dto.Courier.Company;

public class RestaurantCourierCompanyDto : IDto
{
    public Guid Id { get; set; }
    public Guid CourierCompanyId { get; set; }
    public string CompanyName { get; set; }
    public short StatusId { get; set; }
    public string StatusName { get; set; }
    public DateTime? AgreementStartDate { get; set; }
}
```

- [ ] **Step 8: PickupOrderDto**

```csharp
using Base.Entities;

namespace Domain.Dto.Courier.Company;

public class PickupOrderDto : IDto
{
    public Guid OrderId { get; set; }
    public string OrderNumber { get; set; }
    public string RestaurantName { get; set; }
    public string DeliveryAddress { get; set; }
    public double? DeliveryLatitude { get; set; }
    public double? DeliveryLongitude { get; set; }
    public double? DeliveryDistanceKm { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CreatedDate { get; set; }
}
```

- [ ] **Step 9: Commit**

```bash
git add backend/Domain/Dto/Courier/Company/
git commit -m "feat: add DTOs for courier company management"
```

---

### Task 13: Create CourierCompanyService Interface

**Files:**
- Create: `backend/Application/Services/Courier/CourierCompanyService/ICourierCompanyService.cs`

- [ ] **Step 1: Create interface**

```csharp
using Domain.Dto.Courier.Company;
using Domain.Service;

namespace Application.Services.Courier.CourierCompanyService;

public interface ICourierCompanyService
{
    // Company management (CourierCompanyAdmin)
    Task<ServiceObjectResult<CourierCompanyDto>> RegisterCompanyAsync(Guid ownerUserId, RegisterCourierCompanyRequestDto request);
    Task<ServiceObjectResult<CourierCompanyDto>> GetMyCompanyAsync(Guid ownerUserId);
    Task<ServiceObjectResult<bool>> UpdateCompanyAsync(Guid ownerUserId, UpdateCourierCompanyRequestDto request);

    // Member management (CourierCompanyAdmin)
    Task<ServiceCollectionResult<CourierCompanyMemberDto>> SearchCouriersAsync(string query);
    Task<ServiceObjectResult<bool>> RequestCourierMembershipAsync(Guid companyId, Guid courierId);
    Task<ServiceCollectionResult<CourierCompanyMemberDto>> GetCompanyMembersAsync(Guid companyId);
    Task<ServiceObjectResult<bool>> RemoveMemberAsync(Guid companyId, Guid memberId);

    // Courier side
    Task<ServiceCollectionResult<CompanyInviteDto>> GetCompanyInvitesAsync(Guid courierId);
    Task<ServiceObjectResult<bool>> AcceptCompanyInviteAsync(Guid courierId, Guid inviteId);
    Task<ServiceObjectResult<bool>> RejectCompanyInviteAsync(Guid courierId, Guid inviteId);
    Task<ServiceObjectResult<bool>> LeaveCompanyAsync(Guid courierId);

    // Restaurant ↔ Company (from seller side, called by seller service)
    Task<ServiceObjectResult<bool>> InviteCompanyToRestaurantAsync(Guid restaurantId, Guid companyId);
    Task<ServiceCollectionResult<RestaurantCourierCompanyDto>> GetRestaurantCompaniesAsync(Guid restaurantId);
    Task<ServiceObjectResult<bool>> RemoveCompanyFromRestaurantAsync(Guid restaurantId, Guid companyId);

    // Restaurant invites (CourierCompanyAdmin side)
    Task<ServiceCollectionResult<RestaurantCourierCompanyDto>> GetRestaurantInvitesAsync(Guid companyId);
    Task<ServiceObjectResult<bool>> AcceptRestaurantInviteAsync(Guid companyId, Guid inviteId);
    Task<ServiceObjectResult<bool>> RejectRestaurantInviteAsync(Guid companyId, Guid inviteId);

    // Pickup flow
    Task<ServiceCollectionResult<PickupOrderDto>> GetPendingPickupOrdersAsync(Guid courierId, Guid restaurantId, int page, int size);
    Task<ServiceObjectResult<bool>> ConfirmPickupAsync(Guid courierId, Guid orderId);

    // Admin
    Task<ServiceCollectionResult<CourierCompanyDto>> GetAllCompaniesAsync(int page, int size);
    Task<ServiceObjectResult<bool>> ApproveCompanyAsync(Guid companyId);
    Task<ServiceObjectResult<bool>> RejectCompanyAsync(Guid companyId);
    Task<ServiceObjectResult<bool>> SuspendCompanyAsync(Guid companyId);
    Task<ServiceObjectResult<bool>> BanCompanyAsync(Guid companyId);
}
```

- [ ] **Step 2: Commit**

```bash
git add backend/Application/Services/Courier/CourierCompanyService/
git commit -m "feat: add ICourierCompanyService interface"
```

---

### Task 14: Implement CourierCompanyManager — Company CRUD

**Files:**
- Create: `backend/Application/Services/Courier/CourierCompanyService/CourierCompanyManager.cs`

- [ ] **Step 1: Create manager with constructor and company registration**

```csharp
using Base.Enums;
using Domain.Dto.Courier.Company;
using Domain.Entities.Courier;
using Domain.Service;
using Persistence.IRepositories;
using Persistence.IRepositories.Common;
using Persistence.IRepositories.Courier;

namespace Application.Services.Courier.CourierCompanyService;

public class CourierCompanyManager : ICourierCompanyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly ICourierCompanyRepository _courierCompanyRepository;
    private readonly ICourierCompanyMemberRepository _courierCompanyMemberRepository;
    private readonly IRestaurantCourierCompanyRepository _restaurantCourierCompanyRepository;

    public CourierCompanyManager(
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        ICourierCompanyRepository courierCompanyRepository,
        ICourierCompanyMemberRepository courierCompanyMemberRepository,
        IRestaurantCourierCompanyRepository restaurantCourierCompanyRepository)
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _courierCompanyRepository = courierCompanyRepository;
        _courierCompanyMemberRepository = courierCompanyMemberRepository;
        _restaurantCourierCompanyRepository = restaurantCourierCompanyRepository;
    }

    public async Task<ServiceObjectResult<CourierCompanyDto>> RegisterCompanyAsync(Guid ownerUserId, RegisterCourierCompanyRequestDto request)
    {
        var result = new ServiceObjectResult<CourierCompanyDto>();
        try
        {
            // Check user is a courier
            var user = await _userRepository.GetAsync(u => u.Id == ownerUserId);
            if (user == null || user.UserRoleId != (short)AuthorizationServiceEnums.UserRoleEnums.Courier)
            {
                result.Fail("Sadece kuryeler firma kaydı yapabilir.");
                return result;
            }

            // Check user doesn't already own a company
            var existing = await _courierCompanyRepository.GetAsync(c => c.OwnerUserId == ownerUserId);
            if (existing != null)
            {
                result.Fail("Zaten bir firmaya sahipsiniz.");
                return result;
            }

            var company = new CourierCompany
            {
                Name = request.Name,
                LegalName = request.LegalName,
                TaxCode = request.TaxCode,
                TaxArea = request.TaxArea,
                ContactEmail = request.ContactEmail,
                ContactPhone = request.ContactPhone,
                OwnerUserId = ownerUserId,
                StatusId = (short)AuthorizationServiceEnums.CourierCompanyStatusEnums.PendingApproval
            };

            await _courierCompanyRepository.AddAsync(company);
            await _unitOfWork.CompleteAsync();

            result.SetData(new CourierCompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                LegalName = company.LegalName,
                ContactEmail = company.ContactEmail,
                ContactPhone = company.ContactPhone,
                StatusId = company.StatusId,
                StatusName = "Onay Bekliyor",
                MemberCount = 0,
                CreatedDate = company.CreatedDate
            });
            result.AddSuccessMessage("Firma kaydı oluşturuldu, admin onayı bekleniyor.");
        }
        catch (Exception ex)
        {
            result.Fail($"Hata: {ex.Message}");
        }
        return result;
    }

    // ... remaining methods will be implemented in next steps
}
```

Note: Remaining methods (GetMyCompany, UpdateCompany) follow the same pattern. Each method: validate → query/modify → CompleteAsync → map to DTO → return result.

- [ ] **Step 2: Implement member management methods** (SearchCouriers, RequestMembership, GetMembers, RemoveMember)

Key validation for RequestMembership:
```csharp
// Check courier not already in another company (active or pending)
var existingMembership = await _courierCompanyMemberRepository.GetAsync(
    m => m.CourierId == courierId &&
         (m.StatusId == (short)AuthorizationServiceEnums.CourierCompanyMemberStatusEnums.Active ||
          m.StatusId == (short)AuthorizationServiceEnums.CourierCompanyMemberStatusEnums.PendingApproval));
if (existingMembership != null)
{
    result.Fail("Bu kurye zaten bir firmaya bağlı veya bekleyen bir talebi var.");
    return result;
}

// Check target user is not a company owner
var isOwner = await _courierCompanyRepository.GetAsync(c => c.OwnerUserId == courierId);
if (isOwner != null)
{
    result.Fail("Firma sahibi başka bir firmaya bağlanamaz.");
    return result;
}
```

- [ ] **Step 3: Implement courier-side methods** (GetCompanyInvites, Accept, Reject, LeaveCompany)

- [ ] **Step 4: Implement restaurant-company methods** (InviteCompany, GetRestaurantCompanies, RemoveCompany, GetRestaurantInvites, Accept/Reject)

- [ ] **Step 5: Implement pickup flow methods**

GetPendingPickupOrders — find the courier's company, then query orders where:
```csharp
o.CourierCompanyId == companyId &&
o.RestaurantId == restaurantId &&
o.PickedUpByCourierId == null &&
o.StatusId == (short)AuthorizationServiceEnums.OrderStatusEnums.OnTheWay
```
Order by `CreatedDate ASC`, paginated.

ConfirmPickup — validate courier belongs to the company assigned to the order, then:
```csharp
order.PickedUpByCourierId = courierId;
order.PickedUpAt = DateTime.UtcNow;
```

- [ ] **Step 6: Implement admin methods** (GetAll, Approve, Reject, Suspend, Ban)

Approve should also update user role:
```csharp
var owner = await _userRepository.GetAsync(u => u.Id == company.OwnerUserId);
owner.UserRoleId = (short)AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin;
await _userRepository.UpdateAsync(owner);
```

- [ ] **Step 7: Commit**

```bash
git add backend/Application/Services/Courier/CourierCompanyService/
git commit -m "feat: implement CourierCompanyManager with all business logic"
```

---

### Task 15: Create Controllers for Courier Company

**Files:**
- Create: `backend/WebAPI/Controllers/Courier/CourierCompanyController.cs`
- Create: `backend/WebAPI/Controllers/Seller/SellerCourierCompanyController.cs`
- Create: `backend/WebAPI/Controllers/Admin/AdminCourierCompanyController.cs`

- [ ] **Step 1: CourierCompanyController** (courier & company admin endpoints)

```csharp
using Application.Services.Courier.CourierCompanyService;
using Base.Enums;
using Domain.Dto.Courier.Company;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Courier;

[Route("v1/courier/company")]
[ApiController]
public class CourierCompanyController : BaseController
{
    private readonly ICourierCompanyService _service;

    public CourierCompanyController(ICourierCompanyService service)
    {
        _service = service;
    }

    [HttpPost("register")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.Courier)]
    public async Task<ServiceObjectResult<CourierCompanyDto>> Register(
        [FromBody] RegisterCourierCompanyRequestDto request)
        => await _service.RegisterCompanyAsync(CurrentUserId, request);

    [HttpGet("my")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceObjectResult<CourierCompanyDto>> GetMyCompany()
        => await _service.GetMyCompanyAsync(CurrentUserId);

    [HttpPut("my")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceObjectResult<bool>> UpdateCompany(
        [FromBody] UpdateCourierCompanyRequestDto request)
        => await _service.UpdateCompanyAsync(CurrentUserId, request);

    // Member management
    [HttpGet("members/search")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceCollectionResult<CourierCompanyMemberDto>> SearchCouriers(
        [FromQuery] string q)
        => await _service.SearchCouriersAsync(q);

    [HttpPost("members/request")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceObjectResult<bool>> RequestMembership(
        [FromBody] RequestMembershipDto request)
        => await _service.RequestCourierMembershipAsync(GetCompanyId(), request.CourierId);

    [HttpGet("members")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceCollectionResult<CourierCompanyMemberDto>> GetMembers()
        => await _service.GetCompanyMembersAsync(GetCompanyId());

    [HttpDelete("members/{id}")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceObjectResult<bool>> RemoveMember(Guid id)
        => await _service.RemoveMemberAsync(GetCompanyId(), id);

    // Restaurant invites (company side)
    [HttpGet("restaurant-invites")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceCollectionResult<RestaurantCourierCompanyDto>> GetRestaurantInvites()
        => await _service.GetRestaurantInvitesAsync(GetCompanyId());

    [HttpPut("restaurant-invites/{id}/accept")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceObjectResult<bool>> AcceptRestaurantInvite(Guid id)
        => await _service.AcceptRestaurantInviteAsync(GetCompanyId(), id);

    [HttpPut("restaurant-invites/{id}/reject")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceObjectResult<bool>> RejectRestaurantInvite(Guid id)
        => await _service.RejectRestaurantInviteAsync(GetCompanyId(), id);

    // Helper: get company ID from owner user
    private Guid GetCompanyId()
    {
        // This will be resolved via a lookup in the service layer
        // For now, pass CurrentUserId and let the service find the company
        return CurrentUserId; // Service will resolve to companyId
    }
}
```

Note: `CurrentUserId` comes from `BaseController` which extracts it from the auth token. The `GetCompanyId()` helper needs to be adjusted based on how BaseController works — if it only provides UserId, the service methods should accept ownerUserId and resolve the company internally.

- [ ] **Step 2: Courier-side invite endpoints** (separate controller or same)

```csharp
// In a separate route group or controller:
[Route("v1/courier")]
public class CourierInviteController : BaseController
{
    [HttpGet("company-invites")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.Courier,
        AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
    public async Task<ServiceCollectionResult<CompanyInviteDto>> GetCompanyInvites()
        => await _service.GetCompanyInvitesAsync(CurrentUserId);

    [HttpPut("company-invites/{id}/accept")]
    // ... same auth pattern

    [HttpPut("company-invites/{id}/reject")]
    // ... same auth pattern

    [HttpPut("company/leave")]
    // ... same auth pattern
}
```

- [ ] **Step 3: SellerCourierCompanyController**

```csharp
[Route("v1/seller")]
[ApiController]
public class SellerCourierCompanyController : BaseController
{
    [HttpPost("restaurant/{restaurantId}/courier-company/add")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceObjectResult<bool>> InviteCompany(
        Guid restaurantId, [FromBody] InviteCompanyRequestDto request)
        => await _service.InviteCompanyToRestaurantAsync(restaurantId, request.CompanyId);

    [HttpGet("restaurant/{restaurantId}/courier-companies")]
    // ... list companies

    [HttpDelete("restaurant/{restaurantId}/courier-company/{companyId}")]
    // ... remove company
}
```

- [ ] **Step 4: AdminCourierCompanyController**

```csharp
[Route("v1/admin/courier-companies")]
[ApiController]
public class AdminCourierCompanyController : BaseController
{
    [HttpGet]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceCollectionResult<CourierCompanyDto>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int size = 20)
        => await _service.GetAllCompaniesAsync(page, size);

    [HttpPut("{id}/approve")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> Approve(Guid id)
        => await _service.ApproveCompanyAsync(id);

    [HttpPut("{id}/reject")]
    // ...
    [HttpPut("{id}/suspend")]
    // ...
    [HttpPut("{id}/ban")]
    // ...
}
```

- [ ] **Step 5: Register service in DI**

In `ApplicationServiceRegistration.cs`:
```csharp
services.AddScoped<ICourierCompanyService, CourierCompanyManager>();
```

- [ ] **Step 6: Commit**

```bash
git add backend/WebAPI/Controllers/ backend/Application/ApplicationServiceRegistration.cs
git commit -m "feat: add controllers for courier company management"
```

---

### Task 16: Update AuthorizeAPIRequest for CourierCompanyAdmin

**Files:**
- Modify: All existing courier controllers to accept `CourierCompanyAdmin` role

- [ ] **Step 1: Find and update all courier endpoint authorizations**

In each courier controller (`CourierAuthController`, `CourierOrderController`, `CourierLocationController`, etc.), add `AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin` to every `[AuthorizeAPIRequest]` that currently only allows `Courier`.

Example change:
```csharp
// Before:
[AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Courier)]

// After:
[AuthorizeAPIRequest(true, false,
    AuthorizationServiceEnums.UserRoleEnums.Courier,
    AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
```

- [ ] **Step 2: Commit**

```bash
git add backend/WebAPI/Controllers/Courier/
git commit -m "feat: allow CourierCompanyAdmin role on all courier endpoints"
```

---

## Chunk 3: Backend Services — Subscription & iyzico

### Task 17: Create Subscription Usage DTOs

**Files:**
- Create: `backend/Domain/Dto/Seller/Subscription/SubscriptionUsageDto.cs`
- Create: `backend/Domain/Dto/Seller/Subscription/UpgradePreviewDto.cs`
- Create: `backend/Domain/Dto/Seller/Subscription/UpgradeRequestDto.cs`

- [ ] **Step 1: SubscriptionUsageDto**

```csharp
using Base.Entities;

namespace Domain.Dto.Seller.Subscription;

public class SubscriptionUsageDto : IDto
{
    public int OrderCount { get; set; }
    public int MaxOrdersPerMonth { get; set; }
    public double UsagePercentage { get; set; }
    public int RemainingDays { get; set; }
    public string PlanName { get; set; }
}
```

- [ ] **Step 2: UpgradePreviewDto**

```csharp
using Base.Entities;

namespace Domain.Dto.Seller.Subscription;

public class UpgradePreviewDto : IDto
{
    public string CurrentPlanName { get; set; }
    public string TargetPlanName { get; set; }
    public decimal CurrentPrice { get; set; }
    public decimal TargetPrice { get; set; }
    public decimal ProratedAmount { get; set; }
    public int RemainingDays { get; set; }
    public int TotalDays { get; set; }
    public int TargetMaxOrders { get; set; }
}
```

- [ ] **Step 3: UpgradeRequestDto**

```csharp
using Base.Entities;

namespace Domain.Dto.Seller.Subscription;

public class UpgradeRequestDto : IDto
{
    public Guid RestaurantId { get; set; }
    public Guid TargetPlanId { get; set; }
}
```

- [ ] **Step 4: Commit**

```bash
git add backend/Domain/Dto/Seller/Subscription/
git commit -m "feat: add subscription usage and upgrade DTOs"
```

---

### Task 18: Implement Subscription Usage Logic in SubscriptionManager

**Files:**
- Modify: `backend/Application/Services/Seller/SubscriptionService/SubscriptionManager.cs`

- [ ] **Step 1: Add new methods to ISubscriptionService interface**

```csharp
Task<ServiceObjectResult<SubscriptionUsageDto>> GetUsageAsync(Guid sellerId, Guid restaurantId);
Task<ServiceObjectResult<UpgradePreviewDto>> GetUpgradePreviewAsync(Guid sellerId, Guid restaurantId, Guid targetPlanId);
Task<ServiceObjectResult<bool>> UpgradePlanAsync(Guid sellerId, UpgradeRequestDto request);
Task<ServiceObjectResult<bool>> IncrementOrderCountAsync(Guid restaurantId); // called from OrderManager
```

- [ ] **Step 2: Implement GetUsageAsync**

Uses EF Core to get current subscription + usage for the restaurant this month.

- [ ] **Step 3: Implement IncrementOrderCountAsync with Dapper atomic SQL**

```csharp
public async Task<ServiceObjectResult<bool>> IncrementOrderCountAsync(Guid restaurantId)
{
    var result = new ServiceObjectResult<bool>();
    try
    {
        // Find active subscription for restaurant
        var subscription = await _subscriptionRepository.GetAsync(
            s => s.RestaurantId == restaurantId &&
                 s.StatusId == (short)AuthorizationServiceEnums.SubscriptionStatusEnums.Active);

        if (subscription == null)
        {
            result.Fail("Aktif abonelik bulunamadı.");
            return result;
        }

        var plan = await _subscriptionPlanRepository.GetAsync(p => p.Id == subscription.SubscriptionPlanId);
        var now = DateTime.UtcNow;

        // Get DB connection for Dapper
        var conn = _context.Database.GetDbConnection();
        if (conn.State != System.Data.ConnectionState.Open)
            await conn.OpenAsync();

        // Ensure usage record exists (lazy create)
        var usageId = Guid.NewGuid();
        await conn.ExecuteAsync(@"
            INSERT INTO SubscriptionUsage (Id, SubscriptionId, Year, Month, OrderCount, CreatedDate, UpdatedDate)
            VALUES (@id, @subscriptionId, @year, @month, 0, @now, @now)
            ON DUPLICATE KEY UPDATE OrderCount = OrderCount",
            new { id = usageId, subscriptionId = subscription.Id, year = now.Year, month = now.Month, now });

        // Atomic increment with limit check
        var affected = await conn.ExecuteAsync(@"
            UPDATE SubscriptionUsage
            SET OrderCount = OrderCount + 1, UpdatedDate = @now
            WHERE SubscriptionId = @subscriptionId
              AND Year = @year AND Month = @month
              AND OrderCount < @maxOrders",
            new { subscriptionId = subscription.Id, year = now.Year, month = now.Month,
                  maxOrders = plan.MaxOrdersPerMonth, now });

        if (affected == 0)
        {
            result.Fail("Aylık sipariş limitinize ulaştınız. Paketinizi yükseltin.");
            return result;
        }

        result.SetData(true);
    }
    catch (Exception ex)
    {
        result.Fail($"Hata: {ex.Message}");
    }
    return result;
}
```

- [ ] **Step 4: Implement GetUpgradePreviewAsync and UpgradePlanAsync**

Proration formula: `(targetPrice - currentPrice) * (remainingDays / totalDays)`

- [ ] **Step 5: Commit**

```bash
git add backend/Application/Services/Seller/SubscriptionService/
git commit -m "feat: add subscription usage tracking with atomic increment"
```

---

### Task 19: Integrate Order Count Check in OrderManager

**Files:**
- Modify: `backend/Application/Services/Buyer/OrderService/OrderManager.cs`

- [ ] **Step 1: Inject ISubscriptionService into OrderManager constructor**

- [ ] **Step 2: Add subscription check in CreateOrder method**

Before creating the order, call:
```csharp
var usageResult = await _subscriptionService.IncrementOrderCountAsync(order.RestaurantId);
if (usageResult.HasFailed)
{
    result.Fail(usageResult.Messages.First().Description);
    return result;
}
```

- [ ] **Step 3: Commit**

```bash
git add backend/Application/Services/Buyer/OrderService/OrderManager.cs
git commit -m "feat: check subscription order limit when creating orders"
```

---

### Task 20: Update Subscription Endpoints

**Files:**
- Modify: `backend/WebAPI/Controllers/Seller/SubscriptionController.cs` (or create new)

- [ ] **Step 1: Add usage and upgrade endpoints**

```csharp
[HttpGet("usage")]
[AuthorizeAPIRequest(true, false,
    AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
    AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
public async Task<ServiceObjectResult<SubscriptionUsageDto>> GetUsage(
    [FromQuery] Guid restaurantId)
    => await _subscriptionService.GetUsageAsync(CurrentUserId, restaurantId);

[HttpGet("upgrade/preview")]
[AuthorizeAPIRequest(true, false,
    AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
public async Task<ServiceObjectResult<UpgradePreviewDto>> GetUpgradePreview(
    [FromQuery] Guid restaurantId, [FromQuery] Guid planId)
    => await _subscriptionService.GetUpgradePreviewAsync(CurrentUserId, restaurantId, planId);

[HttpPost("upgrade")]
[AuthorizeAPIRequest(true, false,
    AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
public async Task<ServiceObjectResult<bool>> Upgrade(
    [FromBody] UpgradeRequestDto request)
    => await _subscriptionService.UpgradePlanAsync(CurrentUserId, request);
```

- [ ] **Step 2: Update plans endpoint to include MaxOrdersPerMonth in response**

- [ ] **Step 3: Commit**

```bash
git add backend/WebAPI/Controllers/
git commit -m "feat: add subscription usage and upgrade endpoints"
```

---

### Task 21: Fix iyzico Integration — Blocking Registration on Seller Approval

**Files:**
- Modify: `backend/Application/Services/Seller/1_SellerService/SellerManager.cs`
- Modify: `backend/Domain/Dto/Payment/CreateSubMerchantDto.cs`
- Modify: `backend/Infrastructure/Adapters/IyzicoServiceAdapter/IyzicoServiceAdapter.cs`

- [ ] **Step 1: Add IdentityNumber to CreateSubMerchantDto**

```csharp
public string? IdentityNumber { get; set; }
```

- [ ] **Step 2: Update IyzicoServiceAdapter to use IdentityNumber for PERSONAL type**

In `CreateSeller` method, for Individual type:
```csharp
// Change: subMerchantType from PRIVATE_COMPANY to PERSONAL
// Use dto.IdentityNumber instead of dto.TaxCode for identity
```

- [ ] **Step 3: Make iyzico registration blocking in ConfirmSeller**

Change the existing try-catch that silently swallows iyzico errors:
```csharp
// BEFORE: iyzico failure was ignored, seller approved anyway
// AFTER: if iyzico fails, return error, don't approve seller

var iyzicoResult = await _iyzicoServiceAdapter.CreateSeller(subMerchantDto);
if (iyzicoResult == null || string.IsNullOrEmpty(iyzicoResult.SubMerchantKey))
{
    result.Fail("iyzico alt üye işyeri kaydı başarısız. Lütfen tekrar deneyin.");
    return result;
}

// Save subMerchantKey to SellerDetail
// ... existing SellerDetail save logic
```

- [ ] **Step 4: Add retry endpoint for admin**

Add `RetryIyzicoRegistration` method in SellerManager that:
1. Checks if subMerchantKey already exists in SellerDetail
2. If exists → call UpdateSeller instead
3. If not → call CreateSeller, handle "already exists" error by falling back to UpdateSeller

- [ ] **Step 5: Add seller address to iyzico call** (replace hardcoded "Test Adres")

Query seller's address from Address repository and use it.

- [ ] **Step 6: Update admin seller controller** to include retry endpoint

```csharp
[HttpPut("sellers/{id}/retry-iyzico")]
[AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
public async Task<ServiceObjectResult<bool>> RetryIyzico(Guid id)
    => await _sellerService.RetryIyzicoRegistrationAsync(id);
```

- [ ] **Step 7: Commit**

```bash
git add backend/Application/Services/Seller/1_SellerService/ backend/Domain/Dto/Payment/ backend/Infrastructure/Adapters/IyzicoServiceAdapter/ backend/WebAPI/Controllers/Admin/
git commit -m "feat: make iyzico registration blocking on seller approval, add retry endpoint"
```

---

### Task 22: Add Courier Deliver Endpoint

**Files:**
- Modify: `backend/Application/Services/Courier/CourierOrderService/` (or create new)
- Modify: courier order controller

- [ ] **Step 1: Add DeliverOrderAsync method**

```csharp
public async Task<ServiceObjectResult<bool>> DeliverOrderAsync(Guid courierId, Guid orderId)
{
    var result = new ServiceObjectResult<bool>();
    try
    {
        var order = await _orderRepository.GetAsync(o => o.Id == orderId);
        if (order == null)
        {
            result.Fail("Sipariş bulunamadı.");
            return result;
        }

        // Validate courier ownership
        if (order.CourierId != courierId && order.PickedUpByCourierId != courierId)
        {
            result.Fail("Bu siparişi teslim etme yetkiniz yok.");
            return result;
        }

        if (order.StatusId != (short)AuthorizationServiceEnums.OrderStatusEnums.OnTheWay)
        {
            result.Fail("Sipariş 'Yolda' durumunda değil.");
            return result;
        }

        order.StatusId = (short)AuthorizationServiceEnums.OrderStatusEnums.Delivered;
        order.DeliveredAt = DateTime.UtcNow;
        await _orderRepository.UpdateAsync(order);
        await _unitOfWork.CompleteAsync();

        result.SetData(true);
        result.AddSuccessMessage("Sipariş teslim edildi.");
    }
    catch (Exception ex)
    {
        result.Fail($"Hata: {ex.Message}");
    }
    return result;
}
```

- [ ] **Step 2: Add controller endpoint**

```csharp
[HttpPut("orders/{orderId}/deliver")]
[AuthorizeAPIRequest(true, false,
    AuthorizationServiceEnums.UserRoleEnums.Courier,
    AuthorizationServiceEnums.UserRoleEnums.CourierCompanyAdmin)]
public async Task<ServiceObjectResult<bool>> DeliverOrder(Guid orderId)
    => await _courierOrderService.DeliverOrderAsync(CurrentUserId, orderId);
```

- [ ] **Step 3: Add customer courier-location endpoint**

```csharp
// In CustomerOrderController or new endpoint:
[HttpGet("order/{orderId}/courier-location")]
[AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.User)]
public async Task<ServiceObjectResult<CourierLocationDto>> GetCourierLocation(Guid orderId)
    => await _orderService.GetCourierLocationForOrderAsync(orderId);
```

Implementation finds courier via `Order.CourierId ?? Order.PickedUpByCourierId`, then queries `CourierLocation`.

- [ ] **Step 4: Commit**

```bash
git add backend/
git commit -m "feat: add courier deliver and customer courier-location endpoints"
```

---

## Chunk 4: Frontend — Admin Panel Changes

### Task 23: Admin Courier Companies Page

**Files:**
- Create: `front-admin/app/courier-companies/page.js`
- Modify: `front-admin/lib/api.js` (add new API functions)
- Modify: `front-admin/components/layout/Sidebar.js` (add nav link)

- [ ] **Step 1: Add API functions in api.js**

```javascript
// Courier Companies
export const getCourierCompanies = (page = 1, size = 20) =>
  api.get(`/v1/admin/courier-companies?page=${page}&size=${size}`).then(r => r.data);

export const approveCourierCompany = (id) =>
  api.put(`/v1/admin/courier-companies/${id}/approve`).then(r => r.data);

export const rejectCourierCompany = (id) =>
  api.put(`/v1/admin/courier-companies/${id}/reject`).then(r => r.data);

export const suspendCourierCompany = (id) =>
  api.put(`/v1/admin/courier-companies/${id}/suspend`).then(r => r.data);

export const banCourierCompany = (id) =>
  api.put(`/v1/admin/courier-companies/${id}/ban`).then(r => r.data);
```

- [ ] **Step 2: Create courier-companies page**

Follow existing pattern from `front-admin/app/couriers/page.js`:
- Table with columns: Firma Adı, Yetkili Email, Durum, Kurye Sayısı, Kayıt Tarihi, İşlemler
- Status badge (color-coded)
- Action buttons: Onayla, Reddet, Askıya Al, Yasakla
- Confirmation modal before actions
- Pagination
- SWR for data fetching

- [ ] **Step 3: Add sidebar navigation link**

Add "Kurye Firmaları" link in admin sidebar, after existing "Kuryeler" link.

- [ ] **Step 4: Commit**

```bash
git add front-admin/
git commit -m "feat: add courier companies admin page with approval flow"
```

---

### Task 24: Admin Seller Approval — iyzico Integration UI

**Files:**
- Modify: `front-admin/app/sellers/page.js` (or detail page)

- [ ] **Step 1: Add iyzico status badge to seller detail/list**

Show badge based on whether seller has subMerchantKey:
- Kayıtlı (green), Kayıtsız (gray), Hatalı (red)

- [ ] **Step 2: Update approve flow**

When admin clicks "Onayla":
1. Show loading spinner
2. Call approve API
3. On success → show success toast, refresh
4. On failure → show error message with "Tekrar Dene" button

- [ ] **Step 3: Add retry button for sellers with missing iyzico key**

```javascript
const handleRetryIyzico = async (sellerId) => {
  setRetrying(true);
  const res = await retryIyzicoRegistration(sellerId);
  if (res.hasFailed) {
    toast.error(res.messages?.[0]?.description || 'iyzico kaydı başarısız');
  } else {
    toast.success('iyzico kaydı başarılı');
    mutate(); // refresh SWR
  }
  setRetrying(false);
};
```

- [ ] **Step 4: Commit**

```bash
git add front-admin/
git commit -m "feat: add iyzico status and retry to seller approval UI"
```

---

## Chunk 5: Frontend — Seller Panel Changes

### Task 25: Subscription Usage Widget on Seller Dashboard

**Files:**
- Modify: `front-seller/app/dashboard/page.js`
- Modify: `front-seller/lib/api.js`

- [ ] **Step 1: Add API functions**

```javascript
export const getSubscriptionUsage = (restaurantId) =>
  api.get(`/v1/subscription/usage?restaurantId=${restaurantId}`).then(r => r.data);

export const getUpgradePreview = (restaurantId, planId) =>
  api.get(`/v1/subscription/upgrade/preview?restaurantId=${restaurantId}&planId=${planId}`).then(r => r.data);

export const upgradeSubscription = (data) =>
  api.post('/v1/subscription/upgrade', data).then(r => r.data);
```

- [ ] **Step 2: Add usage widget to dashboard**

Progress bar component showing:
- "Bu ay: 73/100 sipariş" with progress bar
- Yellow warning at 80%, red at 100%
- "Paket Yükselt" button when approaching limit
- Remaining days display

- [ ] **Step 3: Commit**

```bash
git add front-seller/
git commit -m "feat: add subscription usage widget to seller dashboard"
```

---

### Task 26: Courier Company Management in Seller Panel

**Files:**
- Create: `front-seller/app/restaurants/[id]/courier-companies/page.js`
- Modify: `front-seller/lib/api.js`

- [ ] **Step 1: Add API functions for courier companies**

```javascript
export const getRestaurantCourierCompanies = (restaurantId) =>
  fetcher(`/v1/seller/restaurant/${restaurantId}/courier-companies`);

export const inviteCourierCompany = (restaurantId, companyId) =>
  api.post(`/v1/seller/restaurant/${restaurantId}/courier-company/add`, { companyId }).then(r => r.data);

export const removeCourierCompany = (restaurantId, companyId) =>
  api.delete(`/v1/seller/restaurant/${restaurantId}/courier-company/${companyId}`).then(r => r.data);
```

- [ ] **Step 2: Create courier-companies page**

Follow existing pattern from `restaurants/[id]/couriers/page.js`:
- Table: Firma Adı, Durum, Anlaşma Başlangıcı, İşlemler
- "Firma Ekle" button → modal with company search
- Remove button with confirmation

- [ ] **Step 3: Update seller order dispatch to allow company selection**

In the order management page, when changing status to "Yola Çıktı":
- Show dropdown: "Bireysel Kurye" or "Kurumsal Firma"
- If company selected → assign CourierCompanyId
- If individual → existing courier selection flow

- [ ] **Step 4: Commit**

```bash
git add front-seller/
git commit -m "feat: add courier company management and dispatch to seller panel"
```

---

## Chunk 6: Frontend — Courier App Changes

### Task 27: Company Registration Screen

**Files:**
- Create: `front-courier/src/screens/Company/CompanyRegistrationScreen.js`
- Modify: `front-courier/src/api/courierService.js`
- Modify: `front-courier/src/navigation/AppNavigator.js`

- [ ] **Step 1: Add API functions**

```javascript
// In courierService.js
export const registerCompany = (data) =>
  apiClient.post('/v1/courier/company/register', data).then(r => r.data);

export const getMyCompany = () =>
  apiClient.get('/v1/courier/company/my').then(r => r.data);

export const updateCompany = (data) =>
  apiClient.put('/v1/courier/company/my', data).then(r => r.data);

export const searchCouriers = (query) =>
  apiClient.get(`/v1/courier/company/members/search?q=${query}`).then(r => r.data);

export const requestMembership = (courierId) =>
  apiClient.post('/v1/courier/company/members/request', { courierId }).then(r => r.data);

export const getCompanyMembers = () =>
  apiClient.get('/v1/courier/company/members').then(r => r.data);

export const removeMember = (id) =>
  apiClient.delete(`/v1/courier/company/members/${id}`).then(r => r.data);

export const getCompanyInvites = () =>
  apiClient.get('/v1/courier/company-invites').then(r => r.data);

export const acceptCompanyInvite = (id) =>
  apiClient.put(`/v1/courier/company-invites/${id}/accept`).then(r => r.data);

export const rejectCompanyInvite = (id) =>
  apiClient.put(`/v1/courier/company-invites/${id}/reject`).then(r => r.data);

export const leaveCompany = () =>
  apiClient.put('/v1/courier/company/leave').then(r => r.data);

export const getRestaurantInvites = () =>
  apiClient.get('/v1/courier/company/restaurant-invites').then(r => r.data);

export const acceptRestaurantInvite = (id) =>
  apiClient.put(`/v1/courier/company/restaurant-invites/${id}/accept`).then(r => r.data);

export const rejectRestaurantInvite = (id) =>
  apiClient.put(`/v1/courier/company/restaurant-invites/${id}/reject`).then(r => r.data);

export const getPendingPickups = (restaurantId, page = 1, size = 20) =>
  apiClient.get(`/v1/courier/pickup/${restaurantId}/pending?page=${page}&size=${size}`).then(r => r.data);

export const confirmPickup = (orderId) =>
  apiClient.put(`/v1/courier/pickup/${orderId}/confirm`).then(r => r.data);

export const deliverOrder = (orderId) =>
  apiClient.put(`/v1/courier/orders/${orderId}/deliver`).then(r => r.data);
```

- [ ] **Step 2: Create CompanyRegistrationScreen**

Form with fields: Firma Adı, Yasal Ünvan, Vergi No, Vergi Dairesi, E-posta, Telefon
Submit → registerCompany API → success → navigate to profile

- [ ] **Step 3: Update navigation for role-based tabs**

```javascript
// In AppNavigator.js, check user role:
// If CourierCompanyAdmin → show extra "Firma" tab
const isCompanyAdmin = userInfo?.userRoleId === 7;

// Add CompanyTab if admin
{isCompanyAdmin && (
  <Tab.Screen name="Company" component={CompanyStackNavigator} ... />
)}
```

- [ ] **Step 4: Commit**

```bash
git add front-courier/
git commit -m "feat: add company registration and role-based navigation"
```

---

### Task 28: Company Management Screens

**Files:**
- Create: `front-courier/src/screens/Company/CompanyDashboardScreen.js`
- Create: `front-courier/src/screens/Company/MemberManagementScreen.js`
- Create: `front-courier/src/screens/Company/RestaurantInvitesScreen.js`

- [ ] **Step 1: CompanyDashboardScreen** — shows company info, member count, status
- [ ] **Step 2: MemberManagementScreen** — search couriers, send claims, list members, remove
- [ ] **Step 3: RestaurantInvitesScreen** — list restaurant invites, accept/reject
- [ ] **Step 4: Commit**

```bash
git add front-courier/src/screens/Company/
git commit -m "feat: add company management screens for courier app"
```

---

### Task 29: Pickup Flow Screen

**Files:**
- Create: `front-courier/src/screens/Orders/PickupScreen.js`
- Modify: `front-courier/src/screens/Orders/ActiveOrdersScreen.js`

- [ ] **Step 1: Create PickupScreen**

- Shows pending orders for a restaurant (FlatList)
- Each order card: order number, delivery address, distance, price
- "Teslim Aldım" button per order
- Pull-to-refresh
- Confirm modal before pickup

- [ ] **Step 2: Update ActiveOrdersScreen to include pickup and deliver actions**

- For picked-up orders: show "Haritada Gör" and "Navigasyonu Başlat" buttons
- "Teslim Ettim" button → deliverOrder API

- [ ] **Step 3: Commit**

```bash
git add front-courier/src/screens/Orders/
git commit -m "feat: add pickup flow and deliver actions to courier app"
```

---

### Task 30: Navigation and Location Tracking

**Files:**
- Create: `front-courier/src/screens/Orders/OrderMapScreen.js`
- Create: `front-courier/src/services/locationTracker.js`

- [ ] **Step 1: Create locationTracker service**

```javascript
import Geolocation from 'react-native-geolocation-service';
import { updateLocation } from '../api/courierService';

let watchId = null;
let intervalId = null;

export const startTracking = () => {
  intervalId = setInterval(async () => {
    Geolocation.getCurrentPosition(
      async (position) => {
        try {
          await updateLocation({
            latitude: position.coords.latitude,
            longitude: position.coords.longitude,
          });
        } catch (e) {
          console.warn('Location update failed:', e);
        }
      },
      (error) => console.warn('GPS error:', error),
      { enableHighAccuracy: true, timeout: 10000 }
    );
  }, 15000); // every 15 seconds
};

export const stopTracking = () => {
  if (intervalId) {
    clearInterval(intervalId);
    intervalId = null;
  }
};
```

- [ ] **Step 2: Create OrderMapScreen** (in-app map view)

```javascript
// Uses react-native-maps
// Shows: courier location (blue pin), delivery address (red pin)
// "Navigasyonu Başlat" button → deep link to external maps
```

- [ ] **Step 3: Add deep link navigation helper**

```javascript
import { Linking, Platform } from 'react-native';

export const openNavigation = (lat, lng) => {
  const url = Platform.select({
    ios: `maps://app?daddr=${lat},${lng}`,
    android: `google.navigation:q=${lat},${lng}`,
  });
  Linking.openURL(url);
};
```

- [ ] **Step 4: Integrate tracking with order lifecycle**

Start tracking when courier has active orders (picked up, not delivered).
Stop when all orders delivered.

- [ ] **Step 5: Install react-native-maps if not already present**

```bash
cd /Users/onuryorulmaz/Desktop/Github/food-order/front-courier
npm install react-native-maps
```

- [ ] **Step 6: Commit**

```bash
git add front-courier/
git commit -m "feat: add navigation, map screen, and background location tracking"
```

---

## Chunk 7: Frontend — Customer App Changes

### Task 31: Courier Tracking on Order Detail

**Files:**
- Modify: `front-app/src/screens/Orders/OrderDetailScreen.js`
- Modify: `front-app/src/api/orderService.js` (or relevant service)

- [ ] **Step 1: Add API function**

```javascript
export const getCourierLocation = (orderId) =>
  apiClient.get(`/v1/customer/order/${orderId}/courier-location`).then(r => r.data);
```

- [ ] **Step 2: Add map to OrderDetailScreen when status is OnTheWay**

When `order.statusId === 7` (OnTheWay):
- Show MapView with courier pin (blue) and delivery address pin (red)
- Poll courier location every 30 seconds
- Show estimated time: `Math.ceil(distance / 25 * 60)` minutes
- Show courier name and phone with call button

```javascript
// Polling logic
useEffect(() => {
  if (order?.statusId !== 7) return;

  const fetchLocation = async () => {
    const res = await getCourierLocation(orderId);
    if (!res.hasFailed && res.data) {
      setCourierLocation({
        latitude: res.data.latitude,
        longitude: res.data.longitude,
      });
    }
  };

  fetchLocation();
  const interval = setInterval(fetchLocation, 30000);
  return () => clearInterval(interval);
}, [order?.statusId, orderId]);
```

- [ ] **Step 3: Install react-native-maps if not already present**

```bash
cd /Users/onuryorulmaz/Desktop/Github/food-order/front-app
npm install react-native-maps
```

- [ ] **Step 4: Commit**

```bash
git add front-app/
git commit -m "feat: add courier tracking map to customer order detail"
```

---

## Chunk 8: Rate Limiting & Final Integration

### Task 32: Configure Rate Limiting for Location Endpoint

**Files:**
- Modify: `backend/WebAPI/Program.cs`

- [ ] **Step 1: Add separate rate limit policy for courier location**

```csharp
options.AddFixedWindowLimiter("courier-location", limiterOptions =>
{
    limiterOptions.PermitLimit = 10;
    limiterOptions.Window = TimeSpan.FromSeconds(15);
});
```

- [ ] **Step 2: Apply to location endpoint** via `[EnableRateLimiting("courier-location")]` attribute

- [ ] **Step 3: Commit**

```bash
git add backend/WebAPI/
git commit -m "feat: add dedicated rate limit for courier location endpoint"
```

---

### Task 33: Update SubscriptionPlan Response DTO

**Files:**
- Modify: relevant subscription plan DTO to include `MaxOrdersPerMonth` and `OverageAction`

- [ ] **Step 1: Update DTO**
- [ ] **Step 2: Update mapper/response in SubscriptionManager**
- [ ] **Step 3: Commit**

```bash
git add backend/
git commit -m "feat: include MaxOrdersPerMonth in subscription plan responses"
```

---

### Task 34: Company Invite Notifications in Courier App

**Files:**
- Create: `front-courier/src/screens/Company/CompanyInviteScreen.js`

- [ ] **Step 1: Create screen showing pending company invites**

FlatList with invite cards: company name, contact email, request date
Accept/Reject buttons per invite

- [ ] **Step 2: Add to navigation** — show in Profile tab or as alert on login

- [ ] **Step 3: Commit**

```bash
git add front-courier/
git commit -m "feat: add company invite notification screen"
```

---

### Task 35: Final Build Verification

- [ ] **Step 1: Build backend**

```bash
cd /Users/onuryorulmaz/Desktop/Github/food-order/backend
dotnet build
```

Expected: 0 errors

- [ ] **Step 2: Verify migration applies cleanly**

```bash
dotnet ef database update --project Persistence --startup-project WebAPI
```

- [ ] **Step 3: Build front-admin**

```bash
cd /Users/onuryorulmaz/Desktop/Github/food-order/front-admin
npm run build
```

- [ ] **Step 4: Build front-seller**

```bash
cd /Users/onuryorulmaz/Desktop/Github/food-order/front-seller
npm run build
```

- [ ] **Step 5: Verify front-courier compiles**

```bash
cd /Users/onuryorulmaz/Desktop/Github/food-order/front-courier
npx react-native build-android --mode=debug 2>&1 | head -20
```

Or just check for JS bundle errors:
```bash
npx react-native start --reset-cache &
sleep 5 && kill %1
```

- [ ] **Step 6: Verify front-app compiles** (same as above)

- [ ] **Step 7: Final commit if any loose changes**

```bash
git add -A
git status
```
