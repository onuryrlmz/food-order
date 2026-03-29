import AuthService from './authService.js';
import CuisineService from './cuisineService.js';
import CustomerServices from './customer/index.js';
import CourierServices from './courier/index.js';
import SellerServices from './seller/index.js';
import AdminServices from './admin/index.js';

export default class Service {
  constructor(client) {
    this.auth = new AuthService(client);
    this.cuisine = new CuisineService(client);
    this.customer = new CustomerServices(client);
    this.courier = new CourierServices(client);
    this.seller = new SellerServices(client);
    this.admin = new AdminServices(client);
  }
}
