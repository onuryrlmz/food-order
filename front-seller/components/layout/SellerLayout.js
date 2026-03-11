'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { isAuthenticated } from '@/lib/auth';
import Sidebar from './Sidebar';
import Header from './Header';

export default function SellerLayout({ children, title, headerActions }) {
  const router = useRouter();
  const [ready, setReady] = useState(false);

  useEffect(() => {
    if (!isAuthenticated()) {
      router.replace('/login');
    } else {
      setReady(true);
    }
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  if (!ready) return null;

  return (
    <div className="flex min-h-screen bg-gray-50">
      <Sidebar />
      <div className="flex-1 ml-64 flex flex-col min-h-screen">
        <Header title={title}>{headerActions}</Header>
        <main className="flex-1 p-6">{children}</main>
      </div>
    </div>
  );
}
