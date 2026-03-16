import React, {useState, useEffect} from 'react';
import {View, Text, StyleSheet, Platform} from 'react-native';
import {useSafeAreaInsets} from 'react-native-safe-area-context';
import {NavigationContainer} from '@react-navigation/native';
import {createNativeStackNavigator} from '@react-navigation/native-stack';
import {createBottomTabNavigator} from '@react-navigation/bottom-tabs';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import AsyncStorage from '@react-native-async-storage/async-storage';

import {useAuth} from '../context/AuthContext';
import {useCart} from '../context/CartContext';
import {useAppData} from '../context/AppDataContext';
import {Colors, Fonts} from '../theme';

import SplashScreen from '../screens/Auth/SplashScreen';
import OnboardingScreen from '../screens/Onboarding/OnboardingScreen';
import LoginScreen from '../screens/Auth/LoginScreen';
import RegisterScreen from '../screens/Auth/RegisterScreen';
import ForgotPasswordScreen from '../screens/Auth/ForgotPasswordScreen';
import VerifyCodeScreen from '../screens/Auth/VerifyCodeScreen';
import ResetPasswordScreen from '../screens/Auth/ResetPasswordScreen';

import HomeScreen from '../screens/Home/HomeScreen';
import RestaurantDetailScreen from '../screens/Restaurant/RestaurantDetailScreen';

import CartScreen from '../screens/Cart/CartScreen';
import CheckoutScreen from '../screens/Cart/CheckoutScreen';

import OrderHistoryScreen from '../screens/Orders/OrderHistoryScreen';
import OrderDetailScreen from '../screens/Orders/OrderDetailScreen';

import ProfileScreen from '../screens/Profile/ProfileScreen';
import EditProfileScreen from '../screens/Profile/EditProfileScreen';
import ChangePasswordScreen from '../screens/Profile/ChangePasswordScreen';
import AddressListScreen from '../screens/Profile/AddressListScreen';
import AddAddressScreen from '../screens/Profile/AddAddressScreen';
import SavedCardsScreen from '../screens/Profile/SavedCardsScreen';
import ThreeDsWebViewScreen from '../screens/Payment/ThreeDsWebViewScreen';

const Stack = createNativeStackNavigator();
const Tab = createBottomTabNavigator();
const HomeStack = createNativeStackNavigator();
const OrderStack = createNativeStackNavigator();
const ProfileStack = createNativeStackNavigator();

const HomeStackNavigator = () => (
  <HomeStack.Navigator screenOptions={{headerShown: false}}>
    <HomeStack.Screen name="Home" component={HomeScreen} />
    <HomeStack.Screen name="RestaurantDetail" component={RestaurantDetailScreen} />
    <HomeStack.Screen name="OrderDetail" component={OrderDetailScreen} />
    <HomeStack.Screen name="OrderHistory" component={OrderHistoryScreen} />
  </HomeStack.Navigator>
);

const OrderStackNavigator = () => (
  <OrderStack.Navigator screenOptions={{headerShown: false}}>
    <OrderStack.Screen name="OrderHistoryList" component={OrderHistoryScreen} />
    <OrderStack.Screen name="OrderDetail" component={OrderDetailScreen} />
  </OrderStack.Navigator>
);

const ProfileStackNavigator = () => (
  <ProfileStack.Navigator screenOptions={{headerShown: false}}>
    <ProfileStack.Screen name="ProfileMain" component={ProfileScreen} />
    <ProfileStack.Screen name="EditProfile" component={EditProfileScreen} />
    <ProfileStack.Screen name="ChangePassword" component={ChangePasswordScreen} />
    <ProfileStack.Screen name="AddressList" component={AddressListScreen} />
    <ProfileStack.Screen name="AddAddress" component={AddAddressScreen} />
    <ProfileStack.Screen name="SavedCards" component={SavedCardsScreen} />
    <ProfileStack.Screen name="OrderHistory" component={OrderHistoryScreen} />
    <ProfileStack.Screen name="OrderDetail" component={OrderDetailScreen} />
  </ProfileStack.Navigator>
);

const CartBadge = ({count}) => {
  if (!count || count === 0) return null;
  return (
    <View style={styles.badge}>
      <Text style={styles.badgeText}>{count > 99 ? '99+' : count}</Text>
    </View>
  );
};

const TabNavigator = () => {
  const {itemCount} = useCart();
  const insets = useSafeAreaInsets();

  return (
    <Tab.Navigator
      screenOptions={({route}) => ({
        headerShown: false,
        tabBarActiveTintColor: Colors.primary,
        tabBarInactiveTintColor: Colors.textTertiary,
        tabBarStyle: {
          ...styles.tabBar,
          paddingBottom: Math.max(insets.bottom, 8),
          height: 56 + Math.max(insets.bottom, 8),
        },
        tabBarLabelStyle: styles.tabBarLabel,
        tabBarIcon: ({focused, color, size}) => {
          let iconName;
          if (route.name === 'HomeTab') {
            iconName = focused ? 'home' : 'home-outline';
          } else if (route.name === 'SearchTab') {
            iconName = focused ? 'magnify' : 'magnify';
          } else if (route.name === 'OrdersTab') {
            iconName = focused ? 'receipt' : 'receipt';
          } else if (route.name === 'ProfileTab') {
            iconName = focused ? 'account' : 'account-outline';
          }
          return <Icon name={iconName} size={focused ? 26 : 24} color={color} />;
        },
      })}
    >
      <Tab.Screen
        name="HomeTab"
        component={HomeStackNavigator}
        options={{tabBarLabel: 'Ana Sayfa'}}
      />
      <Tab.Screen
        name="CartTab"
        component={CartScreen}
        options={{
          tabBarLabel: 'Sepetim',
          tabBarIcon: ({focused, color}) => (
            <View>
              <Icon name={focused ? 'cart' : 'cart-outline'} size={focused ? 26 : 24} color={color} />
              <CartBadge count={itemCount} />
            </View>
          ),
        }}
      />
      <Tab.Screen
        name="OrdersTab"
        component={OrderStackNavigator}
        options={{tabBarLabel: 'Siparişler'}}
      />
      <Tab.Screen
        name="ProfileTab"
        component={ProfileStackNavigator}
        options={{tabBarLabel: 'Profilim'}}
      />
    </Tab.Navigator>
  );
};

const AppNavigator = () => {
  const {isLoading, isAuthenticated} = useAuth();
  const {dataReady, enableLoading} = useAppData();
  const [onboardingDone, setOnboardingDone] = useState(null);

  useEffect(() => {
    AsyncStorage.getItem('onboarding_completed').then(val => {
      const done = val === 'true';
      setOnboardingDone(done);
      if (done) enableLoading();
    });
  }, []);

  const handleOnboardingComplete = () => {
    setOnboardingDone(true);
    enableLoading();
  };

  // Auth yükleniyor veya onboarding durumu henüz bilinmiyor
  if (isLoading || onboardingDone === null) {
    return <SplashScreen />;
  }

  // Onboarding yapılmamış
  if (!onboardingDone) {
    return <OnboardingScreen onComplete={handleOnboardingComplete} />;
  }

  // Onboarding tamam ama veriler henüz yüklenmedi
  if (!dataReady) {
    return <SplashScreen />;
  }

  return (
    <NavigationContainer>
      <Stack.Navigator screenOptions={{headerShown: false}}>
        <Stack.Screen name="MainTabs" component={TabNavigator} />
        <Stack.Screen name="RestaurantDetail" component={RestaurantDetailScreen} />
        <Stack.Screen name="Login" component={LoginScreen} />
        <Stack.Screen name="Register" component={RegisterScreen} />
        <Stack.Screen name="ForgotPassword" component={ForgotPasswordScreen} />
        <Stack.Screen name="VerifyCode" component={VerifyCodeScreen} />
        <Stack.Screen name="ResetPassword" component={ResetPasswordScreen} />
        <Stack.Screen
          name="Cart"
          component={CartScreen}
          options={{presentation: 'modal', animation: 'slide_from_bottom'}}
        />
        <Stack.Screen name="Checkout" component={CheckoutScreen} />
        <Stack.Screen name="ThreeDsWebView" component={ThreeDsWebViewScreen} />
        <Stack.Screen name="OrderDetail" component={OrderDetailScreen} />
        <Stack.Screen name="OrderHistory" component={OrderHistoryScreen} />
        <Stack.Screen name="AddressList" component={AddressListScreen} />
        <Stack.Screen name="AddAddress" component={AddAddressScreen} />
        <Stack.Screen name="EditProfile" component={EditProfileScreen} />
        <Stack.Screen name="ChangePassword" component={ChangePasswordScreen} />
      </Stack.Navigator>
    </NavigationContainer>
  );
};

const styles = StyleSheet.create({
  tabBar: {
    backgroundColor: Colors.surface,
    borderTopWidth: 1,
    borderTopColor: Colors.borderLight,
    paddingTop: 8,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: -2},
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

export default AppNavigator;
