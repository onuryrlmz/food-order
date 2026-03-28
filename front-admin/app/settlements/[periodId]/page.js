'use client';

import { useState } from 'react';
import { useParams, useRouter } from 'next/navigation';
import useSWR from 'swr';
import AdminLayout from '@/components/layout/AdminLayout';
import Badge from '@/components/ui/Badge';
import Button from '@/components/ui/Button';
import StatCard from '@/components/ui/StatCard';
import { useToast } from '@/components/ui/Toast';
import fetcher from '@/lib/fetcher';
import { approveSettlement, paySettlement, cancelSettlement } from '@/lib/api';

const STATUS_MAP = {
  1: { label: 'Beklemede', color: 'yellow' },
  2: { label: 'Onaylandi', color: 'blue' },
  3: { label: 'Odendi', color: 'green' },
  4: { label: 'Iptal', color: 'gray' },
};

export default function SettlementDetailPage() {
  const { periodId } = useParams();
  const router = useRouter();
  const toast = useToast();
  const [actionLoading, setActionLoading] = useState(false);
  const [payForm, setPayForm] = useState({ bankTransferRef: '', notes: '' });
  const [showPayForm, setShowPayForm] = useState(false);

  const { data, isLoading, mutate } = useSWR(`/v1/admin/settlement/period/${periodId}`, fetcher);
  const period = data?.data;
  const items = period?.items || [];

  const fmt = (val) => {
    if (val == null) return '-';
    return new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(val);
  };

  const pct = (val) => {
    if (val == null) return '-';
    return `%${(val * 100).toFixed(1)}`;
  };

  const status = period ? (STATUS_MAP[period.statusId] || { label: '?', color: 'gray' }) : null;

  const handleApprove = async () => {
    if (!confirm('Bu hakedisi onaylamak istediginize emin misiniz?')) return;
    setActionLoading(true);
    try {
      await approveSettlement(periodId);
      toast('Hakedis onaylandi', 'success');
      mutate();
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || err.message || 'Hata olustu', 'error');
    } finally {
      setActionLoading(false);
    }
  };

  const handlePay = async (e) => {
    e.preventDefault();
    setActionLoading(true);
    try {
      await paySettlement(periodId, {
        bankTransferRef: payForm.bankTransferRef,
        notes: payForm.notes,
      });
      toast('Odeme tamamlandi', 'success');
      setShowPayForm(false);
      setPayForm({ bankTransferRef: '', notes: '' });
      mutate();
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || err.message || 'Hata olustu', 'error');
    } finally {
      setActionLoading(false);
    }
  };

  const handleCancel = async () => {
    const reason = prompt('Iptal sebebi:');
    if (reason === null) return;
    setActionLoading(true);
    try {
      await cancelSettlement(periodId, reason);
      toast('Hakedis iptal edildi', 'success');
      mutate();
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || err.message || 'Hata olustu', 'error');
    } finally {
      setActionLoading(false);
    }
  };

  if (isLoading) {
    return (
      <AdminLayout title="Hakedis Detay">
        <div className="flex justify-center py-12">
          <div className="w-8 h-8 border-2 border-orange-400 border-t-transparent rounded-full animate-spin" />
        </div>
      </AdminLayout>
    );
  }

  if (!period) {
    return (
      <AdminLayout title="Hakedis Detay">
        <div className="bg-white rounded-xl border border-gray-200 p-10 text-center text-gray-400 text-sm">
          Hakedis bulunamadi
        </div>
      </AdminLayout>
    );
  }

  return (
    <AdminLayout title="Hakedis Detay">
      <div className="space-y-6">
        {/* Back button */}
        <Button variant="ghost" size="sm" onClick={() => router.push('/settlements')}>
          &larr; Hakedislere Don
        </Button>

        {/* Summary */}
        <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
          <StatCard title="Toplam Tutar" value={fmt(period.totalAmount)} color="orange" />
          <StatCard title="Komisyon" value={fmt(period.totalCommission)} color="red" />
          <StatCard title="Sabit Ucret" value={fmt(period.totalFixedFee)} color="blue" />
          <StatCard title="Net Odeme" value={fmt(period.netAmount)} color="green" />
        </div>

        {/* Period Info */}
        <div className="bg-white rounded-xl border border-gray-200 p-5">
          <div className="flex items-start justify-between gap-4 flex-wrap">
            <div className="space-y-2">
              <div className="flex items-center gap-3">
                <h2 className="text-lg font-semibold text-gray-800">{period.restaurantName || period.restaurantId || '-'}</h2>
                {status && <Badge label={status.label} color={status.color} />}
              </div>
              <div className="flex items-center gap-4 text-sm text-gray-500">
                <span>
                  {period.periodStart ? new Date(period.periodStart).toLocaleDateString('tr-TR') : '-'}
                  {period.periodEnd ? ` - ${new Date(period.periodEnd).toLocaleDateString('tr-TR')}` : ''}
                </span>
                <span>{period.orderCount ?? 0} siparis</span>
              </div>
              {period.bankTransferRef && (
                <p className="text-sm text-gray-500">Havale Ref: <span className="font-mono">{period.bankTransferRef}</span></p>
              )}
              {period.cancelReason && (
                <p className="text-sm text-red-500">Iptal Sebebi: {period.cancelReason}</p>
              )}
            </div>

            <div className="flex gap-2">
              {period.statusId === 1 && (
                <>
                  <Button onClick={handleApprove} loading={actionLoading}>Onayla</Button>
                  <Button variant="danger" onClick={handleCancel} loading={actionLoading}>Iptal</Button>
                </>
              )}
              {period.statusId === 2 && (
                <Button onClick={() => setShowPayForm(!showPayForm)} loading={actionLoading}>
                  {showPayForm ? 'Vazgec' : 'Ode'}
                </Button>
              )}
            </div>
          </div>

          {/* Pay Form */}
          {showPayForm && period.statusId === 2 && (
            <form onSubmit={handlePay} className="mt-4 border-t border-gray-100 pt-4 space-y-4">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Havale Referans No</label>
                  <input
                    type="text"
                    required
                    value={payForm.bankTransferRef}
                    onChange={(e) => setPayForm({ ...payForm, bankTransferRef: e.target.value })}
                    className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm"
                    placeholder="Banka referans numarasi"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Notlar</label>
                  <input
                    type="text"
                    value={payForm.notes}
                    onChange={(e) => setPayForm({ ...payForm, notes: e.target.value })}
                    className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm"
                    placeholder="Ek notlar"
                  />
                </div>
              </div>
              <Button type="submit" loading={actionLoading}>Odemeyi Tamamla</Button>
            </form>
          )}
        </div>

        {/* Items Table */}
        <div className="bg-white rounded-xl border border-gray-200 overflow-hidden">
          <div className="p-5 border-b border-gray-100">
            <h2 className="font-semibold text-gray-800">Siparis Detaylari</h2>
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
                {items.length === 0 ? (
                  <tr>
                    <td colSpan={7} className="px-5 py-10 text-center text-gray-400 text-sm">
                      Kalem bulunamadi
                    </td>
                  </tr>
                ) : (
                  items.map((item, i) => (
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
      </div>
    </AdminLayout>
  );
}
