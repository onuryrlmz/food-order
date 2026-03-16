import React, {useState, useEffect, useCallback} from 'react';
import {
  View,
  Text,
  SectionList,
  StyleSheet,
  TouchableOpacity,
  Alert,
  RefreshControl,
  ActivityIndicator,
} from 'react-native';
import MaterialCommunityIcons from 'react-native-vector-icons/MaterialCommunityIcons';
import {
  getMyRestaurants,
  getPendingInvites,
  acceptInvite,
  rejectInvite,
  leaveRestaurant,
} from '../../api/courierService';

export default function MyRestaurantsScreen() {
  const [restaurants, setRestaurants] = useState([]);
  const [invites, setInvites] = useState([]);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [actionLoading, setActionLoading] = useState(null);

  const fetchAll = useCallback(async () => {
    try {
      const [restResult, invResult] = await Promise.all([
        getMyRestaurants(),
        getPendingInvites(),
      ]);
      if (!restResult.hasFailed && restResult.data) {
        setRestaurants(restResult.data);
      }
      if (!invResult.hasFailed && invResult.data) {
        setInvites(invResult.data);
      }
    } catch (error) {
      console.error('Veri yüklenemedi:', error);
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  }, []);

  useEffect(() => {
    fetchAll();
  }, [fetchAll]);

  const onRefresh = () => {
    setRefreshing(true);
    fetchAll();
  };

  const handleAccept = item => {
    Alert.alert(
      'Daveti Kabul Et',
      `"${item.restaurantName}" restoranının davetini kabul etmek istiyor musunuz?`,
      [
        {text: 'İptal', style: 'cancel'},
        {
          text: 'Kabul Et',
          onPress: async () => {
            setActionLoading(item.restaurantCourierId);
            try {
              const result = await acceptInvite(item.restaurantCourierId);
              if (!result.hasFailed) {
                fetchAll();
              } else {
                Alert.alert(
                  'Hata',
                  result.messages?.map(m => m.description).join(', ') || 'Hata',
                );
              }
            } catch {
              Alert.alert('Hata', 'Bir hata oluştu.');
            } finally {
              setActionLoading(null);
            }
          },
        },
      ],
    );
  };

  const handleReject = item => {
    Alert.alert(
      'Daveti Reddet',
      `"${item.restaurantName}" restoranının davetini reddetmek istiyor musunuz?`,
      [
        {text: 'İptal', style: 'cancel'},
        {
          text: 'Reddet',
          style: 'destructive',
          onPress: async () => {
            setActionLoading(item.restaurantCourierId);
            try {
              const result = await rejectInvite(item.restaurantCourierId);
              if (!result.hasFailed) {
                setInvites(prev =>
                  prev.filter(
                    r => r.restaurantCourierId !== item.restaurantCourierId,
                  ),
                );
              } else {
                Alert.alert(
                  'Hata',
                  result.messages?.map(m => m.description).join(', ') || 'Hata',
                );
              }
            } catch {
              Alert.alert('Hata', 'Bir hata oluştu.');
            } finally {
              setActionLoading(null);
            }
          },
        },
      ],
    );
  };

  const handleLeave = item => {
    Alert.alert(
      'Anlaşmayı Sonlandır',
      `"${item.restaurantName}" ile anlaşmanızı sonlandırmak istiyor musunuz?`,
      [
        {text: 'İptal', style: 'cancel'},
        {
          text: 'Sonlandır',
          style: 'destructive',
          onPress: async () => {
            setActionLoading(item.restaurantCourierId);
            try {
              const result = await leaveRestaurant(item.restaurantCourierId);
              if (!result.hasFailed) {
                setRestaurants(prev =>
                  prev.filter(
                    r => r.restaurantCourierId !== item.restaurantCourierId,
                  ),
                );
              } else {
                Alert.alert(
                  'Hata',
                  result.messages?.map(m => m.description).join(', ') || 'Hata',
                );
              }
            } catch {
              Alert.alert('Hata', 'Bir hata oluştu.');
            } finally {
              setActionLoading(null);
            }
          },
        },
      ],
    );
  };

  const renderInviteItem = ({item}) => {
    const isLoading = actionLoading === item.restaurantCourierId;
    return (
      <View style={[styles.card, styles.inviteCard]}>
        <View style={styles.cardLeft}>
          <View style={[styles.iconContainer, {backgroundColor: '#FFF9C4'}]}>
            <MaterialCommunityIcons name="email-open" size={22} color="#F9A825" />
          </View>
          <View style={styles.cardInfo}>
            <Text style={styles.restaurantName}>{item.restaurantName}</Text>
            <Text style={styles.dateText}>
              Davet: {new Date(item.createdDate).toLocaleDateString('tr-TR')}
            </Text>
          </View>
        </View>
        <View style={styles.inviteActions}>
          <TouchableOpacity
            style={styles.acceptButton}
            onPress={() => handleAccept(item)}
            disabled={isLoading}
            activeOpacity={0.7}>
            {isLoading ? (
              <ActivityIndicator size="small" color="#fff" />
            ) : (
              <>
                <MaterialCommunityIcons name="check" size={16} color="#fff" />
                <Text style={styles.acceptText}>Kabul</Text>
              </>
            )}
          </TouchableOpacity>
          <TouchableOpacity
            style={styles.rejectButton}
            onPress={() => handleReject(item)}
            disabled={isLoading}
            activeOpacity={0.7}>
            <MaterialCommunityIcons name="close" size={16} color="#FF3B30" />
          </TouchableOpacity>
        </View>
      </View>
    );
  };

  const renderRestaurantItem = ({item}) => (
    <View style={styles.card}>
      <View style={styles.cardLeft}>
        <View style={styles.iconContainer}>
          <MaterialCommunityIcons name="store" size={24} color="#FF6B00" />
        </View>
        <View style={styles.cardInfo}>
          <Text style={styles.restaurantName}>{item.restaurantName}</Text>
          <View style={styles.statusRow}>
            <View style={[styles.statusDot, {backgroundColor: '#34C759'}]} />
            <Text style={styles.statusText}>Aktif</Text>
          </View>
          {item.agreementStartDate && (
            <Text style={styles.dateText}>
              Anlaşma:{' '}
              {new Date(item.agreementStartDate).toLocaleDateString('tr-TR')}
            </Text>
          )}
        </View>
      </View>
      <TouchableOpacity
        style={styles.leaveButton}
        onPress={() => handleLeave(item)}
        disabled={actionLoading === item.restaurantCourierId}
        activeOpacity={0.7}>
        <MaterialCommunityIcons name="logout" size={18} color="#FF3B30" />
        <Text style={styles.leaveText}>Ayrıl</Text>
      </TouchableOpacity>
    </View>
  );

  if (loading) {
    return (
      <View style={styles.center}>
        <ActivityIndicator size="large" color="#FF6B00" />
      </View>
    );
  }

  const sections = [];
  if (invites.length > 0) {
    sections.push({
      title: 'Gelen Davetler',
      data: invites,
      renderItem: renderInviteItem,
    });
  }
  sections.push({
    title: 'Restoranlarım',
    data: restaurants,
    renderItem: renderRestaurantItem,
  });

  return (
    <View style={styles.container}>
      <SectionList
        sections={sections}
        keyExtractor={item => item.restaurantCourierId}
        renderSectionHeader={({section}) => (
          <Text style={styles.sectionHeader}>{section.title}</Text>
        )}
        renderItem={({item, section}) => section.renderItem({item})}
        contentContainerStyle={styles.list}
        stickySectionHeadersEnabled={false}
        refreshControl={
          <RefreshControl
            refreshing={refreshing}
            onRefresh={onRefresh}
            colors={['#FF6B00']}
            tintColor="#FF6B00"
          />
        }
        ListEmptyComponent={
          <View style={styles.empty}>
            <MaterialCommunityIcons name="store-off" size={48} color="#ccc" />
            <Text style={styles.emptyText}>
              Henüz bir restorana bağlı değilsiniz.
            </Text>
            <Text style={styles.emptySubtext}>
              Restoran tarafından davet edildiğinizde burada görünecektir.
            </Text>
          </View>
        }
      />
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
    backgroundColor: '#f8f9fa',
  },
  list: {
    padding: 16,
    paddingBottom: 32,
  },
  sectionHeader: {
    fontSize: 14,
    fontWeight: '700',
    color: '#666',
    marginBottom: 8,
    marginTop: 8,
    textTransform: 'uppercase',
    letterSpacing: 0.5,
  },
  card: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    backgroundColor: '#fff',
    padding: 16,
    borderRadius: 12,
    marginBottom: 10,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 1},
    shadowOpacity: 0.06,
    shadowRadius: 4,
    elevation: 2,
  },
  inviteCard: {
    borderLeftWidth: 3,
    borderLeftColor: '#F9A825',
  },
  cardLeft: {
    flexDirection: 'row',
    alignItems: 'center',
    flex: 1,
  },
  iconContainer: {
    width: 44,
    height: 44,
    borderRadius: 22,
    backgroundColor: '#FFF3E0',
    justifyContent: 'center',
    alignItems: 'center',
    marginRight: 12,
  },
  cardInfo: {
    flex: 1,
  },
  restaurantName: {
    fontSize: 16,
    fontWeight: '600',
    color: '#1a1a1a',
    marginBottom: 4,
  },
  statusRow: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: 2,
  },
  statusDot: {
    width: 8,
    height: 8,
    borderRadius: 4,
    marginRight: 6,
  },
  statusText: {
    fontSize: 13,
    color: '#666',
  },
  dateText: {
    fontSize: 12,
    color: '#999',
  },
  inviteActions: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: 6,
    marginLeft: 8,
  },
  acceptButton: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#34C759',
    paddingHorizontal: 12,
    paddingVertical: 8,
    borderRadius: 8,
  },
  acceptText: {
    fontSize: 13,
    fontWeight: '600',
    color: '#fff',
    marginLeft: 4,
  },
  rejectButton: {
    alignItems: 'center',
    justifyContent: 'center',
    backgroundColor: '#FFF0F0',
    width: 34,
    height: 34,
    borderRadius: 8,
  },
  leaveButton: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#FFF0F0',
    paddingHorizontal: 12,
    paddingVertical: 8,
    borderRadius: 8,
    marginLeft: 8,
  },
  leaveText: {
    fontSize: 13,
    fontWeight: '600',
    color: '#FF3B30',
    marginLeft: 4,
  },
  empty: {
    alignItems: 'center',
    paddingTop: 80,
  },
  emptyText: {
    fontSize: 16,
    fontWeight: '600',
    color: '#999',
    marginTop: 16,
  },
  emptySubtext: {
    fontSize: 13,
    color: '#bbb',
    marginTop: 8,
    textAlign: 'center',
    paddingHorizontal: 40,
  },
});
