import React from 'react';
import {View, Text, StyleSheet, TouchableOpacity, Image} from 'react-native';
import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import {Colors, Fonts, Spacing, BorderRadius} from '../theme';

const MenuItemCard = ({item, onPress}) => {
  return (
    <TouchableOpacity style={styles.container} onPress={onPress} activeOpacity={0.9}>
      <View style={styles.content}>
        <Text style={styles.name} numberOfLines={2}>{item.name}</Text>
        {item.description ? (
          <Text style={styles.description} numberOfLines={2}>{item.description}</Text>
        ) : null}
        <View style={styles.priceRow}>
          <Text style={styles.price}>₺{item.price?.toFixed(2)}</Text>
        </View>
      </View>
      <View style={styles.imageSection}>
        {item.imageUrl ? (
          <Image source={{uri: item.imageUrl}} style={styles.image} resizeMode="cover" />
        ) : (
          <View style={styles.placeholderImage}>
            <Icon name="food" size={24} color={Colors.textLight} />
          </View>
        )}
        <TouchableOpacity style={styles.addButton} onPress={onPress} activeOpacity={0.8}>
          <Icon name="plus" size={18} color="#FFF" />
        </TouchableOpacity>
      </View>
    </TouchableOpacity>
  );
};

const styles = StyleSheet.create({
  container: {
    backgroundColor: Colors.surface,
    flexDirection: 'row',
    padding: Spacing.md,
    marginHorizontal: Spacing.base,
    marginBottom: Spacing.sm,
    borderRadius: BorderRadius.lg,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 1},
    shadowOpacity: 0.04,
    shadowRadius: 4,
    elevation: 2,
  },
  content: {
    flex: 1,
    marginRight: Spacing.md,
    justifyContent: 'center',
  },
  name: {
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
    marginBottom: 4,
  },
  description: {
    fontSize: Fonts.sizes.sm,
    color: Colors.textSecondary,
    lineHeight: 18,
    marginBottom: 8,
  },
  priceRow: {
    flexDirection: 'row',
    alignItems: 'center',
  },
  price: {
    fontSize: Fonts.sizes.lg,
    fontWeight: Fonts.weights.heavy,
    color: Colors.primary,
  },
  imageSection: {
    alignItems: 'center',
  },
  image: {
    width: 90,
    height: 90,
    borderRadius: BorderRadius.md,
  },
  placeholderImage: {
    width: 90,
    height: 90,
    borderRadius: BorderRadius.md,
    backgroundColor: Colors.borderLight,
    justifyContent: 'center',
    alignItems: 'center',
  },
  addButton: {
    backgroundColor: Colors.primary,
    width: 32,
    height: 32,
    borderRadius: 16,
    justifyContent: 'center',
    alignItems: 'center',
    marginTop: -16,
    shadowColor: Colors.primary,
    shadowOffset: {width: 0, height: 2},
    shadowOpacity: 0.3,
    shadowRadius: 4,
    elevation: 4,
  },
});

export default MenuItemCard;
