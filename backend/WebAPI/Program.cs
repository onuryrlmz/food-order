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

// .env dosyasını yükle (önce yükle ki diğer config'ler override edebilsin)
DotNetEnv.Env.Load(Path.Combine(Directory.GetCurrentDirectory(), "..", ".env"));
DotNetEnv.Env.Load(); // WebAPI klasöründe .env varsa onu da dene

var builder = WebApplication.CreateBuilder(args);
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

// Rate limiting - API koruması
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", limiterOptions =>
    {
        limiterOptions.PermitLimit = 60;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 5;
    });

    options.AddFixedWindowLimiter("auth", limiterOptions =>
    {
        limiterOptions.PermitLimit = 10;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueLimit = 0;
    });

    options.AddFixedWindowLimiter("password-reset", limiterOptions =>
    {
        limiterOptions.PermitLimit = 3;
        limiterOptions.Window = TimeSpan.FromMinutes(10);
        limiterOptions.QueueLimit = 0;
    });

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
            policy.WithOrigins(allowedOrigins);
        else
            policy.SetIsOriginAllowed(_ => true); // dev fallback

        policy.AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    })
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
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

app.MapControllers();
app.MapHub<OrderHub>("/hubs/order");
app.MapHub<RestaurantHub>("/hubs/restaurant");

// Hangfire dashboard (admin only in production)
app.UseHangfireDashboard("/hangfire");

// Register recurring jobs
RecurringJob.AddOrUpdate<ISubscriptionJobService>("check-expired", s => s.CheckExpiredSubscriptions(), Cron.Hourly);
RecurringJob.AddOrUpdate<ISubscriptionJobService>("expiry-reminder", s => s.SendExpiryReminders(), Cron.Daily);
RecurringJob.AddOrUpdate<ISubscriptionJobService>("auto-renew", s => s.AutoRenewSubscriptions(), Cron.Daily);
RecurringJob.AddOrUpdate<ISubscriptionJobService>("usage-warnings", s => s.CheckUsageWarnings(), Cron.Hourly);
RecurringJob.AddOrUpdate<ICleanupJobService>("cleanup-reset-tokens", s => s.CleanupExpiredResetTokens(), Cron.Daily);
RecurringJob.AddOrUpdate<ICleanupJobService>("cleanup-refresh-tokens", s => s.CleanupExpiredRefreshTokens(), Cron.Weekly);
RecurringJob.AddOrUpdate<IDeliveryTimeoutJobService>("check-delivery-timeouts", s => s.CheckDeliveryTimeouts(), "*/15 * * * *");

app.Run();
