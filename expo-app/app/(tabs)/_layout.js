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
          tabBarLabel: 'Siparisler',
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
