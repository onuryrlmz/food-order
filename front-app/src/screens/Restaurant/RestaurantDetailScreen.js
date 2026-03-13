import React, {useState, useEffect, useRef} from 'react';
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  Image,
  TouchableOpacity,
  Animated,
  Dimensions,
  Alert,
  ActivityIndicator,
} from 'react-native';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import {Colors, Fonts, Spacing, BorderRadius} from '../../theme';
import {restaurantService} from '../../api';
import {useCart} from '../../context/CartContext';
import MenuItemCard from '../../components/MenuItemCard';
import CartFloatingButton from '../../components/CartFloatingButton';
import LoadingSpinner from '../../components/LoadingSpinner';

const {width: SCREEN_WIDTH} = Dimensions.get('window');
const HEADER_MAX_HEIGHT = 240;
const HEADER_MIN_HEIGHT = 90;

const RestaurantDetailScreen = ({route, navigation}) => {
  const {restaurantId} = route.params;
  const {cart, addItem} = useCart();
  const [restaurantData, setRestaurantData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [selectedCategory, setSelectedCategory] = useState(null);
  const scrollY = useRef(new Animated.Value(0)).current;

  useEffect(() => {
    loadRestaurantInfo();
  }, [restaurantId]);

  const loadRestaurantInfo = async () => {
    try {
      const res = await restaurantService.getRestaurantInfo(restaurantId);
      if (res.data && !res.data.hasFailed) {
        let parsed = res.data.data;
        if (typeof parsed === 'string') {
          parsed = JSON.parse(parsed);
        }
        setRestaurantData(parsed);
        if (parsed.categories?.length > 0) {
          setSelectedCategory(parsed.categories[0].id);
        }
      }
    } catch (e) {
      console.log('Restaurant info error:', e);
      Alert.alert('Hata', 'Restoran bilgileri yüklenemedi');
    } finally {
      setLoading(false);
    }
  };

  const handleAddToCart = (menuItem) => {
    if (cart && cart.restaurantId !== restaurantId) {
      Alert.alert(
        'Sepeti Değiştir',
        'Sepetinizde başka bir restoranın ürünleri var. Sepeti temizleyip bu restorandan eklensin mi?',
        [
          {text: 'İptal', style: 'cancel'},
          {
            text: 'Evet',
            style: 'destructive',
            onPress: () => doAddToCart(menuItem),
          },
        ],
      );
    } else {
      doAddToCart(menuItem);
    }
  };

  const doAddToCart = (menuItem) => {
    const item = {
      menuId: menuItem.id,
      menuName: menuItem.name,
      quantity: 1,
      unitPrice: menuItem.price,
      totalPrice: menuItem.price,
      imageUrl: menuItem.imageUrl || null,
      values: [],
    };
    addItem(item, {
      id: restaurantId,
      sellerId: restaurantData?.sellerId,
      name: restaurantData?.name,
      imageUrl: restaurantData?.coverImage,
    });
  };

  const headerTranslate = scrollY.interpolate({
    inputRange: [0, HEADER_MAX_HEIGHT - HEADER_MIN_HEIGHT],
    outputRange: [0, -(HEADER_MAX_HEIGHT - HEADER_MIN_HEIGHT)],
    extrapolate: 'clamp',
  });

  const imageOpacity = scrollY.interpolate({
    inputRange: [0, (HEADER_MAX_HEIGHT - HEADER_MIN_HEIGHT) / 2, HEADER_MAX_HEIGHT - HEADER_MIN_HEIGHT],
    outputRange: [1, 0.5, 0],
    extrapolate: 'clamp',
  });

  if (loading) {
    return <LoadingSpinner message="Restoran yükleniyor..." />;
  }

  if (!restaurantData) {
    return (
      <View style={styles.errorContainer}>
        <Icon name="alert-circle-outline" size={48} color={Colors.error} />
        <Text style={styles.errorText}>Restoran bulunamadı</Text>
        <TouchableOpacity style={styles.errorButton} onPress={() => navigation.goBack()}>
          <Text style={styles.errorButtonText}>Geri Dön</Text>
        </TouchableOpacity>
      </View>
    );
  }

  const categories = restaurantData.categories || [];
  const currentCategory = categories.find(c => c.id === selectedCategory);
  const menuItems = currentCategory?.menus || [];

  return (
    <View style={styles.container}>
      <Animated.View style={[styles.headerImage, {transform: [{translateY: headerTranslate}]}]}>
        {restaurantData.coverImage ? (
          <Animated.Image
            source={{uri: restaurantData.coverImage}}
            style={[styles.coverImage, {opacity: imageOpacity}]}
            resizeMode="cover"
          />
        ) : (
          <View style={styles.coverPlaceholder}>
            <Icon name="food" size={48} color={Colors.textLight} />
          </View>
        )}
        <View style={styles.headerOverlay} />
        <View style={styles.headerActions}>
          <TouchableOpacity style={styles.headerButton} onPress={() => navigation.goBack()}>
            <Icon name="arrow-left" size={24} color="#FFF" />
          </TouchableOpacity>
          <View style={styles.headerRight}>
            <TouchableOpacity style={styles.headerButton}>
              <Icon name="heart-outline" size={24} color="#FFF" />
            </TouchableOpacity>
            <TouchableOpacity style={[styles.headerButton, {marginLeft: 8}]}>
              <Icon name="share-variant-outline" size={24} color="#FFF" />
            </TouchableOpacity>
          </View>
        </View>
      </Animated.View>

      <Animated.ScrollView
        onScroll={Animated.event(
          [{nativeEvent: {contentOffset: {y: scrollY}}}],
          {useNativeDriver: true},
        )}
        scrollEventThrottle={16}
        showsVerticalScrollIndicator={false}
        contentContainerStyle={{paddingTop: HEADER_MAX_HEIGHT, paddingBottom: 100}}
      >
        <View style={styles.infoCard}>
          <Text style={styles.restaurantName}>{restaurantData.name}</Text>
          {restaurantData.description ? (
            <Text style={styles.restaurantDesc}>{restaurantData.description}</Text>
          ) : null}

          <View style={styles.infoRow}>
            <View style={styles.infoChip}>
              <Icon name="star" size={16} color={Colors.star} />
              <Text style={styles.infoChipText}>4.5</Text>
            </View>
            <View style={styles.infoChip}>
              <Icon name="clock-outline" size={16} color={Colors.primary} />
              <Text style={styles.infoChipText}>
                {restaurantData.minDeliveryTime}-{restaurantData.maxDeliveryTime} dk
              </Text>
            </View>
            <View style={styles.infoChip}>
              <Icon name="currency-try" size={16} color={Colors.primary} />
              <Text style={styles.infoChipText}>
                Min ₺{restaurantData.minimumOrderPrice}
              </Text>
            </View>
          </View>

          {restaurantData.isOpen === false && (
            <View style={styles.closedBanner}>
              <Icon name="clock-alert-outline" size={18} color={Colors.error} />
              <Text style={styles.closedText}>Restoran şu anda kapalı</Text>
            </View>
          )}
        </View>

        {categories.length > 0 && (
          <ScrollView
            horizontal
            showsHorizontalScrollIndicator={false}
            contentContainerStyle={styles.categoryTabs}
          >
            {categories.map(cat => {
              const isActive = selectedCategory === cat.id;
              return (
                <TouchableOpacity
                  key={cat.id}
                  style={[styles.categoryTab, isActive && styles.categoryTabActive]}
                  onPress={() => setSelectedCategory(cat.id)}
                  activeOpacity={0.8}
                >
                  <Text style={[styles.categoryTabText, isActive && styles.categoryTabTextActive]}>
                    {cat.name}
                  </Text>
                  {isActive && <View style={styles.categoryIndicator} />}
                </TouchableOpacity>
              );
            })}
          </ScrollView>
        )}

        <View style={styles.menuSection}>
          {menuItems.length > 0 ? (
            menuItems.map(item => (
              <MenuItemCard
                key={item.id}
                item={item}
                onPress={() => handleAddToCart(item)}
              />
            ))
          ) : (
            <View style={styles.emptyMenu}>
              <Icon name="food-off" size={40} color={Colors.textLight} />
              <Text style={styles.emptyMenuText}>Bu kategoride ürün bulunmuyor</Text>
            </View>
          )}
        </View>
      </Animated.ScrollView>

      <CartFloatingButton onPress={() => navigation.navigate('Cart')} />
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: Colors.background,
  },
  headerImage: {
    position: 'absolute',
    top: 0,
    left: 0,
    right: 0,
    height: HEADER_MAX_HEIGHT,
    zIndex: 10,
  },
  coverImage: {
    width: '100%',
    height: '100%',
  },
  coverPlaceholder: {
    flex: 1,
    backgroundColor: Colors.borderLight,
    justifyContent: 'center',
    alignItems: 'center',
  },
  headerOverlay: {
    ...StyleSheet.absoluteFillObject,
    backgroundColor: 'rgba(0,0,0,0.25)',
  },
  headerActions: {
    position: 'absolute',
    top: 50,
    left: 16,
    right: 16,
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
  },
  headerRight: {
    flexDirection: 'row',
  },
  headerButton: {
    width: 42,
    height: 42,
    borderRadius: 21,
    backgroundColor: 'rgba(0,0,0,0.35)',
    justifyContent: 'center',
    alignItems: 'center',
  },
  infoCard: {
    backgroundColor: Colors.surface,
    marginHorizontal: Spacing.base,
    marginTop: -20,
    borderRadius: BorderRadius.xl,
    padding: Spacing.lg,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 4},
    shadowOpacity: 0.08,
    shadowRadius: 12,
    elevation: 6,
  },
  restaurantName: {
    fontSize: Fonts.sizes.xxl,
    fontWeight: Fonts.weights.heavy,
    color: Colors.text,
    marginBottom: 6,
  },
  restaurantDesc: {
    fontSize: Fonts.sizes.md,
    color: Colors.textSecondary,
    marginBottom: Spacing.md,
    lineHeight: 20,
  },
  infoRow: {
    flexDirection: 'row',
    gap: 8,
  },
  infoChip: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: Colors.borderLight,
    paddingHorizontal: 10,
    paddingVertical: 6,
    borderRadius: 8,
  },
  infoChipText: {
    fontSize: Fonts.sizes.sm,
    fontWeight: Fonts.weights.semibold,
    color: Colors.text,
    marginLeft: 4,
  },
  closedBanner: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#FFEBEE',
    padding: Spacing.md,
    borderRadius: BorderRadius.md,
    marginTop: Spacing.md,
  },
  closedText: {
    fontSize: Fonts.sizes.sm,
    color: Colors.error,
    fontWeight: Fonts.weights.semibold,
    marginLeft: 8,
  },
  categoryTabs: {
    paddingHorizontal: Spacing.base,
    paddingTop: Spacing.lg,
    paddingBottom: Spacing.sm,
    gap: 4,
  },
  categoryTab: {
    paddingHorizontal: 18,
    paddingVertical: 10,
    borderRadius: 10,
    position: 'relative',
  },
  categoryTabActive: {
    backgroundColor: Colors.primary + '12',
  },
  categoryTabText: {
    fontSize: Fonts.sizes.md,
    fontWeight: Fonts.weights.semibold,
    color: Colors.textSecondary,
  },
  categoryTabTextActive: {
    color: Colors.primary,
    fontWeight: Fonts.weights.bold,
  },
  categoryIndicator: {
    position: 'absolute',
    bottom: 0,
    left: 18,
    right: 18,
    height: 3,
    backgroundColor: Colors.primary,
    borderRadius: 1.5,
  },
  menuSection: {
    paddingTop: Spacing.md,
    paddingBottom: Spacing.xxl,
  },
  emptyMenu: {
    alignItems: 'center',
    paddingVertical: Spacing.xxxl,
  },
  emptyMenuText: {
    fontSize: Fonts.sizes.md,
    color: Colors.textSecondary,
    marginTop: Spacing.md,
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

export default RestaurantDetailScreen;
