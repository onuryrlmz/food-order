import apiClient from './client';

const PREFIX = '/courier';

export const courierService = {
  // Auth & Profile
  register: data => apiClient.post(`${PREFIX}/register`, data),
  getProfile: () => apiClient.get(`${PREFIX}/profile`),
  updateProfile: data => apiClient.put(`${PREFIX}/profile`, data),

  // Availability
  goOnline: () => apiClient.post(`${PREFIX}/go-online`),
  goOffline: () => apiClient.post(`${PREFIX}/go-offline`),

  // Location (PUT, not POST)
  updateLocation: (latitude, longitude) =>
    apiClient.put(`${PREFIX}/location`, {latitude, longitude}),

  // Assignments
  getActiveAssignment: () => apiClient.get(`${PREFIX}/assignment/active`),
  getAssignmentHistory: (page = 1, pageSize = 20) =>
    apiClient.get(`${PREFIX}/assignment/history`, {params: {page, pageSize}}),
  acceptAssignment: assignmentId =>
    apiClient.post(`${PREFIX}/assignment/${assignmentId}/accept`),
  rejectAssignment: assignmentId =>
    apiClient.post(`${PREFIX}/assignment/${assignmentId}/reject`),
  markPickedUp: assignmentId =>
    apiClient.post(`${PREFIX}/assignment/${assignmentId}/picked-up`),
  markDelivered: assignmentId =>
    apiClient.post(`${PREFIX}/assignment/${assignmentId}/delivered`),

  // Earnings
  getEarnings: (page = 1, pageSize = 20) =>
    apiClient.get(`${PREFIX}/earnings`, {params: {page, pageSize}}),
  getEarningSummary: () => apiClient.get(`${PREFIX}/earnings/summary`),
};

export default courierService;
