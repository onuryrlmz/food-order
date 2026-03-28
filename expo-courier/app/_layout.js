import { StatusBar } from 'react-native';
import { Stack } from 'expo-router';
import { GestureHandlerRootView } from 'react-native-gesture-handler';
import { SafeAreaProvider } from 'react-native-safe-area-context';
import { AuthProvider } from '../src/context/AuthContext';
import { LocationProvider } from '../src/context/LocationContext';
import { Colors } from '../src/theme';

// Import background location task definition (must be at top level)
import '../src/utils/backgroundLocation';

export default function RootLayout() {
  return (
    <GestureHandlerRootView style={{ flex: 1 }}>
      <SafeAreaProvider>
        <AuthProvider>
          <LocationProvider>
            <StatusBar barStyle="dark-content" backgroundColor={Colors.background} />
            <Stack screenOptions={{ headerShown: false }}>
              <Stack.Screen name="index" />
              <Stack.Screen name="(auth)" />
              <Stack.Screen name="(setup)" />
              <Stack.Screen name="(tabs)" />
              <Stack.Screen name="agreements" />
            </Stack>
          </LocationProvider>
        </AuthProvider>
      </SafeAreaProvider>
    </GestureHandlerRootView>
  );
}
