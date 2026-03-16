import React, {useState, useEffect} from 'react';
import {
  View,
  Text,
  StyleSheet,
  TouchableOpacity,
  Dimensions,
} from 'react-native';
import MapView, {Marker} from 'react-native-maps';
import MaterialCommunityIcons from 'react-native-vector-icons/MaterialCommunityIcons';
import Geolocation from 'react-native-geolocation-service';
import {openNavigation} from '../../services/navigationHelper';

const {width, height} = Dimensions.get('window');

export default function OrderMapScreen({route}) {
  const {deliveryLat, deliveryLng, deliveryAddress, orderId} = route.params;
  const [courierLocation, setCourierLocation] = useState(null);

  useEffect(() => {
    Geolocation.getCurrentPosition(
      position => {
        setCourierLocation({
          latitude: position.coords.latitude,
          longitude: position.coords.longitude,
        });
      },
      error => console.warn('GPS error:', error),
      {enableHighAccuracy: true, timeout: 10000},
    );
  }, []);

  const deliveryCoord = {
    latitude: deliveryLat,
    longitude: deliveryLng,
  };

  const initialRegion = courierLocation
    ? {
        latitude: (courierLocation.latitude + deliveryLat) / 2,
        longitude: (courierLocation.longitude + deliveryLng) / 2,
        latitudeDelta:
          Math.abs(courierLocation.latitude - deliveryLat) * 1.5 + 0.01,
        longitudeDelta:
          Math.abs(courierLocation.longitude - deliveryLng) * 1.5 + 0.01,
      }
    : {
        latitude: deliveryLat,
        longitude: deliveryLng,
        latitudeDelta: 0.02,
        longitudeDelta: 0.02,
      };

  return (
    <View style={styles.container}>
      <MapView style={styles.map} initialRegion={initialRegion}>
        {courierLocation && (
          <Marker
            coordinate={courierLocation}
            title="Konumunuz"
            pinColor="blue"
          />
        )}
        <Marker
          coordinate={deliveryCoord}
          title="Teslimat Adresi"
          description={deliveryAddress}
          pinColor="red"
        />
      </MapView>

      <View style={styles.bottomBar}>
        <View style={styles.addressContainer}>
          <MaterialCommunityIcons name="map-marker" size={20} color="#FF6B00" />
          <Text style={styles.addressText} numberOfLines={2}>
            {deliveryAddress}
          </Text>
        </View>
        <TouchableOpacity
          style={styles.navigateButton}
          onPress={() => openNavigation(deliveryLat, deliveryLng)}
          activeOpacity={0.8}>
          <MaterialCommunityIcons name="navigation" size={20} color="#fff" />
          <Text style={styles.navigateButtonText}>Navigasyonu Baslat</Text>
        </TouchableOpacity>
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
  },
  map: {
    flex: 1,
  },
  bottomBar: {
    backgroundColor: '#fff',
    paddingHorizontal: 16,
    paddingVertical: 16,
    paddingBottom: 32,
    borderTopWidth: 1,
    borderTopColor: '#eee',
    shadowColor: '#000',
    shadowOffset: {width: 0, height: -2},
    shadowOpacity: 0.08,
    shadowRadius: 4,
    elevation: 4,
  },
  addressContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: 12,
    gap: 8,
  },
  addressText: {
    flex: 1,
    fontSize: 14,
    color: '#555',
  },
  navigateButton: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    backgroundColor: '#FF6B00',
    paddingVertical: 14,
    borderRadius: 12,
    gap: 8,
  },
  navigateButtonText: {
    color: '#fff',
    fontSize: 16,
    fontWeight: '700',
  },
});
