'use client';

import Link from 'next/link';
import { usePathname } from 'next/navigation';
import useSWR from 'swr';
import { logout } from '@/lib/auth';
import fetcher from '@/lib/fetcher';

const navItems = [
  {
    label: 'Dashboard',
    href: '/dashboard',
    icon: <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />,
  },
  {
    label: 'Siparişler',
    href: '/orders',
    icon: <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />,
  },
  {
    label: 'Abonelik',
    href: '/subscription',
    icon: <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z" />,
  },
  {
    label: 'Kuponlar',
    href: '/coupons',
    icon: <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M7 7h.01M7 3h5c.512 0 1.024.195 1.414.586l7 7a2 2 0 010 2.828l-7 7a2 2 0 01-2.828 0l-7-7A1.994 1.994 0 013 12V7a4 4 0 014-4z" />,
  },
  {
    label: 'Analitik',
    href: '/analytics',
    icon: <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />,
  },
  {
    label: 'Finans',
    href: '/finance',
    icon: <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />,
  },
];

const restaurantSubItems = [
  { label: 'Bilgiler', suffix: '' },
  { label: 'Menü', suffix: '/menu' },
  { label: 'Kuryeler', suffix: '/couriers' },
  { label: 'Kurye Firmaları', suffix: '/courier-companies' },
];

export default function Sidebar() {
  const pathname = usePathname();
  const { data: restaurantsData } = useSWR('/v1/seller/restaurant/list', fetcher);
  const restaurants = restaurantsData?.data || [];

  const isRestaurantsOpen = pathname.startsWith('/restaurants');

  return (
    <aside className="fixed top-0 left-0 h-full w-64 bg-gray-900 text-white flex flex-col z-40">
      <div className="px-6 py-5 border-b border-gray-700">
        <div className="flex items-center gap-3">
          <div className="w-8 h-8 bg-emerald-500 rounded-lg flex items-center justify-center">
            <svg className="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 10V3L4 14h7v7l9-11h-7z" />
            </svg>
          </div>
          <div>
            <p className="font-bold text-sm">FoodOrder</p>
            <p className="text-xs text-gray-400">Satıcı Paneli</p>
          </div>
        </div>
      </div>

      <nav className="flex-1 px-3 py-4 overflow-y-auto">
        <ul className="space-y-1">
          {navItems.map((item) => {
            const isActive = pathname === item.href || pathname.startsWith(item.href + '/');
            return (
              <li key={item.href}>
                <Link
                  href={item.href}
                  className={`flex items-center gap-3 px-3 py-2.5 rounded-lg text-sm font-medium transition-colors ${
                    isActive ? 'bg-emerald-600 text-white' : 'text-gray-300 hover:bg-gray-800 hover:text-white'
                  }`}
                >
                  <svg className="w-5 h-5 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    {item.icon}
                  </svg>
                  {item.label}
                </Link>
              </li>
            );
          })}

          {/* Restoranlarım — with dynamic sub-menu */}
          <li>
            <Link
              href="/restaurants"
              className={`flex items-center gap-3 px-3 py-2.5 rounded-lg text-sm font-medium transition-colors ${
                isRestaurantsOpen ? 'bg-emerald-600 text-white' : 'text-gray-300 hover:bg-gray-800 hover:text-white'
              }`}
            >
              <svg className="w-5 h-5 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-2 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
              </svg>
              Restoranlarım
              <svg className={`w-4 h-4 ml-auto transition-transform ${isRestaurantsOpen ? 'rotate-90' : ''}`} fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
              </svg>
            </Link>

            {isRestaurantsOpen && restaurants.length > 0 && (
              <ul className="mt-1 ml-4 pl-4 border-l border-gray-700 space-y-0.5">
                {restaurants.map((r) => {
                  const basePath = `/restaurants/${r.id}`;
                  const isThisRestaurant = pathname.startsWith(basePath);
                  return (
                    <li key={r.id}>
                      <Link
                        href={basePath}
                        className={`block px-3 py-1.5 rounded-md text-xs font-semibold truncate transition-colors ${
                          isThisRestaurant ? 'text-emerald-400' : 'text-gray-400 hover:text-white'
                        }`}
                      >
                        {r.name}
                      </Link>
                      {isThisRestaurant && (
                        <ul className="ml-2 space-y-0.5">
                          {restaurantSubItems.map((sub) => {
                            const subHref = basePath + sub.suffix;
                            const isSubActive = sub.suffix === ''
                              ? pathname === basePath
                              : pathname.startsWith(subHref);
                            return (
                              <li key={sub.suffix}>
                                <Link
                                  href={subHref}
                                  className={`block px-3 py-1 rounded-md text-xs transition-colors ${
                                    isSubActive
                                      ? 'text-white bg-gray-800'
                                      : 'text-gray-500 hover:text-gray-300'
                                  }`}
                                >
                                  {sub.label}
                                </Link>
                              </li>
                            );
                          })}
                        </ul>
                      )}
                    </li>
                  );
                })}
              </ul>
            )}
          </li>
        </ul>
      </nav>

      <div className="px-3 py-4 border-t border-gray-700">
        <button
          onClick={logout}
          className="w-full flex items-center gap-3 px-3 py-2.5 rounded-lg text-sm font-medium text-gray-300 hover:bg-red-500/20 hover:text-red-400 transition-colors"
        >
          <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
          </svg>
          Çıkış Yap
        </button>
      </div>
    </aside>
  );
}
