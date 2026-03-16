import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';

const BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL || 'http://localhost:3762';

export const createRestaurantConnection = (restaurantId) => {
  const connection = new HubConnectionBuilder()
    .withUrl(`${BASE_URL}/hubs/restaurant`, { withCredentials: true })
    .withAutomaticReconnect()
    .configureLogging(LogLevel.Warning)
    .build();

  connection.start().then(() => {
    connection.invoke('JoinRestaurantGroup', restaurantId).catch(() => {});
  }).catch(() => {});

  return connection;
};

export const createOrderConnection = (orderId) => {
  const connection = new HubConnectionBuilder()
    .withUrl(`${BASE_URL}/hubs/order`, { withCredentials: true })
    .withAutomaticReconnect()
    .configureLogging(LogLevel.Warning)
    .build();

  connection.start().then(() => {
    connection.invoke('JoinOrderGroup', orderId).catch(() => {});
  }).catch(() => {});

  return connection;
};
