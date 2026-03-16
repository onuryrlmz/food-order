using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRestaurantCourierStatusFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RestaurantCourier_Phone",
                table: "RestaurantCourier");

            migrationBuilder.DropIndex(
                name: "IX_RestaurantCourier_RestaurantId_CourierId",
                table: "RestaurantCourier");

            migrationBuilder.DeleteData(
                table: "Address",
                keyColumn: "Id",
                keyValue: new Guid("52129533-b6fb-4fa9-bc25-610d7b46dcac"));

            migrationBuilder.DeleteData(
                table: "Address",
                keyColumn: "Id",
                keyValue: new Guid("71628373-e06b-4550-a8e6-ddb827dce1f8"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("b0c7501f-03eb-4950-be4b-2a92053d1a1a"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("edd03c77-a2c0-46e7-ac6b-f1c820af579f"));

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "RestaurantCourier");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "RestaurantCourier");

            migrationBuilder.RenameColumn(
                name: "VerifiedAt",
                table: "RestaurantCourier",
                newName: "AgreementStartDate");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "RestaurantCourier",
                newName: "AgreementEndDate");

            migrationBuilder.AddColumn<short>(
                name: "StatusId",
                table: "RestaurantCourier",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.InsertData(
                table: "Address",
                columns: new[] { "Id", "AddressLine1", "AddressLine2", "AddressName", "AddressType", "CityId", "CreatedDate", "DeletedDate", "FirstName", "InvoiceType", "IsDefault", "LastName", "Latitude", "Longitude", "NeighbourhoodId", "Phone", "RestaurantId", "SellerId", "TaxArea", "TaxCode", "TownId", "UpdatedDate", "UserId" },
                values: new object[,]
                {
                    { new Guid("b4104b3e-b821-4bea-a212-035fd0eb0e6f"), "Geçit Mah. 1. Begonya Sok. No: 57 Daire: 6", "Oliva Sitesi B Blok", "Teslimat Adresi", (short)2, new Guid("5d0c385c-810d-4dd6-9462-259183584992"), new DateTime(2026, 3, 15, 17, 14, 1, 905, DateTimeKind.Local).AddTicks(2180), null, "Alıcı", (short)1, true, "Mehmet", "40.26587386663734", "28.9617998", new Guid("1c2d8a14-38df-448c-8125-14535bf0b7b3"), "05556667788", null, null, null, null, new Guid("342b6d4d-42bf-4085-92e7-8d2b51de130a"), null, new Guid("67d10056-c978-4e93-89d6-ab078cbab543") },
                    { new Guid("c7f2bdd6-d57a-47cd-a207-b88946391712"), "Ahmet Yesevi, Bey Sk. No:4/B", null, "Gönderim Adresi", (short)4, new Guid("5d0c385c-810d-4dd6-9462-259183584992"), new DateTime(2026, 3, 15, 17, 14, 1, 904, DateTimeKind.Local).AddTicks(9670), null, "Pizzacı", null, true, "Ahmet", "40.267317071584884", "28.9391322447786", new Guid("1c2d8a14-38df-448c-8125-14535bf0b7b3"), "05551112233", new Guid("3a4d6ba2-593d-4f28-a2ce-89fbbb7fc811"), new Guid("bab60c66-11df-4c2d-8fc3-b8702664d9cf"), null, null, new Guid("342b6d4d-42bf-4085-92e7-8d2b51de130a"), null, null }
                });

            migrationBuilder.UpdateData(
                table: "Seller",
                keyColumn: "Id",
                keyValue: new Guid("bab60c66-11df-4c2d-8fc3-b8702664d9cf"),
                column: "CreatedDate",
                value: new DateTime(2026, 3, 15, 17, 14, 1, 897, DateTimeKind.Local).AddTicks(5620));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("67d10056-c978-4e93-89d6-ab078cbab543"),
                columns: new[] { "ActivationKey", "CreatedDate" },
                values: new object[] { new Guid("96495f61-e690-4f79-8c15-9cdda37e9b96"), new DateTime(2026, 3, 15, 17, 14, 1, 896, DateTimeKind.Local).AddTicks(9730) });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "ActivationKey", "BirthDate", "CreatedDate", "DeletedDate", "Email", "FirstName", "LastName", "Password", "PhoneNumber", "SellerId", "SexId", "UpdatedDate", "UserRoleId", "UserStatusId" },
                values: new object[,]
                {
                    { new Guid("e339af9c-f32b-4a4b-bf01-7dabff0af7f0"), new Guid("8e5054a0-b1bf-456c-9697-7be18fa19f70"), new DateTime(1994, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 15, 17, 14, 1, 889, DateTimeKind.Local).AddTicks(110), null, "admin@esnaftan.com", "Esnaftan", "Admin", "$2a$12$FSkQpNFCoggkjDbhmQIKLuk2XIF6GF0lCW7nPK7vbJPsV91.zdvzW", "05519684748", null, (short)1, null, (short)1, (short)1 },
                    { new Guid("f1bfd678-0d07-45c7-9b81-89fffaacbb34"), new Guid("6005d5a7-9fc9-4b64-9fe2-e1e7b03e74bb"), new DateTime(1994, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 15, 17, 14, 1, 896, DateTimeKind.Local).AddTicks(9530), null, "info@pizzaci.com", "Pizzacı", "Ahmet", "$2a$12$FSkQpNFCoggkjDbhmQIKLuk2XIF6GF0lCW7nPK7vbJPsV91.zdvzW", "05519684748", new Guid("bab60c66-11df-4c2d-8fc3-b8702664d9cf"), (short)1, null, (short)4, (short)1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantCourier_RestaurantId_CourierId_StatusId",
                table: "RestaurantCourier",
                columns: new[] { "RestaurantId", "CourierId", "StatusId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RestaurantCourier_RestaurantId_CourierId_StatusId",
                table: "RestaurantCourier");

            migrationBuilder.DeleteData(
                table: "Address",
                keyColumn: "Id",
                keyValue: new Guid("b4104b3e-b821-4bea-a212-035fd0eb0e6f"));

            migrationBuilder.DeleteData(
                table: "Address",
                keyColumn: "Id",
                keyValue: new Guid("c7f2bdd6-d57a-47cd-a207-b88946391712"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("e339af9c-f32b-4a4b-bf01-7dabff0af7f0"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("f1bfd678-0d07-45c7-9b81-89fffaacbb34"));

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "RestaurantCourier");

            migrationBuilder.RenameColumn(
                name: "AgreementStartDate",
                table: "RestaurantCourier",
                newName: "VerifiedAt");

            migrationBuilder.RenameColumn(
                name: "AgreementEndDate",
                table: "RestaurantCourier",
                newName: "DeletedAt");

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "RestaurantCourier",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "RestaurantCourier",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Address",
                columns: new[] { "Id", "AddressLine1", "AddressLine2", "AddressName", "AddressType", "CityId", "CreatedDate", "DeletedDate", "FirstName", "InvoiceType", "IsDefault", "LastName", "Latitude", "Longitude", "NeighbourhoodId", "Phone", "RestaurantId", "SellerId", "TaxArea", "TaxCode", "TownId", "UpdatedDate", "UserId" },
                values: new object[,]
                {
                    { new Guid("52129533-b6fb-4fa9-bc25-610d7b46dcac"), "Ahmet Yesevi, Bey Sk. No:4/B", null, "Gönderim Adresi", (short)4, new Guid("5d0c385c-810d-4dd6-9462-259183584992"), new DateTime(2026, 3, 15, 17, 9, 24, 816, DateTimeKind.Local).AddTicks(6990), null, "Pizzacı", null, true, "Ahmet", "40.267317071584884", "28.9391322447786", new Guid("1c2d8a14-38df-448c-8125-14535bf0b7b3"), "05551112233", new Guid("3a4d6ba2-593d-4f28-a2ce-89fbbb7fc811"), new Guid("bab60c66-11df-4c2d-8fc3-b8702664d9cf"), null, null, new Guid("342b6d4d-42bf-4085-92e7-8d2b51de130a"), null, null },
                    { new Guid("71628373-e06b-4550-a8e6-ddb827dce1f8"), "Geçit Mah. 1. Begonya Sok. No: 57 Daire: 6", "Oliva Sitesi B Blok", "Teslimat Adresi", (short)2, new Guid("5d0c385c-810d-4dd6-9462-259183584992"), new DateTime(2026, 3, 15, 17, 9, 24, 816, DateTimeKind.Local).AddTicks(9810), null, "Alıcı", (short)1, true, "Mehmet", "40.26587386663734", "28.9617998", new Guid("1c2d8a14-38df-448c-8125-14535bf0b7b3"), "05556667788", null, null, null, null, new Guid("342b6d4d-42bf-4085-92e7-8d2b51de130a"), null, new Guid("67d10056-c978-4e93-89d6-ab078cbab543") }
                });

            migrationBuilder.UpdateData(
                table: "Seller",
                keyColumn: "Id",
                keyValue: new Guid("bab60c66-11df-4c2d-8fc3-b8702664d9cf"),
                column: "CreatedDate",
                value: new DateTime(2026, 3, 15, 17, 9, 24, 809, DateTimeKind.Local).AddTicks(4000));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("67d10056-c978-4e93-89d6-ab078cbab543"),
                columns: new[] { "ActivationKey", "CreatedDate" },
                values: new object[] { new Guid("04a70ed6-d0a1-4685-b972-92ec872262e3"), new DateTime(2026, 3, 15, 17, 9, 24, 808, DateTimeKind.Local).AddTicks(7990) });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "ActivationKey", "BirthDate", "CreatedDate", "DeletedDate", "Email", "FirstName", "LastName", "Password", "PhoneNumber", "SellerId", "SexId", "UpdatedDate", "UserRoleId", "UserStatusId" },
                values: new object[,]
                {
                    { new Guid("b0c7501f-03eb-4950-be4b-2a92053d1a1a"), new Guid("73dffe97-8546-45b7-a43e-b15e17c83bf4"), new DateTime(1994, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 15, 17, 9, 24, 801, DateTimeKind.Local).AddTicks(2880), null, "admin@esnaftan.com", "Esnaftan", "Admin", "$2a$12$FSkQpNFCoggkjDbhmQIKLuk2XIF6GF0lCW7nPK7vbJPsV91.zdvzW", "05519684748", null, (short)1, null, (short)1, (short)1 },
                    { new Guid("edd03c77-a2c0-46e7-ac6b-f1c820af579f"), new Guid("e470e854-a2ff-43c4-ac9f-9b7c75688736"), new DateTime(1994, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 15, 17, 9, 24, 808, DateTimeKind.Local).AddTicks(7780), null, "info@pizzaci.com", "Pizzacı", "Ahmet", "$2a$12$FSkQpNFCoggkjDbhmQIKLuk2XIF6GF0lCW7nPK7vbJPsV91.zdvzW", "05519684748", new Guid("bab60c66-11df-4c2d-8fc3-b8702664d9cf"), (short)1, null, (short)4, (short)1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantCourier_Phone",
                table: "RestaurantCourier",
                column: "Phone");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantCourier_RestaurantId_CourierId",
                table: "RestaurantCourier",
                columns: new[] { "RestaurantId", "CourierId" },
                unique: true);
        }
    }
}
