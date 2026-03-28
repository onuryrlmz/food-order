'use client';

import { useState } from 'react';
import useSWR from 'swr';
import { useRouter } from 'next/navigation';
import AdminLayout from '@/components/layout/AdminLayout';
import Badge from '@/components/ui/Badge';
import Button from '@/components/ui/Button';
import { useToast } from '@/components/ui/Toast';
import fetcher from '@/lib/fetcher';
import { approveSettlement, paySettlement, cancelSettlement } from '@/lib/api';

const STATUS_MAP = {
  1: { label: 'Beklemede', color: 'yellow' },
  2: { label: 'Onaylandi', color: 'blue' },
  3: { label: 'Odendi', color: 'green' },
  4: { label: 'Iptal', color: 'gray' },
};

export default function SettlementsPage() {
  const router = useRouter();
  const toast = useToast();
  const [page, setPage] = useState(1);
  const [statusFilter, setStatusFilter] = useState('');
  const [actionLoading, setActionLoading] = useState(null);

  const url = `/v1/admin/settlement/periods?page=${page}&pageSize=20${statusFilter ? `&statusId=${statusFilter}` : ''}`;
  const { data, isLoading, mutate } = useSWR(url, fetcher);

  const periods = data?.data || [];
  const total = data?.totalDataCount || 0;
  const totalPages = Math.ceil(total / 20) || 1;

  const fmt = (val) => {
    if (val == null) return '-';
    return new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(val);
  };

  const handleApprove = async (periodId, e) => {
    e.stopPropagation();
    if (!confirm('Bu hakedisi onaylamak istediginize emin misiniz?')) return;
    setActionLoading(periodId);
    try {
      await approveSettlement(periodId);
      toast('Hakedis onaylandi', 'success');
      mutate();
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || err.message || 'Hata olustu', 'error');
    } finally {
      setActionLoading(null);
    }
  };

  const handleCancel = async (periodId, e) => {
    e.stopPropagation();
    const reason = prompt('Iptal sebebi:');
    if (reason === null) return;
    setActionLoading(periodId);
    try {
      await cancelSettlement(periodId, reason);
      toast('Hakedis iptal edildi', 'success');
      mutate();
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || err.message || 'Hata olustu', 'error');
    } finally {
      setActionLoading(null);
    }
  };

  return (
    <AdminLayout title="Hakedis Yonetimi">
      <div className="space-y-6">
        {/* Filters */}
        <div className="flex items-center gap-3 flex-wrap">
          <select
            value={statusFilter}
            onChange={(e) => { setStatusFilter(e.target.value); setPage(1); }}
            className="border border-gray-300 rounded-lg px-3 py-2 text-sm bg-white"
          >
            <option value="">Tumu</option>
            <option value="1">Beklemede</option>
            <option value="2">Onaylandi</option>
            <option value="3">Odendi</option>
            <option value="4">Iptal</option>
          </select>
          <p className="text-sm text-gray-500 ml-auto">{total} hakedis</p>
        </div>

        {/* Table */}
        {isLoading ? (
          <div className="flex justify-center py-12">
            <div className="w-8 h-8 border-2 border-orange-400 border-t-transparent rounded-full animate-spin" />
          </div>
        ) : periods.length === 0 ? (
          <div className="bg-white rounded-xl border border-gray-200 p-10 text-center text-gray-400 text-sm">
            Hakedis bulunamadi
          </div>
        ) : (
          <div className="bg-white rounded-xl border border-gray-200 overflow-hidden">
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead>
                  <tr className="text-left text-xs font-semibold text-gray-400 uppercase tracking-wider">
                    <th className="px-5 py-3">Restoran</th>
                    <th className="px-5 py-3">Tarih</th>
                    <th className="px-5 py-3">Siparis Sayisi</th>
                    <th className="px-5 py-3">Toplam Tutar</th>
                    <th className="px-5 py-3">Komisyon</th>
                    <th className="px-5 py-3">Sabit Ucret</th>
                    <th className="px-5 py-3">Net Odeme</th>
                    <th className="px-5 py-3">Durum</th>
                    <th className="px-5 py-3">Islemler</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-50">
                  {periods.map((p) => {
                    const status = STATUS_MAP[p.statusId] || { label: '?', color: 'gray' };
                    return (
                      <tr
                        key={p.id}
                        onClick={() => router.push(`/settlements/${p.id}`)}
                        className="hover:bg-gray-50 cursor-pointer"
                      >
                        <td className="px-5 py-3 text-sm font-medium text-gray-800">{p.restaurantName || p.restaurantId || '-'}</td>
                        <td className="px-5 py-3 text-sm text-gray-600">
                          {p.periodStart ? new Date(p.periodStart).toLocaleDateString('tr-TR') : '-'}
                          {p.periodEnd ? ` - ${new Date(p.periodEnd).toLocaleDateString('tr-TR')}` : ''}
                        </td>
                        <td className="px-5 py-3 text-sm text-gray-700">{p.orderCount ?? '-'}</td>
                        <td className="px-5 py-3 text-sm text-gray-700">{fmt(p.totalAmount)}</td>
                        <td className="px-5 py-3 text-sm text-orange-600">{fmt(p.totalCommission)}</td>
                        <td className="px-5 py-3 text-sm text-gray-600">{fmt(p.totalFixedFee)}</td>
                        <td className="px-5 py-3 text-sm text-emerald-600 font-medium">{fmt(p.netAmount)}</td>
                        <td className="px-5 py-3">
                          <Badge label={status.label} color={status.color} />
                        </td>
                        <td className="px-5 py-3">
                          <div className="flex gap-2" onClick={(e) => e.stopPropagation()}>
                            {p.statusId === 1 && (
                              <>
                                <Button size="sm" onClick={(e) => handleApprove(p.id, e)} loading={actionLoading === p.id}>
                                  Onayla
                                </Button>
                                <Button size="sm" variant="danger" onClick={(e) => handleCancel(p.id, e)} loading={actionLoading === p.id}>
                                  Iptal
                                </Button>
                              </>
                            )}
                            {p.statusId === 2 && (
                              <Button size="sm" onClick={() => router.push(`/settlements/${p.id}`)}>
                                Ode
                              </Button>
                            )}
                          </div>
                        </td>
                      </tr>
                    );
                  })}
                </tbody>
              </table>
            </div>
          </div>
        )}

        {/* Pagination */}
        {totalPages > 1 && (
          <div className="flex justify-center gap-2 pt-2">
            <Button variant="outline" size="sm" onClick={() => setPage(p => Math.max(1, p - 1))} disabled={page === 1}>
              Onceki
            </Button>
            <span className="text-sm text-gray-500 self-center">{page} / {totalPages}</span>
            <Button variant="outline" size="sm" onClick={() => setPage(p => Math.min(totalPages, p + 1))} disabled={page === totalPages}>
              Sonraki
            </Button>
          </div>
        )}
      </div>
    </AdminLayout>
  );
}
