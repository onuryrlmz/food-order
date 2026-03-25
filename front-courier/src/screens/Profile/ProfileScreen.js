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
import {useAuth} from '../../context/AuthContext';
import {courierService} from '../../api/courierService';

const ProfileScreen = () => {
  const {user, logout, refreshProfile} = useAuth();
  const [editing, setEditing] = useState(false);
  const [saving, setSaving] = useState(false);
  const [formData, setFormData] = useState({
    fullName: user?.fullName || '',
    phone: user?.phone || '',
    vehicleType: user?.vehicleType || '',
    vehiclePlate: user?.vehiclePlate || '',
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
          <Text style={styles.avatarName}>{user?.fullName || 'Kurye'}</Text>
          <Text style={styles.avatarEmail}>{user?.email || '-'}</Text>
        </View>

        {/* Profile Fields */}
        <View style={styles.card}>
          <Text style={styles.cardTitle}>Kişisel Bilgiler</Text>
          {renderField('Ad Soyad', user?.fullName, 'fullName', 'account-outline')}
          {renderField('Telefon', user?.phone, 'phone', 'phone-outline')}
        </View>

        <View style={styles.card}>
          <Text style={styles.cardTitle}>Araç Bilgileri</Text>
          {renderField('Araç Tipi', user?.vehicleType, 'vehicleType', 'motorbike')}
          {renderField('Plaka', user?.vehiclePlate, 'vehiclePlate', 'card-text-outline')}
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
