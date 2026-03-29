import SellerRestaurantService from './restaurantService.js';
import SellerProductService from './productService.js';
import SellerCategoryService from './categoryService.js';
import SellerMenuService from './menuService.js';
import SellerOptionTemplateService from './optionTemplateService.js';
import SellerCouponService from './couponService.js';
import SellerFinanceService from './financeService.js';
import SellerAnalyticsService from './analyticsService.js';
import SellerCommissionService from './commissionService.js';
import SellerCourierService from './courierService.js';
import SellerOrderService from './orderService.js';

export default class SellerServices {
  constructor(client) {
    this.restaurant = new SellerRestaurantService(client);
    this.product = new SellerProductService(client);
    this.category = new SellerCategoryService(client);
    this.menu = new SellerMenuService(client);
    this.optionTemplate = new SellerOptionTemplateService(client);
    this.coupon = new SellerCouponService(client);
    this.finance = new SellerFinanceService(client);
    this.analytics = new SellerAnalyticsService(client);
    this.commission = new SellerCommissionService(client);
    this.courier = new SellerCourierService(client);
    this.order = new SellerOrderService(client);
  }
}
