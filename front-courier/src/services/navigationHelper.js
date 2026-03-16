import {Linking, Platform} from 'react-native';

export const openNavigation = (lat, lng) => {
  const url = Platform.select({
    ios: `maps://app?daddr=${lat},${lng}`,
    android: `google.navigation:q=${lat},${lng}`,
  });
  Linking.openURL(url);
};
