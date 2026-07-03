import * as signalR from '@microsoft/signalr';
import AsyncStorage from '@react-native-async-storage/async-storage';
import {API_BASE_URL} from './constants';

// Derive base URL without /v1 path for hub connections
const BASE_URL = API_BASE_URL.replace('/v1', '');

export const createCourierConnection = (courierId) => {
  const connection = new signalR.HubConnectionBuilder()
    .withUrl(`${BASE_URL}/hubs/courier`, {
      accessTokenFactory: async () => {
        const token = await AsyncStorage.getItem('auth_token');
        return token;
      },
    })
    .withAutomaticReconnect()
    .build();

  // Yeniden bağlanmada kurye grubuna tekrar katıl; aksi halde soket dönse de kurye
  // yeni atama bildirimlerini almayı sessizce bırakır.
  connection.onreconnected(() => {
    connection.invoke('JoinCourierGroup', courierId).catch(() => {});
  });

  connection.start().then(() => {
    connection.invoke('JoinCourierGroup', courierId);
  }).catch(err => {
    console.warn('SignalR connection error:', err);
  });

  return connection;
};

export const stopCourierConnection = async (connection) => {
  if (connection) {
    try {
      await connection.stop();
    } catch (err) {
      console.warn('SignalR stop error:', err);
    }
  }
};
