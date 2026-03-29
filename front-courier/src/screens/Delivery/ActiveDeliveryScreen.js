import React, {useState, useEffect} from 'react';
import {
  View,
  Text,
  StyleSheet,
  TouchableOpacity,
  ScrollView,
  Alert,
  ActivityIndicator,
  Linking,
} from 'react-native';
import {useNavigation, useRoute} from '@react-navigation/native';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import {Colors, Fonts, Spacing, BorderRadius} from '../../theme';
import {DELIVERY_STATUS} from '../../utils/constants';
import api from '../../api';

const ActiveDeliveryScreen = () => {
  const navigation = useNavigation();
  const route = useRoute();
  const [assignment, setAssignment] = useState(route.params?.assignment || null);
  const [loading, setLoading] = useState(false);
  const [actionLoading, setActionLoading] = useState('');

  useEffect(() => {
    if (!assignment) {
      fetchActiveAssignment();
    }
  }, []);

  const fetchActiveAssignment = async () => {
    setLoading(true);
    try {
      const result = await api.courier.courier.getActiveAssignment();
      if (!result.hasFailed && result.data) {
        setAssignment(result.data);
      }
    } catch (error) {
      // Silent fail
    } finally {
      setLoading(false);
    }
  };

  const handleAccept = async () => {
    setActionLoading('accept');
    try {
      const result = await api.courier.courier.acceptAssignment({assignmentId: assignment.id});
      if (!result.hasFailed) {
        setAssignment({...assignment, status: 3});
      } else {
        Alert.alert('Hata', result.messages?.[0]?.description || 'İşlem başarısız');
      }
    } catch (error) {
      Alert.alert('Hata', 'Bağlantı hatası oluştu');
    } finally {
      setActionLoading('');
    }
  };

  const handleReject = async () => {
    Alert.alert(
      'Teslimatı Reddet',
      'Bu teslimatı reddetmek istediğinize emin misiniz?',
      [
        {text: 'İptal', style: 'cancel'},
        {
          text: 'Reddet',
          style: 'destructive',
          onPress: async () => {
            setActionLoading('reject');
            try {
              const result = await api.courier.courier.rejectAssignment({assignmentId: assignment.id});
              if (!result.hasFailed) {
                navigation.goBack();
              } else {
                Alert.alert('Hata', result.messages?.[0]?.description || 'İşlem başarısız');
              }
            } catch (error) {
              Alert.alert('Hata', 'Bağlantı hatası oluştu');
            } finally {
              setActionLoading('');
            }
          },
        },
      ],
    );
  };

  const handlePickedUp = async () => {
    setActionLoading('pickedUp');
    try {
      const result = await api.courier.courier.markPickedUp({assignmentId: assignment.id});
      if (!result.hasFailed) {
        setAssignment({...assignment, status: 5});
      } else {
        Alert.alert('Hata', result.messages?.[0]?.description || 'İşlem başarısız');
      }
    } catch (error) {
      Alert.alert('Hata', 'Bağlantı hatası oluştu');
    } finally {
      setActionLoading('');
    }
  };

  const handleDelivered = async () => {
    Alert.alert(
      'Teslimat Tamamla',
      'Siparişi teslim ettiğinizi onaylıyor musunuz?',
      [
        {text: 'İptal', style: 'cancel'},
        {
          text: 'Teslim Ettim',
          onPress: async () => {
            setActionLoading('delivered');
            try {
              const result = await api.courier.courier.markDelivered({assignmentId: assignment.id});
              if (!result.hasFailed) {
                Alert.alert('Başarılı', 'Teslimat tamamlandı!', [
                  {text: 'Tamam', onPress: () => navigation.goBack()},
                ]);
              } else {
                Alert.alert('Hata', result.messages?.[0]?.description || 'İşlem başarısız');
              }
            } catch (error) {
              Alert.alert('Hata', 'Bağlantı hatası oluştu');
            } finally {
              setActionLoading('');
            }
          },
        },
      ],
    );
  };

  const handleCall = (phone) => {
    if (phone) {
      Linking.openURL(`tel:${phone}`);
    }
  };

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color={Colors.primary} />
      </View>
    );
  }

  if (!assignment) {
    return (
      <View style={styles.emptyContainer}>
        <Icon name="package-variant" size={60} color={Colors.textTertiary} />
        <Text style={styles.emptyText}>Aktif teslimat bulunamadı</Text>
        <TouchableOpacity style={styles.backButton} onPress={() => navigation.goBack()}>
          <Text style={styles.backButtonText}>Geri Dön</Text>
        </TouchableOpacity>
      </View>
    );
  }

  const status = assignment.status;
  const statusInfo = DELIVERY_STATUS[status] || {};

  return (
    <View style={styles.container}>
      {/* Header */}
      <View style={styles.header}>
        <TouchableOpacity onPress={() => navigation.goBack()} style={styles.headerBackButton}>
          <Icon name="arrow-left" size={24} color={Colors.text} />
        </TouchableOpacity>
        <Text style={styles.headerTitle}>Teslimat Detayı</Text>
        <View style={[styles.headerStatusBadge, {backgroundColor: (statusInfo.color || Colors.textSecondary) + '20'}]}>
          <Text style={[styles.headerStatusText, {color: statusInfo.color || Colors.textSecondary}]}>
            {statusInfo.label || 'Bilinmiyor'}
          </Text>
        </View>
      </View>

      <ScrollView style={styles.content}>
        {/* Restaurant Info */}
        <View style={styles.infoCard}>
          <View style={styles.infoCardHeader}>
            <Icon name="store" size={22} color={Colors.primary} />
            <Text style={styles.infoCardTitle}>Restoran</Text>
          </View>
          <Text style={styles.infoName}>{assignment.restaurantName || 'Restoran'}</Text>
          <Text style={styles.infoAddress}>{assignment.restaurantAddress || '-'}</Text>
          {assignment.restaurantPhone ? (
            <TouchableOpacity
              style={styles.callButton}
              onPress={() => handleCall(assignment.restaurantPhone)}>
              <Icon name="phone" size={18} color={Colors.primary} />
              <Text style={styles.callButtonText}>Ara</Text>
            </TouchableOpacity>
          ) : null}
        </View>

        {/* Customer Info */}
        <View style={styles.infoCard}>
          <View style={styles.infoCardHeader}>
            <Icon name="account" size={22} color={Colors.primary} />
            <Text style={styles.infoCardTitle}>Müşteri</Text>
          </View>
          <Text style={styles.infoName}>{assignment.customerName || 'Müşteri'}</Text>
          <Text style={styles.infoAddress}>{assignment.customerAddress || '-'}</Text>
          {assignment.customerPhone ? (
            <TouchableOpacity
              style={styles.callButton}
              onPress={() => handleCall(assignment.customerPhone)}>
              <Icon name="phone" size={18} color={Colors.primary} />
              <Text style={styles.callButtonText}>Ara</Text>
            </TouchableOpacity>
          ) : null}
        </View>

        {/* Order Summary */}
        <View style={styles.infoCard}>
          <View style={styles.infoCardHeader}>
            <Icon name="clipboard-list" size={22} color={Colors.primary} />
            <Text style={styles.infoCardTitle}>Sipariş Detayı</Text>
          </View>
          {assignment.orderItems?.map((item, index) => (
            <View key={index} style={styles.orderItemRow}>
              <Text style={styles.orderItemQuantity}>{item.quantity}x</Text>
              <Text style={styles.orderItemName}>{item.name}</Text>
              <Text style={styles.orderItemPrice}>{item.totalPrice?.toFixed(2)} TL</Text>
            </View>
          ))}
          {assignment.orderNote ? (
            <View style={styles.orderNoteContainer}>
              <Icon name="note-text" size={16} color={Colors.warning} />
              <Text style={styles.orderNoteText}>{assignment.orderNote}</Text>
            </View>
          ) : null}
          <View style={styles.totalRow}>
            <Text style={styles.totalLabel}>Toplam Tutar</Text>
            <Text style={styles.totalValue}>{assignment.orderTotal?.toFixed(2) || '0.00'} TL</Text>
          </View>
        </View>

        {/* Delivery Fee */}
        <View style={styles.feeCard}>
          <Icon name="cash" size={24} color={Colors.success} />
          <View style={styles.feeInfo}>
            <Text style={styles.feeLabel}>Teslimat Ücreti</Text>
            <Text style={styles.feeValue}>{assignment.deliveryFee?.toFixed(2) || '0.00'} TL</Text>
          </View>
        </View>

        {/* Spacer for action buttons */}
        <View style={{height: 120}} />
      </ScrollView>

      {/* Action Buttons */}
      <View style={styles.actionContainer}>
        {(status === 1 || status === 2) && (
          <View style={styles.actionRow}>
            <TouchableOpacity
              style={[styles.actionButton, styles.rejectButton]}
              onPress={handleReject}
              disabled={!!actionLoading}>
              {actionLoading === 'reject' ? (
                <ActivityIndicator color={Colors.textInverse} />
              ) : (
                <>
                  <Icon name="close" size={22} color={Colors.textInverse} />
                  <Text style={styles.actionButtonText}>Reddet</Text>
                </>
              )}
            </TouchableOpacity>
            <TouchableOpacity
              style={[styles.actionButton, styles.acceptButton]}
              onPress={handleAccept}
              disabled={!!actionLoading}>
              {actionLoading === 'accept' ? (
                <ActivityIndicator color={Colors.textInverse} />
              ) : (
                <>
                  <Icon name="check" size={22} color={Colors.textInverse} />
                  <Text style={styles.actionButtonText}>Kabul Et</Text>
                </>
              )}
            </TouchableOpacity>
          </View>
        )}

        {status === 3 && (
          <TouchableOpacity
            style={[styles.actionButtonFull, styles.pickedUpButton]}
            onPress={handlePickedUp}
            disabled={!!actionLoading}>
            {actionLoading === 'pickedUp' ? (
              <ActivityIndicator color={Colors.textInverse} />
            ) : (
              <>
                <Icon name="package-variant" size={22} color={Colors.textInverse} />
                <Text style={styles.actionButtonText}>Teslim Aldım</Text>
              </>
            )}
          </TouchableOpacity>
        )}

        {status === 5 && (
          <TouchableOpacity
            style={[styles.actionButtonFull, styles.deliveredButton]}
            onPress={handleDelivered}
            disabled={!!actionLoading}>
            {actionLoading === 'delivered' ? (
              <ActivityIndicator color={Colors.textInverse} />
            ) : (
              <>
                <Icon name="check-all" size={22} color={Colors.textInverse} />
                <Text style={styles.actionButtonText}>Teslim Ettim</Text>
              </>
            )}
          </TouchableOpacity>
        )}
      </View>
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
  emptyContainer: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: Colors.background,
    padding: Spacing.xl,
  },
  emptyText: {
    fontSize: Fonts.sizes.lg,
    color: Colors.textSecondary,
    marginTop: Spacing.base,
    marginBottom: Spacing.xl,
  },
  backButton: {
    backgroundColor: Colors.primary,
    paddingHorizontal: Spacing.xl,
    paddingVertical: Spacing.md,
    borderRadius: BorderRadius.md,
  },
  backButtonText: {
    color: Colors.textInverse,
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.semibold,
  },
  header: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingHorizontal: Spacing.base,
    paddingTop: Spacing.huge,
    paddingBottom: Spacing.base,
    backgroundColor: Colors.surface,
    borderBottomWidth: 1,
    borderBottomColor: Colors.borderLight,
  },
  headerBackButton: {
    padding: Spacing.sm,
    marginRight: Spacing.sm,
  },
  headerTitle: {
    flex: 1,
    fontSize: Fonts.sizes.lg,
    fontWeight: Fonts.weights.semibold,
    color: Colors.text,
  },
  headerStatusBadge: {
    paddingHorizontal: Spacing.md,
    paddingVertical: Spacing.xs,
    borderRadius: BorderRadius.round,
  },
  headerStatusText: {
    fontSize: Fonts.sizes.sm,
    fontWeight: Fonts.weights.semibold,
  },
  content: {
    flex: 1,
    paddingHorizontal: Spacing.base,
    paddingTop: Spacing.base,
  },
  infoCard: {
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
  infoCardHeader: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: Spacing.md,
    paddingBottom: Spacing.sm,
    borderBottomWidth: 1,
    borderBottomColor: Colors.borderLight,
  },
  infoCardTitle: {
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.semibold,
    color: Colors.text,
    marginLeft: Spacing.sm,
  },
  infoName: {
    fontSize: Fonts.sizes.lg,
    fontWeight: Fonts.weights.semibold,
    color: Colors.text,
    marginBottom: Spacing.xs,
  },
  infoAddress: {
    fontSize: Fonts.sizes.md,
    color: Colors.textSecondary,
    lineHeight: 20,
  },
  callButton: {
    flexDirection: 'row',
    alignItems: 'center',
    alignSelf: 'flex-start',
    backgroundColor: Colors.primary + '15',
    paddingHorizontal: Spacing.md,
    paddingVertical: Spacing.sm,
    borderRadius: BorderRadius.round,
    marginTop: Spacing.md,
  },
  callButtonText: {
    color: Colors.primary,
    fontSize: Fonts.sizes.md,
    fontWeight: Fonts.weights.semibold,
    marginLeft: Spacing.xs,
  },
  orderItemRow: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingVertical: Spacing.xs,
  },
  orderItemQuantity: {
    fontSize: Fonts.sizes.md,
    fontWeight: Fonts.weights.semibold,
    color: Colors.primary,
    width: 30,
  },
  orderItemName: {
    flex: 1,
    fontSize: Fonts.sizes.md,
    color: Colors.text,
  },
  orderItemPrice: {
    fontSize: Fonts.sizes.md,
    fontWeight: Fonts.weights.medium,
    color: Colors.text,
  },
  orderNoteContainer: {
    flexDirection: 'row',
    alignItems: 'flex-start',
    backgroundColor: Colors.warning + '15',
    borderRadius: BorderRadius.md,
    padding: Spacing.md,
    marginTop: Spacing.md,
  },
  orderNoteText: {
    flex: 1,
    fontSize: Fonts.sizes.md,
    color: Colors.text,
    marginLeft: Spacing.sm,
  },
  totalRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    borderTopWidth: 1,
    borderTopColor: Colors.borderLight,
    paddingTop: Spacing.md,
    marginTop: Spacing.md,
  },
  totalLabel: {
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.semibold,
    color: Colors.text,
  },
  totalValue: {
    fontSize: Fonts.sizes.lg,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
  },
  feeCard: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: Colors.success + '10',
    borderRadius: BorderRadius.xl,
    padding: Spacing.base,
    marginBottom: Spacing.md,
    borderWidth: 1,
    borderColor: Colors.success + '30',
  },
  feeInfo: {
    marginLeft: Spacing.md,
  },
  feeLabel: {
    fontSize: Fonts.sizes.md,
    color: Colors.textSecondary,
  },
  feeValue: {
    fontSize: Fonts.sizes.xl,
    fontWeight: Fonts.weights.bold,
    color: Colors.success,
  },
  actionContainer: {
    position: 'absolute',
    bottom: 0,
    left: 0,
    right: 0,
    backgroundColor: Colors.surface,
    padding: Spacing.base,
    paddingBottom: Spacing.xxl,
    borderTopWidth: 1,
    borderTopColor: Colors.borderLight,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: -4},
    shadowOpacity: 0.1,
    shadowRadius: 12,
    elevation: 10,
  },
  actionRow: {
    flexDirection: 'row',
    gap: Spacing.md,
  },
  actionButton: {
    flex: 1,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: Spacing.base,
    borderRadius: BorderRadius.md,
    gap: Spacing.sm,
  },
  actionButtonFull: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: Spacing.base,
    borderRadius: BorderRadius.md,
    gap: Spacing.sm,
  },
  acceptButton: {
    backgroundColor: Colors.success,
  },
  rejectButton: {
    backgroundColor: Colors.error,
  },
  pickedUpButton: {
    backgroundColor: Colors.primary,
  },
  deliveredButton: {
    backgroundColor: Colors.success,
  },
  actionButtonText: {
    color: Colors.textInverse,
    fontSize: Fonts.sizes.lg,
    fontWeight: Fonts.weights.semibold,
  },
});

export default ActiveDeliveryScreen;
