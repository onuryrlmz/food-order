import BaseService from './base.js';

export default class CuisineService extends BaseService {
  getList() {
    return this.get('/cuisine');
  }
}
