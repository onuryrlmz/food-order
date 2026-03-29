import React, {createContext, useContext, useState, useEffect} from 'react';
import AsyncStorage from '@react-native-async-storage/async-storage';
import api from '../api';
import {setOneSignalUserId, clearOneSignalUserId} from '../utils/onesignal';

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
        const result = await api.auth.getProfile();
        if (!result.hasFailed) {
          setUser(result.data);
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
      const result = await api.auth.login({email, password});
      if (!result.hasFailed) {
        const profileResult = await api.auth.getProfile();
        if (!profileResult.hasFailed) {
          const userData = profileResult.data;
          setUser(userData);
          setIsAuthenticated(true);
          if (userData?.userId) {
            setOneSignalUserId(userData.userId);
          }
          return {success: true};
        }
      }
      const errorMsg =
        result.messages?.[0]?.description || 'Giriş başarısız';
      return {success: false, error: errorMsg};
    } catch (error) {
      return {success: false, error: 'Bağlantı hatası oluştu'};
    }
  };

  const register = async (data) => {
    try {
      const result = await api.auth.register(data);
      if (!result.hasFailed) {
        return {success: true};
      }
      const errorMsg =
        result.messages?.[0]?.description || 'Kayıt başarısız';
      return {success: false, error: errorMsg};
    } catch (error) {
      return {success: false, error: 'Bağlantı hatası oluştu'};
    }
  };

  const logout = async () => {
    try {
      const refreshToken = await AsyncStorage.getItem('refresh_token');
      await api.auth.logout({refreshToken});
    } catch (e) {}
    clearOneSignalUserId();
    await AsyncStorage.multiRemove(['auth_token', 'refresh_token', 'user_info']);
    setUser(null);
    setIsAuthenticated(false);
  };

  const refreshProfile = async () => {
    try {
      const result = await api.auth.getProfile();
      if (!result.hasFailed) {
        setUser(result.data);
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
