using Application;
using Base.Constant;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NArchitecture.Core.CrossCuttingConcerns.Exception.WebApi.Extensions;
using NArchitecture.Core.Security.Encryption;
using Persistence;
using Swashbuckle.AspNetCore.SwaggerUI;
using WebAPI.Helpers;
using TokenOptions = NArchitecture.Core.Security.JWT.TokenOptions;

var builder = WebApplication.CreateBuilder(args);

Global.Configuration = builder.Configuration;
var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<TokenOptions>();
System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

builder.Services.AddControllers();
builder.Services.AddApplicationServices();
//builder.Services.AddSecurityServices<>();
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServices();
builder.Services.AddHttpContextAccessor();

//builder.Services.AddAuthorization();
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = tokenOptions.Issuer,
            ValidAudience = tokenOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = SecurityKeyHelper.CreateSecurityKey(tokenOptions.SecurityKey)
        };
    });

builder.Services.AddDistributedMemoryCache(); // InMemory

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(opt =>
    opt.AddDefaultPolicy(p => { p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); })
);

builder.Services.AddSwaggerGen();
/*
builder.Services.AddSwaggerGen(opt =>
{
    opt.AddSecurityDefinition(
        name: "Bearer",
        securityScheme: new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description =
                "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345.54321\""
        }
    );
    opt.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                },
                new string[] { }
            }
        }
    );
});
*/

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opt => { opt.DocExpansion(DocExpansion.None); });
}

if (app.Environment.IsProduction())
    app.ConfigureCustomExceptionMiddleware();

app.UseAuthentication();
//app.UseAuthorization();

app.MapControllers();
app.UseCors(opt =>
    opt.WithOrigins(app.Configuration.GetSection("WebAPIConfiguration").Get<WebAPIConfiguration>().AllowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
);
app.Run();