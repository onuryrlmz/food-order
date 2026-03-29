import React, {useState, useEffect, useCallback} from 'react';
import {
  View,
  Text,
  StyleSheet,
  FlatList,
  TouchableOpacity,
  RefreshControl,
} from 'react-native';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import {Colors, Fonts, Spacing, BorderRadius} from '../../theme';
import api from '../../api';
import {useAuth} from '../../context/AuthContext';
import {useToast} from '../../context/ToastContext';
import OrderStatusBadge from '../../components/OrderStatusBadge';
import LoadingSpinner from '../../components/LoadingSpinner';
import EmptyState from '../../components/EmptyState';

const OrderHistoryScreen = ({navigation}) => {
  const {isAuthenticated} = useAuth();
  const {showToast} = useToast();
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

  const loadOrders = async (pageNum) => {
    try {
      const result = await api.customer.order.getHistory({page: pageNum, pageSize: 20});
      if (result?.data) {
        const data = result.data;
        if (pageNum === 1) {
          setOrders(data);
        } else {
          setOrders(prev => [...prev, ...data]);
        }
        setHasMore(data.length === 20);
        setPage(pageNum);
      }
    } catch (e) {
      console.log('Order history error:', e);
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

  const handleReorder = async (orderId) => {
    try {
      const result = await api.customer.order.reorder({orderId});
      if (result && !result.hasFailed) {
        showToast('Urunler sepete eklendi', 'success');
        navigation.navigate('CartTab');
      } else {
        showToast(result?.messages?.[0]?.description || 'Tekrar siparis verilemedi', 'error');
      }
    } catch (e) {
      showToast('Tekrar siparis verilemedi', 'error');
    }
  };

  const formatDate = (dateStr) => {
    if (!dateStr) return '';
    const date = new Date(dateStr);
    const day = date.getDate().toString().padStart(2, '0');
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    const year = date.getFullYear();
    const hours = date.getHours().toString().padStart(2, '0');
    const minutes = date.getMinutes().toString().padStart(2, '0');
    return `${day}.${month}.${year} ${hours}:${minutes}`;
  };

  const renderOrderItem = ({item}) => (
    <TouchableOpacity
      style={styles.orderCard}
      onPress={() => navigation.navigate('OrderDetail', {orderId: item.id})}
      activeOpacity={0.9}
    >
      <View style={styles.orderHeader}>
        <View style={styles.restaurantInfo}>
          <View style={styles.restaurantIconBg}>
            <Icon name="store" size={18} color={Colors.primary} />
          </View>
          <View style={{flex: 1, marginLeft: 10}}>
            <Text style={styles.restaurantName} numberOfLines={1}>{item.restaurantName}</Text>
            <Text style={styles.orderDate}>{formatDate(item.createdDate)}</Text>
          </View>
        </View>
        <OrderStatusBadge statusId={item.statusId} size="small" />
      </View>

      <View style={styles.orderItems}>
        {item.items?.slice(0, 3).map((orderItem, index) => (
          <View key={orderItem.id || index} style={styles.orderItemRow}>
            <Text style={styles.orderItemText} numberOfLines={1}>
              {orderItem.quantity}x {orderItem.menuName}
            </Text>
            {orderItem.values?.length > 0 ? (
              <Text style={styles.orderItemOptions} numberOfLines={1}>
                {orderItem.values.map(v => v.valueName).join(', ')}
              </Text>
            ) : null}
          </View>
        ))}
        {item.items?.length > 3 && (
          <Text style={styles.moreItemsText}>+{item.items.length - 3} ürün daha</Text>
        )}
      </View>

      <View style={styles.orderFooter}>
        <Text style={styles.totalPrice}>₺{item.totalPrice?.toFixed(2)}</Text>
        <View style={styles.footerActions}>
          {item.statusId === 8 && (
            <TouchableOpacity
              style={styles.reorderBtn}
              onPress={() => handleReorder(item.id)}
              activeOpacity={0.8}
            >
              <Icon name="refresh" size={16} color={Colors.primary} />
              <Text style={styles.reorderBtnText}>Tekrar</Text>
            </TouchableOpacity>
          )}
          <View style={styles.detailButton}>
            <Text style={styles.detailButtonText}>Detay</Text>
            <Icon name="chevron-right" size={18} color={Colors.primary} />
          </View>
        </View>
      </View>
    </TouchableOpacity>
  );

  if (loading) {
    return <LoadingSpinner message="Siparişler yükleniyor..." />;
  }

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <Text style={styles.headerTitle}>Siparişlerim</Text>
      </View>
      <FlatList
        data={orders}
        keyExtractor={(item) => item.id}
        renderItem={renderOrderItem}
        ListEmptyComponent={
          <EmptyState
            icon="receipt"
            title="Henüz Siparişiniz Yok"
            message="İlk siparişinizi vererek lezzetleri keşfedin"
            actionLabel="Restoran Keşfet"
            onAction={() => navigation.navigate('HomeTab')}
          />
        }
        refreshControl={
          <RefreshControl refreshing={refreshing} onRefresh={onRefresh} tintColor={Colors.primary} />
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
    paddingHorizontal: Spacing.base,
    paddingTop: 60,
    paddingBottom: Spacing.md,
    backgroundColor: Colors.surface,
    borderBottomWidth: 1,
    borderBottomColor: Colors.borderLight,
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
    marginBottom: Spacing.md,
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
    backgroundColor: Colors.primary + '12',
    justifyContent: 'center',
    alignItems: 'center',
  },
  restaurantName: {
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
  },
  orderDate: {
    fontSize: Fonts.sizes.xs,
    color: Colors.textSecondary,
    marginTop: 2,
  },
  orderItems: {
    paddingVertical: Spacing.sm,
    borderTopWidth: 1,
    borderTopColor: Colors.borderLight,
    borderBottomWidth: 1,
    borderBottomColor: Colors.borderLight,
  },
  orderItemRow: {
    marginBottom: 4,
  },
  orderItemText: {
    fontSize: Fonts.sizes.sm,
    color: Colors.textSecondary,
  },
  orderItemOptions: {
    fontSize: Fonts.sizes.xs,
    color: Colors.textTertiary || '#999',
    marginTop: 1,
    marginLeft: 20,
  },
  moreItemsText: {
    fontSize: Fonts.sizes.xs,
    color: Colors.primary,
    fontWeight: Fonts.weights.semibold,
    marginTop: 4,
  },
  orderFooter: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginTop: Spacing.md,
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
  reorderBtn: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingHorizontal: 10,
    paddingVertical: 6,
    borderRadius: 8,
    backgroundColor: Colors.primary + '12',
    gap: 4,
  },
  reorderBtnText: {
    fontSize: Fonts.sizes.sm,
    color: Colors.primary,
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

export default OrderHistoryScreen;
