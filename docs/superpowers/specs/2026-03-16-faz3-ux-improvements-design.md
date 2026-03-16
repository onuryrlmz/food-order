# Faz 3 Tasarım Dokümanı — UX Geliştirmeleri

**Tarih:** 2026-03-16
**Durum:** Onaylandı
**Kapsam:** Backend, front-admin, front-seller, front-courier, front-app

---

## 1. Yorum/Değerlendirme Sistemi

### Entity: Review
```
- Id (Guid)
- OrderId (Guid, FK, unique) — her siparişe 1 review
- UserId (Guid, FK) — müşteri
- RestaurantId (Guid, FK)
- Rating (short) — 1-5 yıldız
- Comment (string?, max 500)
- CreatedAt (DateTime)
```

### Akış
- Sipariş Delivered olduktan sonra müşteri app'te "Değerlendir" butonu
- 1-5 yıldız + opsiyonel yorum
- Restaurant.Rating ve RatingCount güncellenir (running average)
- Satıcı panelinde yorumlar listesi (read-only)
- Admin panelinde yorum moderasyonu (sil)

### API
```
POST /v1/customer/order/{orderId}/review   — yorum yap
GET  /v1/customer/restaurant/{id}/reviews  — restoran yorumları (paginated)
GET  /v1/seller/reviews                    — satıcının yorumları
DELETE /v1/admin/reviews/{id}              — yorum sil (moderasyon)
```

---

## 2. Restoran Arama/Filtreleme

### Mevcut Durum
Sadece konum bazlı arama var (ST_Contains polygon). İsim, mutfak, fiyat, puan filtresi yok.

### Yeni Özellikler
- **İsim araması**: LIKE '%query%' veya MySQL FULLTEXT
- **Mutfak filtresi**: cuisineId parametresi
- **Minimum sipariş filtresi**: maxMinOrderPrice parametresi
- **Puan filtresi**: minRating parametresi
- **Sıralama**: rating, deliveryTime, minOrderPrice
- **Açık/Kapalı filtresi**: isOpen parametresi

### API Güncelleme
```
GET /customer/restaurant/list?latitude=X&longitude=Y
    &search=pizza           — isim/açıklama araması
    &cuisineId=xxx          — mutfak filtresi
    &maxMinOrder=50         — max minimum sipariş tutarı
    &minRating=4            — minimum puan
    &sortBy=rating|time|price  — sıralama
    &isOpen=true            — sadece açık restoranlar
    &page=1&size=20         — pagination
```

### Frontend (front-app)
- HomeScreen'e arama çubuğu (zaten var, backend'e bağlanacak)
- Filtre butonu → modal: mutfak, fiyat aralığı, puan, sıralama
- Aktif filtre badge'leri

---

## 3. Menü Görselleri Upload

### Akış
- Satıcı panelden ürün ekleme/düzenleme sırasında görsel yükleme
- Backend: multipart/form-data → AWS S3/Cloudflare R2'ye upload
- Resize: 800x600 max (server-side veya pre-signed URL ile client-side)
- URL Product/Menu entity'sine kaydedilir

### API
```
POST /v1/seller/product/{id}/image   — görsel yükle (multipart)
DELETE /v1/seller/product/{id}/image  — görsel sil
```

### Entity Değişikliği
ProductImage tablosu zaten var, kullanılacak.

---

## 4. Favori Restoranlar

### Entity: FavoriteRestaurant
```
- Id (Guid)
- UserId (Guid, FK)
- RestaurantId (Guid, FK)
- CreatedAt (DateTime)
```
Unique constraint: (UserId, RestaurantId)

### API
```
POST   /v1/customer/favorites/{restaurantId}  — favoriye ekle
DELETE /v1/customer/favorites/{restaurantId}  — favoriden çıkar
GET    /v1/customer/favorites                 — favori listesi
```

### Frontend (front-app)
- Restoran kartında/detayında kalp ikonu (toggle)
- Profil → Favorilerim sayfası

---

## 5. Tekrar Sipariş (Reorder)

### Akış
- Sipariş geçmişinde "Tekrar Sipariş Ver" butonu
- Mevcut siparişin ürünlerini sepete ekler
- Ürün artık yoksa veya fiyat değiştiyse uyarı gösterilir
- Kullanıcı sepeti review edip onaylar

### API
```
POST /v1/customer/order/{orderId}/reorder  — siparişin ürünlerini sepete ekle
```

Response: başarıyla eklenen ürünler + uyarılar (fiyat değişikliği, stok yok)

---

## 6. Kurye Kazanç Takibi

### Akış
- Kurye app'te "Kazançlarım" tab'ı
- Günlük/haftalık/aylık kazanç özeti
- Teslimat başına kazanç listesi

### Hesaplama
- Her teslim edilen sipariş için: Order.ShipmentPrice (teslimat ücreti)
- Toplam kazanç = teslim edilen siparişlerin ShipmentPrice toplamı

### API
```
GET /v1/courier/earnings?period=daily|weekly|monthly  — kazanç özeti
GET /v1/courier/earnings/history?month=3&year=2026    — detaylı liste
```

---

## 7. Kurye Online/Offline Toggle

### Entity Değişikliği
User tablosuna veya ayrı tabloya:
```
- CourierStatusId (short) — Offline=1, Online=2, OnDelivery=3
```
Mevcut `CourierStatusEnums` zaten tanımlı.

### Akış
- Kurye app'te toggle switch (Online/Offline)
- Online olunca restoranlara görünür (sipariş atanabilir)
- Aktif teslimat varken otomatik OnDelivery
- Tüm teslimatlar bitince Online'a döner

### API
```
PUT /v1/courier/status   — { statusId: 1|2 }
GET /v1/courier/status   — mevcut durum
```

### Satıcı Tarafı
Kurye listesinde online/offline durumu görünür.

---

## 8. Seller Analytics Dashboard

### Metrikler
- Günlük/haftalık/aylık sipariş sayısı (grafik)
- Günlük gelir trendi
- En çok satan ürünler (top 10)
- Ortalama sipariş tutarı
- Müşteri sayısı (unique)
- Sipariş saatleri dağılımı (hangi saatte en çok sipariş)

### API
```
GET /v1/seller/analytics/orders?period=daily|weekly|monthly&restaurantId=X
GET /v1/seller/analytics/revenue?period=daily|weekly|monthly&restaurantId=X
GET /v1/seller/analytics/top-products?restaurantId=X&limit=10
GET /v1/seller/analytics/summary?restaurantId=X  — özet kartlar
```

### Frontend (front-seller)
- Dashboard'a analytics widget'ları
- Ayrı /analytics sayfası: grafikler, tablolar

---

## 9. Admin Raporlar (Genişletilmiş)

### Mevcut dashboard'a eklenenler
- Sipariş trend grafiği (günlük)
- Gelir trend grafiği
- Aktif restoran/satıcı/kurye sayıları (zaman serisi)
- En çok sipariş alan restoranlar (top 10)

### API
```
GET /v1/admin/analytics/orders?period=daily|weekly|monthly
GET /v1/admin/analytics/revenue?period=daily|weekly|monthly
GET /v1/admin/analytics/top-restaurants?limit=10
GET /v1/admin/analytics/summary  — platform özet kartlar
```

---

## 10. Sipariş Zaman Aşımı

### Akış
- Restoran MaxDeliveryTime'ı aşan siparişler için uyarı
- Hangfire job (her 15 dk): `CheckDeliveryTimeouts`
  - OnTheWay durumundaki siparişler kontrol edilir
  - CreatedDate + MaxDeliveryTime < now ise → bildirim gönder (müşteri + restoran)
  - Otomatik iptal yok, sadece uyarı

### API
```
GET /v1/admin/orders/overdue  — zaman aşımı olan siparişler listesi
```

---

## 11. Veritabanı Değişiklikleri Özeti

### Yeni Tablolar
- `Reviews`
- `FavoriteRestaurants`

### Mevcut Tablo Güncellemeleri
- `Users` → + CourierStatusId (short, default 1=Offline)

### Yeni NuGet Paketleri
- Yok (mevcut S3 adapter kullanılır)

### Yeni NPM Paketleri
- Chart kütüphanesi: `recharts` (front-seller, front-admin) veya DevExtreme chart (zaten var)
