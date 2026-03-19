-- Tüm tabloları FK bağımlılık sırasına göre drop eder
-- MySQL için FK kontrolünü geçici olarak kapatıp açar

SET FOREIGN_KEY_CHECKS = 0;

-- Sipariş detay tabloları (en derin bağımlılıklar)
DROP TABLE IF EXISTS `OrderItemValueOptions`;
DROP TABLE IF EXISTS `OrderItemValues`;
DROP TABLE IF EXISTS `OrderItem`;
DROP TABLE IF EXISTS `OrderStatusHistories`;
DROP TABLE IF EXISTS `Payments`;

-- Sepet detay tabloları
DROP TABLE IF EXISTS `BasketItemValueItemValue`;
DROP TABLE IF EXISTS `BasketItemValue`;
DROP TABLE IF EXISTS `BasketItem`;
DROP TABLE IF EXISTS `Basket`;

-- Sipariş ve değerlendirme
DROP TABLE IF EXISTS `Order`;
DROP TABLE IF EXISTS `Review`;
DROP TABLE IF EXISTS `FavoriteRestaurant`;

-- Kupon tabloları
DROP TABLE IF EXISTS `UserCoupon`;
DROP TABLE IF EXISTS `CouponMenu`;
DROP TABLE IF EXISTS `CouponCategory`;
DROP TABLE IF EXISTS `Coupon`;

-- Menü opsiyon zincirleri
DROP TABLE IF EXISTS `MenuOptionValueOptionValue`;
DROP TABLE IF EXISTS `MenuOptionValueOption`;
DROP TABLE IF EXISTS `MenuOptionValue`;
DROP TABLE IF EXISTS `MenuOption`;
DROP TABLE IF EXISTS `Menu`;

-- Şablon opsiyon zincirleri
DROP TABLE IF EXISTS `OptionTemplateValueOptionValue`;
DROP TABLE IF EXISTS `OptionTemplateValueOption`;
DROP TABLE IF EXISTS `OptionTemplateValue`;
DROP TABLE IF EXISTS `OptionTemplate`;

-- Ürün tabloları
DROP TABLE IF EXISTS `ProductAttributeValue`;
DROP TABLE IF EXISTS `ProductAttribute`;
DROP TABLE IF EXISTS `Product`;

-- Kategori tabloları
DROP TABLE IF EXISTS `CategoryDetail`;
DROP TABLE IF EXISTS `Category`;

-- Abonelik tabloları
DROP TABLE IF EXISTS `SubscriptionUsage`;
DROP TABLE IF EXISTS `Subscription`;
DROP TABLE IF EXISTS `SubscriptionPlan`;

-- Restoran tabloları
DROP TABLE IF EXISTS `RestaurantWorkingHour`;
DROP TABLE IF EXISTS `RestaurantCdnUpdateQueue`;
DROP TABLE IF EXISTS `Restaurant`;

-- Satıcı tabloları
DROP TABLE IF EXISTS `SellerDetail`;
DROP TABLE IF EXISTS `Seller`;

-- Kullanıcı tabloları
DROP TABLE IF EXISTS `RefreshToken`;
DROP TABLE IF EXISTS `PasswordResetToken`;
DROP TABLE IF EXISTS `UserExternalInfos`;
DROP TABLE IF EXISTS `Address`;
DROP TABLE IF EXISTS `User`;

-- Ortak tablolar
DROP TABLE IF EXISTS `Cuisine`;
DROP TABLE IF EXISTS `ScheduledTask`;

-- Kurye tabloları (eğer hâlâ DB'de varsa)
DROP TABLE IF EXISTS `RestaurantCourierCompanies`;
DROP TABLE IF EXISTS `RestaurantCouriers`;
DROP TABLE IF EXISTS `CourierCompanyMembers`;
DROP TABLE IF EXISTS `CourierLocations`;
DROP TABLE IF EXISTS `CourierCompanies`;

-- EF Core migration geçmişi
DROP TABLE IF EXISTS `__EFMigrationsHistory`;

SET FOREIGN_KEY_CHECKS = 1;

-- Kontrol: Kalan tablo var mı?
SELECT TABLE_NAME FROM information_schema.TABLES
WHERE TABLE_SCHEMA = DATABASE();
