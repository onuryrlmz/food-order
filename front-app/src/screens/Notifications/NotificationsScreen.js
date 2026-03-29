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
import {useAuth} from '../../context/AuthContext';
import {useToast} from '../../context/ToastContext';
import LoadingSpinner from '../../components/LoadingSpinner';
import EmptyState from '../../components/EmptyState';

const NOTIFICATION_ICONS = {
  1: 'package-variant',
  2: 'tag-outline',
  3: 'comment-text-outline',
  4: 'truck-delivery-outline',
};

const NOTIFICATION_COLORS = {
  1: '#5856D6',
  2: Colors.secondary,
  3: Colors.info,
  4: Colors.success,
};

const NotificationsScreen = ({navigation}) => {
  const {isAuthenticated} = useAuth();
  const {showToast} = useToast();
  const [notifications, setNotifications] = useState([]);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [unreadCount, setUnreadCount] = useState(0);

  useEffect(() => {
    if (isAuthenticated) {
      loadNotifications(1);
    } else {
      setLoading(false);
    }
  }, [isAuthenticated]);

  const loadNotifications = async pageNum => {
    try {
      const result = await api.customer.notification.getList({page: pageNum, pageSize: 20});
      if (result?.data) {
        const data = result.data;
        if (pageNum === 1) {
          setNotifications(data);
        } else {
          setNotifications(prev => [...prev, ...data]);
        }
        setHasMore(data.length === 20);
        setPage(pageNum);
        if (result.unreadCount !== undefined) {
          setUnreadCount(result.unreadCount);
        }
      }
    } catch (e) {
      console.log('Notifications error:', e);
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  };

  const onRefresh = useCallback(() => {
    setRefreshing(true);
    loadNotifications(1);
  }, []);

  const onEndReached = () => {
    if (hasMore && !loading) {
      loadNotifications(page + 1);
    }
  };

  const handleMarkAllRead = async () => {
    try {
      const result = await api.customer.notification.markAllAsRead();
      if (result && !result.hasFailed) {
        setNotifications(prev =>
          prev.map(n => ({...n, isRead: true})),
        );
        setUnreadCount(0);
        showToast('Tüm bildirimler okundu olarak işaretlendi', 'success');
      }
    } catch (e) {
      showToast('İşlem başarısız oldu', 'error');
    }
  };

  const handlePress = async item => {
    if (!item.isRead) {
      try {
        await api.customer.notification.markAsRead({notificationId: item.id});
        setNotifications(prev =>
          prev.map(n => (n.id === item.id ? {...n, isRead: true} : n)),
        );
        setUnreadCount(prev => Math.max(0, prev - 1));
      } catch (e) {
        console.log('Mark as read error:', e);
      }
    }
  };

  const getTimeAgo = dateStr => {
    if (!dateStr) return '';
    const now = new Date();
    const date = new Date(dateStr);
    const diffMs = now - date;
    const diffMin = Math.floor(diffMs / 60000);
    const diffHour = Math.floor(diffMs / 3600000);
    const diffDay = Math.floor(diffMs / 86400000);

    if (diffMin < 1) return 'Az önce';
    if (diffMin < 60) return `${diffMin} dk önce`;
    if (diffHour < 24) return `${diffHour} saat önce`;
    if (diffDay < 7) return `${diffDay} gün önce`;
    const day = date.getDate().toString().padStart(2, '0');
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    return `${day}.${month}.${date.getFullYear()}`;
  };

  const renderNotificationItem = ({item}) => {
    const iconName = NOTIFICATION_ICONS[item.typeId] || 'bell-outline';
    const iconColor = NOTIFICATION_COLORS[item.typeId] || Colors.primary;

    return (
      <TouchableOpacity
        style={[styles.notificationCard, !item.isRead && styles.unreadCard]}
        onPress={() => handlePress(item)}
        activeOpacity={0.9}>
        <View style={[styles.iconBg, {backgroundColor: iconColor + '15'}]}>
          <Icon name={iconName} size={22} color={iconColor} />
        </View>
        <View style={styles.contentContainer}>
          <View style={styles.titleRow}>
            <Text
              style={[styles.title, !item.isRead && styles.unreadTitle]}
              numberOfLines={1}>
              {item.title}
            </Text>
            {!item.isRead && <View style={styles.unreadDot} />}
          </View>
          <Text style={styles.message} numberOfLines={2}>
            {item.message}
          </Text>
          <Text style={styles.timeAgo}>{getTimeAgo(item.createdDate)}</Text>
        </View>
      </TouchableOpacity>
    );
  };

  if (loading) {
    return <LoadingSpinner message="Bildirimler yükleniyor..." />;
  }

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <View style={styles.headerLeft}>
          <TouchableOpacity
            onPress={() => navigation.goBack()}
            style={styles.backButton}
            activeOpacity={0.8}>
            <Icon name="arrow-left" size={24} color={Colors.text} />
          </TouchableOpacity>
          <Text style={styles.headerTitle}>Bildirimler</Text>
          {unreadCount > 0 && (
            <View style={styles.badge}>
              <Text style={styles.badgeText}>{unreadCount}</Text>
            </View>
          )}
        </View>
        {unreadCount > 0 && (
          <TouchableOpacity onPress={handleMarkAllRead} activeOpacity={0.8}>
            <Text style={styles.markAllText}>Tümünü Okundu İşaretle</Text>
          </TouchableOpacity>
        )}
      </View>
      <FlatList
        data={notifications}
        keyExtractor={item => item.id}
        renderItem={renderNotificationItem}
        ListEmptyComponent={
          <EmptyState
            icon="bell-off-outline"
            title="Bildirim Yok"
            message="Henüz bildiriminiz bulunmuyor"
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
  badge: {
    backgroundColor: Colors.primary,
    borderRadius: 10,
    minWidth: 20,
    height: 20,
    justifyContent: 'center',
    alignItems: 'center',
    paddingHorizontal: 6,
    marginLeft: 8,
  },
  badgeText: {
    fontSize: Fonts.sizes.xs,
    fontWeight: Fonts.weights.bold,
    color: '#FFF',
  },
  markAllText: {
    fontSize: Fonts.sizes.sm,
    fontWeight: Fonts.weights.semibold,
    color: Colors.primary,
  },
  listContent: {
    paddingBottom: Spacing.xxl,
    flexGrow: 1,
  },
  notificationCard: {
    flexDirection: 'row',
    backgroundColor: Colors.surface,
    marginHorizontal: Spacing.base,
    marginTop: Spacing.sm,
    borderRadius: BorderRadius.lg,
    padding: Spacing.base,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 1},
    shadowOpacity: 0.04,
    shadowRadius: 4,
    elevation: 2,
  },
  unreadCard: {
    backgroundColor: Colors.primary + '08',
    borderLeftWidth: 3,
    borderLeftColor: Colors.primary,
  },
  iconBg: {
    width: 42,
    height: 42,
    borderRadius: 12,
    justifyContent: 'center',
    alignItems: 'center',
    marginRight: Spacing.md,
  },
  contentContainer: {
    flex: 1,
  },
  titleRow: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: 4,
  },
  title: {
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.semibold,
    color: Colors.text,
    flex: 1,
  },
  unreadTitle: {
    fontWeight: Fonts.weights.bold,
  },
  unreadDot: {
    width: 8,
    height: 8,
    borderRadius: 4,
    backgroundColor: Colors.primary,
    marginLeft: 8,
  },
  message: {
    fontSize: Fonts.sizes.sm,
    color: Colors.textSecondary,
    lineHeight: 20,
    marginBottom: 4,
  },
  timeAgo: {
    fontSize: Fonts.sizes.xs,
    color: Colors.textTertiary,
  },
});

export default NotificationsScreen;
