import * as signalR from '@microsoft/signalr';
import * as SecureStore from 'expo-secure-store';
import { API_BASE_URL } from './constants';

const BASE_URL = API_BASE_URL.replace('/v1', '');

export const createCourierConnection = (courierId) => {
  const connection = new signalR.HubConnectionBuilder()
    .withUrl(`${BASE_URL}/hubs/courier`, {
      accessTokenFactory: async () => {
        const token = await SecureStore.getItemAsync('auth_token');
        return token;
      },
    })
    .withAutomaticReconnect()
    .build();

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
