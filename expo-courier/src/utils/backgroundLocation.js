import * as TaskManager from 'expo-task-manager';
import * as Location from 'expo-location';
import * as SecureStore from 'expo-secure-store';
import axios from 'axios';
import { API_BASE_URL } from './constants';

export const BACKGROUND_LOCATION_TASK = 'BACKGROUND_LOCATION_TASK';

// Define the background task — this MUST be at the top level (outside components)
TaskManager.defineTask(BACKGROUND_LOCATION_TASK, async ({ data, error }) => {
  if (error) {
    console.warn('Background location error:', error.message);
    return;
  }

  if (data) {
    const { locations } = data;
    const location = locations[0];
    if (!location) return;

    try {
      const token = await SecureStore.getItemAsync('auth_token');
      if (!token) return;

      await axios.put(
        `${API_BASE_URL}/courier/location`,
        {
          latitude: location.coords.latitude,
          longitude: location.coords.longitude,
        },
        {
          headers: { Authorization: `Bearer ${token}` },
          timeout: 10000,
        }
      );
    } catch (e) {
      // Silent fail for background updates
    }
  }
});

export const startBackgroundLocation = async () => {
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
  const hasStarted = await Location.hasStartedLocationUpdatesAsync(BACKGROUND_LOCATION_TASK);
  if (hasStarted) {
    await Location.stopLocationUpdatesAsync(BACKGROUND_LOCATION_TASK);
  }
};
