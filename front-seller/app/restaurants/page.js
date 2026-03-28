'use client';

import { useState } from 'react';
import useSWR from 'swr';
import Link from 'next/link';
import SellerLayout from '@/components/layout/SellerLayout';
import Badge from '@/components/ui/Badge';
import Button from '@/components/ui/Button';
import Modal from '@/components/ui/Modal';
import Input from '@/components/ui/Input';
import { useToast } from '@/components/ui/Toast';
import fetcher from '@/lib/fetcher';
import api from '@/lib/api';

const defaultForm = {
  name: '', phone: '', email: '', description: '',
  minimumOrderPrice: 50, minDeliveryTime: 20, maxDeliveryTime: 45,
  sellerId: '00000000-0000-0000-0000-000000000000',
};

export default function RestaurantsPage() {
  const toast = useToast();
  const [addModal, setAddModal] = useState(false);
  const [form, setForm] = useState(defaultForm);
  const [saving, setSaving] = useState(false);
  const [toggling, setToggling] = useState(null);

  const { data, isLoading, mutate } = useSWR('/v1/seller/restaurant/list', fetcher);
  const restaurants = data?.data || [];

  const set = (key) => (e) => setForm(f => ({ ...f, [key]: e.target.value }));

  const handleAdd = async (e) => {
    e.preventDefault();
    setSaving(true);
    try {
      await api.post('/v1/seller/restaurant/add', {
        ...form,
        minimumOrderPrice: parseFloat(form.minimumOrderPrice),
        minDeliveryTime: parseInt(form.minDeliveryTime),
        maxDeliveryTime: parseInt(form.maxDeliveryTime),
      });
      toast('Restoran eklendi', 'success');
      setAddModal(false);
      setForm(defaultForm);
      mutate();
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || 'Hata oluştu', 'error');
    } finally {
      setSaving(false);
    }
  };

  const handleToggleOpen = async (restaurant) => {
    setToggling(restaurant.id);
    try {
      await api.patch(`/v1/seller/restaurant/${restaurant.id}/toggle-open`);
      toast(`Restoran ${restaurant.isOpen ? 'kapatıldı' : 'açıldı'}`, 'success');
      mutate();
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || 'Hata oluştu', 'error');
    } finally {
      setToggling(null);
    }
  };

  return (
    <SellerLayout
      title="Restoranlarım"
      headerActions={
        <Button onClick={() => setAddModal(true)}>
          <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
          </svg>
          Restoran Ekle
        </Button>
      }
    >
      {isLoading ? (
        <div className="flex items-center justify-center py-20">
          <div className="w-8 h-8 border-2 border-emerald-500 border-t-transparent rounded-full animate-spin" />
        </div>
      ) : restaurants.length === 0 ? (
        <div className="text-center py-20">
          <div className="w-16 h-16 bg-gray-100 rounded-full flex items-center justify-center mx-auto mb-4">
            <svg className="w-8 h-8 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16" />
            </svg>
          </div>
          <p className="text-gray-500 mb-4">Henüz restoran eklemediniz</p>
          <Button onClick={() => setAddModal(true)}>İlk Restauranı Ekle</Button>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-4">
          {restaurants.map(r => (
            <div key={r.id} className="bg-white rounded-xl border border-gray-100 shadow-sm overflow-hidden">
              <div className="p-5">
                <div className="flex items-start justify-between mb-3">
                  <div className="flex items-center gap-3">
                    <div className="w-10 h-10 bg-emerald-100 rounded-xl flex items-center justify-center text-emerald-600 font-bold">
                      {r.name?.[0]?.toUpperCase()}
                    </div>
                    <div>
                      <p className="font-semibold text-gray-800">{r.name}</p>
                      <p className="text-xs text-gray-400">{r.phone}</p>
                    </div>
                  </div>
                  <div className="flex flex-col gap-1 items-end">
                    <Badge label={r.isActive ? 'Aktif' : 'Pasif'} color={r.isActive ? 'green' : 'red'} />
                    <Badge label={r.isOpen ? 'Açık' : 'Kapalı'} color={r.isOpen ? 'blue' : 'gray'} />
                  </div>
                </div>

                {!r.isActive && (
                  <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-2 mb-3">
                    <p className="text-xs text-yellow-700">Restoran aktif değil.</p>
                  </div>
                )}

                <div className="flex gap-2 mt-4">
                  <Link href={`/restaurants/${r.id}`} className="flex-1">
                    <Button variant="outline" size="sm" className="w-full justify-center">Yönet</Button>
                  </Link>
                  <Button
                    size="sm"
                    variant={r.isOpen ? 'danger' : 'primary'}
                    loading={toggling === r.id}
                    onClick={() => handleToggleOpen(r)}
                    disabled={!r.isActive}
                  >
                    {r.isOpen ? 'Kapat' : 'Aç'}
                  </Button>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}

      {/* Add Modal */}
      <Modal isOpen={addModal} onClose={() => setAddModal(false)} title="Yeni Restoran Ekle" size="lg">
        <form onSubmit={handleAdd} className="space-y-4">
          <div className="grid grid-cols-2 gap-3">
            <div className="col-span-2">
              <Input label="Restoran Adı" required value={form.name} onChange={set('name')} />
            </div>
            <Input label="Telefon" required value={form.phone} onChange={set('phone')} />
            <Input label="Email" type="email" value={form.email} onChange={set('email')} />
            <Input label="Min. Sipariş (₺)" type="number" value={form.minimumOrderPrice} onChange={set('minimumOrderPrice')} />
            <div className="grid grid-cols-2 gap-2">
              <Input label="Min. Teslimat (dk)" type="number" value={form.minDeliveryTime} onChange={set('minDeliveryTime')} />
              <Input label="Max. Teslimat (dk)" type="number" value={form.maxDeliveryTime} onChange={set('maxDeliveryTime')} />
            </div>
            <div className="col-span-2">
              <label className="block text-sm font-medium text-gray-700 mb-1.5">Açıklama</label>
              <textarea
                value={form.description}
                onChange={(e) => setForm(f => ({ ...f, description: e.target.value }))}
                rows={2}
                className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-emerald-400 resize-none"
              />
            </div>
          </div>
          <div className="flex justify-end gap-2 pt-2">
            <Button type="button" variant="secondary" onClick={() => setAddModal(false)}>İptal</Button>
            <Button type="submit" loading={saving}>Ekle</Button>
          </div>
        </form>
      </Modal>
    </SellerLayout>
  );
}
