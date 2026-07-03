using Application;
using Application.Services.Common.BackgroundJobs;
using Application.Services.Common.NotificationService;
using Base.Constant;
using Hangfire;
using Hangfire.MySql;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using NArchitecture.Core.CrossCuttingConcerns.Exception.WebApi.Extensions;
using NArchitecture.Core.Security.Encryption;
using Persistence;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Threading.RateLimiting;
using WebAPI.Helpers;
using WebAPI.Hubs;
using WebAPI.Services;
using TokenOptions = NArchitecture.Core.Security.JWT.TokenOptions;

// Ana dizindeki tek .env dosyasından yükle
var rootEnvPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".env");
if (File.Exists(rootEnvPath))
    DotNetEnv.Env.Load(rootEnvPath);

var builder = WebApplication.CreateBuilder(args);

// .env değişkenlerini .NET config section'larına map'le
string? env(string key) => Environment.GetEnvironmentVariable(key);
builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    ["ConnectionStrings:FoodOrderApp"] = env("LOCAL_DB_CONNECTION_STRING") ?? env("DB_CONNECTION_STRING"),
    ["TokenOptions:SecurityKey"] = env("TOKEN_SECURITY_KEY"),
    ["AwsS3:Domain"] = env("R2_DOMAIN"),
    ["AwsS3:Endpoint"] = env("R2_ENDPOINT"),
    ["AwsS3:BucketName"] = env("R2_BUCKET"),
    ["AwsS3:AccessKey"] = env("R2_ACCESS_KEY"),
    ["AwsS3:SecretKey"] = env("R2_SECRET_KEY"),
    ["AwsS3:Token"] = env("R2_TOKEN"),
    ["Redis:Host"] = env("REDIS_HOST") ?? "127.0.0.1",
    ["Redis:Port"] = env("REDIS_PORT") ?? "6379",
    ["Redis:Password"] = env("REDIS_PASSWORD") ?? "",
    ["Redis:Ssl"] = "false",
    ["MailSettings:Server"] = env("SMTP_SERVER"),
    ["MailSettings:Port"] = env("SMTP_PORT") ?? "587",
    ["MailSettings:UserName"] = env("SMTP_USER"),
    ["MailSettings:Password"] = env("SMTP_PASSWORD"),
    ["MailSettings:SenderEmail"] = env("SMTP_FROM"),
    ["AiSupport:ApiKey"] = env("AI_SUPPORT_API_KEY"),
    ["AiSupport:Model"] = env("AI_SUPPORT_MODEL"),
});
builder.Configuration.AddEnvironmentVariables();

Global.Configuration = builder.Configuration;
var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<TokenOptions>();
System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

builder.Services.AddControllers();
builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSignalR();
builder.Services.AddScoped<IRealtimeNotifier, SignalRRealtimeNotifier>();

// Hangfire
var connectionString = builder.Configuration.GetConnectionString("FoodOrderApp");
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseStorage(new MySqlStorage(connectionString, new MySqlStorageOptions())));
builder.Services.AddHangfireServer();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = tokenOptions!.Issuer,
            ValidAudience = tokenOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = SecurityKeyHelper.CreateSecurityKey(tokenOptions.SecurityKey)
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddDistributedMemoryCache();

// Rate limiting - API koruması.
// Limiter'lar istemci IP'sine göre partition'lanır — böylece tek bir istemci global kovayı
// doldurup diğer tüm kullanıcıları (özellikle login/şifre sıfırlama) DoS edemez.
// (X-Forwarded-For spoof edilebildiği için güvenilir kaynak olarak RemoteIpAddress kullanılır.)
builder.Services.AddRateLimiter(options =>
{
    static string ClientKey(HttpContext ctx) =>
        ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown";

    options.AddPolicy("fixed", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(ClientKey(httpContext), _ =>
            new FixedWindowRateLimiterOptions
            {
                PermitLimit = 60,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 5
            }));

    options.AddPolicy("auth", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(ClientKey(httpContext), _ =>
            new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));

    options.AddPolicy("password-reset", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(ClientKey(httpContext), _ =>
            new FixedWindowRateLimiterOptions
            {
                PermitLimit = 3,
                Window = TimeSpan.FromMinutes(10),
                QueueLimit = 0
            }));

    options.RejectionStatusCode = 429;
});

builder.Services.AddEndpointsApiExplorer();

// Swagger with Bearer Auth support
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "FoodOrder API", Version = "v1" });

    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "Custom",
        In = ParameterLocation.Header,
        Description = "Token'ı 'Bearer {token}' formatında girin."
    });

    opt.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer"),
            new List<string>()
        }
    });
});

var webApiConfig = builder.Configuration.GetSection("WebAPIConfiguration").Get<WebAPIConfiguration>();
var allowedOrigins = webApiConfig?.AllowedOrigins ?? Array.Empty<string>();

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        }
        else if (builder.Environment.IsDevelopment())
        {
            // Yalnızca geliştirmede: herhangi bir origin'e izin ver. Credentials ile birlikte
            // "*" kullanılamayacağı için origin'i yansıtıyoruz.
            policy.SetIsOriginAllowed(_ => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        }
        // Production'da AllowedOrigins boşsa fail-closed: hiçbir cross-origin credential'lı isteğe izin verilmez.
    })
);

var app = builder.Build();

// Geliştirme ortamında bekleyen EF migration'larını otomatik uygula (local Docker/dev için
// şemayı hazır hale getirir). Production'da migration bilinçli olarak elle uygulanır.
if (app.Environment.IsDevelopment())
{
    using var migrationScope = app.Services.CreateScope();
    var db = migrationScope.ServiceProvider.GetRequiredService<Persistence.Contexts.BaseDbContext>();
    Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.Migrate(db.Database);
}

// Swagger yalnızca production dışında açık — API yüzeyini prod'da ifşa etmemek için.
if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(opt => { opt.DocExpansion(DocExpansion.None); });
}

if (app.Environment.IsProduction())
    app.ConfigureCustomExceptionMiddleware();

app.UseCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

// Genel rate limit (60/dk, IP bazlı) tüm controller'lara uygulanır; auth/password-reset gibi
// daha sıkı limitler ilgili controller'larda [EnableRateLimiting] ile override edilir.
app.MapControllers().RequireRateLimiting("fixed");
app.MapHub<OrderHub>("/hubs/order");
app.MapHub<RestaurantHub>("/hubs/restaurant");
app.MapHub<CourierHub>("/hubs/courier");

// Hangfire dashboard: yetkilendirme filtresi olmadan açık bırakılmamalı. Yalnızca geliştirme
// ortamında ya da yerel isteklerde erişilebilir; production'da uzak erişim reddedilir.
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireDashboardAuthorizationFilter(app.Environment.IsDevelopment()) }
});

// Register recurring jobs
RecurringJob.AddOrUpdate<ICleanupJobService>("cleanup-reset-tokens", s => s.CleanupExpiredResetTokens(), Cron.Daily);
RecurringJob.AddOrUpdate<ICleanupJobService>("cleanup-refresh-tokens", s => s.CleanupExpiredRefreshTokens(), Cron.Weekly);
RecurringJob.AddOrUpdate<IDeliveryTimeoutJobService>("check-delivery-timeouts", s => s.CheckDeliveryTimeouts(), "*/15 * * * *");
RecurringJob.AddOrUpdate<ICourierJobService>("check-expired-assignments", s => s.CheckExpiredAssignments(), "*/1 * * * *");
RecurringJob.AddOrUpdate<ICourierJobService>("auto-offline-couriers", s => s.AutoOfflineInactiveCouriers(), "*/5 * * * *");
RecurringJob.AddOrUpdate<IScheduledOrderJobService>("process-scheduled-orders", s => s.ProcessDueScheduledOrders(), "*/5 * * * *");
RecurringJob.AddOrUpdate<ISettlementJobService>("generate-daily-settlements", s => s.GenerateDailySettlements(), "5 0 * * *");

app.Run();