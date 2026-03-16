import React, {useEffect, useState, useCallback} from 'react';
import {
  View,
  Text,
  StyleSheet,
  FlatList,
  RefreshControl,
  ActivityIndicator,
} from 'react-native';
import {getActiveOrders} from '../../api/courierService';

export default function ActiveOrdersScreen() {
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);

  const fetchOrders = useCallback(async () => {
    try {
      const result = await getActiveOrders();
      if (!result.hasFailed) {
        setOrders(result.data || []);
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
    // 30 saniyede bir yenile
    const interval = setInterval(fetchOrders, 30000);
    return () => clearInterval(interval);
  }, [fetchOrders]);

  const onRefresh = () => {
    setRefreshing(true);
    fetchOrders();
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
            <Text style={styles.emptyText}>Aktif siparişiniz yok</Text>
            <Text style={styles.emptySubtext}>
              Yeni siparişler burada görünecek
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
