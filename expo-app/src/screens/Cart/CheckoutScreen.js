import React, {useState, useEffect} from 'react';
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  TouchableOpacity,
  ActivityIndicator,
  TextInput,
  Platform,
} from 'react-native';
import {useSafeAreaInsets} from 'react-native-safe-area-context';
import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import {Colors, Fonts, Spacing, BorderRadius} from '../../theme';
import {useCart} from '../../context/CartContext';
import {useAuth} from '../../context/AuthContext';
import {useToast} from '../../context/ToastContext';
import {orderService} from '../../api';
import {couponService} from '../../api/couponService';
import {cardService} from '../../api/cardService';
import {PAYMENT_OPTIONS} from '../../utils/constants';

import { router } from 'expo-router';

const CheckoutScreen = () => {
  const {cart, clearCart, appliedCoupon, discountAmount, applyCoupon, removeCoupon, orderNote, setOrderNote} = useCart();
  const {user} = useAuth();
  const {showToast} = useToast();
  const insets = useSafeAreaInsets();
  const [selectedPayment, setSelectedPayment] = useState(1);
  const [paymentDropdownOpen, setPaymentDropdownOpen] = useState(false);
  const [loading, setLoading] = useState(false);
  const [expandedItems, setExpandedItems] = useState({});
  const [couponCode, setCouponCode] = useState('');
  const [couponLoading, setCouponLoading] = useState(false);
  const [couponError, setCouponError] = useState('');
  // Card states
  const [savedCards, setSavedCards] = useState([]);
  const [cardsLoading, setCardsLoading] = useState(false);
  const [selectedCardToken, setSelectedCardToken] = useState(null);
  const [showNewCardForm, setShowNewCardForm] = useState(false);
  const [saveCard, setSaveCard] = useState(true);
  const [cardAlias, setCardAlias] = useState('');
  const [cardInfo, setCardInfo] = useState({
    cardHolderName: '',
    cardNumber: '',
    expireMonth: '',
    expireYear: '',
    cvc: '',
  });

  useEffect(() => {
    loadSavedCards();
  }, []);

  const loadSavedCards = async () => {
    setCardsLoading(true);
    try {
      const res = await cardService.getCards();
      if (res.data && !res.data.hasFailed && res.data.data) {
        const cards = res.data.data;
        setSavedCards(cards);
        if (cards.length > 0) {
          setSelectedCardToken(cards[0].cardToken);
          setShowNewCardForm(false);
        } else {
          setShowNewCardForm(true);
        }
      } else {
        setShowNewCardForm(true);
      }
    } catch (e) {
      setShowNewCardForm(true);
    } finally {
      setCardsLoading(false);
    }
  };

  const handleDeleteCard = async (cardToken) => {
    try {
      const res = await cardService.deleteCard(cardToken);
      if (res.data && !res.data.hasFailed) {
        const updated = savedCards.filter(c => c.cardToken !== cardToken);
        setSavedCards(updated);
        if (selectedCardToken === cardToken) {
          if (updated.length > 0) {
            setSelectedCardToken(updated[0].cardToken);
          } else {
            setSelectedCardToken(null);
            setShowNewCardForm(true);
          }
        }
        showToast('Kart silindi', 'success');
      } else {
        showToast(res.data?.messages?.[0]?.description || 'Kart silinemedi', 'error');
      }
    } catch (e) {
      showToast('Kart silinirken hata olu\u015ftu', 'error');
    }
  };

  const handleSaveNewCard = async () => {
    if (!cardInfo.cardHolderName.trim() || cardInfo.cardNumber.replace(/\s/g, '').length < 15 || !cardInfo.expireMonth || !cardInfo.expireYear) {
      showToast('Kart bilgilerini eksiksiz giriniz', 'warning');
      return;
    }
    try {
      const res = await cardService.createCard({
        cardAlias: cardInfo.cardHolderName.trim(),
        cardNumber: cardInfo.cardNumber.replace(/\s/g, ''),
        expireYear: '20' + cardInfo.expireYear,
        expireMonth: cardInfo.expireMonth,
        cardHolderName: cardInfo.cardHolderName.trim(),
      });
      if (res.data && !res.data.hasFailed) {
        showToast('Kart kaydedildi', 'success');
        await loadSavedCards();
        setCardInfo({cardHolderName: '', cardNumber: '', expireMonth: '', expireYear: '', cvc: ''});
      } else {
        showToast(res.data?.messages?.[0]?.description || 'Kart kaydedilemedi', 'error');
      }
    } catch (e) {
      showToast('Kart kaydedilirken hata olu\u015ftu', 'error');
    }
  };

  const updateCardField = (field, value) => {
    setCardInfo(prev => ({...prev, [field]: value}));
  };

  const formatCardNumber = (text) => {
    const cleaned = text.replace(/\D/g, '').slice(0, 16);
    return cleaned.replace(/(\d{4})(?=\d)/g, '$1 ');
  };

  const getCardIcon = (association) => {
    if (!association) return 'credit-card-outline';
    const a = association.toUpperCase();
    if (a.includes('VISA')) return 'credit-card-outline';
    if (a.includes('MASTER')) return 'credit-card-outline';
    return 'credit-card-outline';
  };

  const formatOptionsSummary = (values) => {
    if (!values || values.length === 0) return null;
    const parts = [];
    values.forEach(val => {
      parts.push(val.valueName);
      if (val.valueOptionValues && val.valueOptionValues.length > 0) {
        val.valueOptionValues.forEach(voov => {
          parts.push(voov.name);
        });
      }
    });
    return parts.join('\n');
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
    } finally {
      setCouponLoading(false);
    }
  };

  const handlePlaceOrder = async () => {
    if (!cart || cart.items.length === 0) {
      showToast('Sepetiniz boş', 'error');
      return;
    }
    if (selectedPayment === 1) {
      if (selectedCardToken && !showNewCardForm) {
        // Saved card selected — ok
      } else {
        if (!cardInfo.cardHolderName.trim()) {
          showToast('Kart üzerindeki ismi giriniz', 'warning');
          return;
        }
        if (cardInfo.cardNumber.replace(/\s/g, '').length < 15) {
          showToast('Geçerli bir kart numarası giriniz', 'warning');
          return;
        }
        if (!cardInfo.expireMonth || !cardInfo.expireYear) {
          showToast('Son kullanma tarihini giriniz', 'warning');
          return;
        }
        if (!cardInfo.cvc || cardInfo.cvc.length < 3) {
          showToast('CVC kodunu giriniz', 'warning');
          return;
        }
      }
    }

    setLoading(true);
    try {
      const orderData = {
        restaurantId: cart.restaurantId,
        paymentOptionId: selectedPayment,
        notes: orderNote.trim() || null,
        couponId: appliedCoupon?.id || null,
        couponCode: appliedCoupon?.code || null,
        discountAmount: discountAmount || 0,
        items: cart.items.map(item => ({
          menuId: item.menuId,
          quantity: item.quantity,
          values: (item.values || []).map(v => ({
            menuOptionId: v.optionId || v.menuOptionId,
            menuOptionValueId: v.valueId || v.menuOptionValueId,
            productId: v.productId || null,
            quantity: v.quantity || 1,
            options: (v.valueOptionValues || []).map(vov => ({
              menuOptionValueOptionId: vov.menuOptionValueOptionId,
              menuOptionValueOptionValueId: vov.id || vov.menuOptionValueOptionValueId,
              productId: vov.productId || null,
              quantity: vov.quantity || 1,
            })),
          })),
        })),
      };
      if (selectedPayment === 1) {
        if (selectedCardToken && !showNewCardForm) {
          orderData.cardToken = selectedCardToken;
        } else {
          orderData.cardHolderName = cardInfo.cardHolderName.trim();
          orderData.cardNumber = cardInfo.cardNumber.replace(/\s/g, '');
          orderData.expireMonth = cardInfo.expireMonth;
          orderData.expireYear = cardInfo.expireYear;
          orderData.cvc = cardInfo.cvc;
          if (saveCard) {
            orderData.saveCard = true;
            orderData.cardAlias = cardAlias.trim() || `Kartım ${savedCards.length + 1}`;
          }
        }
      }

      const res = await orderService.placeOrder(orderData);
      if (!res.data.hasFailed) {
        const result = res.data.data;

        if (result.requiresPayment && result.requiresThreeDs && result.threeDsHtmlContent) {
          // 3DS doğrulama gerekiyor — WebView'a yönlendir
          clearCart();
          router.replace({ pathname: '/three-ds', params: { htmlContent: result.threeDsHtmlContent, orderId: result.orderId } });
        } else if (result.requiresPayment && result.paymentError) {
          // Sipariş oluşturuldu ama ödeme başlatılamadı
          showToast(`Sipariş oluşturuldu ama ödeme başlatılamadı: ${result.paymentError}`, 'warning');
        } else {
          // Kapıda ödeme veya başarılı
          clearCart();
          showToast('Siparişiniz başarıyla oluşturuldu!', 'success');
          router.replace(`/order/${result.orderId}`);
        }
      } else {
        const errorMsg = res.data.messages?.[0]?.description || 'Sipariş oluşturulamadı';
        showToast(errorMsg, 'error');
      }
    } catch (e) {
      showToast('Sipariş oluşturulurken bir hata oluştu', 'error');
    } finally {
      setLoading(false);
    }
  };

  if (!cart) {
    router.back();
    return null;
  }

  const deliveryFee = 9.99;
  const subtotal = cart.totalPrice;
  const finalDiscount = discountAmount || 0;
  const total = Math.max(0, subtotal - finalDiscount + deliveryFee);
  const selectedPaymentOption = PAYMENT_OPTIONS[selectedPayment];
  const bottomPadding = Math.max(insets.bottom, 8) + 56 + 16;

  return (
    <View style={styles.container}>
      <View style={[styles.header, {paddingTop: insets.top + 10}]}>
        <TouchableOpacity style={styles.backButton} onPress={() => router.back()}>
          <Icon name="arrow-left" size={24} color={Colors.text} />
        </TouchableOpacity>
        <Text style={styles.headerTitle}>Sipariş Onayı</Text>
        <View style={{width: 44}} />
      </View>

      <ScrollView showsVerticalScrollIndicator={false} contentContainerStyle={[styles.scrollContent, {paddingBottom: bottomPadding + 80}]}>
        {/* Ödeme Yöntemi - Selectbox */}
        <View style={styles.section}>
          <View style={styles.sectionHeader}>
            <Icon name="credit-card-outline" size={22} color={Colors.primary} />
            <Text style={styles.sectionTitle}>Ödeme Yöntemi</Text>
          </View>
          <TouchableOpacity
            style={styles.selectBox}
            onPress={() => setPaymentDropdownOpen(!paymentDropdownOpen)}
            activeOpacity={0.8}
          >
            <Icon name={selectedPaymentOption?.icon || 'credit-card-outline'} size={22} color={Colors.primary} />
            <Text style={styles.selectBoxText}>{selectedPaymentOption?.label || 'Seçiniz'}</Text>
            <Icon name={paymentDropdownOpen ? 'chevron-up' : 'chevron-down'} size={22} color={Colors.textSecondary} />
          </TouchableOpacity>
          {paymentDropdownOpen && (
            <View style={styles.dropdownList}>
              {Object.entries(PAYMENT_OPTIONS).map(([key, option]) => {
                const payId = parseInt(key, 10);
                const isSelected = selectedPayment === payId;
                return (
                  <TouchableOpacity
                    key={key}
                    style={[styles.dropdownItem, isSelected && styles.dropdownItemSelected]}
                    onPress={() => {
                      setSelectedPayment(payId);
                      setPaymentDropdownOpen(false);
                    }}
                    activeOpacity={0.7}
                  >
                    <Icon name={option.icon} size={20} color={isSelected ? Colors.primary : Colors.textSecondary} />
                    <Text style={[styles.dropdownItemText, isSelected && styles.dropdownItemTextSelected]}>{option.label}</Text>
                    {isSelected && <Icon name="check" size={18} color={Colors.primary} />}
                  </TouchableOpacity>
                );
              })}
            </View>
          )}
          {selectedPayment === 1 && (
            <View style={styles.cardForm}>
              {cardsLoading ? (
                <ActivityIndicator color={Colors.primary} style={{padding: 16}} />
              ) : (
                <>
                  {savedCards.length > 0 && (
                    <View>
                      <Text style={styles.cardFormTitle}>Kayıtlı Kartlar</Text>
                      {savedCards.map(card => (
                        <TouchableOpacity
                          key={card.cardToken}
                          style={[styles.savedCard, selectedCardToken === card.cardToken && !showNewCardForm && styles.savedCardSelected]}
                          onPress={() => {
                            setSelectedCardToken(card.cardToken);
                            setShowNewCardForm(false);
                          }}
                          activeOpacity={0.7}
                        >
                          <View style={[styles.radioOuter, selectedCardToken === card.cardToken && !showNewCardForm && styles.radioOuterSelected]}>
                            {selectedCardToken === card.cardToken && !showNewCardForm && <View style={styles.radioInner} />}
                          </View>
                          <Icon name={getCardIcon(card.cardAssociation)} size={24} color={Colors.primary} style={{marginLeft: 10}} />
                          <View style={{flex: 1, marginLeft: 10}}>
                            <Text style={styles.savedCardName}>
                              {card.cardAlias || card.cardFamily || card.cardAssociation || 'Kart'} *{card.lastFourDigits}
                            </Text>
                            <Text style={styles.savedCardDetail}>
                              {card.cardBankName || ''} {card.expireMonth}/{card.expireYear?.slice(-2)}
                            </Text>
                          </View>
                          <TouchableOpacity
                            onPress={() => handleDeleteCard(card.cardToken)}
                            hitSlop={{top: 10, bottom: 10, left: 10, right: 10}}
                          >
                            <Icon name="trash-can-outline" size={18} color={Colors.error} />
                          </TouchableOpacity>
                        </TouchableOpacity>
                      ))}
                    </View>
                  )}

                  <TouchableOpacity
                    style={[styles.newCardToggle, showNewCardForm && styles.newCardToggleActive]}
                    onPress={() => {
                      setShowNewCardForm(true);
                      setSelectedCardToken(null);
                    }}
                    activeOpacity={0.7}
                  >
                    <View style={[styles.radioOuter, showNewCardForm && styles.radioOuterSelected]}>
                      {showNewCardForm && <View style={styles.radioInner} />}
                    </View>
                    <Icon name="plus-circle-outline" size={22} color={Colors.primary} style={{marginLeft: 10}} />
                    <Text style={styles.newCardToggleText}>Yeni Kart ile Öde</Text>
                  </TouchableOpacity>

                  {showNewCardForm && (
                    <View style={{marginTop: Spacing.sm}}>
                      <TextInput
                        style={styles.cardInput}
                        placeholder="Kart Üzerindeki İsim"
                        placeholderTextColor={Colors.textTertiary}
                        value={cardInfo.cardHolderName}
                        onChangeText={(t) => updateCardField('cardHolderName', t)}
                        autoCapitalize="characters"
                      />
                      <TextInput
                        style={styles.cardInput}
                        placeholder="Kart Numarası"
                        placeholderTextColor={Colors.textTertiary}
                        value={cardInfo.cardNumber}
                        onChangeText={(t) => updateCardField('cardNumber', formatCardNumber(t))}
                        keyboardType="numeric"
                        maxLength={19}
                      />
                      <View style={styles.cardRow}>
                        <TextInput
                          style={[styles.cardInput, styles.cardInputSmall]}
                          placeholder="Ay (MM)"
                          placeholderTextColor={Colors.textTertiary}
                          value={cardInfo.expireMonth}
                          onChangeText={(t) => updateCardField('expireMonth', t.replace(/\D/g, '').slice(0, 2))}
                          keyboardType="numeric"
                          maxLength={2}
                        />
                        <TextInput
                          style={[styles.cardInput, styles.cardInputSmall]}
                          placeholder="Yıl (YY)"
                          placeholderTextColor={Colors.textTertiary}
                          value={cardInfo.expireYear}
                          onChangeText={(t) => updateCardField('expireYear', t.replace(/\D/g, '').slice(0, 2))}
                          keyboardType="numeric"
                          maxLength={2}
                        />
                        <TextInput
                          style={[styles.cardInput, styles.cardInputSmall]}
                          placeholder="CVC"
                          placeholderTextColor={Colors.textTertiary}
                          value={cardInfo.cvc}
                          onChangeText={(t) => updateCardField('cvc', t.replace(/\D/g, '').slice(0, 4))}
                          keyboardType="numeric"
                          maxLength={4}
                          secureTextEntry
                        />
                      </View>
                      <TouchableOpacity
                        style={styles.saveCardRow}
                        onPress={() => setSaveCard(!saveCard)}
                        activeOpacity={0.7}
                      >
                        <Icon
                          name={saveCard ? 'checkbox-marked' : 'checkbox-blank-outline'}
                          size={22}
                          color={saveCard ? Colors.primary : Colors.textSecondary}
                        />
                        <Text style={styles.saveCardText}>Bu kartı kaydet</Text>
                      </TouchableOpacity>
                      {saveCard && (
                        <TextInput
                          style={[styles.cardInput, {marginTop: 8}]}
                          placeholder="Kart adı (ör: İş Bankası Kartım)"
                          placeholderTextColor={Colors.textTertiary}
                          value={cardAlias}
                          onChangeText={setCardAlias}
                          maxLength={50}
                        />
                      )}
                    </View>
                  )}
                </>
              )}
            </View>
          )}
        </View>

        {/* Sipariş Notu */}
        <View style={styles.section}>
          <View style={styles.sectionHeader}>
            <Icon name="note-text-outline" size={22} color={Colors.primary} />
            <Text style={styles.sectionTitle}>Sipariş Notu</Text>
          </View>
          <TextInput
            style={styles.noteInput}
            placeholder="Sipariş notunuzu buraya yazabilirsiniz..."
            placeholderTextColor={Colors.textTertiary}
            value={orderNote}
            onChangeText={setOrderNote}
            multiline
            numberOfLines={3}
            textAlignVertical="top"
          />
        </View>

        {/* Kupon */}
        <View style={styles.section}>
          <View style={styles.sectionHeader}>
            <Icon name="ticket-percent-outline" size={22} color={Colors.primary} />
            <Text style={styles.sectionTitle}>Kupon Kodu</Text>
          </View>
          {appliedCoupon ? (
            <View style={styles.appliedCoupon}>
              <View style={styles.appliedCouponInfo}>
                <Icon name="check-circle" size={20} color={Colors.success} />
                <View style={{marginLeft: 8, flex: 1}}>
                  <Text style={styles.appliedCouponCode}>{appliedCoupon.code}</Text>
                  {appliedCoupon.description ? (
                    <Text style={styles.appliedCouponDesc}>{appliedCoupon.description}</Text>
                  ) : null}
                </View>
              </View>
              <TouchableOpacity onPress={removeCoupon} style={styles.removeCouponBtn}>
                <Icon name="close-circle" size={20} color={Colors.error} />
              </TouchableOpacity>
            </View>
          ) : (
            <View>
              <View style={styles.couponInputRow}>
                <TextInput
                  style={styles.couponInput}
                  placeholder="Kupon kodu giriniz"
                  placeholderTextColor={Colors.textTertiary}
                  value={couponCode}
                  onChangeText={(t) => {
                    setCouponCode(t);
                    setCouponError('');
                  }}
                  autoCapitalize="characters"
                />
                <TouchableOpacity
                  style={[styles.couponApplyBtn, couponLoading && {opacity: 0.6}]}
                  onPress={handleApplyCoupon}
                  disabled={couponLoading}
                >
                  {couponLoading ? (
                    <ActivityIndicator color="#FFF" size="small" />
                  ) : (
                    <Text style={styles.couponApplyText}>Uygula</Text>
                  )}
                </TouchableOpacity>
              </View>
              {couponError ? <Text style={styles.couponErrorText}>{couponError}</Text> : null}
            </View>
          )}
        </View>

        {/* Sipariş Özeti */}
        <View style={styles.summaryContainer}>
          <Text style={styles.summaryTitle}>Sipariş Özeti</Text>
          {cart.items.map(item => {
            const isExpanded = expandedItems[item.cartItemId];
            const optionsSummary = formatOptionsSummary(item.values);
            return (
              <View key={item.cartItemId} style={styles.summaryItemContainer}>
                <View style={styles.summaryItem}>
                  <Text style={styles.summaryItemQty}>{item.quantity}x</Text>
                  <Text style={styles.summaryItemName} numberOfLines={1}>{item.menuName}</Text>
                  <Text style={styles.summaryItemPrice}>₺{item.totalPrice.toFixed(2)}</Text>
                </View>
                {optionsSummary && (
                  <TouchableOpacity
                    activeOpacity={0.7}
                    onPress={() => setExpandedItems(prev => ({...prev, [item.cartItemId]: !prev[item.cartItemId]}))}
                    style={styles.summaryOptionsRow}
                  >
                    <Text
                      style={styles.summaryOptionsText}
                      numberOfLines={isExpanded ? undefined : 1}
                    >
                      {optionsSummary}
                    </Text>
                    <Icon
                      name={isExpanded ? 'chevron-up' : 'chevron-down'}
                      size={14}
                      color={Colors.textSecondary}
                      style={{marginLeft: 4, marginTop: 1}}
                    />
                  </TouchableOpacity>
                )}
              </View>
            );
          })}
          <View style={styles.summaryDivider} />
          <View style={styles.summaryRow}>
            <Text style={styles.summaryLabel}>Ara Toplam</Text>
            <Text style={styles.summaryValue}>₺{subtotal.toFixed(2)}</Text>
          </View>
          {finalDiscount > 0 && (
            <View style={styles.summaryRow}>
              <Text style={[styles.summaryLabel, {color: Colors.success}]}>İndirim ({appliedCoupon?.code})</Text>
              <Text style={[styles.summaryValue, {color: Colors.success, fontWeight: Fonts.weights.bold}]}>-₺{finalDiscount.toFixed(2)}</Text>
            </View>
          )}
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

      <View style={[styles.bottomBar, {paddingBottom: bottomPadding}]}>
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
    paddingTop: Spacing.sm,
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
  // Selectbox / Dropdown
  selectBox: {
    flexDirection: 'row',
    alignItems: 'center',
    padding: Spacing.md,
    borderRadius: BorderRadius.md,
    borderWidth: 1.5,
    borderColor: Colors.border,
    backgroundColor: Colors.borderLight,
  },
  selectBoxText: {
    flex: 1,
    fontSize: Fonts.sizes.md,
    fontWeight: Fonts.weights.semibold,
    color: Colors.text,
    marginLeft: 10,
  },
  dropdownList: {
    marginTop: Spacing.sm,
    borderRadius: BorderRadius.md,
    borderWidth: 1,
    borderColor: Colors.borderLight,
    overflow: 'hidden',
  },
  dropdownItem: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingVertical: 12,
    paddingHorizontal: Spacing.md,
    borderBottomWidth: 1,
    borderBottomColor: Colors.borderLight,
  },
  dropdownItemSelected: {
    backgroundColor: Colors.primary + '0A',
  },
  dropdownItemText: {
    flex: 1,
    fontSize: Fonts.sizes.md,
    color: Colors.textSecondary,
    marginLeft: 10,
  },
  dropdownItemTextSelected: {
    color: Colors.primary,
    fontWeight: Fonts.weights.bold,
  },
  // Kart Formu
  cardForm: {
    marginTop: Spacing.md,
    paddingTop: Spacing.md,
    borderTopWidth: 1,
    borderTopColor: Colors.borderLight,
  },
  cardFormTitle: {
    fontSize: Fonts.sizes.sm,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
    marginBottom: Spacing.sm,
  },
  cardInput: {
    backgroundColor: Colors.borderLight,
    borderRadius: BorderRadius.md,
    paddingHorizontal: Spacing.md,
    paddingVertical: 10,
    fontSize: Fonts.sizes.md,
    color: Colors.text,
    marginBottom: Spacing.sm,
  },
  cardRow: {
    flexDirection: 'row',
    gap: Spacing.sm,
  },
  cardInputSmall: {
    flex: 1,
  },
  savedCard: {
    flexDirection: 'row',
    alignItems: 'center',
    padding: Spacing.md,
    borderRadius: BorderRadius.md,
    borderWidth: 1.5,
    borderColor: Colors.border,
    marginBottom: Spacing.sm,
  },
  savedCardSelected: {
    borderColor: Colors.primary,
    backgroundColor: Colors.primary + '08',
  },
  savedCardName: {
    fontSize: Fonts.sizes.md,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
  },
  savedCardDetail: {
    fontSize: Fonts.sizes.xs,
    color: Colors.textSecondary,
    marginTop: 2,
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
  radioOuterSelected: {
    borderColor: Colors.primary,
  },
  radioInner: {
    width: 12,
    height: 12,
    borderRadius: 6,
    backgroundColor: Colors.primary,
  },
  newCardToggle: {
    flexDirection: 'row',
    alignItems: 'center',
    padding: Spacing.md,
    borderRadius: BorderRadius.md,
    borderWidth: 1.5,
    borderColor: Colors.border,
    borderStyle: 'dashed',
  },
  newCardToggleActive: {
    borderColor: Colors.primary,
    backgroundColor: Colors.primary + '08',
  },
  newCardToggleText: {
    fontSize: Fonts.sizes.md,
    fontWeight: Fonts.weights.semibold,
    color: Colors.primary,
    marginLeft: 8,
  },
  saveCardRow: {
    flexDirection: 'row',
    alignItems: 'center',
    marginTop: 4,
  },
  saveCardText: {
    fontSize: Fonts.sizes.sm,
    color: Colors.textSecondary,
    marginLeft: 8,
  },
  // Sipariş Notu
  noteInput: {
    backgroundColor: Colors.borderLight,
    borderRadius: BorderRadius.md,
    padding: Spacing.md,
    fontSize: Fonts.sizes.md,
    color: Colors.text,
    minHeight: 80,
  },
  // Kupon
  appliedCoupon: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    backgroundColor: Colors.success + '10',
    padding: Spacing.md,
    borderRadius: BorderRadius.md,
    borderWidth: 1,
    borderColor: Colors.success + '30',
  },
  appliedCouponInfo: {
    flexDirection: 'row',
    alignItems: 'center',
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
  removeCouponBtn: {
    padding: 4,
    marginLeft: 8,
  },
  couponInputRow: {
    flexDirection: 'row',
    alignItems: 'center',
  },
  couponInput: {
    flex: 1,
    backgroundColor: Colors.borderLight,
    borderRadius: BorderRadius.md,
    paddingHorizontal: Spacing.md,
    paddingVertical: 10,
    fontSize: Fonts.sizes.md,
    color: Colors.text,
    marginRight: Spacing.sm,
  },
  couponApplyBtn: {
    backgroundColor: Colors.primary,
    paddingHorizontal: 18,
    paddingVertical: 10,
    borderRadius: BorderRadius.md,
  },
  couponApplyText: {
    color: '#FFF',
    fontSize: Fonts.sizes.sm,
    fontWeight: Fonts.weights.bold,
  },
  couponErrorText: {
    fontSize: Fonts.sizes.xs,
    color: Colors.error,
    marginTop: 6,
  },
  // Sipariş Özeti
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
  summaryItemContainer: {
    marginBottom: 8,
  },
  summaryItem: {
    flexDirection: 'row',
    alignItems: 'center',
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
    fontWeight: Fonts.weights.semibold,
  },
  summaryItemPrice: {
    fontSize: Fonts.sizes.sm,
    fontWeight: Fonts.weights.semibold,
    color: Colors.text,
    marginLeft: 8,
  },
  summaryOptionsRow: {
    flexDirection: 'row',
    alignItems: 'flex-start',
    marginLeft: 28,
    marginTop: 2,
  },
  summaryOptionsText: {
    fontSize: Fonts.sizes.xs,
    color: Colors.textSecondary,
    flex: 1,
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
