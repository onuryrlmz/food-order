export default function Table({ columns, data, loading, emptyText = 'Kayıt bulunamadı' }) {
  return (
    <div className="bg-white rounded-xl border border-gray-200 overflow-hidden">
      <div className="overflow-x-auto">
        <table className="w-full text-sm">
          <thead>
            <tr className="bg-gray-50 border-b border-gray-200">
              {columns.map((col, i) => (
                <th
                  key={i}
                  className="px-4 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wide"
                  style={{ width: col.width }}
                >
                  {col.title}
                </th>
              ))}
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr>
                <td colSpan={columns.length} className="px-4 py-12 text-center text-gray-400">
                  <div className="flex items-center justify-center gap-2">
                    <div className="w-4 h-4 border-2 border-orange-400 border-t-transparent rounded-full animate-spin" />
                    Yükleniyor...
                  </div>
                </td>
              </tr>
            ) : !data || data.length === 0 ? (
              <tr>
                <td colSpan={columns.length} className="px-4 py-12 text-center text-gray-400">
                  {emptyText}
                </td>
              </tr>
            ) : (
              data.map((row, i) => (
                <tr key={row.id || i} className="border-b border-gray-100 hover:bg-gray-50/50">
                  {columns.map((col, j) => (
                    <td key={j} className="px-4 py-3 text-gray-700">
                      {col.render ? col.render(row[col.key], row) : row[col.key] ?? '—'}
                    </td>
                  ))}
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
