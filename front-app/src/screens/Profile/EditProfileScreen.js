import React, {useState} from 'react';
import {
  View,
  Text,
  StyleSheet,
  TextInput,
  TouchableOpacity,
  ScrollView,
  ActivityIndicator,
  KeyboardAvoidingView,
  Platform,
} from 'react-native';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import {Colors, Fonts, Spacing, BorderRadius} from '../../theme';
import {useAuth} from '../../context/AuthContext';
import api from '../../api';
import {useToast} from '../../context/ToastContext';

const EditProfileScreen = ({navigation}) => {
  const {user, refreshProfile} = useAuth();
  const {showToast} = useToast();
  const [firstName, setFirstName] = useState(user?.firstName || '');
  const [lastName, setLastName] = useState(user?.lastName || '');
  const [phoneNumber, setPhoneNumber] = useState(user?.phoneNumber || '');
  const [loading, setLoading] = useState(false);

  const handleSave = async () => {
    if (!firstName.trim() || !lastName.trim()) {
      showToast('Ad ve soyad boş bırakılamaz', 'warning');
      return;
    }

    setLoading(true);
    try {
      const result = await api.auth.updateProfile({
        firstName: firstName.trim(),
        lastName: lastName.trim(),
        phoneNumber: phoneNumber.trim(),
      });
      if (!result.hasFailed) {
        await refreshProfile();
        showToast('Profil bilgileriniz güncellendi', 'success');
        navigation.goBack();
      } else {
        showToast(result.messages?.[0]?.description || 'Güncelleme başarısız', 'error');
      }
    } catch (e) {
      showToast('Bir hata oluştu', 'error');
    } finally {
      setLoading(false);
    }
  };

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <TouchableOpacity style={styles.backButton} onPress={() => navigation.goBack()}>
          <Icon name="arrow-left" size={24} color={Colors.text} />
        </TouchableOpacity>
        <Text style={styles.headerTitle}>Profil Düzenle</Text>
        <View style={{width: 44}} />
      </View>

      <KeyboardAvoidingView
        behavior={Platform.OS === 'ios' ? 'padding' : 'height'}
        style={{flex: 1}}
      >
        <ScrollView contentContainerStyle={styles.scrollContent} keyboardShouldPersistTaps="handled">
          <View style={styles.avatarSection}>
            <View style={styles.avatar}>
              <Text style={styles.avatarText}>
                {(firstName?.[0] || '').toUpperCase()}
                {(lastName?.[0] || '').toUpperCase()}
              </Text>
            </View>
          </View>

          <View style={styles.form}>
            <Text style={styles.label}>Ad</Text>
            <View style={styles.inputContainer}>
              <Icon name="account-outline" size={20} color={Colors.textSecondary} />
              <TextInput
                style={styles.input}
                value={firstName}
                onChangeText={setFirstName}
                placeholder="Adınız"
                placeholderTextColor={Colors.textTertiary}
              />
            </View>

            <Text style={styles.label}>Soyad</Text>
            <View style={styles.inputContainer}>
              <Icon name="account-outline" size={20} color={Colors.textSecondary} />
              <TextInput
                style={styles.input}
                value={lastName}
                onChangeText={setLastName}
                placeholder="Soyadınız"
                placeholderTextColor={Colors.textTertiary}
              />
            </View>

            <Text style={styles.label}>E-posta</Text>
            <View style={[styles.inputContainer, styles.inputDisabled]}>
              <Icon name="email-outline" size={20} color={Colors.textTertiary} />
              <Text style={styles.disabledText}>{user?.email}</Text>
            </View>

            <Text style={styles.label}>Telefon</Text>
            <View style={styles.inputContainer}>
              <Icon name="phone-outline" size={20} color={Colors.textSecondary} />
              <TextInput
                style={styles.input}
                value={phoneNumber}
                onChangeText={setPhoneNumber}
                placeholder="Telefon numaranız"
                placeholderTextColor={Colors.textTertiary}
                keyboardType="phone-pad"
              />
            </View>
          </View>

          <TouchableOpacity
            style={[styles.saveButton, loading && styles.buttonDisabled]}
            onPress={handleSave}
            activeOpacity={0.8}
            disabled={loading}
          >
            {loading ? (
              <ActivityIndicator color="#FFF" />
            ) : (
              <Text style={styles.saveButtonText}>Kaydet</Text>
            )}
          </TouchableOpacity>
        </ScrollView>
      </KeyboardAvoidingView>
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
    justifyContent: 'space-between',
    paddingHorizontal: Spacing.base,
    paddingTop: 54,
    paddingBottom: Spacing.md,
    backgroundColor: Colors.surface,
    borderBottomWidth: 1,
    borderBottomColor: Colors.borderLight,
  },
  backButton: {
    width: 44,
    height: 44,
    borderRadius: 22,
    backgroundColor: Colors.borderLight,
    justifyContent: 'center',
    alignItems: 'center',
  },
  headerTitle: {
    fontSize: Fonts.sizes.lg,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
  },
  scrollContent: {
    padding: Spacing.base,
    paddingBottom: Spacing.xxxl,
  },
  avatarSection: {
    alignItems: 'center',
    marginVertical: Spacing.xl,
  },
  avatar: {
    width: 90,
    height: 90,
    borderRadius: 45,
    backgroundColor: Colors.primary,
    justifyContent: 'center',
    alignItems: 'center',
  },
  avatarText: {
    fontSize: 32,
    fontWeight: Fonts.weights.heavy,
    color: '#FFF',
  },
  form: {
    marginBottom: Spacing.xl,
  },
  label: {
    fontSize: Fonts.sizes.sm,
    fontWeight: Fonts.weights.semibold,
    color: Colors.textSecondary,
    marginBottom: 6,
    marginTop: Spacing.md,
    marginLeft: 4,
  },
  inputContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: Colors.surface,
    borderRadius: BorderRadius.lg,
    paddingHorizontal: Spacing.base,
    height: 52,
    borderWidth: 1,
    borderColor: Colors.border,
    gap: 10,
  },
  inputDisabled: {
    backgroundColor: Colors.borderLight,
  },
  input: {
    flex: 1,
    fontSize: Fonts.sizes.base,
    color: Colors.text,
  },
  disabledText: {
    fontSize: Fonts.sizes.base,
    color: Colors.textTertiary,
  },
  saveButton: {
    backgroundColor: Colors.primary,
    borderRadius: BorderRadius.lg,
    height: 56,
    justifyContent: 'center',
    alignItems: 'center',
    shadowColor: Colors.primary,
    shadowOffset: {width: 0, height: 4},
    shadowOpacity: 0.3,
    shadowRadius: 8,
    elevation: 6,
  },
  buttonDisabled: {
    opacity: 0.7,
  },
  saveButtonText: {
    color: '#FFF',
    fontSize: Fonts.sizes.lg,
    fontWeight: Fonts.weights.bold,
  },
});

export default EditProfileScreen;
