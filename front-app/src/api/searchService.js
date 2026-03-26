import apiClient from './client';

export const searchService = {
  recordSearch: (data) =>
    apiClient.post('/customer/search/record', data),

  getRecentSearches: (limit = 20) =>
    apiClient.get('/customer/search/recent', {params: {limit}}),

  clearHistory: () =>
    apiClient.delete('/customer/search/history'),

  getPopularSearches: (limit = 10) =>
    apiClient.get('/customer/search/popular', {params: {limit}}),

  getSuggestions: (q, limit = 10) =>
    apiClient.get('/customer/search/suggestions', {params: {q, limit}}),
};
