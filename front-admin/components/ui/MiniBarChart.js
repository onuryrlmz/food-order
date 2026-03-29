export default function MiniBarChart({ data, valueKey, labelKey, color = '#f97316' }) {
  if (!data || data.length === 0) return <p className="text-sm text-gray-400 text-center py-6">Veri yok</p>;
  const max = Math.max(...data.map(d => d[valueKey] || 0), 1);
  return (
    <div className="flex items-end gap-1 h-40">
      {data.map((d, i) => {
        const pct = ((d[valueKey] || 0) / max) * 100;
        return (
          <div key={i} className="flex-1 flex flex-col items-center gap-1">
            <span className="text-xs text-gray-500 font-medium">{d[valueKey] || 0}</span>
            <div
              className="w-full rounded-t transition-all"
              style={{ height: `${Math.max(pct, 4)}%`, minHeight: 4, backgroundColor: color }}
            />
            <span className="text-[10px] text-gray-400 truncate w-full text-center">{d[labelKey] || ''}</span>
          </div>
        );
      })}
    </div>
  );
}
