import React, {useState, useEffect, useCallback} from 'react';
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  RefreshControl,
  ActivityIndicator,
  TouchableOpacity,
} from 'react-native';
import MaterialCommunityIcons from 'react-native-vector-icons/MaterialCommunityIcons';
import {getMyCompany, getCompanyMembers} from '../../api/courierService';

export default function CompanyDashboardScreen({navigation}) {
  const [company, setCompany] = useState(null);
  const [memberCount, setMemberCount] = useState(0);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);

  const fetchData = useCallback(async () => {
    try {
      const [companyResult, membersResult] = await Promise.all([
        getMyCompany(),
        getCompanyMembers(),
      ]);
      if (!companyResult.hasFailed && companyResult.data) {
        setCompany(companyResult.data);
      }
      if (!membersResult.hasFailed && membersResult.data) {
        setMemberCount(membersResult.data.length || 0);
      }
    } catch (error) {
      console.error('Company data error:', error);
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  }, []);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  const onRefresh = () => {
    setRefreshing(true);
    fetchData();
  };

  const statusLabels = {
    0: {label: 'Onay Bekliyor', color: '#F9A825', bg: '#FFF9C4'},
    1: {label: 'Aktif', color: '#34C759', bg: '#E8F5E9'},
    2: {label: 'Askıya Alındı', color: '#FF9800', bg: '#FFF3E0'},
    3: {label: 'Yasaklandı', color: '#FF3B30', bg: '#FFEBEE'},
  };

  if (loading) {
    return (
      <View style={styles.center}>
        <ActivityIndicator size="large" color="#FF6B00" />
      </View>
    );
  }

  if (!company) {
    return (
      <View style={styles.center}>
        <MaterialCommunityIcons name="office-building-outline" size={48} color="#ccc" />
        <Text style={styles.emptyText}>Firma bilgisi bulunamadı</Text>
      </View>
    );
  }

  const status = statusLabels[company.statusId] || statusLabels[0];

  return (
    <ScrollView
      style={styles.container}
      contentContainerStyle={styles.content}
      refreshControl={
        <RefreshControl refreshing={refreshing} onRefresh={onRefresh} colors={['#FF6B00']} />
      }>
      <View style={styles.companyCard}>
        <View style={styles.companyHeader}>
          <View style={styles.iconContainer}>
            <MaterialCommunityIcons name="office-building" size={28} color="#FF6B00" />
          </View>
          <View style={{flex: 1}}>
            <Text style={styles.companyName}>{company.companyName}</Text>
            <Text style={styles.legalTitle}>{company.legalTitle}</Text>
          </View>
        </View>
        <View style={[styles.statusBadge, {backgroundColor: status.bg}]}>
          <View style={[styles.statusDot, {backgroundColor: status.color}]} />
          <Text style={[styles.statusLabel, {color: status.color}]}>{status.label}</Text>
        </View>
      </View>

      <View style={styles.statsRow}>
        <View style={styles.statCard}>
          <MaterialCommunityIcons name="account-group" size={24} color="#FF6B00" />
          <Text style={styles.statValue}>{memberCount}</Text>
          <Text style={styles.statLabel}>Kurye</Text>
        </View>
        <TouchableOpacity
          style={styles.statCard}
          onPress={() => navigation.navigate('RestaurantInvites')}>
          <MaterialCommunityIcons name="store-check" size={24} color="#FF6B00" />
          <Text style={styles.statLabel}>Restoran Davetleri</Text>
        </TouchableOpacity>
      </View>

      <View style={styles.infoSection}>
        <Text style={styles.sectionTitle}>Firma Bilgileri</Text>
        <InfoRow icon="file-document" label="Vergi No" value={company.taxNumber} />
        <InfoRow icon="bank" label="Vergi Dairesi" value={company.taxOffice} />
        <InfoRow icon="email" label="E-posta" value={company.contactEmail} />
        <InfoRow icon="phone" label="Telefon" value={company.contactPhone} />
      </View>

      <TouchableOpacity
        style={styles.manageButton}
        onPress={() => navigation.navigate('MemberManagement')}
        activeOpacity={0.8}>
        <MaterialCommunityIcons name="account-cog" size={20} color="#fff" />
        <Text style={styles.manageButtonText}>Kurye Yönetimi</Text>
      </TouchableOpacity>
    </ScrollView>
  );
}

function InfoRow({icon, label, value}) {
  return (
    <View style={styles.infoRow}>
      <MaterialCommunityIcons name={icon} size={18} color="#999" />
      <View style={{flex: 1, marginLeft: 10}}>
        <Text style={styles.infoLabel}>{label}</Text>
        <Text style={styles.infoValue}>{value || '-'}</Text>
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f8f9fa',
  },
  content: {
    padding: 16,
    paddingBottom: 32,
  },
  center: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: '#f8f9fa',
  },
  emptyText: {
    fontSize: 16,
    color: '#999',
    marginTop: 12,
  },
  companyCard: {
    backgroundColor: '#fff',
    borderRadius: 16,
    padding: 20,
    marginBottom: 16,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 1},
    shadowOpacity: 0.06,
    shadowRadius: 4,
    elevation: 2,
  },
  companyHeader: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: 12,
  },
  iconContainer: {
    width: 48,
    height: 48,
    borderRadius: 24,
    backgroundColor: '#FFF3E0',
    justifyContent: 'center',
    alignItems: 'center',
    marginRight: 14,
  },
  companyName: {
    fontSize: 20,
    fontWeight: '700',
    color: '#1a1a1a',
  },
  legalTitle: {
    fontSize: 13,
    color: '#888',
    marginTop: 2,
  },
  statusBadge: {
    flexDirection: 'row',
    alignItems: 'center',
    alignSelf: 'flex-start',
    paddingHorizontal: 12,
    paddingVertical: 6,
    borderRadius: 20,
  },
  statusDot: {
    width: 8,
    height: 8,
    borderRadius: 4,
    marginRight: 6,
  },
  statusLabel: {
    fontSize: 13,
    fontWeight: '600',
  },
  statsRow: {
    flexDirection: 'row',
    gap: 12,
    marginBottom: 16,
  },
  statCard: {
    flex: 1,
    backgroundColor: '#fff',
    borderRadius: 12,
    padding: 16,
    alignItems: 'center',
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 1},
    shadowOpacity: 0.04,
    shadowRadius: 3,
    elevation: 1,
  },
  statValue: {
    fontSize: 24,
    fontWeight: '700',
    color: '#1a1a1a',
    marginTop: 6,
  },
  statLabel: {
    fontSize: 12,
    color: '#888',
    marginTop: 4,
    textAlign: 'center',
  },
  infoSection: {
    backgroundColor: '#fff',
    borderRadius: 16,
    padding: 20,
    marginBottom: 16,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 1},
    shadowOpacity: 0.04,
    shadowRadius: 3,
    elevation: 1,
  },
  sectionTitle: {
    fontSize: 16,
    fontWeight: '700',
    color: '#1a1a1a',
    marginBottom: 14,
  },
  infoRow: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingVertical: 10,
    borderBottomWidth: 1,
    borderBottomColor: '#f0f0f0',
  },
  infoLabel: {
    fontSize: 12,
    color: '#999',
  },
  infoValue: {
    fontSize: 15,
    color: '#333',
    fontWeight: '500',
    marginTop: 2,
  },
  manageButton: {
    backgroundColor: '#FF6B00',
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: 16,
    borderRadius: 12,
    gap: 8,
  },
  manageButtonText: {
    color: '#fff',
    fontSize: 16,
    fontWeight: '700',
  },
});
