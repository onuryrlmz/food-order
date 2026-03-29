import CourierService from './courierService.js';
import CompanyService from './companyService.js';

export default class CourierServices {
  constructor(client) {
    this.courier = new CourierService(client);
    this.company = new CompanyService(client);
  }
}
