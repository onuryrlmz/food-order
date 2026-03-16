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

export default function VerifyCodeScreen({navigation, route}) {
  const {emailOrPhone} = route.params;
  const [code, setCode] = useState('');
  const [loading, setLoading] = useState(false);

  const handleVerify = async () => {
    if (code.length !== 6) {
      Alert.alert('Hata', 'Lutfen 6 haneli dogrulama kodunu girin.');
      return;
    }
    setLoading(true);
    try {
      const res = await apiClient.post('/v1/auth/verify-reset-code', {
        emailOrPhone,
        code,
      });
      if (!res.data.hasFailed) {
        navigation.navigate('ResetPassword', {emailOrPhone, code});
      } else {
        Alert.alert('Hata', res.data.messages?.[0]?.description || 'Gecersiz kod.');
      }
    } catch {
      Alert.alert('Hata', 'Bir hata olustu. Tekrar deneyin.');
    } finally {
      setLoading(false);
    }
  };

  const handleResend = async () => {
    try {
      const res = await apiClient.post('/v1/auth/forgot-password', {emailOrPhone});
      if (!res.data.hasFailed) {
        Alert.alert('Basarili', 'Yeni kod gonderildi.');
      } else {
        Alert.alert('Hata', res.data.messages?.[0]?.description || 'Kod gonderilemedi.');
      }
    } catch {
      Alert.alert('Hata', 'Kod gonderilemedi.');
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
            <Text style={styles.logoIcon}>🔢</Text>
          </View>
          <Text style={styles.title}>Dogrulama Kodu</Text>
          <Text style={styles.subtitle}>
            {emailOrPhone} adresine gonderilen 6 haneli kodu girin
          </Text>
        </View>

        <View style={styles.form}>
          <TextInput
            style={styles.codeInput}
            value={code}
            onChangeText={text => setCode(text.replace(/[^0-9]/g, '').slice(0, 6))}
            placeholder="000000"
            placeholderTextColor="#aaa"
            keyboardType="number-pad"
            maxLength={6}
            textAlign="center"
          />

          <TouchableOpacity
            style={[styles.button, loading && styles.buttonDisabled]}
            onPress={handleVerify}
            disabled={loading}
            activeOpacity={0.7}>
            {loading ? (
              <ActivityIndicator color="#fff" />
            ) : (
              <Text style={styles.buttonText}>Dogrula</Text>
            )}
          </TouchableOpacity>

          <TouchableOpacity style={styles.resendLink} onPress={handleResend}>
            <Text style={styles.resendText}>
              Kod gelmedi mi? <Text style={styles.resendAction}>Tekrar gonder</Text>
            </Text>
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
  subtitle: {fontSize: 14, color: '#666', marginTop: 8, textAlign: 'center', paddingHorizontal: 16},
  form: {
    backgroundColor: '#fff', padding: 24, borderRadius: 16,
    shadowColor: '#000', shadowOffset: {width: 0, height: 2},
    shadowOpacity: 0.08, shadowRadius: 8, elevation: 4,
  },
  codeInput: {
    borderWidth: 1, borderColor: '#e0e0e0', borderRadius: 12,
    padding: 16, fontSize: 28, fontWeight: 'bold', marginBottom: 20,
    backgroundColor: '#fafafa', color: '#1a1a1a', letterSpacing: 12,
  },
  button: {
    backgroundColor: '#FF6B00', paddingVertical: 16,
    borderRadius: 12, alignItems: 'center',
  },
  buttonDisabled: {opacity: 0.6},
  buttonText: {color: '#fff', fontSize: 16, fontWeight: '700'},
  resendLink: {marginTop: 20, alignItems: 'center'},
  resendText: {fontSize: 14, color: '#666'},
  resendAction: {color: '#FF6B00', fontWeight: '700'},
});
