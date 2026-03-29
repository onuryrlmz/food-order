import BaseService from '../base.js';
import { CreateAgreementByEmailRequestSchema, DeleteAgreementRequestSchema, AssignCourierRequestSchema, UpdateCourierSettingsRequestSchema, SellerRestaurantIdSchema } from '../../schema/seller/courier.js';

export default class SellerCourierService extends BaseService {
  getAgreements(data) {
    const { restaurantId } = SellerRestaurantIdSchema.parse(data);
    return this.get(`/seller/courier/restaurant/${restaurantId}/agreements`);
  }

  createAgreementByEmail(data) {
    const { restaurantId, email } = CreateAgreementByEmailRequestSchema.parse(data);
    return this.post(`/seller/courier/restaurant/${restaurantId}/agreements/by-email`, { email });
  }

  deleteAgreement(data) {
    const { id } = DeleteAgreementRequestSchema.parse(data);
    return this.delete(`/seller/courier/agreements/${id}`);
  }

  assignCourier(data) {
    const { restaurantId, orderId, courierId } = AssignCourierRequestSchema.parse(data);
    return this.post(`/seller/courier/restaurant/${restaurantId}/assign/${orderId}`, null, { courierId });
  }

  updateSettings(data) {
    const { restaurantId, ...body } = UpdateCourierSettingsRequestSchema.parse(data);
    return this.put(`/seller/courier/restaurant/${restaurantId}/settings`, body);
  }
}
