import React, { createContext, useContext, useState, useCallback, useEffect } from 'react';
import { Alert } from 'react-native';
import * as Location from 'expo-location';
import { courierService } from '../api/courierService';
import {
  startBackgroundLocation,
  stopBackgroundLocation,
  BACKGROUND_LOCATION_TASK,
} from '../utils/backgroundLocation';

const LocationContext = createContext(null);

export const LocationProvider = ({ children }) => {
  const [currentPosition, setCurrentPosition] = useState(null);
  const [isTracking, setIsTracking] = useState(false);

  // Uygulama açıldığında arka plan görevin çalışıp çalışmadığını kontrol et
  useEffect(() => {
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

  const startTracking = useCallback(async () => {
    if (isTracking) return true;

    // Konum izni ve ilk konum al
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

    // Arka plan konum takibini başlat
    const started = await startBackgroundLocation();
    if (!started) {
      Alert.alert(
        'Arka Plan Konum İzni',
        'Uygulama kapalıyken bile konum takibi için "Her zaman izin ver" seçeneğini seçmeniz gerekmektedir.',
      );
      return false;
    }

    // Ön plan konum güncellemeleri (UI için)
    await Location.watchPositionAsync(
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

    setIsTracking(true);
    return true;
  }, [isTracking, getCurrentPosition]);

  const stopTracking = useCallback(async () => {
    await stopBackgroundLocation();
    setIsTracking(false);
  }, []);

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
