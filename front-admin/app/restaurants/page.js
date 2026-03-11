'use client';

import { useState, lazy, Suspense } from 'react';
import useSWR from 'swr';
import AdminLayout from '@/components/layout/AdminLayout';
import Table from '@/components/ui/Table';
import Badge from '@/components/ui/Badge';
import Button from '@/components/ui/Button';
import Input from '@/components/ui/Input';
import Modal from '@/components/ui/Modal';
import { useToast } from '@/components/ui/Toast';
import fetcher from '@/lib/fetcher';
import api from '@/lib/api';

const PolygonMap = lazy(() => import('@/components/ui/PolygonMap'));

const defaultForm = {
  name: '', phone: '', email: '', description: '',
  minimumOrderPrice: 0, minDeliveryTime: 15, maxDeliveryTime: 45,
  lat: null, lng: null, polygonWkt: '',
};

export default function RestaurantsPage() {
  const toast = useToast();
  const [page, setPage] = useState(1);
  const [toggling, setToggling] = useState(null);
  const [editModal, setEditModal] = useState(false);
  const [editTarget, setEditTarget] = useState(null);
  const [form, setForm] = useState(defaultForm);
  const [saving, setSaving] = useState(false);
  const [activeTab, setActiveTab] = useState('info'); // 'info' | 'location'

  const { data, isLoading, mutate } = useSWR(
    `/v1/admin/restaurant/list?page=${page}&pageSize=20`,
    fetcher
  );

  const restaurants = data?.data || [];
  const total = data?.totalDataCount || 0;
  const totalPages = Math.ceil(total / 20) || 1;

  const openEdit = async (restaurant) => {
    setEditTarget(restaurant);
    setForm({
      name: restaurant.name || '',
      phone: restaurant.phone || '',
      email: restaurant.email || '',
      description: restaurant.description || '',
      minimumOrderPrice: restaurant.minimumOrderPrice || 0,
      minDeliveryTime: restaurant.minDeliveryTime || 15,
      maxDeliveryTime: restaurant.maxDeliveryTime || 45,
      lat: restaurant.latitude || null,
      lng: restaurant.longitude || null,
      polygonWkt: restaurant.serviceAreaPolygonWkt || '',
    });
    setActiveTab('info');
    setEditModal(true);
  };

  const handleToggleActive = async (restaurant) => {
    setToggling(restaurant.id);
    try {
      await api.patch(`/v1/admin/restaurant/${restaurant.id}/toggle-active`);
      toast(`Restoran ${restaurant.isActive ? 'pasife alındı' : 'aktif edildi'}`, 'success');
      mutate();
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || 'Hata oluştu', 'error');
    } finally {
      setToggling(null);
    }
  };

  const handleSave = async (e) => {
    e.preventDefault();
    setSaving(true);
    try {
      await api.put(`/v1/admin/restaurant/${editTarget.id}`, {
        name: form.name,
        phone: form.phone,
        email: form.email || null,
        description: form.description || null,
        minimumOrderPrice: parseFloat(form.minimumOrderPrice),
        minDeliveryTime: parseInt(form.minDeliveryTime),
        maxDeliveryTime: parseInt(form.maxDeliveryTime),
        latitude: form.lat ? parseFloat(form.lat) : null,
        longitude: form.lng ? parseFloat(form.lng) : null,
        serviceAreaPolygonWkt: form.polygonWkt || null,
      });
      toast('Restoran güncellendi', 'success');
      setEditModal(false);
      mutate();
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || 'Hata oluştu', 'error');
    } finally {
      setSaving(false);
    }
  };

  const set = (key) => (e) => setForm((f) => ({ ...f, [key]: e.target.value }));

  const columns = [
    {
      title: 'Restoran',
      key: 'name',
      render: (v, row) => (
        <div className="flex items-center gap-3">
          {row.coverImage ? (
            <img src={row.coverImage} alt={v} className="w-9 h-9 rounded-lg object-cover" />
          ) : (
            <div className="w-9 h-9 bg-orange-100 rounded-lg flex items-center justify-center text-orange-600 font-bold text-sm">
              {v?.[0]?.toUpperCase()}
            </div>
          )}
          <div>
            <p className="font-medium text-gray-800">{v}</p>
            <p className="text-xs text-gray-400">{row.phone}</p>
          </div>
        </div>
      ),
    },
    {
      title: 'Durum',
      key: 'isActive',
      render: (v) => <Badge label={v ? 'Aktif' : 'Pasif'} color={v ? 'green' : 'red'} />,
    },
    {
      title: 'Açık',
      key: 'isOpen',
      render: (v) => <Badge label={v ? 'Açık' : 'Kapalı'} color={v ? 'blue' : 'gray'} />,
    },
    {
      title: 'Konum',
      key: 'latitude',
      render: (v, row) =>
        v && row.longitude ? (
          <span className="text-xs text-green-600 font-mono">✓ {Number(v).toFixed(4)}, {Number(row.longitude).toFixed(4)}</span>
        ) : (
          <span className="text-xs text-red-400">Konum yok</span>
        ),
    },
    {
      title: 'Servis Alanı',
      key: 'serviceAreaPolygonWkt',
      render: (v) =>
        v ? (
          <span className="text-xs text-green-600">✓ Polygon var</span>
        ) : (
          <span className="text-xs text-red-400">Polygon yok</span>
        ),
    },
    {
      title: 'Puan',
      key: 'rating',
      render: (v, row) => (
        <span className="text-sm">
          {Number(v || 0).toFixed(1)} ⭐ <span className="text-gray-400 text-xs">({row.ratingCount})</span>
        </span>
      ),
    },
    {
      title: 'Min. Sipariş',
      key: 'minimumOrderPrice',
      render: (v) => <span className="text-sm font-medium">₺{v}</span>,
    },
    {
      title: 'İşlem',
      key: 'id',
      width: 180,
      render: (_, row) => (
        <div className="flex gap-2">
          <Button size="sm" variant="outline" onClick={() => openEdit(row)}>Düzenle</Button>
          <Button
            size="sm"
            variant={row.isActive ? 'danger' : 'secondary'}
            loading={toggling === row.id}
            onClick={() => handleToggleActive(row)}
          >
            {row.isActive ? 'Pasife Al' : 'Aktif Et'}
          </Button>
        </div>
      ),
    },
  ];

  return (
    <AdminLayout title="Restoranlar">
      <div className="space-y-4">
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-4">
            <p className="text-sm text-gray-500">{total} restoran</p>
            <div className="flex items-center gap-3 text-xs text-gray-500">
              <span className="flex items-center gap-1">
                <span className="w-2 h-2 rounded-full bg-green-400" />
                Aktif: {restaurants.filter(r => r.isActive).length}
              </span>
              <span className="flex items-center gap-1">
                <span className="w-2 h-2 rounded-full bg-red-400" />
                Pasif: {restaurants.filter(r => !r.isActive).length}
              </span>
            </div>
          </div>
          <Button variant="secondary" size="sm" onClick={() => mutate()}>
            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
            </svg>
            Yenile
          </Button>
        </div>

        <Table columns={columns} data={restaurants} loading={isLoading} emptyText="Restoran bulunamadı" />

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

      {/* Edit Modal */}
      <Modal isOpen={editModal} onClose={() => setEditModal(false)} title={`Restoran Düzenle — ${editTarget?.name}`} size="xl">
        {/* Tabs */}
        <div className="flex gap-1 border-b border-gray-200 mb-5">
          {['info', 'location'].map((tab) => (
            <button
              key={tab}
              onClick={() => setActiveTab(tab)}
              className={`px-4 py-2 text-sm font-medium rounded-t-lg transition-colors ${
                activeTab === tab
                  ? 'bg-orange-50 text-orange-600 border-b-2 border-orange-500'
                  : 'text-gray-500 hover:text-gray-700'
              }`}
            >
              {tab === 'info' ? 'Genel Bilgiler' : 'Konum & Servis Alanı'}
            </button>
          ))}
        </div>

        <form onSubmit={handleSave}>
          {activeTab === 'info' && (
            <div className="space-y-4">
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
                    rows={3}
                    className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-orange-400 resize-none"
                  />
                </div>
              </div>
            </div>
          )}

          {activeTab === 'location' && (
            <div className="space-y-3">
              <div className="grid grid-cols-2 gap-3">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1.5">Enlem (Latitude)</label>
                  <input
                    type="number"
                    step="any"
                    value={form.lat ?? ''}
                    onChange={(e) => setForm(f => ({ ...f, lat: e.target.value ? parseFloat(e.target.value) : null }))}
                    className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-orange-400 font-mono"
                    placeholder="41.015137"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1.5">Boylam (Longitude)</label>
                  <input
                    type="number"
                    step="any"
                    value={form.lng ?? ''}
                    onChange={(e) => setForm(f => ({ ...f, lng: e.target.value ? parseFloat(e.target.value) : null }))}
                    className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-orange-400 font-mono"
                    placeholder="28.979530"
                  />
                </div>
              </div>
              <p className="text-xs text-gray-500">
                Haritaya tıklayarak veya marker&apos;ı sürükleyerek konumu ayarlayın. Polygon çizmek için sol araç çubuğundaki polygon aracını kullanın.
              </p>
              <Suspense fallback={<div className="h-96 bg-gray-100 rounded-lg flex items-center justify-center text-gray-400 text-sm">Harita yükleniyor...</div>}>
                {editModal && (
                  <PolygonMap
                    lat={form.lat}
                    lng={form.lng}
                    polygonWkt={form.polygonWkt}
                    onChange={({ lat, lng, polygonWkt }) =>
                      setForm(f => ({ ...f, lat, lng, polygonWkt }))
                    }
                  />
                )}
              </Suspense>
              {form.polygonWkt && (
                <div className="bg-gray-50 rounded-lg p-2">
                  <p className="text-xs font-mono text-gray-500 break-all line-clamp-2">{form.polygonWkt}</p>
                  <button
                    type="button"
                    onClick={() => setForm(f => ({ ...f, polygonWkt: '' }))}
                    className="text-xs text-red-500 mt-1 hover:underline"
                  >
                    Polygon&apos;u sil
                  </button>
                </div>
              )}
            </div>
          )}

          <div className="flex justify-end gap-2 pt-5 mt-2 border-t border-gray-100">
            <Button type="button" variant="secondary" onClick={() => setEditModal(false)}>İptal</Button>
            <Button type="submit" loading={saving}>Kaydet</Button>
          </div>
        </form>
      </Modal>
    </AdminLayout>
  );
}
