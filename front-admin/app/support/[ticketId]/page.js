'use client';

import { useState } from 'react';
import { useParams } from 'next/navigation';
import useSWR from 'swr';
import AdminLayout from '@/components/layout/AdminLayout';
import Badge from '@/components/ui/Badge';
import Button from '@/components/ui/Button';
import { useToast } from '@/components/ui/Toast';
import fetcher from '@/lib/fetcher';
import { approveAction, rejectAction } from '@/lib/api';

const STATUS_MAP = {
  1: { label: 'Acik', color: 'purple' },
  2: { label: 'Islemde', color: 'orange' },
  3: { label: 'Cozuldu', color: 'green' },
  4: { label: 'Kapatildi', color: 'gray' },
  5: { label: 'Yonlendirildi', color: 'red' },
};

const TOPIC_MAP = {
  1: 'Siparis Sorunu', 2: 'Iptal Talebi', 3: 'Teslimat Problemi',
  4: 'Genel Soru', 5: 'Hesap Sorunu', 6: 'Odeme Sorunu',
};

const ACTION_TYPE_MAP = {
  1: { label: 'Siparis Iptal', color: 'red' },
  2: { label: 'Yonlendirme', color: 'purple' },
};

const SENDER_MAP = { 1: 'Musteri', 2: 'AI Asistan', 3: 'Sistem' };

export default function TicketDetailPage() {
  const { ticketId } = useParams();
  const toast = useToast();
  const [processing, setProcessing] = useState(null);

  const { data, isLoading, mutate } = useSWR(`/v1/admin/support/ticket/${ticketId}`, fetcher);
  const ticket = data?.data;

  const handleApprove = async (actionId) => {
    if (!confirm('Bu aksiyonu onaylamak istediginize emin misiniz?')) return;
    setProcessing(actionId);
    try {
      await approveAction(actionId);
      toast('Aksiyon onaylandi', 'success');
      mutate();
    } catch (err) {
      toast(err.message || 'Hata olustu', 'error');
    } finally {
      setProcessing(null);
    }
  };

  const handleReject = async (actionId) => {
    if (!confirm('Bu aksiyonu reddetmek istediginize emin misiniz?')) return;
    setProcessing(actionId);
    try {
      await rejectAction(actionId);
      toast('Aksiyon reddedildi', 'success');
      mutate();
    } catch (err) {
      toast(err.message || 'Hata olustu', 'error');
    } finally {
      setProcessing(null);
    }
  };

  if (isLoading) {
    return (
      <AdminLayout title="Ticket Detay">
        <div className="flex justify-center py-12">
          <div className="w-8 h-8 border-2 border-orange-400 border-t-transparent rounded-full animate-spin" />
        </div>
      </AdminLayout>
    );
  }

  if (!ticket) {
    return (
      <AdminLayout title="Ticket Detay">
        <div className="bg-white rounded-xl border border-gray-200 p-10 text-center text-gray-400">
          Ticket bulunamadi
        </div>
      </AdminLayout>
    );
  }

  const status = STATUS_MAP[ticket.statusId] || { label: '?', color: 'gray' };

  return (
    <AdminLayout title={`Ticket: ${ticket.subject}`}>
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Left — Messages */}
        <div className="lg:col-span-2 space-y-4">
          {/* Ticket Info */}
          <div className="bg-white rounded-xl border border-gray-200 p-5">
            <div className="flex items-center gap-3 mb-3">
              <h2 className="text-lg font-bold text-gray-800">{ticket.subject}</h2>
              <Badge label={status.label} color={status.color} />
              {ticket.isEscalated && <Badge label="Escalated" color="red" />}
            </div>
            <div className="flex items-center gap-4 text-sm text-gray-500">
              <span>Konu: {TOPIC_MAP[ticket.topicId] || '—'}</span>
              <span>•</span>
              <span>Olusturulma: {ticket.createdDate ? new Date(ticket.createdDate).toLocaleString('tr-TR') : '—'}</span>
              {ticket.resolvedAt && (
                <>
                  <span>•</span>
                  <span>Cozum: {new Date(ticket.resolvedAt).toLocaleString('tr-TR')}</span>
                </>
              )}
            </div>
          </div>

          {/* Messages */}
          <div className="bg-white rounded-xl border border-gray-200 p-5">
            <h3 className="text-sm font-semibold text-gray-700 mb-4">Mesaj Gecmisi</h3>
            <div className="space-y-4">
              {(ticket.messages || []).map(msg => {
                const isCustomer = msg.senderType === 1;
                const isAI = msg.senderType === 2;
                return (
                  <div key={msg.id} className="flex flex-col">
                    <div className="flex items-center gap-2 mb-1">
                      <span className={`text-xs font-semibold ${isCustomer ? 'text-blue-600' : isAI ? 'text-purple-600' : 'text-gray-500'}`}>
                        {SENDER_MAP[msg.senderType] || 'Bilinmeyen'}
                      </span>
                      <span className="text-xs text-gray-400">
                        {msg.createdDate ? new Date(msg.createdDate).toLocaleString('tr-TR') : ''}
                      </span>
                    </div>
                    <div className={`rounded-lg p-3 text-sm ${
                      isCustomer ? 'bg-blue-50 text-gray-800' : isAI ? 'bg-purple-50 text-gray-800' : 'bg-gray-50 text-gray-600'
                    }`}>
                      {msg.content}
                    </div>
                  </div>
                );
              })}
            </div>
          </div>
        </div>

        {/* Right — Info & Actions */}
        <div className="space-y-4">
          {/* Ticket Meta */}
          <div className="bg-white rounded-xl border border-gray-200 p-5">
            <h3 className="text-sm font-semibold text-gray-700 mb-3">Bilgiler</h3>
            <dl className="space-y-2 text-sm">
              {ticket.orderId && (
                <div className="flex justify-between">
                  <dt className="text-gray-500">Siparis</dt>
                  <dd className="font-mono text-gray-800">{String(ticket.orderId).slice(0, 8)}...</dd>
                </div>
              )}
              {ticket.restaurantId && (
                <div className="flex justify-between">
                  <dt className="text-gray-500">Restoran</dt>
                  <dd className="font-mono text-gray-800">{String(ticket.restaurantId).slice(0, 8)}...</dd>
                </div>
              )}
              {ticket.rating && (
                <div className="flex justify-between">
                  <dt className="text-gray-500">Puan</dt>
                  <dd className="flex items-center gap-1">
                    <svg className="w-4 h-4 text-yellow-400" fill="currentColor" viewBox="0 0 20 20">
                      <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
                    </svg>
                    <span className="font-medium">{ticket.rating}/5</span>
                  </dd>
                </div>
              )}
              {ticket.ratingComment && (
                <div>
                  <dt className="text-gray-500 mb-1">Yorum</dt>
                  <dd className="text-gray-700 bg-gray-50 rounded-lg p-2 text-xs">{ticket.ratingComment}</dd>
                </div>
              )}
            </dl>
          </div>

          {/* Actions */}
          {ticket.actions && ticket.actions.length > 0 && (
            <div className="bg-white rounded-xl border border-gray-200 p-5">
              <h3 className="text-sm font-semibold text-gray-700 mb-3">AI Aksiyonlari</h3>
              <div className="space-y-3">
                {ticket.actions.map(action => {
                  const actionType = ACTION_TYPE_MAP[action.actionType] || { label: 'Bilinmeyen', color: 'gray' };
                  return (
                    <div key={action.id} className="border border-gray-100 rounded-lg p-3">
                      <div className="flex items-center gap-2 mb-2">
                        <Badge label={actionType.label} color={actionType.color} />
                        {action.isApproved && <Badge label="Onaylandi" color="green" />}
                        {action.isExecuted && <Badge label="Yurutuldu" color="blue" />}
                        {!action.isApproved && !action.isExecuted && <Badge label="Beklemede" color="yellow" />}
                      </div>
                      <p className="text-xs text-gray-500 font-mono mb-2 break-all">{action.actionData}</p>
                      {!action.isApproved && !action.isExecuted && (
                        <div className="flex gap-2">
                          <Button
                            size="sm"
                            variant="primary"
                            loading={processing === action.id}
                            onClick={() => handleApprove(action.id)}
                          >
                            Onayla
                          </Button>
                          <Button
                            size="sm"
                            variant="danger"
                            loading={processing === action.id}
                            onClick={() => handleReject(action.id)}
                          >
                            Reddet
                          </Button>
                        </div>
                      )}
                    </div>
                  );
                })}
              </div>
            </div>
          )}
        </div>
      </div>
    </AdminLayout>
  );
}
