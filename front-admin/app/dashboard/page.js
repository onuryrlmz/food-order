'use client';

import { useEffect, useState } from 'react';
import AdminLayout from '@/components/layout/AdminLayout';
import StatCard from '@/components/ui/StatCard';
import api from '@/lib/api';

export default function DashboardPage() {
  const [stats, setStats] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    Promise.allSettled([
      api.get('/v1/admin/subscription/plans'),
      api.get('/v1/admin/subscription/all?page=1&pageSize=1'),
      api.get('/v1/admin/seller/list?page=1&pageSize=1'),
      api.get('/v1/admin/restaurant/list?page=1&pageSize=1'),
      api.get('/v1/admin/order/list?page=1&pageSize=1'),
      api.get('/v1/admin/user/list?page=1&pageSize=1'),
    ]).then(([plansRes, subsRes, sellersRes, restaurantsRes, ordersRes, usersRes]) => {
      const cnt = (res, key) => res.status === 'fulfilled' ? (res.value.data?.[key] ?? '?') : '?';

      setStats({
        plans: plansRes.status === 'fulfilled' ? (plansRes.value.data?.data?.length ?? '?') : '?',
        totalSubs: cnt(subsRes, 'totalDataCount'),
        sellers: cnt(sellersRes, 'totalDataCount'),
        restaurants: cnt(restaurantsRes, 'totalDataCount'),
        orders: cnt(ordersRes, 'totalDataCount'),
        users: cnt(usersRes, 'totalDataCount'),
      });
      setLoading(false);
    });
  }, []);

  const v = (key) => loading ? '...' : (stats?.[key] ?? '?');

  const cards = [
    { key: 'plans', title: 'Abonelik Planları', color: 'blue', href: '/subscriptions/plans', icon: 'M9 12l2 2 4-4M7.835 4.697a3.42 3.42 0 001.946-.806 3.42 3.42 0 014.438 0 3.42 3.42 0 001.946.806 3.42 3.42 0 013.138 3.138 3.42 3.42 0 00.806 1.946 3.42 3.42 0 010 4.438 3.42 3.42 0 00-.806 1.946 3.42 3.42 0 01-3.138 3.138 3.42 3.42 0 00-1.946.806 3.42 3.42 0 01-4.438 0 3.42 3.42 0 00-1.946-.806 3.42 3.42 0 01-3.138-3.138 3.42 3.42 0 00-.806-1.946 3.42 3.42 0 010-4.438 3.42 3.42 0 00.806-1.946 3.42 3.42 0 013.138-3.138z' },
    { key: 'totalSubs', title: 'Abonelikler', color: 'green', href: '/subscriptions', icon: 'M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z' },
    { key: 'sellers', title: 'Satıcılar', color: 'orange', href: '/sellers', icon: 'M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4' },
    { key: 'restaurants', title: 'Restoranlar', color: 'purple', href: '/restaurants', icon: 'M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6' },
    { key: 'orders', title: 'Siparişler', color: 'red', href: '/orders', icon: 'M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2' },
    { key: 'users', title: 'Kullanıcılar', color: 'blue', href: '/users', icon: 'M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0z' },
  ];

  return (
    <AdminLayout title="Dashboard">
      <div className="space-y-6">
        <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-6 gap-4">
          {cards.map(card => (
            <a key={card.key} href={card.href} className="block hover:opacity-90 transition-opacity">
              <StatCard
                title={card.title}
                value={v(card.key)}
                color={card.color}
                icon={
                  <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d={card.icon} />
                  </svg>
                }
              />
            </a>
          ))}
        </div>

        <div className="bg-gradient-to-r from-orange-500 to-orange-600 rounded-xl p-5 text-white">
          <h3 className="font-semibold text-lg">Abonelik Kontrolü</h3>
          <p className="text-orange-100 text-sm mt-1">
            Süresi dolmuş abonelikleri kontrol et ve restoranları otomatik pasife al.
          </p>
          <a
            href="/subscriptions"
            className="inline-block mt-3 bg-white text-orange-600 font-semibold text-sm px-4 py-2 rounded-lg hover:bg-orange-50 transition-colors"
          >
            Aboneliklere Git →
          </a>
        </div>
      </div>
    </AdminLayout>
  );
}
