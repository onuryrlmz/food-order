import React, {createContext, useContext, useState, useEffect, useRef} from 'react';
import Geolocation from 'react-native-geolocation-service';
import api from '../api';
import {useAuth} from './AuthContext';

const AppDataContext = createContext(null);

export const AppDataProvider = ({children}) => {
  const {isAuthenticated, isLoading: authLoading} = useAuth();
  const [cuisines, setCuisines] = useState([]);
  const [restaurants, setRestaurants] = useState([]);
  const [defaultAddress, setDefaultAddress] = useState(null);
  const [dataReady, setDataReady] = useState(false);
  const [enabled, setEnabled] = useState(false);
  const [selectedRestaurant, setSelectedRestaurant] = useState(null);
  const [selectedRestaurantJson, setSelectedRestaurantJson] = useState(null);
  const restaurantJsonCache = useRef({});

  // Dışarıdan tetiklenir (onboarding tamamlandıktan sonra)
  const enableLoading = () => setEnabled(true);

  useEffect(() => {
    if (!authLoading && enabled) {
      loadAppData();
    }
  }, [authLoading, isAuthenticated, enabled]);

  const getDeviceLocation = () => {
    return new Promise((resolve, reject) => {
      Geolocation.getCurrentPosition(
        position => resolve(position.coords),
        error => reject(error),
        {enableHighAccuracy: true, timeout: 10000, maximumAge: 60000},
      );
    });
  };

  const loadAppData = async () => {
    try {
      // Cuisine listesi
      const cuisineResult = await api.cuisine.getList().catch(() => null);
      const cuisineList = cuisineResult?.data || cuisineResult?.rawData || [];
      if (Array.isArray(cuisineList)) {
        setCuisines(cuisineList);
      }

      // Restoran listesi
      if (isAuthenticated) {
        const addressResult = await api.customer.address.getList().catch(() => null);
        const addrList = addressResult?.data || addressResult?.rawData || [];
        const addresses = Array.isArray(addrList) ? addrList : [];
        const def = addresses.find(a => a.isDefault) || addresses[0];
        if (def) {
          setDefaultAddress(def);
          await loadRestaurantsByAddress(def.id);
        } else {
          await loadRestaurantsByDeviceLocation();
        }
      } else {
        await loadRestaurantsByDeviceLocation();
      }
    } catch (e) {
      // Fail silently
    } finally {
      setDataReady(true);
    }
  };

  const loadRestaurantsByAddress = async (addressId) => {
    try {
      const result = await api.customer.restaurant.getByAddress({addressId});
      if (result?.data) {
        setRestaurants(result.data);
      }
    } catch (e) {}
  };

  const loadRestaurantsByDeviceLocation = async () => {
    try {
      const coords = await getDeviceLocation();
      const result = await api.customer.restaurant.getByLocation({latitude: coords.latitude, longitude: coords.longitude});
      if (result?.data) {
        setRestaurants(result.data);
      }
    } catch (e) {}
  };

  const refreshRestaurants = async () => {
    if (defaultAddress?.id) {
      await loadRestaurantsByAddress(defaultAddress.id);
    } else {
      await loadRestaurantsByDeviceLocation();
    }
  };

  const selectRestaurant = async (restaurantId, forceRefresh = false) => {
    // Listeden restoran bilgisini bul ve set et
    const restaurant = restaurants.find(r => r.id === restaurantId);
    setSelectedRestaurant(restaurant || null);

    // Cache'de varsa ve force değilse direkt kullan
    if (!forceRefresh && restaurantJsonCache.current[restaurantId]) {
      setSelectedRestaurantJson(restaurantJsonCache.current[restaurantId]);
      return;
    }

    setSelectedRestaurantJson(null);

    // CDN JSON'u yükle
    try {
      const result = await api.customer.restaurant.getInfo({id: restaurantId});
      if (result && !result.hasFailed) {
        const cdnUrl = result.data;
        if (typeof cdnUrl === 'string' && cdnUrl.startsWith('http')) {
          const jsonRes = await fetch(cdnUrl + "?id=" + new Date().getTime());
          const parsed = await jsonRes.json();
          restaurantJsonCache.current[restaurantId] = parsed;
          setSelectedRestaurantJson(parsed);
        }
      }
    } catch (e) {}
  };

  const refreshAddress = async () => {
    try {
      const result = await api.customer.address.getList();
      const list = result?.data || result?.rawData || [];
      const addresses = Array.isArray(list) ? list : [];
      const def = addresses.find(a => a.isDefault) || addresses[0];
      if (def) {
        setDefaultAddress(def);
      }
    } catch (e) {}
  };

  return (
    <AppDataContext.Provider
      value={{
        cuisines,
        restaurants,
        defaultAddress,
        dataReady,
        refreshRestaurants,
        refreshAddress,
        enableLoading,
        selectedRestaurant,
        selectedRestaurantJson,
        selectRestaurant,
      }}>
      {children}
    </AppDataContext.Provider>
  );
};

export const useAppData = () => {
  const context = useContext(AppDataContext);
  if (!context) {
    throw new Error('useAppData must be used within an AppDataProvider');
  }
  return context;
};
