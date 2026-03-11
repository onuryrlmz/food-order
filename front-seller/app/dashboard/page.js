'use client';

import useSWR from 'swr';
import SellerLayout from '@/components/layout/SellerLayout';
import Badge from '@/components/ui/Badge';
import fetcher from '@/lib/fetcher';
import Link from 'next/link';

const ORDER_STATUS = {
  1: { label: 'Beklemede', color: 'yellow' },
  2: { label: 'Onaylandı', color: 'blue' },
  3: { label: 'Hazırlanıyor', color: 'purple' },
  4: { label: 'Yolda', color: 'orange' },
  5: { label: 'Teslim Edildi', color: 'green' },
  6: { label: 'İptal', color: 'red' },
  7: { label: 'Reddedildi', color: 'red' },
};

export default function DashboardPage() {
  const { data: restaurantsData } = useSWR('/v1/seller/restaurant/list', fetcher);
  const { data: subscriptionsData } = useSWR('/v1/seller/subscription/my', fetcher);

  const restaurants = restaurantsData?.data || [];
  const subscriptions = subscriptionsData?.data || [];

  const activeRestaurants = restaurants.filter(r => r.isActive).length;
  const openRestaurants = restaurants.filter(r => r.isOpen).length;
  const activeSubscriptions = subscriptions.filter(s => s.statusId === 1).length;
  const expiringSoon = subscriptions.filter(s => s.statusId === 1 && s.daysRemaining <= 7).length;

  return (
    <SellerLayout title="Dashboard">
      <div className="space-y-6">
        {/* Stats */}
        <div className="grid grid-cols-2 lg:grid-cols-4 gap-4">
          {[
            { label: 'Toplam Restoran', value: restaurants.length, sub: `${activeRestaurants} aktif`, color: 'bg-blue-500', href: '/restaurants' },
            { label: 'Şu An Açık', value: openRestaurants, sub: 'restoran', color: 'bg-green-500', href: '/restaurants' },
            { label: 'Aktif Abonelik', value: activeSubscriptions, sub: `${expiringSoon} süresi dolmak üzere`, color: 'bg-emerald-500', href: '/subscription' },
            { label: 'Toplam Abonelik', value: subscriptions.length, sub: 'tüm zamanlar', color: 'bg-purple-500', href: '/subscription' },
          ].map((s) => (
            <Link key={s.label} href={s.href} className="bg-white rounded-xl p-5 shadow-sm border border-gray-100 hover:shadow-md transition-shadow">
              <div className={`w-10 h-10 ${s.color} rounded-lg mb-3`} />
              <p className="text-2xl font-bold text-gray-800">{s.value}</p>
              <p className="text-sm font-medium text-gray-600 mt-0.5">{s.label}</p>
              <p className="text-xs text-gray-400 mt-1">{s.sub}</p>
            </Link>
          ))}
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          {/* Restoranlar */}
          <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-5">
            <div className="flex items-center justify-between mb-4">
              <h2 className="font-semibold text-gray-800">Restoranlarım</h2>
              <Link href="/restaurants" className="text-xs text-emerald-600 hover:underline">Tümünü gör →</Link>
            </div>
            {restaurants.length === 0 ? (
              <p className="text-sm text-gray-400 text-center py-6">Henüz restoran yok</p>
            ) : (
              <div className="space-y-3">
                {restaurants.slice(0, 5).map(r => (
                  <Link key={r.id} href={`/restaurants/${r.id}`} className="flex items-center justify-between p-3 rounded-lg hover:bg-gray-50 transition-colors">
                    <div className="flex items-center gap-3">
                      <div className="w-8 h-8 bg-emerald-100 rounded-lg flex items-center justify-center text-emerald-600 font-bold text-xs">
                        {r.name?.[0]?.toUpperCase()}
                      </div>
                      <span className="text-sm font-medium text-gray-700">{r.name}</span>
                    </div>
                    <div className="flex gap-1.5">
                      <Badge label={r.isActive ? 'Aktif' : 'Pasif'} color={r.isActive ? 'green' : 'red'} />
                      <Badge label={r.isOpen ? 'Açık' : 'Kapalı'} color={r.isOpen ? 'blue' : 'gray'} />
                    </div>
                  </Link>
                ))}
              </div>
            )}
          </div>

          {/* Abonelikler */}
          <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-5">
            <div className="flex items-center justify-between mb-4">
              <h2 className="font-semibold text-gray-800">Abonelikler</h2>
              <Link href="/subscription" className="text-xs text-emerald-600 hover:underline">Tümünü gör →</Link>
            </div>
            {subscriptions.length === 0 ? (
              <div className="text-center py-6">
                <p className="text-sm text-gray-400 mb-3">Aktif abonelik yok</p>
                <Link href="/subscription" className="text-sm text-emerald-600 font-medium hover:underline">
                  Plan satın al →
                </Link>
              </div>
            ) : (
              <div className="space-y-3">
                {subscriptions.slice(0, 5).map(s => (
                  <div key={s.id} className="flex items-center justify-between p-3 rounded-lg bg-gray-50">
                    <div>
                      <p className="text-sm font-medium text-gray-700">{s.restaurantName}</p>
                      <p className="text-xs text-gray-400">{s.planName} · {s.daysRemaining} gün kaldı</p>
                    </div>
                    <Badge
                      label={s.statusId === 1 ? 'Aktif' : s.statusId === 2 ? 'Süresi Doldu' : 'İptal'}
                      color={s.statusId === 1 ? (s.daysRemaining <= 7 ? 'yellow' : 'green') : 'red'}
                    />
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      </div>
    </SellerLayout>
  );
}
