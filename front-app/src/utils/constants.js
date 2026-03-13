import {Platform} from 'react-native';

const API_HOST = Platform.select({
  android: '10.0.2.2',
  ios: 'localhost',
});

export const API_BASE_URL = `http://${API_HOST}:3762/v1`;

export const ORDER_STATUS = {
  1: {label: 'Beklemede', color: '#FF9500', icon: 'clock-outline'},
  2: {label: 'Onaylandı', color: '#007AFF', icon: 'check-circle-outline'},
  3: {label: 'Hazırlanıyor', color: '#AF52DE', icon: 'food-variant'},
  4: {label: 'Yolda', color: '#5856D6', icon: 'motorbike'},
  5: {label: 'Teslim Edildi', color: '#34C759', icon: 'check-all'},
  6: {label: 'İptal Edildi', color: '#FF3B30', icon: 'close-circle-outline'},
};

export const PAYMENT_OPTIONS = {
  1: {label: 'Kredi Kartı', icon: 'credit-card-outline'},
  2: {label: 'Kapıda Nakit', icon: 'cash'},
  3: {label: 'Kapıda Kart', icon: 'contactless-payment'},
};

export const CUISINE_ICONS = {
  'Türk Mutfağı': '🍖',
  'Fast Food': '🍔',
  'Pizza': '🍕',
  'Çin Mutfağı': '🥡',
  'Japon Mutfağı': '🍣',
  'İtalyan': '🍝',
  'Tatlı': '🍰',
  'Kebap': '🥙',
  'Döner': '🌯',
  'Kahvaltı': '🥐',
  'Ev Yemekleri': '🥘',
  'Balık': '🐟',
  'Salata': '🥗',
  'Vegan': '🥬',
  'Burger': '🍔',
  'Tavuk': '🍗',
  'default': '🍽️',
};
