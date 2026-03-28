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
} from 'react-native';
import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import { router } from 'expo-router';
import {Colors, Fonts, Spacing, BorderRadius} from '../../theme';
import {useAuth} from '../../context/AuthContext';

const LoginScreen = () => {
  const {login} = useAuth();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [showPassword, setShowPassword] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleLogin = async () => {
    if (!email.trim() || !password.trim()) {
      setError('E-posta ve şifre gereklidir');
      return;
    }
    setError('');
    setLoading(true);
    try {
      const result = await login(email.trim(), password);
      if (!result.success) {
        setError(result.error);
      }
    } catch (e) {
      setError('Bir hata oluştu');
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
        keyboardShouldPersistTaps="handled">
        <View style={styles.headerContainer}>
          <View style={styles.iconContainer}>
            <Icon name="motorbike" size={48} color={Colors.primary} />
          </View>
          <Text style={styles.title}>Kurye Paneli</Text>
          <Text style={styles.subtitle}>Hesabınıza giriş yapın</Text>
        </View>

        <View style={styles.formContainer}>
          {error ? (
            <View style={styles.errorContainer}>
              <Icon name="alert-circle" size={18} color={Colors.error} />
              <Text style={styles.errorText}>{error}</Text>
            </View>
          ) : null}

          <View style={styles.inputContainer}>
            <Icon name="email-outline" size={20} color={Colors.textSecondary} style={styles.inputIcon} />
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
            <Icon name="lock-outline" size={20} color={Colors.textSecondary} style={styles.inputIcon} />
            <TextInput
              style={styles.input}
              placeholder="Şifre"
              placeholderTextColor={Colors.textTertiary}
              value={password}
              onChangeText={setPassword}
              secureTextEntry={!showPassword}
            />
            <TouchableOpacity onPress={() => setShowPassword(!showPassword)}>
              <Icon
                name={showPassword ? 'eye-off-outline' : 'eye-outline'}
                size={20}
                color={Colors.textSecondary}
              />
            </TouchableOpacity>
          </View>

          <TouchableOpacity
            style={[styles.loginButton, loading && styles.loginButtonDisabled]}
            onPress={handleLogin}
            disabled={loading}>
            {loading ? (
              <ActivityIndicator color={Colors.textInverse} />
            ) : (
              <Text style={styles.loginButtonText}>Giriş Yap</Text>
            )}
          </TouchableOpacity>
        </View>

        <View style={styles.registerContainer}>
          <Text style={styles.registerText}>Hesabın yok mu? </Text>
          <TouchableOpacity onPress={() => router.push('/(auth)/register')}>
            <Text style={styles.registerLink}>Kayıt Ol</Text>
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
    justifyContent: 'center',
    paddingHorizontal: Spacing.xl,
  },
  headerContainer: {
    alignItems: 'center',
    marginBottom: Spacing.xxxl,
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
  formContainer: {
    backgroundColor: Colors.surface,
    borderRadius: BorderRadius.xl,
    padding: Spacing.xl,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 2},
    shadowOpacity: 0.06,
    shadowRadius: 8,
    elevation: 4,
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
  inputContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: Colors.background,
    borderRadius: BorderRadius.md,
    paddingHorizontal: Spacing.base,
    marginBottom: Spacing.base,
    height: 52,
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
  loginButton: {
    backgroundColor: Colors.primary,
    borderRadius: BorderRadius.md,
    height: 52,
    justifyContent: 'center',
    alignItems: 'center',
    marginTop: Spacing.sm,
  },
  loginButtonDisabled: {
    opacity: 0.7,
  },
  loginButtonText: {
    color: Colors.textInverse,
    fontSize: Fonts.sizes.lg,
    fontWeight: Fonts.weights.semibold,
  },
  registerContainer: {
    flexDirection: 'row',
    justifyContent: 'center',
    marginTop: 20,
  },
  registerText: {
    color: Colors.textSecondary,
    fontSize: 14,
  },
  registerLink: {
    color: Colors.primary,
    fontSize: 14,
    fontWeight: '600',
  },
});

export default LoginScreen;
