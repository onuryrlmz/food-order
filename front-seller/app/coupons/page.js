'use client';

import { useState } from 'react';
import useSWR from 'swr';
import SellerLayout from '@/components/layout/SellerLayout';
import Badge from '@/components/ui/Badge';
import Button from '@/components/ui/Button';
import Modal from '@/components/ui/Modal';
import Input from '@/components/ui/Input';
import { useToast } from '@/components/ui/Toast';
import fetcher from '@/lib/fetcher';
import sellerApi from '@/lib/service';

const TYPE_MAP = {
  1: { label: 'Yüzde İndirim', color: 'blue' },
  2: { label: 'Sabit Tutar', color: 'green' },
  3: { label: 'X Al Y Öde', color: 'purple' },
};

const defaultForm = {
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
  const [deleting, setDeleting] = useState(null);
  const [modal, setModal] = useState(false);
  const [editModal, setEditModal] = useState(null);
  const [form, setForm] = useState(defaultForm);
  const [editForm, setEditForm] = useState(defaultForm);
  const [saving, setSaving] = useState(false);
  const [editSaving, setEditSaving] = useState(false);

  const [restaurants, setRestaurants] = useState([]);
  const [menus, setMenus] = useState([]);
  const [categories, setCategories] = useState([]);
  const [editMenus, setEditMenus] = useState([]);
  const [editCategories, setEditCategories] = useState([]);

  const { data, isLoading, mutate } = useSWR('/v1/seller/coupon/list', fetcher);
  const coupons = data?.data || [];

  const isExpired = (endDate) => new Date(endDate) < new Date();

  // Fetch restaurants on mount for modals
  const ensureRestaurants = async () => {
    if (restaurants.length > 0) return;
    try {
      const result = await sellerApi.seller.restaurant.getList();
      if (result && !result.hasFailed) setRestaurants(result.data || []);
    } catch {}
  };

  const fetchMenusCats = async (restaurantId, mSetter, cSetter) => {
    if (!restaurantId) { mSetter([]); cSetter([]); return; }
    try {
      const [mRes, cRes] = await Promise.all([
        sellerApi.seller.menu.getByRestaurant({ restaurantId }).catch(() => null),
        sellerApi.seller.category.getList({ restaurantId }).catch(() => null),
      ]);
      mSetter(mRes?.data || []);
      cSetter(cRes?.data || []);
    } catch { mSetter([]); cSetter([]); }
  };

  const toLocalDatetime = (isoStr) => {
    if (!isoStr) return '';
    const d = new Date(isoStr);
    const pad = (n) => String(n).padStart(2, '0');
    return `${d.getFullYear()}-${pad(d.getMonth()+1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
  };

  const set = (key) => (e) => {
    const val = e.target.value;
    setForm((f) => ({ ...f, [key]: val }));
    if (key === 'restaurantId') {
      setForm((f) => ({ ...f, applicableMenuIds: [], applicableCategoryIds: [] }));
      fetchMenusCats(val, setMenus, setCategories);
    }
  };
  const setEdit = (key) => (e) => {
    const val = e.target.value;
    setEditForm((f) => ({ ...f, [key]: val }));
    if (key === 'restaurantId') {
      setEditForm((f) => ({ ...f, applicableMenuIds: [], applicableCategoryIds: [] }));
      fetchMenusCats(val, setEditMenus, setEditCategories);
    }
  };
  const toggleMulti = (formSetter, key, id) => {
    formSetter((f) => {
      const arr = f[key] || [];
      return { ...f, [key]: arr.includes(id) ? arr.filter(x => x !== id) : [...arr, id] };
    });
  };

  // Open create modal
  const openCreate = async () => {
    await ensureRestaurants();
    const today = new Date();
    const nextMonth = new Date(today); nextMonth.setMonth(nextMonth.getMonth() + 1);
    setForm({
      ...defaultForm,
      startDate: toLocalDatetime(today.toISOString()),
      endDate: toLocalDatetime(nextMonth.toISOString()),
    });
    setMenus([]); setCategories([]);
    setModal(true);
  };

  // Open edit modal
  const openEdit = async (coupon) => {
    await ensureRestaurants();
    setEditModal(coupon);
    setEditForm({
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
    if (coupon.restaurantId) await fetchMenusCats(coupon.restaurantId, setEditMenus, setEditCategories);
    else { setEditMenus([]); setEditCategories([]); }
  };

  const handleCreate = async (e) => {
    e.preventDefault();
    setSaving(true);
    try {
      const appType = parseInt(form.applicableType);
      const payload = {
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
        applicableType: appType,
        startDate: new Date(form.startDate).toISOString(),
        endDate: new Date(form.endDate).toISOString(),
        usageLimit: form.usageLimit ? parseInt(form.usageLimit) : null,
        usagePerUser: form.usagePerUser ? parseInt(form.usagePerUser) : null,
        applicableMenuIds: appType === 2 ? form.applicableMenuIds : null,
        applicableCategoryIds: appType === 3 ? form.applicableCategoryIds : null,
      };
      await sellerApi.seller.coupon.create(payload);
      toast('Kupon oluşturuldu', 'success');
      setModal(false);
      mutate();
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || 'Kupon oluşturulamadı', 'error');
    } finally { setSaving(false); }
  };

  const handleEdit = async (e) => {
    e.preventDefault();
    setEditSaving(true);
    try {
      const appType = parseInt(editForm.applicableType);
      const payload = {
        id: editModal.id,
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
      await sellerApi.seller.coupon.update(payload);
      toast('Kupon güncellendi', 'success');
      setEditModal(null);
      mutate();
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || 'Kupon güncellenemedi', 'error');
    } finally { setEditSaving(false); }
  };

  const handleDelete = async (id) => {
    setDeleting(id);
    try {
      await sellerApi.seller.coupon.remove({ id });
      toast('Kupon silindi', 'success'); mutate();
    } catch { toast('Kupon silinemedi', 'error'); }
    finally { setDeleting(null); }
  };

  const formatDate = (date) => {
    if (!date) return '—';
    return new Date(date).toLocaleDateString('tr-TR', { day: '2-digit', month: '2-digit', year: 'numeric' });
  };
  const formatValue = (coupon) => {
    if (coupon.type === 1) return `%${coupon.value}`;
    if (coupon.type === 2) return `₺${coupon.value}`;
    return `${coupon.buyQuantity} al ${coupon.getQuantity} öde`;
  };

  // Reusable coupon form JSX
  const renderCouponForm = (f, setter, menuList, catList, multiSetter) => (
    <>
      <div>
        <p className="text-xs font-semibold text-gray-400 uppercase mb-3">Temel Bilgiler</p>
        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className="text-sm font-medium text-gray-700 block mb-1">Restoran</label>
            <select value={f.restaurantId} onChange={setter('restaurantId')} className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-emerald-400">
              <option value="">Tüm Restoranlarda Geçerli</option>
              {restaurants.map((r) => <option key={r.id} value={r.id}>{r.name}</option>)}
            </select>
          </div>
          <Input label="Kupon Kodu *" required value={f.code} onChange={setter('code')} placeholder="Örn: INDIRIM20" />
          <Input label="Kupon Adı *" required value={f.name} onChange={setter('name')} />
          <Input label="Açıklama" value={f.description} onChange={setter('description')} />
        </div>
      </div>
      <div>
        <p className="text-xs font-semibold text-gray-400 uppercase mb-3">İndirim Bilgileri</p>
        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className="text-sm font-medium text-gray-700 block mb-1">İndirim Türü *</label>
            <select value={f.type} onChange={setter('type')} className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-emerald-400">
              <option value={1}>Yüzde</option>
              <option value={2}>Sabit Tutar</option>
              <option value={3}>X Al Y Öde</option>
            </select>
          </div>
          <Input label="Değer *" required type="number" step="0.01" value={f.value} onChange={setter('value')} placeholder={f.type == 1 ? '%' : '₺'} />
          {f.type == 1 && <Input label="Maks. İndirim" type="number" step="0.01" value={f.maxDiscountAmount} onChange={setter('maxDiscountAmount')} placeholder="₺" />}
          {f.type == 3 && <>
            <Input label="Alınacak Adet" type="number" value={f.buyQuantity} onChange={setter('buyQuantity')} />
            <Input label="Ödenecek Adet" type="number" value={f.getQuantity} onChange={setter('getQuantity')} />
          </>}
          <Input label="Min. Sipariş Tutarı" type="number" step="0.01" value={f.minOrderAmount} onChange={setter('minOrderAmount')} />
        </div>
      </div>
      <div>
        <p className="text-xs font-semibold text-gray-400 uppercase mb-3">Geçerlilik</p>
        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className="text-sm font-medium text-gray-700 block mb-1">Geçerli Olduğu Alan *</label>
            <select value={f.applicableType} onChange={setter('applicableType')} className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-emerald-400">
              <option value={1}>Tüm Ürünler</option>
              <option value={2}>Belirli Menüler</option>
              <option value={3}>Belirli Kategoriler</option>
            </select>
          </div>
          {f.applicableType == 2 && (
            <div className="col-span-2">
              <label className="text-sm font-medium text-gray-700 block mb-1">Geçerli Menüler</label>
              {menuList.length === 0 ? (
                <p className="text-xs text-gray-400">{f.restaurantId ? 'Menü bulunamadı' : 'Önce restoran seçiniz'}</p>
              ) : (
                <div className="border border-gray-300 rounded-lg p-2 max-h-40 overflow-y-auto space-y-1">
                  {menuList.map((m) => (
                    <label key={m.id} className="flex items-center gap-2 text-sm cursor-pointer hover:bg-gray-50 px-2 py-1 rounded">
                      <input type="checkbox" checked={f.applicableMenuIds?.includes(m.id)} onChange={() => multiSetter('applicableMenuIds', m.id)} className="w-4 h-4 accent-emerald-500" />
                      {m.name}
                    </label>
                  ))}
                </div>
              )}
            </div>
          )}
          {f.applicableType == 3 && (
            <div className="col-span-2">
              <label className="text-sm font-medium text-gray-700 block mb-1">Geçerli Kategoriler</label>
              {catList.length === 0 ? (
                <p className="text-xs text-gray-400">{f.restaurantId ? 'Kategori bulunamadı' : 'Önce restoran seçiniz'}</p>
              ) : (
                <div className="border border-gray-300 rounded-lg p-2 max-h-40 overflow-y-auto space-y-1">
                  {catList.map((c) => (
                    <label key={c.id} className="flex items-center gap-2 text-sm cursor-pointer hover:bg-gray-50 px-2 py-1 rounded">
                      <input type="checkbox" checked={f.applicableCategoryIds?.includes(c.id)} onChange={() => multiSetter('applicableCategoryIds', c.id)} className="w-4 h-4 accent-emerald-500" />
                      {c.name}
                    </label>
                  ))}
                </div>
              )}
            </div>
          )}
          <div>
            <label className="text-sm font-medium text-gray-700 block mb-1">Başlangıç *</label>
            <input type="datetime-local" value={f.startDate} onChange={setter('startDate')} required className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-emerald-400" />
          </div>
          <div>
            <label className="text-sm font-medium text-gray-700 block mb-1">Bitiş *</label>
            <input type="datetime-local" value={f.endDate} onChange={setter('endDate')} required className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-emerald-400" />
          </div>
          <Input label="Kullanım Limiti" type="number" value={f.usageLimit} onChange={setter('usageLimit')} placeholder="Boş = sınırsız" />
          <Input label="Kişi Başı Limit" type="number" value={f.usagePerUser} onChange={setter('usagePerUser')} placeholder="Boş = sınırsız" />
        </div>
      </div>
    </>
  );

  return (
    <SellerLayout title="Kuponlar">
      <div className="space-y-4">
        <div className="flex items-center justify-between">
          <p className="text-sm text-gray-500">{coupons.length} kupon</p>
          <Button onClick={openCreate}>
            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
            </svg>
            Yeni Kupon
          </Button>
        </div>

        {isLoading ? (
          <p className="text-center text-gray-400 py-10">Yükleniyor...</p>
        ) : coupons.length === 0 ? (
          <div className="text-center py-16 bg-white rounded-xl">
            <p className="text-gray-500 mb-4">Henüz kupon oluşturmamışsınız.</p>
            <Button onClick={openCreate}>İlk Kuponu Oluştur</Button>
          </div>
        ) : (
          <div className="bg-white rounded-xl shadow-sm overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b border-gray-100">
                  <th className="text-left px-4 py-3 font-semibold text-gray-600">Kod</th>
                  <th className="text-left px-4 py-3 font-semibold text-gray-600">Ad</th>
                  <th className="text-left px-4 py-3 font-semibold text-gray-600">Restoran</th>
                  <th className="text-left px-4 py-3 font-semibold text-gray-600">Tür</th>
                  <th className="text-left px-4 py-3 font-semibold text-gray-600">Değer</th>
                  <th className="text-left px-4 py-3 font-semibold text-gray-600">Geçerlilik</th>
                  <th className="text-left px-4 py-3 font-semibold text-gray-600">Kullanım</th>
                  <th className="text-left px-4 py-3 font-semibold text-gray-600">Durum</th>
                  <th className="text-left px-4 py-3 font-semibold text-gray-600">İşlem</th>
                </tr>
              </thead>
              <tbody>
                {coupons.map((coupon) => {
                  const active = !isExpired(coupon.endDate);
                  return (
                    <tr key={coupon.id} className={`border-b border-gray-50 hover:bg-gray-50 ${!active ? 'opacity-50' : ''}`}>
                      <td className="px-4 py-3"><code className="px-2 py-1 bg-gray-100 rounded text-xs font-mono">{coupon.code}</code></td>
                      <td className="px-4 py-3">{coupon.name}</td>
                      <td className="px-4 py-3 text-gray-500">{coupon.restaurantName || 'Tümü'}</td>
                      <td className="px-4 py-3"><Badge label={TYPE_MAP[coupon.type]?.label || '?'} color={TYPE_MAP[coupon.type]?.color || 'gray'} /></td>
                      <td className="px-4 py-3 font-medium">{formatValue(coupon)}</td>
                      <td className="px-4 py-3 text-xs">
                        <p>{formatDate(coupon.startDate)}</p>
                        <p className="text-gray-400">— {formatDate(coupon.endDate)}</p>
                      </td>
                      <td className="px-4 py-3">{coupon.currentUsageCount}{coupon.usageLimit ? ` / ${coupon.usageLimit}` : ''}</td>
                      <td className="px-4 py-3"><Badge label={active ? 'Aktif' : 'Süresi Dolmuş'} color={active ? 'green' : 'red'} /></td>
                      <td className="px-4 py-3">
                        <div className="flex gap-2">
                          <Button size="sm" variant="outline" onClick={() => openEdit(coupon)}>Düzenle</Button>
                          <Button size="sm" variant="outline" loading={deleting === coupon.id} onClick={() => { if (confirm('Bu kuponu silmek istediğinizden emin misiniz?')) handleDelete(coupon.id); }}>Sil</Button>
                        </div>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {/* Create Modal */}
      <Modal isOpen={modal} onClose={() => setModal(false)} title="Yeni Kupon Oluştur" size="xl">
        <form onSubmit={handleCreate} className="space-y-5">
          {renderCouponForm(form, set, menus, categories, (key, id) => toggleMulti(setForm, key, id))}
          <div className="flex justify-end gap-2 pt-2">
            <Button type="button" variant="secondary" onClick={() => setModal(false)}>İptal</Button>
            <Button type="submit" loading={saving}>Kupon Oluştur</Button>
          </div>
        </form>
      </Modal>

      {/* Edit Modal */}
      <Modal isOpen={!!editModal} onClose={() => setEditModal(null)} title={`Kupon Düzenle — ${editModal?.code}`} size="xl">
        <form onSubmit={handleEdit} className="space-y-5">
          {renderCouponForm(editForm, setEdit, editMenus, editCategories, (key, id) => toggleMulti(setEditForm, key, id))}
          <div className="flex justify-end gap-2 pt-2">
            <Button type="button" variant="secondary" onClick={() => setEditModal(null)}>İptal</Button>
            <Button type="submit" loading={editSaving}>Kaydet</Button>
          </div>
        </form>
      </Modal>
    </SellerLayout>
  );
}
