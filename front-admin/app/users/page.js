'use client';

import { useState } from 'react';
import useSWR from 'swr';
import AdminLayout from '@/components/layout/AdminLayout';
import Table from '@/components/ui/Table';
import Badge from '@/components/ui/Badge';
import Button from '@/components/ui/Button';
import fetcher from '@/lib/fetcher';

const ROLE_MAP = {
  1: { label: 'Admin', color: 'red' },
  2: { label: 'Kullanıcı', color: 'blue' },
  3: { label: 'Anonim', color: 'gray' },
  4: { label: 'Satıcı Admin', color: 'orange' },
  5: { label: 'Satıcı Kullanıcı', color: 'yellow' },
};

const STATUS_MAP = {
  0: { label: 'Aktivasyon Bekliyor', color: 'yellow' },
  1: { label: 'Aktif', color: 'green' },
  2: { label: 'Pasif', color: 'gray' },
  3: { label: 'Silindi', color: 'red' },
};

const ROLE_FILTERS = [
  { label: 'Tümü', value: null },
  { label: 'Admin', value: 1 },
  { label: 'Kullanıcı', value: 2 },
  { label: 'Satıcı Admin', value: 4 },
  { label: 'Satıcı Kullanıcı', value: 5 },
];

export default function UsersPage() {
  const [page, setPage] = useState(1);
  const [roleFilter, setRoleFilter] = useState(null);

  const query = `/v1/admin/user/list?page=${page}&pageSize=20${roleFilter ? `&roleId=${roleFilter}` : ''}`;
  const { data, isLoading } = useSWR(query, fetcher);

  const users = data?.data || [];
  const total = data?.totalDataCount || 0;
  const totalPages = Math.ceil(total / 20) || 1;

  const columns = [
    {
      title: 'Kullanıcı',
      key: 'email',
      render: (v, row) => (
        <div>
          <p className="font-medium text-gray-800">{row.firstName} {row.lastName}</p>
          <p className="text-xs text-gray-400">{v}</p>
        </div>
      ),
    },
    {
      title: 'Telefon',
      key: 'phoneNumber',
      render: (v) => <span className="text-sm text-gray-600">{v || '—'}</span>,
    },
    {
      title: 'Rol',
      key: 'userRoleId',
      render: (v) => {
        const r = ROLE_MAP[v] || { label: String(v), color: 'gray' };
        return <Badge label={r.label} color={r.color} />;
      },
    },
    {
      title: 'Durum',
      key: 'userStatusId',
      render: (v) => {
        const s = STATUS_MAP[v] || { label: String(v), color: 'gray' };
        return <Badge label={s.label} color={s.color} />;
      },
    },
    {
      title: 'Satıcı ID',
      key: 'sellerId',
      render: (v) => v ? (
        <span className="text-xs font-mono text-gray-500">{v.substring(0, 8)}…</span>
      ) : <span className="text-gray-300">—</span>,
    },
    {
      title: 'Kayıt',
      key: 'createdDate',
      render: (v) => v ? new Date(v).toLocaleDateString('tr-TR') : '—',
    },
  ];

  return (
    <AdminLayout title="Kullanıcılar">
      <div className="space-y-4">
        {/* Rol filtresi */}
        <div className="flex items-center justify-between">
          <div className="flex gap-1 bg-gray-100 p-1 rounded-lg">
            {ROLE_FILTERS.map((f) => (
              <button
                key={String(f.value)}
                onClick={() => { setRoleFilter(f.value); setPage(1); }}
                className={`px-3 py-1.5 rounded-md text-sm font-medium transition-colors ${
                  roleFilter === f.value
                    ? 'bg-white text-orange-600 shadow-sm'
                    : 'text-gray-500 hover:text-gray-700'
                }`}
              >
                {f.label}
              </button>
            ))}
          </div>
          <p className="text-sm text-gray-500">{total} kullanıcı</p>
        </div>

        <Table columns={columns} data={users} loading={isLoading} emptyText="Kullanıcı bulunamadı" />

        {totalPages > 1 && (
          <div className="flex justify-center gap-2 pt-2">
            <Button variant="outline" size="sm" onClick={() => setPage(p => Math.max(1, p - 1))} disabled={page === 1}>
              ← Önceki
            </Button>
            <span className="text-sm text-gray-500 self-center">{page} / {totalPages}</span>
            <Button variant="outline" size="sm" onClick={() => setPage(p => Math.min(totalPages, p + 1))} disabled={page === totalPages}>
              Sonraki →
            </Button>
          </div>
        )}
      </div>
    </AdminLayout>
  );
}
