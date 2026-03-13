import React, {useState, useEffect} from 'react';
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  TouchableOpacity,
  Alert,
  ActivityIndicator,
  TextInput,
} from 'react-native';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import {Colors, Fonts, Spacing, BorderRadius} from '../../theme';
import {useCart} from '../../context/CartContext';
import {useAuth} from '../../context/AuthContext';
import {orderService, addressService} from '../../api';
import {PAYMENT_OPTIONS} from '../../utils/constants';

const CheckoutScreen = ({navigation}) => {
  const {cart, clearCart} = useCart();
  const {user} = useAuth();
  const [addresses, setAddresses] = useState([]);
  const [selectedAddress, setSelectedAddress] = useState(null);
  const [selectedPayment, setSelectedPayment] = useState(1);
  const [notes, setNotes] = useState('');
  const [loading, setLoading] = useState(false);
  const [addressLoading, setAddressLoading] = useState(true);

  useEffect(() => {
    loadAddresses();
  }, []);

  const loadAddresses = async () => {
    try {
      const res = await addressService.getList();
      if (res.data?.rawData) {
        const addrs = res.data.rawData;
        setAddresses(addrs);
        const def = addrs.find(a => a.isDefault) || addrs[0];
        if (def) setSelectedAddress(def.id);
      }
    } catch (e) {
      console.log('Address load error:', e);
    } finally {
      setAddressLoading(false);
    }
  };

  const handlePlaceOrder = async () => {
    if (!cart || cart.items.length === 0) {
      Alert.alert('Hata', 'Sepetiniz boş');
      return;
    }
    if (!selectedAddress) {
      Alert.alert('Hata', 'Lütfen teslimat adresi seçiniz');
      return;
    }

    setLoading(true);
    try {
      const orderData = {
        restaurantId: cart.restaurantId,
        deliveryAddressId: selectedAddress,
        invoiceAddressId: selectedAddress,
        paymentOptionId: selectedPayment,
        notes: notes.trim() || null,
        items: cart.items.map(item => ({
          menuId: item.menuId,
          quantity: item.quantity,
          values: item.values || [],
        })),
      };

      const res = await orderService.placeOrder(orderData);
      if (!res.data.hasFailed) {
        clearCart();
        const orderId = res.data.data;
        Alert.alert(
          'Sipariş Alındı! 🎉',
          'Siparişiniz başarıyla oluşturuldu.',
          [
            {
              text: 'Siparişi Takip Et',
              onPress: () => navigation.replace('OrderDetail', {orderId}),
            },
            {
              text: 'Ana Sayfa',
              onPress: () => navigation.navigate('HomeTab'),
            },
          ],
        );
      } else {
        const errorMsg = res.data.messages?.[0]?.description || 'Sipariş oluşturulamadı';
        Alert.alert('Hata', errorMsg);
      }
    } catch (e) {
      Alert.alert('Hata', 'Sipariş oluşturulurken bir hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  if (!cart) {
    navigation.goBack();
    return null;
  }

  const deliveryFee = 9.99;
  const subtotal = cart.totalPrice;
  const total = subtotal + deliveryFee;
  const selectedAddr = addresses.find(a => a.id === selectedAddress);

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <TouchableOpacity style={styles.backButton} onPress={() => navigation.goBack()}>
          <Icon name="arrow-left" size={24} color={Colors.text} />
        </TouchableOpacity>
        <Text style={styles.headerTitle}>Sipariş Onayı</Text>
        <View style={{width: 44}} />
      </View>

      <ScrollView showsVerticalScrollIndicator={false} contentContainerStyle={styles.scrollContent}>
        <View style={styles.section}>
          <View style={styles.sectionHeader}>
            <Icon name="map-marker-outline" size={22} color={Colors.primary} />
            <Text style={styles.sectionTitle}>Teslimat Adresi</Text>
          </View>
          {addressLoading ? (
            <ActivityIndicator color={Colors.primary} style={{padding: 20}} />
          ) : addresses.length === 0 ? (
            <TouchableOpacity
              style={styles.addAddressButton}
              onPress={() => navigation.navigate('AddAddress')}
            >
              <Icon name="plus-circle-outline" size={24} color={Colors.primary} />
              <Text style={styles.addAddressText}>Adres Ekle</Text>
            </TouchableOpacity>
          ) : (
            addresses.map(addr => (
              <TouchableOpacity
                key={addr.id}
                style={[styles.addressCard, selectedAddress === addr.id && styles.addressCardSelected]}
                onPress={() => setSelectedAddress(addr.id)}
                activeOpacity={0.8}
              >
                <View style={styles.radioOuter}>
                  {selectedAddress === addr.id && <View style={styles.radioInner} />}
                </View>
                <View style={styles.addressInfo}>
                  <Text style={styles.addressName}>{addr.addressName}</Text>
                  <Text style={styles.addressLine} numberOfLines={2}>{addr.addressLine1}</Text>
                </View>
                {addr.isDefault && (
                  <View style={styles.defaultBadge}>
                    <Text style={styles.defaultBadgeText}>Varsayılan</Text>
                  </View>
                )}
              </TouchableOpacity>
            ))
          )}
        </View>

        <View style={styles.section}>
          <View style={styles.sectionHeader}>
            <Icon name="credit-card-outline" size={22} color={Colors.primary} />
            <Text style={styles.sectionTitle}>Ödeme Yöntemi</Text>
          </View>
          {Object.entries(PAYMENT_OPTIONS).map(([key, option]) => {
            const payId = parseInt(key, 10);
            const isSelected = selectedPayment === payId;
            return (
              <TouchableOpacity
                key={key}
                style={[styles.paymentCard, isSelected && styles.paymentCardSelected]}
                onPress={() => setSelectedPayment(payId)}
                activeOpacity={0.8}
              >
                <View style={styles.radioOuter}>
                  {isSelected && <View style={styles.radioInner} />}
                </View>
                <Icon name={option.icon} size={24} color={isSelected ? Colors.primary : Colors.textSecondary} style={{marginLeft: 12}} />
                <Text style={[styles.paymentLabel, isSelected && styles.paymentLabelSelected]}>
                  {option.label}
                </Text>
              </TouchableOpacity>
            );
          })}
        </View>

        <View style={styles.section}>
          <View style={styles.sectionHeader}>
            <Icon name="note-text-outline" size={22} color={Colors.primary} />
            <Text style={styles.sectionTitle}>Sipariş Notu</Text>
          </View>
          <TextInput
            style={styles.noteInput}
            placeholder="Sipariş notunuzu buraya yazabilirsiniz..."
            placeholderTextColor={Colors.textTertiary}
            value={notes}
            onChangeText={setNotes}
            multiline
            numberOfLines={3}
            textAlignVertical="top"
          />
        </View>

        <View style={styles.summaryContainer}>
          <Text style={styles.summaryTitle}>Sipariş Özeti</Text>
          {cart.items.map(item => (
            <View key={item.cartItemId} style={styles.summaryItem}>
              <Text style={styles.summaryItemQty}>{item.quantity}x</Text>
              <Text style={styles.summaryItemName} numberOfLines={1}>{item.menuName}</Text>
              <Text style={styles.summaryItemPrice}>₺{item.totalPrice.toFixed(2)}</Text>
            </View>
          ))}
          <View style={styles.summaryDivider} />
          <View style={styles.summaryRow}>
            <Text style={styles.summaryLabel}>Ara Toplam</Text>
            <Text style={styles.summaryValue}>₺{subtotal.toFixed(2)}</Text>
          </View>
          <View style={styles.summaryRow}>
            <Text style={styles.summaryLabel}>Teslimat Ücreti</Text>
            <Text style={styles.summaryValue}>₺{deliveryFee.toFixed(2)}</Text>
          </View>
          <View style={styles.summaryDivider} />
          <View style={styles.summaryRow}>
            <Text style={styles.totalLabel}>Toplam</Text>
            <Text style={styles.totalValue}>₺{total.toFixed(2)}</Text>
          </View>
        </View>
      </ScrollView>

      <View style={styles.bottomBar}>
        <TouchableOpacity
          style={[styles.placeOrderButton, loading && styles.buttonDisabled]}
          onPress={handlePlaceOrder}
          activeOpacity={0.8}
          disabled={loading}
        >
          {loading ? (
            <ActivityIndicator color="#FFF" />
          ) : (
            <>
              <Icon name="check-circle" size={22} color="#FFF" style={{marginRight: 8}} />
              <Text style={styles.placeOrderText}>Siparişi Ver — ₺{total.toFixed(2)}</Text>
            </>
          )}
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
  scrollContent: {
    paddingBottom: 120,
  },
  section: {
    backgroundColor: Colors.surface,
    marginHorizontal: Spacing.base,
    marginTop: Spacing.md,
    borderRadius: BorderRadius.lg,
    padding: Spacing.base,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 1},
    shadowOpacity: 0.04,
    shadowRadius: 4,
    elevation: 2,
  },
  sectionHeader: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: Spacing.md,
  },
  sectionTitle: {
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
    marginLeft: 8,
  },
  addressCard: {
    flexDirection: 'row',
    alignItems: 'center',
    padding: Spacing.md,
    borderRadius: BorderRadius.md,
    borderWidth: 1.5,
    borderColor: Colors.border,
    marginBottom: Spacing.sm,
  },
  addressCardSelected: {
    borderColor: Colors.primary,
    backgroundColor: Colors.primary + '08',
  },
  radioOuter: {
    width: 22,
    height: 22,
    borderRadius: 11,
    borderWidth: 2,
    borderColor: Colors.border,
    justifyContent: 'center',
    alignItems: 'center',
  },
  radioInner: {
    width: 12,
    height: 12,
    borderRadius: 6,
    backgroundColor: Colors.primary,
  },
  addressInfo: {
    flex: 1,
    marginLeft: Spacing.md,
  },
  addressName: {
    fontSize: Fonts.sizes.md,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
    marginBottom: 2,
  },
  addressLine: {
    fontSize: Fonts.sizes.sm,
    color: Colors.textSecondary,
  },
  defaultBadge: {
    backgroundColor: Colors.primary + '15',
    paddingHorizontal: 8,
    paddingVertical: 4,
    borderRadius: 6,
  },
  defaultBadgeText: {
    fontSize: Fonts.sizes.xs,
    color: Colors.primary,
    fontWeight: Fonts.weights.semibold,
  },
  addAddressButton: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    padding: Spacing.lg,
    borderRadius: BorderRadius.md,
    borderWidth: 1.5,
    borderColor: Colors.primary,
    borderStyle: 'dashed',
  },
  addAddressText: {
    fontSize: Fonts.sizes.base,
    color: Colors.primary,
    fontWeight: Fonts.weights.bold,
    marginLeft: 8,
  },
  paymentCard: {
    flexDirection: 'row',
    alignItems: 'center',
    padding: Spacing.md,
    borderRadius: BorderRadius.md,
    borderWidth: 1.5,
    borderColor: Colors.border,
    marginBottom: Spacing.sm,
  },
  paymentCardSelected: {
    borderColor: Colors.primary,
    backgroundColor: Colors.primary + '08',
  },
  paymentLabel: {
    fontSize: Fonts.sizes.md,
    color: Colors.textSecondary,
    fontWeight: Fonts.weights.medium,
    marginLeft: 12,
  },
  paymentLabelSelected: {
    color: Colors.text,
    fontWeight: Fonts.weights.bold,
  },
  noteInput: {
    backgroundColor: Colors.borderLight,
    borderRadius: BorderRadius.md,
    padding: Spacing.md,
    fontSize: Fonts.sizes.md,
    color: Colors.text,
    minHeight: 80,
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
  summaryItem: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: 8,
  },
  summaryItemQty: {
    fontSize: Fonts.sizes.sm,
    fontWeight: Fonts.weights.bold,
    color: Colors.primary,
    width: 28,
  },
  summaryItemName: {
    flex: 1,
    fontSize: Fonts.sizes.sm,
    color: Colors.text,
  },
  summaryItemPrice: {
    fontSize: Fonts.sizes.sm,
    fontWeight: Fonts.weights.semibold,
    color: Colors.text,
    marginLeft: 8,
  },
  summaryDivider: {
    height: 1,
    backgroundColor: Colors.border,
    marginVertical: Spacing.md,
  },
  summaryRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginBottom: 8,
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
    backgroundColor: Colors.surface,
    paddingHorizontal: Spacing.base,
    paddingTop: Spacing.md,
    paddingBottom: 34,
    borderTopWidth: 1,
    borderTopColor: Colors.borderLight,
  },
  placeOrderButton: {
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
  buttonDisabled: {
    opacity: 0.7,
  },
  placeOrderText: {
    color: '#FFF',
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.bold,
  },
});

export default CheckoutScreen;
