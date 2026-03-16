'use client';

import { useState } from 'react';
import useSWR from 'swr';
import AdminLayout from '@/components/layout/AdminLayout';
import Table from '@/components/ui/Table';
import Modal from '@/components/ui/Modal';
import Badge from '@/components/ui/Badge';
import Button from '@/components/ui/Button';
import Input from '@/components/ui/Input';
import { useToast } from '@/components/ui/Toast';
import fetcher from '@/lib/fetcher';
import api from '@/lib/api';
import { retryIyzicoRegistration } from '@/lib/api';

const COMPANY_TYPES = [
  { value: 1, label: 'Şahıs Şirketi' },
  { value: 2, label: 'Limited Şirket' },
];

const STATUS_MAP = {
  1: { label: 'Beklemede', color: 'yellow' },
  2: { label: 'Onaylı', color: 'green' },
  3: { label: 'Reddedildi', color: 'red' },
  4: { label: 'Bloke', color: 'gray' },
};

const defaultForm = {
  companyType: 1,
  name: '',
  legalName: '',
  taxCode: '',
  taxArea: '',
  iban: '',
  isEInvoiceAvaible: false,
  ownerFirstName: '',
  ownerLastName: '',
  ownerEmail: '',
  ownerPhone: '',
  password: '',
  cityId: '00000000-0000-0000-0000-000000000000',
  townId: '00000000-0000-0000-0000-000000000000',
  neighbourhoodId: '00000000-0000-0000-0000-000000000000',
  addressLine1: '',
  addressLine2: '',
};

const defaultEditForm = {
  name: '', legalName: '', taxCode: '', taxArea: '', iban: '', isEInvoiceAvaible: false,
};

export default function SellersPage() {
  const toast = useToast();
  const [page, setPage] = useState(1);
  const [modal, setModal] = useState(false);
  const [confirmModal, setConfirmModal] = useState(null);
  const [editModal, setEditModal] = useState(null);
  const [editForm, setEditForm] = useState(defaultEditForm);
  const [form, setForm] = useState(defaultForm);
  const [saving, setSaving] = useState(false);
  const [editSaving, setEditSaving] = useState(false);
  const [confirming, setConfirming] = useState(false);
  const [retrying, setRetrying] = useState(null);

  const { data, isLoading, mutate } = useSWR(
    `/v1/admin/seller/list?page=${page}&pageSize=20`,
    fetcher
  );

  const sellers = data?.data || [];
  const total = data?.totalDataCount || 0;
  const totalPages = Math.ceil(total / 20) || 1;

  const set = (key) => (e) => setForm((f) => ({ ...f, [key]: e.target.value }));
  const setEdit = (key) => (e) => setEditForm((f) => ({ ...f, [key]: e.target.value }));

  const openEdit = (seller) => {
    setEditModal(seller);
    setEditForm({
      name: seller.name || '',
      legalName: seller.legalName || '',
      taxCode: seller.taxCode || '',
      taxArea: '',
      iban: '',
      isEInvoiceAvaible: false,
    });
  };

  const handleEdit = async (e) => {
    e.preventDefault();
    setEditSaving(true);
    try {
      await api.put(`/v1/admin/seller/${editModal.id}`, {
        ...editForm,
        isEInvoiceAvaible: editForm.isEInvoiceAvaible,
      });
      toast('Satıcı güncellendi', 'success');
      setEditModal(null);
      mutate();
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || 'Hata oluştu', 'error');
    } finally {
      setEditSaving(false);
    }
  };

  const handleAdd = async (e) => {
    e.preventDefault();
    setSaving(true);
    try {
      await api.post('/v1/admin/seller/add', {
        ...form,
        companyType: parseInt(form.companyType),
      });
      toast('Satıcı eklendi', 'success');
      setModal(false);
      setForm(defaultForm);
      mutate();
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || 'Hata oluştu', 'error');
    } finally {
      setSaving(false);
    }
  };

  const handleConfirm = async () => {
    if (!confirmModal) return;
    setConfirming(true);
    try {
      await api.post('/v1/admin/seller/confirm', { id: confirmModal.id });
      toast('Satıcı onaylandı', 'success');
      setConfirmModal(null);
      mutate();
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || 'Hata oluştu', 'error');
    } finally {
      setConfirming(false);
    }
  };

  const handleRetryIyzico = async (sellerId) => {
    setRetrying(sellerId);
    try {
      const res = await retryIyzicoRegistration(sellerId);
      if (res.hasFailed) {
        toast(res.messages?.[0]?.description || 'iyzico kaydı başarısız', 'error');
      } else {
        toast('iyzico kaydı başarılı', 'success');
        mutate();
      }
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || 'iyzico kaydı başarısız', 'error');
    } finally {
      setRetrying(null);
    }
  };

  const columns = [
    {
      title: 'Firma',
      key: 'name',
      render: (v, row) => (
        <div>
          <p className="font-medium text-gray-800">{v}</p>
          <p className="text-xs text-gray-400">{row.legalName}</p>
        </div>
      ),
    },
    {
      title: 'Yetkili',
      key: 'ownerFirstName',
      render: (v, row) => (
        <div>
          <p className="text-sm">{v} {row.ownerLastName}</p>
          <p className="text-xs text-gray-400">{row.ownerEmail}</p>
        </div>
      ),
    },
    { title: 'Vergi No', key: 'taxCode', render: (v) => <span className="font-mono text-xs">{v || '—'}</span> },
    {
      title: 'Tür',
      key: 'companyTypeName',
      render: (v) => <Badge label={v || '—'} color="blue" />,
    },
    {
      title: 'Durum',
      key: 'companyStatus',
      render: (v) => {
        const s = STATUS_MAP[v] || { label: String(v), color: 'gray' };
        return <Badge label={s.label} color={s.color} />;
      },
    },
    {
      title: 'Kayıt',
      key: 'createdDate',
      render: (v) => v ? new Date(v).toLocaleDateString('tr-TR') : '—',
    },
    {
      title: 'iyzico',
      key: 'subMerchantKey',
      width: 140,
      render: (v, row) => {
        if (v) return <Badge label="Kayıtlı" color="green" />;
        if (row.companyStatus === 2) {
          return (
            <div className="flex items-center gap-1.5">
              <Badge label="Kayıtsız" color="gray" />
              <button
                onClick={() => handleRetryIyzico(row.id)}
                disabled={retrying === row.id}
                className="text-xs text-orange-600 hover:text-orange-800 font-medium disabled:opacity-50"
              >
                {retrying === row.id ? '...' : 'Tekrar Dene'}
              </button>
            </div>
          );
        }
        return <Badge label="Kayıtsız" color="gray" />;
      },
    },
    {
      title: 'İşlem',
      key: 'id',
      width: 160,
      render: (_, row) => (
        <div className="flex gap-2">
          <Button size="sm" variant="outline" onClick={() => openEdit(row)}>Düzenle</Button>
          {row.companyStatus !== 2 ? (
            <Button size="sm" onClick={() => setConfirmModal(row)}>Onayla</Button>
          ) : (
            <span className="text-xs text-green-600 font-medium self-center">✓ Onaylı</span>
          )}
        </div>
      ),
    },
  ];

  return (
    <AdminLayout title="Satıcılar">
      <div className="space-y-4">
        <div className="flex items-center justify-between">
          <p className="text-sm text-gray-500">{total} satıcı</p>
          <Button onClick={() => setModal(true)}>
            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
            </svg>
            Satıcı Ekle
          </Button>
        </div>

        <Table columns={columns} data={sellers} loading={isLoading} emptyText="Satıcı bulunamadı" />

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

      {/* Add Seller Modal */}
      <Modal isOpen={modal} onClose={() => setModal(false)} title="Yeni Satıcı Ekle" size="xl">
        <form onSubmit={handleAdd} className="space-y-5">
          <div>
            <p className="text-xs font-semibold text-gray-400 uppercase mb-3">Firma Bilgileri</p>
            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="text-sm font-medium text-gray-700 block mb-1">Firma Tipi</label>
                <select
                  value={form.companyType}
                  onChange={set('companyType')}
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-orange-400"
                >
                  {COMPANY_TYPES.map((o) => (
                    <option key={o.value} value={o.value}>{o.label}</option>
                  ))}
                </select>
              </div>
              <Input label="Firma Adı" required value={form.name} onChange={set('name')} />
              <Input label="Yasal Unvan" required value={form.legalName} onChange={set('legalName')} />
              <Input label="Vergi No" value={form.taxCode} onChange={set('taxCode')} />
              <Input label="Vergi Dairesi" value={form.taxArea} onChange={set('taxArea')} />
              <Input label="IBAN" value={form.iban} onChange={set('iban')} placeholder="TR..." />
            </div>
            <div className="flex items-center gap-2 mt-3">
              <input
                type="checkbox"
                id="eInvoice"
                checked={form.isEInvoiceAvaible}
                onChange={(e) => setForm((f) => ({ ...f, isEInvoiceAvaible: e.target.checked }))}
                className="w-4 h-4 accent-orange-500"
              />
              <label htmlFor="eInvoice" className="text-sm text-gray-700">E-Fatura mükellefi</label>
            </div>
          </div>
          <div>
            <p className="text-xs font-semibold text-gray-400 uppercase mb-3">Yetkili Bilgileri</p>
            <div className="grid grid-cols-2 gap-3">
              <Input label="Ad" required value={form.ownerFirstName} onChange={set('ownerFirstName')} />
              <Input label="Soyad" required value={form.ownerLastName} onChange={set('ownerLastName')} />
              <Input label="Email" type="email" required value={form.ownerEmail} onChange={set('ownerEmail')} />
              <Input label="Telefon" value={form.ownerPhone} onChange={set('ownerPhone')} />
              <div className="col-span-2">
                <Input label="Şifre" type="password" required value={form.password} onChange={set('password')} />
              </div>
            </div>
          </div>
          <div>
            <p className="text-xs font-semibold text-gray-400 uppercase mb-3">Adres</p>
            <div className="grid grid-cols-2 gap-3">
              <Input label="Adres Satır 1" value={form.addressLine1} onChange={set('addressLine1')} />
              <Input label="Adres Satır 2" value={form.addressLine2} onChange={set('addressLine2')} />
            </div>
          </div>
          <div className="flex justify-end gap-2 pt-2">
            <Button type="button" variant="secondary" onClick={() => setModal(false)}>İptal</Button>
            <Button type="submit" loading={saving}>Satıcı Ekle</Button>
          </div>
        </form>
      </Modal>

      {/* Edit Seller Modal */}
      <Modal isOpen={!!editModal} onClose={() => setEditModal(null)} title={`Satıcı Düzenle — ${editModal?.name}`} size="lg">
        <form onSubmit={handleEdit} className="space-y-4">
          <div className="grid grid-cols-2 gap-3">
            <Input label="Firma Adı" required value={editForm.name} onChange={setEdit('name')} />
            <Input label="Yasal Unvan" required value={editForm.legalName} onChange={setEdit('legalName')} />
            <Input label="Vergi No" value={editForm.taxCode} onChange={setEdit('taxCode')} />
            <Input label="Vergi Dairesi" value={editForm.taxArea} onChange={setEdit('taxArea')} />
            <div className="col-span-2">
              <Input label="IBAN" value={editForm.iban} onChange={setEdit('iban')} placeholder="TR..." />
            </div>
          </div>
          <div className="flex items-center gap-2">
            <input
              type="checkbox"
              id="editEInvoice"
              checked={editForm.isEInvoiceAvaible}
              onChange={(e) => setEditForm(f => ({ ...f, isEInvoiceAvaible: e.target.checked }))}
              className="w-4 h-4 accent-orange-500"
            />
            <label htmlFor="editEInvoice" className="text-sm text-gray-700">E-Fatura mükellefi</label>
          </div>
          <div className="flex justify-end gap-2 pt-2">
            <Button type="button" variant="secondary" onClick={() => setEditModal(null)}>İptal</Button>
            <Button type="submit" loading={editSaving}>Kaydet</Button>
          </div>
        </form>
      </Modal>

      {/* Confirm Modal */}
      <Modal isOpen={!!confirmModal} onClose={() => setConfirmModal(null)} title="Satıcı Onayla" size="sm">
        <div className="space-y-4">
          <p className="text-sm text-gray-600">
            <strong>{confirmModal?.name}</strong> adlı satıcıyı onaylamak istiyor musunuz?
          </p>
          <p className="text-xs text-gray-400 font-mono">{confirmModal?.id}</p>
          <div className="flex justify-end gap-2">
            <Button variant="secondary" onClick={() => setConfirmModal(null)}>İptal</Button>
            <Button loading={confirming} onClick={handleConfirm}>Onayla</Button>
          </div>
        </div>
      </Modal>
    </AdminLayout>
  );
}
