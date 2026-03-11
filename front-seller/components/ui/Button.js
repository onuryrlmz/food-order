'use client';

const variants = {
  primary: 'bg-emerald-600 hover:bg-emerald-700 text-white',
  secondary: 'bg-gray-100 hover:bg-gray-200 text-gray-700',
  outline: 'border border-gray-300 hover:bg-gray-50 text-gray-700',
  danger: 'bg-red-500 hover:bg-red-600 text-white',
  ghost: 'hover:bg-gray-100 text-gray-600',
};
const sizes = {
  sm: 'px-3 py-1.5 text-xs',
  md: 'px-4 py-2 text-sm',
  lg: 'px-5 py-2.5 text-sm',
};

export default function Button({ children, variant = 'primary', size = 'md', loading, className = '', ...props }) {
  return (
    <button
      {...props}
      disabled={loading || props.disabled}
      className={`inline-flex items-center gap-2 font-medium rounded-lg transition-colors disabled:opacity-50 disabled:cursor-not-allowed ${variants[variant]} ${sizes[size]} ${className}`}
    >
      {loading && <span className="w-3.5 h-3.5 border-2 border-current border-t-transparent rounded-full animate-spin" />}
      {children}
    </button>
  );
}
