let OneSignal = null;
try {
  OneSignal = require('react-native-onesignal').OneSignal;
} catch (e) {
  // OneSignal native module not available (e.g. simulator)
}

const ONESIGNAL_APP_ID = 'YOUR_ONESIGNAL_APP_ID'; // TODO: Replace with actual app ID from env/CI

export const initOneSignal = (navigationRef) => {
  if (!OneSignal) return;
  if (!ONESIGNAL_APP_ID || ONESIGNAL_APP_ID === 'YOUR_ONESIGNAL_APP_ID') return;
  try {
    OneSignal.initialize(ONESIGNAL_APP_ID);
    OneSignal.Notifications.requestPermission(true);

    OneSignal.Notifications.addEventListener('click', (event) => {
      const data = event.notification.additionalData;
      if (data?.orderId && navigationRef?.current) {
        navigationRef.current.navigate('OrderDetail', {orderId: data.orderId});
      }
    });
  } catch (e) {
    // Silent fail on simulator
  }
};

export const setOneSignalUserId = (userId) => {
  if (!OneSignal) return;
  try { OneSignal.login(String(userId)); } catch (e) {}
};

export const clearOneSignalUserId = () => {
  if (!OneSignal) return;
  try { OneSignal.logout(); } catch (e) {}
};
