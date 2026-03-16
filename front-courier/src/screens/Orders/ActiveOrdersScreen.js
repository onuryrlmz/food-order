import React, {useEffect, useState, useCallback} from 'react';
import {
  View,
  Text,
  StyleSheet,
  FlatList,
  RefreshControl,
  ActivityIndicator,
  TouchableOpacity,
  Alert,
} from 'react-native';
import MaterialCommunityIcons from 'react-native-vector-icons/MaterialCommunityIcons';
import {getActiveOrders, deliverOrder} from '../../api/courierService';
import {startTracking, stopTracking} from '../../services/locationTracker';
import {openNavigation} from '../../services/navigationHelper';

export default function ActiveOrdersScreen({navigation}) {
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [actionLoading, setActionLoading] = useState(null);

  const fetchOrders = useCallback(async () => {
    try {
      const result = await getActiveOrders();
      if (!result.hasFailed) {
        const data = result.data || [];
        setOrders(data);

        // Start/stop location tracking based on picked-up orders
        const hasPickedUp = data.some(
          o => o.statusId === 7 && o.pickedUpByCourierId,
        );
        if (hasPickedUp) {
          startTracking();
        } else {
          stopTracking();
        }
      }
    } catch (error) {
      console.error('Fetch orders error:', error);
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  }, []);

  useEffect(() => {
    fetchOrders();
    const interval = setInterval(fetchOrders, 30000);
    return () => {
      clearInterval(interval);
      stopTracking();
    };
  }, [fetchOrders]);

  const onRefresh = () => {
    setRefreshing(true);
    fetchOrders();
  };

  const handleDeliver = order => {
    Alert.alert(
      'Teslim Et',
      `Siparis #${order.orderNumber || order.orderId?.slice(0, 8)} teslim edildi olarak isaretlensin mi?`,
      [
        {text: 'Iptal', style: 'cancel'},
        {
          text: 'Teslim Ettim',
          onPress: async () => {
            setActionLoading(order.orderId);
            try {
              const result = await deliverOrder(order.orderId);
              if (!result.hasFailed) {
                fetchOrders();
              } else {
                Alert.alert(
                  'Hata',
                  result.messages?.map(m => m.description).join(', ') || 'Teslim edilemedi.',
                );
              }
            } catch {
              Alert.alert('Hata', 'Bir hata olustu.');
            } finally {
              setActionLoading(null);
            }
          },
        },
      ],
    );
  };

  const getStatusStyle = statusId => {
    switch (statusId) {
      case 6: // Preparing
        return {backgroundColor: '#FFF3CD', color: '#856404'};
      case 7: // OnTheWay
        return {backgroundColor: '#D1ECF1', color: '#0C5460'};
      default:
        return {backgroundColor: '#E2E3E5', color: '#383D41'};
    }
  };

  const renderOrder = ({item}) => {
    const statusStyle = getStatusStyle(item.statusId);
    const isPickedUp = item.statusId === 7 && item.pickedUpByCourierId;
    const isDelivering = actionLoading === item.orderId;

    return (
      <View style={styles.orderCard}>
        <View style={styles.orderHeader}>
          <Text style={styles.restaurantName}>{item.restaurantName}</Text>
          {item.deliveryDistanceKm != null && (
            <Text style={styles.distance}>
              {item.deliveryDistanceKm.toFixed(1)} km
            </Text>
          )}
        </View>
        <View style={styles.orderBody}>
          <Text style={styles.deliveryArea}>{item.deliveryArea}</Text>
          <View
            style={[
              styles.statusBadge,
              {backgroundColor: statusStyle.backgroundColor},
            ]}>
            <Text style={[styles.statusText, {color: statusStyle.color}]}>
              {item.statusName}
            </Text>
          </View>
        </View>
        <Text style={styles.timestamp}>
          {new Date(item.createdDate).toLocaleString('tr-TR')}
        </Text>

        {isPickedUp && (
          <View style={styles.actionRow}>
            <TouchableOpacity
              style={styles.mapButton}
              onPress={() =>
                navigation.navigate('OrderMap', {
                  deliveryLat: item.deliveryLatitude,
                  deliveryLng: item.deliveryLongitude,
                  deliveryAddress: item.deliveryArea,
                  orderId: item.orderId,
                })
              }
              activeOpacity={0.7}>
              <MaterialCommunityIcons name="map" size={16} color="#0C5460" />
              <Text style={styles.mapButtonText}>Haritada Gor</Text>
            </TouchableOpacity>

            <TouchableOpacity
              style={styles.navButton}
              onPress={() =>
                openNavigation(item.deliveryLatitude, item.deliveryLongitude)
              }
              activeOpacity={0.7}>
              <MaterialCommunityIcons name="navigation" size={16} color="#fff" />
              <Text style={styles.navButtonText}>Navigasyon</Text>
            </TouchableOpacity>

            <TouchableOpacity
              style={[styles.deliverButton, isDelivering && {opacity: 0.7}]}
              onPress={() => handleDeliver(item)}
              disabled={isDelivering}
              activeOpacity={0.8}>
              {isDelivering ? (
                <ActivityIndicator size="small" color="#fff" />
              ) : (
                <>
                  <MaterialCommunityIcons name="check-circle" size={16} color="#fff" />
                  <Text style={styles.deliverButtonText}>Teslim Ettim</Text>
                </>
              )}
            </TouchableOpacity>
          </View>
        )}
      </View>
    );
  };

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color="#FF6B00" />
      </View>
    );
  }

  return (
    <View style={styles.container}>
      <FlatList
        data={orders}
        renderItem={renderOrder}
        keyExtractor={item => item.orderId}
        contentContainerStyle={styles.listContent}
        refreshControl={
          <RefreshControl
            refreshing={refreshing}
            onRefresh={onRefresh}
            colors={['#FF6B00']}
          />
        }
        ListEmptyComponent={
          <View style={styles.empty}>
            <Text style={styles.emptyIcon}>📦</Text>
            <Text style={styles.emptyText}>Aktif siparisiniz yok</Text>
            <Text style={styles.emptySubtext}>
              Yeni siparisler burada gorunecek
            </Text>
          </View>
        }
      />
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f8f9fa',
  },
  loadingContainer: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: '#f8f9fa',
  },
  listContent: {
    padding: 16,
    paddingBottom: 32,
  },
  orderCard: {
    backgroundColor: '#fff',
    borderRadius: 16,
    padding: 16,
    marginBottom: 12,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 1},
    shadowOpacity: 0.06,
    shadowRadius: 4,
    elevation: 2,
  },
  orderHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 12,
  },
  restaurantName: {
    fontSize: 17,
    fontWeight: '600',
    color: '#1a1a1a',
    flex: 1,
  },
  distance: {
    fontSize: 15,
    fontWeight: '500',
    color: '#666',
    marginLeft: 8,
  },
  orderBody: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 10,
  },
  deliveryArea: {
    fontSize: 14,
    color: '#555',
    flex: 1,
  },
  statusBadge: {
    paddingHorizontal: 12,
    paddingVertical: 4,
    borderRadius: 12,
  },
  statusText: {
    fontSize: 12,
    fontWeight: '600',
  },
  timestamp: {
    fontSize: 12,
    color: '#999',
  },
  actionRow: {
    flexDirection: 'row',
    marginTop: 12,
    gap: 8,
  },
  mapButton: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#D1ECF1',
    paddingHorizontal: 12,
    paddingVertical: 8,
    borderRadius: 8,
    gap: 4,
  },
  mapButtonText: {
    fontSize: 12,
    fontWeight: '600',
    color: '#0C5460',
  },
  navButton: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#007AFF',
    paddingHorizontal: 12,
    paddingVertical: 8,
    borderRadius: 8,
    gap: 4,
  },
  navButtonText: {
    fontSize: 12,
    fontWeight: '600',
    color: '#fff',
  },
  deliverButton: {
    flex: 1,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    backgroundColor: '#34C759',
    paddingVertical: 8,
    borderRadius: 8,
    gap: 4,
  },
  deliverButtonText: {
    fontSize: 12,
    fontWeight: '700',
    color: '#fff',
  },
  empty: {
    alignItems: 'center',
    paddingVertical: 60,
  },
  emptyIcon: {
    fontSize: 48,
    marginBottom: 12,
  },
  emptyText: {
    fontSize: 17,
    fontWeight: '600',
    color: '#666',
  },
  emptySubtext: {
    fontSize: 14,
    color: '#999',
    marginTop: 4,
  },
});
