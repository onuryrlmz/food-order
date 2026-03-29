import BaseService from '../base.js';
import { CreateCardRequestSchema, DeleteCardSchema } from '../../schema/customer/card.js';

export default class CardService extends BaseService {
  getList() {
    return this.get('/customer/card/list');
  }

  create(data) {
    const parsed = CreateCardRequestSchema.parse(data);
    return this.post('/customer/card/create', parsed);
  }

  remove(data) {
    const { cardToken } = DeleteCardSchema.parse(data);
    return this.delete(`/customer/card/${cardToken}`);
  }
}
