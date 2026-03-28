import React, {useState, useEffect, useRef} from 'react';
import {
  View,
  Text,
  StyleSheet,
  FlatList,
  TouchableOpacity,
  TextInput,
  KeyboardAvoidingView,
  Platform,
  ActivityIndicator,
} from 'react-native';
import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import {useSafeAreaInsets} from 'react-native-safe-area-context';
import {Colors, Fonts, Spacing, BorderRadius} from '../../theme';
import {supportService} from '../../api';
import {useToast} from '../../context/ToastContext';
import LoadingSpinner from '../../components/LoadingSpinner';

const STATUS_MAP = {
  Open: {label: 'Açık', color: '#5856D6'},
  InProgress: {label: 'İşlemde', color: '#FF9500'},
  Resolved: {label: 'Çözüldü', color: '#34C759'},
  Closed: {label: 'Kapatıldı', color: '#8E8E93'},
  Escalated: {label: 'Yönlendirildi', color: '#FF3B30'},
};

import { router, useLocalSearchParams } from 'expo-router';

const SupportChatScreen = () => {
  const {ticketId} = useLocalSearchParams();
  const insets = useSafeAreaInsets();
  const {showToast} = useToast();
  const flatListRef = useRef(null);

  const [ticket, setTicket] = useState(null);
  const [messages, setMessages] = useState([]);
  const [loading, setLoading] = useState(true);
  const [sending, setSending] = useState(false);
  const [inputText, setInputText] = useState('');
  const [rating, setRating] = useState(0);
  const [hasRated, setHasRated] = useState(false);

  useEffect(() => {
    loadTicketDetail();
  }, []);

  const loadTicketDetail = async () => {
    try {
      const res = await supportService.getTicketDetail(ticketId);
      if (res.data?.data) {
        const data = res.data.data;
        setTicket(data);
        setMessages(data.messages || []);
        if (data.rating) {
          setRating(data.rating);
          setHasRated(true);
        }
      }
    } catch (e) {
      console.log('Ticket detail error:', e);
      showToast('Destek talebi yüklenemedi', 'error');
    } finally {
      setLoading(false);
    }
  };

  const handleSend = async () => {
    const text = inputText.trim();
    if (!text || sending) return;

    const tempMessage = {
      id: 'temp_' + Date.now(),
      message: text,
      isCustomer: true,
      createdDate: new Date().toISOString(),
    };

    setMessages(prev => [...prev, tempMessage]);
    setInputText('');
    setSending(true);

    try {
      const res = await supportService.sendMessage(ticketId, text);
      if (res.data && !res.data.hasFailed) {
        const responseData = res.data.data;
        setMessages(prev => {
          const updated = prev.map(m =>
            m.id === tempMessage.id
              ? {...m, id: responseData.customerMessageId || m.id}
              : m,
          );
          if (responseData.aiMessage) {
            updated.push({
              id: responseData.aiMessage.id || 'ai_' + Date.now(),
              message: responseData.aiMessage.message,
              isCustomer: false,
              createdDate:
                responseData.aiMessage.createdDate || new Date().toISOString(),
            });
          }
          return updated;
        });
      } else {
        setMessages(prev => prev.filter(m => m.id !== tempMessage.id));
        showToast('Mesaj gönderilemedi', 'error');
      }
    } catch (e) {
      setMessages(prev => prev.filter(m => m.id !== tempMessage.id));
      showToast('Mesaj gönderilemedi', 'error');
    } finally {
      setSending(false);
    }
  };

  const handleRate = async starValue => {
    setRating(starValue);
    try {
      const res = await supportService.rateTicket(ticketId, starValue);
      if (res.data && !res.data.hasFailed) {
        setHasRated(true);
        showToast('Değerlendirmeniz kaydedildi', 'success');
      } else {
        setRating(0);
        showToast('Değerlendirme kaydedilemedi', 'error');
      }
    } catch (e) {
      setRating(0);
      showToast('Değerlendirme kaydedilemedi', 'error');
    }
  };

  const formatTime = dateStr => {
    if (!dateStr) return '';
    const date = new Date(dateStr);
    const hours = date.getHours().toString().padStart(2, '0');
    const minutes = date.getMinutes().toString().padStart(2, '0');
    return `${hours}:${minutes}`;
  };

  const renderMessage = ({item}) => {
    const isCustomer = item.isCustomer;

    return (
      <View
        style={[
          styles.messageRow,
          isCustomer ? styles.messageRowRight : styles.messageRowLeft,
        ]}>
        {!isCustomer && (
          <Text style={styles.aiLabel}>AI Asistan</Text>
        )}
        <View
          style={[
            styles.messageBubble,
            isCustomer ? styles.customerBubble : styles.aiBubble,
          ]}>
          <Text
            style={[
              styles.messageText,
              isCustomer ? styles.customerText : styles.aiText,
            ]}>
            {item.message}
          </Text>
          <Text
            style={[
              styles.messageTime,
              isCustomer ? styles.customerTime : styles.aiTime,
            ]}>
            {formatTime(item.createdDate)}
          </Text>
        </View>
      </View>
    );
  };

  const isTicketClosed =
    ticket?.status === 'Resolved' || ticket?.status === 'Closed';

  const renderRatingSection = () => {
    if (!isTicketClosed) return null;

    return (
      <View style={styles.ratingContainer}>
        <Text style={styles.ratingTitle}>
          {hasRated
            ? 'Değerlendirmeniz'
            : 'Bu destek talebini değerlendirin'}
        </Text>
        <View style={styles.starsRow}>
          {[1, 2, 3, 4, 5].map(star => (
            <TouchableOpacity
              key={star}
              onPress={() => !hasRated && handleRate(star)}
              activeOpacity={hasRated ? 1 : 0.7}>
              <Icon
                name={star <= rating ? 'star' : 'star-outline'}
                size={32}
                color={star <= rating ? Colors.star : Colors.textLight}
              />
            </TouchableOpacity>
          ))}
        </View>
      </View>
    );
  };

  if (loading) {
    return <LoadingSpinner message="Yükleniyor..." />;
  }

  const statusInfo = STATUS_MAP[ticket?.status] || {
    label: ticket?.status,
    color: '#8E8E93',
  };

  return (
    <KeyboardAvoidingView
      style={styles.container}
      behavior={Platform.OS === 'ios' ? 'padding' : undefined}
      keyboardVerticalOffset={0}>
      <View style={styles.header}>
        <TouchableOpacity
          onPress={() => router.back()}
          style={styles.backButton}
          activeOpacity={0.8}>
          <Icon name="arrow-left" size={24} color={Colors.text} />
        </TouchableOpacity>
        <View style={styles.headerCenter}>
          <Text style={styles.headerTitle} numberOfLines={1}>
            {ticket?.subject || 'Destek'}
          </Text>
          <View
            style={[
              styles.statusBadge,
              {backgroundColor: statusInfo.color + '18'},
            ]}>
            <Text style={[styles.statusText, {color: statusInfo.color}]}>
              {statusInfo.label}
            </Text>
          </View>
        </View>
        <TouchableOpacity
          onPress={() => router.back()}
          style={styles.closeButton}
          activeOpacity={0.8}>
          <Icon name="close" size={24} color={Colors.textSecondary} />
        </TouchableOpacity>
      </View>

      <FlatList
        ref={flatListRef}
        data={messages}
        keyExtractor={item => String(item.id)}
        renderItem={renderMessage}
        contentContainerStyle={styles.chatContent}
        showsVerticalScrollIndicator={false}
        onContentSizeChange={() =>
          flatListRef.current?.scrollToEnd({animated: true})
        }
        ListFooterComponent={
          <>
            {sending && (
              <View style={styles.typingIndicator}>
                <ActivityIndicator size="small" color={Colors.primary} />
                <Text style={styles.typingText}>AI yanıt yazıyor...</Text>
              </View>
            )}
            {renderRatingSection()}
          </>
        }
      />

      {!isTicketClosed && (
        <View
          style={[styles.inputContainer, {paddingBottom: insets.bottom || 16}]}>
          <TextInput
            style={styles.textInput}
            placeholder="Mesajınızı yazın..."
            placeholderTextColor={Colors.textTertiary}
            value={inputText}
            onChangeText={setInputText}
            multiline
            maxLength={1000}
            editable={!sending}
          />
          <TouchableOpacity
            style={[
              styles.sendButton,
              (!inputText.trim() || sending) && styles.sendButtonDisabled,
            ]}
            onPress={handleSend}
            disabled={!inputText.trim() || sending}
            activeOpacity={0.8}>
            <Icon
              name="send"
              size={20}
              color={
                inputText.trim() && !sending ? '#FFF' : Colors.textLight
              }
            />
          </TouchableOpacity>
        </View>
      )}
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
  headerCenter: {
    flex: 1,
    alignItems: 'center',
  },
  headerTitle: {
    fontSize: Fonts.sizes.lg,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
    marginBottom: 4,
  },
  statusBadge: {
    paddingHorizontal: 8,
    paddingVertical: 2,
    borderRadius: 6,
  },
  statusText: {
    fontSize: Fonts.sizes.xs,
    fontWeight: Fonts.weights.bold,
  },
  closeButton: {
    marginLeft: Spacing.sm,
  },
  chatContent: {
    paddingHorizontal: Spacing.base,
    paddingVertical: Spacing.md,
    flexGrow: 1,
  },
  messageRow: {
    marginBottom: Spacing.md,
    maxWidth: '80%',
  },
  messageRowRight: {
    alignSelf: 'flex-end',
  },
  messageRowLeft: {
    alignSelf: 'flex-start',
  },
  aiLabel: {
    fontSize: Fonts.sizes.xs,
    fontWeight: Fonts.weights.semibold,
    color: Colors.textSecondary,
    marginBottom: 4,
    marginLeft: 4,
  },
  messageBubble: {
    padding: Spacing.md,
    borderRadius: BorderRadius.lg,
  },
  customerBubble: {
    backgroundColor: Colors.primary,
    borderBottomRightRadius: 4,
  },
  aiBubble: {
    backgroundColor: Colors.surface,
    borderWidth: 1,
    borderColor: Colors.borderLight,
    borderBottomLeftRadius: 4,
  },
  messageText: {
    fontSize: Fonts.sizes.base,
    lineHeight: 22,
  },
  customerText: {
    color: '#FFF',
  },
  aiText: {
    color: Colors.text,
  },
  messageTime: {
    fontSize: Fonts.sizes.xs,
    marginTop: 4,
    alignSelf: 'flex-end',
  },
  customerTime: {
    color: 'rgba(255,255,255,0.7)',
  },
  aiTime: {
    color: Colors.textTertiary,
  },
  typingIndicator: {
    flexDirection: 'row',
    alignItems: 'center',
    alignSelf: 'flex-start',
    paddingHorizontal: Spacing.md,
    paddingVertical: Spacing.sm,
    backgroundColor: Colors.surface,
    borderRadius: BorderRadius.lg,
    borderWidth: 1,
    borderColor: Colors.borderLight,
    gap: 8,
    marginBottom: Spacing.md,
  },
  typingText: {
    fontSize: Fonts.sizes.sm,
    color: Colors.textSecondary,
    fontStyle: 'italic',
  },
  ratingContainer: {
    alignItems: 'center',
    paddingVertical: Spacing.lg,
    marginTop: Spacing.md,
    backgroundColor: Colors.surface,
    borderRadius: BorderRadius.lg,
    borderWidth: 1,
    borderColor: Colors.borderLight,
  },
  ratingTitle: {
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.semibold,
    color: Colors.text,
    marginBottom: Spacing.md,
  },
  starsRow: {
    flexDirection: 'row',
    gap: 8,
  },
  inputContainer: {
    flexDirection: 'row',
    alignItems: 'flex-end',
    paddingHorizontal: Spacing.base,
    paddingTop: Spacing.sm,
    backgroundColor: Colors.surface,
    borderTopWidth: 1,
    borderTopColor: Colors.borderLight,
  },
  textInput: {
    flex: 1,
    minHeight: 40,
    maxHeight: 100,
    backgroundColor: Colors.background,
    borderRadius: BorderRadius.lg,
    paddingHorizontal: Spacing.md,
    paddingTop: 10,
    paddingBottom: 10,
    fontSize: Fonts.sizes.base,
    color: Colors.text,
    marginRight: Spacing.sm,
  },
  sendButton: {
    width: 40,
    height: 40,
    borderRadius: 20,
    backgroundColor: Colors.primary,
    justifyContent: 'center',
    alignItems: 'center',
    marginBottom: 0,
  },
  sendButtonDisabled: {
    backgroundColor: Colors.borderLight,
  },
});

export default SupportChatScreen;
