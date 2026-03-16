# Faz 2 Implementation Plan — Kritik Altyapı

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development or superpowers:executing-plans. Steps use checkbox (`- [ ]`) syntax.

**Goal:** Add refund, push notifications, real-time updates, refresh tokens, password reset, background jobs, seller payout, and auto-renewal.

**Architecture:** Same as Faz 1. New additions: SignalR hubs, Hangfire background jobs, OneSignal REST API integration.

**Tech Stack:** .NET 8, SignalR, Hangfire, OneSignal REST API, react-native-onesignal, @microsoft/signalr

**Spec:** `docs/superpowers/specs/2026-03-16-faz2-infrastructure-design.md`

---

## Chunk 1: Backend Foundation — Entities, Repos, Packages

### Task 1: Add NuGet Packages

**Files:**
- Modify: `backend/WebAPI/WebAPI.csproj`
- Modify: `backend/Infrastructure/Infrastructure.csproj`

- [ ] **Step 1: Add Hangfire packages to WebAPI.csproj**

```xml
<PackageReference Include="Hangfire.Core" Version="1.8.*" />
<PackageReference Include="Hangfire.AspNetCore" Version="1.8.*" />
<PackageReference Include="Hangfire.MySqlStorage" Version="2.0.*" />
```

- [ ] **Step 2: SignalR is built-in to ASP.NET Core — no extra package needed**

- [ ] **Step 3: Commit**

```bash
git add backend/
git commit -m "feat: add Hangfire NuGet packages"
```

---

### Task 2: Create New Entities

**Files:**
- Create: `backend/Domain/Entities/Common/RefreshToken.cs`
- Create: `backend/Domain/Entities/Common/PasswordResetToken.cs`

- [ ] **Step 1: RefreshToken entity**

```csharp
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Common;

public class RefreshToken : Entity<Guid>
{
    public Guid UserId { get; set; }
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? ReplacedByToken { get; set; }
    public virtual User User { get; set; }
}
```

- [ ] **Step 2: PasswordResetToken entity**

```csharp
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities.Common;

public class PasswordResetToken : Entity<Guid>
{
    public Guid UserId { get; set; }
    public string Code { get; set; }
    public short Method { get; set; } // Email=1, Sms=2
    public DateTime ExpiresAt { get; set; }
    public DateTime? UsedAt { get; set; }
    public virtual User User { get; set; }
}
```

- [ ] **Step 3: Commit**

```bash
git add backend/Domain/Entities/Common/
git commit -m "feat: add RefreshToken and PasswordResetToken entities"
```

---

### Task 3: Modify Existing Entities

**Files:**
- Modify: `backend/Domain/Entities/Buyer/Payment.cs`
- Modify: `backend/Domain/Entities/Seller/Subscription.cs`
- Modify: `backend/Domain/Entities/Seller/SubscriptionPlan.cs`

- [ ] **Step 1: Add refund fields to Payment**

```csharp
public DateTime? RefundedAt { get; set; }
public string? RefundTransactionId { get; set; }
public string? RefundReason { get; set; }
```

- [ ] **Step 2: Add auto-renewal fields to Subscription**

```csharp
public bool AutoRenew { get; set; } = true;
public int RenewalAttempts { get; set; } = 0;
public DateTime? LastRenewalAttemptAt { get; set; }
```

- [ ] **Step 3: Add CommissionRate to SubscriptionPlan**

```csharp
public decimal CommissionRate { get; set; } = 0.10m;
```

- [ ] **Step 4: Commit**

```bash
git add backend/Domain/Entities/
git commit -m "feat: add refund, auto-renewal, commission fields to entities"
```

---

### Task 4: Entity Configurations, DbSets, Repositories

- [ ] **Step 1: Create RefreshTokenConfiguration and PasswordResetTokenConfiguration**
- [ ] **Step 2: Register DbSets in BaseDbContext**
- [ ] **Step 3: Create repository interfaces and implementations**
- [ ] **Step 4: Register in IUnitOfWork and PersistenceServiceRegistration**
- [ ] **Step 5: Commit**

```bash
git add backend/Persistence/
git commit -m "feat: add configs, repos for RefreshToken and PasswordResetToken"
```

---

### Task 5: Add Enums

**Files:**
- Modify: `backend/Base/Enums/AuthorizationServiceEnums.cs`

- [ ] **Step 1: Add PasswordResetMethodEnums**

```csharp
public enum PasswordResetMethodEnums : short
{
    Email = 1,
    Sms = 2
}
```

- [ ] **Step 2: Commit**

```bash
git add backend/Base/
git commit -m "feat: add PasswordResetMethod enum"
```

---

### Task 6: Generate Migration

- [ ] **Step 1: Generate EF Core migration**

```bash
cd /Users/onuryorulmaz/Desktop/Github/food-order/backend
dotnet ef migrations add AddFaz2Infrastructure --project Persistence --startup-project WebAPI
```

- [ ] **Step 2: Commit**

---

## Chunk 2: Backend Services — Auth (Refresh Token + Password Reset)

### Task 7: Implement Refresh Token Service

**Files:**
- Create: `backend/Application/Services/Common/AuthService/IAuthTokenService.cs`
- Create: `backend/Application/Services/Common/AuthService/AuthTokenManager.cs`

- [ ] **Step 1: Create interface**

```csharp
public interface IAuthTokenService
{
    Task<TokenPairDto> GenerateTokenPairAsync(User user);
    Task<TokenPairDto> RefreshTokenAsync(string refreshToken);
    Task RevokeRefreshTokenAsync(string refreshToken);
    Task RevokeAllUserTokensAsync(Guid userId);
}
```

- [ ] **Step 2: Implement AuthTokenManager**

Key logic:
- GenerateTokenPair: create access token (existing logic, 15min expiry) + random refresh token (30 days), save to DB
- RefreshToken: validate refresh token → not expired, not revoked → generate new pair → revoke old (rotation)
- RevokeRefreshToken: set RevokedAt = now

- [ ] **Step 3: Update UserManager login to return token pair**

Modify existing login method to use AuthTokenService instead of generating token directly.

- [ ] **Step 4: Add refresh and logout endpoints**

```csharp
[Route("v1/auth")]
public class AuthController : BaseController
{
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ServiceObjectResult<TokenPairDto>> Refresh([FromBody] RefreshRequestDto request)
        => await _authTokenService.RefreshTokenAsync(request.RefreshToken);

    [HttpPost("logout")]
    [AuthorizeAPIRequest(true, false)]
    public async Task<ServiceObjectResult<bool>> Logout([FromBody] LogoutRequestDto request)
    {
        await _authTokenService.RevokeRefreshTokenAsync(request.RefreshToken);
        return new ServiceObjectResult<bool> { /* success */ };
    }
}
```

- [ ] **Step 5: Commit**

```bash
git add backend/
git commit -m "feat: implement refresh token with rotation"
```

---

### Task 8: Implement Password Reset Service

**Files:**
- Create: `backend/Application/Services/Common/PasswordResetService/IPasswordResetService.cs`
- Create: `backend/Application/Services/Common/PasswordResetService/PasswordResetManager.cs`

- [ ] **Step 1: Create interface**

```csharp
public interface IPasswordResetService
{
    Task<ServiceObjectResult<bool>> SendResetCodeAsync(string emailOrPhone);
    Task<ServiceObjectResult<bool>> VerifyCodeAsync(string emailOrPhone, string code);
    Task<ServiceObjectResult<bool>> ResetPasswordAsync(string emailOrPhone, string code, string newPassword);
}
```

- [ ] **Step 2: Implement PasswordResetManager**

Key logic:
- SendResetCode: find user by email or phone → generate 6-digit code → save to DB (expires in 10min) → send via email (SMTP) or SMS
- VerifyCode: check code exists, not expired, not used
- ResetPassword: verify code → hash new password with BCrypt → update user → mark code as used
- Rate limit: check count of recent codes per email/phone (max 3 per 10 min)
- Brute force: check failed attempts (max 5 wrong codes → 30 min lockout)

- [ ] **Step 3: Add endpoints**

```csharp
[HttpPost("forgot-password")]
[AllowAnonymous]
public async Task<ServiceObjectResult<bool>> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
    => await _passwordResetService.SendResetCodeAsync(request.EmailOrPhone);

[HttpPost("verify-reset-code")]
[AllowAnonymous]
public async Task<ServiceObjectResult<bool>> VerifyCode([FromBody] VerifyResetCodeRequestDto request)
    => await _passwordResetService.VerifyCodeAsync(request.EmailOrPhone, request.Code);

[HttpPost("reset-password")]
[AllowAnonymous]
public async Task<ServiceObjectResult<bool>> ResetPassword([FromBody] ResetPasswordRequestDto request)
    => await _passwordResetService.ResetPasswordAsync(request.EmailOrPhone, request.Code, request.NewPassword);
```

- [ ] **Step 4: Add dedicated rate limit for password reset**

```csharp
options.AddFixedWindowLimiter("password-reset", limiterOptions =>
{
    limiterOptions.PermitLimit = 3;
    limiterOptions.Window = TimeSpan.FromMinutes(10);
});
```

- [ ] **Step 5: Commit**

```bash
git add backend/
git commit -m "feat: implement password reset with email + SMS, rate limiting"
```

---

## Chunk 3: Backend Services — Notifications + SignalR

### Task 9: Implement OneSignal Notification Service

**Files:**
- Create: `backend/Infrastructure/Adapters/OneSignalAdapter/INotificationService.cs`
- Create: `backend/Infrastructure/Adapters/OneSignalAdapter/OneSignalNotificationService.cs`

- [ ] **Step 1: Create interface**

```csharp
public interface INotificationService
{
    Task SendToUserAsync(Guid userId, string title, string message, Dictionary<string, string>? data = null);
    Task SendToUsersAsync(List<Guid> userIds, string title, string message, Dictionary<string, string>? data = null);
}
```

- [ ] **Step 2: Implement OneSignal REST API integration**

Uses HttpClient to call OneSignal API:
```
POST https://onesignal.com/api/v1/notifications
Headers: Authorization: Basic {REST_API_KEY}
Body: { app_id, include_external_user_ids: [userId], headings: {en: title}, contents: {en: message}, data }
```

- [ ] **Step 3: Add device registration endpoint**

```csharp
[HttpPost("register-device")]
public async Task<ServiceObjectResult<bool>> RegisterDevice([FromBody] RegisterDeviceDto request)
{
    // Save OneSignal playerId to UserExternalInfo
    // Provider = "onesignal", Key = "playerId", Value = request.PlayerId
}
```

- [ ] **Step 4: Commit**

```bash
git add backend/
git commit -m "feat: implement OneSignal notification service"
```

---

### Task 10: Add SignalR Hubs

**Files:**
- Create: `backend/WebAPI/Hubs/OrderHub.cs`
- Create: `backend/WebAPI/Hubs/RestaurantHub.cs`

- [ ] **Step 1: Create OrderHub**

```csharp
using Microsoft.AspNetCore.SignalR;

namespace WebAPI.Hubs;

public class OrderHub : Hub
{
    public async Task JoinOrderGroup(string orderId)
        => await Groups.AddToGroupAsync(Context.ConnectionId, $"order-{orderId}");

    public async Task LeaveOrderGroup(string orderId)
        => await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"order-{orderId}");
}
```

- [ ] **Step 2: Create RestaurantHub**

```csharp
public class RestaurantHub : Hub
{
    public async Task JoinRestaurantGroup(string restaurantId)
        => await Groups.AddToGroupAsync(Context.ConnectionId, $"restaurant-{restaurantId}");

    public async Task LeaveRestaurantGroup(string restaurantId)
        => await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"restaurant-{restaurantId}");
}
```

- [ ] **Step 3: Register in Program.cs**

```csharp
builder.Services.AddSignalR();
// ...
app.MapHub<OrderHub>("/hubs/order");
app.MapHub<RestaurantHub>("/hubs/restaurant");
```

- [ ] **Step 4: Commit**

```bash
git add backend/WebAPI/
git commit -m "feat: add SignalR OrderHub and RestaurantHub"
```

---

### Task 11: Integrate Notifications into Order Flow

**Files:**
- Modify: `backend/Application/Services/Buyer/OrderService/OrderManager.cs`
- Modify: `backend/Application/Services/Courier/CourierCompanyService/CourierCompanyManager.cs`

- [ ] **Step 1: Inject INotificationService and IHubContext into OrderManager**

- [ ] **Step 2: Add notification calls on order status changes**

After each status change:
```csharp
// Push notification
await _notificationService.SendToUserAsync(order.UserId, "Sipariş Güncelleme", "Siparişiniz onaylandı");

// SignalR real-time
await _orderHubContext.Clients.Group($"order-{order.Id}").SendAsync("OrderStatusChanged",
    order.Id, order.StatusId, DateTime.UtcNow);
```

- [ ] **Step 3: Add notification on new order for restaurant**

```csharp
await _restaurantHubContext.Clients.Group($"restaurant-{order.RestaurantId}").SendAsync("NewOrder", orderDto);
await _notificationService.SendToUserAsync(sellerUserId, "Yeni Sipariş", $"Sipariş #{orderNo}");
```

- [ ] **Step 4: Add courier location SignalR push** (replace polling)

In CourierLocationManager, after saving location:
```csharp
// Find active orders for this courier
// For each order, push location to OrderHub group
await _orderHubContext.Clients.Group($"order-{orderId}").SendAsync("CourierLocationUpdated",
    orderId, lat, lng, DateTime.UtcNow);
```

- [ ] **Step 5: Commit**

```bash
git add backend/Application/
git commit -m "feat: integrate push notifications and SignalR into order flow"
```

---

## Chunk 4: Backend Services — Refund, Payout, Hangfire

### Task 12: Implement Refund Service

**Files:**
- Modify: `backend/Infrastructure/Adapters/IyzicoServiceAdapter/IyzicoServiceAdapter.cs`
- Modify: `backend/Application/Services/Buyer/PaymentService/PaymentManager.cs`

- [ ] **Step 1: Add RefundPayment to IyzicoServiceAdapter**

```csharp
public async Task<RefundResult> RefundPaymentAsync(string paymentTransactionId, decimal amount)
{
    // iyzipay Refund API
    var request = new CreateRefundRequest
    {
        PaymentTransactionId = paymentTransactionId,
        Price = amount.ToString("F2"),
        Ip = "85.34.78.112",
        ConversationId = Guid.NewGuid().ToString()
    };
    var refund = Refund.Create(request, _options);
    return new RefundResult { Success = refund.Status == "success", TransactionId = refund.PaymentTransactionId };
}
```

- [ ] **Step 2: Add RefundOrderAsync to PaymentManager**

```csharp
public async Task<ServiceObjectResult<bool>> RefundOrderAsync(Guid orderId, string? reason = null)
{
    // Find payment for order
    // Validate: StatusId == Completed
    // Call iyzico refund
    // Update: Payment.StatusId = Refunded, RefundedAt, RefundTransactionId, RefundReason
    // Send notification to customer
}
```

- [ ] **Step 3: Integrate auto-refund on order cancellation/rejection**

In OrderManager, when order is cancelled or rejected:
```csharp
if (payment != null && payment.StatusId == (short)PaymentStatusEnums.Completed)
{
    await _paymentManager.RefundOrderAsync(order.Id, "Sipariş iptal/red");
}
```

- [ ] **Step 4: Add admin refund endpoint**

```csharp
[HttpPost("orders/{orderId}/refund")]
[AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
public async Task<ServiceObjectResult<bool>> RefundOrder(Guid orderId, [FromBody] RefundRequestDto request)
    => await _paymentService.RefundOrderAsync(orderId, request.Reason);
```

- [ ] **Step 5: Commit**

```bash
git add backend/
git commit -m "feat: implement refund via iyzico with auto-refund on cancellation"
```

---

### Task 13: Implement Payout/Commission Logic

**Files:**
- Modify: `backend/Application/Services/Buyer/PaymentService/PaymentManager.cs`

- [ ] **Step 1: Calculate commission on payment completion**

In HandlePaymentCallback (when payment succeeds):
```csharp
var plan = await GetSubscriptionPlanForSeller(payment.SellerId);
var commissionRate = plan?.CommissionRate ?? 0.10m;
payment.CommissionAmount = payment.Amount * commissionRate;
payment.SellerPayoutAmount = payment.Amount - payment.CommissionAmount;
```

- [ ] **Step 2: Create admin finance endpoints**

```csharp
[HttpGet("finance/summary")]
// Returns: total revenue, total commission, total payout, order count

[HttpGet("finance/sellers/{sellerId}")]
// Returns: seller's payment history with payout amounts

[HttpPut("settings/commission-rate")]
// Updates SubscriptionPlan.CommissionRate
```

- [ ] **Step 3: Create seller finance endpoints**

```csharp
[HttpGet("finance/summary")]
// Returns: seller's total revenue, payout, commission

[HttpGet("finance/payments")]
// Returns: paginated payment list with payout amounts
```

- [ ] **Step 4: Commit**

```bash
git add backend/
git commit -m "feat: implement commission calculation and finance endpoints"
```

---

### Task 14: Setup Hangfire and Background Jobs

**Files:**
- Modify: `backend/WebAPI/Program.cs`
- Create: `backend/Application/Services/Common/BackgroundJobs/SubscriptionJobService.cs`
- Create: `backend/Application/Services/Common/BackgroundJobs/CleanupJobService.cs`

- [ ] **Step 1: Configure Hangfire in Program.cs**

```csharp
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseStorage(new MySqlStorage(connectionString, new MySqlStorageOptions())));

builder.Services.AddHangfireServer();
// ...
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAdminAuthFilter() }
});

// Register recurring jobs
RecurringJob.AddOrUpdate<ISubscriptionJobService>("check-expired", s => s.CheckExpiredSubscriptions(), Cron.Hourly);
RecurringJob.AddOrUpdate<ISubscriptionJobService>("expiry-reminder", s => s.SendExpiryReminders(), Cron.Daily);
RecurringJob.AddOrUpdate<ISubscriptionJobService>("auto-renew", s => s.AutoRenewSubscriptions(), Cron.Daily);
RecurringJob.AddOrUpdate<ISubscriptionJobService>("usage-warnings", s => s.CheckUsageWarnings(), Cron.Hourly);
RecurringJob.AddOrUpdate<ICleanupJobService>("cleanup-reset-tokens", s => s.CleanupExpiredResetTokens(), Cron.Daily);
RecurringJob.AddOrUpdate<ICleanupJobService>("cleanup-refresh-tokens", s => s.CleanupExpiredRefreshTokens(), Cron.Weekly);
```

- [ ] **Step 2: Implement SubscriptionJobService**

```csharp
public class SubscriptionJobService : ISubscriptionJobService
{
    // CheckExpiredSubscriptions: move existing logic from SubscriptionManager endpoint
    // SendExpiryReminders: find subs expiring in 3 days → send notification
    // AutoRenewSubscriptions: find subs expiring tomorrow with AutoRenew=true
    //   → attempt stored card payment → create new subscription or increment retry
    // CheckUsageWarnings: find subs at 80%+ usage → send notification (once per day)
}
```

- [ ] **Step 3: Implement CleanupJobService**

```csharp
public class CleanupJobService : ICleanupJobService
{
    // CleanupExpiredResetTokens: DELETE WHERE ExpiresAt < NOW() AND UsedAt IS NULL
    // CleanupExpiredRefreshTokens: DELETE WHERE ExpiresAt < NOW() AND RevokedAt IS NOT NULL
}
```

- [ ] **Step 4: Implement auto-renewal with stored card payment**

```csharp
// In AutoRenewSubscriptions:
// 1. Find subscriptions expiring within 24h with AutoRenew=true
// 2. For each: get seller's stored card (UserExternalInfo, provider=iyzico)
// 3. If no card: send notification, skip
// 4. Attempt payment via iyzico stored card API
// 5. Success: create new Subscription, reset RenewalAttempts
// 6. Failure: increment RenewalAttempts, send notification
// 7. If RenewalAttempts >= 3: expire subscription, deactivate restaurant
```

- [ ] **Step 5: Register services in DI**

- [ ] **Step 6: Commit**

```bash
git add backend/
git commit -m "feat: setup Hangfire with subscription, renewal, and cleanup jobs"
```

---

## Chunk 5: Frontend — Auth Updates (All Apps)

### Task 15: Update Auth Flow for Refresh Token (All Apps)

**Files:**
- Modify: `front-app/src/api/client.js`
- Modify: `front-app/src/context/AuthContext.js`
- Modify: `front-courier/src/api/client.js`
- Modify: `front-seller/lib/api.js`
- Modify: `front-admin/lib/api.js`

- [ ] **Step 1: Update mobile API clients with refresh interceptor**

```javascript
// In response interceptor:
apiClient.interceptors.response.use(
  response => response,
  async error => {
    if (error.response?.status === 401 && !error.config._retry) {
      error.config._retry = true;
      const refreshToken = await AsyncStorage.getItem('refresh_token');
      if (refreshToken) {
        try {
          const res = await axios.post(`${BASE_URL}/v1/auth/refresh`, { refreshToken });
          const { accessToken, refreshToken: newRefresh } = res.data.data;
          await AsyncStorage.setItem('auth_token', accessToken);
          await AsyncStorage.setItem('refresh_token', newRefresh);
          error.config.headers.Authorization = `Bearer ${accessToken}`;
          return apiClient(error.config);
        } catch {
          await AsyncStorage.multiRemove(['auth_token', 'refresh_token', 'user_info']);
          // Navigate to login
        }
      }
    }
    return Promise.reject(error);
  }
);
```

- [ ] **Step 2: Update AuthContext to store refresh token on login**

- [ ] **Step 3: Update web API clients (front-seller, front-admin) with same pattern**

- [ ] **Step 4: Commit**

```bash
git add front-app/ front-courier/ front-seller/ front-admin/
git commit -m "feat: add refresh token interceptor to all API clients"
```

---

### Task 16: Add Password Reset Screens

**Files:**
- Create: `front-app/src/screens/Auth/ForgotPasswordScreen.js`
- Create: `front-app/src/screens/Auth/VerifyCodeScreen.js`
- Create: `front-app/src/screens/Auth/ResetPasswordScreen.js`
- Create: `front-courier/src/screens/Auth/ForgotPasswordScreen.js`
- Create: `front-courier/src/screens/Auth/VerifyCodeScreen.js`
- Create: `front-courier/src/screens/Auth/ResetPasswordScreen.js`
- Create: `front-seller/app/forgot-password/page.js`
- Create: `front-admin/app/forgot-password/page.js`

- [ ] **Step 1: Mobile forgot password flow (3 screens)**

ForgotPasswordScreen: email/phone input → call API → navigate to VerifyCode
VerifyCodeScreen: 6-digit input → verify → navigate to ResetPassword
ResetPasswordScreen: new password + confirm → reset → navigate to Login

- [ ] **Step 2: Web forgot password flow (single page with steps)**

Step 1: email input → Step 2: code input → Step 3: new password

- [ ] **Step 3: Add "Şifremi Unuttum" link to all login pages**

- [ ] **Step 4: Commit**

```bash
git add front-app/ front-courier/ front-seller/ front-admin/
git commit -m "feat: add forgot password flow to all apps"
```

---

## Chunk 6: Frontend — Notifications + SignalR + Finance

### Task 17: OneSignal Mobile Integration

**Files:**
- Modify: `front-app/package.json`
- Modify: `front-courier/package.json`
- Modify: `front-app/src/context/AuthContext.js`
- Modify: `front-courier/src/context/AuthContext.js`

- [ ] **Step 1: Install react-native-onesignal**

```bash
cd front-app && npm install react-native-onesignal
cd ../front-courier && npm install react-native-onesignal
```

- [ ] **Step 2: Initialize OneSignal in App.js / entry point**

```javascript
import OneSignal from 'react-native-onesignal';

OneSignal.initialize('YOUR_ONESIGNAL_APP_ID');
OneSignal.Notifications.requestPermission(true);
```

- [ ] **Step 3: Set external user ID on login, remove on logout**

```javascript
// On login success:
OneSignal.login(userId);
await registerDevice(playerId); // call backend API

// On logout:
OneSignal.logout();
```

- [ ] **Step 4: Handle notification tap → navigate to relevant screen**

```javascript
OneSignal.Notifications.addEventListener('click', (event) => {
  const data = event.notification.additionalData;
  if (data?.orderId) {
    navigation.navigate('OrderDetail', { orderId: data.orderId });
  }
});
```

- [ ] **Step 5: Commit**

```bash
git add front-app/ front-courier/
git commit -m "feat: integrate OneSignal push notifications in mobile apps"
```

---

### Task 18: SignalR Frontend Integration

**Files:**
- Modify: `front-seller/package.json`
- Create: `front-seller/lib/signalr.js`
- Modify: `front-seller/app/orders/page.js`
- Modify: `front-app/src/screens/Orders/OrderDetailScreen.js`

- [ ] **Step 1: Install @microsoft/signalr in web apps**

```bash
cd front-seller && npm install @microsoft/signalr
```

- [ ] **Step 2: Create SignalR connection helper for seller**

```javascript
import { HubConnectionBuilder } from '@microsoft/signalr';

export const createRestaurantConnection = (restaurantId, token) => {
  const connection = new HubConnectionBuilder()
    .withUrl(`${API_BASE}/hubs/restaurant`, { accessTokenFactory: () => token })
    .withAutomaticReconnect()
    .build();

  connection.start().then(() => {
    connection.invoke('JoinRestaurantGroup', restaurantId);
  });

  return connection;
};
```

- [ ] **Step 3: Update seller orders page — listen for NewOrder event**

Replace 30s polling with SignalR:
```javascript
connection.on('NewOrder', (orderDto) => {
  // Add to orders list, play sound, show toast
  mutate(); // SWR revalidate
});
```

- [ ] **Step 4: Update customer OrderDetailScreen — listen for status + courier location**

```javascript
// Replace polling with SignalR
connection.on('OrderStatusChanged', (orderId, newStatus) => { ... });
connection.on('CourierLocationUpdated', (orderId, lat, lng) => { ... });
```

- [ ] **Step 5: Commit**

```bash
git add front-seller/ front-app/
git commit -m "feat: integrate SignalR for real-time order updates"
```

---

### Task 19: Admin Finance Dashboard

**Files:**
- Create: `front-admin/app/finance/page.js`
- Modify: `front-admin/lib/api.js`
- Modify: `front-admin/components/layout/Sidebar.js`

- [ ] **Step 1: Add finance API functions**

```javascript
export const getFinanceSummary = () => api.get('/v1/admin/finance/summary').then(r => r.data);
export const getSellerFinance = (sellerId) => api.get(`/v1/admin/finance/sellers/${sellerId}`).then(r => r.data);
export const updateCommissionRate = (rate) => api.put('/v1/admin/settings/commission-rate', { rate }).then(r => r.data);
```

- [ ] **Step 2: Create finance page**

Cards: Toplam Gelir, Toplam Komisyon, Toplam Payout, Sipariş Sayısı
Seller payment table: satıcı adı, toplam gelir, komisyon, payout
Commission rate setting

- [ ] **Step 3: Add sidebar link**

- [ ] **Step 4: Commit**

```bash
git add front-admin/
git commit -m "feat: add admin finance dashboard with commission settings"
```

---

### Task 20: Seller Finance & Auto-renewal UI

**Files:**
- Create: `front-seller/app/finance/page.js`
- Modify: `front-seller/app/subscription/page.js` (or relevant)
- Modify: `front-seller/lib/api.js`

- [ ] **Step 1: Add seller finance API**

```javascript
export const getSellerFinanceSummary = () => api.get('/v1/seller/finance/summary').then(r => r.data);
export const getSellerPayments = (page) => api.get(`/v1/seller/finance/payments?page=${page}`).then(r => r.data);
export const toggleAutoRenew = (restaurantId, enabled) =>
  api.put('/v1/subscription/auto-renew', { restaurantId, enabled }).then(r => r.data);
```

- [ ] **Step 2: Create seller finance page** (gelir özeti, ödeme listesi)

- [ ] **Step 3: Add auto-renewal toggle to subscription page**

Switch component: "Otomatik Yenileme" on/off
Warning if no saved card

- [ ] **Step 4: Commit**

```bash
git add front-seller/
git commit -m "feat: add seller finance page and auto-renewal toggle"
```

---

### Task 21: Final Build Verification

- [ ] **Step 1: Backend build**
- [ ] **Step 2: Front-admin build**
- [ ] **Step 3: Front-seller build**
- [ ] **Step 4: Front-courier bundle check**
- [ ] **Step 5: Front-app bundle check**
