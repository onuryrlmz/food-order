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
import {Colors, Fonts, Spacing, BorderRadius} from '../../theme';
import {useAuth} from '../../context/AuthContext';
import {companyService} from '../../api/companyService';

const COMPANY_TYPES = [
  {value: 1, label: 'Şahıs'},
  {value: 2, label: 'Şirket'},
];

const CompanyRegisterScreen = () => {
  const {refreshProfile} = useAuth();
  const [name, setName] = useState('');
  const [legalName, setLegalName] = useState('');
  const [companyType, setCompanyType] = useState(1);
  const [taxCode, setTaxCode] = useState('');
  const [taxArea, setTaxArea] = useState('');
  const [identityNumber, setIdentityNumber] = useState('');
  const [iban, setIban] = useState('');
  const [phone, setPhone] = useState('');
  const [email, setEmail] = useState('');
  const [contactPerson, setContactPerson] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async () => {
    if (
      !name.trim() ||
      !legalName.trim() ||
      !taxCode.trim() ||
      !taxArea.trim() ||
      !iban.trim() ||
      !phone.trim() ||
      !email.trim() ||
      !contactPerson.trim()
    ) {
      Alert.alert('Uyarı', 'Lütfen tüm zorunlu alanları doldurun');
      return;
    }
    if (companyType === 1 && !identityNumber.trim()) {
      Alert.alert('Uyarı', 'Şahıs firması için TC/Kimlik No zorunludur');
      return;
    }

    setError('');
    setLoading(true);
    try {
      const data = {
        name: name.trim(),
        legalName: legalName.trim(),
        companyType,
        taxCode: taxCode.trim(),
        taxArea: taxArea.trim(),
        iban: iban.trim(),
        phone: phone.trim(),
        email: email.trim(),
        contactPerson: contactPerson.trim(),
      };
      if (identityNumber.trim()) {
        data.identityNumber = identityNumber.trim();
      }

      const response = await companyService.register(data);
      if (response.data.hasFailed) {
        const errorMsg =
          response.data.messages?.[0]?.description || 'Başvuru gönderilemedi';
        setError(errorMsg);
        Alert.alert('Hata', errorMsg);
      } else {
        // refreshProfile will set user, navigator auto-switches
        await refreshProfile();
      }
    } catch (e) {
      const errorMsg =
        e.response?.data?.messages?.[0]?.description || 'Bir hata oluştu';
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
          <View style={styles.iconContainer}>
            <Icon name="office-building" size={48} color={Colors.primary} />
          </View>
          <Text style={styles.title}>Firma Bilgileri</Text>
          <Text style={styles.subtitle}>
            Firma başvurunuzu tamamlayın
          </Text>
        </View>

        <View style={styles.formContainer}>
          {error ? (
            <View style={styles.errorContainer}>
              <Icon name="alert-circle" size={18} color={Colors.error} />
              <Text style={styles.errorText}>{error}</Text>
            </View>
          ) : null}

          {/* Company Type Selector */}
          <Text style={styles.label}>Firma Tipi</Text>
          <View style={styles.selectorRow}>
            {COMPANY_TYPES.map(type => (
              <TouchableOpacity
                key={type.value}
                style={[
                  styles.selectorButton,
                  companyType === type.value && styles.selectorButtonActive,
                ]}
                onPress={() => setCompanyType(type.value)}>
                <Text
                  style={[
                    styles.selectorText,
                    companyType === type.value && styles.selectorTextActive,
                  ]}>
                  {type.label}
                </Text>
              </TouchableOpacity>
            ))}
          </View>

          <View style={styles.inputContainer}>
            <Icon
              name="domain"
              size={20}
              color={Colors.textSecondary}
              style={styles.inputIcon}
            />
            <TextInput
              style={styles.input}
              placeholder="Firma Adı"
              placeholderTextColor={Colors.textTertiary}
              value={name}
              onChangeText={setName}
            />
          </View>

          <View style={styles.inputContainer}>
            <Icon
              name="file-document-outline"
              size={20}
              color={Colors.textSecondary}
              style={styles.inputIcon}
            />
            <TextInput
              style={styles.input}
              placeholder="Yasal Ünvan"
              placeholderTextColor={Colors.textTertiary}
              value={legalName}
              onChangeText={setLegalName}
            />
          </View>

          <View style={styles.inputContainer}>
            <Icon
              name="receipt"
              size={20}
              color={Colors.textSecondary}
              style={styles.inputIcon}
            />
            <TextInput
              style={styles.input}
              placeholder="Vergi No"
              placeholderTextColor={Colors.textTertiary}
              value={taxCode}
              onChangeText={setTaxCode}
              keyboardType="number-pad"
            />
          </View>

          <View style={styles.inputContainer}>
            <Icon
              name="map-marker-outline"
              size={20}
              color={Colors.textSecondary}
              style={styles.inputIcon}
            />
            <TextInput
              style={styles.input}
              placeholder="Vergi Dairesi"
              placeholderTextColor={Colors.textTertiary}
              value={taxArea}
              onChangeText={setTaxArea}
            />
          </View>

          {companyType === 1 && (
            <View style={styles.inputContainer}>
              <Icon
                name="card-account-details-outline"
                size={20}
                color={Colors.textSecondary}
                style={styles.inputIcon}
              />
              <TextInput
                style={styles.input}
                placeholder="TC/Kimlik No"
                placeholderTextColor={Colors.textTertiary}
                value={identityNumber}
                onChangeText={setIdentityNumber}
                keyboardType="number-pad"
                maxLength={11}
              />
            </View>
          )}

          <View style={styles.inputContainer}>
            <Icon
              name="bank-outline"
              size={20}
              color={Colors.textSecondary}
              style={styles.inputIcon}
            />
            <TextInput
              style={styles.input}
              placeholder="IBAN"
              placeholderTextColor={Colors.textTertiary}
              value={iban}
              onChangeText={setIban}
              autoCapitalize="characters"
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
              value={phone}
              onChangeText={setPhone}
              keyboardType="phone-pad"
            />
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
            />
          </View>

          <View style={styles.inputContainer}>
            <Icon
              name="account-tie-outline"
              size={20}
              color={Colors.textSecondary}
              style={styles.inputIcon}
            />
            <TextInput
              style={styles.input}
              placeholder="İletişim Kişisi"
              placeholderTextColor={Colors.textTertiary}
              value={contactPerson}
              onChangeText={setContactPerson}
            />
          </View>

          <TouchableOpacity
            style={[styles.submitButton, loading && styles.submitButtonDisabled]}
            onPress={handleSubmit}
            disabled={loading}>
            {loading ? (
              <ActivityIndicator color={Colors.textInverse} />
            ) : (
              <Text style={styles.submitButtonText}>Başvuru Gönder</Text>
            )}
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
  label: {
    fontSize: Fonts.sizes.md,
    fontWeight: Fonts.weights.semibold,
    color: Colors.text,
    marginBottom: Spacing.sm,
  },
  selectorRow: {
    flexDirection: 'row',
    marginBottom: Spacing.base,
  },
  selectorButton: {
    flex: 1,
    paddingVertical: Spacing.md,
    borderRadius: BorderRadius.md,
    backgroundColor: Colors.background,
    alignItems: 'center',
    marginHorizontal: Spacing.xs,
  },
  selectorButtonActive: {
    backgroundColor: Colors.primary,
  },
  selectorText: {
    fontSize: Fonts.sizes.md,
    fontWeight: Fonts.weights.semibold,
    color: Colors.textSecondary,
  },
  selectorTextActive: {
    color: Colors.textInverse,
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
});

export default CompanyRegisterScreen;
