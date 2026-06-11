# Restoran Self-Servis Onboarding Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Restoranların admin müdahalesi olmadan platforma başvurabilmesi: public kayıt endpoint'i + front-seller kayıt sayfası + bekleyen başvuru için anlamlı login mesajı. Admin onayı (`ConfirmSeller` → İyzico alt üye) dolandırıcılık filtresi olarak aynen kalır.

**Architecture:** Mevcut `SellerManager.AddSeller` (Pending satıcı + SellerAdmin kullanıcı oluşturur) yeniden kullanılır; önüne saf bir doğrulama sınıfı (`SellerRegistrationRules`) ve duplicate e-posta kontrolü eklenir, anonim `POST /v1/seller/register` endpoint'i ile dışarı açılır. front-seller'a login deseninde 2 adımlı kayıt sayfası eklenir. Onay akışı değişmez: admin `POST /v1/admin/seller/confirm` → İyzico SubMerchant → kullanıcı Active.

**Tech Stack:** .NET 8 (Clean Architecture, Manager pattern, ServiceResult), xUnit (yeni test projesi), Next.js App Router (JS, Tailwind), paylaşılan `service/` paketi (axios + zod).

**Önkoşul notları:**
- Backend portu: `http://localhost:3762` (`front-seller/lib/service.js` BASE_URL).
- City/Town entity'si yok; admin formu gibi boş Guid (`00000000-0000-0000-0000-000000000000`) gönderilir. İl/ilçe lookup bu planın kapsamı DIŞINDA.
- Mail altyapısı yok; bilgilendirme mesajları "onaylandığında giriş yapabilirsiniz" der, e-posta vaat etmez.
- `ConfirmSeller`, Individual satıcıda 11 haneli TC arar (`SellerManager.cs:144-149`) — bu yüzden kayıtta `IdentityNumber` toplanır.

---

## Dosya Haritası

| Dosya | İşlem | Sorumluluk |
|---|---|---|
| `backend/Application.Tests/Application.Tests.csproj` | Create | Test projesi (xUnit) |
| `backend/Application.Tests/SellerRegistrationRulesTests.cs` | Create | Doğrulama kuralı testleri |
| `backend/Domain/Dto/Seller/AddSellerDto.cs` | Modify | `IdentityNumber` alanı |
| `backend/Application/Services/Seller/SellerService/SellerRegistrationRules.cs` | Create | Saf doğrulama kuralları |
| `backend/Application/Services/Seller/SellerService/SellerManager.cs` | Modify | Doğrulama + duplicate e-posta + IdentityNumber persist + e-posta lowercase |
| `backend/Application/Services/Common/UserService/UserManager.cs` | Modify | Login'de "onay bekliyor" mesajı |
| `backend/WebAPI/Controllers/Base/SellerRegisterController.cs` | Create | Anonim kayıt endpoint'i |
| `service/schema/auth.js` | Modify | `SellerRegisterRequestSchema` (zod) |
| `service/services/authService.js` | Modify | `registerSeller()` çağrısı |
| `front-seller/lib/auth.js` | Modify | `registerSeller` helper |
| `front-seller/app/register/page.js` | Create | 2 adımlı kayıt formu |
| `front-seller/app/login/page.js` | Modify | Kayıt sayfası linki |
| `backend/CLAUDE.md` | Modify | Bayat abonelik içeriği → komisyon gerçeği |

---

### Task 1: Test altyapısını kur (xUnit)

Backend'de hiç test projesi yok. Saf doğrulama sınıfını TDD ile yazabilmek için önce altyapı.

**Files:**
- Create: `backend/Application.Tests/Application.Tests.csproj`
- Create: `backend/Application.Tests/SmokeTests.cs`
- Modify: `backend/FoodOrder.Backend.sln` (dotnet sln komutu ile)

- [ ] **Step 1: Test projesini oluştur**

`backend/Application.Tests/Application.Tests.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.11.1" />
    <PackageReference Include="xunit" Version="2.9.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.8.2" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="../Application/Application.csproj" />
  </ItemGroup>

</Project>
```

`backend/Application.Tests/SmokeTests.cs`:

```csharp
namespace Application.Tests;

public class SmokeTests
{
    [Fact]
    public void TestInfrastructure_Works() => Assert.True(true);
}
```

- [ ] **Step 2: Solution'a ekle ve çalıştır**

```bash
cd ~/Desktop/Github/food-order/backend
dotnet sln FoodOrder.Backend.sln add Application.Tests/Application.Tests.csproj
dotnet test Application.Tests --nologo
```

Beklenen: `Passed! - Failed: 0, Passed: 1`

- [ ] **Step 3: Commit**

```bash
git add Application.Tests FoodOrder.Backend.sln
git commit -m "test: xUnit test projesi — Application.Tests"
```

---

### Task 2: SellerRegistrationRules — saf doğrulama (TDD)

`AddSellerDto`'ya `IdentityNumber` eklenir; tüm alan doğrulamaları saf statik sınıfta toplanır (mock gerekmez).

**Files:**
- Modify: `backend/Domain/Dto/Seller/AddSellerDto.cs`
- Create: `backend/Application/Services/Seller/SellerService/SellerRegistrationRules.cs`
- Test: `backend/Application.Tests/SellerRegistrationRulesTests.cs`

- [ ] **Step 1: DTO'ya IdentityNumber ekle**

`backend/Domain/Dto/Seller/AddSellerDto.cs` içinde `IsEInvoiceAvaible` satırının altına:

```csharp
    public string? IdentityNumber { get; set; }
```

- [ ] **Step 2: Başarısız olacak testleri yaz**

`backend/Application.Tests/SellerRegistrationRulesTests.cs`:

```csharp
using Application.Services.Seller.SellerService;
using Base.Enums;
using Domain.Dto.Seller;

namespace Application.Tests;

public class SellerRegistrationRulesTests
{
    private static AddSellerDto ValidDto() => new()
    {
        CompanyType = (short)CompanyTypeEnums.Company,
        Name = "Test Lokantası",
        LegalName = "Test Gıda Ltd. Şti.",
        TaxCode = "1234567890",
        TaxArea = "Kadıköy VD",
        IBAN = "TR" + new string('1', 24),
        IsEInvoiceAvaible = false,
        OwnerFirstName = "Onur",
        OwnerLastName = "Test",
        OwnerEmail = "onur@test.com",
        OwnerPhone = "5551112233",
        Password = "Sifre1234",
        AddressLine1 = "Test Mah. Deneme Sk. No:1"
    };

    [Fact]
    public void Validate_ValidCompanyDto_ReturnsNoErrors()
        => Assert.Empty(SellerRegistrationRules.Validate(ValidDto()));

    [Fact]
    public void Validate_InvalidEmail_ReturnsError()
    {
        var dto = ValidDto();
        dto.OwnerEmail = "gecersiz-eposta";
        Assert.Contains(SellerRegistrationRules.Validate(dto), e => e.Contains("e-posta"));
    }

    [Fact]
    public void Validate_ShortPassword_ReturnsError()
    {
        var dto = ValidDto();
        dto.Password = "kisa";
        Assert.Contains(SellerRegistrationRules.Validate(dto), e => e.Contains("Şifre"));
    }

    [Fact]
    public void Validate_InvalidIban_ReturnsError()
    {
        var dto = ValidDto();
        dto.IBAN = "TR123";
        Assert.Contains(SellerRegistrationRules.Validate(dto), e => e.Contains("IBAN"));
    }

    [Fact]
    public void Validate_IbanWithSpaces_IsAccepted()
    {
        var dto = ValidDto();
        dto.IBAN = "TR11 1111 1111 1111 1111 1111 11";
        Assert.Empty(SellerRegistrationRules.Validate(dto));
    }

    [Fact]
    public void Validate_NonNumericTaxCode_ReturnsError()
    {
        var dto = ValidDto();
        dto.TaxCode = "12345abcde";
        Assert.Contains(SellerRegistrationRules.Validate(dto), e => e.Contains("Vergi"));
    }

    [Fact]
    public void Validate_IndividualWithoutIdentityNumber_ReturnsError()
    {
        var dto = ValidDto();
        dto.CompanyType = (short)CompanyTypeEnums.Individual;
        dto.TaxCode = "12345678901";
        dto.IdentityNumber = null;
        Assert.Contains(SellerRegistrationRules.Validate(dto), e => e.Contains("TC"));
    }

    [Fact]
    public void Validate_IndividualWithValidIdentityNumber_ReturnsNoErrors()
    {
        var dto = ValidDto();
        dto.CompanyType = (short)CompanyTypeEnums.Individual;
        dto.TaxCode = "12345678901";
        dto.IdentityNumber = "12345678901";
        Assert.Empty(SellerRegistrationRules.Validate(dto));
    }

    [Fact]
    public void Validate_MissingBusinessName_ReturnsError()
    {
        var dto = ValidDto();
        dto.Name = "";
        Assert.Contains(SellerRegistrationRules.Validate(dto), e => e.Contains("İşletme adı"));
    }

    [Fact]
    public void Validate_ShortPhone_ReturnsError()
    {
        var dto = ValidDto();
        dto.OwnerPhone = "123";
        Assert.Contains(SellerRegistrationRules.Validate(dto), e => e.Contains("telefon"));
    }
}
```

- [ ] **Step 3: Testlerin derlenemediğini/başarısız olduğunu doğrula**

```bash
cd ~/Desktop/Github/food-order/backend && dotnet test Application.Tests --nologo 2>&1 | tail -5
```

Beklenen: derleme hatası — `SellerRegistrationRules` henüz yok.

- [ ] **Step 4: SellerRegistrationRules'u yaz**

`backend/Application/Services/Seller/SellerService/SellerRegistrationRules.cs`:

```csharp
using System.Text.RegularExpressions;
using Base.Enums;
using Domain.Dto.Seller;

namespace Application.Services.Seller.SellerService;

public static class SellerRegistrationRules
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
    private static readonly Regex IbanRegex = new(@"^TR\d{24}$", RegexOptions.Compiled);

    public static List<string> Validate(AddSellerDto dto)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(dto.Name))
            errors.Add("İşletme adı zorunludur.");
        if (string.IsNullOrWhiteSpace(dto.LegalName))
            errors.Add("Ticari unvan zorunludur.");
        if (string.IsNullOrWhiteSpace(dto.OwnerFirstName))
            errors.Add("Yetkili adı zorunludur.");
        if (string.IsNullOrWhiteSpace(dto.OwnerLastName))
            errors.Add("Yetkili soyadı zorunludur.");
        if (string.IsNullOrWhiteSpace(dto.AddressLine1))
            errors.Add("Adres zorunludur.");

        if (string.IsNullOrWhiteSpace(dto.OwnerEmail) || !EmailRegex.IsMatch(dto.OwnerEmail.Trim()))
            errors.Add("Geçerli bir e-posta adresi giriniz.");

        if (string.IsNullOrWhiteSpace(dto.OwnerPhone) || dto.OwnerPhone.Count(char.IsDigit) < 10)
            errors.Add("Geçerli bir telefon numarası giriniz (en az 10 hane).");

        if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 8)
            errors.Add("Şifre en az 8 karakter olmalıdır.");

        var iban = dto.IBAN?.Replace(" ", "").ToUpperInvariant() ?? "";
        if (!IbanRegex.IsMatch(iban))
            errors.Add("Geçerli bir IBAN giriniz (TR + 24 hane).");

        var taxCode = dto.TaxCode?.Trim() ?? "";
        if (taxCode.Length is not (10 or 11) || !taxCode.All(char.IsDigit))
            errors.Add("Vergi numarası 10 veya 11 haneli ve rakamlardan oluşmalıdır.");

        if (dto.CompanyType == (short)CompanyTypeEnums.Individual)
        {
            var tc = dto.IdentityNumber?.Trim() ?? "";
            if (tc.Length != 11 || !tc.All(char.IsDigit) || tc[0] == '0')
                errors.Add("Şahıs işletmesi için geçerli 11 haneli TC kimlik numarası zorunludur.");
        }

        return errors;
    }
}
```

- [ ] **Step 5: Testlerin geçtiğini doğrula**

```bash
dotnet test Application.Tests --nologo 2>&1 | tail -3
```

Beklenen: `Passed! - Failed: 0, Passed: 11` (smoke dahil)

- [ ] **Step 6: Commit**

```bash
git add Domain/Dto/Seller/AddSellerDto.cs Application/Services/Seller/SellerService/SellerRegistrationRules.cs Application.Tests/SellerRegistrationRulesTests.cs
git commit -m "feat(backend): satıcı kayıt doğrulama kuralları — SellerRegistrationRules"
```

---

### Task 3: AddSeller'a doğrulama + duplicate e-posta + IdentityNumber

**Files:**
- Modify: `backend/Application/Services/Seller/SellerService/SellerManager.cs:33-115` (`AddSeller` metodu)

- [ ] **Step 1: AddSeller'ı güncelle**

`SellerManager.cs` içinde `AddSeller` metodunun `try` bloğunun BAŞINA (mevcut `var seller = await _sellerRepository.GetAsync(...)` satırından ÖNCE) ekle:

```csharp
            var validationErrors = SellerRegistrationRules.Validate(requestDto);
            if (validationErrors.Count > 0)
            {
                foreach (var error in validationErrors)
                    result.AddErrorMessage(error);
                return result;
            }

            var ownerEmail = requestDto.OwnerEmail.Trim().ToLowerInvariant();
            var existingUser = await _userRepository.GetAsync(x => x.Email == ownerEmail);
            if (existingUser != null)
            {
                result.AddErrorMessage("Bu e-posta adresi ile kayıtlı bir kullanıcı bulunmaktadır.");
                return result;
            }
```

Aynı metotta iki değişiklik daha:

1. `Seller` oluşturulan nesne başlatıcısına (`IsEInvoiceAvaible = requestDto.IsEInvoiceAvaible` satırının altına) ekle:

```csharp
                    IdentityNumber = requestDto.IdentityNumber?.Trim()
```

2. `User` oluşturulan satırdaki `Email = requestDto.OwnerEmail,` satırını şununla DEĞİŞTİR (login e-postayı lowercase arıyor; mevcut kod raw kaydediyordu — bug düzeltmesi):

```csharp
                    Email = ownerEmail,
```

- [ ] **Step 2: Derle ve testleri çalıştır**

```bash
cd ~/Desktop/Github/food-order/backend
dotnet build FoodOrder.Backend.sln --nologo -v q 2>&1 | tail -2 && dotnet test Application.Tests --nologo 2>&1 | tail -2
```

Beklenen: `0 Hata` + tüm testler PASS.

- [ ] **Step 3: Commit**

```bash
git add Application/Services/Seller/SellerService/SellerManager.cs
git commit -m "feat(backend): AddSeller — doğrulama, duplicate e-posta kontrolü, IdentityNumber, e-posta lowercase düzeltmesi"
```

---

### Task 4: Anonim kayıt endpoint'i — POST /v1/seller/register

**Files:**
- Create: `backend/WebAPI/Controllers/Base/SellerRegisterController.cs`

- [ ] **Step 1: Controller'ı yaz**

`backend/WebAPI/Controllers/Base/SellerRegisterController.cs` (UserController ile aynı desende; auth attribute YOK — anonim; rate limit "auth" politikası):

```csharp
using Application.Services.Seller.SellerService;
using Domain.Dto.Seller;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace WebAPI.Controllers.Base;

[Route("v1/seller")]
[ApiController]
public class SellerRegisterController : BaseController
{
    private readonly ISellerService _sellerService;

    public SellerRegisterController(ISellerService sellerService)
    {
        _sellerService = sellerService;
    }

    [HttpPost("register")]
    [EnableRateLimiting("auth")]
    public async Task<ServiceObjectResult<bool>> Register([FromBody] AddSellerDto requestDto)
    {
        return await _sellerService.AddSeller(requestDto);
    }
}
```

Not: `BaseController` aynı namespace'te (`WebAPI.Controllers.Base`) — UserController da ondan türüyor. Route çakışması yok: SellerRestaurantController `v1/seller/restaurant` kullanıyor.

- [ ] **Step 2: Derle ve elle doğrula**

```bash
cd ~/Desktop/Github/food-order/backend && dotnet build FoodOrder.Backend.sln --nologo -v q 2>&1 | tail -2
cd WebAPI && dotnet run --launch-profile http &
sleep 8
curl -s -X POST http://localhost:3762/v1/seller/register -H 'Content-Type: application/json' -d '{
  "companyType": 2,
  "name": "Curl Test Lokantası",
  "legalName": "Curl Test Gıda Ltd.",
  "taxCode": "9876543210",
  "taxArea": "Test VD",
  "iban": "TR111111111111111111111111",
  "isEInvoiceAvaible": false,
  "ownerFirstName": "Test",
  "ownerLastName": "Kullanıcı",
  "ownerEmail": "curltest@example.com",
  "ownerPhone": "5550001122",
  "password": "Sifre1234",
  "cityId": "00000000-0000-0000-0000-000000000000",
  "townId": "00000000-0000-0000-0000-000000000000",
  "neighbourhoodId": "00000000-0000-0000-0000-000000000000",
  "addressLine1": "Test Mah. No:1"
}'
```

Beklenen: `"hasFailed":false` ve `"data":true`. İkinci kez aynı istek → `"Bu vergi numarası ile kayıtlı bir satıcı bulunmaktadır."`. Geçersiz e-posta ile istek → doğrulama mesajı. (`CompanyTypeEnums`: **Individual=1, Şirket=2** — `backend/Base/Enums/CompanyTypeEnums.cs`.)

Sunucuyu durdur: `kill %1`

- [ ] **Step 3: Commit**

```bash
git add WebAPI/Controllers/Base/SellerRegisterController.cs
git commit -m "feat(backend): anonim restoran başvuru endpoint'i — POST /v1/seller/register"
```

---

### Task 5: Login'de "başvurunuz onay bekliyor" mesajı

Şu an `Login` yalnızca `Active` kullanıcıyı arıyor; onay bekleyen satıcı "E-posta veya şifre hatalı" görüyor — self-servis akışta kafa karıştırır.

**Files:**
- Modify: `backend/Application/Services/Common/UserService/UserManager.cs` (`Login` metodu, ~63. satır)

- [ ] **Step 1: Login sorgusunu ve kontrolü değiştir**

Mevcut kod:

```csharp
            var user = await _userRepository.GetAsync(x =>
                x.Email == requestDto.Email.ToLowerInvariant() &&
                x.UserStatusId == (short)UserStatusEnums.Active);

            if (user == null || !BCrypt.Net.BCrypt.Verify(requestDto.Password, user.Password))
            {
                response.AddErrorMessage("E-posta veya şifre hatalı.");
                return response;
            }
```

Yeni kod:

```csharp
            var user = await _userRepository.GetAsync(x =>
                x.Email == requestDto.Email.ToLowerInvariant());

            if (user == null || !BCrypt.Net.BCrypt.Verify(requestDto.Password, user.Password))
            {
                response.AddErrorMessage("E-posta veya şifre hatalı.");
                return response;
            }

            if (user.UserStatusId == (short)UserStatusEnums.WaitingForActivation)
            {
                response.AddErrorMessage("Başvurunuz onay bekliyor. Onaylandığında giriş yapabilirsiniz.");
                return response;
            }

            if (user.UserStatusId != (short)UserStatusEnums.Active)
            {
                response.AddErrorMessage("E-posta veya şifre hatalı.");
                return response;
            }
```

Güvenlik notu: "onay bekliyor" mesajı YALNIZCA şifre doğruysa döner (BCrypt.Verify'dan sonra) — e-posta varlığını dışarı sızdırmaz.

- [ ] **Step 2: Derle + elle doğrula**

```bash
cd ~/Desktop/Github/food-order/backend && dotnet build FoodOrder.Backend.sln --nologo -v q 2>&1 | tail -2
cd WebAPI && dotnet run --launch-profile http &
sleep 8
curl -s -X POST http://localhost:3762/v1/auth/login -H 'Content-Type: application/json' \
  -d '{"email":"curltest@example.com","password":"Sifre1234"}'
```

Beklenen: `"Başvurunuz onay bekliyor. Onaylandığında giriş yapabilirsiniz."` — Task 4'teki kayıt Pending durumda olduğu için. Yanlış şifreyle: `"E-posta veya şifre hatalı."`. Sunucuyu durdur: `kill %1`

- [ ] **Step 3: Commit**

```bash
git add Application/Services/Common/UserService/UserManager.cs
git commit -m "feat(backend): login — onay bekleyen başvuru için açıklayıcı mesaj"
```

---

### Task 6: Paylaşılan service paketine registerSeller

**Files:**
- Modify: `service/schema/auth.js`
- Modify: `service/services/authService.js`

- [ ] **Step 1: Zod şemasını ekle**

`service/schema/auth.js` dosyasının SONUNA (mevcut şema deseniyle aynı; dosyanın başındaki `z` import'u zaten var):

```js
export const SellerRegisterRequestSchema = z.object({
  companyType: z.number().int(),
  name: z.string().min(1),
  legalName: z.string().min(1),
  taxCode: z.string().min(10).max(11),
  taxArea: z.string().min(1),
  iban: z.string().min(26),
  isEInvoiceAvaible: z.boolean().default(false),
  identityNumber: z.string().length(11).optional(),
  ownerFirstName: z.string().min(1),
  ownerLastName: z.string().min(1),
  ownerEmail: z.string().email(),
  ownerPhone: z.string().min(10),
  password: z.string().min(8),
  cityId: z.string().uuid(),
  townId: z.string().uuid(),
  neighbourhoodId: z.string().uuid(),
  addressLine1: z.string().min(1),
  addressLine2: z.string().optional(),
});
```

- [ ] **Step 2: AuthService'e metodu ekle**

`service/services/authService.js`:

1. Import bloğuna `SellerRegisterRequestSchema,` ekle (`RegisterRequestSchema,` satırının altına).
2. `register(data)` metodunun altına ekle:

```js
  registerSeller(data) {
    const parsed = SellerRegisterRequestSchema.parse(data);
    return this.post('/seller/register', parsed);
  }
```

- [ ] **Step 3: Şema hızlı doğrulama**

```bash
cd ~/Desktop/Github/food-order && node -e "
import('./service/schema/auth.js').then(m => {
  const r = m.SellerRegisterRequestSchema.safeParse({
    companyType: 1, name: 'X', legalName: 'X Ltd', taxCode: '1234567890',
    taxArea: 'VD', iban: 'TR111111111111111111111111', isEInvoiceAvaible: false,
    ownerFirstName: 'A', ownerLastName: 'B', ownerEmail: 'a@b.com',
    ownerPhone: '5550001122', password: 'Sifre1234',
    cityId: '00000000-0000-0000-0000-000000000000',
    townId: '00000000-0000-0000-0000-000000000000',
    neighbourhoodId: '00000000-0000-0000-0000-000000000000',
    addressLine1: 'Adres'
  });
  console.log(r.success ? 'SCHEMA OK' : r.error.issues);
})"
```

Beklenen: `SCHEMA OK`

- [ ] **Step 4: Commit**

```bash
git add service/schema/auth.js service/services/authService.js
git commit -m "feat(service): registerSeller çağrısı + SellerRegisterRequestSchema"
```

---

### Task 7: front-seller kayıt sayfası + login linki

**Files:**
- Modify: `front-seller/lib/auth.js`
- Create: `front-seller/app/register/page.js`
- Modify: `front-seller/app/login/page.js`

- [ ] **Step 1: auth.js'e registerSeller ekle**

`front-seller/lib/auth.js` — `login` fonksiyonunun altına:

```js
export async function registerSeller(form) {
  const res = await sellerApi.auth.registerSeller(form);
  if (res.data?.hasFailed) {
    const msg = res.data?.messages?.map((m) => m.description).join('\n') || 'Başvuru başarısız';
    throw new Error(msg);
  }
  return res.data;
}
```

- [ ] **Step 2: Kayıt sayfasını oluştur**

`front-seller/app/register/page.js` (login sayfasıyla aynı görsel dil: gray-900 zemin, beyaz kart, emerald vurgu; 2 adım: işletme bilgileri → hesap bilgileri; başarıda "başvurunuz alındı" ekranı):

```js
'use client';

import { useState } from 'react';
import Link from 'next/link';
import { registerSeller } from '@/lib/auth';

const EMPTY_GUID = '00000000-0000-0000-0000-000000000000';

// CompanyTypeEnums: Individual=1, Company=2
const initialForm = {
  companyType: 2,
  name: '', legalName: '', taxCode: '', taxArea: '', iban: '',
  identityNumber: '',
  ownerFirstName: '', ownerLastName: '', ownerEmail: '', ownerPhone: '',
  password: '', passwordConfirm: '',
  addressLine1: '', addressLine2: '',
};

function Field({ label, children }) {
  return (
    <label className="block">
      <span className="text-sm font-medium text-gray-700">{label}</span>
      <div className="mt-1">{children}</div>
    </label>
  );
}

const inputCls =
  'w-full rounded-lg border border-gray-300 px-3 py-2 text-sm text-gray-900 focus:border-emerald-500 focus:outline-none focus:ring-1 focus:ring-emerald-500';

export default function RegisterPage() {
  const [form, setForm] = useState(initialForm);
  const [step, setStep] = useState(1);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [done, setDone] = useState(false);

  const set = (key) => (e) => setForm((f) => ({ ...f, [key]: e.target.value }));
  const isIndividual = Number(form.companyType) === 1;

  const validateStep1 = () => {
    if (!form.name || !form.legalName || !form.taxCode || !form.taxArea || !form.iban || !form.addressLine1)
      return 'Lütfen tüm zorunlu alanları doldurun.';
    if (isIndividual && form.identityNumber.length !== 11)
      return 'Şahıs işletmesi için 11 haneli TC kimlik numarası gereklidir.';
    return '';
  };

  const validateStep2 = () => {
    if (!form.ownerFirstName || !form.ownerLastName || !form.ownerEmail || !form.ownerPhone)
      return 'Lütfen tüm zorunlu alanları doldurun.';
    if (form.password.length < 8) return 'Şifre en az 8 karakter olmalıdır.';
    if (form.password !== form.passwordConfirm) return 'Şifreler eşleşmiyor.';
    return '';
  };

  const next = () => {
    const msg = validateStep1();
    if (msg) { setError(msg); return; }
    setError('');
    setStep(2);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    const msg = validateStep2();
    if (msg) { setError(msg); return; }
    setError('');
    setLoading(true);
    try {
      await registerSeller({
        companyType: Number(form.companyType),
        name: form.name,
        legalName: form.legalName,
        taxCode: form.taxCode.trim(),
        taxArea: form.taxArea,
        iban: form.iban.replace(/\s/g, '').toUpperCase(),
        isEInvoiceAvaible: false,
        ...(isIndividual ? { identityNumber: form.identityNumber.trim() } : {}),
        ownerFirstName: form.ownerFirstName,
        ownerLastName: form.ownerLastName,
        ownerEmail: form.ownerEmail.trim(),
        ownerPhone: form.ownerPhone,
        password: form.password,
        cityId: EMPTY_GUID, townId: EMPTY_GUID, neighbourhoodId: EMPTY_GUID,
        addressLine1: form.addressLine1,
        ...(form.addressLine2 ? { addressLine2: form.addressLine2 } : {}),
      });
      setDone(true);
    } catch (err) {
      setError(err.message || 'Başvuru gönderilemedi. Lütfen tekrar deneyin.');
    } finally {
      setLoading(false);
    }
  };

  if (done) {
    return (
      <div className="min-h-screen bg-gray-900 flex items-center justify-center p-4">
        <div className="w-full max-w-md bg-white rounded-2xl shadow-xl p-8 text-center">
          <div className="w-14 h-14 bg-emerald-100 rounded-full flex items-center justify-center mx-auto mb-4">
            <svg className="w-8 h-8 text-emerald-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
            </svg>
          </div>
          <h1 className="text-xl font-bold text-gray-900">Başvurunuz alındı</h1>
          <p className="text-gray-600 mt-2 text-sm">
            Başvurunuz inceleniyor. Onaylandığında e-posta adresiniz ve şifrenizle giriş yapabilirsiniz.
          </p>
          <Link href="/login" className="inline-block mt-6 text-emerald-600 font-medium hover:underline text-sm">
            Giriş sayfasına dön
          </Link>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-900 flex items-center justify-center p-4">
      <div className="w-full max-w-lg">
        <div className="text-center mb-8">
          <h1 className="text-2xl font-bold text-white">Restoran Başvurusu</h1>
          <p className="text-gray-400 mt-1 text-sm">
            Adım {step}/2 — {step === 1 ? 'İşletme bilgileri' : 'Hesap bilgileri'}
          </p>
        </div>

        <div className="bg-white rounded-2xl shadow-xl p-8">
          <form onSubmit={handleSubmit} className="space-y-4">
            {step === 1 && (
              <>
                <Field label="İşletme türü">
                  <select className={inputCls} value={form.companyType} onChange={set('companyType')}>
                    <option value={2}>Şirket (Ltd/A.Ş.)</option>
                    <option value={1}>Şahıs işletmesi</option>
                  </select>
                </Field>
                <Field label="İşletme adı (tabela adı)">
                  <input className={inputCls} value={form.name} onChange={set('name')} placeholder="Örn. Lezzet Lokantası" />
                </Field>
                <Field label="Ticari unvan">
                  <input className={inputCls} value={form.legalName} onChange={set('legalName')} placeholder="Örn. Lezzet Gıda Ltd. Şti." />
                </Field>
                <div className="grid grid-cols-2 gap-3">
                  <Field label="Vergi numarası">
                    <input className={inputCls} value={form.taxCode} onChange={set('taxCode')} inputMode="numeric" maxLength={11} />
                  </Field>
                  <Field label="Vergi dairesi">
                    <input className={inputCls} value={form.taxArea} onChange={set('taxArea')} />
                  </Field>
                </div>
                {isIndividual && (
                  <Field label="TC kimlik numarası">
                    <input className={inputCls} value={form.identityNumber} onChange={set('identityNumber')} inputMode="numeric" maxLength={11} />
                  </Field>
                )}
                <Field label="IBAN (hakediş ödemeleri için)">
                  <input className={inputCls} value={form.iban} onChange={set('iban')} placeholder="TR__ ____ ____ ____ ____ ____ __" />
                </Field>
                <Field label="İşletme adresi">
                  <input className={inputCls} value={form.addressLine1} onChange={set('addressLine1')} placeholder="Mahalle, sokak, no, ilçe/il" />
                </Field>
                <Field label="Adres satırı 2 (isteğe bağlı)">
                  <input className={inputCls} value={form.addressLine2} onChange={set('addressLine2')} />
                </Field>
              </>
            )}

            {step === 2 && (
              <>
                <div className="grid grid-cols-2 gap-3">
                  <Field label="Yetkili adı">
                    <input className={inputCls} value={form.ownerFirstName} onChange={set('ownerFirstName')} />
                  </Field>
                  <Field label="Yetkili soyadı">
                    <input className={inputCls} value={form.ownerLastName} onChange={set('ownerLastName')} />
                  </Field>
                </div>
                <Field label="E-posta (giriş için kullanılacak)">
                  <input className={inputCls} type="email" value={form.ownerEmail} onChange={set('ownerEmail')} />
                </Field>
                <Field label="Telefon">
                  <input className={inputCls} type="tel" value={form.ownerPhone} onChange={set('ownerPhone')} placeholder="5__ ___ __ __" />
                </Field>
                <Field label="Şifre (en az 8 karakter)">
                  <input className={inputCls} type="password" value={form.password} onChange={set('password')} />
                </Field>
                <Field label="Şifre (tekrar)">
                  <input className={inputCls} type="password" value={form.passwordConfirm} onChange={set('passwordConfirm')} />
                </Field>
              </>
            )}

            {error && (
              <div className="rounded-lg bg-red-50 border border-red-200 px-3 py-2 text-sm text-red-700 whitespace-pre-line">
                {error}
              </div>
            )}

            <div className="flex gap-3 pt-2">
              {step === 2 && (
                <button type="button" onClick={() => { setError(''); setStep(1); }}
                  className="flex-1 rounded-lg border border-gray-300 py-2.5 text-sm font-medium text-gray-700 hover:bg-gray-50">
                  Geri
                </button>
              )}
              {step === 1 ? (
                <button type="button" onClick={next}
                  className="flex-1 rounded-lg bg-emerald-500 py-2.5 text-sm font-semibold text-white hover:bg-emerald-600">
                  Devam et
                </button>
              ) : (
                <button type="submit" disabled={loading}
                  className="flex-1 rounded-lg bg-emerald-500 py-2.5 text-sm font-semibold text-white hover:bg-emerald-600 disabled:opacity-50">
                  {loading ? 'Gönderiliyor…' : 'Başvuruyu gönder'}
                </button>
              )}
            </div>
          </form>

          <p className="text-center text-sm text-gray-500 mt-6">
            Zaten hesabınız var mı?{' '}
            <Link href="/login" className="text-emerald-600 font-medium hover:underline">Giriş yapın</Link>
          </p>
        </div>
      </div>
    </div>
  );
}
```

- [ ] **Step 3: Login sayfasına başvuru linki ekle**

`front-seller/app/login/page.js` — beyaz kartın (`bg-white rounded-2xl ...` div'inin) İÇİNDE, formun kapanışından sonra (login formundaki son elemandan sonra, kart kapanmadan önce) ekle:

```jsx
          <p className="text-center text-sm text-gray-500 mt-6">
            Restoranınızı eklemek mi istiyorsunuz?{' '}
            <Link href="/register" className="text-emerald-600 font-medium hover:underline">Başvuru yapın</Link>
          </p>
```

Dosyanın başına `import Link from 'next/link';` ekle (yoksa).

- [ ] **Step 4: Elle doğrula (backend + seller panel ayakta)**

```bash
cd ~/Desktop/Github/food-order/backend/WebAPI && dotnet run --launch-profile http &
cd ~/Desktop/Github/food-order/front-seller && npm run dev -- -p 3000 &
sleep 12
```

Tarayıcıda `http://localhost:3000/register`:
1. Adım 1'i boş gönder → "zorunlu alanlar" hatası görünmeli.
2. Geçerli işletme bilgileri → Devam → hesap bilgileri → Gönder → "Başvurunuz alındı" ekranı.
3. `http://localhost:3000/login` → aynı e-posta/şifre ile giriş → "Başvurunuz onay bekliyor" mesajı.
4. Şahıs işletmesi seçilince TC alanının göründüğünü doğrula.

Süreçleri durdur: `kill %1 %2`

- [ ] **Step 5: Commit**

```bash
git add front-seller/lib/auth.js front-seller/app/register/page.js front-seller/app/login/page.js
git commit -m "feat(front-seller): restoran self-servis başvuru sayfası + login linki"
```

---

### Task 8: backend/CLAUDE.md güncelle (bayat abonelik içeriği)

CLAUDE.md hâlâ abonelik modelini "mevcut" diye anlatıyor; kod Mart 2026'da komisyona geçti. Yanlış rehberlik üretiyor.

**Files:**
- Modify: `backend/CLAUDE.md`

- [ ] **Step 1: Şu değişiklikleri yap**

1. Giriş paragrafındaki cümleyi DEĞİŞTİR:
   - Eski: `Platformun gelir modeli abonelik tabanlıdır — restoran başına aylık ücret alınır, sipariş başına komisyon alınmaz.`
   - Yeni: `Platformun gelir modeli komisyon tabanlıdır — sipariş başına %X komisyon + sabit ücret alınır (platform varsayılanı PlatformCommissionSchedule, restoran bazlı özel oran RestaurantCommission). Aylık abonelik yoktur.`

2. `### Seller` modül listesinde `SubscriptionPlan` ve `Subscription` satırlarını SİL; yerine:

```markdown
- `RestaurantCommission` — restoran bazlı komisyon oranı geçmişi (EffectiveFrom/To)
- `SettlementPeriod` → `SettlementItem` — günlük hakediş dönemleri ve kalemleri
```

3. `### Common` listesine ekle: `- PlatformCommissionSchedule — platform geneli varsayılan komisyon tarifesi`

4. `## Abonelik İş Mantığı` bölümünü tamamen DEĞİŞTİR:

```markdown
## Komisyon İş Mantığı

1. Admin `PlatformCommissionSchedule` ile platform varsayılan oranını yönetir (tarih bazlı planlama)
2. Restoran bazlı özel oran `RestaurantCommission` ile tanımlanır; yoksa platform varsayılanı uygulanır
3. Sipariş tutarı üzerinden komisyon hesaplanır; günlük hakediş `SettlementPeriod`/`SettlementItem` ile satıcıya dağıtılır
4. Satıcı onayında (`ConfirmSeller`) İyzico alt üye işyeri (SubMerchant) kaydı yapılır — BLOCKING
```

5. API endpoint tablosundaki 6 `subscription` satırını SİL; yerine:

```markdown
| POST | /v1/seller/register | - | Restoran self-servis başvuru (anonim) |
| POST | /v1/admin/seller/confirm | Admin | Başvuru onayı → İyzico alt üye kaydı |
```

6. `## Geliştirme Notları`ndaki son maddeyi (`Abonelik kontrolünü atlamak için...`) SİL.

7. `## Önemli Dosyalar` bölümündeki `SubscriptionService` ve `SubscriptionPlan/Subscription` entity satırlarını SİL; yerine:

```markdown
Application/Services/Seller/CommissionService/             — komisyon çözümleme iş mantığı
Domain/Entities/Seller/RestaurantCommission.cs              — restoran bazlı oran geçmişi
Domain/Entities/Common/PlatformCommissionSchedule.cs        — platform varsayılan tarifesi
```

- [ ] **Step 2: Commit**

```bash
git add backend/CLAUDE.md
git commit -m "docs(backend): CLAUDE.md — bayat abonelik içeriği komisyon modeline güncellendi"
```

---

### Task 9: Uçtan uca smoke testi

**Files:** — (yalnızca doğrulama)

- [ ] **Step 1: Tüm testler + tam build**

```bash
cd ~/Desktop/Github/food-order/backend
dotnet build FoodOrder.Backend.sln --nologo -v q 2>&1 | tail -2
dotnet test Application.Tests --nologo 2>&1 | tail -2
```

Beklenen: `0 Hata`, tüm testler PASS.

- [ ] **Step 2: Akışı uçtan uca doğrula**

Backend'i başlat, sırasıyla:

1. **Kayıt:** Task 4'teki curl (farklı taxCode/e-posta ile) → `hasFailed:false`
2. **Pending login:** Task 5'teki curl → "Başvurunuz onay bekliyor"
3. **Admin onayı:** Admin panelden (http://localhost:3001 → Sellers) başvuruyu onayla — İyzico sandbox yapılandırılmışsa SubMerchant kaydı başarılı olmalı; sandbox yoksa bu adımın İyzico hatası vermesi BEKLENEN durumdur, not düş.
4. **Onay sonrası login:** Aynı kullanıcıyla `POST /v1/auth/login` → token döner.
5. **Seller panel:** http://localhost:3000 → giriş → dashboard açılır; restoran ekleme sayfası (`/restaurants`) erişilebilir.

- [ ] **Step 3: Push (kullanıcı onayıyla)**

```bash
git log --oneline origin/main..HEAD
# Kullanıcıya commit listesini göster, onay alınca:
git push origin main
```

---

## Kapsam Dışı (bilinçli)

- İl/ilçe (City/Town) lookup sistemi — entity yok, admin formuyla aynı boş-Guid kısayolu kullanıldı
- E-posta gönderimi (aktivasyon/bilgilendirme) — mail altyapısı yok; ayrı iş paketi
- Menü yükleme sihirbazı / "30 dakikada yayında" onboarding turu — kayıt sonrası mevcut panel sayfaları kullanılır; ayrı iş paketi
- TC kimlik checksum algoritması — yalnızca format kontrolü yapılır; gerçek doğrulama İyzico/KPS tarafında
