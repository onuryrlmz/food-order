'use client';

import { useState } from 'react';
import useSWR from 'swr';
import SellerLayout from '@/components/layout/SellerLayout';
import Badge from '@/components/ui/Badge';
import Button from '@/components/ui/Button';
import Modal from '@/components/ui/Modal';
import { useToast } from '@/components/ui/Toast';
import fetcher from '@/lib/fetcher';
import api, { toggleAutoRenew } from '@/lib/api';

const STATUS_MAP = {
  1: { label: 'Aktif', color: 'green' },
  2: { label: 'Süresi Doldu', color: 'red' },
  3: { label: 'İptal Edildi', color: 'gray' },
  4: { label: 'Askıya Alındı', color: 'yellow' },
};

export default function SubscriptionPage() {
  const toast = useToast();
  const [subscribeModal, setSubscribeModal] = useState(false);
  const [selectedPlan, setSelectedPlan] = useState(null);
  const [selectedRestaurant, setSelectedRestaurant] = useState('');
  const [subscribing, setSubscribing] = useState(false);
  const [cancelling, setCancelling] = useState(null);
  const [togglingRenew, setTogglingRenew] = useState(null);

  const { data: subsData, mutate } = useSWR('/v1/seller/subscription/my', fetcher);
  const { data: plansData } = useSWR('/v1/seller/subscription/plans', fetcher);
  const { data: restaurantsData } = useSWR('/v1/seller/restaurant/list', fetcher);

  const subscriptions = subsData?.data || [];
  const plans = plansData?.data || [];
  const restaurants = restaurantsData?.data || [];

  const handleSubscribe = async () => {
    if (!selectedPlan || !selectedRestaurant) {
      toast('Plan ve restoran seçin', 'error');
      return;
    }
    setSubscribing(true);
    try {
      await api.post('/v1/seller/subscription/subscribe', {
        subscriptionPlanId: selectedPlan,
        restaurantId: selectedRestaurant,
      });
      toast('Abonelik başlatıldı', 'success');
      setSubscribeModal(false);
      setSelectedPlan(null);
      setSelectedRestaurant('');
      mutate();
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || 'Hata oluştu', 'error');
    } finally {
      setSubscribing(false);
    }
  };

  const handleCancel = async (id) => {
    setCancelling(id);
    try {
      await api.post(`/v1/seller/subscription/${id}/cancel`);
      toast('Abonelik iptal edildi', 'success');
      mutate();
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || 'Hata oluştu', 'error');
    } finally {
      setCancelling(null);
    }
  };

  const handleToggleAutoRenew = async (sub) => {
    setTogglingRenew(sub.id);
    try {
      const res = await toggleAutoRenew(sub.restaurantId, !sub.autoRenew);
      if (!res.hasFailed) {
        toast(sub.autoRenew ? 'Otomatik yenileme kapatildi' : 'Otomatik yenileme acildi', 'success');
        mutate();
      } else {
        toast(res.messages?.[0]?.description || 'Hata olustu', 'error');
      }
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || 'Hata olustu', 'error');
    } finally {
      setTogglingRenew(null);
    }
  };

  const planTypeLabel = (t) => t === 1 ? 'Basic' : t === 2 ? 'Pro' : 'Premium';
  const planColor = (t) => t === 1 ? 'gray' : t === 2 ? 'blue' : 'purple';

  return (
    <SellerLayout
      title="Abonelik"
      headerActions={
        <Button onClick={() => setSubscribeModal(true)}>
          <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
          </svg>
          Abone Ol
        </Button>
      }
    >
      <div className="space-y-6">
        {/* Mevcut abonelikler */}
        <div>
          <h2 className="text-sm font-semibold text-gray-500 uppercase mb-3">Aboneliklerim</h2>
          {subscriptions.length === 0 ? (
            <div className="bg-white rounded-xl border border-gray-100 shadow-sm p-10 text-center">
              <p className="text-gray-400 text-sm mb-3">Henüz abonelik yok</p>
              <Button onClick={() => setSubscribeModal(true)}>İlk Aboneliği Başlat</Button>
            </div>
          ) : (
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              {subscriptions.map(s => (
                <div key={s.id} className="bg-white rounded-xl border border-gray-100 shadow-sm p-5">
                  <div className="flex items-start justify-between mb-3">
                    <div>
                      <p className="font-semibold text-gray-800">{s.restaurantName}</p>
                      <p className="text-sm text-gray-500 mt-0.5">{s.planName} · ₺{s.monthlyPrice}/ay</p>
                    </div>
                    <Badge label={STATUS_MAP[s.statusId]?.label || '?'} color={STATUS_MAP[s.statusId]?.color || 'gray'} />
                  </div>

                  <div className="space-y-1 text-xs text-gray-500">
                    <div className="flex justify-between">
                      <span>Başlangıç</span>
                      <span>{new Date(s.startDate).toLocaleDateString('tr-TR')}</span>
                    </div>
                    <div className="flex justify-between">
                      <span>Bitiş</span>
                      <span>{new Date(s.endDate).toLocaleDateString('tr-TR')}</span>
                    </div>
                    <div className="flex justify-between font-medium">
                      <span>Kalan</span>
                      <span className={s.daysRemaining <= 7 ? 'text-red-500' : 'text-emerald-600'}>
                        {s.daysRemaining} gün
                      </span>
                    </div>
                  </div>

                  {s.daysRemaining <= 7 && s.statusId === 1 && (
                    <div className="mt-3 bg-yellow-50 border border-yellow-200 rounded-lg p-2 text-xs text-yellow-700">
                      Aboneliğiniz yakında sona eriyor! Yenilemek için abone olun.
                    </div>
                  )}

                  {s.statusId === 1 && (
                    <div className="mt-3 flex items-center justify-between border-t border-gray-100 pt-3">
                      <div className="flex items-center gap-2">
                        <span className="text-xs font-medium text-gray-600">Otomatik Yenileme</span>
                        <button
                          onClick={() => handleToggleAutoRenew(s)}
                          disabled={togglingRenew === s.id}
                          className={`relative inline-flex h-5 w-9 items-center rounded-full transition-colors ${
                            s.autoRenew ? 'bg-emerald-500' : 'bg-gray-300'
                          } ${togglingRenew === s.id ? 'opacity-50' : ''}`}
                        >
                          <span
                            className={`inline-block h-3.5 w-3.5 rounded-full bg-white transition-transform ${
                              s.autoRenew ? 'translate-x-4' : 'translate-x-0.5'
                            }`}
                          />
                        </button>
                      </div>
                    </div>
                  )}

                  {s.statusId === 1 && (
                    <Button
                      size="sm"
                      variant="danger"
                      className="mt-3 w-full justify-center"
                      loading={cancelling === s.id}
                      onClick={() => handleCancel(s.id)}
                    >
                      İptal Et
                    </Button>
                  )}
                </div>
              ))}
            </div>
          )}
        </div>

        {/* Planlar */}
        <div>
          <h2 className="text-sm font-semibold text-gray-500 uppercase mb-3">Mevcut Planlar</h2>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            {plans.map(plan => (
              <div key={plan.id} className="bg-white rounded-xl border border-gray-100 shadow-sm p-5 flex flex-col">
                <div className="flex items-center gap-2 mb-3">
                  <Badge label={planTypeLabel(plan.planType)} color={planColor(plan.planType)} />
                  <span className="font-semibold text-gray-800">{plan.name}</span>
                </div>
                <p className="text-2xl font-bold text-gray-900 mb-1">₺{plan.monthlyPrice}<span className="text-sm font-normal text-gray-500">/ay</span></p>
                {plan.description && <p className="text-xs text-gray-500 mb-3">{plan.description}</p>}
                <p className="text-xs text-gray-400 mb-4">Maks. {plan.maxRestaurants} restoran</p>
                <Button
                  className="mt-auto w-full justify-center"
                  onClick={() => { setSelectedPlan(plan.id); setSubscribeModal(true); }}
                >
                  Bu Planı Seç
                </Button>
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* Subscribe Modal */}
      <Modal isOpen={subscribeModal} onClose={() => setSubscribeModal(false)} title="Abonelik Başlat" size="sm">
        <div className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1.5">Plan</label>
            <select
              value={selectedPlan || ''}
              onChange={e => setSelectedPlan(e.target.value)}
              className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-emerald-400"
            >
              <option value="">Plan seçin...</option>
              {plans.map(p => (
                <option key={p.id} value={p.id}>{p.name} — ₺{p.monthlyPrice}/ay</option>
              ))}
            </select>
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1.5">Restoran</label>
            <select
              value={selectedRestaurant}
              onChange={e => setSelectedRestaurant(e.target.value)}
              className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-emerald-400"
            >
              <option value="">Restoran seçin...</option>
              {restaurants.map(r => (
                <option key={r.id} value={r.id}>{r.name}</option>
              ))}
            </select>
          </div>
          <div className="flex justify-end gap-2 pt-2">
            <Button variant="secondary" onClick={() => setSubscribeModal(false)}>İptal</Button>
            <Button loading={subscribing} onClick={handleSubscribe}>Abone Ol</Button>
          </div>
        </div>
      </Modal>
    </SellerLayout>
  );
}
