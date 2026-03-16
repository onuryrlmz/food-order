import React, {createContext, useContext, useState, useEffect} from 'react';
import AsyncStorage from '@react-native-async-storage/async-storage';
import {login as loginApi} from '../api/courierService';

const AuthContext = createContext(null);

export const AuthProvider = ({children}) => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [courier, setCourier] = useState(null);
  const [token, setToken] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    checkAuth();
  }, []);

  const checkAuth = async () => {
    try {
      const storedToken = await AsyncStorage.getItem('courier_token');
      const storedCourier = await AsyncStorage.getItem('courier_info');
      if (storedToken && storedCourier) {
        setToken(storedToken);
        setCourier(JSON.parse(storedCourier));
        setIsAuthenticated(true);
      }
    } catch (error) {
      console.error('Auth check error:', error);
    } finally {
      setLoading(false);
    }
  };

  const login = async (email, password) => {
    try {
      const result = await loginApi(email, password);
      if (!result.hasFailed && result.data) {
        const {
          userId,
          email: userEmail,
          phoneNumber,
          firstName,
          lastName,
          userRoleId,
          token: newToken,
        } = result.data;

        await AsyncStorage.setItem('courier_token', newToken);
        await AsyncStorage.setItem(
          'courier_info',
          JSON.stringify({userId, email: userEmail, phoneNumber, firstName, lastName, userRoleId}),
        );

        setToken(newToken);
        setCourier({userId, email: userEmail, phoneNumber, firstName, lastName, userRoleId});
        setIsAuthenticated(true);
        return {success: true};
      }
      const errMsg =
        result.messages?.map(m => m.description).join(', ') ||
        'Giriş başarısız.';
      return {success: false, message: errMsg};
    } catch (error) {
      return {success: false, message: error.message};
    }
  };

  const logout = async () => {
    await AsyncStorage.removeItem('courier_token');
    await AsyncStorage.removeItem('courier_info');
    setToken(null);
    setCourier(null);
    setIsAuthenticated(false);
  };

  return (
    <AuthContext.Provider
      value={{
        isAuthenticated,
        courier,
        token,
        loading,
        login,
        logout,
      }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within AuthProvider');
  }
  return context;
};
