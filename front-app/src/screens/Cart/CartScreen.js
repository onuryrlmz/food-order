import React, {useState} from 'react';
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  TouchableOpacity,
  Image,
  TextInput,
  ActivityIndicator,
} from 'react-native';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import {Colors, Fonts, Spacing, BorderRadius} from '../../theme';
import {useCart} from '../../context/CartContext';
import {useAppData} from '../../context/AppDataContext';
import EmptyState from '../../components/EmptyState';
import {couponService} from '../../api/couponService';
import {useToast} from '../../context/ToastContext';

const CartScreen = ({navigation}) => {
  const {cart, updateItemQuantity, removeItem, clearCart, appliedCoupon, discountAmount, applyCoupon, removeCoupon, getFinalTotal, orderNote, setOrderNote} = useCart();
  const {selectRestaurant} = useAppData();
  const {showConfirm} = useToast();
  const [couponCode, setCouponCode] = useState('');
  const [couponLoading, setCouponLoading] = useState(false);
  const [couponError, setCouponError] = useState('');
  const [expandedItems, setExpandedItems] = useState({});

  // Format selected options into readable summary
  const formatOptionsSummary = (values) => {
    if (!values || values.length === 0) return null;

    const parts = [];

    values.forEach(val => {
      // Add main value name
      parts.push(val.valueName);

      // Add nested value option values
      if (val.valueOptionValues && val.valueOptionValues.length > 0) {
        val.valueOptionValues.forEach(voov => {
          parts.push(voov.name);
        });
      }
    });

    return parts.join('\n');
  };

  const handleClearCart = () => {
    showConfirm({
      title: 'Sepeti Temizle',
      message: 'Sepetinizdeki tüm ürünler silinecek. Emin misiniz?',
      confirmText: 'Temizle',
      cancelText: 'İptal',
      confirmStyle: 'destructive',
      onConfirm: clearCart,
    });
  };

  const handleApplyCoupon = async () => {
    if (!couponCode.trim()) {
      setCouponError('Kupon kodu giriniz');
      return;
    }

    setCouponLoading(true);
    setCouponError('');

    try {
      const items = cart.items.map(item => ({
        menuId: item.menuId,
        quantity: item.quantity,
        unitPrice: item.unitPrice,
      }));

      const res = await couponService.validateCoupon(
        couponCode.trim().toUpperCase(),
        cart.restaurantId,
        cart.totalPrice,
        items,
      );

      if (res.data && !res.data.hasFailed) {
        const result = res.data.data;
        if (result.isValid) {
          applyCoupon({
            id: result.couponId,
            code: result.couponCode,
            name: result.couponName,
            description: result.discountDescription,
          }, result.discountAmount);
          setCouponCode('');
        } else {
          setCouponError(result.errorMessage || 'Kupon geçersiz');
        }
      } else {
        setCouponError(res.data?.messages?.[0]?.description || 'Kupon doğrulanamadı');
      }
    } catch (err) {
      setCouponError('Kupon doğrulanırken bir hata oluştu');
      console.error(err);
    } finally {
      setCouponLoading(false);
    }
  };

  if (!cart || cart.items.length === 0) {
    return (
      <View style={styles.container}>
        <View style={styles.header}>
          <TouchableOpacity style={styles.backButton} onPress={() => navigation.goBack()}>
            <Icon name="arrow-left" size={24} color={Colors.text} />
          </TouchableOpacity>
          <Text style={styles.headerTitle}>Sepetim</Text>
          <View style={{width: 44}} />
        </View>
        <EmptyState
          icon="cart-off"
          title="Sepetiniz Boş"
          message="Lezzetli yemekleri keşfetmek için restoranları inceleyin"
          actionLabel="Restoran Keşfet"
          onAction={() => navigation.navigate('HomeTab')}
        />
      </View>
    );
  }

  const deliveryFee = 9.99;
  const subtotal = cart.totalPrice;
  const finalDiscount = discountAmount;
  const finalTotal = Math.max(0, subtotal - finalDiscount + deliveryFee);

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <TouchableOpacity style={styles.backButton} onPress={() => navigation.goBack()}>
          <Icon name="arrow-left" size={24} color={Colors.text} />
        </TouchableOpacity>
        <Text style={styles.headerTitle}>Sepetim</Text>
        <TouchableOpacity style={styles.clearButton} onPress={handleClearCart}>
          <Icon name="delete-outline" size={22} color={Colors.error} />
        </TouchableOpacity>
      </View>

      <ScrollView showsVerticalScrollIndicator={false} contentContainerStyle={styles.scrollContent}>
        <View style={styles.restaurantInfo}>
          <View style={styles.restaurantIcon}>
            <Icon name="store" size={22} color={Colors.primary} />
          </View>
          <View style={styles.restaurantTextContainer}>
            <Text style={styles.restaurantName}>{cart.restaurantName}</Text>
            <Text style={styles.itemCountText}>{cart.totalQuantity} ürün</Text>
          </View>
          <TouchableOpacity
            style={styles.addMoreButton}
            onPress={() => {
              selectRestaurant(cart.restaurantId);
              navigation.navigate('HomeTab', {screen: 'RestaurantDetail', params: {restaurantId: cart.restaurantId}});
            }}
          >
            <Icon name="plus" size={16} color={Colors.primary} />
            <Text style={styles.addMoreText}>Ekle</Text>
          </TouchableOpacity>
        </View>

        <View style={styles.itemsContainer}>
          {cart.items.map((item) => (
            <View key={item.cartItemId} style={styles.cartItem}>
              <View style={styles.cartItemLeft}>
                {item.imageUrl ? (
                  <Image source={{uri: item.imageUrl}} style={styles.itemImage} />
                ) : (
                  <View style={styles.itemImagePlaceholder}>
                    <Icon name="food" size={20} color={Colors.textLight} />
                  </View>
                )}
                <View style={styles.itemDetails}>
                  <Text style={styles.itemName} numberOfLines={2}>{item.menuName}</Text>
                  {item.values && item.values.length > 0 && (
                    <TouchableOpacity
                      activeOpacity={0.7}
                      onPress={() => setExpandedItems(prev => ({...prev, [item.cartItemId]: !prev[item.cartItemId]}))}
                      style={styles.itemOptionsRow}
                    >
                      <Text
                        style={styles.itemOptions}
                        numberOfLines={expandedItems[item.cartItemId] ? undefined : 1}
                      >
                        {formatOptionsSummary(item.values)}
                      </Text>
                      <Icon
                        name={expandedItems[item.cartItemId] ? 'chevron-up' : 'chevron-down'}
                        size={14}
                        color={Colors.textSecondary}
                        style={styles.expandIcon}
                      />
                    </TouchableOpacity>
                  )}
                  <Text style={styles.itemPrice}>₺{item.unitPrice.toFixed(2)}</Text>
                </View>
              </View>
              <View style={styles.quantityControl}>
                <TouchableOpacity
                  style={styles.quantityButton}
                  onPress={() => {
                    if (item.quantity === 1) {
                      showConfirm({
                        title: 'Ürünü Kaldır',
                        message: 'Bu ürünü sepetten kaldırmak istiyor musunuz?',
                        confirmText: 'Kaldır',
                        cancelText: 'İptal',
                        confirmStyle: 'destructive',
                        onConfirm: () => removeItem(item.cartItemId),
                      });
                    } else {
                      updateItemQuantity(item.cartItemId, item.quantity - 1);
                    }
                  }}
                >
                  <Icon
                    name={item.quantity === 1 ? 'delete-outline' : 'minus'}
                    size={16}
                    color={item.quantity === 1 ? Colors.error : Colors.primary}
                  />
                </TouchableOpacity>
                <Text style={styles.quantityText}>{item.quantity}</Text>
                <TouchableOpacity
                  style={[styles.quantityButton, styles.quantityButtonPlus]}
                  onPress={() => updateItemQuantity(item.cartItemId, item.quantity + 1)}
                >
                  <Icon name="plus" size={16} color="#FFF" />
                </TouchableOpacity>
              </View>
            </View>
          ))}
        </View>

        <View style={styles.noteContainer}>
          <Icon name="note-text-outline" size={20} color={Colors.textSecondary} style={styles.noteIcon} />
          <TextInput
            style={styles.noteInput}
            placeholder="Sipariş notu ekleyebilirsiniz..."
            placeholderTextColor={Colors.textLight}
            value={orderNote}
            onChangeText={setOrderNote}
            multiline
            maxLength={200}
          />
        </View>

        {/* Coupon Section */}
        <View style={styles.couponSection}>
          <Text style={styles.couponTitle}>Kupon Kodu</Text>
          
          {appliedCoupon ? (
            <View style={styles.appliedCouponContainer}>
              <View style={styles.appliedCouponInfo}>
                <Icon name="ticket-percent" size={20} color={Colors.success} />
                <View style={styles.appliedCouponText}>
                  <Text style={styles.appliedCouponCode}>{appliedCoupon.code}</Text>
                  <Text style={styles.appliedCouponDesc}>{appliedCoupon.description}</Text>
                </View>
              </View>
              <TouchableOpacity onPress={removeCoupon} style={styles.removeCouponButton}>
                <Icon name="close" size={20} color={Colors.error} />
              </TouchableOpacity>
            </View>
          ) : (
            <View style={styles.couponInputContainer}>
              <TextInput
                style={styles.couponInput}
                placeholder="Kupon kodunu girin"
                value={couponCode}
                onChangeText={setCouponCode}
                autoCapitalize="characters"
                placeholderTextColor={Colors.textLight}
              />
              <TouchableOpacity 
                style={[styles.applyButton, couponLoading && styles.applyButtonDisabled]}
                onPress={handleApplyCoupon}
                disabled={couponLoading}
              >
                {couponLoading ? (
                  <ActivityIndicator size="small" color="#FFF" />
                ) : (
                  <Text style={styles.applyButtonText}>Uygula</Text>
                )}
              </TouchableOpacity>
            </View>
          )}
          
          {couponError ? (
            <Text style={styles.couponError}>{couponError}</Text>
          ) : null}
        </View>

        <View style={styles.summaryContainer}>
          <Text style={styles.summaryTitle}>Sipariş Özeti</Text>
          <View style={styles.summaryRow}>
            <Text style={styles.summaryLabel}>Ara Toplam</Text>
            <Text style={styles.summaryValue}>₺{subtotal.toFixed(2)}</Text>
          </View>
          {finalDiscount > 0 && (
            <View style={styles.summaryRow}>
              <Text style={[styles.summaryLabel, styles.discountLabel]}>İndirim</Text>
              <Text style={[styles.summaryValue, styles.discountValue]}>-₺{finalDiscount.toFixed(2)}</Text>
            </View>
          )}
          <View style={styles.summaryRow}>
            <Text style={styles.summaryLabel}>Teslimat Ücreti</Text>
            <Text style={styles.summaryValue}>₺{deliveryFee.toFixed(2)}</Text>
          </View>
          <View style={styles.summaryDivider} />
          <View style={styles.summaryRow}>
            <Text style={styles.totalLabel}>Toplam</Text>
            <Text style={styles.totalValue}>₺{finalTotal.toFixed(2)}</Text>
          </View>
        </View>
      </ScrollView>

      <View style={styles.bottomBar}>
        <View style={styles.bottomInfo}>
          <Text style={styles.bottomTotal}>₺{finalTotal.toFixed(2)}</Text>
          <Text style={styles.bottomTotalLabel}>{cart.totalQuantity} ürün</Text>
        </View>
        <TouchableOpacity
          style={styles.checkoutButton}
          onPress={() => navigation.navigate('Checkout')}
          activeOpacity={0.8}
        >
          <Text style={styles.checkoutButtonText}>Siparişi Onayla</Text>
          <Icon name="arrow-right" size={20} color="#FFF" />
        </TouchableOpacity>
      </View>
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
  clearButton: {
    width: 44,
    height: 44,
    borderRadius: 22,
    backgroundColor: '#FFEBEE',
    justifyContent: 'center',
    alignItems: 'center',
  },
  scrollContent: {
    paddingBottom: 120,
  },
  restaurantInfo: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: Colors.surface,
    marginHorizontal: Spacing.base,
    marginTop: Spacing.md,
    padding: Spacing.base,
    borderRadius: BorderRadius.lg,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 1},
    shadowOpacity: 0.04,
    shadowRadius: 4,
    elevation: 2,
  },
  restaurantIcon: {
    width: 44,
    height: 44,
    borderRadius: 12,
    backgroundColor: Colors.primary + '12',
    justifyContent: 'center',
    alignItems: 'center',
  },
  restaurantTextContainer: {
    flex: 1,
    marginLeft: Spacing.md,
  },
  restaurantName: {
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
  },
  itemCountText: {
    fontSize: Fonts.sizes.sm,
    color: Colors.textSecondary,
    marginTop: 2,
  },
  addMoreButton: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: Colors.primary + '12',
    paddingHorizontal: 12,
    paddingVertical: 8,
    borderRadius: 10,
  },
  addMoreText: {
    fontSize: Fonts.sizes.sm,
    color: Colors.primary,
    fontWeight: Fonts.weights.bold,
    marginLeft: 4,
  },
  itemsContainer: {
    backgroundColor: Colors.surface,
    marginHorizontal: Spacing.base,
    marginTop: Spacing.md,
    borderRadius: BorderRadius.lg,
    overflow: 'hidden',
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 1},
    shadowOpacity: 0.04,
    shadowRadius: 4,
    elevation: 2,
  },
  cartItem: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    padding: Spacing.base,
    borderBottomWidth: 1,
    borderBottomColor: Colors.borderLight,
  },
  cartItemLeft: {
    flexDirection: 'row',
    alignItems: 'center',
    flex: 1,
    marginRight: Spacing.md,
  },
  itemImage: {
    width: 56,
    height: 56,
    borderRadius: 12,
  },
  itemImagePlaceholder: {
    width: 56,
    height: 56,
    borderRadius: 12,
    backgroundColor: Colors.borderLight,
    justifyContent: 'center',
    alignItems: 'center',
  },
  itemDetails: {
    flex: 1,
    marginLeft: Spacing.md,
  },
  itemName: {
    fontSize: Fonts.sizes.md,
    fontWeight: Fonts.weights.semibold,
    color: Colors.text,
    marginBottom: 2,
  },
  itemOptionsRow: {
    flexDirection: 'row',
    alignItems: 'flex-start',
    marginBottom: 4,
  },
  itemOptions: {
    fontSize: Fonts.sizes.xs,
    color: Colors.textSecondary,
    flex: 1,
  },
  expandIcon: {
    marginLeft: 2,
    marginTop: 1,
  },
  itemPrice: {
    fontSize: Fonts.sizes.md,
    fontWeight: Fonts.weights.bold,
    color: Colors.primary,
  },
  quantityControl: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: Colors.borderLight,
    borderRadius: 12,
    padding: 2,
  },
  quantityButton: {
    width: 32,
    height: 32,
    borderRadius: 10,
    backgroundColor: Colors.surface,
    justifyContent: 'center',
    alignItems: 'center',
  },
  quantityButtonPlus: {
    backgroundColor: Colors.primary,
  },
  quantityText: {
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
    marginHorizontal: 12,
    minWidth: 20,
    textAlign: 'center',
  },
  noteContainer: {
    flexDirection: 'row',
    alignItems: 'flex-start',
    backgroundColor: Colors.surface,
    marginHorizontal: Spacing.base,
    marginTop: Spacing.md,
    padding: Spacing.base,
    borderRadius: BorderRadius.lg,
    borderWidth: 1,
    borderColor: Colors.border,
    borderStyle: 'dashed',
  },
  noteIcon: {
    marginTop: 4,
    marginRight: Spacing.sm,
  },
  noteInput: {
    flex: 1,
    fontSize: Fonts.sizes.md,
    color: Colors.text,
    padding: 0,
    minHeight: 40,
    textAlignVertical: 'top',
  },
  summaryContainer: {
    backgroundColor: Colors.surface,
    marginHorizontal: Spacing.base,
    marginTop: Spacing.md,
    padding: Spacing.lg,
    borderRadius: BorderRadius.lg,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 1},
    shadowOpacity: 0.04,
    shadowRadius: 4,
    elevation: 2,
  },
  summaryTitle: {
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
    marginBottom: Spacing.md,
  },
  summaryRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: Spacing.sm,
  },
  summaryLabel: {
    fontSize: Fonts.sizes.md,
    color: Colors.textSecondary,
  },
  summaryValue: {
    fontSize: Fonts.sizes.md,
    color: Colors.text,
    fontWeight: Fonts.weights.medium,
  },
  summaryDivider: {
    height: 1,
    backgroundColor: Colors.border,
    marginVertical: Spacing.md,
  },
  totalLabel: {
    fontSize: Fonts.sizes.lg,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
  },
  totalValue: {
    fontSize: Fonts.sizes.xl,
    fontWeight: Fonts.weights.heavy,
    color: Colors.primary,
  },
  bottomBar: {
    position: 'absolute',
    bottom: 0,
    left: 0,
    right: 0,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    backgroundColor: Colors.surface,
    paddingHorizontal: Spacing.lg,
    paddingTop: Spacing.md,
    paddingBottom: 34,
    borderTopWidth: 1,
    borderTopColor: Colors.borderLight,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: -4},
    shadowOpacity: 0.06,
    shadowRadius: 8,
    elevation: 10,
  },
  bottomInfo: {
    marginRight: Spacing.md,
  },
  bottomTotal: {
    fontSize: Fonts.sizes.xl,
    fontWeight: Fonts.weights.heavy,
    color: Colors.text,
  },
  bottomTotalLabel: {
    fontSize: Fonts.sizes.xs,
    color: Colors.textSecondary,
    marginTop: 2,
  },
  checkoutButton: {
    flex: 1,
    flexDirection: 'row',
    backgroundColor: Colors.primary,
    borderRadius: 14,
    paddingVertical: 16,
    justifyContent: 'center',
    alignItems: 'center',
    shadowColor: Colors.primary,
    shadowOffset: {width: 0, height: 4},
    shadowOpacity: 0.3,
    shadowRadius: 8,
    elevation: 6,
  },
  checkoutButtonText: {
    color: '#FFF',
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.bold,
    marginRight: 8,
  },
  // Coupon styles
  couponSection: {
    backgroundColor: Colors.surface,
    marginHorizontal: Spacing.base,
    marginTop: Spacing.md,
    padding: Spacing.base,
    borderRadius: BorderRadius.lg,
  },
  couponTitle: {
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
    marginBottom: Spacing.sm,
  },
  couponInputContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: Spacing.sm,
  },
  couponInput: {
    flex: 1,
    borderWidth: 1,
    borderColor: Colors.border,
    borderRadius: 10,
    paddingHorizontal: Spacing.md,
    paddingVertical: 10,
    fontSize: Fonts.sizes.md,
    color: Colors.text,
    backgroundColor: Colors.background,
  },
  applyButton: {
    backgroundColor: Colors.primary,
    paddingHorizontal: Spacing.md,
    paddingVertical: 10,
    borderRadius: 10,
    minWidth: 80,
    alignItems: 'center',
  },
  applyButtonDisabled: {
    opacity: 0.6,
  },
  applyButtonText: {
    color: '#FFF',
    fontSize: Fonts.sizes.sm,
    fontWeight: Fonts.weights.bold,
  },
  couponError: {
    color: Colors.error,
    fontSize: Fonts.sizes.xs,
    marginTop: Spacing.xs,
  },
  appliedCouponContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    backgroundColor: Colors.success + '10',
    padding: Spacing.sm,
    borderRadius: 10,
    borderWidth: 1,
    borderColor: Colors.success + '30',
  },
  appliedCouponInfo: {
    flexDirection: 'row',
    alignItems: 'center',
    flex: 1,
  },
  appliedCouponText: {
    marginLeft: Spacing.sm,
    flex: 1,
  },
  appliedCouponCode: {
    fontSize: Fonts.sizes.md,
    fontWeight: Fonts.weights.bold,
    color: Colors.success,
  },
  appliedCouponDesc: {
    fontSize: Fonts.sizes.xs,
    color: Colors.textSecondary,
    marginTop: 2,
  },
  removeCouponButton: {
    padding: 4,
  },
  discountLabel: {
    color: Colors.success,
  },
  discountValue: {
    color: Colors.success,
    fontWeight: Fonts.weights.bold,
  },
});

export default CartScreen;
