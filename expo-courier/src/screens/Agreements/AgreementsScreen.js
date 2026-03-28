import React, {useState, useCallback} from 'react';
import {
  View,
  Text,
  StyleSheet,
  FlatList,
  ActivityIndicator,
  RefreshControl,
  TouchableOpacity,
  Alert,
} from 'react-native';
import { useFocusEffect, router } from 'expo-router';
import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import {Colors, Fonts, Spacing, BorderRadius} from '../../theme';
import {courierService} from '../../api/courierService';

const STATUS_CONFIG = {
  1: {label: 'Onay Bekliyor', color: Colors.warning, icon: 'clock-outline'},
  2: {label: 'Aktif', color: Colors.success, icon: 'check-circle-outline'},
  3: {label: 'Askıya Alındı', color: '#FFCC00', icon: 'pause-circle-outline'},
  4: {label: 'Sonlandırıldı', color: Colors.textSecondary, icon: 'close-circle-outline'},
};

const AgreementsScreen = () => {
  const [agreements, setAgreements] = useState([]);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [actionLoading, setActionLoading] = useState(null);

  const fetchAgreements = async () => {
    try {
      const response = await courierService.getMyAgreements();
      if (!response.data.hasFailed) {
        const items = response.data.data || response.data.rawData || [];
        setAgreements(Array.isArray(items) ? items : []);
      }
    } catch (error) {
      // Silent fail
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  };

  useFocusEffect(
    useCallback(() => {
      fetchAgreements();
    }, []),
  );

  const onRefresh = () => {
    setRefreshing(true);
    fetchAgreements();
  };

  const handleAccept = (agreementId) => {
    Alert.alert(
      'Anlasmayı Kabul Et',
      'Bu anlasmayi kabul etmek istediginize emin misiniz?',
      [
        {text: 'Iptal', style: 'cancel'},
        {
          text: 'Kabul Et',
          onPress: async () => {
            setActionLoading(agreementId);
            try {
              const response = await courierService.acceptAgreement(agreementId);
              if (!response.data.hasFailed) {
                fetchAgreements();
              } else {
                Alert.alert('Hata', response.data.messages?.[0]?.description || 'Islem basarisiz');
              }
            } catch (error) {
              Alert.alert('Hata', 'Baglanti hatasi olustu');
            } finally {
              setActionLoading(null);
            }
          },
        },
      ],
    );
  };

  const handleReject = (agreementId) => {
    Alert.alert(
      'Anlasmayı Reddet',
      'Bu anlasmayi reddetmek istediginize emin misiniz?',
      [
        {text: 'Iptal', style: 'cancel'},
        {
          text: 'Reddet',
          style: 'destructive',
          onPress: async () => {
            setActionLoading(agreementId);
            try {
              const response = await courierService.rejectAgreement(agreementId);
              if (!response.data.hasFailed) {
                fetchAgreements();
              } else {
                Alert.alert('Hata', response.data.messages?.[0]?.description || 'Islem basarisiz');
              }
            } catch (error) {
              Alert.alert('Hata', 'Baglanti hatasi olustu');
            } finally {
              setActionLoading(null);
            }
          },
        },
      ],
    );
  };

  const handleTerminate = (agreementId) => {
    Alert.alert(
      'Anlasmayı Sonlandır',
      'Bu anlasmayi sonlandirmak istediginize emin misiniz? Bu islem geri alinamaz.',
      [
        {text: 'Iptal', style: 'cancel'},
        {
          text: 'Sonlandir',
          style: 'destructive',
          onPress: async () => {
            setActionLoading(agreementId);
            try {
              const response = await courierService.terminateAgreement(agreementId);
              if (!response.data.hasFailed) {
                fetchAgreements();
              } else {
                Alert.alert('Hata', response.data.messages?.[0]?.description || 'Islem basarisiz');
              }
            } catch (error) {
              Alert.alert('Hata', 'Baglanti hatasi olustu');
            } finally {
              setActionLoading(null);
            }
          },
        },
      ],
    );
  };

  const formatDate = (dateStr) => {
    if (!dateStr) return '-';
    const date = new Date(dateStr);
    return date.toLocaleDateString('tr-TR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
    });
  };

  const renderItem = ({item}) => {
    const status = STATUS_CONFIG[item.statusId] || STATUS_CONFIG[4];
    const isPending = item.statusId === 1;
    const isActive = item.statusId === 2;
    const isLoading = actionLoading === item.id;

    return (
      <View style={styles.card}>
        <View style={styles.cardHeader}>
          <View style={styles.restaurantInfo}>
            <Icon name="store" size={20} color={Colors.primary} />
            <Text style={styles.restaurantName} numberOfLines={1}>
              {item.restaurantName || 'Restoran'}
            </Text>
          </View>
          <View style={[styles.statusBadge, {backgroundColor: status.color + '20'}]}>
            <Icon name={status.icon} size={14} color={status.color} />
            <Text style={[styles.statusText, {color: status.color}]}>{status.label}</Text>
          </View>
        </View>

        <View style={styles.cardBody}>
          <View style={styles.detailRow}>
            <Icon name="cash" size={16} color={Colors.textSecondary} />
            <Text style={styles.detailLabel}>Teslimat Ucreti:</Text>
            <Text style={styles.detailValue}>
              {item.agreedDeliveryFee != null ? `${item.agreedDeliveryFee.toFixed(2)} TL` : '-'}
            </Text>
          </View>
          {item.perKmFee != null && (
            <View style={styles.detailRow}>
              <Icon name="map-marker-distance" size={16} color={Colors.textSecondary} />
              <Text style={styles.detailLabel}>Km Basina:</Text>
              <Text style={styles.detailValue}>{item.perKmFee.toFixed(2)} TL</Text>
            </View>
          )}
          <View style={styles.detailRow}>
            <Icon name="calendar" size={16} color={Colors.textSecondary} />
            <Text style={styles.detailLabel}>Tarih:</Text>
            <Text style={styles.detailValue}>{formatDate(item.createdDate)}</Text>
          </View>
          {item.effectiveUntil && (
            <View style={styles.detailRow}>
              <Icon name="calendar-end" size={16} color={Colors.textSecondary} />
              <Text style={styles.detailLabel}>Bitis:</Text>
              <Text style={styles.detailValue}>{formatDate(item.effectiveUntil)}</Text>
            </View>
          )}
        </View>

        {(isPending || isActive) && (
          <View style={styles.cardActions}>
            {isPending && (
              <>
                <TouchableOpacity
                  style={[styles.actionButton, styles.acceptButton]}
                  onPress={() => handleAccept(item.id)}
                  disabled={isLoading}>
                  {isLoading ? (
                    <ActivityIndicator size="small" color={Colors.textInverse} />
                  ) : (
                    <>
                      <Icon name="check" size={16} color={Colors.textInverse} />
                      <Text style={styles.acceptButtonText}>Kabul Et</Text>
                    </>
                  )}
                </TouchableOpacity>
                <TouchableOpacity
                  style={[styles.actionButton, styles.rejectButton]}
                  onPress={() => handleReject(item.id)}
                  disabled={isLoading}>
                  <Icon name="close" size={16} color={Colors.error} />
                  <Text style={styles.rejectButtonText}>Reddet</Text>
                </TouchableOpacity>
              </>
            )}
            {isActive && (
              <TouchableOpacity
                style={[styles.actionButton, styles.terminateButton]}
                onPress={() => handleTerminate(item.id)}
                disabled={isLoading}>
                {isLoading ? (
                  <ActivityIndicator size="small" color={Colors.error} />
                ) : (
                  <>
                    <Icon name="stop-circle-outline" size={16} color={Colors.error} />
                    <Text style={styles.terminateButtonText}>Sonlandir</Text>
                  </>
                )}
              </TouchableOpacity>
            )}
          </View>
        )}
      </View>
    );
  };

  const renderEmpty = () => {
    if (loading) return null;
    return (
      <View style={styles.emptyContainer}>
        <Icon name="handshake-outline" size={60} color={Colors.textTertiary} />
        <Text style={styles.emptyText}>Henuz anlasma bulunmuyor</Text>
        <Text style={styles.emptySubText}>
          Restoranlar size anlasma teklifi gonderdiginde burada gorunecektir.
        </Text>
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
        <TouchableOpacity onPress={() => router.back()} style={styles.backButton}>
          <Icon name="arrow-left" size={24} color={Colors.text} />
        </TouchableOpacity>
        <Text style={styles.headerTitle}>Anlasmalarim</Text>
        <View style={styles.headerRight} />
      </View>
      <FlatList
        data={agreements}
        renderItem={renderItem}
        keyExtractor={(item, index) => item.id?.toString() || index.toString()}
        contentContainerStyle={styles.listContent}
        refreshControl={
          <RefreshControl refreshing={refreshing} onRefresh={onRefresh} />
        }
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
    flexDirection: 'row',
    alignItems: 'center',
    paddingHorizontal: Spacing.base,
    paddingTop: Spacing.huge,
    paddingBottom: Spacing.base,
    backgroundColor: Colors.surface,
    borderBottomWidth: 1,
    borderBottomColor: Colors.borderLight,
  },
  backButton: {
    padding: Spacing.xs,
  },
  headerTitle: {
    flex: 1,
    fontSize: Fonts.sizes.xl,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
    textAlign: 'center',
  },
  headerRight: {
    width: 32,
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
    paddingBottom: Spacing.sm,
    borderBottomWidth: 1,
    borderBottomColor: Colors.borderLight,
  },
  restaurantInfo: {
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
    flexDirection: 'row',
    alignItems: 'center',
    paddingHorizontal: Spacing.sm,
    paddingVertical: Spacing.xs,
    borderRadius: BorderRadius.md,
  },
  statusText: {
    fontSize: Fonts.sizes.xs,
    fontWeight: Fonts.weights.semibold,
    marginLeft: 4,
  },
  cardBody: {
    gap: Spacing.sm,
  },
  detailRow: {
    flexDirection: 'row',
    alignItems: 'center',
  },
  detailLabel: {
    fontSize: Fonts.sizes.sm,
    color: Colors.textSecondary,
    marginLeft: Spacing.xs,
    marginRight: Spacing.xs,
  },
  detailValue: {
    fontSize: Fonts.sizes.sm,
    fontWeight: Fonts.weights.medium,
    color: Colors.text,
  },
  cardActions: {
    flexDirection: 'row',
    gap: Spacing.sm,
    marginTop: Spacing.md,
    paddingTop: Spacing.md,
    borderTopWidth: 1,
    borderTopColor: Colors.borderLight,
  },
  actionButton: {
    flex: 1,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: Spacing.sm,
    borderRadius: BorderRadius.md,
    gap: Spacing.xs,
  },
  acceptButton: {
    backgroundColor: Colors.success,
  },
  acceptButtonText: {
    color: Colors.textInverse,
    fontSize: Fonts.sizes.sm,
    fontWeight: Fonts.weights.semibold,
  },
  rejectButton: {
    backgroundColor: Colors.surface,
    borderWidth: 1,
    borderColor: Colors.error,
  },
  rejectButtonText: {
    color: Colors.error,
    fontSize: Fonts.sizes.sm,
    fontWeight: Fonts.weights.semibold,
  },
  terminateButton: {
    backgroundColor: Colors.surface,
    borderWidth: 1,
    borderColor: Colors.error,
  },
  terminateButtonText: {
    color: Colors.error,
    fontSize: Fonts.sizes.sm,
    fontWeight: Fonts.weights.semibold,
  },
  emptyContainer: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    paddingTop: Spacing.massive,
    paddingHorizontal: Spacing.xl,
  },
  emptyText: {
    fontSize: Fonts.sizes.lg,
    color: Colors.textSecondary,
    marginTop: Spacing.base,
    fontWeight: Fonts.weights.semibold,
  },
  emptySubText: {
    fontSize: Fonts.sizes.sm,
    color: Colors.textTertiary,
    marginTop: Spacing.sm,
    textAlign: 'center',
  },
});

export default AgreementsScreen;
