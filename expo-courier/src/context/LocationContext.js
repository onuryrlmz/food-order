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

  // Uygulama açıldığında arka plan konum takibinin zaten çalışıp çalışmadığını kontrol et
  useEffect(() => {
    const checkExistingTracking = async () => {
      try {
        const hasStarted = await Location.hasStartedLocationUpdatesAsync(BACKGROUND_LOCATION_TASK);
        if (hasStarted) {
          setIsTracking(true);
          // Mevcut konumu da al
          const location = await Location.getLastKnownPositionAsync();
          if (location) {
            setCurrentPosition({
              latitude: location.coords.latitude,
              longitude: location.coords.longitude,
            });
          }
        }
      } catch (e) {
        // Task henüz tanımlanmamış olabilir, sessizce geç
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

    try {
      const coords = await getCurrentPosition();
      if (!coords) {
        Alert.alert(
          'Konum Izni Gerekli',
          'Cevrimici olabilmek icin konum izni vermeniz gerekmektedir.',
        );
        return false;
      }
      await courierService.updateLocation(coords.latitude, coords.longitude);
    } catch (e) {
      Alert.alert('Konum Hatasi', 'Konumunuz alinamadi. GPS acik oldugundan emin olun.');
      return false;
    }

    const started = await startBackgroundLocation();
    if (!started) {
      Alert.alert(
        'Arka Plan Konum Izni',
        'Uygulama kapaliyken bile konum takibi icin "Her zaman izin ver" secenegini secmeniz gerekmektedir.',
      );
      return false;
    }

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
