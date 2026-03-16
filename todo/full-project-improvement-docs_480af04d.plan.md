---
name: full-project-improvement-docs
overview: Prepare medium-detail, wide-scope improvement idea documents for backend, front-admin, and front-seller, and later save them as markdown files under the ai folder.
todos:
  - id: plan-backend-improvements-doc
    content: Backend için orta detaylı, çok yönlü geliştirme önerilerini içeren bir MD dokümanı planlamak (ai/backend-improvement-ideas.md).
    status: pending
  - id: plan-front-admin-improvements-doc
    content: front-admin için orta detaylı, çok yönlü geliştirme önerilerini içeren bir MD dokümanı planlamak (ai/front-admin-improvement-ideas.md).
    status: pending
  - id: plan-front-seller-improvements-doc
    content: front-seller için orta detaylı, çok yönlü geliştirme önerilerini içeren bir MD dokümanı planlamak (ai/front-seller-improvement-ideas.md).
    status: pending
isProject: false
---

## Tüm Proje İçin Geliştirme Önerileri Doküman Planı

Kullanıcı isteği: Backend, `front-admin` ve `front-seller` için; yeni özellik, UX, teknik/mimari ve süreç bazlı öneriler içeren **orta detaylı** (madde madde, 1-2 cümle açıklama) MD dosyaları hazırlanacak ve proje kökündeki `ai` klasörü altında saklanacak.

### 1. Backend Geliştirme Önerileri

- `backend` klasör yapısını ve ana bounded context’leri (Buyer, Seller, Admin, kupon, sipariş vb.) gözden geçir.
- Aşağıdaki başlıklar altında, her biri için 1-2 cümle açıklamalı maddeler üret:
  - Özellik / domain genişletmeleri (ör. kupon kuralları, raporlama, abonelik, çoklu para birimi, restoran çalışma saatleri vb.).
  - Mimari ve teknik iyileştirmeler (katmanlar arası sınırların netleştirilmesi, modülerlik, caching, security hardening vb.).
  - Performans, ölçeklenebilirlik, logging/monitoring ve test stratejileri.
  - Süreç ve altyapı (CI/CD, migration yönetimi, dokümantasyon, kod kalite kuralları).
- Bu içeriği `ai/backend-improvement-ideas.md` dosyası olarak kaydet (kullanıcı planı onayladıktan sonra).

### 2. front-admin Geliştirme Önerileri

- `front-admin` Next.js admin panelinin ekran akışlarını ve temel modüllerini (kupon yönetimi, restoran yönetimi, kullanıcı/satıcı yönetimi vb.) gözden geçir.
- Aşağıdaki başlıklar altında öneriler üret (her madde için 1-2 cümle):
  - Yeni veya geliştirilmiş admin özellikleri (gelişmiş filtreler, dashboard’lar, rol bazlı erişim, bulk işlemler vb.).
  - UX / tasarım iyileştirmeleri (bilgi mimarisi, tablo/filtre deneyimi, feedback mesajları, erişilebilirlik).
  - Teknik iyileştirmeler (data fetching stratejisi, state management, error boundary, performans optimizasyonu).
  - Süreç ve kalite (storybook/stil rehberi, testler, tip güvenliği, CI kontrolleri).
- Bu içeriği `ai/front-admin-improvement-ideas.md` dosyası olarak kaydet (kullanıcı planı onayladıktan sonra).

### 3. front-seller Geliştirme Önerileri

- `front-seller` Next.js satıcı panelinin temel işlevlerini (menü/ürün yönetimi, sipariş takibi, kupon, abonelik vb.) gözden geçir.
- Aşağıdaki başlıklarda, her madde için 1-2 cümle açıklamalı öneriler yaz:
  - Özellik geliştirmeleri (gelişmiş raporlar, kampanya sihirbazları, gelir analizi, müşteri segmentleri vb.).
  - UX / tasarım (dashboard düzeni, bildirimler, mobil uyumluluk, formlarda hata gösterimi vb.).
  - Teknik iyileştirmeler (DevExtreme bileşenlerinin verimli kullanımı, data caching, hata yönetimi, performans).
  - Süreç/kalite (testler, tip güvenliği, storybook, dokümantasyon, CI pipeline’ları).
- Bu içeriği `ai/front-seller-improvement-ideas.md` dosyası olarak kaydet (kullanıcı planı onayladıktan sonra).

### 4. Önceliklendirme İçin Hazırlık

- Her dokümanın başına, önerilerin sadece bir “fikir havuzu” olduğu ve sonraki adımın ürün/teknik önceliklendirme yapılması gerektiğini belirten kısa bir açıklama ekle.
- İstenirse daha sonra, bu dokümanlardan seçilen maddeler için ayrıntılı user story ve teknik task breakdown yapılabileceğini not et.

