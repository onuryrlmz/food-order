import BaseService from '../base.js';
import {
  CreateMenuRequestSchema, UpdateMenuRequestSchema, DeleteMenuRequestSchema, GetMenusByRestaurantSchema,
  CreateMenuOptionRequestSchema, UpdateMenuOptionRequestSchema, DeleteMenuOptionRequestSchema,
  CreateMenuOptionValueRequestSchema, DeleteMenuOptionValueRequestSchema,
  CreateMenuOptionValueOptionRequestSchema, UpdateMenuOptionValueOptionRequestSchema, DeleteMenuOptionValueOptionRequestSchema,
  CreateMenuOptionValueOptionValueRequestSchema, UpdateMenuOptionValueOptionValueRequestSchema, DeleteMenuOptionValueOptionValueRequestSchema,
} from '../../schema/seller/menu.js';

export default class SellerMenuService extends BaseService {
  getByRestaurant(data) {
    const { restaurantId } = GetMenusByRestaurantSchema.parse(data);
    return this.get(`/seller/menu/by-restaurant/${restaurantId}`);
  }

  create(data) {
    const parsed = CreateMenuRequestSchema.parse(data);
    return this.post('/seller/menu', parsed);
  }

  update(data) {
    const parsed = UpdateMenuRequestSchema.parse(data);
    return this.put('/seller/menu', parsed);
  }

  remove(data) {
    const parsed = DeleteMenuRequestSchema.parse(data);
    return this.delete('/seller/menu', parsed);
  }

  // Menu Options
  createOption(data) {
    const parsed = CreateMenuOptionRequestSchema.parse(data);
    return this.post('/seller/menu-option', parsed);
  }

  updateOption(data) {
    const parsed = UpdateMenuOptionRequestSchema.parse(data);
    return this.put('/seller/menu-option', parsed);
  }

  removeOption(data) {
    const parsed = DeleteMenuOptionRequestSchema.parse(data);
    return this.delete('/seller/menu-option', parsed);
  }

  // Menu Option Values
  createOptionValue(data) {
    const parsed = CreateMenuOptionValueRequestSchema.parse(data);
    return this.post('/seller/menu-option-value', parsed);
  }

  removeOptionValue(data) {
    const parsed = DeleteMenuOptionValueRequestSchema.parse(data);
    return this.delete('/seller/menu-option-value', parsed);
  }

  // Menu Option Value Options
  createValueOption(data) {
    const parsed = CreateMenuOptionValueOptionRequestSchema.parse(data);
    return this.post('/seller/menu-option-value-option', parsed);
  }

  updateValueOption(data) {
    const parsed = UpdateMenuOptionValueOptionRequestSchema.parse(data);
    return this.put('/seller/menu-option-value-option', parsed);
  }

  removeValueOption(data) {
    const parsed = DeleteMenuOptionValueOptionRequestSchema.parse(data);
    return this.delete('/seller/menu-option-value-option', parsed);
  }

  // Menu Option Value Option Values
  createValueOptionValue(data) {
    const parsed = CreateMenuOptionValueOptionValueRequestSchema.parse(data);
    return this.post('/seller/menu-option-value-option-value', parsed);
  }

  updateValueOptionValue(data) {
    const parsed = UpdateMenuOptionValueOptionValueRequestSchema.parse(data);
    return this.put('/seller/menu-option-value-option-value', parsed);
  }

  removeValueOptionValue(data) {
    const parsed = DeleteMenuOptionValueOptionValueRequestSchema.parse(data);
    return this.delete('/seller/menu-option-value-option-value', parsed);
  }
}
