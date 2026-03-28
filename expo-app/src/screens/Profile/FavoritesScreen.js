import React, {useState, useEffect, useCallback} from 'react';
import {
  View,
  Text,
  StyleSheet,
  FlatList,
  TouchableOpacity,
  RefreshControl,
} from 'react-native';
import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import {Colors, Fonts, Spacing, BorderRadius} from '../../theme';
import {favoriteService} from '../../api';
import RestaurantCard from '../../components/RestaurantCard';
import LoadingSpinner from '../../components/LoadingSpinner';
import EmptyState from '../../components/EmptyState';

import { router } from 'expo-router';
const FavoritesScreen = () => {
  const [favorites, setFavorites] = useState([]);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);

  useEffect(() => {
    loadFavorites();
  }, []);

  const loadFavorites = async () => {
    try {
      const res = await favoriteService.getFavorites(1, 50);
      if (res.data && !res.data.hasFailed) {
        setFavorites(res.data.data || []);
      }
    } catch (e) {
      console.log('Favorites error:', e);
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  };

  const onRefresh = useCallback(() => {
    setRefreshing(true);
    loadFavorites();
  }, []);

  if (loading) {
    return <LoadingSpinner message="Favoriler yukleniyor..." />;
  }

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <TouchableOpacity style={styles.backButton} onPress={() => router.back()}>
          <Icon name="arrow-left" size={24} color={Colors.text} />
        </TouchableOpacity>
        <Text style={styles.headerTitle}>Favori Restoranlarim</Text>
        <View style={{width: 44}} />
      </View>

      <FlatList
        data={favorites}
        keyExtractor={(item) => item.restaurantId || item.id}
        renderItem={({item}) => (
          <RestaurantCard
            restaurant={item}
            onPress={() => router.push(`/restaurant/${item.restaurantId || item.id}`)}
          />
        )}
        ListEmptyComponent={
          <EmptyState
            icon="heart-off-outline"
            title="Henuz Favoriniz Yok"
            message="Begendginiz restoranlari favorilere ekleyerek hizlica erisebilirsiniz"
          />
        }
        refreshControl={
          <RefreshControl refreshing={refreshing} onRefresh={onRefresh} tintColor={Colors.primary} />
        }
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
  listContent: {
    paddingTop: Spacing.md,
    paddingBottom: Spacing.xxl,
    flexGrow: 1,
  },
});

export default FavoritesScreen;
