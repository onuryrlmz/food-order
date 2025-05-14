using Base.Enums;
using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("Addresses").HasKey(c => c.Id);

        Address[] seeds =
        {
            #region Satıcının Adresleri

            new()
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                SellerId = Guid.Parse("bab60c66-11df-4c2d-8fc3-b8702664d9cf"),
                SellerBranchId = Guid.Parse("37a4b80b-e591-4281-93f3-1f650b0b34e0"),
                AddressType = (short)AuthorizationServiceEnums.AddressTypeEnums.Invoice,
                AddressName = "Fatura Adresi",
                FirstName = "Onur",
                LastName = "YORULMAZ",
                Phone = "05519684748",
                CityId = Guid.Parse("5d0c385c-810d-4dd6-9462-259183584992"),
                TownId = Guid.Parse("342b6d4d-42bf-4085-92e7-8d2b51de130a"),
                NeighbourhoodId = Guid.Parse("1c2d8a14-38df-448c-8125-14535bf0b7b3"),
                AddressLine1 = "Geçit Mah. 1. Bego Sok. No: 57 Daire: 6",
                IsDefault = true
            },
            new()
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                SellerId = Guid.Parse("bab60c66-11df-4c2d-8fc3-b8702664d9cf"),
                SellerBranchId = Guid.Parse("37a4b80b-e591-4281-93f3-1f650b0b34e0"),
                AddressType = (short)AuthorizationServiceEnums.AddressTypeEnums.Shipping,
                AddressName = "Fatura Adresi",
                FirstName = "Onur",
                LastName = "YORULMAZ",
                Phone = "05519684748",
                CityId = Guid.Parse("5d0c385c-810d-4dd6-9462-259183584992"),
                TownId = Guid.Parse("342b6d4d-42bf-4085-92e7-8d2b51de130a"),
                NeighbourhoodId = Guid.Parse("1c2d8a14-38df-448c-8125-14535bf0b7b3"),
                AddressLine1 = "Geçit Mah. 1. Bego Sok. No: 57 Daire: 6",
                IsDefault = true
            },
            new()
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                SellerId = Guid.Parse("bab60c66-11df-4c2d-8fc3-b8702664d9cf"),
                SellerBranchId = Guid.Parse("37a4b80b-e591-4281-93f3-1f650b0b34e0"),
                AddressType = (short)AuthorizationServiceEnums.AddressTypeEnums.Return,
                AddressName = "Fatura Adresi",
                FirstName = "Onur",
                LastName = "YORULMAZ",
                Phone = "05519684748",
                CityId = Guid.Parse("5d0c385c-810d-4dd6-9462-259183584992"),
                TownId = Guid.Parse("342b6d4d-42bf-4085-92e7-8d2b51de130a"),
                NeighbourhoodId = Guid.Parse("1c2d8a14-38df-448c-8125-14535bf0b7b3"),
                AddressLine1 = "Geçit Mah. 1. Bego Sok. No: 57 Daire: 6",
                IsDefault = true
            },

            #endregion

            #region Alıcının Adresleri

            new()
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                UserId = Guid.Parse("67d10056-c978-4e93-89d6-ab078cbab543"),
                AddressType = (short)AuthorizationServiceEnums.AddressTypeEnums.Invoice,
                AddressName = "Fatura Adresi",
                FirstName = "Onur",
                LastName = "YORULMAZ",
                Phone = "05519684748",
                CityId = Guid.Parse("5d0c385c-810d-4dd6-9462-259183584992"),
                TownId = Guid.Parse("342b6d4d-42bf-4085-92e7-8d2b51de130a"),
                NeighbourhoodId = Guid.Parse("1c2d8a14-38df-448c-8125-14535bf0b7b3"),
                AddressLine1 = "Geçit Mah. 1. Bego Sok. No: 57 Daire: 6",
                IsDefault = true
            },
            new()
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                UserId = Guid.Parse("67d10056-c978-4e93-89d6-ab078cbab543"),
                AddressType = (short)AuthorizationServiceEnums.AddressTypeEnums.Shipping,
                AddressName = "Fatura Adresi",
                FirstName = "Onur",
                LastName = "YORULMAZ",
                Phone = "05519684748",
                CityId = Guid.Parse("5d0c385c-810d-4dd6-9462-259183584992"),
                TownId = Guid.Parse("342b6d4d-42bf-4085-92e7-8d2b51de130a"),
                NeighbourhoodId = Guid.Parse("1c2d8a14-38df-448c-8125-14535bf0b7b3"),
                AddressLine1 = "Geçit Mah. 1. Bego Sok. No: 57 Daire: 6",
                IsDefault = true
            }

            #endregion
        };
        builder.HasData(seeds);
    }
}