import React from 'react';
import {createNativeStackNavigator} from '@react-navigation/native-stack';
import {createBottomTabNavigator} from '@react-navigation/bottom-tabs';
import MaterialCommunityIcons from 'react-native-vector-icons/MaterialCommunityIcons';
import {useAuth} from '../context/AuthContext';
import LoginScreen from '../screens/Auth/LoginScreen';
import RegisterScreen from '../screens/Auth/RegisterScreen';
import ActiveOrdersScreen from '../screens/Orders/ActiveOrdersScreen';
import OrderHistoryScreen from '../screens/Orders/OrderHistoryScreen';
import PickupScreen from '../screens/Orders/PickupScreen';
import OrderMapScreen from '../screens/Orders/OrderMapScreen';
import MyRestaurantsScreen from '../screens/Restaurants/MyRestaurantsScreen';
import ProfileScreen from '../screens/Profile/ProfileScreen';
import CompanyRegistrationScreen from '../screens/Company/CompanyRegistrationScreen';
import CompanyDashboardScreen from '../screens/Company/CompanyDashboardScreen';
import MemberManagementScreen from '../screens/Company/MemberManagementScreen';
import RestaurantInvitesScreen from '../screens/Company/RestaurantInvitesScreen';
import {ActivityIndicator, View} from 'react-native';

const Stack = createNativeStackNavigator();
const Tab = createBottomTabNavigator();
const CompanyStack = createNativeStackNavigator();
const OrdersStack = createNativeStackNavigator();

function CompanyStackNavigator() {
  return (
    <CompanyStack.Navigator
      screenOptions={{
        headerStyle: {backgroundColor: '#FF6B00'},
        headerTintColor: '#fff',
        headerTitleStyle: {fontWeight: '700', fontSize: 18},
      }}>
      <CompanyStack.Screen
        name="CompanyDashboard"
        component={CompanyDashboardScreen}
        options={{headerTitle: 'Firma'}}
      />
      <CompanyStack.Screen
        name="MemberManagement"
        component={MemberManagementScreen}
        options={{headerTitle: 'Kurye Yönetimi'}}
      />
      <CompanyStack.Screen
        name="RestaurantInvites"
        component={RestaurantInvitesScreen}
        options={{headerTitle: 'Restoran Davetleri'}}
      />
    </CompanyStack.Navigator>
  );
}

function OrdersStackNavigator() {
  return (
    <OrdersStack.Navigator
      screenOptions={{
        headerStyle: {backgroundColor: '#FF6B00'},
        headerTintColor: '#fff',
        headerTitleStyle: {fontWeight: '700', fontSize: 18},
      }}>
      <OrdersStack.Screen
        name="ActiveOrdersList"
        component={ActiveOrdersScreen}
        options={{headerTitle: 'Aktif Siparisler'}}
      />
      <OrdersStack.Screen
        name="Pickup"
        component={PickupScreen}
        options={{headerTitle: 'Teslim Al'}}
      />
      <OrdersStack.Screen
        name="OrderMap"
        component={OrderMapScreen}
        options={{headerTitle: 'Harita'}}
      />
    </OrdersStack.Navigator>
  );
}

function MainTabs() {
  const {courier} = useAuth();
  const isCompanyAdmin = courier?.userRoleId === 7;

  return (
    <Tab.Navigator
      screenOptions={{
        tabBarActiveTintColor: '#FF6B00',
        tabBarInactiveTintColor: '#999',
        tabBarStyle: {
          backgroundColor: '#fff',
          borderTopWidth: 1,
          borderTopColor: '#eee',
        },
        headerStyle: {
          backgroundColor: '#FF6B00',
        },
        headerTintColor: '#fff',
        headerTitleStyle: {
          fontWeight: '700',
          fontSize: 18,
        },
        headerShown: false,
      }}>
      <Tab.Screen
        name="ActiveOrders"
        component={OrdersStackNavigator}
        options={{
          tabBarLabel: 'Aktif',
          tabBarIcon: ({color, size}) => (
            <MaterialCommunityIcons
              name="bike-fast"
              color={color}
              size={size}
            />
          ),
        }}
      />
      <Tab.Screen
        name="OrderHistory"
        component={OrderHistoryScreen}
        options={{
          headerShown: true,
          headerTitle: 'Siparis Gecmisi',
          tabBarLabel: 'Gecmis',
          tabBarIcon: ({color, size}) => (
            <MaterialCommunityIcons
              name="history"
              color={color}
              size={size}
            />
          ),
        }}
      />
      <Tab.Screen
        name="MyRestaurants"
        component={MyRestaurantsScreen}
        options={{
          headerShown: true,
          headerTitle: 'Restoranlarim',
          tabBarLabel: 'Restoranlar',
          tabBarIcon: ({color, size}) => (
            <MaterialCommunityIcons
              name="store"
              color={color}
              size={size}
            />
          ),
        }}
      />
      {isCompanyAdmin && (
        <Tab.Screen
          name="Company"
          component={CompanyStackNavigator}
          options={{
            tabBarLabel: 'Firma',
            tabBarIcon: ({color, size}) => (
              <MaterialCommunityIcons
                name="office-building"
                color={color}
                size={size}
              />
            ),
          }}
        />
      )}
      <Tab.Screen
        name="Profile"
        component={ProfileScreen}
        options={{
          headerShown: true,
          headerTitle: 'Profil',
          tabBarLabel: 'Profil',
          tabBarIcon: ({color, size}) => (
            <MaterialCommunityIcons
              name="account"
              color={color}
              size={size}
            />
          ),
        }}
      />
    </Tab.Navigator>
  );
}

export default function AppNavigator() {
  const {isAuthenticated, loading} = useAuth();

  if (loading) {
    return (
      <View style={{flex: 1, justifyContent: 'center', alignItems: 'center'}}>
        <ActivityIndicator size="large" color="#FF6B00" />
      </View>
    );
  }

  return (
    <Stack.Navigator screenOptions={{headerShown: false}}>
      {isAuthenticated ? (
        <Stack.Screen name="Main" component={MainTabs} />
      ) : (
        <>
          <Stack.Screen name="Login" component={LoginScreen} />
          <Stack.Screen name="Register" component={RegisterScreen} />
          <Stack.Screen
            name="CompanyRegistration"
            component={CompanyRegistrationScreen}
            options={{headerShown: true, headerTitle: 'Firma Kayit'}}
          />
        </>
      )}
    </Stack.Navigator>
  );
}
