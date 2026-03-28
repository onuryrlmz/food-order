import { Redirect } from 'expo-router';
import { View, ActivityIndicator, Text, StyleSheet } from 'react-native';
import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import { useAuth } from '../src/context/AuthContext';
import { Colors } from '../src/theme';

export default function Index() {
  const { isLoading, isAuthenticated, user } = useAuth();

  if (isLoading) {
    return (
      <View style={styles.splashContainer}>
        <Icon name="motorbike" size={64} color={Colors.primary} />
        <Text style={styles.splashTitle}>Kurye Paneli</Text>
        <ActivityIndicator size="large" color={Colors.primary} style={styles.splashLoader} />
      </View>
    );
  }

  if (!isAuthenticated) {
    return <Redirect href="/(auth)/login" />;
  }

  if (!user) {
    return <Redirect href="/(setup)/courier-register" />;
  }

  return <Redirect href="/(tabs)" />;
}

const styles = StyleSheet.create({
  splashContainer: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: Colors.background,
  },
  splashTitle: {
    fontSize: 24,
    fontWeight: '700',
    color: Colors.text,
    marginTop: 16,
  },
  splashLoader: {
    marginTop: 24,
  },
});
