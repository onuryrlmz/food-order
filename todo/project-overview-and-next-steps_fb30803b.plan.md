---
name: project-overview-and-next-steps
overview: Provide a clear multi-level overview of the food-order monorepo and be ready for future feature or refactor work.
todos:
  - id: explain-architecture
    content: Projede backend ve üç frontend uygulamasının genel mimarisini, teknoloji stacklerini ve aralarındaki ilişkiyi kullanıcıya açıklamak.
    status: in_progress
  - id: map-domain-modules
    content: Sipariş, Restoran, Kupon vb. ana domain modüllerini ve hangi katmanda nasıl temsil edildiklerini çıkarıp gerektiğinde kullanıcıya detaylandırmak.
    status: pending
isProject: false
---

## Proje İncelemesi ve Sonraki Adımlar

- Backend ve üç ayrı frontend uygulamasının genel mimarisini özetle.
- Backend katmanlarını (Domain, Application, Persistence, WebAPI) ve temel sorumluluklarını açıkla.
- Admin, Seller ve Customer (mobil) arayüzlerinin hangi teknolojilerle yazıldığını ve backend ile nasıl konuştuklarını kısaca anlat.
- Öne çıkan alan (domain) modüllerini (Sipariş, Sepet, Restoran, Kupon, Abonelik vb.) isim isim say ve ne işe yaradıklarını kısaca açıklamaya hazır ol.
- Kullanıcı ileride özellik eklemek veya refactor yapmak isterse, ilgili katman ve modülleri referans gösterecek şekilde daha detaylı plan çıkar.