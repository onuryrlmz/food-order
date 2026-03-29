import React from 'react';
import {View, Text, StyleSheet, ActivityIndicator, Platform} from 'react-native';
import {NavigationContainer} from '@react-navigation/native';
import {createNativeStackNavigator} from '@react-navigation/native-stack';
import {createBottomTabNavigator} from '@react-navigation/bottom-tabs';
import {useSafeAreaInsets} from 'react-native-safe-area-context';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';

import {useAuth} from '../context/AuthContext';
import {Colors} from '../theme';

import LoginScreen from '../screens/Auth/LoginScreen';
import RegisterScreen from '../screens/Auth/RegisterScreen';
import CourierRegisterScreen from '../screens/Auth/CourierRegisterScreen';
import CompanyRegisterScreen from '../screens/Auth/CompanyRegisterScreen';
import PendingApprovalScreen from '../screens/Auth/PendingApprovalScreen';
import DashboardScreen from '../screens/Dashboard/DashboardScreen';
import DeliveryHistoryScreen from '../screens/Delivery/DeliveryHistoryScreen';
import EarningsScreen from '../screens/Earnings/EarningsScreen';
import ProfileScreen from '../screens/Profile/ProfileScreen';
import AgreementsScreen from '../screens/Agreements/AgreementsScreen';
import ActiveDeliveryScreen from '../screens/Delivery/ActiveDeliveryScreen';

const AuthStackNav = createNativeStackNavigator();
const SetupStackNav = createNativeStackNavigator();
const ProfileStackNav = createNativeStackNavigator();
const MainStackNav = createNativeStackNavigator();
const Tab = createBottomTabNavigator();

// Auth screens: Login & Register (no courier profile, no token)
const AuthStack = () => (
  <AuthStackNav.Navigator screenOptions={{headerShown: false}}>
    <AuthStackNav.Screen name="Login" component={LoginScreen} />
    <AuthStackNav.Screen name="Register" component={RegisterScreen} />
  </AuthStackNav.Navigator>
);

// Setup screens: authenticated but no courier profile yet
const CourierSetupStack = () => (
  <SetupStackNav.Navigator screenOptions={{headerShown: false}}>
    <SetupStackNav.Screen name="CourierRegister" component={CourierRegisterScreen} />
    <SetupStackNav.Screen name="CompanyRegister" component={CompanyRegisterScreen} />
    <SetupStackNav.Screen name="PendingApproval" component={PendingApprovalScreen} />
  </SetupStackNav.Navigator>
);

// Profile stack: Profile + sub-screens
const ProfileStack = () => (
  <ProfileStackNav.Navigator screenOptions={{headerShown: false}}>
    <ProfileStackNav.Screen name="ProfileMain" component={ProfileScreen} />
    <ProfileStackNav.Screen name="Agreements" component={AgreementsScreen} />
  </ProfileStackNav.Navigator>
);

// Main tabs: authenticated with courier profile
const MainTabs = () => {
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
      tabBarIcon: ({focused, color}) => {
        let iconName;
        if (route.name === 'DashboardTab') {
          iconName = focused ? 'home' : 'home-outline';
        } else if (route.name === 'DeliveryTab') {
          iconName = focused ? 'package-variant-closed' : 'package-variant';
        } else if (route.name === 'EarningsTab') {
          iconName = focused ? 'cash-multiple' : 'cash';
        } else if (route.name === 'ProfileTab') {
          iconName = focused ? 'account' : 'account-outline';
        }
        return <Icon name={iconName} size={focused ? 26 : 24} color={color} />;
      },
    })}>
    <Tab.Screen
      name="DashboardTab"
      component={DashboardScreen}
      options={{tabBarLabel: 'Ana Sayfa'}}
    />
    <Tab.Screen
      name="DeliveryTab"
      component={DeliveryHistoryScreen}
      options={{tabBarLabel: 'Teslimatlar'}}
    />
    <Tab.Screen
      name="EarningsTab"
      component={EarningsScreen}
      options={{tabBarLabel: 'Kazançlar'}}
    />
    <Tab.Screen
      name="ProfileTab"
      component={ProfileStack}
      options={{tabBarLabel: 'Profilim'}}
    />
  </Tab.Navigator>
  );
};

// Main stack: tabs + full-screen routes accessible from any tab
const MainStack = () => (
  <MainStackNav.Navigator screenOptions={{headerShown: false}}>
    <MainStackNav.Screen name="MainTabs" component={MainTabs} />
    <MainStackNav.Screen name="ActiveDelivery" component={ActiveDeliveryScreen} />
  </MainStackNav.Navigator>
);

const SplashView = () => (
  <View style={styles.splashContainer}>
    <Icon name="motorbike" size={64} color={Colors.primary} />
    <Text style={styles.splashTitle}>Kurye Paneli</Text>
    <ActivityIndicator size="large" color={Colors.primary} style={styles.splashLoader} />
  </View>
);

const AppNavigator = () => {
  const {isLoading, isAuthenticated, user} = useAuth();

  if (isLoading) {
    return <SplashView />;
  }

  return (
    <NavigationContainer>
      {!isAuthenticated ? (
        <AuthStack />
      ) : isAuthenticated && !user ? (
        <CourierSetupStack />
      ) : (
        <MainStack />
      )}
    </NavigationContainer>
  );
};

const styles = StyleSheet.create({
  tabBar: {
    backgroundColor: '#FFFFFF',
    borderTopWidth: 1,
    borderTopColor: '#F0F0F0',
    paddingTop: 8,
    elevation: 8,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: -2},
    shadowOpacity: 0.1,
    shadowRadius: 4,
  },
  tabBarLabel: {
    fontSize: 11,
    fontWeight: '600',
    marginTop: 2,
  },
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

export default AppNavigator;
