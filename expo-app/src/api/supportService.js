import apiClient from './client';

export const supportService = {
  createTicket: (data) =>
    apiClient.post('/customer/support/ticket', data),

  sendMessage: (ticketId, data) =>
    apiClient.post(`/customer/support/ticket/${ticketId}/message`, data),

  getTicketDetail: (ticketId) =>
    apiClient.get(`/customer/support/ticket/${ticketId}`),

  getMyTickets: (statusId, page = 1, pageSize = 20) =>
    apiClient.get('/customer/support/tickets', {params: {statusId, page, pageSize}}),

  closeTicket: (ticketId) =>
    apiClient.post(`/customer/support/ticket/${ticketId}/close`),

  rateTicket: (ticketId, data) =>
    apiClient.post(`/customer/support/ticket/${ticketId}/rate`, data),
};
