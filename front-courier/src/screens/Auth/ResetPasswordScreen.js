import React, {useState} from 'react';
import {
  View,
  Text,
  TextInput,
  StyleSheet,
  Alert,
  KeyboardAvoidingView,
  Platform,
  TouchableOpacity,
  ScrollView,
  ActivityIndicator,
} from 'react-native';
import apiClient from '../../api/client';

export default function ResetPasswordScreen({navigation, route}) {
  const {emailOrPhone, code} = route.params;
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [loading, setLoading] = useState(false);

  const handleReset = async () => {
    if (newPassword.length < 6) {
      Alert.alert('Hata', 'Sifre en az 6 karakter olmalidir.');
      return;
    }
    if (newPassword !== confirmPassword) {
      Alert.alert('Hata', 'Sifreler eslesmemektedir.');
      return;
    }
    setLoading(true);
    try {
      const res = await apiClient.post('/v1/auth/reset-password', {
        emailOrPhone,
        code,
        newPassword,
      });
      if (!res.data.hasFailed) {
        Alert.alert('Basarili', 'Sifreniz degistirildi. Giris yapabilirsiniz.', [
          {text: 'Tamam', onPress: () => navigation.navigate('Login')},
        ]);
      } else {
        Alert.alert('Hata', res.data.messages?.[0]?.description || 'Sifre degistirilemedi.');
      }
    } catch {
      Alert.alert('Hata', 'Bir hata olustu. Tekrar deneyin.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <KeyboardAvoidingView
      style={styles.container}
      behavior={Platform.OS === 'ios' ? 'padding' : undefined}>
      <ScrollView
        contentContainerStyle={styles.content}
        keyboardShouldPersistTaps="handled">
        <View style={styles.header}>
          <View style={styles.logoContainer}>
            <Text style={styles.logoIcon}>🔒</Text>
          </View>
          <Text style={styles.title}>Yeni Sifre</Text>
          <Text style={styles.subtitle}>Yeni sifrenizi belirleyin</Text>
        </View>

        <View style={styles.form}>
          <Text style={styles.label}>Yeni Sifre</Text>
          <TextInput
            style={styles.input}
            value={newPassword}
            onChangeText={setNewPassword}
            placeholder="••••••••"
            placeholderTextColor="#aaa"
            secureTextEntry
          />

          <Text style={styles.label}>Sifre Tekrar</Text>
          <TextInput
            style={styles.input}
            value={confirmPassword}
            onChangeText={setConfirmPassword}
            placeholder="••••••••"
            placeholderTextColor="#aaa"
            secureTextEntry
          />

          <TouchableOpacity
            style={[styles.button, loading && styles.buttonDisabled]}
            onPress={handleReset}
            disabled={loading}
            activeOpacity={0.7}>
            {loading ? (
              <ActivityIndicator color="#fff" />
            ) : (
              <Text style={styles.buttonText}>Sifreyi Degistir</Text>
            )}
          </TouchableOpacity>
        </View>
      </ScrollView>
    </KeyboardAvoidingView>
  );
}

const styles = StyleSheet.create({
  container: {flex: 1, backgroundColor: '#f8f9fa'},
  content: {flex: 1, justifyContent: 'center', padding: 24},
  header: {alignItems: 'center', marginBottom: 40},
  logoContainer: {
    width: 80, height: 80, borderRadius: 40, backgroundColor: '#FFF3E0',
    justifyContent: 'center', alignItems: 'center', marginBottom: 16,
  },
  logoIcon: {fontSize: 40},
  title: {fontSize: 28, fontWeight: 'bold', color: '#1a1a1a'},
  subtitle: {fontSize: 16, color: '#666', marginTop: 8},
  form: {
    backgroundColor: '#fff', padding: 24, borderRadius: 16,
    shadowColor: '#000', shadowOffset: {width: 0, height: 2},
    shadowOpacity: 0.08, shadowRadius: 8, elevation: 4,
  },
  label: {fontSize: 14, fontWeight: '600', marginBottom: 8, color: '#333'},
  input: {
    borderWidth: 1, borderColor: '#e0e0e0', borderRadius: 12,
    padding: 14, fontSize: 16, marginBottom: 20,
    backgroundColor: '#fafafa', color: '#1a1a1a',
  },
  button: {
    backgroundColor: '#FF6B00', paddingVertical: 16,
    borderRadius: 12, alignItems: 'center',
  },
  buttonDisabled: {opacity: 0.6},
  buttonText: {color: '#fff', fontSize: 16, fontWeight: '700'},
});
