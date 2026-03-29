'use client';

import { use, useState } from 'react';
import useSWR from 'swr';
import Link from 'next/link';
import SellerLayout from '@/components/layout/SellerLayout';
import Button from '@/components/ui/Button';
import Input from '@/components/ui/Input';
import Modal from '@/components/ui/Modal';
import Badge from '@/components/ui/Badge';
import { useToast } from '@/components/ui/Toast';
import fetcher from '@/lib/fetcher';
import sellerApi from '@/lib/service';

const DAYS = ['Pazartesi', 'Salı', 'Çarşamba', 'Perşembe', 'Cuma', 'Cumartesi', 'Pazar'];

export default function RestaurantDetailPage({ params }) {
  const { id } = use(params);
  const toast = useToast();
  const [activeTab, setActiveTab] = useState('info');
  const [editModal, setEditModal] = useState(false);
  const [editForm, setEditForm] = useState(null);
  const [saving, setSaving] = useState(false);
  const [savingHour, setSavingHour] = useState(null);

  const { data: hoursData, mutate: mutateHours } = useSWR(`/v1/seller/restaurant/${id}/working-hours`, fetcher);
  const { data: restaurantsData } = useSWR('/v1/seller/restaurant/list', fetcher);

  const restaurant = restaurantsData?.data?.find(r => r.id === id);
  const hours = hoursData?.data || [];

  const openEdit = () => {
    if (!restaurant) return;
    setEditForm({
      id,
      name: restaurant.name || '',
      phone: restaurant.phone || '',
      email: restaurant.email || '',
      description: '',
      minimumOrderPrice: 0,
      minDeliveryTime: 20,
      maxDeliveryTime: 45,
      coverImage: '',
    });
    setEditModal(true);
  };

  const handleUpdate = async (e) => {
    e.preventDefault();
    setSaving(true);
    try {
      await sellerApi.seller.restaurant.update({
        ...editForm,
        minimumOrderPrice: parseFloat(editForm.minimumOrderPrice),
        minDeliveryTime: parseInt(editForm.minDeliveryTime),
        maxDeliveryTime: parseInt(editForm.maxDeliveryTime),
      });
      toast('Restoran güncellendi', 'success');
      setEditModal(false);
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || 'Hata oluştu', 'error');
    } finally {
      setSaving(false);
    }
  };

  const handleUpsertHour = async (dayOfWeek, data) => {
    setSavingHour(dayOfWeek);
    try {
      await sellerApi.seller.restaurant.updateWorkingHours({ restaurantId: id, dayOfWeek, ...data });
      toast('Çalışma saati güncellendi', 'success');
      mutateHours();
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || 'Hata oluştu', 'error');
    } finally {
      setSavingHour(null);
    }
  };

  const set = (key) => (e) => setEditForm(f => ({ ...f, [key]: e.target.value }));

  return (
    <SellerLayout
      title={restaurant?.name || 'Restoran Detayı'}
      headerActions={
        <div className="flex gap-2">
          <Badge label={restaurant?.isActive ? 'Aktif' : 'Pasif'} color={restaurant?.isActive ? 'green' : 'red'} />
          <Badge label={restaurant?.isOpen ? 'Açık' : 'Kapalı'} color={restaurant?.isOpen ? 'blue' : 'gray'} />
          <Button size="sm" onClick={openEdit}>Düzenle</Button>
        </div>
      }
    >
      {/* Tabs */}
      <div className="flex gap-1 bg-gray-100 p-1 rounded-xl w-fit mb-6">
        {[
          { key: 'info', label: 'Bilgiler' },
          { key: 'hours', label: 'Çalışma Saatleri' },
        ].map(tab => (
          <button
            key={tab.key}
            onClick={() => setActiveTab(tab.key)}
            className={`px-4 py-2 rounded-lg text-sm font-medium transition-colors ${
              activeTab === tab.key ? 'bg-white text-emerald-600 shadow-sm' : 'text-gray-500 hover:text-gray-700'
            }`}
          >
            {tab.label}
          </button>
        ))}
        <Link
          href={`/restaurants/${id}/menu`}
          className="px-4 py-2 rounded-lg text-sm font-medium text-emerald-600 border border-emerald-200 hover:bg-emerald-50 transition-colors"
        >
          Menü Yönetimi →
        </Link>
      </div>

      {/* Info Tab */}
      {activeTab === 'info' && restaurant && (
        <div className="bg-white rounded-xl border border-gray-100 shadow-sm p-6 max-w-lg">
          <div className="space-y-3">
            {[
              { label: 'Ad', value: restaurant.name },
              { label: 'Telefon', value: restaurant.phone },
            ].map(f => (
              <div key={f.label} className="flex items-center justify-between py-2 border-b border-gray-50">
                <span className="text-sm text-gray-500">{f.label}</span>
                <span className="text-sm font-medium text-gray-800">{f.value || '—'}</span>
              </div>
            ))}
          </div>
          <Button className="mt-5" onClick={openEdit}>Bilgileri Düzenle</Button>
        </div>
      )}

      {/* Working Hours Tab */}
      {activeTab === 'hours' && (
        <div className="bg-white rounded-xl border border-gray-100 shadow-sm overflow-hidden max-w-xl">
          <div className="divide-y divide-gray-50">
            {DAYS.map((day, i) => {
              const dayNum = i + 1;
              const existing = hours.find(h => h.dayOfWeek === dayNum);
              return (
                <WorkingHourRow
                  key={dayNum}
                  day={day}
                  dayOfWeek={dayNum}
                  existing={existing}
                  saving={savingHour === dayNum}
                  onSave={handleUpsertHour}
                />
              );
            })}
          </div>
        </div>
      )}

      {/* Edit Modal */}
      <Modal isOpen={editModal} onClose={() => setEditModal(false)} title="Restoran Bilgilerini Düzenle" size="lg">
        {editForm && (
          <form onSubmit={handleUpdate} className="space-y-4">
            <div className="grid grid-cols-2 gap-3">
              <div className="col-span-2">
                <Input label="Restoran Adı" required value={editForm.name} onChange={set('name')} />
              </div>
              <Input label="Telefon" required value={editForm.phone} onChange={set('phone')} />
              <Input label="Email" type="email" value={editForm.email} onChange={set('email')} />
              <Input label="Min. Sipariş (₺)" type="number" value={editForm.minimumOrderPrice} onChange={set('minimumOrderPrice')} />
              <div className="grid grid-cols-2 gap-2">
                <Input label="Min. Teslimat (dk)" type="number" value={editForm.minDeliveryTime} onChange={set('minDeliveryTime')} />
                <Input label="Max. Teslimat (dk)" type="number" value={editForm.maxDeliveryTime} onChange={set('maxDeliveryTime')} />
              </div>
              <div className="col-span-2">
                <label className="block text-sm font-medium text-gray-700 mb-1.5">Açıklama</label>
                <textarea
                  value={editForm.description}
                  onChange={(e) => setEditForm(f => ({ ...f, description: e.target.value }))}
                  rows={2}
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-emerald-400 resize-none"
                />
              </div>
            </div>
            <div className="flex justify-end gap-2 pt-2">
              <Button type="button" variant="secondary" onClick={() => setEditModal(false)}>İptal</Button>
              <Button type="submit" loading={saving}>Kaydet</Button>
            </div>
          </form>
        )}
      </Modal>
    </SellerLayout>
  );
}

function WorkingHourRow({ day, dayOfWeek, existing, saving, onSave }) {
  const [isClosed, setIsClosed] = useState(existing?.isClosed ?? false);
  const [openTime, setOpenTime] = useState(existing?.openTime ?? '09:00:00');
  const [closeTime, setCloseTime] = useState(existing?.closeTime ?? '22:00:00');

  const toTimeString = (t) => t?.substring(0, 5) || '';
  const fromTimeString = (t) => t + ':00';

  return (
    <div className="px-5 py-4 flex items-center gap-4">
      <span className="w-24 text-sm font-medium text-gray-700 shrink-0">{day}</span>
      <label className="flex items-center gap-2 shrink-0">
        <input
          type="checkbox"
          checked={isClosed}
          onChange={e => setIsClosed(e.target.checked)}
          className="w-4 h-4 accent-red-500"
        />
        <span className="text-xs text-gray-500">Kapalı</span>
      </label>
      {!isClosed && (
        <>
          <input
            type="time"
            value={toTimeString(openTime)}
            onChange={e => setOpenTime(fromTimeString(e.target.value))}
            className="border border-gray-200 rounded-lg px-2 py-1.5 text-sm outline-none focus:ring-2 focus:ring-emerald-400"
          />
          <span className="text-gray-400 text-sm">–</span>
          <input
            type="time"
            value={toTimeString(closeTime)}
            onChange={e => setCloseTime(fromTimeString(e.target.value))}
            className="border border-gray-200 rounded-lg px-2 py-1.5 text-sm outline-none focus:ring-2 focus:ring-emerald-400"
          />
        </>
      )}
      <Button
        size="sm"
        variant="outline"
        loading={saving}
        onClick={() => onSave(dayOfWeek, { openTime, closeTime, isClosed })}
        className="ml-auto"
      >
        Kaydet
      </Button>
    </div>
  );
}
