import React, {useState, useEffect, useCallback} from 'react';
import {
  View,
  Text,
  StyleSheet,
  FlatList,
  TouchableOpacity,
  RefreshControl,
} from 'react-native';
import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import {Colors, Fonts, Spacing, BorderRadius} from '../../theme';
import {scheduledOrderService} from '../../api';
import {useAuth} from '../../context/AuthContext';
import {useToast} from '../../context/ToastContext';
import LoadingSpinner from '../../components/LoadingSpinner';
import EmptyState from '../../components/EmptyState';

const STATUS_MAP = {
  Scheduled: {label: 'Planlandı', color: '#5856D6'},
  Processing: {label: 'İşleniyor', color: '#FF9500'},
  Cancelled: {label: 'İptal', color: '#FF3B30'},
  Converted: {label: 'Dönüştü', color: '#34C759'},
};

import { router } from 'expo-router';
const ScheduledOrdersScreen = () => {
  const {isAuthenticated} = useAuth();
  const {showToast, showConfirm} = useToast();
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);

  useEffect(() => {
    if (isAuthenticated) {
      loadOrders(1);
    } else {
      setLoading(false);
    }
  }, [isAuthenticated]);

  const loadOrders = async pageNum => {
    try {
      const res = await scheduledOrderService.getAll(pageNum, 20);
      if (res.data?.data) {
        const data = res.data.data;
        if (pageNum === 1) {
          setOrders(data);
        } else {
          setOrders(prev => [...prev, ...data]);
        }
        setHasMore(data.length === 20);
        setPage(pageNum);
      }
    } catch (e) {
      console.log('Scheduled orders error:', e);
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  };

  const onRefresh = useCallback(() => {
    setRefreshing(true);
    loadOrders(1);
  }, []);

  const onEndReached = () => {
    if (hasMore && !loading) {
      loadOrders(page + 1);
    }
  };

  const handleCancel = order => {
    showConfirm({
      title: 'Siparişi İptal Et',
      message: `"${order.restaurantName}" için zamanlı siparişi iptal etmek istediğinize emin misiniz?`,
      confirmText: 'İptal Et',
      cancelText: 'Vazgeç',
      confirmStyle: 'destructive',
      onConfirm: async () => {
        try {
          const res = await scheduledOrderService.cancel(order.id);
          if (res.data && !res.data.hasFailed) {
            setOrders(prev =>
              prev.map(o =>
                o.id === order.id ? {...o, status: 'Cancelled'} : o,
              ),
            );
            showToast('Zamanlı sipariş iptal edildi', 'success');
          } else {
            showToast(
              res.data?.messages?.[0]?.description || 'İptal işlemi başarısız',
              'error',
            );
          }
        } catch (e) {
          showToast('İptal işlemi başarısız', 'error');
        }
      },
    });
  };

  const formatDateTime = dateStr => {
    if (!dateStr) return '';
    const date = new Date(dateStr);
    const day = date.getDate().toString().padStart(2, '0');
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    const year = date.getFullYear();
    const hours = date.getHours().toString().padStart(2, '0');
    const minutes = date.getMinutes().toString().padStart(2, '0');
    return `${day}.${month}.${year} ${hours}:${minutes}`;
  };

  const renderStatusBadge = status => {
    const statusInfo = STATUS_MAP[status] || {label: status, color: '#8E8E93'};
    return (
      <View style={[styles.statusBadge, {backgroundColor: statusInfo.color + '18'}]}>
        <Text style={[styles.statusText, {color: statusInfo.color}]}>
          {statusInfo.label}
        </Text>
      </View>
    );
  };

  const renderOrderItem = ({item}) => (
    <TouchableOpacity
      style={styles.orderCard}
      onPress={() =>
        router.push(`/order/${item.id}`)
      }
      activeOpacity={0.9}>
      <View style={styles.orderHeader}>
        <View style={styles.restaurantInfo}>
          <View style={styles.restaurantIconBg}>
            <Icon name="clock-outline" size={18} color="#5856D6" />
          </View>
          <View style={{flex: 1, marginLeft: 10}}>
            <Text style={styles.restaurantName} numberOfLines={1}>
              {item.restaurantName}
            </Text>
            <View style={styles.timeRow}>
              <Icon
                name="calendar-clock"
                size={14}
                color={Colors.textSecondary}
              />
              <Text style={styles.scheduledTime}>
                {formatDateTime(item.scheduledTime)}
              </Text>
            </View>
          </View>
        </View>
        {renderStatusBadge(item.status)}
      </View>

      {item.totalPrice !== undefined && (
        <View style={styles.orderFooter}>
          <Text style={styles.totalPrice}>
            ₺{item.totalPrice?.toFixed(2)}
          </Text>
          <View style={styles.footerActions}>
            {item.status === 'Scheduled' && (
              <TouchableOpacity
                style={styles.cancelBtn}
                onPress={() => handleCancel(item)}
                activeOpacity={0.8}>
                <Icon name="close-circle-outline" size={16} color={Colors.error} />
                <Text style={styles.cancelBtnText}>İptal</Text>
              </TouchableOpacity>
            )}
            <View style={styles.detailButton}>
              <Text style={styles.detailButtonText}>Detay</Text>
              <Icon name="chevron-right" size={18} color={Colors.primary} />
            </View>
          </View>
        </View>
      )}
    </TouchableOpacity>
  );

  if (loading) {
    return <LoadingSpinner message="Zamanlı siparişler yükleniyor..." />;
  }

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <TouchableOpacity
          onPress={() => router.back()}
          style={styles.backButton}
          activeOpacity={0.8}>
          <Icon name="arrow-left" size={24} color={Colors.text} />
        </TouchableOpacity>
        <Text style={styles.headerTitle}>Zamanlı Siparişler</Text>
      </View>
      <FlatList
        data={orders}
        keyExtractor={item => item.id}
        renderItem={renderOrderItem}
        ListEmptyComponent={
          <EmptyState
            icon="clock-outline"
            title="Zamanlı Sipariş Yok"
            message="Henüz zamanlı siparişiniz bulunmuyor"
            actionLabel="Sipariş Ver"
            onAction={() => router.push('/(tabs)')}
          />
        }
        refreshControl={
          <RefreshControl
            refreshing={refreshing}
            onRefresh={onRefresh}
            tintColor={Colors.primary}
          />
        }
        onEndReached={onEndReached}
        onEndReachedThreshold={0.3}
        contentContainerStyle={styles.listContent}
        showsVerticalScrollIndicator={false}
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
    paddingHorizontal: Spacing.base,
    paddingTop: 60,
    paddingBottom: Spacing.md,
    backgroundColor: Colors.surface,
    borderBottomWidth: 1,
    borderBottomColor: Colors.borderLight,
  },
  backButton: {
    marginRight: Spacing.sm,
  },
  headerTitle: {
    fontSize: Fonts.sizes.xxl,
    fontWeight: Fonts.weights.heavy,
    color: Colors.text,
  },
  listContent: {
    paddingBottom: Spacing.xxl,
    flexGrow: 1,
  },
  orderCard: {
    backgroundColor: Colors.surface,
    marginHorizontal: Spacing.base,
    marginTop: Spacing.md,
    borderRadius: BorderRadius.lg,
    padding: Spacing.base,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 2},
    shadowOpacity: 0.05,
    shadowRadius: 8,
    elevation: 3,
  },
  orderHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'flex-start',
  },
  restaurantInfo: {
    flexDirection: 'row',
    alignItems: 'center',
    flex: 1,
    marginRight: Spacing.sm,
  },
  restaurantIconBg: {
    width: 38,
    height: 38,
    borderRadius: 10,
    backgroundColor: '#5856D6' + '12',
    justifyContent: 'center',
    alignItems: 'center',
  },
  restaurantName: {
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
  },
  timeRow: {
    flexDirection: 'row',
    alignItems: 'center',
    marginTop: 4,
    gap: 4,
  },
  scheduledTime: {
    fontSize: Fonts.sizes.xs,
    color: Colors.textSecondary,
  },
  statusBadge: {
    paddingHorizontal: 10,
    paddingVertical: 4,
    borderRadius: 8,
  },
  statusText: {
    fontSize: Fonts.sizes.xs,
    fontWeight: Fonts.weights.bold,
  },
  orderFooter: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginTop: Spacing.md,
    paddingTop: Spacing.md,
    borderTopWidth: 1,
    borderTopColor: Colors.borderLight,
  },
  totalPrice: {
    fontSize: Fonts.sizes.lg,
    fontWeight: Fonts.weights.heavy,
    color: Colors.text,
  },
  footerActions: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: 12,
  },
  cancelBtn: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingHorizontal: 10,
    paddingVertical: 6,
    borderRadius: 8,
    backgroundColor: Colors.error + '12',
    gap: 4,
  },
  cancelBtnText: {
    fontSize: Fonts.sizes.sm,
    color: Colors.error,
    fontWeight: Fonts.weights.bold,
  },
  detailButton: {
    flexDirection: 'row',
    alignItems: 'center',
  },
  detailButtonText: {
    fontSize: Fonts.sizes.sm,
    color: Colors.primary,
    fontWeight: Fonts.weights.bold,
  },
});

export default ScheduledOrdersScreen;
