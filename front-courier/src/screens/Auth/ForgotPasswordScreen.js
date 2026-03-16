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

export default function ForgotPasswordScreen({navigation}) {
  const [emailOrPhone, setEmailOrPhone] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSendCode = async () => {
    if (!emailOrPhone.trim()) {
      Alert.alert('Hata', 'Lutfen e-posta veya telefon numaranizi girin.');
      return;
    }
    setLoading(true);
    try {
      const res = await apiClient.post('/v1/auth/forgot-password', {
        emailOrPhone: emailOrPhone.trim(),
      });
      if (!res.data.hasFailed) {
        navigation.navigate('VerifyCode', {emailOrPhone: emailOrPhone.trim()});
      } else {
        Alert.alert('Hata', res.data.messages?.[0]?.description || 'Kod gonderilemedi.');
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
            <Text style={styles.logoIcon}>🔑</Text>
          </View>
          <Text style={styles.title}>Sifremi Unuttum</Text>
          <Text style={styles.subtitle}>
            E-posta veya telefon numaranizi girin
          </Text>
        </View>

        <View style={styles.form}>
          <Text style={styles.label}>E-posta veya Telefon</Text>
          <TextInput
            style={styles.input}
            value={emailOrPhone}
            onChangeText={setEmailOrPhone}
            placeholder="ornek@email.com veya 05xx..."
            placeholderTextColor="#aaa"
            autoCapitalize="none"
            autoCorrect={false}
          />

          <TouchableOpacity
            style={[styles.button, loading && styles.buttonDisabled]}
            onPress={handleSendCode}
            disabled={loading}
            activeOpacity={0.7}>
            {loading ? (
              <ActivityIndicator color="#fff" />
            ) : (
              <Text style={styles.buttonText}>Kod Gonder</Text>
            )}
          </TouchableOpacity>

          <TouchableOpacity
            style={styles.backLink}
            onPress={() => navigation.goBack()}>
            <Text style={styles.backText}>Giris ekranina don</Text>
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
  subtitle: {fontSize: 16, color: '#666', marginTop: 8, textAlign: 'center'},
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
  backLink: {marginTop: 20, alignItems: 'center'},
  backText: {fontSize: 14, color: '#FF6B00', fontWeight: '600'},
});
