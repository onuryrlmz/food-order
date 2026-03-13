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
  container: {
    paddingHorizontal: Spacing.base,
    paddingBottom: Spacing.md,
    gap: 8,
  },
  chip: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: Colors.surface,
    paddingHorizontal: 14,
    paddingVertical: 10,
    borderRadius: BorderRadius.round,
    borderWidth: 1,
    borderColor: Colors.border,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 1},
    shadowOpacity: 0.04,
    shadowRadius: 2,
    elevation: 1,
  },
  chipActive: {
    backgroundColor: Colors.primary,
    borderColor: Colors.primary,
  },
  chipIcon: {
    fontSize: 18,
    marginRight: 6,
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
