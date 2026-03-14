import {Platform} from 'react-native';

const API_HOST = Platform.select({
  android: '10.0.2.2',
  ios: 'localhost',
});

export const API_BASE_URL = `http://${API_HOST}:3762/v1`;

export const ORDER_STATUS = {
  1: {label: 'Ödeme Bekliyor', color: '#FF9500', icon: 'clock-outline'},
  2: {label: 'Ödeme Başarısız', color: '#FF3B30', icon: 'credit-card-off-outline'},
  3: {label: 'Alıcı İptal Etti', color: '#FF3B30', icon: 'close-circle-outline'},
  4: {label: 'Restoran Onayı Bekliyor', color: '#FF9500', icon: 'store-clock-outline'},
  5: {label: 'Restoran Reddetti', color: '#FF3B30', icon: 'store-remove-outline'},
  6: {label: 'Hazırlanıyor', color: '#AF52DE', icon: 'food-variant'},
  7: {label: 'Yola Çıktı', color: '#5856D6', icon: 'motorbike'},
  8: {label: 'Teslim Edildi', color: '#34C759', icon: 'check-all'},
};

export const PAYMENT_OPTIONS = {
  1: {label: 'Online Kredi/Banka Kartı', icon: 'credit-card-outline'},
  2: {label: 'Kapıda Nakit', icon: 'cash'},
  3: {label: 'Kapıda Kredi/Banka Kartı', icon: 'contactless-payment'},
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
