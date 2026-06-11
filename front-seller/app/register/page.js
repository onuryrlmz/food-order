'use client';

import { useState } from 'react';
import Link from 'next/link';
import { registerSeller } from '@/lib/auth';

const EMPTY_GUID = '00000000-0000-0000-0000-000000000000';

// CompanyTypeEnums: Individual=1, Company=2
const initialForm = {
  companyType: 2,
  name: '', legalName: '', taxCode: '', taxArea: '', iban: '',
  identityNumber: '',
  ownerFirstName: '', ownerLastName: '', ownerEmail: '', ownerPhone: '',
  password: '', passwordConfirm: '',
  addressLine1: '', addressLine2: '',
};

function Field({ label, children }) {
  return (
    <label className="block">
      <span className="text-sm font-medium text-gray-700">{label}</span>
      <div className="mt-1">{children}</div>
    </label>
  );
}

const inputCls =
  'w-full rounded-lg border border-gray-300 px-3 py-2 text-sm text-gray-900 focus:border-emerald-500 focus:outline-none focus:ring-1 focus:ring-emerald-500';

export default function RegisterPage() {
  const [form, setForm] = useState(initialForm);
  const [step, setStep] = useState(1);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [done, setDone] = useState(false);

  const set = (key) => (e) => setForm((f) => ({ ...f, [key]: e.target.value }));
  const isIndividual = Number(form.companyType) === 1;

  const validateStep1 = () => {
    if (!form.name || !form.legalName || !form.taxCode || !form.taxArea || !form.iban || !form.addressLine1)
      return 'Lütfen tüm zorunlu alanları doldurun.';
    const taxCode = form.taxCode.trim();
    if (isIndividual ? taxCode.length !== 11 : taxCode.length !== 10)
      return isIndividual
        ? 'Şahıs işletmesi için vergi kimlik numarası 11 haneli olmalıdır.'
        : 'Şirket için vergi numarası 10 haneli olmalıdır.';
    if (form.iban.replace(/\s/g, '').length !== 26)
      return 'IBAN, TR ile birlikte 26 karakter olmalıdır.';
    if (isIndividual && form.identityNumber.length !== 11)
      return 'Şahıs işletmesi için 11 haneli TC kimlik numarası gereklidir.';
    return '';
  };

  const validateStep2 = () => {
    if (!form.ownerFirstName || !form.ownerLastName || !form.ownerEmail || !form.ownerPhone)
      return 'Lütfen tüm zorunlu alanları doldurun.';
    if (form.ownerPhone.replace(/\D/g, '').length < 10)
      return 'Geçerli bir telefon numarası giriniz (en az 10 hane).';
    if (form.password.length < 8) return 'Şifre en az 8 karakter olmalıdır.';
    if (form.password !== form.passwordConfirm) return 'Şifreler eşleşmiyor.';
    return '';
  };

  const next = () => {
    const msg = validateStep1();
    if (msg) { setError(msg); return; }
    setError('');
    setStep(2);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    const msg = validateStep2();
    if (msg) { setError(msg); return; }
    setError('');
    setLoading(true);
    try {
      await registerSeller({
        companyType: Number(form.companyType),
        name: form.name,
        legalName: form.legalName,
        taxCode: form.taxCode.trim(),
        taxArea: form.taxArea,
        iban: form.iban.replace(/\s/g, '').toUpperCase(),
        isEInvoiceAvaible: false,
        ...(isIndividual ? { identityNumber: form.identityNumber.trim() } : {}),
        ownerFirstName: form.ownerFirstName,
        ownerLastName: form.ownerLastName,
        ownerEmail: form.ownerEmail.trim(),
        ownerPhone: form.ownerPhone,
        password: form.password,
        cityId: EMPTY_GUID, townId: EMPTY_GUID, neighbourhoodId: EMPTY_GUID,
        addressLine1: form.addressLine1,
        ...(form.addressLine2 ? { addressLine2: form.addressLine2 } : {}),
      });
      setDone(true);
    } catch (err) {
      setError(err.message || 'Başvuru gönderilemedi. Lütfen tekrar deneyin.');
    } finally {
      setLoading(false);
    }
  };

  if (done) {
    return (
      <div className="min-h-screen bg-gray-900 flex items-center justify-center p-4">
        <div className="w-full max-w-md bg-white rounded-2xl shadow-xl p-8 text-center">
          <div className="w-14 h-14 bg-emerald-100 rounded-full flex items-center justify-center mx-auto mb-4">
            <svg className="w-8 h-8 text-emerald-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
            </svg>
          </div>
          <h1 className="text-xl font-bold text-gray-900">Başvurunuz alındı</h1>
          <p className="text-gray-600 mt-2 text-sm">
            Başvurunuz inceleniyor. Onaylandığında e-posta adresiniz ve şifrenizle giriş yapabilirsiniz.
          </p>
          <Link href="/login" className="inline-block mt-6 text-emerald-600 font-medium hover:underline text-sm">
            Giriş sayfasına dön
          </Link>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-900 flex items-center justify-center p-4">
      <div className="w-full max-w-lg">
        <div className="text-center mb-8">
          <h1 className="text-2xl font-bold text-white">Restoran Başvurusu</h1>
          <p className="text-gray-400 mt-1 text-sm">
            Adım {step}/2 — {step === 1 ? 'İşletme bilgileri' : 'Hesap bilgileri'}
          </p>
        </div>

        <div className="bg-white rounded-2xl shadow-xl p-8">
          <form onSubmit={handleSubmit} className="space-y-4">
            {step === 1 && (
              <>
                <Field label="İşletme türü">
                  <select className={inputCls} value={form.companyType} onChange={set('companyType')}>
                    <option value={2}>Şirket (Ltd/A.Ş.)</option>
                    <option value={1}>Şahıs işletmesi</option>
                  </select>
                </Field>
                <Field label="İşletme adı (tabela adı)">
                  <input className={inputCls} value={form.name} onChange={set('name')} placeholder="Örn. Lezzet Lokantası" />
                </Field>
                <Field label="Ticari unvan">
                  <input className={inputCls} value={form.legalName} onChange={set('legalName')} placeholder="Örn. Lezzet Gıda Ltd. Şti." />
                </Field>
                <div className="grid grid-cols-2 gap-3">
                  <Field label="Vergi numarası">
                    <input className={inputCls} value={form.taxCode} onChange={set('taxCode')} inputMode="numeric" maxLength={11} />
                  </Field>
                  <Field label="Vergi dairesi">
                    <input className={inputCls} value={form.taxArea} onChange={set('taxArea')} />
                  </Field>
                </div>
                {isIndividual && (
                  <Field label="TC kimlik numarası">
                    <input className={inputCls} value={form.identityNumber} onChange={set('identityNumber')} inputMode="numeric" maxLength={11} />
                  </Field>
                )}
                <Field label="IBAN (hakediş ödemeleri için)">
                  <input className={inputCls} value={form.iban} onChange={set('iban')} placeholder="TR__ ____ ____ ____ ____ ____ __" />
                </Field>
                <Field label="İşletme adresi">
                  <input className={inputCls} value={form.addressLine1} onChange={set('addressLine1')} placeholder="Mahalle, sokak, no, ilçe/il" />
                </Field>
                <Field label="Adres satırı 2 (isteğe bağlı)">
                  <input className={inputCls} value={form.addressLine2} onChange={set('addressLine2')} />
                </Field>
              </>
            )}

            {step === 2 && (
              <>
                <div className="grid grid-cols-2 gap-3">
                  <Field label="Yetkili adı">
                    <input className={inputCls} value={form.ownerFirstName} onChange={set('ownerFirstName')} />
                  </Field>
                  <Field label="Yetkili soyadı">
                    <input className={inputCls} value={form.ownerLastName} onChange={set('ownerLastName')} />
                  </Field>
                </div>
                <Field label="E-posta (giriş için kullanılacak)">
                  <input className={inputCls} type="email" value={form.ownerEmail} onChange={set('ownerEmail')} />
                </Field>
                <Field label="Telefon">
                  <input className={inputCls} type="tel" value={form.ownerPhone} onChange={set('ownerPhone')} placeholder="5__ ___ __ __" />
                </Field>
                <Field label="Şifre (en az 8 karakter)">
                  <input className={inputCls} type="password" value={form.password} onChange={set('password')} />
                </Field>
                <Field label="Şifre (tekrar)">
                  <input className={inputCls} type="password" value={form.passwordConfirm} onChange={set('passwordConfirm')} />
                </Field>
              </>
            )}

            {error && (
              <div className="rounded-lg bg-red-50 border border-red-200 px-3 py-2 text-sm text-red-700 whitespace-pre-line">
                {error}
              </div>
            )}

            <div className="flex gap-3 pt-2">
              {step === 2 && (
                <button type="button" onClick={() => { setError(''); setStep(1); }}
                  className="flex-1 rounded-lg border border-gray-300 py-2.5 text-sm font-medium text-gray-700 hover:bg-gray-50">
                  Geri
                </button>
              )}
              {step === 1 ? (
                <button type="button" onClick={next}
                  className="flex-1 rounded-lg bg-emerald-500 py-2.5 text-sm font-semibold text-white hover:bg-emerald-600">
                  Devam et
                </button>
              ) : (
                <button type="submit" disabled={loading}
                  className="flex-1 rounded-lg bg-emerald-500 py-2.5 text-sm font-semibold text-white hover:bg-emerald-600 disabled:opacity-50">
                  {loading ? 'Gönderiliyor…' : 'Başvuruyu gönder'}
                </button>
              )}
            </div>
          </form>

          <p className="text-center text-sm text-gray-500 mt-6">
            Zaten hesabınız var mı?{' '}
            <Link href="/login" className="text-emerald-600 font-medium hover:underline">Giriş yapın</Link>
          </p>
        </div>
      </div>
    </div>
  );
}
