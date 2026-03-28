import React, { createContext, useContext, useState, useEffect } from 'react';
import * as SecureStore from 'expo-secure-store';
import { courierService } from '../api/courierService';
import apiClient from '../api/client';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  const checkAuth = async () => {
    try {
      const token = await SecureStore.getItemAsync('auth_token');
      if (token) {
        setIsAuthenticated(true);
        try {
          const response = await courierService.getProfile();
          setUser(response.data.data);
        } catch (profileError) {
          if (profileError.response?.status === 401) {
            await SecureStore.deleteItemAsync('auth_token');
            await SecureStore.deleteItemAsync('refresh_token');
            setIsAuthenticated(false);
          }
        }
      }
    } catch (error) {
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
    try {
      const response = await apiClient.post('/auth/login', { email, password });
      const { token, refreshToken } = response.data;
      await SecureStore.setItemAsync('auth_token', token);
      await SecureStore.setItemAsync('refresh_token', refreshToken);
      setIsAuthenticated(true);

      try {
        const profileRes = await courierService.getProfile();
        setUser(profileRes.data.data);
        return { success: true, hasCourierProfile: true };
      } catch (e) {
        return { success: true, hasCourierProfile: false };
      }
    } catch (error) {
      return { success: false, error: error.response?.data?.message || 'Giris basarisiz' };
    }
  };

  const register = async (firstName, lastName, email, phoneNumber, password) => {
    try {
      const response = await apiClient.post('/auth/register', {
        firstName, lastName, email, phoneNumber, password,
      });
      const { token, refreshToken } = response.data;
      await SecureStore.setItemAsync('auth_token', token);
      await SecureStore.setItemAsync('refresh_token', refreshToken);
      setIsAuthenticated(true);
      return { success: true };
    } catch (error) {
      return { success: false, error: error.response?.data?.message || 'Kayit basarisiz' };
    }
  };

  const logout = async () => {
    await SecureStore.deleteItemAsync('auth_token');
    await SecureStore.deleteItemAsync('refresh_token');
    setUser(null);
    setIsAuthenticated(false);
  };

  const refreshProfile = async () => {
    try {
      const response = await courierService.getProfile();
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
