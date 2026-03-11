import api from './api';

export async function login(email, password) {
  const res = await api.post('/v1/auth/login', { email, password });
  if (res.data?.hasFailed) throw new Error(res.data?.messages?.[0]?.description || 'Giriş başarısız');
  localStorage.setItem('seller_logged_in', '1');
  return res.data;
}

export async function logout() {
  localStorage.removeItem('seller_logged_in');
  try { await api.post('/v1/auth/logout'); } catch { }
  window.location.href = '/login';
}

export function isAuthenticated() {
  if (typeof window === 'undefined') return false;
  return localStorage.getItem('seller_logged_in') === '1';
}
