import React, {useState} from 'react';
import {
  View,
  Text,
  TextInput,
  StyleSheet,
  TouchableOpacity,
  ScrollView,
  Alert,
  ActivityIndicator,
} from 'react-native';
import {registerCompany} from '../../api/courierService';

export default function CompanyRegistrationScreen({navigation}) {
  const [form, setForm] = useState({
    companyName: '',
    legalTitle: '',
    taxNumber: '',
    taxOffice: '',
    email: '',
    phone: '',
  });
  const [loading, setLoading] = useState(false);

  const handleChange = (key, value) => {
    setForm(prev => ({...prev, [key]: value}));
  };

  const handleSubmit = async () => {
    if (!form.companyName || !form.legalTitle || !form.taxNumber || !form.taxOffice || !form.email || !form.phone) {
      Alert.alert('Hata', 'Lütfen tüm alanları doldurun.');
      return;
    }

    setLoading(true);
    try {
      const result = await registerCompany(form);
      if (!result.hasFailed) {
        Alert.alert('Başarılı', 'Firma başvurunuz alındı. Onay sonrası aktif olacaktır.', [
          {text: 'Tamam', onPress: () => navigation.goBack()},
        ]);
      } else {
        Alert.alert(
          'Hata',
          result.messages?.map(m => m.description).join(', ') || 'Kayıt başarısız.',
        );
      }
    } catch (error) {
      Alert.alert('Hata', 'Bir hata oluştu.');
    } finally {
      setLoading(false);
    }
  };

  const fields = [
    {key: 'companyName', label: 'Firma Adı', placeholder: 'Firma adını girin'},
    {key: 'legalTitle', label: 'Yasal Ünvan', placeholder: 'Yasal ünvanı girin'},
    {key: 'taxNumber', label: 'Vergi No', placeholder: 'Vergi numarasını girin', keyboardType: 'numeric'},
    {key: 'taxOffice', label: 'Vergi Dairesi', placeholder: 'Vergi dairesini girin'},
    {key: 'email', label: 'E-posta', placeholder: 'E-posta adresini girin', keyboardType: 'email-address'},
    {key: 'phone', label: 'Telefon', placeholder: 'Telefon numarasını girin', keyboardType: 'phone-pad'},
  ];

  return (
    <ScrollView style={styles.container} contentContainerStyle={styles.content}>
      <Text style={styles.title}>Kurye Firması Kayıt</Text>
      <Text style={styles.subtitle}>
        Firma bilgilerinizi girerek başvurunuzu oluşturun.
      </Text>

      {fields.map(field => (
        <View key={field.key} style={styles.inputGroup}>
          <Text style={styles.label}>{field.label}</Text>
          <TextInput
            style={styles.input}
            placeholder={field.placeholder}
            placeholderTextColor="#999"
            value={form[field.key]}
            onChangeText={val => handleChange(field.key, val)}
            keyboardType={field.keyboardType || 'default'}
            autoCapitalize="none"
          />
        </View>
      ))}

      <TouchableOpacity
        style={[styles.submitButton, loading && styles.submitButtonDisabled]}
        onPress={handleSubmit}
        disabled={loading}
        activeOpacity={0.8}>
        {loading ? (
          <ActivityIndicator color="#fff" />
        ) : (
          <Text style={styles.submitButtonText}>Başvuru Yap</Text>
        )}
      </TouchableOpacity>
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f8f9fa',
  },
  content: {
    padding: 20,
    paddingBottom: 40,
  },
  title: {
    fontSize: 24,
    fontWeight: '700',
    color: '#1a1a1a',
    marginBottom: 6,
  },
  subtitle: {
    fontSize: 14,
    color: '#666',
    marginBottom: 24,
    lineHeight: 20,
  },
  inputGroup: {
    marginBottom: 16,
  },
  label: {
    fontSize: 14,
    fontWeight: '600',
    color: '#333',
    marginBottom: 6,
  },
  input: {
    backgroundColor: '#fff',
    borderWidth: 1,
    borderColor: '#ddd',
    borderRadius: 10,
    padding: 14,
    fontSize: 15,
    color: '#1a1a1a',
  },
  submitButton: {
    backgroundColor: '#FF6B00',
    paddingVertical: 16,
    borderRadius: 12,
    alignItems: 'center',
    marginTop: 12,
  },
  submitButtonDisabled: {
    opacity: 0.7,
  },
  submitButtonText: {
    color: '#fff',
    fontSize: 16,
    fontWeight: '700',
  },
});
