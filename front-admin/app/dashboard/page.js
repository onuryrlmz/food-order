'use client';

import { useEffect, useState } from 'react';
import AdminLayout from '@/components/layout/AdminLayout';
import StatCard from '@/components/ui/StatCard';
import api from '@/lib/api';
import { getAdminAnalytics, getAdminOrderTrends } from '@/lib/api';

function MiniBarChart({ data, valueKey, labelKey, color = '#f97316' }) {
  if (!data || data.length === 0) return <p className="text-sm text-gray-400 text-center py-6">Veri yok</p>;
  const max = Math.max(...data.map(d => d[valueKey] || 0), 1);
  return (
    <div className="flex items-end gap-1 h-36">
      {data.map((d, i) => {
        const pct = ((d[valueKey] || 0) / max) * 100;
        return (
          <div key={i} className="flex-1 flex flex-col items-center gap-1">
            <span className="text-xs text-gray-500 font-medium">{d[valueKey] || 0}</span>
            <div
              className="w-full rounded-t transition-all"
              style={{ height: `${Math.max(pct, 4)}%`, minHeight: 4, backgroundColor: color }}
            />
            <span className="text-[10px] text-gray-400 truncate w-full text-center">{d[labelKey] || ''}</span>
          </div>
        );
      })}
    </div>
  );
}

export default function DashboardPage() {
  const [stats, setStats] = useState(null);
  const [loading, setLoading] = useState(true);
  const [analytics, setAnalytics] = useState(null);
  const [trends, setTrends] = useState([]);

  useEffect(() => {
    Promise.allSettled([
      api.get('/v1/admin/seller/list?page=1&pageSize=1'),
      api.get('/v1/admin/restaurant/list?page=1&pageSize=1'),
      api.get('/v1/admin/order/list?page=1&pageSize=1'),
      api.get('/v1/admin/user/list?page=1&pageSize=1'),
      getAdminAnalytics('month'),
      getAdminOrderTrends('month'),
    ]).then(([sellersRes, restaurantsRes, ordersRes, usersRes, analyticsRes, trendsRes]) => {
      const cnt = (res, key) => res.status === 'fulfilled' ? (res.value.data?.[key] ?? '?') : '?';

      setStats({
        sellers: cnt(sellersRes, 'totalDataCount'),
        restaurants: cnt(restaurantsRes, 'totalDataCount'),
        orders: cnt(ordersRes, 'totalDataCount'),
        users: cnt(usersRes, 'totalDataCount'),
      });
      setAnalytics(analyticsRes.status === 'fulfilled' ? analyticsRes.value?.data : null);
      setTrends(trendsRes.status === 'fulfilled' ? (trendsRes.value?.data || []) : []);
      setLoading(false);
    });
  }, []);

  const v = (key) => loading ? '...' : (stats?.[key] ?? '?');

  const cards = [
    { key: 'sellers', title: 'Saticilar', color: 'orange', href: '/sellers', icon: 'M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4' },
    { key: 'restaurants', title: 'Restoranlar', color: 'purple', href: '/restaurants', icon: 'M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6' },
    { key: 'orders', title: 'Siparisler', color: 'red', href: '/orders', icon: 'M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2' },
    { key: 'users', title: 'Kullanicilar', color: 'blue', href: '/users', icon: 'M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0z' },
  ];

  return (
    <AdminLayout title="Dashboard">
      <div className="space-y-6">
        <div className="grid grid-cols-2 sm:grid-cols-2 lg:grid-cols-4 gap-4">
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

        {/* Analytics Summary */}
        {analytics && (
          <div className="grid grid-cols-2 lg:grid-cols-4 gap-4">
            {[
              { label: 'Aylik Siparis', value: analytics.totalOrders ?? '—', color: 'orange' },
              { label: 'Aylik Gelir', value: analytics.totalRevenue != null ? `₺${Number(analytics.totalRevenue).toFixed(0)}` : '—', color: 'green' },
              { label: 'Ort. Siparis', value: analytics.averageOrderValue != null ? `₺${Number(analytics.averageOrderValue).toFixed(0)}` : '—', color: 'blue' },
              { label: 'Aktif Musteri', value: analytics.uniqueCustomers ?? '—', color: 'purple' },
            ].map(s => (
              <StatCard
                key={s.label}
                title={s.label}
                value={s.value}
                color={s.color}
                icon={
                  <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                  </svg>
                }
              />
            ))}
          </div>
        )}

        {/* Trend Charts */}
        {trends.length > 0 && (
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-5">
              <h3 className="font-semibold text-gray-800 mb-4">Siparis Trendi (Aylik)</h3>
              <MiniBarChart data={trends} valueKey="orderCount" labelKey="date" color="#f97316" />
            </div>
            <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-5">
              <h3 className="font-semibold text-gray-800 mb-4">Gelir Trendi (Aylik)</h3>
              <MiniBarChart data={trends} valueKey="revenue" labelKey="date" color="#10b981" />
            </div>
          </div>
        )}

      </div>
    </AdminLayout>
  );
}
