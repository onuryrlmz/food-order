import React from 'react';
import {View, Text, StyleSheet, TouchableOpacity} from 'react-native';
import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import {Colors, Fonts, Spacing, BorderRadius} from '../theme';
import {ORDER_STATUS} from '../utils/constants';

const STEPS = [
  {statusId: 4, label: 'Onay'},
  {statusId: 6, label: 'Hazırlanıyor'},
  {statusId: 9, label: 'Kurye Atandı'},
  {statusId: 10, label: 'Teslim Aldı'},
  {statusId: 7, label: 'Yolda'},
  {statusId: 8, label: 'Teslim'},
];

const getEstimatedTime = (order) => {
  const created = new Date(order.createdDate);
  const now = new Date();
  const diffMin = Math.floor((now - created) / 60000);

  switch (order.statusId) {
    case 4:
      return {remaining: Math.max(35 - diffMin, 5), total: 35, label: 'Tahmini teslim'};
    case 6:
      return {remaining: Math.max(25 - diffMin, 5), total: 25, label: 'Tahmini teslim'};
    case 9:
      return {remaining: Math.max(20 - diffMin, 5), total: 20, label: 'Tahmini teslim'};
    case 10:
      return {remaining: Math.max(15 - diffMin, 3), total: 15, label: 'Tahmini teslim'};
    case 7:
      return {remaining: Math.max(15 - diffMin, 3), total: 15, label: 'Tahmini varış'};
    default:
      return {remaining: 30, total: 30, label: 'Tahmini teslim'};
  }
};

const getStepIndex = (statusId) => {
  if (statusId === 4) return 0;
  if (statusId === 6) return 1;
  if (statusId === 9) return 2;
  if (statusId === 10) return 3;
  if (statusId === 7) return 4;
  if (statusId === 8) return 5;
  return 0;
};

const ActiveOrdersSummary = ({orders, onPress}) => {
  if (!orders || orders.length === 0) return null;

  const statusCounts = {};
  orders.forEach(o => {
    const s = ORDER_STATUS[o.statusId] || ORDER_STATUS[4];
    if (!statusCounts[o.statusId]) statusCounts[o.statusId] = {count: 0, ...s};
    statusCounts[o.statusId].count++;
  });

  return (
    <TouchableOpacity style={styles.summaryContainer} onPress={onPress} activeOpacity={0.85}>
      <View style={styles.summaryLeft}>
        <View style={styles.summaryIconBg}>
          <Icon name="package-variant" size={20} color={Colors.primary} />
          <View style={styles.summaryBadge}>
            <Text style={styles.summaryBadgeText}>{orders.length}</Text>
          </View>
        </View>
        <View style={styles.summaryTextContainer}>
          <Text style={styles.summaryTitle}>{orders.length} aktif siparişiniz var</Text>
          <View style={styles.summaryChips}>
            {Object.values(statusCounts).map((s, i) => (
              <View key={i} style={[styles.summaryChip, {backgroundColor: s.color + '15'}]}>
                <Icon name={s.icon} size={12} color={s.color} />
                <Text style={[styles.summaryChipText, {color: s.color}]}>{s.count} {s.label.toLowerCase()}</Text>
              </View>
            ))}
          </View>
        </View>
      </View>
      <Icon name="chevron-right" size={22} color={Colors.textSecondary} />
    </TouchableOpacity>
  );
};

const ActiveOrderBanner = ({order, onPress}) => {
  if (!order) return null;

  const status = ORDER_STATUS[order.statusId] || ORDER_STATUS[4];
  const est = getEstimatedTime(order);
  const currentStep = getStepIndex(order.statusId);
  const itemCount = order.items?.reduce((sum, i) => sum + i.quantity, 0) || 0;

  return (
    <TouchableOpacity style={styles.container} onPress={onPress} activeOpacity={0.85}>
      <View style={styles.topRow}>
        <View style={[styles.statusBadge, {backgroundColor: status.color + '18'}]}>
          <Icon name={status.icon} size={16} color={status.color} />
          <Text style={[styles.statusText, {color: status.color}]}>{status.label}</Text>
        </View>
        <View style={styles.timeContainer}>
          <Icon name="clock-fast" size={16} color={Colors.primary} />
          <Text style={styles.timeText}>~{est.remaining} dk</Text>
        </View>
      </View>

      <View style={styles.infoRow}>
        <Icon name="store-outline" size={18} color={Colors.text} />
        <Text style={styles.restaurantName} numberOfLines={1}>{order.restaurantName}</Text>
        <Text style={styles.itemCount}>{itemCount} ürün</Text>
      </View>

      <View style={styles.progressContainer}>
        {STEPS.map((step, index) => {
          const isActive = index <= currentStep;
          const isLast = index === STEPS.length - 1;
          return (
            <View key={step.statusId} style={styles.stepWrapper}>
              <View style={styles.stepRow}>
                <View style={[styles.stepDot, isActive && {backgroundColor: status.color}]}>
                  {isActive && <Icon name="check" size={10} color="#FFF" />}
                </View>
                {!isLast && (
                  <View style={[styles.stepLine, isActive && index < currentStep && {backgroundColor: status.color}]} />
                )}
              </View>
              <Text style={[styles.stepLabel, isActive && {color: Colors.text, fontWeight: Fonts.weights.semibold}]}>
                {step.label}
              </Text>
            </View>
          );
        })}
      </View>

      <View style={styles.footer}>
        <Text style={styles.footerText}>Sipariş detayı için dokunun</Text>
        <Icon name="chevron-right" size={18} color={Colors.textTertiary} />
      </View>
    </TouchableOpacity>
  );
};

const styles = StyleSheet.create({
  container: {
    backgroundColor: Colors.surface,
    marginHorizontal: Spacing.base,
    marginBottom: Spacing.md,
    borderRadius: BorderRadius.lg,
    padding: Spacing.base,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 2},
    shadowOpacity: 0.08,
    shadowRadius: 8,
    elevation: 3,
    borderWidth: 1,
    borderColor: Colors.primary + '20',
  },
  topRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: Spacing.md,
  },
  statusBadge: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingHorizontal: 10,
    paddingVertical: 5,
    borderRadius: 20,
    gap: 5,
  },
  statusText: {
    fontSize: Fonts.sizes.xs,
    fontWeight: Fonts.weights.bold,
  },
  timeContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: 4,
  },
  timeText: {
    fontSize: Fonts.sizes.md,
    fontWeight: Fonts.weights.heavy,
    color: Colors.primary,
  },
  infoRow: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: Spacing.md,
    gap: 6,
  },
  restaurantName: {
    flex: 1,
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
  },
  itemCount: {
    fontSize: Fonts.sizes.sm,
    color: Colors.textSecondary,
  },
  progressContainer: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginBottom: Spacing.md,
  },
  stepWrapper: {
    flex: 1,
    alignItems: 'center',
  },
  stepRow: {
    flexDirection: 'row',
    alignItems: 'center',
    width: '100%',
    justifyContent: 'center',
    marginBottom: 6,
  },
  stepDot: {
    width: 20,
    height: 20,
    borderRadius: 10,
    backgroundColor: Colors.borderLight,
    justifyContent: 'center',
    alignItems: 'center',
  },
  stepLine: {
    flex: 1,
    height: 2,
    backgroundColor: Colors.borderLight,
    marginHorizontal: 2,
  },
  stepLabel: {
    fontSize: Fonts.sizes.xs,
    color: Colors.textTertiary,
  },
  footer: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    paddingTop: Spacing.sm,
    borderTopWidth: 1,
    borderTopColor: Colors.borderLight,
    gap: 4,
  },
  footerText: {
    fontSize: Fonts.sizes.xs,
    color: Colors.textTertiary,
  },
  summaryContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: Colors.surface,
    marginHorizontal: Spacing.base,
    marginBottom: Spacing.md,
    borderRadius: BorderRadius.lg,
    padding: Spacing.md,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 1},
    shadowOpacity: 0.06,
    shadowRadius: 6,
    elevation: 2,
    borderWidth: 1,
    borderColor: Colors.primary + '20',
  },
  summaryLeft: {
    flex: 1,
    flexDirection: 'row',
    alignItems: 'center',
    gap: Spacing.md,
  },
  summaryIconBg: {
    width: 44,
    height: 44,
    borderRadius: 14,
    backgroundColor: Colors.primary + '12',
    justifyContent: 'center',
    alignItems: 'center',
  },
  summaryBadge: {
    position: 'absolute',
    top: -4,
    right: -4,
    backgroundColor: Colors.primary,
    width: 18,
    height: 18,
    borderRadius: 9,
    justifyContent: 'center',
    alignItems: 'center',
  },
  summaryBadgeText: {
    color: '#FFF',
    fontSize: 10,
    fontWeight: Fonts.weights.bold,
  },
  summaryTextContainer: {
    flex: 1,
  },
  summaryTitle: {
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
    marginBottom: 4,
  },
  summaryChips: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    gap: 6,
  },
  summaryChip: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingHorizontal: 8,
    paddingVertical: 3,
    borderRadius: 12,
    gap: 4,
  },
  summaryChipText: {
    fontSize: Fonts.sizes.xs,
    fontWeight: Fonts.weights.semibold,
  },
});

export {ActiveOrdersSummary};
export default ActiveOrderBanner;
