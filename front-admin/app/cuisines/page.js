'use client';

import { useState } from 'react';
import useSWR from 'swr';
import AdminLayout from '@/components/layout/AdminLayout';
import Table from '@/components/ui/Table';
import Modal from '@/components/ui/Modal';
import Button from '@/components/ui/Button';
import Input from '@/components/ui/Input';
import { useToast } from '@/components/ui/Toast';
import fetcher from '@/lib/fetcher';
import api from '@/lib/api';

export default function CuisinesPage() {
  const toast = useToast();
  // GET /v1/cuisine → { data: CuisineResponseDto[] }
  const { data, isLoading, mutate } = useSWR('/v1/cuisine', fetcher);
  const cuisines = data?.data || [];

  const [modal, setModal] = useState({ open: false, mode: 'create', item: null });
  const [form, setForm] = useState({ name: '', orderIndex: 0 });
  const [saving, setSaving] = useState(false);
  const [deleteTarget, setDeleteTarget] = useState(null);
  const [deleting, setDeleting] = useState(false);

  const openCreate = () => {
    setForm({ name: '', orderIndex: cuisines.length });
    setModal({ open: true, mode: 'create', item: null });
  };

  const openEdit = (item) => {
    setForm({ name: item.name || '', orderIndex: item.orderIndex ?? 0 });
    setModal({ open: true, mode: 'edit', item });
  };

  const closeModal = () => setModal({ open: false, mode: 'create', item: null });

  const handleSave = async (e) => {
    e.preventDefault();
    setSaving(true);
    try {
      const payload = { name: form.name, orderIndex: parseInt(form.orderIndex) };
      if (modal.mode === 'create') {
        // POST /v1/cuisine  body: AddCuisineDto { name, orderIndex }
        await api.post('/v1/cuisine', payload);
        toast('Mutfak oluşturuldu', 'success');
      } else {
        // PUT /v1/cuisine  body: UpdateCuisineDto { id, name, orderIndex }
        await api.put('/v1/cuisine', { id: modal.item.id, ...payload });
        toast('Mutfak güncellendi', 'success');
      }
      mutate();
      closeModal();
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.message || err.response?.data?.message || 'Hata oluştu', 'error');
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async () => {
    setDeleting(true);
    try {
      // DELETE /v1/cuisine/{id}
      await api.delete(`/v1/cuisine/${deleteTarget.id}`);
      toast('Mutfak silindi', 'success');
      mutate();
      setDeleteTarget(null);
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.message || 'Silinemedi', 'error');
    } finally {
      setDeleting(false);
    }
  };

  const columns = [
    {
      title: 'Mutfak',
      key: 'name',
      render: (val) => (
        <div className="flex items-center gap-3">
          <div className="w-8 h-8 bg-orange-100 rounded-lg flex items-center justify-center text-orange-500 text-xs font-bold">
            {val?.[0]?.toUpperCase()}
          </div>
          <span className="font-medium">{val}</span>
        </div>
      ),
    },
    { title: 'Sıra', key: 'orderIndex', width: 80 },
    {
      title: 'İşlemler',
      key: 'id',
      width: 160,
      render: (_, row) => (
        <div className="flex items-center gap-2">
          <Button size="sm" variant="outline" onClick={() => openEdit(row)}>Düzenle</Button>
          <Button size="sm" variant="danger" onClick={() => setDeleteTarget(row)}>Sil</Button>
        </div>
      ),
    },
  ];

  return (
    <AdminLayout title="Mutfaklar">
      <div className="space-y-4">
        <div className="flex items-center justify-between">
          <p className="text-sm text-gray-500">{cuisines.length} mutfak</p>
          <Button onClick={openCreate}>
            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
            </svg>
            Mutfak Ekle
          </Button>
        </div>

        <Table columns={columns} data={cuisines} loading={isLoading} emptyText="Henüz mutfak yok" />
      </div>

      <Modal isOpen={modal.open} onClose={closeModal} title={modal.mode === 'create' ? 'Yeni Mutfak' : 'Mutfak Düzenle'}>
        <form onSubmit={handleSave} className="space-y-4">
          <Input
            label="Mutfak Adı"
            required
            value={form.name}
            onChange={(e) => setForm((f) => ({ ...f, name: e.target.value }))}
            placeholder="Türk Mutfağı"
          />
          <Input
            label="Sıra (orderIndex)"
            type="number"
            min="0"
            value={form.orderIndex}
            onChange={(e) => setForm((f) => ({ ...f, orderIndex: e.target.value }))}
          />
          <div className="flex justify-end gap-2 pt-2">
            <Button type="button" variant="secondary" onClick={closeModal}>İptal</Button>
            <Button type="submit" loading={saving}>{modal.mode === 'create' ? 'Oluştur' : 'Kaydet'}</Button>
          </div>
        </form>
      </Modal>

      <Modal isOpen={!!deleteTarget} onClose={() => setDeleteTarget(null)} title="Mutfağı Sil" size="sm">
        <p className="text-gray-600 text-sm">
          <strong>{deleteTarget?.name}</strong> mutfağını silmek istediğinize emin misiniz? Bu işlem geri alınamaz.
        </p>
        <div className="flex justify-end gap-2 mt-4">
          <Button variant="secondary" onClick={() => setDeleteTarget(null)}>İptal</Button>
          <Button variant="danger" loading={deleting} onClick={handleDelete}>Sil</Button>
        </div>
      </Modal>
    </AdminLayout>
  );
}
