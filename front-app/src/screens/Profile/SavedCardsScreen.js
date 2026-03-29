import React, {useState, useEffect, useCallback} from 'react';
import {
  View,
  Text,
  StyleSheet,
  FlatList,
  TouchableOpacity,
  RefreshControl,
  TextInput,
  ActivityIndicator,
  KeyboardAvoidingView,
  Platform,
} from 'react-native';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import {Colors, Fonts, Spacing, BorderRadius} from '../../theme';
import api from '../../api';
import {useToast} from '../../context/ToastContext';
import LoadingSpinner from '../../components/LoadingSpinner';
import EmptyState from '../../components/EmptyState';

const getCardIcon = (association) => {
  if (!association) return 'credit-card-outline';
  const a = association.toLowerCase();
  if (a.includes('visa')) return 'credit-card-outline';
  if (a.includes('master')) return 'credit-card-outline';
  return 'credit-card-outline';
};

const SavedCardsScreen = ({navigation}) => {
  const {showToast, showConfirm} = useToast();
  const [cards, setCards] = useState([]);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [showAddForm, setShowAddForm] = useState(false);
  const [saving, setSaving] = useState(false);
  const [cardForm, setCardForm] = useState({
    cardAlias: '',
    cardHolderName: '',
    cardNumber: '',
    expireMonth: '',
    expireYear: '',
  });

  useEffect(() => {
    loadCards();
  }, []);

  const loadCards = async () => {
    try {
      const result = await api.customer.card.getList();
      if (result && !result.hasFailed && result.data) {
        setCards(result.data);
      } else {
        setCards([]);
      }
    } catch (e) {
      console.log('Cards load error:', e);
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  };

  const onRefresh = useCallback(() => {
    setRefreshing(true);
    loadCards();
  }, []);

  const handleDelete = (cardToken) => {
    showConfirm({
      title: 'Kartı Sil',
      message: 'Bu kartı silmek istediğinize emin misiniz?',
      confirmText: 'Sil',
      cancelText: 'İptal',
      confirmStyle: 'destructive',
      onConfirm: async () => {
        try {
          const result = await api.customer.card.remove({cardToken});
          if (result && !result.hasFailed) {
            setCards(prev => prev.filter(c => c.cardToken !== cardToken));
            showToast('Kart silindi', 'success');
          } else {
            showToast(result?.messages?.[0]?.description || 'Kart silinemedi', 'error');
          }
        } catch (e) {
          showToast('Kart silinemedi', 'error');
        }
      },
    });
  };

  const updateField = (field, value) => {
    setCardForm(prev => ({...prev, [field]: value}));
  };

  const formatCardNumber = (text) => {
    const cleaned = text.replace(/\D/g, '').slice(0, 16);
    return cleaned.replace(/(.{4})/g, '$1 ').trim();
  };

  const handleAddCard = async () => {
    const {cardAlias, cardHolderName, cardNumber, expireMonth, expireYear} = cardForm;
    if (!cardHolderName.trim()) {
      showToast('Kart sahibi adı giriniz', 'warning');
      return;
    }
    if (cardNumber.replace(/\s/g, '').length < 15) {
      showToast('Geçerli bir kart numarası giriniz', 'warning');
      return;
    }
    if (!expireMonth || !expireYear) {
      showToast('Son kullanma tarihi giriniz', 'warning');
      return;
    }

    setSaving(true);
    try {
      const result = await api.customer.card.create({
        cardAlias: cardAlias.trim() || cardHolderName.trim(),
        cardNumber: cardNumber.replace(/\s/g, ''),
        expireYear: '20' + expireYear,
        expireMonth: expireMonth,
        cardHolderName: cardHolderName.trim(),
      });
      if (result && !result.hasFailed) {
        showToast('Kart kaydedildi', 'success');
        setCardForm({cardAlias: '', cardHolderName: '', cardNumber: '', expireMonth: '', expireYear: ''});
        setShowAddForm(false);
        await loadCards();
      } else {
        showToast(result?.messages?.[0]?.description || 'Kart kaydedilemedi', 'error');
      }
    } catch (e) {
      showToast('Kart kaydedilemedi', 'error');
    } finally {
      setSaving(false);
    }
  };

  const renderCard = ({item}) => (
    <View style={styles.cardItem}>
      <View style={styles.cardHeader}>
        <View style={styles.cardIconBg}>
          <Icon name={getCardIcon(item.cardAssociation)} size={22} color={Colors.primary} />
        </View>
        <View style={styles.cardInfo}>
          <Text style={styles.cardName}>
            {item.cardAlias || item.cardFamily || item.cardAssociation || 'Kart'}
          </Text>
          <Text style={styles.cardNumber}>**** **** **** {item.lastFourDigits}</Text>
        </View>
      </View>
      <View style={styles.cardDetails}>
        <View style={styles.cardDetailRow}>
          <Text style={styles.cardDetailLabel}>Banka</Text>
          <Text style={styles.cardDetailValue}>{item.cardBankName || '-'}</Text>
        </View>
        <View style={styles.cardDetailRow}>
          <Text style={styles.cardDetailLabel}>Tür</Text>
          <Text style={styles.cardDetailValue}>{item.cardAssociation || '-'} {item.cardType || ''}</Text>
        </View>
        {item.expireMonth && item.expireYear && (
          <View style={styles.cardDetailRow}>
            <Text style={styles.cardDetailLabel}>Son Kullanma</Text>
            <Text style={styles.cardDetailValue}>{item.expireMonth}/{item.expireYear?.slice(-2)}</Text>
          </View>
        )}
      </View>
      <TouchableOpacity
        style={styles.deleteButton}
        onPress={() => handleDelete(item.cardToken)}
        activeOpacity={0.7}
      >
        <Icon name="delete-outline" size={16} color={Colors.error} />
        <Text style={styles.deleteButtonText}>Kartı Sil</Text>
      </TouchableOpacity>
    </View>
  );

  const renderAddForm = () => (
    <View style={styles.addFormContainer}>
      <View style={styles.addFormHeader}>
        <Text style={styles.addFormTitle}>Yeni Kart Ekle</Text>
        <TouchableOpacity onPress={() => setShowAddForm(false)}>
          <Icon name="close" size={22} color={Colors.textSecondary} />
        </TouchableOpacity>
      </View>
      <TextInput
        style={styles.input}
        placeholder="Kart adı (ör: İş Bankası Kartım)"
        placeholderTextColor={Colors.textTertiary}
        value={cardForm.cardAlias}
        onChangeText={t => updateField('cardAlias', t)}
        maxLength={50}
      />
      <TextInput
        style={styles.input}
        placeholder="Kart sahibi adı"
        placeholderTextColor={Colors.textTertiary}
        value={cardForm.cardHolderName}
        onChangeText={t => updateField('cardHolderName', t)}
        autoCapitalize="words"
      />
      <TextInput
        style={styles.input}
        placeholder="Kart numarası"
        placeholderTextColor={Colors.textTertiary}
        value={cardForm.cardNumber}
        onChangeText={t => updateField('cardNumber', formatCardNumber(t))}
        keyboardType="numeric"
        maxLength={19}
      />
      <View style={styles.inputRow}>
        <TextInput
          style={[styles.input, styles.inputSmall]}
          placeholder="Ay (MM)"
          placeholderTextColor={Colors.textTertiary}
          value={cardForm.expireMonth}
          onChangeText={t => updateField('expireMonth', t.replace(/\D/g, '').slice(0, 2))}
          keyboardType="numeric"
          maxLength={2}
        />
        <TextInput
          style={[styles.input, styles.inputSmall]}
          placeholder="Yıl (YY)"
          placeholderTextColor={Colors.textTertiary}
          value={cardForm.expireYear}
          onChangeText={t => updateField('expireYear', t.replace(/\D/g, '').slice(0, 2))}
          keyboardType="numeric"
          maxLength={2}
        />
      </View>
      <TouchableOpacity
        style={[styles.saveButton, saving && styles.saveButtonDisabled]}
        onPress={handleAddCard}
        disabled={saving}
        activeOpacity={0.8}
      >
        {saving ? (
          <ActivityIndicator color="#FFF" size="small" />
        ) : (
          <>
            <Icon name="credit-card-plus-outline" size={18} color="#FFF" />
            <Text style={styles.saveButtonText}>Kartı Kaydet</Text>
          </>
        )}
      </TouchableOpacity>
    </View>
  );

  if (loading) {
    return <LoadingSpinner message="Kartlar yükleniyor..." />;
  }

  return (
    <KeyboardAvoidingView
      style={styles.container}
      behavior={Platform.OS === 'ios' ? 'padding' : undefined}
    >
      <View style={styles.header}>
        <TouchableOpacity style={styles.backButton} onPress={() => navigation.goBack()}>
          <Icon name="arrow-left" size={24} color={Colors.text} />
        </TouchableOpacity>
        <Text style={styles.headerTitle}>Kayıtlı Kartlarım</Text>
        <TouchableOpacity
          style={styles.addHeaderButton}
          onPress={() => setShowAddForm(true)}
        >
          <Icon name="plus" size={24} color={Colors.primary} />
        </TouchableOpacity>
      </View>

      <FlatList
        data={cards}
        keyExtractor={(item) => item.cardToken}
        renderItem={renderCard}
        ListHeaderComponent={showAddForm ? renderAddForm : null}
        ListEmptyComponent={
          !showAddForm ? (
            <EmptyState
              icon="credit-card-off-outline"
              title="Kayıtlı Kart Yok"
              message="Henüz kayıtlı kartınız yok. Yeni kart ekleyebilirsiniz."
              actionLabel="Kart Ekle"
              onAction={() => setShowAddForm(true)}
            />
          ) : null
        }
        refreshControl={
          <RefreshControl refreshing={refreshing} onRefresh={onRefresh} tintColor={Colors.primary} />
        }
        contentContainerStyle={styles.listContent}
        showsVerticalScrollIndicator={false}
      />
    </KeyboardAvoidingView>
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
  addHeaderButton: {
    width: 44,
    height: 44,
    borderRadius: 22,
    backgroundColor: Colors.primary + '12',
    justifyContent: 'center',
    alignItems: 'center',
  },
  listContent: {
    paddingBottom: Spacing.xxl,
    flexGrow: 1,
  },
  cardItem: {
    backgroundColor: Colors.surface,
    marginHorizontal: Spacing.base,
    marginTop: Spacing.md,
    borderRadius: BorderRadius.lg,
    padding: Spacing.base,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 1},
    shadowOpacity: 0.04,
    shadowRadius: 4,
    elevation: 2,
    borderWidth: 1,
    borderColor: Colors.borderLight,
  },
  cardHeader: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: Spacing.md,
  },
  cardIconBg: {
    width: 44,
    height: 44,
    borderRadius: 12,
    backgroundColor: Colors.primary + '12',
    justifyContent: 'center',
    alignItems: 'center',
  },
  cardInfo: {
    flex: 1,
    marginLeft: Spacing.md,
  },
  cardName: {
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
  },
  cardNumber: {
    fontSize: Fonts.sizes.sm,
    color: Colors.textSecondary,
    marginTop: 2,
    fontFamily: Platform.OS === 'ios' ? 'Menlo' : 'monospace',
  },
  cardDetails: {
    backgroundColor: Colors.background,
    borderRadius: BorderRadius.md,
    padding: Spacing.sm,
    marginBottom: Spacing.md,
  },
  cardDetailRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    paddingVertical: 4,
  },
  cardDetailLabel: {
    fontSize: Fonts.sizes.sm,
    color: Colors.textTertiary,
  },
  cardDetailValue: {
    fontSize: Fonts.sizes.sm,
    color: Colors.text,
    fontWeight: Fonts.weights.medium,
  },
  deleteButton: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: 10,
    borderTopWidth: 1,
    borderTopColor: Colors.borderLight,
  },
  deleteButtonText: {
    fontSize: Fonts.sizes.sm,
    color: Colors.error,
    fontWeight: Fonts.weights.semibold,
    marginLeft: 6,
  },
  addFormContainer: {
    backgroundColor: Colors.surface,
    marginHorizontal: Spacing.base,
    marginTop: Spacing.md,
    borderRadius: BorderRadius.lg,
    padding: Spacing.base,
    borderWidth: 1,
    borderColor: Colors.primary + '30',
  },
  addFormHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: Spacing.md,
  },
  addFormTitle: {
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
  },
  input: {
    backgroundColor: Colors.background,
    borderRadius: BorderRadius.md,
    paddingHorizontal: Spacing.md,
    paddingVertical: 12,
    fontSize: Fonts.sizes.base,
    color: Colors.text,
    borderWidth: 1,
    borderColor: Colors.borderLight,
    marginBottom: Spacing.sm,
  },
  inputRow: {
    flexDirection: 'row',
    gap: Spacing.sm,
  },
  inputSmall: {
    flex: 1,
  },
  saveButton: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    backgroundColor: Colors.primary,
    borderRadius: BorderRadius.md,
    paddingVertical: 14,
    marginTop: Spacing.sm,
    gap: 8,
  },
  saveButtonDisabled: {
    opacity: 0.6,
  },
  saveButtonText: {
    color: '#FFF',
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.bold,
  },
});

export default SavedCardsScreen;
