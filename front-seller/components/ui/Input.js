export default function Input({ label, error, className = '', ...props }) {
  return (
    <div className="w-full">
      {label && <label className="block text-sm font-medium text-gray-700 mb-1.5">{label}</label>}
      <input
        {...props}
        className={`w-full border ${error ? 'border-red-400' : 'border-gray-300'} rounded-lg px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-emerald-400 focus:border-emerald-400 ${className}`}
      />
      {error && <p className="text-xs text-red-500 mt-1">{error}</p>}
    </div>
  );
}
