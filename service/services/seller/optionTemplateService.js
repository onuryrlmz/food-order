import BaseService from '../base.js';
import {
  CreateOptionTemplateRequestSchema, UpdateOptionTemplateRequestSchema, DeleteOptionTemplateRequestSchema,
  CreateTemplateValueRequestSchema, DeleteTemplateValueRequestSchema,
  CreateTemplateValueOptionRequestSchema, UpdateTemplateValueOptionRequestSchema, DeleteTemplateValueOptionRequestSchema,
  CreateTemplateValueOptionValueRequestSchema, UpdateTemplateValueOptionValueRequestSchema, DeleteTemplateValueOptionValueRequestSchema,
} from '../../schema/seller/optionTemplate.js';

export default class SellerOptionTemplateService extends BaseService {
  create(data) {
    const parsed = CreateOptionTemplateRequestSchema.parse(data);
    return this.post('/seller/option-template', parsed);
  }

  update(data) {
    const parsed = UpdateOptionTemplateRequestSchema.parse(data);
    return this.put('/seller/option-template', parsed);
  }

  remove(data) {
    const parsed = DeleteOptionTemplateRequestSchema.parse(data);
    return this.delete('/seller/option-template', parsed);
  }

  createValue(data) {
    const parsed = CreateTemplateValueRequestSchema.parse(data);
    return this.post('/seller/option-template/value', parsed);
  }

  removeValue(data) {
    const parsed = DeleteTemplateValueRequestSchema.parse(data);
    return this.delete('/seller/option-template/value', parsed);
  }

  createValueOption(data) {
    const parsed = CreateTemplateValueOptionRequestSchema.parse(data);
    return this.post('/seller/option-template/value-option', parsed);
  }

  updateValueOption(data) {
    const parsed = UpdateTemplateValueOptionRequestSchema.parse(data);
    return this.put('/seller/option-template/value-option', parsed);
  }

  removeValueOption(data) {
    const parsed = DeleteTemplateValueOptionRequestSchema.parse(data);
    return this.delete('/seller/option-template/value-option', parsed);
  }

  createValueOptionValue(data) {
    const parsed = CreateTemplateValueOptionValueRequestSchema.parse(data);
    return this.post('/seller/option-template/value-option-value', parsed);
  }

  updateValueOptionValue(data) {
    const parsed = UpdateTemplateValueOptionValueRequestSchema.parse(data);
    return this.put('/seller/option-template/value-option-value', parsed);
  }

  removeValueOptionValue(data) {
    const parsed = DeleteTemplateValueOptionValueRequestSchema.parse(data);
    return this.delete('/seller/option-template/value-option-value', parsed);
  }
}
