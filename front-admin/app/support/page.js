'use client';

import { useState } from 'react';
import useSWR from 'swr';
import { useRouter } from 'next/navigation';
import AdminLayout from '@/components/layout/AdminLayout';
import Badge from '@/components/ui/Badge';
import Button from '@/components/ui/Button';
import StatCard from '@/components/ui/StatCard';
import { useToast } from '@/components/ui/Toast';
import fetcher from '@/lib/fetcher';

const STATUS_MAP = {
  1: { label: 'Acik', color: 'purple' },
  2: { label: 'Islemde', color: 'orange' },
  3: { label: 'Cozuldu', color: 'green' },
  4: { label: 'Kapatildi', color: 'gray' },
  5: { label: 'Yonlendirildi', color: 'red' },
};

const TOPIC_MAP = {
  1: 'Siparis Sorunu',
  2: 'Iptal Talebi',
  3: 'Teslimat Problemi',
  4: 'Genel Soru',
  5: 'Hesap Sorunu',
  6: 'Odeme Sorunu',
};

export default function SupportPage() {
  const router = useRouter();
  const [page, setPage] = useState(1);
  const [statusFilter, setStatusFilter] = useState('');
  const [topicFilter, setTopicFilter] = useState('');

  const statsUrl = '/v1/admin/support/stats';
  const ticketsUrl = `/v1/admin/support/tickets?page=${page}&pageSize=20${statusFilter ? `&statusId=${statusFilter}` : ''}${topicFilter ? `&topicId=${topicFilter}` : ''}`;

  const { data: statsData } = useSWR(statsUrl, fetcher);
  const { data: ticketsData, isLoading } = useSWR(ticketsUrl, fetcher);

  const stats = statsData?.data;
  const tickets = ticketsData?.data || [];
  const total = ticketsData?.totalDataCount || 0;
  const totalPages = Math.ceil(total / 20) || 1;

  return (
    <AdminLayout title="Destek Yonetimi">
      <div className="space-y-6">
        {/* Stats */}
        {stats && (
          <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-7 gap-4">
            <StatCard title="Toplam" value={stats.totalTickets} />
            <StatCard title="Acik" value={stats.openTickets} color="purple" />
            <StatCard title="Yonlendirilen" value={stats.escalatedTickets} color="red" />
            <StatCard title="Cozulen" value={stats.resolvedTickets} color="green" />
            <StatCard title="Kapatilan" value={stats.closedTickets} color="gray" />
            <StatCard title="Ort. Puan" value={stats.averageRating?.toFixed(1) || '—'} />
            <StatCard title="Ort. Cozum (sa)" value={stats.averageResolutionHours?.toFixed(1) || '—'} color="blue" />
          </div>
        )}

        {/* Filters */}
        <div className="flex items-center gap-3 flex-wrap">
          <select
            value={statusFilter}
            onChange={(e) => { setStatusFilter(e.target.value); setPage(1); }}
            className="border border-gray-300 rounded-lg px-3 py-2 text-sm bg-white"
          >
            <option value="">Tum Durumlar</option>
            {Object.entries(STATUS_MAP).map(([key, val]) => (
              <option key={key} value={key}>{val.label}</option>
            ))}
          </select>
          <select
            value={topicFilter}
            onChange={(e) => { setTopicFilter(e.target.value); setPage(1); }}
            className="border border-gray-300 rounded-lg px-3 py-2 text-sm bg-white"
          >
            <option value="">Tum Konular</option>
            {Object.entries(TOPIC_MAP).map(([key, val]) => (
              <option key={key} value={key}>{val}</option>
            ))}
          </select>
          <p className="text-sm text-gray-500 ml-auto">{total} ticket</p>
        </div>

        {/* Tickets List */}
        {isLoading ? (
          <div className="flex justify-center py-12">
            <div className="w-8 h-8 border-2 border-orange-400 border-t-transparent rounded-full animate-spin" />
          </div>
        ) : tickets.length === 0 ? (
          <div className="bg-white rounded-xl border border-gray-200 p-10 text-center text-gray-400 text-sm">
            Ticket bulunamadi
          </div>
        ) : (
          <div className="space-y-3">
            {tickets.map(ticket => {
              const status = STATUS_MAP[ticket.statusId] || { label: '?', color: 'gray' };
              return (
                <div
                  key={ticket.id}
                  onClick={() => router.push(`/support/${ticket.id}`)}
                  className={`bg-white rounded-xl border p-5 cursor-pointer hover:shadow-md transition-shadow ${
                    ticket.isEscalated ? 'border-red-300 bg-red-50/30' : 'border-gray-200'
                  }`}
                >
                  <div className="flex items-start justify-between gap-4">
                    <div className="flex-1">
                      <div className="flex items-center gap-2 mb-1">
                        <h3 className="text-sm font-semibold text-gray-800">{ticket.subject}</h3>
                        <Badge label={status.label} color={status.color} />
                        {ticket.isEscalated && <Badge label="Escalated" color="red" />}
                      </div>
                      <div className="flex items-center gap-3 text-xs text-gray-500">
                        <span>{TOPIC_MAP[ticket.topicId] || 'Bilinmeyen'}</span>
                        <span>•</span>
                        <span>{ticket.messageCount || 0} mesaj</span>
                        <span>•</span>
                        <span>{ticket.createdDate ? new Date(ticket.createdDate).toLocaleString('tr-TR') : ''}</span>
                      </div>
                    </div>
                    {ticket.rating && (
                      <div className="flex items-center gap-1">
                        <svg className="w-4 h-4 text-yellow-400" fill="currentColor" viewBox="0 0 20 20">
                          <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
                        </svg>
                        <span className="text-sm font-medium text-gray-700">{ticket.rating}</span>
                      </div>
                    )}
                  </div>
                </div>
              );
            })}
          </div>
        )}

        {/* Pagination */}
        {totalPages > 1 && (
          <div className="flex justify-center gap-2 pt-2">
            <Button variant="outline" size="sm" onClick={() => setPage(p => Math.max(1, p - 1))} disabled={page === 1}>
              Onceki
            </Button>
            <span className="text-sm text-gray-500 self-center">{page} / {totalPages}</span>
            <Button variant="outline" size="sm" onClick={() => setPage(p => Math.min(totalPages, p + 1))} disabled={page === totalPages}>
              Sonraki
            </Button>
          </div>
        )}
      </div>
    </AdminLayout>
  );
}
