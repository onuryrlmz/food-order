import React, {useState} from 'react';
import {
  View,
  Text,
  StyleSheet,
  TextInput,
  TouchableOpacity,
  ScrollView,
  Alert,
  ActivityIndicator,
  KeyboardAvoidingView,
  Platform,
} from 'react-native';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import {Colors, Fonts, Spacing, BorderRadius} from '../../theme';
import {addressService} from '../../api';

const AddAddressScreen = ({route, navigation}) => {
  const editAddress = route.params?.address;
  const isEditing = !!editAddress;

  const [addressName, setAddressName] = useState(editAddress?.addressName || '');
  const [firstName, setFirstName] = useState(editAddress?.firstName || '');
  const [lastName, setLastName] = useState(editAddress?.lastName || '');
  const [phone, setPhone] = useState(editAddress?.phone || '');
  const [addressLine1, setAddressLine1] = useState(editAddress?.addressLine1 || '');
  const [addressLine2, setAddressLine2] = useState(editAddress?.addressLine2 || '');
  const [addressType, setAddressType] = useState(editAddress?.addressType || 1);
  const [isDefault, setIsDefault] = useState(editAddress?.isDefault || false);
  const [loading, setLoading] = useState(false);

  const handleSave = async () => {
    if (!addressName.trim() || !firstName.trim() || !lastName.trim() || !phone.trim() || !addressLine1.trim()) {
      Alert.alert('Hata', 'Lütfen zorunlu alanları doldurun');
      return;
    }

    setLoading(true);
    try {
      const data = {
        addressType,
        addressName: addressName.trim(),
        firstName: firstName.trim(),
        lastName: lastName.trim(),
        phone: phone.trim(),
        cityId: '00000000-0000-0000-0000-000000000001',
        townId: '00000000-0000-0000-0000-000000000001',
        neighbourhoodId: '00000000-0000-0000-0000-000000000001',
        addressLine1: addressLine1.trim(),
        addressLine2: addressLine2.trim() || null,
        latitude: editAddress?.latitude || '41.0082',
        longitude: editAddress?.longitude || '28.9784',
        isDefault,
        invoiceType: 1,
      };

      let res;
      if (isEditing) {
        res = await addressService.update({...data, id: editAddress.id});
      } else {
        res = await addressService.add(data);
      }

      if (!res.data.hasFailed) {
        Alert.alert('Başarılı', isEditing ? 'Adres güncellendi' : 'Adres eklendi', [
          {text: 'Tamam', onPress: () => navigation.goBack()},
        ]);
      } else {
        Alert.alert('Hata', res.data.messages?.[0]?.description || 'İşlem başarısız');
      }
    } catch (e) {
      Alert.alert('Hata', 'Bir hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <TouchableOpacity style={styles.backButton} onPress={() => navigation.goBack()}>
          <Icon name="arrow-left" size={24} color={Colors.text} />
        </TouchableOpacity>
        <Text style={styles.headerTitle}>{isEditing ? 'Adresi Düzenle' : 'Yeni Adres'}</Text>
        <View style={{width: 44}} />
      </View>

      <KeyboardAvoidingView behavior={Platform.OS === 'ios' ? 'padding' : 'height'} style={{flex: 1}}>
        <ScrollView contentContainerStyle={styles.scrollContent} keyboardShouldPersistTaps="handled">
          <Text style={styles.sectionLabel}>Adres Tipi</Text>
          <View style={styles.typeRow}>
            <TouchableOpacity
              style={[styles.typeButton, addressType === 1 && styles.typeButtonActive]}
              onPress={() => setAddressType(1)}
            >
              <Icon name="home-outline" size={22} color={addressType === 1 ? '#FFF' : Colors.textSecondary} />
              <Text style={[styles.typeText, addressType === 1 && styles.typeTextActive]}>Ev</Text>
            </TouchableOpacity>
            <TouchableOpacity
              style={[styles.typeButton, addressType === 2 && styles.typeButtonActive]}
              onPress={() => setAddressType(2)}
            >
              <Icon name="briefcase-outline" size={22} color={addressType === 2 ? '#FFF' : Colors.textSecondary} />
              <Text style={[styles.typeText, addressType === 2 && styles.typeTextActive]}>İş</Text>
            </TouchableOpacity>
            <TouchableOpacity
              style={[styles.typeButton, addressType === 3 && styles.typeButtonActive]}
              onPress={() => setAddressType(3)}
            >
              <Icon name="map-marker-outline" size={22} color={addressType === 3 ? '#FFF' : Colors.textSecondary} />
              <Text style={[styles.typeText, addressType === 3 && styles.typeTextActive]}>Diğer</Text>
            </TouchableOpacity>
          </View>

          <Text style={styles.label}>Adres Başlığı *</Text>
          <View style={styles.inputContainer}>
            <TextInput
              style={styles.input}
              value={addressName}
              onChangeText={setAddressName}
              placeholder="Örn: Evim, İş Yerim"
              placeholderTextColor={Colors.textTertiary}
            />
          </View>

          <View style={styles.row}>
            <View style={{flex: 1}}>
              <Text style={styles.label}>Ad *</Text>
              <View style={styles.inputContainer}>
                <TextInput
                  style={styles.input}
                  value={firstName}
                  onChangeText={setFirstName}
                  placeholder="Ad"
                  placeholderTextColor={Colors.textTertiary}
                />
              </View>
            </View>
            <View style={{width: 10}} />
            <View style={{flex: 1}}>
              <Text style={styles.label}>Soyad *</Text>
              <View style={styles.inputContainer}>
                <TextInput
                  style={styles.input}
                  value={lastName}
                  onChangeText={setLastName}
                  placeholder="Soyad"
                  placeholderTextColor={Colors.textTertiary}
                />
              </View>
            </View>
          </View>

          <Text style={styles.label}>Telefon *</Text>
          <View style={styles.inputContainer}>
            <TextInput
              style={styles.input}
              value={phone}
              onChangeText={setPhone}
              placeholder="05xx xxx xx xx"
              placeholderTextColor={Colors.textTertiary}
              keyboardType="phone-pad"
            />
          </View>

          <Text style={styles.label}>Adres Satırı 1 *</Text>
          <View style={[styles.inputContainer, {height: 80}]}>
            <TextInput
              style={[styles.input, {textAlignVertical: 'top', paddingTop: 12}]}
              value={addressLine1}
              onChangeText={setAddressLine1}
              placeholder="Mahalle, sokak, no bilgileriniz"
              placeholderTextColor={Colors.textTertiary}
              multiline
              numberOfLines={3}
            />
          </View>

          <Text style={styles.label}>Adres Satırı 2 (Opsiyonel)</Text>
          <View style={styles.inputContainer}>
            <TextInput
              style={styles.input}
              value={addressLine2}
              onChangeText={setAddressLine2}
              placeholder="Bina adı, kat, daire"
              placeholderTextColor={Colors.textTertiary}
            />
          </View>

          <TouchableOpacity
            style={styles.defaultSwitch}
            onPress={() => setIsDefault(!isDefault)}
          >
            <Icon
              name={isDefault ? 'checkbox-marked' : 'checkbox-blank-outline'}
              size={24}
              color={isDefault ? Colors.primary : Colors.textSecondary}
            />
            <Text style={styles.defaultSwitchText}>Varsayılan adres olarak ayarla</Text>
          </TouchableOpacity>

          <TouchableOpacity
            style={[styles.saveButton, loading && styles.buttonDisabled]}
            onPress={handleSave}
            activeOpacity={0.8}
            disabled={loading}
          >
            {loading ? (
              <ActivityIndicator color="#FFF" />
            ) : (
              <Text style={styles.saveButtonText}>
                {isEditing ? 'Adresi Güncelle' : 'Adresi Kaydet'}
              </Text>
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
  sectionLabel: {
    fontSize: Fonts.sizes.sm,
    fontWeight: Fonts.weights.semibold,
    color: Colors.textSecondary,
    marginBottom: 10,
    marginLeft: 4,
  },
  typeRow: {
    flexDirection: 'row',
    gap: 10,
    marginBottom: Spacing.lg,
  },
  typeButton: {
    flex: 1,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: 14,
    borderRadius: BorderRadius.lg,
    backgroundColor: Colors.surface,
    borderWidth: 1.5,
    borderColor: Colors.border,
    gap: 6,
  },
  typeButtonActive: {
    backgroundColor: Colors.primary,
    borderColor: Colors.primary,
  },
  typeText: {
    fontSize: Fonts.sizes.md,
    fontWeight: Fonts.weights.semibold,
    color: Colors.textSecondary,
  },
  typeTextActive: {
    color: '#FFF',
  },
  label: {
    fontSize: Fonts.sizes.sm,
    fontWeight: Fonts.weights.semibold,
    color: Colors.textSecondary,
    marginBottom: 6,
    marginTop: Spacing.md,
    marginLeft: 4,
  },
  row: {
    flexDirection: 'row',
  },
  inputContainer: {
    backgroundColor: Colors.surface,
    borderRadius: BorderRadius.lg,
    paddingHorizontal: Spacing.base,
    height: 52,
    justifyContent: 'center',
    borderWidth: 1,
    borderColor: Colors.border,
  },
  input: {
    fontSize: Fonts.sizes.base,
    color: Colors.text,
  },
  defaultSwitch: {
    flexDirection: 'row',
    alignItems: 'center',
    marginTop: Spacing.lg,
    marginBottom: Spacing.xl,
  },
  defaultSwitchText: {
    fontSize: Fonts.sizes.md,
    color: Colors.text,
    fontWeight: Fonts.weights.medium,
    marginLeft: Spacing.sm,
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

export default AddAddressScreen;
