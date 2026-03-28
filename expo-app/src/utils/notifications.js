import * as Notifications from 'expo-notifications';
import { Platform } from 'react-native';
import { router } from 'expo-router';
import { API_BASE_URL } from './constants';
import * as SecureStore from 'expo-secure-store';
import axios from 'axios';

Notifications.setNotificationHandler({
  handleNotification: async () => ({
    shouldShowAlert: true,
    shouldPlaySound: true,
    shouldSetBadge: true,
  }),
});

export const initNotifications = () => {
  const subscription = Notifications.addNotificationResponseReceivedListener(
    (response) => {
      const data = response.notification.request.content.data;
      if (data?.orderId) {
        router.push(`/order/${data.orderId}`);
      }
    }
  );
  return subscription;
};

export const registerForPushNotifications = async (userId) => {
  const { status: existingStatus } = await Notifications.getPermissionsAsync();
  let finalStatus = existingStatus;

  if (existingStatus !== 'granted') {
    const { status } = await Notifications.requestPermissionsAsync();
    finalStatus = status;
  }

  if (finalStatus !== 'granted') {
    return null;
  }

  const tokenData = await Notifications.getExpoPushTokenAsync();
  const pushToken = tokenData.data;

  try {
    const authToken = await SecureStore.getItemAsync('auth_token');
    await axios.post(
      `${API_BASE_URL}/auth/push-token`,
      { token: pushToken, platform: Platform.OS },
      { headers: { Authorization: `Bearer ${authToken}` } }
    );
  } catch (e) {
    // Silent fail
  }

  return pushToken;
};

export const clearPushToken = async () => {
  try {
    const authToken = await SecureStore.getItemAsync('auth_token');
    await axios.delete(`${API_BASE_URL}/auth/push-token`, {
      headers: { Authorization: `Bearer ${authToken}` },
    });
  } catch (e) {
    // Silent fail
  }
};
