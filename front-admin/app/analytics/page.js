'use client';

import { useState, useEffect } from 'react';
import AdminLayout from '@/components/layout/AdminLayout';
import StatCard from '@/components/ui/StatCard';
import { getAdminAnalytics, getAdminOrderTrends, getAdminTopRestaurants } from '@/lib/api';

const PERIODS = [
  { key: 'week', label: 'Bu Hafta' },
  { key: 'month', label: 'Bu Ay' },
  { key: 'year', label: 'Bu Yil' },
];

function MiniBarChart({ data, valueKey, labelKey, color = '#f97316' }) {
  if (!data || data.length === 0) return <p className="text-sm text-gray-400 text-center py-6">Veri yok</p>;
  const max = Math.max(...data.map(d => d[valueKey] || 0), 1);
  return (
    <div className="flex items-end gap-1 h-40">
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

export default function AnalyticsPage() {
  const [period, setPeriod] = useState('month');
  const [summary, setSummary] = useState(null);
  const [trends, setTrends] = useState([]);
  const [topRestaurants, setTopRestaurants] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    setLoading(true);
    Promise.allSettled([
      getAdminAnalytics(period),
      getAdminOrderTrends(period),
      getAdminTopRestaurants(period),
    ]).then(([summaryRes, trendsRes, restaurantsRes]) => {
      setSummary(summaryRes.status === 'fulfilled' ? summaryRes.value?.data : null);
      setTrends(trendsRes.status === 'fulfilled' ? (trendsRes.value?.data || []) : []);
      setTopRestaurants(restaurantsRes.status === 'fulfilled' ? (restaurantsRes.value?.data || []) : []);
      setLoading(false);
    });
  }, [period]);

  const summaryCards = [
    { label: 'Toplam Siparis', value: summary?.totalOrders ?? '—', color: 'orange', icon: 'M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2' },
    { label: 'Toplam Gelir', value: summary?.totalRevenue != null ? `₺${Number(summary.totalRevenue).toFixed(2)}` : '—', color: 'green', icon: 'M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z' },
    { label: 'Ort. Siparis Tutari', value: summary?.avgOrderValue != null ? `₺${Number(summary.avgOrderValue).toFixed(2)}` : '—', color: 'blue', icon: 'M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z' },
    { label: 'Tekil Musteri', value: summary?.uniqueCustomers ?? '—', color: 'purple', icon: 'M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0z' },
  ];

  return (
    <AdminLayout title="Analitik">
      <div className="space-y-6">
        {/* Period Filter */}
        <div className="flex gap-1">
          {PERIODS.map(p => (
            <button
              key={p.key}
              onClick={() => setPeriod(p.key)}
              className={`px-3 py-1.5 rounded-lg text-xs font-medium transition-colors ${
                period === p.key
                  ? 'bg-orange-500 text-white'
                  : 'bg-white border border-gray-200 text-gray-600 hover:bg-gray-50'
              }`}
            >
              {p.label}
            </button>
          ))}
        </div>

        {loading ? (
          <div className="flex justify-center py-12">
            <div className="w-8 h-8 border-2 border-orange-400 border-t-transparent rounded-full animate-spin" />
          </div>
        ) : (
          <>
            {/* Summary Cards */}
            <div className="grid grid-cols-2 lg:grid-cols-4 gap-4">
              {summaryCards.map(s => (
                <StatCard
                  key={s.label}
                  title={s.label}
                  value={s.value}
                  color={s.color}
                  icon={
                    <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d={s.icon} />
                    </svg>
                  }
                />
              ))}
            </div>

            {/* Trend Charts */}
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
              <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-5">
                <h3 className="font-semibold text-gray-800 mb-4">Siparis Trendi</h3>
                <MiniBarChart data={trends} valueKey="orderCount" labelKey="label" color="#f97316" />
              </div>
              <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-5">
                <h3 className="font-semibold text-gray-800 mb-4">Gelir Trendi</h3>
                <MiniBarChart data={trends} valueKey="revenue" labelKey="label" color="#10b981" />
              </div>
            </div>

            {/* Top Restaurants */}
            <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-5">
              <h3 className="font-semibold text-gray-800 mb-4">En Cok Siparis Alan Restoranlar</h3>
              {topRestaurants.length === 0 ? (
                <p className="text-sm text-gray-400 text-center py-6">Veri yok</p>
              ) : (
                <div className="overflow-x-auto">
                  <table className="w-full text-sm">
                    <thead className="bg-gray-50 text-xs text-gray-500 uppercase">
                      <tr>
                        <th className="px-4 py-3 text-left">#</th>
                        <th className="px-4 py-3 text-left">Restoran</th>
                        <th className="px-4 py-3 text-right">Siparis</th>
                        <th className="px-4 py-3 text-right">Gelir</th>
                        <th className="px-4 py-3 text-right">Ort. Puan</th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-gray-50">
                      {topRestaurants.map((r, i) => (
                        <tr key={r.restaurantId || i} className="hover:bg-gray-50">
                          <td className="px-4 py-3 text-gray-400 font-medium">{i + 1}</td>
                          <td className="px-4 py-3 font-medium text-gray-800">{r.restaurantName}</td>
                          <td className="px-4 py-3 text-right">{r.totalOrders}</td>
                          <td className="px-4 py-3 text-right font-semibold text-orange-600">₺{Number(r.totalRevenue || 0).toFixed(2)}</td>
                          <td className="px-4 py-3 text-right">
                            <span className="inline-flex items-center gap-1">
                              <svg className="w-3.5 h-3.5 text-yellow-400" fill="currentColor" viewBox="0 0 20 20">
                                <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
                              </svg>
                              {r.avgRating != null ? Number(r.avgRating).toFixed(1) : '—'}
                            </span>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </div>
          </>
        )}
      </div>
    </AdminLayout>
  );
}
