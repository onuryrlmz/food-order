using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSubscriptionAddCommission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Basket",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserShippingAddressId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserInvoiceAddressId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SellerId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RestaurantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    PaymentOptionId = table.Column<int>(type: "int", nullable: false),
                    TotalQuantity = table.Column<int>(type: "int", nullable: false),
                    TotalProductPrice = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    TotalShipmentPrice = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    TotalShipmentDiscount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    TotalDiscount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Basket", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CourierCompany",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LegalName = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TaxCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TaxArea = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IBAN = table.Column<string>(type: "varchar(34)", maxLength: 34, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Phone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ContactPerson = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StatusId = table.Column<short>(type: "smallint", nullable: false),
                    CompanyTypeId = table.Column<short>(type: "smallint", nullable: false),
                    IdentityNumber = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CommissionRate = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    LogoUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourierCompany", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Cuisine",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cuisine", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SellerId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RestaurantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DeliveryAddressId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    InvoiceAddressId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    StatusId = table.Column<short>(type: "smallint", nullable: false),
                    PaymentStatusId = table.Column<short>(type: "smallint", nullable: false),
                    PaymentOptionId = table.Column<int>(type: "int", nullable: false),
                    TotalProductPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ShipmentPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CouponId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CouponCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Notes = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CancellationReason = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PickedUpAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeliveredAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeliveryDistanceKm = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    CourierId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    DeliveryAssignmentId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PaymentLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrderId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    PaymentId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    Action = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RequestData = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ResponseData = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StatusCode = table.Column<int>(type: "int", nullable: true),
                    IsSuccess = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ErrorMessage = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DurationMs = table.Column<int>(type: "int", nullable: true),
                    IpAddress = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentLogs", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PlatformCommissionSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CommissionRate = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    FixedFee = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    SetByUserId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    Notes = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformCommissionSchedules", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Restaurant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SellerId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Phone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MinimumOrderPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MinDeliveryTime = table.Column<int>(type: "int", nullable: false),
                    MaxDeliveryTime = table.Column<int>(type: "int", nullable: false),
                    CoverImage = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Latitude = table.Column<decimal>(type: "decimal(10,7)", precision: 10, scale: 7, nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(10,7)", precision: 10, scale: 7, nullable: true),
                    ServiceAreaPolygonWkt = table.Column<string>(type: "LONGTEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsOpen = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Rating = table.Column<decimal>(type: "decimal(3,2)", precision: 3, scale: 2, nullable: false),
                    RatingCount = table.Column<int>(type: "int", nullable: false),
                    DefaultAssignmentStrategyId = table.Column<short>(type: "smallint", nullable: true),
                    HasOwnCouriers = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ApprovedByUserId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Restaurant", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RestaurantCdnUpdateQueue",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RestaurantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    StatusId = table.Column<short>(type: "smallint", nullable: false),
                    ErrorMessage = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProcessedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantCdnUpdateQueue", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ScheduledOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RestaurantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DeliveryAddressId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    InvoiceAddressId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    StatusId = table.Column<short>(type: "smallint", nullable: false),
                    ScheduledDeliveryTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ProcessAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PaymentOptionId = table.Column<short>(type: "smallint", nullable: false),
                    Notes = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CancellationReason = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConvertedOrderId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    BasketSnapshotJson = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CouponId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CouponCode = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledOrders", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ScheduledTask",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RestaurantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsStart = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsFinish = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ScheduleJobId = table.Column<int>(type: "int", nullable: false),
                    ExtraId1 = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ExtraId2 = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ExtraId3 = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledTask", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Seller",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CompanyType = table.Column<short>(type: "smallint", nullable: false),
                    CompanyStatus = table.Column<short>(type: "smallint", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LegalName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TaxCode = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TaxArea = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IBAN = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ApiKey = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ApiSecret = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsEInvoiceAvaible = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IdentityNumber = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seller", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SellerDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SellerId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Key1 = table.Column<int>(type: "int", nullable: false),
                    Key2 = table.Column<int>(type: "int", nullable: true),
                    ValueStr = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ValueInt = table.Column<int>(type: "int", nullable: true),
                    ValueDec = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    ValueDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellerDetail", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SettlementPeriods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SellerId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RestaurantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PeriodDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TotalOrderCount = table.Column<int>(type: "int", nullable: false),
                    TotalOrderAmount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    TotalCommission = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    TotalFixedFee = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    TotalNetAmount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    StatusId = table.Column<short>(type: "smallint", nullable: false),
                    IBAN = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BankTransferRef = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ApprovedByUserId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ApprovedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    PaidAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Notes = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettlementPeriods", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserRoleId = table.Column<short>(type: "smallint", nullable: false),
                    UserStatusId = table.Column<short>(type: "smallint", nullable: false),
                    Email = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FirstName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BirthDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    SexId = table.Column<short>(type: "smallint", nullable: true),
                    ProfilePhotoUrl = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SellerId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ActivationKey = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OrderItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrderId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MenuId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ItemSnapshotJson = table.Column<string>(type: "LONGTEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItem_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OrderStatusHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrderId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    StatusId = table.Column<short>(type: "smallint", nullable: false),
                    Note = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OccurredAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatusHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderStatusHistories_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrderId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SellerId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Amount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    SellerPayoutAmount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    CommissionAmount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    StatusId = table.Column<short>(type: "smallint", nullable: false),
                    ProviderPaymentId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProviderConversationId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProviderTransactionId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProviderFraudStatus = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PaymentOptionId = table.Column<int>(type: "int", nullable: false),
                    CardLastFourDigits = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CardType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CardAssociation = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CardAlias = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ErrorMessage = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ErrorCode = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    FailedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    RefundedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    RefundTransactionId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RefundReason = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Tips",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrderId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CourierId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    Amount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    PresetPercentage = table.Column<short>(type: "smallint", nullable: true),
                    IsPreDelivery = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsSettled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tips_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RestaurantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Category_Restaurant_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Menu",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RestaurantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Price = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Menu_Restaurant_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OptionTemplate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RestaurantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MinCount = table.Column<int>(type: "int", nullable: false),
                    MaxCount = table.Column<int>(type: "int", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OptionTemplate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OptionTemplate_Restaurant_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RestaurantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CuisineId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProductType = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Product_Restaurant_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RestaurantCommissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RestaurantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CommissionRate = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    FixedFee = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    SetByUserId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    Reason = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Notes = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantCommissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestaurantCommissions_Restaurant_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RestaurantWorkingHour",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RestaurantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DayOfWeek = table.Column<short>(type: "smallint", nullable: false),
                    OpenTime = table.Column<TimeOnly>(type: "time(6)", nullable: false),
                    CloseTime = table.Column<TimeOnly>(type: "time(6)", nullable: false),
                    IsClosed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantWorkingHour", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestaurantWorkingHour_Restaurant_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Coupon",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SellerId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RestaurantId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    Code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<short>(type: "smallint", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaxDiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    BuyQuantity = table.Column<int>(type: "int", nullable: false),
                    GetQuantity = table.Column<int>(type: "int", nullable: false),
                    MinOrderAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ApplicableType = table.Column<short>(type: "smallint", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UsageLimit = table.Column<int>(type: "int", nullable: true),
                    UsagePerUser = table.Column<int>(type: "int", nullable: true),
                    CurrentUsageCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Coupon_Restaurant_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurant",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Coupon_Seller_SellerId",
                        column: x => x.SellerId,
                        principalTable: "Seller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SettlementItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrderId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SellerId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RestaurantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrderAmount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    CommissionRate = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    CommissionAmount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    FixedFee = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    CommissionSourceType = table.Column<short>(type: "smallint", nullable: false),
                    CommissionSourceId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PeriodDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    SettlementPeriodId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    Notes = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettlementItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SettlementItems_SettlementPeriods_SettlementPeriodId",
                        column: x => x.SettlementPeriodId,
                        principalTable: "SettlementPeriods",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    SellerId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    RestaurantId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    AddressType = table.Column<short>(type: "smallint", nullable: false),
                    AddressName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FirstName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Phone = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CityId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TownId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    NeighbourhoodId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AddressLine1 = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AddressLine2 = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Latitude = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Longitude = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDefault = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    InvoiceType = table.Column<short>(type: "smallint", nullable: true),
                    TaxCode = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TaxArea = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Address_Restaurant_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurant",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Address_Seller_SellerId",
                        column: x => x.SellerId,
                        principalTable: "Seller",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Address_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Courier",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CourierCompanyId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    RestaurantId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CourierTypeId = table.Column<short>(type: "smallint", nullable: false),
                    StatusId = table.Column<short>(type: "smallint", nullable: false),
                    AvailabilityStatusId = table.Column<short>(type: "smallint", nullable: false),
                    VehicleType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VehiclePlate = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdentityNumber = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IBAN = table.Column<string>(type: "varchar(34)", maxLength: 34, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Rating = table.Column<decimal>(type: "decimal(3,2)", precision: 3, scale: 2, nullable: false),
                    RatingCount = table.Column<int>(type: "int", nullable: false),
                    TotalDeliveries = table.Column<int>(type: "int", nullable: false),
                    CurrentLatitude = table.Column<decimal>(type: "decimal(10,7)", precision: 10, scale: 7, nullable: true),
                    CurrentLongitude = table.Column<decimal>(type: "decimal(10,7)", precision: 10, scale: 7, nullable: true),
                    LastLocationUpdate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courier", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courier_CourierCompany_CourierCompanyId",
                        column: x => x.CourierCompanyId,
                        principalTable: "CourierCompany",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Courier_Restaurant_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurant",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Courier_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FavoriteRestaurant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RestaurantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteRestaurant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FavoriteRestaurant_Restaurant_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FavoriteRestaurant_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "NotificationPreferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    NotificationTypeId = table.Column<short>(type: "smallint", nullable: false),
                    IsEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationPreferences_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TypeId = table.Column<short>(type: "smallint", nullable: false),
                    Title = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Message = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Data = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsRead = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    RelatedOrderId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PasswordResetToken",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Code = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Method = table.Column<short>(type: "smallint", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    FailedAttempts = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResetToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordResetToken_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Token = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExpiresAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ReplacedByToken = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshToken_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Review",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrderId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RestaurantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Rating = table.Column<short>(type: "smallint", nullable: false),
                    Comment = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Review", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Review_Restaurant_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Review_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SearchHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Query = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SearchType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ResultCount = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SearchHistories_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SupportTickets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrderId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    RestaurantId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    TopicId = table.Column<short>(type: "smallint", nullable: false),
                    StatusId = table.Column<short>(type: "smallint", nullable: false),
                    Subject = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsEscalated = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Rating = table.Column<short>(type: "smallint", nullable: true),
                    RatingComment = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ResolvedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ClosedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportTickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupportTickets_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserExternalInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Provider = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Key = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserExternalInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserExternalInfos_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CategoryDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CategoryId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MenuId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryDetail_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BasketItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    BasketId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MenuId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasketItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BasketItem_Basket_BasketId",
                        column: x => x.BasketId,
                        principalTable: "Basket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BasketItem_Menu_MenuId",
                        column: x => x.MenuId,
                        principalTable: "Menu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MenuOption",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MenuId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OptionTemplateId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MinCount = table.Column<int>(type: "int", nullable: false),
                    MaxCount = table.Column<int>(type: "int", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuOption_Menu_MenuId",
                        column: x => x.MenuId,
                        principalTable: "Menu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuOption_OptionTemplate_OptionTemplateId",
                        column: x => x.OptionTemplateId,
                        principalTable: "OptionTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OptionTemplateValue",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OptionTemplateId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ProductId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Price = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OptionTemplateValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OptionTemplateValue_OptionTemplate_OptionTemplateId",
                        column: x => x.OptionTemplateId,
                        principalTable: "OptionTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OptionTemplateValue_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProductAttribute",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ProductId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MinCount = table.Column<int>(type: "int", nullable: false),
                    MaxCount = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttribute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductAttribute_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CouponCategory",
                columns: table => new
                {
                    CouponId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CategoryId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CouponCategory", x => new { x.CouponId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_CouponCategory_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CouponCategory_Coupon_CouponId",
                        column: x => x.CouponId,
                        principalTable: "Coupon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CouponMenu",
                columns: table => new
                {
                    CouponId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MenuId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CouponMenu", x => new { x.CouponId, x.MenuId });
                    table.ForeignKey(
                        name: "FK_CouponMenu_Coupon_CouponId",
                        column: x => x.CouponId,
                        principalTable: "Coupon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CouponMenu_Menu_MenuId",
                        column: x => x.MenuId,
                        principalTable: "Menu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserCoupon",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CouponId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrderId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UsedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UsageCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCoupon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCoupon_Coupon_CouponId",
                        column: x => x.CouponId,
                        principalTable: "Coupon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCoupon_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserCoupon_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RestaurantCourierAgreement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RestaurantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CourierCompanyId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CourierId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    StatusId = table.Column<short>(type: "smallint", nullable: false),
                    AssignmentStrategyId = table.Column<short>(type: "smallint", nullable: false),
                    AgreedDeliveryFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    PerKmFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    IsDefault = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    EffectiveUntil = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantCourierAgreement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestaurantCourierAgreement_CourierCompany_CourierCompanyId",
                        column: x => x.CourierCompanyId,
                        principalTable: "CourierCompany",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RestaurantCourierAgreement_Courier_CourierId",
                        column: x => x.CourierId,
                        principalTable: "Courier",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RestaurantCourierAgreement_Restaurant_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SupportActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TicketId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ActionType = table.Column<short>(type: "smallint", nullable: false),
                    ActionData = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsApproved = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsExecuted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ApprovedByUserId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ExecutedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupportActions_SupportTickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "SupportTickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SupportMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TicketId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SenderType = table.Column<short>(type: "smallint", nullable: false),
                    Content = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AiModelUsed = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TokensUsed = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupportMessages_SupportTickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "SupportTickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MenuOptionValue",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MenuOptionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ProductId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OptionTemplateValueId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    Price = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuOptionValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuOptionValue_MenuOption_MenuOptionId",
                        column: x => x.MenuOptionId,
                        principalTable: "MenuOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuOptionValue_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OptionTemplateValueOption",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OptionTemplateValueId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MinCount = table.Column<int>(type: "int", nullable: false),
                    MaxCount = table.Column<int>(type: "int", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OptionTemplateValueOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OptionTemplateValueOption_OptionTemplateValue_OptionTemplate~",
                        column: x => x.OptionTemplateValueId,
                        principalTable: "OptionTemplateValue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProductAttributeValue",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ProductAttributeId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ProductId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttributeValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductAttributeValue_ProductAttribute_ProductAttributeId",
                        column: x => x.ProductAttributeId,
                        principalTable: "ProductAttribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductAttributeValue_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DeliveryAssignment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrderId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RestaurantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CourierId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CourierCompanyId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    AgreementId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    StatusId = table.Column<short>(type: "smallint", nullable: false),
                    AssignmentStrategyId = table.Column<short>(type: "smallint", nullable: false),
                    DeliveryFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    DistanceKm = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    CustomerLatitude = table.Column<decimal>(type: "decimal(10,7)", precision: 10, scale: 7, nullable: true),
                    CustomerLongitude = table.Column<decimal>(type: "decimal(10,7)", precision: 10, scale: 7, nullable: true),
                    RestaurantLatitude = table.Column<decimal>(type: "decimal(10,7)", precision: 10, scale: 7, nullable: true),
                    RestaurantLongitude = table.Column<decimal>(type: "decimal(10,7)", precision: 10, scale: 7, nullable: true),
                    OfferedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    AcceptedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    RejectedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    PickedUpAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeliveredAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CancellationReason = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExpiresAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    AttemptNumber = table.Column<int>(type: "int", nullable: false),
                    RejectionReason = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryAssignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryAssignment_CourierCompany_CourierCompanyId",
                        column: x => x.CourierCompanyId,
                        principalTable: "CourierCompany",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DeliveryAssignment_Courier_CourierId",
                        column: x => x.CourierId,
                        principalTable: "Courier",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DeliveryAssignment_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeliveryAssignment_RestaurantCourierAgreement_AgreementId",
                        column: x => x.AgreementId,
                        principalTable: "RestaurantCourierAgreement",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BasketItemValue",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    BasketItemId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MenuOptionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MenuOptionValueId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ProductId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasketItemValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BasketItemValue_BasketItem_BasketItemId",
                        column: x => x.BasketItemId,
                        principalTable: "BasketItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BasketItemValue_MenuOptionValue_MenuOptionValueId",
                        column: x => x.MenuOptionValueId,
                        principalTable: "MenuOptionValue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BasketItemValue_MenuOption_MenuOptionId",
                        column: x => x.MenuOptionId,
                        principalTable: "MenuOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BasketItemValue_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MenuOptionValueOption",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MenuOptionValueId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OptionTemplateValueOptionId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MinCount = table.Column<int>(type: "int", nullable: false),
                    MaxCount = table.Column<int>(type: "int", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuOptionValueOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuOptionValueOption_MenuOptionValue_MenuOptionValueId",
                        column: x => x.MenuOptionValueId,
                        principalTable: "MenuOptionValue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OrderItemValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrderItemId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MenuOptionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MenuOptionValueId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ProductId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItemValues_MenuOptionValue_MenuOptionValueId",
                        column: x => x.MenuOptionValueId,
                        principalTable: "MenuOptionValue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItemValues_MenuOption_MenuOptionId",
                        column: x => x.MenuOptionId,
                        principalTable: "MenuOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItemValues_OrderItem_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItemValues_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OptionTemplateValueOptionValue",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OptionTemplateValueOptionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ProductId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Price = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OptionTemplateValueOptionValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OptionTemplateValueOptionValue_OptionTemplateValueOption_Opt~",
                        column: x => x.OptionTemplateValueOptionId,
                        principalTable: "OptionTemplateValueOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OptionTemplateValueOptionValue_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CourierEarning",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CourierId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DeliveryAssignmentId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrderId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DeliveryFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TipAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    BonusAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    TotalEarning = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsSettled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SettledAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    SettlementReference = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourierEarning", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourierEarning_Courier_CourierId",
                        column: x => x.CourierId,
                        principalTable: "Courier",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourierEarning_DeliveryAssignment_DeliveryAssignmentId",
                        column: x => x.DeliveryAssignmentId,
                        principalTable: "DeliveryAssignment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CourierLocationHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CourierId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DeliveryAssignmentId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    Latitude = table.Column<decimal>(type: "decimal(10,7)", precision: 10, scale: 7, nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(10,7)", precision: 10, scale: 7, nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourierLocationHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourierLocationHistory_Courier_CourierId",
                        column: x => x.CourierId,
                        principalTable: "Courier",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourierLocationHistory_DeliveryAssignment_DeliveryAssignment~",
                        column: x => x.DeliveryAssignmentId,
                        principalTable: "DeliveryAssignment",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MenuOptionValueOptionValue",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MenuOptionValueOptionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ProductId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OptionTemplateValueOptionValueId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    Price = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuOptionValueOptionValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuOptionValueOptionValue_MenuOptionValueOption_MenuOptionV~",
                        column: x => x.MenuOptionValueOptionId,
                        principalTable: "MenuOptionValueOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuOptionValueOptionValue_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BasketItemValueItemValue",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    BasketItemValueId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MenuOptionValueOptionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MenuOptionValueOptionValueId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ProductId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasketItemValueItemValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BasketItemValueItemValue_BasketItemValue_BasketItemValueId",
                        column: x => x.BasketItemValueId,
                        principalTable: "BasketItemValue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BasketItemValueItemValue_MenuOptionValueOptionValue_MenuOpti~",
                        column: x => x.MenuOptionValueOptionValueId,
                        principalTable: "MenuOptionValueOptionValue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BasketItemValueItemValue_MenuOptionValueOption_MenuOptionVal~",
                        column: x => x.MenuOptionValueOptionId,
                        principalTable: "MenuOptionValueOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BasketItemValueItemValue_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OrderItemValueOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrderItemValueId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MenuOptionValueOptionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MenuOptionValueOptionValueId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ProductId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemValueOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItemValueOptions_MenuOptionValueOptionValue_MenuOptionV~",
                        column: x => x.MenuOptionValueOptionValueId,
                        principalTable: "MenuOptionValueOptionValue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItemValueOptions_MenuOptionValueOption_MenuOptionValueO~",
                        column: x => x.MenuOptionValueOptionId,
                        principalTable: "MenuOptionValueOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItemValueOptions_OrderItemValues_OrderItemValueId",
                        column: x => x.OrderItemValueId,
                        principalTable: "OrderItemValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItemValueOptions_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "CourierCompany",
                columns: new[] { "Id", "CommissionRate", "CompanyTypeId", "ContactPerson", "CreatedDate", "DeletedDate", "Email", "IBAN", "IdentityNumber", "LegalName", "LogoUrl", "Name", "Phone", "StatusId", "TaxArea", "TaxCode", "UpdatedDate" },
                values: new object[] { new Guid("ae011111-1111-1111-1111-111111111111"), 0.15m, (short)2, "Veli Firma", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "firma@kurye.com", "TR999999999999999999999999", null, "Hızlı Kurye Ltd. Şti.", null, "Hızlı Kurye", "05500000007", (short)2, "Nilüfer", "9999999999", null });

            migrationBuilder.InsertData(
                table: "Cuisine",
                columns: new[] { "Id", "CreatedDate", "DeletedDate", "Name", "OrderIndex", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("cccc1111-1111-1111-1111-111111111111"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "Pizza", 0, null },
                    { new Guid("cccc2222-2222-2222-2222-222222222222"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "Burger", 0, null },
                    { new Guid("cccc3333-3333-3333-3333-333333333333"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "Kebap", 0, null },
                    { new Guid("cccc4444-4444-4444-4444-444444444444"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "Türk Mutfağı", 0, null },
                    { new Guid("cccc5555-5555-5555-5555-555555555555"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "Fast Food", 0, null }
                });

            migrationBuilder.InsertData(
                table: "PlatformCommissionSchedules",
                columns: new[] { "Id", "CommissionRate", "CreatedDate", "DeletedDate", "EffectiveFrom", "EffectiveTo", "FixedFee", "Notes", "SetByUserId", "UpdatedDate" },
                values: new object[] { new Guid("ab111111-1111-1111-1111-111111111111"), 0.10m, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, 5.00m, "Varsayılan platform komisyonu", new Guid("11111111-1111-1111-1111-111111111111"), null });

            migrationBuilder.InsertData(
                table: "Restaurant",
                columns: new[] { "Id", "ApprovedAt", "ApprovedByUserId", "CoverImage", "CreatedDate", "DefaultAssignmentStrategyId", "DeletedDate", "Description", "Email", "HasOwnCouriers", "IsActive", "IsOpen", "Latitude", "Longitude", "MaxDeliveryTime", "MinDeliveryTime", "MinimumOrderPrice", "Name", "Phone", "Rating", "RatingCount", "SellerId", "ServiceAreaPolygonWkt", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("bbbb1111-1111-1111-1111-111111111111"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), "https://picsum.photos/seed/pizza1/800/400", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, null, "Taş fırında İtalyan pizzalar", "nilufer@pizzaci.com", false, true, true, 40.2273m, 28.8891m, 45, 25, 150m, "Pizzacı Ahmet - Nilüfer", "05551112233", 4.5m, 120, new Guid("aaaa1111-1111-1111-1111-111111111111"), null, null },
                    { new Guid("bbbb2222-2222-2222-2222-222222222222"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), "https://picsum.photos/seed/pizza2/800/400", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, null, "Lezzetli pizzalar, hızlı teslimat", "osmangazi@pizzaci.com", false, true, true, 40.1885m, 29.0610m, 50, 30, 100m, "Pizzacı Ahmet - Osmangazi", "05551112244", 4.2m, 85, new Guid("aaaa1111-1111-1111-1111-111111111111"), null, null },
                    { new Guid("bbbb3333-3333-3333-3333-333333333333"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), "https://picsum.photos/seed/kebap1/800/400", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, null, "Geleneksel Türk kebapları, mangal lezzetleri", "info@kebapci.com", true, true, true, 40.1950m, 29.0200m, 55, 35, 200m, "Kebapçı Mehmet", "05551113355", 4.7m, 230, new Guid("aaaa2222-2222-2222-2222-222222222222"), null, null }
                });

            migrationBuilder.InsertData(
                table: "Seller",
                columns: new[] { "Id", "ApiKey", "ApiSecret", "CompanyStatus", "CompanyType", "CreatedDate", "DeletedDate", "IBAN", "IdentityNumber", "IsEInvoiceAvaible", "LegalName", "Name", "TaxArea", "TaxCode", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("aaaa1111-1111-1111-1111-111111111111"), null, null, (short)2, (short)2, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "TR111111111111111111111111", null, true, "Pizzacı Ahmet Ltd. Şti.", "Pizzacı Ahmet", "Nilüfer", "1111111111", null },
                    { new Guid("aaaa2222-2222-2222-2222-222222222222"), null, null, (short)2, (short)1, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "TR222222222222222222222222", "12345678901", false, "Mehmet Kebap", "Kebapçı Mehmet", "Osmangazi", "2222222222", null }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "ActivationKey", "BirthDate", "CreatedDate", "DeletedDate", "Email", "FirstName", "LastName", "Password", "PhoneNumber", "ProfilePhotoUrl", "SellerId", "SexId", "UpdatedDate", "UserRoleId", "UserStatusId" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), null, new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "admin@esnaftan.com", "Esnaftan", "Admin", "$2a$12$FSkQpNFCoggkjDbhmQIKLuk2XIF6GF0lCW7nPK7vbJPsV91.zdvzW", "05500000001", null, null, (short)1, null, (short)1, (short)1 },
                    { new Guid("22222222-2222-2222-2222-222222222222"), null, null, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "info@pizzaci.com", "Ahmet", "Pizzacı", "$2a$12$FSkQpNFCoggkjDbhmQIKLuk2XIF6GF0lCW7nPK7vbJPsV91.zdvzW", "05500000002", null, new Guid("aaaa1111-1111-1111-1111-111111111111"), (short)1, null, (short)4, (short)1 },
                    { new Guid("33333333-3333-3333-3333-333333333333"), null, null, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "info@kebapci.com", "Mehmet", "Kebapçı", "$2a$12$FSkQpNFCoggkjDbhmQIKLuk2XIF6GF0lCW7nPK7vbJPsV91.zdvzW", "05500000003", null, new Guid("aaaa2222-2222-2222-2222-222222222222"), (short)1, null, (short)4, (short)1 },
                    { new Guid("44444444-4444-4444-4444-444444444444"), null, null, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "ali@gmail.com", "Ali", "Yılmaz", "$2a$12$FSkQpNFCoggkjDbhmQIKLuk2XIF6GF0lCW7nPK7vbJPsV91.zdvzW", "05500000004", null, null, (short)1, null, (short)2, (short)1 },
                    { new Guid("55555555-5555-5555-5555-555555555555"), null, null, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "ayse@gmail.com", "Ayşe", "Demir", "$2a$12$FSkQpNFCoggkjDbhmQIKLuk2XIF6GF0lCW7nPK7vbJPsV91.zdvzW", "05500000005", null, null, (short)2, null, (short)2, (short)1 },
                    { new Guid("66666666-6666-6666-6666-666666666666"), null, null, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "kurye@gmail.com", "Hasan", "Kurye", "$2a$12$FSkQpNFCoggkjDbhmQIKLuk2XIF6GF0lCW7nPK7vbJPsV91.zdvzW", "05500000006", null, null, (short)1, null, (short)6, (short)1 },
                    { new Guid("77777777-7777-7777-7777-777777777777"), null, null, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "firma@kurye.com", "Veli", "Firma", "$2a$12$FSkQpNFCoggkjDbhmQIKLuk2XIF6GF0lCW7nPK7vbJPsV91.zdvzW", "05500000007", null, null, (short)1, null, (short)7, (short)1 }
                });

            migrationBuilder.InsertData(
                table: "Address",
                columns: new[] { "Id", "AddressLine1", "AddressLine2", "AddressName", "AddressType", "CityId", "CreatedDate", "DeletedDate", "FirstName", "InvoiceType", "IsDefault", "LastName", "Latitude", "Longitude", "NeighbourhoodId", "Phone", "RestaurantId", "SellerId", "TaxArea", "TaxCode", "TownId", "UpdatedDate", "UserId" },
                values: new object[,]
                {
                    { new Guid("ad011111-1111-1111-1111-111111111111"), "Ahmet Yesevi Mah. Bey Sk. No:4/B", null, "Restoran Adresi", (short)4, new Guid("5d0c385c-810d-4dd6-9462-259183584992"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "Pizzacı", null, true, "Ahmet", "40.2273", "28.8891", new Guid("1c2d8a14-38df-448c-8125-14535bf0b7b3"), "05551112233", new Guid("bbbb1111-1111-1111-1111-111111111111"), new Guid("aaaa1111-1111-1111-1111-111111111111"), null, null, new Guid("342b6d4d-42bf-4085-92e7-8d2b51de130a"), null, null },
                    { new Guid("ad022222-2222-2222-2222-222222222222"), "Çamlıca Mah. Kebap Sok. No:15", null, "Restoran Adresi", (short)4, new Guid("5d0c385c-810d-4dd6-9462-259183584992"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "Kebapçı", null, true, "Mehmet", "40.1950", "29.0200", new Guid("1c2d8a14-38df-448c-8125-14535bf0b7b3"), "05551113355", new Guid("bbbb3333-3333-3333-3333-333333333333"), new Guid("aaaa2222-2222-2222-2222-222222222222"), null, null, new Guid("342b6d4d-42bf-4085-92e7-8d2b51de130a"), null, null },
                    { new Guid("ad031111-1111-1111-1111-111111111111"), "Geçit Mah. 1. Begonya Sok. No:57 D:6", "Oliva Sitesi B Blok", "Ev", (short)2, new Guid("5d0c385c-810d-4dd6-9462-259183584992"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "Ali", (short)1, true, "Yılmaz", "40.2659", "28.9618", new Guid("1c2d8a14-38df-448c-8125-14535bf0b7b3"), "05500000004", null, null, null, null, new Guid("342b6d4d-42bf-4085-92e7-8d2b51de130a"), null, new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("ad042222-2222-2222-2222-222222222222"), "Özlüce Mah. İş Merkezi No:22 K:3", null, "İş", (short)2, new Guid("5d0c385c-810d-4dd6-9462-259183584992"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "Ayşe", (short)1, true, "Demir", "40.2300", "28.9100", new Guid("1c2d8a14-38df-448c-8125-14535bf0b7b3"), "05500000005", null, null, null, null, new Guid("342b6d4d-42bf-4085-92e7-8d2b51de130a"), null, new Guid("55555555-5555-5555-5555-555555555555") }
                });

            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "Id", "CreatedDate", "DeletedDate", "Name", "OrderIndex", "RestaurantId", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("aabb1111-1111-1111-1111-111111111111"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "Pizzalar", 0, new Guid("bbbb1111-1111-1111-1111-111111111111"), null },
                    { new Guid("aabb2222-2222-2222-2222-222222222222"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "İçecekler", 1, new Guid("bbbb1111-1111-1111-1111-111111111111"), null },
                    { new Guid("aabb3333-3333-3333-3333-333333333333"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "Kebaplar", 0, new Guid("bbbb3333-3333-3333-3333-333333333333"), null }
                });

            migrationBuilder.InsertData(
                table: "Courier",
                columns: new[] { "Id", "AvailabilityStatusId", "CourierCompanyId", "CourierTypeId", "CreatedDate", "CurrentLatitude", "CurrentLongitude", "DeletedDate", "IBAN", "IdentityNumber", "LastLocationUpdate", "Rating", "RatingCount", "RestaurantId", "StatusId", "TotalDeliveries", "UpdatedDate", "UserId", "VehiclePlate", "VehicleType" },
                values: new object[] { new Guid("af011111-1111-1111-1111-111111111111"), (short)1, new Guid("ae011111-1111-1111-1111-111111111111"), (short)3, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), 40.2273m, 28.8891m, null, "TR666666666666666666666666", null, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), 4.8m, 50, null, (short)2, 200, null, new Guid("66666666-6666-6666-6666-666666666666"), "16 AB 123", "Motosiklet" });

            migrationBuilder.InsertData(
                table: "Menu",
                columns: new[] { "Id", "CreatedDate", "DeletedDate", "Description", "Name", "OrderIndex", "Price", "RestaurantId", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("eeee1111-1111-1111-1111-111111111111"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "Klasik İtalyan", "Margarita Pizza (Orta)", 0, 180m, new Guid("bbbb1111-1111-1111-1111-111111111111"), null },
                    { new Guid("eeee2222-2222-2222-2222-222222222222"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "Bol malzemeli", "Karışık Pizza (Orta)", 1, 220m, new Guid("bbbb1111-1111-1111-1111-111111111111"), null },
                    { new Guid("eeee3333-3333-3333-3333-333333333333"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "", "Cola 330ml", 10, 35m, new Guid("bbbb1111-1111-1111-1111-111111111111"), null },
                    { new Guid("eeee4444-4444-4444-4444-444444444444"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "", "Ayran", 11, 20m, new Guid("bbbb1111-1111-1111-1111-111111111111"), null },
                    { new Guid("eeee5555-5555-5555-5555-555555555555"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "2 şiş, lavaş, közlenmiş", "Adana Kebap Porsiyon", 0, 320m, new Guid("bbbb3333-3333-3333-3333-333333333333"), null },
                    { new Guid("eeee6666-6666-6666-6666-666666666666"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "2 şiş, lavaş, közlenmiş", "Urfa Kebap Porsiyon", 1, 300m, new Guid("bbbb3333-3333-3333-3333-333333333333"), null }
                });

            migrationBuilder.InsertData(
                table: "Product",
                columns: new[] { "Id", "CreatedDate", "CuisineId", "DeletedDate", "Description", "Name", "OrderIndex", "Price", "ProductType", "RestaurantId", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("dddd1111-1111-1111-1111-111111111111"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), new Guid("cccc1111-1111-1111-1111-111111111111"), null, "Domates sos, mozzarella, fesleğen", "Margarita Pizza", 0, 0m, 1, new Guid("bbbb1111-1111-1111-1111-111111111111"), null },
                    { new Guid("dddd2222-2222-2222-2222-222222222222"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), new Guid("cccc1111-1111-1111-1111-111111111111"), null, "Sucuk, mantar, biber, mısır", "Karışık Pizza", 1, 0m, 1, new Guid("bbbb1111-1111-1111-1111-111111111111"), null },
                    { new Guid("dddd3333-3333-3333-3333-333333333333"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), new Guid("cccc1111-1111-1111-1111-111111111111"), null, "", "Sucuk", 0, 0m, 2, new Guid("bbbb1111-1111-1111-1111-111111111111"), null },
                    { new Guid("dddd4444-4444-4444-4444-444444444444"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), new Guid("cccc1111-1111-1111-1111-111111111111"), null, "", "Mantar", 1, 0m, 2, new Guid("bbbb1111-1111-1111-1111-111111111111"), null },
                    { new Guid("dddd5555-5555-5555-5555-555555555555"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), new Guid("cccc2222-2222-2222-2222-222222222222"), null, "Özel soslu dana burger", "Cheeseburger", 0, 0m, 1, new Guid("bbbb3333-3333-3333-3333-333333333333"), null },
                    { new Guid("dddd6666-6666-6666-6666-666666666666"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), new Guid("cccc2222-2222-2222-2222-222222222222"), null, "Çıtır tavuk burger", "Chicken Burger", 1, 0m, 1, new Guid("bbbb3333-3333-3333-3333-333333333333"), null },
                    { new Guid("dddd7777-7777-7777-7777-777777777777"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), new Guid("cccc3333-3333-3333-3333-333333333333"), null, "Acılı el yapımı kebap", "Adana Kebap", 2, 0m, 1, new Guid("bbbb3333-3333-3333-3333-333333333333"), null },
                    { new Guid("dddd8888-8888-8888-8888-888888888888"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), new Guid("cccc3333-3333-3333-3333-333333333333"), null, "Acısız kebap", "Urfa Kebap", 3, 0m, 1, new Guid("bbbb3333-3333-3333-3333-333333333333"), null },
                    { new Guid("dddd9999-9999-9999-9999-999999999999"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), new Guid("cccc5555-5555-5555-5555-555555555555"), null, "", "Cola 330ml", 10, 0m, 1, new Guid("bbbb1111-1111-1111-1111-111111111111"), null },
                    { new Guid("ddddaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), new Guid("cccc4444-4444-4444-4444-444444444444"), null, "", "Ayran", 11, 0m, 1, new Guid("bbbb1111-1111-1111-1111-111111111111"), null }
                });

            migrationBuilder.InsertData(
                table: "RestaurantCommissions",
                columns: new[] { "Id", "CommissionRate", "CreatedDate", "DeletedDate", "EffectiveFrom", "EffectiveTo", "FixedFee", "Notes", "Reason", "RestaurantId", "SetByUserId", "UpdatedDate" },
                values: new object[] { new Guid("ab221111-1111-1111-1111-111111111111"), 0.08m, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, 3.00m, null, "Özel anlaşma", new Guid("bbbb3333-3333-3333-3333-333333333333"), new Guid("11111111-1111-1111-1111-111111111111"), null });

            migrationBuilder.InsertData(
                table: "CategoryDetail",
                columns: new[] { "Id", "CategoryId", "CreatedDate", "DeletedDate", "MenuId", "OrderIndex", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("ca011111-1111-1111-1111-111111111111"), new Guid("aabb1111-1111-1111-1111-111111111111"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, new Guid("eeee1111-1111-1111-1111-111111111111"), 0, null },
                    { new Guid("ca022222-2222-2222-2222-222222222222"), new Guid("aabb1111-1111-1111-1111-111111111111"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, new Guid("eeee2222-2222-2222-2222-222222222222"), 1, null },
                    { new Guid("ca033333-3333-3333-3333-333333333333"), new Guid("aabb2222-2222-2222-2222-222222222222"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, new Guid("eeee3333-3333-3333-3333-333333333333"), 0, null },
                    { new Guid("ca044444-4444-4444-4444-444444444444"), new Guid("aabb2222-2222-2222-2222-222222222222"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, new Guid("eeee4444-4444-4444-4444-444444444444"), 1, null },
                    { new Guid("ca055555-5555-5555-5555-555555555555"), new Guid("aabb3333-3333-3333-3333-333333333333"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, new Guid("eeee5555-5555-5555-5555-555555555555"), 0, null },
                    { new Guid("ca066666-6666-6666-6666-666666666666"), new Guid("aabb3333-3333-3333-3333-333333333333"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, new Guid("eeee6666-6666-6666-6666-666666666666"), 1, null }
                });

            migrationBuilder.InsertData(
                table: "MenuOption",
                columns: new[] { "Id", "CreatedDate", "DeletedDate", "Description", "MaxCount", "MenuId", "MinCount", "Name", "OptionTemplateId", "OrderIndex", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("ff001111-1111-1111-1111-111111111111"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "Pizza hamur tipini seçin", 1, new Guid("eeee2222-2222-2222-2222-222222222222"), 1, "Pizza Tercihi", null, 0, null },
                    { new Guid("ff002222-2222-2222-2222-222222222222"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, "İstediğiniz malzemeleri ekleyin", 3, new Guid("eeee2222-2222-2222-2222-222222222222"), 0, "Ekstra Malzeme", null, 1, null }
                });

            migrationBuilder.InsertData(
                table: "MenuOptionValue",
                columns: new[] { "Id", "CreatedDate", "DeletedDate", "MenuOptionId", "OptionTemplateValueId", "OrderIndex", "Price", "ProductId", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("ff011111-1111-1111-1111-111111111111"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, new Guid("ff001111-1111-1111-1111-111111111111"), null, 0, 0m, new Guid("dddd2222-2222-2222-2222-222222222222"), null },
                    { new Guid("ff012222-2222-2222-2222-222222222222"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, new Guid("ff002222-2222-2222-2222-222222222222"), null, 0, 15m, new Guid("dddd3333-3333-3333-3333-333333333333"), null }
                });

            migrationBuilder.InsertData(
                table: "MenuOptionValueOption",
                columns: new[] { "Id", "CreatedDate", "DeletedDate", "Description", "MaxCount", "MenuOptionValueId", "MinCount", "Name", "OptionTemplateValueOptionId", "OrderIndex", "UpdatedDate" },
                values: new object[] { new Guid("ff021111-1111-1111-1111-111111111111"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, null, 4, new Guid("ff011111-1111-1111-1111-111111111111"), 0, "Çıkarılacak Malzemeler", null, 0, null });

            migrationBuilder.InsertData(
                table: "MenuOptionValueOptionValue",
                columns: new[] { "Id", "CreatedDate", "DeletedDate", "MenuOptionValueOptionId", "OptionTemplateValueOptionValueId", "OrderIndex", "Price", "ProductId", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("ff031111-1111-1111-1111-111111111111"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, new Guid("ff021111-1111-1111-1111-111111111111"), null, 0, 0m, new Guid("dddd3333-3333-3333-3333-333333333333"), null },
                    { new Guid("ff032222-2222-2222-2222-222222222222"), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, new Guid("ff021111-1111-1111-1111-111111111111"), null, 1, 0m, new Guid("dddd4444-4444-4444-4444-444444444444"), null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_RestaurantId",
                table: "Address",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_Address_SellerId",
                table: "Address",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_Address_UserId",
                table: "Address",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BasketItem_BasketId",
                table: "BasketItem",
                column: "BasketId");

            migrationBuilder.CreateIndex(
                name: "IX_BasketItem_MenuId",
                table: "BasketItem",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_BasketItemValue_BasketItemId",
                table: "BasketItemValue",
                column: "BasketItemId");

            migrationBuilder.CreateIndex(
                name: "IX_BasketItemValue_MenuOptionId",
                table: "BasketItemValue",
                column: "MenuOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_BasketItemValue_MenuOptionValueId",
                table: "BasketItemValue",
                column: "MenuOptionValueId");

            migrationBuilder.CreateIndex(
                name: "IX_BasketItemValue_ProductId",
                table: "BasketItemValue",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_BasketItemValueItemValue_BasketItemValueId",
                table: "BasketItemValueItemValue",
                column: "BasketItemValueId");

            migrationBuilder.CreateIndex(
                name: "IX_BasketItemValueItemValue_MenuOptionValueOptionId",
                table: "BasketItemValueItemValue",
                column: "MenuOptionValueOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_BasketItemValueItemValue_MenuOptionValueOptionValueId",
                table: "BasketItemValueItemValue",
                column: "MenuOptionValueOptionValueId");

            migrationBuilder.CreateIndex(
                name: "IX_BasketItemValueItemValue_ProductId",
                table: "BasketItemValueItemValue",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Category_RestaurantId",
                table: "Category",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryDetail_CategoryId",
                table: "CategoryDetail",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Coupon_Code",
                table: "Coupon",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coupon_RestaurantId",
                table: "Coupon",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_Coupon_SellerId",
                table: "Coupon",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_CouponCategory_CategoryId",
                table: "CouponCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CouponMenu_MenuId",
                table: "CouponMenu",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_Courier_AvailabilityStatusId",
                table: "Courier",
                column: "AvailabilityStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Courier_CourierCompanyId",
                table: "Courier",
                column: "CourierCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Courier_RestaurantId",
                table: "Courier",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_Courier_UserId",
                table: "Courier",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourierEarning_CourierId",
                table: "CourierEarning",
                column: "CourierId");

            migrationBuilder.CreateIndex(
                name: "IX_CourierEarning_DeliveryAssignmentId",
                table: "CourierEarning",
                column: "DeliveryAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CourierEarning_IsSettled",
                table: "CourierEarning",
                column: "IsSettled");

            migrationBuilder.CreateIndex(
                name: "IX_CourierLocationHistory_CourierId",
                table: "CourierLocationHistory",
                column: "CourierId");

            migrationBuilder.CreateIndex(
                name: "IX_CourierLocationHistory_DeliveryAssignmentId",
                table: "CourierLocationHistory",
                column: "DeliveryAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CourierLocationHistory_RecordedAt",
                table: "CourierLocationHistory",
                column: "RecordedAt");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryAssignment_AgreementId",
                table: "DeliveryAssignment",
                column: "AgreementId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryAssignment_CourierCompanyId",
                table: "DeliveryAssignment",
                column: "CourierCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryAssignment_CourierId",
                table: "DeliveryAssignment",
                column: "CourierId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryAssignment_OrderId",
                table: "DeliveryAssignment",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryAssignment_StatusId",
                table: "DeliveryAssignment",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteRestaurant_RestaurantId",
                table: "FavoriteRestaurant",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteRestaurant_UserId_RestaurantId",
                table: "FavoriteRestaurant",
                columns: new[] { "UserId", "RestaurantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Menu_RestaurantId",
                table: "Menu",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuOption_MenuId",
                table: "MenuOption",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuOption_OptionTemplateId",
                table: "MenuOption",
                column: "OptionTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuOptionValue_MenuOptionId",
                table: "MenuOptionValue",
                column: "MenuOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuOptionValue_ProductId",
                table: "MenuOptionValue",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuOptionValueOption_MenuOptionValueId",
                table: "MenuOptionValueOption",
                column: "MenuOptionValueId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuOptionValueOptionValue_MenuOptionValueOptionId",
                table: "MenuOptionValueOptionValue",
                column: "MenuOptionValueOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuOptionValueOptionValue_ProductId",
                table: "MenuOptionValueOptionValue",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationPreferences_UserId",
                table: "NotificationPreferences",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OptionTemplate_RestaurantId",
                table: "OptionTemplate",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_OptionTemplateValue_OptionTemplateId",
                table: "OptionTemplateValue",
                column: "OptionTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_OptionTemplateValue_ProductId",
                table: "OptionTemplateValue",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OptionTemplateValueOption_OptionTemplateValueId",
                table: "OptionTemplateValueOption",
                column: "OptionTemplateValueId");

            migrationBuilder.CreateIndex(
                name: "IX_OptionTemplateValueOptionValue_OptionTemplateValueOptionId",
                table: "OptionTemplateValueOptionValue",
                column: "OptionTemplateValueOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_OptionTemplateValueOptionValue_ProductId",
                table: "OptionTemplateValueOptionValue",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_OrderId",
                table: "OrderItem",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemValueOptions_MenuOptionValueOptionId",
                table: "OrderItemValueOptions",
                column: "MenuOptionValueOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemValueOptions_MenuOptionValueOptionValueId",
                table: "OrderItemValueOptions",
                column: "MenuOptionValueOptionValueId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemValueOptions_OrderItemValueId",
                table: "OrderItemValueOptions",
                column: "OrderItemValueId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemValueOptions_ProductId",
                table: "OrderItemValueOptions",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemValues_MenuOptionId",
                table: "OrderItemValues",
                column: "MenuOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemValues_MenuOptionValueId",
                table: "OrderItemValues",
                column: "MenuOptionValueId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemValues_OrderItemId",
                table: "OrderItemValues",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemValues_ProductId",
                table: "OrderItemValues",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatusHistories_OrderId",
                table: "OrderStatusHistories",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetToken_UserId",
                table: "PasswordResetToken",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId",
                table: "Payments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_RestaurantId",
                table: "Product",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttribute_ProductId",
                table: "ProductAttribute",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeValue_ProductAttributeId",
                table: "ProductAttributeValue",
                column: "ProductAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeValue_ProductId",
                table: "ProductAttributeValue",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_Token",
                table: "RefreshToken",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_UserId",
                table: "RefreshToken",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantCdnUpdateQueue_StatusId_CreatedDate",
                table: "RestaurantCdnUpdateQueue",
                columns: new[] { "StatusId", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantCommissions_RestaurantId",
                table: "RestaurantCommissions",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantCourierAgreement_CourierCompanyId",
                table: "RestaurantCourierAgreement",
                column: "CourierCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantCourierAgreement_CourierId",
                table: "RestaurantCourierAgreement",
                column: "CourierId");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantCourierAgreement_RestaurantId",
                table: "RestaurantCourierAgreement",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantWorkingHour_RestaurantId_DayOfWeek",
                table: "RestaurantWorkingHour",
                columns: new[] { "RestaurantId", "DayOfWeek" });

            migrationBuilder.CreateIndex(
                name: "IX_Review_OrderId",
                table: "Review",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Review_RestaurantId",
                table: "Review",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_UserId",
                table: "Review",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SearchHistories_UserId",
                table: "SearchHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SettlementItems_SettlementPeriodId",
                table: "SettlementItems",
                column: "SettlementPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportActions_TicketId",
                table: "SupportActions",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportMessages_TicketId",
                table: "SupportMessages",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_UserId",
                table: "SupportTickets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tips_OrderId",
                table: "Tips",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCoupon_CouponId",
                table: "UserCoupon",
                column: "CouponId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCoupon_OrderId",
                table: "UserCoupon",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCoupon_UserId_CouponId",
                table: "UserCoupon",
                columns: new[] { "UserId", "CouponId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserExternalInfos_UserId",
                table: "UserExternalInfos",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "BasketItemValueItemValue");

            migrationBuilder.DropTable(
                name: "CategoryDetail");

            migrationBuilder.DropTable(
                name: "CouponCategory");

            migrationBuilder.DropTable(
                name: "CouponMenu");

            migrationBuilder.DropTable(
                name: "CourierEarning");

            migrationBuilder.DropTable(
                name: "CourierLocationHistory");

            migrationBuilder.DropTable(
                name: "Cuisine");

            migrationBuilder.DropTable(
                name: "FavoriteRestaurant");

            migrationBuilder.DropTable(
                name: "NotificationPreferences");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "OptionTemplateValueOptionValue");

            migrationBuilder.DropTable(
                name: "OrderItemValueOptions");

            migrationBuilder.DropTable(
                name: "OrderStatusHistories");

            migrationBuilder.DropTable(
                name: "PasswordResetToken");

            migrationBuilder.DropTable(
                name: "PaymentLogs");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "PlatformCommissionSchedules");

            migrationBuilder.DropTable(
                name: "ProductAttributeValue");

            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropTable(
                name: "RestaurantCdnUpdateQueue");

            migrationBuilder.DropTable(
                name: "RestaurantCommissions");

            migrationBuilder.DropTable(
                name: "RestaurantWorkingHour");

            migrationBuilder.DropTable(
                name: "Review");

            migrationBuilder.DropTable(
                name: "ScheduledOrders");

            migrationBuilder.DropTable(
                name: "ScheduledTask");

            migrationBuilder.DropTable(
                name: "SearchHistories");

            migrationBuilder.DropTable(
                name: "SellerDetail");

            migrationBuilder.DropTable(
                name: "SettlementItems");

            migrationBuilder.DropTable(
                name: "SupportActions");

            migrationBuilder.DropTable(
                name: "SupportMessages");

            migrationBuilder.DropTable(
                name: "Tips");

            migrationBuilder.DropTable(
                name: "UserCoupon");

            migrationBuilder.DropTable(
                name: "UserExternalInfos");

            migrationBuilder.DropTable(
                name: "BasketItemValue");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "DeliveryAssignment");

            migrationBuilder.DropTable(
                name: "OptionTemplateValueOption");

            migrationBuilder.DropTable(
                name: "MenuOptionValueOptionValue");

            migrationBuilder.DropTable(
                name: "OrderItemValues");

            migrationBuilder.DropTable(
                name: "ProductAttribute");

            migrationBuilder.DropTable(
                name: "SettlementPeriods");

            migrationBuilder.DropTable(
                name: "SupportTickets");

            migrationBuilder.DropTable(
                name: "Coupon");

            migrationBuilder.DropTable(
                name: "BasketItem");

            migrationBuilder.DropTable(
                name: "RestaurantCourierAgreement");

            migrationBuilder.DropTable(
                name: "OptionTemplateValue");

            migrationBuilder.DropTable(
                name: "MenuOptionValueOption");

            migrationBuilder.DropTable(
                name: "OrderItem");

            migrationBuilder.DropTable(
                name: "Seller");

            migrationBuilder.DropTable(
                name: "Basket");

            migrationBuilder.DropTable(
                name: "Courier");

            migrationBuilder.DropTable(
                name: "MenuOptionValue");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "CourierCompany");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "MenuOption");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "Menu");

            migrationBuilder.DropTable(
                name: "OptionTemplate");

            migrationBuilder.DropTable(
                name: "Restaurant");
        }
    }
}
