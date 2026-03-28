import React, {createContext, useContext, useState, useRef, useCallback} from 'react';
import {Platform, PermissionsAndroid, Alert} from 'react-native';
import Geolocation from 'react-native-geolocation-service';
import {courierService} from '../api/courierService';

const LocationContext = createContext(null);

const LOCATION_INTERVAL = 30000; // 30 seconds

export const LocationProvider = ({children}) => {
  const [currentPosition, setCurrentPosition] = useState(null);
  const [isTracking, setIsTracking] = useState(false);
  const intervalRef = useRef(null);
  const watchIdRef = useRef(null);

  const requestPermission = async () => {
    if (Platform.OS === 'android') {
      try {
        const granted = await PermissionsAndroid.request(
          PermissionsAndroid.PERMISSIONS.ACCESS_FINE_LOCATION,
          {
            title: 'Konum İzni',
            message: 'Teslimat yapabilmek için konum izni gereklidir.',
            buttonPositive: 'İzin Ver',
            buttonNegative: 'Reddet',
          },
        );
        return granted === PermissionsAndroid.RESULTS.GRANTED;
      } catch (err) {
        console.log('Permission error:', err);
        return false;
      }
    }
    return true;
  };

  const getCurrentPosition = useCallback(() => {
    return new Promise((resolve, reject) => {
      Geolocation.getCurrentPosition(
        position => {
          const coords = {
            latitude: position.coords.latitude,
            longitude: position.coords.longitude,
          };
          setCurrentPosition(coords);
          resolve(coords);
        },
        error => {
          console.log('Location error:', error.code, error.message);
          reject(error);
        },
        {enableHighAccuracy: true, timeout: 15000, maximumAge: 10000},
      );
    });
  }, []);

  const sendLocation = useCallback(async () => {
    try {
      const coords = await getCurrentPosition();
      await courierService.updateLocation(coords.latitude, coords.longitude);
    } catch (error) {
      // Silent fail for background location updates
    }
  }, [getCurrentPosition]);

  const stopTracking = useCallback(() => {
    if (watchIdRef.current !== null) {
      Geolocation.clearWatch(watchIdRef.current);
      watchIdRef.current = null;
    }
    if (intervalRef.current) {
      clearInterval(intervalRef.current);
      intervalRef.current = null;
    }
    setIsTracking(false);
  }, []);

  const startTracking = useCallback(async () => {
    if (isTracking) {
      return true;
    }

    const hasPermission = await requestPermission();
    if (!hasPermission) {
      Alert.alert(
        'Konum İzni Gerekli',
        'Çevrimiçi olabilmek için konum izni vermeniz gerekmektedir. Lütfen ayarlardan konum iznini açın.',
      );
      return false;
    }

    try {
      await sendLocation();
    } catch (e) {
      Alert.alert('Konum Hatası', 'Konumunuz alınamadı. GPS açık olduğundan emin olun.');
      return false;
    }

    watchIdRef.current = Geolocation.watchPosition(
      position => {
        setCurrentPosition({
          latitude: position.coords.latitude,
          longitude: position.coords.longitude,
        });
      },
      () => {},
      {enableHighAccuracy: true, distanceFilter: 50, interval: 10000, fastestInterval: 5000},
    );

    intervalRef.current = setInterval(sendLocation, LOCATION_INTERVAL);
    setIsTracking(true);
    return true;
  }, [isTracking, sendLocation]);

  return (
    <LocationContext.Provider
      value={{
        currentPosition,
        isTracking,
        startTracking,
        stopTracking,
        getCurrentPosition,
      }}>
      {children}
    </LocationContext.Provider>
  );
};

export const useLocation = () => {
  const context = useContext(LocationContext);
  if (!context) {
    throw new Error('useLocation must be used within a LocationProvider');
  }
  return context;
};
