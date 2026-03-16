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
import {
  approveCourierCompany,
  rejectCourierCompany,
  suspendCourierCompany,
  banCourierCompany,
} from '@/lib/api';

const STATUS_MAP = {
  0: { label: 'Onay Bekliyor', color: 'yellow' },
  1: { label: 'Aktif', color: 'green' },
  2: { label: 'Askıya Alındı', color: 'orange' },
  3: { label: 'Yasaklandı', color: 'red' },
};

export default function CourierCompaniesPage() {
  const toast = useToast();
  const [page, setPage] = useState(1);
  const [actionModal, setActionModal] = useState(null);
  const [processing, setProcessing] = useState(false);

  const { data, isLoading, mutate } = useSWR(
    `/v1/admin/courier-companies?page=${page}&size=20`,
    fetcher
  );

  const companies = data?.data || [];
  const total = data?.totalDataCount || 0;
  const totalPages = Math.ceil(total / 20) || 1;

  const handleAction = async (action) => {
    if (!actionModal) return;
    setProcessing(true);
    try {
      const id = actionModal.id;
      let result;
      let msg;
      switch (action) {
        case 'approve':
          result = await approveCourierCompany(id);
          msg = 'Firma onaylandı';
          break;
        case 'reject':
          result = await rejectCourierCompany(id);
          msg = 'Firma reddedildi';
          break;
        case 'suspend':
          result = await suspendCourierCompany(id);
          msg = 'Firma askıya alındı';
          break;
        case 'ban':
          result = await banCourierCompany(id);
          msg = 'Firma yasaklandı';
          break;
      }
      if (result?.hasFailed) {
        toast(result.messages?.[0]?.description || 'Hata oluştu', 'error');
      } else {
        toast(msg, 'success');
        setActionModal(null);
        mutate();
      }
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || 'Hata oluştu', 'error');
    } finally {
      setProcessing(false);
    }
  };

  const columns = [
    {
      title: 'Firma Adı',
      key: 'name',
      render: (v, row) => (
        <div>
          <p className="font-medium text-gray-800">{v}</p>
          {row.legalName && <p className="text-xs text-gray-400">{row.legalName}</p>}
        </div>
      ),
    },
    {
      title: 'Yetkili Email',
      key: 'contactEmail',
      render: (v) => <span className="text-sm">{v || '—'}</span>,
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
      title: 'Kurye Sayısı',
      key: 'memberCount',
      render: (v) => <span className="text-sm font-medium">{v ?? 0}</span>,
    },
    {
      title: 'Kayıt Tarihi',
      key: 'createdDate',
      render: (v) => v ? new Date(v).toLocaleDateString('tr-TR') : '—',
    },
    {
      title: 'İşlemler',
      key: 'id',
      width: 120,
      render: (_, row) => (
        <Button size="sm" variant="outline" onClick={() => setActionModal(row)}>
          İşlem Yap
        </Button>
      ),
    },
  ];

  return (
    <AdminLayout title="Kurye Firmaları">
      <div className="space-y-4">
        <div className="flex items-center justify-between">
          <p className="text-sm text-gray-500">{total} kurye firması</p>
        </div>

        <Table columns={columns} data={companies} loading={isLoading} emptyText="Kurye firması bulunamadı" />

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

      {/* Action Modal */}
      <Modal isOpen={!!actionModal} onClose={() => setActionModal(null)} title="Firma İşlem" size="sm">
        <div className="space-y-4">
          <div className="bg-gray-50 rounded-lg p-4 space-y-2">
            <p className="text-sm font-medium text-gray-800">{actionModal?.name}</p>
            {actionModal?.legalName && (
              <p className="text-xs text-gray-500">{actionModal.legalName}</p>
            )}
            <p className="text-xs text-gray-500">{actionModal?.contactEmail}</p>
            <p className="text-xs text-gray-500">{actionModal?.contactPhone}</p>
            <p className="text-xs text-gray-400">
              Durum: {STATUS_MAP[actionModal?.statusId]?.label || '—'}
            </p>
            <p className="text-xs text-gray-400">
              Kurye Sayısı: {actionModal?.memberCount ?? 0}
            </p>
          </div>
          <div className="flex flex-wrap justify-end gap-2">
            <Button variant="secondary" onClick={() => setActionModal(null)}>Kapat</Button>
            {actionModal?.statusId === 0 && (
              <>
                <Button
                  variant="outline"
                  className="!text-red-600 !border-red-300 hover:!bg-red-50"
                  loading={processing}
                  onClick={() => handleAction('reject')}
                >
                  Reddet
                </Button>
                <Button loading={processing} onClick={() => handleAction('approve')}>
                  Onayla
                </Button>
              </>
            )}
            {actionModal?.statusId === 1 && (
              <>
                <Button
                  variant="outline"
                  className="!text-orange-600 !border-orange-300 hover:!bg-orange-50"
                  loading={processing}
                  onClick={() => handleAction('suspend')}
                >
                  Askıya Al
                </Button>
                <Button
                  variant="outline"
                  className="!text-red-600 !border-red-300 hover:!bg-red-50"
                  loading={processing}
                  onClick={() => handleAction('ban')}
                >
                  Yasakla
                </Button>
              </>
            )}
            {actionModal?.statusId === 2 && (
              <>
                <Button loading={processing} onClick={() => handleAction('approve')}>
                  Aktifleştir
                </Button>
                <Button
                  variant="outline"
                  className="!text-red-600 !border-red-300 hover:!bg-red-50"
                  loading={processing}
                  onClick={() => handleAction('ban')}
                >
                  Yasakla
                </Button>
              </>
            )}
          </div>
        </div>
      </Modal>
    </AdminLayout>
  );
}
