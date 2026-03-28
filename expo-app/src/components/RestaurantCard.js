import React from 'react';
import {View, Text, StyleSheet, TouchableOpacity, Image} from 'react-native';
import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import {Colors, Fonts, Spacing, BorderRadius} from '../theme';

const RestaurantCard = ({restaurant, onPress}) => {
  const deliveryTimeText = restaurant.minDeliveryTime === restaurant.maxDeliveryTime
    ? `${restaurant.minDeliveryTime} dk`
    : `${restaurant.minDeliveryTime}-${restaurant.maxDeliveryTime} dk`;

  return (
    <TouchableOpacity style={styles.container} onPress={onPress} activeOpacity={0.9}>
      <View style={styles.imageContainer}>
        {restaurant.imageUrl ? (
          <Image source={{uri: restaurant.imageUrl}} style={styles.image} resizeMode="cover" />
        ) : (
          <View style={styles.placeholderImage}>
            <Icon name="food" size={40} color={Colors.textLight} />
          </View>
        )}
        {restaurant.categories?.length > 0 && (
          <View style={styles.categoryBadge}>
            <Text style={styles.categoryText} numberOfLines={1}>
              {restaurant.categories[0]}
            </Text>
          </View>
        )}
      </View>
      <View style={styles.content}>
        <View style={styles.header}>
          <Text style={styles.name} numberOfLines={1}>{restaurant.name}</Text>
          <View style={styles.ratingBadge}>
            <Icon name="star" size={14} color={Colors.star} />
            <Text style={styles.ratingText}>4.5</Text>
          </View>
        </View>
        {restaurant.description ? (
          <Text style={styles.description} numberOfLines={1}>{restaurant.description}</Text>
        ) : null}
        <View style={styles.infoRow}>
          <View style={styles.infoItem}>
            <Icon name="clock-outline" size={14} color={Colors.primary} />
            <Text style={styles.infoText}>{deliveryTimeText}</Text>
          </View>
          <View style={styles.infoDot} />
          <View style={styles.infoItem}>
            <Icon name="currency-try" size={14} color={Colors.primary} />
            <Text style={styles.infoText}>Min ₺{restaurant.minBasketPrice}</Text>
          </View>
          {restaurant.deliveryPrice > 0 && (
            <>
              <View style={styles.infoDot} />
              <View style={styles.infoItem}>
                <Icon name="motorbike" size={14} color={Colors.primary} />
                <Text style={styles.infoText}>₺{restaurant.deliveryPrice}</Text>
              </View>
            </>
          )}
          {restaurant.deliveryPrice === 0 && (
            <>
              <View style={styles.infoDot} />
              <View style={styles.freeDeliveryBadge}>
                <Icon name="motorbike" size={12} color={Colors.success} />
                <Text style={styles.freeDeliveryText}>Ücretsiz</Text>
              </View>
            </>
          )}
        </View>
      </View>
    </TouchableOpacity>
  );
};

const styles = StyleSheet.create({
  container: {
    backgroundColor: Colors.surface,
    borderRadius: BorderRadius.lg,
    marginHorizontal: Spacing.base,
    marginBottom: Spacing.md,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 2},
    shadowOpacity: 0.06,
    shadowRadius: 8,
    elevation: 3,
    overflow: 'hidden',
  },
  imageContainer: {
    height: 160,
    backgroundColor: Colors.borderLight,
  },
  image: {
    width: '100%',
    height: '100%',
  },
  placeholderImage: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: Colors.borderLight,
  },
  categoryBadge: {
    position: 'absolute',
    bottom: 10,
    left: 10,
    backgroundColor: 'rgba(0,0,0,0.65)',
    paddingHorizontal: 10,
    paddingVertical: 4,
    borderRadius: 20,
  },
  categoryText: {
    color: '#FFF',
    fontSize: Fonts.sizes.xs,
    fontWeight: Fonts.weights.semibold,
  },
  content: {
    padding: Spacing.md,
  },
  header: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 4,
  },
  name: {
    fontSize: Fonts.sizes.lg,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
    flex: 1,
    marginRight: 8,
  },
  ratingBadge: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#FFF9E6',
    paddingHorizontal: 8,
    paddingVertical: 3,
    borderRadius: 8,
  },
  ratingText: {
    fontSize: Fonts.sizes.sm,
    fontWeight: Fonts.weights.bold,
    color: '#B8860B',
    marginLeft: 3,
  },
  description: {
    fontSize: Fonts.sizes.sm,
    color: Colors.textSecondary,
    marginBottom: 8,
  },
  infoRow: {
    flexDirection: 'row',
    alignItems: 'center',
  },
  infoItem: {
    flexDirection: 'row',
    alignItems: 'center',
  },
  infoText: {
    fontSize: Fonts.sizes.sm,
    color: Colors.textSecondary,
    marginLeft: 4,
    fontWeight: Fonts.weights.medium,
  },
  infoDot: {
    width: 3,
    height: 3,
    borderRadius: 1.5,
    backgroundColor: Colors.textLight,
    marginHorizontal: 8,
  },
  freeDeliveryBadge: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#E8F8ED',
    paddingHorizontal: 6,
    paddingVertical: 2,
    borderRadius: 6,
  },
  freeDeliveryText: {
    fontSize: Fonts.sizes.xs,
    color: Colors.success,
    fontWeight: Fonts.weights.semibold,
    marginLeft: 3,
  },
});

export default RestaurantCard;
