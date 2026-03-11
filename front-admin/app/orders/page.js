'use client';

import { useState } from 'react';
import useSWR from 'swr';
import AdminLayout from '@/components/layout/AdminLayout';
import Table from '@/components/ui/Table';
import Modal from '@/components/ui/Modal';
import Badge from '@/components/ui/Badge';
import Button from '@/components/ui/Button';
import { useToast } from '@/components/ui/Toast';
import fetcher from '@/lib/fetcher';
import api from '@/lib/api';

// Backend: Pending=1, Confirmed=2, Preparing=3, OnTheWay=4, Delivered=5, Cancelled=6, Rejected=7
const STATUS_MAP = {
  1: { label: 'Bekliyor', color: 'yellow' },
  2: { label: 'Onaylandı', color: 'blue' },
  3: { label: 'Hazırlanıyor', color: 'orange' },
  4: { label: 'Yolda', color: 'purple' },
  5: { label: 'Teslim Edildi', color: 'green' },
  6: { label: 'İptal', color: 'red' },
  7: { label: 'Reddedildi', color: 'red' },
};

const STATUS_OPTIONS = [
  { value: 2, label: 'Onaylandı' },
  { value: 3, label: 'Hazırlanıyor' },
  { value: 4, label: 'Yolda' },
  { value: 5, label: 'Teslim Edildi' },
  { value: 6, label: 'İptal' },
  { value: 7, label: 'Reddedildi' },
];

const FILTER_TABS = [
  { key: null, label: 'Tümü' },
  { key: 1, label: 'Bekliyor' },
  { key: 2, label: 'Onaylandı' },
  { key: 3, label: 'Hazırlanıyor' },
  { key: 5, label: 'Teslim Edildi' },
  { key: 6, label: 'İptal' },
];

export default function OrdersPage() {
  const toast = useToast();
  const [page, setPage] = useState(1);
  const [statusFilter, setStatusFilter] = useState(null);
  const [statusModal, setStatusModal] = useState(null);
  const [newStatus, setNewStatus] = useState('');
  const [updating, setUpdating] = useState(false);

  const swrKey = `/v1/admin/order/list?page=${page}&pageSize=20${statusFilter ? `&statusId=${statusFilter}` : ''}`;
  const { data, isLoading, mutate } = useSWR(swrKey, fetcher);

  const orders = data?.data || [];
  const total = data?.totalDataCount || 0;
  const totalPages = Math.ceil(total / 20) || 1;

  const handleFilterChange = (key) => {
    setStatusFilter(key);
    setPage(1);
  };

  const handleUpdateStatus = async () => {
    if (!newStatus) return;
    setUpdating(true);
    try {
      await api.put(`/v1/admin/order/${statusModal.id}/status?statusId=${newStatus}`);
      toast('Sipariş durumu güncellendi', 'success');
      mutate();
      setStatusModal(null);
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || 'Hata oluştu', 'error');
    } finally {
      setUpdating(false);
    }
  };

  const columns = [
    {
      title: 'Sipariş',
      key: 'id',
      width: 90,
      render: (v) => <span className="font-mono text-xs text-gray-500">{String(v).slice(0, 8)}…</span>,
    },
    {
      title: 'Müşteri',
      key: 'customerName',
      render: (v, row) => (
        <div>
          <p className="font-medium text-sm">{v || row.userId?.slice(0, 8) || '—'}</p>
          <p className="text-xs text-gray-400">{row.deliveryAddress || ''}</p>
        </div>
      ),
    },
    {
      title: 'Restoran',
      key: 'restaurantName',
      render: (v) => <span className="text-sm">{v || '—'}</span>,
    },
    {
      title: 'Tutar',
      key: 'totalPrice',
      render: (v) => <span className="font-semibold text-sm">₺{Number(v || 0).toFixed(2)}</span>,
    },
    {
      title: 'Durum',
      key: 'statusId',
      render: (v) => {
        const s = STATUS_MAP[v] || { label: String(v), color: 'gray' };
        return <Badge label={s.label} color={s.color} />;
      },
    },
    {
      title: 'Tarih',
      key: 'createdDate',
      render: (v) => v ? new Date(v).toLocaleString('tr-TR') : '—',
    },
    {
      title: 'İşlem',
      key: 'id',
      width: 130,
      render: (_, row) => (
        <Button
          size="sm"
          variant="outline"
          onClick={() => { setStatusModal(row); setNewStatus(''); }}
        >
          Durum Güncelle
        </Button>
      ),
    },
  ];

  return (
    <AdminLayout title="Siparişler">
      <div className="space-y-4">
        <div className="flex items-center gap-2 flex-wrap">
          {FILTER_TABS.map((tab) => (
            <button
              key={String(tab.key)}
              onClick={() => handleFilterChange(tab.key)}
              className={`px-3 py-1.5 rounded-lg text-xs font-medium transition-colors ${
                statusFilter === tab.key
                  ? 'bg-orange-500 text-white'
                  : 'bg-white border border-gray-200 text-gray-600 hover:bg-gray-50'
              }`}
            >
              {tab.label}
            </button>
          ))}
          <span className="text-sm text-gray-400 ml-2">{total} sipariş</span>
        </div>

        <Table columns={columns} data={orders} loading={isLoading} emptyText="Sipariş bulunamadı" />

        {totalPages > 1 && (
          <div className="flex justify-center gap-2 pt-2">
            <Button variant="outline" size="sm" onClick={() => setPage(p => Math.max(1, p - 1))} disabled={page === 1}>
              ← Önceki
            </Button>
            <span className="text-sm text-gray-500 self-center">{page} / {totalPages}</span>
            <Button variant="outline" size="sm" onClick={() => setPage(p => Math.min(totalPages, p + 1))} disabled={page === totalPages}>
              Sonraki →
            </Button>
          </div>
        )}
      </div>

      <Modal isOpen={!!statusModal} onClose={() => setStatusModal(null)} title="Durum Güncelle" size="sm">
        <div className="space-y-4">
          <div className="bg-gray-50 rounded-lg p-3 text-sm">
            <p className="text-gray-500 text-xs mb-1">Sipariş ID</p>
            <p className="font-mono text-xs">{statusModal?.id}</p>
            <div className="mt-2 flex items-center gap-2">
              <span className="text-gray-500 text-xs">Mevcut:</span>
              <Badge
                label={STATUS_MAP[statusModal?.statusId]?.label || String(statusModal?.statusId)}
                color={STATUS_MAP[statusModal?.statusId]?.color || 'gray'}
              />
            </div>
          </div>
          <div>
            <label className="text-sm font-medium text-gray-700 block mb-1">Yeni Durum</label>
            <select
              value={newStatus}
              onChange={(e) => setNewStatus(e.target.value)}
              className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-orange-400"
            >
              <option value="">Seçin...</option>
              {STATUS_OPTIONS.map((opt) => (
                <option key={opt.value} value={opt.value}>{opt.label}</option>
              ))}
            </select>
          </div>
          <div className="flex justify-end gap-2">
            <Button variant="secondary" onClick={() => setStatusModal(null)}>İptal</Button>
            <Button disabled={!newStatus} loading={updating} onClick={handleUpdateStatus}>Güncelle</Button>
          </div>
        </div>
      </Modal>
    </AdminLayout>
  );
}
