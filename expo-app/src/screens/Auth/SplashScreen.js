import React from 'react';
import {View, Text, StyleSheet, ActivityIndicator} from 'react-native';
import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import {Colors, Fonts} from '../../theme';

const SplashScreen = () => {
  return (
    <View style={styles.container}>
      <View style={styles.logoContainer}>
        <View style={styles.iconBg}>
          <Icon name="food-fork-drink" size={48} color="#FFF" />
        </View>
        <Text style={styles.appName}>FoodOrder</Text>
        <Text style={styles.tagline}>Lezzetler kapınızda</Text>
      </View>
      <ActivityIndicator size="large" color="#FFF" style={styles.loader} />
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: Colors.primary,
    justifyContent: 'center',
    alignItems: 'center',
  },
  logoContainer: {
    alignItems: 'center',
  },
  iconBg: {
    width: 96,
    height: 96,
    borderRadius: 28,
    backgroundColor: 'rgba(255,255,255,0.2)',
    justifyContent: 'center',
    alignItems: 'center',
    marginBottom: 20,
  },
  appName: {
    fontSize: Fonts.sizes.hero,
    fontWeight: Fonts.weights.heavy,
    color: '#FFF',
    letterSpacing: 1,
  },
  tagline: {
    fontSize: Fonts.sizes.base,
    color: 'rgba(255,255,255,0.8)',
    marginTop: 8,
    fontWeight: Fonts.weights.medium,
  },
  loader: {
    marginTop: 60,
  },
});

export default SplashScreen;
