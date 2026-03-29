import { API_BASE_URL as ENV_API_URL } from '@env';

export const API_BASE_URL = ENV_API_URL || 'https://food-order-api.yrlmzteknoloji.com/v1';

export const DELIVERY_STATUS = {
  1: {label: 'Bekliyor', color: '#FF9500', icon: 'clock-outline'},
  2: {label: 'Teklif Edildi', color: '#5856D6', icon: 'bell-ring'},
  3: {label: 'Kabul Edildi', color: '#34C759', icon: 'check-circle'},
  4: {label: 'Reddedildi', color: '#FF3B30', icon: 'close-circle'},
  5: {label: 'Teslim Alındı', color: '#007AFF', icon: 'package-variant'},
  6: {label: 'Teslim Edildi', color: '#34C759', icon: 'check-all'},
  7: {label: 'İptal Edildi', color: '#FF3B30', icon: 'cancel'},
  8: {label: 'Süre Doldu', color: '#8E8E93', icon: 'timer-off'},
};

export const AVAILABILITY_STATUS = {
  0: {label: 'Çevrimdışı', color: '#8E8E93'},
  1: {label: 'Çevrimiçi', color: '#34C759'},
  2: {label: "Teslimat'ta", color: '#007AFF'},
};
