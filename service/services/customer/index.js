import RestaurantService from './restaurantService.js';
import OrderService from './orderService.js';
import BasketService from './basketService.js';
import AddressService from './addressService.js';
import CouponService from './couponService.js';
import ReviewService from './reviewService.js';
import FavoriteService from './favoriteService.js';
import SearchService from './searchService.js';
import TipService from './tipService.js';
import NotificationService from './notificationService.js';
import ScheduledOrderService from './scheduledOrderService.js';
import SupportService from './supportService.js';
import CardService from './cardService.js';

export default class CustomerServices {
  constructor(client) {
    this.restaurant = new RestaurantService(client);
    this.order = new OrderService(client);
    this.basket = new BasketService(client);
    this.address = new AddressService(client);
    this.coupon = new CouponService(client);
    this.review = new ReviewService(client);
    this.favorite = new FavoriteService(client);
    this.search = new SearchService(client);
    this.tip = new TipService(client);
    this.notification = new NotificationService(client);
    this.scheduledOrder = new ScheduledOrderService(client);
    this.support = new SupportService(client);
    this.card = new CardService(client);
  }
}
