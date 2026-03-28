# Expo Migration Plan — Customer + Courier Apps

## Kapsam
Mevcut 2 React Native CLI uygulamasını Expo bare workflow'a taşıma.
- `front-app/` → `expo-app/` (Müşteri uygulaması)
- `front-courier/` → `expo-courier/` (Kurye uygulaması)

## Teknoloji Stack
- Expo SDK 52+ (bare workflow)
- expo-router (file-based routing)
- expo-location + expo-task-manager (arka plan konum)
- expo-notifications (push)
- @signalr/client (real-time)
- axios (HTTP)
- expo-secure-store (token storage, AsyncStorage yerine)
- expo-image (image loading)
- react-native-maps (harita)

## Müşteri App Ekranları (front-app → expo-app)
1. Auth: Login, Register, ForgotPassword, VerifyCode, ResetPassword
2. Home: RestaurantList, RestaurantDetail
3. Cart: CartScreen, CheckoutScreen
4. Orders: OrderHistory, OrderDetail
5. Profile: Profile, EditProfile, ChangePassword, AddressList, AddAddress, SavedCards, Favorites
6. Notifications: NotificationsScreen, NotificationPreferences
7. Scheduled Orders: ScheduledOrdersScreen
8. Support: SupportScreen, SupportChatScreen, NewTicketScreen
9. Payment: ThreeDsWebView
10. Onboarding: OnboardingScreen

Bileşenler: RestaurantCard, MenuItemCard, SearchBar, CuisineFilter, MenuOptionModal, ReviewModal, TipModal, OrderStatusBadge, FilterModal, ActiveOrderBanner, CartFloatingButton, EmptyState, LoadingSpinner

Context'ler: AuthContext, CartContext, AppDataContext, ToastContext

## Kurye App Ekranları (front-courier → expo-courier)
1. Auth: Login, Register, CourierRegister, CompanyRegister, PendingApproval
2. Dashboard: DashboardScreen (online/offline toggle)
3. Delivery: ActiveDeliveryScreen, DeliveryHistoryScreen
4. Earnings: EarningsScreen (ay/yıl filtre)
5. Profile: ProfileScreen
6. Agreements: AgreementsScreen

Context'ler: AuthContext, LocationContext (expo-location + background tracking)

## Arka Plan Konum (Kurye App)
```
expo-location + expo-task-manager:
- TaskManager.defineTask('BACKGROUND_LOCATION_TASK', ...)
- Location.startLocationUpdatesAsync('BACKGROUND_LOCATION_TASK', {
    accuracy: Location.Accuracy.High,
    timeInterval: 30000,
    distanceInterval: 50,
    foregroundService: { notificationTitle: "Konum takibi aktif" }
  })
- Uygulama kapalıyken bile çalışır
```

## Uygulama Sırası
1. Expo projeleri oluştur (create-expo-app --template bare-minimum)
2. Paket yapılandırması (package.json, app.json, eas.json)
3. Theme + utils + API layer taşı
4. Context'leri taşı/yeniden yaz
5. Bileşenleri taşı
6. Ekranları taşı
7. Navigasyonu expo-router ile yeniden yaz
8. Arka plan konum (kurye app)
9. Push notifications (expo-notifications)
10. Test + APK build (eas build)
