import BaseService from '../base.js';
import { CreateProductRequestSchema, UpdateProductRequestSchema, DeleteProductRequestSchema, DeleteProductImageSchema } from '../../schema/seller/product.js';

export default class SellerProductService extends BaseService {
  create(data) {
    const parsed = CreateProductRequestSchema.parse(data);
    return this.post('/seller/product', parsed);
  }

  update(data) {
    const parsed = UpdateProductRequestSchema.parse(data);
    return this.put('/seller/product', parsed);
  }

  remove(data) {
    const parsed = DeleteProductRequestSchema.parse(data);
    return this.delete('/seller/product', parsed);
  }

  uploadImage(productId, file) {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('productId', productId);
    return this.postForm('/seller/product/image', formData);
  }

  deleteImage(data) {
    const { imageId } = DeleteProductImageSchema.parse(data);
    return this.delete(`/seller/product/image/${imageId}`);
  }
}
