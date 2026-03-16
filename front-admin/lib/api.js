import axios from 'axios';

const BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL || 'http://localhost:3762';

const api = axios.create({
  baseURL: BASE_URL,
  headers: { 'Content-Type': 'application/json' },
  withCredentials: true,
});

api.interceptors.response.use(
  (res) => res,
  (err) => {
    if (err.response?.status === 401) {
      if (typeof window !== 'undefined') window.location.href = '/login';
    }
    return Promise.reject(err);
  }
);

// Courier Companies
export const getCourierCompanies = (page = 1, size = 20) =>
  api.get(`/v1/admin/courier-companies?page=${page}&size=${size}`).then(r => r.data);

export const approveCourierCompany = (id) =>
  api.put(`/v1/admin/courier-companies/${id}/approve`).then(r => r.data);

export const rejectCourierCompany = (id) =>
  api.put(`/v1/admin/courier-companies/${id}/reject`).then(r => r.data);

export const suspendCourierCompany = (id) =>
  api.put(`/v1/admin/courier-companies/${id}/suspend`).then(r => r.data);

export const banCourierCompany = (id) =>
  api.put(`/v1/admin/courier-companies/${id}/ban`).then(r => r.data);

// iyzico
export const retryIyzicoRegistration = (sellerId) =>
  api.post(`/v1/admin/seller/${sellerId}/retry-iyzico`).then(r => r.data);

export default api;
