import BaseService from '../base.js';
import {
  CourierRegisterRequestSchema,
  UpdateCourierProfileRequestSchema,
  UpdateLocationRequestSchema,
  AssignmentIdSchema,
  AgreementIdSchema,
  AssignmentHistoryQuerySchema,
  EarningsQuerySchema,
} from '../../schema/courier/courier.js';

export default class CourierService extends BaseService {
  register(data) {
    const parsed = CourierRegisterRequestSchema.parse(data);
    return this.post('/courier/register', parsed);
  }

  getProfile() {
    return this.get('/courier/profile');
  }

  updateProfile(data) {
    const parsed = UpdateCourierProfileRequestSchema.parse(data);
    return this.put('/courier/profile', parsed);
  }

  goOnline() {
    return this.post('/courier/go-online');
  }

  goOffline() {
    return this.post('/courier/go-offline');
  }

  updateLocation(data) {
    const parsed = UpdateLocationRequestSchema.parse(data);
    return this.put('/courier/location', parsed);
  }

  getActiveAssignment() {
    return this.get('/courier/assignment/active');
  }

  getAssignmentHistory(data) {
    const parsed = AssignmentHistoryQuerySchema.parse(data || {});
    return this.get('/courier/assignment/history', parsed);
  }

  acceptAssignment(data) {
    const { assignmentId } = AssignmentIdSchema.parse(data);
    return this.post(`/courier/assignment/${assignmentId}/accept`);
  }

  rejectAssignment(data) {
    const { assignmentId } = AssignmentIdSchema.parse(data);
    return this.post(`/courier/assignment/${assignmentId}/reject`);
  }

  markPickedUp(data) {
    const { assignmentId } = AssignmentIdSchema.parse(data);
    return this.post(`/courier/assignment/${assignmentId}/picked-up`);
  }

  markDelivered(data) {
    const { assignmentId } = AssignmentIdSchema.parse(data);
    return this.post(`/courier/assignment/${assignmentId}/delivered`);
  }

  getAgreements() {
    return this.get('/courier/agreements');
  }

  acceptAgreement(data) {
    const { agreementId } = AgreementIdSchema.parse(data);
    return this.post(`/courier/agreements/${agreementId}/accept`);
  }

  rejectAgreement(data) {
    const { agreementId } = AgreementIdSchema.parse(data);
    return this.post(`/courier/agreements/${agreementId}/reject`);
  }

  terminateAgreement(data) {
    const { agreementId } = AgreementIdSchema.parse(data);
    return this.post(`/courier/agreements/${agreementId}/terminate`);
  }

  getEarnings(data) {
    const parsed = EarningsQuerySchema.parse(data || {});
    return this.get('/courier/earnings', parsed);
  }

  getEarningSummary() {
    return this.get('/courier/earnings/summary');
  }
}
