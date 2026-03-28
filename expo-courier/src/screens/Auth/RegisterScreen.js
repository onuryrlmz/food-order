import React, {useState} from 'react';
import {
  View,
  Text,
  TextInput,
  TouchableOpacity,
  StyleSheet,
  ActivityIndicator,
  KeyboardAvoidingView,
  Platform,
  ScrollView,
  Alert,
} from 'react-native';
import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import { router } from 'expo-router';
import {Colors, Fonts, Spacing, BorderRadius} from '../../theme';
import {useAuth} from '../../context/AuthContext';

const REGISTER_TYPES = [
  {key: 'courier', label: 'Kurye olarak kayıt ol', icon: 'motorbike'},
  {key: 'company', label: 'Firma olarak kayıt ol', icon: 'office-building'},
];

const RegisterScreen = () => {
  const {register} = useAuth();
  const [registerType, setRegisterType] = useState('courier');
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [email, setEmail] = useState('');
  const [phoneNumber, setPhoneNumber] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [showPassword, setShowPassword] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleRegister = async () => {
    if (
      !firstName.trim() ||
      !lastName.trim() ||
      !email.trim() ||
      !phoneNumber.trim() ||
      !password
    ) {
      Alert.alert('Uyarı', 'Lütfen tüm alanları doldurun');
      return;
    }
    if (password !== confirmPassword) {
      Alert.alert('Uyarı', 'Şifreler eşleşmiyor');
      return;
    }
    if (password.length < 6) {
      Alert.alert('Uyarı', 'Şifre en az 6 karakter olmalıdır');
      return;
    }

    setError('');
    setLoading(true);
    try {
      const result = await register(firstName.trim(), lastName.trim(), email.trim(), phoneNumber.trim(), password);
      if (result.success) {
        // Navigator auto-switches to CourierSetupStack, no manual navigation needed
      } else {
        setError(result.error);
        Alert.alert('Hata', result.error);
      }
    } catch (e) {
      const errorMsg = 'Bir hata oluştu';
      setError(errorMsg);
      Alert.alert('Hata', errorMsg);
    } finally {
      setLoading(false);
    }
  };

  return (
    <KeyboardAvoidingView
      style={styles.container}
      behavior={Platform.OS === 'ios' ? 'padding' : 'height'}>
      <ScrollView
        contentContainerStyle={styles.scrollContent}
        keyboardShouldPersistTaps="handled"
        showsVerticalScrollIndicator={false}>
        <View style={styles.headerContainer}>
          <TouchableOpacity
            style={styles.backButton}
            onPress={() => router.back()}>
            <Icon name="arrow-left" size={24} color={Colors.text} />
          </TouchableOpacity>
          <View style={styles.iconContainer}>
            <Icon name="account-plus" size={48} color={Colors.primary} />
          </View>
          <Text style={styles.title}>Hesap Oluştur</Text>
          <Text style={styles.subtitle}>Kayıt olun ve başlayın</Text>
        </View>

        {/* Register Type Toggle */}
        <View style={styles.toggleContainer}>
          {REGISTER_TYPES.map(type => (
            <TouchableOpacity
              key={type.key}
              style={[
                styles.toggleButton,
                registerType === type.key && styles.toggleButtonActive,
              ]}
              onPress={() => setRegisterType(type.key)}>
              <Icon
                name={type.icon}
                size={18}
                color={
                  registerType === type.key
                    ? Colors.textInverse
                    : Colors.textSecondary
                }
              />
              <Text
                style={[
                  styles.toggleText,
                  registerType === type.key && styles.toggleTextActive,
                ]}>
                {type.label}
              </Text>
            </TouchableOpacity>
          ))}
        </View>

        <View style={styles.formContainer}>
          {error ? (
            <View style={styles.errorContainer}>
              <Icon name="alert-circle" size={18} color={Colors.error} />
              <Text style={styles.errorText}>{error}</Text>
            </View>
          ) : null}

          <View style={styles.row}>
            <View style={[styles.inputContainer, styles.halfInput]}>
              <Icon
                name="account-outline"
                size={20}
                color={Colors.textSecondary}
                style={styles.inputIcon}
              />
              <TextInput
                style={styles.input}
                placeholder="Ad"
                placeholderTextColor={Colors.textTertiary}
                value={firstName}
                onChangeText={setFirstName}
              />
            </View>
            <View style={{width: Spacing.sm}} />
            <View style={[styles.inputContainer, styles.halfInput]}>
              <TextInput
                style={styles.input}
                placeholder="Soyad"
                placeholderTextColor={Colors.textTertiary}
                value={lastName}
                onChangeText={setLastName}
              />
            </View>
          </View>

          <View style={styles.inputContainer}>
            <Icon
              name="email-outline"
              size={20}
              color={Colors.textSecondary}
              style={styles.inputIcon}
            />
            <TextInput
              style={styles.input}
              placeholder="E-posta"
              placeholderTextColor={Colors.textTertiary}
              value={email}
              onChangeText={setEmail}
              keyboardType="email-address"
              autoCapitalize="none"
              autoCorrect={false}
            />
          </View>

          <View style={styles.inputContainer}>
            <Icon
              name="phone-outline"
              size={20}
              color={Colors.textSecondary}
              style={styles.inputIcon}
            />
            <TextInput
              style={styles.input}
              placeholder="Telefon"
              placeholderTextColor={Colors.textTertiary}
              value={phoneNumber}
              onChangeText={setPhoneNumber}
              keyboardType="phone-pad"
            />
          </View>

          <View style={styles.inputContainer}>
            <Icon
              name="lock-outline"
              size={20}
              color={Colors.textSecondary}
              style={styles.inputIcon}
            />
            <TextInput
              style={styles.input}
              placeholder="Şifre"
              placeholderTextColor={Colors.textTertiary}
              value={password}
              onChangeText={setPassword}
              //secureTextEntry={!showPassword}
            />
            <TouchableOpacity onPress={() => setShowPassword(!showPassword)}>
              <Icon
                name={showPassword ? 'eye-off-outline' : 'eye-outline'}
                size={20}
                color={Colors.textSecondary}
              />
            </TouchableOpacity>
          </View>

          <View style={styles.inputContainer}>
            <Icon
              name="lock-check-outline"
              size={20}
              color={Colors.textSecondary}
              style={styles.inputIcon}
            />
            <TextInput
              style={styles.input}
              placeholder="Şifre Tekrar"
              placeholderTextColor={Colors.textTertiary}
              value={confirmPassword}
              onChangeText={setConfirmPassword}
              //secureTextEntry={!showPassword}
            />
          </View>

          <TouchableOpacity
            style={[styles.submitButton, loading && styles.submitButtonDisabled]}
            onPress={handleRegister}
            disabled={loading}>
            {loading ? (
              <ActivityIndicator color={Colors.textInverse} />
            ) : (
              <Text style={styles.submitButtonText}>Devam Et</Text>
            )}
          </TouchableOpacity>
        </View>

        <View style={styles.footer}>
          <Text style={styles.footerText}>Zaten hesabınız var mı? </Text>
          <TouchableOpacity onPress={() => router.back()}>
            <Text style={styles.footerLink}>Giriş Yapın</Text>
          </TouchableOpacity>
        </View>
      </ScrollView>
    </KeyboardAvoidingView>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: Colors.background,
  },
  scrollContent: {
    flexGrow: 1,
    paddingHorizontal: Spacing.xl,
    paddingVertical: Spacing.xxl,
  },
  headerContainer: {
    alignItems: 'center',
    marginBottom: Spacing.xl,
  },
  backButton: {
    position: 'absolute',
    left: 0,
    top: 0,
    width: 44,
    height: 44,
    borderRadius: 22,
    backgroundColor: Colors.surface,
    justifyContent: 'center',
    alignItems: 'center',
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 1},
    shadowOpacity: 0.08,
    shadowRadius: 4,
    elevation: 2,
  },
  iconContainer: {
    width: 96,
    height: 96,
    borderRadius: BorderRadius.xxl,
    backgroundColor: Colors.surface,
    justifyContent: 'center',
    alignItems: 'center',
    marginBottom: Spacing.lg,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 4},
    shadowOpacity: 0.1,
    shadowRadius: 12,
    elevation: 6,
  },
  title: {
    fontSize: Fonts.sizes.xxxl,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
    marginBottom: Spacing.xs,
  },
  subtitle: {
    fontSize: Fonts.sizes.base,
    color: Colors.textSecondary,
  },
  toggleContainer: {
    flexDirection: 'row',
    backgroundColor: Colors.surface,
    borderRadius: BorderRadius.md,
    padding: Spacing.xs,
    marginBottom: Spacing.xl,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 2},
    shadowOpacity: 0.06,
    shadowRadius: 8,
    elevation: 4,
  },
  toggleButton: {
    flex: 1,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: Spacing.md,
    borderRadius: BorderRadius.sm,
  },
  toggleButtonActive: {
    backgroundColor: Colors.primary,
  },
  toggleText: {
    fontSize: Fonts.sizes.sm,
    fontWeight: Fonts.weights.semibold,
    color: Colors.textSecondary,
    marginLeft: Spacing.xs,
  },
  toggleTextActive: {
    color: Colors.textInverse,
  },
  formContainer: {
    backgroundColor: Colors.surface,
    borderRadius: BorderRadius.xl,
    padding: Spacing.xl,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 2},
    shadowOpacity: 0.06,
    shadowRadius: 8,
    elevation: 4,
    marginBottom: Spacing.xl,
  },
  errorContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#FFF0F0',
    borderRadius: BorderRadius.md,
    padding: Spacing.md,
    marginBottom: Spacing.base,
  },
  errorText: {
    color: Colors.error,
    fontSize: Fonts.sizes.md,
    marginLeft: Spacing.sm,
    flex: 1,
  },
  row: {
    flexDirection: 'row',
  },
  inputContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: Colors.background,
    borderRadius: BorderRadius.md,
    paddingHorizontal: Spacing.base,
    marginBottom: Spacing.base,
    height: 52,
  },
  halfInput: {
    flex: 1,
  },
  inputIcon: {
    marginRight: Spacing.md,
  },
  input: {
    flex: 1,
    fontSize: Fonts.sizes.base,
    color: Colors.text,
    height: '100%',
  },
  submitButton: {
    backgroundColor: Colors.primary,
    borderRadius: BorderRadius.md,
    height: 52,
    justifyContent: 'center',
    alignItems: 'center',
    marginTop: Spacing.sm,
  },
  submitButtonDisabled: {
    opacity: 0.7,
  },
  submitButtonText: {
    color: Colors.textInverse,
    fontSize: Fonts.sizes.lg,
    fontWeight: Fonts.weights.semibold,
  },
  footer: {
    flexDirection: 'row',
    justifyContent: 'center',
    alignItems: 'center',
  },
  footerText: {
    fontSize: Fonts.sizes.md,
    color: Colors.textSecondary,
  },
  footerLink: {
    fontSize: Fonts.sizes.md,
    color: Colors.primary,
    fontWeight: Fonts.weights.bold,
  },
});

export default RegisterScreen;
