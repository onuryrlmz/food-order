import React, {useState, useEffect, useCallback} from 'react';
import {
  View,
  Text,
  StyleSheet,
  FlatList,
  RefreshControl,
  TouchableOpacity,
  StatusBar,
} from 'react-native';
import {useSafeAreaInsets} from 'react-native-safe-area-context';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import {Colors, Fonts, Spacing} from '../../theme';
import {useAuth} from '../../context/AuthContext';
import {useCart} from '../../context/CartContext';
import {useAppData} from '../../context/AppDataContext';
import RestaurantCard from '../../components/RestaurantCard';
import SearchBar from '../../components/SearchBar';
import CuisineFilter from '../../components/CuisineFilter';
import EmptyState from '../../components/EmptyState';

const HomeScreen = ({navigation}) => {
  const {user, isAuthenticated} = useAuth();
  const {itemCount} = useCart();
  const {cuisines, restaurants, defaultAddress, refreshRestaurants, refreshAddress, selectRestaurant} = useAppData();
  const insets = useSafeAreaInsets();
  const [filteredRestaurants, setFilteredRestaurants] = useState([]);
  const [selectedCuisine, setSelectedCuisine] = useState(null);
  const [searchQuery, setSearchQuery] = useState('');
  const [refreshing, setRefreshing] = useState(false);

  useEffect(() => {
    const unsubscribe = navigation.addListener('focus', () => {
      if (isAuthenticated) refreshAddress();
    });
    return unsubscribe;
  }, [navigation, isAuthenticated]);

  useEffect(() => {
    filterRestaurants();
  }, [searchQuery, selectedCuisine, restaurants]);

  const onRefresh = useCallback(async () => {
    setRefreshing(true);
    await refreshRestaurants();
    setRefreshing(false);
  }, [refreshRestaurants]);

  const filterRestaurants = () => {
    let filtered = [...restaurants];
    if (searchQuery.trim()) {
      const q = searchQuery.toLowerCase();
      filtered = filtered.filter(
        r =>
          r.name?.toLowerCase().includes(q) ||
          r.description?.toLowerCase().includes(q) ||
          r.categories?.some(c => c.toLowerCase().includes(q)),
      );
    }
    if (selectedCuisine) {
      const cuisine = cuisines.find(c => c.id === selectedCuisine);
      if (cuisine) {
        filtered = filtered.filter(r =>
          r.categories?.some(c => c.toLowerCase().includes(cuisine.name.toLowerCase())),
        );
      }
    }
    setFilteredRestaurants(filtered);
  };

  return (
    <View style={[styles.container, {paddingTop: insets.top}]}>
      <StatusBar barStyle="dark-content" backgroundColor={Colors.background} />
      <View style={styles.topBar}>
        <TouchableOpacity style={styles.locationButton} onPress={() => navigation.navigate('AddressList')}>
          <Icon name="map-marker" size={22} color={Colors.primary} />
          <View style={styles.locationTextContainer}>
            <Text style={styles.locationLabel}>Teslimat Adresi</Text>
            <Text style={styles.locationAddress} numberOfLines={1}>
              {defaultAddress ? defaultAddress.addressName : 'Adres seçiniz'}
            </Text>
          </View>
          <Icon name="chevron-down" size={22} color={Colors.textSecondary} />
        </TouchableOpacity>
        <TouchableOpacity
          style={styles.notificationButton}
          onPress={() => navigation.navigate('OrderHistory')}
        >
          <Icon name="bell-outline" size={24} color={Colors.text} />
        </TouchableOpacity>
      </View>

      <View style={styles.greetingContainer}>
        <Text style={styles.greeting}>
          Merhaba, {user?.firstName || 'Hoş geldiniz'} 👋
        </Text>
        <Text style={styles.greetingSub}>Ne yemek istersiniz?</Text>
      </View>

      <SearchBar
        value={searchQuery}
        onChangeText={setSearchQuery}
        onClear={() => setSearchQuery('')}
      />

      {cuisines.length > 0 && (
        <CuisineFilter
          cuisines={cuisines}
          selectedCuisine={selectedCuisine}
          onSelect={setSelectedCuisine}
        />
      )}

      <View style={styles.sectionHeader}>
        <Text style={styles.sectionTitle}>
          {selectedCuisine
            ? cuisines.find(c => c.id === selectedCuisine)?.name || 'Restoranlar'
            : 'Yakınındaki Restoranlar'}
        </Text>
        <Text style={styles.resultCount}>
          {filteredRestaurants.length} restoran
        </Text>
      </View>

      <FlatList
        data={filteredRestaurants}
        keyExtractor={item => item.id}
        renderItem={({item}) => (
          <RestaurantCard
            restaurant={item}
            onPress={() => {
              selectRestaurant(item.id);
              navigation.navigate('RestaurantDetail', {restaurantId: item.id});
            }}
          />
        )}
        ListEmptyComponent={
          <EmptyState
            icon="food-off"
            title="Restoran bulunamadı"
            message={searchQuery ? 'Arama kriterlerinize uygun restoran bulunamadı' : 'Yakınızda aktif restoran bulunmuyor'}
            actionLabel={searchQuery ? 'Aramayı Temizle' : null}
            onAction={searchQuery ? () => setSearchQuery('') : null}
          />
        }
        refreshControl={
          <RefreshControl refreshing={refreshing} onRefresh={onRefresh} tintColor={Colors.primary} />
        }
        contentContainerStyle={styles.listContent}
        showsVerticalScrollIndicator={false}
        keyboardShouldPersistTaps="handled"
      />
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: Colors.background,
  },
  listContent: {
    paddingBottom: 100,
  },
  topBar: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    paddingHorizontal: Spacing.base,
    paddingTop: Spacing.sm,
    paddingBottom: Spacing.sm,
  },
  locationButton: {
    flexDirection: 'row',
    alignItems: 'center',
    flex: 1,
    marginRight: Spacing.md,
  },
  locationTextContainer: {
    flex: 1,
    marginLeft: 8,
    marginRight: 4,
  },
  locationLabel: {
    fontSize: Fonts.sizes.xs,
    color: Colors.textSecondary,
    fontWeight: Fonts.weights.medium,
  },
  locationAddress: {
    fontSize: Fonts.sizes.md,
    color: Colors.text,
    fontWeight: Fonts.weights.bold,
  },
  notificationButton: {
    width: 44,
    height: 44,
    borderRadius: 22,
    backgroundColor: Colors.surface,
    justifyContent: 'center',
    alignItems: 'center',
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 1},
    shadowOpacity: 0.06,
    shadowRadius: 4,
    elevation: 2,
  },
  greetingContainer: {
    paddingHorizontal: Spacing.base,
    paddingTop: Spacing.md,
    paddingBottom: Spacing.lg,
  },
  greeting: {
    fontSize: Fonts.sizes.xxl,
    fontWeight: Fonts.weights.heavy,
    color: Colors.text,
  },
  greetingSub: {
    fontSize: Fonts.sizes.base,
    color: Colors.textSecondary,
    marginTop: 4,
  },
  sectionHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    paddingHorizontal: Spacing.base,
    marginBottom: Spacing.md,
  },
  sectionTitle: {
    fontSize: Fonts.sizes.lg,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
  },
  resultCount: {
    fontSize: Fonts.sizes.sm,
    color: Colors.textSecondary,
    fontWeight: Fonts.weights.medium,
  },
});

export default HomeScreen;
