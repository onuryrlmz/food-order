'use client';

import { use, useState } from 'react';
import useSWR from 'swr';
import Link from 'next/link';
import SellerLayout from '@/components/layout/SellerLayout';
import Button from '@/components/ui/Button';
import Input from '@/components/ui/Input';
import Modal from '@/components/ui/Modal';
import { useToast } from '@/components/ui/Toast';
import fetcher from '@/lib/fetcher';
import { uploadProductImage, deleteProductImage } from '@/lib/api';
import sellerApi from '@/lib/service';

// ─── Image Upload Section ────────────────────────────────────────────────────
function ProductImageUpload({ productId, images = [], onUpdate }) {
  const toast = useToast();
  const [uploading, setUploading] = useState(false);

  const handleFileChange = async (e) => {
    const file = e.target.files?.[0];
    if (!file) return;
    const validTypes = ['image/jpeg', 'image/png', 'image/webp'];
    if (!validTypes.includes(file.type)) {
      toast('Sadece JPG, PNG, WebP dosyalari yuklenebilir', 'error');
      return;
    }
    if (file.size > 5 * 1024 * 1024) {
      toast('Dosya boyutu 5MB\'dan kucuk olmalidir', 'error');
      return;
    }
    setUploading(true);
    try {
      await uploadProductImage(productId, file);
      toast('Gorsel yuklendi', 'success');
      onUpdate?.();
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || 'Yuklenirken hata olustu', 'error');
    } finally {
      setUploading(false);
      e.target.value = '';
    }
  };

  const handleDelete = async (imageId) => {
    if (!confirm('Bu gorseli silmek istediginize emin misiniz?')) return;
    try {
      await deleteProductImage(imageId);
      toast('Gorsel silindi', 'success');
      onUpdate?.();
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || 'Silinirken hata olustu', 'error');
    }
  };

  return (
    <div className="col-span-2">
      <label className="block text-sm font-medium text-gray-700 mb-1.5">Urun Gorselleri</label>
      <div className="flex gap-2 flex-wrap mb-2">
        {images.map(img => (
          <div key={img.id} className="relative group w-20 h-20 rounded-lg overflow-hidden border border-gray-200">
            <img src={img.url} alt="" className="w-full h-full object-cover" />
            <button
              type="button"
              onClick={() => handleDelete(img.id)}
              className="absolute inset-0 bg-black/50 opacity-0 group-hover:opacity-100 transition-opacity flex items-center justify-center"
            >
              <svg className="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
              </svg>
            </button>
          </div>
        ))}
      </div>
      <label className={`inline-flex items-center gap-2 px-3 py-2 rounded-lg border border-dashed border-gray-300 text-sm text-gray-600 hover:bg-gray-50 cursor-pointer transition-colors ${uploading ? 'opacity-50 pointer-events-none' : ''}`}>
        {uploading ? (
          <div className="w-4 h-4 border-2 border-emerald-400 border-t-transparent rounded-full animate-spin" />
        ) : (
          <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
          </svg>
        )}
        {uploading ? 'Yukleniyor...' : 'Gorsel Yukle'}
        <input type="file" accept="image/jpeg,image/png,image/webp" className="hidden" onChange={handleFileChange} />
      </label>
    </div>
  );
}

// ─── Products Tab ────────────────────────────────────────────────────────────
function ProductsTab({ restaurantId }) {
  const toast = useToast();
  const [modal, setModal] = useState(null); // null | 'add' | product obj
  const [form, setForm] = useState({});
  const [saving, setSaving] = useState(false);
  const [deleting, setDeleting] = useState(null);

  const { data, mutate } = useSWR(
    `/v1/seller/product?restaurantId=${restaurantId}&getDetails=false`,
    fetcher
  );
  const products = data?.data || [];

  const openAdd = () => {
    setForm({ name: '', price: 0, productType: 1, description: '', orderIndex: 0 });
    setModal('add');
  };
  const openEdit = (p) => {
    setForm({ ...p });
    setModal(p);
  };

  const handleSave = async (e) => {
    e.preventDefault();
    setSaving(true);
    try {
      const payload = {
        ...form,
        restaurantId,
        price: parseFloat(form.price),
        productType: parseInt(form.productType),
        orderIndex: parseInt(form.orderIndex) || 0,
      };
      if (modal === 'add') await sellerApi.seller.product.create(payload);
      else await sellerApi.seller.product.update(payload);
      toast(modal === 'add' ? 'Ürün eklendi' : 'Ürün güncellendi', 'success');
      setModal(null);
      mutate();
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || 'Hata oluştu', 'error');
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async (id) => {
    if (!confirm('Bu ürünü silmek istediğinize emin misiniz?')) return;
    setDeleting(id);
    try {
      await sellerApi.seller.product.remove({ id, restaurantId });
      toast('Ürün silindi', 'success');
      mutate();
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || 'Hata oluştu', 'error');
    } finally {
      setDeleting(null);
    }
  };

  const set = (key) => (e) => setForm(f => ({ ...f, [key]: e.target.value }));

  return (
    <div className="space-y-4">
      <div className="flex justify-between items-center">
        <p className="text-sm text-gray-500">{products.length} ürün</p>
        <Button size="sm" onClick={openAdd}>+ Ürün Ekle</Button>
      </div>

      <div className="bg-white rounded-xl border border-gray-100 shadow-sm overflow-hidden">
        {products.length === 0 ? (
          <div className="p-10 text-center text-gray-400 text-sm">Henüz ürün yok</div>
        ) : (
          <table className="w-full text-sm">
            <thead className="bg-gray-50 text-xs text-gray-500 uppercase">
              <tr>
                <th className="px-4 py-3 text-left">Ürün Adı</th>
                <th className="px-4 py-3 text-left">Tip</th>
                <th className="px-4 py-3 text-right">Fiyat</th>
                <th className="px-4 py-3 text-right">Sıra</th>
                <th className="px-4 py-3" />
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-50">
              {products.map(p => (
                <tr key={p.id} className="hover:bg-gray-50">
                  <td className="px-4 py-3">
                    <p className="font-medium text-gray-800">{p.name}</p>
                    {p.description && <p className="text-xs text-gray-400 mt-0.5">{p.description}</p>}
                  </td>
                  <td className="px-4 py-3">
                    <span className={`text-xs font-medium px-2 py-0.5 rounded-full ${p.productType === 1 ? 'bg-blue-100 text-blue-700' : 'bg-purple-100 text-purple-700'}`}>
                      {p.productType === 1 ? 'Master' : 'Sub'}
                    </span>
                  </td>
                  <td className="px-4 py-3 text-right font-semibold text-emerald-600">₺{Number(p.price).toFixed(2)}</td>
                  <td className="px-4 py-3 text-right text-gray-400">{p.orderIndex}</td>
                  <td className="px-4 py-3">
                    <div className="flex gap-2 justify-end">
                      <Button size="sm" variant="outline" onClick={() => openEdit(p)}>Düzenle</Button>
                      <Button size="sm" variant="danger" loading={deleting === p.id} onClick={() => handleDelete(p.id)}>Sil</Button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      <Modal isOpen={!!modal} onClose={() => setModal(null)} title={modal === 'add' ? 'Ürün Ekle' : 'Ürün Düzenle'} size="md">
        <form onSubmit={handleSave} className="space-y-4">
          <div className="grid grid-cols-2 gap-3">
            <div className="col-span-2">
              <Input label="Ürün Adı" required value={form.name || ''} onChange={set('name')} />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1.5">Tip</label>
              <select value={form.productType || 1} onChange={set('productType')}
                className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-emerald-400">
                <option value={1}>Master (Ana ürün)</option>
                <option value={2}>Sub (Seçenek/Ek)</option>
              </select>
            </div>
            <Input label="Fiyat (₺)" type="number" step="0.01" value={form.price || 0} onChange={set('price')} />
            <Input label="Sıra" type="number" value={form.orderIndex || 0} onChange={set('orderIndex')} />
            <div className="col-span-2">
              <label className="block text-sm font-medium text-gray-700 mb-1.5">Açıklama</label>
              <textarea value={form.description || ''} onChange={(e) => setForm(f => ({ ...f, description: e.target.value }))}
                rows={2} className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-emerald-400 resize-none" />
            </div>
            {modal && modal !== 'add' && modal.id && (
              <ProductImageUpload
                productId={modal.id}
                images={modal.images || []}
                onUpdate={() => mutate()}
              />
            )}
          </div>
          <div className="flex justify-end gap-2 pt-2">
            <Button type="button" variant="secondary" onClick={() => setModal(null)}>İptal</Button>
            <Button type="submit" loading={saving}>Kaydet</Button>
          </div>
        </form>
      </Modal>
    </div>
  );
}

// ─── Categories Tab ───────────────────────────────────────────────────────────
function CategoriesTab({ restaurantId }) {
  const toast = useToast();
  const [modal, setModal] = useState(null);
  const [form, setForm] = useState({});
  const [saving, setSaving] = useState(false);
  const [assignModal, setAssignModal] = useState(null); // category obj
  const [assignMenuId, setAssignMenuId] = useState('');
  const [assigning, setAssigning] = useState(false);

  const { data: catData, mutate: mutateCats } = useSWR(
    `/v1/seller/category?restaurantId=${restaurantId}&getMenus=true`,
    fetcher
  );
  const { data: menuData } = useSWR(`/v1/seller/menu/by-restaurant/${restaurantId}/with-options`, fetcher);
  const categories = catData?.data || [];
  const menus = menuData?.data || [];

  const openAdd = () => { setForm({ name: '', orderIndex: 0 }); setModal('add'); };
  const openEdit = (c) => { setForm({ ...c }); setModal(c); };
  const set = (key) => (e) => setForm(f => ({ ...f, [key]: e.target.value }));

  const handleSave = async (e) => {
    e.preventDefault();
    setSaving(true);
    try {
      const payload = { ...form, restaurantId, orderIndex: parseInt(form.orderIndex) || 0 };
      if (modal === 'add') await sellerApi.seller.category.create(payload);
      else await sellerApi.seller.category.update(payload);
      toast(modal === 'add' ? 'Kategori eklendi' : 'Kategori güncellendi', 'success');
      setModal(null);
      mutateCats();
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || 'Hata oluştu', 'error');
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async (id) => {
    if (!confirm('Kategoriyi silmek istiyor musunuz?')) return;
    try {
      await sellerApi.seller.category.remove({ id, restaurantId });
      toast('Kategori silindi', 'success');
      mutateCats();
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || 'Hata oluştu', 'error');
    }
  };

  const handleAssignMenu = async () => {
    if (!assignMenuId) return;
    setAssigning(true);
    try {
      const existingDetails = assignModal.categoryDetails || [];
      await sellerApi.seller.category.createDetail({
        categoryId: assignModal.id,
        menuId: assignMenuId,
        orderIndex: existingDetails.length,
      });
      toast('Menü kategoriye eklendi', 'success');
      setAssignModal(null);
      setAssignMenuId('');
      mutateCats();
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || 'Hata oluştu', 'error');
    } finally {
      setAssigning(false);
    }
  };

  const handleRemoveDetail = async (detail) => {
    try {
      await sellerApi.seller.category.removeDetail({ id: detail.id, categoryId: detail.categoryId, restaurantId });
      toast('Menü kategoriden çıkarıldı', 'success');
      mutateCats();
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || 'Hata oluştu', 'error');
    }
  };

  return (
    <div className="space-y-4">
      <div className="flex justify-between items-center">
        <p className="text-sm text-gray-500">{categories.length} kategori</p>
        <Button size="sm" onClick={openAdd}>+ Kategori Ekle</Button>
      </div>

      {categories.length === 0 ? (
        <div className="bg-white rounded-xl border border-gray-100 shadow-sm p-10 text-center text-gray-400 text-sm">Henüz kategori yok</div>
      ) : (
        <div className="space-y-3">
          {categories.map(cat => (
            <div key={cat.id} className="bg-white rounded-xl border border-gray-100 shadow-sm overflow-hidden">
              <div className="flex items-center justify-between px-5 py-3 bg-gray-50 border-b border-gray-100">
                <div className="flex items-center gap-3">
                  <span className="font-semibold text-gray-700">{cat.name}</span>
                  <span className="text-xs text-gray-400">Sıra: {cat.orderIndex}</span>
                </div>
                <div className="flex gap-2">
                  <Button size="sm" variant="outline" onClick={() => { setAssignModal(cat); setAssignMenuId(''); }}>+ Menü Ekle</Button>
                  <Button size="sm" variant="ghost" onClick={() => openEdit(cat)}>Düzenle</Button>
                  <Button size="sm" variant="danger" onClick={() => handleDelete(cat.id)}>Sil</Button>
                </div>
              </div>
              <div className="divide-y divide-gray-50">
                {(cat.menus || []).length === 0 ? (
                  <p className="px-5 py-3 text-xs text-gray-400">Henüz menü eklenmedi</p>
                ) : (
                  (cat.menus || []).map(menu => (
                    <div key={menu.id} className="flex items-center justify-between px-5 py-2.5">
                      <span className="text-sm text-gray-700">{menu.name}</span>
                      <div className="flex items-center gap-3">
                        <span className="text-sm font-semibold text-emerald-600">₺{Number(menu.price).toFixed(2)}</span>
                        <button
                          onClick={() => {
                            const detail = (cat.categoryDetails || []).find(d => d.menuId === menu.id);
                            if (detail) handleRemoveDetail(detail);
                          }}
                          className="text-xs text-red-400 hover:text-red-600"
                        >
                          Çıkar
                        </button>
                      </div>
                    </div>
                  ))
                )}
              </div>
            </div>
          ))}
        </div>
      )}

      {/* Add/Edit Category Modal */}
      <Modal isOpen={!!modal} onClose={() => setModal(null)} title={modal === 'add' ? 'Kategori Ekle' : 'Kategori Düzenle'} size="sm">
        <form onSubmit={handleSave} className="space-y-4">
          <Input label="Kategori Adı" required value={form.name || ''} onChange={set('name')} />
          <Input label="Sıra" type="number" value={form.orderIndex || 0} onChange={set('orderIndex')} />
          <div className="flex justify-end gap-2">
            <Button type="button" variant="secondary" onClick={() => setModal(null)}>İptal</Button>
            <Button type="submit" loading={saving}>Kaydet</Button>
          </div>
        </form>
      </Modal>

      {/* Assign Menu Modal */}
      <Modal isOpen={!!assignModal} onClose={() => setAssignModal(null)} title={`"${assignModal?.name}" — Menü Ekle`} size="sm">
        <div className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1.5">Menü Seç</label>
            <select value={assignMenuId} onChange={e => setAssignMenuId(e.target.value)}
              className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-emerald-400">
              <option value="">Menü seçin...</option>
              {menus.map(m => (
                <option key={m.id} value={m.id}>{m.name} — ₺{Number(m.price).toFixed(2)}</option>
              ))}
            </select>
          </div>
          <div className="flex justify-end gap-2">
            <Button variant="secondary" onClick={() => setAssignModal(null)}>İptal</Button>
            <Button loading={assigning} onClick={handleAssignMenu}>Ekle</Button>
          </div>
        </div>
      </Modal>
    </div>
  );
}

// ─── Menu Detail Modal (Trendyol style) ──────────────────────────────────────
function MenuDetailModal({ menu, onClose, restaurantId, mutate, products, subProducts, toast, templates }) {
  const [expandedOpt, setExpandedOpt] = useState(null);
  const [subModal, setSubModal] = useState(null); // { type, targetId }
  const [subForm, setSubForm] = useState({});
  const [subSaving, setSubSaving] = useState(false);
  const [editModal, setEditModal] = useState(null); // { type, data }
  const [editForm, setEditForm] = useState({});
  const [editSaving, setEditSaving] = useState(false);
  // Template picker state (used when subModal.type === 'option')
  const [optionMode, setOptionMode] = useState('new'); // 'new' | 'template'
  const [selectedTemplateId, setSelectedTemplateId] = useState('');

  const masterProducts = products.filter(p => p.productType === 1);

  const call = async (fn) => { try { await fn(); mutate(); } catch (err) { toast(err.response?.data?.messages?.[0]?.description || 'Hata oluştu', 'error'); } };

  const handleSubSave = async (e) => {
    e.preventDefault();
    setSubSaving(true);
    try {
      const { type, targetId } = subModal;
      if (type === 'option') {
        if (optionMode === 'template' && selectedTemplateId) {
          await sellerApi.seller.menu.createOption({ menuId: targetId, optionTemplateId: selectedTemplateId });
        } else {
          await sellerApi.seller.menu.createOption({ menuId: targetId, name: subForm.name, minCount: +subForm.minCount||1, maxCount: +subForm.maxCount||1, orderIndex: 0 });
        }
      }
      if (type === 'value')        await sellerApi.seller.menu.createOptionValue({ menuOptionId: targetId, productId: subForm.productId, price: +subForm.price||0, orderIndex: 0 });
      if (type === 'vOption')      await sellerApi.seller.menu.createValueOption({ menuOptionValueId: targetId, name: subForm.name, minCount: +subForm.minCount||0, maxCount: +subForm.maxCount||99, orderIndex: 0 });
      if (type === 'vOptionValue') await sellerApi.seller.menu.createValueOptionValue({ menuOptionValueOptionId: targetId, productId: subForm.productId, price: +subForm.price||0, orderIndex: 0 });
      toast('Eklendi', 'success'); setSubModal(null); mutate();
    } catch (err) { toast(err.response?.data?.messages?.[0]?.description || 'Hata oluştu', 'error'); }
    finally { setSubSaving(false); }
  };

  const handleEditSave = async (e) => {
    e.preventDefault();
    setEditSaving(true);
    try {
      const { type } = editModal;
      if (type === 'menu')         await sellerApi.seller.menu.update({ ...editForm, restaurantId });
      if (type === 'option')       await sellerApi.seller.menu.updateOption(editForm);
      if (type === 'vOption')      await sellerApi.seller.menu.updateValueOption(editForm);
      if (type === 'vOptionValue') await sellerApi.seller.menu.updateValueOptionValue({ id: editForm.id, productId: editForm.productId, price: +editForm.price || 0, orderIndex: editForm.orderIndex || 0 });
      toast('Güncellendi', 'success'); setEditModal(null); mutate();
    } catch (err) { toast(err.response?.data?.messages?.[0]?.description || 'Hata oluştu', 'error'); }
    finally { setEditSaving(false); }
  };

  const openAdd = (type, targetId, defaults = {}) => { setSubForm(defaults); setSubModal({ type, targetId }); };
  const openEdit = (type, data) => { setEditForm({ ...data }); setEditModal({ type }); };
  const ef = (k) => (e) => setEditForm(f => ({ ...f, [k]: e.target.value }));
  const sf = (k) => (e) => setSubForm(f => ({ ...f, [k]: e.target.value }));

  return (
    <div className="fixed inset-0 z-50 flex items-end sm:items-center justify-center bg-black/50" onClick={onClose}>
      <div
        className="bg-gray-50 w-full sm:max-w-md rounded-t-3xl sm:rounded-2xl overflow-hidden flex flex-col"
        style={{ maxHeight: '90vh' }}
        onClick={e => e.stopPropagation()}
      >
        {/* ── Header ── */}
        <div className="flex items-center justify-between px-5 py-4 bg-white border-b border-gray-100 shrink-0">
          <button onClick={onClose} className="w-8 h-8 flex items-center justify-center rounded-full hover:bg-gray-100 text-gray-500">✕</button>
          <span className="font-semibold text-gray-800 text-base">{menu.name}</span>
          <button onClick={() => openEdit('menu', menu)} className="text-sm text-emerald-600 font-medium hover:text-emerald-800">Düzenle</button>
        </div>

        <div className="overflow-y-auto flex-1">
          {/* ── Menü bilgi kartı ── */}
          <div className="bg-white mx-3 mt-3 rounded-2xl p-5 shadow-sm">
            <div className="w-24 h-24 bg-gray-100 rounded-2xl mx-auto mb-4 flex items-center justify-center text-4xl">🍽️</div>
            <h2 className="text-lg font-bold text-gray-900 text-center">{menu.name}</h2>
            {menu.description && <p className="text-sm text-gray-500 text-center mt-1">{menu.description}</p>}
            <div className="flex items-center justify-between mt-4 pt-4 border-t border-gray-100">
              <span className="text-sm text-gray-500">Fiyat</span>
              <span className="text-emerald-600 font-bold text-xl">₺{Number(menu.price).toFixed(2)}</span>
            </div>
          </div>

          {/* ── Seçenek grupları ── */}
          <div className="mx-3 mt-3 space-y-2 pb-4">
            {(menu.menuOptions || []).map(opt => (
              <div key={opt.id} className="bg-white rounded-2xl shadow-sm overflow-hidden">
                {/* Accordion başlık */}
                <div
                  className="w-full flex items-center justify-between px-4 py-4 text-left cursor-pointer select-none"
                  onClick={() => setExpandedOpt(expandedOpt === opt.id ? null : opt.id)}
                >
                  <div>
                    <span className={`font-semibold text-sm ${expandedOpt === opt.id ? 'text-emerald-600' : 'text-gray-800'}`}>{opt.name}</span>
                    <span className="text-xs text-gray-400 ml-2">{opt.minCount}–{opt.maxCount} seçim</span>
                    {opt.optionTemplateId && <span className="ml-2 text-xs bg-blue-50 text-blue-500 border border-blue-100 rounded px-1.5 py-0.5">🔗 Şablon</span>}
                  </div>
                  <div className="flex items-center gap-2">
                    <button onClick={e => { e.stopPropagation(); openEdit('option', opt); }} className="text-xs text-gray-400 hover:text-gray-700 px-2 py-0.5 rounded border border-gray-200">Düzenle</button>
                    <button onClick={e => { e.stopPropagation(); call(() => sellerApi.seller.menu.removeOption({ id: opt.id })); }} className="text-xs text-red-400 hover:text-red-600 px-2 py-0.5 rounded border border-red-100">Sil</button>
                    <svg className={`w-4 h-4 text-gray-400 transition-transform ${expandedOpt === opt.id ? 'rotate-180' : ''}`} fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" /></svg>
                  </div>
                </div>

                {expandedOpt === opt.id && (
                  <div className="border-t border-gray-100 px-4 pb-4 space-y-3 pt-3">
                    {(opt.menuOptionValues || []).map(val => (
                      <div key={val.id} className="border border-gray-100 rounded-xl overflow-hidden">
                        {/* Master ürün satırı */}
                        <div className="flex items-center justify-between px-3 py-2.5 bg-gray-50">
                          <div>
                            <span className="text-sm font-semibold text-gray-800">{val.product?.name || '—'}</span>
                            {val.price > 0 && <span className="text-xs text-emerald-600 ml-2">+₺{Number(val.price).toFixed(2)}</span>}
                          </div>
                          <div className="flex gap-1">
                            <button onClick={() => openAdd('vOption', val.id, { name: '', minCount: 0, maxCount: 99 })} className="text-xs text-emerald-500 hover:text-emerald-700 px-2 py-0.5 rounded border border-emerald-200">+ Malzeme Grubu</button>
                            <button onClick={() => call(() => sellerApi.seller.menu.removeOptionValue({ id: val.id }))} className="text-xs text-red-400 hover:text-red-600 px-2 py-0.5 rounded border border-red-100">Sil</button>
                          </div>
                        </div>

                        {/* Malzeme grupları */}
                        {(val.menuOptionValueOptions || []).map(vOpt => (
                          <div key={vOpt.id} className="px-3 py-3 border-t border-gray-100">
                            <div className="flex items-center justify-between mb-2">
                              <div>
                                <span className="text-sm font-medium text-gray-700">{vOpt.name}</span>
                                <span className="text-xs text-gray-400 ml-1">({vOpt.minCount}–{vOpt.maxCount})</span>
                              </div>
                              <div className="flex gap-1">
                                <button onClick={() => openEdit('vOption', vOpt)} className="text-xs text-gray-400 hover:text-gray-700 px-2 py-0.5 rounded border border-gray-200">Düzenle</button>
                                <button onClick={() => openAdd('vOptionValue', vOpt.id, { productId: '', price: 0 })} className="text-xs text-emerald-500 hover:text-emerald-700 px-2 py-0.5 rounded border border-emerald-200">+ Ekle</button>
                                <button onClick={() => call(() => sellerApi.seller.menu.removeValueOption({ id: vOpt.id }))} className="text-xs text-red-400 hover:text-red-600 px-2 py-0.5 rounded border border-red-100">Sil</button>
                              </div>
                            </div>
                            {/* Sub ürün chips */}
                            <div className="flex flex-wrap gap-2">
                              {(vOpt.menuOptionValueOptionValues || []).map(vov => (
                                <span key={vov.id} className="group/chip flex items-center gap-1 px-3 py-1 border border-gray-200 rounded-full text-xs text-gray-700 bg-white hover:border-gray-300">
                                  {vov.product?.name || '—'}
                                  {vov.price > 0 && <span className="text-emerald-600">+₺{Number(vov.price).toFixed(2)}</span>}
                                  <button onClick={() => openEdit('vOptionValue', { ...vov, productId: vov.product?.id || vov.productId })} className="text-blue-300 hover:text-blue-500 ml-0.5 hidden group-hover/chip:inline" title="Düzenle">✎</button>
                                  <button onClick={() => call(() => sellerApi.seller.menu.removeValueOptionValue({ id: vov.id }))} className="text-red-300 hover:text-red-500 hidden group-hover/chip:inline">✕</button>
                                </span>
                              ))}
                            </div>
                          </div>
                        ))}
                      </div>
                    ))}

                    <button onClick={() => openAdd('value', opt.id, { productId: '', price: 0 })}
                      className="w-full py-2 text-sm text-emerald-600 border border-dashed border-emerald-200 rounded-xl hover:bg-emerald-50">
                      + Master Ürün Ekle
                    </button>
                  </div>
                )}
              </div>
            ))}

            <button onClick={() => { setOptionMode('new'); setSelectedTemplateId(''); openAdd('option', menu.id, { name: '', minCount: 1, maxCount: 1 }); }}
              className="w-full py-3 text-sm text-emerald-600 border border-dashed border-emerald-200 rounded-2xl hover:bg-emerald-50">
              + Seçenek Grubu Ekle
            </button>
          </div>
        </div>
      </div>

      {/* ── Sub modal (Ekle) ── */}
      {subModal && (
        <div className="fixed inset-0 z-60 flex items-center justify-center bg-black/40" onClick={() => setSubModal(null)}>
          <div className="bg-white rounded-2xl p-5 w-80 shadow-xl space-y-4" onClick={e => e.stopPropagation()}>
            <h3 className="font-semibold text-gray-800">
              {{ option: 'Seçenek Grubu Ekle', value: 'Master Ürün Ekle', vOption: 'Malzeme Grubu Ekle', vOptionValue: 'Malzeme Ekle' }[subModal.type]}
            </h3>
            <form onSubmit={handleSubSave} className="space-y-3">
              {subModal.type === 'option' && (
                <>
                  <div className="flex rounded-lg border border-gray-200 overflow-hidden text-sm">
                    <button type="button" onClick={() => setOptionMode('new')}
                      className={`flex-1 py-1.5 font-medium transition-colors ${optionMode === 'new' ? 'bg-emerald-50 text-emerald-700' : 'text-gray-500 hover:bg-gray-50'}`}>
                      Yeni Oluştur
                    </button>
                    <button type="button" onClick={() => setOptionMode('template')}
                      disabled={!templates || templates.length === 0}
                      className={`flex-1 py-1.5 font-medium transition-colors ${optionMode === 'template' ? 'bg-emerald-50 text-emerald-700' : 'text-gray-500 hover:bg-gray-50'} disabled:opacity-40`}>
                      Şablondan Ekle
                    </button>
                  </div>
                  {optionMode === 'new' ? (
                    <>
                      <Input label="Ad" required value={subForm.name || ''} onChange={sf('name')} />
                      <div className="grid grid-cols-2 gap-2">
                        <Input label="Min" type="number" value={subForm.minCount ?? 0} onChange={sf('minCount')} />
                        <Input label="Max" type="number" value={subForm.maxCount ?? 1} onChange={sf('maxCount')} />
                      </div>
                    </>
                  ) : (
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">Şablon Seç</label>
                      <select value={selectedTemplateId} onChange={e => setSelectedTemplateId(e.target.value)} required
                        className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-emerald-400">
                        <option value="">Şablon seçin...</option>
                        {(templates || []).map(t => (
                          <option key={t.id} value={t.id}>{t.name} ({t.minCount}–{t.maxCount})</option>
                        ))}
                      </select>
                    </div>
                  )}
                </>
              )}
              {subModal.type === 'vOption' && (
                <>
                  <Input label="Ad" required value={subForm.name || ''} onChange={sf('name')} />
                  <div className="grid grid-cols-2 gap-2">
                    <Input label="Min" type="number" value={subForm.minCount ?? 0} onChange={sf('minCount')} />
                    <Input label="Max" type="number" value={subForm.maxCount ?? 1} onChange={sf('maxCount')} />
                  </div>
                </>
              )}
              {['value', 'vOptionValue'].includes(subModal.type) && (
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Ürün</label>
                  <select value={subForm.productId || ''} onChange={sf('productId')} required
                    className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-emerald-400">
                    <option value="">Seçin...</option>
                    {(subModal.type === 'value' ? masterProducts : subProducts).map(p => (
                      <option key={p.id} value={p.id}>{p.name}{p.price > 0 ? ` — ₺${p.price}` : ''}</option>
                    ))}
                  </select>
                </div>
              )}
              {['value', 'vOptionValue'].includes(subModal.type) && (
                <Input label="Ek Fiyat (₺)" type="number" step="0.01" value={subForm.price || 0} onChange={sf('price')} />
              )}
              <div className="flex justify-end gap-2 pt-1">
                <Button type="button" variant="secondary" onClick={() => setSubModal(null)}>İptal</Button>
                <Button type="submit" loading={subSaving}>Ekle</Button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* ── Edit modal (Düzenle) ── */}
      {editModal && (
        <div className="fixed inset-0 z-60 flex items-center justify-center bg-black/40" onClick={() => setEditModal(null)}>
          <div className="bg-white rounded-2xl p-5 w-80 shadow-xl space-y-4" onClick={e => e.stopPropagation()}>
            <h3 className="font-semibold text-gray-800">
              {{ menu: 'Menü Düzenle', option: 'Seçenek Grubu Düzenle', vOption: 'Malzeme Grubu Düzenle', vOptionValue: 'Malzeme Düzenle' }[editModal.type]}
            </h3>
            <form onSubmit={handleEditSave} className="space-y-3">
              {editModal.type === 'vOptionValue' && (
                <>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Ürün</label>
                    <select value={editForm.productId || ''} onChange={ef('productId')} required
                      className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-emerald-400">
                      <option value="">Seçin...</option>
                      {subProducts.map(p => (
                        <option key={p.id} value={p.id}>{p.name}{p.price > 0 ? ` — ₺${p.price}` : ''}</option>
                      ))}
                    </select>
                  </div>
                  <Input label="Ek Fiyat (₺)" type="number" step="0.01" value={editForm.price ?? 0} onChange={ef('price')} />
                </>
              )}
              {editModal.type === 'menu' && (
                <>
                  <Input label="Menü Adı" required value={editForm.name || ''} onChange={ef('name')} />
                  <div className="grid grid-cols-2 gap-2">
                    <Input label="Fiyat (₺)" type="number" step="0.01" value={editForm.price || 0} onChange={ef('price')} />
                    <Input label="Sıra" type="number" value={editForm.orderIndex || 0} onChange={ef('orderIndex')} />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Açıklama</label>
                    <textarea value={editForm.description || ''} onChange={(e) => setEditForm(f => ({ ...f, description: e.target.value }))}
                      rows={2} className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-emerald-400 resize-none" />
                  </div>
                </>
              )}
              {['option', 'vOption'].includes(editModal.type) && (
                <>
                  <Input label="Ad" required value={editForm.name || ''} onChange={ef('name')} />
                  <div className="grid grid-cols-2 gap-2">
                    <Input label="Min" type="number" value={editForm.minCount ?? 0} onChange={ef('minCount')} />
                    <Input label="Max" type="number" value={editForm.maxCount ?? 1} onChange={ef('maxCount')} />
                  </div>
                </>
              )}
              <div className="flex justify-end gap-2 pt-1">
                <Button type="button" variant="secondary" onClick={() => setEditModal(null)}>İptal</Button>
                <Button type="submit" loading={editSaving}>Kaydet</Button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}

// ─── Menus Tab ────────────────────────────────────────────────────────────────
function MenusTab({ restaurantId }) {
  const toast = useToast();
  const [selectedMenuId, setSelectedMenuId] = useState(null);
  const [addModal, setAddModal] = useState(false);
  const [form, setForm] = useState({});
  const [saving, setSaving] = useState(false);

  const { data: menuData, mutate } = useSWR(`/v1/seller/menu/by-restaurant/${restaurantId}/with-options`, fetcher);
  const { data: productData } = useSWR(`/v1/seller/product?restaurantId=${restaurantId}&getDetails=false`, fetcher);
  const { data: templateData } = useSWR(`/v1/seller/option-template/by-restaurant/${restaurantId}`, fetcher);
  const menus = menuData?.data || [];
  const products = productData?.data || [];
  const subProducts = products.filter(p => p.productType === 2);
  const templates = templateData?.data || [];

  const selectedMenu = menus.find(m => m.id === selectedMenuId) || null;
  const set = (k) => (e) => setForm(f => ({ ...f, [k]: e.target.value }));

  const handleSave = async (e) => {
    e.preventDefault(); setSaving(true);
    try {
      await sellerApi.seller.menu.create({ ...form, restaurantId, price: parseFloat(form.price), orderIndex: parseInt(form.orderIndex) || 0 });
      toast('Menü eklendi', 'success'); setAddModal(false); mutate();
    } catch (err) { toast(err.response?.data?.messages?.[0]?.description || 'Hata oluştu', 'error'); }
    finally { setSaving(false); }
  };

  const handleDelete = async (id, e) => {
    e.stopPropagation();
    if (!confirm('Bu menüyü silmek istiyor musunuz?')) return;
    try {
      await sellerApi.seller.menu.remove({ id, restaurantId });
      toast('Menü silindi', 'success'); mutate();
    } catch (err) { toast(err.response?.data?.messages?.[0]?.description || 'Hata oluştu', 'error'); }
  };

  return (
    <div className="space-y-4">
      <div className="flex justify-between items-center">
        <p className="text-sm text-gray-500">{menus.length} menü — tıklayarak düzenleyin</p>
        <Button size="sm" onClick={() => { setForm({ name: '', price: 0, description: '', orderIndex: 0 }); setAddModal(true); }}>+ Menü Ekle</Button>
      </div>

      {menus.length === 0 ? (
        <div className="bg-white rounded-xl border border-gray-100 shadow-sm p-10 text-center text-gray-400 text-sm">Henüz menü yok</div>
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3">
          {menus.map(menu => (
            <div key={menu.id} onClick={() => setSelectedMenuId(menu.id)}
              className="bg-white rounded-2xl border border-gray-100 shadow-sm p-4 cursor-pointer hover:shadow-md hover:border-emerald-200 transition-all group">
              <div className="flex items-start justify-between">
                <div className="flex-1 min-w-0">
                  <p className="font-semibold text-gray-800 truncate">{menu.name}</p>
                  {menu.description && <p className="text-xs text-gray-400 truncate mt-0.5">{menu.description}</p>}
                </div>
                <button onClick={(e) => handleDelete(menu.id, e)} className="opacity-0 group-hover:opacity-100 text-red-400 hover:text-red-600 text-xs ml-2 shrink-0">Sil</button>
              </div>
              <div className="flex items-center justify-between mt-3 pt-3 border-t border-gray-100">
                <span className="text-xs text-gray-400">{(menu.menuOptions || []).length} seçenek grubu</span>
                <span className="text-emerald-600 font-bold">₺{Number(menu.price).toFixed(2)}</span>
              </div>
            </div>
          ))}
        </div>
      )}

      {/* Menü Detay Modal */}
      {selectedMenu && (
        <MenuDetailModal
          menu={selectedMenu}
          onClose={() => setSelectedMenuId(null)}
          restaurantId={restaurantId}
          mutate={mutate}
          products={products}
          subProducts={subProducts}
          toast={toast}
          templates={templates}
        />
      )}

      {/* Menü Ekle */}
      <Modal isOpen={addModal} onClose={() => setAddModal(false)} title="Menü Ekle" size="md">
        <form onSubmit={handleSave} className="space-y-4">
          <Input label="Menü Adı" required value={form.name || ''} onChange={set('name')} />
          <div className="grid grid-cols-2 gap-3">
            <Input label="Fiyat (₺)" type="number" step="0.01" value={form.price || 0} onChange={set('price')} />
            <Input label="Sıra" type="number" value={form.orderIndex || 0} onChange={set('orderIndex')} />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1.5">Açıklama</label>
            <textarea value={form.description || ''} onChange={(e) => setForm(f => ({ ...f, description: e.target.value }))}
              rows={2} className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-emerald-400 resize-none" />
          </div>
          <div className="flex justify-end gap-2">
            <Button type="button" variant="secondary" onClick={() => setAddModal(false)}>İptal</Button>
            <Button type="submit" loading={saving}>Kaydet</Button>
          </div>
        </form>
      </Modal>
    </div>
  );
}

// ─── Templates Tab ────────────────────────────────────────────────────────────
function TemplatesTab({ restaurantId }) {
  const toast = useToast();
  const [selectedTemplateId, setSelectedTemplateId] = useState(null);
  const [addModal, setAddModal] = useState(false);
  const [form, setForm] = useState({});
  const [saving, setSaving] = useState(false);
  const [expandedVal, setExpandedVal] = useState(null);
  const [subModal, setSubModal] = useState(null);
  const [subForm, setSubForm] = useState({});
  const [subSaving, setSubSaving] = useState(false);
  const [editModal, setEditModal] = useState(null);
  const [editForm, setEditForm] = useState({});
  const [editSaving, setEditSaving] = useState(false);

  const { data: templateData, mutate } = useSWR(`/v1/seller/option-template/by-restaurant/${restaurantId}`, fetcher);
  const { data: productData } = useSWR(`/v1/seller/product?restaurantId=${restaurantId}&getDetails=false`, fetcher);
  const templates = templateData?.data || [];
  const allProducts = productData?.data || [];
  const masterProducts = allProducts.filter(p => p.productType === 1);
  const subProducts = allProducts.filter(p => p.productType === 2);

  const selectedTemplate = templates.find(t => t.id === selectedTemplateId) || null;
  const set = (k) => (e) => setForm(f => ({ ...f, [k]: e.target.value }));
  const sf = (k) => (e) => setSubForm(f => ({ ...f, [k]: e.target.value }));
  const ef = (k) => (e) => setEditForm(f => ({ ...f, [k]: e.target.value }));

  const call = async (fn) => { try { await fn(); mutate(); } catch (err) { toast(err.response?.data?.messages?.[0]?.description || 'Hata oluştu', 'error'); } };

  const handleSave = async (e) => {
    e.preventDefault(); setSaving(true);
    try {
      await sellerApi.seller.optionTemplate.create({ ...form, restaurantId, minCount: +form.minCount || 1, maxCount: +form.maxCount || 1 });
      toast('Şablon eklendi', 'success'); setAddModal(false); mutate();
    } catch (err) { toast(err.response?.data?.messages?.[0]?.description || 'Hata oluştu', 'error'); }
    finally { setSaving(false); }
  };

  const handleSubSave = async (e) => {
    e.preventDefault(); setSubSaving(true);
    try {
      const { type, targetId } = subModal;
      if (type === 'value')       await sellerApi.seller.optionTemplate.createValue({ optionTemplateId: targetId, productId: subForm.productId, price: +subForm.price || 0 });
      if (type === 'vOption')     await sellerApi.seller.optionTemplate.createValueOption({ optionTemplateValueId: targetId, name: subForm.name, minCount: +subForm.minCount || 0, maxCount: +subForm.maxCount || 99 });
      if (type === 'vOptionValue') await sellerApi.seller.optionTemplate.createValueOptionValue({ optionTemplateValueOptionId: targetId, productId: subForm.productId, price: +subForm.price || 0 });
      toast('Eklendi', 'success'); setSubModal(null); mutate();
    } catch (err) { toast(err.response?.data?.messages?.[0]?.description || 'Hata oluştu', 'error'); }
    finally { setSubSaving(false); }
  };

  const handleEditSave = async (e) => {
    e.preventDefault(); setEditSaving(true);
    try {
      const { type } = editModal;
      if (type === 'template')     await sellerApi.seller.optionTemplate.update({ ...editForm, restaurantId });
      if (type === 'vOption')      await sellerApi.seller.optionTemplate.updateValueOption(editForm);
      if (type === 'vOptionValue') await sellerApi.seller.optionTemplate.updateValueOptionValue({ id: editForm.id, productId: editForm.productId, price: +editForm.price || 0 });
      toast('Güncellendi', 'success'); setEditModal(null); mutate();
    } catch (err) { toast(err.response?.data?.messages?.[0]?.description || 'Hata oluştu', 'error'); }
    finally { setEditSaving(false); }
  };

  return (
    <div className="space-y-4">
      <div className="flex justify-between items-center">
        <p className="text-sm text-gray-500">{templates.length} şablon — tıklayarak düzenleyin</p>
        <Button size="sm" onClick={() => { setForm({ name: '', minCount: 1, maxCount: 1 }); setAddModal(true); }}>+ Şablon Ekle</Button>
      </div>

      {templates.length === 0 ? (
        <div className="bg-white rounded-xl border border-gray-100 shadow-sm p-10 text-center text-gray-400 text-sm">
          <div className="text-2xl mb-2">📋</div>
          Henüz şablon yok. Şablon oluşturarak aynı seçenek grubunu birden fazla menüde kullanabilirsiniz.
        </div>
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3">
          {templates.map(t => (
            <div key={t.id} onClick={() => setSelectedTemplateId(t.id)}
              className="bg-white rounded-2xl border border-gray-100 shadow-sm p-4 cursor-pointer hover:shadow-md hover:border-emerald-200 transition-all group">
              <div className="flex items-start justify-between">
                <div className="flex-1 min-w-0">
                  <p className="font-semibold text-gray-800 truncate">{t.name}</p>
                  {t.description && <p className="text-xs text-gray-400 truncate mt-0.5">{t.description}</p>}
                </div>
                <button onClick={(e) => { e.stopPropagation(); if (!confirm('Bu şablonu silmek istiyor musunuz?')) return; call(() => sellerApi.seller.optionTemplate.remove({ id: t.id, restaurantId })); }}
                  className="opacity-0 group-hover:opacity-100 text-red-400 hover:text-red-600 text-xs ml-2 shrink-0">Sil</button>
              </div>
              <div className="flex items-center justify-between mt-3 pt-3 border-t border-gray-100">
                <span className="text-xs text-gray-400">{(t.optionTemplateValues || []).length} ürün</span>
                <span className="text-xs text-blue-500 border border-blue-100 bg-blue-50 rounded px-2 py-0.5">{t.minCount}–{t.maxCount} seçim</span>
              </div>
            </div>
          ))}
        </div>
      )}

      {/* Template Detail Modal */}
      {selectedTemplate && (
        <div className="fixed inset-0 z-50 flex items-end sm:items-center justify-center bg-black/50" onClick={() => setSelectedTemplateId(null)}>
          <div className="bg-gray-50 w-full sm:max-w-md rounded-t-3xl sm:rounded-2xl overflow-hidden flex flex-col" style={{ maxHeight: '90vh' }} onClick={e => e.stopPropagation()}>
            <div className="flex items-center justify-between px-5 py-4 bg-white border-b border-gray-100 shrink-0">
              <button onClick={() => setSelectedTemplateId(null)} className="w-8 h-8 flex items-center justify-center rounded-full hover:bg-gray-100 text-gray-500">✕</button>
              <span className="font-semibold text-gray-800">{selectedTemplate.name}</span>
              <button onClick={() => { setEditForm({ ...selectedTemplate }); setEditModal({ type: 'template' }); }} className="text-sm text-emerald-600 font-medium hover:text-emerald-800">Düzenle</button>
            </div>
            <div className="overflow-y-auto flex-1">
              <div className="bg-white mx-3 mt-3 rounded-2xl p-4 shadow-sm flex items-center justify-between">
                <span className="text-sm text-gray-600">Seçim aralığı</span>
                <span className="text-blue-600 font-semibold">{selectedTemplate.minCount}–{selectedTemplate.maxCount}</span>
              </div>

              <div className="mx-3 mt-3 space-y-2 pb-4">
                {(selectedTemplate.optionTemplateValues || []).map(val => (
                  <div key={val.id} className="bg-white rounded-2xl shadow-sm overflow-hidden">
                    <div className="flex items-center justify-between px-4 py-3 bg-gray-50 cursor-pointer select-none"
                      onClick={() => setExpandedVal(expandedVal === val.id ? null : val.id)}>
                      <div>
                        <span className="font-semibold text-sm text-gray-800">{val.product?.name || '—'}</span>
                        {val.price > 0 && <span className="text-xs text-emerald-600 ml-2">+₺{Number(val.price).toFixed(2)}</span>}
                      </div>
                      <div className="flex gap-1">
                        <button onClick={(e) => { e.stopPropagation(); openSubModal('vOption', val.id, { name: '', minCount: 0, maxCount: 99 }); }}
                          className="text-xs text-emerald-500 hover:text-emerald-700 px-2 py-0.5 rounded border border-emerald-200">+ Malzeme Grubu</button>
                        <button onClick={(e) => { e.stopPropagation(); call(() => sellerApi.seller.optionTemplate.removeValue({ id: val.id })); }}
                          className="text-xs text-red-400 hover:text-red-600 px-2 py-0.5 rounded border border-red-100">Sil</button>
                        <svg className={`w-4 h-4 text-gray-400 transition-transform ${expandedVal === val.id ? 'rotate-180' : ''}`} fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" /></svg>
                      </div>
                    </div>
                    {expandedVal === val.id && (
                      <div className="px-3 pb-3 pt-2 space-y-2">
                        {(val.optionTemplateValueOptions || []).map(vOpt => (
                          <div key={vOpt.id} className="border border-gray-100 rounded-xl p-3">
                            <div className="flex items-center justify-between mb-2">
                              <div>
                                <span className="text-sm font-medium text-gray-700">{vOpt.name}</span>
                                <span className="text-xs text-gray-400 ml-1">({vOpt.minCount}–{vOpt.maxCount})</span>
                              </div>
                              <div className="flex gap-1">
                                <button onClick={() => { setEditForm({ ...vOpt }); setEditModal({ type: 'vOption' }); }}
                                  className="text-xs text-gray-400 hover:text-gray-700 px-2 py-0.5 rounded border border-gray-200">Düzenle</button>
                                <button onClick={() => openSubModal('vOptionValue', vOpt.id, { productId: '', price: 0 })}
                                  className="text-xs text-emerald-500 hover:text-emerald-700 px-2 py-0.5 rounded border border-emerald-200">+ Ekle</button>
                                <button onClick={() => call(() => sellerApi.seller.optionTemplate.removeValueOption({ id: vOpt.id }))}
                                  className="text-xs text-red-400 hover:text-red-600 px-2 py-0.5 rounded border border-red-100">Sil</button>
                              </div>
                            </div>
                            <div className="flex flex-wrap gap-1.5">
                              {(vOpt.optionTemplateValueOptionValues || []).map(ov => (
                                <span key={ov.id} className="group/chip relative inline-flex items-center gap-1 text-xs bg-gray-100 text-gray-700 rounded-full px-2.5 py-1">
                                  {ov.product?.name}{ov.price > 0 ? ` +₺${ov.price}` : ''}
                                  <button onClick={() => { setEditForm({ ...ov, productId: ov.product?.id || ov.productId }); setEditModal({ type: 'vOptionValue' }); }}
                                    className="opacity-0 group-hover/chip:opacity-100 text-blue-400 hover:text-blue-600 ml-0.5 leading-none" title="Düzenle">✎</button>
                                  <button onClick={() => call(() => sellerApi.seller.optionTemplate.removeValueOptionValue({ id: ov.id }))}
                                    className="opacity-0 group-hover/chip:opacity-100 text-red-400 hover:text-red-600 leading-none">✕</button>
                                </span>
                              ))}
                            </div>
                          </div>
                        ))}
                        <button onClick={() => openSubModal('vOption', val.id, { name: '', minCount: 0, maxCount: 99 })}
                          className="w-full py-1.5 text-xs text-emerald-600 border border-dashed border-emerald-200 rounded-xl hover:bg-emerald-50">
                          + Malzeme Grubu Ekle
                        </button>
                      </div>
                    )}
                  </div>
                ))}
                <button onClick={() => openSubModal('value', selectedTemplate.id, { productId: '', price: 0 })}
                  className="w-full py-3 text-sm text-emerald-600 border border-dashed border-emerald-200 rounded-2xl hover:bg-emerald-50">
                  + Ürün Ekle
                </button>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Sub modal */}
      {subModal && (
        <div className="fixed inset-0 z-[60] flex items-center justify-center bg-black/40" onClick={() => setSubModal(null)}>
          <div className="bg-white rounded-2xl p-5 w-80 shadow-xl space-y-4" onClick={e => e.stopPropagation()}>
            <h3 className="font-semibold text-gray-800">
              {{ value: 'Ürün Ekle', vOption: 'Malzeme Grubu Ekle', vOptionValue: 'Malzeme Ekle' }[subModal.type]}
            </h3>
            <form onSubmit={handleSubSave} className="space-y-3">
              {subModal.type === 'vOption' && (
                <>
                  <Input label="Ad" required value={subForm.name || ''} onChange={sf('name')} />
                  <div className="grid grid-cols-2 gap-2">
                    <Input label="Min" type="number" value={subForm.minCount ?? 0} onChange={sf('minCount')} />
                    <Input label="Max" type="number" value={subForm.maxCount ?? 99} onChange={sf('maxCount')} />
                  </div>
                </>
              )}
              {['value', 'vOptionValue'].includes(subModal.type) && (
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Ürün</label>
                  <select value={subForm.productId || ''} onChange={sf('productId')} required
                    className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-emerald-400">
                    <option value="">Seçin...</option>
                    {(subModal.type === 'value' ? masterProducts : subProducts).map(p => (
                      <option key={p.id} value={p.id}>{p.name}{p.price > 0 ? ` — ₺${p.price}` : ''}</option>
                    ))}
                  </select>
                </div>
              )}
              {['value', 'vOptionValue'].includes(subModal.type) && (
                <Input label="Ek Fiyat (₺)" type="number" step="0.01" value={subForm.price || 0} onChange={sf('price')} />
              )}
              <div className="flex justify-end gap-2 pt-1">
                <Button type="button" variant="secondary" onClick={() => setSubModal(null)}>İptal</Button>
                <Button type="submit" loading={subSaving}>Ekle</Button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Edit modal */}
      {editModal && (
        <div className="fixed inset-0 z-[60] flex items-center justify-center bg-black/40" onClick={() => setEditModal(null)}>
          <div className="bg-white rounded-2xl p-5 w-80 shadow-xl space-y-4" onClick={e => e.stopPropagation()}>
            <h3 className="font-semibold text-gray-800">
              {{ template: 'Şablon Düzenle', vOption: 'Malzeme Grubu Düzenle', vOptionValue: 'Malzeme Düzenle' }[editModal.type]}
            </h3>
            <form onSubmit={handleEditSave} className="space-y-3">
              {editModal.type === 'vOptionValue' && (
                <>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Ürün</label>
                    <select value={editForm.productId || ''} onChange={ef('productId')} required
                      className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-emerald-400">
                      <option value="">Seçin...</option>
                      {subProducts.map(p => (
                        <option key={p.id} value={p.id}>{p.name}{p.price > 0 ? ` — ₺${p.price}` : ''}</option>
                      ))}
                    </select>
                  </div>
                  <Input label="Ek Fiyat (₺)" type="number" step="0.01" value={editForm.price ?? 0} onChange={ef('price')} />
                </>
              )}
              {editModal.type === 'template' && (
                <>
                  <Input label="Ad" required value={editForm.name || ''} onChange={ef('name')} />
                  <div className="grid grid-cols-2 gap-2">
                    <Input label="Min" type="number" value={editForm.minCount ?? 1} onChange={ef('minCount')} />
                    <Input label="Max" type="number" value={editForm.maxCount ?? 1} onChange={ef('maxCount')} />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Açıklama</label>
                    <textarea value={editForm.description || ''} onChange={(e) => setEditForm(f => ({ ...f, description: e.target.value }))}
                      rows={2} className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-emerald-400 resize-none" />
                  </div>
                </>
              )}
              {editModal.type === 'vOption' && (
                <>
                  <Input label="Ad" required value={editForm.name || ''} onChange={ef('name')} />
                  <div className="grid grid-cols-2 gap-2">
                    <Input label="Min" type="number" value={editForm.minCount ?? 0} onChange={ef('minCount')} />
                    <Input label="Max" type="number" value={editForm.maxCount ?? 99} onChange={ef('maxCount')} />
                  </div>
                </>
              )}
              <div className="flex justify-end gap-2 pt-1">
                <Button type="button" variant="secondary" onClick={() => setEditModal(null)}>İptal</Button>
                <Button type="submit" loading={editSaving}>Kaydet</Button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Add Template Modal */}
      <Modal isOpen={addModal} onClose={() => setAddModal(false)} title="Şablon Ekle" size="md">
        <form onSubmit={handleSave} className="space-y-4">
          <Input label="Şablon Adı" required value={form.name || ''} onChange={set('name')} placeholder="ör: Pizza Seçimi" />
          <div className="grid grid-cols-2 gap-3">
            <Input label="Min Seçim" type="number" value={form.minCount ?? 1} onChange={set('minCount')} />
            <Input label="Max Seçim" type="number" value={form.maxCount ?? 1} onChange={set('maxCount')} />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1.5">Açıklama (opsiyonel)</label>
            <textarea value={form.description || ''} onChange={(e) => setForm(f => ({ ...f, description: e.target.value }))}
              rows={2} className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-emerald-400 resize-none" />
          </div>
          <div className="flex justify-end gap-2">
            <Button type="button" variant="secondary" onClick={() => setAddModal(false)}>İptal</Button>
            <Button type="submit" loading={saving}>Kaydet</Button>
          </div>
        </form>
      </Modal>
    </div>
  );

  function openSubModal(type, targetId, defaults) {
    setSubForm(defaults);
    setSubModal({ type, targetId });
  }
}

// ─── Main Page ────────────────────────────────────────────────────────────────
export default function MenuManagementPage({ params }) {
  const { id } = use(params);
  const [activeTab, setActiveTab] = useState('menus');

  const { data: restaurantsData } = useSWR('/v1/seller/restaurant/list', fetcher);
  const restaurant = restaurantsData?.data?.find(r => r.id === id);

  const tabs = [
    { key: 'menus', label: 'Menüler' },
    { key: 'templates', label: 'Şablonlar' },
    { key: 'categories', label: 'Kategoriler' },
    { key: 'products', label: 'Ürünler' },
  ];

  return (
    <SellerLayout
      title={`Menü Yönetimi${restaurant ? ` — ${restaurant.name}` : ''}`}
      headerActions={
        <Link href={`/restaurants/${id}`} className="text-sm text-gray-500 hover:text-gray-700">
          ← Restorana Dön
        </Link>
      }
    >
      <div className="mb-4 bg-gray-100 p-1 rounded-xl w-fit flex gap-1">
        {tabs.map(tab => (
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
      </div>

      {activeTab === 'menus' && <MenusTab restaurantId={id} />}
      {activeTab === 'templates' && <TemplatesTab restaurantId={id} />}
      {activeTab === 'categories' && <CategoriesTab restaurantId={id} />}
      {activeTab === 'products' && <ProductsTab restaurantId={id} />}
    </SellerLayout>
  );
}
