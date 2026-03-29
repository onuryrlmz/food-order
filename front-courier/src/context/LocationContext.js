import React, {createContext, useContext, useState, useRef, useCallback} from 'react';
import {Platform, PermissionsAndroid, Alert} from 'react-native';
import Geolocation from 'react-native-geolocation-service';
import api from '../api';

const LocationContext = createContext(null);

const LOCATION_INTERVAL = 30000; // 30 saniye

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
        return false;
      }
    } else {
      const status = await Geolocation.requestAuthorization('whenInUse');
      return status === 'always' || status === 'whenInUse';
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
        error => reject(error),
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

  const sendLocationWithCheck = useCallback(async () => {
    try {
      const coords = await getCurrentPosition();

      try {
        const profileResult = await api.courier.courier.getProfile();
        const status = profileResult?.data?.availabilityStatusId;

        if (status === 0) {
          stopTracking();
          return;
        }
      } catch (profileErr) {
        // Profil alınamazsa yine de konum gönder
      }

      await api.courier.courier.updateLocation({latitude: coords.latitude, longitude: coords.longitude});
    } catch (error) {
      // Sessizce geç
    }
  }, [getCurrentPosition, stopTracking]);

  const startTracking = useCallback(async () => {
    if (isTracking) return true;

    const hasPermission = await requestPermission();
    if (!hasPermission) {
      Alert.alert(
        'Konum İzni Gerekli',
        'Çevrimiçi olabilmek için konum izni vermeniz gerekmektedir.',
      );
      return false;
    }

    try {
      const coords = await getCurrentPosition();
      await api.courier.courier.updateLocation({latitude: coords.latitude, longitude: coords.longitude});
    } catch (e) {
      Alert.alert('Konum Hatası', 'Konumunuz alınamadı. GPS açık olduğundan emin olun.');
      return false;
    }

    // Foreground konum izleme (UI güncelleme)
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

    // Foreground'da her 30 saniyede konum gönder
    intervalRef.current = setInterval(sendLocationWithCheck, LOCATION_INTERVAL);

    setIsTracking(true);
    return true;
  }, [isTracking, getCurrentPosition, sendLocationWithCheck]);

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
