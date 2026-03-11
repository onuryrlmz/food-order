'use client';

export default function Header({ title, children }) {
  return (
    <header className="h-16 bg-white border-b border-gray-200 flex items-center justify-between px-6 shrink-0">
      <h1 className="text-lg font-semibold text-gray-800">{title}</h1>
      {children && <div className="flex items-center gap-3">{children}</div>}
    </header>
  );
}
