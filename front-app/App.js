import React from 'react';
import {StatusBar} from 'react-native';
import {SafeAreaProvider} from 'react-native-safe-area-context';
import {GestureHandlerRootView} from 'react-native-gesture-handler';
import {AuthProvider} from './src/context/AuthContext';
import {CartProvider} from './src/context/CartContext';
import {AppDataProvider} from './src/context/AppDataContext';
import {ToastProvider} from './src/context/ToastContext';
import AppNavigator from './src/navigation/AppNavigator';
import {Colors} from './src/theme';

function App() {
  return (
    <GestureHandlerRootView style={{flex: 1}}>
      <SafeAreaProvider>
        <AuthProvider>
          <AppDataProvider>
            <CartProvider>
              <ToastProvider>
                <StatusBar barStyle="dark-content" backgroundColor={Colors.background} />
                <AppNavigator />
              </ToastProvider>
            </CartProvider>
          </AppDataProvider>
        </AuthProvider>
      </SafeAreaProvider>
    </GestureHandlerRootView>
  );
}

export default App;
