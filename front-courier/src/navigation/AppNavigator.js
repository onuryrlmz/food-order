import React from 'react';
import {createNativeStackNavigator} from '@react-navigation/native-stack';
import {createBottomTabNavigator} from '@react-navigation/bottom-tabs';
import MaterialCommunityIcons from 'react-native-vector-icons/MaterialCommunityIcons';
import {useAuth} from '../context/AuthContext';
import LoginScreen from '../screens/Auth/LoginScreen';
import RegisterScreen from '../screens/Auth/RegisterScreen';
import ActiveOrdersScreen from '../screens/Orders/ActiveOrdersScreen';
import OrderHistoryScreen from '../screens/Orders/OrderHistoryScreen';
import MyRestaurantsScreen from '../screens/Restaurants/MyRestaurantsScreen';
import ProfileScreen from '../screens/Profile/ProfileScreen';
import {ActivityIndicator, View} from 'react-native';

const Stack = createNativeStackNavigator();
const Tab = createBottomTabNavigator();

function MainTabs() {
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
        headerShown: true,
      }}>
      <Tab.Screen
        name="ActiveOrders"
        component={ActiveOrdersScreen}
        options={{
          headerTitle: 'Aktif Siparişler',
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
          headerTitle: 'Sipariş Geçmişi',
          tabBarLabel: 'Geçmiş',
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
          headerTitle: 'Restoranlarım',
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
      <Tab.Screen
        name="Profile"
        component={ProfileScreen}
        options={{
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
        </>
      )}
    </Stack.Navigator>
  );
}
