import {HubConnectionBuilder, LogLevel} from '@microsoft/signalr';
import AsyncStorage from '@react-native-async-storage/async-storage';
import {API_BASE_URL} from './constants';

const BASE_URL = API_BASE_URL.replace('/v1', '');

export const createOrderConnection = async (orderId) => {
  const token = await AsyncStorage.getItem('auth_token');

  const connection = new HubConnectionBuilder()
    .withUrl(`${BASE_URL}/hubs/order`, {
      accessTokenFactory: () => token,
    })
    .withAutomaticReconnect()
    .configureLogging(LogLevel.Warning)
    .build();

  try {
    await connection.start();
    await connection.invoke('JoinOrderGroup', orderId);
  } catch (err) {
    console.log('SignalR connection error:', err);
  }

  return connection;
};
