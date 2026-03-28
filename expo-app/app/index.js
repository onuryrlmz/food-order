import { useEffect, useState } from 'react';
import { Redirect } from 'expo-router';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { useAuth } from '../src/context/AuthContext';
import { useAppData } from '../src/context/AppDataContext';
import SplashScreen from '../src/screens/Auth/SplashScreen';

export default function Index() {
  const { isLoading } = useAuth();
  const { dataReady, enableLoading } = useAppData();
  const [onboardingDone, setOnboardingDone] = useState(null);

  useEffect(() => {
    AsyncStorage.getItem('onboarding_completed').then(val => {
      const done = val === 'true';
      setOnboardingDone(done);
      if (done) enableLoading();
    });
  }, []);

  if (isLoading || onboardingDone === null) {
    return <SplashScreen />;
  }

  if (!onboardingDone) {
    return <Redirect href="/onboarding" />;
  }

  if (!dataReady) {
    return <SplashScreen />;
  }

  return <Redirect href="/(tabs)" />;
}
