import React, {useState, useEffect, useCallback} from 'react';
import {
  View,
  Text,
  TextInput,
  StyleSheet,
  TouchableOpacity,
  Alert,
  ScrollView,
  ActivityIndicator,
  Modal,
} from 'react-native';
import MaterialCommunityIcons from 'react-native-vector-icons/MaterialCommunityIcons';
import {useAuth} from '../../context/AuthContext';
import {
  getProfile,
  updateProfile,
  changePassword,
} from '../../api/courierService';

export default function ProfileScreen() {
  const {logout} = useAuth();

  const [profile, setProfile] = useState(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [editing, setEditing] = useState(false);

  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [phoneNumber, setPhoneNumber] = useState('');

  const [pwdModal, setPwdModal] = useState(false);
  const [currentPwd, setCurrentPwd] = useState('');
  const [newPwd, setNewPwd] = useState('');
  const [newPwdConfirm, setNewPwdConfirm] = useState('');
  const [pwdSaving, setPwdSaving] = useState(false);

  const fetchProfile = useCallback(async () => {
    try {
      const result = await getProfile();
      if (!result.hasFailed && result.data) {
        setProfile(result.data);
        setFirstName(result.data.firstName || '');
        setLastName(result.data.lastName || '');
        setPhoneNumber(result.data.phoneNumber || '');
      }
    } catch (e) {
      console.error('Profile fetch error:', e);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchProfile();
  }, [fetchProfile]);

  const handleSave = async () => {
    if (!firstName.trim() || !lastName.trim()) {
      Alert.alert('Hata', 'Ad ve soyad boş bırakılamaz.');
      return;
    }
    setSaving(true);
    try {
      const result = await updateProfile(
        firstName.trim(),
        lastName.trim(),
        phoneNumber.trim(),
      );
      if (!result.hasFailed) {
        Alert.alert('Başarılı', 'Profil güncellendi.');
        setEditing(false);
        fetchProfile();
      } else {
        const msg =
          result.messages?.map(m => m.description).join(', ') || 'Hata oluştu';
        Alert.alert('Hata', msg);
      }
    } catch (e) {
      Alert.alert('Hata', 'Bir hata oluştu.');
    } finally {
      setSaving(false);
    }
  };

  const handleChangePassword = async () => {
    if (!currentPwd || !newPwd) {
      Alert.alert('Hata', 'Lütfen tüm alanları doldurun.');
      return;
    }
    if (newPwd.length < 6) {
      Alert.alert('Hata', 'Yeni şifre en az 6 karakter olmalıdır.');
      return;
    }
    if (newPwd !== newPwdConfirm) {
      Alert.alert('Hata', 'Yeni şifreler eşleşmiyor.');
      return;
    }
    setPwdSaving(true);
    try {
      const result = await changePassword(currentPwd, newPwd);
      if (!result.hasFailed) {
        Alert.alert('Başarılı', 'Şifreniz değiştirildi.');
        setPwdModal(false);
        setCurrentPwd('');
        setNewPwd('');
        setNewPwdConfirm('');
      } else {
        const msg =
          result.messages?.map(m => m.description).join(', ') || 'Hata oluştu';
        Alert.alert('Hata', msg);
      }
    } catch (e) {
      Alert.alert('Hata', 'Bir hata oluştu.');
    } finally {
      setPwdSaving(false);
    }
  };

  const handleLogout = () => {
    Alert.alert('Çıkış', 'Çıkış yapmak istediğinize emin misiniz?', [
      {text: 'İptal', style: 'cancel'},
      {text: 'Çıkış Yap', style: 'destructive', onPress: logout},
    ]);
  };

  if (loading) {
    return (
      <View style={styles.center}>
        <ActivityIndicator size="large" color="#FF6B00" />
      </View>
    );
  }

  return (
    <ScrollView style={styles.container} contentContainerStyle={styles.content}>
      {/* Avatar + Email */}
      <View style={styles.avatarSection}>
        <View style={styles.avatar}>
          <MaterialCommunityIcons name="account" size={40} color="#FF6B00" />
        </View>
        <Text style={styles.email}>{profile?.email || '-'}</Text>
      </View>

      {/* Profile Info */}
      <View style={styles.card}>
        <View style={styles.cardHeader}>
          <Text style={styles.cardTitle}>Kişisel Bilgiler</Text>
          {!editing && (
            <TouchableOpacity onPress={() => setEditing(true)}>
              <MaterialCommunityIcons
                name="pencil"
                size={20}
                color="#FF6B00"
              />
            </TouchableOpacity>
          )}
        </View>

        <Text style={styles.label}>Ad</Text>
        <TextInput
          style={[styles.input, !editing && styles.inputDisabled]}
          value={firstName}
          onChangeText={setFirstName}
          editable={editing}
          placeholder="Ad"
          placeholderTextColor="#bbb"
        />

        <Text style={styles.label}>Soyad</Text>
        <TextInput
          style={[styles.input, !editing && styles.inputDisabled]}
          value={lastName}
          onChangeText={setLastName}
          editable={editing}
          placeholder="Soyad"
          placeholderTextColor="#bbb"
        />

        <Text style={styles.label}>Telefon</Text>
        <TextInput
          style={[styles.input, !editing && styles.inputDisabled]}
          value={phoneNumber}
          onChangeText={setPhoneNumber}
          editable={editing}
          placeholder="Telefon"
          placeholderTextColor="#bbb"
          keyboardType="phone-pad"
        />

        {editing && (
          <View style={styles.editActions}>
            <TouchableOpacity
              style={styles.cancelBtn}
              onPress={() => {
                setEditing(false);
                setFirstName(profile?.firstName || '');
                setLastName(profile?.lastName || '');
                setPhoneNumber(profile?.phoneNumber || '');
              }}>
              <Text style={styles.cancelBtnText}>İptal</Text>
            </TouchableOpacity>
            <TouchableOpacity
              style={[styles.saveBtn, saving && {opacity: 0.6}]}
              onPress={handleSave}
              disabled={saving}>
              <Text style={styles.saveBtnText}>
                {saving ? 'Kaydediliyor...' : 'Kaydet'}
              </Text>
            </TouchableOpacity>
          </View>
        )}
      </View>

      {/* Menu Items */}
      <View style={styles.card}>
        <TouchableOpacity
          style={styles.menuItem}
          onPress={() => setPwdModal(true)}>
          <MaterialCommunityIcons name="lock-outline" size={22} color="#555" />
          <Text style={styles.menuText}>Şifre Değiştir</Text>
          <MaterialCommunityIcons
            name="chevron-right"
            size={22}
            color="#ccc"
          />
        </TouchableOpacity>
      </View>

      <View style={styles.card}>
        <TouchableOpacity style={styles.logoutButton} onPress={handleLogout}>
          <MaterialCommunityIcons name="logout" size={22} color="#dc3545" />
          <Text style={styles.logoutText}>Çıkış Yap</Text>
        </TouchableOpacity>
      </View>

      {/* Change Password Modal */}
      <Modal visible={pwdModal} transparent animationType="fade">
        <View style={styles.modalOverlay}>
          <View style={styles.modalContent}>
            <Text style={styles.modalTitle}>Şifre Değiştir</Text>

            <Text style={styles.label}>Mevcut Şifre</Text>
            <TextInput
              style={styles.input}
              value={currentPwd}
              onChangeText={setCurrentPwd}
              secureTextEntry
              placeholder="Mevcut şifreniz"
              placeholderTextColor="#bbb"
            />

            <Text style={styles.label}>Yeni Şifre</Text>
            <TextInput
              style={styles.input}
              value={newPwd}
              onChangeText={setNewPwd}
              secureTextEntry
              placeholder="En az 6 karakter"
              placeholderTextColor="#bbb"
            />

            <Text style={styles.label}>Yeni Şifre Tekrar</Text>
            <TextInput
              style={styles.input}
              value={newPwdConfirm}
              onChangeText={setNewPwdConfirm}
              secureTextEntry
              placeholder="Yeni şifrenizi tekrar girin"
              placeholderTextColor="#bbb"
            />

            <View style={styles.editActions}>
              <TouchableOpacity
                style={styles.cancelBtn}
                onPress={() => {
                  setPwdModal(false);
                  setCurrentPwd('');
                  setNewPwd('');
                  setNewPwdConfirm('');
                }}>
                <Text style={styles.cancelBtnText}>İptal</Text>
              </TouchableOpacity>
              <TouchableOpacity
                style={[styles.saveBtn, pwdSaving && {opacity: 0.6}]}
                onPress={handleChangePassword}
                disabled={pwdSaving}>
                <Text style={styles.saveBtnText}>
                  {pwdSaving ? 'Değiştiriliyor...' : 'Değiştir'}
                </Text>
              </TouchableOpacity>
            </View>
          </View>
        </View>
      </Modal>
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f8f9fa',
  },
  content: {
    padding: 16,
    paddingBottom: 40,
  },
  center: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: '#f8f9fa',
  },
  avatarSection: {
    alignItems: 'center',
    paddingVertical: 24,
  },
  avatar: {
    width: 72,
    height: 72,
    borderRadius: 36,
    backgroundColor: '#FFF3E0',
    justifyContent: 'center',
    alignItems: 'center',
    marginBottom: 12,
  },
  email: {
    fontSize: 15,
    color: '#666',
  },
  card: {
    backgroundColor: '#fff',
    borderRadius: 16,
    padding: 20,
    marginBottom: 12,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 1},
    shadowOpacity: 0.05,
    shadowRadius: 4,
    elevation: 2,
  },
  cardHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 16,
  },
  cardTitle: {
    fontSize: 16,
    fontWeight: '700',
    color: '#1a1a1a',
  },
  label: {
    fontSize: 13,
    fontWeight: '600',
    color: '#888',
    marginBottom: 6,
    marginTop: 8,
  },
  input: {
    borderWidth: 1,
    borderColor: '#e0e0e0',
    borderRadius: 10,
    paddingHorizontal: 14,
    paddingVertical: 12,
    fontSize: 15,
    color: '#1a1a1a',
    backgroundColor: '#fafafa',
    marginBottom: 4,
  },
  inputDisabled: {
    backgroundColor: '#f5f5f5',
    color: '#666',
  },
  editActions: {
    flexDirection: 'row',
    justifyContent: 'flex-end',
    gap: 10,
    marginTop: 16,
  },
  cancelBtn: {
    paddingHorizontal: 20,
    paddingVertical: 10,
    borderRadius: 10,
    borderWidth: 1,
    borderColor: '#ddd',
  },
  cancelBtnText: {
    fontSize: 14,
    fontWeight: '600',
    color: '#666',
  },
  saveBtn: {
    paddingHorizontal: 20,
    paddingVertical: 10,
    borderRadius: 10,
    backgroundColor: '#FF6B00',
  },
  saveBtnText: {
    fontSize: 14,
    fontWeight: '600',
    color: '#fff',
  },
  menuItem: {
    flexDirection: 'row',
    alignItems: 'center',
  },
  menuText: {
    flex: 1,
    fontSize: 16,
    color: '#333',
    marginLeft: 14,
  },
  logoutButton: {
    flexDirection: 'row',
    alignItems: 'center',
  },
  logoutText: {
    fontSize: 16,
    color: '#dc3545',
    fontWeight: '600',
    marginLeft: 14,
  },
  modalOverlay: {
    flex: 1,
    backgroundColor: 'rgba(0,0,0,0.5)',
    justifyContent: 'center',
    padding: 24,
  },
  modalContent: {
    backgroundColor: '#fff',
    borderRadius: 20,
    padding: 24,
  },
  modalTitle: {
    fontSize: 18,
    fontWeight: '700',
    color: '#1a1a1a',
    marginBottom: 16,
    textAlign: 'center',
  },
});
