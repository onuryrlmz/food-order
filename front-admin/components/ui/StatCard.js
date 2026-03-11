export default function StatCard({ title, value, icon, color = 'orange', trend }) {
  const colors = {
    orange: 'bg-orange-50 text-orange-600',
    blue: 'bg-blue-50 text-blue-600',
    green: 'bg-green-50 text-green-600',
    purple: 'bg-purple-50 text-purple-600',
    red: 'bg-red-50 text-red-600',
  };

  return (
    <div className="bg-white rounded-xl border border-gray-200 p-5">
      <div className="flex items-start justify-between">
        <div>
          <p className="text-sm text-gray-500 font-medium">{title}</p>
          <p className="text-2xl font-bold text-gray-800 mt-1">{value ?? '—'}</p>
          {trend && (
            <p className="text-xs text-green-600 mt-1 font-medium">{trend}</p>
          )}
        </div>
        <div className={`w-11 h-11 rounded-xl flex items-center justify-center ${colors[color]}`}>
          {icon}
        </div>
      </div>
    </div>
  );
}
