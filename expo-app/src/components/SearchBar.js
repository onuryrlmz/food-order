import React from 'react';
import {View, TextInput, StyleSheet, TouchableOpacity} from 'react-native';
import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import {Colors, Fonts, Spacing, BorderRadius} from '../theme';

const SearchBar = ({value, onChangeText, placeholder = 'Restoran veya yemek ara...', onClear}) => {
  return (
    <View style={styles.container}>
      <Icon name="magnify" size={22} color={Colors.textSecondary} style={styles.icon} />
      <TextInput
        style={styles.input}
        value={value}
        onChangeText={onChangeText}
        placeholder={placeholder}
        placeholderTextColor={Colors.textTertiary}
        returnKeyType="search"
      />
      {value ? (
        <TouchableOpacity onPress={onClear} hitSlop={{top: 10, bottom: 10, left: 10, right: 10}}>
          <Icon name="close-circle" size={20} color={Colors.textLight} />
        </TouchableOpacity>
      ) : null}
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: Colors.borderLight,
    borderRadius: BorderRadius.lg,
    paddingHorizontal: Spacing.md,
    marginHorizontal: Spacing.base,
    marginBottom: Spacing.md,
    height: 48,
  },
  icon: {
    marginRight: Spacing.sm,
  },
  input: {
    flex: 1,
    fontSize: Fonts.sizes.md,
    color: Colors.text,
    padding: 0,
  },
});

export default SearchBar;
