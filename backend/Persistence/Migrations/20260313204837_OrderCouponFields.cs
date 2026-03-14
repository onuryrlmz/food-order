using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class OrderCouponFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Address",
                keyColumn: "Id",
                keyValue: new Guid("1f7fb4f9-0823-40b3-be79-37f4300c756a"));

            migrationBuilder.DeleteData(
                table: "Address",
                keyColumn: "Id",
                keyValue: new Guid("e9f20612-4b09-4fdc-bef1-f4ca906bd36d"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("d90adced-d4e6-4b53-8574-89241ba7ec49"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("de5ad48b-5863-4377-b781-850a21090887"));

            migrationBuilder.AlterColumn<string>(
                name: "CouponCode",
                table: "Order",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Address",
                columns: new[] { "Id", "AddressLine1", "AddressLine2", "AddressName", "AddressType", "CityId", "CreatedDate", "DeletedDate", "FirstName", "InvoiceType", "IsDefault", "LastName", "Latitude", "Longitude", "NeighbourhoodId", "Phone", "RestaurantId", "SellerId", "TaxArea", "TaxCode", "TownId", "UpdatedDate", "UserId" },
                values: new object[,]
                {
                    { new Guid("728ab7a7-3cfb-45ad-a920-a0bbe5719f15"), "Ahmet Yesevi, Bey Sk. No:4/B", null, "Gönderim Adresi", (short)4, new Guid("5d0c385c-810d-4dd6-9462-259183584992"), new DateTime(2026, 3, 13, 23, 48, 36, 557, DateTimeKind.Local).AddTicks(2420), null, "Pizzacı", null, true, "Ahmet", "40.267317071584884", "28.9391322447786", new Guid("1c2d8a14-38df-448c-8125-14535bf0b7b3"), "05551112233", new Guid("3a4d6ba2-593d-4f28-a2ce-89fbbb7fc811"), new Guid("bab60c66-11df-4c2d-8fc3-b8702664d9cf"), null, null, new Guid("342b6d4d-42bf-4085-92e7-8d2b51de130a"), null, null },
                    { new Guid("94b5afdf-edbc-4a0c-81f3-dba0f3afbab4"), "Geçit Mah. 1. Begonya Sok. No: 57 Daire: 6", "Oliva Sitesi B Blok", "Teslimat Adresi", (short)2, new Guid("5d0c385c-810d-4dd6-9462-259183584992"), new DateTime(2026, 3, 13, 23, 48, 36, 557, DateTimeKind.Local).AddTicks(5210), null, "Alıcı", (short)1, true, "Mehmet", "40.26587386663734", "28.9617998", new Guid("1c2d8a14-38df-448c-8125-14535bf0b7b3"), "05556667788", null, null, null, null, new Guid("342b6d4d-42bf-4085-92e7-8d2b51de130a"), null, new Guid("67d10056-c978-4e93-89d6-ab078cbab543") }
                });

            migrationBuilder.UpdateData(
                table: "Seller",
                keyColumn: "Id",
                keyValue: new Guid("bab60c66-11df-4c2d-8fc3-b8702664d9cf"),
                column: "CreatedDate",
                value: new DateTime(2026, 3, 13, 23, 48, 36, 550, DateTimeKind.Local).AddTicks(230));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("67d10056-c978-4e93-89d6-ab078cbab543"),
                columns: new[] { "ActivationKey", "CreatedDate" },
                values: new object[] { new Guid("53353295-ddfc-4430-a84e-0bf3afa970ce"), new DateTime(2026, 3, 13, 23, 48, 36, 549, DateTimeKind.Local).AddTicks(4250) });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "ActivationKey", "BirthDate", "CreatedDate", "DeletedDate", "Email", "FirstName", "LastName", "Password", "PhoneNumber", "SellerId", "SexId", "UpdatedDate", "UserRoleId", "UserStatusId" },
                values: new object[,]
                {
                    { new Guid("0869777d-0e11-4433-9c32-d042efeb3f23"), new Guid("7822052b-9cf7-4562-b08c-b1a9bb3a7dfc"), new DateTime(1994, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 13, 23, 48, 36, 541, DateTimeKind.Local).AddTicks(7210), null, "admin@esnaftan.com", "Esnaftan", "Admin", "$2a$12$FSkQpNFCoggkjDbhmQIKLuk2XIF6GF0lCW7nPK7vbJPsV91.zdvzW", "05519684748", null, (short)1, null, (short)1, (short)1 },
                    { new Guid("d73b23d1-e497-4c28-9df6-335d2058fb18"), new Guid("bb3e53b8-3afc-479c-be87-7b0f1ed17005"), new DateTime(1994, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 13, 23, 48, 36, 549, DateTimeKind.Local).AddTicks(4040), null, "info@pizzaci.com", "Pizzacı", "Ahmet", "$2a$12$FSkQpNFCoggkjDbhmQIKLuk2XIF6GF0lCW7nPK7vbJPsV91.zdvzW", "05519684748", new Guid("bab60c66-11df-4c2d-8fc3-b8702664d9cf"), (short)1, null, (short)4, (short)1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Address",
                keyColumn: "Id",
                keyValue: new Guid("728ab7a7-3cfb-45ad-a920-a0bbe5719f15"));

            migrationBuilder.DeleteData(
                table: "Address",
                keyColumn: "Id",
                keyValue: new Guid("94b5afdf-edbc-4a0c-81f3-dba0f3afbab4"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("0869777d-0e11-4433-9c32-d042efeb3f23"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("d73b23d1-e497-4c28-9df6-335d2058fb18"));

            migrationBuilder.AlterColumn<string>(
                name: "CouponCode",
                table: "Order",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Address",
                columns: new[] { "Id", "AddressLine1", "AddressLine2", "AddressName", "AddressType", "CityId", "CreatedDate", "DeletedDate", "FirstName", "InvoiceType", "IsDefault", "LastName", "Latitude", "Longitude", "NeighbourhoodId", "Phone", "RestaurantId", "SellerId", "TaxArea", "TaxCode", "TownId", "UpdatedDate", "UserId" },
                values: new object[,]
                {
                    { new Guid("1f7fb4f9-0823-40b3-be79-37f4300c756a"), "Geçit Mah. 1. Begonya Sok. No: 57 Daire: 6", "Oliva Sitesi B Blok", "Teslimat Adresi", (short)2, new Guid("5d0c385c-810d-4dd6-9462-259183584992"), new DateTime(2026, 3, 13, 23, 21, 34, 609, DateTimeKind.Local).AddTicks(710), null, "Alıcı", (short)1, true, "Mehmet", "40.26587386663734", "28.9617998", new Guid("1c2d8a14-38df-448c-8125-14535bf0b7b3"), "05556667788", null, null, null, null, new Guid("342b6d4d-42bf-4085-92e7-8d2b51de130a"), null, new Guid("67d10056-c978-4e93-89d6-ab078cbab543") },
                    { new Guid("e9f20612-4b09-4fdc-bef1-f4ca906bd36d"), "Ahmet Yesevi, Bey Sk. No:4/B", null, "Gönderim Adresi", (short)4, new Guid("5d0c385c-810d-4dd6-9462-259183584992"), new DateTime(2026, 3, 13, 23, 21, 34, 608, DateTimeKind.Local).AddTicks(8270), null, "Pizzacı", null, true, "Ahmet", "40.267317071584884", "28.9391322447786", new Guid("1c2d8a14-38df-448c-8125-14535bf0b7b3"), "05551112233", new Guid("3a4d6ba2-593d-4f28-a2ce-89fbbb7fc811"), new Guid("bab60c66-11df-4c2d-8fc3-b8702664d9cf"), null, null, new Guid("342b6d4d-42bf-4085-92e7-8d2b51de130a"), null, null }
                });

            migrationBuilder.UpdateData(
                table: "Seller",
                keyColumn: "Id",
                keyValue: new Guid("bab60c66-11df-4c2d-8fc3-b8702664d9cf"),
                column: "CreatedDate",
                value: new DateTime(2026, 3, 13, 23, 21, 34, 602, DateTimeKind.Local).AddTicks(1160));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("67d10056-c978-4e93-89d6-ab078cbab543"),
                columns: new[] { "ActivationKey", "CreatedDate" },
                values: new object[] { new Guid("1c598129-fb80-4fb2-a16e-7ec6d56da146"), new DateTime(2026, 3, 13, 23, 21, 34, 601, DateTimeKind.Local).AddTicks(5490) });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "ActivationKey", "BirthDate", "CreatedDate", "DeletedDate", "Email", "FirstName", "LastName", "Password", "PhoneNumber", "SellerId", "SexId", "UpdatedDate", "UserRoleId", "UserStatusId" },
                values: new object[,]
                {
                    { new Guid("d90adced-d4e6-4b53-8574-89241ba7ec49"), new Guid("96bcc3ec-fee0-47da-940f-9e73d1ac81ce"), new DateTime(1994, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 13, 23, 21, 34, 601, DateTimeKind.Local).AddTicks(5290), null, "info@pizzaci.com", "Pizzacı", "Ahmet", "$2a$12$FSkQpNFCoggkjDbhmQIKLuk2XIF6GF0lCW7nPK7vbJPsV91.zdvzW", "05519684748", new Guid("bab60c66-11df-4c2d-8fc3-b8702664d9cf"), (short)1, null, (short)4, (short)1 },
                    { new Guid("de5ad48b-5863-4377-b781-850a21090887"), new Guid("f3e91787-1515-4b4a-805c-1473b298407a"), new DateTime(1994, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 13, 23, 21, 34, 593, DateTimeKind.Local).AddTicks(6330), null, "admin@esnaftan.com", "Esnaftan", "Admin", "$2a$12$FSkQpNFCoggkjDbhmQIKLuk2XIF6GF0lCW7nPK7vbJPsV91.zdvzW", "05519684748", null, (short)1, null, (short)1, (short)1 }
                });
        }
    }
}
