import { router } from 'expo-router';
import AsyncStorage from '@react-native-async-storage/async-storage';
import OnboardingScreen from '../src/screens/Onboarding/OnboardingScreen';

export default function Onboarding() {
  const handleComplete = async () => {
    await AsyncStorage.setItem('onboarding_completed', 'true');
    router.replace('/(tabs)');
  };

  return <OnboardingScreen onComplete={handleComplete} />;
}
