import React, {createContext, useContext, useState, useCallback, useRef, useEffect} from 'react';
import {
  View,
  Text,
  StyleSheet,
  Animated,
  TouchableOpacity,
  Modal,
  Dimensions,
  StatusBar,
} from 'react-native';
import {useSafeAreaInsets} from 'react-native-safe-area-context';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import {Colors, Fonts, Spacing, BorderRadius} from '../theme';

const ToastContext = createContext(null);
const {width: SCREEN_WIDTH} = Dimensions.get('window');

export const ToastProvider = ({children}) => {
  // --- Toast (üstten bilgilendirme) ---
  const [toast, setToast] = useState(null);
  const toastAnim = useRef(new Animated.Value(-100)).current;
  const toastTimer = useRef(null);

  // --- Confirm (ortada butonlu modal) ---
  const [confirm, setConfirm] = useState(null);
  const confirmAnim = useRef(new Animated.Value(0)).current;

  // Toast göster
  const showToast = useCallback((message, type = 'info', duration = 3000) => {
    if (toastTimer.current) clearTimeout(toastTimer.current);
    setToast({message, type});
    Animated.spring(toastAnim, {
      toValue: 0,
      useNativeDriver: true,
      tension: 80,
      friction: 12,
    }).start();
    toastTimer.current = setTimeout(() => {
      hideToast();
    }, duration);
  }, []);

  const hideToast = useCallback(() => {
    Animated.timing(toastAnim, {
      toValue: -100,
      duration: 200,
      useNativeDriver: true,
    }).start(() => setToast(null));
  }, []);

  // Confirm göster
  const showConfirm = useCallback(({title, message, confirmText = 'Evet', cancelText = 'Hayır', confirmStyle = 'destructive', onConfirm, onCancel}) => {
    setConfirm({title, message, confirmText, cancelText, confirmStyle, onConfirm, onCancel});
    Animated.spring(confirmAnim, {
      toValue: 1,
      useNativeDriver: true,
      tension: 65,
      friction: 10,
    }).start();
  }, []);

  const hideConfirm = useCallback(() => {
    Animated.timing(confirmAnim, {
      toValue: 0,
      duration: 150,
      useNativeDriver: true,
    }).start(() => setConfirm(null));
  }, []);

  const handleConfirm = useCallback(() => {
    const cb = confirm?.onConfirm;
    hideConfirm();
    if (cb) setTimeout(cb, 200);
  }, [confirm]);

  const handleCancel = useCallback(() => {
    const cb = confirm?.onCancel;
    hideConfirm();
    if (cb) setTimeout(cb, 200);
  }, [confirm]);

  const insets = useSafeAreaInsets();

  const getToastIcon = (type) => {
    switch (type) {
      case 'success': return 'check-circle';
      case 'error': return 'alert-circle';
      case 'warning': return 'alert';
      default: return 'information';
    }
  };

  const getToastColor = (type) => {
    switch (type) {
      case 'success': return '#16A34A';
      case 'error': return '#DC2626';
      case 'warning': return '#F59E0B';
      default: return Colors.primary;
    }
  };

  const getConfirmButtonColor = (style) => {
    switch (style) {
      case 'destructive': return '#DC2626';
      case 'primary': return Colors.primary;
      default: return Colors.primary;
    }
  };

  return (
    <ToastContext.Provider value={{showToast, showConfirm}}>
      {children}

      {/* Toast - Üstten bilgilendirme */}
      {toast && (
        <Animated.View
          style={[
            styles.toastContainer,
            {
              paddingTop: insets.top + 8,
              transform: [{translateY: toastAnim}],
            },
          ]}
        >
          <TouchableOpacity style={styles.toastContent} onPress={hideToast} activeOpacity={0.9}>
            <View style={[styles.toastIconContainer, {backgroundColor: getToastColor(toast.type) + '15'}]}>
              <Icon name={getToastIcon(toast.type)} size={20} color={getToastColor(toast.type)} />
            </View>
            <Text style={styles.toastMessage} numberOfLines={3}>{toast.message}</Text>
            <Icon name="close" size={18} color={Colors.textSecondary} />
          </TouchableOpacity>
        </Animated.View>
      )}

      {/* Confirm - Ortada butonlu modal */}
      {confirm && (
        <Modal transparent visible animationType="none" statusBarTranslucent>
          <Animated.View
            style={[
              styles.confirmOverlay,
              {opacity: confirmAnim},
            ]}
          >
            <TouchableOpacity style={styles.confirmBackdrop} activeOpacity={1} onPress={handleCancel} />
            <Animated.View
              style={[
                styles.confirmCard,
                {
                  transform: [{
                    scale: confirmAnim.interpolate({
                      inputRange: [0, 1],
                      outputRange: [0.85, 1],
                    }),
                  }],
                },
              ]}
            >
              {confirm.title && (
                <Text style={styles.confirmTitle}>{confirm.title}</Text>
              )}
              {confirm.message && (
                <Text style={styles.confirmMessage}>{confirm.message}</Text>
              )}
              <View style={styles.confirmButtons}>
                <TouchableOpacity
                  style={[styles.confirmButton, styles.confirmButtonCancel]}
                  onPress={handleCancel}
                  activeOpacity={0.8}
                >
                  <Text style={styles.confirmButtonCancelText}>{confirm.cancelText}</Text>
                </TouchableOpacity>
                <TouchableOpacity
                  style={[styles.confirmButton, {backgroundColor: getConfirmButtonColor(confirm.confirmStyle)}]}
                  onPress={handleConfirm}
                  activeOpacity={0.8}
                >
                  <Text style={styles.confirmButtonConfirmText}>{confirm.confirmText}</Text>
                </TouchableOpacity>
              </View>
            </Animated.View>
          </Animated.View>
        </Modal>
      )}
    </ToastContext.Provider>
  );
};

export const useToast = () => {
  const context = useContext(ToastContext);
  if (!context) {
    throw new Error('useToast must be used within a ToastProvider');
  }
  return context;
};

const styles = StyleSheet.create({
  // Toast styles
  toastContainer: {
    position: 'absolute',
    top: 0,
    left: 0,
    right: 0,
    zIndex: 9999,
    paddingHorizontal: 16,
    paddingBottom: 8,
    backgroundColor: 'transparent',
  },
  toastContent: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#FFFFFF',
    borderRadius: 12,
    paddingHorizontal: 14,
    paddingVertical: 12,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 4},
    shadowOpacity: 0.15,
    shadowRadius: 12,
    elevation: 8,
    borderWidth: 1,
    borderColor: 'rgba(0,0,0,0.05)',
  },
  toastIconContainer: {
    width: 32,
    height: 32,
    borderRadius: 16,
    justifyContent: 'center',
    alignItems: 'center',
    marginRight: 10,
  },
  toastMessage: {
    flex: 1,
    fontSize: 14,
    fontWeight: '500',
    color: '#1F2937',
    lineHeight: 20,
  },

  // Confirm styles
  confirmOverlay: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: 'rgba(0,0,0,0.5)',
  },
  confirmBackdrop: {
    ...StyleSheet.absoluteFillObject,
  },
  confirmCard: {
    width: SCREEN_WIDTH - 56,
    backgroundColor: '#FFFFFF',
    borderRadius: 20,
    paddingTop: 28,
    paddingHorizontal: 24,
    paddingBottom: 20,
    shadowColor: '#000',
    shadowOffset: {width: 0, height: 8},
    shadowOpacity: 0.2,
    shadowRadius: 24,
    elevation: 12,
  },
  confirmTitle: {
    fontSize: 18,
    fontWeight: '700',
    color: '#1F2937',
    textAlign: 'center',
    marginBottom: 8,
  },
  confirmMessage: {
    fontSize: 15,
    color: '#6B7280',
    textAlign: 'center',
    lineHeight: 22,
    marginBottom: 24,
  },
  confirmButtons: {
    flexDirection: 'row',
    gap: 10,
  },
  confirmButton: {
    flex: 1,
    paddingVertical: 13,
    borderRadius: 12,
    alignItems: 'center',
    justifyContent: 'center',
  },
  confirmButtonCancel: {
    backgroundColor: '#F3F4F6',
  },
  confirmButtonCancelText: {
    fontSize: 15,
    fontWeight: '600',
    color: '#6B7280',
  },
  confirmButtonConfirmText: {
    fontSize: 15,
    fontWeight: '600',
    color: '#FFFFFF',
  },
});
