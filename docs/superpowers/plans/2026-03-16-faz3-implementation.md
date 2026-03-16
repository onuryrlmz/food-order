# Faz 3 Implementation Plan — UX Geliştirmeleri

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development or superpowers:executing-plans.

**Goal:** Add reviews, search/filters, image upload, favorites, reorder, courier earnings/status, analytics dashboards, delivery timeout.

**Architecture:** Same patterns as Faz 1-2.

**Spec:** `docs/superpowers/specs/2026-03-16-faz3-ux-improvements-design.md`

---

## Chunk 1: Backend — Entities, Repos, New Features

### Task 1: Create Review and FavoriteRestaurant Entities + Repos

**Files:**
- Create: `backend/Domain/Entities/Buyer/Review.cs`
- Create: `backend/Domain/Entities/Buyer/FavoriteRestaurant.cs`
- Create configs, repos, register in DbContext/UnitOfWork/DI

- [ ] **Step 1: Review entity**
```csharp
public class Review : Entity<Guid>
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public Guid RestaurantId { get; set; }
    public short Rating { get; set; }
    public string? Comment { get; set; }
    public virtual User User { get; set; }
    public virtual Restaurant Restaurant { get; set; }
}
```

- [ ] **Step 2: FavoriteRestaurant entity**
```csharp
public class FavoriteRestaurant : Entity<Guid>
{
    public Guid UserId { get; set; }
    public Guid RestaurantId { get; set; }
    public virtual User User { get; set; }
    public virtual Restaurant Restaurant { get; set; }
}
```

- [ ] **Step 3: Add CourierStatusId to User entity** (default=1 Offline)
- [ ] **Step 4: Entity configs with unique constraints**
- [ ] **Step 5: Repos, DbSets, UnitOfWork, DI**
- [ ] **Step 6: Commit**

---

### Task 2: Implement Review Service

- [ ] **Step 1: Create IReviewService + ReviewManager**
  - CreateReview: validate order delivered + belongs to user + no existing review
  - Update Restaurant.Rating running average: `(oldRating * count + newRating) / (count + 1)`
  - GetRestaurantReviews: paginated, sorted by date desc
  - DeleteReview (admin)
- [ ] **Step 2: Create DTOs** (ReviewDto, CreateReviewRequestDto)
- [ ] **Step 3: Create controller endpoints**
- [ ] **Step 4: Commit**

---

### Task 3: Implement Search/Filter Enhancement

- [ ] **Step 1: Update RestaurantManager search query** (Dapper)
  Add WHERE clauses for: name LIKE, cuisineId, minRating, maxMinOrder, isOpen
  Add ORDER BY for sortBy parameter
  Add pagination (LIMIT/OFFSET)
- [ ] **Step 2: Update DTOs and controller** to accept filter params
- [ ] **Step 3: Commit**

---

### Task 4: Implement Image Upload

- [ ] **Step 1: Create upload endpoint** using existing S3/R2 adapter
  - Accept multipart/form-data
  - Validate file type (jpg, png, webp) and size (<5MB)
  - Upload to S3, save URL to ProductImage
- [ ] **Step 2: Create delete endpoint**
- [ ] **Step 3: Commit**

---

### Task 5: Implement Favorites Service

- [ ] **Step 1: Create IFavoriteService + FavoriteManager**
  - AddFavorite, RemoveFavorite, GetFavorites
- [ ] **Step 2: DTOs and controller**
- [ ] **Step 3: Update restaurant list response** to include isFavorite flag for authenticated users
- [ ] **Step 4: Commit**

---

### Task 6: Implement Reorder

- [ ] **Step 1: Add ReorderAsync to OrderManager/BasketManager**
  - Get order items → check each product still exists and is active
  - Add to basket with current prices
  - Return warnings for price changes or unavailable items
- [ ] **Step 2: Controller endpoint**
- [ ] **Step 3: Commit**

---

### Task 7: Implement Courier Earnings + Status

- [ ] **Step 1: Create ICourierEarningsService + CourierEarningsManager**
  - GetEarnings: aggregate ShipmentPrice by period (Dapper)
  - GetEarningsHistory: paginated delivery list with earnings
- [ ] **Step 2: Implement courier status toggle**
  - Update User.CourierStatusId
  - Auto-set OnDelivery when pickup confirmed
  - Auto-set Online when all deliveries done
- [ ] **Step 3: DTOs and controllers**
- [ ] **Step 4: Update seller courier list** to show online/offline status
- [ ] **Step 5: Commit**

---

### Task 8: Implement Analytics Services

- [ ] **Step 1: Create IAnalyticsService + AnalyticsManager** (Dapper raw SQL)
  - Seller analytics: order count, revenue, top products (GROUP BY with date ranges)
  - Admin analytics: platform-wide order/revenue trends, top restaurants
- [ ] **Step 2: DTOs** (AnalyticsSummaryDto, OrderTrendDto, TopProductDto, etc.)
- [ ] **Step 3: Seller analytics controller**
- [ ] **Step 4: Admin analytics controller**
- [ ] **Step 5: Commit**

---

### Task 9: Implement Delivery Timeout Check

- [ ] **Step 1: Add Hangfire job** CheckDeliveryTimeouts
  - Find OnTheWay orders where CreatedDate + MaxDeliveryTime < now
  - Send notification to customer + restaurant
  - Mark order with warning flag (or just log)
- [ ] **Step 2: Admin overdue orders endpoint**
- [ ] **Step 3: Register recurring job** (every 15 min)
- [ ] **Step 4: Commit**

---

## Chunk 2: Frontend — Customer App

### Task 10: Customer App — Reviews

- [ ] **Step 1: Add review API functions**
- [ ] **Step 2: Add "Değerlendir" button to OrderDetailScreen** (when status=Delivered)
- [ ] **Step 3: Create ReviewModal** (star rating + comment textarea)
- [ ] **Step 4: Show reviews on RestaurantDetailScreen** (list under menu)
- [ ] **Step 5: Commit**

---

### Task 11: Customer App — Search/Filters

- [ ] **Step 1: Update restaurant list API call** with filter params
- [ ] **Step 2: Create FilterModal** (cuisine picker, price slider, rating, sort)
- [ ] **Step 3: Add filter button + active filter badges** to HomeScreen
- [ ] **Step 4: Commit**

---

### Task 12: Customer App — Favorites + Reorder

- [ ] **Step 1: Add favorites API functions**
- [ ] **Step 2: Add heart icon to RestaurantCard and RestaurantDetailScreen**
- [ ] **Step 3: Create FavoritesScreen** in Profile tab
- [ ] **Step 4: Add reorder API function**
- [ ] **Step 5: Add "Tekrar Sipariş" button to OrderHistoryScreen/OrderDetailScreen**
- [ ] **Step 6: Commit**

---

## Chunk 3: Frontend — Courier App

### Task 13: Courier App — Earnings + Status

- [ ] **Step 1: Add earnings API functions to courierService.js**
- [ ] **Step 2: Create EarningsScreen** (daily/weekly/monthly toggle, total earnings card, delivery list)
- [ ] **Step 3: Add "Kazançlarım" tab to navigation** (or in Profile)
- [ ] **Step 4: Add online/offline toggle** to header/profile
- [ ] **Step 5: Commit**

---

## Chunk 4: Frontend — Seller Panel

### Task 14: Seller Analytics Dashboard

- [ ] **Step 1: Add analytics API functions to api.js**
- [ ] **Step 2: Create /app/analytics/page.js**
  - Order trend chart (line chart)
  - Revenue trend chart
  - Top products table
  - Summary cards (total orders, revenue, avg order, unique customers)
- [ ] **Step 3: Add menu image upload** to product edit form
- [ ] **Step 4: Add sidebar link** for Analytics
- [ ] **Step 5: Commit**

---

## Chunk 5: Frontend — Admin Panel

### Task 15: Admin Analytics + Reports

- [ ] **Step 1: Add analytics API functions**
- [ ] **Step 2: Update dashboard** with trend charts (orders, revenue)
- [ ] **Step 3: Create /app/analytics/page.js** (top restaurants, platform metrics)
- [ ] **Step 4: Add overdue orders section** to orders page
- [ ] **Step 5: Add review moderation** to a reviews page or inline in orders
- [ ] **Step 6: Commit**

---

### Task 16: Final Build Verification

- [ ] **Step 1: Backend build**
- [ ] **Step 2: Front-admin build**
- [ ] **Step 3: Front-seller build**
- [ ] **Step 4: Front-courier check**
- [ ] **Step 5: Front-app check**
