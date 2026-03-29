import { createService } from '../../service';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { API_BASE_URL } from './utils/constants';

let _onAuthReset = null;
export const setOnAuthReset = (fn) => { _onAuthReset = fn; };

const api = createService({
  baseURL: API_BASE_URL,
  getToken: () => AsyncStorage.getItem('auth_token'),
  onTokenReceived: (key, value) => AsyncStorage.setItem(key, value),
  onUnauthorized: async () => {
    await AsyncStorage.multiRemove(['auth_token', 'refresh_token']);
    if (_onAuthReset) _onAuthReset();
  },
});

export default api;
