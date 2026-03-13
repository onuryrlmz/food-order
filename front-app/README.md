# FoodOrder Mobile App

Çok satıcılı yemek sipariş platformunun React Native (CLI) mobil uygulaması. Yemeksepeti, Getir Yemek ve Migros Yemek'ten ilham alınmıştır.

## Teknoloji Stack

- **React Native 0.84** (CLI — Expo yok)
- **JavaScript** (TypeScript yok)
- **React Navigation 7** — Native Stack + Bottom Tabs
- **Axios** — HTTP client, auth interceptor
- **AsyncStorage** — Token saklama
- **react-native-vector-icons** — MaterialCommunityIcons
- **react-native-gesture-handler** — Gesture desteği
- **react-native-screens** — Native screen optimizasyonu
- **react-native-safe-area-context** — Safe area desteği

## Proje Yapısı

```
src/
├── api/                   # API servisleri (axios client, endpoints)
│   ├── client.js          # Axios instance + interceptors
│   ├── authService.js     # Login, register, profile
│   ├── restaurantService.js
│   ├── basketService.js
│   ├── orderService.js
│   ├── addressService.js
│   └── cuisineService.js
├── context/               # React Context providers
│   ├── AuthContext.js      # Auth state, login/logout
│   └── CartContext.js      # Sepet yönetimi
├── components/            # Paylaşılan UI bileşenleri
│   ├── RestaurantCard.js
│   ├── MenuItemCard.js
│   ├── CartFloatingButton.js
│   ├── SearchBar.js
│   ├── CuisineFilter.js
│   ├── OrderStatusBadge.js
│   ├── LoadingSpinner.js
│   └── EmptyState.js
├── screens/               # Ekranlar
│   ├── Auth/              # Login, Register, Splash
│   ├── Home/              # Ana sayfa, restoran listesi
│   ├── Restaurant/        # Restoran detay, menü
│   ├── Cart/              # Sepet, ödeme onayı
│   ├── Orders/            # Sipariş geçmişi, sipariş detay
│   └── Profile/           # Profil, adres yönetimi, şifre değiştir
├── navigation/            # React Navigation yapılandırması
│   └── AppNavigator.js
├── theme/                 # Tasarım sistemi
│   ├── colors.js
│   ├── fonts.js
│   └── spacing.js
└── utils/                 # Yardımcı sabitler
    └── constants.js
```

## Özellikler

- **Kullanıcı Girişi/Kayıt** — E-posta + şifre ile auth
- **Restoran Keşfi** — Konum bazlı restoran listesi, arama, mutfak filtreleme
- **Restoran Detay** — Kategori bazlı menü, parallax header
- **Sepet Yönetimi** — Ürün ekleme/çıkarma, miktar güncelleme, restoran değişikliği uyarısı
- **Sipariş** — Adres seçimi, ödeme yöntemi, sipariş notu
- **Sipariş Takibi** — Sipariş durumu, zaman çizelgesi, iptal
- **Sipariş Geçmişi** — Paginated liste, detay görüntüleme
- **Profil** — Bilgi düzenleme, şifre değiştirme
- **Adres Yönetimi** — CRUD, varsayılan adres, tip seçimi (Ev/İş/Diğer)

## Kurulum

```sh
# Bağımlılıkları yükle
npm install

# iOS pod'ları yükle
cd ios && pod install && cd ..

# Metro bundler başlat
npm start

# iOS simulator'da çalıştır
npm run ios

# Android emulator'da çalıştır
npm run android
```

## Backend Bağlantısı

API base URL `src/utils/constants.js` dosyasında tanımlıdır:
```js
export const API_BASE_URL = 'http://localhost:3762/v1';
```

Backend'in `.NET 8` projesinin çalışır durumda olması gerekir.
