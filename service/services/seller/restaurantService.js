import BaseService from '../base.js';
import { AddRestaurantRequestSchema, UpdateRestaurantRequestSchema, RestaurantIdSchema, UpdateWorkingHoursRequestSchema } from '../../schema/seller/restaurant.js';

export default class SellerRestaurantService extends BaseService {
  getList() {
    return this.get('/seller/restaurant/list');
  }

  add(data) {
    const parsed = AddRestaurantRequestSchema.parse(data);
    return this.post('/seller/restaurant/add', parsed);
  }

  update(data) {
    const parsed = UpdateRestaurantRequestSchema.parse(data);
    return this.put('/seller/restaurant/update', parsed);
  }

  toggleOpen(data) {
    const { restaurantId } = RestaurantIdSchema.parse(data);
    return this.patch(`/seller/restaurant/${restaurantId}/toggle-open`);
  }

  updateWorkingHours(data) {
    const { restaurantId, ...body } = UpdateWorkingHoursRequestSchema.parse(data);
    return this.put(`/seller/restaurant/${restaurantId}/working-hours`, body);
  }
}
