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

// --- Company APIs ---

export const registerCompany = async data => {
  const res = await apiClient.post('/v1/courier/company/register', data);
  return res.data;
};

export const getMyCompany = async () => {
  const res = await apiClient.get('/v1/courier/company/my');
  return res.data;
};

export const updateCompany = async data => {
  const res = await apiClient.put('/v1/courier/company/my', data);
  return res.data;
};

export const searchCouriers = async query => {
  const res = await apiClient.get(
    `/v1/courier/company/members/search?q=${query}`,
  );
  return res.data;
};

export const requestMembership = async courierId => {
  const res = await apiClient.post('/v1/courier/company/members/request', {
    courierId,
  });
  return res.data;
};

export const getCompanyMembers = async () => {
  const res = await apiClient.get('/v1/courier/company/members');
  return res.data;
};

export const removeMember = async id => {
  const res = await apiClient.delete(`/v1/courier/company/members/${id}`);
  return res.data;
};

export const getCompanyInvites = async () => {
  const res = await apiClient.get('/v1/courier/company-invites');
  return res.data;
};

export const acceptCompanyInvite = async id => {
  const res = await apiClient.put(`/v1/courier/company-invites/${id}/accept`);
  return res.data;
};

export const rejectCompanyInvite = async id => {
  const res = await apiClient.put(`/v1/courier/company-invites/${id}/reject`);
  return res.data;
};

export const leaveCompany = async () => {
  const res = await apiClient.put('/v1/courier/company/leave');
  return res.data;
};

export const getRestaurantInvites = async () => {
  const res = await apiClient.get('/v1/courier/company/restaurant-invites');
  return res.data;
};

export const acceptRestaurantInvite = async id => {
  const res = await apiClient.put(
    `/v1/courier/company/restaurant-invites/${id}/accept`,
  );
  return res.data;
};

export const rejectRestaurantInvite = async id => {
  const res = await apiClient.put(
    `/v1/courier/company/restaurant-invites/${id}/reject`,
  );
  return res.data;
};

// --- Pickup & Delivery APIs ---

export const getPendingPickups = async (restaurantId, page = 1, size = 20) => {
  const res = await apiClient.get(
    `/v1/courier/pickup/${restaurantId}/pending?page=${page}&size=${size}`,
  );
  return res.data;
};

export const confirmPickup = async orderId => {
  const res = await apiClient.put(`/v1/courier/pickup/${orderId}/confirm`);
  return res.data;
};

export const deliverOrder = async orderId => {
  const res = await apiClient.put(`/v1/courier/orders/${orderId}/deliver`);
  return res.data;
};
