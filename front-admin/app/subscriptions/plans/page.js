'use client';

import { useState } from 'react';
import useSWR from 'swr';
import AdminLayout from '@/components/layout/AdminLayout';
import Modal from '@/components/ui/Modal';
import Button from '@/components/ui/Button';
import Input from '@/components/ui/Input';
import Badge from '@/components/ui/Badge';
import { useToast } from '@/components/ui/Toast';
import fetcher from '@/lib/fetcher';
import api from '@/lib/api';

// CreateSubscriptionPlanDto: { name, description, planType (int), monthlyPrice, maxRestaurants }
// PUT /v1/admin/subscription/plans/{planId} — aynı alanlar

const PLAN_TYPE_OPTIONS = [
  { value: 1, label: 'Basic' },
  { value: 2, label: 'Pro' },
  { value: 3, label: 'Premium' },
];

export default function SubscriptionPlansPage() {
  const toast = useToast();
  // GET /v1/admin/subscription/plans
  const { data, isLoading, mutate } = useSWR('/v1/admin/subscription/plans', fetcher);
  const plans = data?.data || [];

  const [modal, setModal] = useState({ open: false, mode: 'create', item: null });
  const [form, setForm] = useState({ name: '', description: '', planType: 1, monthlyPrice: '', maxRestaurants: 1 });
  const [saving, setSaving] = useState(false);

  const openCreate = () => {
    setForm({ name: '', description: '', planType: 1, monthlyPrice: '', maxRestaurants: 1 });
    setModal({ open: true, mode: 'create', item: null });
  };

  const openEdit = (item) => {
    setForm({
      name: item.name || '',
      description: item.description || '',
      planType: item.planType ?? 1,
      monthlyPrice: item.monthlyPrice ?? '',
      maxRestaurants: item.maxRestaurants ?? 1,
    });
    setModal({ open: true, mode: 'edit', item });
  };

  const closeModal = () => setModal({ open: false, mode: 'create', item: null });

  const handleSave = async (e) => {
    e.preventDefault();
    setSaving(true);
    try {
      const payload = {
        name: form.name,
        description: form.description,
        planType: parseInt(form.planType),
        monthlyPrice: parseFloat(form.monthlyPrice),
        maxRestaurants: parseInt(form.maxRestaurants),
      };
      if (modal.mode === 'create') {
        // POST /v1/admin/subscription/plans
        await api.post('/v1/admin/subscription/plans', payload);
        toast('Plan oluşturuldu', 'success');
      } else {
        // PUT /v1/admin/subscription/plans/{planId}
        await api.put(`/v1/admin/subscription/plans/${modal.item.id}`, payload);
        toast('Plan güncellendi', 'success');
      }
      mutate();
      closeModal();
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.message || 'Hata oluştu', 'error');
    } finally {
      setSaving(false);
    }
  };

  const planTypeLabel = (val) => PLAN_TYPE_OPTIONS.find(o => o.value === val)?.label || String(val);

  return (
    <AdminLayout title="Abonelik Planları">
      <div className="space-y-4">
        <div className="flex items-center justify-between">
          <p className="text-sm text-gray-500">{plans.length} plan</p>
          <Button onClick={openCreate}>
            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
            </svg>
            Plan Ekle
          </Button>
        </div>

        {isLoading ? (
          <div className="text-center py-12 text-gray-400">Yükleniyor...</div>
        ) : plans.length === 0 ? (
          <div className="text-center py-12 text-gray-400">Henüz plan yok</div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            {plans.map((plan) => (
              <div key={plan.id} className="bg-white rounded-xl border border-gray-200 p-5">
                <div className="flex items-start justify-between">
                  <div>
                    <h3 className="font-semibold text-gray-800">{plan.name}</h3>
                    {plan.description && <p className="text-xs text-gray-500 mt-0.5">{plan.description}</p>}
                  </div>
                  <Badge label={planTypeLabel(plan.planType)} color="blue" />
                </div>
                <div className="mt-4">
                  <span className="text-3xl font-bold text-gray-900">₺{plan.monthlyPrice}</span>
                  <span className="text-gray-400 text-sm">/ay</span>
                </div>
                <div className="mt-3 flex items-center gap-2 text-xs text-gray-500">
                  <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-2 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
                  </svg>
                  Max {plan.maxRestaurants} restoran
                </div>
                <div className="mt-4 pt-4 border-t border-gray-100">
                  <Button size="sm" variant="outline" className="w-full" onClick={() => openEdit(plan)}>Düzenle</Button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      <Modal isOpen={modal.open} onClose={closeModal} title={modal.mode === 'create' ? 'Yeni Plan' : 'Plan Düzenle'}>
        <form onSubmit={handleSave} className="space-y-4">
          <Input
            label="Plan Adı"
            required
            value={form.name}
            onChange={(e) => setForm((f) => ({ ...f, name: e.target.value }))}
            placeholder="Basic / Pro / Premium"
          />
          <Input
            label="Açıklama"
            value={form.description}
            onChange={(e) => setForm((f) => ({ ...f, description: e.target.value }))}
            placeholder="Plan açıklaması..."
          />
          <div>
            <label className="text-sm font-medium text-gray-700 block mb-1">Plan Tipi</label>
            <select
              value={form.planType}
              onChange={(e) => setForm((f) => ({ ...f, planType: e.target.value }))}
              className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-orange-400"
            >
              {PLAN_TYPE_OPTIONS.map((o) => (
                <option key={o.value} value={o.value}>{o.label}</option>
              ))}
            </select>
          </div>
          <div className="grid grid-cols-2 gap-3">
            <Input
              label="Aylık Fiyat (₺)"
              type="number"
              min="0"
              step="0.01"
              required
              value={form.monthlyPrice}
              onChange={(e) => setForm((f) => ({ ...f, monthlyPrice: e.target.value }))}
              placeholder="299.99"
            />
            <Input
              label="Max Restoran"
              type="number"
              min="1"
              required
              value={form.maxRestaurants}
              onChange={(e) => setForm((f) => ({ ...f, maxRestaurants: e.target.value }))}
            />
          </div>
          <div className="flex justify-end gap-2 pt-2">
            <Button type="button" variant="secondary" onClick={closeModal}>İptal</Button>
            <Button type="submit" loading={saving}>{modal.mode === 'create' ? 'Oluştur' : 'Kaydet'}</Button>
          </div>
        </form>
      </Modal>
    </AdminLayout>
  );
}
