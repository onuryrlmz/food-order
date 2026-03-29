/**
 * Shared interceptors for Next.js web projects (seller + admin).
 * Handles business error conversion and 401 token refresh queue.
 *
 * @param {import('axios').AxiosInstance} client
 * @param {Object} options
 * @param {string} options.loginFlag - localStorage key to clear on auth failure
 * @param {string} [options.loginPath] - redirect path on auth failure
 */
export function attachWebInterceptors(client, { loginFlag, loginPath = '/login' }) {
  let isRefreshing = false;
  let failedQueue = [];

  const processQueue = (error) => {
    failedQueue.forEach((prom) => (error ? prom.reject(error) : prom.resolve()));
    failedQueue = [];
  };

  // Business error: hasFailed === true → reject as Error
  client.interceptors.response.use(
    (res) => {
      const body = res.data;
      if (body && body.hasFailed === true) {
        const msg = body.messages?.[0]?.description || 'Bir hata oluştu';
        const error = new Error(msg);
        error.response = res;
        error.isBusinessError = true;
        return Promise.reject(error);
      }
      return res;
    },
    async (err) => {
      const originalRequest = err.config;
      if (err.response?.status === 401 && !originalRequest._retry) {
        if (isRefreshing) {
          return new Promise((resolve, reject) => {
            failedQueue.push({ resolve, reject });
          }).then(() => client(originalRequest));
        }
        originalRequest._retry = true;
        isRefreshing = true;
        try {
          await client.post('/v1/auth/refresh');
          processQueue(null);
          return client(originalRequest);
        } catch (refreshError) {
          processQueue(refreshError);
          if (typeof window !== 'undefined') {
            localStorage.removeItem(loginFlag);
            window.location.href = loginPath;
          }
          return Promise.reject(refreshError);
        } finally {
          isRefreshing = false;
        }
      }
      return Promise.reject(err);
    },
  );
}
