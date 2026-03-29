import React, {useState, useEffect} from 'react';
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  TouchableOpacity,
  TextInput,
  ActivityIndicator,
} from 'react-native';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import {Colors, Fonts, Spacing, BorderRadius} from '../../theme';
import api from '../../api';
import {useToast} from '../../context/ToastContext';

const TOPICS = [
  {id: 'OrderIssue', label: 'Sipariş Sorunu', icon: 'alert-circle-outline', color: '#FF3B30'},
  {id: 'Cancellation', label: 'İptal Talebi', icon: 'cancel', color: '#FF9500'},
  {id: 'Delivery', label: 'Teslimat Problemi', icon: 'truck-delivery-outline', color: '#5856D6'},
  {id: 'General', label: 'Genel Soru', icon: 'help-circle-outline', color: Colors.info},
  {id: 'Account', label: 'Hesap Sorunu', icon: 'account-alert-outline', color: Colors.secondary},
  {id: 'Payment', label: 'Ödeme Sorunu', icon: 'credit-card-outline', color: Colors.error},
];

const ORDER_RELATED_TOPICS = ['OrderIssue', 'Refund', 'Delivery'];

const NewTicketScreen = ({navigation}) => {
  const {showToast} = useToast();
  const [selectedTopic, setSelectedTopic] = useState(null);
  const [subject, setSubject] = useState('');
  const [message, setMessage] = useState('');
  const [selectedOrderId, setSelectedOrderId] = useState(null);
  const [recentOrders, setRecentOrders] = useState([]);
  const [loadingOrders, setLoadingOrders] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [showOrderSelector, setShowOrderSelector] = useState(false);

  useEffect(() => {
    if (selectedTopic && ORDER_RELATED_TOPICS.includes(selectedTopic)) {
      loadRecentOrders();
    } else {
      setSelectedOrderId(null);
      setShowOrderSelector(false);
    }
  }, [selectedTopic]);

  const loadRecentOrders = async () => {
    setLoadingOrders(true);
    try {
      const result = await api.customer.order.getHistory({page: 1, pageSize: 10});
      if (result?.data) {
        setRecentOrders(result.data);
      }
    } catch (e) {
      console.log('Recent orders error:', e);
    } finally {
      setLoadingOrders(false);
    }
  };

  const handleSubmit = async () => {
    if (!selectedTopic) {
      showToast('Lütfen bir konu seçin', 'error');
      return;
    }
    if (!subject.trim()) {
      showToast('Lütfen bir başlık girin', 'error');
      return;
    }
    if (!message.trim()) {
      showToast('Lütfen mesajınızı yazın', 'error');
      return;
    }

    setSubmitting(true);
    try {
      const payload = {
        topic: selectedTopic,
        subject: subject.trim(),
        message: message.trim(),
      };
      if (selectedOrderId) {
        payload.orderId = selectedOrderId;
      }

      const result = await api.customer.support.createTicket(payload);
      if (result && !result.hasFailed) {
        showToast('Destek talebi oluşturuldu', 'success');
        const ticketId = result.data?.id || result.data;
        navigation.replace('SupportChat', {ticketId});
      } else {
        showToast(
          result?.messages?.[0]?.description || 'Talep oluşturulamadı',
          'error',
        );
      }
    } catch (e) {
      showToast('Talep oluşturulamadı', 'error');
    } finally {
      setSubmitting(false);
    }
  };

  const formatDate = dateStr => {
    if (!dateStr) return '';
    const date = new Date(dateStr);
    const day = date.getDate().toString().padStart(2, '0');
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    return `${day}.${month}.${date.getFullYear()}`;
  };

  const isOrderRelated =
    selectedTopic && ORDER_RELATED_TOPICS.includes(selectedTopic);

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <TouchableOpacity
          onPress={() => navigation.goBack()}
          style={styles.backButton}
          activeOpacity={0.8}>
          <Icon name="arrow-left" size={24} color={Colors.text} />
        </TouchableOpacity>
        <Text style={styles.headerTitle}>Yeni Destek Talebi</Text>
      </View>

      <ScrollView
        showsVerticalScrollIndicator={false}
        contentContainerStyle={styles.scrollContent}
        keyboardShouldPersistTaps="handled">
        <Text style={styles.sectionTitle}>Konu Seçin</Text>
        <View style={styles.topicGrid}>
          {TOPICS.map(topic => (
            <TouchableOpacity
              key={topic.id}
              style={[
                styles.topicCard,
                selectedTopic === topic.id && styles.topicCardSelected,
                selectedTopic === topic.id && {
                  borderColor: topic.color,
                },
              ]}
              onPress={() => setSelectedTopic(topic.id)}
              activeOpacity={0.8}>
              <View
                style={[
                  styles.topicIconBg,
                  {backgroundColor: topic.color + '15'},
                  selectedTopic === topic.id && {
                    backgroundColor: topic.color + '25',
                  },
                ]}>
                <Icon name={topic.icon} size={24} color={topic.color} />
              </View>
              <Text
                style={[
                  styles.topicLabel,
                  selectedTopic === topic.id && styles.topicLabelSelected,
                ]}>
                {topic.label}
              </Text>
            </TouchableOpacity>
          ))}
        </View>

        {isOrderRelated && (
          <View style={styles.orderSection}>
            <Text style={styles.sectionTitle}>İlgili Sipariş (Opsiyonel)</Text>
            {loadingOrders ? (
              <ActivityIndicator
                size="small"
                color={Colors.primary}
                style={{marginVertical: Spacing.md}}
              />
            ) : (
              <>
                <TouchableOpacity
                  style={styles.orderSelector}
                  onPress={() => setShowOrderSelector(!showOrderSelector)}
                  activeOpacity={0.8}>
                  <Text
                    style={[
                      styles.orderSelectorText,
                      selectedOrderId && styles.orderSelectorTextSelected,
                    ]}>
                    {selectedOrderId
                      ? recentOrders.find(o => o.id === selectedOrderId)
                          ?.restaurantName || 'Seçilen Sipariş'
                      : 'Sipariş seçin'}
                  </Text>
                  <Icon
                    name={showOrderSelector ? 'chevron-up' : 'chevron-down'}
                    size={20}
                    color={Colors.textSecondary}
                  />
                </TouchableOpacity>
                {showOrderSelector && (
                  <View style={styles.orderList}>
                    {recentOrders.map(order => (
                      <TouchableOpacity
                        key={order.id}
                        style={[
                          styles.orderItem,
                          selectedOrderId === order.id &&
                            styles.orderItemSelected,
                        ]}
                        onPress={() => {
                          setSelectedOrderId(order.id);
                          setShowOrderSelector(false);
                        }}
                        activeOpacity={0.8}>
                        <View style={{flex: 1}}>
                          <Text style={styles.orderName} numberOfLines={1}>
                            {order.restaurantName}
                          </Text>
                          <Text style={styles.orderDate}>
                            {formatDate(order.createdDate)} -{' '}
                            ₺{order.totalPrice?.toFixed(2)}
                          </Text>
                        </View>
                        {selectedOrderId === order.id && (
                          <Icon
                            name="check-circle"
                            size={20}
                            color={Colors.primary}
                          />
                        )}
                      </TouchableOpacity>
                    ))}
                    {recentOrders.length === 0 && (
                      <Text style={styles.noOrdersText}>
                        Son sipariş bulunamadı
                      </Text>
                    )}
                  </View>
                )}
              </>
            )}
          </View>
        )}

        <Text style={styles.sectionTitle}>Başlık</Text>
        <TextInput
          style={styles.input}
          placeholder="Konu başlığı girin"
          placeholderTextColor={Colors.textTertiary}
          value={subject}
          onChangeText={setSubject}
          maxLength={100}
        />

        <Text style={styles.sectionTitle}>Mesajınız</Text>
        <TextInput
          style={[styles.input, styles.textArea]}
          placeholder="Sorununuzu detaylı bir şekilde anlatın..."
          placeholderTextColor={Colors.textTertiary}
          value={message}
          onChangeText={setMessage}
          multiline
          textAlignVertical="top"
          maxLength={2000}
        />

        <TouchableOpacity
          style={[styles.submitButton, submitting && styles.submitButtonDisabled]}
          onPress={handleSubmit}
          disabled={submitting}
          activeOpacity={0.8}>
          {submitting ? (
            <ActivityIndicator size="small" color="#FFF" />
          ) : (
            <>
              <Icon name="send" size={20} color="#FFF" />
              <Text style={styles.submitButtonText}>Gönder</Text>
            </>
          )}
        </TouchableOpacity>
      </ScrollView>
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
    paddingHorizontal: Spacing.base,
    paddingTop: 60,
    paddingBottom: Spacing.md,
    backgroundColor: Colors.surface,
    borderBottomWidth: 1,
    borderBottomColor: Colors.borderLight,
  },
  backButton: {
    marginRight: Spacing.sm,
  },
  headerTitle: {
    fontSize: Fonts.sizes.xxl,
    fontWeight: Fonts.weights.heavy,
    color: Colors.text,
  },
  scrollContent: {
    padding: Spacing.base,
    paddingBottom: Spacing.xxxl,
  },
  sectionTitle: {
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
    marginBottom: Spacing.sm,
    marginTop: Spacing.md,
  },
  topicGrid: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    gap: Spacing.sm,
  },
  topicCard: {
    width: '31%',
    backgroundColor: Colors.surface,
    borderRadius: BorderRadius.lg,
    padding: Spacing.md,
    alignItems: 'center',
    borderWidth: 2,
    borderColor: 'transparent',
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 1},
    shadowOpacity: 0.04,
    shadowRadius: 4,
    elevation: 2,
  },
  topicCardSelected: {
    borderWidth: 2,
  },
  topicIconBg: {
    width: 48,
    height: 48,
    borderRadius: 14,
    justifyContent: 'center',
    alignItems: 'center',
    marginBottom: 8,
  },
  topicLabel: {
    fontSize: Fonts.sizes.xs,
    fontWeight: Fonts.weights.semibold,
    color: Colors.textSecondary,
    textAlign: 'center',
  },
  topicLabelSelected: {
    color: Colors.text,
    fontWeight: Fonts.weights.bold,
  },
  orderSection: {
    marginTop: Spacing.sm,
  },
  orderSelector: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    backgroundColor: Colors.surface,
    borderRadius: BorderRadius.lg,
    padding: Spacing.base,
    borderWidth: 1,
    borderColor: Colors.border,
  },
  orderSelectorText: {
    fontSize: Fonts.sizes.base,
    color: Colors.textTertiary,
  },
  orderSelectorTextSelected: {
    color: Colors.text,
    fontWeight: Fonts.weights.semibold,
  },
  orderList: {
    backgroundColor: Colors.surface,
    borderRadius: BorderRadius.lg,
    marginTop: Spacing.sm,
    borderWidth: 1,
    borderColor: Colors.border,
    overflow: 'hidden',
  },
  orderItem: {
    flexDirection: 'row',
    alignItems: 'center',
    padding: Spacing.base,
    borderBottomWidth: 1,
    borderBottomColor: Colors.borderLight,
  },
  orderItemSelected: {
    backgroundColor: Colors.primary + '08',
  },
  orderName: {
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.semibold,
    color: Colors.text,
  },
  orderDate: {
    fontSize: Fonts.sizes.sm,
    color: Colors.textSecondary,
    marginTop: 2,
  },
  noOrdersText: {
    fontSize: Fonts.sizes.sm,
    color: Colors.textTertiary,
    textAlign: 'center',
    padding: Spacing.base,
  },
  input: {
    backgroundColor: Colors.surface,
    borderRadius: BorderRadius.lg,
    padding: Spacing.base,
    fontSize: Fonts.sizes.base,
    color: Colors.text,
    borderWidth: 1,
    borderColor: Colors.border,
  },
  textArea: {
    minHeight: 120,
    textAlignVertical: 'top',
  },
  submitButton: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    backgroundColor: Colors.primary,
    borderRadius: BorderRadius.lg,
    paddingVertical: 16,
    marginTop: Spacing.xl,
    gap: 8,
  },
  submitButtonDisabled: {
    opacity: 0.6,
  },
  submitButtonText: {
    fontSize: Fonts.sizes.lg,
    fontWeight: Fonts.weights.bold,
    color: '#FFF',
  },
});

export default NewTicketScreen;
