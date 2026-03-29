import { createClient } from './client.js';
import Service from './services/index.js';

/**
 * Create a configured service instance.
 *
 * Usage (React Native - front-app/front-courier):
 *   import AsyncStorage from '@react-native-async-storage/async-storage';
 *   const api = createService({
 *     baseURL: API_BASE_URL,
 *     getToken: () => AsyncStorage.getItem('auth_token'),
 *     onUnauthorized: () => { ... },
 *   });
 *   const result = await api.auth.login({ email, password });
 *   const restaurants = await api.customer.restaurant.getByLocation({ latitude, longitude });
 *
 * Usage (Next.js - front-seller):
 *   const api = createService({
 *     baseURL: process.env.NEXT_PUBLIC_API_BASE_URL,
 *     withCredentials: true,
 *   });
 *   const list = await api.seller.restaurant.getList();
 */
export function createService(config) {
  const client = createClient(config);
  return new Service(client);
}

// Re-export schemas for direct import
export * from './schema/index.js';

// Re-export client factory
export { createClient } from './client.js';
