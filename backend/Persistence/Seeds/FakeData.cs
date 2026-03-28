using Base.Enums;
using Domain.Entities.Buyer;
using Domain.Entities.Common;
using Domain.Entities.Courier;
using Domain.Entities.Seller;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Seeds;

public class FakeData
{
    // ===== FIXED GUIDs =====
    // Users
    private static readonly Guid AdminUserId = new("11111111-1111-1111-1111-111111111111");
    private static readonly Guid SellerUser1Id = new("22222222-2222-2222-2222-222222222222");
    private static readonly Guid SellerUser2Id = new("33333333-3333-3333-3333-333333333333");
    private static readonly Guid CustomerUser1Id = new("44444444-4444-4444-4444-444444444444");
    private static readonly Guid CustomerUser2Id = new("55555555-5555-5555-5555-555555555555");
    private static readonly Guid CourierUserId = new("66666666-6666-6666-6666-666666666666");
    private static readonly Guid CompanyAdminUserId = new("77777777-7777-7777-7777-777777777777");

    // Sellers
    private static readonly Guid Seller1Id = new("aaaa1111-1111-1111-1111-111111111111");
    private static readonly Guid Seller2Id = new("aaaa2222-2222-2222-2222-222222222222");

    // Restaurants
    private static readonly Guid Restaurant1Id = new("bbbb1111-1111-1111-1111-111111111111");
    private static readonly Guid Restaurant2Id = new("bbbb2222-2222-2222-2222-222222222222");
    private static readonly Guid Restaurant3Id = new("bbbb3333-3333-3333-3333-333333333333");

    // Cuisines
    private static readonly Guid CuisinePizzaId = new("cccc1111-1111-1111-1111-111111111111");
    private static readonly Guid CuisineBurgerId = new("cccc2222-2222-2222-2222-222222222222");
    private static readonly Guid CuisineKebapId = new("cccc3333-3333-3333-3333-333333333333");
    private static readonly Guid CuisineTurkId = new("cccc4444-4444-4444-4444-444444444444");
    private static readonly Guid CuisineFastFoodId = new("cccc5555-5555-5555-5555-555555555555");

    // Products
    private static readonly Guid Product1Id = new("dddd1111-1111-1111-1111-111111111111"); // Margarita Pizza
    private static readonly Guid Product2Id = new("dddd2222-2222-2222-2222-222222222222"); // Karışık Pizza
    private static readonly Guid Product3Id = new("dddd3333-3333-3333-3333-333333333333"); // Sucuk (malzeme)
    private static readonly Guid Product4Id = new("dddd4444-4444-4444-4444-444444444444"); // Mantar (malzeme)
    private static readonly Guid Product5Id = new("dddd5555-5555-5555-5555-555555555555"); // Cheeseburger
    private static readonly Guid Product6Id = new("dddd6666-6666-6666-6666-666666666666"); // Chicken Burger
    private static readonly Guid Product7Id = new("dddd7777-7777-7777-7777-777777777777"); // Adana Kebap
    private static readonly Guid Product8Id = new("dddd8888-8888-8888-8888-888888888888"); // Urfa Kebap
    private static readonly Guid Product9Id = new("dddd9999-9999-9999-9999-999999999999"); // Cola
    private static readonly Guid Product10Id = new("ddddaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"); // Ayran

    // Menus
    private static readonly Guid Menu1Id = new("eeee1111-1111-1111-1111-111111111111");
    private static readonly Guid Menu2Id = new("eeee2222-2222-2222-2222-222222222222");
    private static readonly Guid Menu3Id = new("eeee3333-3333-3333-3333-333333333333");
    private static readonly Guid Menu4Id = new("eeee4444-4444-4444-4444-444444444444");
    private static readonly Guid Menu5Id = new("eeee5555-5555-5555-5555-555555555555");
    private static readonly Guid Menu6Id = new("eeee6666-6666-6666-6666-666666666666");

    // Menu Options
    private static readonly Guid MenuOption1Id = new("ff001111-1111-1111-1111-111111111111");
    private static readonly Guid MenuOption2Id = new("ff002222-2222-2222-2222-222222222222");

    // Menu Option Values
    private static readonly Guid MenuOptionValue1Id = new("ff011111-1111-1111-1111-111111111111");
    private static readonly Guid MenuOptionValue2Id = new("ff012222-2222-2222-2222-222222222222");

    // Menu Option Value Options
    private static readonly Guid MenuOptionValueOption1Id = new("ff021111-1111-1111-1111-111111111111");

    // Menu Option Value Option Values
    private static readonly Guid MenuOptionValueOptionValue1Id = new("ff031111-1111-1111-1111-111111111111");
    private static readonly Guid MenuOptionValueOptionValue2Id = new("ff032222-2222-2222-2222-222222222222");

    // Categories
    private static readonly Guid Category1Id = new("aabb1111-1111-1111-1111-111111111111");
    private static readonly Guid Category2Id = new("aabb2222-2222-2222-2222-222222222222");
    private static readonly Guid Category3Id = new("aabb3333-3333-3333-3333-333333333333");

    // Addresses
    private static readonly Guid SellerAddress1Id = new("ad011111-1111-1111-1111-111111111111");
    private static readonly Guid SellerAddress2Id = new("ad022222-2222-2222-2222-222222222222");
    private static readonly Guid CustomerAddress1Id = new("ad031111-1111-1111-1111-111111111111");
    private static readonly Guid CustomerAddress2Id = new("ad042222-2222-2222-2222-222222222222");

    // Courier
    private static readonly Guid CourierCompanyId = new("ae011111-1111-1111-1111-111111111111");
    private static readonly Guid CourierId = new("af011111-1111-1111-1111-111111111111");

    // Commission
    private static readonly Guid PlatformCommission1Id = new("ab111111-1111-1111-1111-111111111111");
    private static readonly Guid RestaurantCommission1Id = new("ab221111-1111-1111-1111-111111111111");

    // Location constants (Bursa Nilüfer)
    private static readonly Guid CityBursaId = new("5d0c385c-810d-4dd6-9462-259183584992");
    private static readonly Guid TownNiluferId = new("342b6d4d-42bf-4085-92e7-8d2b51de130a");
    private static readonly Guid NeighbourhoodId = new("1c2d8a14-38df-448c-8125-14535bf0b7b3");

    private static readonly DateTime SeedDate = new(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);
    private const string PasswordHash = "$2a$12$FSkQpNFCoggkjDbhmQIKLuk2XIF6GF0lCW7nPK7vbJPsV91.zdvzW"; // Admin123!

    public void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cuisine>(SeedCuisines);
        modelBuilder.Entity<User>(SeedUsers);
        modelBuilder.Entity<Seller>(SeedSellers);
        modelBuilder.Entity<Restaurant>(SeedRestaurants);
        modelBuilder.Entity<Address>(SeedAddresses);
        modelBuilder.Entity<Product>(SeedProducts);
        modelBuilder.Entity<Menu>(SeedMenus);
        modelBuilder.Entity<MenuOption>(SeedMenuOptions);
        modelBuilder.Entity<MenuOptionValue>(SeedMenuOptionValues);
        modelBuilder.Entity<MenuOptionValueOption>(SeedMenuOptionValueOptions);
        modelBuilder.Entity<MenuOptionValueOptionValue>(SeedMenuOptionValueOptionValues);
        modelBuilder.Entity<Category>(SeedCategories);
        modelBuilder.Entity<CategoryDetail>(SeedCategoryDetails);
        modelBuilder.Entity<CourierCompany>(SeedCourierCompany);
        modelBuilder.Entity<Courier>(SeedCourier);
        modelBuilder.Entity<Domain.Entities.Common.PlatformCommissionSchedule>(SeedPlatformCommission);
        modelBuilder.Entity<RestaurantCommission>(SeedRestaurantCommission);
    }

    private void SeedCuisines(EntityTypeBuilder<Cuisine> builder)
    {
        builder.HasData(
            new Cuisine { Id = CuisinePizzaId, CreatedDate = SeedDate, Name = "Pizza" },
            new Cuisine { Id = CuisineBurgerId, CreatedDate = SeedDate, Name = "Burger" },
            new Cuisine { Id = CuisineKebapId, CreatedDate = SeedDate, Name = "Kebap" },
            new Cuisine { Id = CuisineTurkId, CreatedDate = SeedDate, Name = "Türk Mutfağı" },
            new Cuisine { Id = CuisineFastFoodId, CreatedDate = SeedDate, Name = "Fast Food" }
        );
    }

    private void SeedUsers(EntityTypeBuilder<User> builder)
    {
        builder.HasData(
            // Admin
            new User
            {
                Id = AdminUserId, CreatedDate = SeedDate,
                UserRoleId = (short)UserRoleEnums.Admin,
                UserStatusId = (short)UserStatusEnums.Active,
                Email = "admin@esnaftan.com", Password = PasswordHash,
                FirstName = "Esnaftan", LastName = "Admin",
                PhoneNumber = "05500000001", SexId = (short)SexEnums.Male,
                BirthDate = new DateTime(1990, 1, 1)
            },
            // Seller 1 Admin
            new User
            {
                Id = SellerUser1Id, CreatedDate = SeedDate,
                UserRoleId = (short)UserRoleEnums.SellerAdmin,
                UserStatusId = (short)UserStatusEnums.Active,
                Email = "info@pizzaci.com", Password = PasswordHash,
                FirstName = "Ahmet", LastName = "Pizzacı",
                PhoneNumber = "05500000002", SexId = (short)SexEnums.Male,
                SellerId = Seller1Id
            },
            // Seller 2 Admin
            new User
            {
                Id = SellerUser2Id, CreatedDate = SeedDate,
                UserRoleId = (short)UserRoleEnums.SellerAdmin,
                UserStatusId = (short)UserStatusEnums.Active,
                Email = "info@kebapci.com", Password = PasswordHash,
                FirstName = "Mehmet", LastName = "Kebapçı",
                PhoneNumber = "05500000003", SexId = (short)SexEnums.Male,
                SellerId = Seller2Id
            },
            // Customer 1
            new User
            {
                Id = CustomerUser1Id, CreatedDate = SeedDate,
                UserRoleId = (short)UserRoleEnums.User,
                UserStatusId = (short)UserStatusEnums.Active,
                Email = "ali@gmail.com", Password = PasswordHash,
                FirstName = "Ali", LastName = "Yılmaz",
                PhoneNumber = "05500000004", SexId = (short)SexEnums.Male
            },
            // Customer 2
            new User
            {
                Id = CustomerUser2Id, CreatedDate = SeedDate,
                UserRoleId = (short)UserRoleEnums.User,
                UserStatusId = (short)UserStatusEnums.Active,
                Email = "ayse@gmail.com", Password = PasswordHash,
                FirstName = "Ayşe", LastName = "Demir",
                PhoneNumber = "05500000005", SexId = (short)SexEnums.Female
            },
            // Courier User
            new User
            {
                Id = CourierUserId, CreatedDate = SeedDate,
                UserRoleId = (short)UserRoleEnums.Courier,
                UserStatusId = (short)UserStatusEnums.Active,
                Email = "kurye@gmail.com", Password = PasswordHash,
                FirstName = "Hasan", LastName = "Kurye",
                PhoneNumber = "05500000006", SexId = (short)SexEnums.Male
            },
            // Courier Company Admin
            new User
            {
                Id = CompanyAdminUserId, CreatedDate = SeedDate,
                UserRoleId = (short)UserRoleEnums.CourierCompanyAdmin,
                UserStatusId = (short)UserStatusEnums.Active,
                Email = "firma@kurye.com", Password = PasswordHash,
                FirstName = "Veli", LastName = "Firma",
                PhoneNumber = "05500000007", SexId = (short)SexEnums.Male
            }
        );
    }

    private void SeedSellers(EntityTypeBuilder<Seller> builder)
    {
        builder.HasData(
            new Seller
            {
                Id = Seller1Id, CreatedDate = SeedDate,
                CompanyType = (short)CompanyTypeEnums.Company,
                CompanyStatus = (short)CompanyStatusEnums.Approved,
                Name = "Pizzacı Ahmet", LegalName = "Pizzacı Ahmet Ltd. Şti.",
                TaxCode = "1111111111", TaxArea = "Nilüfer",
                IBAN = "TR111111111111111111111111", IsEInvoiceAvaible = true
            },
            new Seller
            {
                Id = Seller2Id, CreatedDate = SeedDate,
                CompanyType = (short)CompanyTypeEnums.Individual,
                CompanyStatus = (short)CompanyStatusEnums.Approved,
                Name = "Kebapçı Mehmet", LegalName = "Mehmet Kebap",
                TaxCode = "2222222222", TaxArea = "Osmangazi",
                IBAN = "TR222222222222222222222222", IsEInvoiceAvaible = false,
                IdentityNumber = "12345678901"
            }
        );
    }

    private void SeedRestaurants(EntityTypeBuilder<Restaurant> builder)
    {
        builder.HasData(
            new Restaurant
            {
                Id = Restaurant1Id, SellerId = Seller1Id, CreatedDate = SeedDate,
                Name = "Pizzacı Ahmet - Nilüfer", Phone = "05551112233", Email = "nilufer@pizzaci.com",
                MinimumOrderPrice = 150, MinDeliveryTime = 25, MaxDeliveryTime = 45,
                CoverImage = "https://picsum.photos/seed/pizza1/800/400",
                Description = "Taş fırında İtalyan pizzalar",
                Latitude = 40.2273m, Longitude = 28.8891m,
                IsActive = true, IsOpen = true, Rating = 4.5m, RatingCount = 120,
                HasOwnCouriers = false,
                ApprovedAt = SeedDate, ApprovedByUserId = AdminUserId
            },
            new Restaurant
            {
                Id = Restaurant2Id, SellerId = Seller1Id, CreatedDate = SeedDate,
                Name = "Pizzacı Ahmet - Osmangazi", Phone = "05551112244", Email = "osmangazi@pizzaci.com",
                MinimumOrderPrice = 100, MinDeliveryTime = 30, MaxDeliveryTime = 50,
                CoverImage = "https://picsum.photos/seed/pizza2/800/400",
                Description = "Lezzetli pizzalar, hızlı teslimat",
                Latitude = 40.1885m, Longitude = 29.0610m,
                IsActive = true, IsOpen = true, Rating = 4.2m, RatingCount = 85,
                HasOwnCouriers = false,
                ApprovedAt = SeedDate, ApprovedByUserId = AdminUserId
            },
            new Restaurant
            {
                Id = Restaurant3Id, SellerId = Seller2Id, CreatedDate = SeedDate,
                Name = "Kebapçı Mehmet", Phone = "05551113355", Email = "info@kebapci.com",
                MinimumOrderPrice = 200, MinDeliveryTime = 35, MaxDeliveryTime = 55,
                CoverImage = "https://picsum.photos/seed/kebap1/800/400",
                Description = "Geleneksel Türk kebapları, mangal lezzetleri",
                Latitude = 40.1950m, Longitude = 29.0200m,
                IsActive = true, IsOpen = true, Rating = 4.7m, RatingCount = 230,
                HasOwnCouriers = true,
                ApprovedAt = SeedDate, ApprovedByUserId = AdminUserId
            }
        );
    }

    private void SeedAddresses(EntityTypeBuilder<Address> builder)
    {
        builder.HasData(
            // Restaurant 1 address
            new Address
            {
                Id = SellerAddress1Id, CreatedDate = SeedDate, SellerId = Seller1Id, RestaurantId = Restaurant1Id,
                AddressType = (short)AddressTypeEnums.Delivery, AddressName = "Restoran Adresi",
                FirstName = "Pizzacı", LastName = "Ahmet", Phone = "05551112233",
                CityId = CityBursaId, TownId = TownNiluferId, NeighbourhoodId = NeighbourhoodId,
                AddressLine1 = "Ahmet Yesevi Mah. Bey Sk. No:4/B",
                Latitude = "40.2273", Longitude = "28.8891", IsDefault = true
            },
            // Restaurant 3 address
            new Address
            {
                Id = SellerAddress2Id, CreatedDate = SeedDate, SellerId = Seller2Id, RestaurantId = Restaurant3Id,
                AddressType = (short)AddressTypeEnums.Delivery, AddressName = "Restoran Adresi",
                FirstName = "Kebapçı", LastName = "Mehmet", Phone = "05551113355",
                CityId = CityBursaId, TownId = TownNiluferId, NeighbourhoodId = NeighbourhoodId,
                AddressLine1 = "Çamlıca Mah. Kebap Sok. No:15",
                Latitude = "40.1950", Longitude = "29.0200", IsDefault = true
            },
            // Customer 1 address
            new Address
            {
                Id = CustomerAddress1Id, CreatedDate = SeedDate, UserId = CustomerUser1Id,
                AddressType = (short)AddressTypeEnums.Shipping, AddressName = "Ev",
                FirstName = "Ali", LastName = "Yılmaz", Phone = "05500000004",
                CityId = CityBursaId, TownId = TownNiluferId, NeighbourhoodId = NeighbourhoodId,
                AddressLine1 = "Geçit Mah. 1. Begonya Sok. No:57 D:6",
                AddressLine2 = "Oliva Sitesi B Blok",
                Latitude = "40.2659", Longitude = "28.9618",
                IsDefault = true, InvoiceType = (short)InvoiceTypeEnums.Personal
            },
            // Customer 2 address
            new Address
            {
                Id = CustomerAddress2Id, CreatedDate = SeedDate, UserId = CustomerUser2Id,
                AddressType = (short)AddressTypeEnums.Shipping, AddressName = "İş",
                FirstName = "Ayşe", LastName = "Demir", Phone = "05500000005",
                CityId = CityBursaId, TownId = TownNiluferId, NeighbourhoodId = NeighbourhoodId,
                AddressLine1 = "Özlüce Mah. İş Merkezi No:22 K:3",
                Latitude = "40.2300", Longitude = "28.9100",
                IsDefault = true, InvoiceType = (short)InvoiceTypeEnums.Personal
            }
        );
    }

    private void SeedProducts(EntityTypeBuilder<Product> builder)
    {
        builder.HasData(
            // Restaurant 1 products (Pizza)
            new Product { Id = Product1Id, CreatedDate = SeedDate, RestaurantId = Restaurant1Id, CuisineId = CuisinePizzaId, Name = "Margarita Pizza", ProductType = 1, Price = 0, Description = "Domates sos, mozzarella, fesleğen", OrderIndex = 0 },
            new Product { Id = Product2Id, CreatedDate = SeedDate, RestaurantId = Restaurant1Id, CuisineId = CuisinePizzaId, Name = "Karışık Pizza", ProductType = 1, Price = 0, Description = "Sucuk, mantar, biber, mısır", OrderIndex = 1 },
            new Product { Id = Product3Id, CreatedDate = SeedDate, RestaurantId = Restaurant1Id, CuisineId = CuisinePizzaId, Name = "Sucuk", ProductType = 2, Price = 0, Description = "", OrderIndex = 0 },
            new Product { Id = Product4Id, CreatedDate = SeedDate, RestaurantId = Restaurant1Id, CuisineId = CuisinePizzaId, Name = "Mantar", ProductType = 2, Price = 0, Description = "", OrderIndex = 1 },
            new Product { Id = Product9Id, CreatedDate = SeedDate, RestaurantId = Restaurant1Id, CuisineId = CuisineFastFoodId, Name = "Cola 330ml", ProductType = 1, Price = 0, Description = "", OrderIndex = 10 },
            new Product { Id = Product10Id, CreatedDate = SeedDate, RestaurantId = Restaurant1Id, CuisineId = CuisineTurkId, Name = "Ayran", ProductType = 1, Price = 0, Description = "", OrderIndex = 11 },
            // Restaurant 3 products (Kebap)
            new Product { Id = Product5Id, CreatedDate = SeedDate, RestaurantId = Restaurant3Id, CuisineId = CuisineBurgerId, Name = "Cheeseburger", ProductType = 1, Price = 0, Description = "Özel soslu dana burger", OrderIndex = 0 },
            new Product { Id = Product6Id, CreatedDate = SeedDate, RestaurantId = Restaurant3Id, CuisineId = CuisineBurgerId, Name = "Chicken Burger", ProductType = 1, Price = 0, Description = "Çıtır tavuk burger", OrderIndex = 1 },
            new Product { Id = Product7Id, CreatedDate = SeedDate, RestaurantId = Restaurant3Id, CuisineId = CuisineKebapId, Name = "Adana Kebap", ProductType = 1, Price = 0, Description = "Acılı el yapımı kebap", OrderIndex = 2 },
            new Product { Id = Product8Id, CreatedDate = SeedDate, RestaurantId = Restaurant3Id, CuisineId = CuisineKebapId, Name = "Urfa Kebap", ProductType = 1, Price = 0, Description = "Acısız kebap", OrderIndex = 3 }
        );
    }

    private void SeedMenus(EntityTypeBuilder<Menu> builder)
    {
        builder.HasData(
            // Restaurant 1 - Pizza
            new Menu { Id = Menu1Id, CreatedDate = SeedDate, RestaurantId = Restaurant1Id, Name = "Margarita Pizza (Orta)", Description = "Klasik İtalyan", Price = 180, OrderIndex = 0 },
            new Menu { Id = Menu2Id, CreatedDate = SeedDate, RestaurantId = Restaurant1Id, Name = "Karışık Pizza (Orta)", Description = "Bol malzemeli", Price = 220, OrderIndex = 1 },
            new Menu { Id = Menu3Id, CreatedDate = SeedDate, RestaurantId = Restaurant1Id, Name = "Cola 330ml", Description = "", Price = 35, OrderIndex = 10 },
            new Menu { Id = Menu4Id, CreatedDate = SeedDate, RestaurantId = Restaurant1Id, Name = "Ayran", Description = "", Price = 20, OrderIndex = 11 },
            // Restaurant 3 - Kebap
            new Menu { Id = Menu5Id, CreatedDate = SeedDate, RestaurantId = Restaurant3Id, Name = "Adana Kebap Porsiyon", Description = "2 şiş, lavaş, közlenmiş", Price = 320, OrderIndex = 0 },
            new Menu { Id = Menu6Id, CreatedDate = SeedDate, RestaurantId = Restaurant3Id, Name = "Urfa Kebap Porsiyon", Description = "2 şiş, lavaş, közlenmiş", Price = 300, OrderIndex = 1 }
        );
    }

    private void SeedMenuOptions(EntityTypeBuilder<MenuOption> builder)
    {
        builder.HasData(
            // Karışık Pizza -> Pizza Seçimi
            new MenuOption { Id = MenuOption1Id, CreatedDate = SeedDate, MenuId = Menu2Id, Name = "Pizza Tercihi", Description = "Pizza hamur tipini seçin", MinCount = 1, MaxCount = 1, OrderIndex = 0 },
            // Karışık Pizza -> Ekstra Malzeme
            new MenuOption { Id = MenuOption2Id, CreatedDate = SeedDate, MenuId = Menu2Id, Name = "Ekstra Malzeme", Description = "İstediğiniz malzemeleri ekleyin", MinCount = 0, MaxCount = 3, OrderIndex = 1 }
        );
    }

    private void SeedMenuOptionValues(EntityTypeBuilder<MenuOptionValue> builder)
    {
        builder.HasData(
            // Pizza Tercihi -> Karışık Pizza
            new MenuOptionValue { Id = MenuOptionValue1Id, CreatedDate = SeedDate, MenuOptionId = MenuOption1Id, ProductId = Product2Id, Price = 0, OrderIndex = 0 },
            // Ekstra Malzeme -> Sucuk (+15₺)
            new MenuOptionValue { Id = MenuOptionValue2Id, CreatedDate = SeedDate, MenuOptionId = MenuOption2Id, ProductId = Product3Id, Price = 15, OrderIndex = 0 }
        );
    }

    private void SeedMenuOptionValueOptions(EntityTypeBuilder<MenuOptionValueOption> builder)
    {
        builder.HasData(
            // Karışık Pizza -> Çıkarılacak Malzemeler
            new MenuOptionValueOption { Id = MenuOptionValueOption1Id, CreatedDate = SeedDate, MenuOptionValueId = MenuOptionValue1Id, Name = "Çıkarılacak Malzemeler", MinCount = 0, MaxCount = 4, OrderIndex = 0 }
        );
    }

    private void SeedMenuOptionValueOptionValues(EntityTypeBuilder<MenuOptionValueOptionValue> builder)
    {
        builder.HasData(
            new MenuOptionValueOptionValue { Id = MenuOptionValueOptionValue1Id, CreatedDate = SeedDate, MenuOptionValueOptionId = MenuOptionValueOption1Id, ProductId = Product3Id, Price = 0, OrderIndex = 0 },
            new MenuOptionValueOptionValue { Id = MenuOptionValueOptionValue2Id, CreatedDate = SeedDate, MenuOptionValueOptionId = MenuOptionValueOption1Id, ProductId = Product4Id, Price = 0, OrderIndex = 1 }
        );
    }

    private void SeedCategories(EntityTypeBuilder<Category> builder)
    {
        builder.HasData(
            new Category { Id = Category1Id, CreatedDate = SeedDate, RestaurantId = Restaurant1Id, Name = "Pizzalar", OrderIndex = 0 },
            new Category { Id = Category2Id, CreatedDate = SeedDate, RestaurantId = Restaurant1Id, Name = "İçecekler", OrderIndex = 1 },
            new Category { Id = Category3Id, CreatedDate = SeedDate, RestaurantId = Restaurant3Id, Name = "Kebaplar", OrderIndex = 0 }
        );
    }

    private void SeedCategoryDetails(EntityTypeBuilder<CategoryDetail> builder)
    {
        builder.HasData(
            new CategoryDetail { Id = new Guid("ca011111-1111-1111-1111-111111111111"), CreatedDate = SeedDate, CategoryId = Category1Id, MenuId = Menu1Id, OrderIndex = 0 },
            new CategoryDetail { Id = new Guid("ca022222-2222-2222-2222-222222222222"), CreatedDate = SeedDate, CategoryId = Category1Id, MenuId = Menu2Id, OrderIndex = 1 },
            new CategoryDetail { Id = new Guid("ca033333-3333-3333-3333-333333333333"), CreatedDate = SeedDate, CategoryId = Category2Id, MenuId = Menu3Id, OrderIndex = 0 },
            new CategoryDetail { Id = new Guid("ca044444-4444-4444-4444-444444444444"), CreatedDate = SeedDate, CategoryId = Category2Id, MenuId = Menu4Id, OrderIndex = 1 },
            new CategoryDetail { Id = new Guid("ca055555-5555-5555-5555-555555555555"), CreatedDate = SeedDate, CategoryId = Category3Id, MenuId = Menu5Id, OrderIndex = 0 },
            new CategoryDetail { Id = new Guid("ca066666-6666-6666-6666-666666666666"), CreatedDate = SeedDate, CategoryId = Category3Id, MenuId = Menu6Id, OrderIndex = 1 }
        );
    }

    private void SeedCourierCompany(EntityTypeBuilder<CourierCompany> builder)
    {
        builder.HasData(
            new CourierCompany
            {
                Id = CourierCompanyId, CreatedDate = SeedDate,
                Name = "Hızlı Kurye", LegalName = "Hızlı Kurye Ltd. Şti.",
                TaxCode = "9999999999", TaxArea = "Nilüfer",
                IBAN = "TR999999999999999999999999",
                Phone = "05500000007", Email = "firma@kurye.com",
                ContactPerson = "Veli Firma",
                StatusId = (short)CourierCompanyStatusEnums.Active,
                CompanyTypeId = (short)CourierCompanyTypeEnums.Corporate,
                CommissionRate = 0.15m
            }
        );
    }

    private void SeedCourier(EntityTypeBuilder<Courier> builder)
    {
        builder.HasData(
            new Courier
            {
                Id = CourierId, CreatedDate = SeedDate,
                UserId = CourierUserId,
                CourierCompanyId = CourierCompanyId,
                CourierTypeId = (short)CourierTypeEnums.CompanyMember,
                StatusId = (short)CourierStatusEnums.Active,
                AvailabilityStatusId = (short)CourierAvailabilityEnums.Online,
                VehicleType = "Motosiklet", VehiclePlate = "16 AB 123",
                IBAN = "TR666666666666666666666666",
                Rating = 4.8m, RatingCount = 50, TotalDeliveries = 200,
                CurrentLatitude = 40.2273m, CurrentLongitude = 28.8891m,
                LastLocationUpdate = SeedDate
            }
        );
    }

    private void SeedPlatformCommission(EntityTypeBuilder<Domain.Entities.Common.PlatformCommissionSchedule> builder)
    {
        builder.HasData(
            new Domain.Entities.Common.PlatformCommissionSchedule
            {
                Id = PlatformCommission1Id, CreatedDate = SeedDate,
                CommissionRate = 0.10m, FixedFee = 5.00m,
                EffectiveFrom = SeedDate,
                SetByUserId = AdminUserId,
                Notes = "Varsayılan platform komisyonu"
            }
        );
    }

    private void SeedRestaurantCommission(EntityTypeBuilder<RestaurantCommission> builder)
    {
        builder.HasData(
            new RestaurantCommission
            {
                Id = RestaurantCommission1Id, CreatedDate = SeedDate,
                RestaurantId = Restaurant3Id,
                CommissionRate = 0.08m, FixedFee = 3.00m,
                EffectiveFrom = SeedDate,
                SetByUserId = AdminUserId,
                Reason = "Özel anlaşma"
            }
        );
    }
}
