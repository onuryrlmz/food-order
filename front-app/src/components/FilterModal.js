import React, {useState, useEffect} from 'react';
import {
  View,
  Text,
  StyleSheet,
  Modal,
  TouchableOpacity,
  ScrollView,
} from 'react-native';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import {Colors, Fonts, Spacing, BorderRadius} from '../theme';

const SORT_OPTIONS = [
  {id: 'default', label: 'Varsayilan'},
  {id: 'rating', label: 'Puana Gore'},
  {id: 'delivery_time', label: 'Teslimat Suresi'},
  {id: 'min_order', label: 'Min. Siparis'},
];

const RATING_OPTIONS = [
  {id: null, label: 'Hepsi'},
  {id: 4, label: '4+'},
  {id: 3, label: '3+'},
];

const FilterModal = ({visible, onClose, onApply, cuisines, initialFilters}) => {
  const [selectedCuisine, setSelectedCuisine] = useState(null);
  const [minRating, setMinRating] = useState(null);
  const [sortBy, setSortBy] = useState('default');

  useEffect(() => {
    if (initialFilters) {
      setSelectedCuisine(initialFilters.cuisineId || null);
      setMinRating(initialFilters.minRating || null);
      setSortBy(initialFilters.sortBy || 'default');
    }
  }, [initialFilters, visible]);

  const handleApply = () => {
    onApply({
      cuisineId: selectedCuisine,
      minRating,
      sortBy: sortBy === 'default' ? null : sortBy,
    });
    onClose();
  };

  const handleReset = () => {
    setSelectedCuisine(null);
    setMinRating(null);
    setSortBy('default');
  };

  const activeCount =
    (selectedCuisine ? 1 : 0) + (minRating ? 1 : 0) + (sortBy !== 'default' ? 1 : 0);

  return (
    <Modal visible={visible} transparent animationType="slide" onRequestClose={onClose}>
      <View style={styles.overlay}>
        <View style={styles.content}>
          <View style={styles.header}>
            <Text style={styles.title}>Filtrele</Text>
            <TouchableOpacity onPress={onClose} style={styles.closeButton}>
              <Icon name="close" size={22} color={Colors.textSecondary} />
            </TouchableOpacity>
          </View>

          <ScrollView showsVerticalScrollIndicator={false} style={styles.body}>
            {/* Cuisine Filter */}
            {cuisines && cuisines.length > 0 && (
              <View style={styles.section}>
                <Text style={styles.sectionTitle}>Mutfak</Text>
                <View style={styles.chipContainer}>
                  <TouchableOpacity
                    style={[styles.chip, !selectedCuisine && styles.chipActive]}
                    onPress={() => setSelectedCuisine(null)}
                  >
                    <Text style={[styles.chipText, !selectedCuisine && styles.chipTextActive]}>
                      Hepsi
                    </Text>
                  </TouchableOpacity>
                  {cuisines.map(cuisine => (
                    <TouchableOpacity
                      key={cuisine.id}
                      style={[styles.chip, selectedCuisine === cuisine.id && styles.chipActive]}
                      onPress={() =>
                        setSelectedCuisine(selectedCuisine === cuisine.id ? null : cuisine.id)
                      }
                    >
                      <Text
                        style={[
                          styles.chipText,
                          selectedCuisine === cuisine.id && styles.chipTextActive,
                        ]}
                      >
                        {cuisine.name}
                      </Text>
                    </TouchableOpacity>
                  ))}
                </View>
              </View>
            )}

            {/* Rating Filter */}
            <View style={styles.section}>
              <Text style={styles.sectionTitle}>Minimum Puan</Text>
              <View style={styles.chipContainer}>
                {RATING_OPTIONS.map(opt => (
                  <TouchableOpacity
                    key={opt.id || 'all'}
                    style={[styles.chip, minRating === opt.id && styles.chipActive]}
                    onPress={() => setMinRating(opt.id)}
                  >
                    {opt.id && <Icon name="star" size={14} color={minRating === opt.id ? '#FFF' : Colors.star} />}
                    <Text style={[styles.chipText, minRating === opt.id && styles.chipTextActive]}>
                      {opt.label}
                    </Text>
                  </TouchableOpacity>
                ))}
              </View>
            </View>

            {/* Sort */}
            <View style={styles.section}>
              <Text style={styles.sectionTitle}>Siralama</Text>
              {SORT_OPTIONS.map(opt => (
                <TouchableOpacity
                  key={opt.id}
                  style={styles.sortOption}
                  onPress={() => setSortBy(opt.id)}
                >
                  <Icon
                    name={sortBy === opt.id ? 'radiobox-marked' : 'radiobox-blank'}
                    size={22}
                    color={sortBy === opt.id ? Colors.primary : Colors.textLight}
                  />
                  <Text
                    style={[styles.sortOptionText, sortBy === opt.id && styles.sortOptionTextActive]}
                  >
                    {opt.label}
                  </Text>
                </TouchableOpacity>
              ))}
            </View>
          </ScrollView>

          <View style={styles.footer}>
            {activeCount > 0 && (
              <TouchableOpacity style={styles.resetButton} onPress={handleReset}>
                <Text style={styles.resetButtonText}>Sifirla</Text>
              </TouchableOpacity>
            )}
            <TouchableOpacity
              style={[styles.applyButton, activeCount === 0 && {flex: 1}]}
              onPress={handleApply}
              activeOpacity={0.8}
            >
              <Text style={styles.applyButtonText}>
                Uygula{activeCount > 0 ? ` (${activeCount})` : ''}
              </Text>
            </TouchableOpacity>
          </View>
        </View>
      </View>
    </Modal>
  );
};

const styles = StyleSheet.create({
  overlay: {
    flex: 1,
    backgroundColor: Colors.overlay,
    justifyContent: 'flex-end',
  },
  content: {
    backgroundColor: Colors.surface,
    borderTopLeftRadius: BorderRadius.xxl,
    borderTopRightRadius: BorderRadius.xxl,
    maxHeight: '80%',
  },
  header: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    paddingHorizontal: Spacing.xl,
    paddingTop: Spacing.xl,
    paddingBottom: Spacing.md,
  },
  title: {
    fontSize: Fonts.sizes.xl,
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
  body: {
    paddingHorizontal: Spacing.xl,
  },
  section: {
    marginBottom: Spacing.xl,
  },
  sectionTitle: {
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
    marginBottom: Spacing.md,
  },
  chipContainer: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    gap: 8,
  },
  chip: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingHorizontal: 14,
    paddingVertical: 8,
    borderRadius: BorderRadius.round,
    backgroundColor: Colors.borderLight,
    gap: 4,
  },
  chipActive: {
    backgroundColor: Colors.primary,
  },
  chipText: {
    fontSize: Fonts.sizes.sm,
    fontWeight: Fonts.weights.semibold,
    color: Colors.text,
  },
  chipTextActive: {
    color: '#FFF',
  },
  sortOption: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingVertical: 10,
  },
  sortOptionText: {
    fontSize: Fonts.sizes.md,
    color: Colors.textSecondary,
    marginLeft: 10,
  },
  sortOptionTextActive: {
    color: Colors.text,
    fontWeight: Fonts.weights.semibold,
  },
  footer: {
    flexDirection: 'row',
    paddingHorizontal: Spacing.xl,
    paddingVertical: Spacing.lg,
    gap: 12,
    borderTopWidth: 1,
    borderTopColor: Colors.borderLight,
  },
  resetButton: {
    flex: 1,
    paddingVertical: 14,
    alignItems: 'center',
    borderRadius: BorderRadius.md,
    borderWidth: 1,
    borderColor: Colors.border,
  },
  resetButtonText: {
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.bold,
    color: Colors.textSecondary,
  },
  applyButton: {
    flex: 2,
    paddingVertical: 14,
    alignItems: 'center',
    borderRadius: BorderRadius.md,
    backgroundColor: Colors.primary,
  },
  applyButtonText: {
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.bold,
    color: '#FFF',
  },
});

export default FilterModal;
