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
import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import {Colors, Fonts, Spacing, BorderRadius} from '../../theme';
import {authService} from '../../api';
import {useToast} from '../../context/ToastContext';

import { router } from 'expo-router';
const ChangePasswordScreen = () => {
  const {showToast} = useToast();
  const [currentPassword, setCurrentPassword] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [showCurrent, setShowCurrent] = useState(false);
  const [showNew, setShowNew] = useState(false);
  const [loading, setLoading] = useState(false);

  const handleChange = async () => {
    if (!currentPassword || !newPassword || !confirmPassword) {
      showToast('Lütfen tüm alanları doldurun', 'warning');
      return;
    }
    if (newPassword.length < 6) {
      showToast('Yeni şifre en az 6 karakter olmalıdır', 'warning');
      return;
    }
    if (newPassword !== confirmPassword) {
      showToast('Yeni şifreler eşleşmiyor', 'warning');
      return;
    }

    setLoading(true);
    try {
      const res = await authService.changePassword({
        currentPassword,
        newPassword,
      });
      if (!res.data.hasFailed) {
        showToast('Şifreniz başarıyla değiştirildi', 'success');
        router.back();
      } else {
        showToast(res.data.messages?.[0]?.description || 'Şifre değiştirilemedi', 'error');
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
        <TouchableOpacity style={styles.backButton} onPress={() => router.back()}>
          <Icon name="arrow-left" size={24} color={Colors.text} />
        </TouchableOpacity>
        <Text style={styles.headerTitle}>Şifre Değiştir</Text>
        <View style={{width: 44}} />
      </View>

      <KeyboardAvoidingView behavior={Platform.OS === 'ios' ? 'padding' : 'height'} style={{flex: 1}}>
        <ScrollView contentContainerStyle={styles.scrollContent} keyboardShouldPersistTaps="handled">
          <View style={styles.iconSection}>
            <View style={styles.lockIcon}>
              <Icon name="shield-lock-outline" size={40} color={Colors.primary} />
            </View>
            <Text style={styles.infoText}>
              Güvenliğiniz için şifrenizi düzenli olarak değiştirmenizi öneririz.
            </Text>
          </View>

          <View style={styles.form}>
            <Text style={styles.label}>Mevcut Şifre</Text>
            <View style={styles.inputContainer}>
              <Icon name="lock-outline" size={20} color={Colors.textSecondary} />
              <TextInput
                style={styles.input}
                value={currentPassword}
                onChangeText={setCurrentPassword}
                placeholder="Mevcut şifreniz"
                placeholderTextColor={Colors.textTertiary}
                secureTextEntry={!showCurrent}
              />
              <TouchableOpacity onPress={() => setShowCurrent(!showCurrent)}>
                <Icon name={showCurrent ? 'eye-off-outline' : 'eye-outline'} size={20} color={Colors.textSecondary} />
              </TouchableOpacity>
            </View>

            <Text style={styles.label}>Yeni Şifre</Text>
            <View style={styles.inputContainer}>
              <Icon name="lock-plus-outline" size={20} color={Colors.textSecondary} />
              <TextInput
                style={styles.input}
                value={newPassword}
                onChangeText={setNewPassword}
                placeholder="Yeni şifreniz"
                placeholderTextColor={Colors.textTertiary}
                secureTextEntry={!showNew}
              />
              <TouchableOpacity onPress={() => setShowNew(!showNew)}>
                <Icon name={showNew ? 'eye-off-outline' : 'eye-outline'} size={20} color={Colors.textSecondary} />
              </TouchableOpacity>
            </View>

            <Text style={styles.label}>Yeni Şifre (Tekrar)</Text>
            <View style={styles.inputContainer}>
              <Icon name="lock-check-outline" size={20} color={Colors.textSecondary} />
              <TextInput
                style={styles.input}
                value={confirmPassword}
                onChangeText={setConfirmPassword}
                placeholder="Yeni şifrenizi tekrar girin"
                placeholderTextColor={Colors.textTertiary}
                secureTextEntry={!showNew}
              />
            </View>
          </View>

          <TouchableOpacity
            style={[styles.saveButton, loading && styles.buttonDisabled]}
            onPress={handleChange}
            activeOpacity={0.8}
            disabled={loading}
          >
            {loading ? (
              <ActivityIndicator color="#FFF" />
            ) : (
              <Text style={styles.saveButtonText}>Şifreyi Değiştir</Text>
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
  iconSection: {
    alignItems: 'center',
    marginVertical: Spacing.xl,
  },
  lockIcon: {
    width: 80,
    height: 80,
    borderRadius: 40,
    backgroundColor: Colors.primary + '12',
    justifyContent: 'center',
    alignItems: 'center',
    marginBottom: Spacing.md,
  },
  infoText: {
    fontSize: Fonts.sizes.md,
    color: Colors.textSecondary,
    textAlign: 'center',
    lineHeight: 22,
    paddingHorizontal: Spacing.xl,
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
  input: {
    flex: 1,
    fontSize: Fonts.sizes.base,
    color: Colors.text,
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

export default ChangePasswordScreen;
