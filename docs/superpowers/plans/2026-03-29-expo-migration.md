# Expo Bare Workflow Migration Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Migrate `front-app/` and `front-courier/` from React Native CLI to Expo bare workflow with expo-router, expo-secure-store, expo-notifications, and expo-location background tracking.

**Architecture:** Two independent Expo bare workflow apps (`expo-app/` and `expo-courier/`) using file-based routing via expo-router. Screen code migrated with minimal changes — main rewrites are navigation (expo-router layouts), token storage (expo-secure-store), push notifications (expo-notifications), and location tracking (expo-location + expo-task-manager for background). Theme, API layer, and components copied with only import path adjustments.

**Tech Stack:** Expo SDK 52, expo-router v4, expo-location, expo-task-manager, expo-notifications, expo-secure-store, expo-image, @expo/vector-icons, axios, @microsoft/signalr, react-native-maps

---

## File Structure

### expo-app/ (Customer App)

```
expo-app/
├── app/                              # expo-router file-based routing
│   ├── _layout.js                    # Root layout (providers + slot)
│   ├── index.js                      # Entry redirect (splash/onboarding check)
│   ├── onboarding.js                 # Onboarding screen
│   ├── (auth)/
│   │   ├── _layout.js                # Auth stack layout
│   │   ├── login.js
│   │   ├── register.js
│   │   ├── forgot-password.js
│   │   ├── verify-code.js
│   │   └── reset-password.js
│   ├── (tabs)/
│   │   ├── _layout.js                # Tab navigator layout
│   │   ├── index.js                  # Home tab (HomeScreen)
│   │   ├── cart.js                   # Cart tab
│   │   ├── orders.js                 # Orders tab (OrderHistory)
│   │   └── profile.js               # Profile tab
│   ├── restaurant/
│   │   └── [id].js                   # Restaurant detail
│   ├── order/
│   │   └── [id].js                   # Order detail
│   ├── checkout.js
│   ├── three-ds.js                   # 3DS WebView
│   ├── address-list.js
│   ├── add-address.js
│   ├── edit-profile.js
│   ├── change-password.js
│   ├── saved-cards.js
│   ├── favorites.js
│   ├── notifications.js
│   ├── notification-preferences.js
│   ├── scheduled-orders.js
│   ├── support.js
│   ├── support-chat.js
│   └── new-ticket.js
├── src/
│   ├── api/                          # Copied from front-app, AsyncStorage → SecureStore
│   │   ├── client.js
│   │   ├── authService.js
│   │   ├── restaurantService.js
│   │   ├── basketService.js
│   │   ├── orderService.js
│   │   ├── addressService.js
│   │   ├── cuisineService.js
│   │   ├── couponService.js
│   │   ├── reviewService.js
│   │   ├── favoriteService.js
│   │   ├── searchService.js
│   │   ├── tipService.js
│   │   ├── notificationService.js
│   │   ├── scheduledOrderService.js
│   │   ├── supportService.js
│   │   └── cardService.js
│   ├── components/                   # Copied from front-app, icon import changes
│   │   ├── ActiveOrderBanner.js
│   │   ├── CartFloatingButton.js
│   │   ├── CuisineFilter.js
│   │   ├── EmptyState.js
│   │   ├── FilterModal.js
│   │   ├── LoadingSpinner.js
│   │   ├── MenuItemCard.js
│   │   ├── MenuOptionModal.js
│   │   ├── OrderStatusBadge.js
│   │   ├── RestaurantCard.js
│   │   ├── ReviewList.js
│   │   ├── ReviewModal.js
│   │   ├── SearchBar.js
│   │   └── TipModal.js
│   ├── context/
│   │   ├── AuthContext.js            # Rewritten: SecureStore + expo-notifications + expo-router
│   │   ├── CartContext.js            # Copied, minimal changes
│   │   ├── AppDataContext.js         # Rewritten: expo-location for device location
│   │   └── ToastContext.js           # Copied as-is
│   ├── theme/                        # Copied as-is
│   │   ├── colors.js
│   │   ├── fonts.js
│   │   ├── spacing.js
│   │   └── index.js
│   └── utils/
│       ├── constants.js              # Copied as-is
│       ├── notifications.js          # NEW: expo-notifications setup (replaces onesignal.js)
│       └── signalr.js               # Rewritten: SecureStore instead of AsyncStorage
├── app.json                          # Expo config
├── babel.config.js
├── metro.config.js
├── package.json
├── eas.json                          # EAS Build config
└── tsconfig.json
```

### expo-courier/ (Courier App)

```
expo-courier/
├── app/
│   ├── _layout.js                    # Root layout (providers + auth routing)
│   ├── index.js                      # Entry redirect based on auth state
│   ├── (auth)/
│   │   ├── _layout.js
│   │   ├── login.js
│   │   └── register.js
│   ├── (setup)/
│   │   ├── _layout.js
│   │   ├── courier-register.js
│   │   ├── company-register.js
│   │   └── pending-approval.js
│   ├── (tabs)/
│   │   ├── _layout.js               # Tab navigator
│   │   ├── index.js                  # Dashboard
│   │   ├── deliveries.js            # Delivery history
│   │   ├── earnings.js
│   │   └── profile.js
│   └── agreements.js
├── src/
│   ├── api/
│   │   ├── client.js                # SecureStore version
│   │   ├── courierService.js        # Copied
│   │   └── companyService.js        # Copied
│   ├── context/
│   │   ├── AuthContext.js           # Rewritten: SecureStore + expo-router
│   │   └── LocationContext.js       # Rewritten: expo-location + expo-task-manager
│   ├── theme/                       # Copied
│   │   ├── colors.js
│   │   ├── fonts.js
│   │   ├── spacing.js
│   │   └── index.js
│   └── utils/
│       ├── constants.js             # Copied
│       ├── signalr.js              # SecureStore version
│       └── backgroundLocation.js   # NEW: TaskManager task definition
├── app.json
├── babel.config.js
├── metro.config.js
├── package.json
├── eas.json
└── tsconfig.json
```

---

## Task 1: Create expo-app Project Scaffold

**Files:**
- Create: `expo-app/package.json`
- Create: `expo-app/app.json`
- Create: `expo-app/babel.config.js`
- Create: `expo-app/metro.config.js`
- Create: `expo-app/tsconfig.json`
- Create: `expo-app/eas.json`
- Create: `expo-app/index.js`

- [ ] **Step 1: Create Expo project**

```bash
cd /Users/onuryorulmaz/Desktop/Github/food-order
npx create-expo-app@latest expo-app --template bare-minimum
```

- [ ] **Step 2: Install all dependencies**

```bash
cd /Users/onuryorulmaz/Desktop/Github/food-order/expo-app
npx expo install expo-router expo-secure-store expo-location expo-task-manager expo-notifications expo-image @expo/vector-icons expo-linear-gradient expo-font expo-splash-screen expo-status-bar expo-web-browser expo-linking
npx expo install react-native-maps react-native-webview react-native-gesture-handler react-native-safe-area-context react-native-screens react-native-reanimated
npm install axios @microsoft/signalr @react-native-async-storage/async-storage
```

- [ ] **Step 3: Configure app.json**

Replace `expo-app/app.json` with:

```json
{
  "expo": {
    "name": "FoodOrderApp",
    "slug": "food-order-app",
    "version": "1.0.0",
    "orientation": "portrait",
    "icon": "./assets/icon.png",
    "scheme": "foodorderapp",
    "userInterfaceStyle": "light",
    "newArchEnabled": true,
    "splash": {
      "backgroundColor": "#F2F2F7"
    },
    "ios": {
      "supportsTablet": false,
      "bundleIdentifier": "com.yrlmz.foodorderapp",
      "infoPlist": {
        "NSLocationWhenInUseUsageDescription": "Yakınınızdaki restoranları göstermek için konumunuza ihtiyacımız var."
      }
    },
    "android": {
      "adaptiveIcon": {
        "backgroundColor": "#F2F2F7"
      },
      "package": "com.yrlmz.foodorderapp",
      "permissions": ["ACCESS_FINE_LOCATION", "ACCESS_COARSE_LOCATION"]
    },
    "plugins": [
      "expo-router",
      "expo-secure-store",
      "expo-notifications",
      [
        "expo-location",
        {
          "locationWhenInUsePermission": "Yakınınızdaki restoranları göstermek için konumunuza ihtiyacımız var."
        }
      ]
    ]
  }
}
```

- [ ] **Step 4: Configure babel.config.js**

Replace `expo-app/babel.config.js` with:

```javascript
module.exports = function (api) {
  api.cache(true);
  return {
    presets: ['babel-preset-expo'],
    plugins: ['react-native-reanimated/plugin'],
  };
};
```

- [ ] **Step 5: Configure metro.config.js**

Replace `expo-app/metro.config.js` with:

```javascript
const { getDefaultConfig } = require('expo/metro-config');

const config = getDefaultConfig(__dirname);

module.exports = config;
```

- [ ] **Step 6: Create eas.json**

Create `expo-app/eas.json`:

```json
{
  "cli": {
    "version": ">= 12.0.0"
  },
  "build": {
    "development": {
      "developmentClient": true,
      "distribution": "internal"
    },
    "preview": {
      "android": {
        "buildType": "apk"
      },
      "distribution": "internal"
    },
    "production": {}
  }
}
```

- [ ] **Step 7: Update package.json main entry**

In `expo-app/package.json`, ensure the `main` field points to expo-router:

```json
{
  "main": "expo-router/entry"
}
```

- [ ] **Step 8: Create assets directory and placeholder icon**

```bash
mkdir -p expo-app/assets
cp front-app/src/assets/icon.png expo-app/assets/icon.png 2>/dev/null || echo "placeholder" > expo-app/assets/icon.png
```

- [ ] **Step 9: Commit**

```bash
git add expo-app/
git commit -m "feat(expo-app): scaffold Expo bare workflow project with dependencies"
```

---

## Task 2: Create expo-courier Project Scaffold

**Files:**
- Create: `expo-courier/package.json`
- Create: `expo-courier/app.json`
- Create: `expo-courier/babel.config.js`
- Create: `expo-courier/metro.config.js`
- Create: `expo-courier/eas.json`

- [ ] **Step 1: Create Expo project**

```bash
cd /Users/onuryorulmaz/Desktop/Github/food-order
npx create-expo-app@latest expo-courier --template bare-minimum
```

- [ ] **Step 2: Install all dependencies**

```bash
cd /Users/onuryorulmaz/Desktop/Github/food-order/expo-courier
npx expo install expo-router expo-secure-store expo-location expo-task-manager expo-notifications expo-image @expo/vector-icons expo-linear-gradient expo-font expo-splash-screen expo-status-bar expo-linking
npx expo install react-native-maps react-native-gesture-handler react-native-safe-area-context react-native-screens react-native-reanimated
npm install axios @microsoft/signalr @react-native-async-storage/async-storage
```

- [ ] **Step 3: Configure app.json**

Replace `expo-courier/app.json`:

```json
{
  "expo": {
    "name": "FoodOrder Courier",
    "slug": "food-order-courier",
    "version": "1.0.0",
    "orientation": "portrait",
    "icon": "./assets/icon.png",
    "scheme": "foodordercourier",
    "userInterfaceStyle": "light",
    "newArchEnabled": true,
    "splash": {
      "backgroundColor": "#F2F2F7"
    },
    "ios": {
      "supportsTablet": false,
      "bundleIdentifier": "com.yrlmz.foodordercourier",
      "infoPlist": {
        "NSLocationWhenInUseUsageDescription": "Teslimat yapabilmek için konumunuza ihtiyacımız var.",
        "NSLocationAlwaysAndWhenInUseUsageDescription": "Uygulama kapalıyken bile teslimat takibi için konum izni gereklidir.",
        "NSLocationAlwaysUsageDescription": "Arka planda konum takibi için izin gereklidir.",
        "UIBackgroundModes": ["location", "fetch"]
      }
    },
    "android": {
      "adaptiveIcon": {
        "backgroundColor": "#F2F2F7"
      },
      "package": "com.yrlmz.foodordercourier",
      "permissions": [
        "ACCESS_FINE_LOCATION",
        "ACCESS_COARSE_LOCATION",
        "ACCESS_BACKGROUND_LOCATION",
        "FOREGROUND_SERVICE",
        "FOREGROUND_SERVICE_LOCATION"
      ]
    },
    "plugins": [
      "expo-router",
      "expo-secure-store",
      "expo-notifications",
      [
        "expo-location",
        {
          "locationWhenInUsePermission": "Teslimat yapabilmek için konumunuza ihtiyacımız var.",
          "locationAlwaysAndWhenInUsePermission": "Uygulama kapalıyken bile teslimat takibi için konum izni gereklidir.",
          "locationAlwaysPermission": "Arka planda konum takibi için izin gereklidir.",
          "isAndroidBackgroundLocationEnabled": true,
          "isAndroidForegroundServiceEnabled": true
        }
      ],
      [
        "expo-task-manager"
      ]
    ]
  }
}
```

- [ ] **Step 4: Configure babel.config.js**

Replace `expo-courier/babel.config.js`:

```javascript
module.exports = function (api) {
  api.cache(true);
  return {
    presets: ['babel-preset-expo'],
    plugins: ['react-native-reanimated/plugin'],
  };
};
```

- [ ] **Step 5: Configure metro.config.js**

Replace `expo-courier/metro.config.js`:

```javascript
const { getDefaultConfig } = require('expo/metro-config');

const config = getDefaultConfig(__dirname);

module.exports = config;
```

- [ ] **Step 6: Create eas.json**

Create `expo-courier/eas.json`:

```json
{
  "cli": {
    "version": ">= 12.0.0"
  },
  "build": {
    "development": {
      "developmentClient": true,
      "distribution": "internal"
    },
    "preview": {
      "android": {
        "buildType": "apk"
      },
      "distribution": "internal"
    },
    "production": {}
  }
}
```

- [ ] **Step 7: Update package.json main entry**

In `expo-courier/package.json`, ensure:

```json
{
  "main": "expo-router/entry"
}
```

- [ ] **Step 8: Create assets directory**

```bash
mkdir -p expo-courier/assets
echo "placeholder" > expo-courier/assets/icon.png
```

- [ ] **Step 9: Commit**

```bash
git add expo-courier/
git commit -m "feat(expo-courier): scaffold Expo bare workflow project with background location config"
```

---

## Task 3: Migrate expo-app Theme + Utils

**Files:**
- Create: `expo-app/src/theme/colors.js` (copy from `front-app/src/theme/colors.js`)
- Create: `expo-app/src/theme/fonts.js` (copy from `front-app/src/theme/fonts.js`)
- Create: `expo-app/src/theme/spacing.js` (copy from `front-app/src/theme/spacing.js`)
- Create: `expo-app/src/theme/index.js` (copy from `front-app/src/theme/index.js`)
- Create: `expo-app/src/utils/constants.js` (copy from `front-app/src/utils/constants.js`)
- Create: `expo-app/src/utils/signalr.js` (rewrite with SecureStore)
- Create: `expo-app/src/utils/notifications.js` (new, replaces onesignal.js)

- [ ] **Step 1: Copy theme files as-is**

```bash
mkdir -p expo-app/src/theme
cp front-app/src/theme/colors.js expo-app/src/theme/colors.js
cp front-app/src/theme/fonts.js expo-app/src/theme/fonts.js
cp front-app/src/theme/spacing.js expo-app/src/theme/spacing.js
cp front-app/src/theme/index.js expo-app/src/theme/index.js
```

- [ ] **Step 2: Copy constants.js as-is**

```bash
mkdir -p expo-app/src/utils
cp front-app/src/utils/constants.js expo-app/src/utils/constants.js
```

- [ ] **Step 3: Create signalr.js with SecureStore**

Create `expo-app/src/utils/signalr.js`:

```javascript
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import * as SecureStore from 'expo-secure-store';
import { API_BASE_URL } from './constants';

const BASE_URL = API_BASE_URL.replace('/v1', '');

export const createOrderConnection = async (orderId) => {
  const token = await SecureStore.getItemAsync('auth_token');

  const connection = new HubConnectionBuilder()
    .withUrl(`${BASE_URL}/hubs/order`, {
      accessTokenFactory: () => token,
    })
    .withAutomaticReconnect()
    .configureLogging(LogLevel.Warning)
    .build();

  try {
    await connection.start();
    await connection.invoke('JoinOrderGroup', orderId);
  } catch (err) {
    console.log('SignalR connection error:', err);
  }

  return connection;
};
```

- [ ] **Step 4: Create notifications.js (expo-notifications)**

Create `expo-app/src/utils/notifications.js`:

```javascript
import * as Notifications from 'expo-notifications';
import { Platform } from 'react-native';
import { router } from 'expo-router';
import { API_BASE_URL } from './constants';
import * as SecureStore from 'expo-secure-store';
import axios from 'axios';

Notifications.setNotificationHandler({
  handleNotification: async () => ({
    shouldShowAlert: true,
    shouldPlaySound: true,
    shouldSetBadge: true,
  }),
});

export const initNotifications = () => {
  const subscription = Notifications.addNotificationResponseReceivedListener(
    (response) => {
      const data = response.notification.request.content.data;
      if (data?.orderId) {
        router.push(`/order/${data.orderId}`);
      }
    }
  );
  return subscription;
};

export const registerForPushNotifications = async (userId) => {
  const { status: existingStatus } = await Notifications.getPermissionsAsync();
  let finalStatus = existingStatus;

  if (existingStatus !== 'granted') {
    const { status } = await Notifications.requestPermissionsAsync();
    finalStatus = status;
  }

  if (finalStatus !== 'granted') {
    return null;
  }

  const tokenData = await Notifications.getExpoPushTokenAsync();
  const pushToken = tokenData.data;

  // Send token to backend
  try {
    const authToken = await SecureStore.getItemAsync('auth_token');
    await axios.post(
      `${API_BASE_URL}/auth/push-token`,
      { token: pushToken, platform: Platform.OS },
      { headers: { Authorization: `Bearer ${authToken}` } }
    );
  } catch (e) {
    // Silent fail
  }

  return pushToken;
};

export const clearPushToken = async () => {
  try {
    const authToken = await SecureStore.getItemAsync('auth_token');
    await axios.delete(`${API_BASE_URL}/auth/push-token`, {
      headers: { Authorization: `Bearer ${authToken}` },
    });
  } catch (e) {
    // Silent fail
  }
};
```

- [ ] **Step 5: Commit**

```bash
git add expo-app/src/theme/ expo-app/src/utils/
git commit -m "feat(expo-app): add theme, constants, signalr, and notification utils"
```

---

## Task 4: Migrate expo-courier Theme + Utils

**Files:**
- Create: `expo-courier/src/theme/*` (copy from `front-courier/src/theme/`)
- Create: `expo-courier/src/utils/constants.js` (copy)
- Create: `expo-courier/src/utils/signalr.js` (rewrite with SecureStore)
- Create: `expo-courier/src/utils/backgroundLocation.js` (new)

- [ ] **Step 1: Copy theme files**

```bash
mkdir -p expo-courier/src/theme
cp front-courier/src/theme/colors.js expo-courier/src/theme/colors.js
cp front-courier/src/theme/fonts.js expo-courier/src/theme/fonts.js
cp front-courier/src/theme/spacing.js expo-courier/src/theme/spacing.js
cp front-courier/src/theme/index.js expo-courier/src/theme/index.js
```

- [ ] **Step 2: Copy constants.js**

```bash
mkdir -p expo-courier/src/utils
cp front-courier/src/utils/constants.js expo-courier/src/utils/constants.js
```

- [ ] **Step 3: Create signalr.js with SecureStore**

Create `expo-courier/src/utils/signalr.js`:

```javascript
import * as signalR from '@microsoft/signalr';
import * as SecureStore from 'expo-secure-store';
import { API_BASE_URL } from './constants';

const BASE_URL = API_BASE_URL.replace('/v1', '');

export const createCourierConnection = (courierId) => {
  const connection = new signalR.HubConnectionBuilder()
    .withUrl(`${BASE_URL}/hubs/courier`, {
      accessTokenFactory: async () => {
        const token = await SecureStore.getItemAsync('auth_token');
        return token;
      },
    })
    .withAutomaticReconnect()
    .build();

  connection.start().then(() => {
    connection.invoke('JoinCourierGroup', courierId);
  }).catch(err => {
    console.warn('SignalR connection error:', err);
  });

  return connection;
};

export const stopCourierConnection = async (connection) => {
  if (connection) {
    try {
      await connection.stop();
    } catch (err) {
      console.warn('SignalR stop error:', err);
    }
  }
};
```

- [ ] **Step 4: Create backgroundLocation.js (TaskManager task)**

Create `expo-courier/src/utils/backgroundLocation.js`:

```javascript
import * as TaskManager from 'expo-task-manager';
import * as Location from 'expo-location';
import * as SecureStore from 'expo-secure-store';
import axios from 'axios';
import { API_BASE_URL } from './constants';

export const BACKGROUND_LOCATION_TASK = 'BACKGROUND_LOCATION_TASK';

// Define the background task — this MUST be at the top level (outside components)
TaskManager.defineTask(BACKGROUND_LOCATION_TASK, async ({ data, error }) => {
  if (error) {
    console.warn('Background location error:', error.message);
    return;
  }

  if (data) {
    const { locations } = data;
    const location = locations[0];
    if (!location) return;

    try {
      const token = await SecureStore.getItemAsync('auth_token');
      if (!token) return;

      await axios.put(
        `${API_BASE_URL}/courier/location`,
        {
          latitude: location.coords.latitude,
          longitude: location.coords.longitude,
        },
        {
          headers: { Authorization: `Bearer ${token}` },
          timeout: 10000,
        }
      );
    } catch (e) {
      // Silent fail for background updates
    }
  }
});

export const startBackgroundLocation = async () => {
  const { status: foregroundStatus } = await Location.requestForegroundPermissionsAsync();
  if (foregroundStatus !== 'granted') {
    return false;
  }

  const { status: backgroundStatus } = await Location.requestBackgroundPermissionsAsync();
  if (backgroundStatus !== 'granted') {
    return false;
  }

  const isTaskDefined = TaskManager.isTaskDefined(BACKGROUND_LOCATION_TASK);
  if (!isTaskDefined) {
    console.warn('Background location task is not defined');
    return false;
  }

  const hasStarted = await Location.hasStartedLocationUpdatesAsync(BACKGROUND_LOCATION_TASK);
  if (hasStarted) {
    return true; // Already running
  }

  await Location.startLocationUpdatesAsync(BACKGROUND_LOCATION_TASK, {
    accuracy: Location.Accuracy.High,
    timeInterval: 30000,
    distanceInterval: 50,
    deferredUpdatesInterval: 30000,
    showsBackgroundLocationIndicator: true,
    foregroundService: {
      notificationTitle: 'Konum takibi aktif',
      notificationBody: 'Teslimat için konumunuz takip ediliyor',
      notificationColor: '#007AFF',
    },
  });

  return true;
};

export const stopBackgroundLocation = async () => {
  const hasStarted = await Location.hasStartedLocationUpdatesAsync(BACKGROUND_LOCATION_TASK);
  if (hasStarted) {
    await Location.stopLocationUpdatesAsync(BACKGROUND_LOCATION_TASK);
  }
};
```

- [ ] **Step 5: Commit**

```bash
git add expo-courier/src/theme/ expo-courier/src/utils/
git commit -m "feat(expo-courier): add theme, constants, signalr, and background location task"
```

---

## Task 5: Migrate expo-app API Layer

**Files:**
- Create: `expo-app/src/api/client.js` (rewrite with SecureStore)
- Create: `expo-app/src/api/*.js` (copy all service files from front-app)

- [ ] **Step 1: Create API client with SecureStore**

Create `expo-app/src/api/client.js`:

```javascript
import axios from 'axios';
import * as SecureStore from 'expo-secure-store';
import { API_BASE_URL } from '../utils/constants';

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: 15000,
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: true,
});

apiClient.interceptors.request.use(
  async config => {
    const token = await SecureStore.getItemAsync('auth_token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  error => Promise.reject(error),
);

let isRefreshing = false;
let failedQueue = [];

const processQueue = (error, token = null) => {
  failedQueue.forEach(prom => {
    if (error) {
      prom.reject(error);
    } else {
      prom.resolve(token);
    }
  });
  failedQueue = [];
};

apiClient.interceptors.response.use(
  async response => {
    const token = response.data?.token;
    if (token) {
      await SecureStore.setItemAsync('auth_token', token);
    }
    if (response.data?.refreshToken) {
      await SecureStore.setItemAsync('refresh_token', response.data.refreshToken);
    }
    return response;
  },
  async error => {
    const originalRequest = error.config;

    if (error.response?.status === 401 && !originalRequest._retry) {
      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject });
        }).then(token => {
          originalRequest.headers.Authorization = `Bearer ${token}`;
          return apiClient(originalRequest);
        });
      }

      originalRequest._retry = true;
      isRefreshing = true;

      const refreshToken = await SecureStore.getItemAsync('refresh_token');
      if (refreshToken) {
        try {
          const res = await axios.post(`${API_BASE_URL}/auth/refresh`, { refreshToken });
          const { accessToken, refreshToken: newRefresh } = res.data.data;
          await SecureStore.setItemAsync('auth_token', accessToken);
          await SecureStore.setItemAsync('refresh_token', newRefresh);
          processQueue(null, accessToken);
          originalRequest.headers.Authorization = `Bearer ${accessToken}`;
          return apiClient(originalRequest);
        } catch (refreshError) {
          processQueue(refreshError, null);
          await SecureStore.deleteItemAsync('auth_token');
          await SecureStore.deleteItemAsync('refresh_token');
          return Promise.reject(refreshError);
        } finally {
          isRefreshing = false;
        }
      }
    }

    return Promise.reject(error);
  },
);

export default apiClient;
```

- [ ] **Step 2: Copy all service files as-is**

Service files only import `apiClient` from `./client` — no changes needed.

```bash
mkdir -p expo-app/src/api
cp front-app/src/api/authService.js expo-app/src/api/authService.js
cp front-app/src/api/restaurantService.js expo-app/src/api/restaurantService.js
cp front-app/src/api/basketService.js expo-app/src/api/basketService.js
cp front-app/src/api/orderService.js expo-app/src/api/orderService.js
cp front-app/src/api/addressService.js expo-app/src/api/addressService.js
cp front-app/src/api/cuisineService.js expo-app/src/api/cuisineService.js
cp front-app/src/api/couponService.js expo-app/src/api/couponService.js
cp front-app/src/api/reviewService.js expo-app/src/api/reviewService.js
cp front-app/src/api/favoriteService.js expo-app/src/api/favoriteService.js
cp front-app/src/api/searchService.js expo-app/src/api/searchService.js
cp front-app/src/api/tipService.js expo-app/src/api/tipService.js
cp front-app/src/api/notificationService.js expo-app/src/api/notificationService.js
cp front-app/src/api/scheduledOrderService.js expo-app/src/api/scheduledOrderService.js
cp front-app/src/api/supportService.js expo-app/src/api/supportService.js
cp front-app/src/api/cardService.js expo-app/src/api/cardService.js
```

- [ ] **Step 3: Commit**

```bash
git add expo-app/src/api/
git commit -m "feat(expo-app): add API layer with SecureStore token management"
```

---

## Task 6: Migrate expo-courier API Layer

**Files:**
- Create: `expo-courier/src/api/client.js`
- Create: `expo-courier/src/api/courierService.js`
- Create: `expo-courier/src/api/companyService.js`

- [ ] **Step 1: Create API client with SecureStore**

Create `expo-courier/src/api/client.js`:

```javascript
import axios from 'axios';
import * as SecureStore from 'expo-secure-store';
import { API_BASE_URL } from '../utils/constants';

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: 15000,
  headers: {
    'Content-Type': 'application/json',
  },
});

apiClient.interceptors.request.use(
  async config => {
    const token = await SecureStore.getItemAsync('auth_token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  error => Promise.reject(error),
);

apiClient.interceptors.response.use(
  async response => {
    const token = response.data?.token;
    if (token) {
      await SecureStore.setItemAsync('auth_token', token);
    }
    if (response.data?.refreshToken) {
      await SecureStore.setItemAsync('refresh_token', response.data.refreshToken);
    }
    return response;
  },
  async error => {
    if (error.response?.status === 401) {
      await SecureStore.deleteItemAsync('auth_token');
      await SecureStore.deleteItemAsync('refresh_token');
    }
    return Promise.reject(error);
  },
);

export default apiClient;
```

- [ ] **Step 2: Copy service files as-is**

```bash
mkdir -p expo-courier/src/api
cp front-courier/src/api/courierService.js expo-courier/src/api/courierService.js
cp front-courier/src/api/companyService.js expo-courier/src/api/companyService.js
```

- [ ] **Step 3: Commit**

```bash
git add expo-courier/src/api/
git commit -m "feat(expo-courier): add API layer with SecureStore token management"
```

---

## Task 7: Migrate expo-app Contexts

**Files:**
- Create: `expo-app/src/context/AuthContext.js`
- Create: `expo-app/src/context/CartContext.js`
- Create: `expo-app/src/context/AppDataContext.js`
- Create: `expo-app/src/context/ToastContext.js`

- [ ] **Step 1: Create AuthContext with SecureStore + expo-notifications**

Create `expo-app/src/context/AuthContext.js`:

```javascript
import React, { createContext, useContext, useState, useEffect } from 'react';
import * as SecureStore from 'expo-secure-store';
import { authService } from '../api/authService';
import { registerForPushNotifications, clearPushToken } from '../utils/notifications';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  const checkAuth = async () => {
    try {
      const token = await SecureStore.getItemAsync('auth_token');
      if (token) {
        const response = await authService.getProfile();
        setUser(response.data.data);
        setIsAuthenticated(true);
      }
    } catch (error) {
      await SecureStore.deleteItemAsync('auth_token');
      await SecureStore.deleteItemAsync('refresh_token');
      setIsAuthenticated(false);
      setUser(null);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    checkAuth();
  }, []);

  const login = async (email, password) => {
    const response = await authService.login(email, password);
    const { token, refreshToken, user: userData } = response.data;
    await SecureStore.setItemAsync('auth_token', token);
    await SecureStore.setItemAsync('refresh_token', refreshToken);
    setUser(userData);
    setIsAuthenticated(true);
    registerForPushNotifications(userData.id);
    return response.data;
  };

  const register = async (data) => {
    const response = await authService.register(data);
    const { token, refreshToken, user: userData } = response.data;
    await SecureStore.setItemAsync('auth_token', token);
    await SecureStore.setItemAsync('refresh_token', refreshToken);
    setUser(userData);
    setIsAuthenticated(true);
    registerForPushNotifications(userData.id);
    return response.data;
  };

  const logout = async () => {
    try {
      await clearPushToken();
      const refreshToken = await SecureStore.getItemAsync('refresh_token');
      if (refreshToken) {
        await authService.logout(refreshToken);
      }
    } catch (e) {
      // Silent fail
    }
    await SecureStore.deleteItemAsync('auth_token');
    await SecureStore.deleteItemAsync('refresh_token');
    setUser(null);
    setIsAuthenticated(false);
  };

  const refreshProfile = async () => {
    try {
      const response = await authService.getProfile();
      setUser(response.data.data);
    } catch (e) {
      // Silent fail
    }
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        isLoading,
        isAuthenticated,
        login,
        register,
        logout,
        refreshProfile,
      }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
```

- [ ] **Step 2: Copy CartContext — replace AsyncStorage imports**

Copy `front-app/src/context/CartContext.js` to `expo-app/src/context/CartContext.js`.

Then do a search-and-replace in the file:
- If CartContext uses `AsyncStorage` for token access, replace with `SecureStore`.
- If it only uses `AsyncStorage` for non-sensitive cart data (like `onboarding_completed`), keep `AsyncStorage` for those and only change token reads to `SecureStore`.

```bash
cp front-app/src/context/CartContext.js expo-app/src/context/CartContext.js
```

Review the file and make targeted replacements where `AsyncStorage.getItem('auth_token')` appears.

- [ ] **Step 3: Create AppDataContext with expo-location**

Copy `front-app/src/context/AppDataContext.js` to `expo-app/src/context/AppDataContext.js`.

Then replace the `getDeviceLocation()` method. The original uses `react-native-geolocation-service`. Replace with `expo-location`:

In the copied file, change the import:
```javascript
// REMOVE: import Geolocation from 'react-native-geolocation-service';
// ADD:
import * as Location from 'expo-location';
```

Replace the `getDeviceLocation` function body:
```javascript
const getDeviceLocation = async () => {
  try {
    const { status } = await Location.requestForegroundPermissionsAsync();
    if (status !== 'granted') {
      return null;
    }
    const location = await Location.getCurrentPositionAsync({
      accuracy: Location.Accuracy.High,
    });
    return {
      latitude: location.coords.latitude,
      longitude: location.coords.longitude,
    };
  } catch (error) {
    console.log('Location error:', error);
    return null;
  }
};
```

Also replace any `AsyncStorage` usage for tokens with `SecureStore` if present.

- [ ] **Step 4: Copy ToastContext as-is**

```bash
cp front-app/src/context/ToastContext.js expo-app/src/context/ToastContext.js
```

No changes needed — ToastContext has no platform-specific dependencies.

- [ ] **Step 5: Commit**

```bash
git add expo-app/src/context/
git commit -m "feat(expo-app): add contexts with SecureStore, expo-location, expo-notifications"
```

---

## Task 8: Migrate expo-courier Contexts

**Files:**
- Create: `expo-courier/src/context/AuthContext.js`
- Create: `expo-courier/src/context/LocationContext.js`

- [ ] **Step 1: Create AuthContext with SecureStore + expo-router**

Create `expo-courier/src/context/AuthContext.js`:

```javascript
import React, { createContext, useContext, useState, useEffect } from 'react';
import * as SecureStore from 'expo-secure-store';
import { courierService } from '../api/courierService';
import apiClient from '../api/client';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  const checkAuth = async () => {
    try {
      const token = await SecureStore.getItemAsync('auth_token');
      if (token) {
        setIsAuthenticated(true);
        try {
          const response = await courierService.getProfile();
          setUser(response.data.data);
        } catch (profileError) {
          if (profileError.response?.status === 401) {
            await SecureStore.deleteItemAsync('auth_token');
            await SecureStore.deleteItemAsync('refresh_token');
            setIsAuthenticated(false);
          }
          // 404 = authenticated but no courier profile yet — user stays null
        }
      }
    } catch (error) {
      setIsAuthenticated(false);
      setUser(null);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    checkAuth();
  }, []);

  const login = async (email, password) => {
    try {
      const response = await apiClient.post('/auth/login', { email, password });
      const { token, refreshToken } = response.data;
      await SecureStore.setItemAsync('auth_token', token);
      await SecureStore.setItemAsync('refresh_token', refreshToken);
      setIsAuthenticated(true);

      try {
        const profileRes = await courierService.getProfile();
        setUser(profileRes.data.data);
        return { success: true, hasCourierProfile: true };
      } catch (e) {
        return { success: true, hasCourierProfile: false };
      }
    } catch (error) {
      return { success: false, error: error.response?.data?.message || 'Giriş başarısız' };
    }
  };

  const register = async (firstName, lastName, email, phoneNumber, password) => {
    try {
      const response = await apiClient.post('/auth/register', {
        firstName, lastName, email, phoneNumber, password,
      });
      const { token, refreshToken } = response.data;
      await SecureStore.setItemAsync('auth_token', token);
      await SecureStore.setItemAsync('refresh_token', refreshToken);
      setIsAuthenticated(true);
      return { success: true };
    } catch (error) {
      return { success: false, error: error.response?.data?.message || 'Kayıt başarısız' };
    }
  };

  const logout = async () => {
    await SecureStore.deleteItemAsync('auth_token');
    await SecureStore.deleteItemAsync('refresh_token');
    setUser(null);
    setIsAuthenticated(false);
  };

  const refreshProfile = async () => {
    try {
      const response = await courierService.getProfile();
      setUser(response.data.data);
    } catch (e) {
      // Silent fail
    }
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        isLoading,
        isAuthenticated,
        login,
        register,
        logout,
        refreshProfile,
      }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
```

- [ ] **Step 2: Create LocationContext with expo-location + expo-task-manager**

Create `expo-courier/src/context/LocationContext.js`:

```javascript
import React, { createContext, useContext, useState, useCallback } from 'react';
import { Alert } from 'react-native';
import * as Location from 'expo-location';
import { courierService } from '../api/courierService';
import {
  startBackgroundLocation,
  stopBackgroundLocation,
} from '../utils/backgroundLocation';

const LocationContext = createContext(null);

export const LocationProvider = ({ children }) => {
  const [currentPosition, setCurrentPosition] = useState(null);
  const [isTracking, setIsTracking] = useState(false);

  const getCurrentPosition = useCallback(async () => {
    const { status } = await Location.requestForegroundPermissionsAsync();
    if (status !== 'granted') {
      return null;
    }

    const location = await Location.getCurrentPositionAsync({
      accuracy: Location.Accuracy.High,
    });

    const coords = {
      latitude: location.coords.latitude,
      longitude: location.coords.longitude,
    };
    setCurrentPosition(coords);
    return coords;
  }, []);

  const startTracking = useCallback(async () => {
    if (isTracking) return true;

    // Get initial position and send to backend
    try {
      const coords = await getCurrentPosition();
      if (!coords) {
        Alert.alert(
          'Konum İzni Gerekli',
          'Çevrimiçi olabilmek için konum izni vermeniz gerekmektedir.',
        );
        return false;
      }
      await courierService.updateLocation(coords.latitude, coords.longitude);
    } catch (e) {
      Alert.alert('Konum Hatası', 'Konumunuz alınamadı. GPS açık olduğundan emin olun.');
      return false;
    }

    // Start background location updates
    const started = await startBackgroundLocation();
    if (!started) {
      Alert.alert(
        'Arka Plan Konum İzni',
        'Uygulama kapalıyken bile konum takibi için "Her zaman izin ver" seçeneğini seçmeniz gerekmektedir.',
      );
      return false;
    }

    // Also subscribe to foreground location changes for UI updates
    await Location.watchPositionAsync(
      {
        accuracy: Location.Accuracy.High,
        distanceInterval: 50,
        timeInterval: 10000,
      },
      (location) => {
        setCurrentPosition({
          latitude: location.coords.latitude,
          longitude: location.coords.longitude,
        });
      },
    );

    setIsTracking(true);
    return true;
  }, [isTracking, getCurrentPosition]);

  const stopTracking = useCallback(async () => {
    await stopBackgroundLocation();
    setIsTracking(false);
  }, []);

  return (
    <LocationContext.Provider
      value={{
        currentPosition,
        isTracking,
        startTracking,
        stopTracking,
        getCurrentPosition,
      }}>
      {children}
    </LocationContext.Provider>
  );
};

export const useLocation = () => {
  const context = useContext(LocationContext);
  if (!context) {
    throw new Error('useLocation must be used within a LocationProvider');
  }
  return context;
};
```

- [ ] **Step 3: Commit**

```bash
git add expo-courier/src/context/
git commit -m "feat(expo-courier): add AuthContext with SecureStore and LocationContext with background tracking"
```

---

## Task 9: Migrate expo-app Components

**Files:**
- Create: `expo-app/src/components/*.js` (all 14 component files)

- [ ] **Step 1: Copy all component files**

```bash
mkdir -p expo-app/src/components
cp front-app/src/components/*.js expo-app/src/components/
```

- [ ] **Step 2: Fix icon imports in all components**

In every component file that imports from `react-native-vector-icons/MaterialCommunityIcons`, replace:

```javascript
// BEFORE:
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';

// AFTER:
import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
```

Apply this to all files in `expo-app/src/components/` that have this import.

- [ ] **Step 3: Fix LinearGradient imports if present**

If any component imports `react-native-linear-gradient`, replace:

```javascript
// BEFORE:
import LinearGradient from 'react-native-linear-gradient';

// AFTER:
import { LinearGradient } from 'expo-linear-gradient';
```

- [ ] **Step 4: Commit**

```bash
git add expo-app/src/components/
git commit -m "feat(expo-app): migrate components with @expo/vector-icons"
```

---

## Task 10: Migrate expo-app Screens

**Files:**
- Copy all screen files from `front-app/src/screens/` to `expo-app/src/screens/`

- [ ] **Step 1: Copy all screen directories**

```bash
mkdir -p expo-app/src/screens
cp -r front-app/src/screens/* expo-app/src/screens/
```

- [ ] **Step 2: Fix icon imports in all screen files**

Find and replace in all `.js` files under `expo-app/src/screens/`:

```javascript
// BEFORE:
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';

// AFTER:
import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
```

- [ ] **Step 3: Fix navigation calls**

In all screen files, replace React Navigation patterns with expo-router:

```javascript
// BEFORE:
import { useNavigation } from '@react-navigation/native';
// ...
const navigation = useNavigation();
navigation.navigate('ScreenName', { param: value });
navigation.goBack();

// AFTER:
import { router } from 'expo-router';
// ...
// Remove useNavigation() call
router.push('/screen-path');  // or router.push({ pathname: '/screen-path', params: { param: value } })
router.back();
```

**Navigation mapping (screen name → expo-router path):**

| Old `navigation.navigate(...)` | New `router.push(...)` |
|---|---|
| `'Login'` | `'/(auth)/login'` |
| `'Register'` | `'/(auth)/register'` |
| `'ForgotPassword'` | `'/(auth)/forgot-password'` |
| `'VerifyCode', {email}` | `{ pathname: '/(auth)/verify-code', params: {email} }` |
| `'ResetPassword', {email, code}` | `{ pathname: '/(auth)/reset-password', params: {email, code} }` |
| `'RestaurantDetail', {id}` | `{ pathname: '/restaurant/[id]', params: {id} }` |
| `'OrderDetail', {orderId}` | `{ pathname: '/order/[id]', params: {id: orderId} }` |
| `'Cart'` | `'/cart'` (via tabs) |
| `'Checkout'` | `'/checkout'` |
| `'ThreeDsWebView', {url}` | `{ pathname: '/three-ds', params: {url} }` |
| `'AddressList'` | `'/address-list'` |
| `'AddAddress'` | `'/add-address'` |
| `'EditProfile'` | `'/edit-profile'` |
| `'ChangePassword'` | `'/change-password'` |
| `'SavedCards'` | `'/saved-cards'` |
| `'Favorites'` | `'/favorites'` |
| `'Notifications'` | `'/notifications'` |
| `'NotificationPreferences'` | `'/notification-preferences'` |
| `'ScheduledOrders'` | `'/scheduled-orders'` |
| `'Support'` | `'/support'` |
| `'SupportChat', {ticketId}` | `{ pathname: '/support-chat', params: {ticketId} }` |
| `'NewTicket'` | `'/new-ticket'` |
| `'OrderHistory'` | `'/(tabs)/orders'` |
| `'Home'` | `'/(tabs)'` |

Also replace `route.params` access:
```javascript
// BEFORE:
const { paramName } = route.params;

// AFTER:
import { useLocalSearchParams } from 'expo-router';
const { paramName } = useLocalSearchParams();
```

- [ ] **Step 4: Fix LinearGradient imports in screens**

```javascript
// BEFORE:
import LinearGradient from 'react-native-linear-gradient';

// AFTER:
import { LinearGradient } from 'expo-linear-gradient';
```

- [ ] **Step 5: Fix AsyncStorage to SecureStore for token access**

In any screen file that directly reads `auth_token` from `AsyncStorage`, switch to `SecureStore`:

```javascript
// BEFORE:
import AsyncStorage from '@react-native-async-storage/async-storage';
const token = await AsyncStorage.getItem('auth_token');

// AFTER:
import * as SecureStore from 'expo-secure-store';
const token = await SecureStore.getItemAsync('auth_token');
```

Keep `AsyncStorage` for non-sensitive data like `onboarding_completed`.

- [ ] **Step 6: Commit**

```bash
git add expo-app/src/screens/
git commit -m "feat(expo-app): migrate all screens with expo-router navigation and icon updates"
```

---

## Task 11: Migrate expo-courier Screens

**Files:**
- Copy all screen files from `front-courier/src/screens/` to `expo-courier/src/screens/`

- [ ] **Step 1: Copy all screen directories**

```bash
mkdir -p expo-courier/src/screens
cp -r front-courier/src/screens/* expo-courier/src/screens/
```

- [ ] **Step 2: Fix icon imports**

In all `.js` files under `expo-courier/src/screens/`:

```javascript
// BEFORE:
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';

// AFTER:
import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
```

- [ ] **Step 3: Fix navigation calls**

Replace React Navigation with expo-router:

**Navigation mapping:**

| Old `navigation.navigate(...)` | New `router.push(...)` |
|---|---|
| `'Login'` | `'/(auth)/login'` |
| `'Register'` | `'/(auth)/register'` |
| `'CourierRegister'` | `'/(setup)/courier-register'` |
| `'CompanyRegister'` | `'/(setup)/company-register'` |
| `'PendingApproval'` | `'/(setup)/pending-approval'` |
| `'Agreements'` | `'/agreements'` |

Replace `route.params` with `useLocalSearchParams()`.
Replace `useNavigation()` with `router` from `expo-router`.

- [ ] **Step 4: Fix LinearGradient imports if present**

Same pattern as Task 10.

- [ ] **Step 5: Commit**

```bash
git add expo-courier/src/screens/
git commit -m "feat(expo-courier): migrate all screens with expo-router navigation"
```

---

## Task 12: Create expo-app Router Layouts

**Files:**
- Create: `expo-app/app/_layout.js`
- Create: `expo-app/app/index.js`
- Create: `expo-app/app/onboarding.js`
- Create: `expo-app/app/(auth)/_layout.js`
- Create: `expo-app/app/(auth)/login.js` (and all auth routes)
- Create: `expo-app/app/(tabs)/_layout.js`
- Create: `expo-app/app/(tabs)/index.js` (and all tab routes)
- Create: All remaining route files

- [ ] **Step 1: Create root layout**

Create `expo-app/app/_layout.js`:

```javascript
import { useEffect } from 'react';
import { StatusBar } from 'react-native';
import { Stack } from 'expo-router';
import { GestureHandlerRootView } from 'react-native-gesture-handler';
import { SafeAreaProvider } from 'react-native-safe-area-context';
import { AuthProvider } from '../src/context/AuthContext';
import { AppDataProvider } from '../src/context/AppDataContext';
import { CartProvider } from '../src/context/CartContext';
import { ToastProvider } from '../src/context/ToastContext';
import { Colors } from '../src/theme';
import { initNotifications } from '../src/utils/notifications';

export default function RootLayout() {
  useEffect(() => {
    const subscription = initNotifications();
    return () => subscription?.remove();
  }, []);

  return (
    <GestureHandlerRootView style={{ flex: 1 }}>
      <SafeAreaProvider>
        <AuthProvider>
          <AppDataProvider>
            <CartProvider>
              <ToastProvider>
                <StatusBar barStyle="dark-content" backgroundColor={Colors.background} />
                <Stack screenOptions={{ headerShown: false }}>
                  <Stack.Screen name="index" />
                  <Stack.Screen name="onboarding" />
                  <Stack.Screen name="(auth)" />
                  <Stack.Screen name="(tabs)" />
                  <Stack.Screen name="restaurant/[id]" />
                  <Stack.Screen name="order/[id]" />
                  <Stack.Screen name="checkout" />
                  <Stack.Screen name="three-ds" />
                  <Stack.Screen name="address-list" />
                  <Stack.Screen name="add-address" />
                  <Stack.Screen name="edit-profile" />
                  <Stack.Screen name="change-password" />
                  <Stack.Screen name="saved-cards" />
                  <Stack.Screen name="favorites" />
                  <Stack.Screen name="notifications" />
                  <Stack.Screen name="notification-preferences" />
                  <Stack.Screen name="scheduled-orders" />
                  <Stack.Screen name="support" />
                  <Stack.Screen name="support-chat" />
                  <Stack.Screen name="new-ticket" />
                </Stack>
              </ToastProvider>
            </CartProvider>
          </AppDataProvider>
        </AuthProvider>
      </SafeAreaProvider>
    </GestureHandlerRootView>
  );
}
```

- [ ] **Step 2: Create index.js (entry redirect)**

Create `expo-app/app/index.js`:

```javascript
import { useEffect, useState } from 'react';
import { Redirect } from 'expo-router';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { useAuth } from '../src/context/AuthContext';
import { useAppData } from '../src/context/AppDataContext';
import SplashScreen from '../src/screens/Auth/SplashScreen';

export default function Index() {
  const { isLoading } = useAuth();
  const { dataReady, enableLoading } = useAppData();
  const [onboardingDone, setOnboardingDone] = useState(null);

  useEffect(() => {
    AsyncStorage.getItem('onboarding_completed').then(val => {
      const done = val === 'true';
      setOnboardingDone(done);
      if (done) enableLoading();
    });
  }, []);

  if (isLoading || onboardingDone === null) {
    return <SplashScreen />;
  }

  if (!onboardingDone) {
    return <Redirect href="/onboarding" />;
  }

  if (!dataReady) {
    return <SplashScreen />;
  }

  return <Redirect href="/(tabs)" />;
}
```

- [ ] **Step 3: Create onboarding route**

Create `expo-app/app/onboarding.js`:

```javascript
import { router } from 'expo-router';
import AsyncStorage from '@react-native-async-storage/async-storage';
import OnboardingScreen from '../src/screens/Onboarding/OnboardingScreen';

export default function Onboarding() {
  const handleComplete = async () => {
    await AsyncStorage.setItem('onboarding_completed', 'true');
    router.replace('/(tabs)');
  };

  return <OnboardingScreen onComplete={handleComplete} />;
}
```

- [ ] **Step 4: Create auth layout and routes**

Create `expo-app/app/(auth)/_layout.js`:

```javascript
import { Stack } from 'expo-router';

export default function AuthLayout() {
  return <Stack screenOptions={{ headerShown: false }} />;
}
```

Create `expo-app/app/(auth)/login.js`:

```javascript
import LoginScreen from '../../src/screens/Auth/LoginScreen';
export default LoginScreen;
```

Create `expo-app/app/(auth)/register.js`:

```javascript
import RegisterScreen from '../../src/screens/Auth/RegisterScreen';
export default RegisterScreen;
```

Create `expo-app/app/(auth)/forgot-password.js`:

```javascript
import ForgotPasswordScreen from '../../src/screens/Auth/ForgotPasswordScreen';
export default ForgotPasswordScreen;
```

Create `expo-app/app/(auth)/verify-code.js`:

```javascript
import VerifyCodeScreen from '../../src/screens/Auth/VerifyCodeScreen';
export default VerifyCodeScreen;
```

Create `expo-app/app/(auth)/reset-password.js`:

```javascript
import ResetPasswordScreen from '../../src/screens/Auth/ResetPasswordScreen';
export default ResetPasswordScreen;
```

- [ ] **Step 5: Create tabs layout**

Create `expo-app/app/(tabs)/_layout.js`:

```javascript
import { View, Text, StyleSheet } from 'react-native';
import { Tabs } from 'expo-router';
import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { useCart } from '../../src/context/CartContext';
import { Colors } from '../../src/theme';

const CartBadge = ({ count }) => {
  if (!count || count === 0) return null;
  return (
    <View style={styles.badge}>
      <Text style={styles.badgeText}>{count > 99 ? '99+' : count}</Text>
    </View>
  );
};

export default function TabLayout() {
  const { itemCount } = useCart();
  const insets = useSafeAreaInsets();

  return (
    <Tabs
      screenOptions={{
        headerShown: false,
        tabBarActiveTintColor: Colors.primary,
        tabBarInactiveTintColor: Colors.textTertiary,
        tabBarStyle: {
          ...styles.tabBar,
          paddingBottom: Math.max(insets.bottom, 8),
          height: 56 + Math.max(insets.bottom, 8),
        },
        tabBarLabelStyle: styles.tabBarLabel,
      }}>
      <Tabs.Screen
        name="index"
        options={{
          tabBarLabel: 'Ana Sayfa',
          tabBarIcon: ({ focused, color }) => (
            <Icon name={focused ? 'home' : 'home-outline'} size={focused ? 26 : 24} color={color} />
          ),
        }}
      />
      <Tabs.Screen
        name="cart"
        options={{
          tabBarLabel: 'Sepetim',
          tabBarIcon: ({ focused, color }) => (
            <View>
              <Icon name={focused ? 'cart' : 'cart-outline'} size={focused ? 26 : 24} color={color} />
              <CartBadge count={itemCount} />
            </View>
          ),
        }}
      />
      <Tabs.Screen
        name="orders"
        options={{
          tabBarLabel: 'Siparişler',
          tabBarIcon: ({ focused, color }) => (
            <Icon name="receipt" size={focused ? 26 : 24} color={color} />
          ),
        }}
      />
      <Tabs.Screen
        name="profile"
        options={{
          tabBarLabel: 'Profilim',
          tabBarIcon: ({ focused, color }) => (
            <Icon name={focused ? 'account' : 'account-outline'} size={focused ? 26 : 24} color={color} />
          ),
        }}
      />
    </Tabs>
  );
}

const styles = StyleSheet.create({
  tabBar: {
    backgroundColor: Colors.surface,
    borderTopWidth: 1,
    borderTopColor: Colors.borderLight,
    paddingTop: 8,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: -2 },
    shadowOpacity: 0.06,
    shadowRadius: 8,
    elevation: 10,
  },
  tabBarLabel: {
    fontSize: 11,
    fontWeight: '600',
    marginTop: 2,
  },
  badge: {
    position: 'absolute',
    right: -8,
    top: -4,
    backgroundColor: Colors.primary,
    borderRadius: 10,
    minWidth: 18,
    height: 18,
    justifyContent: 'center',
    alignItems: 'center',
    paddingHorizontal: 4,
    borderWidth: 2,
    borderColor: Colors.surface,
  },
  badgeText: {
    color: '#FFF',
    fontSize: 10,
    fontWeight: '700',
  },
});
```

- [ ] **Step 6: Create tab screen files**

Create `expo-app/app/(tabs)/index.js`:

```javascript
import HomeScreen from '../../src/screens/Home/HomeScreen';
export default HomeScreen;
```

Create `expo-app/app/(tabs)/cart.js`:

```javascript
import CartScreen from '../../src/screens/Cart/CartScreen';
export default CartScreen;
```

Create `expo-app/app/(tabs)/orders.js`:

```javascript
import OrderHistoryScreen from '../../src/screens/Orders/OrderHistoryScreen';
export default OrderHistoryScreen;
```

Create `expo-app/app/(tabs)/profile.js`:

```javascript
import ProfileScreen from '../../src/screens/Profile/ProfileScreen';
export default ProfileScreen;
```

- [ ] **Step 7: Create remaining route files**

Create `expo-app/app/restaurant/[id].js`:

```javascript
import RestaurantDetailScreen from '../../src/screens/Restaurant/RestaurantDetailScreen';
export default RestaurantDetailScreen;
```

Create `expo-app/app/order/[id].js`:

```javascript
import OrderDetailScreen from '../../src/screens/Orders/OrderDetailScreen';
export default OrderDetailScreen;
```

Create `expo-app/app/checkout.js`:

```javascript
import CheckoutScreen from '../../src/screens/Cart/CheckoutScreen';
export default CheckoutScreen;
```

Create `expo-app/app/three-ds.js`:

```javascript
import ThreeDsWebViewScreen from '../../src/screens/Payment/ThreeDsWebViewScreen';
export default ThreeDsWebViewScreen;
```

Create `expo-app/app/address-list.js`:

```javascript
import AddressListScreen from '../../src/screens/Profile/AddressListScreen';
export default AddressListScreen;
```

Create `expo-app/app/add-address.js`:

```javascript
import AddAddressScreen from '../../src/screens/Profile/AddAddressScreen';
export default AddAddressScreen;
```

Create `expo-app/app/edit-profile.js`:

```javascript
import EditProfileScreen from '../../src/screens/Profile/EditProfileScreen';
export default EditProfileScreen;
```

Create `expo-app/app/change-password.js`:

```javascript
import ChangePasswordScreen from '../../src/screens/Profile/ChangePasswordScreen';
export default ChangePasswordScreen;
```

Create `expo-app/app/saved-cards.js`:

```javascript
import SavedCardsScreen from '../../src/screens/Profile/SavedCardsScreen';
export default SavedCardsScreen;
```

Create `expo-app/app/favorites.js`:

```javascript
import FavoritesScreen from '../../src/screens/Profile/FavoritesScreen';
export default FavoritesScreen;
```

Create `expo-app/app/notifications.js`:

```javascript
import NotificationsScreen from '../../src/screens/Notifications/NotificationsScreen';
export default NotificationsScreen;
```

Create `expo-app/app/notification-preferences.js`:

```javascript
import NotificationPreferencesScreen from '../../src/screens/Notifications/NotificationPreferencesScreen';
export default NotificationPreferencesScreen;
```

Create `expo-app/app/scheduled-orders.js`:

```javascript
import ScheduledOrdersScreen from '../../src/screens/Orders/ScheduledOrdersScreen';
export default ScheduledOrdersScreen;
```

Create `expo-app/app/support.js`:

```javascript
import SupportScreen from '../../src/screens/Support/SupportScreen';
export default SupportScreen;
```

Create `expo-app/app/support-chat.js`:

```javascript
import SupportChatScreen from '../../src/screens/Support/SupportChatScreen';
export default SupportChatScreen;
```

Create `expo-app/app/new-ticket.js`:

```javascript
import NewTicketScreen from '../../src/screens/Support/NewTicketScreen';
export default NewTicketScreen;
```

- [ ] **Step 8: Commit**

```bash
git add expo-app/app/
git commit -m "feat(expo-app): create expo-router layouts and all route files"
```

---

## Task 13: Create expo-courier Router Layouts

**Files:**
- Create: `expo-courier/app/_layout.js`
- Create: `expo-courier/app/index.js`
- Create: `expo-courier/app/(auth)/_layout.js` and routes
- Create: `expo-courier/app/(setup)/_layout.js` and routes
- Create: `expo-courier/app/(tabs)/_layout.js` and routes
- Create: `expo-courier/app/agreements.js`

- [ ] **Step 1: Create root layout**

Create `expo-courier/app/_layout.js`:

```javascript
import { StatusBar } from 'react-native';
import { Stack } from 'expo-router';
import { GestureHandlerRootView } from 'react-native-gesture-handler';
import { SafeAreaProvider } from 'react-native-safe-area-context';
import { AuthProvider } from '../src/context/AuthContext';
import { LocationProvider } from '../src/context/LocationContext';
import { Colors } from '../src/theme';

// Import background location task definition (must be at top level)
import '../src/utils/backgroundLocation';

export default function RootLayout() {
  return (
    <GestureHandlerRootView style={{ flex: 1 }}>
      <SafeAreaProvider>
        <AuthProvider>
          <LocationProvider>
            <StatusBar barStyle="dark-content" backgroundColor={Colors.background} />
            <Stack screenOptions={{ headerShown: false }}>
              <Stack.Screen name="index" />
              <Stack.Screen name="(auth)" />
              <Stack.Screen name="(setup)" />
              <Stack.Screen name="(tabs)" />
              <Stack.Screen name="agreements" />
            </Stack>
          </LocationProvider>
        </AuthProvider>
      </SafeAreaProvider>
    </GestureHandlerRootView>
  );
}
```

- [ ] **Step 2: Create index.js (auth-based redirect)**

Create `expo-courier/app/index.js`:

```javascript
import { Redirect } from 'expo-router';
import { View, ActivityIndicator, Text, StyleSheet } from 'react-native';
import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import { useAuth } from '../src/context/AuthContext';
import { Colors } from '../src/theme';

export default function Index() {
  const { isLoading, isAuthenticated, user } = useAuth();

  if (isLoading) {
    return (
      <View style={styles.splashContainer}>
        <Icon name="motorbike" size={64} color={Colors.primary} />
        <Text style={styles.splashTitle}>Kurye Paneli</Text>
        <ActivityIndicator size="large" color={Colors.primary} style={styles.splashLoader} />
      </View>
    );
  }

  if (!isAuthenticated) {
    return <Redirect href="/(auth)/login" />;
  }

  if (!user) {
    return <Redirect href="/(setup)/courier-register" />;
  }

  return <Redirect href="/(tabs)" />;
}

const styles = StyleSheet.create({
  splashContainer: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: Colors.background,
  },
  splashTitle: {
    fontSize: 24,
    fontWeight: '700',
    color: Colors.text,
    marginTop: 16,
  },
  splashLoader: {
    marginTop: 24,
  },
});
```

- [ ] **Step 3: Create auth layout and routes**

Create `expo-courier/app/(auth)/_layout.js`:

```javascript
import { Stack } from 'expo-router';
export default function AuthLayout() {
  return <Stack screenOptions={{ headerShown: false }} />;
}
```

Create `expo-courier/app/(auth)/login.js`:

```javascript
import LoginScreen from '../../src/screens/Auth/LoginScreen';
export default LoginScreen;
```

Create `expo-courier/app/(auth)/register.js`:

```javascript
import RegisterScreen from '../../src/screens/Auth/RegisterScreen';
export default RegisterScreen;
```

- [ ] **Step 4: Create setup layout and routes**

Create `expo-courier/app/(setup)/_layout.js`:

```javascript
import { Stack } from 'expo-router';
export default function SetupLayout() {
  return <Stack screenOptions={{ headerShown: false }} />;
}
```

Create `expo-courier/app/(setup)/courier-register.js`:

```javascript
import CourierRegisterScreen from '../../src/screens/Auth/CourierRegisterScreen';
export default CourierRegisterScreen;
```

Create `expo-courier/app/(setup)/company-register.js`:

```javascript
import CompanyRegisterScreen from '../../src/screens/Auth/CompanyRegisterScreen';
export default CompanyRegisterScreen;
```

Create `expo-courier/app/(setup)/pending-approval.js`:

```javascript
import PendingApprovalScreen from '../../src/screens/Auth/PendingApprovalScreen';
export default PendingApprovalScreen;
```

- [ ] **Step 5: Create tabs layout**

Create `expo-courier/app/(tabs)/_layout.js`:

```javascript
import { StyleSheet } from 'react-native';
import { Tabs } from 'expo-router';
import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { Colors } from '../../src/theme';

export default function TabLayout() {
  const insets = useSafeAreaInsets();

  return (
    <Tabs
      screenOptions={{
        headerShown: false,
        tabBarActiveTintColor: Colors.primary,
        tabBarInactiveTintColor: Colors.textTertiary,
        tabBarStyle: {
          ...styles.tabBar,
          paddingBottom: Math.max(insets.bottom, 8),
          height: 56 + Math.max(insets.bottom, 8),
        },
        tabBarLabelStyle: styles.tabBarLabel,
      }}>
      <Tabs.Screen
        name="index"
        options={{
          tabBarLabel: 'Ana Sayfa',
          tabBarIcon: ({ focused, color }) => (
            <Icon name={focused ? 'home' : 'home-outline'} size={focused ? 26 : 24} color={color} />
          ),
        }}
      />
      <Tabs.Screen
        name="deliveries"
        options={{
          tabBarLabel: 'Teslimatlar',
          tabBarIcon: ({ focused, color }) => (
            <Icon
              name={focused ? 'package-variant-closed' : 'package-variant'}
              size={focused ? 26 : 24}
              color={color}
            />
          ),
        }}
      />
      <Tabs.Screen
        name="earnings"
        options={{
          tabBarLabel: 'Kazançlar',
          tabBarIcon: ({ focused, color }) => (
            <Icon
              name={focused ? 'cash-multiple' : 'cash'}
              size={focused ? 26 : 24}
              color={color}
            />
          ),
        }}
      />
      <Tabs.Screen
        name="profile"
        options={{
          tabBarLabel: 'Profilim',
          tabBarIcon: ({ focused, color }) => (
            <Icon name={focused ? 'account' : 'account-outline'} size={focused ? 26 : 24} color={color} />
          ),
        }}
      />
    </Tabs>
  );
}

const styles = StyleSheet.create({
  tabBar: {
    backgroundColor: '#FFFFFF',
    borderTopWidth: 1,
    borderTopColor: '#F0F0F0',
    paddingTop: 8,
    elevation: 8,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: -2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
  },
  tabBarLabel: {
    fontSize: 11,
    fontWeight: '600',
    marginTop: 2,
  },
});
```

- [ ] **Step 6: Create tab screen files**

Create `expo-courier/app/(tabs)/index.js`:

```javascript
import DashboardScreen from '../../src/screens/Dashboard/DashboardScreen';
export default DashboardScreen;
```

Create `expo-courier/app/(tabs)/deliveries.js`:

```javascript
import DeliveryHistoryScreen from '../../src/screens/Delivery/DeliveryHistoryScreen';
export default DeliveryHistoryScreen;
```

Create `expo-courier/app/(tabs)/earnings.js`:

```javascript
import EarningsScreen from '../../src/screens/Earnings/EarningsScreen';
export default EarningsScreen;
```

Create `expo-courier/app/(tabs)/profile.js`:

```javascript
import ProfileScreen from '../../src/screens/Profile/ProfileScreen';
export default ProfileScreen;
```

- [ ] **Step 7: Create agreements route**

Create `expo-courier/app/agreements.js`:

```javascript
import AgreementsScreen from '../src/screens/Agreements/AgreementsScreen';
export default AgreementsScreen;
```

- [ ] **Step 8: Commit**

```bash
git add expo-courier/app/
git commit -m "feat(expo-courier): create expo-router layouts with auth/setup/tabs routing"
```

---

## Task 14: Verify expo-app Builds

- [ ] **Step 1: Install dependencies and verify**

```bash
cd /Users/onuryorulmaz/Desktop/Github/food-order/expo-app
npm install
npx expo doctor
```

Fix any issues reported by `expo doctor`.

- [ ] **Step 2: Run Metro bundler to check for import errors**

```bash
cd /Users/onuryorulmaz/Desktop/Github/food-order/expo-app
npx expo start --clear
```

Check the terminal output for any import resolution errors or missing modules. Fix any issues found.

- [ ] **Step 3: Build Android APK**

```bash
cd /Users/onuryorulmaz/Desktop/Github/food-order/expo-app
npx expo prebuild --clean
npx expo run:android
```

Alternatively for a release APK:
```bash
eas build -p android --profile preview --local
```

- [ ] **Step 4: Fix any build errors and commit**

```bash
git add expo-app/
git commit -m "fix(expo-app): resolve build errors and verify Metro bundling"
```

---

## Task 15: Verify expo-courier Builds

- [ ] **Step 1: Install dependencies and verify**

```bash
cd /Users/onuryorulmaz/Desktop/Github/food-order/expo-courier
npm install
npx expo doctor
```

- [ ] **Step 2: Run Metro bundler**

```bash
cd /Users/onuryorulmaz/Desktop/Github/food-order/expo-courier
npx expo start --clear
```

- [ ] **Step 3: Build Android APK**

```bash
cd /Users/onuryorulmaz/Desktop/Github/food-order/expo-courier
npx expo prebuild --clean
npx expo run:android
```

- [ ] **Step 4: Fix any build errors and commit**

```bash
git add expo-courier/
git commit -m "fix(expo-courier): resolve build errors and verify Metro bundling"
```

---

## Parallelization Guide

Tasks can be executed in parallel as follows:

| Phase | Tasks | Description |
|-------|-------|-------------|
| **Phase 1** | Task 1 + Task 2 | Project scaffolds (parallel) |
| **Phase 2** | Task 3 + Task 4 | Theme + utils (parallel) |
| **Phase 3** | Task 5 + Task 6 | API layers (parallel) |
| **Phase 4** | Task 7 + Task 8 | Contexts (parallel) |
| **Phase 5** | Task 9 + Task 10 + Task 11 | Components + screens (parallel) |
| **Phase 6** | Task 12 + Task 13 | Router layouts (parallel) |
| **Phase 7** | Task 14 + Task 15 | Build verification (parallel) |

Each phase depends on the previous phase completing. Within each phase, tasks are fully independent and can run in parallel.
