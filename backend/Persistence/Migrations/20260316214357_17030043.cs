using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class _17030043 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<short>(
                name: "CourierStatusId",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

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

            migrationBuilder.InsertData(
                table: "Address",
                columns: new[] { "Id", "AddressLine1", "AddressLine2", "AddressName", "AddressType", "CityId", "CreatedDate", "DeletedDate", "FirstName", "InvoiceType", "IsDefault", "LastName", "Latitude", "Longitude", "NeighbourhoodId", "Phone", "RestaurantId", "SellerId", "TaxArea", "TaxCode", "TownId", "UpdatedDate", "UserId" },
                values: new object[,]
                {
                    { new Guid("21deccc9-a345-48f9-ba0a-9bc57b2e4c4e"), "Geçit Mah. 1. Begonya Sok. No: 57 Daire: 6", "Oliva Sitesi B Blok", "Teslimat Adresi", (short)2, new Guid("5d0c385c-810d-4dd6-9462-259183584992"), new DateTime(2026, 3, 17, 0, 43, 57, 21, DateTimeKind.Local).AddTicks(3690), null, "Alıcı", (short)1, true, "Mehmet", "40.26587386663734", "28.9617998", new Guid("1c2d8a14-38df-448c-8125-14535bf0b7b3"), "05556667788", null, null, null, null, new Guid("342b6d4d-42bf-4085-92e7-8d2b51de130a"), null, new Guid("67d10056-c978-4e93-89d6-ab078cbab543") },
                    { new Guid("e2ffe34f-03a1-4362-a886-7520d6f0e95b"), "Ahmet Yesevi, Bey Sk. No:4/B", null, "Gönderim Adresi", (short)4, new Guid("5d0c385c-810d-4dd6-9462-259183584992"), new DateTime(2026, 3, 17, 0, 43, 57, 21, DateTimeKind.Local).AddTicks(1160), null, "Pizzacı", null, true, "Ahmet", "40.267317071584884", "28.9391322447786", new Guid("1c2d8a14-38df-448c-8125-14535bf0b7b3"), "05551112233", new Guid("3a4d6ba2-593d-4f28-a2ce-89fbbb7fc811"), new Guid("bab60c66-11df-4c2d-8fc3-b8702664d9cf"), null, null, new Guid("342b6d4d-42bf-4085-92e7-8d2b51de130a"), null, null }
                });

            migrationBuilder.UpdateData(
                table: "Seller",
                keyColumn: "Id",
                keyValue: new Guid("bab60c66-11df-4c2d-8fc3-b8702664d9cf"),
                column: "CreatedDate",
                value: new DateTime(2026, 3, 17, 0, 43, 57, 13, DateTimeKind.Local).AddTicks(9290));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("67d10056-c978-4e93-89d6-ab078cbab543"),
                columns: new[] { "ActivationKey", "CourierStatusId", "CreatedDate" },
                values: new object[] { new Guid("f6d0f858-361b-4c20-bd49-722d1456544b"), (short)1, new DateTime(2026, 3, 17, 0, 43, 57, 13, DateTimeKind.Local).AddTicks(3150) });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "ActivationKey", "BirthDate", "CourierStatusId", "CreatedDate", "DeletedDate", "Email", "FirstName", "LastName", "Password", "PhoneNumber", "SellerId", "SexId", "UpdatedDate", "UserRoleId", "UserStatusId" },
                values: new object[,]
                {
                    { new Guid("513272bc-62cc-479d-b54b-b152e8e2409b"), new Guid("1148fe43-09ac-447f-ba86-7461d66b21ba"), new DateTime(1994, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), (short)1, new DateTime(2026, 3, 17, 0, 43, 57, 13, DateTimeKind.Local).AddTicks(2940), null, "info@pizzaci.com", "Pizzacı", "Ahmet", "$2a$12$FSkQpNFCoggkjDbhmQIKLuk2XIF6GF0lCW7nPK7vbJPsV91.zdvzW", "05519684748", new Guid("bab60c66-11df-4c2d-8fc3-b8702664d9cf"), (short)1, null, (short)4, (short)1 },
                    { new Guid("c03013b0-3fbb-4456-9f75-f15adb942309"), new Guid("e171181e-bf00-467f-95f1-ad99ba18d338"), new DateTime(1994, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), (short)1, new DateTime(2026, 3, 17, 0, 43, 57, 6, DateTimeKind.Local).AddTicks(150), null, "admin@esnaftan.com", "Esnaftan", "Admin", "$2a$12$FSkQpNFCoggkjDbhmQIKLuk2XIF6GF0lCW7nPK7vbJPsV91.zdvzW", "05519684748", null, (short)1, null, (short)1, (short)1 }
                });

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavoriteRestaurant");

            migrationBuilder.DropTable(
                name: "Review");

            migrationBuilder.DeleteData(
                table: "Address",
                keyColumn: "Id",
                keyValue: new Guid("21deccc9-a345-48f9-ba0a-9bc57b2e4c4e"));

            migrationBuilder.DeleteData(
                table: "Address",
                keyColumn: "Id",
                keyValue: new Guid("e2ffe34f-03a1-4362-a886-7520d6f0e95b"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("513272bc-62cc-479d-b54b-b152e8e2409b"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("c03013b0-3fbb-4456-9f75-f15adb942309"));

            migrationBuilder.DropColumn(
                name: "CourierStatusId",
                table: "User");

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
                column: "CreatedDate",
                value: new DateTime(2026, 3, 17, 0, 18, 50, 988, DateTimeKind.Local).AddTicks(180));

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
        }
    }
}
