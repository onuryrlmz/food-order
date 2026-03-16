'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import { forgotPassword, verifyResetCode, resetPassword } from '@/lib/api';

export default function ForgotPasswordPage() {
  const router = useRouter();
  const [step, setStep] = useState(1);
  const [emailOrPhone, setEmailOrPhone] = useState('');
  const [code, setCode] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const handleSendCode = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      const res = await forgotPassword(emailOrPhone);
      if (!res.hasFailed) {
        setSuccess('Dogrulama kodu gonderildi');
        setStep(2);
      } else {
        setError(res.messages?.[0]?.description || 'Kod gonderilemedi');
      }
    } catch {
      setError('Bir hata olustu. Tekrar deneyin.');
    } finally {
      setLoading(false);
    }
  };

  const handleVerifyCode = async (e) => {
    e.preventDefault();
    setError('');
    setSuccess('');
    setLoading(true);
    try {
      const res = await verifyResetCode(emailOrPhone, code);
      if (!res.hasFailed) {
        setStep(3);
      } else {
        setError(res.messages?.[0]?.description || 'Gecersiz kod');
      }
    } catch {
      setError('Bir hata olustu. Tekrar deneyin.');
    } finally {
      setLoading(false);
    }
  };

  const handleResetPassword = async (e) => {
    e.preventDefault();
    setError('');
    if (newPassword.length < 6) {
      setError('Sifre en az 6 karakter olmalidir');
      return;
    }
    if (newPassword !== confirmPassword) {
      setError('Sifreler eslesmemektedir');
      return;
    }
    setLoading(true);
    try {
      const res = await resetPassword(emailOrPhone, code, newPassword);
      if (!res.hasFailed) {
        setSuccess('Sifreniz basariyla degistirildi! Giris yapabilirsiniz.');
        setTimeout(() => router.push('/login'), 2000);
      } else {
        setError(res.messages?.[0]?.description || 'Sifre degistirilemedi');
      }
    } catch {
      setError('Bir hata olustu. Tekrar deneyin.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gray-900 flex items-center justify-center p-4">
      <div className="w-full max-w-md">
        <div className="text-center mb-8">
          <div className="w-14 h-14 bg-emerald-500 rounded-2xl flex items-center justify-center mx-auto mb-4">
            <svg className="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 7a2 2 0 012 2m4 0a6 6 0 01-7.743 5.743L11 17H9v2H7v2H4a1 1 0 01-1-1v-2.586a1 1 0 01.293-.707l5.964-5.964A6 6 0 1121 9z" />
            </svg>
          </div>
          <h1 className="text-2xl font-bold text-white">Sifremi Unuttum</h1>
          <p className="text-gray-400 mt-1 text-sm">
            {step === 1 && 'E-posta adresinizi girin'}
            {step === 2 && 'Dogrulama kodunu girin'}
            {step === 3 && 'Yeni sifrenizi belirleyin'}
          </p>
        </div>

        {/* Step indicator */}
        <div className="flex items-center justify-center gap-2 mb-6">
          {[1, 2, 3].map(s => (
            <div
              key={s}
              className={`h-2 rounded-full transition-all ${
                s === step ? 'w-8 bg-emerald-500' : s < step ? 'w-8 bg-emerald-700' : 'w-8 bg-gray-700'
              }`}
            />
          ))}
        </div>

        <div className="bg-white rounded-2xl shadow-xl p-8">
          {step === 1 && (
            <form onSubmit={handleSendCode} className="space-y-5">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1.5">E-posta veya Telefon</label>
                <input
                  type="text"
                  required
                  value={emailOrPhone}
                  onChange={(e) => setEmailOrPhone(e.target.value)}
                  placeholder="ornek@firma.com"
                  className="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-sm outline-none focus:ring-2 focus:ring-emerald-400 focus:border-emerald-400"
                />
              </div>
              {error && <div className="bg-red-50 border border-red-200 rounded-lg p-3 text-sm text-red-600">{error}</div>}
              <button
                type="submit"
                disabled={loading}
                className="w-full bg-emerald-600 hover:bg-emerald-700 text-white font-semibold py-2.5 rounded-lg text-sm disabled:opacity-60 flex items-center justify-center gap-2"
              >
                {loading && <span className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin" />}
                {loading ? 'Gonderiliyor...' : 'Kod Gonder'}
              </button>
            </form>
          )}

          {step === 2 && (
            <form onSubmit={handleVerifyCode} className="space-y-5">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1.5">Dogrulama Kodu</label>
                <input
                  type="text"
                  required
                  value={code}
                  onChange={(e) => setCode(e.target.value.replace(/[^0-9]/g, '').slice(0, 6))}
                  placeholder="000000"
                  maxLength={6}
                  className="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-sm text-center text-2xl font-bold tracking-widest outline-none focus:ring-2 focus:ring-emerald-400 focus:border-emerald-400"
                />
              </div>
              {success && <div className="bg-green-50 border border-green-200 rounded-lg p-3 text-sm text-green-600">{success}</div>}
              {error && <div className="bg-red-50 border border-red-200 rounded-lg p-3 text-sm text-red-600">{error}</div>}
              <button
                type="submit"
                disabled={loading}
                className="w-full bg-emerald-600 hover:bg-emerald-700 text-white font-semibold py-2.5 rounded-lg text-sm disabled:opacity-60 flex items-center justify-center gap-2"
              >
                {loading && <span className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin" />}
                {loading ? 'Dogrulanıyor...' : 'Dogrula'}
              </button>
              <button
                type="button"
                onClick={() => { setStep(1); setError(''); setSuccess(''); }}
                className="w-full text-sm text-gray-500 hover:text-gray-700"
              >
                Tekrar kod gonder
              </button>
            </form>
          )}

          {step === 3 && (
            <form onSubmit={handleResetPassword} className="space-y-5">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1.5">Yeni Sifre</label>
                <input
                  type="password"
                  required
                  value={newPassword}
                  onChange={(e) => setNewPassword(e.target.value)}
                  placeholder="••••••••"
                  className="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-sm outline-none focus:ring-2 focus:ring-emerald-400 focus:border-emerald-400"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1.5">Sifre Tekrar</label>
                <input
                  type="password"
                  required
                  value={confirmPassword}
                  onChange={(e) => setConfirmPassword(e.target.value)}
                  placeholder="••••••••"
                  className="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-sm outline-none focus:ring-2 focus:ring-emerald-400 focus:border-emerald-400"
                />
              </div>
              {success && <div className="bg-green-50 border border-green-200 rounded-lg p-3 text-sm text-green-600">{success}</div>}
              {error && <div className="bg-red-50 border border-red-200 rounded-lg p-3 text-sm text-red-600">{error}</div>}
              <button
                type="submit"
                disabled={loading}
                className="w-full bg-emerald-600 hover:bg-emerald-700 text-white font-semibold py-2.5 rounded-lg text-sm disabled:opacity-60 flex items-center justify-center gap-2"
              >
                {loading && <span className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin" />}
                {loading ? 'Degistiriliyor...' : 'Sifreyi Degistir'}
              </button>
            </form>
          )}

          <div className="mt-4 text-center">
            <a href="/login" className="text-sm text-emerald-600 hover:underline">Giris ekranina don</a>
          </div>
        </div>
      </div>
    </div>
  );
}
