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

const STATUS_MAP = {
  0: { label: 'Onay Bekliyor', color: 'yellow' },
  1: { label: 'Aktif', color: 'green' },
  2: { label: 'Pasif', color: 'gray' },
  3: { label: 'Silinmiş', color: 'red' },
};

export default function CouriersPage() {
  const toast = useToast();
  const [page, setPage] = useState(1);
  const [actionModal, setActionModal] = useState(null);
  const [processing, setProcessing] = useState(false);

  const { data, isLoading, mutate } = useSWR(
    `/v1/admin/courier/list?page=${page}&pageSize=20`,
    fetcher
  );

  const couriers = data?.data || [];
  const total = data?.totalDataCount || 0;
  const totalPages = Math.ceil(total / 20) || 1;

  const handleApprove = async (approve) => {
    if (!actionModal) return;
    setProcessing(true);
    try {
      await api.post('/v1/admin/courier/approve', {
        userId: actionModal.userId,
        approve,
      });
      toast(approve ? 'Kurye onaylandı' : 'Kurye reddedildi', 'success');
      setActionModal(null);
      mutate();
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || 'Hata oluştu', 'error');
    } finally {
      setProcessing(false);
    }
  };

  const columns = [
    {
      title: 'Ad Soyad',
      key: 'firstName',
      render: (v, row) => (
        <div>
          <p className="font-medium text-gray-800">{v} {row.lastName}</p>
          <p className="text-xs text-gray-400">{row.email}</p>
        </div>
      ),
    },
    {
      title: 'Telefon',
      key: 'phoneNumber',
      render: (v) => <span className="text-sm">{v || '—'}</span>,
    },
    {
      title: 'Durum',
      key: 'userStatusId',
      render: (v) => {
        const s = STATUS_MAP[v] || { label: String(v), color: 'gray' };
        return <Badge label={s.label} color={s.color} />;
      },
    },
    {
      title: 'Kayıt Tarihi',
      key: 'createdDate',
      render: (v) => v ? new Date(v).toLocaleDateString('tr-TR') : '—',
    },
    {
      title: 'İşlem',
      key: 'userId',
      width: 180,
      render: (_, row) => {
        if (row.userStatusId === 1) {
          return <span className="text-xs text-green-600 font-medium">✓ Onaylı</span>;
        }
        if (row.userStatusId === 0) {
          return (
            <Button size="sm" onClick={() => setActionModal(row)}>
              İşlem Yap
            </Button>
          );
        }
        return (
          <Button size="sm" variant="outline" onClick={() => setActionModal(row)}>
            İşlem Yap
          </Button>
        );
      },
    },
  ];

  return (
    <AdminLayout title="Kuryeler">
      <div className="space-y-4">
        <div className="flex items-center justify-between">
          <p className="text-sm text-gray-500">{total} kurye</p>
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

      {/* Approve / Reject Modal */}
      <Modal isOpen={!!actionModal} onClose={() => setActionModal(null)} title="Kurye İşlem" size="sm">
        <div className="space-y-4">
          <div className="bg-gray-50 rounded-lg p-4 space-y-2">
            <p className="text-sm font-medium text-gray-800">
              {actionModal?.firstName} {actionModal?.lastName}
            </p>
            <p className="text-xs text-gray-500">{actionModal?.email}</p>
            <p className="text-xs text-gray-500">{actionModal?.phoneNumber}</p>
            <p className="text-xs text-gray-400">
              Durum: {STATUS_MAP[actionModal?.userStatusId]?.label || '—'}
            </p>
          </div>
          <div className="flex justify-end gap-2">
            <Button variant="secondary" onClick={() => setActionModal(null)}>Kapat</Button>
            {actionModal?.userStatusId !== 2 && (
              <Button
                variant="outline"
                className="!text-red-600 !border-red-300 hover:!bg-red-50"
                loading={processing}
                onClick={() => handleApprove(false)}
              >
                Reddet
              </Button>
            )}
            {actionModal?.userStatusId !== 1 && (
              <Button loading={processing} onClick={() => handleApprove(true)}>
                Onayla
              </Button>
            )}
          </div>
        </div>
      </Modal>
    </AdminLayout>
  );
}
