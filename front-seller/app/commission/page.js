'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import useSWR from 'swr';
import SellerLayout from '@/components/layout/SellerLayout';
import fetcher from '@/lib/fetcher';

const STATUS_MAP = {
  1: { label: 'Beklemede', color: 'bg-yellow-100 text-yellow-700' },
  2: { label: 'Onaylandi', color: 'bg-blue-100 text-blue-700' },
  3: { label: 'Odendi', color: 'bg-green-100 text-green-700' },
  4: { label: 'Iptal', color: 'bg-gray-100 text-gray-600' },
};

export default function SellerCommissionPage() {
  const router = useRouter();
  const [page, setPage] = useState(1);
  const [selectedPeriod, setSelectedPeriod] = useState(null);

  const { data: commData, isLoading: commLoading } = useSWR('/v1/seller/commission/my', fetcher);
  const { data: periodsData, isLoading: periodsLoading } = useSWR(
    `/v1/seller/commission/settlement/periods?page=${page}&pageSize=20`,
    fetcher
  );
  const { data: periodDetailData } = useSWR(
    selectedPeriod ? `/v1/seller/commission/settlement/period/${selectedPeriod}` : null,
    fetcher
  );

  const commissions = commData?.data || [];
  const periods = periodsData?.data || [];
  const total = periodsData?.totalDataCount || 0;
  const totalPages = Math.ceil(total / 20) || 1;
  const periodDetail = periodDetailData?.data;
  const detailItems = periodDetail?.items || [];

  const fmt = (val) => {
    if (val == null) return '-';
    return new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(val);
  };

  const pct = (val) => {
    if (val == null) return '-';
    return `%${(val * 100).toFixed(1)}`;
  };

  return (
    <SellerLayout title="Komisyon">
      <div className="space-y-6">
        {/* Commission Info Cards */}
        {commLoading ? (
          <div className="flex justify-center py-8">
            <div className="w-8 h-8 border-2 border-emerald-400 border-t-transparent rounded-full animate-spin" />
          </div>
        ) : commissions.length === 0 ? (
          <div className="bg-gradient-to-r from-emerald-500 to-emerald-600 rounded-xl p-5 text-white">
            <h3 className="font-semibold text-lg">Komisyon Bilgisi</h3>
            <p className="text-emerald-100 text-sm mt-1">Platform varsayilani uygulanıyor</p>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {(Array.isArray(commissions) ? commissions : [commissions]).map((c, i) => (
              <div key={c.restaurantId || i} className="bg-white rounded-xl p-5 shadow-sm border border-gray-100">
                <h3 className="font-semibold text-gray-800 mb-3">{c.restaurantName || `Restoran ${i + 1}`}</h3>
                <div className="grid grid-cols-3 gap-3">
                  <div>
                    <p className="text-xs text-gray-500">Komisyon Orani</p>
                    <p className="text-lg font-bold text-gray-800">{pct(c.commissionRate)}</p>
                  </div>
                  <div>
                    <p className="text-xs text-gray-500">Sabit Ucret</p>
                    <p className="text-lg font-bold text-gray-800">{fmt(c.fixedFee)}</p>
                  </div>
                  <div>
                    <p className="text-xs text-gray-500">Gecerlilik</p>
                    <p className="text-lg font-bold text-gray-800">
                      {c.effectiveFrom ? new Date(c.effectiveFrom).toLocaleDateString('tr-TR') : '-'}
                    </p>
                  </div>
                </div>
                {c.isCustom === false && (
                  <p className="text-xs text-emerald-600 mt-2">Platform varsayilani uygulanıyor</p>
                )}
              </div>
            ))}
          </div>
        )}

        {/* Settlement Periods */}
        <div className="bg-white rounded-xl border border-gray-100 shadow-sm">
          <div className="p-5 border-b border-gray-100">
            <h2 className="font-semibold text-gray-800">Hakedis Donemleri</h2>
          </div>
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead>
                <tr className="text-left text-xs font-semibold text-gray-400 uppercase tracking-wider">
                  <th className="px-5 py-3">Tarih</th>
                  <th className="px-5 py-3">Siparis Sayisi</th>
                  <th className="px-5 py-3">Toplam Tutar</th>
                  <th className="px-5 py-3">Komisyon</th>
                  <th className="px-5 py-3">Net Odeme</th>
                  <th className="px-5 py-3">Durum</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-50">
                {periodsLoading ? (
                  <tr>
                    <td colSpan={6} className="px-5 py-10 text-center">
                      <div className="flex justify-center">
                        <div className="w-6 h-6 border-2 border-emerald-400 border-t-transparent rounded-full animate-spin" />
                      </div>
                    </td>
                  </tr>
                ) : periods.length === 0 ? (
                  <tr>
                    <td colSpan={6} className="px-5 py-10 text-center text-gray-400 text-sm">
                      Henuz hakedis donemi bulunmuyor
                    </td>
                  </tr>
                ) : (
                  periods.map((p) => {
                    const status = STATUS_MAP[p.statusId] || { label: '?', color: 'bg-gray-100 text-gray-600' };
                    return (
                      <tr
                        key={p.id}
                        onClick={() => setSelectedPeriod(selectedPeriod === p.id ? null : p.id)}
                        className={`hover:bg-gray-50 cursor-pointer ${selectedPeriod === p.id ? 'bg-emerald-50' : ''}`}
                      >
                        <td className="px-5 py-3 text-sm text-gray-600">
                          {p.periodStart ? new Date(p.periodStart).toLocaleDateString('tr-TR') : '-'}
                          {p.periodEnd ? ` - ${new Date(p.periodEnd).toLocaleDateString('tr-TR')}` : ''}
                        </td>
                        <td className="px-5 py-3 text-sm text-gray-700">{p.orderCount ?? '-'}</td>
                        <td className="px-5 py-3 text-sm text-gray-700">{fmt(p.totalAmount)}</td>
                        <td className="px-5 py-3 text-sm text-orange-600">{fmt(p.totalCommission)}</td>
                        <td className="px-5 py-3 text-sm text-emerald-600 font-medium">{fmt(p.netAmount)}</td>
                        <td className="px-5 py-3">
                          <span className={`inline-block px-2 py-0.5 rounded-full text-xs font-medium ${status.color}`}>
                            {status.label}
                          </span>
                        </td>
                      </tr>
                    );
                  })
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

        {/* Period Detail (inline) */}
        {selectedPeriod && periodDetail && (
          <div className="bg-white rounded-xl border border-gray-100 shadow-sm">
            <div className="p-5 border-b border-gray-100">
              <div className="flex items-center justify-between">
                <h2 className="font-semibold text-gray-800">Donem Detayi</h2>
                <button
                  onClick={() => setSelectedPeriod(null)}
                  className="text-sm text-gray-400 hover:text-gray-600"
                >
                  Kapat
                </button>
              </div>
              <div className="flex items-center gap-4 mt-2 text-sm text-gray-500">
                <span>
                  {periodDetail.periodStart ? new Date(periodDetail.periodStart).toLocaleDateString('tr-TR') : '-'}
                  {periodDetail.periodEnd ? ` - ${new Date(periodDetail.periodEnd).toLocaleDateString('tr-TR')}` : ''}
                </span>
                <span>{periodDetail.orderCount ?? 0} siparis</span>
                <span>Net: {fmt(periodDetail.netAmount)}</span>
              </div>
            </div>
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead>
                  <tr className="text-left text-xs font-semibold text-gray-400 uppercase tracking-wider">
                    <th className="px-5 py-3">Siparis ID</th>
                    <th className="px-5 py-3">Tutar</th>
                    <th className="px-5 py-3">Komisyon Orani</th>
                    <th className="px-5 py-3">Komisyon</th>
                    <th className="px-5 py-3">Sabit Ucret</th>
                    <th className="px-5 py-3">Net</th>
                    <th className="px-5 py-3">Tarih</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-50">
                  {detailItems.length === 0 ? (
                    <tr>
                      <td colSpan={7} className="px-5 py-10 text-center text-gray-400 text-sm">
                        Kalem bulunamadi
                      </td>
                    </tr>
                  ) : (
                    detailItems.map((item, i) => (
                      <tr key={item.id || i} className="hover:bg-gray-50">
                        <td className="px-5 py-3 text-sm font-mono text-gray-500">
                          {item.orderId ? String(item.orderId).slice(0, 8) + '...' : '-'}
                        </td>
                        <td className="px-5 py-3 text-sm text-gray-700">{fmt(item.orderAmount)}</td>
                        <td className="px-5 py-3 text-sm text-gray-600">{pct(item.commissionRate)}</td>
                        <td className="px-5 py-3 text-sm text-orange-600">{fmt(item.commissionAmount)}</td>
                        <td className="px-5 py-3 text-sm text-gray-600">{fmt(item.fixedFeeAmount)}</td>
                        <td className="px-5 py-3 text-sm text-emerald-600 font-medium">{fmt(item.netAmount)}</td>
                        <td className="px-5 py-3 text-sm text-gray-600">
                          {item.orderDate ? new Date(item.orderDate).toLocaleString('tr-TR') : '-'}
                        </td>
                      </tr>
                    ))
                  )}
                </tbody>
              </table>
            </div>
          </div>
        )}
      </div>
    </SellerLayout>
  );
}
