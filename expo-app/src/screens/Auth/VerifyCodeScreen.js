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

const VerifyCodeScreen = () => {
  const {showToast} = useToast();
  const {emailOrPhone} = useLocalSearchParams();
  const [code, setCode] = useState('');
  const [loading, setLoading] = useState(false);

  const handleVerify = async () => {
    if (code.length !== 6) {
      showToast('Lutfen 6 haneli dogrulama kodunu girin', 'warning');
      return;
    }
    setLoading(true);
    try {
      const res = await authService.verifyResetCode(emailOrPhone, code);
      if (!res.data.hasFailed) {
        router.push({ pathname: '/(auth)/reset-password', params: { emailOrPhone, code } });
      } else {
        showToast(res.data.messages?.[0]?.description || 'Gecersiz kod', 'error');
      }
    } catch (error) {
      showToast('Bir hata olustu. Tekrar deneyin.', 'error');
    } finally {
      setLoading(false);
    }
  };

  const handleResend = async () => {
    try {
      const res = await authService.forgotPassword(emailOrPhone);
      if (!res.data.hasFailed) {
        showToast('Yeni kod gonderildi', 'success');
      } else {
        showToast(res.data.messages?.[0]?.description || 'Kod gonderilemedi', 'error');
      }
    } catch {
      showToast('Kod gonderilemedi', 'error');
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
              <Icon name="shield-check-outline" size={36} color="#FFF" />
            </View>
            <Text style={styles.title}>Dogrulama Kodu</Text>
            <Text style={styles.subtitle}>
              {emailOrPhone} adresine gonderilen 6 haneli kodu girin.
            </Text>
          </View>

          <View style={styles.form}>
            <View style={styles.inputContainer}>
              <TextInput
                style={styles.codeInput}
                placeholder="000000"
                placeholderTextColor={Colors.textTertiary}
                value={code}
                onChangeText={(text) => setCode(text.replace(/[^0-9]/g, '').slice(0, 6))}
                keyboardType="number-pad"
                maxLength={6}
                textAlign="center"
              />
            </View>

            <TouchableOpacity
              style={[styles.submitButton, loading && styles.submitButtonDisabled]}
              onPress={handleVerify}
              activeOpacity={0.8}
              disabled={loading}>
              {loading ? (
                <ActivityIndicator color="#FFF" />
              ) : (
                <Text style={styles.submitButtonText}>Dogrula</Text>
              )}
            </TouchableOpacity>

            <TouchableOpacity style={styles.resendLink} onPress={handleResend}>
              <Text style={styles.resendText}>Kod gelmedi mi? </Text>
              <Text style={styles.resendAction}>Tekrar gonder</Text>
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
    paddingHorizontal: Spacing.lg,
  },
  form: {
    marginBottom: Spacing.xxl,
  },
  inputContainer: {
    marginBottom: Spacing.lg,
  },
  codeInput: {
    backgroundColor: Colors.surface,
    borderRadius: BorderRadius.lg,
    height: 64,
    fontSize: 28,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
    letterSpacing: 12,
    borderWidth: 1,
    borderColor: Colors.border,
    paddingHorizontal: Spacing.xl,
  },
  submitButton: {
    backgroundColor: Colors.primary,
    borderRadius: BorderRadius.lg,
    height: 56,
    justifyContent: 'center',
    alignItems: 'center',
  },
  submitButtonDisabled: {
    opacity: 0.7,
  },
  submitButtonText: {
    color: '#FFF',
    fontSize: Fonts.sizes.lg,
    fontWeight: Fonts.weights.bold,
  },
  resendLink: {
    flexDirection: 'row',
    justifyContent: 'center',
    marginTop: Spacing.lg,
  },
  resendText: {
    fontSize: Fonts.sizes.md,
    color: Colors.textSecondary,
  },
  resendAction: {
    fontSize: Fonts.sizes.md,
    color: Colors.primary,
    fontWeight: Fonts.weights.bold,
  },
});

export default VerifyCodeScreen;
