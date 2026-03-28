import apiClient from './client';

export const cuisineService = {
  getList: () =>
    apiClient.get('/cuisine'),
};
