import React, { createContext, useContext, useState, useCallback, useEffect, useRef } from 'react';
import { Alert, AppState } from 'react-native';
import * as Location from 'expo-location';
import Constants from 'expo-constants';
import { courierService } from '../api/courierService';
import {
  startBackgroundLocation,
  stopBackgroundLocation,
  BACKGROUND_LOCATION_TASK,
} from '../utils/backgroundLocation';

const LocationContext = createContext(null);

const isExpoGo = Constants.appOwnership === 'expo';
const LOCATION_INTERVAL = 30000; // 30 saniye

export const LocationProvider = ({ children }) => {
  const [currentPosition, setCurrentPosition] = useState(null);
  const [isTracking, setIsTracking] = useState(false);
  const intervalRef = useRef(null);
  const watchRef = useRef(null);

  // Uygulama açıldığında arka plan görevin çalışıp çalışmadığını kontrol et
  useEffect(() => {
    if (isExpoGo) return;

    const checkExistingTracking = async () => {
      try {
        const hasStarted = await Location.hasStartedLocationUpdatesAsync(BACKGROUND_LOCATION_TASK);
        if (hasStarted) {
          setIsTracking(true);
          const location = await Location.getLastKnownPositionAsync();
          if (location) {
            setCurrentPosition({
              latitude: location.coords.latitude,
              longitude: location.coords.longitude,
            });
          }
        }
      } catch (e) {
        // Task henüz tanımlanmamış olabilir
      }
    };
    checkExistingTracking();
  }, []);

  const getCurrentPosition = useCallback(async () => {
    const { status } = await Location.requestForegroundPermissionsAsync();
    if (status !== 'granted') {
      return null;
    }

    const location = await Location.getCurrentPositionAsync({
      accuracy: Location.Accuracy.High,
    });

    const coords = {
      latitude: location.coords.latitude,
      longitude: location.coords.longitude,
    };
    setCurrentPosition(coords);
    return coords;
  }, []);

  // Expo Go'da foreground interval ile konum gönder
  const startForegroundTracking = useCallback(async () => {
    // İlk konumu gönder
    const sendLocation = async () => {
      try {
        const coords = await getCurrentPosition();
        if (coords) {
          await courierService.updateLocation(coords.latitude, coords.longitude);
        }
      } catch (e) {
        // Sessizce geç
      }
    };

    await sendLocation();

    // Periyodik konum gönderimi
    if (intervalRef.current) clearInterval(intervalRef.current);
    intervalRef.current = setInterval(sendLocation, LOCATION_INTERVAL);

    // Konum değişikliklerini izle (UI güncellemesi için)
    if (watchRef.current) watchRef.current.remove();
    watchRef.current = await Location.watchPositionAsync(
      {
        accuracy: Location.Accuracy.High,
        distanceInterval: 50,
        timeInterval: 10000,
      },
      (location) => {
        setCurrentPosition({
          latitude: location.coords.latitude,
          longitude: location.coords.longitude,
        });
      },
    );
  }, [getCurrentPosition]);

  const stopForegroundTracking = useCallback(() => {
    if (intervalRef.current) {
      clearInterval(intervalRef.current);
      intervalRef.current = null;
    }
    if (watchRef.current) {
      watchRef.current.remove();
      watchRef.current = null;
    }
  }, []);

  const startTracking = useCallback(async () => {
    if (isTracking) return true;

    // Konum izni al
    try {
      const coords = await getCurrentPosition();
      if (!coords) {
        Alert.alert(
          'Konum İzni Gerekli',
          'Çevrimiçi olabilmek için konum izni vermeniz gerekmektedir.',
        );
        return false;
      }
      await courierService.updateLocation(coords.latitude, coords.longitude);
    } catch (e) {
      Alert.alert('Konum Hatası', 'Konumunuz alınamadı. GPS açık olduğundan emin olun.');
      return false;
    }

    if (isExpoGo) {
      // Expo Go: sadece foreground tracking
      await startForegroundTracking();
    } else {
      // Development/Production build: arka plan tracking
      const started = await startBackgroundLocation();
      if (!started) {
        // Fallback: foreground tracking
        await startForegroundTracking();
      } else {
        // Foreground konum izleme (UI güncellemesi için)
        if (watchRef.current) watchRef.current.remove();
        watchRef.current = await Location.watchPositionAsync(
          {
            accuracy: Location.Accuracy.High,
            distanceInterval: 50,
            timeInterval: 10000,
          },
          (location) => {
            setCurrentPosition({
              latitude: location.coords.latitude,
              longitude: location.coords.longitude,
            });
          },
        );
      }
    }

    setIsTracking(true);
    return true;
  }, [isTracking, getCurrentPosition, startForegroundTracking]);

  const stopTracking = useCallback(async () => {
    stopForegroundTracking();
    await stopBackgroundLocation();
    setIsTracking(false);
  }, [stopForegroundTracking]);

  return (
    <LocationContext.Provider value={{ currentPosition, isTracking, startTracking, stopTracking, getCurrentPosition }}>
      {children}
    </LocationContext.Provider>
  );
};

export const useLocation = () => {
  const context = useContext(LocationContext);
  if (!context) throw new Error('useLocation must be used within a LocationProvider');
  return context;
};
