import React, {createContext, useContext, useState, useEffect, useRef} from 'react';
import * as Location from 'expo-location';
import {restaurantService, cuisineService, addressService} from '../api';
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

  // Disaridan tetiklenir (onboarding tamamlandiktan sonra)
  const enableLoading = () => setEnabled(true);

  useEffect(() => {
    if (!authLoading && enabled) {
      loadAppData();
    }
  }, [authLoading, isAuthenticated, enabled]);

  const getDeviceLocation = async () => {
    try {
      const { status } = await Location.requestForegroundPermissionsAsync();
      if (status !== 'granted') {
        return null;
      }
      const location = await Location.getCurrentPositionAsync({
        accuracy: Location.Accuracy.High,
      });
      return {
        latitude: location.coords.latitude,
        longitude: location.coords.longitude,
      };
    } catch (error) {
      console.log('Location error:', error);
      return null;
    }
  };

  const loadAppData = async () => {
    try {
      // Cuisine listesi
      const cuisineRes = await cuisineService.getList().catch(() => null);
      const cuisineList = cuisineRes?.data?.data || cuisineRes?.data?.rawData || [];
      if (Array.isArray(cuisineList)) {
        setCuisines(cuisineList);
      }

      // Restoran listesi
      if (isAuthenticated) {
        const addressRes = await addressService.getList().catch(() => null);
        const addrList = addressRes?.data?.data || addressRes?.data?.rawData || [];
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
      const res = await restaurantService.getRestaurants(addressId);
      if (res.data?.data) {
        setRestaurants(res.data.data);
      }
    } catch (e) {}
  };

  const loadRestaurantsByDeviceLocation = async () => {
    try {
      const coords = await getDeviceLocation();
      if (!coords) return;
      const res = await restaurantService.getRestaurantsByLocation(coords.latitude, coords.longitude);
      if (res.data?.data) {
        setRestaurants(res.data.data);
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

    // Cache'de varsa ve force degilse direkt kullan
    if (!forceRefresh && restaurantJsonCache.current[restaurantId]) {
      setSelectedRestaurantJson(restaurantJsonCache.current[restaurantId]);
      return;
    }

    setSelectedRestaurantJson(null);

    // CDN JSON'u yukle
    try {
      const res = await restaurantService.getRestaurantInfo(restaurantId);
      if (res.data && !res.data.hasFailed) {
        const cdnUrl = res.data.data;
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
      const res = await addressService.getList();
      const list = res.data?.data || res.data?.rawData || [];
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
