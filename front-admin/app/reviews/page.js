'use client';

import { useState } from 'react';
import useSWR from 'swr';
import AdminLayout from '@/components/layout/AdminLayout';
import Badge from '@/components/ui/Badge';
import Button from '@/components/ui/Button';
import { useToast } from '@/components/ui/Toast';
import fetcher from '@/lib/fetcher';
import { deleteReview } from '@/lib/api';

function StarRating({ rating }) {
  return (
    <div className="flex gap-0.5">
      {[1, 2, 3, 4, 5].map(i => (
        <svg
          key={i}
          className={`w-4 h-4 ${i <= rating ? 'text-yellow-400' : 'text-gray-200'}`}
          fill="currentColor"
          viewBox="0 0 20 20"
        >
          <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
        </svg>
      ))}
    </div>
  );
}

export default function ReviewsPage() {
  const toast = useToast();
  const [page, setPage] = useState(1);
  const [deleting, setDeleting] = useState(null);

  const { data, isLoading, mutate } = useSWR(`/v1/admin/reviews?page=${page}&pageSize=20`, fetcher);

  const reviews = data?.data || [];
  const total = data?.totalDataCount || 0;
  const totalPages = Math.ceil(total / 20) || 1;

  const handleDelete = async (reviewId) => {
    if (!confirm('Bu degerlendirmeyi silmek istediginize emin misiniz?')) return;
    setDeleting(reviewId);
    try {
      await deleteReview(reviewId);
      toast('Degerlendirme silindi', 'success');
      mutate();
    } catch (err) {
      toast(err.response?.data?.messages?.[0]?.description || 'Hata olustu', 'error');
    } finally {
      setDeleting(null);
    }
  };

  return (
    <AdminLayout title="Degerlendirmeler">
      <div className="space-y-4">
        <div className="flex items-center justify-between">
          <p className="text-sm text-gray-500">{total} degerlendirme</p>
        </div>

        {isLoading ? (
          <div className="flex justify-center py-12">
            <div className="w-8 h-8 border-2 border-orange-400 border-t-transparent rounded-full animate-spin" />
          </div>
        ) : reviews.length === 0 ? (
          <div className="bg-white rounded-xl border border-gray-200 p-10 text-center text-gray-400 text-sm">
            Henuz degerlendirme yok
          </div>
        ) : (
          <div className="space-y-3">
            {reviews.map(review => (
              <div key={review.id} className="bg-white rounded-xl border border-gray-200 p-5">
                <div className="flex items-start justify-between gap-4">
                  <div className="flex-1">
                    <div className="flex items-center gap-3 mb-2">
                      <StarRating rating={review.rating} />
                      <span className="text-xs text-gray-400">
                        {review.createdDate ? new Date(review.createdDate).toLocaleString('tr-TR') : ''}
                      </span>
                    </div>
                    <div className="flex items-center gap-2 mb-2">
                      <span className="text-sm font-medium text-gray-800">{review.userName || 'Anonim'}</span>
                      <span className="text-xs text-gray-400">-</span>
                      <span className="text-sm text-gray-600">{review.restaurantName || '—'}</span>
                    </div>
                    {review.comment && (
                      <p className="text-sm text-gray-600 bg-gray-50 rounded-lg p-3">{review.comment}</p>
                    )}
                    <p className="text-xs text-gray-400 mt-2">
                      Siparis: <span className="font-mono">{review.orderId ? String(review.orderId).slice(0, 8) + '...' : '—'}</span>
                    </p>
                  </div>
                  <Button
                    size="sm"
                    variant="danger"
                    loading={deleting === review.id}
                    onClick={() => handleDelete(review.id)}
                  >
                    Sil
                  </Button>
                </div>
              </div>
            ))}
          </div>
        )}

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
