import React, {createContext, useContext, useState, useCallback} from 'react';

const CartContext = createContext(null);

export const CartProvider = ({children}) => {
  const [cart, setCart] = useState(null);
  const [itemCount, setItemCount] = useState(0);

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

  const addItem = useCallback((item, restaurant) => {
    setCart(prev => {
      if (prev && prev.restaurantId !== restaurant.id) {
        const newCart = {
          restaurantId: restaurant.id,
          sellerId: restaurant.sellerId,
          restaurantName: restaurant.name,
          restaurantImage: restaurant.imageUrl,
          items: [],
          totalQuantity: 0,
          totalPrice: 0,
        };
        return addItemToCart(newCart, item);
      }
      if (!prev) {
        const newCart = {
          restaurantId: restaurant.id,
          sellerId: restaurant.sellerId,
          restaurantName: restaurant.name,
          restaurantImage: restaurant.imageUrl,
          items: [],
          totalQuantity: 0,
          totalPrice: 0,
        };
        return addItemToCart(newCart, item);
      }
      return addItemToCart({...prev}, item);
    });
  }, []);

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
        return null;
      }

      return updated;
    });
  }, []);

  const removeItem = useCallback((cartItemId) => {
    updateItemQuantity(cartItemId, 0);
  }, [updateItemQuantity]);

  const clearCart = useCallback(() => {
    setCart(null);
    setItemCount(0);
  }, []);

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
