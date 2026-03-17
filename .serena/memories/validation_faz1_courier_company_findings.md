# Faz 1 Courier Company Data Model Validation — Findings

**Date:** 2026-03-16
**Analyst:** Analyst 1
**Status:** VALIDATED (3 findings noted)

## Summary
Spec (Section 2) and Plan (Chunks 1-2) are **generally aligned** on courier company entity relationships and API contracts. Data model is **architecturally sound** with minor clarifications needed.

## Key Validations

### ✅ Entity Relationships — CORRECT
- **CourierCompany** (firm) ↔ **CourierCompanyMember** (request/approval chain) ✓
- **CourierCompany** ↔ **RestaurantCourierCompany** (restaurant-firm agreements) ✓
- **Order** now supports both paths: `CourierId` (individual) OR `CourierCompanyId+PickedUpByCourierId` (firm) ✓
- **Mevcut RestaurantCourier** table reserved for individual couriers only (no breaking change) ✓

**Finding:** Entity relationships follow Clean Architecture pattern correctly. No issues found.

### ✅ API Contract Consistency — CORRECT
Spec Section 2.4 endpoints align with Plan's task list structure:
- Firm management (CourierCompanyAdmin only): register, get, update ✓
- Firm ↔ Courier: member search, request, list, delete ✓
- Courier-side: invites, accept/reject, leave ✓
- Restaurant ↔ Firm: add firm, list agreements, terminate ✓
- Firm-side: restaurant invites, approve/reject ✓
- Order pickup: pending list, confirm pickup ✓
- Admin: firm approval/suspend/ban ✓

**Finding:** All endpoint paths and methods are consistent. No conflicts detected.

### ⚠️ Status Enum Value Mismatch — CLARIFICATION NEEDED

**Spec defines:**
```
CourierCompanyStatusEnums:
  PendingApproval = 1
  Active = 2
  Suspended = 3
  Banned = 4
```

**Plan (Chunk 1, Task 1) defines:**
```
CourierCompanyStatusEnums:
  PendingApproval = 0
  Active = 1
  Suspended = 2
  Banned = 3
```

**Issue:** Spec uses 1-indexed, Plan uses 0-indexed. Same for `CourierCompanyMemberStatusEnums` and `RestaurantCourierCompanyStatusEnums`.

**Impact:** Moderate. Database queries would return different enum values, potentially breaking frontend parsing if mixed.

**Recommendation:** **Use Plan's 0-indexed version** (more consistent with existing UserStatusEnums in codebase where WaitingForActivation=0). Spec likely drafted before implementation phase alignment.

### ✅ Authorization Rules — COMPLETE
- **CourierCompanyAdmin** role (value 7) introduced correctly ✓
- Includes all Courier role permissions (inheritance pattern) ✓
- Firm owner cannot be claimed by another firm ✓
- Application-level constraint for single-firm membership enforced ✓

**Finding:** Authorization design is complete and secure. No gaps.

### ✅ Subscription Usage Logic — SOUND
- **SubscriptionUsage** tracks (SubscriptionId, Year, Month, OrderCount) ✓
- **Atomicity:** Dapper raw SQL with WHERE OrderCount < MaxOrders prevents race conditions ✓
- **Lazy creation:** GET-OR-CREATE pattern for new months (no cron job) ✓
- **Scoping:** Order limit **per restaurant**, not per seller account ✓

**Finding:** Subscription overage prevention is architecturally solid. Implementation in Plan (Chunk 2) is safe.

### ✅ iyzico Integration Mapping — CORRECT
- **Individual (1) → "PERSONAL"** type ✓
- **Company (2) → "LIMITED_OR_JOINT_STOCK_COMPANY"** type ✓
- **New IdentityNumber field** separates individual ID from TaxCode ✓
- **SellerDetail storage** (Key1=PaymentSubMerchantKey, Key2=Iyzico) matches mevcut pattern ✓

**Finding:** iyzico mapping is correct. Breaking change (PERSONAL type) is documented. Implementation plan handles retry/idempotency.

### ✅ Order Flow Hybridization — VALID
- **Pickup flow:** Both individual (`CourierId` → `PickedUpByCourierId`) and firm (`CourierCompanyId` → `PickedUpByCourierId`) now track who physically picked up ✓
- **Delivery:** Kurye can mark `DeliveredAt` (new trust model, spec-compliant) ✓
- **Backward compat:** Old orders unaffected (PickedUpByCourierId, PickedUpAt, DeliveredAt nullable) ✓

**Finding:** Order model update preserves existing data while enabling new workflow. Clean design.

---

## Edge Cases Verified

1. **Kurye switching firms:** Application-level check before member request prevents duplicate active memberships ✓
2. **Firm suspension:** Not explicitly handled in API endpoints (Spec 2.4). PM to clarify: should suspended firms remain visible in dropdown or not? 
   - **Spec says:** "Only Active firms shown" for restaurant agreements
   - **Plan says:** Implement suspension mechanism but no endpoint lock-out
   - **Action:** Implementation should treat Suspended ≈ Banned (block member requests, hide from dropdowns)

3. **Order history:** Multiple pickups/deliveries per order not prevented by schema (edge case: admin reassigns order mid-delivery)
   - **Status:** By design (Order.StatusId + PickedUpByCourierId + DeliveredAt combination validates state)

---

## Blocking Issues: NONE

✅ **Ready for implementation.** Plan can proceed with Chunk 1 (DB setup) and Chunk 2 (services).

---

## Recommendations for Backend Dev

1. **Enum Values:** Use Plan's 0-indexed version (lines 32-37 of Plan Task 1, Step 2-4)
2. **Firm Suspension:** Add to CourierCompanyManager — prevent member requests + remove from restaurant dropdowns when StatusId != Active
3. **Unique Constraint:** CourierCompanyConfiguration already has `HasIndex(c => c.OwnerUserId).IsUnique()` — good
4. **SubscriptionUsage Migration:** Set default MaxOrdersPerMonth = int.MaxValue for existing plans (backward-compatible)
5. **iyzico IdentityNumber:** Validate 11-digit TC number on Individual firm registration (add custom validator)

---

## Open Questions for PM

1. Should API reject member requests if firm is Suspended? (Spec unclear)
2. Firm downgrade scenario: can admin suspend a firm mid-month? What happens to orders already assigned?
3. Kurye location polling cadence: Spec says 30s client-side, 15s server-side. Rate limit as separate bucket confirmed ✓

