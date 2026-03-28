import * as TaskManager from 'expo-task-manager';
import * as Location from 'expo-location';
import * as SecureStore from 'expo-secure-store';
import Constants from 'expo-constants';
import axios from 'axios';
import { API_BASE_URL } from './constants';

export const BACKGROUND_LOCATION_TASK = 'BACKGROUND_LOCATION_TASK';

// Expo Go'da TaskManager arka plan görevleri desteklenmiyor
const isExpoGo = Constants.appOwnership === 'expo';

// Define the background task — this MUST be at the top level (outside components)
// Expo Go'da defineTask çağrılabilir ama çalıştırılamaz
if (!isExpoGo) {
  TaskManager.defineTask(BACKGROUND_LOCATION_TASK, async ({ data, error }) => {
    if (error) {
      console.warn('Background location error:', error.message);
      return;
    }

    if (!data) return;

    const { locations } = data;
    const location = locations[0];
    if (!location) return;

    try {
      const token = await SecureStore.getItemAsync('auth_token');
      if (!token) return;

      const headers = { Authorization: `Bearer ${token}` };

      // Önce backend'den kurye profili al — çevrimiçi mi kontrol et
      const profileRes = await axios.get(`${API_BASE_URL}/courier/profile`, {
        headers,
        timeout: 10000,
      });

      const availabilityStatus = profileRes.data?.data?.availabilityStatus;

      // Çevrimdışı (0) ise konum gönderme, takibi durdur
      if (availabilityStatus === 0) {
        await Location.stopLocationUpdatesAsync(BACKGROUND_LOCATION_TASK);
        return;
      }

      // Çevrimiçi (1) veya teslimat'ta (2) — konum gönder
      await axios.put(
        `${API_BASE_URL}/courier/location`,
        {
          latitude: location.coords.latitude,
          longitude: location.coords.longitude,
        },
        { headers, timeout: 10000 }
      );
    } catch (e) {
      // Ağ hatası durumunda sessizce geç
    }
  });
}

export const startBackgroundLocation = async () => {
  // Expo Go'da arka plan konum çalışmaz — sadece foreground tracking kullan
  if (isExpoGo) {
    console.log('Background location is not supported in Expo Go. Use a development build.');
    return false;
  }

  const { status: foregroundStatus } = await Location.requestForegroundPermissionsAsync();
  if (foregroundStatus !== 'granted') {
    return false;
  }

  const { status: backgroundStatus } = await Location.requestBackgroundPermissionsAsync();
  if (backgroundStatus !== 'granted') {
    return false;
  }

  const isTaskDefined = TaskManager.isTaskDefined(BACKGROUND_LOCATION_TASK);
  if (!isTaskDefined) {
    console.warn('Background location task is not defined');
    return false;
  }

  const hasStarted = await Location.hasStartedLocationUpdatesAsync(BACKGROUND_LOCATION_TASK);
  if (hasStarted) {
    return true;
  }

  await Location.startLocationUpdatesAsync(BACKGROUND_LOCATION_TASK, {
    accuracy: Location.Accuracy.High,
    timeInterval: 30000,
    distanceInterval: 50,
    deferredUpdatesInterval: 30000,
    showsBackgroundLocationIndicator: true,
    foregroundService: {
      notificationTitle: 'Konum takibi aktif',
      notificationBody: 'Teslimat için konumunuz takip ediliyor',
      notificationColor: '#007AFF',
    },
  });

  return true;
};

export const stopBackgroundLocation = async () => {
  if (isExpoGo) return;

  try {
    const hasStarted = await Location.hasStartedLocationUpdatesAsync(BACKGROUND_LOCATION_TASK);
    if (hasStarted) {
      await Location.stopLocationUpdatesAsync(BACKGROUND_LOCATION_TASK);
    }
  } catch (e) {
    // Sessizce geç
  }
};
