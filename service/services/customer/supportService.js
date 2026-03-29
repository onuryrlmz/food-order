import BaseService from '../base.js';
import { CreateTicketRequestSchema, SendMessageRequestSchema, TicketIdSchema, TicketListQuerySchema, RateTicketRequestSchema } from '../../schema/customer/support.js';

export default class SupportService extends BaseService {
  createTicket(data) {
    const parsed = CreateTicketRequestSchema.parse(data);
    return this.post('/customer/support/ticket', parsed);
  }

  sendMessage(data) {
    const { ticketId, message } = SendMessageRequestSchema.parse(data);
    return this.post(`/customer/support/ticket/${ticketId}/message`, { message });
  }

  getTicket(data) {
    const { ticketId } = TicketIdSchema.parse(data);
    return this.get(`/customer/support/ticket/${ticketId}`);
  }

  getTickets(data) {
    const parsed = TicketListQuerySchema.parse(data || {});
    return this.get('/customer/support/tickets', parsed);
  }

  closeTicket(data) {
    const { ticketId } = TicketIdSchema.parse(data);
    return this.post(`/customer/support/ticket/${ticketId}/close`);
  }

  rateTicket(data) {
    const { ticketId, rating } = RateTicketRequestSchema.parse(data);
    return this.post(`/customer/support/ticket/${ticketId}/rate`, { rating });
  }
}
