'use client';

export default function Header({ title }) {
  return (
    <header className="bg-white border-b border-gray-200 px-6 py-4 flex items-center justify-between">
      <h1 className="text-xl font-semibold text-gray-800">{title}</h1>
      <div className="flex items-center gap-3">
        <div className="w-8 h-8 bg-orange-100 rounded-full flex items-center justify-center">
          <span className="text-orange-600 font-bold text-sm">A</span>
        </div>
        <span className="text-sm text-gray-600 font-medium">Admin</span>
      </div>
    </header>
  );
}
