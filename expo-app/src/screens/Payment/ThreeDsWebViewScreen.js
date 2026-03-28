import React, {useRef} from 'react';
import {View, Text, StyleSheet, ActivityIndicator, TouchableOpacity} from 'react-native';
import {useSafeAreaInsets} from 'react-native-safe-area-context';
import {WebView} from 'react-native-webview';
import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import {Colors, Fonts, Spacing} from '../../theme';
import {useToast} from '../../context/ToastContext';
import { router, useLocalSearchParams } from 'expo-router';

const ThreeDsWebViewScreen = () => {
  const {htmlContent, orderId} = useLocalSearchParams();
  const insets = useSafeAreaInsets();
  const {showToast} = useToast();
  const webViewRef = useRef(null);

  const handleMessage = (event) => {
    try {
      const data = JSON.parse(event.nativeEvent.data);
      if (data.status === 'success') {
        showToast('Ödeme başarılı!', 'success');
        router.replace('/(tabs)');
      } else {
        showToast(data.message || 'Ödeme başarısız oldu.', 'error');
        router.replace('/(tabs)');
      }
    } catch (e) {
      // Ignore non-JSON messages
    }
  };

  const handleClose = () => {
    showToast('Ödeme iptal edildi. Siparişiniz ödeme bekliyor durumunda.', 'warning');
    router.replace('/(tabs)');
  };

  return (
    <View style={styles.container}>
      <View style={[styles.header, {paddingTop: insets.top + 10}]}>
        <TouchableOpacity style={styles.closeButton} onPress={handleClose}>
          <Icon name="close" size={24} color={Colors.text} />
        </TouchableOpacity>
        <Text style={styles.headerTitle}>Ödeme Doğrulama</Text>
        <View style={{width: 44}} />
      </View>
      <WebView
        ref={webViewRef}
        source={{html: htmlContent}}
        style={styles.webView}
        onMessage={handleMessage}
        javaScriptEnabled={true}
        domStorageEnabled={true}
        startInLoadingState={true}
        renderLoading={() => (
          <View style={styles.loading}>
            <ActivityIndicator size="large" color={Colors.primary} />
            <Text style={styles.loadingText}>Yükleniyor...</Text>
          </View>
        )}
      />
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: Colors.background,
  },
  header: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    paddingHorizontal: Spacing.base,
    paddingBottom: Spacing.md,
    backgroundColor: Colors.surface,
    borderBottomWidth: 1,
    borderBottomColor: Colors.borderLight,
  },
  closeButton: {
    width: 44,
    height: 44,
    borderRadius: 22,
    backgroundColor: Colors.borderLight,
    justifyContent: 'center',
    alignItems: 'center',
  },
  headerTitle: {
    fontSize: Fonts.sizes.lg,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
  },
  webView: {
    flex: 1,
  },
  loading: {
    position: 'absolute',
    top: 0,
    left: 0,
    right: 0,
    bottom: 0,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: Colors.background,
  },
  loadingText: {
    marginTop: Spacing.md,
    fontSize: Fonts.sizes.md,
    color: Colors.textSecondary,
  },
});

export default ThreeDsWebViewScreen;
