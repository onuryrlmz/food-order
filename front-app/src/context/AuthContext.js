import React, {createContext, useContext, useState, useEffect} from 'react';
import AsyncStorage from '@react-native-async-storage/async-storage';
import {authService} from '../api';

const AuthContext = createContext(null);

export const AuthProvider = ({children}) => {
  const [user, setUser] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  useEffect(() => {
    checkAuth();
  }, []);

  const checkAuth = async () => {
    try {
      const token = await AsyncStorage.getItem('auth_token');
      if (token) {
        const response = await authService.getProfile();
        if (!response.data.hasFailed) {
          setUser(response.data.data);
          setIsAuthenticated(true);
        } else {
          await AsyncStorage.removeItem('auth_token');
        }
      }
    } catch (error) {
      await AsyncStorage.removeItem('auth_token');
    } finally {
      setIsLoading(false);
    }
  };

  const login = async (email, password) => {
    try {
      const response = await authService.login(email, password);
      if (!response.data.hasFailed) {
        if (response.data.token) {
          await AsyncStorage.setItem('auth_token', response.data.token);
        }
        const profileResponse = await authService.getProfile();
        if (!profileResponse.data.hasFailed) {
          setUser(profileResponse.data.data);
          setIsAuthenticated(true);
          return {success: true};
        }
      }
      const errorMsg =
        response.data.messages?.[0]?.description || 'Giriş başarısız';
      return {success: false, error: errorMsg};
    } catch (error) {
      return {success: false, error: 'Bağlantı hatası oluştu'};
    }
  };

  const register = async (data) => {
    try {
      const response = await authService.register(data);
      if (!response.data.hasFailed) {
        return {success: true};
      }
      const errorMsg =
        response.data.messages?.[0]?.description || 'Kayıt başarısız';
      return {success: false, error: errorMsg};
    } catch (error) {
      return {success: false, error: 'Bağlantı hatası oluştu'};
    }
  };

  const logout = async () => {
    try {
      await authService.logout();
    } catch (e) {}
    await AsyncStorage.removeItem('auth_token');
    setUser(null);
    setIsAuthenticated(false);
  };

  const refreshProfile = async () => {
    try {
      const response = await authService.getProfile();
      if (!response.data.hasFailed) {
        setUser(response.data.data);
      }
    } catch (e) {}
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        isLoading,
        isAuthenticated,
        login,
        register,
        logout,
        refreshProfile,
      }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
