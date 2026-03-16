import React, {useState, useEffect} from 'react';
import {View, Text, StyleSheet, TouchableOpacity, ActivityIndicator} from 'react-native';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import {Colors, Fonts, Spacing, BorderRadius} from '../theme';
import {reviewService} from '../api';

const ReviewList = ({restaurantId}) => {
  const [reviews, setReviews] = useState([]);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(false);

  useEffect(() => {
    loadReviews(1);
  }, [restaurantId]);

  const loadReviews = async (pageNum) => {
    try {
      const res = await reviewService.getRestaurantReviews(restaurantId, pageNum, 5);
      if (res.data && !res.data.hasFailed) {
        const data = res.data.data || [];
        if (pageNum === 1) {
          setReviews(data);
        } else {
          setReviews(prev => [...prev, ...data]);
        }
        setHasMore(data.length === 5);
        setPage(pageNum);
      }
    } catch (e) {
      // silently fail - reviews are supplementary
    } finally {
      setLoading(false);
    }
  };

  const formatDate = (dateStr) => {
    if (!dateStr) return '';
    const date = new Date(dateStr);
    const day = date.getDate().toString().padStart(2, '0');
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    const year = date.getFullYear();
    return `${day}.${month}.${year}`;
  };

  const renderStars = (count) => {
    return (
      <View style={styles.starsRow}>
        {[1, 2, 3, 4, 5].map(i => (
          <Icon
            key={i}
            name={i <= count ? 'star' : 'star-outline'}
            size={14}
            color={i <= count ? Colors.star : Colors.border}
          />
        ))}
      </View>
    );
  };

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="small" color={Colors.primary} />
      </View>
    );
  }

  if (reviews.length === 0) {
    return (
      <View style={styles.emptyContainer}>
        <Icon name="comment-text-outline" size={32} color={Colors.textLight} />
        <Text style={styles.emptyText}>Henuz degerlendirme yok</Text>
      </View>
    );
  }

  return (
    <View style={styles.container}>
      <Text style={styles.sectionTitle}>Degerlendirmeler</Text>
      {reviews.map((review, index) => (
        <View key={review.id || index} style={styles.reviewCard}>
          <View style={styles.reviewHeader}>
            <View style={styles.reviewUser}>
              <View style={styles.avatarCircle}>
                <Icon name="account" size={16} color={Colors.primary} />
              </View>
              <Text style={styles.userName}>
                {review.userName || 'Kullanici'}
              </Text>
            </View>
            <Text style={styles.reviewDate}>{formatDate(review.createdDate)}</Text>
          </View>
          {renderStars(review.rating)}
          {review.comment ? (
            <Text style={styles.comment}>{review.comment}</Text>
          ) : null}
        </View>
      ))}
      {hasMore && (
        <TouchableOpacity
          style={styles.loadMoreButton}
          onPress={() => loadReviews(page + 1)}
        >
          <Text style={styles.loadMoreText}>Daha fazla goster</Text>
        </TouchableOpacity>
      )}
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    paddingHorizontal: Spacing.base,
    paddingTop: Spacing.lg,
  },
  loadingContainer: {
    paddingVertical: Spacing.xl,
    alignItems: 'center',
  },
  emptyContainer: {
    paddingVertical: Spacing.xxl,
    alignItems: 'center',
  },
  emptyText: {
    fontSize: Fonts.sizes.md,
    color: Colors.textSecondary,
    marginTop: Spacing.sm,
  },
  sectionTitle: {
    fontSize: Fonts.sizes.lg,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
    marginBottom: Spacing.md,
  },
  reviewCard: {
    backgroundColor: Colors.borderLight,
    borderRadius: BorderRadius.md,
    padding: Spacing.md,
    marginBottom: Spacing.sm,
  },
  reviewHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 6,
  },
  reviewUser: {
    flexDirection: 'row',
    alignItems: 'center',
  },
  avatarCircle: {
    width: 28,
    height: 28,
    borderRadius: 14,
    backgroundColor: Colors.primary + '15',
    justifyContent: 'center',
    alignItems: 'center',
    marginRight: 8,
  },
  userName: {
    fontSize: Fonts.sizes.sm,
    fontWeight: Fonts.weights.semibold,
    color: Colors.text,
  },
  reviewDate: {
    fontSize: Fonts.sizes.xs,
    color: Colors.textSecondary,
  },
  starsRow: {
    flexDirection: 'row',
    marginBottom: 6,
  },
  comment: {
    fontSize: Fonts.sizes.sm,
    color: Colors.textSecondary,
    lineHeight: 20,
  },
  loadMoreButton: {
    alignItems: 'center',
    paddingVertical: Spacing.md,
  },
  loadMoreText: {
    fontSize: Fonts.sizes.sm,
    color: Colors.primary,
    fontWeight: Fonts.weights.bold,
  },
});

export default ReviewList;
