import BaseService from '../base.js';
import { GetRestaurantsByAddressSchema, GetRestaurantsByLocationSchema, GetRestaurantInfoSchema } from '../../schema/customer/restaurant.js';

export default class RestaurantService extends BaseService {
  getByAddress(data) {
    const { addressId } = GetRestaurantsByAddressSchema.parse(data);
    return this.get('/customer/restaurant', { addressId });
  }

  getByLocation(data) {
    const { latitude, longitude } = GetRestaurantsByLocationSchema.parse(data);
    return this.get('/customer/restaurant', { latitude, longitude });
  }

  getInfo(data) {
    const { id } = GetRestaurantInfoSchema.parse(data);
    return this.get(`/customer/restaurant/${id}`);
  }
}
