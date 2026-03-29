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
    } else {
      // iOS
      const status = await Geolocation.requestAuthorization('whenInUse');
      return status === 'granted';
    }
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

  // Konum gönderiminde backend'den çevrimiçi durumunu kontrol et
  const sendLocationWithCheck = useCallback(async (stopTrackingFn) => {
    try {
      const coords = await getCurrentPosition();

      // Backend'den profil al — çevrimiçi mi kontrol et
      try {
        const profileRes = await courierService.getProfile();
        const status = profileRes.data?.data?.availabilityStatusId;

        if (status === 0) {
          // Backend çevrimdışı diyor — takibi durdur
          stopTrackingFn();
          return;
        }
      } catch (profileErr) {
        // Profil alınamazsa yine de konum gönder
      }

      await courierService.updateLocation(coords.latitude, coords.longitude);
    } catch (error) {
      // Konum alınamazsa sessizce geç
    }
  }, [getCurrentPosition]);

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
      const coords = await getCurrentPosition();
      await courierService.updateLocation(coords.latitude, coords.longitude);
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

    // Her 30 saniyede konum gönder + backend'den çevrimiçi durumunu kontrol et
    intervalRef.current = setInterval(() => sendLocationWithCheck(stopTracking), LOCATION_INTERVAL);
    setIsTracking(true);
    return true;
  }, [isTracking, getCurrentPosition, sendLocationWithCheck, stopTracking]);

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
