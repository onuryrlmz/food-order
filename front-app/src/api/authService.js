import apiClient from './client';

export const authService = {
  login: (email, password) =>
    apiClient.post('/auth/login', {email, password}),

  register: (data) =>
    apiClient.post('/auth/register', data),

  getProfile: () =>
    apiClient.get('/auth/profile'),

  updateProfile: (data) =>
    apiClient.put('/auth/profile', data),

  changePassword: (data) =>
    apiClient.put('/auth/change-password', data),

  logout: () =>
    apiClient.post('/auth/logout'),
};
