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
import {courierService} from '../../api/courierService';

const COURIER_TYPES = [
  {value: 2, label: 'Bireysel Kurye'},
  {value: 1, label: 'Restoran Kuryesi'},
];

const VEHICLE_TYPES = [
  {value: 'Motorcycle', label: 'Motosiklet', icon: 'motorbike'},
  {value: 'Bicycle', label: 'Bisiklet', icon: 'bicycle'},
  {value: 'Car', label: 'Otomobil', icon: 'car'},
  {value: 'Walking', label: 'Yaya', icon: 'walk'},
];

const CourierRegisterScreen = () => {
  const {refreshProfile} = useAuth();
  const [courierType, setCourierType] = useState(2);
  const [vehicleType, setVehicleType] = useState('Motorcycle');
  const [vehiclePlate, setVehiclePlate] = useState('');
  const [identityNumber, setIdentityNumber] = useState('');
  const [iban, setIban] = useState('');
  const [restaurantId, setRestaurantId] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const needsPlate = vehicleType === 'Motorcycle' || vehicleType === 'Car';

  const handleSubmit = async () => {
    if (!identityNumber.trim() || !iban.trim()) {
      Alert.alert('Uyarı', 'Lütfen zorunlu alanları doldurun');
      return;
    }
    if (needsPlate && !vehiclePlate.trim()) {
      Alert.alert('Uyarı', 'Araç plakası zorunludur');
      return;
    }
    if (courierType === 1 && !restaurantId.trim()) {
      Alert.alert('Uyarı', 'Restoran ID zorunludur');
      return;
    }

    setError('');
    setLoading(true);
    try {
      const data = {
        courierTypeId: courierType,
        vehicleType: vehicleType.toLowerCase(),
        identityNumber: identityNumber.trim(),
        iban: iban.trim(),
      };
      if (vehiclePlate.trim()) {
        data.vehiclePlate = vehiclePlate.trim();
      }
      if (courierType === 1 && restaurantId.trim()) {
        data.restaurantId = restaurantId.trim();
      }

      const response = await courierService.register(data);
      if (response.data.hasFailed) {
        const errorMsg =
          response.data.messages?.[0]?.description || 'Başvuru gönderilemedi';
        setError(errorMsg);
        Alert.alert('Hata', errorMsg);
      } else {
        // refreshProfile will set user, navigator auto-switches to MainTabs
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
            <Icon name="motorbike" size={48} color={Colors.primary} />
          </View>
          <Text style={styles.title}>Kurye Bilgileri</Text>
          <Text style={styles.subtitle}>
            Kurye başvurunuzu tamamlayın
          </Text>
        </View>

        <View style={styles.formContainer}>
          {error ? (
            <View style={styles.errorContainer}>
              <Icon name="alert-circle" size={18} color={Colors.error} />
              <Text style={styles.errorText}>{error}</Text>
            </View>
          ) : null}

          {/* Courier Type Selector */}
          <Text style={styles.label}>Kurye Tipi</Text>
          <View style={styles.selectorRow}>
            {COURIER_TYPES.map(type => (
              <TouchableOpacity
                key={type.value}
                style={[
                  styles.selectorButton,
                  courierType === type.value && styles.selectorButtonActive,
                ]}
                onPress={() => setCourierType(type.value)}>
                <Text
                  style={[
                    styles.selectorText,
                    courierType === type.value && styles.selectorTextActive,
                  ]}>
                  {type.label}
                </Text>
              </TouchableOpacity>
            ))}
          </View>

          {/* Vehicle Type Selector */}
          <Text style={styles.label}>Araç Tipi</Text>
          <View style={styles.vehicleRow}>
            {VEHICLE_TYPES.map(type => (
              <TouchableOpacity
                key={type.value}
                style={[
                  styles.vehicleButton,
                  vehicleType === type.value && styles.vehicleButtonActive,
                ]}
                onPress={() => setVehicleType(type.value)}>
                <Icon
                  name={type.icon}
                  size={22}
                  color={
                    vehicleType === type.value
                      ? Colors.primary
                      : Colors.textSecondary
                  }
                />
                <Text
                  style={[
                    styles.vehicleText,
                    vehicleType === type.value && styles.vehicleTextActive,
                  ]}>
                  {type.label}
                </Text>
              </TouchableOpacity>
            ))}
          </View>

          {/* Vehicle Plate */}
          {needsPlate && (
            <View style={styles.inputContainer}>
              <Icon
                name="card-text-outline"
                size={20}
                color={Colors.textSecondary}
                style={styles.inputIcon}
              />
              <TextInput
                style={styles.input}
                placeholder="Araç Plakası"
                placeholderTextColor={Colors.textTertiary}
                value={vehiclePlate}
                onChangeText={setVehiclePlate}
                autoCapitalize="characters"
              />
            </View>
          )}

          {/* Identity Number */}
          <View style={styles.inputContainer}>
            <Icon
              name="card-account-details-outline"
              size={20}
              color={Colors.textSecondary}
              style={styles.inputIcon}
            />
            <TextInput
              style={styles.input}
              placeholder="TC Kimlik No"
              placeholderTextColor={Colors.textTertiary}
              value={identityNumber}
              onChangeText={setIdentityNumber}
              keyboardType="number-pad"
              maxLength={11}
            />
          </View>

          {/* IBAN */}
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

          {/* Restaurant ID (only for RestaurantOwn) */}
          {courierType === 1 && (
            <View style={styles.inputContainer}>
              <Icon
                name="store-outline"
                size={20}
                color={Colors.textSecondary}
                style={styles.inputIcon}
              />
              <TextInput
                style={styles.input}
                placeholder="Restoran ID"
                placeholderTextColor={Colors.textTertiary}
                value={restaurantId}
                onChangeText={setRestaurantId}
              />
            </View>
          )}

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
  vehicleRow: {
    flexDirection: 'row',
    marginBottom: Spacing.base,
  },
  vehicleButton: {
    flex: 1,
    alignItems: 'center',
    paddingVertical: Spacing.md,
    borderRadius: BorderRadius.md,
    backgroundColor: Colors.background,
    marginHorizontal: Spacing.xs,
  },
  vehicleButtonActive: {
    backgroundColor: '#E8F2FF',
    borderWidth: 1,
    borderColor: Colors.primary,
  },
  vehicleText: {
    fontSize: Fonts.sizes.xs,
    fontWeight: Fonts.weights.medium,
    color: Colors.textSecondary,
    marginTop: Spacing.xs,
  },
  vehicleTextActive: {
    color: Colors.primary,
    fontWeight: Fonts.weights.semibold,
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

export default CourierRegisterScreen;
