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
import api from '@/lib/api';

const AGREEMENT_STATUS = { 1: 'Onay Bekliyor', 2: 'Aktif', 3: 'Askıda', 4: 'Sonlandırıldı' };
const AGREEMENT_STATUS_COLOR = { 1: 'yellow', 2: 'green', 3: 'orange', 4: 'red' };
const ASSIGNMENT_STRATEGY = { 1: 'Manuel', 2: 'Otomatik (En Yakın)', 3: 'Yayın (İlk Kabul)' };
const DELIVERY_STATUS = { 1: 'Bekliyor', 2: 'Teklif Edildi', 3: 'Kabul Edildi', 4: 'Reddedildi', 5: 'Teslim Alındı', 6: 'Teslim Edildi', 7: 'İptal', 8: 'Süre Doldu' };
const DELIVERY_STATUS_COLOR = { 1: 'gray', 2: 'blue', 3: 'green', 4: 'red', 5: 'purple', 6: 'green', 7: 'red', 8: 'orange' };
const COURIER_AVAILABILITY = { 0: 'Çevrimdışı', 1: 'Çevrimiçi', 2: 'Teslivat\'ta' };
const COURIER_AVAILABILITY_COLOR = { 0: 'gray', 1: 'green', 2: 'blue' };

const TABS = [
  { key: 'agreements', label: 'Anlaşmalar' },
  { key: 'couriers', label: 'Kuryeler' },
  { key: 'deliveries', label: 'Aktif Teslimatlar' },
];

const EMPTY_AGREEMENT = {
  courierCompanyId: '',
  strategyId: 1,
  feePerDelivery: '',
  priority: 1,
};

export default function CouriersPage() {
  const toast = useToast();
  const [selectedRestaurant, setSelectedRestaurant] = useState(null);
  const [activeTab, setActiveTab] = useState('agreements');
  const [agreementModal, setAgreementModal] = useState(null); // null=closed, object=form data
  const [settingsModal, setSettingsModal] = useState(false);
  const [saving, setSaving] = useState(false);
  const [deleteConfirm, setDeleteConfirm] = useState(null);
  const [courierInfoModal, setCourierInfoModal] = useState(false);

  // Settings form state
  const [settingsForm, setSettingsForm] = useState({ defaultAssignmentStrategy: 1, hasOwnCouriers: false });

  const { data: restaurantsData } = useSWR('/v1/seller/restaurant/list', fetcher);
  const restaurants = restaurantsData?.data || [];

  // Agreements
  const agreementsUrl = selectedRestaurant ? `/v1/seller/courier/restaurant/${selectedRestaurant}/agreements` : null;
  const { data: agreementsData, isLoading: agreementsLoading, mutate: mutateAgreements } = useSWR(
    activeTab === 'agreements' ? agreementsUrl : null, fetcher
  );
  const agreements = agreementsData?.data || [];

  // Couriers
  const couriersUrl = selectedRestaurant ? `/v1/seller/courier/restaurant/${selectedRestaurant}/couriers` : null;
  const { data: couriersData, isLoading: couriersLoading } = useSWR(
    activeTab === 'couriers' ? couriersUrl : null, fetcher
  );
  const couriers = couriersData?.data || [];

  // Deliveries
  const deliveriesUrl = selectedRestaurant ? `/v1/seller/courier/restaurant/${selectedRestaurant}/deliveries` : null;
  const { data: deliveriesData, isLoading: deliveriesLoading, mutate: mutateDeliveries } = useSWR(
    activeTab === 'deliveries' ? deliveriesUrl : null, fetcher
  );
  const deliveries = deliveriesData?.data || [];

  const isLoading = activeTab === 'agreements' ? agreementsLoading : activeTab === 'couriers' ? couriersLoading : deliveriesLoading;

  // Agreement CRUD
  const handleSaveAgreement = async () => {
    if (!agreementModal) return;
    setSaving(true);
    try {
      const payload = {
        ...agreementModal,
        strategyId: Number(agreementModal.strategyId),
        feePerDelivery: Number(agreementModal.feePerDelivery),
        priority: Number(agreementModal.priority),
      };
      if (agreementModal.id) {
        await api.put(`/v1/seller/courier/agreements/${agreementModal.id}`, payload);
        toast('Anlaşma güncellendi', 'success');
      } else {
        await api.post(`/v1/seller/courier/restaurant/${selectedRestaurant}/agreements`, payload);
        toast('Anlaşma oluşturuldu', 'success');
      }
      mutateAgreements();
      setAgreementModal(null);
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || 'Hata oluştu', 'error');
    } finally {
      setSaving(false);
    }
  };

  const handleDeleteAgreement = async (id) => {
    setSaving(true);
    try {
      await api.delete(`/v1/seller/courier/agreements/${id}`);
      toast('Anlaşma sonlandırıldı', 'success');
      mutateAgreements();
      setDeleteConfirm(null);
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || 'Hata oluştu', 'error');
    } finally {
      setSaving(false);
    }
  };

  // Manual assign
  const handleManualAssign = async (orderId, courierId) => {
    try {
      await api.post(`/v1/seller/courier/restaurant/${selectedRestaurant}/assign/${orderId}?courierId=${courierId}`);
      toast('Kurye atandı', 'success');
      mutateDeliveries();
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || 'Hata oluştu', 'error');
    }
  };

  // Settings
  const handleSaveSettings = async () => {
    setSaving(true);
    try {
      await api.put(`/v1/seller/courier/restaurant/${selectedRestaurant}/settings`, {
        defaultAssignmentStrategy: Number(settingsForm.defaultAssignmentStrategy),
        hasOwnCouriers: settingsForm.hasOwnCouriers,
      });
      toast('Teslimat ayarları güncellendi', 'success');
      setSettingsModal(false);
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || 'Hata oluştu', 'error');
    } finally {
      setSaving(false);
    }
  };

  return (
    <SellerLayout title="Kurye Yönetimi">
      <div className="space-y-4">
        {/* Restoran seç */}
        {restaurants.length > 0 && (
          <div className="bg-white rounded-xl border border-gray-100 shadow-sm p-4">
            <p className="text-xs font-semibold text-gray-400 uppercase mb-2">Restoran Seç</p>
            <div className="flex flex-wrap gap-2">
              {restaurants.map(r => (
                <button
                  key={r.id}
                  onClick={() => { setSelectedRestaurant(r.id); }}
                  className={`px-3 py-1.5 rounded-lg text-sm font-medium transition-colors ${
                    selectedRestaurant === r.id
                      ? 'bg-emerald-600 text-white'
                      : 'bg-gray-100 text-gray-600 hover:bg-gray-200'
                  }`}
                >
                  {r.name}
                </button>
              ))}
            </div>
          </div>
        )}

        {!selectedRestaurant ? (
          <div className="text-center py-20 text-gray-400">
            <p className="text-sm">Kurye bilgilerini görmek için bir restoran seçin</p>
          </div>
        ) : (
          <>
            {/* Tab bar + action buttons */}
            <div className="flex items-center justify-between flex-wrap gap-2">
              <div className="flex flex-wrap gap-2">
                {TABS.map(tab => (
                  <button
                    key={tab.key}
                    onClick={() => setActiveTab(tab.key)}
                    className={`px-3 py-1.5 rounded-lg text-xs font-medium transition-colors ${
                      activeTab === tab.key
                        ? 'bg-emerald-600 text-white'
                        : 'bg-white border border-gray-200 text-gray-600 hover:bg-gray-50'
                    }`}
                  >
                    {tab.label}
                  </button>
                ))}
              </div>
              <div className="flex gap-2">
                {activeTab === 'agreements' && (
                  <Button size="sm" onClick={() => setAgreementModal({ ...EMPTY_AGREEMENT })}>
                    + Yeni Anlaşma
                  </Button>
                )}
                {activeTab === 'couriers' && (
                  <Button size="sm" onClick={() => setCourierInfoModal(true)}>
                    + Kendi Kuryeni Ekle
                  </Button>
                )}
                <Button size="sm" variant="outline" onClick={() => setSettingsModal(true)}>
                  Teslimat Ayarları
                </Button>
              </div>
            </div>

            {/* Content */}
            {isLoading ? (
              <div className="flex justify-center py-10">
                <div className="w-8 h-8 border-2 border-emerald-500 border-t-transparent rounded-full animate-spin" />
              </div>
            ) : (
              <>
                {/* Agreements Tab */}
                {activeTab === 'agreements' && (
                  agreements.length === 0 ? (
                    <div className="bg-white rounded-xl border border-gray-100 shadow-sm p-10 text-center">
                      <p className="text-gray-400 text-sm">Henüz kurye anlaşması yok</p>
                    </div>
                  ) : (
                    <div className="bg-white rounded-xl border border-gray-100 shadow-sm overflow-hidden">
                      <div className="overflow-x-auto">
                        <table className="w-full text-sm">
                          <thead>
                            <tr className="bg-gray-50 text-left">
                              <th className="px-4 py-3 text-xs font-semibold text-gray-500 uppercase">Firma / Kurye</th>
                              <th className="px-4 py-3 text-xs font-semibold text-gray-500 uppercase">Strateji</th>
                              <th className="px-4 py-3 text-xs font-semibold text-gray-500 uppercase">Ücret</th>
                              <th className="px-4 py-3 text-xs font-semibold text-gray-500 uppercase">Öncelik</th>
                              <th className="px-4 py-3 text-xs font-semibold text-gray-500 uppercase">Durum</th>
                              <th className="px-4 py-3 text-xs font-semibold text-gray-500 uppercase">İşlem</th>
                            </tr>
                          </thead>
                          <tbody className="divide-y divide-gray-100">
                            {agreements.map(ag => (
                              <tr key={ag.id} className="hover:bg-gray-50/50">
                                <td className="px-4 py-3">
                                  <p className="font-medium text-gray-800">{ag.courierCompanyName || ag.courierName || '-'}</p>
                                </td>
                                <td className="px-4 py-3 text-gray-600">{ASSIGNMENT_STRATEGY[ag.strategyId] || '-'}</td>
                                <td className="px-4 py-3 font-medium text-emerald-600">₺{Number(ag.feePerDelivery || 0).toFixed(2)}</td>
                                <td className="px-4 py-3 text-gray-600">{ag.priority}</td>
                                <td className="px-4 py-3">
                                  <Badge label={AGREEMENT_STATUS[ag.statusId] || '?'} color={AGREEMENT_STATUS_COLOR[ag.statusId] || 'gray'} />
                                </td>
                                <td className="px-4 py-3">
                                  <div className="flex gap-1">
                                    <Button
                                      size="sm"
                                      variant="ghost"
                                      onClick={() => setAgreementModal({
                                        id: ag.id,
                                        courierCompanyId: ag.courierCompanyId || '',
                                        strategyId: ag.strategyId || 1,
                                        feePerDelivery: ag.feePerDelivery || '',
                                        priority: ag.priority || 1,
                                      })}
                                    >
                                      Düzenle
                                    </Button>
                                    {ag.statusId !== 4 && (
                                      <Button size="sm" variant="ghost" onClick={() => setDeleteConfirm(ag)}>
                                        Sonlandır
                                      </Button>
                                    )}
                                  </div>
                                </td>
                              </tr>
                            ))}
                          </tbody>
                        </table>
                      </div>
                    </div>
                  )
                )}

                {/* Couriers Tab */}
                {activeTab === 'couriers' && (
                  <>
                    {/* HasOwnCouriers status note */}
                    {settingsForm.hasOwnCouriers ? (
                      <div className="bg-emerald-50 border border-emerald-200 rounded-xl p-4 flex items-start gap-3">
                        <svg className="w-5 h-5 text-emerald-600 mt-0.5 flex-shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
                          <path strokeLinecap="round" strokeLinejoin="round" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                        </svg>
                        <div>
                          <p className="text-sm font-medium text-emerald-800">Kendi kuryelerimiz var - Aktif</p>
                          <p className="text-xs text-emerald-600 mt-0.5">Restoran kuryeleri bu listede gorunur. Kuryeleriniz courier uygulamasindan kayit olarak sisteme dahil olabilir.</p>
                        </div>
                      </div>
                    ) : (
                      <div className="bg-amber-50 border border-amber-200 rounded-xl p-4 flex items-start gap-3">
                        <svg className="w-5 h-5 text-amber-600 mt-0.5 flex-shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
                          <path strokeLinecap="round" strokeLinejoin="round" d="M12 9v2m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                        </svg>
                        <div>
                          <p className="text-sm font-medium text-amber-800">Kendi kurye ozelliginiz aktif degil</p>
                          <p className="text-xs text-amber-600 mt-0.5">
                            Kendi kuryelerinizi ekleyebilmek icin{' '}
                            <button
                              onClick={() => setSettingsModal(true)}
                              className="underline font-medium hover:text-amber-800 transition-colors"
                            >
                              Teslimat Ayarlari
                            </button>
                            {' '}sayfasindan &quot;Kendi kuryelerimiz var&quot; secenegini aktif edin.
                          </p>
                        </div>
                      </div>
                    )}
                    {couriers.length === 0 ? (
                    <div className="bg-white rounded-xl border border-gray-100 shadow-sm p-10 text-center">
                      <p className="text-gray-400 text-sm">Uygun kurye bulunamadi</p>
                    </div>
                  ) : (
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                      {couriers.map(c => (
                        <div key={c.id} className="bg-white rounded-xl border border-gray-100 shadow-sm p-6">
                          <div className="flex items-start justify-between mb-3">
                            <div>
                              <h3 className="font-semibold text-gray-800">{c.fullName || c.name || '-'}</h3>
                              <p className="text-xs text-gray-400 mt-0.5">{c.vehicleType || 'Belirtilmemiş'}</p>
                            </div>
                            <Badge
                              label={COURIER_AVAILABILITY[c.availabilityStatus] || 'Bilinmiyor'}
                              color={COURIER_AVAILABILITY_COLOR[c.availabilityStatus] || 'gray'}
                            />
                          </div>
                          <div className="flex items-center gap-4 text-sm text-gray-500">
                            <div className="flex items-center gap-1">
                              <svg className="w-4 h-4 text-yellow-400" fill="currentColor" viewBox="0 0 20 20">
                                <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
                              </svg>
                              <span>{c.rating != null ? Number(c.rating).toFixed(1) : '-'}</span>
                            </div>
                            {c.completedDeliveries != null && (
                              <span className="text-xs">{c.completedDeliveries} teslimat</span>
                            )}
                          </div>
                          {c.phone && (
                            <p className="text-xs text-gray-400 mt-2">{c.phone}</p>
                          )}
                        </div>
                      ))}
                    </div>
                  )}
                  </>
                )}

                {/* Deliveries Tab */}
                {activeTab === 'deliveries' && (
                  deliveries.length === 0 ? (
                    <div className="bg-white rounded-xl border border-gray-100 shadow-sm p-10 text-center">
                      <p className="text-gray-400 text-sm">Aktif teslimat bulunamadı</p>
                    </div>
                  ) : (
                    <div className="space-y-2">
                      {deliveries.map(d => (
                        <div key={d.id} className="bg-white rounded-xl border border-gray-100 shadow-sm p-4 flex items-center gap-4">
                          <div className="flex-1 min-w-0">
                            <div className="flex items-center gap-2 mb-1">
                              <span className="font-mono text-xs text-gray-400">
                                {(d.orderId || d.id || '').substring(0, 8)}
                              </span>
                              <Badge
                                label={DELIVERY_STATUS[d.statusId] || '?'}
                                color={DELIVERY_STATUS_COLOR[d.statusId] || 'gray'}
                              />
                              {d.courierName && (
                                <span className="text-sm font-medium text-gray-700">{d.courierName}</span>
                              )}
                              {d.createdDate && (
                                <span className="text-xs text-gray-400 ml-auto">
                                  {new Date(d.createdDate).toLocaleString('tr-TR')}
                                </span>
                              )}
                            </div>
                            <div className="flex items-center gap-3 text-xs text-gray-500">
                              {d.estimatedDeliveryTime && (
                                <span>Tahmini: {d.estimatedDeliveryTime} dk</span>
                              )}
                              {d.elapsedMinutes != null && (
                                <span>Geçen: {d.elapsedMinutes} dk</span>
                              )}
                              {d.customerAddress && (
                                <span className="truncate max-w-xs">{d.customerAddress}</span>
                              )}
                            </div>
                          </div>
                          {d.statusId === 1 && (
                            <Button size="sm" variant="outline" onClick={() => {
                              const courierId = prompt('Kurye ID girin:');
                              if (courierId) handleManualAssign(d.orderId || d.id, courierId);
                            }}>
                              Kurye Ata
                            </Button>
                          )}
                        </div>
                      ))}
                    </div>
                  )
                )}
              </>
            )}
          </>
        )}
      </div>

      {/* Create/Edit Agreement Modal */}
      <Modal isOpen={!!agreementModal} onClose={() => setAgreementModal(null)} title={agreementModal?.id ? 'Anlaşmayı Düzenle' : 'Yeni Anlaşma'} size="md">
        {agreementModal && (
          <div className="space-y-4">
            <Input
              label="Kurye Firma ID"
              placeholder="Kurye firma ID'sini girin"
              value={agreementModal.courierCompanyId}
              onChange={e => setAgreementModal(prev => ({ ...prev, courierCompanyId: e.target.value }))}
            />
            <div className="w-full">
              <label className="block text-sm font-medium text-gray-700 mb-1.5">Atama Stratejisi</label>
              <select
                value={agreementModal.strategyId}
                onChange={e => setAgreementModal(prev => ({ ...prev, strategyId: Number(e.target.value) }))}
                className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-emerald-400 focus:border-emerald-400"
              >
                {Object.entries(ASSIGNMENT_STRATEGY).map(([k, v]) => (
                  <option key={k} value={k}>{v}</option>
                ))}
              </select>
            </div>
            <Input
              label="Teslimat Başı Ücret (₺)"
              type="number"
              placeholder="0.00"
              value={agreementModal.feePerDelivery}
              onChange={e => setAgreementModal(prev => ({ ...prev, feePerDelivery: e.target.value }))}
            />
            <Input
              label="Öncelik"
              type="number"
              placeholder="1"
              min="1"
              value={agreementModal.priority}
              onChange={e => setAgreementModal(prev => ({ ...prev, priority: e.target.value }))}
            />
            <div className="flex justify-end gap-2 pt-2">
              <Button variant="secondary" onClick={() => setAgreementModal(null)}>Vazgeç</Button>
              <Button loading={saving} onClick={handleSaveAgreement}>
                {agreementModal.id ? 'Güncelle' : 'Oluştur'}
              </Button>
            </div>
          </div>
        )}
      </Modal>

      {/* Delete Confirm Modal */}
      <Modal isOpen={!!deleteConfirm} onClose={() => setDeleteConfirm(null)} title="Anlaşmayı Sonlandır" size="sm">
        <div className="space-y-4">
          <div className="bg-red-50 rounded-lg p-4 text-sm text-red-700">
            <p className="font-medium mb-1">Bu anlaşmayı sonlandırmak istediğinize emin misiniz?</p>
            <p className="text-xs text-red-500">Bu işlem geri alınamaz.</p>
          </div>
          <div className="bg-gray-50 rounded-lg p-3 text-sm">
            <p className="text-gray-500 text-xs mb-1">Firma</p>
            <p className="font-medium">{deleteConfirm?.courierCompanyName || deleteConfirm?.courierName || '-'}</p>
          </div>
          <div className="flex justify-end gap-2">
            <Button variant="secondary" onClick={() => setDeleteConfirm(null)}>Vazgeç</Button>
            <Button variant="danger" loading={saving} onClick={() => handleDeleteAgreement(deleteConfirm.id)}>Sonlandır</Button>
          </div>
        </div>
      </Modal>

      {/* Add Own Courier Info Modal */}
      <Modal isOpen={courierInfoModal} onClose={() => setCourierInfoModal(false)} title="Kendi Kuryeni Ekle" size="md">
        <div className="space-y-4">
          <div className="bg-blue-50 rounded-lg p-4">
            <h4 className="text-sm font-semibold text-blue-800 mb-2">Kurye ekleme nasil calisir?</h4>
            <ul className="space-y-3 text-sm text-blue-700">
              <li className="flex items-start gap-2">
                <span className="bg-blue-200 text-blue-800 rounded-full w-5 h-5 flex items-center justify-center text-xs font-bold flex-shrink-0 mt-0.5">1</span>
                <span>Kuryeniz, <strong>courier uygulamasindan</strong> kayit olmalidir.</span>
              </li>
              <li className="flex items-start gap-2">
                <span className="bg-blue-200 text-blue-800 rounded-full w-5 h-5 flex items-center justify-center text-xs font-bold flex-shrink-0 mt-0.5">2</span>
                <span>Kayit sirasinda kurye tipi olarak <strong>&quot;Restoran Kuryesi&quot;</strong> secilmelidir.</span>
              </li>
              <li className="flex items-start gap-2">
                <span className="bg-blue-200 text-blue-800 rounded-full w-5 h-5 flex items-center justify-center text-xs font-bold flex-shrink-0 mt-0.5">3</span>
                <span>Kurye, kayit sirasinda restoraninizin ID&apos;sini girmelidir.</span>
              </li>
              <li className="flex items-start gap-2">
                <span className="bg-blue-200 text-blue-800 rounded-full w-5 h-5 flex items-center justify-center text-xs font-bold flex-shrink-0 mt-0.5">4</span>
                <span>Kayit tamamlandiktan sonra kurye bu listede otomatik olarak gorunecektir.</span>
              </li>
            </ul>
          </div>

          {!settingsForm.hasOwnCouriers && (
            <div className="bg-amber-50 border border-amber-200 rounded-lg p-4">
              <p className="text-sm text-amber-800 font-medium">Onemli: &quot;Kendi kuryelerimiz var&quot; ayari aktif degil</p>
              <p className="text-xs text-amber-600 mt-1">
                Kendi kuryelerinizi kullanabilmek icin once{' '}
                <button
                  onClick={() => { setCourierInfoModal(false); setSettingsModal(true); }}
                  className="underline font-medium hover:text-amber-800 transition-colors"
                >
                  Teslimat Ayarlari
                </button>
                {' '}sayfasindan bu secenegi aktif etmeniz gerekir.
              </p>
            </div>
          )}

          {selectedRestaurant && (
            <div className="bg-gray-50 rounded-lg p-3">
              <p className="text-xs text-gray-500 mb-1">Restoran ID (kuryenizle paylasin)</p>
              <p className="font-mono text-sm text-gray-800 select-all">{selectedRestaurant}</p>
            </div>
          )}

          <div className="flex justify-end pt-2">
            <Button variant="secondary" onClick={() => setCourierInfoModal(false)}>Kapat</Button>
          </div>
        </div>
      </Modal>

      {/* Delivery Settings Modal */}
      <Modal isOpen={settingsModal} onClose={() => setSettingsModal(false)} title="Teslimat Ayarları" size="md">
        <div className="space-y-4">
          <div className="w-full">
            <label className="block text-sm font-medium text-gray-700 mb-1.5">Varsayılan Atama Stratejisi</label>
            <select
              value={settingsForm.defaultAssignmentStrategy}
              onChange={e => setSettingsForm(prev => ({ ...prev, defaultAssignmentStrategy: Number(e.target.value) }))}
              className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-emerald-400 focus:border-emerald-400"
            >
              {Object.entries(ASSIGNMENT_STRATEGY).map(([k, v]) => (
                <option key={k} value={k}>{v}</option>
              ))}
            </select>
          </div>
          <div className="bg-gray-50 rounded-lg p-4">
            <div className="flex items-center gap-3">
              <input
                type="checkbox"
                id="hasOwnCouriers"
                checked={settingsForm.hasOwnCouriers}
                onChange={e => setSettingsForm(prev => ({ ...prev, hasOwnCouriers: e.target.checked }))}
                className="w-5 h-5 text-emerald-600 border-gray-300 rounded focus:ring-emerald-400"
              />
              <label htmlFor="hasOwnCouriers" className="text-sm font-semibold text-gray-800">
                Kendi kuryelerimiz var
              </label>
            </div>
            <p className="text-xs text-gray-500 mt-2 ml-8">
              Bu ayari aktif ederseniz, kendi kurye kadronuzu kullanabilirsiniz. Kuryeleriniz courier uygulamasindan &quot;Restoran Kuryesi&quot; olarak kayit olacak ve siparislere oncelikli olarak atanacaktir.
            </p>
          </div>
          <div className="flex justify-end gap-2 pt-2">
            <Button variant="secondary" onClick={() => setSettingsModal(false)}>Vazgeç</Button>
            <Button loading={saving} onClick={handleSaveSettings}>Kaydet</Button>
          </div>
        </div>
      </Modal>
    </SellerLayout>
  );
}
