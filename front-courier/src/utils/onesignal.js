import {OneSignal} from 'react-native-onesignal';

const ONESIGNAL_APP_ID = 'YOUR_ONESIGNAL_APP_ID'; // TODO: Replace with actual app ID

export const initOneSignal = () => {
  OneSignal.initialize(ONESIGNAL_APP_ID);
  OneSignal.Notifications.requestPermission(true);
};

export const setOneSignalUserId = (userId) => {
  OneSignal.login(String(userId));
};

export const clearOneSignalUserId = () => {
  OneSignal.logout();
};
