import BaseService from '../base.js';
import { ToggleFavoriteRequestSchema, FavoritesQuerySchema } from '../../schema/customer/favorite.js';

export default class FavoriteService extends BaseService {
  add(data) {
    const parsed = ToggleFavoriteRequestSchema.parse(data);
    return this.post('/customer/favorite', parsed);
  }

  remove(data) {
    const { restaurantId } = ToggleFavoriteRequestSchema.parse(data);
    return this.delete(`/customer/favorite/${restaurantId}`);
  }

  getList(data) {
    const parsed = FavoritesQuerySchema.parse(data || {});
    return this.get('/customer/favorite', parsed);
  }
}
