import React, {useState, useEffect} from 'react';
import {View, Text, TouchableOpacity, TextInput, StyleSheet, ActivityIndicator} from 'react-native';
import Modal from 'react-native-modal';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import {Colors, Fonts, Spacing, BorderRadius} from '../theme';
import {tipService} from '../api';
import {useToast} from '../context/ToastContext';

const TipModal = ({visible, onClose, orderId, onTipAdded}) => {
  const {showToast} = useToast();
  const [options, setOptions] = useState(null);
  const [selectedPercentage, setSelectedPercentage] = useState(null);
  const [customAmount, setCustomAmount] = useState('');
  const [loading, setLoading] = useState(false);
  const [existingTip, setExistingTip] = useState(null);
  const [loadingOptions, setLoadingOptions] = useState(true);

  useEffect(() => {
    if (visible && orderId) {
      loadData();
    }
  }, [visible, orderId]);

  const loadData = async () => {
    setLoadingOptions(true);
    try {
      // Check existing tip
      const tipRes = await tipService.getTipByOrder(orderId);
      if (!tipRes.data.hasFailed && tipRes.data.data) {
        setExistingTip(tipRes.data.data);
      }

      // Load options
      const optRes = await tipService.getTipOptions(orderId);
      if (!optRes.data.hasFailed) {
        setOptions(optRes.data.data);
      }
    } catch (e) {
      // ignore
    } finally {
      setLoadingOptions(false);
    }
  };

  const handleSubmit = async () => {
    if (!selectedPercentage && !customAmount) {
      showToast('Bahşiş tutarı seçin', 'warning');
      return;
    }
    setLoading(true);
    try {
      const data = selectedPercentage
        ? {orderId, presetPercentage: selectedPercentage}
        : {orderId, customAmount: parseFloat(customAmount)};

      const res = await tipService.addTip(data);
      if (!res.data.hasFailed) {
        showToast('Bahşiş eklendi!', 'success');
        onTipAdded?.();
        onClose();
      } else {
        showToast(res.data.messages?.[0]?.description || 'Hata', 'error');
      }
    } catch (e) {
      showToast('Bağlantı hatası', 'error');
    } finally {
      setLoading(false);
    }
  };

  const selectPercentage = (pct) => {
    setSelectedPercentage(pct);
    setCustomAmount('');
  };

  const handleCustomAmount = (val) => {
    setCustomAmount(val);
    setSelectedPercentage(null);
  };

  return (
    <Modal
      isVisible={visible}
      onBackdropPress={onClose}
      onSwipeComplete={onClose}
      swipeDirection="down"
      style={styles.modal}
    >
      <View style={styles.container}>
        <View style={styles.handle} />
        <Text style={styles.title}>Kuryeye Bahşiş</Text>

        {loadingOptions ? (
          <ActivityIndicator size="large" color={Colors.primary} style={{marginVertical: 30}} />
        ) : existingTip ? (
          <View style={styles.existingTip}>
            <Icon name="check-circle" size={48} color={Colors.success} />
            <Text style={styles.existingTipText}>
              Bu sipariş için {existingTip.amount.toFixed(2)} ₺ bahşiş eklediniz
            </Text>
          </View>
        ) : (
          <>
            <Text style={styles.subtitle}>
              Sipariş Tutarı: {options?.orderTotal?.toFixed(2)} ₺
            </Text>

            <View style={styles.percentageRow}>
              {options?.presetPercentages?.map((pct, idx) => (
                <TouchableOpacity
                  key={pct}
                  style={[
                    styles.percentageButton,
                    selectedPercentage === pct && styles.percentageButtonActive,
                  ]}
                  onPress={() => selectPercentage(pct)}
                >
                  <Text style={[
                    styles.percentageText,
                    selectedPercentage === pct && styles.percentageTextActive,
                  ]}>
                    %{pct}
                  </Text>
                  <Text style={[
                    styles.amountText,
                    selectedPercentage === pct && styles.amountTextActive,
                  ]}>
                    {options?.presetAmounts?.[idx]?.toFixed(2)} ₺
                  </Text>
                </TouchableOpacity>
              ))}
            </View>

            <Text style={styles.orText}>veya</Text>

            <TextInput
              style={styles.customInput}
              placeholder="Özel tutar (₺)"
              placeholderTextColor={Colors.textTertiary}
              keyboardType="decimal-pad"
              value={customAmount}
              onChangeText={handleCustomAmount}
            />

            <TouchableOpacity
              style={[styles.submitButton, loading && styles.submitButtonDisabled]}
              onPress={handleSubmit}
              disabled={loading}
            >
              {loading ? (
                <ActivityIndicator color="#FFF" />
              ) : (
                <Text style={styles.submitButtonText}>Bahşiş Ekle</Text>
              )}
            </TouchableOpacity>
          </>
        )}
      </View>
    </Modal>
  );
};

const styles = StyleSheet.create({
  modal: {
    justifyContent: 'flex-end',
    margin: 0,
  },
  container: {
    backgroundColor: Colors.surface,
    borderTopLeftRadius: BorderRadius.xxl,
    borderTopRightRadius: BorderRadius.xxl,
    padding: Spacing.xl,
    paddingBottom: Spacing.xxxl,
  },
  handle: {
    width: 40,
    height: 4,
    backgroundColor: Colors.border,
    borderRadius: 2,
    alignSelf: 'center',
    marginBottom: Spacing.lg,
  },
  title: {
    fontSize: Fonts.sizes.xl,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
    textAlign: 'center',
    marginBottom: Spacing.sm,
  },
  subtitle: {
    fontSize: Fonts.sizes.md,
    color: Colors.textSecondary,
    textAlign: 'center',
    marginBottom: Spacing.lg,
  },
  percentageRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginBottom: Spacing.base,
  },
  percentageButton: {
    flex: 1,
    marginHorizontal: Spacing.xs,
    paddingVertical: Spacing.md,
    borderRadius: BorderRadius.lg,
    borderWidth: 2,
    borderColor: Colors.border,
    alignItems: 'center',
  },
  percentageButtonActive: {
    borderColor: Colors.primary,
    backgroundColor: Colors.primary + '10',
  },
  percentageText: {
    fontSize: Fonts.sizes.lg,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
  },
  percentageTextActive: {
    color: Colors.primary,
  },
  amountText: {
    fontSize: Fonts.sizes.sm,
    color: Colors.textSecondary,
    marginTop: Spacing.xs,
  },
  amountTextActive: {
    color: Colors.primary,
  },
  orText: {
    fontSize: Fonts.sizes.md,
    color: Colors.textSecondary,
    textAlign: 'center',
    marginVertical: Spacing.sm,
  },
  customInput: {
    borderWidth: 1,
    borderColor: Colors.border,
    borderRadius: BorderRadius.lg,
    paddingHorizontal: Spacing.base,
    paddingVertical: Spacing.md,
    fontSize: Fonts.sizes.base,
    color: Colors.text,
    textAlign: 'center',
    marginBottom: Spacing.lg,
  },
  submitButton: {
    backgroundColor: Colors.primary,
    borderRadius: BorderRadius.lg,
    paddingVertical: Spacing.md,
    alignItems: 'center',
  },
  submitButtonDisabled: {
    opacity: 0.6,
  },
  submitButtonText: {
    color: '#FFF',
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.bold,
  },
  existingTip: {
    alignItems: 'center',
    paddingVertical: Spacing.xl,
  },
  existingTipText: {
    fontSize: Fonts.sizes.base,
    color: Colors.textSecondary,
    marginTop: Spacing.md,
    textAlign: 'center',
  },
});

export default TipModal;
