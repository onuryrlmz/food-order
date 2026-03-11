import './globals.css';
import { ToastProvider } from '@/components/ui/Toast';

export const metadata = { title: 'FoodOrder Satıcı', description: 'Satıcı Paneli' };

export default function RootLayout({ children }) {
  return (
    <html lang="tr">
      <body className="antialiased">
        <ToastProvider>{children}</ToastProvider>
      </body>
    </html>
  );
}
