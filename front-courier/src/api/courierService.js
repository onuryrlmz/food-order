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

  // Agreements
  getMyAgreements: () =>
    apiClient.get(`${PREFIX}/agreements`),
  acceptAgreement: (agreementId) =>
    apiClient.post(`${PREFIX}/agreements/${agreementId}/accept`),
  rejectAgreement: (agreementId) =>
    apiClient.post(`${PREFIX}/agreements/${agreementId}/reject`),
  terminateAgreement: (agreementId) =>
    apiClient.post(`${PREFIX}/agreements/${agreementId}/terminate`),

  // Earnings
  getEarnings: (page = 1, pageSize = 20, from = null, to = null) =>
    apiClient.get(`${PREFIX}/earnings`, {params: {page, pageSize, from, to}}),
  getEarningSummary: () => apiClient.get(`${PREFIX}/earnings/summary`),
};

export default courierService;
