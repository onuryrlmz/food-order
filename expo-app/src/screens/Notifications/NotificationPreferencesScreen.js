import React, {useState, useEffect} from 'react';
import {
  View,
  Text,
  StyleSheet,
  Switch,
  TouchableOpacity,
  ActivityIndicator,
} from 'react-native';
import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import {Colors, Fonts, Spacing, BorderRadius} from '../../theme';
import {notificationService} from '../../api';
import {useToast} from '../../context/ToastContext';
import LoadingSpinner from '../../components/LoadingSpinner';

const PREFERENCE_TYPES = [
  {
    id: 1,
    label: 'Sipariş Durumu',
    description: 'Siparişlerinizin durum güncellemeleri',
    icon: 'package-variant',
    color: '#5856D6',
  },
  {
    id: 2,
    label: 'Kampanyalar',
    description: 'Fırsat ve kampanya bildirimleri',
    icon: 'tag-outline',
    color: Colors.secondary,
  },
  {
    id: 3,
    label: 'Yorum Yanıtları',
    description: 'Yorumlarınıza gelen yanıtlar',
    icon: 'comment-text-outline',
    color: Colors.info,
  },
  {
    id: 4,
    label: 'Teslimat Güncellemeleri',
    description: 'Teslimat süreciyle ilgili bildirimler',
    icon: 'truck-delivery-outline',
    color: Colors.success,
  },
];

import { router } from 'expo-router';
const NotificationPreferencesScreen = () => {
  const {showToast} = useToast();
  const [loading, setLoading] = useState(true);
  const [preferences, setPreferences] = useState({});

  useEffect(() => {
    loadPreferences();
  }, []);

  const loadPreferences = async () => {
    try {
      const res = await notificationService.getPreferences();
      if (res.data?.data) {
        setPreferences(res.data.data);
      }
    } catch (e) {
      console.log('Notification preferences error:', e);
    } finally {
      setLoading(false);
    }
  };

  const handleToggle = async (typeId, value) => {
    const prev = {...preferences};
    setPreferences(p => ({...p, [typeId]: value}));

    try {
      const res = await notificationService.updatePreference(typeId, value);
      if (res.data?.hasFailed) {
        setPreferences(prev);
        showToast('Tercih güncellenemedi', 'error');
      }
    } catch (e) {
      setPreferences(prev);
      showToast('Tercih güncellenemedi', 'error');
    }
  };

  if (loading) {
    return <LoadingSpinner message="Tercihler yükleniyor..." />;
  }

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <TouchableOpacity
          onPress={() => router.back()}
          style={styles.backButton}
          activeOpacity={0.8}>
          <Icon name="arrow-left" size={24} color={Colors.text} />
        </TouchableOpacity>
        <Text style={styles.headerTitle}>Bildirim Tercihleri</Text>
      </View>

      <View style={styles.card}>
        {PREFERENCE_TYPES.map((type, index) => (
          <View
            key={type.id}
            style={[
              styles.preferenceItem,
              index < PREFERENCE_TYPES.length - 1 && styles.preferenceItemBorder,
            ]}>
            <View style={[styles.iconBg, {backgroundColor: type.color + '15'}]}>
              <Icon name={type.icon} size={22} color={type.color} />
            </View>
            <View style={styles.textContainer}>
              <Text style={styles.label}>{type.label}</Text>
              <Text style={styles.description}>{type.description}</Text>
            </View>
            <Switch
              value={preferences[type.id] !== false}
              onValueChange={value => handleToggle(type.id, value)}
              trackColor={{false: Colors.border, true: Colors.primary + '60'}}
              thumbColor={
                preferences[type.id] !== false ? Colors.primary : '#F4F4F4'
              }
            />
          </View>
        ))}
      </View>

      <Text style={styles.footerNote}>
        Kapatılan bildirim türlerini almayı durdurursunuz. Sipariş durumu
        bildirimleri kritik güncellemeler içerebilir.
      </Text>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: Colors.background,
  },
  header: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingHorizontal: Spacing.base,
    paddingTop: 60,
    paddingBottom: Spacing.md,
    backgroundColor: Colors.surface,
    borderBottomWidth: 1,
    borderBottomColor: Colors.borderLight,
  },
  backButton: {
    marginRight: Spacing.sm,
  },
  headerTitle: {
    fontSize: Fonts.sizes.xxl,
    fontWeight: Fonts.weights.heavy,
    color: Colors.text,
  },
  card: {
    backgroundColor: Colors.surface,
    marginHorizontal: Spacing.base,
    marginTop: Spacing.md,
    borderRadius: BorderRadius.lg,
    overflow: 'hidden',
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 1},
    shadowOpacity: 0.04,
    shadowRadius: 4,
    elevation: 2,
  },
  preferenceItem: {
    flexDirection: 'row',
    alignItems: 'center',
    padding: Spacing.base,
  },
  preferenceItemBorder: {
    borderBottomWidth: 1,
    borderBottomColor: Colors.borderLight,
  },
  iconBg: {
    width: 42,
    height: 42,
    borderRadius: 12,
    justifyContent: 'center',
    alignItems: 'center',
  },
  textContainer: {
    flex: 1,
    marginLeft: Spacing.md,
    marginRight: Spacing.sm,
  },
  label: {
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.semibold,
    color: Colors.text,
  },
  description: {
    fontSize: Fonts.sizes.sm,
    color: Colors.textSecondary,
    marginTop: 2,
  },
  footerNote: {
    fontSize: Fonts.sizes.sm,
    color: Colors.textTertiary,
    marginHorizontal: Spacing.xl,
    marginTop: Spacing.lg,
    lineHeight: 20,
    textAlign: 'center',
  },
});

export default NotificationPreferencesScreen;
