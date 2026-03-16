'use client';

import { useState, useEffect } from 'react';
import useSWR from 'swr';
import SellerLayout from '@/components/layout/SellerLayout';
import fetcher from '@/lib/fetcher';
import { getSellerAnalytics, getSellerOrderTrends, getSellerTopProducts } from '@/lib/api';

const PERIODS = [
  { key: 'week', label: 'Bu Hafta' },
  { key: 'month', label: 'Bu Ay' },
  { key: 'year', label: 'Bu Yil' },
];

function MiniBarChart({ data, valueKey, labelKey, color = 'emerald' }) {
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
              className={`w-full rounded-t bg-${color}-500 transition-all`}
              style={{ height: `${Math.max(pct, 4)}%`, minHeight: 4, backgroundColor: color === 'emerald' ? '#10b981' : '#3b82f6' }}
            />
            <span className="text-[10px] text-gray-400 truncate w-full text-center">{d[labelKey] || ''}</span>
          </div>
        );
      })}
    </div>
  );
}

export default function AnalyticsPage() {
  const { data: restaurantsData } = useSWR('/v1/seller/restaurant/list', fetcher);
  const restaurants = restaurantsData?.data || [];

  const [selectedRestaurant, setSelectedRestaurant] = useState('');
  const [period, setPeriod] = useState('month');
  const [summary, setSummary] = useState(null);
  const [trends, setTrends] = useState([]);
  const [topProducts, setTopProducts] = useState([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (restaurants.length > 0 && !selectedRestaurant) {
      setSelectedRestaurant(restaurants[0].id);
    }
  }, [restaurants, selectedRestaurant]);

  useEffect(() => {
    if (!selectedRestaurant) return;
    setLoading(true);
    Promise.allSettled([
      getSellerAnalytics(selectedRestaurant, period),
      getSellerOrderTrends(selectedRestaurant, period),
      getSellerTopProducts(selectedRestaurant, period),
    ]).then(([summaryRes, trendsRes, productsRes]) => {
      setSummary(summaryRes.status === 'fulfilled' ? summaryRes.value?.data : null);
      setTrends(trendsRes.status === 'fulfilled' ? (trendsRes.value?.data || []) : []);
      setTopProducts(productsRes.status === 'fulfilled' ? (productsRes.value?.data || []) : []);
      setLoading(false);
    });
  }, [selectedRestaurant, period]);

  const summaryCards = [
    { label: 'Toplam Siparis', value: summary?.totalOrders ?? '—', color: 'bg-blue-500' },
    { label: 'Toplam Gelir', value: summary?.totalRevenue != null ? `₺${Number(summary.totalRevenue).toFixed(2)}` : '—', color: 'bg-emerald-500' },
    { label: 'Ort. Siparis Tutari', value: summary?.avgOrderValue != null ? `₺${Number(summary.avgOrderValue).toFixed(2)}` : '—', color: 'bg-purple-500' },
    { label: 'Tekil Musteri', value: summary?.uniqueCustomers ?? '—', color: 'bg-orange-500' },
  ];

  return (
    <SellerLayout title="Analitik">
      <div className="space-y-6">
        {/* Filters */}
        <div className="flex items-center gap-4 flex-wrap">
          <select
            value={selectedRestaurant}
            onChange={(e) => setSelectedRestaurant(e.target.value)}
            className="border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-emerald-400"
          >
            {restaurants.map(r => (
              <option key={r.id} value={r.id}>{r.name}</option>
            ))}
          </select>
          <div className="flex gap-1">
            {PERIODS.map(p => (
              <button
                key={p.key}
                onClick={() => setPeriod(p.key)}
                className={`px-3 py-1.5 rounded-lg text-xs font-medium transition-colors ${
                  period === p.key
                    ? 'bg-emerald-500 text-white'
                    : 'bg-white border border-gray-200 text-gray-600 hover:bg-gray-50'
                }`}
              >
                {p.label}
              </button>
            ))}
          </div>
        </div>

        {loading ? (
          <div className="flex justify-center py-12">
            <div className="w-8 h-8 border-2 border-emerald-400 border-t-transparent rounded-full animate-spin" />
          </div>
        ) : (
          <>
            {/* Summary Cards */}
            <div className="grid grid-cols-2 lg:grid-cols-4 gap-4">
              {summaryCards.map(s => (
                <div key={s.label} className="bg-white rounded-xl p-5 shadow-sm border border-gray-100">
                  <div className={`w-10 h-10 ${s.color} rounded-lg mb-3`} />
                  <p className="text-2xl font-bold text-gray-800">{s.value}</p>
                  <p className="text-sm font-medium text-gray-600 mt-0.5">{s.label}</p>
                </div>
              ))}
            </div>

            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
              {/* Order Trend Chart */}
              <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-5">
                <h2 className="font-semibold text-gray-800 mb-4">Siparis Trendi</h2>
                <MiniBarChart data={trends} valueKey="orderCount" labelKey="label" color="emerald" />
              </div>

              {/* Revenue Trend Chart */}
              <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-5">
                <h2 className="font-semibold text-gray-800 mb-4">Gelir Trendi</h2>
                <MiniBarChart data={trends} valueKey="revenue" labelKey="label" color="blue" />
              </div>
            </div>

            {/* Top Products */}
            <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-5">
              <h2 className="font-semibold text-gray-800 mb-4">En Cok Satan Urunler</h2>
              {topProducts.length === 0 ? (
                <p className="text-sm text-gray-400 text-center py-6">Veri yok</p>
              ) : (
                <div className="overflow-x-auto">
                  <table className="w-full text-sm">
                    <thead className="bg-gray-50 text-xs text-gray-500 uppercase">
                      <tr>
                        <th className="px-4 py-3 text-left">#</th>
                        <th className="px-4 py-3 text-left">Urun</th>
                        <th className="px-4 py-3 text-right">Adet</th>
                        <th className="px-4 py-3 text-right">Gelir</th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-gray-50">
                      {topProducts.map((p, i) => (
                        <tr key={p.productId || i} className="hover:bg-gray-50">
                          <td className="px-4 py-3 text-gray-400 font-medium">{i + 1}</td>
                          <td className="px-4 py-3 font-medium text-gray-800">{p.productName}</td>
                          <td className="px-4 py-3 text-right">{p.totalQuantity}</td>
                          <td className="px-4 py-3 text-right font-semibold text-emerald-600">₺{Number(p.totalRevenue || 0).toFixed(2)}</td>
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
    </SellerLayout>
  );
}
