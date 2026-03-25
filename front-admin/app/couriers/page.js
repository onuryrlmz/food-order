'use client';

import React, { useState } from 'react';
import useSWR from 'swr';
import AdminLayout from '@/components/layout/AdminLayout';
import Table from '@/components/ui/Table';
import Modal from '@/components/ui/Modal';
import Badge from '@/components/ui/Badge';
import Button from '@/components/ui/Button';
import { useToast } from '@/components/ui/Toast';
import fetcher from '@/lib/fetcher';
import api from '@/lib/api';

const STATUS_MAP = {
  1: { label: 'Onay Bekliyor', color: 'yellow' },
  2: { label: 'Aktif', color: 'green' },
  3: { label: 'Askıda', color: 'orange' },
  4: { label: 'Deaktif', color: 'red' },
};

const COURIER_TYPE_MAP = {
  1: 'Restoran Kuryesi',
  2: 'Bireysel',
  3: 'Firma Kuryesi',
};

const FILTER_TABS = [
  { key: null, label: 'Tümü' },
  { key: 1, label: 'Onay Bekliyor' },
  { key: 2, label: 'Aktif' },
  { key: 3, label: 'Askıda' },
];

export default function CouriersPage() {
  const toast = useToast();
  const [page, setPage] = useState(1);
  const [statusFilter, setStatusFilter] = useState(null);
  const [detailModal, setDetailModal] = useState(null);
  const [actionLoading, setActionLoading] = useState(false);

  const swrKey = `/v1/admin/courier/couriers?page=${page}&pageSize=20${statusFilter ? `&statusId=${statusFilter}` : ''}`;
  const { data, isLoading, mutate } = useSWR(swrKey, fetcher);

  const couriers = data?.data || [];
  const total = data?.totalDataCount || 0;
  const totalPages = Math.ceil(total / 20) || 1;

  const handleFilterChange = (key) => {
    setStatusFilter(key);
    setPage(1);
  };

  const handleApprove = async (id, e) => {
    if (e) e.stopPropagation();
    setActionLoading(true);
    try {
      await api.put(`/v1/admin/courier/couriers/${id}/approve`);
      toast('Kurye onaylandı', 'success');
      mutate();
      if (detailModal?.id === id) setDetailModal(null);
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || 'Hata oluştu', 'error');
    } finally {
      setActionLoading(false);
    }
  };

  const handleSuspend = async (id, e) => {
    if (e) e.stopPropagation();
    setActionLoading(true);
    try {
      await api.put(`/v1/admin/courier/couriers/${id}/suspend`);
      toast('Kurye askıya alındı', 'success');
      mutate();
      if (detailModal?.id === id) setDetailModal(null);
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || 'Hata oluştu', 'error');
    } finally {
      setActionLoading(false);
    }
  };

  const columns = [
    {
      title: 'Ad Soyad',
      key: 'fullName',
      render: (v, row) => (
        <div>
          <p className="font-medium text-sm">{v || `${row.firstName || ''} ${row.lastName || ''}`.trim() || '—'}</p>
        </div>
      ),
    },
    {
      title: 'E-posta',
      key: 'email',
      render: (v) => <span className="text-sm text-gray-600">{v || '—'}</span>,
    },
    {
      title: 'Telefon',
      key: 'phone',
      render: (v) => <span className="text-sm">{v || '—'}</span>,
    },
    {
      title: 'Tip',
      key: 'courierTypeId',
      render: (v) => <span className="text-xs font-medium text-gray-600">{COURIER_TYPE_MAP[v] || '—'}</span>,
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
      title: 'Araç',
      key: 'vehicleType',
      render: (v) => <span className="text-sm">{v || '—'}</span>,
    },
    {
      title: 'Firma',
      key: 'companyName',
      render: (v) => <span className="text-sm text-gray-600">{v || '—'}</span>,
    },
    {
      title: 'Rating',
      key: 'rating',
      render: (v) => v != null ? <span className="text-sm font-semibold text-orange-500">{Number(v).toFixed(1)}</span> : <span className="text-gray-400">—</span>,
    },
    {
      title: 'Teslimat',
      key: 'deliveryCount',
      render: (v) => <span className="text-sm font-semibold">{v ?? 0}</span>,
    },
    {
      title: 'Kayıt Tarihi',
      key: 'createdDate',
      render: (v) => v ? <span className="text-xs text-gray-500">{new Date(v).toLocaleDateString('tr-TR')}</span> : '—',
    },
    {
      title: 'İşlemler',
      key: 'id',
      width: 200,
      render: (_, row) => (
        <div className="flex gap-1">
          <Button size="sm" variant="ghost" onClick={(e) => { e.stopPropagation(); setDetailModal(row); }}>Detay</Button>
          {row.statusId === 1 && (
            <Button size="sm" variant="outline" onClick={(e) => handleApprove(row.id, e)} loading={actionLoading}>Onayla</Button>
          )}
          {row.statusId === 2 && (
            <Button size="sm" variant="outline" onClick={(e) => handleSuspend(row.id, e)} loading={actionLoading}>Askıya Al</Button>
          )}
        </div>
      ),
    },
  ];

  return (
    <AdminLayout title="Kuryeler">
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
          <span className="text-sm text-gray-400 ml-2">{total} kurye</span>
        </div>

        <Table columns={columns} data={couriers} loading={isLoading} emptyText="Kurye bulunamadı" />

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

      {/* Kurye Detay Modal */}
      <Modal isOpen={!!detailModal} onClose={() => setDetailModal(null)} title="Kurye Detayı" size="lg">
        {detailModal && (
          <div className="space-y-5">
            <div className="grid grid-cols-2 gap-4">
              <InfoCard label="Ad Soyad" value={detailModal.fullName || `${detailModal.firstName || ''} ${detailModal.lastName || ''}`.trim() || '—'} />
              <InfoCard label="E-posta" value={detailModal.email || '—'} />
              <InfoCard label="Telefon" value={detailModal.phone || '—'} />
              <InfoCard label="Kurye Tipi" value={COURIER_TYPE_MAP[detailModal.courierTypeId] || '—'} />
              <InfoCard label="Araç Tipi" value={detailModal.vehicleType || '—'} />
              <InfoCard label="Plaka" value={detailModal.vehiclePlate || '—'} />
              <InfoCard label="Firma" value={detailModal.companyName || '—'} />
              <InfoCard label="Rating" value={detailModal.rating != null ? Number(detailModal.rating).toFixed(1) : '—'} />
              <InfoCard label="Toplam Teslimat" value={String(detailModal.deliveryCount ?? 0)} />
              <InfoCard label="Kayıt Tarihi" value={detailModal.createdDate ? new Date(detailModal.createdDate).toLocaleString('tr-TR') : '—'} />
            </div>

            <div className="flex items-center gap-3">
              <span className="text-sm font-medium text-gray-600">Durum:</span>
              <Badge
                label={STATUS_MAP[detailModal.statusId]?.label || String(detailModal.statusId)}
                color={STATUS_MAP[detailModal.statusId]?.color || 'gray'}
              />
            </div>

            {detailModal.identityNumber && (
              <div className="bg-gray-50 rounded-lg p-4">
                <h4 className="text-sm font-semibold text-gray-700 mb-1">Kimlik Bilgileri</h4>
                <p className="text-sm text-gray-600 font-mono">{detailModal.identityNumber}</p>
              </div>
            )}

            <div className="flex justify-end gap-2">
              {detailModal.statusId === 1 && (
                <Button onClick={() => handleApprove(detailModal.id)} loading={actionLoading}>Onayla</Button>
              )}
              {detailModal.statusId === 2 && (
                <Button variant="secondary" onClick={() => handleSuspend(detailModal.id)} loading={actionLoading}>Askıya Al</Button>
              )}
              <Button variant="secondary" onClick={() => setDetailModal(null)}>Kapat</Button>
            </div>
          </div>
        )}
      </Modal>
    </AdminLayout>
  );
}

function InfoCard({ label, value, sub, mono }) {
  return (
    <div className="bg-gray-50 rounded-lg p-3">
      <p className="text-xs text-gray-500 mb-0.5">{label}</p>
      <p className={`text-sm font-medium text-gray-800 ${mono ? 'font-mono text-xs' : ''}`}>{value}</p>
      {sub && <p className="text-xs text-gray-400">{sub}</p>}
    </div>
  );
}
