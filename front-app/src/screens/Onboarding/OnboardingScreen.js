import React, {useState, useRef} from 'react';
import {
  View,
  Text,
  StyleSheet,
  FlatList,
  Dimensions,
  TouchableOpacity,
  Platform,
  Linking,
  PermissionsAndroid,
} from 'react-native';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import Geolocation from 'react-native-geolocation-service';
import AsyncStorage from '@react-native-async-storage/async-storage';
import {Colors, Fonts} from '../../theme';
import {useToast} from '../../context/ToastContext';

const {width, height} = Dimensions.get('window');

const slides = [
  {
    id: '1',
    icon: 'food',
    title: 'Lezzetli Yemekler',
    description: 'Yakınındaki en iyi restoranlardan sipariş ver, kapına gelsin.',
    color: Colors.primary,
  },
  {
    id: '2',
    icon: 'motorbike',
    title: 'Hızlı Teslimat',
    description: 'Siparişin en kısa sürede, sıcacık kapına ulaşsın.',
    color: Colors.secondary,
  },
  {
    id: '3',
    icon: 'map-marker-radius',
    title: 'Konumunu Paylaş',
    description: 'Yakınındaki restoranları görebilmemiz için konum iznine ihtiyacımız var.',
    color: Colors.info,
    isLocationSlide: true,
  },
];

const OnboardingScreen = ({onComplete}) => {
  const {showConfirm} = useToast();
  const [currentIndex, setCurrentIndex] = useState(0);
  const [locationGranted, setLocationGranted] = useState(false);
  const flatListRef = useRef(null);

  const requestLocationPermission = async () => {
    try {
      if (Platform.OS === 'ios') {
        const status = await Geolocation.requestAuthorization('whenInUse');
        if (status === 'granted') {
          setLocationGranted(true);
          return true;
        }
      } else {
        const result = await PermissionsAndroid.request(
          PermissionsAndroid.PERMISSIONS.ACCESS_FINE_LOCATION,
          {
            title: 'Konum İzni',
            message: 'Yakınındaki restoranları gösterebilmemiz için konum iznine ihtiyacımız var.',
            buttonPositive: 'İzin Ver',
          },
        );
        if (result === PermissionsAndroid.RESULTS.GRANTED) {
          setLocationGranted(true);
          return true;
        }
      }

      showConfirm({
        title: 'Konum İzni Gerekli',
        message: 'Yakınındaki restoranları gösterebilmemiz için konum iznine ihtiyacımız var. Lütfen ayarlardan izin verin.',
        confirmText: 'Ayarlara Git',
        cancelText: 'Tekrar Dene',
        confirmStyle: 'primary',
        onConfirm: () => Linking.openSettings(),
        onCancel: () => requestLocationPermission(),
      });
      return false;
    } catch (e) {
      console.log('Location permission error:', e);
      return false;
    }
  };

  const handleNext = async () => {
    const isLastSlide = currentIndex === slides.length - 1;
    const isLocationSlide = slides[currentIndex].isLocationSlide;

    if (isLocationSlide && !locationGranted) {
      const granted = await requestLocationPermission();
      if (!granted) return;
    }

    if (isLastSlide) {
      await AsyncStorage.setItem('onboarding_completed', 'true');
      onComplete();
    } else {
      flatListRef.current?.scrollToIndex({index: currentIndex + 1});
    }
  };

  const handleSkip = () => {
    flatListRef.current?.scrollToIndex({index: slides.length - 1});
  };

  const onViewableItemsChanged = useRef(({viewableItems}) => {
    if (viewableItems.length > 0) {
      setCurrentIndex(viewableItems[0].index);
    }
  }).current;

  const renderSlide = ({item}) => (
    <View style={styles.slide}>
      <View style={[styles.iconContainer, {backgroundColor: item.color + '15'}]}>
        <Icon name={item.icon} size={80} color={item.color} />
      </View>
      <Text style={styles.title}>{item.title}</Text>
      <Text style={styles.description}>{item.description}</Text>
    </View>
  );

  const renderDots = () => (
    <View style={styles.dotsContainer}>
      {slides.map((_, index) => (
        <View
          key={index}
          style={[
            styles.dot,
            index === currentIndex ? styles.dotActive : styles.dotInactive,
          ]}
        />
      ))}
    </View>
  );

  const isLastSlide = currentIndex === slides.length - 1;
  const isLocationSlide = slides[currentIndex]?.isLocationSlide;

  return (
    <View style={styles.container}>
      <View style={styles.skipContainer}>
        {!isLastSlide && (
          <TouchableOpacity onPress={handleSkip}>
            <Text style={styles.skipText}>Atla</Text>
          </TouchableOpacity>
        )}
      </View>

      <FlatList
        ref={flatListRef}
        data={slides}
        renderItem={renderSlide}
        horizontal
        pagingEnabled
        showsHorizontalScrollIndicator={false}
        keyExtractor={item => item.id}
        onViewableItemsChanged={onViewableItemsChanged}
        viewabilityConfig={{viewAreaCoveragePercentThreshold: 50}}
      />

      {renderDots()}

      <View style={styles.bottomContainer}>
        <TouchableOpacity
          style={[styles.button, isLocationSlide && !locationGranted && styles.buttonLocation]}
          onPress={handleNext}>
          <Text style={styles.buttonText}>
            {isLocationSlide && !locationGranted
              ? 'Konum İzni Ver'
              : isLastSlide
              ? 'Başla'
              : 'Devam'}
          </Text>
          <Icon
            name={isLocationSlide && !locationGranted ? 'map-marker' : 'arrow-right'}
            size={20}
            color="#FFF"
          />
        </TouchableOpacity>
      </View>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: Colors.surface,
  },
  skipContainer: {
    position: 'absolute',
    top: Platform.OS === 'ios' ? 60 : 20,
    right: 20,
    zIndex: 10,
  },
  skipText: {
    fontSize: 16,
    color: Colors.textSecondary,
    fontWeight: '600',
  },
  slide: {
    width,
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    paddingHorizontal: 40,
  },
  iconContainer: {
    width: 160,
    height: 160,
    borderRadius: 80,
    justifyContent: 'center',
    alignItems: 'center',
    marginBottom: 40,
  },
  title: {
    fontSize: 28,
    fontWeight: '700',
    color: Colors.text,
    textAlign: 'center',
    marginBottom: 16,
  },
  description: {
    fontSize: 16,
    color: Colors.textSecondary,
    textAlign: 'center',
    lineHeight: 24,
  },
  dotsContainer: {
    flexDirection: 'row',
    justifyContent: 'center',
    alignItems: 'center',
    marginBottom: 30,
  },
  dot: {
    width: 8,
    height: 8,
    borderRadius: 4,
    marginHorizontal: 4,
  },
  dotActive: {
    backgroundColor: Colors.primary,
    width: 24,
  },
  dotInactive: {
    backgroundColor: Colors.borderLight,
  },
  bottomContainer: {
    paddingHorizontal: 20,
    paddingBottom: Platform.OS === 'ios' ? 50 : 30,
  },
  button: {
    backgroundColor: Colors.primary,
    borderRadius: 16,
    paddingVertical: 16,
    flexDirection: 'row',
    justifyContent: 'center',
    alignItems: 'center',
    gap: 8,
  },
  buttonLocation: {
    backgroundColor: Colors.info,
  },
  buttonText: {
    color: '#FFF',
    fontSize: 18,
    fontWeight: '700',
  },
});

export default OnboardingScreen;
