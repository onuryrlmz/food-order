import BaseService from '../base.js';
import { CompanyRegisterRequestSchema } from '../../schema/courier/company.js';

export default class CompanyService extends BaseService {
  register(data) {
    const parsed = CompanyRegisterRequestSchema.parse(data);
    return this.post('/courier-company/register', parsed);
  }
}
