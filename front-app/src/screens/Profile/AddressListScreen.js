import React, {useState, useEffect, useCallback} from 'react';
import {
  View,
  Text,
  StyleSheet,
  FlatList,
  TouchableOpacity,
  RefreshControl,
} from 'react-native';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import {Colors, Fonts, Spacing, BorderRadius} from '../../theme';
import api from '../../api';
import {useToast} from '../../context/ToastContext';
import LoadingSpinner from '../../components/LoadingSpinner';
import EmptyState from '../../components/EmptyState';

const AddressListScreen = ({navigation}) => {
  const {showToast, showConfirm} = useToast();
  const [addresses, setAddresses] = useState([]);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);

  useEffect(() => {
    loadAddresses();
  }, []);

  useEffect(() => {
    const unsubscribe = navigation.addListener('focus', () => {
      loadAddresses();
    });
    return unsubscribe;
  }, [navigation]);

  const loadAddresses = async () => {
    try {
      const result = await api.customer.address.getList();
      console.log('Address API response:', JSON.stringify(result));
      const list = result?.rawData || result?.data || [];
      const items = Array.isArray(list) ? list : [];
      setAddresses(items);
    } catch (e) {
      console.log('Address list error:', e);
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  };

  const onRefresh = useCallback(() => {
    setRefreshing(true);
    loadAddresses();
  }, []);

  const handleSetDefault = async (id) => {
    try {
      const result = await api.customer.address.setDefault({id});
      if (!result.hasFailed) {
        loadAddresses();
      }
    } catch (e) {
      showToast('Varsayılan adres değiştirilemedi', 'error');
    }
  };

  const handleDelete = (id) => {
    showConfirm({
      title: 'Adresi Sil',
      message: 'Bu adresi silmek istediğinize emin misiniz?',
      confirmText: 'Sil',
      cancelText: 'İptal',
      confirmStyle: 'destructive',
      onConfirm: async () => {
        try {
          const result = await api.customer.address.remove({id});
          if (!result.hasFailed) {
            loadAddresses();
          } else {
            showToast(result.messages?.[0]?.description || 'Silinemedi', 'error');
          }
        } catch (e) {
          showToast('Adres silinemedi', 'error');
        }
      },
    });
  };

  const renderAddress = ({item}) => (
    <TouchableOpacity
      style={[styles.addressCard, item.isDefault && styles.addressCardSelected]}
      activeOpacity={0.7}
      onPress={() => {
        if (!item.isDefault) {
          handleSetDefault(item.id);
        }
      }}
    >
      <View style={styles.addressHeader}>
        <View style={[styles.addressIconBg, item.isDefault && styles.addressIconBgSelected]}>
          <Icon
            name={item.isDefault ? 'check-circle' : item.addressType === 1 ? 'home-outline' : 'briefcase-outline'}
            size={20}
            color={item.isDefault ? Colors.success : Colors.primary}
          />
        </View>
        <View style={styles.addressTextContainer}>
          <View style={styles.addressNameRow}>
            <Text style={styles.addressName}>{item.addressName}</Text>
            {item.isDefault && (
              <View style={styles.defaultBadge}>
                <Text style={styles.defaultBadgeText}>Seçili</Text>
              </View>
            )}
          </View>
          <Text style={styles.addressFullName}>{item.firstName} {item.lastName}</Text>
        </View>
        {!item.isDefault && (
          <View style={styles.selectIndicator}>
            <Text style={styles.selectIndicatorText}>Seç</Text>
          </View>
        )}
      </View>

      <Text style={styles.addressLine} numberOfLines={2}>
        {item.addressLine1}
        {item.addressLine2 ? `, ${item.addressLine2}` : ''}
      </Text>
      <Text style={styles.addressPhone}>{item.phone}</Text>

      <View style={styles.addressActions}>
        <TouchableOpacity
          style={styles.actionButton}
          onPress={() => navigation.navigate('AddAddress', {address: item})}
        >
          <Icon name="pencil-outline" size={16} color={Colors.info} />
          <Text style={[styles.actionText, {color: Colors.info}]}>Düzenle</Text>
        </TouchableOpacity>
        <TouchableOpacity
          style={styles.actionButton}
          onPress={() => handleDelete(item.id)}
        >
          <Icon name="delete-outline" size={16} color={Colors.error} />
          <Text style={[styles.actionText, {color: Colors.error}]}>Sil</Text>
        </TouchableOpacity>
      </View>
    </TouchableOpacity>
  );

  if (loading) {
    return <LoadingSpinner message="Adresler yükleniyor..." />;
  }

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <TouchableOpacity style={styles.backButton} onPress={() => navigation.goBack()}>
          <Icon name="arrow-left" size={24} color={Colors.text} />
        </TouchableOpacity>
        <Text style={styles.headerTitle}>Adreslerim</Text>
        <TouchableOpacity
          style={styles.addButton}
          onPress={() => navigation.navigate('AddAddress')}
        >
          <Icon name="plus" size={24} color={Colors.primary} />
        </TouchableOpacity>
      </View>

      <FlatList
        data={addresses}
        keyExtractor={(item) => item.id}
        renderItem={renderAddress}
        ListEmptyComponent={
          <EmptyState
            icon="map-marker-off"
            title="Adres Bulunamadı"
            message="Henüz kayıtlı adresiniz yok. Yeni adres ekleyin."
            actionLabel="Adres Ekle"
            onAction={() => navigation.navigate('AddAddress')}
          />
        }
        refreshControl={
          <RefreshControl refreshing={refreshing} onRefresh={onRefresh} tintColor={Colors.primary} />
        }
        contentContainerStyle={styles.listContent}
        showsVerticalScrollIndicator={false}
      />
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
  addButton: {
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
  addressCard: {
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
    borderWidth: 1.5,
    borderColor: 'transparent',
  },
  addressCardSelected: {
    borderColor: Colors.success,
    backgroundColor: Colors.surface,
  },
  addressHeader: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: Spacing.sm,
  },
  addressIconBg: {
    width: 40,
    height: 40,
    borderRadius: 12,
    backgroundColor: Colors.primary + '12',
    justifyContent: 'center',
    alignItems: 'center',
  },
  addressIconBgSelected: {
    backgroundColor: Colors.success + '15',
  },
  addressTextContainer: {
    flex: 1,
    marginLeft: Spacing.md,
  },
  addressNameRow: {
    flexDirection: 'row',
    alignItems: 'center',
  },
  addressName: {
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
    marginRight: 8,
  },
  defaultBadge: {
    backgroundColor: Colors.success + '15',
    paddingHorizontal: 8,
    paddingVertical: 2,
    borderRadius: 6,
  },
  defaultBadgeText: {
    fontSize: Fonts.sizes.xs,
    color: Colors.success,
    fontWeight: Fonts.weights.semibold,
  },
  addressFullName: {
    fontSize: Fonts.sizes.sm,
    color: Colors.textSecondary,
    marginTop: 2,
  },
  addressLine: {
    fontSize: Fonts.sizes.md,
    color: Colors.textSecondary,
    lineHeight: 20,
    marginBottom: 4,
  },
  addressPhone: {
    fontSize: Fonts.sizes.sm,
    color: Colors.textTertiary,
    marginBottom: Spacing.md,
  },
  addressActions: {
    flexDirection: 'row',
    borderTopWidth: 1,
    borderTopColor: Colors.borderLight,
    paddingTop: Spacing.md,
    gap: 12,
  },
  actionButton: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingHorizontal: 10,
    paddingVertical: 6,
    borderRadius: 8,
    backgroundColor: Colors.borderLight,
  },
  actionText: {
    fontSize: Fonts.sizes.xs,
    fontWeight: Fonts.weights.semibold,
    marginLeft: 4,
  },
  selectIndicator: {
    backgroundColor: Colors.primary,
    paddingHorizontal: 14,
    paddingVertical: 8,
    borderRadius: 8,
  },
  selectIndicatorText: {
    color: '#FFF',
    fontSize: Fonts.sizes.sm,
    fontWeight: Fonts.weights.bold,
  },
});

export default AddressListScreen;
