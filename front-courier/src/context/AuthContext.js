import React, {createContext, useContext, useState, useEffect} from 'react';
import AsyncStorage from '@react-native-async-storage/async-storage';
import apiClient from '../api/client';

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
      if (!token) {
        return;
      }

      try {
        const profileResponse = await apiClient.get('/courier/profile');
        if (!profileResponse.data.hasFailed && profileResponse.data.data) {
          setUser(profileResponse.data.data);
          setIsAuthenticated(true);
        } else {
          // Token valid but no courier profile yet (pending registration)
          setUser(null);
          setIsAuthenticated(true);
        }
      } catch (profileError) {
        if (profileError.response?.status === 401) {
          // Token expired or invalid
          await AsyncStorage.multiRemove(['auth_token', 'refresh_token']);
          setUser(null);
          setIsAuthenticated(false);
        } else {
          // Profile fetch failed for other reasons (404 = no courier record)
          // Token is still valid, user is authenticated but has no courier profile
          setUser(null);
          setIsAuthenticated(true);
        }
      }
    } catch (error) {
      await AsyncStorage.multiRemove(['auth_token', 'refresh_token']);
      setUser(null);
      setIsAuthenticated(false);
    } finally {
      setIsLoading(false);
    }
  };

  const login = async (email, password, {skipProfileFetch = false} = {}) => {
    try {
      const response = await apiClient.post('/auth/login', {email, password});

      if (response.data.hasFailed) {
        const errorMsg =
          response.data.messages?.[0]?.description || 'Giris basarisiz';
        return {success: false, error: errorMsg};
      }

      // Store tokens
      if (response.data.token) {
        await AsyncStorage.setItem('auth_token', response.data.token);
      }
      if (response.data.refreshToken) {
        await AsyncStorage.setItem('refresh_token', response.data.refreshToken);
      }

      setIsAuthenticated(true);

      if (skipProfileFetch) {
        return {success: true, hasCourierProfile: false};
      }

      // Try to fetch courier profile
      let hasCourierProfile = false;
      try {
        const profileResponse = await apiClient.get('/courier/profile');
        if (!profileResponse.data.hasFailed && profileResponse.data.data) {
          setUser(profileResponse.data.data);
          hasCourierProfile = true;
        }
      } catch (profileErr) {
        // No courier profile — that's OK, user just hasn't registered as courier yet
      }

      return {success: true, hasCourierProfile};
    } catch (error) {
      const errorMsg =
        error.response?.data?.messages?.[0]?.description ||
        'Baglanti hatasi olustu';
      return {success: false, error: errorMsg};
    }
  };

  const register = async (firstName, lastName, email, phoneNumber, password) => {
    try {
      const response = await apiClient.post('/auth/register', {
        firstName,
        lastName,
        email,
        phoneNumber,
        password,
      });

      if (response.data.hasFailed) {
        const errorMsg =
          response.data.messages?.[0]?.description || 'Kayit basarisiz';
        return {success: false, error: errorMsg};
      }

      // Auto-login after successful registration
      const loginResult = await login(email, password, {
        skipProfileFetch: true,
      });

      if (!loginResult.success) {
        return {
          success: false,
          error: loginResult.error || 'Otomatik giris basarisiz',
        };
      }

      return {success: true};
    } catch (error) {
      const errorMsg =
        error.response?.data?.messages?.[0]?.description ||
        'Baglanti hatasi olustu';
      return {success: false, error: errorMsg};
    }
  };

  const logout = async () => {
    try {
      const refreshToken = await AsyncStorage.getItem('refresh_token');
      if (refreshToken) {
        await apiClient.post('/auth/logout', {refreshToken});
      }
    } catch (e) {
      // Logout API call failed — clear local state anyway
    }
    await AsyncStorage.multiRemove(['auth_token', 'refresh_token']);
    setUser(null);
    setIsAuthenticated(false);
  };

  const refreshProfile = async () => {
    try {
      const response = await apiClient.get('/courier/profile');
      if (!response.data.hasFailed && response.data.data) {
        setUser(response.data.data);
        return true;
      }
      return false;
    } catch (e) {
      return false;
    }
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
