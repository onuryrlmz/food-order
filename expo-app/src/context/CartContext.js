import React, {createContext, useContext, useState, useCallback, useEffect} from 'react';
import {basketService} from '../api/basketService';
import {restaurantService} from '../api';
import * as SecureStore from 'expo-secure-store';

const CartContext = createContext(null);

const EMPTY_GUID = '00000000-0000-0000-0000-000000000000';

export const CartProvider = ({children}) => {
  const [cart, setCart] = useState(null);
  const [itemCount, setItemCount] = useState(0);
  const [appliedCoupon, setAppliedCoupon] = useState(null);
  const [discountAmount, setDiscountAmount] = useState(0);
  const [orderNote, setOrderNote] = useState('');

  // Load basket from backend on mount
  useEffect(() => {
    loadBasketFromBackend();
  }, []);

  const fetchRestaurantJson = async (restaurantId) => {
    try {
      const res = await restaurantService.getRestaurantInfo(restaurantId);
      if (res.data && !res.data.hasFailed) {
        const cdnUrl = res.data.data;
        if (typeof cdnUrl === 'string' && cdnUrl.startsWith('http')) {
          const jsonRes = await fetch(cdnUrl);
          return await jsonRes.json();
        }
      }
    } catch (e) {}
    return null;
  };

  const enrichValuesFromJson = (basketItemValues, restaurantJson) => {
    if (!restaurantJson || !basketItemValues || basketItemValues.length === 0) {
      return basketItemValues || [];
    }
    // Tum menu opsiyonlarini duzlestir
    const allMenus = restaurantJson.flatMap(cat =>
      (cat.categoriesDetail || []).flatMap(cd => cd.menus || []),
    );
    const allOptions = allMenus.flatMap(m => m.menuOptions || []);
    const allValues = allOptions.flatMap(o => o.menuOptionValues || []);

    return basketItemValues.map(biv => {
      const matchedOption = allOptions.find(o => o.id === biv.menuOptionId);
      const matchedValue = allValues.find(v => v.id === biv.menuOptionValueId);

      const enrichedVoovs = (biv.basketItemValueItemValues || []).map(voov => {
        const allVoos = allValues.flatMap(v => v.menuOptionValueOptions || []);
        const matchedVoo = allVoos.find(voo => voo.id === voov.menuOptionValueOptionId);
        const allVoovs = allVoos.flatMap(voo => voo.menuOptionValueOptionValues || []);
        const matchedVoov = allVoovs.find(v => v.id === voov.menuOptionValueOptionValueId);
        return {
          ...voov,
          name: matchedVoov?.name || '',
          menuOptionValueOptionId: voov.menuOptionValueOptionId,
          parentOptionName: matchedVoo?.name || '',
        };
      });

      return {
        optionId: biv.menuOptionId,
        optionName: matchedOption?.name || '',
        valueId: biv.menuOptionValueId,
        valueName: matchedValue?.name || '',
        valuePrice: biv.unitPrice || 0,
        productId: biv.productId,
        quantity: biv.quantity,
        valueOptionValues: enrichedVoovs,
      };
    });
  };

  const loadBasketFromBackend = async () => {
    try {
      const token = await SecureStore.getItemAsync('auth_token');
      if (!token) return;
      const res = await basketService.getBasket();
      if (res.data && !res.data.hasFailed && res.data.data) {
        const b = res.data.data;
        if (b.basketItems && b.basketItems.length > 0) {
          // Restoran JSON'unu cekip value isimlerini zenginlestir
          const restaurantJson = await fetchRestaurantJson(b.restaurantId);

          const items = b.basketItems.map((bi, idx) => ({
            cartItemId: bi.id || (Date.now().toString() + idx),
            menuId: bi.menuId,
            menuName: bi.menuName || '',
            quantity: bi.quantity,
            unitPrice: bi.unitPrice,
            totalPrice: bi.totalPrice,
            values: enrichValuesFromJson(bi.basketItemValues, restaurantJson),
          }));
          const loaded = {
            restaurantId: b.restaurantId,
            sellerId: b.sellerId,
            restaurantName: b.restaurantName || '',
            restaurantImage: null,
            items,
            totalQuantity: b.totalQuantity || items.reduce((s, i) => s + i.quantity, 0),
            totalPrice: b.totalProductPrice || items.reduce((s, i) => s + i.totalPrice, 0),
          };
          setCart(loaded);
          setItemCount(loaded.totalQuantity);
        }
      }
    } catch (e) {
      // Silently fail - user might not be logged in
    }
  };

  // Fire-and-forget sync to backend (no debounce)
  const syncToBackend = useCallback((cartData) => {
    (async () => {
      try {
        const token = await SecureStore.getItemAsync('auth_token');
        if (!token) return;
        if (!cartData) {
          basketService.clearBasket().catch(() => {});
          return;
        }
        const payload = {
          id: EMPTY_GUID,
          restaurantId: cartData.restaurantId || EMPTY_GUID,
          sellerId: cartData.sellerId || EMPTY_GUID,
          userShippingAddressId: EMPTY_GUID,
          userInvoiceAddressId: EMPTY_GUID,
          paymentOptionId: 1,
          basketItems: cartData.items.map(item => ({
            menuId: item.menuId,
            quantity: item.quantity,
            basketItemValues: (item.values || []).map(v => ({
              menuOptionId: v.optionId || v.menuOptionId || EMPTY_GUID,
              menuOptionValueId: v.valueId || v.menuOptionValueId || EMPTY_GUID,
              productId: v.productId || EMPTY_GUID,
              quantity: v.quantity || 1,
              basketItemValueItemValues: (v.valueOptionValues || []).map(vov => ({
                menuOptionValueOptionId: vov.menuOptionValueOptionId || EMPTY_GUID,
                menuOptionValueOptionValueId: vov.id || vov.menuOptionValueOptionValueId || EMPTY_GUID,
                productId: vov.productId || EMPTY_GUID,
                quantity: vov.quantity || 1,
              })),
            })),
          })),
        };
        basketService.updateBasket(payload).catch(() => {});
      } catch (e) {
        // Silently fail
      }
    })();
  }, []);

  const initCart = useCallback((restaurantId, sellerId, restaurantName, restaurantImage) => {
    setCart({
      restaurantId,
      sellerId,
      restaurantName,
      restaurantImage,
      items: [],
      totalQuantity: 0,
      totalPrice: 0,
    });
  }, []);

  const isDifferentRestaurant = useCallback((restaurantId) => {
    return cart && cart.items.length > 0 && cart.restaurantId !== restaurantId;
  }, [cart]);

  const addItem = useCallback((item, restaurant) => {
    setCart(prev => {
      let cartData;
      if (!prev || prev.restaurantId !== restaurant.id) {
        // Farkli restoran veya bos sepet — yeni sepet olustur
        // Farkli restoransa once Redis'teki eski sepeti sil
        if (prev && prev.restaurantId !== restaurant.id) {
          basketService.clearBasket().catch(() => {});
        }
        cartData = {
          restaurantId: restaurant.id,
          sellerId: restaurant.sellerId,
          restaurantName: restaurant.name,
          restaurantImage: restaurant.imageUrl,
          items: [],
          totalQuantity: 0,
          totalPrice: 0,
        };
      } else {
        cartData = {...prev};
      }
      const result = addItemToCart(cartData, item);
      syncToBackend(result);
      return result;
    });
  }, [syncToBackend, cart]);

  const addItemToCart = (cartData, item) => {
    const existingIndex = cartData.items.findIndex(
      i => i.menuId === item.menuId && JSON.stringify(i.values) === JSON.stringify(item.values),
    );

    if (existingIndex >= 0) {
      cartData.items[existingIndex].quantity += item.quantity;
      cartData.items[existingIndex].totalPrice =
        cartData.items[existingIndex].quantity * cartData.items[existingIndex].unitPrice;
    } else {
      cartData.items.push({
        ...item,
        cartItemId: Date.now().toString() + Math.random().toString(36).substr(2, 9),
      });
    }

    cartData.totalQuantity = cartData.items.reduce((sum, i) => sum + i.quantity, 0);
    cartData.totalPrice = cartData.items.reduce((sum, i) => sum + i.totalPrice, 0);
    setItemCount(cartData.totalQuantity);
    return cartData;
  };

  const updateItemQuantity = useCallback((cartItemId, quantity) => {
    // Invalidate coupon when cart changes
    setAppliedCoupon(null);
    setDiscountAmount(0);

    setCart(prev => {
      if (!prev) return prev;
      const updated = {...prev, items: [...prev.items]};
      const index = updated.items.findIndex(i => i.cartItemId === cartItemId);
      if (index < 0) return prev;

      if (quantity <= 0) {
        updated.items.splice(index, 1);
      } else {
        updated.items[index] = {
          ...updated.items[index],
          quantity,
          totalPrice: quantity * updated.items[index].unitPrice,
        };
      }

      updated.totalQuantity = updated.items.reduce((sum, i) => sum + i.quantity, 0);
      updated.totalPrice = updated.items.reduce((sum, i) => sum + i.totalPrice, 0);
      setItemCount(updated.totalQuantity);

      if (updated.items.length === 0) {
        setItemCount(0);
        syncToBackend(null);
        return null;
      }

      syncToBackend(updated);
      return updated;
    });
  }, [syncToBackend]);

  const removeItem = useCallback((cartItemId) => {
    updateItemQuantity(cartItemId, 0);
  }, [updateItemQuantity]);

  const clearCart = useCallback(() => {
    syncToBackend(null);
    setCart(null);
    setItemCount(0);
    setAppliedCoupon(null);
    setDiscountAmount(0);
    setOrderNote('');
  }, [syncToBackend]);

  const applyCoupon = useCallback((coupon, discount) => {
    setAppliedCoupon(coupon);
    setDiscountAmount(discount);
  }, []);

  const removeCoupon = useCallback(() => {
    setAppliedCoupon(null);
    setDiscountAmount(0);
  }, []);

  const getFinalTotal = useCallback(() => {
    if (!cart) return 0;
    return Math.max(0, cart.totalPrice - discountAmount);
  }, [cart, discountAmount]);

  const getCartTotal = useCallback(() => {
    if (!cart) return 0;
    return cart.totalPrice;
  }, [cart]);

  return (
    <CartContext.Provider
      value={{
        cart,
        itemCount,
        addItem,
        updateItemQuantity,
        removeItem,
        clearCart,
        getCartTotal,
        initCart,
        appliedCoupon,
        discountAmount,
        applyCoupon,
        removeCoupon,
        getFinalTotal,
        loadBasketFromBackend,
        isDifferentRestaurant,
        orderNote,
        setOrderNote,
      }}>
      {children}
    </CartContext.Provider>
  );
};

export const useCart = () => {
  const context = useContext(CartContext);
  if (!context) {
    throw new Error('useCart must be used within a CartProvider');
  }
  return context;
};
