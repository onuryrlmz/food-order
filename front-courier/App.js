import React from 'react';
import {StatusBar} from 'react-native';
import {SafeAreaProvider} from 'react-native-safe-area-context';
import {GestureHandlerRootView} from 'react-native-gesture-handler';
import {AuthProvider} from './src/context/AuthContext';
import {LocationProvider} from './src/context/LocationContext';
import AppNavigator from './src/navigation/AppNavigator';
import {Colors} from './src/theme';

function App() {
  return (
    <GestureHandlerRootView style={{flex: 1}}>
      <SafeAreaProvider>
        <AuthProvider>
          <LocationProvider>
            <StatusBar barStyle="dark-content" backgroundColor={Colors.background} />
            <AppNavigator />
          </LocationProvider>
        </AuthProvider>
      </SafeAreaProvider>
    </GestureHandlerRootView>
  );
}

export default App;
