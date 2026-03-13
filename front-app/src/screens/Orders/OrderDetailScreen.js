import React, {useState, useEffect} from 'react';
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  TouchableOpacity,
  Alert,
  ActivityIndicator,
} from 'react-native';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import {Colors, Fonts, Spacing, BorderRadius} from '../../theme';
import {orderService} from '../../api';
import {ORDER_STATUS, PAYMENT_OPTIONS} from '../../utils/constants';
import OrderStatusBadge from '../../components/OrderStatusBadge';
import LoadingSpinner from '../../components/LoadingSpinner';

const OrderDetailScreen = ({route, navigation}) => {
  const {orderId} = route.params;
  const [order, setOrder] = useState(null);
  const [loading, setLoading] = useState(true);
  const [cancelling, setCancelling] = useState(false);

  useEffect(() => {
    loadOrder();
  }, [orderId]);

  const loadOrder = async () => {
    try {
      const res = await orderService.getOrderById(orderId);
      if (res.data && !res.data.hasFailed) {
        setOrder(res.data.data);
      }
    } catch (e) {
      console.log('Order detail error:', e);
    } finally {
      setLoading(false);
    }
  };

  const handleCancel = () => {
    Alert.alert(
      'Siparişi İptal Et',
      'Bu siparişi iptal etmek istediğinize emin misiniz?',
      [
        {text: 'Vazgeç', style: 'cancel'},
        {
          text: 'İptal Et',
          style: 'destructive',
          onPress: async () => {
            setCancelling(true);
            try {
              const res = await orderService.cancelOrder(orderId, 'Müşteri iptal etti');
              if (!res.data.hasFailed) {
                Alert.alert('Başarılı', 'Siparişiniz iptal edildi');
                loadOrder();
              } else {
                Alert.alert('Hata', res.data.messages?.[0]?.description || 'İptal edilemedi');
              }
            } catch (e) {
              Alert.alert('Hata', 'Sipariş iptal edilemedi');
            } finally {
              setCancelling(false);
            }
          },
        },
      ],
    );
  };

  const formatDate = (dateStr) => {
    if (!dateStr) return '-';
    const date = new Date(dateStr);
    const day = date.getDate().toString().padStart(2, '0');
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    const year = date.getFullYear();
    const hours = date.getHours().toString().padStart(2, '0');
    const minutes = date.getMinutes().toString().padStart(2, '0');
    return `${day}.${month}.${year} ${hours}:${minutes}`;
  };

  const canCancel = order && (order.statusId === 1 || order.statusId === 2);

  if (loading) {
    return <LoadingSpinner message="Sipariş detayı yükleniyor..." />;
  }

  if (!order) {
    return (
      <View style={styles.errorContainer}>
        <Icon name="alert-circle-outline" size={48} color={Colors.error} />
        <Text style={styles.errorText}>Sipariş bulunamadı</Text>
        <TouchableOpacity style={styles.errorButton} onPress={() => navigation.goBack()}>
          <Text style={styles.errorButtonText}>Geri Dön</Text>
        </TouchableOpacity>
      </View>
    );
  }

  const statusInfo = ORDER_STATUS[order.statusId] || {label: 'Bilinmiyor', color: '#999', icon: 'help-circle-outline'};
  const paymentInfo = PAYMENT_OPTIONS[order.paymentOptionId] || {label: 'Bilinmiyor', icon: 'help'};

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <TouchableOpacity style={styles.backButton} onPress={() => navigation.goBack()}>
          <Icon name="arrow-left" size={24} color={Colors.text} />
        </TouchableOpacity>
        <Text style={styles.headerTitle}>Sipariş Detayı</Text>
        <View style={{width: 44}} />
      </View>

      <ScrollView showsVerticalScrollIndicator={false} contentContainerStyle={styles.scrollContent}>
        <View style={styles.statusCard}>
          <View style={[styles.statusIconBg, {backgroundColor: statusInfo.color + '18'}]}>
            <Icon name={statusInfo.icon} size={32} color={statusInfo.color} />
          </View>
          <Text style={[styles.statusText, {color: statusInfo.color}]}>{statusInfo.label}</Text>
          <Text style={styles.statusDate}>Sipariş: {formatDate(order.createdDate)}</Text>

          <View style={styles.trackingBar}>
            {[1, 2, 3, 4, 5].map(step => {
              const isActive = order.statusId >= step && order.statusId !== 6;
              const isCurrent = order.statusId === step;
              return (
                <View key={step} style={styles.trackingStep}>
                  <View
                    style={[
                      styles.trackingDot,
                      isActive && {backgroundColor: statusInfo.color},
                      isCurrent && styles.trackingDotCurrent,
                    ]}
                  />
                  {step < 5 && (
                    <View
                      style={[
                        styles.trackingLine,
                        isActive && order.statusId > step && {backgroundColor: statusInfo.color},
                      ]}
                    />
                  )}
                </View>
              );
            })}
          </View>
        </View>

        <View style={styles.section}>
          <View style={styles.sectionHeader}>
            <Icon name="store" size={20} color={Colors.primary} />
            <Text style={styles.sectionTitle}>Restoran</Text>
          </View>
          <Text style={styles.restaurantName}>{order.restaurantName}</Text>
        </View>

        <View style={styles.section}>
          <View style={styles.sectionHeader}>
            <Icon name="food" size={20} color={Colors.primary} />
            <Text style={styles.sectionTitle}>Sipariş Kalemleri</Text>
          </View>
          {order.items?.map((item, index) => (
            <View key={item.id || index} style={styles.orderItemRow}>
              <View style={styles.orderItemQtyBadge}>
                <Text style={styles.orderItemQtyText}>{item.quantity}x</Text>
              </View>
              <Text style={styles.orderItemName} numberOfLines={1}>{item.menuName}</Text>
              <Text style={styles.orderItemPrice}>₺{item.totalPrice?.toFixed(2)}</Text>
            </View>
          ))}
        </View>

        <View style={styles.section}>
          <View style={styles.sectionHeader}>
            <Icon name="credit-card-outline" size={20} color={Colors.primary} />
            <Text style={styles.sectionTitle}>Ödeme Bilgileri</Text>
          </View>
          <View style={styles.paymentRow}>
            <Icon name={paymentInfo.icon} size={20} color={Colors.textSecondary} />
            <Text style={styles.paymentText}>{paymentInfo.label}</Text>
          </View>
          <View style={styles.priceBreakdown}>
            <View style={styles.priceRow}>
              <Text style={styles.priceLabel}>Ürünler</Text>
              <Text style={styles.priceValue}>₺{order.totalProductPrice?.toFixed(2)}</Text>
            </View>
            <View style={styles.priceRow}>
              <Text style={styles.priceLabel}>Teslimat</Text>
              <Text style={styles.priceValue}>₺{order.shipmentPrice?.toFixed(2)}</Text>
            </View>
            {order.discountAmount > 0 && (
              <View style={styles.priceRow}>
                <Text style={styles.priceLabel}>İndirim</Text>
                <Text style={[styles.priceValue, {color: Colors.success}]}>-₺{order.discountAmount?.toFixed(2)}</Text>
              </View>
            )}
            <View style={styles.priceDivider} />
            <View style={styles.priceRow}>
              <Text style={styles.totalLabel}>Toplam</Text>
              <Text style={styles.totalValue}>₺{order.totalPrice?.toFixed(2)}</Text>
            </View>
          </View>
        </View>

        {order.notes && (
          <View style={styles.section}>
            <View style={styles.sectionHeader}>
              <Icon name="note-text-outline" size={20} color={Colors.primary} />
              <Text style={styles.sectionTitle}>Sipariş Notu</Text>
            </View>
            <Text style={styles.noteText}>{order.notes}</Text>
          </View>
        )}

        {order.cancellationReason && (
          <View style={[styles.section, {borderLeftWidth: 3, borderLeftColor: Colors.error}]}>
            <View style={styles.sectionHeader}>
              <Icon name="cancel" size={20} color={Colors.error} />
              <Text style={[styles.sectionTitle, {color: Colors.error}]}>İptal Sebebi</Text>
            </View>
            <Text style={styles.noteText}>{order.cancellationReason}</Text>
          </View>
        )}

        <View style={styles.section}>
          <View style={styles.sectionHeader}>
            <Icon name="clock-outline" size={20} color={Colors.primary} />
            <Text style={styles.sectionTitle}>Zaman Çizelgesi</Text>
          </View>
          <View style={styles.timelineItem}>
            <Text style={styles.timelineLabel}>Sipariş Verildi</Text>
            <Text style={styles.timelineValue}>{formatDate(order.createdDate)}</Text>
          </View>
          {order.confirmedAt && (
            <View style={styles.timelineItem}>
              <Text style={styles.timelineLabel}>Onaylandı</Text>
              <Text style={styles.timelineValue}>{formatDate(order.confirmedAt)}</Text>
            </View>
          )}
          {order.deliveredAt && (
            <View style={styles.timelineItem}>
              <Text style={styles.timelineLabel}>Teslim Edildi</Text>
              <Text style={styles.timelineValue}>{formatDate(order.deliveredAt)}</Text>
            </View>
          )}
        </View>

        {canCancel && (
          <TouchableOpacity
            style={styles.cancelButton}
            onPress={handleCancel}
            activeOpacity={0.8}
            disabled={cancelling}
          >
            {cancelling ? (
              <ActivityIndicator color={Colors.error} />
            ) : (
              <>
                <Icon name="close-circle-outline" size={20} color={Colors.error} />
                <Text style={styles.cancelButtonText}>Siparişi İptal Et</Text>
              </>
            )}
          </TouchableOpacity>
        )}
      </ScrollView>
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
    justifyContent: 'space-between',
    paddingHorizontal: Spacing.base,
    paddingTop: 54,
    paddingBottom: Spacing.md,
    backgroundColor: Colors.surface,
    borderBottomWidth: 1,
    borderBottomColor: Colors.borderLight,
  },
  backButton: {
    width: 44,
    height: 44,
    borderRadius: 22,
    backgroundColor: Colors.borderLight,
    justifyContent: 'center',
    alignItems: 'center',
  },
  headerTitle: {
    fontSize: Fonts.sizes.lg,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
  },
  scrollContent: {
    paddingBottom: Spacing.xxxl,
  },
  statusCard: {
    backgroundColor: Colors.surface,
    marginHorizontal: Spacing.base,
    marginTop: Spacing.md,
    borderRadius: BorderRadius.xl,
    padding: Spacing.xl,
    alignItems: 'center',
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 2},
    shadowOpacity: 0.06,
    shadowRadius: 8,
    elevation: 4,
  },
  statusIconBg: {
    width: 72,
    height: 72,
    borderRadius: 36,
    justifyContent: 'center',
    alignItems: 'center',
    marginBottom: Spacing.md,
  },
  statusText: {
    fontSize: Fonts.sizes.xl,
    fontWeight: Fonts.weights.heavy,
    marginBottom: 4,
  },
  statusDate: {
    fontSize: Fonts.sizes.sm,
    color: Colors.textSecondary,
  },
  trackingBar: {
    flexDirection: 'row',
    alignItems: 'center',
    marginTop: Spacing.lg,
    paddingHorizontal: Spacing.lg,
  },
  trackingStep: {
    flexDirection: 'row',
    alignItems: 'center',
    flex: 1,
  },
  trackingDot: {
    width: 12,
    height: 12,
    borderRadius: 6,
    backgroundColor: Colors.border,
  },
  trackingDotCurrent: {
    width: 16,
    height: 16,
    borderRadius: 8,
    borderWidth: 3,
    borderColor: '#FFF',
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 1},
    shadowOpacity: 0.2,
    shadowRadius: 2,
    elevation: 2,
  },
  trackingLine: {
    flex: 1,
    height: 3,
    backgroundColor: Colors.border,
    marginHorizontal: 2,
    borderRadius: 1.5,
  },
  section: {
    backgroundColor: Colors.surface,
    marginHorizontal: Spacing.base,
    marginTop: Spacing.md,
    borderRadius: BorderRadius.lg,
    padding: Spacing.base,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 1},
    shadowOpacity: 0.04,
    shadowRadius: 4,
    elevation: 2,
  },
  sectionHeader: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: Spacing.md,
  },
  sectionTitle: {
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
    marginLeft: 8,
  },
  restaurantName: {
    fontSize: Fonts.sizes.lg,
    fontWeight: Fonts.weights.semibold,
    color: Colors.text,
  },
  orderItemRow: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingVertical: 8,
    borderBottomWidth: 1,
    borderBottomColor: Colors.borderLight,
  },
  orderItemQtyBadge: {
    backgroundColor: Colors.primary + '15',
    paddingHorizontal: 8,
    paddingVertical: 3,
    borderRadius: 6,
    marginRight: 10,
  },
  orderItemQtyText: {
    fontSize: Fonts.sizes.sm,
    fontWeight: Fonts.weights.bold,
    color: Colors.primary,
  },
  orderItemName: {
    flex: 1,
    fontSize: Fonts.sizes.md,
    color: Colors.text,
  },
  orderItemPrice: {
    fontSize: Fonts.sizes.md,
    fontWeight: Fonts.weights.semibold,
    color: Colors.text,
    marginLeft: 8,
  },
  paymentRow: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: Spacing.md,
  },
  paymentText: {
    fontSize: Fonts.sizes.md,
    color: Colors.text,
    fontWeight: Fonts.weights.medium,
    marginLeft: 8,
  },
  priceBreakdown: {
    backgroundColor: Colors.borderLight,
    borderRadius: BorderRadius.md,
    padding: Spacing.md,
  },
  priceRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginBottom: 6,
  },
  priceLabel: {
    fontSize: Fonts.sizes.md,
    color: Colors.textSecondary,
  },
  priceValue: {
    fontSize: Fonts.sizes.md,
    color: Colors.text,
    fontWeight: Fonts.weights.medium,
  },
  priceDivider: {
    height: 1,
    backgroundColor: Colors.border,
    marginVertical: Spacing.sm,
  },
  totalLabel: {
    fontSize: Fonts.sizes.lg,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
  },
  totalValue: {
    fontSize: Fonts.sizes.lg,
    fontWeight: Fonts.weights.heavy,
    color: Colors.primary,
  },
  noteText: {
    fontSize: Fonts.sizes.md,
    color: Colors.textSecondary,
    lineHeight: 22,
  },
  timelineItem: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    paddingVertical: 6,
    borderBottomWidth: 1,
    borderBottomColor: Colors.borderLight,
  },
  timelineLabel: {
    fontSize: Fonts.sizes.md,
    color: Colors.textSecondary,
  },
  timelineValue: {
    fontSize: Fonts.sizes.sm,
    color: Colors.text,
    fontWeight: Fonts.weights.medium,
  },
  cancelButton: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    marginHorizontal: Spacing.base,
    marginTop: Spacing.lg,
    paddingVertical: 16,
    borderRadius: BorderRadius.lg,
    backgroundColor: '#FFEBEE',
    borderWidth: 1,
    borderColor: Colors.error + '30',
  },
  cancelButtonText: {
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.bold,
    color: Colors.error,
    marginLeft: 8,
  },
  errorContainer: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: Colors.background,
    padding: Spacing.xxl,
  },
  errorText: {
    fontSize: Fonts.sizes.lg,
    color: Colors.text,
    fontWeight: Fonts.weights.semibold,
    marginTop: Spacing.md,
  },
  errorButton: {
    marginTop: Spacing.lg,
    backgroundColor: Colors.primary,
    paddingHorizontal: Spacing.xl,
    paddingVertical: Spacing.md,
    borderRadius: 12,
  },
  errorButtonText: {
    color: '#FFF',
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.bold,
  },
});

export default OrderDetailScreen;
