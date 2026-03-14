using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class _0017 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Address",
                keyColumn: "Id",
                keyValue: new Guid("d6012e04-9070-4841-b816-f798d61a9616"));

            migrationBuilder.DeleteData(
                table: "Address",
                keyColumn: "Id",
                keyValue: new Guid("f76bf318-7350-4e5d-8bae-f12fbd835fd3"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("ea162dc7-8961-45b8-a57e-bb9e3a13c9eb"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("ef0a2700-8551-4159-bdae-2e4525f5c7f0"));

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
                columns: new[] { "ActivationKey", "CreatedDate" },
                values: new object[] { new Guid("6c88cdc4-a3b3-41bc-b957-a67773555188"), new DateTime(2026, 3, 15, 0, 17, 38, 245, DateTimeKind.Local).AddTicks(1180) });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "ActivationKey", "BirthDate", "CardUserKey", "CreatedDate", "DeletedDate", "Email", "FirstName", "LastName", "Password", "PhoneNumber", "SellerId", "SexId", "UpdatedDate", "UserRoleId", "UserStatusId" },
                values: new object[,]
                {
                    { new Guid("cd25b2a0-6854-48a4-b505-623635b5c587"), new Guid("bf487616-9267-425e-8066-ab710766f68a"), new DateTime(1994, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2026, 3, 15, 0, 17, 38, 245, DateTimeKind.Local).AddTicks(950), null, "info@pizzaci.com", "Pizzacı", "Ahmet", "$2a$12$FSkQpNFCoggkjDbhmQIKLuk2XIF6GF0lCW7nPK7vbJPsV91.zdvzW", "05519684748", new Guid("bab60c66-11df-4c2d-8fc3-b8702664d9cf"), (short)1, null, (short)4, (short)1 },
                    { new Guid("e0f1f845-ca76-4e9e-a345-7dae183c8b3a"), new Guid("4f292a87-0406-4245-8f9d-bd9106fb7d71"), new DateTime(1994, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2026, 3, 15, 0, 17, 38, 187, DateTimeKind.Local).AddTicks(5230), null, "admin@esnaftan.com", "Esnaftan", "Admin", "$2a$12$FSkQpNFCoggkjDbhmQIKLuk2XIF6GF0lCW7nPK7vbJPsV91.zdvzW", "05519684748", null, (short)1, null, (short)1, (short)1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "Address",
                columns: new[] { "Id", "AddressLine1", "AddressLine2", "AddressName", "AddressType", "CityId", "CreatedDate", "DeletedDate", "FirstName", "InvoiceType", "IsDefault", "LastName", "Latitude", "Longitude", "NeighbourhoodId", "Phone", "RestaurantId", "SellerId", "TaxArea", "TaxCode", "TownId", "UpdatedDate", "UserId" },
                values: new object[,]
                {
                    { new Guid("d6012e04-9070-4841-b816-f798d61a9616"), "Geçit Mah. 1. Begonya Sok. No: 57 Daire: 6", "Oliva Sitesi B Blok", "Teslimat Adresi", (short)2, new Guid("5d0c385c-810d-4dd6-9462-259183584992"), new DateTime(2026, 3, 15, 0, 8, 3, 772, DateTimeKind.Local).AddTicks(6490), null, "Alıcı", (short)1, true, "Mehmet", "40.26587386663734", "28.9617998", new Guid("1c2d8a14-38df-448c-8125-14535bf0b7b3"), "05556667788", null, null, null, null, new Guid("342b6d4d-42bf-4085-92e7-8d2b51de130a"), null, new Guid("67d10056-c978-4e93-89d6-ab078cbab543") },
                    { new Guid("f76bf318-7350-4e5d-8bae-f12fbd835fd3"), "Ahmet Yesevi, Bey Sk. No:4/B", null, "Gönderim Adresi", (short)4, new Guid("5d0c385c-810d-4dd6-9462-259183584992"), new DateTime(2026, 3, 15, 0, 8, 3, 772, DateTimeKind.Local).AddTicks(4300), null, "Pizzacı", null, true, "Ahmet", "40.267317071584884", "28.9391322447786", new Guid("1c2d8a14-38df-448c-8125-14535bf0b7b3"), "05551112233", new Guid("3a4d6ba2-593d-4f28-a2ce-89fbbb7fc811"), new Guid("bab60c66-11df-4c2d-8fc3-b8702664d9cf"), null, null, new Guid("342b6d4d-42bf-4085-92e7-8d2b51de130a"), null, null }
                });

            migrationBuilder.UpdateData(
                table: "Seller",
                keyColumn: "Id",
                keyValue: new Guid("bab60c66-11df-4c2d-8fc3-b8702664d9cf"),
                column: "CreatedDate",
                value: new DateTime(2026, 3, 15, 0, 8, 3, 766, DateTimeKind.Local).AddTicks(3890));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("67d10056-c978-4e93-89d6-ab078cbab543"),
                columns: new[] { "ActivationKey", "CreatedDate" },
                values: new object[] { new Guid("45ea4212-6c78-41f2-8d18-18376dbda200"), new DateTime(2026, 3, 15, 0, 8, 3, 765, DateTimeKind.Local).AddTicks(9070) });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "ActivationKey", "BirthDate", "CardUserKey", "CreatedDate", "DeletedDate", "Email", "FirstName", "LastName", "Password", "PhoneNumber", "SellerId", "SexId", "UpdatedDate", "UserRoleId", "UserStatusId" },
                values: new object[,]
                {
                    { new Guid("ea162dc7-8961-45b8-a57e-bb9e3a13c9eb"), new Guid("b5477e9b-aa1b-4484-96a3-54a3a110183d"), new DateTime(1994, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2026, 3, 15, 0, 8, 3, 759, DateTimeKind.Local).AddTicks(5450), null, "admin@esnaftan.com", "Esnaftan", "Admin", "$2a$12$FSkQpNFCoggkjDbhmQIKLuk2XIF6GF0lCW7nPK7vbJPsV91.zdvzW", "05519684748", null, (short)1, null, (short)1, (short)1 },
                    { new Guid("ef0a2700-8551-4159-bdae-2e4525f5c7f0"), new Guid("7b0349cd-d114-4a04-b4a3-a082fcb573bc"), new DateTime(1994, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2026, 3, 15, 0, 8, 3, 765, DateTimeKind.Local).AddTicks(8900), null, "info@pizzaci.com", "Pizzacı", "Ahmet", "$2a$12$FSkQpNFCoggkjDbhmQIKLuk2XIF6GF0lCW7nPK7vbJPsV91.zdvzW", "05519684748", new Guid("bab60c66-11df-4c2d-8fc3-b8702664d9cf"), (short)1, null, (short)4, (short)1 }
                });
        }
    }
}
