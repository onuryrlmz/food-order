using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
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
                    Notes = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CancellationReason = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConfirmedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeliveredAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
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
                name: "SubscriptionPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PlanType = table.Column<short>(type: "smallint", nullable: false),
                    MonthlyPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaxRestaurants = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPlan", x => x.Id);
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
                name: "Subscription",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SellerId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RestaurantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SubscriptionPlanId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    StatusId = table.Column<short>(type: "smallint", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Notes = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscription", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscription_Restaurant_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subscription_Seller_SellerId",
                        column: x => x.SellerId,
                        principalTable: "Seller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subscription_SubscriptionPlan_SubscriptionPlanId",
                        column: x => x.SubscriptionPlanId,
                        principalTable: "SubscriptionPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.InsertData(
                table: "Restaurant",
                columns: new[] { "Id", "CoverImage", "CreatedDate", "DeletedDate", "Description", "Email", "IsActive", "IsOpen", "Latitude", "Longitude", "MaxDeliveryTime", "MinDeliveryTime", "MinimumOrderPrice", "Name", "Phone", "Rating", "RatingCount", "SellerId", "ServiceAreaPolygonWkt", "UpdatedDate" },
                values: new object[] { new Guid("3a4d6ba2-593d-4f28-a2ce-89fbbb7fc811"), "https://cdn.getiryemek.com/restaurants/1741075067957_1125x522.webp", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Süper lezzetli pizzalar, hızlı teslimat!", "a@a.com", false, false, null, null, 45, 25, 250m, "Pizzacı Ahmet", "05551112233", 0m, 0, new Guid("bab60c66-11df-4c2d-8fc3-b8702664d9cf"), null, null });

            migrationBuilder.InsertData(
                table: "Seller",
                columns: new[] { "Id", "ApiKey", "ApiSecret", "CompanyStatus", "CompanyType", "CreatedDate", "DeletedDate", "IBAN", "IsEInvoiceAvaible", "LegalName", "Name", "TaxArea", "TaxCode", "UpdatedDate" },
                values: new object[] { new Guid("bab60c66-11df-4c2d-8fc3-b8702664d9cf"), null, null, (short)1, (short)2, new DateTime(2026, 3, 12, 0, 33, 13, 131, DateTimeKind.Local).AddTicks(2150), null, "TR260006266822193294982978", true, "Pizzacı Ahmet Ltd. Şti.", "Pizzacı Ahmet", "Nilüfer", "1234567890", null });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "ActivationKey", "BirthDate", "CreatedDate", "DeletedDate", "Email", "FirstName", "LastName", "Password", "PhoneNumber", "SellerId", "SexId", "UpdatedDate", "UserRoleId", "UserStatusId" },
                values: new object[,]
                {
                    { new Guid("110a400e-0e1c-4df9-90d4-8e6a3394df2b"), new Guid("3a4b0b37-ed9f-4229-ab79-2c9ef35bbd0c"), new DateTime(1994, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 12, 0, 33, 13, 128, DateTimeKind.Local).AddTicks(9290), null, "info@pizzaci.com", "Pizzacı", "Ahmet", "$2a$12$FSkQpNFCoggkjDbhmQIKLuk2XIF6GF0lCW7nPK7vbJPsV91.zdvzW", "05519684748", new Guid("bab60c66-11df-4c2d-8fc3-b8702664d9cf"), (short)1, null, (short)4, (short)1 },
                    { new Guid("67d10056-c978-4e93-89d6-ab078cbab543"), new Guid("cee9f111-d19f-4ec1-bb76-ae33b7a370bb"), new DateTime(1994, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 12, 0, 33, 13, 128, DateTimeKind.Local).AddTicks(9490), null, "alici@gmail.com", "Alıcı", "Mehmet", "$2a$12$FSkQpNFCoggkjDbhmQIKLuk2XIF6GF0lCW7nPK7vbJPsV91.zdvzW", "05556667788", null, (short)1, null, (short)2, (short)1 },
                    { new Guid("770cd221-bcd5-4b45-a9ba-458f55f50859"), new Guid("7d5858f0-b090-4177-9eaf-1f0b9fde7137"), new DateTime(1994, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 12, 0, 33, 13, 120, DateTimeKind.Local).AddTicks(7610), null, "admin@esnaftan.com", "Esnaftan", "Admin", "$2a$12$FSkQpNFCoggkjDbhmQIKLuk2XIF6GF0lCW7nPK7vbJPsV91.zdvzW", "05519684748", null, (short)1, null, (short)1, (short)1 }
                });

            migrationBuilder.InsertData(
                table: "Address",
                columns: new[] { "Id", "AddressLine1", "AddressLine2", "AddressName", "AddressType", "CityId", "CreatedDate", "DeletedDate", "FirstName", "InvoiceType", "IsDefault", "LastName", "Latitude", "Longitude", "NeighbourhoodId", "Phone", "RestaurantId", "SellerId", "TaxArea", "TaxCode", "TownId", "UpdatedDate", "UserId" },
                values: new object[,]
                {
                    { new Guid("5a42a307-8da3-49ec-abe2-fd90dde9e64c"), "Geçit Mah. 1. Begonya Sok. No: 57 Daire: 6", "Oliva Sitesi B Blok", "Teslimat Adresi", (short)2, new Guid("5d0c385c-810d-4dd6-9462-259183584992"), new DateTime(2026, 3, 12, 0, 33, 13, 202, DateTimeKind.Local).AddTicks(3040), null, "Alıcı", (short)1, true, "Mehmet", "40.26587386663734", "28.9617998", new Guid("1c2d8a14-38df-448c-8125-14535bf0b7b3"), "05556667788", null, null, null, null, new Guid("342b6d4d-42bf-4085-92e7-8d2b51de130a"), null, new Guid("67d10056-c978-4e93-89d6-ab078cbab543") },
                    { new Guid("5ac17430-b672-40a1-9a34-8197e6c08a04"), "Ahmet Yesevi, Bey Sk. No:4/B", null, "Gönderim Adresi", (short)4, new Guid("5d0c385c-810d-4dd6-9462-259183584992"), new DateTime(2026, 3, 12, 0, 33, 13, 202, DateTimeKind.Local).AddTicks(490), null, "Pizzacı", null, true, "Ahmet", "40.267317071584884", "28.9391322447786", new Guid("1c2d8a14-38df-448c-8125-14535bf0b7b3"), "05551112233", new Guid("3a4d6ba2-593d-4f28-a2ce-89fbbb7fc811"), new Guid("bab60c66-11df-4c2d-8fc3-b8702664d9cf"), null, null, new Guid("342b6d4d-42bf-4085-92e7-8d2b51de130a"), null, null }
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
                name: "IX_RestaurantCdnUpdateQueue_StatusId_CreatedDate",
                table: "RestaurantCdnUpdateQueue",
                columns: new[] { "StatusId", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantWorkingHour_RestaurantId_DayOfWeek",
                table: "RestaurantWorkingHour",
                columns: new[] { "RestaurantId", "DayOfWeek" });

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_RestaurantId",
                table: "Subscription",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_SellerId",
                table: "Subscription",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_SubscriptionPlanId",
                table: "Subscription",
                column: "SubscriptionPlanId");
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
                name: "Cuisine");

            migrationBuilder.DropTable(
                name: "OptionTemplateValueOptionValue");

            migrationBuilder.DropTable(
                name: "OrderItem");

            migrationBuilder.DropTable(
                name: "ProductAttributeValue");

            migrationBuilder.DropTable(
                name: "RestaurantCdnUpdateQueue");

            migrationBuilder.DropTable(
                name: "RestaurantWorkingHour");

            migrationBuilder.DropTable(
                name: "ScheduledTask");

            migrationBuilder.DropTable(
                name: "SellerDetail");

            migrationBuilder.DropTable(
                name: "Subscription");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "BasketItemValue");

            migrationBuilder.DropTable(
                name: "MenuOptionValueOptionValue");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "OptionTemplateValueOption");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "ProductAttribute");

            migrationBuilder.DropTable(
                name: "Seller");

            migrationBuilder.DropTable(
                name: "SubscriptionPlan");

            migrationBuilder.DropTable(
                name: "BasketItem");

            migrationBuilder.DropTable(
                name: "MenuOptionValueOption");

            migrationBuilder.DropTable(
                name: "OptionTemplateValue");

            migrationBuilder.DropTable(
                name: "Basket");

            migrationBuilder.DropTable(
                name: "MenuOptionValue");

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
