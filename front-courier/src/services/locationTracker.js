import Geolocation from 'react-native-geolocation-service';
import {updateLocation} from '../api/courierService';

let intervalId = null;

export const startTracking = () => {
  if (intervalId) return;

  intervalId = setInterval(async () => {
    Geolocation.getCurrentPosition(
      async position => {
        try {
          await updateLocation(
            position.coords.latitude,
            position.coords.longitude,
          );
        } catch (e) {
          console.warn('Location update failed:', e);
        }
      },
      error => console.warn('GPS error:', error),
      {enableHighAccuracy: true, timeout: 10000},
    );
  }, 15000);
};

export const stopTracking = () => {
  if (intervalId) {
    clearInterval(intervalId);
    intervalId = null;
  }
};

export const isTracking = () => intervalId !== null;
