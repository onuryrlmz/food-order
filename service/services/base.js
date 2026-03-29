/**
 * Base service class — all domain services extend this.
 * Provides HTTP verb methods with Zod validation.
 */
export default class BaseService {
  constructor(client) {
    this.client = client;
  }

  async get(url, params) {
    const res = await this.client.get(url, { params });
    return res.data;
  }

  async post(url, data) {
    const res = await this.client.post(url, data);
    return res.data;
  }

  async put(url, data) {
    const res = await this.client.put(url, data);
    return res.data;
  }

  async patch(url, data) {
    const res = await this.client.patch(url, data);
    return res.data;
  }

  async delete(url, data) {
    const res = await this.client.delete(url, { data });
    return res.data;
  }
}
