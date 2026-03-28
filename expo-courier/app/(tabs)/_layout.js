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
          tabBarLabel: 'Kazanclar',
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
