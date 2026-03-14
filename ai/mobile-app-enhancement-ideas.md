## Mobil Uygulamada Yapılabilecek Geliştirmeler

Aşağıdaki maddeler, mevcut React Native müşteri uygulamasına (`front-app`) yönelik yeni özellik, UX ve teknik iyileştirme önerilerini içerir.

- **Özellik Geliştirmeleri**
  - Restoran ve ürünler için gelişmiş filtreleme/sıralama (puan, mesafe, teslimat süresi, fiyat aralığı).
  - Kupon ve kampanya merkezi: Kullanıcının tüm aktif kuponlarını, restoran kampanyalarını ve özel fırsatları görebileceği ayrı bir ekran.
  - Sadakat sistemi: Sipariş başına puan toplama, seviye sistemi (Bronz/Gümüş/Altın), puanla indirim veya ücretsiz ürün.
  - Favoriler / tekrar sipariş: Sık sipariş verilen restoran ve ürünleri kaydetme, tek tıkla \"yeniden sipariş\" akışı.
  - Canlı sipariş takibi: Kuryenin konumunu ve tahmini teslimat süresini gösteren gerçek zamanlı takip ekranı.
  - Değerlendirme & yorumlar: Sipariş sonrası restoran ve ürünlere yıldız verme, yorum yazma ve önceki yorumları görüntüleme.

- **UX / Tasarım İyileştirmeleri**
  - Ana sayfada kişiselleştirilmiş öneriler (geçmiş siparişler, bulunduğu konum, tercih ettiği mutfaklara göre).
  - Restoran ve ürün kartlarının daha zengin görsellerle, rozetlerle (\"Popüler\", \"Yeni\", \"İndirimli\") desteklenmesi.
  - Kupon kullanma akışının sadeleştirilmesi: Sepet ekranında çok net bir kupon girişi alanı, geçerli/geçersiz durumunu anlık gösteren geri bildirim.
  - Karanlık mod desteği ve erişilebilirlik (font boyutu, kontrast, ekran okuyucu uyumu) iyileştirmeleri.

- **Teknik / Performans ve Dayanıklılık**
  - Kritik ekranlarda (ana liste, restoran detayı, sepet) skeleton loading ve akıllı önbellekleme.
  - Offline mod: Kısa süreli bağlantı kopmalarında kullanıcıyı uygulamadan koparmadan sepeti ve son baktığı restoranları gösterme; bağlantı gelince senkronizasyon.
  - Hata yönetimi: Global error boundary, merkezi loglama ve kullanıcıya daha anlaşılır hata mesajları.
  - Push bildirimleri: Sipariş durumu güncellemeleri, kampanya bildirimleri, hatırlatma ve geri dönüş (feedback) bildirimleri.

- **Analitik ve Deneyim Ölçümü**
  - Ekran bazlı analitik: Hangi sayfalar daha çok kullanılıyor, sepetten vazgeçme oranı nerede yükseliyor.
  - A/B testleri: Örneğin farklı sepet tasarımları veya kupon gösterim biçimlerinin denenmesi.

Bu doküman, uygulamayı özellik, deneyim ve teknik açıdan olgunlaştırmak için bir fikir havuzu sunar. Sonraki adım, bu havuzdan önceliklendirilmiş bir kısa liste seçip, her birini ayrı user story ve teknik görevlere bölmektir.
