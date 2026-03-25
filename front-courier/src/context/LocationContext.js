import React, {createContext, useContext, useState, useRef, useCallback} from 'react';
import {Platform, PermissionsAndroid, Alert} from 'react-native';
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
            title: 'Konum Izni',
            message: 'Teslimat yapabilmek icin konum izni gereklidir.',
            buttonPositive: 'Izin Ver',
            buttonNegative: 'Reddet',
          },
        );
        return granted === PermissionsAndroid.RESULTS.GRANTED;
      } catch (err) {
        return false;
      }
    }
    // iOS handles permission via Info.plist prompt automatically
    return true;
  };

  const getCurrentPosition = useCallback(() => {
    return new Promise((resolve, reject) => {
      navigator.geolocation.getCurrentPosition(
        position => {
          const coords = {
            latitude: position.coords.latitude,
            longitude: position.coords.longitude,
          };
          setCurrentPosition(coords);
          resolve(coords);
        },
        error => reject(error),
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
      navigator.geolocation.clearWatch(watchIdRef.current);
      watchIdRef.current = null;
    }
    if (intervalRef.current) {
      clearInterval(intervalRef.current);
      intervalRef.current = null;
    }
    setIsTracking(false);
  }, []);

  const startTracking = useCallback(async () => {
    // Prevent double tracking
    if (isTracking) {
      return true;
    }

    const hasPermission = await requestPermission();
    if (!hasPermission) {
      Alert.alert(
        'Konum Izni Gerekli',
        'Cevrimici olabilmek icin konum izni vermeniz gerekmektedir.',
      );
      return false;
    }

    // Get initial position and send to server
    await sendLocation();

    // Watch position changes locally
    watchIdRef.current = navigator.geolocation.watchPosition(
      position => {
        setCurrentPosition({
          latitude: position.coords.latitude,
          longitude: position.coords.longitude,
        });
      },
      () => {},
      {enableHighAccuracy: true, distanceFilter: 50},
    );

    // Send location to server periodically
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
