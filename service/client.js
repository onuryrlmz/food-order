import axios from 'axios';

/**
 * Configurable axios client factory.
 * @param {Object} config
 * @param {string} config.baseURL - API base URL
 * @param {Function} [config.getToken] - Async function returning auth token (RN: AsyncStorage, Web: cookie)
 * @param {Function} [config.onUnauthorized] - Called on 401 (e.g. clear token, redirect)
 * @param {boolean} [config.withCredentials] - Send cookies (for web apps)
 */
export function createClient(config) {
  const client = axios.create({
    baseURL: config.baseURL,
    timeout: 15000,
    headers: { 'Content-Type': 'application/json' },
    withCredentials: config.withCredentials || false,
  });

  // Request: inject Bearer token
  if (config.getToken) {
    client.interceptors.request.use(async (req) => {
      const token = await config.getToken();
      if (token) req.headers.Authorization = `Bearer ${token}`;
      return req;
    });
  }

  // Response: persist tokens + handle 401
  client.interceptors.response.use(
    (res) => {
      if (config.onTokenReceived) {
        if (res.data?.token) config.onTokenReceived('auth_token', res.data.token);
        if (res.data?.refreshToken) config.onTokenReceived('refresh_token', res.data.refreshToken);
      }
      return res;
    },
    async (error) => {
      if (error.response?.status === 401 && config.onUnauthorized) {
        config.onUnauthorized(error);
      }
      return Promise.reject(error);
    },
  );

  return client;
}
