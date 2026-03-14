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
  1: { label: 'Ödeme Bekliyor', color: 'yellow' },
  2: { label: 'Ödeme Başarısız', color: 'red' },
  3: { label: 'Alıcı İptal Etti', color: 'red' },
  4: { label: 'Restoran Onayı Bekliyor', color: 'orange' },
  5: { label: 'Restoran Reddetti', color: 'red' },
  6: { label: 'Hazırlanıyor', color: 'purple' },
  7: { label: 'Yola Çıktı', color: 'blue' },
  8: { label: 'Teslim Edildi', color: 'green' },
};

const PAYMENT_STATUS_MAP = {
  1: { label: 'Bekliyor', color: 'yellow' },
  2: { label: 'Tamamlandı', color: 'green' },
  3: { label: 'Başarısız', color: 'red' },
  4: { label: 'İade Edildi', color: 'orange' },
};

const PAYMENT_OPTION_MAP = {
  1: 'Online Kredi Kartı',
  2: 'Kapıda Nakit',
  3: 'Kapıda Kredi Kartı',
};

const STATUS_OPTIONS = [
  { value: 4, label: 'Restoran Onayı Bekliyor' },
  { value: 5, label: 'Restoran Reddetti' },
  { value: 6, label: 'Hazırlanıyor' },
  { value: 7, label: 'Yola Çıktı' },
  { value: 8, label: 'Teslim Edildi' },
];

const FILTER_TABS = [
  { key: null, label: 'Tümü' },
  { key: 1, label: 'Ödeme Bekliyor' },
  { key: 4, label: 'Onay Bekliyor' },
  { key: 6, label: 'Hazırlanıyor' },
  { key: 7, label: 'Yolda' },
  { key: 8, label: 'Teslim Edildi' },
  { key: 2, label: 'Ödeme Başarısız' },
  { key: 3, label: 'İptal' },
];

export default function OrdersPage() {
  const toast = useToast();
  const [page, setPage] = useState(1);
  const [statusFilter, setStatusFilter] = useState(null);
  const [statusModal, setStatusModal] = useState(null);
  const [detailModal, setDetailModal] = useState(null);
  const [detailLoading, setDetailLoading] = useState(false);
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

  const handleShowDetail = async (order) => {
    setDetailLoading(true);
    setDetailModal(order);
    try {
      const res = await api.get(`/v1/admin/order/${order.id}`);
      if (!res.data?.hasFailed) {
        setDetailModal(res.data.data);
      }
    } catch { }
    setDetailLoading(false);
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
          <p className="font-medium text-sm">{v || '—'}</p>
          <p className="text-xs text-gray-400">{row.customerPhone || ''}</p>
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
      title: 'Sipariş Durumu',
      key: 'statusId',
      render: (v) => {
        const s = STATUS_MAP[v] || { label: String(v), color: 'gray' };
        return <Badge label={s.label} color={s.color} />;
      },
    },
    {
      title: 'Ödeme',
      key: 'paymentStatusId',
      render: (v, row) => {
        const ps = PAYMENT_STATUS_MAP[v] || { label: String(v), color: 'gray' };
        return (
          <div>
            <Badge label={ps.label} color={ps.color} />
            <p className="text-xs text-gray-400 mt-0.5">{PAYMENT_OPTION_MAP[row.paymentOptionId] || ''}</p>
          </div>
        );
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
      width: 180,
      render: (_, row) => (
        <div className="flex gap-1">
          <Button size="sm" variant="ghost" onClick={() => handleShowDetail(row)}>Detay</Button>
          <Button size="sm" variant="outline" onClick={() => { setStatusModal(row); setNewStatus(''); }}>Durum</Button>
        </div>
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

      {/* Durum Güncelle Modal */}
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

      {/* Sipariş Detay Modal */}
      <Modal isOpen={!!detailModal} onClose={() => setDetailModal(null)} title="Sipariş Detayı" size="lg">
        {detailLoading ? (
          <div className="flex justify-center py-8">
            <div className="w-6 h-6 border-2 border-orange-400 border-t-transparent rounded-full animate-spin" />
          </div>
        ) : detailModal && (
          <div className="space-y-5">
            {/* Genel Bilgiler */}
            <div className="grid grid-cols-2 gap-4">
              <InfoCard label="Sipariş ID" value={detailModal.id} mono />
              <InfoCard label="Tarih" value={detailModal.createdDate ? new Date(detailModal.createdDate).toLocaleString('tr-TR') : '—'} />
              <InfoCard label="Müşteri" value={detailModal.customerName || '—'} sub={detailModal.customerEmail} />
              <InfoCard label="Telefon" value={detailModal.customerPhone || '—'} />
              <InfoCard label="Restoran" value={detailModal.restaurantName || '—'} />
              <InfoCard label="Ödeme Yöntemi" value={PAYMENT_OPTION_MAP[detailModal.paymentOptionId] || '—'} />
            </div>

            {/* Durum */}
            <div className="flex items-center gap-3">
              <span className="text-sm font-medium text-gray-600">Sipariş Durumu:</span>
              <Badge label={STATUS_MAP[detailModal.statusId]?.label || String(detailModal.statusId)} color={STATUS_MAP[detailModal.statusId]?.color || 'gray'} />
              <span className="text-sm font-medium text-gray-600 ml-4">Ödeme:</span>
              <Badge label={PAYMENT_STATUS_MAP[detailModal.paymentStatusId]?.label || String(detailModal.paymentStatusId)} color={PAYMENT_STATUS_MAP[detailModal.paymentStatusId]?.color || 'gray'} />
            </div>

            {detailModal.cancellationReason && (
              <div className="bg-red-50 border border-red-200 rounded-lg p-3 text-sm text-red-700">
                <span className="font-medium">İptal/Red Nedeni:</span> {detailModal.cancellationReason}
              </div>
            )}

            {/* Tutarlar */}
            <div className="bg-gray-50 rounded-lg p-4">
              <h4 className="text-sm font-semibold text-gray-700 mb-2">Tutarlar</h4>
              <div className="grid grid-cols-4 gap-3 text-sm">
                <div><span className="text-gray-500">Ürünler:</span> <span className="font-medium">₺{Number(detailModal.totalProductPrice || 0).toFixed(2)}</span></div>
                <div><span className="text-gray-500">Teslimat:</span> <span className="font-medium">₺{Number(detailModal.shipmentPrice || 0).toFixed(2)}</span></div>
                <div><span className="text-gray-500">İndirim:</span> <span className="font-medium text-green-600">-₺{Number(detailModal.discountAmount || 0).toFixed(2)}</span></div>
                <div><span className="text-gray-500">Toplam:</span> <span className="font-bold text-orange-600">₺{Number(detailModal.totalPrice || 0).toFixed(2)}</span></div>
              </div>
              {detailModal.couponCode && <p className="text-xs text-gray-400 mt-1">Kupon: {detailModal.couponCode}</p>}
            </div>

            {/* Ürünler */}
            {detailModal.items?.length > 0 && (
              <div>
                <h4 className="text-sm font-semibold text-gray-700 mb-2">Ürünler</h4>
                <div className="border border-gray-200 rounded-lg overflow-hidden">
                  <table className="w-full text-sm">
                    <thead><tr className="bg-gray-50 text-xs text-gray-500 uppercase">
                      <th className="px-3 py-2 text-left">Ürün</th>
                      <th className="px-3 py-2 text-right">Adet</th>
                      <th className="px-3 py-2 text-right">Birim Fiyat</th>
                      <th className="px-3 py-2 text-right">Toplam</th>
                    </tr></thead>
                    <tbody>
                      {detailModal.items.map((item) => (
                        <React.Fragment key={item.id}>
                          <tr className="border-t border-gray-100">
                            <td className="px-3 py-2">
                              <p className="font-medium">{item.menuName || '—'}</p>
                              {item.description && <p className="text-xs text-gray-400">{item.description}</p>}
                            </td>
                            <td className="px-3 py-2 text-right">{item.quantity}</td>
                            <td className="px-3 py-2 text-right">₺{Number(item.unitPrice).toFixed(2)}</td>
                            <td className="px-3 py-2 text-right font-medium">₺{Number(item.totalPrice).toFixed(2)}</td>
                          </tr>
                          {item.values?.length > 0 && (
                            <tr>
                              <td colSpan={4} className="px-3 pb-2">
                                <div className="bg-gray-50 rounded p-2 space-y-1">
                                  {item.values.map((val, vi) => (
                                    <div key={vi}>
                                      <div className="flex items-center justify-between text-xs">
                                        <span className="text-gray-600"><span className="font-medium">{val.optionName}:</span> {val.valueName}{val.quantity > 1 ? ` x${val.quantity}` : ''}</span>
                                        {val.unitPrice > 0 && <span className="text-gray-500">+₺{Number(val.totalPrice).toFixed(2)}</span>}
                                      </div>
                                      {val.options?.length > 0 && val.options.map((opt, oi) => (
                                        <div key={oi} className="flex items-center justify-between text-xs text-gray-400 ml-3">
                                          <span><span className="font-medium">{opt.optionName}:</span> {opt.valueName}{opt.quantity > 1 ? ` x${opt.quantity}` : ''}</span>
                                          {opt.unitPrice > 0 && <span>+₺{Number(opt.totalPrice).toFixed(2)}</span>}
                                        </div>
                                      ))}
                                    </div>
                                  ))}
                                </div>
                              </td>
                            </tr>
                          )}
                        </React.Fragment>
                      ))}
                    </tbody>
                  </table>
                </div>
              </div>
            )}

            {/* Ödeme Kayıtları */}
            {detailModal.payments?.length > 0 && (
              <div>
                <h4 className="text-sm font-semibold text-gray-700 mb-2">Ödeme Kayıtları</h4>
                <div className="space-y-2">
                  {detailModal.payments.map((p) => (
                    <div key={p.id} className="border border-gray-200 rounded-lg p-3 text-sm">
                      <div className="flex items-center justify-between mb-2">
                        <Badge label={PAYMENT_STATUS_MAP[p.statusId]?.label || String(p.statusId)} color={PAYMENT_STATUS_MAP[p.statusId]?.color || 'gray'} />
                        <span className="font-semibold">₺{Number(p.amount).toFixed(2)}</span>
                      </div>
                      <div className="grid grid-cols-2 gap-x-4 gap-y-1 text-xs text-gray-500">
                        {p.providerPaymentId && <p>Provider Payment ID: <span className="font-mono text-gray-700">{p.providerPaymentId}</span></p>}
                        {p.providerConversationId && <p>Conversation ID: <span className="font-mono text-gray-700">{p.providerConversationId}</span></p>}
                        {p.cardLastFourDigits && <p>Kart: **** {p.cardLastFourDigits} {p.cardType && `(${p.cardType})`} {p.cardAssociation && `- ${p.cardAssociation}`}</p>}
                        {p.errorMessage && <p className="col-span-2 text-red-500">Hata: {p.errorMessage}</p>}
                        {p.completedAt && <p>Tamamlanma: {new Date(p.completedAt).toLocaleString('tr-TR')}</p>}
                        {p.failedAt && <p>Başarısız: {new Date(p.failedAt).toLocaleString('tr-TR')}</p>}
                        <p>Oluşturma: {new Date(p.createdDate).toLocaleString('tr-TR')}</p>
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            )}

            {/* Durum Geçmişi */}
            {detailModal.statusHistory?.length > 0 && (
              <div>
                <h4 className="text-sm font-semibold text-gray-700 mb-2">Durum Geçmişi</h4>
                <div className="border border-gray-200 rounded-lg overflow-hidden">
                  <table className="w-full text-sm">
                    <thead><tr className="bg-gray-50 text-xs text-gray-500 uppercase">
                      <th className="px-3 py-2 text-left">Durum</th>
                      <th className="px-3 py-2 text-left">Not</th>
                      <th className="px-3 py-2 text-left">Tarih</th>
                    </tr></thead>
                    <tbody>
                      {detailModal.statusHistory.map((sh) => (
                        <tr key={sh.id} className="border-t border-gray-100">
                          <td className="px-3 py-2"><Badge label={STATUS_MAP[sh.statusId]?.label || sh.statusName} color={STATUS_MAP[sh.statusId]?.color || 'gray'} /></td>
                          <td className="px-3 py-2 text-gray-500 text-xs">{sh.note || '—'}</td>
                          <td className="px-3 py-2 text-xs text-gray-500">{new Date(sh.occurredAt).toLocaleString('tr-TR')}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              </div>
            )}

            {detailModal.notes && (
              <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-3 text-sm">
                <span className="font-medium text-yellow-800">Sipariş Notu:</span> <span className="text-yellow-700">{detailModal.notes}</span>
              </div>
            )}
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
