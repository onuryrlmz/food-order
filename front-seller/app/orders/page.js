'use client';

import { useState, useEffect, useRef } from 'react';
import useSWR from 'swr';
import SellerLayout from '@/components/layout/SellerLayout';
import Badge from '@/components/ui/Badge';
import Button from '@/components/ui/Button';
import Modal from '@/components/ui/Modal';
import { useToast } from '@/components/ui/Toast';
import fetcher from '@/lib/fetcher';
import api from '@/lib/api';
import { createRestaurantConnection } from '@/lib/signalr';

const STATUS_MAP = {
  4: { label: 'Onay Bekliyor', color: 'yellow' },
  5: { label: 'Reddedildi', color: 'red' },
  6: { label: 'Hazırlanıyor', color: 'purple' },
  7: { label: 'Yola Çıktı', color: 'blue' },
  8: { label: 'Teslim Edildi', color: 'green' },
};

// Satıcının yapabileceği geçişler (sadece 4 ve 6 durumlarında)
const NEXT_STATUSES = {
  4: [{ id: 6, label: 'Onayla', variant: 'primary' }, { id: 5, label: 'Reddet', variant: 'danger' }],
  6: [{ id: 7, label: 'Yola Çıktı', variant: 'primary' }],
};

const FILTER_TABS = [
  { key: null, label: 'Tümü' },
  { key: 4, label: 'Onay Bekliyor' },
  { key: 6, label: 'Hazırlanıyor' },
  { key: 7, label: 'Yolda' },
  { key: 8, label: 'Teslim Edildi' },
  { key: 5, label: 'Reddedilen' },
];

const PAYMENT_OPTION_MAP = {
  1: 'Online Kredi Kartı',
  2: 'Kapıda Nakit',
  3: 'Kapıda Kredi Kartı',
};

export default function OrdersPage() {
  const toast = useToast();
  const [selectedRestaurant, setSelectedRestaurant] = useState(null);
  const [statusFilter, setStatusFilter] = useState(null);
  const [page, setPage] = useState(1);
  const [detailModal, setDetailModal] = useState(null);
  const [updating, setUpdating] = useState(null);
  const [rejectModal, setRejectModal] = useState(null);
  const { data: restaurantsData } = useSWR('/v1/seller/restaurant/list', fetcher);
  const restaurants = restaurantsData?.data || [];

  const ordersUrl = selectedRestaurant
    ? `/v1/seller/order/restaurant/${selectedRestaurant}?page=${page}&pageSize=20${statusFilter ? `&statusId=${statusFilter}` : ''}`
    : null;

  const { data: ordersData, isLoading, mutate } = useSWR(ordersUrl, fetcher);
  const orders = ordersData?.data || [];
  const total = ordersData?.totalDataCount || 0;
  const totalPages = Math.ceil(total / 20) || 1;

  // SignalR real-time updates
  const connectionRef = useRef(null);
  useEffect(() => {
    if (!selectedRestaurant) return;

    const conn = createRestaurantConnection(selectedRestaurant);
    connectionRef.current = conn;

    conn.on('NewOrder', () => {
      toast('Yeni siparis geldi!', 'info');
      mutate();
    });

    conn.on('OrderStatusChanged', () => {
      mutate();
    });

    return () => {
      if (connectionRef.current) {
        connectionRef.current.stop().catch(() => {});
        connectionRef.current = null;
      }
    };
  }, [selectedRestaurant]);

  const handleStatusUpdate = async (orderId, statusId) => {
    setUpdating(orderId);
    try {
      let url = `/v1/seller/order/${orderId}/status?statusId=${statusId}`;
      await api.put(url);
      const statusLabel = STATUS_MAP[statusId]?.label || 'Güncellendi';
      toast(`Sipariş durumu: ${statusLabel}`, 'success');
      mutate();
      if (detailModal?.id === orderId) setDetailModal(prev => ({ ...prev, statusId }));
      if (rejectModal?.id === orderId) setRejectModal(null);
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || 'Hata oluştu', 'error');
    } finally {
      setUpdating(null);
    }
  };

  const handleReject = (order) => {
    setRejectModal(order);
  };

  const confirmReject = () => {
    if (rejectModal) handleStatusUpdate(rejectModal.id, 5);
  };

  return (
    <SellerLayout title="Siparişler">
      <div className="space-y-4">
        {/* Restoran seç */}
        {restaurants.length > 0 && (
          <div className="bg-white rounded-xl border border-gray-100 shadow-sm p-4">
            <p className="text-xs font-semibold text-gray-400 uppercase mb-2">Restoran Seç</p>
            <div className="flex flex-wrap gap-2">
              {restaurants.map(r => (
                <button
                  key={r.id}
                  onClick={() => { setSelectedRestaurant(r.id); setPage(1); }}
                  className={`px-3 py-1.5 rounded-lg text-sm font-medium transition-colors ${
                    selectedRestaurant === r.id
                      ? 'bg-emerald-600 text-white'
                      : 'bg-gray-100 text-gray-600 hover:bg-gray-200'
                  }`}
                >
                  {r.name}
                </button>
              ))}
            </div>
          </div>
        )}

        {!selectedRestaurant ? (
          <div className="text-center py-20 text-gray-400">
            <p className="text-sm">Siparişleri görmek için bir restoran seçin</p>
          </div>
        ) : (
          <>
            {/* Durum filtresi */}
            <div className="flex flex-wrap gap-2">
              {FILTER_TABS.map(tab => (
                <button
                  key={String(tab.key)}
                  onClick={() => { setStatusFilter(tab.key); setPage(1); }}
                  className={`px-3 py-1.5 rounded-lg text-xs font-medium transition-colors ${
                    statusFilter === tab.key
                      ? 'bg-emerald-600 text-white'
                      : 'bg-white border border-gray-200 text-gray-600 hover:bg-gray-50'
                  }`}
                >
                  {tab.label}
                </button>
              ))}
              <span className="text-sm text-gray-400 self-center ml-2">{total} sipariş</span>
            </div>

            {isLoading ? (
              <div className="flex justify-center py-10">
                <div className="w-8 h-8 border-2 border-emerald-500 border-t-transparent rounded-full animate-spin" />
              </div>
            ) : orders.length === 0 ? (
              <div className="bg-white rounded-xl border border-gray-100 shadow-sm p-10 text-center">
                <p className="text-gray-400 text-sm">Bu filtrede sipariş bulunamadı</p>
              </div>
            ) : (
              <div className="space-y-2">
                {orders.map(order => {
                  const actions = NEXT_STATUSES[order.statusId] || [];
                  return (
                    <div
                      key={order.id}
                      className={`bg-white rounded-xl border shadow-sm p-4 flex items-center gap-4 ${
                        order.statusId === 4 ? 'border-yellow-300 bg-yellow-50/30' : 'border-gray-100'
                      }`}
                    >
                      <div className="flex-1 min-w-0">
                        <div className="flex items-center gap-2 mb-1">
                          <span className="font-mono text-xs text-gray-400">{order.id.substring(0, 8)}</span>
                          <Badge label={STATUS_MAP[order.statusId]?.label || '?'} color={STATUS_MAP[order.statusId]?.color || 'gray'} />
                          <span className="text-sm font-bold text-emerald-600">₺{Number(order.totalPrice).toFixed(2)}</span>
                          <span className="text-xs text-gray-400">{PAYMENT_OPTION_MAP[order.paymentOptionId] || ''}</span>
                          <span className="text-xs text-gray-400 ml-auto">{new Date(order.createdDate).toLocaleString('tr-TR')}</span>
                        </div>
                        <div className="text-xs text-gray-500">
                          {(order.items || []).map((item, i) => (
                            <span key={item.id}>
                              {i > 0 && <span className="text-gray-300"> · </span>}
                              <span>{item.quantity}x {item.menuName}</span>
                              {item.values?.length > 0 && (
                                <span className="text-gray-400"> ({item.values.map(v => v.valueName).join(', ')})</span>
                              )}
                            </span>
                          ))}
                        </div>
                      </div>
                      <div className="flex gap-2 shrink-0">
                        {actions.map(ns => (
                          <Button
                            key={ns.id}
                            size="sm"
                            variant={ns.variant}
                            loading={updating === order.id}
                            onClick={() => ns.id === 5 ? handleReject(order) : handleStatusUpdate(order.id, ns.id)}
                          >
                            {ns.label}
                          </Button>
                        ))}
                        <Button size="sm" variant="ghost" onClick={() => setDetailModal(order)}>
                          Detay
                        </Button>
                      </div>
                    </div>
                  );
                })}
              </div>
            )}

            {totalPages > 1 && (
              <div className="flex justify-center gap-2">
                <Button variant="outline" size="sm" onClick={() => setPage(p => Math.max(1, p - 1))} disabled={page === 1}>← Önceki</Button>
                <span className="text-sm text-gray-500 self-center">{page} / {totalPages}</span>
                <Button variant="outline" size="sm" onClick={() => setPage(p => Math.min(totalPages, p + 1))} disabled={page === totalPages}>Sonraki →</Button>
              </div>
            )}
          </>
        )}
      </div>

      {/* Sipariş Detay Modal */}
      <Modal isOpen={!!detailModal} onClose={() => setDetailModal(null)} title="Sipariş Detayı" size="md">
        {detailModal && (
          <div className="space-y-4">
            <div className="flex items-center gap-2">
              <span className="font-mono text-xs text-gray-400">{detailModal.id}</span>
              <Badge label={STATUS_MAP[detailModal.statusId]?.label} color={STATUS_MAP[detailModal.statusId]?.color} />
              <span className="text-xs text-gray-400 ml-auto">{PAYMENT_OPTION_MAP[detailModal.paymentOptionId] || ''}</span>
            </div>

            <div className="space-y-2">
              <h4 className="text-xs font-semibold text-gray-400 uppercase">Ürünler</h4>
              {(detailModal.items || []).map(item => (
                <div key={item.id} className="bg-gray-50 rounded-lg p-3">
                  <div className="flex items-start justify-between">
                    <div className="flex-1 min-w-0">
                      <p className="text-sm font-medium text-gray-800">{item.menuName}</p>
                      {item.description && <p className="text-xs text-gray-400 mt-0.5 line-clamp-2">{item.description}</p>}
                    </div>
                    <span className="text-sm font-bold text-gray-800 ml-3 shrink-0">₺{Number(item.totalPrice).toFixed(2)}</span>
                  </div>
                  <div className="flex items-center gap-3 mt-1.5 text-xs text-gray-500">
                    <span>Adet: <span className="font-medium text-gray-700">{item.quantity}</span></span>
                    <span>Birim: <span className="font-medium text-gray-700">₺{Number(item.unitPrice).toFixed(2)}</span></span>
                  </div>
                  {item.values?.length > 0 && (
                    <div className="mt-2 border-t border-gray-200 pt-2 space-y-1.5">
                      {item.values.map((val, vi) => (
                        <div key={vi}>
                          <div className="flex items-center justify-between text-xs">
                            <span className="text-gray-600">
                              <span className="font-medium text-gray-500">{val.optionName}:</span> {val.valueName}
                              {val.quantity > 1 && <span className="text-gray-400"> x{val.quantity}</span>}
                            </span>
                            {val.unitPrice > 0 && <span className="text-gray-500">+₺{Number(val.totalPrice).toFixed(2)}</span>}
                          </div>
                          {val.options?.length > 0 && (
                            <div className="ml-3 mt-0.5 space-y-0.5">
                              {val.options.map((opt, oi) => (
                                <div key={oi} className="flex items-center justify-between text-xs text-gray-400">
                                  <span>
                                    <span className="font-medium">{opt.optionName}:</span> {opt.valueName}
                                    {opt.quantity > 1 && <span> x{opt.quantity}</span>}
                                  </span>
                                  {opt.unitPrice > 0 && <span>+₺{Number(opt.totalPrice).toFixed(2)}</span>}
                                </div>
                              ))}
                            </div>
                          )}
                        </div>
                      ))}
                    </div>
                  )}
                </div>
              ))}
            </div>

            <div className="border-t border-gray-100 pt-3 space-y-1">
              <div className="flex justify-between text-sm text-gray-500">
                <span>Ürünler</span><span>₺{Number(detailModal.totalProductPrice || 0).toFixed(2)}</span>
              </div>
              <div className="flex justify-between text-sm text-gray-500">
                <span>Teslimat</span><span>₺{Number(detailModal.shipmentPrice || 0).toFixed(2)}</span>
              </div>
              {detailModal.discountAmount > 0 && (
                <div className="flex justify-between text-sm text-green-600">
                  <span>İndirim</span><span>-₺{Number(detailModal.discountAmount).toFixed(2)}</span>
                </div>
              )}
              <div className="flex justify-between text-base font-bold text-gray-800 pt-1">
                <span>Toplam</span><span>₺{Number(detailModal.totalPrice || 0).toFixed(2)}</span>
              </div>
            </div>

            {detailModal.notes && (
              <div className="bg-yellow-50 rounded-lg p-3 text-sm text-yellow-700">
                <span className="font-medium">Müşteri Notu: </span>{detailModal.notes}
              </div>
            )}

            {detailModal.cancellationReason && (
              <div className="bg-red-50 rounded-lg p-3 text-sm text-red-700">
                <span className="font-medium">İptal/Red Nedeni: </span>{detailModal.cancellationReason}
              </div>
            )}

            <div className="flex gap-2 pt-2">
              {(NEXT_STATUSES[detailModal.statusId] || []).map(ns => (
                <Button
                  key={ns.id}
                  size="sm"
                  variant={ns.variant}
                  loading={updating === detailModal.id}
                  onClick={() => ns.id === 5 ? handleReject(detailModal) : handleStatusUpdate(detailModal.id, ns.id)}
                >
                  {ns.label}
                </Button>
              ))}
            </div>
          </div>
        )}
      </Modal>

      {/* Reddetme Onay Modal */}
      <Modal isOpen={!!rejectModal} onClose={() => setRejectModal(null)} title="Siparişi Reddet" size="sm">
        <div className="space-y-4">
          <div className="bg-red-50 rounded-lg p-4 text-sm text-red-700">
            <p className="font-medium mb-1">Bu siparişi reddetmek istediğinize emin misiniz?</p>
            <p className="text-xs text-red-500">Bu işlem geri alınamaz. Müşteriye bildirim gönderilecektir.</p>
          </div>
          <div className="bg-gray-50 rounded-lg p-3 text-sm">
            <p className="text-gray-500 text-xs mb-1">Sipariş</p>
            <p className="font-mono text-xs">{rejectModal?.id?.substring(0, 8)}</p>
            <p className="font-bold text-emerald-600 mt-1">₺{Number(rejectModal?.totalPrice || 0).toFixed(2)}</p>
          </div>
          <div className="flex justify-end gap-2">
            <Button variant="secondary" onClick={() => setRejectModal(null)}>Vazgeç</Button>
            <Button variant="danger" loading={updating === rejectModal?.id} onClick={confirmReject}>Reddet</Button>
          </div>
        </div>
      </Modal>
    </SellerLayout>
  );
}
