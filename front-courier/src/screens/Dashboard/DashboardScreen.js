import React, {useState, useEffect, useCallback} from 'react';
import {
  View,
  Text,
  StyleSheet,
  TouchableOpacity,
  RefreshControl,
  ScrollView,
  ActivityIndicator,
  Alert,
} from 'react-native';
import {useFocusEffect, useNavigation} from '@react-navigation/native';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import {Colors, Fonts, Spacing, BorderRadius} from '../../theme';
import {AVAILABILITY_STATUS, DELIVERY_STATUS} from '../../utils/constants';
import {useAuth} from '../../context/AuthContext';
import {useLocation} from '../../context/LocationContext';
import {courierService} from '../../api/courierService';

const DashboardScreen = () => {
  const navigation = useNavigation();
  const {user, refreshProfile} = useAuth();
  const {startTracking, stopTracking} = useLocation();

  const [isOnline, setIsOnline] = useState(false);
  const [activeDelivery, setActiveDelivery] = useState(null);
  const [todayStats, setTodayStats] = useState({deliveries: 0, earnings: 0});
  const [loading, setLoading] = useState(false);
  const [refreshing, setRefreshing] = useState(false);
  const [toggling, setToggling] = useState(false);

  const availabilityStatus = activeDelivery
    ? AVAILABILITY_STATUS[2]
    : isOnline
    ? AVAILABILITY_STATUS[1]
    : AVAILABILITY_STATUS[0];

  const fetchData = async () => {
    try {
      const [assignmentRes, earningRes] = await Promise.all([
        courierService.getActiveAssignment(),
        courierService.getEarningSummary(),
      ]);

      if (!assignmentRes.data.hasFailed && assignmentRes.data.data) {
        setActiveDelivery(assignmentRes.data.data);
      } else {
        setActiveDelivery(null);
      }

      if (!earningRes.data.hasFailed && earningRes.data.data) {
        setTodayStats({
          deliveries: earningRes.data.data.todayDeliveries || 0,
          earnings: earningRes.data.data.todayEarnings || 0,
        });
      }
    } catch (error) {
      // Silent fail
    }
  };

  // Ekran her odaklandığında backend'den güncel durumu al
  useFocusEffect(
    useCallback(() => {
      const syncStatus = async () => {
        // Profili yenile — backend'den güncel availabilityStatus gelsin
        await refreshProfile();
        await fetchData();
      };
      syncStatus();
    }, []),
  );

  // user değiştiğinde (ilk yükleme veya refreshProfile sonrası) durumu senkronize et
  useEffect(() => {
    if (!user) return;

    const isBackendOnline = user.availabilityStatus === 1 || user.availabilityStatus === 2;
    setIsOnline(isBackendOnline);

    // Backend çevrimiçi diyor — konum takibini başlat
    if (isBackendOnline) {
      startTracking().catch(() => {});
    }
  }, [user?.availabilityStatus]);

  const handleToggleOnline = async () => {
    setToggling(true);
    try {
      if (isOnline) {
        await courierService.goOffline();
        stopTracking();
        setIsOnline(false);
      } else {
        const trackingStarted = await startTracking();
        if (trackingStarted) {
          await courierService.goOnline();
          setIsOnline(true);
        }
      }
      await refreshProfile();
    } catch (error) {
      Alert.alert('Hata', error?.message || 'Bir hata oluştu. Lütfen tekrar deneyin.');
    } finally {
      setToggling(false);
    }
  };

  const onRefresh = async () => {
    setRefreshing(true);
    await fetchData();
    setRefreshing(false);
  };

  const handleDeliveryPress = () => {
    if (activeDelivery) {
      navigation.navigate('ActiveDelivery', {assignment: activeDelivery});
    }
  };

  return (
    <View style={styles.container}>
      {/* Header */}
      <View style={styles.header}>
        <View>
          <Text style={styles.greeting}>Merhaba,</Text>
          <Text style={styles.userName}>{user?.fullName || 'Kurye'}</Text>
        </View>
        <View style={[styles.statusBadge, {backgroundColor: availabilityStatus.color + '20'}]}>
          <View style={[styles.statusDot, {backgroundColor: availabilityStatus.color}]} />
          <Text style={[styles.statusText, {color: availabilityStatus.color}]}>
            {availabilityStatus.label}
          </Text>
        </View>
      </View>

      <ScrollView
        style={styles.content}
        refreshControl={
          <RefreshControl refreshing={refreshing} onRefresh={onRefresh} />
        }>
        {/* Toggle Button */}
        <TouchableOpacity
          style={[
            styles.toggleButton,
            isOnline ? styles.toggleButtonOnline : styles.toggleButtonOffline,
          ]}
          onPress={handleToggleOnline}
          disabled={toggling}
          activeOpacity={0.8}>
          {toggling ? (
            <ActivityIndicator color={Colors.textInverse} size="large" />
          ) : (
            <>
              <Icon
                name={isOnline ? 'power' : 'power-off'}
                size={48}
                color={Colors.textInverse}
              />
              <Text style={styles.toggleText}>
                {isOnline ? 'Çevrimiçi' : 'Çevrimdışı'}
              </Text>
              <Text style={styles.toggleSubtext}>
                {isOnline ? 'Dokunarak çevrimdışı olun' : 'Dokunarak çevrimiçi olun'}
              </Text>
            </>
          )}
        </TouchableOpacity>

        {/* Active Delivery Card */}
        {activeDelivery ? (
          <TouchableOpacity style={styles.deliveryCard} onPress={handleDeliveryPress}>
            <View style={styles.deliveryCardHeader}>
              <Icon name="motorbike" size={24} color={Colors.primary} />
              <Text style={styles.deliveryCardTitle}>Aktif Teslimat</Text>
              <View
                style={[
                  styles.deliveryStatusBadge,
                  {backgroundColor: (DELIVERY_STATUS[activeDelivery.status]?.color || Colors.textSecondary) + '20'},
                ]}>
                <Text
                  style={[
                    styles.deliveryStatusText,
                    {color: DELIVERY_STATUS[activeDelivery.status]?.color || Colors.textSecondary},
                  ]}>
                  {DELIVERY_STATUS[activeDelivery.status]?.label || 'Bilinmiyor'}
                </Text>
              </View>
            </View>
            <View style={styles.deliveryCardBody}>
              <View style={styles.deliveryInfoRow}>
                <Icon name="store" size={18} color={Colors.textSecondary} />
                <Text style={styles.deliveryInfoText} numberOfLines={1}>
                  {activeDelivery.restaurantName || 'Restoran'}
                </Text>
              </View>
              <View style={styles.deliveryInfoRow}>
                <Icon name="account" size={18} color={Colors.textSecondary} />
                <Text style={styles.deliveryInfoText} numberOfLines={1}>
                  {activeDelivery.customerName || 'Müşteri'}
                </Text>
              </View>
              <View style={styles.deliveryInfoRow}>
                <Icon name="cash" size={18} color={Colors.success} />
                <Text style={[styles.deliveryInfoText, {color: Colors.success, fontWeight: Fonts.weights.semibold}]}>
                  {activeDelivery.deliveryFee?.toFixed(2) || '0.00'} TL
                </Text>
              </View>
            </View>
            <View style={styles.deliveryCardFooter}>
              <Text style={styles.viewDetailsText}>Detayları Gör</Text>
              <Icon name="chevron-right" size={20} color={Colors.primary} />
            </View>
          </TouchableOpacity>
        ) : isOnline ? (
          <View style={styles.waitingCard}>
            <Icon name="clock-outline" size={40} color={Colors.textTertiary} />
            <Text style={styles.waitingText}>Yeni teslimat bekleniyor...</Text>
            <Text style={styles.waitingSubtext}>
              Yeni bir teslimat atandığında bildirim alacaksınız
            </Text>
          </View>
        ) : null}

        {/* Today's Stats */}
        <View style={styles.statsContainer}>
          <Text style={styles.statsTitle}>Bugünkü Özet</Text>
          <View style={styles.statsRow}>
            <View style={styles.statCard}>
              <Icon name="package-variant-closed" size={28} color={Colors.primary} />
              <Text style={styles.statValue}>{todayStats.deliveries}</Text>
              <Text style={styles.statLabel}>Teslimat</Text>
            </View>
            <View style={styles.statCard}>
              <Icon name="cash-multiple" size={28} color={Colors.success} />
              <Text style={styles.statValue}>{todayStats.earnings.toFixed(2)} TL</Text>
              <Text style={styles.statLabel}>Kazanç</Text>
            </View>
          </View>
        </View>
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
    justifyContent: 'space-between',
    alignItems: 'center',
    paddingHorizontal: Spacing.xl,
    paddingTop: Spacing.huge,
    paddingBottom: Spacing.base,
    backgroundColor: Colors.surface,
    borderBottomWidth: 1,
    borderBottomColor: Colors.borderLight,
  },
  greeting: {
    fontSize: Fonts.sizes.md,
    color: Colors.textSecondary,
  },
  userName: {
    fontSize: Fonts.sizes.xl,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
  },
  statusBadge: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingHorizontal: Spacing.md,
    paddingVertical: Spacing.xs,
    borderRadius: BorderRadius.round,
  },
  statusDot: {
    width: 8,
    height: 8,
    borderRadius: 4,
    marginRight: Spacing.xs,
  },
  statusText: {
    fontSize: Fonts.sizes.sm,
    fontWeight: Fonts.weights.semibold,
  },
  content: {
    flex: 1,
    paddingHorizontal: Spacing.base,
    paddingTop: Spacing.base,
  },
  toggleButton: {
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: Spacing.xxxl,
    borderRadius: BorderRadius.xl,
    marginBottom: Spacing.base,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 4},
    shadowOpacity: 0.15,
    shadowRadius: 12,
    elevation: 8,
  },
  toggleButtonOnline: {
    backgroundColor: Colors.success,
  },
  toggleButtonOffline: {
    backgroundColor: Colors.offline,
  },
  toggleText: {
    fontSize: Fonts.sizes.xxl,
    fontWeight: Fonts.weights.bold,
    color: Colors.textInverse,
    marginTop: Spacing.md,
  },
  toggleSubtext: {
    fontSize: Fonts.sizes.md,
    color: 'rgba(255,255,255,0.8)',
    marginTop: Spacing.xs,
  },
  deliveryCard: {
    backgroundColor: Colors.surface,
    borderRadius: BorderRadius.xl,
    padding: Spacing.base,
    marginBottom: Spacing.base,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 2},
    shadowOpacity: 0.06,
    shadowRadius: 8,
    elevation: 4,
  },
  deliveryCardHeader: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: Spacing.md,
  },
  deliveryCardTitle: {
    flex: 1,
    fontSize: Fonts.sizes.lg,
    fontWeight: Fonts.weights.semibold,
    color: Colors.text,
    marginLeft: Spacing.sm,
  },
  deliveryStatusBadge: {
    paddingHorizontal: Spacing.md,
    paddingVertical: Spacing.xs,
    borderRadius: BorderRadius.round,
  },
  deliveryStatusText: {
    fontSize: Fonts.sizes.sm,
    fontWeight: Fonts.weights.semibold,
  },
  deliveryCardBody: {
    borderTopWidth: 1,
    borderTopColor: Colors.borderLight,
    paddingTop: Spacing.md,
  },
  deliveryInfoRow: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: Spacing.sm,
  },
  deliveryInfoText: {
    fontSize: Fonts.sizes.md,
    color: Colors.text,
    marginLeft: Spacing.sm,
    flex: 1,
  },
  deliveryCardFooter: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'flex-end',
    borderTopWidth: 1,
    borderTopColor: Colors.borderLight,
    paddingTop: Spacing.md,
    marginTop: Spacing.sm,
  },
  viewDetailsText: {
    fontSize: Fonts.sizes.md,
    color: Colors.primary,
    fontWeight: Fonts.weights.semibold,
  },
  waitingCard: {
    backgroundColor: Colors.surface,
    borderRadius: BorderRadius.xl,
    padding: Spacing.xxl,
    marginBottom: Spacing.base,
    alignItems: 'center',
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 2},
    shadowOpacity: 0.06,
    shadowRadius: 8,
    elevation: 4,
  },
  waitingText: {
    fontSize: Fonts.sizes.lg,
    fontWeight: Fonts.weights.semibold,
    color: Colors.textSecondary,
    marginTop: Spacing.md,
  },
  waitingSubtext: {
    fontSize: Fonts.sizes.md,
    color: Colors.textTertiary,
    marginTop: Spacing.xs,
    textAlign: 'center',
  },
  statsContainer: {
    marginBottom: Spacing.xxl,
  },
  statsTitle: {
    fontSize: Fonts.sizes.lg,
    fontWeight: Fonts.weights.semibold,
    color: Colors.text,
    marginBottom: Spacing.md,
  },
  statsRow: {
    flexDirection: 'row',
    gap: Spacing.md,
  },
  statCard: {
    flex: 1,
    backgroundColor: Colors.surface,
    borderRadius: BorderRadius.xl,
    padding: Spacing.base,
    alignItems: 'center',
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 2},
    shadowOpacity: 0.06,
    shadowRadius: 8,
    elevation: 4,
  },
  statValue: {
    fontSize: Fonts.sizes.xl,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
    marginTop: Spacing.sm,
  },
  statLabel: {
    fontSize: Fonts.sizes.sm,
    color: Colors.textSecondary,
    marginTop: Spacing.xs,
  },
});

export default DashboardScreen;
