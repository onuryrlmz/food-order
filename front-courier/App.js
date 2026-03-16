import React, {useEffect} from 'react';
import {NavigationContainer} from '@react-navigation/native';
import {SafeAreaProvider} from 'react-native-safe-area-context';
import {AuthProvider} from './src/context/AuthContext';
import AppNavigator from './src/navigation/AppNavigator';
import {
  requestLocationPermission,
  requestBatteryOptimizationExemption,
} from './src/utils/locationPermission';
import {initOneSignal} from './src/utils/onesignal';

function App() {
  useEffect(() => {
    initOneSignal();
    const initPermissions = async () => {
      const granted = await requestLocationPermission();
      if (granted) {
        requestBatteryOptimizationExemption();
      }
    };
    initPermissions();
  }, []);

  return (
    <SafeAreaProvider>
      <AuthProvider>
        <NavigationContainer>
          <AppNavigator />
        </NavigationContainer>
      </AuthProvider>
    </SafeAreaProvider>
  );
}

export default App;
