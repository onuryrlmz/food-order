'use client';

import { use, useState } from 'react';
import useSWR from 'swr';
import SellerLayout from '@/components/layout/SellerLayout';
import Button from '@/components/ui/Button';
import Input from '@/components/ui/Input';
import Modal from '@/components/ui/Modal';
import Badge from '@/components/ui/Badge';
import { useToast } from '@/components/ui/Toast';
import fetcher from '@/lib/fetcher';
import { inviteCourierCompany, removeCourierCompany, searchCourierCompanies } from '@/lib/api';

const STATUS_MAP = {
  0: { label: 'Onay Bekliyor', color: 'yellow' },
  1: { label: 'Aktif', color: 'green' },
  2: { label: 'Askıya Alındı', color: 'orange' },
  3: { label: 'Sonlandırıldı', color: 'red' },
};

export default function CourierCompaniesPage({ params }) {
  const { id } = use(params);
  const toast = useToast();

  const [addModal, setAddModal] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');
  const [searchResults, setSearchResults] = useState([]);
  const [searching, setSearching] = useState(false);
  const [adding, setAdding] = useState(null);
  const [removing, setRemoving] = useState(null);

  const { data: companiesData, mutate: mutateCompanies } = useSWR(
    `/v1/seller/restaurant/${id}/courier-companies`,
    fetcher
  );

  const companies = companiesData?.data || [];

  const handleSearch = async (e) => {
    e.preventDefault();
    if (!searchQuery.trim()) return;
    setSearching(true);
    try {
      const res = await searchCourierCompanies(searchQuery);
      setSearchResults(!res.hasFailed ? (res.data || []) : []);
    } catch {
      setSearchResults([]);
    } finally {
      setSearching(false);
    }
  };

  const handleAdd = async (companyId) => {
    setAdding(companyId);
    try {
      const result = await inviteCourierCompany(id, companyId);
      if (!result.hasFailed) {
        toast.success('Kurye firması eklendi');
        setAddModal(false);
        setSearchQuery('');
        setSearchResults([]);
        mutateCompanies();
      } else {
        toast.error(result.messages?.map(m => m.description).join(', ') || 'Hata oluştu');
      }
    } catch (err) {
      toast.error('İstek başarısız');
    } finally {
      setAdding(null);
    }
  };

  const handleRemove = async (companyId) => {
    if (!confirm('Bu kurye firmasını çıkarmak istediğinizden emin misiniz?')) return;
    setRemoving(companyId);
    try {
      const result = await removeCourierCompany(id, companyId);
      if (!result.hasFailed) {
        toast.success('Kurye firması çıkarıldı');
        mutateCompanies();
      } else {
        toast.error(result.messages?.map(m => m.description).join(', ') || 'Hata oluştu');
      }
    } catch (err) {
      toast.error('İstek başarısız');
    } finally {
      setRemoving(null);
    }
  };

  return (
    <SellerLayout>
      <div className="max-w-5xl mx-auto p-6">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-2xl font-bold">Kurye Firmaları</h1>
          <Button onClick={() => setAddModal(true)}>
            + Firma Ekle
          </Button>
        </div>

        {companies.length === 0 ? (
          <div className="text-center py-12 text-gray-500">
            <svg className="w-12 h-12 mx-auto mb-3 text-gray-300" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-2 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
            </svg>
            <p className="font-medium">Henüz kurye firması eklemediniz.</p>
            <p className="text-sm mt-1">Kurumsal kurye firmalarını arayarak ekleyebilirsiniz.</p>
          </div>
        ) : (
          <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
            <table className="min-w-full divide-y divide-gray-100">
              <thead className="bg-gray-50/80">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">Firma Adı</th>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">Durum</th>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">Anlaşma Başlangıcı</th>
                  <th className="px-6 py-3 text-right text-xs font-semibold text-gray-500 uppercase tracking-wider">İşlem</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-50">
                {companies.map((company) => (
                  <tr key={company.id} className="hover:bg-gray-50/50 transition-colors">
                    <td className="px-6 py-4">
                      <p className="font-medium text-gray-800">{company.companyName}</p>
                    </td>
                    <td className="px-6 py-4">
                      <Badge
                        label={STATUS_MAP[company.statusId]?.label || company.statusName}
                        color={STATUS_MAP[company.statusId]?.color || 'gray'}
                      />
                    </td>
                    <td className="px-6 py-4 text-sm text-gray-400">
                      {company.agreementStartDate
                        ? new Date(company.agreementStartDate).toLocaleDateString('tr-TR')
                        : '—'}
                    </td>
                    <td className="px-6 py-4 text-right">
                      <button
                        onClick={() => handleRemove(company.courierCompanyId)}
                        disabled={removing === company.courierCompanyId}
                        className="text-red-600 hover:text-red-800 text-sm font-medium disabled:opacity-50"
                      >
                        {removing === company.courierCompanyId ? 'Çıkarılıyor...' : 'Çıkar'}
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {/* Add Company Modal */}
      <Modal isOpen={addModal} onClose={() => { setAddModal(false); setSearchResults([]); setSearchQuery(''); }} title="Kurye Firması Ekle">
        <div className="space-y-4">
          <form onSubmit={handleSearch} className="flex gap-2">
            <Input
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              placeholder="Firma adı ile arayın..."
              className="flex-1"
            />
            <Button type="submit" disabled={searching}>
              {searching ? 'Aranıyor...' : 'Ara'}
            </Button>
          </form>

          {searchResults.length > 0 && (
            <div className="space-y-2 max-h-[300px] overflow-y-auto">
              {searchResults.map((company) => (
                <div key={company.id} className="flex items-center justify-between p-3 rounded-lg bg-gray-50 border border-gray-100">
                  <div>
                    <p className="text-sm font-medium text-gray-800">{company.name}</p>
                    <p className="text-xs text-gray-400">{company.contactEmail}</p>
                  </div>
                  <Button
                    size="sm"
                    onClick={() => handleAdd(company.id)}
                    disabled={adding === company.id}
                  >
                    {adding === company.id ? 'Ekleniyor...' : 'Ekle'}
                  </Button>
                </div>
              ))}
            </div>
          )}

          {searchResults.length === 0 && searchQuery && !searching && (
            <p className="text-sm text-gray-400 text-center py-4">Firma bulunamadı</p>
          )}
        </div>
      </Modal>
    </SellerLayout>
  );
}
