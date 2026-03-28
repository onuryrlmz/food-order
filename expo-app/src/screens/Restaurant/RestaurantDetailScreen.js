import React, { useState, useEffect, useRef, useCallback, useMemo } from 'react';
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  TouchableOpacity,
  TextInput,
  Animated,
  Dimensions,
  Platform,
} from 'react-native';
import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import { Colors, Fonts, Spacing, BorderRadius } from '../../theme';
import { addressService } from '../../api';
import { useCart } from '../../context/CartContext';
import { useAuth } from '../../context/AuthContext';
import { useAppData } from '../../context/AppDataContext';
import { useToast } from '../../context/ToastContext';
import MenuItemCard from '../../components/MenuItemCard';
import LoadingSpinner from '../../components/LoadingSpinner';
import MenuOptionModal from '../../components/MenuOptionModal';
import ReviewList from '../../components/ReviewList';
import {favoriteService} from '../../api';

const { width: SCREEN_WIDTH } = Dimensions.get('window');
const COVER_HEIGHT = 240;
const STATUS_BAR_HEIGHT = Platform.OS === 'ios' ? 50 : 24;

import { router, useLocalSearchParams, useFocusEffect } from 'expo-router';

const RestaurantDetailScreen = () => {
  const params = useLocalSearchParams();
  const restaurantId = params.restaurantId || params.id;
  const { cart, addItem, isDifferentRestaurant } = useCart();
  const { isAuthenticated } = useAuth();
  const { selectedRestaurant, selectedRestaurantJson, selectRestaurant } = useAppData();
  const { showToast, showConfirm } = useToast();

  const restaurantName = selectedRestaurant?.name || '';
  const coverImage = selectedRestaurant?.imageUrl || null;
  const sellerId = selectedRestaurant?.sellerId || null;

  const [menuData, setMenuData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [selectedCategory, setSelectedCategory] = useState(null);
  const [modalVisible, setModalVisible] = useState(false);
  const [selectedMenu, setSelectedMenu] = useState(null);
  const [pendingCartAction, setPendingCartAction] = useState(null);
  const [searchQuery, setSearchQuery] = useState('');
  const [isFavorite, setIsFavorite] = useState(false);
  const mainScrollRef = useRef(null);
  const tabScrollRef = useRef(null);
  const sectionLayouts = useRef({});
  const tabLayouts = useRef({});
  const isUserTap = useRef(false);
  const currentScrollY = useRef(0);
  const [isSticky, setIsSticky] = useState(false);
  const [infoCardHeight, setInfoCardHeight] = useState(0);
  const [tabBarHeight, setTabBarHeight] = useState(0);
  const stickyThreshold = useRef(0);

  useEffect(() => {
    // Eğer selectedRestaurant yoksa (direkt URL ile gelindi vs.) yükle
    if (!selectedRestaurant || selectedRestaurant.id !== restaurantId) {
      selectRestaurant(restaurantId);
    }
  }, [restaurantId]);

  // selectedRestaurantJson yüklenince menuData'yı set et
  useEffect(() => {
    if (selectedRestaurantJson && selectedRestaurantJson.length > 0) {
      setMenuData(selectedRestaurantJson);
      setSelectedCategory(selectedRestaurantJson[0].id);
      setLoading(false);
    }
  }, [selectedRestaurantJson]);

  // Login veya adres ekleme sonrasi geri donunce bekleyen sepet aksiyonunu calistir
  useFocusEffect(
    useCallback(() => {
      if (pendingCartAction && isAuthenticated) {
        checkAddressAndAdd(pendingCartAction.menuItem, pendingCartAction.selectedOptions, pendingCartAction.totalPrice);
      }
    }, [pendingCartAction, isAuthenticated])
  );

  const handleAddToCart = (menuItem) => {
    const hasOptions = menuItem.menuOptions && menuItem.menuOptions.length > 0;

    if (hasOptions) {
      setSelectedMenu(menuItem);
      setModalVisible(true);
    } else {
      confirmAndAdd(menuItem, [], menuItem.price);
    }
  };

  const handleModalAddToCart = (menu, selectedOptions, totalPrice) => {
    confirmAndAdd(menu, selectedOptions, totalPrice);
  };

  const confirmAndAdd = (menuItem, selectedOptions, totalPrice) => {
    if (isDifferentRestaurant(restaurantId)) {
      showConfirm({
        title: 'Sepeti Değiştir',
        message: `Sepetinizde "${cart.restaurantName}" restoranından ürünler var. Sepeti temizleyip bu restorandan eklensin mi?`,
        confirmText: 'Evet, Değiştir',
        cancelText: 'Hayır',
        confirmStyle: 'destructive',
        onConfirm: () => doAddToCart(menuItem, selectedOptions, totalPrice),
      });
    } else {
      doAddToCart(menuItem, selectedOptions, totalPrice);
    }
  };

  const doAddToCart = (menuItem, selectedOptions, totalPrice) => {
    if (!isAuthenticated) {
      setPendingCartAction({ menuItem, selectedOptions, totalPrice });
      router.push('/(auth)/login');
      return;
    }
    checkAddressAndAdd(menuItem, selectedOptions, totalPrice);
  };

  const checkAddressAndAdd = async (menuItem, selectedOptions, totalPrice) => {
    try {
      const res = await addressService.getList();
      const list = res.data?.data || res.data?.rawData || [];
      const addresses = Array.isArray(list) ? list : [];
      if (addresses.length === 0) {
        setPendingCartAction({ menuItem, selectedOptions, totalPrice });
        showConfirm({
          title: 'Adres Gerekli',
          message: 'Sipariş verebilmek için bir teslimat adresi eklemelisiniz.',
          confirmText: 'Adres Ekle',
          cancelText: 'İptal',
          confirmStyle: 'primary',
          onConfirm: () => router.push('/add-address'),
          onCancel: () => setPendingCartAction(null),
        });
        return;
      }
    } catch (e) {}
    setPendingCartAction(null);
    const item = {
      menuId: menuItem.id,
      menuName: menuItem.name,
      quantity: 1,
      unitPrice: totalPrice,
      totalPrice: totalPrice,
      imageUrl: menuItem.imageUrl || null,
      values: selectedOptions,
    };
    addItem(item, {
      id: restaurantId,
      sellerId: sellerId,
      name: restaurantName,
      imageUrl: coverImage,
    });
  };

  const toggleFavorite = async () => {
    if (!isAuthenticated) return;
    try {
      if (isFavorite) {
        await favoriteService.removeFavorite(restaurantId);
        setIsFavorite(false);
      } else {
        await favoriteService.addFavorite(restaurantId);
        setIsFavorite(true);
      }
    } catch (e) {
      // silently fail
    }
  };

  const handleInfoCardLayout = useCallback((event) => {
    const h = event.nativeEvent.layout.height;
    setInfoCardHeight(h);
  }, []);

  const handleTabBarLayout = useCallback((event) => {
    const h = event.nativeEvent.layout.height;
    setTabBarHeight(h);
  }, []);

  const handleSectionLayout = useCallback((categoryId, event) => {
    sectionLayouts.current[categoryId] = event.nativeEvent.layout.y;
  }, []);

  const handleTabLayout = useCallback((categoryId, event) => {
    tabLayouts.current[categoryId] = event.nativeEvent.layout.x;
  }, []);

  const stickyTop = STATUS_BAR_HEIGHT + 52;

  const handleScroll = useCallback((event) => {
    const offsetY = event.nativeEvent.contentOffset.y;
    currentScrollY.current = offsetY;

    const threshold = COVER_HEIGHT - stickyTop - 20;
    const shouldStick = offsetY >= threshold;
    setIsSticky(shouldStick);

    if (isUserTap.current) return;

    const stickyOffset = stickyTop + infoCardHeight + tabBarHeight;
    const adjustedY = offsetY - COVER_HEIGHT + stickyOffset + 20;
    const entries = Object.entries(sectionLayouts.current)
      .map(([id, y]) => ({ id, y }))
      .sort((a, b) => a.y - b.y);

    let activeCat = entries[0]?.id;
    for (const entry of entries) {
      if (entry.y <= adjustedY) {
        activeCat = entry.id;
      } else {
        break;
      }
    }

    if (activeCat && activeCat !== selectedCategory) {
      setSelectedCategory(activeCat);
      scrollTabIntoView(activeCat);
    }
  }, [selectedCategory, infoCardHeight, tabBarHeight]);

  const scrollTabIntoView = useCallback((categoryId) => {
    const tabX = tabLayouts.current[categoryId];
    if (tabX != null && tabScrollRef.current) {
      tabScrollRef.current.scrollTo({ x: Math.max(0, tabX - 40), animated: true });
    }
  }, []);

  const handleTabPress = useCallback((categoryId) => {
    setSelectedCategory(categoryId);
    isUserTap.current = true;

    const sectionY = sectionLayouts.current[categoryId];
    if (sectionY != null && mainScrollRef.current) {
      const stickyOffset = stickyTop + infoCardHeight + tabBarHeight;
      mainScrollRef.current.scrollTo({
        y: sectionY + COVER_HEIGHT - stickyOffset + 10,
        animated: true,
      });
    }
    scrollTabIntoView(categoryId);

    setTimeout(() => {
      isUserTap.current = false;
    }, 600);
  }, [scrollTabIntoView, infoCardHeight, tabBarHeight]);

  const categories = useMemo(() => {
    const seen = new Map();
    return menuData.filter(cat => {
      if (seen.has(cat.name)) {
        // Merge categoriesDetail into existing
        const existing = seen.get(cat.name);
        if (cat.categoriesDetail) {
          existing.categoriesDetail = [...(existing.categoriesDetail || []), ...cat.categoriesDetail];
        }
        return false;
      }
      seen.set(cat.name, cat);
      return true;
    });
  }, [menuData]);

  const filteredCategories = useMemo(() => {
    const q = searchQuery.trim().toLowerCase();
    if (!q) return categories;

    return categories
      .map(cat => {
        const matchedMenus = (cat.categoriesDetail || []).map(cd => {
          const filtered = (cd.menus || []).filter(menu => {
            if (menu.name?.toLowerCase().includes(q)) return true;
            if (menu.description?.toLowerCase().includes(q)) return true;
            if (menu.menuOptions?.some(opt =>
              opt.name?.toLowerCase().includes(q) ||
              opt.menuOptionValues?.some(v => v.name?.toLowerCase().includes(q))
            )) return true;
            return false;
          });
          return { ...cd, menus: filtered };
        }).filter(cd => cd.menus.length > 0);

        if (cat.name?.toLowerCase().includes(q) && matchedMenus.length === 0) {
          return cat;
        }

        if (matchedMenus.length === 0) return null;
        return { ...cat, categoriesDetail: matchedMenus };
      })
      .filter(Boolean);
  }, [categories, searchQuery]);

  const isSearching = searchQuery.trim().length > 0;
  const displayCategories = isSearching ? filteredCategories : categories;

  if (loading) {
    return <LoadingSpinner message="Restoran yükleniyor..." />;
  }

  if (!menuData || menuData.length === 0) {
    return (
      <View style={styles.errorContainer}>
        <Icon name="alert-circle-outline" size={48} color={Colors.error} />
        <Text style={styles.errorText}>Menü bulunamadı</Text>
        <TouchableOpacity style={styles.errorButton} onPress={() => router.back()}>
          <Text style={styles.errorButtonText}>Geri Dön</Text>
        </TouchableOpacity>
      </View>
    );
  }

  const renderInfoCard = () => (
    <View style={styles.infoCardInner}>
      <Text style={styles.restaurantName}>{restaurantName || 'Restoran'}</Text>
      <View style={styles.infoRow}>
        <View style={styles.infoChip}>
          <Icon name="star" size={16} color={Colors.star} />
          <Text style={styles.infoChipText}>4.5</Text>
        </View>
        <View style={styles.infoChip}>
          <Icon name="clock-outline" size={16} color={Colors.primary} />
          <Text style={styles.infoChipText}>30-45 dk</Text>
        </View>
      </View>
      <View style={styles.searchContainer}>
        <Icon name="magnify" size={20} color={Colors.textLight} style={styles.searchIcon} />
        <TextInput
          style={styles.searchInput}
          placeholder="Menüde ara..."
          placeholderTextColor={Colors.textLight}
          value={searchQuery}
          onChangeText={setSearchQuery}
          returnKeyType="search"
        />
        {searchQuery.length > 0 && (
          <TouchableOpacity onPress={() => setSearchQuery('')} style={styles.searchClear}>
            <Icon name="close-circle" size={18} color={Colors.textLight} />
          </TouchableOpacity>
        )}
      </View>
    </View>
  );

  const renderTabBar = () => (
    <ScrollView
      ref={tabScrollRef}
      horizontal
      showsHorizontalScrollIndicator={false}
      contentContainerStyle={styles.categoryTabs}
    >
      {displayCategories.map(cat => {
        const isActive = selectedCategory === cat.id;
        return (
          <TouchableOpacity
            key={cat.id}
            style={[styles.categoryTab, isActive && styles.categoryTabActive]}
            onPress={() => handleTabPress(cat.id)}
            onLayout={(e) => handleTabLayout(cat.id, e)}
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
  );

  return (
    <View style={styles.container}>
      <ScrollView
        ref={mainScrollRef}
        onScroll={handleScroll}
        scrollEventThrottle={16}
        showsVerticalScrollIndicator={false}
        contentContainerStyle={{ paddingBottom: 100 }}
      >
        {/* Cover Image */}
        <View style={styles.coverContainer}>
          {coverImage ? (
            <Animated.Image
              source={{ uri: coverImage }}
              style={styles.coverImage}
              resizeMode="cover"
            />
          ) : (
            <View style={styles.coverPlaceholder}>
              <Icon name="store" size={48} color={Colors.textLight} />
            </View>
          )}
          <View style={styles.headerOverlay} />
        </View>

        {/* Info Card - inline (scrolls with content) */}
        <View style={[styles.infoCard, isSticky && { opacity: 0 }]} onLayout={handleInfoCardLayout}>
          {renderInfoCard()}
        </View>

        {/* Tab Bar - inline (scrolls with content) */}
        {displayCategories.length > 0 && (
          <View style={isSticky ? { opacity: 0 } : undefined} onLayout={handleTabBarLayout}>
            {renderTabBar()}
          </View>
        )}

        {/* Menu Sections */}
        <View style={styles.menuSection}>
          {displayCategories.map(cat => {
            const catMenuItems = cat.categoriesDetail?.flatMap(cd => cd.menus || []) || [];
            if (catMenuItems.length === 0) return null;
            return (
              <View
                key={cat.id}
                onLayout={(e) => handleSectionLayout(cat.id, e)}
              >
                <Text style={styles.sectionTitle}>{cat.name}</Text>
                {catMenuItems.map(item => (
                  <MenuItemCard
                    key={item.id}
                    item={item}
                    onPress={() => handleAddToCart(item)}
                  />
                ))}
              </View>
            );
          })}
          {isSearching && displayCategories.length === 0 && (
            <View style={styles.emptySearch}>
              <Icon name="magnify" size={40} color={Colors.textLight} />
              <Text style={styles.emptySearchText}>"{ searchQuery}" için sonuç bulunamadı</Text>
            </View>
          )}
        </View>

        {/* Reviews Section */}
        {!isSearching && <ReviewList restaurantId={restaurantId} />}
      </ScrollView>

      {/* Sticky Header Overlay */}
      {isSticky && (
        <View style={styles.stickyHeader}>
          <View style={styles.stickyActionRow}>
            <TouchableOpacity
              style={[styles.headerButton, styles.headerButtonSticky]}
              onPress={() => router.back()}
            >
              <Icon name="arrow-left" size={24} color={Colors.text} />
            </TouchableOpacity>
            <View style={styles.headerRight}>
              <TouchableOpacity style={[styles.headerButton, styles.headerButtonSticky]} onPress={toggleFavorite}>
                <Icon name={isFavorite ? 'heart' : 'heart-outline'} size={24} color={isFavorite ? Colors.primary : Colors.text} />
              </TouchableOpacity>
              <TouchableOpacity style={[styles.headerButton, styles.headerButtonSticky, styles.headerButtonML]}>
                <Icon name="share-variant-outline" size={24} color={Colors.text} />
              </TouchableOpacity>
            </View>
          </View>
          <View style={styles.stickyInfoCard}>
            {renderInfoCard()}
          </View>
          {displayCategories.length > 0 && (
            <View style={styles.stickyTabBar}>
              {renderTabBar()}
            </View>
          )}
        </View>
      )}

      {/* Back Button - shown only when NOT sticky */}
      {!isSticky && (
        <View style={styles.headerActions}>
          <TouchableOpacity
            style={styles.headerButton}
            onPress={() => router.back()}
          >
            <Icon name="arrow-left" size={24} color="#FFF" />
          </TouchableOpacity>
          <View style={styles.headerRight}>
            <TouchableOpacity style={styles.headerButton} onPress={toggleFavorite}>
              <Icon name={isFavorite ? 'heart' : 'heart-outline'} size={24} color={isFavorite ? Colors.primary : '#FFF'} />
            </TouchableOpacity>
            <TouchableOpacity style={[styles.headerButton, styles.headerButtonML]}>
              <Icon name="share-variant-outline" size={24} color="#FFF" />
            </TouchableOpacity>
          </View>
        </View>
      )}

      <MenuOptionModal
        visible={modalVisible}
        menu={selectedMenu}
        onClose={() => {
          setModalVisible(false);
          setSelectedMenu(null);
        }}
        onAddToCart={handleModalAddToCart}
      />
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: Colors.background,
  },
  coverContainer: {
    height: COVER_HEIGHT,
    width: '100%',
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
    top: STATUS_BAR_HEIGHT,
    left: 16,
    right: 16,
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    zIndex: 30,
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
  headerButtonSticky: {
    backgroundColor: Colors.borderLight,
  },
  headerButtonML: {
    marginLeft: 8,
  },
  infoCard: {
    backgroundColor: Colors.surface,
    marginHorizontal: Spacing.base,
    marginTop: -20,
    borderRadius: BorderRadius.xl,
    padding: Spacing.lg,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.08,
    shadowRadius: 12,
    elevation: 6,
  },
  infoCardInner: {},
  restaurantName: {
    fontSize: Fonts.sizes.xxl,
    fontWeight: Fonts.weights.heavy,
    color: Colors.text,
    marginBottom: 6,
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
  stickyHeader: {
    position: 'absolute',
    top: 0,
    left: 0,
    right: 0,
    zIndex: 20,
    backgroundColor: Colors.surface,
    paddingTop: STATUS_BAR_HEIGHT,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 8,
  },
  stickyActionRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    paddingHorizontal: 16,
    paddingVertical: 6,
  },
  stickyInfoCard: {
    backgroundColor: Colors.surface,
    paddingHorizontal: Spacing.lg,
    paddingBottom: Spacing.sm,
  },
  stickyTabBar: {
    backgroundColor: Colors.surface,
    borderBottomWidth: StyleSheet.hairlineWidth,
    borderBottomColor: Colors.borderLight,
  },
  categoryTabs: {
    paddingHorizontal: Spacing.base,
    paddingTop: Spacing.sm,
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
    paddingTop: Spacing.sm,
    paddingBottom: Spacing.xxl,
  },
  sectionTitle: {
    fontSize: Fonts.sizes.lg,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
    paddingHorizontal: Spacing.base,
    paddingTop: Spacing.lg,
    paddingBottom: Spacing.sm,
  },
  searchContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: Colors.borderLight,
    borderRadius: 10,
    marginTop: Spacing.md,
    paddingHorizontal: 12,
    height: 40,
  },
  searchIcon: {
    marginRight: 8,
  },
  searchInput: {
    flex: 1,
    fontSize: Fonts.sizes.md,
    color: Colors.text,
    padding: 0,
    height: 40,
  },
  searchClear: {
    padding: 4,
    marginLeft: 4,
  },
  emptySearch: {
    alignItems: 'center',
    paddingVertical: Spacing.xxxl,
  },
  emptySearchText: {
    fontSize: Fonts.sizes.md,
    color: Colors.textSecondary,
    marginTop: Spacing.md,
    textAlign: 'center',
    paddingHorizontal: Spacing.lg,
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
