import React from 'react';
import {View, ActivityIndicator, StyleSheet, Text} from 'react-native';
import {Colors, Fonts} from '../theme';

const LoadingSpinner = ({message = 'Yükleniyor...', fullScreen = true}) => {
  if (!fullScreen) {
    return (
      <View style={styles.inline}>
        <ActivityIndicator size="small" color={Colors.primary} />
        {message ? <Text style={styles.inlineText}>{message}</Text> : null}
      </View>
    );
  }

  return (
    <View style={styles.container}>
      <View style={styles.card}>
        <ActivityIndicator size="large" color={Colors.primary} />
        {message ? <Text style={styles.text}>{message}</Text> : null}
      </View>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: Colors.background,
  },
  card: {
    backgroundColor: Colors.surface,
    padding: 32,
    borderRadius: 16,
    alignItems: 'center',
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 2},
    shadowOpacity: 0.1,
    shadowRadius: 8,
    elevation: 4,
  },
  text: {
    marginTop: 16,
    fontSize: Fonts.sizes.base,
    color: Colors.textSecondary,
    fontWeight: Fonts.weights.medium,
  },
  inline: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    padding: 20,
  },
  inlineText: {
    marginLeft: 10,
    fontSize: Fonts.sizes.md,
    color: Colors.textSecondary,
  },
});

export default LoadingSpinner;
