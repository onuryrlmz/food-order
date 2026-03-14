'use client';

import { useState } from 'react';
import useSWR from 'swr';
import AdminLayout from '@/components/layout/AdminLayout';
import Table from '@/components/ui/Table';
import Badge from '@/components/ui/Badge';
import Button from '@/components/ui/Button';
import Modal from '@/components/ui/Modal';
import Input from '@/components/ui/Input';
import { useToast } from '@/components/ui/Toast';
import fetcher from '@/lib/fetcher';
import api from '@/lib/api';

const TYPE_MAP = {
  1: { label: 'Yüzde', color: 'blue' },
  2: { label: 'Sabit', color: 'green' },
  3: { label: 'X Al Y Öde', color: 'purple' },
};

const APPLICABLE_MAP = {
  1: { label: 'Tümü', color: 'gray' },
  2: { label: 'Menüler', color: 'orange' },
  3: { label: 'Kategoriler', color: 'yellow' },
};

const defaultForm = {
  sellerId: '',
  restaurantId: '',
  code: '',
  name: '',
  description: '',
  type: 1,
  value: '',
  maxDiscountAmount: '',
  buyQuantity: 2,
  getQuantity: 1,
  minOrderAmount: 0,
  applicableType: 1,
  startDate: '',
  endDate: '',
  usageLimit: '',
  usagePerUser: '',
  applicableMenuIds: [],
  applicableCategoryIds: [],
};

export default function CouponsPage() {
  const toast = useToast();
  const [page, setPage] = useState(1);
  const [deleting, setDeleting] = useState(null);
  const [modal, setModal] = useState(false);
  const [editModal, setEditModal] = useState(null);
  const [form, setForm] = useState(defaultForm);
  const [editForm, setEditForm] = useState(defaultForm);
  const [saving, setSaving] = useState(false);
  const [editSaving, setEditSaving] = useState(false);

  // Create modal dynamic data
  const [restaurants, setRestaurants] = useState([]);
  const [menus, setMenus] = useState([]);
  const [categories, setCategories] = useState([]);
  // Edit modal dynamic data
  const [editRestaurants, setEditRestaurants] = useState([]);
  const [editMenus, setEditMenus] = useState([]);
  const [editCategories, setEditCategories] = useState([]);

  const { data, isLoading, mutate } = useSWR(
    `/v1/admin/coupon/list?page=${page}&pageSize=20`,
    fetcher
  );

  const { data: sellersData } = useSWR('/v1/admin/seller/list?page=1&pageSize=100', fetcher);
  const sellers = sellersData?.data || [];

  const fetchRestaurants = async (sellerId, setter) => {
    if (!sellerId) { setter([]); return; }
    try {
      const res = await api.get(`/v1/admin/restaurant/list?page=1&pageSize=100`);
      if (res.data && !res.data.hasFailed) {
        setter((res.data.data || []).filter(r => r.sellerId === sellerId));
      }
    } catch { setter([]); }
  };

  const fetchMenusAndCategories = async (restaurantId, menuSetter, catSetter) => {
    if (!restaurantId) { menuSetter([]); catSetter([]); return; }
    try {
      const [menuRes, catRes] = await Promise.all([
        api.get(`/v1/admin/coupon/menus/${restaurantId}`).catch(() => null),
        api.get(`/v1/admin/coupon/categories/${restaurantId}`).catch(() => null),
      ]);
      menuSetter(menuRes?.data?.data || []);
      catSetter(catRes?.data?.data || []);
    } catch { menuSetter([]); catSetter([]); }
  };

  const coupons = data?.data || [];
  const total = data?.totalDataCount || 0;
  const totalPages = Math.ceil(total / 20) || 1;

  const isExpired = (endDate) => new Date(endDate) < new Date();
  const isActive = (coupon) => !isExpired(coupon.endDate);

  const set = (key) => (e) => {
    const val = e.target.value;
    setForm((f) => ({ ...f, [key]: val }));
    if (key === 'sellerId') {
      setForm((f) => ({ ...f, restaurantId: '', applicableMenuIds: [], applicableCategoryIds: [] }));
      setMenus([]); setCategories([]);
      fetchRestaurants(val, setRestaurants);
    }
    if (key === 'restaurantId') {
      setForm((f) => ({ ...f, applicableMenuIds: [], applicableCategoryIds: [] }));
      fetchMenusAndCategories(val, setMenus, setCategories);
    }
  };
  const setEdit = (key) => (e) => {
    const val = e.target.value;
    setEditForm((f) => ({ ...f, [key]: val }));
    if (key === 'sellerId') {
      setEditForm((f) => ({ ...f, restaurantId: '', applicableMenuIds: [], applicableCategoryIds: [] }));
      setEditMenus([]); setEditCategories([]);
      fetchRestaurants(val, setEditRestaurants);
    }
    if (key === 'restaurantId') {
      setEditForm((f) => ({ ...f, applicableMenuIds: [], applicableCategoryIds: [] }));
      fetchMenusAndCategories(val, setEditMenus, setEditCategories);
    }
  };
  const toggleMulti = (formSetter, key, id) => {
    formSetter((f) => {
      const arr = f[key] || [];
      return { ...f, [key]: arr.includes(id) ? arr.filter(x => x !== id) : [...arr, id] };
    });
  };

  const toLocalDatetime = (isoStr) => {
    if (!isoStr) return '';
    const d = new Date(isoStr);
    const pad = (n) => String(n).padStart(2, '0');
    return `${d.getFullYear()}-${pad(d.getMonth()+1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
  };

  const openEdit = async (coupon) => {
    setEditModal(coupon);
    setEditForm({
      sellerId: coupon.sellerId || '',
      restaurantId: coupon.restaurantId || '',
      code: coupon.code || '',
      name: coupon.name || '',
      description: coupon.description || '',
      type: coupon.type,
      value: coupon.value?.toString() || '',
      maxDiscountAmount: coupon.maxDiscountAmount?.toString() || '',
      buyQuantity: coupon.buyQuantity || 2,
      getQuantity: coupon.getQuantity || 1,
      minOrderAmount: coupon.minOrderAmount || 0,
      applicableType: coupon.applicableType || 1,
      startDate: toLocalDatetime(coupon.startDate),
      endDate: toLocalDatetime(coupon.endDate),
      usageLimit: coupon.usageLimit?.toString() || '',
      usagePerUser: coupon.usagePerUser?.toString() || '',
      applicableMenuIds: coupon.applicableMenus?.map(m => m.id) || [],
      applicableCategoryIds: coupon.applicableCategories?.map(c => c.id) || [],
    });
    if (coupon.sellerId) await fetchRestaurants(coupon.sellerId, setEditRestaurants);
    if (coupon.restaurantId) await fetchMenusAndCategories(coupon.restaurantId, setEditMenus, setEditCategories);
  };

  const handleEdit = async (e) => {
    e.preventDefault();
    setEditSaving(true);
    try {
      const appType = parseInt(editForm.applicableType);
      const payload = {
        id: editModal.id,
        sellerId: editForm.sellerId,
        restaurantId: editForm.restaurantId || null,
        code: editForm.code.toUpperCase(),
        name: editForm.name,
        description: editForm.description || null,
        type: parseInt(editForm.type),
        value: parseFloat(editForm.value),
        maxDiscountAmount: editForm.maxDiscountAmount ? parseFloat(editForm.maxDiscountAmount) : null,
        buyQuantity: parseInt(editForm.buyQuantity),
        getQuantity: parseInt(editForm.getQuantity),
        minOrderAmount: parseFloat(editForm.minOrderAmount),
        applicableType: appType,
        startDate: new Date(editForm.startDate).toISOString(),
        endDate: new Date(editForm.endDate).toISOString(),
        usageLimit: editForm.usageLimit ? parseInt(editForm.usageLimit) : null,
        usagePerUser: editForm.usagePerUser ? parseInt(editForm.usagePerUser) : null,
        applicableMenuIds: appType === 2 ? editForm.applicableMenuIds : null,
        applicableCategoryIds: appType === 3 ? editForm.applicableCategoryIds : null,
      };

      const res = await api.put('/v1/admin/coupon/update', payload);
      if (res.data && !res.data.hasFailed) {
        toast('Kupon güncellendi', 'success');
        setEditModal(null);
        mutate();
      } else {
        toast(res.data?.messages?.[0]?.description || 'Kupon güncellenemedi', 'error');
      }
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || 'Kupon güncellenemedi', 'error');
    } finally {
      setEditSaving(false);
    }
  };

  const handleCreate = async (e) => {
    e.preventDefault();
    setSaving(true);
    try {
      const cAppType = parseInt(form.applicableType);
      const payload = {
        sellerId: form.sellerId,
        restaurantId: form.restaurantId || null,
        code: form.code.toUpperCase(),
        name: form.name,
        description: form.description || null,
        type: parseInt(form.type),
        value: parseFloat(form.value),
        maxDiscountAmount: form.maxDiscountAmount ? parseFloat(form.maxDiscountAmount) : null,
        buyQuantity: parseInt(form.buyQuantity),
        getQuantity: parseInt(form.getQuantity),
        minOrderAmount: parseFloat(form.minOrderAmount),
        applicableType: cAppType,
        startDate: new Date(form.startDate).toISOString(),
        endDate: new Date(form.endDate).toISOString(),
        usageLimit: form.usageLimit ? parseInt(form.usageLimit) : null,
        usagePerUser: form.usagePerUser ? parseInt(form.usagePerUser) : null,
        applicableMenuIds: cAppType === 2 ? form.applicableMenuIds : null,
        applicableCategoryIds: cAppType === 3 ? form.applicableCategoryIds : null,
      };

      const res = await api.post('/v1/admin/coupon/create', payload);
      if (res.data && !res.data.hasFailed) {
        toast('Kupon oluşturuldu', 'success');
        setModal(false);
        setForm(defaultForm);
        mutate();
      } else {
        toast(res.data?.messages?.[0]?.description || 'Kupon oluşturulamadı', 'error');
      }
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || 'Kupon oluşturulamadı', 'error');
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async (id) => {
    setDeleting(id);
    try {
      const res = await api.delete(`/v1/admin/coupon/${id}`);
      if (res.data && !res.data.hasFailed) {
        toast('Kupon silindi', 'success');
        mutate();
      } else {
        toast(res.data?.messages?.[0]?.description || 'Kupon silinemedi', 'error');
      }
    } catch (err) {
      toast('Kupon silinemedi', 'error');
    } finally {
      setDeleting(null);
    }
  };

  const formatDate = (date) => {
    if (!date) return '—';
    return new Date(date).toLocaleDateString('tr-TR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
    });
  };

  const formatValue = (coupon) => {
    if (coupon.type === 1) return `%${coupon.value}`;
    if (coupon.type === 2) return `₺${coupon.value}`;
    return `${coupon.buyQuantity} al ${coupon.getQuantity} öde`;
  };

  const columns = [
    {
      title: 'Kod',
      key: 'code',
      render: (v) => <code className="px-2 py-1 bg-gray-100 rounded text-sm font-mono">{v}</code>,
    },
    {
      title: 'Kupon',
      key: 'name',
      render: (v, row) => (
        <div>
          <p className="font-medium text-gray-800">{v}</p>
          <p className="text-xs text-gray-400">{row.description || '—'}</p>
        </div>
      ),
    },
    {
      title: 'Satıcı',
      key: 'sellerName',
      render: (v) => <span className="text-sm">{v || '—'}</span>,
    },
    {
      title: 'Restoran',
      key: 'restaurantName',
      render: (v) => <span className="text-sm">{v || 'Tüm Restoranlar'}</span>,
    },
    {
      title: 'Tür',
      key: 'type',
      render: (v) => {
        const t = TYPE_MAP[v] || { label: String(v), color: 'gray' };
        return <Badge label={t.label} color={t.color} />;
      },
    },
    {
      title: 'Değer',
      key: 'value',
      render: (_, row) => <span className="font-medium">{formatValue(row)}</span>,
    },
    {
      title: 'Geçerlilik',
      key: 'startDate',
      render: (_, row) => (
        <div className="text-xs">
          <p>{formatDate(row.startDate)}</p>
          <p className="text-gray-400">— {formatDate(row.endDate)}</p>
        </div>
      ),
    },
    {
      title: 'Kullanım',
      key: 'currentUsageCount',
      render: (v, row) => (
        <span className="text-sm">
          {v}{row.usageLimit ? ` / ${row.usageLimit}` : ''}
        </span>
      ),
    },
    {
      title: 'Durum',
      key: 'endDate',
      render: (_, row) => {
        const active = isActive(row);
        return <Badge label={active ? 'Aktif' : 'Süresi Dolmuş'} color={active ? 'green' : 'red'} />;
      },
    },
    {
      title: 'İşlem',
      key: 'id',
      width: 160,
      render: (_, row) => (
        <div className="flex gap-2">
          <Button size="sm" variant="outline" onClick={() => openEdit(row)}>Düzenle</Button>
          <Button
            size="sm"
            variant="outline"
            onClick={() => {
              if (confirm('Bu kuponu silmek istediğinizden emin misiniz?')) {
                handleDelete(row.id);
              }
            }}
            loading={deleting === row.id}
          >
            Sil
          </Button>
        </div>
      ),
    },
  ];

  return (
    <AdminLayout title="Kuponlar">
      <div className="space-y-4">
        <div className="flex items-center justify-between">
          <p className="text-sm text-gray-500">{total} kupon</p>
          <Button onClick={() => setModal(true)}>
            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
            </svg>
            Kupon Ekle
          </Button>
        </div>

        <Table columns={columns} data={coupons} loading={isLoading} emptyText="Kupon bulunamadı" />

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

      {/* Create Coupon Modal */}
      <Modal isOpen={modal} onClose={() => setModal(false)} title="Yeni Kupon Ekle" size="xl">
        <form onSubmit={handleCreate} className="space-y-5">
          <div>
            <p className="text-xs font-semibold text-gray-400 uppercase mb-3">Temel Bilgiler</p>
            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="text-sm font-medium text-gray-700 block mb-1">Satıcı *</label>
                <select
                  value={form.sellerId}
                  onChange={set('sellerId')}
                  required
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-orange-400"
                >
                  <option value="">Seçiniz</option>
                  {sellers.map((s) => (
                    <option key={s.id} value={s.id}>{s.name}</option>
                  ))}
                </select>
              </div>
              {form.sellerId && (
                <div>
                  <label className="text-sm font-medium text-gray-700 block mb-1">Restoran</label>
                  <select
                    value={form.restaurantId}
                    onChange={set('restaurantId')}
                    className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-orange-400"
                  >
                    <option value="">Tüm Restoranlarda Geçerli</option>
                    {restaurants.map((r) => (
                      <option key={r.id} value={r.id}>{r.name}</option>
                    ))}
                  </select>
                </div>
              )}
              <Input label="Kupon Kodu *" required value={form.code} onChange={set('code')} placeholder="Örn: INDIRIM20" />
              <Input label="Kupon Adı *" required value={form.name} onChange={set('name')} />
              <Input label="Açıklama" value={form.description} onChange={set('description')} />
            </div>
          </div>

          <div>
            <p className="text-xs font-semibold text-gray-400 uppercase mb-3">İndirim Bilgileri</p>
            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="text-sm font-medium text-gray-700 block mb-1">İndirim Türü *</label>
                <select
                  value={form.type}
                  onChange={set('type')}
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-orange-400"
                >
                  <option value={1}>Yüzde</option>
                  <option value={2}>Sabit Tutar</option>
                  <option value={3}>X Al Y Öde</option>
                </select>
              </div>
              <Input label="Değer *" required type="number" step="0.01" value={form.value} onChange={set('value')} placeholder={form.type == 1 ? '%' : '₺'} />
              {form.type == 1 && (
                <Input label="Maks. İndirim" type="number" step="0.01" value={form.maxDiscountAmount} onChange={set('maxDiscountAmount')} placeholder="₺" />
              )}
              {form.type == 3 && (
                <>
                  <Input label="Alınacak Adet" type="number" value={form.buyQuantity} onChange={set('buyQuantity')} />
                  <Input label="Ödenecek Adet" type="number" value={form.getQuantity} onChange={set('getQuantity')} />
                </>
              )}
              <Input label="Min. Sipariş Tutarı" type="number" step="0.01" value={form.minOrderAmount} onChange={set('minOrderAmount')} />
            </div>
          </div>

          <div>
            <p className="text-xs font-semibold text-gray-400 uppercase mb-3">Geçerlilik</p>
            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="text-sm font-medium text-gray-700 block mb-1">Geçerli Olduğu Alan *</label>
                <select
                  value={form.applicableType}
                  onChange={set('applicableType')}
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-orange-400"
                >
                  <option value={1}>Tüm Ürünler</option>
                  <option value={2}>Belirli Menüler</option>
                  <option value={3}>Belirli Kategoriler</option>
                </select>
              </div>
              {form.applicableType == 2 && (
                <div className="col-span-2">
                  <label className="text-sm font-medium text-gray-700 block mb-1">Geçerli Menüler</label>
                  {menus.length === 0 ? (
                    <p className="text-xs text-gray-400">{form.restaurantId ? 'Menü bulunamadı' : 'Önce restoran seçiniz'}</p>
                  ) : (
                    <div className="border border-gray-300 rounded-lg p-2 max-h-40 overflow-y-auto space-y-1">
                      {menus.map((m) => (
                        <label key={m.id} className="flex items-center gap-2 text-sm cursor-pointer hover:bg-gray-50 px-2 py-1 rounded">
                          <input type="checkbox" checked={form.applicableMenuIds?.includes(m.id)} onChange={() => toggleMulti(setForm, 'applicableMenuIds', m.id)} className="w-4 h-4 accent-orange-500" />
                          {m.name}
                        </label>
                      ))}
                    </div>
                  )}
                </div>
              )}
              {form.applicableType == 3 && (
                <div className="col-span-2">
                  <label className="text-sm font-medium text-gray-700 block mb-1">Geçerli Kategoriler</label>
                  {categories.length === 0 ? (
                    <p className="text-xs text-gray-400">{form.restaurantId ? 'Kategori bulunamadı' : 'Önce restoran seçiniz'}</p>
                  ) : (
                    <div className="border border-gray-300 rounded-lg p-2 max-h-40 overflow-y-auto space-y-1">
                      {categories.map((c) => (
                        <label key={c.id} className="flex items-center gap-2 text-sm cursor-pointer hover:bg-gray-50 px-2 py-1 rounded">
                          <input type="checkbox" checked={form.applicableCategoryIds?.includes(c.id)} onChange={() => toggleMulti(setForm, 'applicableCategoryIds', c.id)} className="w-4 h-4 accent-orange-500" />
                          {c.name}
                        </label>
                      ))}
                    </div>
                  )}
                </div>
              )}
              <div>
                <label className="text-sm font-medium text-gray-700 block mb-1">Başlangıç *</label>
                <input
                  type="datetime-local"
                  value={form.startDate}
                  onChange={set('startDate')}
                  required
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-orange-400"
                />
              </div>
              <div>
                <label className="text-sm font-medium text-gray-700 block mb-1">Bitiş *</label>
                <input
                  type="datetime-local"
                  value={form.endDate}
                  onChange={set('endDate')}
                  required
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-orange-400"
                />
              </div>
              <Input label="Kullanım Limiti" type="number" value={form.usageLimit} onChange={set('usageLimit')} placeholder="Boş = sınırsız" />
              <Input label="Kişi Başı Limit" type="number" value={form.usagePerUser} onChange={set('usagePerUser')} placeholder="Boş = sınırsız" />
            </div>
          </div>

          <div className="flex justify-end gap-2 pt-2">
            <Button type="button" variant="secondary" onClick={() => setModal(false)}>İptal</Button>
            <Button type="submit" loading={saving}>Kupon Ekle</Button>
          </div>
        </form>
      </Modal>

      {/* Edit Coupon Modal */}
      <Modal isOpen={!!editModal} onClose={() => setEditModal(null)} title={`Kupon Düzenle — ${editModal?.code}`} size="xl">
        <form onSubmit={handleEdit} className="space-y-5">
          <div>
            <p className="text-xs font-semibold text-gray-400 uppercase mb-3">Temel Bilgiler</p>
            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="text-sm font-medium text-gray-700 block mb-1">Satıcı *</label>
                <select
                  value={editForm.sellerId}
                  onChange={setEdit('sellerId')}
                  required
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-orange-400"
                >
                  <option value="">Seçiniz</option>
                  {sellers.map((s) => (
                    <option key={s.id} value={s.id}>{s.name}</option>
                  ))}
                </select>
              </div>
              {editForm.sellerId && (
                <div>
                  <label className="text-sm font-medium text-gray-700 block mb-1">Restoran</label>
                  <select
                    value={editForm.restaurantId}
                    onChange={setEdit('restaurantId')}
                    className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-orange-400"
                  >
                    <option value="">Tüm Restoranlarda Geçerli</option>
                    {editRestaurants.map((r) => (
                      <option key={r.id} value={r.id}>{r.name}</option>
                    ))}
                  </select>
                </div>
              )}
              <Input label="Kupon Kodu *" required value={editForm.code} onChange={setEdit('code')} />
              <Input label="Kupon Adı *" required value={editForm.name} onChange={setEdit('name')} />
              <Input label="Açıklama" value={editForm.description} onChange={setEdit('description')} />
            </div>
          </div>

          <div>
            <p className="text-xs font-semibold text-gray-400 uppercase mb-3">İndirim Bilgileri</p>
            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="text-sm font-medium text-gray-700 block mb-1">İndirim Türü *</label>
                <select
                  value={editForm.type}
                  onChange={setEdit('type')}
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-orange-400"
                >
                  <option value={1}>Yüzde</option>
                  <option value={2}>Sabit Tutar</option>
                  <option value={3}>X Al Y Öde</option>
                </select>
              </div>
              <Input label="Değer *" required type="number" step="0.01" value={editForm.value} onChange={setEdit('value')} />
              {editForm.type == 1 && (
                <Input label="Maks. İndirim" type="number" step="0.01" value={editForm.maxDiscountAmount} onChange={setEdit('maxDiscountAmount')} placeholder="₺" />
              )}
              {editForm.type == 3 && (
                <>
                  <Input label="Alınacak Adet" type="number" value={editForm.buyQuantity} onChange={setEdit('buyQuantity')} />
                  <Input label="Ödenecek Adet" type="number" value={editForm.getQuantity} onChange={setEdit('getQuantity')} />
                </>
              )}
              <Input label="Min. Sipariş Tutarı" type="number" step="0.01" value={editForm.minOrderAmount} onChange={setEdit('minOrderAmount')} />
            </div>
          </div>

          <div>
            <p className="text-xs font-semibold text-gray-400 uppercase mb-3">Geçerlilik</p>
            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="text-sm font-medium text-gray-700 block mb-1">Geçerli Olduğu Alan *</label>
                <select
                  value={editForm.applicableType}
                  onChange={setEdit('applicableType')}
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-orange-400"
                >
                  <option value={1}>Tüm Ürünler</option>
                  <option value={2}>Belirli Menüler</option>
                  <option value={3}>Belirli Kategoriler</option>
                </select>
              </div>
              {editForm.applicableType == 2 && (
                <div className="col-span-2">
                  <label className="text-sm font-medium text-gray-700 block mb-1">Geçerli Menüler</label>
                  {editMenus.length === 0 ? (
                    <p className="text-xs text-gray-400">{editForm.restaurantId ? 'Menü bulunamadı' : 'Önce restoran seçiniz'}</p>
                  ) : (
                    <div className="border border-gray-300 rounded-lg p-2 max-h-40 overflow-y-auto space-y-1">
                      {editMenus.map((m) => (
                        <label key={m.id} className="flex items-center gap-2 text-sm cursor-pointer hover:bg-gray-50 px-2 py-1 rounded">
                          <input type="checkbox" checked={editForm.applicableMenuIds?.includes(m.id)} onChange={() => toggleMulti(setEditForm, 'applicableMenuIds', m.id)} className="w-4 h-4 accent-orange-500" />
                          {m.name}
                        </label>
                      ))}
                    </div>
                  )}
                </div>
              )}
              {editForm.applicableType == 3 && (
                <div className="col-span-2">
                  <label className="text-sm font-medium text-gray-700 block mb-1">Geçerli Kategoriler</label>
                  {editCategories.length === 0 ? (
                    <p className="text-xs text-gray-400">{editForm.restaurantId ? 'Kategori bulunamadı' : 'Önce restoran seçiniz'}</p>
                  ) : (
                    <div className="border border-gray-300 rounded-lg p-2 max-h-40 overflow-y-auto space-y-1">
                      {editCategories.map((c) => (
                        <label key={c.id} className="flex items-center gap-2 text-sm cursor-pointer hover:bg-gray-50 px-2 py-1 rounded">
                          <input type="checkbox" checked={editForm.applicableCategoryIds?.includes(c.id)} onChange={() => toggleMulti(setEditForm, 'applicableCategoryIds', c.id)} className="w-4 h-4 accent-orange-500" />
                          {c.name}
                        </label>
                      ))}
                    </div>
                  )}
                </div>
              )}
              <div>
                <label className="text-sm font-medium text-gray-700 block mb-1">Başlangıç *</label>
                <input
                  type="datetime-local"
                  value={editForm.startDate}
                  onChange={setEdit('startDate')}
                  required
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-orange-400"
                />
              </div>
              <div>
                <label className="text-sm font-medium text-gray-700 block mb-1">Bitiş *</label>
                <input
                  type="datetime-local"
                  value={editForm.endDate}
                  onChange={setEdit('endDate')}
                  required
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-orange-400"
                />
              </div>
              <Input label="Kullanım Limiti" type="number" value={editForm.usageLimit} onChange={setEdit('usageLimit')} placeholder="Boş = sınırsız" />
              <Input label="Kişi Başı Limit" type="number" value={editForm.usagePerUser} onChange={setEdit('usagePerUser')} placeholder="Boş = sınırsız" />
            </div>
          </div>

          <div className="flex justify-end gap-2 pt-2">
            <Button type="button" variant="secondary" onClick={() => setEditModal(null)}>İptal</Button>
            <Button type="submit" loading={editSaving}>Kaydet</Button>
          </div>
        </form>
      </Modal>
    </AdminLayout>
  );
}
