import BaseService from '../base.js';
import { CreateCategoryRequestSchema, UpdateCategoryRequestSchema, DeleteCategoryRequestSchema, GetCategoriesQuerySchema, CreateCategoryDetailRequestSchema, DeleteCategoryDetailRequestSchema } from '../../schema/seller/category.js';

export default class SellerCategoryService extends BaseService {
  getList(data) {
    const parsed = GetCategoriesQuerySchema.parse(data);
    return this.get('/seller/category', parsed);
  }

  create(data) {
    const parsed = CreateCategoryRequestSchema.parse(data);
    return this.post('/seller/category', parsed);
  }

  update(data) {
    const parsed = UpdateCategoryRequestSchema.parse(data);
    return this.put('/seller/category', parsed);
  }

  remove(data) {
    const parsed = DeleteCategoryRequestSchema.parse(data);
    return this.delete('/seller/category', parsed);
  }

  createDetail(data) {
    const parsed = CreateCategoryDetailRequestSchema.parse(data);
    return this.post('/seller/category-detail', parsed);
  }

  removeDetail(data) {
    const parsed = DeleteCategoryDetailRequestSchema.parse(data);
    return this.delete('/seller/category-detail', parsed);
  }
}
