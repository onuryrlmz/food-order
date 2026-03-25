import React from 'react';
import {View, Text, StyleSheet, ActivityIndicator} from 'react-native';
import {NavigationContainer} from '@react-navigation/native';
import {createNativeStackNavigator} from '@react-navigation/native-stack';
import {createBottomTabNavigator} from '@react-navigation/bottom-tabs';
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

const AuthStackNav = createNativeStackNavigator();
const SetupStackNav = createNativeStackNavigator();
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

// Main tabs: authenticated with courier profile
const MainTabs = () => (
  <Tab.Navigator
    screenOptions={({route}) => ({
      headerShown: false,
      tabBarActiveTintColor: Colors.primary,
      tabBarInactiveTintColor: Colors.textTertiary,
      tabBarStyle: styles.tabBar,
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
      component={ProfileScreen}
      options={{tabBarLabel: 'Profilim'}}
    />
  </Tab.Navigator>
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
        <MainTabs />
      )}
    </NavigationContainer>
  );
};

const styles = StyleSheet.create({
  tabBar: {
    backgroundColor: Colors.surface,
    borderTopWidth: 1,
    borderTopColor: Colors.borderLight,
    paddingTop: 8,
    paddingBottom: 8,
    height: 64,
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
