import React, {useState, useEffect, useCallback} from 'react';
import {
  View,
  Text,
  StyleSheet,
  FlatList,
  TouchableOpacity,
  RefreshControl,
} from 'react-native';
import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import {Colors, Fonts, Spacing, BorderRadius} from '../../theme';
import {supportService} from '../../api';
import {useAuth} from '../../context/AuthContext';
import LoadingSpinner from '../../components/LoadingSpinner';
import EmptyState from '../../components/EmptyState';

const STATUS_MAP = {
  Open: {label: 'Açık', color: '#5856D6'},
  InProgress: {label: 'İşlemde', color: '#FF9500'},
  Resolved: {label: 'Çözüldü', color: '#34C759'},
  Closed: {label: 'Kapatıldı', color: '#8E8E93'},
  Escalated: {label: 'Yönlendirildi', color: '#FF3B30'},
};

const TOPIC_ICONS = {
  OrderIssue: 'alert-circle-outline',
  Refund: 'cash-refund',
  Delivery: 'truck-delivery-outline',
  General: 'help-circle-outline',
  Account: 'account-alert-outline',
  Payment: 'credit-card-outline',
};

import { router } from 'expo-router';
const SupportScreen = () => {
  const {isAuthenticated} = useAuth();
  const [tickets, setTickets] = useState([]);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);

  useEffect(() => {
    if (isAuthenticated) {
      loadTickets(1);
    } else {
      setLoading(false);
    }
  }, [isAuthenticated]);

  const loadTickets = async pageNum => {
    try {
      const res = await supportService.getTickets(pageNum, 20);
      if (res.data?.data) {
        const data = res.data.data;
        if (pageNum === 1) {
          setTickets(data);
        } else {
          setTickets(prev => [...prev, ...data]);
        }
        setHasMore(data.length === 20);
        setPage(pageNum);
      }
    } catch (e) {
      console.log('Support tickets error:', e);
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  };

  const onRefresh = useCallback(() => {
    setRefreshing(true);
    loadTickets(1);
  }, []);

  const onEndReached = () => {
    if (hasMore && !loading) {
      loadTickets(page + 1);
    }
  };

  const formatDate = dateStr => {
    if (!dateStr) return '';
    const date = new Date(dateStr);
    const day = date.getDate().toString().padStart(2, '0');
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    const year = date.getFullYear();
    return `${day}.${month}.${year}`;
  };

  const renderStatusBadge = status => {
    const statusInfo = STATUS_MAP[status] || {label: status, color: '#8E8E93'};
    return (
      <View
        style={[styles.statusBadge, {backgroundColor: statusInfo.color + '18'}]}>
        <Text style={[styles.statusText, {color: statusInfo.color}]}>
          {statusInfo.label}
        </Text>
      </View>
    );
  };

  const renderTicketItem = ({item}) => {
    const topicIcon = TOPIC_ICONS[item.topic] || 'help-circle-outline';

    return (
      <TouchableOpacity
        style={styles.ticketCard}
        onPress={() =>
          router.push({ pathname: '/support-chat', params: { ticketId: item.id } })
        }
        activeOpacity={0.9}>
        <View style={styles.ticketHeader}>
          <View style={styles.topicInfo}>
            <View style={styles.topicIconBg}>
              <Icon name={topicIcon} size={18} color={Colors.primary} />
            </View>
            <View style={{flex: 1, marginLeft: 10}}>
              <Text style={styles.subject} numberOfLines={1}>
                {item.subject}
              </Text>
              <Text style={styles.date}>{formatDate(item.createdDate)}</Text>
            </View>
          </View>
          {renderStatusBadge(item.status)}
        </View>

        <View style={styles.ticketFooter}>
          {item.messageCount !== undefined && (
            <View style={styles.messageCountRow}>
              <Icon
                name="message-text-outline"
                size={14}
                color={Colors.textSecondary}
              />
              <Text style={styles.messageCount}>
                {item.messageCount} mesaj
              </Text>
            </View>
          )}
          <View style={styles.detailButton}>
            <Text style={styles.detailButtonText}>Görüntüle</Text>
            <Icon name="chevron-right" size={18} color={Colors.primary} />
          </View>
        </View>
      </TouchableOpacity>
    );
  };

  if (loading) {
    return <LoadingSpinner message="Destek talepleri yükleniyor..." />;
  }

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <View style={styles.headerLeft}>
          <TouchableOpacity
            onPress={() => router.back()}
            style={styles.backButton}
            activeOpacity={0.8}>
            <Icon name="arrow-left" size={24} color={Colors.text} />
          </TouchableOpacity>
          <Text style={styles.headerTitle}>Destek</Text>
        </View>
        <TouchableOpacity
          onPress={() => router.push('/new-ticket')}
          style={styles.newTicketBtn}
          activeOpacity={0.8}>
          <Icon name="plus" size={20} color="#FFF" />
          <Text style={styles.newTicketText}>Yeni Talep</Text>
        </TouchableOpacity>
      </View>
      <FlatList
        data={tickets}
        keyExtractor={item => item.id}
        renderItem={renderTicketItem}
        ListEmptyComponent={
          <EmptyState
            icon="headset"
            title="Destek Talebi Yok"
            message="Henüz destek talebiniz yok"
            actionLabel="Yeni Talep Oluştur"
            onAction={() => router.push('/new-ticket')}
          />
        }
        refreshControl={
          <RefreshControl
            refreshing={refreshing}
            onRefresh={onRefresh}
            tintColor={Colors.primary}
          />
        }
        onEndReached={onEndReached}
        onEndReachedThreshold={0.3}
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
    justifyContent: 'space-between',
    alignItems: 'center',
    paddingHorizontal: Spacing.base,
    paddingTop: 60,
    paddingBottom: Spacing.md,
    backgroundColor: Colors.surface,
    borderBottomWidth: 1,
    borderBottomColor: Colors.borderLight,
  },
  headerLeft: {
    flexDirection: 'row',
    alignItems: 'center',
  },
  backButton: {
    marginRight: Spacing.sm,
  },
  headerTitle: {
    fontSize: Fonts.sizes.xxl,
    fontWeight: Fonts.weights.heavy,
    color: Colors.text,
  },
  newTicketBtn: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: Colors.primary,
    paddingHorizontal: 14,
    paddingVertical: 8,
    borderRadius: BorderRadius.lg,
    gap: 4,
  },
  newTicketText: {
    fontSize: Fonts.sizes.sm,
    fontWeight: Fonts.weights.bold,
    color: '#FFF',
  },
  listContent: {
    paddingBottom: Spacing.xxl,
    flexGrow: 1,
  },
  ticketCard: {
    backgroundColor: Colors.surface,
    marginHorizontal: Spacing.base,
    marginTop: Spacing.md,
    borderRadius: BorderRadius.lg,
    padding: Spacing.base,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 2},
    shadowOpacity: 0.05,
    shadowRadius: 8,
    elevation: 3,
  },
  ticketHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'flex-start',
  },
  topicInfo: {
    flexDirection: 'row',
    alignItems: 'center',
    flex: 1,
    marginRight: Spacing.sm,
  },
  topicIconBg: {
    width: 38,
    height: 38,
    borderRadius: 10,
    backgroundColor: Colors.primary + '12',
    justifyContent: 'center',
    alignItems: 'center',
  },
  subject: {
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
  },
  date: {
    fontSize: Fonts.sizes.xs,
    color: Colors.textSecondary,
    marginTop: 2,
  },
  statusBadge: {
    paddingHorizontal: 10,
    paddingVertical: 4,
    borderRadius: 8,
  },
  statusText: {
    fontSize: Fonts.sizes.xs,
    fontWeight: Fonts.weights.bold,
  },
  ticketFooter: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginTop: Spacing.md,
    paddingTop: Spacing.md,
    borderTopWidth: 1,
    borderTopColor: Colors.borderLight,
  },
  messageCountRow: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: 4,
  },
  messageCount: {
    fontSize: Fonts.sizes.sm,
    color: Colors.textSecondary,
  },
  detailButton: {
    flexDirection: 'row',
    alignItems: 'center',
  },
  detailButtonText: {
    fontSize: Fonts.sizes.sm,
    color: Colors.primary,
    fontWeight: Fonts.weights.bold,
  },
});

export default SupportScreen;
