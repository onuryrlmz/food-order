import React, { useState, useMemo } from 'react';
import {
  View,
  Text,
  Modal,
  TouchableOpacity,
  ScrollView,
  StyleSheet,
  Platform,
} from 'react-native';
import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import { Colors, Fonts, Spacing, BorderRadius } from '../theme';
import { useToast } from '../context/ToastContext';

const MenuOptionModal = ({ visible, menu, onClose, onAddToCart }) => {
  const { showToast } = useToast();
  const [selections, setSelections] = useState({});
  const [expandedDropdowns, setExpandedDropdowns] = useState({});

  // Reset selections when modal opens with new menu
  React.useEffect(() => {
    if (visible && menu) {
      const initialSelections = {};
      menu.menuOptions?.forEach(option => {
        initialSelections[option.id] = {
          selectedValues: {},
          valueOptionSelections: {},
        };
      });
      setSelections(initialSelections);
      setExpandedDropdowns({});
    }
  }, [visible, menu?.id]);

  const calculateTotalPrice = useMemo(() => {
    if (!menu) return 0;
    let total = menu.price || 0;

    Object.values(selections).forEach(optionSelection => {
      Object.entries(optionSelection.selectedValues || {}).forEach(([valueId, isSelected]) => {
        if (isSelected) {
          const value = menu.menuOptions
            ?.flatMap(o => o.menuOptionValues || [])
            .find(v => v.id === valueId);
          if (value?.price) total += value.price;

          // Add value option prices
          const valueOptionSelections = optionSelection.valueOptionSelections?.[valueId] || {};
          Object.entries(valueOptionSelections).forEach(([vooId, vooSelections]) => {
            Object.entries(vooSelections).forEach(([voovId, isSelected]) => {
              if (isSelected) {
                const voov = value?.menuOptionValueOptions
                  ?.find(voo => voo.id === vooId)
                  ?.menuOptionValueOptionValues?.find(v => v.id === voovId);
                if (voov?.price) total += voov.price;
              }
            });
          });
        }
      });
    });

    return total;
  }, [menu, selections]);

  const selectSingleValue = (option, value) => {
    const optionSel = selections[option.id] || { selectedValues: {}, valueOptionSelections: {} };
    const newSelectedValues = {};
    newSelectedValues[value.id] = true;

    setSelections(prev => ({
      ...prev,
      [option.id]: {
        ...optionSel,
        selectedValues: newSelectedValues,
        valueOptionSelections: optionSel.valueOptionSelections || {},
      },
    }));
    setExpandedDropdowns(prev => ({ ...prev, [option.id]: false }));
  };

  const toggleDropdown = (optionId) => {
    setExpandedDropdowns(prev => ({ ...prev, [optionId]: !prev[optionId] }));
  };

  const toggleValueSelection = (option, value) => {
    const optionSel = selections[option.id] || { selectedValues: {}, valueOptionSelections: {} };
    const currentSelectedCount = Object.values(optionSel.selectedValues).filter(Boolean).length;
    const isCurrentlySelected = optionSel.selectedValues[value.id];

    // Check maxCount
    if (!isCurrentlySelected && option.maxCount && currentSelectedCount >= option.maxCount) {
      showToast(`Bu seçenekten en fazla ${option.maxCount} adet seçebilirsiniz.`, 'warning');
      return;
    }

    setSelections(prev => ({
      ...prev,
      [option.id]: {
        ...optionSel,
        selectedValues: {
          ...optionSel.selectedValues,
          [value.id]: !isCurrentlySelected,
        },
        valueOptionSelections: optionSel.valueOptionSelections || {},
      },
    }));
  };

  const toggleValueOptionValue = (option, value, voo, voov) => {
    const optionSel = selections[option.id] || { selectedValues: {}, valueOptionSelections: {} };
    const vooSelections = optionSel.valueOptionSelections?.[value.id]?.[voo.id] || {};
    const currentCount = Object.values(vooSelections).filter(Boolean).length;
    const isCurrentlySelected = vooSelections[voov.id];

    if (!isCurrentlySelected && voo.maxCount && currentCount >= voo.maxCount) {
      showToast(`Bu seçenekten en fazla ${voo.maxCount} adet seçebilirsiniz.`, 'warning');
      return;
    }

    setSelections(prev => ({
      ...prev,
      [option.id]: {
        ...optionSel,
        selectedValues: optionSel.selectedValues || {},
        valueOptionSelections: {
          ...optionSel.valueOptionSelections,
          [value.id]: {
            ...optionSel.valueOptionSelections?.[value.id],
            [voo.id]: {
              ...vooSelections,
              [voov.id]: !isCurrentlySelected,
            },
          },
        },
      },
    }));
  };

  const validateSelections = () => {
    if (!menu?.menuOptions) return true;

    for (const option of menu.menuOptions) {
      const optionSel = selections[option.id] || { selectedValues: {} };
      const selectedCount = Object.values(optionSel.selectedValues).filter(Boolean).length;

      if (option.minCount > 0 && selectedCount < option.minCount) {
        showToast(`"${option.name}" için en az ${option.minCount} seçim yapmalısınız.`, 'warning');
        return false;
      }
    }
    return true;
  };

  const handleAddToCart = () => {
    if (!validateSelections()) return;

    const selectedOptions = [];

    menu.menuOptions?.forEach(option => {
      const optionSel = selections[option.id] || { selectedValues: {}, valueOptionSelections: {} };

      Object.entries(optionSel.selectedValues).forEach(([valueId, isSelected]) => {
        if (isSelected) {
          const value = option.menuOptionValues?.find(v => v.id === valueId);
          if (!value) return;

          const selectedValueOptionValues = [];

          // Get nested selections
          const vooSelections = optionSel.valueOptionSelections?.[valueId] || {};
          Object.entries(vooSelections).forEach(([vooId, voovSelections]) => {
            const voo = value.menuOptionValueOptions?.find(o => o.id === vooId);
            Object.entries(voovSelections).forEach(([voovId, isVoovSelected]) => {
              if (isVoovSelected) {
                const voov = voo?.menuOptionValueOptionValues?.find(v => v.id === voovId);
                if (voov) {
                  selectedValueOptionValues.push({
                    id: voov.id,
                    name: voov.name,
                    price: voov.price || 0,
                    productId: voov.productId,
                    menuOptionValueOptionId: voo?.id,
                    parentOptionName: voo?.name,
                  });
                }
              }
            });
          });

          selectedOptions.push({
            optionId: option.id,
            optionName: option.name,
            valueId: value.id,
            valueName: value.name,
            valuePrice: value.price || 0,
            productId: value.productId,
            valueOptionValues: selectedValueOptionValues,
          });
        }
      });
    });

    onAddToCart(menu, selectedOptions, calculateTotalPrice);
    onClose();
  };

  if (!menu) return null;

  return (
    <Modal visible={visible} animationType="slide" onRequestClose={onClose}>
      <View style={styles.overlay}>
        <View style={styles.container}>
          <View style={styles.header}>
            <View style={styles.headerTop}>
              <Text style={styles.menuName}>{menu.name}</Text>
              <TouchableOpacity onPress={onClose} style={styles.closeButton}>
                <Icon name="close" size={24} color={Colors.text} />
              </TouchableOpacity>
            </View>
            <Text style={styles.basePrice}>Başlangıç fiyatı: ₺{menu.price?.toFixed(2)}</Text>
          </View>

          <ScrollView style={styles.content} showsVerticalScrollIndicator={false}>
            {menu.menuOptions?.map(option => {
              const optionSel = selections[option.id] || { selectedValues: {}, valueOptionSelections: {} };
              const selectedCount = Object.values(optionSel.selectedValues).filter(Boolean).length;

              return (
                <View key={option.id} style={styles.optionSection}>
                  <View style={styles.optionHeader}>
                    <Text style={styles.optionName}>{option.name}</Text>
                    <Text style={styles.optionCount}>
                      {selectedCount}/{option.maxCount || '∞'}
                      {option.minCount > 0 && <Text style={styles.required}> *Zorunlu</Text>}
                    </Text>
                  </View>

                  {option.maxCount === 1 ? (
                    /* Single select - Dropdown */
                    <View>
                      {(() => {
                        const selectedValue = option.menuOptionValues?.find(v => optionSel.selectedValues?.[v.id]);
                        const isOpen = expandedDropdowns[option.id];
                        return (
                          <>
                            <TouchableOpacity
                              style={styles.dropdownTrigger}
                              onPress={() => toggleDropdown(option.id)}
                              activeOpacity={0.7}
                            >
                              <Text style={selectedValue ? styles.dropdownText : styles.dropdownPlaceholder}>
                                {selectedValue ? selectedValue.name : 'Seçiniz...'}
                              </Text>
                              <View style={styles.dropdownRight}>
                                {selectedValue?.price > 0 && (
                                  <Text style={styles.dropdownPrice}>+₺{selectedValue.price?.toFixed(2)}</Text>
                                )}
                                <Icon name={isOpen ? 'chevron-up' : 'chevron-down'} size={22} color={Colors.textSecondary} />
                              </View>
                            </TouchableOpacity>
                            {isOpen && (
                              <View style={styles.dropdownList}>
                                {option.menuOptionValues?.map(value => {
                                  const isSelected = optionSel.selectedValues?.[value.id];
                                  return (
                                    <TouchableOpacity
                                      key={value.id}
                                      style={[styles.dropdownItem, isSelected && styles.dropdownItemSelected]}
                                      onPress={() => selectSingleValue(option, value)}
                                    >
                                      <Text style={[styles.dropdownItemText, isSelected && styles.dropdownItemTextSelected]}>
                                        {value.name}
                                      </Text>
                                      {value.price > 0 && (
                                        <Text style={styles.dropdownItemPrice}>+₺{value.price?.toFixed(2)}</Text>
                                      )}
                                    </TouchableOpacity>
                                  );
                                })}
                              </View>
                            )}
                            {/* Nested options for selected value */}
                            {selectedValue && selectedValue.menuOptionValueOptions?.map(voo => {
                              const vooSelections = optionSel.valueOptionSelections?.[selectedValue.id] || {};
                              const vooSelCount = Object.values(vooSelections[voo.id] || {}).filter(Boolean).length;
                              return (
                                <View key={voo.id} style={styles.nestedSection}>
                                  <View style={styles.nestedHeader}>
                                    <Text style={styles.nestedName}>{voo.name}</Text>
                                    <Text style={styles.nestedCount}>{vooSelCount}/{voo.maxCount || '∞'}</Text>
                                  </View>
                                  {voo.menuOptionValueOptionValues?.map(voov => {
                                    const isVoovSelected = vooSelections[voo.id]?.[voov.id];
                                    return (
                                      <TouchableOpacity
                                        key={voov.id}
                                        style={[styles.nestedItem, isVoovSelected && styles.nestedItemSelected]}
                                        onPress={() => toggleValueOptionValue(option, selectedValue, voo, voov)}
                                      >
                                        <View style={styles.valueLeft}>
                                          <View style={[styles.smallCheckbox, isVoovSelected && styles.smallCheckboxSelected]}>
                                            {isVoovSelected && <Icon name="check" size={12} color="#FFF" />}
                                          </View>
                                          <Text style={styles.nestedItemName}>{voov.name}</Text>
                                        </View>
                                        {voov.price > 0 && (
                                          <Text style={styles.nestedItemPrice}>+₺{voov.price?.toFixed(2)}</Text>
                                        )}
                                      </TouchableOpacity>
                                    );
                                  })}
                                </View>
                              );
                            })}
                          </>
                        );
                      })()}
                    </View>
                  ) : (
                    /* Multi select - Checkbox list */
                    option.menuOptionValues?.map(value => {
                      const isSelected = optionSel.selectedValues?.[value.id];
                      const vooSelections = optionSel.valueOptionSelections?.[value.id] || {};

                      return (
                        <View key={value.id}>
                          <TouchableOpacity
                            style={[styles.valueItem, isSelected && styles.valueItemSelected]}
                            onPress={() => toggleValueSelection(option, value)}
                          >
                            <View style={styles.valueLeft}>
                              <View style={[styles.checkbox, isSelected && styles.checkboxSelected]}>
                                {isSelected && <Icon name="check" size={16} color="#FFF" />}
                              </View>
                              <Text style={styles.valueName}>{value.name}</Text>
                            </View>
                            {value.price > 0 && (
                              <Text style={styles.valuePrice}>+₺{value.price?.toFixed(2)}</Text>
                            )}
                          </TouchableOpacity>

                          {/* Nested Value Options */}
                          {isSelected && value.menuOptionValueOptions?.map(voo => {
                            const vooSelCount = Object.values(vooSelections[voo.id] || {}).filter(Boolean).length;

                            return (
                              <View key={voo.id} style={styles.nestedSection}>
                                <View style={styles.nestedHeader}>
                                  <Text style={styles.nestedName}>{voo.name}</Text>
                                  <Text style={styles.nestedCount}>
                                    {vooSelCount}/{voo.maxCount || '∞'}
                                  </Text>
                                </View>

                                {voo.menuOptionValueOptionValues?.map(voov => {
                                  const isVoovSelected = vooSelections[voo.id]?.[voov.id];

                                  return (
                                    <TouchableOpacity
                                      key={voov.id}
                                      style={[styles.nestedItem, isVoovSelected && styles.nestedItemSelected]}
                                      onPress={() => toggleValueOptionValue(option, value, voo, voov)}
                                    >
                                      <View style={styles.valueLeft}>
                                        <View style={[styles.smallCheckbox, isVoovSelected && styles.smallCheckboxSelected]}>
                                          {isVoovSelected && <Icon name="check" size={12} color="#FFF" />}
                                        </View>
                                        <Text style={styles.nestedItemName}>{voov.name}</Text>
                                      </View>
                                      {voov.price > 0 && (
                                        <Text style={styles.nestedItemPrice}>+₺{voov.price?.toFixed(2)}</Text>
                                      )}
                                    </TouchableOpacity>
                                  );
                                })}
                              </View>
                            );
                          })}
                        </View>
                      );
                    })
                  )}
                </View>
              );
            })}
          </ScrollView>

          <View style={styles.footer}>
            <View style={styles.totalContainer}>
              <Text style={styles.totalLabel}>Toplam</Text>
              <Text style={styles.totalPrice}>₺{calculateTotalPrice.toFixed(2)}</Text>
            </View>
            <TouchableOpacity style={styles.addButton} onPress={handleAddToCart}>
              <Text style={styles.addButtonText}>Sepete Ekle</Text>
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
    backgroundColor: Colors.background,
  },
  container: {
    flex: 1,
    backgroundColor: Colors.background,
    paddingTop: Platform.OS === 'ios' ? 50 : 24,
  },
  header: {
    padding: Spacing.lg,
    borderBottomWidth: 1,
    borderBottomColor: Colors.borderLight,
  },
  headerTop: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
  },
  menuName: {
    ...Fonts.h3,
    color: Colors.text,
    flex: 1,
  },
  closeButton: {
    padding: Spacing.xs,
  },
  basePrice: {
    ...Fonts.body2,
    color: Colors.textSecondary,
    marginTop: Spacing.xs,
  },
  content: {
    flex: 1,
    padding: Spacing.md,
  },
  optionSection: {
    marginBottom: Spacing.lg,
  },
  optionHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: Spacing.sm,
  },
  optionName: {
    ...Fonts.h4,
    color: Colors.text,
  },
  optionCount: {
    ...Fonts.caption,
    color: Colors.textSecondary,
  },
  required: {
    color: Colors.error,
    fontWeight: '600',
  },
  valueItem: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    padding: Spacing.md,
    backgroundColor: Colors.card,
    borderRadius: BorderRadius.md,
    marginBottom: Spacing.xs,
    borderWidth: 1,
    borderColor: Colors.borderLight,
  },
  valueItemSelected: {
    borderColor: Colors.primary,
    backgroundColor: Colors.primaryLight,
  },
  valueLeft: {
    flexDirection: 'row',
    alignItems: 'center',
    flex: 1,
  },
  checkbox: {
    width: 22,
    height: 22,
    borderRadius: 6,
    borderWidth: 2,
    borderColor: Colors.border,
    marginRight: Spacing.sm,
    alignItems: 'center',
    justifyContent: 'center',
  },
  checkboxSelected: {
    backgroundColor: Colors.primary,
    borderColor: Colors.primary,
  },
  valueName: {
    ...Fonts.body,
    color: Colors.text,
    flex: 1,
  },
  valuePrice: {
    ...Fonts.body2,
    color: Colors.primary,
    fontWeight: '600',
  },
  nestedSection: {
    marginLeft: Spacing.lg,
    marginTop: Spacing.sm,
    paddingLeft: Spacing.sm,
    borderLeftWidth: 2,
    borderLeftColor: Colors.borderLight,
  },
  nestedHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginBottom: Spacing.xs,
  },
  nestedName: {
    ...Fonts.body2,
    color: Colors.textSecondary,
    fontWeight: '600',
  },
  nestedCount: {
    ...Fonts.caption,
    color: Colors.textLight,
  },
  nestedItem: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    padding: Spacing.sm,
    backgroundColor: Colors.background,
    borderRadius: BorderRadius.sm,
    marginBottom: Spacing.xs,
  },
  nestedItemSelected: {
    backgroundColor: Colors.primaryLight,
  },
  smallCheckbox: {
    width: 18,
    height: 18,
    borderRadius: 4,
    borderWidth: 1.5,
    borderColor: Colors.border,
    marginRight: Spacing.sm,
    alignItems: 'center',
    justifyContent: 'center',
  },
  smallCheckboxSelected: {
    backgroundColor: Colors.primary,
    borderColor: Colors.primary,
  },
  nestedItemName: {
    ...Fonts.body2,
    color: Colors.text,
  },
  nestedItemPrice: {
    ...Fonts.caption,
    color: Colors.primary,
    fontWeight: '600',
  },
  footer: {
    flexDirection: 'row',
    alignItems: 'center',
    padding: Spacing.lg,
    borderTopWidth: 1,
    borderTopColor: Colors.borderLight,
    backgroundColor: Colors.card,
  },
  totalContainer: {
    flex: 1,
  },
  totalLabel: {
    ...Fonts.caption,
    color: Colors.textSecondary,
  },
  totalPrice: {
    ...Fonts.h3,
    color: Colors.text,
  },
  addButton: {
    backgroundColor: Colors.primary,
    paddingHorizontal: Spacing.xl,
    paddingVertical: Spacing.md,
    borderRadius: BorderRadius.lg,
  },
  addButtonText: {
    ...Fonts.body,
    color: '#FFF',
    fontWeight: '600',
  },
  dropdownTrigger: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    padding: Spacing.md,
    backgroundColor: Colors.card,
    borderRadius: BorderRadius.md,
    borderWidth: 1,
    borderColor: Colors.borderLight,
  },
  dropdownText: {
    ...Fonts.body,
    color: Colors.text,
    flex: 1,
  },
  dropdownPlaceholder: {
    ...Fonts.body,
    color: Colors.textLight,
    flex: 1,
  },
  dropdownRight: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: 6,
  },
  dropdownPrice: {
    ...Fonts.body2,
    color: Colors.primary,
    fontWeight: '600',
  },
  dropdownList: {
    backgroundColor: Colors.card,
    borderRadius: BorderRadius.md,
    borderWidth: 1,
    borderColor: Colors.borderLight,
    marginTop: 4,
    overflow: 'hidden',
  },
  dropdownItem: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    paddingVertical: Spacing.sm,
    paddingHorizontal: Spacing.md,
    borderBottomWidth: StyleSheet.hairlineWidth,
    borderBottomColor: Colors.borderLight,
  },
  dropdownItemSelected: {
    backgroundColor: Colors.primaryLight,
  },
  dropdownItemText: {
    ...Fonts.body,
    color: Colors.text,
    flex: 1,
  },
  dropdownItemTextSelected: {
    color: Colors.primary,
    fontWeight: '600',
  },
  dropdownItemPrice: {
    ...Fonts.body2,
    color: Colors.primary,
    fontWeight: '600',
  },
});

export default MenuOptionModal;
