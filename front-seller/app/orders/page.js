'use client';

import { useState } from 'react';
import useSWR from 'swr';
import SellerLayout from '@/components/layout/SellerLayout';
import Badge from '@/components/ui/Badge';
import Button from '@/components/ui/Button';
import Modal from '@/components/ui/Modal';
import { useToast } from '@/components/ui/Toast';
import fetcher from '@/lib/fetcher';
import api from '@/lib/api';

const STATUS_MAP = {
  1: { label: 'Beklemede', color: 'yellow' },
  2: { label: 'Onaylandı', color: 'blue' },
  3: { label: 'Hazırlanıyor', color: 'purple' },
  4: { label: 'Yolda', color: 'orange' },
  5: { label: 'Teslim Edildi', color: 'green' },
  6: { label: 'İptal', color: 'red' },
  7: { label: 'Reddedildi', color: 'red' },
};

const NEXT_STATUSES = {
  1: [{ id: 2, label: 'Onayla' }, { id: 7, label: 'Reddet' }],
  2: [{ id: 3, label: 'Hazırlamaya Başla' }],
  3: [{ id: 4, label: 'Yola Çıktı' }],
  4: [{ id: 5, label: 'Teslim Edildi' }],
};

export default function OrdersPage() {
  const toast = useToast();
  const [selectedRestaurant, setSelectedRestaurant] = useState(null);
  const [statusFilter, setStatusFilter] = useState(null);
  const [page, setPage] = useState(1);
  const [detailModal, setDetailModal] = useState(null);
  const [updating, setUpdating] = useState(null);

  const { data: restaurantsData } = useSWR('/v1/seller/restaurant/list', fetcher);
  const restaurants = restaurantsData?.data || [];

  const ordersUrl = selectedRestaurant
    ? `/v1/seller/order/restaurant/${selectedRestaurant}?page=${page}&pageSize=20${statusFilter ? `&statusId=${statusFilter}` : ''}`
    : null;

  const { data: ordersData, isLoading, mutate } = useSWR(ordersUrl, fetcher);
  const orders = ordersData?.data || [];
  const total = ordersData?.totalDataCount || 0;
  const totalPages = Math.ceil(total / 20) || 1;

  const handleStatusUpdate = async (orderId, statusId) => {
    setUpdating(orderId);
    try {
      await api.put(`/v1/seller/order/${orderId}/status?statusId=${statusId}`);
      toast('Sipariş durumu güncellendi', 'success');
      mutate();
      if (detailModal?.id === orderId) setDetailModal(prev => ({ ...prev, statusId }));
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || 'Hata oluştu', 'error');
    } finally {
      setUpdating(null);
    }
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
              <button
                onClick={() => { setStatusFilter(null); setPage(1); }}
                className={`px-3 py-1.5 rounded-lg text-xs font-medium ${!statusFilter ? 'bg-gray-800 text-white' : 'bg-white border border-gray-200 text-gray-600'}`}
              >
                Tümü
              </button>
              {Object.entries(STATUS_MAP).map(([id, s]) => (
                <button
                  key={id}
                  onClick={() => { setStatusFilter(Number(id)); setPage(1); }}
                  className={`px-3 py-1.5 rounded-lg text-xs font-medium ${statusFilter === Number(id) ? 'bg-gray-800 text-white' : 'bg-white border border-gray-200 text-gray-600'}`}
                >
                  {s.label}
                </button>
              ))}
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
                {orders.map(order => (
                  <div
                    key={order.id}
                    className="bg-white rounded-xl border border-gray-100 shadow-sm p-4 flex items-center gap-4"
                  >
                    <div className="flex-1 min-w-0">
                      <div className="flex items-center gap-2 mb-1">
                        <span className="font-mono text-xs text-gray-400">{order.id.substring(0, 8)}</span>
                        <Badge label={STATUS_MAP[order.statusId]?.label || '?'} color={STATUS_MAP[order.statusId]?.color || 'gray'} />
                      </div>
                      <div className="flex items-center gap-4">
                        <span className="text-sm text-gray-600">{order.items?.length || 0} ürün</span>
                        <span className="text-sm font-bold text-emerald-600">₺{Number(order.totalPrice).toFixed(2)}</span>
                        <span className="text-xs text-gray-400">{new Date(order.createdDate).toLocaleString('tr-TR')}</span>
                      </div>
                    </div>
                    <div className="flex gap-2 shrink-0">
                      {(NEXT_STATUSES[order.statusId] || []).map(ns => (
                        <Button
                          key={ns.id}
                          size="sm"
                          variant={ns.id === 7 ? 'danger' : 'primary'}
                          loading={updating === order.id}
                          onClick={() => handleStatusUpdate(order.id, ns.id)}
                        >
                          {ns.label}
                        </Button>
                      ))}
                      <Button size="sm" variant="ghost" onClick={() => setDetailModal(order)}>
                        Detay
                      </Button>
                    </div>
                  </div>
                ))}
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

      {/* Order Detail Modal */}
      <Modal isOpen={!!detailModal} onClose={() => setDetailModal(null)} title="Sipariş Detayı" size="md">
        {detailModal && (
          <div className="space-y-4">
            <div className="flex items-center gap-2">
              <span className="font-mono text-xs text-gray-400">{detailModal.id}</span>
              <Badge label={STATUS_MAP[detailModal.statusId]?.label} color={STATUS_MAP[detailModal.statusId]?.color} />
            </div>
            <div className="space-y-2">
              {(detailModal.items || []).map(item => (
                <div key={item.id} className="flex items-center justify-between text-sm">
                  <span className="text-gray-700">{item.quantity}x {item.menuName}</span>
                  <span className="font-medium">₺{Number(item.totalPrice).toFixed(2)}</span>
                </div>
              ))}
            </div>
            <div className="border-t border-gray-100 pt-3 space-y-1">
              <div className="flex justify-between text-sm text-gray-500">
                <span>Ürünler</span><span>₺{Number(detailModal.totalProductPrice).toFixed(2)}</span>
              </div>
              <div className="flex justify-between text-sm text-gray-500">
                <span>Teslimat</span><span>₺{Number(detailModal.shipmentPrice).toFixed(2)}</span>
              </div>
              <div className="flex justify-between text-base font-bold text-gray-800 pt-1">
                <span>Toplam</span><span>₺{Number(detailModal.totalPrice).toFixed(2)}</span>
              </div>
            </div>
            {detailModal.notes && (
              <div className="bg-yellow-50 rounded-lg p-3 text-sm text-yellow-700">
                <span className="font-medium">Not: </span>{detailModal.notes}
              </div>
            )}
            <div className="flex gap-2 pt-2">
              {(NEXT_STATUSES[detailModal.statusId] || []).map(ns => (
                <Button
                  key={ns.id}
                  size="sm"
                  variant={ns.id === 7 ? 'danger' : 'primary'}
                  loading={updating === detailModal.id}
                  onClick={() => handleStatusUpdate(detailModal.id, ns.id)}
                >
                  {ns.label}
                </Button>
              ))}
            </div>
          </div>
        )}
      </Modal>
    </SellerLayout>
  );
}
