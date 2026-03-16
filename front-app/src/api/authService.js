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

  logout: (refreshToken) =>
    apiClient.post('/auth/logout', {refreshToken}),

  forgotPassword: (emailOrPhone) =>
    apiClient.post('/auth/forgot-password', {emailOrPhone}),

  verifyResetCode: (emailOrPhone, code) =>
    apiClient.post('/auth/verify-reset-code', {emailOrPhone, code}),

  resetPassword: (emailOrPhone, code, newPassword) =>
    apiClient.post('/auth/reset-password', {emailOrPhone, code, newPassword}),
};
