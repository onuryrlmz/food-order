import {Platform, PermissionsAndroid, Alert, Linking} from 'react-native';

export async function requestLocationPermission() {
  if (Platform.OS !== 'android') {
    return true;
  }

  try {
    // Step 1: Request fine location (foreground)
    const fineGranted = await PermissionsAndroid.request(
      PermissionsAndroid.PERMISSIONS.ACCESS_FINE_LOCATION,
      {
        title: 'Konum İzni',
        message:
          'Kurye uygulaması siparişleri teslim edebilmeniz için konumunuza erişmelidir.',
        buttonPositive: 'İzin Ver',
        buttonNegative: 'Reddet',
      },
    );

    if (fineGranted !== PermissionsAndroid.RESULTS.GRANTED) {
      Alert.alert(
        'Konum İzni Gerekli',
        'Uygulamanın düzgün çalışması için konum iznine ihtiyacı vardır. Lütfen ayarlardan izin verin.',
        [
          {text: 'Ayarları Aç', onPress: () => Linking.openSettings()},
          {text: 'Kapat'},
        ],
      );
      return false;
    }

    // Step 2: Request background location (Android 10+)
    if (Platform.Version >= 29) {
      const bgGranted = await PermissionsAndroid.request(
        PermissionsAndroid.PERMISSIONS.ACCESS_BACKGROUND_LOCATION,
        {
          title: 'Arka Plan Konum İzni',
          message:
            'Uygulama kapalıyken de konumunuzu takip edebilmemiz için "Her zaman izin ver" seçeneğini seçin.',
          buttonPositive: 'İzin Ver',
          buttonNegative: 'Reddet',
        },
      );

      if (bgGranted !== PermissionsAndroid.RESULTS.GRANTED) {
        Alert.alert(
          'Arka Plan Konum İzni',
          'Sipariş teslimatı sırasında konumunuzu takip edebilmemiz için arka plan konum iznine ihtiyacımız var. Lütfen ayarlardan "Her zaman izin ver" seçin.',
          [
            {text: 'Ayarları Aç', onPress: () => Linking.openSettings()},
            {text: 'Daha Sonra'},
          ],
        );
      }
    }

    return true;
  } catch (err) {
    console.warn('Location permission error:', err);
    return false;
  }
}

export async function requestBatteryOptimizationExemption() {
  if (Platform.OS !== 'android') {
    return;
  }

  try {
    // Open battery optimization settings - user needs to manually exempt
    Alert.alert(
      'Pil Optimizasyonu',
      'Uygulamanın arka planda düzgün çalışması için pil optimizasyonunu kapatmanız önerilir. Ayarları açmak ister misiniz?',
      [
        {
          text: 'Ayarları Aç',
          onPress: () =>
            Linking.sendIntent(
              'android.settings.REQUEST_IGNORE_BATTERY_OPTIMIZATIONS',
              [{key: 'package', value: 'com.frontcourier'}],
            ).catch(() => Linking.openSettings()),
        },
        {text: 'Daha Sonra'},
      ],
    );
  } catch (err) {
    console.warn('Battery optimization error:', err);
  }
}
