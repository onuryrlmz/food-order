'use client';

import { useEffect, useState } from 'react';
import SellerLayout from '@/components/layout/SellerLayout';
import { getSellerFinanceSummary, getSellerPayments } from '@/lib/api';

export default function SellerFinancePage() {
  const [summary, setSummary] = useState(null);
  const [payments, setPayments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);

  useEffect(() => {
    loadSummary();
  }, []);

  useEffect(() => {
    loadPayments();
  }, [page]);

  const loadSummary = async () => {
    try {
      const res = await getSellerFinanceSummary();
      if (!res.hasFailed && res.data) {
        setSummary(res.data);
      }
    } catch {
      // Not available yet
    } finally {
      setLoading(false);
    }
  };

  const loadPayments = async () => {
    try {
      const res = await getSellerPayments(page);
      if (!res.hasFailed) {
        setPayments(res.data || []);
        setTotalPages(Math.ceil((res.totalDataCount || 0) / 20) || 1);
      }
    } catch {
      // Not available yet
    }
  };

  const fmt = (val) => {
    if (val == null) return '-';
    return new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(val);
  };

  return (
    <SellerLayout title="Finans">
      <div className="space-y-6">
        {/* Summary Cards */}
        <div className="grid grid-cols-2 lg:grid-cols-4 gap-4">
          {[
            { label: 'Toplam Gelir', value: summary?.totalRevenue, color: 'bg-green-500' },
            { label: 'Komisyon', value: summary?.totalCommission, color: 'bg-orange-500' },
            { label: 'Net Kazanc', value: summary?.totalPayout, color: 'bg-emerald-500' },
            { label: 'Siparis Sayisi', value: summary?.orderCount, color: 'bg-blue-500', isCurrency: false },
          ].map((card) => (
            <div key={card.label} className="bg-white rounded-xl p-5 shadow-sm border border-gray-100">
              <div className={`w-10 h-10 ${card.color} rounded-lg mb-3`} />
              <p className="text-2xl font-bold text-gray-800">
                {loading ? '...' : card.isCurrency === false ? (card.value ?? 0) : fmt(card.value)}
              </p>
              <p className="text-sm font-medium text-gray-600 mt-0.5">{card.label}</p>
            </div>
          ))}
        </div>

        {/* Commission Info */}
        {summary?.commissionRate != null && (
          <div className="bg-gradient-to-r from-emerald-500 to-emerald-600 rounded-xl p-5 text-white">
            <h3 className="font-semibold text-lg">Komisyon Orani</h3>
            <p className="text-emerald-100 text-sm mt-1">
              Mevcut komisyon oraniniz: <span className="font-bold text-white">%{(summary.commissionRate * 100).toFixed(1)}</span>
            </p>
          </div>
        )}

        {/* Payment History */}
        <div className="bg-white rounded-xl border border-gray-100 shadow-sm">
          <div className="p-5 border-b border-gray-100">
            <h2 className="font-semibold text-gray-800">Odeme Gecmisi</h2>
          </div>
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead>
                <tr className="text-left text-xs font-semibold text-gray-400 uppercase tracking-wider">
                  <th className="px-5 py-3">Tarih</th>
                  <th className="px-5 py-3">Siparis</th>
                  <th className="px-5 py-3">Tutar</th>
                  <th className="px-5 py-3">Komisyon</th>
                  <th className="px-5 py-3">Net</th>
                  <th className="px-5 py-3">Durum</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-50">
                {payments.length === 0 ? (
                  <tr>
                    <td colSpan={6} className="px-5 py-10 text-center text-gray-400 text-sm">
                      Henuz odeme verisi bulunmuyor
                    </td>
                  </tr>
                ) : (
                  payments.map((p) => (
                    <tr key={p.id} className="hover:bg-gray-50">
                      <td className="px-5 py-3 text-sm text-gray-600">
                        {new Date(p.createdDate).toLocaleDateString('tr-TR')}
                      </td>
                      <td className="px-5 py-3 text-sm font-mono text-gray-500">
                        {p.orderId?.substring(0, 8)}
                      </td>
                      <td className="px-5 py-3 text-sm text-gray-700">{fmt(p.amount)}</td>
                      <td className="px-5 py-3 text-sm text-orange-600">{fmt(p.commissionAmount)}</td>
                      <td className="px-5 py-3 text-sm text-emerald-600 font-medium">{fmt(p.sellerPayoutAmount)}</td>
                      <td className="px-5 py-3">
                        <span className={`inline-block px-2 py-0.5 rounded-full text-xs font-medium ${
                          p.statusId === 1 ? 'bg-green-100 text-green-700' :
                          p.statusId === 2 ? 'bg-red-100 text-red-700' :
                          'bg-gray-100 text-gray-600'
                        }`}>
                          {p.statusId === 1 ? 'Tamamlandi' : p.statusId === 2 ? 'Iade Edildi' : 'Beklemede'}
                        </span>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
          {totalPages > 1 && (
            <div className="flex justify-center gap-2 p-4 border-t border-gray-100">
              <button
                onClick={() => setPage(p => Math.max(1, p - 1))}
                disabled={page === 1}
                className="px-3 py-1.5 text-sm rounded-lg border border-gray-200 disabled:opacity-40 hover:bg-gray-50"
              >
                Onceki
              </button>
              <span className="text-sm text-gray-500 self-center">{page} / {totalPages}</span>
              <button
                onClick={() => setPage(p => Math.min(totalPages, p + 1))}
                disabled={page === totalPages}
                className="px-3 py-1.5 text-sm rounded-lg border border-gray-200 disabled:opacity-40 hover:bg-gray-50"
              >
                Sonraki
              </button>
            </div>
          )}
        </div>
      </div>
    </SellerLayout>
  );
}
