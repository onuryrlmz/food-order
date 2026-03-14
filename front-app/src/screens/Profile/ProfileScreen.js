import React from 'react';
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  TouchableOpacity,
} from 'react-native';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import {Colors, Fonts, Spacing, BorderRadius} from '../../theme';
import {useAuth} from '../../context/AuthContext';
import {useToast} from '../../context/ToastContext';

const ProfileScreen = ({navigation}) => {
  const {user, logout, isAuthenticated} = useAuth();
  const {showConfirm} = useToast();

  if (!isAuthenticated) {
    return (
      <View style={styles.container}>
        <View style={styles.header}>
          <Text style={styles.headerTitle}>Profilim</Text>
        </View>
        <View style={styles.guestContainer}>
          <Icon name="account-circle-outline" size={80} color={Colors.textTertiary} />
          <Text style={styles.guestTitle}>Giriş Yapın</Text>
          <Text style={styles.guestSubtitle}>Sipariş vermek ve profilinizi yönetmek için giriş yapın</Text>
          <TouchableOpacity style={styles.guestLoginButton} onPress={() => navigation.navigate('Login')}>
            <Text style={styles.guestLoginButtonText}>Giriş Yap</Text>
          </TouchableOpacity>
          <TouchableOpacity onPress={() => navigation.navigate('Register')}>
            <Text style={styles.guestRegisterText}>Hesabınız yok mu? <Text style={{color: Colors.primary, fontWeight: '700'}}>Kayıt Olun</Text></Text>
          </TouchableOpacity>
        </View>
      </View>
    );
  }

  const handleLogout = () => {
    showConfirm({
      title: 'Çıkış Yap',
      message: 'Hesabınızdan çıkış yapmak istediğinize emin misiniz?',
      confirmText: 'Çıkış Yap',
      cancelText: 'İptal',
      confirmStyle: 'destructive',
      onConfirm: logout,
    });
  };

  const menuItems = [
    {
      icon: 'account-edit-outline',
      label: 'Profil Bilgileri',
      subtitle: 'Ad, soyad, e-posta, telefon',
      onPress: () => navigation.navigate('EditProfile'),
      color: Colors.info,
    },
    {
      icon: 'map-marker-outline',
      label: 'Adreslerim',
      subtitle: 'Teslimat adreslerinizi yönetin',
      onPress: () => navigation.navigate('AddressList'),
      color: Colors.success,
    },
    {
      icon: 'credit-card-outline',
      label: 'Kayıtlı Kartlarım',
      subtitle: 'Ödeme kartlarınızı yönetin',
      onPress: () => navigation.navigate('SavedCards'),
      color: '#5856D6',
    },
    {
      icon: 'receipt',
      label: 'Sipariş Geçmişi',
      subtitle: 'Geçmiş siparişlerinizi görüntüleyin',
      onPress: () => navigation.navigate('OrderHistory'),
      color: Colors.secondary,
    },
    {
      icon: 'lock-outline',
      label: 'Şifre Değiştir',
      subtitle: 'Hesap güvenliğiniz için',
      onPress: () => navigation.navigate('ChangePassword'),
      color: Colors.warning,
    },
    {
      icon: 'help-circle-outline',
      label: 'Yardım & Destek',
      subtitle: 'Sıkça sorulan sorular',
      onPress: () => {},
      color: '#8E8E93',
    },
    {
      icon: 'information-outline',
      label: 'Hakkımızda',
      subtitle: 'Uygulama bilgileri',
      onPress: () => {},
      color: '#636366',
    },
  ];

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <Text style={styles.headerTitle}>Profilim</Text>
      </View>

      <ScrollView showsVerticalScrollIndicator={false} contentContainerStyle={styles.scrollContent}>
        <View style={styles.profileCard}>
          <View style={styles.avatarContainer}>
            <View style={styles.avatar}>
              <Text style={styles.avatarText}>
                {(user?.firstName?.[0] || '').toUpperCase()}
                {(user?.lastName?.[0] || '').toUpperCase()}
              </Text>
            </View>
          </View>
          <Text style={styles.userName}>
            {user?.firstName} {user?.lastName}
          </Text>
          <Text style={styles.userEmail}>{user?.email}</Text>
          {user?.phoneNumber && (
            <Text style={styles.userPhone}>{user.phoneNumber}</Text>
          )}
        </View>

        <View style={styles.menuContainer}>
          {menuItems.map((item, index) => (
            <TouchableOpacity
              key={index}
              style={styles.menuItem}
              onPress={item.onPress}
              activeOpacity={0.8}
            >
              <View style={[styles.menuIconBg, {backgroundColor: item.color + '15'}]}>
                <Icon name={item.icon} size={22} color={item.color} />
              </View>
              <View style={styles.menuTextContainer}>
                <Text style={styles.menuLabel}>{item.label}</Text>
                <Text style={styles.menuSubtitle}>{item.subtitle}</Text>
              </View>
              <Icon name="chevron-right" size={22} color={Colors.textLight} />
            </TouchableOpacity>
          ))}
        </View>

        <TouchableOpacity style={styles.logoutButton} onPress={handleLogout} activeOpacity={0.8}>
          <Icon name="logout" size={22} color={Colors.error} />
          <Text style={styles.logoutText}>Çıkış Yap</Text>
        </TouchableOpacity>

        <Text style={styles.versionText}>FoodOrder v1.0.0</Text>
      </ScrollView>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: Colors.background,
  },
  header: {
    paddingHorizontal: Spacing.base,
    paddingTop: 60,
    paddingBottom: Spacing.md,
    backgroundColor: Colors.surface,
    borderBottomWidth: 1,
    borderBottomColor: Colors.borderLight,
  },
  headerTitle: {
    fontSize: Fonts.sizes.xxl,
    fontWeight: Fonts.weights.heavy,
    color: Colors.text,
  },
  scrollContent: {
    paddingBottom: Spacing.xxxl,
  },
  profileCard: {
    backgroundColor: Colors.surface,
    marginHorizontal: Spacing.base,
    marginTop: Spacing.md,
    borderRadius: BorderRadius.xl,
    padding: Spacing.xl,
    alignItems: 'center',
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 2},
    shadowOpacity: 0.06,
    shadowRadius: 8,
    elevation: 4,
  },
  avatarContainer: {
    marginBottom: Spacing.md,
  },
  avatar: {
    width: 80,
    height: 80,
    borderRadius: 40,
    backgroundColor: Colors.primary,
    justifyContent: 'center',
    alignItems: 'center',
    shadowColor: Colors.primary,
    shadowOffset: {width: 0, height: 4},
    shadowOpacity: 0.3,
    shadowRadius: 8,
    elevation: 6,
  },
  avatarText: {
    fontSize: Fonts.sizes.xxxl,
    fontWeight: Fonts.weights.heavy,
    color: '#FFF',
  },
  userName: {
    fontSize: Fonts.sizes.xl,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
    marginBottom: 4,
  },
  userEmail: {
    fontSize: Fonts.sizes.md,
    color: Colors.textSecondary,
  },
  userPhone: {
    fontSize: Fonts.sizes.sm,
    color: Colors.textTertiary,
    marginTop: 2,
  },
  menuContainer: {
    backgroundColor: Colors.surface,
    marginHorizontal: Spacing.base,
    marginTop: Spacing.md,
    borderRadius: BorderRadius.lg,
    overflow: 'hidden',
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 1},
    shadowOpacity: 0.04,
    shadowRadius: 4,
    elevation: 2,
  },
  menuItem: {
    flexDirection: 'row',
    alignItems: 'center',
    padding: Spacing.base,
    borderBottomWidth: 1,
    borderBottomColor: Colors.borderLight,
  },
  menuIconBg: {
    width: 42,
    height: 42,
    borderRadius: 12,
    justifyContent: 'center',
    alignItems: 'center',
  },
  menuTextContainer: {
    flex: 1,
    marginLeft: Spacing.md,
  },
  menuLabel: {
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.semibold,
    color: Colors.text,
  },
  menuSubtitle: {
    fontSize: Fonts.sizes.sm,
    color: Colors.textSecondary,
    marginTop: 2,
  },
  logoutButton: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    marginHorizontal: Spacing.base,
    marginTop: Spacing.lg,
    paddingVertical: 16,
    borderRadius: BorderRadius.lg,
    backgroundColor: '#FFEBEE',
  },
  logoutText: {
    fontSize: Fonts.sizes.base,
    fontWeight: Fonts.weights.bold,
    color: Colors.error,
    marginLeft: 8,
  },
  versionText: {
    fontSize: Fonts.sizes.xs,
    color: Colors.textTertiary,
    textAlign: 'center',
    marginTop: Spacing.lg,
  },
  guestContainer: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    paddingHorizontal: 40,
  },
  guestTitle: {
    fontSize: Fonts.sizes.xxl,
    fontWeight: Fonts.weights.bold,
    color: Colors.text,
    marginTop: Spacing.lg,
    marginBottom: Spacing.sm,
  },
  guestSubtitle: {
    fontSize: Fonts.sizes.md,
    color: Colors.textSecondary,
    textAlign: 'center',
    lineHeight: 22,
    marginBottom: Spacing.xl,
  },
  guestLoginButton: {
    backgroundColor: Colors.primary,
    borderRadius: BorderRadius.lg,
    paddingVertical: 16,
    paddingHorizontal: 48,
    marginBottom: Spacing.md,
  },
  guestLoginButtonText: {
    color: '#FFF',
    fontSize: Fonts.sizes.lg,
    fontWeight: Fonts.weights.bold,
  },
  guestRegisterText: {
    fontSize: Fonts.sizes.md,
    color: Colors.textSecondary,
  },
});

export default ProfileScreen;
