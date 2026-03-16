import React, {useState, useEffect, useRef} from 'react';
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  TouchableOpacity,
  ActivityIndicator,
  Dimensions,
  Linking,
  Platform,
} from 'react-native';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import MapView, {Marker} from 'react-native-maps';
import {Colors, Fonts, Spacing, BorderRadius} from '../../theme';
import {orderService, reviewService} from '../../api';
import {useToast} from '../../context/ToastContext';
import {ORDER_STATUS, PAYMENT_OPTIONS} from '../../utils/constants';
import OrderStatusBadge from '../../components/OrderStatusBadge';
import LoadingSpinner from '../../components/LoadingSpinner';
import ReviewModal from '../../components/ReviewModal';
import {createOrderConnection} from '../../utils/signalr';

const OrderDetailScreen = ({route, navigation}) => {
  const {showToast, showConfirm} = useToast();
  const {orderId} = route.params;
  const [order, setOrder] = useState(null);
  const [loading, setLoading] = useState(true);
  const [cancelling, setCancelling] = useState(false);
  const [courierLocation, setCourierLocation] = useState(null);
  const [reviewModalVisible, setReviewModalVisible] = useState(false);
  const [reordering, setReordering] = useState(false);

  const connectionRef = useRef(null);

  useEffect(() => {
    loadOrder();
  }, [orderId]);

  // SignalR real-time updates for order status and courier location
  useEffect(() => {
    let conn = null;

    const setupSignalR = async () => {
      try {
        conn = await createOrderConnection(orderId);
        connectionRef.current = conn;

        conn.on('OrderStatusChanged', (changedOrderId, newStatus) => {
          if (changedOrderId === orderId) {
            loadOrder();
          }
        });

        conn.on('CourierLocationUpdated', (changedOrderId, lat, lng) => {
          if (changedOrderId === orderId) {
            setCourierLocation(prev => ({
              ...prev,
              latitude: lat,
              longitude: lng,
            }));
          }
        });
      } catch (e) {
        console.log('SignalR setup error:', e);
      }
    };

    setupSignalR();

    return () => {
      if (connectionRef.current) {
        connectionRef.current.stop().catch(() => {});
        connectionRef.current = null;
      }
    };
  }, [orderId]);

  // Initial courier location fetch (fallback if SignalR not yet connected)
  useEffect(() => {
    if (order?.statusId !== 7) return;

    const fetchLocation = async () => {
      try {
        const res = await orderService.getCourierLocation(orderId);
        if (res.data && !res.data.hasFailed && res.data.data) {
          setCourierLocation({
            latitude: res.data.data.latitude,
            longitude: res.data.data.longitude,
            courierName: res.data.data.courierName,
            courierPhone: res.data.data.courierPhone,
          });
        }
      } catch (e) {
        console.log('Courier location error:', e);
      }
    };

    fetchLocation();
  }, [order?.statusId, orderId]);

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
    showConfirm({
      title: 'Siparişi İptal Et',
      message: 'Bu siparişi iptal etmek istediğinize emin misiniz?',
      confirmText: 'İptal Et',
      cancelText: 'Vazgeç',
      confirmStyle: 'destructive',
      onConfirm: async () => {
        setCancelling(true);
        try {
          const res = await orderService.cancelOrder(orderId, 'Müşteri iptal etti');
          if (!res.data.hasFailed) {
            showToast('Siparişiniz iptal edildi', 'success');
            loadOrder();
          } else {
            showToast(res.data.messages?.[0]?.description || 'İptal edilemedi', 'error');
          }
        } catch (e) {
          showToast('Sipariş iptal edilemedi', 'error');
        } finally {
          setCancelling(false);
        }
      },
    });
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

  const handleReviewSubmit = async ({rating, comment}) => {
    try {
      const res = await reviewService.createReview({
        orderId,
        restaurantId: order.restaurantId,
        rating,
        comment,
      });
      if (res.data && !res.data.hasFailed) {
        showToast('Degerlendirmeniz kaydedildi', 'success');
      } else {
        showToast(res.data?.messages?.[0]?.description || 'Degerlendirme gonderilemedi', 'error');
      }
    } catch (e) {
      showToast('Degerlendirme gonderilemedi', 'error');
      throw e;
    }
  };

  const handleReorder = async () => {
    setReordering(true);
    try {
      const res = await orderService.reorder(orderId);
      if (res.data && !res.data.hasFailed) {
        showToast('Urunler sepete eklendi', 'success');
        navigation.navigate('CartTab');
      } else {
        showToast(res.data?.messages?.[0]?.description || 'Tekrar siparis verilemedi', 'error');
      }
    } catch (e) {
      showToast('Tekrar siparis verilemedi', 'error');
    } finally {
      setReordering(false);
    }
  };

  const canCancel = order && (order.statusId === 1 || order.statusId === 2);
  const isDelivered = order && order.statusId === 8;

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

        {order.statusId === 7 && courierLocation && (
          <View style={styles.courierTrackingSection}>
            <View style={styles.sectionHeader}>
              <Icon name="bike-fast" size={20} color={Colors.primary} />
              <Text style={styles.sectionTitle}>Kurye Takibi</Text>
            </View>
            <View style={styles.mapContainer}>
              <MapView
                style={styles.map}
                initialRegion={{
                  latitude: courierLocation.latitude,
                  longitude: courierLocation.longitude,
                  latitudeDelta: 0.02,
                  longitudeDelta: 0.02,
                }}
                region={{
                  latitude: courierLocation.latitude,
                  longitude: courierLocation.longitude,
                  latitudeDelta: 0.02,
                  longitudeDelta: 0.02,
                }}>
                <Marker
                  coordinate={{
                    latitude: courierLocation.latitude,
                    longitude: courierLocation.longitude,
                  }}
                  title="Kurye"
                  pinColor="blue"
                />
                {order.deliveryLatitude && order.deliveryLongitude && (
                  <Marker
                    coordinate={{
                      latitude: order.deliveryLatitude,
                      longitude: order.deliveryLongitude,
                    }}
                    title="Teslimat Adresi"
                    pinColor="red"
                  />
                )}
              </MapView>
            </View>
            {courierLocation.courierName && (
              <View style={styles.courierInfo}>
                <View style={styles.courierNameRow}>
                  <Icon name="account" size={18} color={Colors.text} />
                  <Text style={styles.courierName}>{courierLocation.courierName}</Text>
                </View>
                {courierLocation.courierPhone && (
                  <TouchableOpacity
                    style={styles.callButton}
                    onPress={() => Linking.openURL(`tel:${courierLocation.courierPhone}`)}
                    activeOpacity={0.7}>
                    <Icon name="phone" size={16} color="#fff" />
                    <Text style={styles.callButtonText}>Ara</Text>
                  </TouchableOpacity>
                )}
              </View>
            )}
          </View>
        )}

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
            <View key={item.id || index} style={styles.orderItemCard}>
              <View style={styles.orderItemTop}>
                <View style={styles.orderItemQtyBadge}>
                  <Text style={styles.orderItemQtyText}>{item.quantity}x</Text>
                </View>
                <View style={{flex: 1}}>
                  <Text style={styles.orderItemName} numberOfLines={2}>{item.menuName}</Text>
                  {item.description ? (
                    <Text style={styles.orderItemDesc} numberOfLines={2}>{item.description}</Text>
                  ) : null}
                </View>
                <Text style={styles.orderItemPrice}>₺{item.totalPrice?.toFixed(2)}</Text>
              </View>
              {item.unitPrice ? (
                <Text style={styles.orderItemUnitPrice}>Birim: ₺{Number(item.unitPrice).toFixed(2)}</Text>
              ) : null}
              {item.values?.length > 0 ? (
                <View style={styles.orderItemValues}>
                  {item.values.map((val, vi) => (
                    <View key={vi}>
                      <View style={styles.valueRow}>
                        <Text style={styles.valueName}>{val.optionName}: {val.valueName}{val.quantity > 1 ? ` x${val.quantity}` : ''}</Text>
                        {val.unitPrice > 0 ? <Text style={styles.valuePrice}>+₺{Number(val.totalPrice).toFixed(2)}</Text> : null}
                      </View>
                      {val.options?.length > 0 ? val.options.map((opt, oi) => (
                        <View key={oi} style={styles.subValueRow}>
                          <Text style={styles.subValueName}>{opt.optionName}: {opt.valueName}{opt.quantity > 1 ? ` x${opt.quantity}` : ''}</Text>
                          {opt.unitPrice > 0 ? <Text style={styles.subValuePrice}>+₺{Number(opt.totalPrice).toFixed(2)}</Text> : null}
                        </View>
                      )) : null}
                    </View>
                  ))}
                </View>
              ) : null}
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
                <Text style={styles.priceLabel}>İndirim{order.couponCode ? ` (${order.couponCode})` : ''}</Text>
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

        {isDelivered && (
          <View style={styles.deliveredActions}>
            <TouchableOpacity
              style={styles.reviewButton}
              onPress={() => setReviewModalVisible(true)}
              activeOpacity={0.8}
            >
              <Icon name="star-outline" size={20} color={Colors.star} />
              <Text style={styles.reviewButtonText}>Degerlendir</Text>
            </TouchableOpacity>
            <TouchableOpacity
              style={styles.reorderButton}
              onPress={handleReorder}
              activeOpacity={0.8}
              disabled={reordering}
            >
              {reordering ? (
                <ActivityIndicator color="#FFF" size="small" />
              ) : (
                <>
                  <Icon name="refresh" size={20} color="#FFF" />
                  <Text style={styles.reorderButtonText}>Tekrar Siparis</Text>
                </>
              )}
            </TouchableOpacity>
          </View>
        )}

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

      <ReviewModal
        visible={reviewModalVisible}
        onClose={() => setReviewModalVisible(false)}
        onSubmit={handleReviewSubmit}
        restaurantName={order?.restaurantName}
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
  orderItemCard: {
    backgroundColor: Colors.borderLight + '60',
    borderRadius: BorderRadius.md,
    padding: Spacing.md,
    marginBottom: Spacing.sm,
  },
  orderItemTop: {
    flexDirection: 'row',
    alignItems: 'flex-start',
  },
  orderItemQtyBadge: {
    backgroundColor: Colors.primary + '15',
    paddingHorizontal: 8,
    paddingVertical: 3,
    borderRadius: 6,
    marginRight: 10,
    marginTop: 2,
  },
  orderItemQtyText: {
    fontSize: Fonts.sizes.sm,
    fontWeight: Fonts.weights.bold,
    color: Colors.primary,
  },
  orderItemName: {
    fontSize: Fonts.sizes.md,
    fontWeight: Fonts.weights.semibold,
    color: Colors.text,
  },
  orderItemDesc: {
    fontSize: Fonts.sizes.sm,
    color: Colors.textSecondary,
    marginTop: 2,
  },
  orderItemPrice: {
    fontSize: Fonts.sizes.md,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
    marginLeft: 8,
  },
  orderItemUnitPrice: {
    fontSize: Fonts.sizes.xs,
    color: Colors.textSecondary,
    marginTop: 4,
    marginLeft: 42,
  },
  orderItemValues: {
    marginTop: 8,
    marginLeft: 42,
    borderTopWidth: 1,
    borderTopColor: Colors.borderLight,
    paddingTop: 8,
  },
  valueRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 4,
  },
  valueName: {
    fontSize: Fonts.sizes.sm,
    color: Colors.textSecondary,
    flex: 1,
  },
  valuePrice: {
    fontSize: Fonts.sizes.sm,
    color: Colors.textSecondary,
    fontWeight: Fonts.weights.medium,
    marginLeft: 8,
  },
  subValueRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 3,
    marginLeft: 12,
  },
  subValueName: {
    fontSize: Fonts.sizes.xs,
    color: Colors.textTertiary || '#999',
    flex: 1,
  },
  subValuePrice: {
    fontSize: Fonts.sizes.xs,
    color: Colors.textTertiary || '#999',
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
  courierTrackingSection: {
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
  mapContainer: {
    borderRadius: BorderRadius.md,
    overflow: 'hidden',
    height: 200,
    marginBottom: Spacing.md,
  },
  map: {
    flex: 1,
  },
  courierInfo: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
  },
  courierNameRow: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: 6,
  },
  courierName: {
    fontSize: Fonts.sizes.md,
    fontWeight: Fonts.weights.semibold,
    color: Colors.text,
  },
  callButton: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#34C759',
    paddingHorizontal: 14,
    paddingVertical: 8,
    borderRadius: 8,
    gap: 4,
  },
  callButtonText: {
    color: '#fff',
    fontSize: Fonts.sizes.sm,
    fontWeight: Fonts.weights.bold,
  },
  deliveredActions: {
    flexDirection: 'row',
    marginHorizontal: Spacing.base,
    marginTop: Spacing.lg,
    gap: 10,
  },
  reviewButton: {
    flex: 1,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: 14,
    borderRadius: BorderRadius.lg,
    backgroundColor: '#FFF9E6',
    borderWidth: 1,
    borderColor: Colors.star + '30',
  },
  reviewButtonText: {
    fontSize: Fonts.sizes.md,
    fontWeight: Fonts.weights.bold,
    color: '#B8860B',
    marginLeft: 6,
  },
  reorderButton: {
    flex: 1,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: 14,
    borderRadius: BorderRadius.lg,
    backgroundColor: Colors.primary,
  },
  reorderButtonText: {
    fontSize: Fonts.sizes.md,
    fontWeight: Fonts.weights.bold,
    color: '#FFF',
    marginLeft: 6,
  },
});

export default OrderDetailScreen;
