import React from 'react';
import {View, Text, StyleSheet, ScrollView, TouchableOpacity} from 'react-native';
import {Colors, Fonts, Spacing, BorderRadius} from '../theme';
import {CUISINE_ICONS} from '../utils/constants';

const CuisineFilter = ({cuisines, selectedCuisine, onSelect}) => {
  const getIcon = (name) => CUISINE_ICONS[name] || CUISINE_ICONS.default;

  return (
    <ScrollView
      horizontal
      showsHorizontalScrollIndicator={false}
      contentContainerStyle={styles.container}
      style={styles.scrollView}
    >
      <TouchableOpacity
        style={[styles.chip, !selectedCuisine && styles.chipActive]}
        onPress={() => onSelect(null)}
        activeOpacity={0.8}
      >
        <Text style={styles.chipIcon}>🍽️</Text>
        <Text style={[styles.chipText, !selectedCuisine && styles.chipTextActive]}>Tümü</Text>
      </TouchableOpacity>
      {cuisines.map((cuisine) => {
        const isActive = selectedCuisine === cuisine.id;
        return (
          <TouchableOpacity
            key={cuisine.id}
            style={[styles.chip, isActive && styles.chipActive]}
            onPress={() => onSelect(isActive ? null : cuisine.id)}
            activeOpacity={0.8}
          >
            <Text style={styles.chipIcon}>{getIcon(cuisine.name)}</Text>
            <Text style={[styles.chipText, isActive && styles.chipTextActive]}>
              {cuisine.name}
            </Text>
          </TouchableOpacity>
        );
      })}
    </ScrollView>
  );
};

const styles = StyleSheet.create({
  scrollView: {
    maxHeight: 44,
    marginBottom: Spacing.sm,
  },
  container: {
    paddingHorizontal: Spacing.base,
    gap: 8,
    alignItems: 'center',
  },
  chip: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: Colors.surface,
    paddingHorizontal: 12,
    height: 36,
    borderRadius: 18,
    borderWidth: 1,
    borderColor: Colors.border,
  },
  chipActive: {
    backgroundColor: Colors.primary,
    borderColor: Colors.primary,
  },
  chipIcon: {
    fontSize: 14,
    marginRight: 4,
  },
  chipText: {
    fontSize: Fonts.sizes.sm,
    fontWeight: Fonts.weights.semibold,
    color: Colors.text,
  },
  chipTextActive: {
    color: '#FFF',
  },
});

export default CuisineFilter;
