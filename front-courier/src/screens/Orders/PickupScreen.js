import React, {useState, useEffect, useCallback} from 'react';
import {
  View,
  Text,
  StyleSheet,
  FlatList,
  TouchableOpacity,
  Alert,
  RefreshControl,
  ActivityIndicator,
} from 'react-native';
import MaterialCommunityIcons from 'react-native-vector-icons/MaterialCommunityIcons';
import {getPendingPickups, confirmPickup} from '../../api/courierService';

export default function PickupScreen({route}) {
  const {restaurantId, restaurantName} = route.params;
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [actionLoading, setActionLoading] = useState(null);

  const fetchOrders = useCallback(async () => {
    try {
      const result = await getPendingPickups(restaurantId);
      if (!result.hasFailed && result.data) {
        setOrders(result.data);
      }
    } catch (error) {
      console.error('Pickup orders error:', error);
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  }, [restaurantId]);

  useEffect(() => {
    fetchOrders();
  }, [fetchOrders]);

  const onRefresh = () => {
    setRefreshing(true);
    fetchOrders();
  };

  const handlePickup = order => {
    Alert.alert(
      'Teslim Al',
      `Siparis #${order.orderNumber || order.orderId} teslim almak istiyor musunuz?`,
      [
        {text: 'Iptal', style: 'cancel'},
        {
          text: 'Teslim Aldim',
          onPress: async () => {
            setActionLoading(order.orderId);
            try {
              const result = await confirmPickup(order.orderId);
              if (!result.hasFailed) {
                setOrders(prev =>
                  prev.filter(o => o.orderId !== order.orderId),
                );
                Alert.alert('Basarili', 'Siparis teslim alindi.');
              } else {
                Alert.alert(
                  'Hata',
                  result.messages?.map(m => m.description).join(', ') || 'Teslim alinamadi.',
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

  const renderOrder = ({item}) => (
    <View style={styles.orderCard}>
      <View style={styles.orderHeader}>
        <View style={styles.orderNumberBadge}>
          <Text style={styles.orderNumberText}>
            #{item.orderNumber || item.orderId?.slice(0, 8)}
          </Text>
        </View>
        {item.deliveryDistanceKm != null && (
          <Text style={styles.distance}>
            {item.deliveryDistanceKm.toFixed(1)} km
          </Text>
        )}
      </View>
      <Text style={styles.deliveryAddress}>{item.deliveryArea || item.deliveryAddress}</Text>
      {item.totalPrice != null && (
        <Text style={styles.price}>{item.totalPrice.toFixed(2)} TL</Text>
      )}
      <Text style={styles.timestamp}>
        {new Date(item.createdDate).toLocaleString('tr-TR')}
      </Text>
      <TouchableOpacity
        style={[styles.pickupButton, actionLoading === item.orderId && styles.pickupButtonDisabled]}
        onPress={() => handlePickup(item)}
        disabled={actionLoading === item.orderId}
        activeOpacity={0.8}>
        {actionLoading === item.orderId ? (
          <ActivityIndicator size="small" color="#fff" />
        ) : (
          <>
            <MaterialCommunityIcons name="package-variant" size={18} color="#fff" />
            <Text style={styles.pickupButtonText}>Teslim Aldim</Text>
          </>
        )}
      </TouchableOpacity>
    </View>
  );

  if (loading) {
    return (
      <View style={styles.center}>
        <ActivityIndicator size="large" color="#FF6B00" />
      </View>
    );
  }

  return (
    <View style={styles.container}>
      <View style={styles.headerInfo}>
        <MaterialCommunityIcons name="store" size={20} color="#FF6B00" />
        <Text style={styles.headerInfoText}>{restaurantName}</Text>
      </View>
      <FlatList
        data={orders}
        renderItem={renderOrder}
        keyExtractor={item => item.orderId}
        contentContainerStyle={styles.listContent}
        refreshControl={
          <RefreshControl refreshing={refreshing} onRefresh={onRefresh} colors={['#FF6B00']} />
        }
        ListEmptyComponent={
          <View style={styles.empty}>
            <MaterialCommunityIcons name="package-variant-closed" size={48} color="#ccc" />
            <Text style={styles.emptyText}>Bekleyen siparis yok</Text>
            <Text style={styles.emptySubtext}>
              Teslim alinacak siparisler burada gorunecek
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
  center: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: '#f8f9fa',
  },
  headerInfo: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#fff',
    paddingHorizontal: 16,
    paddingVertical: 12,
    borderBottomWidth: 1,
    borderBottomColor: '#eee',
    gap: 8,
  },
  headerInfoText: {
    fontSize: 16,
    fontWeight: '600',
    color: '#1a1a1a',
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
    marginBottom: 10,
  },
  orderNumberBadge: {
    backgroundColor: '#FFF3E0',
    paddingHorizontal: 10,
    paddingVertical: 4,
    borderRadius: 8,
  },
  orderNumberText: {
    fontSize: 14,
    fontWeight: '700',
    color: '#FF6B00',
  },
  distance: {
    fontSize: 14,
    fontWeight: '500',
    color: '#666',
  },
  deliveryAddress: {
    fontSize: 14,
    color: '#555',
    marginBottom: 6,
  },
  price: {
    fontSize: 16,
    fontWeight: '700',
    color: '#1a1a1a',
    marginBottom: 4,
  },
  timestamp: {
    fontSize: 12,
    color: '#999',
    marginBottom: 12,
  },
  pickupButton: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    backgroundColor: '#34C759',
    paddingVertical: 12,
    borderRadius: 10,
    gap: 6,
  },
  pickupButtonDisabled: {
    opacity: 0.7,
  },
  pickupButtonText: {
    color: '#fff',
    fontSize: 15,
    fontWeight: '700',
  },
  empty: {
    alignItems: 'center',
    paddingVertical: 60,
  },
  emptyText: {
    fontSize: 17,
    fontWeight: '600',
    color: '#666',
    marginTop: 12,
  },
  emptySubtext: {
    fontSize: 14,
    color: '#999',
    marginTop: 4,
  },
});
