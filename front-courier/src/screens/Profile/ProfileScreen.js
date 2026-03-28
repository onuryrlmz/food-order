import React, {useState} from 'react';
import {
  View,
  Text,
  StyleSheet,
  TouchableOpacity,
  ScrollView,
  Alert,
  TextInput,
  ActivityIndicator,
} from 'react-native';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import {Colors, Fonts, Spacing, BorderRadius} from '../../theme';
import {useNavigation} from '@react-navigation/native';
import {useAuth} from '../../context/AuthContext';
import {courierService} from '../../api/courierService';

const ProfileScreen = () => {
  const navigation = useNavigation();
  const {user, logout, refreshProfile} = useAuth();
  const [editing, setEditing] = useState(false);
  const [saving, setSaving] = useState(false);
  const [formData, setFormData] = useState({
    vehicleType: user?.vehicleType || '',
    vehiclePlate: user?.vehiclePlate || '',
    iban: user?.iban || '',
  });

  const handleSave = async () => {
    setSaving(true);
    try {
      const response = await courierService.updateProfile(formData);
      if (!response.data.hasFailed) {
        await refreshProfile();
        setEditing(false);
        Alert.alert('Başarılı', 'Profil güncellendi');
      } else {
        Alert.alert('Hata', response.data.messages?.[0]?.description || 'Güncelleme başarısız');
      }
    } catch (error) {
      Alert.alert('Hata', 'Bağlantı hatası oluştu');
    } finally {
      setSaving(false);
    }
  };

  const handleLogout = () => {
    Alert.alert(
      'Çıkış Yap',
      'Hesabınızdan çıkış yapmak istediğinize emin misiniz?',
      [
        {text: 'İptal', style: 'cancel'},
        {
          text: 'Çıkış Yap',
          style: 'destructive',
          onPress: logout,
        },
      ],
    );
  };

  const renderReadOnlyField = (label, value, icon) => (
    <View style={styles.fieldContainer}>
      <View style={styles.fieldLabelRow}>
        <Icon name={icon} size={18} color={Colors.textSecondary} />
        <Text style={styles.fieldLabel}>{label}</Text>
      </View>
      <Text style={styles.fieldValue}>{value || '-'}</Text>
    </View>
  );

  const renderField = (label, value, key, icon) => {
    if (editing) {
      return (
        <View style={styles.fieldContainer}>
          <View style={styles.fieldLabelRow}>
            <Icon name={icon} size={18} color={Colors.textSecondary} />
            <Text style={styles.fieldLabel}>{label}</Text>
          </View>
          <TextInput
            style={styles.fieldInput}
            value={formData[key]}
            onChangeText={text => setFormData(prev => ({...prev, [key]: text}))}
            placeholder={label}
            placeholderTextColor={Colors.textTertiary}
          />
        </View>
      );
    }
    return (
      <View style={styles.fieldContainer}>
        <View style={styles.fieldLabelRow}>
          <Icon name={icon} size={18} color={Colors.textSecondary} />
          <Text style={styles.fieldLabel}>{label}</Text>
        </View>
        <Text style={styles.fieldValue}>{value || '-'}</Text>
      </View>
    );
  };

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <Text style={styles.headerTitle}>Profilim</Text>
        {!editing ? (
          <TouchableOpacity onPress={() => setEditing(true)}>
            <Icon name="pencil" size={22} color={Colors.primary} />
          </TouchableOpacity>
        ) : (
          <TouchableOpacity onPress={() => setEditing(false)}>
            <Icon name="close" size={22} color={Colors.textSecondary} />
          </TouchableOpacity>
        )}
      </View>

      <ScrollView style={styles.content}>
        {/* Avatar */}
        <View style={styles.avatarContainer}>
          <View style={styles.avatar}>
            <Icon name="account" size={48} color={Colors.primary} />
          </View>
          <Text style={styles.avatarName}>{user?.firstName && user?.lastName ? `${user.firstName} ${user.lastName}` : 'Kurye'}</Text>
          <Text style={styles.avatarEmail}>{user?.phone || '-'}</Text>
        </View>

        {/* Profile Fields */}
        <View style={styles.card}>
          <Text style={styles.cardTitle}>Kişisel Bilgiler</Text>
          {renderReadOnlyField('Ad', user?.firstName, 'account-outline')}
          {renderReadOnlyField('Soyad', user?.lastName, 'account-outline')}
          {renderReadOnlyField('Telefon', user?.phone, 'phone-outline')}
        </View>

        <View style={styles.card}>
          <Text style={styles.cardTitle}>Araç Bilgileri</Text>
          {renderField('Araç Tipi', user?.vehicleType, 'vehicleType', 'motorbike')}
          {renderField('Plaka', user?.vehiclePlate, 'vehiclePlate', 'card-text-outline')}
        </View>

        <View style={styles.card}>
          <Text style={styles.cardTitle}>Ödeme Bilgileri</Text>
          {renderField('IBAN', user?.iban, 'iban', 'bank-outline')}
        </View>

        {editing && (
          <TouchableOpacity
            style={styles.saveButton}
            onPress={handleSave}
            disabled={saving}>
            {saving ? (
              <ActivityIndicator color={Colors.textInverse} />
            ) : (
              <Text style={styles.saveButtonText}>Kaydet</Text>
            )}
          </TouchableOpacity>
        )}

        {/* Menu Items */}
        <TouchableOpacity
          style={styles.menuItem}
          onPress={() => navigation.navigate('Agreements')}>
          <View style={styles.menuItemLeft}>
            <Icon name="handshake-outline" size={20} color={Colors.primary} />
            <Text style={styles.menuItemText}>Anlasmalarim</Text>
          </View>
          <Icon name="chevron-right" size={20} color={Colors.textTertiary} />
        </TouchableOpacity>

        {/* Logout */}
        <TouchableOpacity style={styles.logoutButton} onPress={handleLogout}>
          <Icon name="logout" size={20} color={Colors.error} />
          <Text style={styles.logoutText}>Çıkış Yap</Text>
        </TouchableOpacity>

        <View style={{height: Spacing.xxl}} />
      </ScrollView>
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
    justifyContent: 'space-between',
    alignItems: 'center',
    paddingHorizontal: Spacing.xl,
    paddingTop: Spacing.huge,
    paddingBottom: Spacing.base,
    backgroundColor: Colors.surface,
    borderBottomWidth: 1,
    borderBottomColor: Colors.borderLight,
  },
  headerTitle: {
    fontSize: Fonts.sizes.xl,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
  },
  content: {
    flex: 1,
    paddingHorizontal: Spacing.base,
    paddingTop: Spacing.base,
  },
  avatarContainer: {
    alignItems: 'center',
    marginBottom: Spacing.xl,
  },
  avatar: {
    width: 88,
    height: 88,
    borderRadius: 44,
    backgroundColor: Colors.primary + '15',
    justifyContent: 'center',
    alignItems: 'center',
    marginBottom: Spacing.md,
  },
  avatarName: {
    fontSize: Fonts.sizes.xl,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
  },
  avatarEmail: {
    fontSize: Fonts.sizes.md,
    color: Colors.textSecondary,
    marginTop: Spacing.xs,
  },
  card: {
    backgroundColor: Colors.surface,
    borderRadius: BorderRadius.xl,
    padding: Spacing.base,
    marginBottom: Spacing.base,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 2},
    shadowOpacity: 0.06,
    shadowRadius: 8,
    elevation: 4,
  },
  cardTitle: {
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.semibold,
    color: Colors.text,
    marginBottom: Spacing.md,
    paddingBottom: Spacing.sm,
    borderBottomWidth: 1,
    borderBottomColor: Colors.borderLight,
  },
  fieldContainer: {
    marginBottom: Spacing.md,
  },
  fieldLabelRow: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: Spacing.xs,
  },
  fieldLabel: {
    fontSize: Fonts.sizes.sm,
    color: Colors.textSecondary,
    marginLeft: Spacing.xs,
  },
  fieldValue: {
    fontSize: Fonts.sizes.base,
    color: Colors.text,
    fontWeight: Fonts.weights.medium,
    paddingLeft: Spacing.xl,
  },
  fieldInput: {
    fontSize: Fonts.sizes.base,
    color: Colors.text,
    backgroundColor: Colors.background,
    borderRadius: BorderRadius.md,
    paddingHorizontal: Spacing.md,
    paddingVertical: Spacing.sm,
    marginLeft: Spacing.xl,
  },
  saveButton: {
    backgroundColor: Colors.primary,
    borderRadius: BorderRadius.md,
    paddingVertical: Spacing.base,
    alignItems: 'center',
    marginBottom: Spacing.base,
  },
  saveButtonText: {
    color: Colors.textInverse,
    fontSize: Fonts.sizes.lg,
    fontWeight: Fonts.weights.semibold,
  },
  menuItem: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    backgroundColor: Colors.surface,
    borderRadius: BorderRadius.xl,
    padding: Spacing.base,
    marginBottom: Spacing.base,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 2},
    shadowOpacity: 0.06,
    shadowRadius: 8,
    elevation: 4,
  },
  menuItemLeft: {
    flexDirection: 'row',
    alignItems: 'center',
  },
  menuItemText: {
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.semibold,
    color: Colors.text,
    marginLeft: Spacing.md,
  },
  logoutButton: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    backgroundColor: Colors.surface,
    borderRadius: BorderRadius.xl,
    padding: Spacing.base,
    borderWidth: 1,
    borderColor: Colors.error + '30',
  },
  logoutText: {
    color: Colors.error,
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.semibold,
    marginLeft: Spacing.sm,
  },
});

export default ProfileScreen;
