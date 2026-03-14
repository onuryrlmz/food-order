using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class _0043 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Address",
                keyColumn: "Id",
                keyValue: new Guid("76b9e4c4-806e-4571-9087-48d619b0a257"));

            migrationBuilder.DeleteData(
                table: "Address",
                keyColumn: "Id",
                keyValue: new Guid("f5e3aa49-6151-440f-a727-50fc383aba6d"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("cd25b2a0-6854-48a4-b505-623635b5c587"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("e0f1f845-ca76-4e9e-a345-7dae183c8b3a"));

            migrationBuilder.DropColumn(
                name: "CardUserKey",
                table: "User");

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

            migrationBuilder.InsertData(
                table: "Address",
                columns: new[] { "Id", "AddressLine1", "AddressLine2", "AddressName", "AddressType", "CityId", "CreatedDate", "DeletedDate", "FirstName", "InvoiceType", "IsDefault", "LastName", "Latitude", "Longitude", "NeighbourhoodId", "Phone", "RestaurantId", "SellerId", "TaxArea", "TaxCode", "TownId", "UpdatedDate", "UserId" },
                values: new object[,]
                {
                    { new Guid("3807c93f-b0e0-4f80-9cc7-0518d040bdb2"), "Ahmet Yesevi, Bey Sk. No:4/B", null, "Gönderim Adresi", (short)4, new Guid("5d0c385c-810d-4dd6-9462-259183584992"), new DateTime(2026, 3, 15, 0, 43, 29, 39, DateTimeKind.Local).AddTicks(560), null, "Pizzacı", null, true, "Ahmet", "40.267317071584884", "28.9391322447786", new Guid("1c2d8a14-38df-448c-8125-14535bf0b7b3"), "05551112233", new Guid("3a4d6ba2-593d-4f28-a2ce-89fbbb7fc811"), new Guid("bab60c66-11df-4c2d-8fc3-b8702664d9cf"), null, null, new Guid("342b6d4d-42bf-4085-92e7-8d2b51de130a"), null, null },
                    { new Guid("fed9e08c-52d7-4f6a-8eea-a6f7a9921396"), "Geçit Mah. 1. Begonya Sok. No: 57 Daire: 6", "Oliva Sitesi B Blok", "Teslimat Adresi", (short)2, new Guid("5d0c385c-810d-4dd6-9462-259183584992"), new DateTime(2026, 3, 15, 0, 43, 29, 39, DateTimeKind.Local).AddTicks(3040), null, "Alıcı", (short)1, true, "Mehmet", "40.26587386663734", "28.9617998", new Guid("1c2d8a14-38df-448c-8125-14535bf0b7b3"), "05556667788", null, null, null, null, new Guid("342b6d4d-42bf-4085-92e7-8d2b51de130a"), null, new Guid("67d10056-c978-4e93-89d6-ab078cbab543") }
                });

            migrationBuilder.UpdateData(
                table: "Seller",
                keyColumn: "Id",
                keyValue: new Guid("bab60c66-11df-4c2d-8fc3-b8702664d9cf"),
                column: "CreatedDate",
                value: new DateTime(2026, 3, 15, 0, 43, 29, 31, DateTimeKind.Local).AddTicks(8780));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("67d10056-c978-4e93-89d6-ab078cbab543"),
                columns: new[] { "ActivationKey", "CreatedDate" },
                values: new object[] { new Guid("c1874e60-0191-456f-aaf6-81c80b854431"), new DateTime(2026, 3, 15, 0, 43, 29, 30, DateTimeKind.Local).AddTicks(9890) });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "ActivationKey", "BirthDate", "CreatedDate", "DeletedDate", "Email", "FirstName", "LastName", "Password", "PhoneNumber", "SellerId", "SexId", "UpdatedDate", "UserRoleId", "UserStatusId" },
                values: new object[,]
                {
                    { new Guid("c6824012-2e9a-476c-b472-08c7caab16bc"), new Guid("8bec984b-9d90-4134-b545-d215924751ee"), new DateTime(1994, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 15, 0, 43, 29, 23, DateTimeKind.Local).AddTicks(4920), null, "admin@esnaftan.com", "Esnaftan", "Admin", "$2a$12$FSkQpNFCoggkjDbhmQIKLuk2XIF6GF0lCW7nPK7vbJPsV91.zdvzW", "05519684748", null, (short)1, null, (short)1, (short)1 },
                    { new Guid("e2035b44-07f0-4de2-bd60-6ae3f684741c"), new Guid("f6ee3eff-a367-4141-b48c-e79f1e239e47"), new DateTime(1994, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 15, 0, 43, 29, 30, DateTimeKind.Local).AddTicks(9680), null, "info@pizzaci.com", "Pizzacı", "Ahmet", "$2a$12$FSkQpNFCoggkjDbhmQIKLuk2XIF6GF0lCW7nPK7vbJPsV91.zdvzW", "05519684748", new Guid("bab60c66-11df-4c2d-8fc3-b8702664d9cf"), (short)1, null, (short)4, (short)1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserExternalInfos_UserId",
                table: "UserExternalInfos",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserExternalInfos");

            migrationBuilder.DeleteData(
                table: "Address",
                keyColumn: "Id",
                keyValue: new Guid("3807c93f-b0e0-4f80-9cc7-0518d040bdb2"));

            migrationBuilder.DeleteData(
                table: "Address",
                keyColumn: "Id",
                keyValue: new Guid("fed9e08c-52d7-4f6a-8eea-a6f7a9921396"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("c6824012-2e9a-476c-b472-08c7caab16bc"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("e2035b44-07f0-4de2-bd60-6ae3f684741c"));

            migrationBuilder.AddColumn<string>(
                name: "CardUserKey",
                table: "User",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Address",
                columns: new[] { "Id", "AddressLine1", "AddressLine2", "AddressName", "AddressType", "CityId", "CreatedDate", "DeletedDate", "FirstName", "InvoiceType", "IsDefault", "LastName", "Latitude", "Longitude", "NeighbourhoodId", "Phone", "RestaurantId", "SellerId", "TaxArea", "TaxCode", "TownId", "UpdatedDate", "UserId" },
                values: new object[,]
                {
                    { new Guid("76b9e4c4-806e-4571-9087-48d619b0a257"), "Ahmet Yesevi, Bey Sk. No:4/B", null, "Gönderim Adresi", (short)4, new Guid("5d0c385c-810d-4dd6-9462-259183584992"), new DateTime(2026, 3, 15, 0, 17, 38, 261, DateTimeKind.Local).AddTicks(1080), null, "Pizzacı", null, true, "Ahmet", "40.267317071584884", "28.9391322447786", new Guid("1c2d8a14-38df-448c-8125-14535bf0b7b3"), "05551112233", new Guid("3a4d6ba2-593d-4f28-a2ce-89fbbb7fc811"), new Guid("bab60c66-11df-4c2d-8fc3-b8702664d9cf"), null, null, new Guid("342b6d4d-42bf-4085-92e7-8d2b51de130a"), null, null },
                    { new Guid("f5e3aa49-6151-440f-a727-50fc383aba6d"), "Geçit Mah. 1. Begonya Sok. No: 57 Daire: 6", "Oliva Sitesi B Blok", "Teslimat Adresi", (short)2, new Guid("5d0c385c-810d-4dd6-9462-259183584992"), new DateTime(2026, 3, 15, 0, 17, 38, 261, DateTimeKind.Local).AddTicks(3960), null, "Alıcı", (short)1, true, "Mehmet", "40.26587386663734", "28.9617998", new Guid("1c2d8a14-38df-448c-8125-14535bf0b7b3"), "05556667788", null, null, null, null, new Guid("342b6d4d-42bf-4085-92e7-8d2b51de130a"), null, new Guid("67d10056-c978-4e93-89d6-ab078cbab543") }
                });

            migrationBuilder.UpdateData(
                table: "Seller",
                keyColumn: "Id",
                keyValue: new Guid("bab60c66-11df-4c2d-8fc3-b8702664d9cf"),
                column: "CreatedDate",
                value: new DateTime(2026, 3, 15, 0, 17, 38, 245, DateTimeKind.Local).AddTicks(7310));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("67d10056-c978-4e93-89d6-ab078cbab543"),
                columns: new[] { "ActivationKey", "CardUserKey", "CreatedDate" },
                values: new object[] { new Guid("6c88cdc4-a3b3-41bc-b957-a67773555188"), null, new DateTime(2026, 3, 15, 0, 17, 38, 245, DateTimeKind.Local).AddTicks(1180) });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "ActivationKey", "BirthDate", "CardUserKey", "CreatedDate", "DeletedDate", "Email", "FirstName", "LastName", "Password", "PhoneNumber", "SellerId", "SexId", "UpdatedDate", "UserRoleId", "UserStatusId" },
                values: new object[,]
                {
                    { new Guid("cd25b2a0-6854-48a4-b505-623635b5c587"), new Guid("bf487616-9267-425e-8066-ab710766f68a"), new DateTime(1994, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2026, 3, 15, 0, 17, 38, 245, DateTimeKind.Local).AddTicks(950), null, "info@pizzaci.com", "Pizzacı", "Ahmet", "$2a$12$FSkQpNFCoggkjDbhmQIKLuk2XIF6GF0lCW7nPK7vbJPsV91.zdvzW", "05519684748", new Guid("bab60c66-11df-4c2d-8fc3-b8702664d9cf"), (short)1, null, (short)4, (short)1 },
                    { new Guid("e0f1f845-ca76-4e9e-a345-7dae183c8b3a"), new Guid("4f292a87-0406-4245-8f9d-bd9106fb7d71"), new DateTime(1994, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2026, 3, 15, 0, 17, 38, 187, DateTimeKind.Local).AddTicks(5230), null, "admin@esnaftan.com", "Esnaftan", "Admin", "$2a$12$FSkQpNFCoggkjDbhmQIKLuk2XIF6GF0lCW7nPK7vbJPsV91.zdvzW", "05519684748", null, (short)1, null, (short)1, (short)1 }
                });
        }
    }
}
