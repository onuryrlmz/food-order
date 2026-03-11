'use client';

import { useState } from 'react';
import useSWR from 'swr';
import AdminLayout from '@/components/layout/AdminLayout';
import Table from '@/components/ui/Table';
import Badge from '@/components/ui/Badge';
import Button from '@/components/ui/Button';
import { useToast } from '@/components/ui/Toast';
import fetcher from '@/lib/fetcher';
import api from '@/lib/api';

// SubscriptionStatusEnums: Active=1, Expired=2, Cancelled=3, Suspended=4
const STATUS_MAP = {
  1: { label: 'Aktif', color: 'green' },
  2: { label: 'Süresi Dolmuş', color: 'red' },
  3: { label: 'İptal', color: 'gray' },
  4: { label: 'Askıya Alındı', color: 'yellow' },
};

export default function SubscriptionsPage() {
  const toast = useToast();
  // GET /v1/admin/subscription/all?page=1&pageSize=100
  const { data, isLoading, mutate } = useSWR('/v1/admin/subscription/all?page=1&pageSize=100', fetcher);
  const subscriptions = data?.data || [];
  const [expiring, setExpiring] = useState(false);

  const runExpireCheck = async () => {
    setExpiring(true);
    try {
      // POST /v1/admin/subscription/expire-check
      await api.post('/v1/admin/subscription/expire-check');
      toast('Süresi dolmuş abonelikler kapatıldı', 'success');
      mutate();
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.message || 'Hata oluştu', 'error');
    } finally {
      setExpiring(false);
    }
  };

  const getStatus = (v) => STATUS_MAP[v] || { label: String(v ?? '?'), color: 'gray' };

  const columns = [
    { title: '#', key: 'id', width: 80, render: (v) => <span className="font-mono text-xs text-gray-400">{String(v).slice(0,8)}…</span> },
    { title: 'Restoran', key: 'restaurantName', render: (v) => <span className="font-medium">{v || '—'}</span> },
    { title: 'Plan', key: 'planName', render: (v) => <Badge label={v || '?'} color="blue" /> },
    { title: 'Başlangıç', key: 'startDate', render: (v) => v ? new Date(v).toLocaleDateString('tr-TR') : '—' },
    { title: 'Bitiş', key: 'endDate', render: (v) => v ? new Date(v).toLocaleDateString('tr-TR') : '—' },
    {
      title: 'Durum',
      key: 'statusId',
      render: (v) => {
        const s = getStatus(v);
        return <Badge label={s.label} color={s.color} />;
      },
    },
    { title: 'Aylık Fiyat', key: 'monthlyPrice', render: (v) => v != null ? `₺${v}` : '—' },
  ];

  const activeCount = subscriptions.filter(s => s.statusId === 1).length;
  const expiredCount = subscriptions.filter(s => s.statusId === 2).length;
  const cancelledCount = subscriptions.filter(s => s.statusId === 3).length;

  return (
    <AdminLayout title="Abonelikler">
      <div className="space-y-4">
        <div className="flex items-center justify-between flex-wrap gap-3">
          <p className="text-sm text-gray-500">{subscriptions.length} abonelik</p>
          <Button variant="danger" loading={expiring} onClick={runExpireCheck}>
            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
            Süresi Dolmuşları Kapat
          </Button>
        </div>

        <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
          {[
            { label: 'Aktif', color: 'border-green-200 bg-green-50', textColor: 'text-green-700', count: activeCount },
            { label: 'Süresi Dolmuş', color: 'border-red-200 bg-red-50', textColor: 'text-red-700', count: expiredCount },
            { label: 'İptal', color: 'border-gray-200 bg-gray-50', textColor: 'text-gray-700', count: cancelledCount },
          ].map((item) => (
            <div key={item.label} className={`rounded-xl border ${item.color} p-4`}>
              <p className="text-xs font-medium text-gray-500">{item.label}</p>
              <p className={`text-2xl font-bold mt-1 ${item.textColor}`}>{item.count}</p>
            </div>
          ))}
        </div>

        <Table columns={columns} data={subscriptions} loading={isLoading} emptyText="Abonelik bulunamadı" />
      </div>
    </AdminLayout>
  );
}
