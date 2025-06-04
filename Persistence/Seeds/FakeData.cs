using Base.Enums;
using Domain.Entities.Common;
using Domain.Entities.Seller;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Seeds;

public class FakeData
{
    private readonly Guid UserId = new("67d10056-c978-4e93-89d6-ab078cbab543");
    private readonly Guid SellerId = new("bab60c66-11df-4c2d-8fc3-b8702664d9cf");
    private readonly Guid RestaurantId = new("3a4d6ba2-593d-4f28-a2ce-89fbbb7fc811");

    private List<User>? _users;
    private List<Restaurant>? _restaurants;
    private List<Product>? _products;
    private List<Menu>? _menus;
    private List<MenuOption>? _menuOptions;
    private List<MenuOptionValue>? _menuOptionsValues;
    private List<MenuOptionValueOption>? _menuOptionsValueOptions;
    private List<MenuOptionValueOptionValue>? _menuOptionsValueOptionValues;
    private List<Category>? _categories;
    private List<CategoryDetail>? _categoryDetails;

    public void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(SeedUsers);
        modelBuilder.Entity<Seller>(SeedSellers);
        modelBuilder.Entity<Restaurant>(SeedRestaurant);
        modelBuilder.Entity<Address>(SeedAddress);

        //modelBuilder.Entity<Product>(SeedProducts);
        //modelBuilder.Entity<Menu>(SeedMenus);
        //modelBuilder.Entity<MenuOption>(SeedMenuOptions);
        //modelBuilder.Entity<MenuOptionValue>(SeedMenuOptionValues);
        //modelBuilder.Entity<MenuOptionValueOption>(SeedMenuOptionValueOptions);
        //modelBuilder.Entity<MenuOptionValueOptionValue>(SeedMenuOptionValueOptionValues);
        //modelBuilder.Entity<Category>(SeedCategory);
        //modelBuilder.Entity<CategoryDetail>(SeedCategoryDetail);
    }

    private void SeedUsers(EntityTypeBuilder<User> builder)
    {
        _users =
        [
            //Admin
            new User
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                UserRoleId = (short)AuthorizationServiceEnums.UserRoleEnums.Admin,
                UserStatusId = (short)AuthorizationServiceEnums.UserStatusEnums.Active,
                Email = "admin@esnaftan.com",
                Password = "xwmnabLZ756bcYU+Nn9V3Q==",
                FirstName = "Esnaftan",
                LastName = "Admin",
                BirthDate = new DateTime(1994, 2, 1),
                SexId = (short)AuthorizationServiceEnums.SexEnums.Male,
                PhoneNumber = "05519684748",
                ActivationKey = Guid.NewGuid()
            },
            //Seller Admin
            new User
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                UserRoleId = (short)AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
                UserStatusId = (short)AuthorizationServiceEnums.UserStatusEnums.Active,
                Email = "info@pizzaci.com",
                Password = "xwmnabLZ756bcYU+Nn9V3Q==",
                FirstName = "Pizzacı",
                LastName = "Ahmet",
                BirthDate = new DateTime(1994, 2, 1),
                SexId = (short)AuthorizationServiceEnums.SexEnums.Male,
                PhoneNumber = "05519684748",
                ActivationKey = Guid.NewGuid(),
                SellerId = SellerId
            },
            //User
            new User
            {
                Id = UserId,
                CreatedDate = DateTime.Now,
                UserRoleId = (short)AuthorizationServiceEnums.UserRoleEnums.User,
                UserStatusId = (short)AuthorizationServiceEnums.UserStatusEnums.Active,
                Email = "alici@gmail.com",
                Password = "xwmnabLZ756bcYU+Nn9V3Q==",
                FirstName = "Alıcı",
                LastName = "Mehmet",
                BirthDate = new DateTime(1994, 2, 1),
                SexId = (short)AuthorizationServiceEnums.SexEnums.Male,
                PhoneNumber = "05556667788",
                ActivationKey = Guid.NewGuid(),
            }
        ];

        builder.HasData(_users);
    }

    private void SeedSellers(EntityTypeBuilder<Seller> builder)
    {
        Seller[] seeds =
        [
            new()
            {
                Id = SellerId,
                CreatedDate = DateTime.Now,
                CompanyType = (short)AuthorizationServiceEnums.CompanyTypeEnums.Company,
                CompanyStatus = (short)AuthorizationServiceEnums.CompanyStatusEnums.Pending,
                Name = "Pizzacı Ahmet",
                LegalName = "Pizzacı Ahmet Ltd. Şti.",
                TaxCode = "1234567890",
                TaxArea = "Nilüfer",
                IBAN = "TR260006266822193294982978",
                IsEInvoiceAvaible = true
            }
        ];
        builder.HasData(seeds);
    }

    private void SeedRestaurant(EntityTypeBuilder<Restaurant> builder)
    {
        _restaurants =
        [
            new Restaurant
            {
                Id = RestaurantId,
                SellerId = SellerId,
                Name = "Pizzacı Ahmet",
                Phone = "05551112233",
                Email = "a@a.com",
                MinimumOrderPrice = 250,
                MinDeliveryTime = 25,
                MaxDeliveryTime = 45,
                CoverImage = "https://cdn.getiryemek.com/restaurants/1741075067957_1125x522.webp",
                Description = "Süper lezzetli pizzalar, hızlı teslimat!",
            }
        ];

        builder.HasData(_restaurants);
    }

    private void SeedAddress(EntityTypeBuilder<Address> builder)
    {
        Address[] seeds =
        [
            //Satıcı Adresi
            new()
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                SellerId = SellerId,
                RestaurantId = RestaurantId,
                AddressType = (short)AuthorizationServiceEnums.AddressTypeEnums.Delivery,
                AddressName = "Gönderim Adresi",
                FirstName = "Pizzacı",
                LastName = "Ahmet",
                Phone = "05551112233",
                CityId = Guid.Parse("5d0c385c-810d-4dd6-9462-259183584992"),
                TownId = Guid.Parse("342b6d4d-42bf-4085-92e7-8d2b51de130a"),
                NeighbourhoodId = Guid.Parse("1c2d8a14-38df-448c-8125-14535bf0b7b3"),
                AddressLine1 = "Ahmet Yesevi, Bey Sk. No:4/B",
                AddressLine2 = null,
                Latitude = "40.267317071584884",
                Longitude = "28.9391322447786",
                IsDefault = true
            },
            //Alıcı Adresi
            new()
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                UserId = UserId,
                AddressType = (short)AuthorizationServiceEnums.AddressTypeEnums.Shipping,
                AddressName = "Teslimat Adresi",
                FirstName = "Alıcı",
                LastName = "Mehmet",
                Phone = "05556667788",
                CityId = Guid.Parse("5d0c385c-810d-4dd6-9462-259183584992"),
                TownId = Guid.Parse("342b6d4d-42bf-4085-92e7-8d2b51de130a"),
                NeighbourhoodId = Guid.Parse("1c2d8a14-38df-448c-8125-14535bf0b7b3"),
                AddressLine1 = "Geçit Mah. 1. Begonya Sok. No: 57 Daire: 6",
                AddressLine2 = "Oliva Sitesi B Blok",
                Latitude = "40.26587386663734",
                Longitude = "28.9617998",
                IsDefault = true,
                InvoiceType = (short)AuthorizationServiceEnums.InvoiceTypeEnums.Personal
            }
        ];
        builder.HasData(seeds);
    }

    private void SeedProducts(EntityTypeBuilder<Product> builder)
    {
        _products =
        [
            new Product
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                RestaurantId = this.RestaurantId,
                Name = "Bol Bol Pizza - Orta Boy",
                ProductType = 1,
                Description = string.Empty,
                OrderIndex = 0
            },
            new Product
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                RestaurantId = this.RestaurantId,
                Name = "Mantar",
                ProductType = 2,
                Description = string.Empty,
                OrderIndex = 1
            },
            new Product
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                RestaurantId = this.RestaurantId,
                Name = "Sucuk",
                ProductType = 2,
                Description = string.Empty,
                OrderIndex = 1
            }
        ];

        builder.HasData(_products);
    }

    private void SeedMenus(EntityTypeBuilder<Menu> builder)
    {
        _menus =
        [
            new Menu
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                RestaurantId = this.RestaurantId,
                Name = "Orta Boy Pizzalar",
                Description = string.Empty,
                Price = 100,
                OrderIndex = 0
            }
        ];

        builder.HasData(_menus);
    }

    private void SeedMenuOptions(EntityTypeBuilder<MenuOption> builder)
    {
        _menuOptions =
        [
            new MenuOption
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                MenuId = _menus[0].Id,
                Name = "Pizza Tercihi",
                Description = string.Empty,
                MinCount = 1,
                MaxCount = 1,
                OrderIndex = 0
            }
        ];

        builder.HasData(_menuOptions);
    }

    private void SeedMenuOptionValues(EntityTypeBuilder<MenuOptionValue> builder)
    {
        _menuOptionsValues =
        [
            new MenuOptionValue
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                MenuOptionId = _menuOptions[0].Id,
                ProductId = _products[0].Id,
                Price = 0,
                OrderIndex = 0
            }
        ];

        builder.HasData(_menuOptionsValues);
    }

    private void SeedMenuOptionValueOptions(EntityTypeBuilder<MenuOptionValueOption> builder)
    {
        _menuOptionsValueOptions =
        [
            new MenuOptionValueOption
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                MenuOptionValueId = _menuOptionsValues[0].Id,
                Name = "Çıkarılacak Malzemeler",
                Description = null,
                MinCount = 0,
                MaxCount = 2,
                OrderIndex = 0
            }
        ];

        builder.HasData(_menuOptionsValueOptions);
    }

    private void SeedMenuOptionValueOptionValues(EntityTypeBuilder<MenuOptionValueOptionValue> builder)
    {
        _menuOptionsValueOptionValues =
        [
            new MenuOptionValueOptionValue
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                MenuOptionValueOptionId = _menuOptionsValueOptions[0].Id,
                ProductId = _products[1].Id,
                Price = 0,
                OrderIndex = 0
            },
            new MenuOptionValueOptionValue
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                MenuOptionValueOptionId = _menuOptionsValueOptions[0].Id,
                ProductId = _products[2].Id,
                Price = 0,
                OrderIndex = 1
            }
        ];

        builder.HasData(_menuOptionsValueOptionValues);
    }

    private void SeedCategory(EntityTypeBuilder<Category> builder)
    {
        _categories =
        [
            new()
            {
                Id = Guid.NewGuid(),
                RestaurantId = this.RestaurantId,
                CreatedDate = DateTime.Now,
                Name = "Pizzalar",
                OrderIndex = 0
            }

        ];

        builder.HasData(_categories);
    }

    private void SeedCategoryDetail(EntityTypeBuilder<CategoryDetail> builder)
    {
        _categoryDetails =
        [
            new()
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                CategoryId = _categories[0].Id,
                MenuId = _menus[0].Id,
                OrderIndex = 0
            }
        ];

        builder.HasData(_categoryDetails);
    }
}