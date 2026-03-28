export const API_BASE_URL = 'https://food-order-api.yrlmzteknoloji.com/v1';

export const ORDER_STATUS = {
  1: {label: 'Ödeme Bekliyor', color: '#FF9500', icon: 'clock-outline'},
  2: {label: 'Ödeme Başarısız', color: '#FF3B30', icon: 'credit-card-off-outline'},
  3: {label: 'Alıcı İptal Etti', color: '#FF3B30', icon: 'close-circle-outline'},
  4: {label: 'Restoran Onayı Bekliyor', color: '#FF9500', icon: 'store-clock-outline'},
  5: {label: 'Restoran Reddetti', color: '#FF3B30', icon: 'store-remove-outline'},
  6: {label: 'Hazırlanıyor', color: '#AF52DE', icon: 'food-variant'},
  7: {label: 'Yola Çıktı', color: '#5856D6', icon: 'motorbike'},
  8: {label: 'Teslim Edildi', color: '#34C759', icon: 'check-all'},
  9: {label: 'Kurye Atandı', color: '#5856D6', icon: 'account-check'},
  10: {label: 'Kurye Teslim Aldı', color: '#5856D6', icon: 'package-variant'},
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

export const NOTIFICATION_TYPES = {
  1: {label: 'Sipariş Durumu', icon: 'package-variant'},
  2: {label: 'Kampanyalar', icon: 'tag-outline'},
  3: {label: 'Yorum Yanıtları', icon: 'comment-text-outline'},
  4: {label: 'Teslimat Güncellemeleri', icon: 'truck-delivery-outline'},
};

export const SCHEDULED_ORDER_STATUS = {
  1: {label: 'Planlandı', color: '#5856D6', icon: 'calendar-clock'},
  2: {label: 'İşleniyor', color: '#FF9500', icon: 'progress-clock'},
  3: {label: 'İptal Edildi', color: '#FF3B30', icon: 'calendar-remove'},
  4: {label: 'Siparişe Dönüştü', color: '#34C759', icon: 'calendar-check'},
};

export const SUPPORT_TOPICS = {
  1: {label: 'Sipariş Sorunu', icon: 'package-variant-closed'},
  2: {label: 'İptal Talebi', icon: 'cancel'},
  3: {label: 'Teslimat Problemi', icon: 'truck-alert-outline'},
  4: {label: 'Genel Soru', icon: 'help-circle-outline'},
  5: {label: 'Hesap Sorunu', icon: 'account-alert-outline'},
  6: {label: 'Ödeme Sorunu', icon: 'credit-card-off-outline'},
};

export const SUPPORT_STATUS = {
  1: {label: 'Açık', color: '#5856D6'},
  2: {label: 'İşlemde', color: '#FF9500'},
  3: {label: 'Çözüldü', color: '#34C759'},
  4: {label: 'Kapatıldı', color: '#8E8E93'},
  5: {label: 'Yönlendirildi', color: '#FF3B30'},
};

export const TIP_PERCENTAGES = [10, 15, 20];
