'use client';

import { useEffect, useState } from 'react';
import AdminLayout from '@/components/layout/AdminLayout';
import StatCard from '@/components/ui/StatCard';
import { getFinanceSummary, getSellerFinance, updateCommissionRate } from '@/lib/api';

export default function FinancePage() {
  const [summary, setSummary] = useState(null);
  const [sellers, setSellers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [commissionRate, setCommissionRate] = useState('');
  const [savingRate, setSavingRate] = useState(false);
  const [rateSuccess, setRateSuccess] = useState('');

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      const res = await getFinanceSummary();
      if (!res.hasFailed && res.data) {
        setSummary(res.data);
        setCommissionRate(String((res.data.commissionRate || 0.1) * 100));
        if (res.data.sellers) {
          setSellers(res.data.sellers);
        }
      }
    } catch {
      // Finance data not available yet
    } finally {
      setLoading(false);
    }
  };

  const handleUpdateRate = async () => {
    const rate = parseFloat(commissionRate) / 100;
    if (isNaN(rate) || rate < 0 || rate > 1) return;
    setSavingRate(true);
    setRateSuccess('');
    try {
      const res = await updateCommissionRate(rate);
      if (!res.hasFailed) {
        setRateSuccess('Komisyon orani guncellendi');
        setTimeout(() => setRateSuccess(''), 3000);
      }
    } catch {
      // Error handling
    } finally {
      setSavingRate(false);
    }
  };

  const fmt = (val) => {
    if (val == null) return '...';
    return new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(val);
  };

  return (
    <AdminLayout title="Finans">
      <div className="space-y-6">
        {/* Summary Cards */}
        <div className="grid grid-cols-2 lg:grid-cols-4 gap-4">
          <StatCard
            title="Toplam Gelir"
            value={loading ? '...' : fmt(summary?.totalRevenue)}
            color="green"
            icon={
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            }
          />
          <StatCard
            title="Toplam Komisyon"
            value={loading ? '...' : fmt(summary?.totalCommission)}
            color="orange"
            icon={
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 14l6-6m-5.5.5h.01m4.99 5h.01M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16l3.5-2 3.5 2 3.5-2 3.5 2z" />
              </svg>
            }
          />
          <StatCard
            title="Toplam Payout"
            value={loading ? '...' : fmt(summary?.totalPayout)}
            color="blue"
            icon={
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 9V7a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2m2 4h10a2 2 0 002-2v-6a2 2 0 00-2-2H9a2 2 0 00-2 2v6a2 2 0 002 2zm7-5a2 2 0 11-4 0 2 2 0 014 0z" />
              </svg>
            }
          />
          <StatCard
            title="Siparis Sayisi"
            value={loading ? '...' : (summary?.orderCount ?? 0)}
            color="purple"
            icon={
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
              </svg>
            }
          />
        </div>

        {/* Commission Rate Setting */}
        <div className="bg-white rounded-xl border border-gray-200 p-6">
          <h2 className="text-lg font-semibold text-gray-800 mb-4">Komisyon Orani Ayari</h2>
          <div className="flex items-end gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-600 mb-1">Komisyon Orani (%)</label>
              <input
                type="number"
                min="0"
                max="100"
                step="0.1"
                value={commissionRate}
                onChange={(e) => setCommissionRate(e.target.value)}
                className="border border-gray-300 rounded-lg px-4 py-2 text-sm w-32 outline-none focus:ring-2 focus:ring-orange-400"
              />
            </div>
            <button
              onClick={handleUpdateRate}
              disabled={savingRate}
              className="bg-orange-500 hover:bg-orange-600 text-white px-4 py-2 rounded-lg text-sm font-semibold disabled:opacity-60"
            >
              {savingRate ? 'Kaydediliyor...' : 'Kaydet'}
            </button>
            {rateSuccess && <span className="text-sm text-green-600 font-medium">{rateSuccess}</span>}
          </div>
        </div>

        {/* Seller Payment Table */}
        <div className="bg-white rounded-xl border border-gray-200">
          <div className="p-5 border-b border-gray-100">
            <h2 className="text-lg font-semibold text-gray-800">Satici Finanslari</h2>
          </div>
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead>
                <tr className="text-left text-xs font-semibold text-gray-400 uppercase tracking-wider">
                  <th className="px-5 py-3">Satici</th>
                  <th className="px-5 py-3">Toplam Gelir</th>
                  <th className="px-5 py-3">Komisyon</th>
                  <th className="px-5 py-3">Payout</th>
                  <th className="px-5 py-3">Siparis</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-50">
                {loading ? (
                  <tr>
                    <td colSpan={5} className="px-5 py-10 text-center text-gray-400 text-sm">Yukleniyor...</td>
                  </tr>
                ) : sellers.length === 0 ? (
                  <tr>
                    <td colSpan={5} className="px-5 py-10 text-center text-gray-400 text-sm">
                      Henuz finans verisi bulunmuyor
                    </td>
                  </tr>
                ) : (
                  sellers.map((seller) => (
                    <tr key={seller.sellerId} className="hover:bg-gray-50">
                      <td className="px-5 py-3">
                        <p className="text-sm font-medium text-gray-800">{seller.sellerName || seller.email}</p>
                      </td>
                      <td className="px-5 py-3 text-sm text-gray-700">{fmt(seller.totalRevenue)}</td>
                      <td className="px-5 py-3 text-sm text-orange-600 font-medium">{fmt(seller.totalCommission)}</td>
                      <td className="px-5 py-3 text-sm text-blue-600 font-medium">{fmt(seller.totalPayout)}</td>
                      <td className="px-5 py-3 text-sm text-gray-600">{seller.orderCount ?? 0}</td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </AdminLayout>
  );
}
