import React, {useState, useEffect, useCallback} from 'react';
import {
  View,
  Text,
  StyleSheet,
  FlatList,
  TouchableOpacity,
  Alert,
  RefreshControl,
  ActivityIndicator,
} from 'react-native';
import MaterialCommunityIcons from 'react-native-vector-icons/MaterialCommunityIcons';
import {
  getRestaurantInvites,
  acceptRestaurantInvite,
  rejectRestaurantInvite,
} from '../../api/courierService';

export default function RestaurantInvitesScreen() {
  const [invites, setInvites] = useState([]);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [actionLoading, setActionLoading] = useState(null);

  const fetchInvites = useCallback(async () => {
    try {
      const result = await getRestaurantInvites();
      if (!result.hasFailed && result.data) {
        setInvites(result.data);
      }
    } catch (error) {
      console.error('Invites fetch error:', error);
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  }, []);

  useEffect(() => {
    fetchInvites();
  }, [fetchInvites]);

  const onRefresh = () => {
    setRefreshing(true);
    fetchInvites();
  };

  const handleAccept = invite => {
    Alert.alert(
      'Daveti Kabul Et',
      `"${invite.restaurantName}" restoranının davetini kabul etmek istiyor musunuz?`,
      [
        {text: 'İptal', style: 'cancel'},
        {
          text: 'Kabul Et',
          onPress: async () => {
            setActionLoading(invite.id);
            try {
              const result = await acceptRestaurantInvite(invite.id);
              if (!result.hasFailed) {
                fetchInvites();
              } else {
                Alert.alert(
                  'Hata',
                  result.messages?.map(m => m.description).join(', ') || 'Kabul edilemedi.',
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

  const handleReject = invite => {
    Alert.alert(
      'Daveti Reddet',
      `"${invite.restaurantName}" restoranının davetini reddetmek istiyor musunuz?`,
      [
        {text: 'İptal', style: 'cancel'},
        {
          text: 'Reddet',
          style: 'destructive',
          onPress: async () => {
            setActionLoading(invite.id);
            try {
              const result = await rejectRestaurantInvite(invite.id);
              if (!result.hasFailed) {
                setInvites(prev => prev.filter(i => i.id !== invite.id));
              } else {
                Alert.alert(
                  'Hata',
                  result.messages?.map(m => m.description).join(', ') || 'Reddedilemedi.',
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

  const renderInvite = ({item}) => {
    const isLoading = actionLoading === item.id;
    return (
      <View style={styles.card}>
        <View style={styles.cardLeft}>
          <View style={styles.iconContainer}>
            <MaterialCommunityIcons name="store" size={24} color="#FF6B00" />
          </View>
          <View style={{flex: 1}}>
            <Text style={styles.restaurantName}>{item.restaurantName}</Text>
            <Text style={styles.dateText}>
              Davet: {new Date(item.createdDate).toLocaleDateString('tr-TR')}
            </Text>
          </View>
        </View>
        <View style={styles.actions}>
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

  if (loading) {
    return (
      <View style={styles.center}>
        <ActivityIndicator size="large" color="#FF6B00" />
      </View>
    );
  }

  return (
    <View style={styles.container}>
      <FlatList
        data={invites}
        renderItem={renderInvite}
        keyExtractor={item => item.id}
        contentContainerStyle={styles.listContent}
        refreshControl={
          <RefreshControl refreshing={refreshing} onRefresh={onRefresh} colors={['#FF6B00']} />
        }
        ListEmptyComponent={
          <View style={styles.empty}>
            <MaterialCommunityIcons name="email-open-outline" size={48} color="#ccc" />
            <Text style={styles.emptyText}>Restoran daveti bulunmuyor</Text>
            <Text style={styles.emptySubtext}>
              Restoranlar firmanıza davet gönderdiğinde burada görünecektir.
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
  listContent: {
    padding: 16,
    paddingBottom: 32,
  },
  card: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    backgroundColor: '#fff',
    padding: 16,
    borderRadius: 12,
    marginBottom: 10,
    borderLeftWidth: 3,
    borderLeftColor: '#F9A825',
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 1},
    shadowOpacity: 0.06,
    shadowRadius: 4,
    elevation: 2,
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
  restaurantName: {
    fontSize: 16,
    fontWeight: '600',
    color: '#1a1a1a',
    marginBottom: 4,
  },
  dateText: {
    fontSize: 12,
    color: '#999',
  },
  actions: {
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
  empty: {
    alignItems: 'center',
    paddingVertical: 60,
  },
  emptyText: {
    fontSize: 16,
    fontWeight: '600',
    color: '#999',
    marginTop: 12,
  },
  emptySubtext: {
    fontSize: 13,
    color: '#bbb',
    marginTop: 6,
    textAlign: 'center',
    paddingHorizontal: 40,
  },
});
