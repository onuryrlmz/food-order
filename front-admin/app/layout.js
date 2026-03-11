import "./globals.css";
import { ToastProvider } from '@/components/ui/Toast';

export const metadata = {
  title: "FoodOrder Admin",
  description: "FoodOrder yönetim paneli",
};

export default function RootLayout({ children }) {
  return (
    <html lang="tr">
      <body>
        <ToastProvider>
          {children}
        </ToastProvider>
      </body>
    </html>
  );
}
