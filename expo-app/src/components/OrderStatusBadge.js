import React from 'react';
import {View, Text, StyleSheet} from 'react-native';
import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import {ORDER_STATUS} from '../utils/constants';
import {Fonts} from '../theme';

const OrderStatusBadge = ({statusId, size = 'normal'}) => {
  const status = ORDER_STATUS[statusId] || {label: 'Bilinmiyor', color: '#999', icon: 'help-circle-outline'};

  const isSmall = size === 'small';

  return (
    <View style={[styles.badge, {backgroundColor: status.color + '15'}, isSmall && styles.badgeSmall]}>
      <Icon name={status.icon} size={isSmall ? 12 : 16} color={status.color} />
      <Text style={[styles.text, {color: status.color}, isSmall && styles.textSmall]}>
        {status.label}
      </Text>
    </View>
  );
};

const styles = StyleSheet.create({
  badge: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingHorizontal: 10,
    paddingVertical: 6,
    borderRadius: 8,
    alignSelf: 'flex-start',
  },
  badgeSmall: {
    paddingHorizontal: 8,
    paddingVertical: 4,
    borderRadius: 6,
  },
  text: {
    fontSize: Fonts.sizes.sm,
    fontWeight: Fonts.weights.semibold,
    marginLeft: 5,
  },
  textSmall: {
    fontSize: Fonts.sizes.xs,
  },
});

export default OrderStatusBadge;
