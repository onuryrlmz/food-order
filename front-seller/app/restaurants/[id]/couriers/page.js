'use client';

import { use, useState } from 'react';
import useSWR from 'swr';
import SellerLayout from '@/components/layout/SellerLayout';
import Button from '@/components/ui/Button';
import Input from '@/components/ui/Input';
import Modal from '@/components/ui/Modal';
import Badge from '@/components/ui/Badge';
import { useToast } from '@/components/ui/Toast';
import fetcher from '@/lib/fetcher';
import { addCourier, removeCourier, getCourierOrderHistory } from '@/lib/api';

const STATUS_MAP = {
  0: { label: 'Onay Bekliyor', color: 'yellow' },
  1: { label: 'Aktif', color: 'green' },
  2: { label: 'Reddedildi', color: 'red' },
  3: { label: 'Kurye Ayrıldı', color: 'gray' },
  4: { label: 'Sonlandırıldı', color: 'red' },
};

const MONTHS = [
  'Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran',
  'Temmuz', 'Ağustos', 'Eylül', 'Ekim', 'Kasım', 'Aralık',
];

export default function CouriersPage({ params }) {
  const { id } = use(params);
  const toast = useToast();

  // Add courier state
  const [addModal, setAddModal] = useState(false);
  const [email, setEmail] = useState('');
  const [adding, setAdding] = useState(false);
  const [removing, setRemoving] = useState(null);

  // History modal state
  const [historyModal, setHistoryModal] = useState(null);
  const [historyMonth, setHistoryMonth] = useState(new Date().getMonth() + 1);
  const [historyYear, setHistoryYear] = useState(new Date().getFullYear());
  const [historyOrders, setHistoryOrders] = useState([]);
  const [historyLoading, setHistoryLoading] = useState(false);

  const { data: couriersData, mutate: mutateCouriers } = useSWR(
    `/v1/seller/restaurant/${id}/couriers`,
    fetcher
  );

  const couriers = couriersData?.data || [];

  const handleAdd = async (e) => {
    e.preventDefault();
    setAdding(true);
    try {
      const result = await addCourier(id, email);
      if (!result.hasFailed) {
        toast.success('Kurye eklendi');
        setEmail('');
        setAddModal(false);
        mutateCouriers();
      } else {
        const errMsg = result.messages?.map(m => m.description).join(', ') || 'Hata oluştu';
        toast.error(errMsg);
      }
    } catch (err) {
      toast.error('İstek başarısız');
    } finally {
      setAdding(false);
    }
  };

  const handleRemove = async (courierId) => {
    if (!confirm('Bu kuryeyi çıkarmak istediğinizden emin misiniz?')) return;
    setRemoving(courierId);
    try {
      const result = await removeCourier(id, courierId);
      if (!result.hasFailed) {
        toast.success('Kurye çıkarıldı');
        mutateCouriers();
      } else {
        const errMsg = result.messages?.map(m => m.description).join(', ') || 'Hata oluştu';
        toast.error(errMsg);
      }
    } catch (err) {
      toast.error('İstek başarısız');
    } finally {
      setRemoving(null);
    }
  };

  const openHistory = (courier) => {
    setHistoryModal(courier);
    const m = new Date().getMonth() + 1;
    const y = new Date().getFullYear();
    setHistoryMonth(m);
    setHistoryYear(y);
    fetchHistory(courier.courierId, m, y);
  };

  const fetchHistory = async (courierId, month, year) => {
    setHistoryLoading(true);
    try {
      const result = await getCourierOrderHistory(courierId, month, year);
      setHistoryOrders(!result.hasFailed ? (result.data || []) : []);
    } catch {
      setHistoryOrders([]);
    } finally {
      setHistoryLoading(false);
    }
  };

  const goHistoryPrev = () => {
    const m = historyMonth === 1 ? 12 : historyMonth - 1;
    const y = historyMonth === 1 ? historyYear - 1 : historyYear;
    setHistoryMonth(m);
    setHistoryYear(y);
    fetchHistory(historyModal.courierId, m, y);
  };

  const goHistoryNext = () => {
    const now = new Date();
    if (historyMonth === now.getMonth() + 1 && historyYear === now.getFullYear()) return;
    const m = historyMonth === 12 ? 1 : historyMonth + 1;
    const y = historyMonth === 12 ? historyYear + 1 : historyYear;
    setHistoryMonth(m);
    setHistoryYear(y);
    fetchHistory(historyModal.courierId, m, y);
  };

  return (
    <SellerLayout>
      <div className="max-w-5xl mx-auto p-6">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-2xl font-bold">Kuryelerim</h1>
          <Button onClick={() => setAddModal(true)}>
            + Yeni Kurye Ekle
          </Button>
        </div>

        {couriers.length === 0 ? (
          <div className="text-center py-12 text-gray-500">
            <svg className="w-12 h-12 mx-auto mb-3 text-gray-300" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M13 16V6a1 1 0 00-1-1H4a1 1 0 00-1 1v10a1 1 0 001 1h1m8-1a1 1 0 01-1 1H9m4-1V8a1 1 0 011-1h2.586a1 1 0 01.707.293l3.414 3.414a1 1 0 01.293.707V16a1 1 0 01-1 1h-1m-6-1a1 1 0 001 1h1M5 17a2 2 0 104 0m-4 0a2 2 0 114 0m6 0a2 2 0 104 0m-4 0a2 2 0 114 0" />
            </svg>
            <p className="font-medium">Henüz kurye eklemediniz.</p>
            <p className="text-sm mt-1">Anlaştığınız kuryelerin e-posta adresini girerek ekleyebilirsiniz.</p>
          </div>
        ) : (
          <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
            <table className="min-w-full divide-y divide-gray-100">
              <thead className="bg-gray-50/80">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">Ad Soyad</th>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">E-posta</th>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">Telefon</th>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">Durum</th>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">Anlaşma Tarihi</th>
                  <th className="px-6 py-3 text-right text-xs font-semibold text-gray-500 uppercase tracking-wider">İşlem</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-50">
                {couriers.map((courier) => (
                  <tr key={courier.courierId} className="hover:bg-gray-50/50 transition-colors">
                    <td className="px-6 py-4 font-medium text-gray-800">
                      {courier.firstName || courier.lastName
                        ? `${courier.firstName || ''} ${courier.lastName || ''}`.trim()
                        : '-'}
                    </td>
                    <td className="px-6 py-4 text-sm text-gray-500">{courier.email || '-'}</td>
                    <td className="px-6 py-4 text-sm text-gray-500">{courier.phoneNumber || '-'}</td>
                    <td className="px-6 py-4">
                      <Badge
                        label={STATUS_MAP[courier.statusId]?.label || courier.statusName}
                        color={STATUS_MAP[courier.statusId]?.color || 'gray'}
                      />
                    </td>
                    <td className="px-6 py-4 text-sm text-gray-400">
                      {courier.agreementStartDate
                        ? new Date(courier.agreementStartDate).toLocaleDateString('tr-TR')
                        : new Date(courier.createdDate).toLocaleDateString('tr-TR')}
                    </td>
                    <td className="px-6 py-4 text-right space-x-3">
                      {courier.statusId === 1 && (
                        <button
                          onClick={() => openHistory(courier)}
                          className="text-blue-600 hover:text-blue-800 text-sm font-medium"
                        >
                          Geçmiş
                        </button>
                      )}
                      <button
                        onClick={() => handleRemove(courier.courierId)}
                        disabled={removing === courier.courierId}
                        className="text-red-600 hover:text-red-800 text-sm font-medium disabled:opacity-50"
                      >
                        {removing === courier.courierId ? 'Çıkarılıyor...' : (courier.statusId === 0 ? 'İptal Et' : 'Sonlandır')}
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {/* Yeni Kurye Ekle Modal */}
      <Modal isOpen={addModal} onClose={() => setAddModal(false)} title="Yeni Kurye Ekle">
        <form onSubmit={handleAdd} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Kurye E-posta Adresi
            </label>
            <Input
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="kurye@ornek.com"
              required
            />
            <p className="text-xs text-gray-500 mt-1">
              Kuryenin kayıt olduğu e-posta adresini girin. Kurye aktif ve onaylanmış olmalıdır.
            </p>
          </div>
          <div className="flex justify-end gap-2">
            <Button type="button" variant="outline" onClick={() => setAddModal(false)}>İptal</Button>
            <Button type="submit" disabled={adding}>
              {adding ? 'Ekleniyor...' : 'Kurye Ekle'}
            </Button>
          </div>
        </form>
      </Modal>

      {/* Sipariş Geçmişi Modal */}
      <Modal
        isOpen={!!historyModal}
        onClose={() => { setHistoryModal(null); setHistoryOrders([]); }}
        title={`Sipariş Geçmişi — ${historyModal?.firstName || ''} ${historyModal?.lastName || ''}`.trim()}
        size="xl"
      >
        <div className="space-y-4">
          {/* Month Selector */}
          <div className="flex items-center justify-center gap-4">
            <button onClick={goHistoryPrev} className="p-1.5 rounded-lg hover:bg-gray-100 text-orange-500 font-bold text-lg">◀</button>
            <span className="text-sm font-semibold text-gray-700 min-w-[130px] text-center">
              {MONTHS[historyMonth - 1]} {historyYear}
            </span>
            <button onClick={goHistoryNext} className="p-1.5 rounded-lg hover:bg-gray-100 text-orange-500 font-bold text-lg">▶</button>
          </div>

          {historyLoading ? (
            <div className="text-center py-8 text-gray-400">Yükleniyor...</div>
          ) : historyOrders.length === 0 ? (
            <div className="text-center py-8 text-gray-400">
              <p>{MONTHS[historyMonth - 1]} {historyYear} için sipariş bulunamadı.</p>
            </div>
          ) : (
            <>
              <p className="text-xs text-gray-400 text-right">{historyOrders.length} sipariş</p>
              <div className="max-h-[400px] overflow-y-auto space-y-2">
                {historyOrders.map((order) => (
                  <div key={order.orderId} className="flex items-center justify-between p-3 rounded-lg bg-gray-50 border border-gray-100">
                    <div>
                      <p className="text-sm font-medium text-gray-800">{order.restaurantName}</p>
                      <p className="text-xs text-gray-400">{order.deliveryArea}</p>
                    </div>
                    <div className="text-right">
                      <Badge label={order.statusName} color="green" />
                      <p className="text-xs text-gray-400 mt-1">
                        {new Date(order.createdDate).toLocaleDateString('tr-TR')}
                      </p>
                    </div>
                  </div>
                ))}
              </div>
            </>
          )}
        </div>
      </Modal>
    </SellerLayout>
  );
}
