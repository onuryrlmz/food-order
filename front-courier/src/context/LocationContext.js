import React, {createContext, useContext, useState, useEffect, useCallback} from 'react';
import {Platform, Alert} from 'react-native';
import BackgroundGeolocation from '@mauron85/react-native-background-geolocation';
import AsyncStorage from '@react-native-async-storage/async-storage';
import {courierService} from '../api/courierService';
import {API_BASE_URL} from '../utils/constants';

const LocationContext = createContext(null);

export const LocationProvider = ({children}) => {
  const [currentPosition, setCurrentPosition] = useState(null);
  const [isTracking, setIsTracking] = useState(false);

  // Arka plan konum servisini yapılandır (bir kez)
  useEffect(() => {
    BackgroundGeolocation.configure({
      desiredAccuracy: BackgroundGeolocation.HIGH_ACCURACY,
      stationaryRadius: 30,
      distanceFilter: 50,
      interval: 30000, // 30 saniye
      fastestInterval: 15000,
      activitiesInterval: 30000,
      // Uygulama kapatılsa bile çalışmaya devam et
      stopOnTerminate: false,
      startOnBoot: false,
      // Android foreground service bildirimi
      notificationTitle: 'Konum takibi aktif',
      notificationText: 'Teslimat için konumunuz takip ediliyor',
      notificationIconColor: '#007AFF',
      // iOS
      saveBatteryOnBackground: true,
      // Konum URL'e otomatik POST yapmasın — biz manuel göndereceğiz
      url: null,
      syncUrl: null,
    });

    // Konum güncellemelerini dinle
    BackgroundGeolocation.on('location', async (location) => {
      setCurrentPosition({
        latitude: location.latitude,
        longitude: location.longitude,
      });

      // Backend'e konum gönder
      try {
        const token = await AsyncStorage.getItem('auth_token');
        if (!token) return;

        // Önce çevrimiçi mi kontrol et
        const profileRes = await fetch(`${API_BASE_URL}/courier/profile`, {
          headers: {Authorization: `Bearer ${token}`},
        });
        const profileData = await profileRes.json();
        const status = profileData?.data?.availabilityStatusId;

        if (status === 0) {
          // Çevrimdışı — takibi durdur
          BackgroundGeolocation.stop();
          setIsTracking(false);
          return;
        }

        // Konum gönder
        await fetch(`${API_BASE_URL}/courier/location`, {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify({
            latitude: location.latitude,
            longitude: location.longitude,
          }),
        });
      } catch (e) {
        // Ağ hatası — bir sonraki güncelleme tekrar dener
      }

      // Arka planda olduğumuzu kütüphaneye bildir
      BackgroundGeolocation.finish();
    });

    BackgroundGeolocation.on('error', (error) => {
      console.log('Background location error:', error.message);
    });

    // Servis zaten çalışıyorsa (uygulama restart sonrası) state'i güncelle
    BackgroundGeolocation.checkStatus(({isRunning}) => {
      if (isRunning) {
        setIsTracking(true);
      }
    });

    return () => {
      BackgroundGeolocation.removeAllListeners();
    };
  }, []);

  const getCurrentPosition = useCallback(() => {
    return new Promise((resolve, reject) => {
      BackgroundGeolocation.getCurrentLocation(
        (location) => {
          const coords = {
            latitude: location.latitude,
            longitude: location.longitude,
          };
          setCurrentPosition(coords);
          resolve(coords);
        },
        (error) => {
          reject(error);
        },
        {
          timeout: 15000,
          maximumAge: 10000,
          enableHighAccuracy: true,
        },
      );
    });
  }, []);

  const startTracking = useCallback(async () => {
    if (isTracking) return true;

    // İzin kontrolü — BackgroundGeolocation kendi izin yönetimini yapar
    // ama ilk konumu almayı deneyelim
    try {
      const coords = await getCurrentPosition();
      await courierService.updateLocation(coords.latitude, coords.longitude);
    } catch (e) {
      Alert.alert('Konum Hatası', 'Konumunuz alınamadı. GPS açık olduğundan ve konum izni verdiğinizden emin olun.');
      return false;
    }

    BackgroundGeolocation.start();
    setIsTracking(true);
    return true;
  }, [isTracking, getCurrentPosition]);

  const stopTracking = useCallback(() => {
    BackgroundGeolocation.stop();
    setIsTracking(false);
  }, []);

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
