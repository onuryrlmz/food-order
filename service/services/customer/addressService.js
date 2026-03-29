import BaseService from '../base.js';
import { CreateAddressRequestSchema, UpdateAddressRequestSchema, AddressIdSchema } from '../../schema/customer/address.js';

export default class AddressService extends BaseService {
  getList() {
    return this.get('/customer/address');
  }

  create(data) {
    const parsed = CreateAddressRequestSchema.parse(data);
    return this.post('/customer/address', parsed);
  }

  update(data) {
    const parsed = UpdateAddressRequestSchema.parse(data);
    return this.put('/customer/address', parsed);
  }

  remove(data) {
    const { id } = AddressIdSchema.parse(data);
    return this.delete(`/customer/address/${id}`);
  }

  setDefault(data) {
    const { id } = AddressIdSchema.parse(data);
    return this.patch(`/customer/address/${id}/set-default`);
  }
}
