import BaseService from '../base.js';
import { UpdateBasketRequestSchema } from '../../schema/customer/basket.js';

export default class BasketService extends BaseService {
  getBasket() {
    return this.get('/customer/basket');
  }

  update(data) {
    const parsed = UpdateBasketRequestSchema.parse(data);
    return this.put('/customer/basket', parsed);
  }

  clear() {
    return this.delete('/customer/basket');
  }
}
