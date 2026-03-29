'use client';

import Link from 'next/link';
import { usePathname } from 'next/navigation';
import useSWR from 'swr';
import { logout } from '@/lib/auth';
import fetcher from '@/lib/fetcher';

const navGroups = [
  {
    items: [
      { label: 'Dashboard', href: '/dashboard', icon: 'M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6' },
    ],
  },
  {
    label: 'RESTORAN',
    items: [
      { label: 'Restoranlarım', href: '/restaurants', icon: 'M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-2 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4' },
    ],
  },
  {
    label: 'SİPARİŞLER',
    items: [
      { label: 'Siparişler', href: '/orders', icon: 'M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2' },
      { label: 'Kuryeler', href: '/couriers', icon: 'M13 16V6a1 1 0 00-1-1H4a1 1 0 00-1 1v10a1 1 0 001 1h1m8-1a1 1 0 01-1 1H9m4-1V8a1 1 0 011-1h2.586a1 1 0 01.707.293l3.414 3.414a1 1 0 01.293.707V16a1 1 0 01-1 1h-1m-6-1a1 1 0 001 1h1M5 17a2 2 0 104 0m-4 0a2 2 0 114 0m6 0a2 2 0 104 0m-4 0a2 2 0 114 0' },
    ],
  },
  {
    label: 'FİNANS',
    items: [
      { label: 'Komisyon', href: '/commission', icon: 'M9 7h6m0 10v-3m-3 3h.01M9 17h.01M9 14h.01M12 14h.01M15 11h.01M12 11h.01M9 11h.01M7 21h10a2 2 0 002-2V5a2 2 0 00-2-2H7a2 2 0 00-2 2v14a2 2 0 002 2z' },
      { label: 'Finans', href: '/finance', icon: 'M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z' },
    ],
  },
  {
    label: 'İÇERİK',
    items: [
      { label: 'Kuponlar', href: '/coupons', icon: 'M7 7h.01M7 3h5c.512 0 1.024.195 1.414.586l7 7a2 2 0 010 2.828l-7 7a2 2 0 01-2.828 0l-7-7A1.994 1.994 0 013 12V7a4 4 0 014-4z' },
    ],
  },
  {
    items: [
      { label: 'Analitik', href: '/analytics', icon: 'M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z' },
    ],
  },
];

const restaurantSubItems = [
  { label: 'Bilgiler', suffix: '' },
  { label: 'Menü', suffix: '/menu' },
];

export default function Sidebar() {
  const pathname = usePathname();
  const { data: restaurantsData } = useSWR('/v1/seller/restaurant/list', fetcher);
  const restaurants = restaurantsData?.data || [];

  const isRestaurantsOpen = pathname.startsWith('/restaurants');

  return (
    <aside className="fixed top-0 left-0 h-full w-64 bg-gradient-to-b from-slate-900 to-slate-950 text-white flex flex-col z-40">
      {/* Logo */}
      <div className="px-5 py-5 border-b border-white/5">
        <div className="flex items-center gap-3">
          <div className="w-9 h-9 bg-gradient-to-br from-indigo-400 to-indigo-600 rounded-xl flex items-center justify-center shadow-lg shadow-indigo-500/20">
            <svg className="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 10V3L4 14h7v7l9-11h-7z" />
            </svg>
          </div>
          <div>
            <p className="font-bold text-sm tracking-wide">FoodOrder</p>
            <p className="text-[10px] text-slate-400 font-medium">Satıcı Paneli</p>
          </div>
        </div>
      </div>

      {/* Nav */}
      <nav className="flex-1 px-3 py-4 overflow-y-auto scrollbar-thin">
        {navGroups.map((group, gi) => (
          <div key={gi} className={gi > 0 ? 'mt-5' : ''}>
            {group.label && (
              <p className="px-3 mb-2 text-[10px] font-bold tracking-widest text-slate-500 uppercase">
                {group.label}
              </p>
            )}
            <ul className="space-y-0.5">
              {group.items.map((item) => {
                const isActive = item.href === '/restaurants'
                  ? isRestaurantsOpen
                  : pathname === item.href || pathname.startsWith(item.href + '/');
                return (
                  <li key={item.href}>
                    <Link
                      href={item.href}
                      className={`group flex items-center gap-3 px-3 py-2 rounded-lg text-[13px] font-medium transition-all duration-150 ${
                        isActive
                          ? 'bg-indigo-500/15 text-indigo-400 border-l-2 border-indigo-400 ml-0'
                          : 'text-slate-400 hover:bg-white/5 hover:text-slate-200'
                      }`}
                    >
                      <svg className={`w-[18px] h-[18px] flex-shrink-0 transition-colors ${isActive ? 'text-indigo-400' : 'text-slate-500 group-hover:text-slate-300'}`} fill="none" stroke="currentColor" strokeWidth={1.5} viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" d={item.icon} />
                      </svg>
                      {item.label}
                      {item.href === '/restaurants' && (
                        <svg className={`w-4 h-4 ml-auto transition-transform ${isRestaurantsOpen ? 'rotate-90' : ''}`} fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
                        </svg>
                      )}
                    </Link>

                    {/* Restaurant sub-menu */}
                    {item.href === '/restaurants' && isRestaurantsOpen && restaurants.length > 0 && (
                      <ul className="mt-1 ml-5 pl-3 border-l border-white/5 space-y-0.5">
                        {restaurants.map((r) => {
                          const basePath = `/restaurants/${r.id}`;
                          const isThisRestaurant = pathname.startsWith(basePath);
                          return (
                            <li key={r.id}>
                              <Link
                                href={basePath}
                                className={`block px-3 py-1.5 rounded-md text-xs font-semibold truncate transition-colors ${
                                  isThisRestaurant ? 'text-indigo-400' : 'text-slate-500 hover:text-slate-300'
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
                                              ? 'text-white bg-white/5'
                                              : 'text-slate-500 hover:text-slate-300'
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
                );
              })}
            </ul>
          </div>
        ))}
      </nav>

      {/* Logout */}
      <div className="px-3 py-3 border-t border-white/5">
        <button
          onClick={logout}
          className="w-full flex items-center gap-3 px-3 py-2 rounded-lg text-[13px] font-medium text-slate-500 hover:bg-red-500/10 hover:text-red-400 transition-all duration-150"
        >
          <svg className="w-[18px] h-[18px]" fill="none" stroke="currentColor" strokeWidth={1.5} viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
          </svg>
          Çıkış Yap
        </button>
      </div>
    </aside>
  );
}
