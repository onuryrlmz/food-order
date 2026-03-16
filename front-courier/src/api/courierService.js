import apiClient from './client';

export const register = async (email, password, phoneNumber, firstName, lastName) => {
  const res = await apiClient.post('/v1/courier/auth/register', {
    email,
    password,
    phoneNumber,
    firstName,
    lastName,
  });
  return res.data;
};

export const login = async (email, password) => {
  const res = await apiClient.post('/v1/courier/auth/login', {email, password});
  return res.data;
};

export const getActiveOrders = async () => {
  const res = await apiClient.get('/v1/courier/orders/active');
  return res.data;
};

export const getOrderHistory = async (month, year) => {
  const res = await apiClient.get(
    `/v1/courier/orders/history?month=${month}&year=${year}`,
  );
  return res.data;
};

export const updateLocation = async (latitude, longitude, orderId = null) => {
  const res = await apiClient.post('/v1/courier/location', {
    latitude,
    longitude,
    orderId,
  });
  return res.data;
};

export const getMyRestaurants = async () => {
  const res = await apiClient.get('/v1/courier/restaurants');
  return res.data;
};

export const getPendingInvites = async () => {
  const res = await apiClient.get('/v1/courier/restaurants/invites');
  return res.data;
};

export const acceptInvite = async restaurantCourierId => {
  const res = await apiClient.put(
    `/v1/courier/restaurants/${restaurantCourierId}/accept`,
  );
  return res.data;
};

export const rejectInvite = async restaurantCourierId => {
  const res = await apiClient.put(
    `/v1/courier/restaurants/${restaurantCourierId}/reject`,
  );
  return res.data;
};

export const leaveRestaurant = async restaurantCourierId => {
  const res = await apiClient.delete(
    `/v1/courier/restaurants/${restaurantCourierId}`,
  );
  return res.data;
};

export const getProfile = async () => {
  const res = await apiClient.get('/v1/auth/profile');
  return res.data;
};

export const updateProfile = async (firstName, lastName, phoneNumber) => {
  const res = await apiClient.put('/v1/auth/profile', {
    firstName,
    lastName,
    phoneNumber,
  });
  return res.data;
};

export const changePassword = async (currentPassword, newPassword) => {
  const res = await apiClient.put('/v1/auth/change-password', {
    currentPassword,
    newPassword,
  });
  return res.data;
};
