import React, {useState, useEffect, useCallback} from 'react';
import {
  View,
  Text,
  TextInput,
  StyleSheet,
  FlatList,
  TouchableOpacity,
  Alert,
  RefreshControl,
  ActivityIndicator,
} from 'react-native';
import MaterialCommunityIcons from 'react-native-vector-icons/MaterialCommunityIcons';
import {
  getCompanyMembers,
  searchCouriers,
  requestMembership,
  removeMember,
} from '../../api/courierService';

export default function MemberManagementScreen() {
  const [members, setMembers] = useState([]);
  const [searchQuery, setSearchQuery] = useState('');
  const [searchResults, setSearchResults] = useState([]);
  const [searching, setSearching] = useState(false);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [actionLoading, setActionLoading] = useState(null);

  const fetchMembers = useCallback(async () => {
    try {
      const result = await getCompanyMembers();
      if (!result.hasFailed && result.data) {
        setMembers(result.data);
      }
    } catch (error) {
      console.error('Members fetch error:', error);
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  }, []);

  useEffect(() => {
    fetchMembers();
  }, [fetchMembers]);

  const onRefresh = () => {
    setRefreshing(true);
    fetchMembers();
  };

  const handleSearch = async () => {
    if (!searchQuery.trim()) return;
    setSearching(true);
    try {
      const result = await searchCouriers(searchQuery.trim());
      if (!result.hasFailed && result.data) {
        setSearchResults(result.data);
      }
    } catch (error) {
      console.error('Search error:', error);
    } finally {
      setSearching(false);
    }
  };

  const handleRequest = courier => {
    Alert.alert(
      'Kurye Ekle',
      `"${courier.firstName} ${courier.lastName}" kuryesini firmaya eklemek istiyor musunuz?`,
      [
        {text: 'İptal', style: 'cancel'},
        {
          text: 'Ekle',
          onPress: async () => {
            setActionLoading(courier.userId);
            try {
              const result = await requestMembership(courier.userId);
              if (!result.hasFailed) {
                Alert.alert('Başarılı', 'Üyelik isteği gönderildi.');
                setSearchResults([]);
                setSearchQuery('');
              } else {
                Alert.alert(
                  'Hata',
                  result.messages?.map(m => m.description).join(', ') || 'İstek gönderilemedi.',
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

  const handleRemove = member => {
    Alert.alert(
      'Kurye Çıkar',
      `"${member.firstName} ${member.lastName}" kuryesini firmadan çıkarmak istiyor musunuz?`,
      [
        {text: 'İptal', style: 'cancel'},
        {
          text: 'Çıkar',
          style: 'destructive',
          onPress: async () => {
            setActionLoading(member.id);
            try {
              const result = await removeMember(member.id);
              if (!result.hasFailed) {
                fetchMembers();
              } else {
                Alert.alert(
                  'Hata',
                  result.messages?.map(m => m.description).join(', ') || 'Çıkarma başarısız.',
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

  const statusLabels = {
    0: {label: 'Onay Bekliyor', color: '#F9A825'},
    1: {label: 'Aktif', color: '#34C759'},
  };

  const renderMember = ({item}) => {
    const status = statusLabels[item.statusId] || statusLabels[0];
    return (
      <View style={styles.memberCard}>
        <View style={styles.memberInfo}>
          <View style={styles.avatar}>
            <Text style={styles.avatarText}>
              {(item.firstName?.[0] || '') + (item.lastName?.[0] || '')}
            </Text>
          </View>
          <View style={{flex: 1}}>
            <Text style={styles.memberName}>
              {item.firstName} {item.lastName}
            </Text>
            <View style={styles.statusRow}>
              <View style={[styles.statusDot, {backgroundColor: status.color}]} />
              <Text style={[styles.statusText, {color: status.color}]}>{status.label}</Text>
            </View>
          </View>
        </View>
        <TouchableOpacity
          style={styles.removeButton}
          onPress={() => handleRemove(item)}
          disabled={actionLoading === item.id}
          activeOpacity={0.7}>
          {actionLoading === item.id ? (
            <ActivityIndicator size="small" color="#FF3B30" />
          ) : (
            <MaterialCommunityIcons name="account-remove" size={20} color="#FF3B30" />
          )}
        </TouchableOpacity>
      </View>
    );
  };

  const renderSearchResult = ({item}) => (
    <View style={styles.searchResultCard}>
      <View style={{flex: 1}}>
        <Text style={styles.memberName}>
          {item.firstName} {item.lastName}
        </Text>
        <Text style={styles.searchEmail}>{item.email}</Text>
      </View>
      <TouchableOpacity
        style={styles.addButton}
        onPress={() => handleRequest(item)}
        disabled={actionLoading === item.userId}
        activeOpacity={0.7}>
        {actionLoading === item.userId ? (
          <ActivityIndicator size="small" color="#fff" />
        ) : (
          <>
            <MaterialCommunityIcons name="account-plus" size={16} color="#fff" />
            <Text style={styles.addButtonText}>Ekle</Text>
          </>
        )}
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

  return (
    <View style={styles.container}>
      <View style={styles.searchContainer}>
        <TextInput
          style={styles.searchInput}
          placeholder="Kurye ara (isim veya e-posta)"
          placeholderTextColor="#999"
          value={searchQuery}
          onChangeText={setSearchQuery}
          onSubmitEditing={handleSearch}
          returnKeyType="search"
        />
        <TouchableOpacity
          style={styles.searchButton}
          onPress={handleSearch}
          disabled={searching}>
          {searching ? (
            <ActivityIndicator size="small" color="#fff" />
          ) : (
            <MaterialCommunityIcons name="magnify" size={22} color="#fff" />
          )}
        </TouchableOpacity>
      </View>

      {searchResults.length > 0 && (
        <View style={styles.searchResultsContainer}>
          <Text style={styles.sectionHeader}>Arama Sonuçları</Text>
          <FlatList
            data={searchResults}
            renderItem={renderSearchResult}
            keyExtractor={item => item.userId}
            scrollEnabled={false}
          />
        </View>
      )}

      <Text style={styles.sectionHeader}>
        Firma Kuryeleri ({members.length})
      </Text>
      <FlatList
        data={members}
        renderItem={renderMember}
        keyExtractor={item => item.id}
        contentContainerStyle={styles.listContent}
        refreshControl={
          <RefreshControl refreshing={refreshing} onRefresh={onRefresh} colors={['#FF6B00']} />
        }
        ListEmptyComponent={
          <View style={styles.empty}>
            <MaterialCommunityIcons name="account-group-outline" size={48} color="#ccc" />
            <Text style={styles.emptyText}>Henüz kurye eklenmemiş</Text>
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
  searchContainer: {
    flexDirection: 'row',
    padding: 16,
    paddingBottom: 8,
    gap: 8,
  },
  searchInput: {
    flex: 1,
    backgroundColor: '#fff',
    borderWidth: 1,
    borderColor: '#ddd',
    borderRadius: 10,
    padding: 12,
    fontSize: 15,
    color: '#1a1a1a',
  },
  searchButton: {
    backgroundColor: '#FF6B00',
    width: 48,
    borderRadius: 10,
    justifyContent: 'center',
    alignItems: 'center',
  },
  searchResultsContainer: {
    paddingHorizontal: 16,
    marginBottom: 8,
  },
  searchResultCard: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#fff',
    padding: 12,
    borderRadius: 10,
    marginBottom: 6,
    borderLeftWidth: 3,
    borderLeftColor: '#FF6B00',
  },
  searchEmail: {
    fontSize: 12,
    color: '#999',
    marginTop: 2,
  },
  sectionHeader: {
    fontSize: 14,
    fontWeight: '700',
    color: '#666',
    paddingHorizontal: 16,
    paddingVertical: 8,
    textTransform: 'uppercase',
    letterSpacing: 0.5,
  },
  listContent: {
    padding: 16,
    paddingTop: 0,
    paddingBottom: 32,
  },
  memberCard: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    backgroundColor: '#fff',
    padding: 14,
    borderRadius: 12,
    marginBottom: 8,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 1},
    shadowOpacity: 0.04,
    shadowRadius: 3,
    elevation: 1,
  },
  memberInfo: {
    flexDirection: 'row',
    alignItems: 'center',
    flex: 1,
  },
  avatar: {
    width: 40,
    height: 40,
    borderRadius: 20,
    backgroundColor: '#FFF3E0',
    justifyContent: 'center',
    alignItems: 'center',
    marginRight: 12,
  },
  avatarText: {
    fontSize: 14,
    fontWeight: '700',
    color: '#FF6B00',
  },
  memberName: {
    fontSize: 15,
    fontWeight: '600',
    color: '#1a1a1a',
  },
  statusRow: {
    flexDirection: 'row',
    alignItems: 'center',
    marginTop: 4,
  },
  statusDot: {
    width: 6,
    height: 6,
    borderRadius: 3,
    marginRight: 5,
  },
  statusText: {
    fontSize: 12,
    fontWeight: '500',
  },
  removeButton: {
    width: 40,
    height: 40,
    borderRadius: 20,
    backgroundColor: '#FFF0F0',
    justifyContent: 'center',
    alignItems: 'center',
    marginLeft: 8,
  },
  addButton: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#FF6B00',
    paddingHorizontal: 14,
    paddingVertical: 8,
    borderRadius: 8,
    gap: 4,
    marginLeft: 8,
  },
  addButtonText: {
    color: '#fff',
    fontSize: 13,
    fontWeight: '600',
  },
  empty: {
    alignItems: 'center',
    paddingVertical: 60,
  },
  emptyText: {
    fontSize: 16,
    color: '#999',
    marginTop: 12,
  },
});
