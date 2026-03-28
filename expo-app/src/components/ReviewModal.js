import React, {useState} from 'react';
import {
  View,
  Text,
  StyleSheet,
  Modal,
  TouchableOpacity,
  TextInput,
  ActivityIndicator,
  KeyboardAvoidingView,
  Platform,
} from 'react-native';
import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import {Colors, Fonts, Spacing, BorderRadius} from '../theme';

const ReviewModal = ({visible, onClose, onSubmit, restaurantName}) => {
  const [rating, setRating] = useState(0);
  const [comment, setComment] = useState('');
  const [submitting, setSubmitting] = useState(false);

  const handleSubmit = async () => {
    if (rating === 0) return;
    setSubmitting(true);
    try {
      await onSubmit({rating, comment: comment.trim() || null});
      setRating(0);
      setComment('');
      onClose();
    } catch (e) {
      // error handled by parent
    } finally {
      setSubmitting(false);
    }
  };

  const handleClose = () => {
    setRating(0);
    setComment('');
    onClose();
  };

  return (
    <Modal visible={visible} transparent animationType="fade" onRequestClose={handleClose}>
      <KeyboardAvoidingView
        style={styles.overlay}
        behavior={Platform.OS === 'ios' ? 'padding' : undefined}
      >
        <View style={styles.content}>
          <View style={styles.header}>
            <Text style={styles.title}>Siparisi Degerlendir</Text>
            <TouchableOpacity onPress={handleClose} style={styles.closeButton}>
              <Icon name="close" size={22} color={Colors.textSecondary} />
            </TouchableOpacity>
          </View>

          {restaurantName ? (
            <Text style={styles.restaurantName}>{restaurantName}</Text>
          ) : null}

          <View style={styles.starsContainer}>
            {[1, 2, 3, 4, 5].map(star => (
              <TouchableOpacity
                key={star}
                onPress={() => setRating(star)}
                activeOpacity={0.7}
                style={styles.starButton}
              >
                <Icon
                  name={star <= rating ? 'star' : 'star-outline'}
                  size={40}
                  color={star <= rating ? Colors.star : Colors.border}
                />
              </TouchableOpacity>
            ))}
          </View>
          <Text style={styles.ratingLabel}>
            {rating === 0
              ? 'Puan verin'
              : rating <= 2
              ? 'Vasat'
              : rating <= 3
              ? 'Orta'
              : rating <= 4
              ? 'Iyi'
              : 'Mukemmel'}
          </Text>

          <TextInput
            style={styles.commentInput}
            placeholder="Yorumunuzu yazin (istege bagli)..."
            placeholderTextColor={Colors.textLight}
            value={comment}
            onChangeText={setComment}
            multiline
            maxLength={500}
            textAlignVertical="top"
          />

          <TouchableOpacity
            style={[styles.submitButton, rating === 0 && styles.submitButtonDisabled]}
            onPress={handleSubmit}
            disabled={rating === 0 || submitting}
            activeOpacity={0.8}
          >
            {submitting ? (
              <ActivityIndicator color="#FFF" />
            ) : (
              <Text style={styles.submitButtonText}>Gonder</Text>
            )}
          </TouchableOpacity>
        </View>
      </KeyboardAvoidingView>
    </Modal>
  );
};

const styles = StyleSheet.create({
  overlay: {
    flex: 1,
    backgroundColor: Colors.overlay,
    justifyContent: 'center',
    padding: Spacing.xl,
  },
  content: {
    backgroundColor: Colors.surface,
    borderRadius: BorderRadius.xl,
    padding: Spacing.xl,
  },
  header: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: Spacing.md,
  },
  title: {
    fontSize: Fonts.sizes.lg,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
  },
  closeButton: {
    width: 36,
    height: 36,
    borderRadius: 18,
    backgroundColor: Colors.borderLight,
    justifyContent: 'center',
    alignItems: 'center',
  },
  restaurantName: {
    fontSize: Fonts.sizes.md,
    color: Colors.textSecondary,
    marginBottom: Spacing.lg,
  },
  starsContainer: {
    flexDirection: 'row',
    justifyContent: 'center',
    marginBottom: Spacing.sm,
  },
  starButton: {
    paddingHorizontal: 6,
  },
  ratingLabel: {
    textAlign: 'center',
    fontSize: Fonts.sizes.md,
    color: Colors.textSecondary,
    fontWeight: Fonts.weights.medium,
    marginBottom: Spacing.lg,
  },
  commentInput: {
    borderWidth: 1,
    borderColor: Colors.border,
    borderRadius: BorderRadius.md,
    padding: Spacing.md,
    fontSize: Fonts.sizes.md,
    color: Colors.text,
    minHeight: 100,
    marginBottom: Spacing.lg,
  },
  submitButton: {
    backgroundColor: Colors.primary,
    borderRadius: BorderRadius.md,
    paddingVertical: 14,
    alignItems: 'center',
  },
  submitButtonDisabled: {
    opacity: 0.5,
  },
  submitButtonText: {
    color: '#FFF',
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.bold,
  },
});

export default ReviewModal;
