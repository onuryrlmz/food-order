'use client';

export default function Header({ title }) {
  return (
    <header className="sticky top-0 z-30 bg-white/80 backdrop-blur-md border-b border-slate-200/60 px-6 lg:px-8 py-4 flex items-center justify-between shadow-sm shadow-slate-100">
      {/* Title */}
      <h1 className="text-lg font-semibold text-slate-800 tracking-tight">{title}</h1>

      {/* Right side */}
      <div className="flex items-center gap-4">
        {/* Search */}
        <div className="hidden md:flex items-center gap-2 bg-slate-100 rounded-lg px-3 py-1.5 text-sm text-slate-400 w-56 cursor-pointer hover:bg-slate-200/70 transition-colors">
          <svg className="w-4 h-4" fill="none" stroke="currentColor" strokeWidth={2} viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
          </svg>
          <span>Ara...</span>
          <kbd className="ml-auto text-[10px] font-medium bg-white rounded px-1.5 py-0.5 text-slate-400 border border-slate-200">⌘K</kbd>
        </div>

        {/* Notification Bell */}
        <button className="relative p-2 rounded-lg text-slate-400 hover:bg-slate-100 hover:text-slate-600 transition-colors">
          <svg className="w-5 h-5" fill="none" stroke="currentColor" strokeWidth={1.5} viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" d="M14.857 17.082a23.848 23.848 0 005.454-1.31A8.967 8.967 0 0118 9.75v-.7V9A6 6 0 006 9v.75a8.967 8.967 0 01-2.312 6.022c1.733.64 3.56 1.085 5.455 1.31m5.714 0a24.255 24.255 0 01-5.714 0m5.714 0a3 3 0 11-5.714 0" />
          </svg>
          <span className="absolute top-1.5 right-1.5 w-2 h-2 bg-orange-500 rounded-full ring-2 ring-white"></span>
        </button>

        {/* Divider */}
        <div className="h-6 w-px bg-slate-200"></div>

        {/* Avatar & Name */}
        <div className="flex items-center gap-2.5 cursor-pointer">
          <div className="w-8 h-8 bg-gradient-to-br from-orange-400 to-orange-600 rounded-full flex items-center justify-center shadow-sm">
            <span className="text-white font-bold text-xs">A</span>
          </div>
          <div className="hidden sm:block">
            <p className="text-sm font-medium text-slate-700 leading-tight">Admin</p>
            <p className="text-[11px] text-slate-400 leading-tight">Yönetici</p>
          </div>
        </div>
      </div>
    </header>
  );
}
