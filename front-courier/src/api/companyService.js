import apiClient from './client';

const PREFIX = '/courier-company';

export const companyService = {
  register: data => apiClient.post(`${PREFIX}/register`, data),
};

export default companyService;
