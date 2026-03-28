import { useEffect } from 'react';
import { StatusBar } from 'react-native';
import { Stack } from 'expo-router';
import { GestureHandlerRootView } from 'react-native-gesture-handler';
import { SafeAreaProvider } from 'react-native-safe-area-context';
import { AuthProvider } from '../src/context/AuthContext';
import { AppDataProvider } from '../src/context/AppDataContext';
import { CartProvider } from '../src/context/CartContext';
import { ToastProvider } from '../src/context/ToastContext';
import { Colors } from '../src/theme';
import { initNotifications } from '../src/utils/notifications';

export default function RootLayout() {
  useEffect(() => {
    const subscription = initNotifications();
    return () => subscription?.remove();
  }, []);

  return (
    <GestureHandlerRootView style={{ flex: 1 }}>
      <SafeAreaProvider>
        <AuthProvider>
          <AppDataProvider>
            <CartProvider>
              <ToastProvider>
                <StatusBar barStyle="dark-content" backgroundColor={Colors.background} />
                <Stack screenOptions={{ headerShown: false }}>
                  <Stack.Screen name="index" />
                  <Stack.Screen name="onboarding" />
                  <Stack.Screen name="(auth)" />
                  <Stack.Screen name="(tabs)" />
                  <Stack.Screen name="restaurant/[id]" />
                  <Stack.Screen name="order/[id]" />
                  <Stack.Screen name="checkout" />
                  <Stack.Screen name="three-ds" />
                  <Stack.Screen name="address-list" />
                  <Stack.Screen name="add-address" />
                  <Stack.Screen name="edit-profile" />
                  <Stack.Screen name="change-password" />
                  <Stack.Screen name="saved-cards" />
                  <Stack.Screen name="favorites" />
                  <Stack.Screen name="notifications" />
                  <Stack.Screen name="notification-preferences" />
                  <Stack.Screen name="scheduled-orders" />
                  <Stack.Screen name="support" />
                  <Stack.Screen name="support-chat" />
                  <Stack.Screen name="new-ticket" />
                </Stack>
              </ToastProvider>
            </CartProvider>
          </AppDataProvider>
        </AuthProvider>
      </SafeAreaProvider>
    </GestureHandlerRootView>
  );
}
