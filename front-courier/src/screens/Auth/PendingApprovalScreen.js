import React from 'react';
import {View, Text, TouchableOpacity, StyleSheet} from 'react-native';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import {Colors, Fonts, Spacing, BorderRadius} from '../../theme';
import {useAuth} from '../../context/AuthContext';

const PendingApprovalScreen = ({navigation}) => {
  const {logout} = useAuth();
  return (
    <View style={styles.container}>
      <View style={styles.content}>
        <View style={styles.iconContainer}>
          <Icon
            name="clock-check-outline"
            size={72}
            color={Colors.primary}
          />
        </View>

        <Text style={styles.title}>Başvurunuz Alındı!</Text>

        <Text style={styles.message}>
          Başvurunuz yönetici tarafından inceleniyor. Onaylandığında size
          bildirim göndereceğiz.
        </Text>

        <TouchableOpacity
          style={styles.button}
          onPress={() => logout()}>
          <Icon
            name="arrow-left"
            size={20}
            color={Colors.textInverse}
            style={styles.buttonIcon}
          />
          <Text style={styles.buttonText}>Giriş Sayfasına Dön</Text>
        </TouchableOpacity>
      </View>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: Colors.background,
    justifyContent: 'center',
    alignItems: 'center',
    paddingHorizontal: Spacing.xl,
  },
  content: {
    backgroundColor: Colors.surface,
    borderRadius: BorderRadius.xl,
    padding: Spacing.xxxl,
    alignItems: 'center',
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 2},
    shadowOpacity: 0.06,
    shadowRadius: 8,
    elevation: 4,
    width: '100%',
  },
  iconContainer: {
    width: 120,
    height: 120,
    borderRadius: 60,
    backgroundColor: '#E8F2FF',
    justifyContent: 'center',
    alignItems: 'center',
    marginBottom: Spacing.xl,
  },
  title: {
    fontSize: Fonts.sizes.xxl,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
    marginBottom: Spacing.base,
    textAlign: 'center',
  },
  message: {
    fontSize: Fonts.sizes.base,
    color: Colors.textSecondary,
    textAlign: 'center',
    lineHeight: 24,
    marginBottom: Spacing.xxxl,
  },
  button: {
    flexDirection: 'row',
    backgroundColor: Colors.primary,
    borderRadius: BorderRadius.md,
    height: 52,
    justifyContent: 'center',
    alignItems: 'center',
    width: '100%',
  },
  buttonIcon: {
    marginRight: Spacing.sm,
  },
  buttonText: {
    color: Colors.textInverse,
    fontSize: Fonts.sizes.lg,
    fontWeight: Fonts.weights.semibold,
  },
});

export default PendingApprovalScreen;
