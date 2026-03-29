import BaseService from '../base.js';
import { CreateReviewRequestSchema, GetReviewsQuerySchema, DeleteReviewSchema } from '../../schema/customer/review.js';

export default class ReviewService extends BaseService {
  create(data) {
    const parsed = CreateReviewRequestSchema.parse(data);
    return this.post('/customer/review', parsed);
  }

  getByRestaurant(data) {
    const { restaurantId, ...pagination } = GetReviewsQuerySchema.parse(data);
    return this.get(`/customer/review/restaurant/${restaurantId}`, pagination);
  }

  remove(data) {
    const { reviewId } = DeleteReviewSchema.parse(data);
    return this.delete(`/customer/review/${reviewId}`);
  }
}
