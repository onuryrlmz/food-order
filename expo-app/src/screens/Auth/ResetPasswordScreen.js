import React, {useState} from 'react';
import {
  View,
  Text,
  StyleSheet,
  TextInput,
  TouchableOpacity,
  KeyboardAvoidingView,
  Platform,
  ScrollView,
  ActivityIndicator,
} from 'react-native';
import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import {Colors, Fonts, Spacing, BorderRadius} from '../../theme';
import {authService} from '../../api';
import {useToast} from '../../context/ToastContext';
import { router, useLocalSearchParams } from 'expo-router';

const ResetPasswordScreen = () => {
  const {showToast} = useToast();
  const {emailOrPhone, code} = useLocalSearchParams();
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [showPassword, setShowPassword] = useState(false);
  const [loading, setLoading] = useState(false);

  const handleReset = async () => {
    if (newPassword.length < 6) {
      showToast('Sifre en az 6 karakter olmalidir', 'warning');
      return;
    }
    if (newPassword !== confirmPassword) {
      showToast('Sifreler eslesmemektedir', 'warning');
      return;
    }
    setLoading(true);
    try {
      const res = await authService.resetPassword(emailOrPhone, code, newPassword);
      if (!res.data.hasFailed) {
        showToast('Sifreniz basariyla degistirildi', 'success');
        router.push('/(auth)/login');
      } else {
        showToast(res.data.messages?.[0]?.description || 'Sifre degistirilemedi', 'error');
      }
    } catch (error) {
      showToast('Bir hata olustu. Tekrar deneyin.', 'error');
    } finally {
      setLoading(false);
    }
  };

  return (
    <View style={styles.container}>
      <KeyboardAvoidingView
        behavior={Platform.OS === 'ios' ? 'padding' : 'height'}
        style={styles.flex}>
        <ScrollView
          contentContainerStyle={styles.scrollContent}
          keyboardShouldPersistTaps="handled"
          showsVerticalScrollIndicator={false}>
          <TouchableOpacity
            style={styles.backButton}
            onPress={() => router.back()}>
            <Icon name="arrow-left" size={24} color={Colors.text} />
          </TouchableOpacity>

          <View style={styles.header}>
            <View style={styles.iconBg}>
              <Icon name="lock-check-outline" size={36} color="#FFF" />
            </View>
            <Text style={styles.title}>Yeni Sifre</Text>
            <Text style={styles.subtitle}>
              Yeni sifrenizi belirleyin.
            </Text>
          </View>

          <View style={styles.form}>
            <View style={styles.inputContainer}>
              <Icon
                name="lock-outline"
                size={22}
                color={Colors.textSecondary}
                style={styles.inputIcon}
              />
              <TextInput
                style={styles.input}
                placeholder="Yeni sifre"
                placeholderTextColor={Colors.textTertiary}
                value={newPassword}
                onChangeText={setNewPassword}
                secureTextEntry={!showPassword}
              />
              <TouchableOpacity onPress={() => setShowPassword(!showPassword)}>
                <Icon
                  name={showPassword ? 'eye-off-outline' : 'eye-outline'}
                  size={22}
                  color={Colors.textSecondary}
                />
              </TouchableOpacity>
            </View>

            <View style={styles.inputContainer}>
              <Icon
                name="lock-outline"
                size={22}
                color={Colors.textSecondary}
                style={styles.inputIcon}
              />
              <TextInput
                style={styles.input}
                placeholder="Sifre tekrar"
                placeholderTextColor={Colors.textTertiary}
                value={confirmPassword}
                onChangeText={setConfirmPassword}
                secureTextEntry={!showPassword}
              />
            </View>

            <TouchableOpacity
              style={[styles.submitButton, loading && styles.submitButtonDisabled]}
              onPress={handleReset}
              activeOpacity={0.8}
              disabled={loading}>
              {loading ? (
                <ActivityIndicator color="#FFF" />
              ) : (
                <Text style={styles.submitButtonText}>Sifreyi Degistir</Text>
              )}
            </TouchableOpacity>
          </View>
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
  flex: {
    flex: 1,
  },
  scrollContent: {
    flexGrow: 1,
    paddingHorizontal: Spacing.xl,
    paddingVertical: Spacing.xxl,
  },
  backButton: {
    width: 44,
    height: 44,
    borderRadius: 22,
    backgroundColor: Colors.surface,
    justifyContent: 'center',
    alignItems: 'center',
    marginBottom: Spacing.lg,
  },
  header: {
    alignItems: 'center',
    marginBottom: Spacing.xxxl,
  },
  iconBg: {
    width: 72,
    height: 72,
    borderRadius: 22,
    backgroundColor: Colors.primary,
    justifyContent: 'center',
    alignItems: 'center',
    marginBottom: Spacing.lg,
  },
  title: {
    fontSize: Fonts.sizes.xxxl,
    fontWeight: Fonts.weights.heavy,
    color: Colors.text,
    marginBottom: Spacing.sm,
  },
  subtitle: {
    fontSize: Fonts.sizes.md,
    color: Colors.textSecondary,
    textAlign: 'center',
    lineHeight: 22,
  },
  form: {
    marginBottom: Spacing.xxl,
  },
  inputContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: Colors.surface,
    borderRadius: BorderRadius.lg,
    paddingHorizontal: Spacing.base,
    marginBottom: Spacing.md,
    height: 56,
    borderWidth: 1,
    borderColor: Colors.border,
  },
  inputIcon: {
    marginRight: Spacing.md,
  },
  input: {
    flex: 1,
    fontSize: Fonts.sizes.base,
    color: Colors.text,
  },
  submitButton: {
    backgroundColor: Colors.primary,
    borderRadius: BorderRadius.lg,
    height: 56,
    justifyContent: 'center',
    alignItems: 'center',
    marginTop: Spacing.md,
  },
  submitButtonDisabled: {
    opacity: 0.7,
  },
  submitButtonText: {
    color: '#FFF',
    fontSize: Fonts.sizes.lg,
    fontWeight: Fonts.weights.bold,
  },
});

export default ResetPasswordScreen;
