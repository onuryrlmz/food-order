import React, {useState, useEffect, useCallback} from 'react';
import {
  View,
  Text,
  StyleSheet,
  FlatList,
  TouchableOpacity,
  RefreshControl,
  ActivityIndicator,
} from 'react-native';
import MaterialCommunityIcons from 'react-native-vector-icons/MaterialCommunityIcons';
import {getEarnings, getEarningsHistory} from '../../api/courierService';

const PERIODS = [
  {id: 'daily', label: 'Bugun'},
  {id: 'weekly', label: 'Bu Hafta'},
  {id: 'monthly', label: 'Bu Ay'},
];

export default function EarningsScreen() {
  const [period, setPeriod] = useState('daily');
  const [summary, setSummary] = useState(null);
  const [deliveries, setDeliveries] = useState([]);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);

  const loadData = useCallback(async () => {
    try {
      const [earningsRes, historyRes] = await Promise.all([
        getEarnings(period),
        getEarningsHistory(period),
      ]);
      if (!earningsRes.hasFailed && earningsRes.data) {
        setSummary(earningsRes.data);
      }
      if (!historyRes.hasFailed) {
        setDeliveries(historyRes.data || []);
      }
    } catch (e) {
      console.log('Earnings error:', e);
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  }, [period]);

  useEffect(() => {
    setLoading(true);
    loadData();
  }, [loadData]);

  const onRefresh = useCallback(() => {
    setRefreshing(true);
    loadData();
  }, [loadData]);

  const formatDate = (dateStr) => {
    if (!dateStr) return '';
    const date = new Date(dateStr);
    const day = date.getDate().toString().padStart(2, '0');
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    const hours = date.getHours().toString().padStart(2, '0');
    const minutes = date.getMinutes().toString().padStart(2, '0');
    return `${day}.${month} ${hours}:${minutes}`;
  };

  const renderDeliveryItem = ({item}) => (
    <View style={styles.deliveryCard}>
      <View style={styles.deliveryHeader}>
        <View style={styles.deliveryIconBg}>
          <MaterialCommunityIcons name="package-variant-closed" size={18} color="#FF6B00" />
        </View>
        <View style={{flex: 1, marginLeft: 10}}>
          <Text style={styles.deliveryRestaurant} numberOfLines={1}>
            {item.restaurantName || 'Siparis'}
          </Text>
          <Text style={styles.deliveryDate}>{formatDate(item.deliveredAt || item.createdDate)}</Text>
        </View>
        <Text style={styles.deliveryEarning}>+{'\u20BA'}{Number(item.shipmentPrice || 0).toFixed(2)}</Text>
      </View>
    </View>
  );

  return (
    <View style={styles.container}>
      {/* Period Tabs */}
      <View style={styles.periodTabs}>
        {PERIODS.map(p => (
          <TouchableOpacity
            key={p.id}
            style={[styles.periodTab, period === p.id && styles.periodTabActive]}
            onPress={() => setPeriod(p.id)}
          >
            <Text style={[styles.periodTabText, period === p.id && styles.periodTabTextActive]}>
              {p.label}
            </Text>
          </TouchableOpacity>
        ))}
      </View>

      {loading ? (
        <View style={styles.center}>
          <ActivityIndicator size="large" color="#FF6B00" />
        </View>
      ) : (
        <>
          {/* Summary Card */}
          <View style={styles.summaryCard}>
            <Text style={styles.summaryLabel}>Toplam Kazanc</Text>
            <Text style={styles.summaryAmount}>
              {'\u20BA'}{Number(summary?.totalEarnings || 0).toFixed(2)}
            </Text>
            <View style={styles.summaryRow}>
              <View style={styles.summaryItem}>
                <MaterialCommunityIcons name="bike-fast" size={18} color="#FF6B00" />
                <Text style={styles.summaryItemValue}>{summary?.deliveryCount || 0}</Text>
                <Text style={styles.summaryItemLabel}>Teslimat</Text>
              </View>
              <View style={styles.summaryDivider} />
              <View style={styles.summaryItem}>
                <MaterialCommunityIcons name="cash-multiple" size={18} color="#34C759" />
                <Text style={styles.summaryItemValue}>
                  {'\u20BA'}{Number(summary?.avgEarning || 0).toFixed(2)}
                </Text>
                <Text style={styles.summaryItemLabel}>Ortalama</Text>
              </View>
            </View>
          </View>

          {/* Delivery List */}
          <Text style={styles.listTitle}>Teslimatlar</Text>
          <FlatList
            data={deliveries}
            keyExtractor={(item, index) => item.id || String(index)}
            renderItem={renderDeliveryItem}
            ListEmptyComponent={
              <View style={styles.emptyContainer}>
                <MaterialCommunityIcons name="cash-remove" size={40} color="#ccc" />
                <Text style={styles.emptyText}>Bu donemde teslimat yok</Text>
              </View>
            }
            refreshControl={
              <RefreshControl refreshing={refreshing} onRefresh={onRefresh} tintColor="#FF6B00" />
            }
            contentContainerStyle={styles.listContent}
            showsVerticalScrollIndicator={false}
          />
        </>
      )}
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
  },
  periodTabs: {
    flexDirection: 'row',
    backgroundColor: '#fff',
    paddingHorizontal: 16,
    paddingVertical: 12,
    gap: 8,
  },
  periodTab: {
    flex: 1,
    paddingVertical: 10,
    borderRadius: 10,
    backgroundColor: '#f2f2f7',
    alignItems: 'center',
  },
  periodTabActive: {
    backgroundColor: '#FF6B00',
  },
  periodTabText: {
    fontSize: 14,
    fontWeight: '600',
    color: '#666',
  },
  periodTabTextActive: {
    color: '#fff',
  },
  summaryCard: {
    backgroundColor: '#fff',
    margin: 16,
    borderRadius: 16,
    padding: 20,
    alignItems: 'center',
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 2},
    shadowOpacity: 0.06,
    shadowRadius: 8,
    elevation: 3,
  },
  summaryLabel: {
    fontSize: 14,
    color: '#888',
    fontWeight: '500',
  },
  summaryAmount: {
    fontSize: 36,
    fontWeight: '800',
    color: '#1a1a1a',
    marginVertical: 8,
  },
  summaryRow: {
    flexDirection: 'row',
    alignItems: 'center',
    marginTop: 12,
    width: '100%',
  },
  summaryItem: {
    flex: 1,
    alignItems: 'center',
  },
  summaryItemValue: {
    fontSize: 18,
    fontWeight: '700',
    color: '#1a1a1a',
    marginTop: 4,
  },
  summaryItemLabel: {
    fontSize: 12,
    color: '#888',
    marginTop: 2,
  },
  summaryDivider: {
    width: 1,
    height: 40,
    backgroundColor: '#eee',
  },
  listTitle: {
    fontSize: 16,
    fontWeight: '700',
    color: '#1a1a1a',
    paddingHorizontal: 16,
    marginBottom: 8,
  },
  listContent: {
    paddingHorizontal: 16,
    paddingBottom: 40,
    flexGrow: 1,
  },
  deliveryCard: {
    backgroundColor: '#fff',
    borderRadius: 12,
    padding: 14,
    marginBottom: 8,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 1},
    shadowOpacity: 0.04,
    shadowRadius: 4,
    elevation: 2,
  },
  deliveryHeader: {
    flexDirection: 'row',
    alignItems: 'center',
  },
  deliveryIconBg: {
    width: 36,
    height: 36,
    borderRadius: 10,
    backgroundColor: '#FFF3E0',
    justifyContent: 'center',
    alignItems: 'center',
  },
  deliveryRestaurant: {
    fontSize: 14,
    fontWeight: '600',
    color: '#1a1a1a',
  },
  deliveryDate: {
    fontSize: 12,
    color: '#888',
    marginTop: 2,
  },
  deliveryEarning: {
    fontSize: 16,
    fontWeight: '700',
    color: '#34C759',
  },
  emptyContainer: {
    alignItems: 'center',
    paddingVertical: 40,
  },
  emptyText: {
    fontSize: 14,
    color: '#999',
    marginTop: 8,
  },
});
