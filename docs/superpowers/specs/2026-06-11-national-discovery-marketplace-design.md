# Ulusal Keşif Pazaryeri Tasarımı — %5 Komisyon Modeli

**Tarih:** 2026-06-11
**Durum:** Onaylandı (kullanıcı, 2026-06-11)
**İlgili tasarımlar:** `2026-03-28-subscription-to-commission-design.md`, `2026-03-29-expo-migration-plan.md`

---

## 1. Amaç ve İş Modeli

Platform, ilk günden Türkiye genelinde hizmet veren bir **tüketici keşif pazaryeri** olarak lanse edilir: kullanıcı üye olur, konumuna göre çevresindeki restoranları görür, sipariş verir ve online öder.

**Gelir modeli:** Sipariş başına **%5 komisyon** (platform varsayılanı). Abonelik modeli tamamen kaldırılır — `2026-03-28` tasarımındaki `PlatformCommissionSchedule` / `RestaurantCommission` yapısı uygulanır. Restoran bazlı özel oran altyapısı pazarlık aracı olarak korunur.

**Restoran tarafı satış cümlesi:** "Dükkânda POS'a %2,5-4 komisyon ödüyorsun ve müşteri zaten orada. Burada %5'e hem online ödemeyi hem yeni müşteri kanalını alıyorsun; sabit ücret yok, kazanmadıkça ödemezsin."

**Rakip konumlandırma:**
- Yemeksepeti/Getir/Trendyol Yemek'e karşı: %30+ yerine %5 komisyon.
- RestApp/Restajet/Menulux'e karşı: aylık sabit ücret yerine sadece kazandıkça ödeme + tüketici keşif uygulamasından gelen yeni müşteri.

### Birim ekonomisi (bilinçli kabul edilen kısıt)

İyzico pazaryeri modeli ~%3 + işlem ücreti keser; platformun net payı GMV'nin ~%1,5-2'sidir. Model yalnızca **maliyet tabanı sıfıra yakın tutulursa** çalışır: saha satışı yok, self-servis onboarding, operasyon otomasyonu. Bu kısıt tüm ürün kararlarının filtresi olarak kullanılır.

## 2. Lansman Stratejisi: Ulusal Kapı, Yoğunluk-Tetiklemeli Ateşleme

- Uygulama ve restoran kaydı ilk günden **tüm Türkiye'ye açık**. Herhangi bir ildeki restoran self-servis kaydolabilir.
- Tüketici pazarlama bütçesi yoğunluk eşiğini geçen ilçelere akar: bir ilçede **15-20 aktif restoran** oluştuğunda o ilçeye hedefli sosyal medya reklamı (Instagram/TikTok) başlar. Kullanıcı uygulamayı reklamla keşfettiğinde her zaman dolu vitrin görür.
- Her restorana kayıt sonrası **QR/link materyali** üretilir (uygulama içi restoran profiline yönlendirir). Restoran mevcut müşterilerini kendisi taşır → tüketici tarafı sıfır maliyetle tohumlanır.

## 3. Ödeme ve Komisyon Akışı

- **Faz 1: Yalnızca online ödeme.** Müşteri İyzico üzerinden öder → split payment ile restoran hakedişi otomatik ayrılır → %5 platformda kalır. Komisyon tahsilat riski sıfır.
- **Faz 2: Kapıda ödeme.** Haftalık komisyon faturalaması ve tahsilat akışı tasarlandıktan sonra eklenir. Faz 1 kapsamı dışındadır.
- Günlük hakediş sistemi `2026-03-28` tasarımındaki gibi uygulanır.

## 4. Kurye Modeli

İlk günden yalnızca **restoranın kendi kuryesi** (`RestaurantOwn` — kodda mevcut). Platform kurye ağı kurmaz, kurye tedarik operasyonu yürütmez. `Individual`/`CompanyMember` kurye tipleri ve `RestaurantCourierAgreement` altyapısı kodda korunur, ileri faz için kapalı tutulur.

## 5. Yazılım Kapsamı (öncelik sırasıyla)

1. **Komisyon refactor** — `2026-03-28-subscription-to-commission-design.md` uygulanır: abonelik söküm, `PlatformCommissionSchedule`, `RestaurantCommission`, günlük hakediş.
2. **Restoran self-servis onboarding** — yeni kritik akış: kayıt → işletme/vergi/IBAN bilgileri → İyzico alt üye işyeri başvurusu (API üzerinden) → menü yükleme → otomatik yayına alma. Hedef: restoran "30 dakikada yayında". Admin onay adımı yalnızca dolandırıcılık filtresi olarak kalır.
3. **İyzico pazaryeri canlıya hazırlık** — alt üye onboarding, split payment, hakediş, iade/iptal para akışları; sandbox → prod geçiş kontrol listesi.
4. **Expo migration** — `2026-03-29-expo-migration-plan.md` uygulanır (müşteri + kurye uygulamaları), App Store / Google Play yayını.
5. **Restoran QR/paylaşım sayfası** — restoran profiline giden derin link + QR üretimi; web fallback sayfası (uygulama yüklü değilse store'a yönlendirme + temel menü görünümü).

Mevcut mimari (Clean Architecture, .NET 8, MySQL, Redis, React Native, Next.js panelleri) korunur; yeni alt sistem eklenmez.

## 6. Operasyon ve Yasal

- **Şirket:** Yeni Ltd kurulumu (İyzico pazaryeri başvurusu ve olası yatırım turu için şahıs şirketine tercih edilir).
- **İyzico pazaryeri başvurusu** şirket kurulumunun hemen ardından — onay süresi kritik yol üzerindedir.
- Marka tescili (uygulama adı netleşince), KVKK uyumu (aydınlatma metinleri, gerekiyorsa VERBİS), restoran üyelik sözleşmesi, tüketici mesafeli satış metinleri.

## 7. Riskler

| Risk | Karşılık |
|---|---|
| Boş vitrin: kullanıcı açar, az restoran görür | Pazarlama yalnızca yoğunluk eşiğini geçen ilçelerde; organik kullanıcı restoran QR'ından geldiği için zaten dolu profile iner |
| ~%1,5-2 net marj darlığı | Self-servis onboarding, saha satışsız büyüme, otomasyon; aksi her karar bu filtreden geçer |
| Devlerin fiyat tepkisi | Birim ekonomileri %5'e inmelerine izin vermez; platformun maliyet yapısı zaten ince |
| Restoran kaydolur, menü yüklemez | "30 dakikada yayında" akışı; menü fotoğrafından AI ile menü çıkarma (aday özellik, Faz 1'de değerlendirilecek) |
| İyzico pazaryeri onayının gecikmesi | Başvuru kritik yolda en öne alınır; onay gelene dek sandbox ile geliştirme sürer |

## 8. Başarı Metrikleri

- **6. ay:** ≥3 ilçede yoğunluk eşiği aşılmış (ilçe başına 15-20 aktif restoran), platform genelinde günde ≥100 sipariş.
- **12. ay:** ≥10 ilçede eşik aşılmış, günde ≥500 sipariş (≈ aylık ~6M TL GMV → ~300 bin TL komisyon → ~120 bin TL net platform payı).
- Metrikler tutmazsa 12. ayda strateji gözden geçirme (model pivotu veya bölgesel yoğunlaşma) planın parçasıdır.
