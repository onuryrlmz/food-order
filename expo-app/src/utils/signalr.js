import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import * as SecureStore from 'expo-secure-store';
import { API_BASE_URL } from './constants';

const BASE_URL = API_BASE_URL.replace('/v1', '');

export const createOrderConnection = async (orderId) => {
  const token = await SecureStore.getItemAsync('auth_token');

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
