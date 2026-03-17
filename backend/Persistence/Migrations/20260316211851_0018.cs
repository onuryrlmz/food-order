using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class _0018 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Address",
                keyColumn: "Id",
                keyValue: new Guid("9a3dd2e3-990f-4c74-a5e8-80ab4aae43bc"));

            migrationBuilder.DeleteData(
                table: "Address",
                keyColumn: "Id",
                keyValue: new Guid("fad0aa27-367d-4eb7-98d9-988a14a50bc1"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("a84fb8a0-947c-4075-8e0c-51fdef4e63bc"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("f684cf9c-7086-488d-9166-dd278802fcbd"));

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionRate",
                table: "SubscriptionPlan",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "MaxOrdersPerMonth",
                table: "SubscriptionPlan",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<short>(
                name: "OverageAction",
                table: "SubscriptionPlan",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<bool>(
                name: "AutoRenew",
                table: "Subscription",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastRenewalAttemptAt",
                table: "Subscription",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RenewalAttempts",
                table: "Subscription",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "IdentityNumber",
                table: "Seller",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "RefundReason",
                table: "Payments",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "RefundTransactionId",
                table: "Payments",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "RefundedAt",
                table: "Payments",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CourierCompanyId",
                table: "Order",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveredAt",
                table: "Order",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PickedUpAt",
                table: "Order",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PickedUpByCourierId",
                table: "Order",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "CourierCompany",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LegalName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TaxCode = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TaxArea = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ContactEmail = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ContactPhone = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OwnerUserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    StatusId = table.Column<short>(type: "smallint", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourierCompany", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourierCompany_User_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "SubscriptionUsage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SubscriptionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    OrderCount = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionUsage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscriptionUsage_Subscription_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CourierCompanyMember",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CourierCompanyId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CourierId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    StatusId = table.Column<short>(type: "smallint", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourierCompanyMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourierCompanyMember_CourierCompany_CourierCompanyId",
                        column: x => x.CourierCompanyId,
                        principalTable: "CourierCompany",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourierCompanyMember_User_CourierId",
                        column: x => x.CourierId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RestaurantCourierCompany",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RestaurantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CourierCompanyId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    StatusId = table.Column<short>(type: "smallint", nullable: false),
                    AgreementStartDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    AgreementEndDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantCourierCompany", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestaurantCourierCompany_CourierCompany_CourierCompanyId",
                        column: x => x.CourierCompanyId,
                        principalTable: "CourierCompany",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RestaurantCourierCompany_Restaurant_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Address",
                columns: new[] { "Id", "AddressLine1", "AddressLine2", "AddressName", "AddressType", "CityId", "CreatedDate", "DeletedDate", "FirstName", "InvoiceType", "IsDefault", "LastName", "Latitude", "Longitude", "NeighbourhoodId", "Phone", "RestaurantId", "SellerId", "TaxArea", "TaxCode", "TownId", "UpdatedDate", "UserId" },
                values: new object[,]
                {
                    { new Guid("601c5ac7-fe49-4877-9326-79a85ef8f872"), "Ahmet Yesevi, Bey Sk. No:4/B", null, "Gönderim Adresi", (short)4, new Guid("5d0c385c-810d-4dd6-9462-259183584992"), new DateTime(2026, 3, 17, 0, 18, 50, 996, DateTimeKind.Local).AddTicks(490), null, "Pizzacı", null, true, "Ahmet", "40.267317071584884", "28.9391322447786", new Guid("1c2d8a14-38df-448c-8125-14535bf0b7b3"), "05551112233", new Guid("3a4d6ba2-593d-4f28-a2ce-89fbbb7fc811"), new Guid("bab60c66-11df-4c2d-8fc3-b8702664d9cf"), null, null, new Guid("342b6d4d-42bf-4085-92e7-8d2b51de130a"), null, null },
                    { new Guid("9ff619ae-4f10-46ef-a4ab-e714de6420fa"), "Geçit Mah. 1. Begonya Sok. No: 57 Daire: 6", "Oliva Sitesi B Blok", "Teslimat Adresi", (short)2, new Guid("5d0c385c-810d-4dd6-9462-259183584992"), new DateTime(2026, 3, 17, 0, 18, 50, 996, DateTimeKind.Local).AddTicks(3220), null, "Alıcı", (short)1, true, "Mehmet", "40.26587386663734", "28.9617998", new Guid("1c2d8a14-38df-448c-8125-14535bf0b7b3"), "05556667788", null, null, null, null, new Guid("342b6d4d-42bf-4085-92e7-8d2b51de130a"), null, new Guid("67d10056-c978-4e93-89d6-ab078cbab543") }
                });

            migrationBuilder.UpdateData(
                table: "Seller",
                keyColumn: "Id",
                keyValue: new Guid("bab60c66-11df-4c2d-8fc3-b8702664d9cf"),
                columns: new[] { "CreatedDate", "IdentityNumber" },
                values: new object[] { new DateTime(2026, 3, 17, 0, 18, 50, 988, DateTimeKind.Local).AddTicks(180), null });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("67d10056-c978-4e93-89d6-ab078cbab543"),
                columns: new[] { "ActivationKey", "CreatedDate" },
                values: new object[] { new Guid("d20d42c0-8461-4243-b508-24f53921e8d5"), new DateTime(2026, 3, 17, 0, 18, 50, 986, DateTimeKind.Local).AddTicks(9920) });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "ActivationKey", "BirthDate", "CreatedDate", "DeletedDate", "Email", "FirstName", "LastName", "Password", "PhoneNumber", "SellerId", "SexId", "UpdatedDate", "UserRoleId", "UserStatusId" },
                values: new object[,]
                {
                    { new Guid("8b0a234a-207d-4634-864c-7784ecbf0bb0"), new Guid("39bdb659-f757-4e4f-87b6-1dd5b7cce39f"), new DateTime(1994, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 17, 0, 18, 50, 986, DateTimeKind.Local).AddTicks(9710), null, "info@pizzaci.com", "Pizzacı", "Ahmet", "$2a$12$FSkQpNFCoggkjDbhmQIKLuk2XIF6GF0lCW7nPK7vbJPsV91.zdvzW", "05519684748", new Guid("bab60c66-11df-4c2d-8fc3-b8702664d9cf"), (short)1, null, (short)4, (short)1 },
                    { new Guid("e2323a89-6d2c-41ad-95f9-41d9afcff9ff"), new Guid("cf7c9be1-54a9-44d3-ad45-b4f2a066cfa7"), new DateTime(1994, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 17, 0, 18, 50, 979, DateTimeKind.Local).AddTicks(5250), null, "admin@esnaftan.com", "Esnaftan", "Admin", "$2a$12$FSkQpNFCoggkjDbhmQIKLuk2XIF6GF0lCW7nPK7vbJPsV91.zdvzW", "05519684748", null, (short)1, null, (short)1, (short)1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Order_CourierCompanyId",
                table: "Order",
                column: "CourierCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CourierCompany_OwnerUserId",
                table: "CourierCompany",
                column: "OwnerUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourierCompanyMember_CourierCompanyId_CourierId_StatusId",
                table: "CourierCompanyMember",
                columns: new[] { "CourierCompanyId", "CourierId", "StatusId" });

            migrationBuilder.CreateIndex(
                name: "IX_CourierCompanyMember_CourierId",
                table: "CourierCompanyMember",
                column: "CourierId");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetToken_UserId",
                table: "PasswordResetToken",
                column: "UserId");

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
                name: "IX_RestaurantCourierCompany_CourierCompanyId",
                table: "RestaurantCourierCompany",
                column: "CourierCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantCourierCompany_RestaurantId_CourierCompanyId_Statu~",
                table: "RestaurantCourierCompany",
                columns: new[] { "RestaurantId", "CourierCompanyId", "StatusId" });

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionUsage_SubscriptionId_Year_Month",
                table: "SubscriptionUsage",
                columns: new[] { "SubscriptionId", "Year", "Month" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_CourierCompany_CourierCompanyId",
                table: "Order",
                column: "CourierCompanyId",
                principalTable: "CourierCompany",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_CourierCompany_CourierCompanyId",
                table: "Order");

            migrationBuilder.DropTable(
                name: "CourierCompanyMember");

            migrationBuilder.DropTable(
                name: "PasswordResetToken");

            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropTable(
                name: "RestaurantCourierCompany");

            migrationBuilder.DropTable(
                name: "SubscriptionUsage");

            migrationBuilder.DropTable(
                name: "CourierCompany");

            migrationBuilder.DropIndex(
                name: "IX_Order_CourierCompanyId",
                table: "Order");

            migrationBuilder.DeleteData(
                table: "Address",
                keyColumn: "Id",
                keyValue: new Guid("601c5ac7-fe49-4877-9326-79a85ef8f872"));

            migrationBuilder.DeleteData(
                table: "Address",
                keyColumn: "Id",
                keyValue: new Guid("9ff619ae-4f10-46ef-a4ab-e714de6420fa"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("8b0a234a-207d-4634-864c-7784ecbf0bb0"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("e2323a89-6d2c-41ad-95f9-41d9afcff9ff"));

            migrationBuilder.DropColumn(
                name: "CommissionRate",
                table: "SubscriptionPlan");

            migrationBuilder.DropColumn(
                name: "MaxOrdersPerMonth",
                table: "SubscriptionPlan");

            migrationBuilder.DropColumn(
                name: "OverageAction",
                table: "SubscriptionPlan");

            migrationBuilder.DropColumn(
                name: "AutoRenew",
                table: "Subscription");

            migrationBuilder.DropColumn(
                name: "LastRenewalAttemptAt",
                table: "Subscription");

            migrationBuilder.DropColumn(
                name: "RenewalAttempts",
                table: "Subscription");

            migrationBuilder.DropColumn(
                name: "IdentityNumber",
                table: "Seller");

            migrationBuilder.DropColumn(
                name: "RefundReason",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "RefundTransactionId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "RefundedAt",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CourierCompanyId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "DeliveredAt",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "PickedUpAt",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "PickedUpByCourierId",
                table: "Order");

            migrationBuilder.InsertData(
                table: "Address",
                columns: new[] { "Id", "AddressLine1", "AddressLine2", "AddressName", "AddressType", "CityId", "CreatedDate", "DeletedDate", "FirstName", "InvoiceType", "IsDefault", "LastName", "Latitude", "Longitude", "NeighbourhoodId", "Phone", "RestaurantId", "SellerId", "TaxArea", "TaxCode", "TownId", "UpdatedDate", "UserId" },
                values: new object[,]
                {
                    { new Guid("9a3dd2e3-990f-4c74-a5e8-80ab4aae43bc"), "Ahmet Yesevi, Bey Sk. No:4/B", null, "Gönderim Adresi", (short)4, new Guid("5d0c385c-810d-4dd6-9462-259183584992"), new DateTime(2026, 3, 15, 17, 30, 44, 146, DateTimeKind.Local).AddTicks(9880), null, "Pizzacı", null, true, "Ahmet", "40.267317071584884", "28.9391322447786", new Guid("1c2d8a14-38df-448c-8125-14535bf0b7b3"), "05551112233", new Guid("3a4d6ba2-593d-4f28-a2ce-89fbbb7fc811"), new Guid("bab60c66-11df-4c2d-8fc3-b8702664d9cf"), null, null, new Guid("342b6d4d-42bf-4085-92e7-8d2b51de130a"), null, null },
                    { new Guid("fad0aa27-367d-4eb7-98d9-988a14a50bc1"), "Geçit Mah. 1. Begonya Sok. No: 57 Daire: 6", "Oliva Sitesi B Blok", "Teslimat Adresi", (short)2, new Guid("5d0c385c-810d-4dd6-9462-259183584992"), new DateTime(2026, 3, 15, 17, 30, 44, 147, DateTimeKind.Local).AddTicks(2480), null, "Alıcı", (short)1, true, "Mehmet", "40.26587386663734", "28.9617998", new Guid("1c2d8a14-38df-448c-8125-14535bf0b7b3"), "05556667788", null, null, null, null, new Guid("342b6d4d-42bf-4085-92e7-8d2b51de130a"), null, new Guid("67d10056-c978-4e93-89d6-ab078cbab543") }
                });

            migrationBuilder.UpdateData(
                table: "Seller",
                keyColumn: "Id",
                keyValue: new Guid("bab60c66-11df-4c2d-8fc3-b8702664d9cf"),
                column: "CreatedDate",
                value: new DateTime(2026, 3, 15, 17, 30, 44, 139, DateTimeKind.Local).AddTicks(2550));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("67d10056-c978-4e93-89d6-ab078cbab543"),
                columns: new[] { "ActivationKey", "CreatedDate" },
                values: new object[] { new Guid("e08dcfd0-f4ba-4634-9aac-19d0fd6f467c"), new DateTime(2026, 3, 15, 17, 30, 44, 138, DateTimeKind.Local).AddTicks(6190) });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "ActivationKey", "BirthDate", "CreatedDate", "DeletedDate", "Email", "FirstName", "LastName", "Password", "PhoneNumber", "SellerId", "SexId", "UpdatedDate", "UserRoleId", "UserStatusId" },
                values: new object[,]
                {
                    { new Guid("a84fb8a0-947c-4075-8e0c-51fdef4e63bc"), new Guid("1858998e-4973-47ec-87fc-4d125bd8527a"), new DateTime(1994, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 15, 17, 30, 44, 130, DateTimeKind.Local).AddTicks(8920), null, "admin@esnaftan.com", "Esnaftan", "Admin", "$2a$12$FSkQpNFCoggkjDbhmQIKLuk2XIF6GF0lCW7nPK7vbJPsV91.zdvzW", "05519684748", null, (short)1, null, (short)1, (short)1 },
                    { new Guid("f684cf9c-7086-488d-9166-dd278802fcbd"), new Guid("2891fa77-ec96-498c-bbca-9e00315ef2c3"), new DateTime(1994, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 15, 17, 30, 44, 138, DateTimeKind.Local).AddTicks(5980), null, "info@pizzaci.com", "Pizzacı", "Ahmet", "$2a$12$FSkQpNFCoggkjDbhmQIKLuk2XIF6GF0lCW7nPK7vbJPsV91.zdvzW", "05519684748", new Guid("bab60c66-11df-4c2d-8fc3-b8702664d9cf"), (short)1, null, (short)4, (short)1 }
                });
        }
    }
}
