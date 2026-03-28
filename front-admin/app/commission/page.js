'use client';

import { useState } from 'react';
import useSWR from 'swr';
import AdminLayout from '@/components/layout/AdminLayout';
import Badge from '@/components/ui/Badge';
import Button from '@/components/ui/Button';
import StatCard from '@/components/ui/StatCard';
import { useToast } from '@/components/ui/Toast';
import fetcher from '@/lib/fetcher';
import { createPlatformSchedule, getRestaurantCommission, getRestaurantCommissionHistory, setRestaurantCommission } from '@/lib/api';

export default function CommissionPage() {
  const toast = useToast();
  const [showForm, setShowForm] = useState(false);
  const [saving, setSaving] = useState(false);
  const [form, setForm] = useState({ commissionRate: '', fixedFee: '', effectiveFrom: '', notes: '' });

  // Restaurant custom commission
  const [restaurantId, setRestaurantId] = useState('');
  const [searchedId, setSearchedId] = useState('');
  const [restaurantForm, setRestaurantForm] = useState({ commissionRate: '', fixedFee: '', effectiveFrom: '', notes: '' });
  const [savingRestaurant, setSavingRestaurant] = useState(false);

  const { data: activeData } = useSWR('/v1/admin/commission/settings/active', fetcher);
  const { data: schedulesData, isLoading, mutate } = useSWR('/v1/admin/commission/settings', fetcher);
  const { data: restaurantCommData, mutate: mutateRestComm } = useSWR(
    searchedId ? `/v1/admin/commission/restaurant/${searchedId}` : null,
    fetcher
  );
  const { data: restaurantHistoryData, mutate: mutateRestHistory } = useSWR(
    searchedId ? `/v1/admin/commission/restaurant/${searchedId}/history` : null,
    fetcher
  );

  const active = activeData?.data;
  const schedules = schedulesData?.data || [];
  const restaurantComm = restaurantCommData?.data;
  const restaurantHistory = restaurantHistoryData?.data || [];

  const fmt = (val) => {
    if (val == null) return '-';
    return new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(val);
  };

  const pct = (val) => {
    if (val == null) return '-';
    return `%${(val * 100).toFixed(1)}`;
  };

  const handleCreateSchedule = async (e) => {
    e.preventDefault();
    setSaving(true);
    try {
      await createPlatformSchedule({
        commissionRate: parseFloat(form.commissionRate) / 100,
        fixedFee: parseFloat(form.fixedFee) || 0,
        effectiveFrom: form.effectiveFrom,
        notes: form.notes,
      });
      toast('Yeni tarife eklendi', 'success');
      setShowForm(false);
      setForm({ commissionRate: '', fixedFee: '', effectiveFrom: '', notes: '' });
      mutate();
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || err.message || 'Hata olustu', 'error');
    } finally {
      setSaving(false);
    }
  };

  const handleSearchRestaurant = () => {
    if (restaurantId.trim()) {
      setSearchedId(restaurantId.trim());
    }
  };

  const handleSetRestaurantCommission = async (e) => {
    e.preventDefault();
    setSavingRestaurant(true);
    try {
      await setRestaurantCommission(searchedId, {
        commissionRate: parseFloat(restaurantForm.commissionRate) / 100,
        fixedFee: parseFloat(restaurantForm.fixedFee) || 0,
        effectiveFrom: restaurantForm.effectiveFrom,
        notes: restaurantForm.notes,
      });
      toast('Restoran komisyonu guncellendi', 'success');
      setRestaurantForm({ commissionRate: '', fixedFee: '', effectiveFrom: '', notes: '' });
      mutateRestComm();
      mutateRestHistory();
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || err.message || 'Hata olustu', 'error');
    } finally {
      setSavingRestaurant(false);
    }
  };

  return (
    <AdminLayout title="Komisyon Yonetimi">
      <div className="space-y-6">
        {/* Active Schedule Stats */}
        {active && (
          <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
            <StatCard title="Aktif Komisyon Orani" value={pct(active.commissionRate)} color="orange" />
            <StatCard title="Sabit Ucret" value={fmt(active.fixedFee)} color="blue" />
            <StatCard title="Gecerlilik Baslangici" value={active.effectiveFrom ? new Date(active.effectiveFrom).toLocaleDateString('tr-TR') : '-'} color="green" />
          </div>
        )}

        {/* New Schedule Form */}
        <div className="flex items-center justify-between">
          <h2 className="text-lg font-semibold text-gray-800">Platform Tarifeleri</h2>
          <Button onClick={() => setShowForm(!showForm)}>
            {showForm ? 'Iptal' : 'Yeni Tarife Ekle'}
          </Button>
        </div>

        {showForm && (
          <form onSubmit={handleCreateSchedule} className="bg-white rounded-xl border border-gray-200 p-5 space-y-4">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Komisyon Orani (%)</label>
                <input
                  type="number"
                  step="0.1"
                  required
                  value={form.commissionRate}
                  onChange={(e) => setForm({ ...form, commissionRate: e.target.value })}
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm"
                  placeholder="orn: 10"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Sabit Ucret (TL)</label>
                <input
                  type="number"
                  step="0.01"
                  value={form.fixedFee}
                  onChange={(e) => setForm({ ...form, fixedFee: e.target.value })}
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm"
                  placeholder="orn: 2.50"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Gecerlilik Tarihi</label>
                <input
                  type="date"
                  required
                  value={form.effectiveFrom}
                  onChange={(e) => setForm({ ...form, effectiveFrom: e.target.value })}
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Notlar</label>
                <input
                  type="text"
                  value={form.notes}
                  onChange={(e) => setForm({ ...form, notes: e.target.value })}
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm"
                  placeholder="Aciklama"
                />
              </div>
            </div>
            <Button type="submit" loading={saving}>Kaydet</Button>
          </form>
        )}

        {/* Schedule History Table */}
        {isLoading ? (
          <div className="flex justify-center py-12">
            <div className="w-8 h-8 border-2 border-orange-400 border-t-transparent rounded-full animate-spin" />
          </div>
        ) : schedules.length === 0 ? (
          <div className="bg-white rounded-xl border border-gray-200 p-10 text-center text-gray-400 text-sm">
            Henuz tarife bulunmuyor
          </div>
        ) : (
          <div className="bg-white rounded-xl border border-gray-200 overflow-hidden">
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead>
                  <tr className="text-left text-xs font-semibold text-gray-400 uppercase tracking-wider">
                    <th className="px-5 py-3">Oran</th>
                    <th className="px-5 py-3">Sabit Ucret</th>
                    <th className="px-5 py-3">Baslangic</th>
                    <th className="px-5 py-3">Bitis</th>
                    <th className="px-5 py-3">Notlar</th>
                    <th className="px-5 py-3">Durum</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-50">
                  {schedules.map((s, i) => (
                    <tr key={s.id || i} className="hover:bg-gray-50">
                      <td className="px-5 py-3 text-sm text-gray-700">{pct(s.commissionRate)}</td>
                      <td className="px-5 py-3 text-sm text-gray-700">{fmt(s.fixedFee)}</td>
                      <td className="px-5 py-3 text-sm text-gray-600">
                        {s.effectiveFrom ? new Date(s.effectiveFrom).toLocaleDateString('tr-TR') : '-'}
                      </td>
                      <td className="px-5 py-3 text-sm text-gray-600">
                        {s.effectiveTo ? new Date(s.effectiveTo).toLocaleDateString('tr-TR') : '-'}
                      </td>
                      <td className="px-5 py-3 text-sm text-gray-500">{s.notes || '-'}</td>
                      <td className="px-5 py-3">
                        <Badge label={s.isActive ? 'Aktif' : 'Pasif'} color={s.isActive ? 'green' : 'gray'} />
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        )}

        {/* Restaurant Custom Commission */}
        <div className="border-t border-gray-200 pt-6">
          <h2 className="text-lg font-semibold text-gray-800 mb-4">Restoran Ozel Komisyonu</h2>
          <div className="flex gap-3 mb-4">
            <input
              type="text"
              value={restaurantId}
              onChange={(e) => setRestaurantId(e.target.value)}
              placeholder="Restoran ID girin"
              className="border border-gray-300 rounded-lg px-3 py-2 text-sm flex-1 max-w-md"
            />
            <Button onClick={handleSearchRestaurant} variant="outline">Ara</Button>
          </div>

          {searchedId && restaurantComm && (
            <div className="bg-white rounded-xl border border-gray-200 p-5 space-y-4">
              <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
                <div>
                  <p className="text-xs text-gray-500">Mevcut Oran</p>
                  <p className="text-lg font-bold text-gray-800">{pct(restaurantComm.commissionRate)}</p>
                </div>
                <div>
                  <p className="text-xs text-gray-500">Sabit Ucret</p>
                  <p className="text-lg font-bold text-gray-800">{fmt(restaurantComm.fixedFee)}</p>
                </div>
                <div>
                  <p className="text-xs text-gray-500">Gecerlilik</p>
                  <p className="text-lg font-bold text-gray-800">
                    {restaurantComm.effectiveFrom ? new Date(restaurantComm.effectiveFrom).toLocaleDateString('tr-TR') : '-'}
                  </p>
                </div>
              </div>

              <form onSubmit={handleSetRestaurantCommission} className="border-t border-gray-100 pt-4 space-y-4">
                <h3 className="text-sm font-semibold text-gray-700">Yeni Komisyon Belirle</h3>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Komisyon Orani (%)</label>
                    <input
                      type="number"
                      step="0.1"
                      required
                      value={restaurantForm.commissionRate}
                      onChange={(e) => setRestaurantForm({ ...restaurantForm, commissionRate: e.target.value })}
                      className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Sabit Ucret (TL)</label>
                    <input
                      type="number"
                      step="0.01"
                      value={restaurantForm.fixedFee}
                      onChange={(e) => setRestaurantForm({ ...restaurantForm, fixedFee: e.target.value })}
                      className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Gecerlilik Tarihi</label>
                    <input
                      type="date"
                      required
                      value={restaurantForm.effectiveFrom}
                      onChange={(e) => setRestaurantForm({ ...restaurantForm, effectiveFrom: e.target.value })}
                      className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Notlar</label>
                    <input
                      type="text"
                      value={restaurantForm.notes}
                      onChange={(e) => setRestaurantForm({ ...restaurantForm, notes: e.target.value })}
                      className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm"
                    />
                  </div>
                </div>
                <Button type="submit" loading={savingRestaurant}>Kaydet</Button>
              </form>

              {/* Restaurant Commission History */}
              {restaurantHistory.length > 0 && (
                <div className="border-t border-gray-100 pt-4">
                  <h3 className="text-sm font-semibold text-gray-700 mb-3">Komisyon Gecmisi</h3>
                  <div className="overflow-x-auto">
                    <table className="w-full">
                      <thead>
                        <tr className="text-left text-xs font-semibold text-gray-400 uppercase tracking-wider">
                          <th className="px-4 py-2">Oran</th>
                          <th className="px-4 py-2">Sabit Ucret</th>
                          <th className="px-4 py-2">Baslangic</th>
                          <th className="px-4 py-2">Bitis</th>
                          <th className="px-4 py-2">Durum</th>
                        </tr>
                      </thead>
                      <tbody className="divide-y divide-gray-50">
                        {restaurantHistory.map((h, i) => (
                          <tr key={h.id || i}>
                            <td className="px-4 py-2 text-sm text-gray-700">{pct(h.commissionRate)}</td>
                            <td className="px-4 py-2 text-sm text-gray-700">{fmt(h.fixedFee)}</td>
                            <td className="px-4 py-2 text-sm text-gray-600">
                              {h.effectiveFrom ? new Date(h.effectiveFrom).toLocaleDateString('tr-TR') : '-'}
                            </td>
                            <td className="px-4 py-2 text-sm text-gray-600">
                              {h.effectiveTo ? new Date(h.effectiveTo).toLocaleDateString('tr-TR') : '-'}
                            </td>
                            <td className="px-4 py-2">
                              <Badge label={h.isActive ? 'Aktif' : 'Pasif'} color={h.isActive ? 'green' : 'gray'} />
                            </td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                </div>
              )}
            </div>
          )}
        </div>
      </div>
    </AdminLayout>
  );
}
