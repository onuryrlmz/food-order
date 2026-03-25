import React, {useState, useEffect, useCallback} from 'react';
import {
  View,
  Text,
  StyleSheet,
  FlatList,
  TouchableOpacity,
  ActivityIndicator,
  RefreshControl,
} from 'react-native';
import {useFocusEffect, useNavigation} from '@react-navigation/native';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import {Colors, Fonts, Spacing, BorderRadius} from '../../theme';
import {DELIVERY_STATUS} from '../../utils/constants';
import {courierService} from '../../api/courierService';

const DeliveryHistoryScreen = () => {
  const navigation = useNavigation();
  const [deliveries, setDeliveries] = useState([]);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [loadingMore, setLoadingMore] = useState(false);

  const fetchDeliveries = async (pageNum = 1, isRefresh = false) => {
    try {
      const response = await courierService.getAssignmentHistory(pageNum, 20);
      if (!response.data.hasFailed) {
        const items = response.data.data?.items || response.data.data || [];
        if (isRefresh || pageNum === 1) {
          setDeliveries(items);
        } else {
          setDeliveries(prev => [...prev, ...items]);
        }
        setHasMore(items.length === 20);
        setPage(pageNum);
      }
    } catch (error) {
      // Silent fail
    } finally {
      setLoading(false);
      setRefreshing(false);
      setLoadingMore(false);
    }
  };

  useFocusEffect(
    useCallback(() => {
      fetchDeliveries(1, true);
    }, []),
  );

  const onRefresh = () => {
    setRefreshing(true);
    fetchDeliveries(1, true);
  };

  const onEndReached = () => {
    if (!loadingMore && hasMore) {
      setLoadingMore(true);
      fetchDeliveries(page + 1);
    }
  };

  const formatDate = (dateStr) => {
    if (!dateStr) return '-';
    const date = new Date(dateStr);
    return date.toLocaleDateString('tr-TR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  const renderItem = ({item}) => {
    const statusInfo = DELIVERY_STATUS[item.status] || {};
    return (
      <TouchableOpacity
        style={styles.card}
        onPress={() => navigation.navigate('ActiveDelivery', {assignment: item})}
        activeOpacity={0.7}>
        <View style={styles.cardHeader}>
          <View style={styles.cardHeaderLeft}>
            <Icon name="store" size={18} color={Colors.textSecondary} />
            <Text style={styles.restaurantName} numberOfLines={1}>
              {item.restaurantName || 'Restoran'}
            </Text>
          </View>
          <View style={[styles.statusBadge, {backgroundColor: (statusInfo.color || Colors.textSecondary) + '20'}]}>
            <Text style={[styles.statusText, {color: statusInfo.color || Colors.textSecondary}]}>
              {statusInfo.label || 'Bilinmiyor'}
            </Text>
          </View>
        </View>

        <View style={styles.cardBody}>
          <View style={styles.infoRow}>
            <Icon name="account" size={16} color={Colors.textTertiary} />
            <Text style={styles.infoText}>{item.customerName || 'Müşteri'}</Text>
          </View>
          <View style={styles.infoRow}>
            <Icon name="calendar-clock" size={16} color={Colors.textTertiary} />
            <Text style={styles.infoText}>{formatDate(item.createdAt)}</Text>
          </View>
        </View>

        <View style={styles.cardFooter}>
          <Text style={styles.feeText}>{item.deliveryFee?.toFixed(2) || '0.00'} TL</Text>
          <Icon name="chevron-right" size={20} color={Colors.textTertiary} />
        </View>
      </TouchableOpacity>
    );
  };

  const renderFooter = () => {
    if (!loadingMore) return null;
    return (
      <View style={styles.footerLoader}>
        <ActivityIndicator size="small" color={Colors.primary} />
      </View>
    );
  };

  const renderEmpty = () => {
    if (loading) return null;
    return (
      <View style={styles.emptyContainer}>
        <Icon name="package-variant" size={60} color={Colors.textTertiary} />
        <Text style={styles.emptyText}>Henüz teslimat geçmişi yok</Text>
      </View>
    );
  };

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color={Colors.primary} />
      </View>
    );
  }

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <Text style={styles.headerTitle}>Teslimat Geçmişi</Text>
      </View>
      <FlatList
        data={deliveries}
        renderItem={renderItem}
        keyExtractor={(item, index) => item.id?.toString() || index.toString()}
        contentContainerStyle={styles.listContent}
        refreshControl={
          <RefreshControl refreshing={refreshing} onRefresh={onRefresh} />
        }
        onEndReached={onEndReached}
        onEndReachedThreshold={0.3}
        ListFooterComponent={renderFooter}
        ListEmptyComponent={renderEmpty}
      />
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: Colors.background,
  },
  loadingContainer: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: Colors.background,
  },
  header: {
    paddingHorizontal: Spacing.xl,
    paddingTop: Spacing.huge,
    paddingBottom: Spacing.base,
    backgroundColor: Colors.surface,
    borderBottomWidth: 1,
    borderBottomColor: Colors.borderLight,
  },
  headerTitle: {
    fontSize: Fonts.sizes.xl,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
  },
  listContent: {
    padding: Spacing.base,
    flexGrow: 1,
  },
  card: {
    backgroundColor: Colors.surface,
    borderRadius: BorderRadius.xl,
    padding: Spacing.base,
    marginBottom: Spacing.md,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 2},
    shadowOpacity: 0.06,
    shadowRadius: 8,
    elevation: 4,
  },
  cardHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: Spacing.md,
  },
  cardHeaderLeft: {
    flexDirection: 'row',
    alignItems: 'center',
    flex: 1,
    marginRight: Spacing.sm,
  },
  restaurantName: {
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.semibold,
    color: Colors.text,
    marginLeft: Spacing.sm,
    flex: 1,
  },
  statusBadge: {
    paddingHorizontal: Spacing.md,
    paddingVertical: Spacing.xs,
    borderRadius: BorderRadius.round,
  },
  statusText: {
    fontSize: Fonts.sizes.xs,
    fontWeight: Fonts.weights.semibold,
  },
  cardBody: {
    borderTopWidth: 1,
    borderTopColor: Colors.borderLight,
    paddingTop: Spacing.md,
  },
  infoRow: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: Spacing.xs,
  },
  infoText: {
    fontSize: Fonts.sizes.md,
    color: Colors.textSecondary,
    marginLeft: Spacing.sm,
  },
  cardFooter: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    borderTopWidth: 1,
    borderTopColor: Colors.borderLight,
    paddingTop: Spacing.md,
    marginTop: Spacing.sm,
  },
  feeText: {
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.bold,
    color: Colors.success,
  },
  footerLoader: {
    paddingVertical: Spacing.base,
    alignItems: 'center',
  },
  emptyContainer: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    paddingTop: Spacing.massive,
  },
  emptyText: {
    fontSize: Fonts.sizes.lg,
    color: Colors.textSecondary,
    marginTop: Spacing.base,
  },
});

export default DeliveryHistoryScreen;
