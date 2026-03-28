import React, { createContext, useContext, useState, useEffect } from 'react';
import * as SecureStore from 'expo-secure-store';
import { authService } from '../api/authService';
import { registerForPushNotifications, clearPushToken } from '../utils/notifications';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  const checkAuth = async () => {
    try {
      const token = await SecureStore.getItemAsync('auth_token');
      if (token) {
        const response = await authService.getProfile();
        setUser(response.data.data);
        setIsAuthenticated(true);
      }
    } catch (error) {
      await SecureStore.deleteItemAsync('auth_token');
      await SecureStore.deleteItemAsync('refresh_token');
      setIsAuthenticated(false);
      setUser(null);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    checkAuth();
  }, []);

  const login = async (email, password) => {
    const response = await authService.login(email, password);
    const { token, refreshToken, user: userData } = response.data;
    await SecureStore.setItemAsync('auth_token', token);
    await SecureStore.setItemAsync('refresh_token', refreshToken);
    setUser(userData);
    setIsAuthenticated(true);
    registerForPushNotifications(userData.id);
    return response.data;
  };

  const register = async (data) => {
    const response = await authService.register(data);
    const { token, refreshToken, user: userData } = response.data;
    await SecureStore.setItemAsync('auth_token', token);
    await SecureStore.setItemAsync('refresh_token', refreshToken);
    setUser(userData);
    setIsAuthenticated(true);
    registerForPushNotifications(userData.id);
    return response.data;
  };

  const logout = async () => {
    try {
      await clearPushToken();
      const refreshToken = await SecureStore.getItemAsync('refresh_token');
      if (refreshToken) {
        await authService.logout(refreshToken);
      }
    } catch (e) {}
    await SecureStore.deleteItemAsync('auth_token');
    await SecureStore.deleteItemAsync('refresh_token');
    setUser(null);
    setIsAuthenticated(false);
  };

  const refreshProfile = async () => {
    try {
      const response = await authService.getProfile();
      setUser(response.data.data);
    } catch (e) {}
  };

  return (
    <AuthContext.Provider value={{ user, isLoading, isAuthenticated, login, register, logout, refreshProfile }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) throw new Error('useAuth must be used within an AuthProvider');
  return context;
};
