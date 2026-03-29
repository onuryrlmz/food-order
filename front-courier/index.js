/**
 * @format
 */

import { AppRegistry } from 'react-native';
import App from './App';
import { name as appName } from './app.json';
import BackgroundFetch from 'react-native-background-fetch';
import AsyncStorage from '@react-native-async-storage/async-storage';
import Geolocation from 'react-native-geolocation-service';
import { API_BASE_URL } from './src/utils/constants';

AppRegistry.registerComponent(appName, () => App);

// Android Headless Task — uygulama tamamen kapalıyken çalışır
BackgroundFetch.registerHeadlessTask(async ({taskId, timeout}) => {
  if (timeout) {
    BackgroundFetch.finish(taskId);
    return;
  }

  try {
    const token = await AsyncStorage.getItem('auth_token');
    if (!token) {
      BackgroundFetch.finish(taskId);
      return;
    }

    const profileRes = await fetch(`${API_BASE_URL}/courier/profile`, {
      headers: {Authorization: `Bearer ${token}`},
    });
    const profileData = await profileRes.json();
    const status = profileData?.data?.availabilityStatusId;

    if (status === 1 || status === 2) {
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
          } catch (e) {}
          BackgroundFetch.finish(taskId);
        },
        () => BackgroundFetch.finish(taskId),
        {enableHighAccuracy: true, timeout: 10000, maximumAge: 5000},
      );
    } else {
      BackgroundFetch.finish(taskId);
    }
  } catch (e) {
    BackgroundFetch.finish(taskId);
  }
});
