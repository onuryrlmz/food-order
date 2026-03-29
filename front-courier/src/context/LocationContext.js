import React, {createContext, useContext, useState, useRef, useCallback, useEffect} from 'react';
import {Platform, PermissionsAndroid, Alert, AppState} from 'react-native';
import Geolocation from 'react-native-geolocation-service';
import BackgroundFetch from 'react-native-background-fetch';
import AsyncStorage from '@react-native-async-storage/async-storage';
import api from '../api';
import {API_BASE_URL} from '../utils/constants';

const LocationContext = createContext(null);

const LOCATION_INTERVAL = 30000; // 30 saniye

export const LocationProvider = ({children}) => {
  const [currentPosition, setCurrentPosition] = useState(null);
  const [isTracking, setIsTracking] = useState(false);
  const intervalRef = useRef(null);
  const watchIdRef = useRef(null);

  // Background Fetch yapılandırması — uygulama kapalıyken konum gönderimi
  useEffect(() => {
    const initBackgroundFetch = async () => {
      await BackgroundFetch.configure(
        {
          minimumFetchInterval: 15, // dakika (iOS minimum 15dk)
          stopOnTerminate: false,   // Android: uygulama kapansa bile çalış
          startOnBoot: true,        // Android: cihaz açılınca başla
          enableHeadless: true,     // Android: headless task
          requiredNetworkType: BackgroundFetch.NETWORK_TYPE_ANY,
        },
        async (taskId) => {
          // Arka plan görevi çalıştığında
          try {
            const token = await AsyncStorage.getItem('auth_token');
            if (!token) {
              BackgroundFetch.finish(taskId);
              return;
            }

            // Backend'den çevrimiçi mi kontrol et
            const profileRes = await fetch(`${API_BASE_URL}/courier/profile`, {
              headers: {Authorization: `Bearer ${token}`},
            });
            const profileData = await profileRes.json();
            const status = profileData?.data?.availabilityStatusId;

            if (status === 1 || status === 2) {
              // Çevrimiçi — konum al ve gönder
              Geolocation.getCurrentPosition(
                async (position) => {
                  try {
                    await fetch(`${API_BASE_URL}/courier/location`, {
                      method: 'PUT',
                      headers: {
                        'Content-Type': 'application/json',
                        Authorization: `Bearer ${token}`,
                      },
                      body: JSON.stringify({
                        latitude: position.coords.latitude,
                        longitude: position.coords.longitude,
                      }),
                    });
                  } catch (e) {
                    // Sessizce geç
                  }
                  BackgroundFetch.finish(taskId);
                },
                () => BackgroundFetch.finish(taskId),
                {enableHighAccuracy: true, timeout: 10000, maximumAge: 5000},
              );
            } else {
              // Çevrimdışı — bir şey yapma
              BackgroundFetch.finish(taskId);
            }
          } catch (e) {
            BackgroundFetch.finish(taskId);
          }
        },
        (taskId) => {
          // Timeout callback
          BackgroundFetch.finish(taskId);
        },
      );
    };

    initBackgroundFetch();
  }, []);

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
      // iOS
      const status = await Geolocation.requestAuthorization('always');
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
    // Background fetch'i durdurma — çevrimiçi kontrolünü o kendi yapıyor
    setIsTracking(false);
  }, []);

  // Foreground'da periyodik konum gönderimi + backend durum kontrolü
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

    // Background fetch'i başlat
    BackgroundFetch.start();

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
