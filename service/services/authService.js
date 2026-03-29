import BaseService from './base.js';
import {
  LoginRequestSchema,
  RegisterRequestSchema,
  UpdateProfileRequestSchema,
  ChangePasswordRequestSchema,
  ForgotPasswordRequestSchema,
  VerifyResetCodeRequestSchema,
  ResetPasswordRequestSchema,
  LogoutRequestSchema,
  RefreshTokenRequestSchema,
} from '../schema/auth.js';

export default class AuthService extends BaseService {
  login(data) {
    const parsed = LoginRequestSchema.parse(data);
    return this.post('/auth/login', parsed);
  }

  register(data) {
    const parsed = RegisterRequestSchema.parse(data);
    return this.post('/auth/register', parsed);
  }

  getProfile() {
    return this.get('/auth/profile');
  }

  updateProfile(data) {
    const parsed = UpdateProfileRequestSchema.parse(data);
    return this.put('/auth/profile', parsed);
  }

  changePassword(data) {
    const parsed = ChangePasswordRequestSchema.parse(data);
    return this.put('/auth/change-password', parsed);
  }

  logout(data) {
    const parsed = LogoutRequestSchema.parse(data);
    return this.post('/auth/logout', parsed);
  }

  forgotPassword(data) {
    const parsed = ForgotPasswordRequestSchema.parse(data);
    return this.post('/auth/forgot-password', parsed);
  }

  verifyResetCode(data) {
    const parsed = VerifyResetCodeRequestSchema.parse(data);
    return this.post('/auth/verify-reset-code', parsed);
  }

  resetPassword(data) {
    const parsed = ResetPasswordRequestSchema.parse(data);
    return this.post('/auth/reset-password', parsed);
  }

  refresh(data) {
    const parsed = RefreshTokenRequestSchema.parse(data);
    return this.post('/auth/refresh', parsed);
  }
}
