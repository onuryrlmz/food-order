import React, {useState, useCallback} from 'react';
import {
  View,
  Text,
  StyleSheet,
  FlatList,
  ActivityIndicator,
  RefreshControl,
} from 'react-native';
import {useFocusEffect} from '@react-navigation/native';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import {Colors, Fonts, Spacing, BorderRadius} from '../../theme';
import {courierService} from '../../api/courierService';

const EarningsScreen = () => {
  const [earnings, setEarnings] = useState([]);
  const [summary, setSummary] = useState(null);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [loadingMore, setLoadingMore] = useState(false);

  const fetchData = async (pageNum = 1, isRefresh = false) => {
    try {
      const requests = [courierService.getEarnings(pageNum, 20)];
      if (pageNum === 1) {
        requests.push(courierService.getEarningSummary());
      }

      const responses = await Promise.all(requests);

      const earningsRes = responses[0];
      if (!earningsRes.data.hasFailed) {
        const items = earningsRes.data.data?.items || earningsRes.data.data || [];
        if (isRefresh || pageNum === 1) {
          setEarnings(items);
        } else {
          setEarnings(prev => [...prev, ...items]);
        }
        setHasMore(items.length === 20);
        setPage(pageNum);
      }

      if (responses[1] && !responses[1].data.hasFailed) {
        setSummary(responses[1].data.data);
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
      fetchData(1, true);
    }, []),
  );

  const onRefresh = () => {
    setRefreshing(true);
    fetchData(1, true);
  };

  const onEndReached = () => {
    if (!loadingMore && hasMore) {
      setLoadingMore(true);
      fetchData(page + 1);
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

  const renderSummary = () => {
    if (!summary) return null;
    return (
      <View style={styles.summaryContainer}>
        <View style={styles.summaryCard}>
          <Icon name="cash-multiple" size={28} color={Colors.success} />
          <Text style={styles.summaryValue}>
            {(summary.totalEarnings || 0).toFixed(2)} TL
          </Text>
          <Text style={styles.summaryLabel}>Toplam Kazanç</Text>
        </View>
        <View style={styles.summaryRow}>
          <View style={styles.summarySmallCard}>
            <Text style={styles.summarySmallValue}>
              {(summary.todayEarnings || 0).toFixed(2)} TL
            </Text>
            <Text style={styles.summarySmallLabel}>Bugün</Text>
          </View>
          <View style={styles.summarySmallCard}>
            <Text style={styles.summarySmallValue}>
              {(summary.weeklyEarnings || 0).toFixed(2)} TL
            </Text>
            <Text style={styles.summarySmallLabel}>Bu Hafta</Text>
          </View>
          <View style={styles.summarySmallCard}>
            <Text style={styles.summarySmallValue}>
              {(summary.monthlyEarnings || 0).toFixed(2)} TL
            </Text>
            <Text style={styles.summarySmallLabel}>Bu Ay</Text>
          </View>
        </View>
      </View>
    );
  };

  const renderItem = ({item}) => (
    <View style={styles.earningCard}>
      <View style={styles.earningLeft}>
        <View style={styles.earningIconContainer}>
          <Icon name="motorbike" size={20} color={Colors.primary} />
        </View>
        <View style={styles.earningInfo}>
          <Text style={styles.earningRestaurant} numberOfLines={1}>
            {item.restaurantName || 'Teslimat'}
          </Text>
          <Text style={styles.earningDate}>{formatDate(item.completedAt || item.createdAt)}</Text>
        </View>
      </View>
      <Text style={styles.earningAmount}>+{item.amount?.toFixed(2) || '0.00'} TL</Text>
    </View>
  );

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
        <Icon name="cash-remove" size={60} color={Colors.textTertiary} />
        <Text style={styles.emptyText}>Henüz kazanç kaydı yok</Text>
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
        <Text style={styles.headerTitle}>Kazançlarım</Text>
      </View>
      <FlatList
        data={earnings}
        renderItem={renderItem}
        keyExtractor={(item, index) => item.id?.toString() || index.toString()}
        contentContainerStyle={styles.listContent}
        refreshControl={
          <RefreshControl refreshing={refreshing} onRefresh={onRefresh} />
        }
        onEndReached={onEndReached}
        onEndReachedThreshold={0.3}
        ListHeaderComponent={renderSummary}
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
  summaryContainer: {
    marginBottom: Spacing.base,
  },
  summaryCard: {
    backgroundColor: Colors.surface,
    borderRadius: BorderRadius.xl,
    padding: Spacing.xl,
    alignItems: 'center',
    marginBottom: Spacing.md,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 2},
    shadowOpacity: 0.06,
    shadowRadius: 8,
    elevation: 4,
  },
  summaryValue: {
    fontSize: Fonts.sizes.display,
    fontWeight: Fonts.weights.bold,
    color: Colors.success,
    marginTop: Spacing.sm,
  },
  summaryLabel: {
    fontSize: Fonts.sizes.md,
    color: Colors.textSecondary,
    marginTop: Spacing.xs,
  },
  summaryRow: {
    flexDirection: 'row',
    gap: Spacing.sm,
  },
  summarySmallCard: {
    flex: 1,
    backgroundColor: Colors.surface,
    borderRadius: BorderRadius.lg,
    padding: Spacing.md,
    alignItems: 'center',
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 1},
    shadowOpacity: 0.04,
    shadowRadius: 4,
    elevation: 2,
  },
  summarySmallValue: {
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
  },
  summarySmallLabel: {
    fontSize: Fonts.sizes.xs,
    color: Colors.textSecondary,
    marginTop: Spacing.xs,
  },
  earningCard: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    backgroundColor: Colors.surface,
    borderRadius: BorderRadius.lg,
    padding: Spacing.base,
    marginBottom: Spacing.sm,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 1},
    shadowOpacity: 0.04,
    shadowRadius: 4,
    elevation: 2,
  },
  earningLeft: {
    flexDirection: 'row',
    alignItems: 'center',
    flex: 1,
    marginRight: Spacing.md,
  },
  earningIconContainer: {
    width: 40,
    height: 40,
    borderRadius: BorderRadius.md,
    backgroundColor: Colors.primary + '15',
    justifyContent: 'center',
    alignItems: 'center',
    marginRight: Spacing.md,
  },
  earningInfo: {
    flex: 1,
  },
  earningRestaurant: {
    fontSize: Fonts.sizes.md,
    fontWeight: Fonts.weights.semibold,
    color: Colors.text,
  },
  earningDate: {
    fontSize: Fonts.sizes.sm,
    color: Colors.textSecondary,
    marginTop: 2,
  },
  earningAmount: {
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

export default EarningsScreen;
