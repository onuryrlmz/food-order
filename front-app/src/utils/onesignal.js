import {OneSignal} from 'react-native-onesignal';

const ONESIGNAL_APP_ID = 'YOUR_ONESIGNAL_APP_ID'; // TODO: Replace with actual app ID

export const initOneSignal = (navigationRef) => {
  OneSignal.initialize(ONESIGNAL_APP_ID);
  OneSignal.Notifications.requestPermission(true);

  OneSignal.Notifications.addEventListener('click', (event) => {
    const data = event.notification.additionalData;
    if (data?.orderId && navigationRef?.current) {
      navigationRef.current.navigate('OrderDetail', {orderId: data.orderId});
    }
  });
};

export const setOneSignalUserId = (userId) => {
  OneSignal.login(String(userId));
};

export const clearOneSignalUserId = () => {
  OneSignal.logout();
};
