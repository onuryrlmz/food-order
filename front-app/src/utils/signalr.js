import {HubConnectionBuilder, LogLevel} from '@microsoft/signalr';
import AsyncStorage from '@react-native-async-storage/async-storage';
import {API_BASE_URL} from './constants';

const BASE_URL = API_BASE_URL.replace('/v1', '');

export const createOrderConnection = async (orderId) => {
  const connection = new HubConnectionBuilder()
    .withUrl(`${BASE_URL}/hubs/order`, {
      // Her (yeniden) bağlantıda güncel token'ı oku — tek seferlik yakalanan token
      // yeniden bağlanmada bayatlar.
      accessTokenFactory: async () => (await AsyncStorage.getItem('auth_token')) || '',
    })
    .withAutomaticReconnect()
    .configureLogging(LogLevel.Warning)
    .build();

  // Otomatik yeniden bağlanmada sipariş grubuna tekrar katıl; aksi halde soket yeniden
  // bağlansa da istemci gruptan düşer ve durum güncellemeleri sessizce durur.
  connection.onreconnected(() => {
    connection.invoke('JoinOrderGroup', orderId).catch(() => {});
  });

  try {
    await connection.start();
    await connection.invoke('JoinOrderGroup', orderId);
  } catch (err) {
    console.log('SignalR connection error:', err);
  }

  return connection;
};
