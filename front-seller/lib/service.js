import axios from 'axios';
import Service from '../../service/services/index.js';
import { attachWebInterceptors } from '../../service/interceptors.js';

const BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL || 'http://localhost:3762';
const INTERCEPTOR_OPTS = { loginFlag: 'seller_logged_in', loginPath: '/login' };

// Raw axios client (baseURL without /v1) — SWR fetcher + direct URL calls
const client = axios.create({
  baseURL: BASE_URL,
  timeout: 15000,
  headers: { 'Content-Type': 'application/json' },
  withCredentials: true,
});
attachWebInterceptors(client, INTERCEPTOR_OPTS);

// Service client (baseURL with /v1) — centralized service methods
const serviceClient = axios.create({
  baseURL: `${BASE_URL}/v1`,
  timeout: 15000,
  headers: { 'Content-Type': 'application/json' },
  withCredentials: true,
});
attachWebInterceptors(serviceClient, INTERCEPTOR_OPTS);

const api = new Service(serviceClient);

export default api;
export { client };
