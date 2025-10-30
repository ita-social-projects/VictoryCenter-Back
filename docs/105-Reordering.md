# Research on Reordering Implementation Approaches

This document describes and compares various approaches for implementing the element reordering functionality in the system.

## Current Implementation

The current version uses a priority swapping approach. To avoid violating the unique index constraint during the swap, elements are temporarily assigned negative priority values.

### Testing Methodology

The following scenario was used to evaluate performance:
1.  **Dataset:** 100 elements in the database, divided into 2 groups.
2.  **Load:** 60 consecutive reordering requests, each containing between 2 and 50 IDs.
3.  **Conditions:** A random delay was introduced between requests.

---

## Researched approaches

### 1) Priority field **with** unique constraint

**Entity interface**
```csharp
public interface IOrderableEntity
{
    long Priority { get; set; }
}
```

**Average measured latency:** ~95 ms

**Pros**
- Simple to reason about: sorting by `Priority` gives canonical order.
- Easy debugging — priority value directly represents order.
- DB-level uniqueness enforces data integrity (no duplicate priorities).

**Cons**
- Requires an additional update step (temporary priorities) during swaps — extra writes and extra `SaveChangesAsync()`.
- Swapping is relatively slow due to the two-step save.
- Concurrency and transaction handling become more complex.
- Intermediate states may be visible unless the operation is run inside a transaction.

---

### 2) Priority field **without** unique constraint

**Entity interface**
```csharp
public interface IOrderableEntity
{
    long Priority { get; set; }
}
```

**Average measured latency:** ~75 ms

**Pros**
- Straightforward sort by priority.
- Faster swaps (no temporary values, fewer DB writes).
- Simpler implementation and lower latency.

**Cons**
- No DB-level guarantee of unique priorities.
- Requires application-level consistency checks and compensations.
- Risk of duplicates if concurrency is not handled carefully.

---

### 3) Linked-list approach

**Entity interface**
```csharp
public interface IOrderableEntity
{
    long NextElementId { get; set; }
}
```

**Pros**
- O(1) updates for small local swaps (few pointer changes).
- Efficient for frequent move-up/move-down operations.

**Cons**
- Significant data model and test changes required.
- Harder to debug (pointer chasing) and more error-prone.
- Full-list reads are expensive (traversal required).
- Hard to support random-access position queries efficiently without extra metadata.

---

## Alternative approaches

### 4) Gap-based priorities (spaced / fractional priorities)

**Entity interface**
```csharp
public interface IOrderableEntity
{
    decimal Priority { get; set; }  // or double for fractional
}
```

Initialize priorities with large gaps (e.g., 1000, 2000, 3000).
Insert between items by picking a midpoint (e.g., (1000 + 2000) / 2 = 1500).
When gaps run out, perform a renumber (compaction) pass.

**Pros**
- Fast reorders and inserts in common cases.
- Reduces frequency of global renumber operations.
- No unique constraint issues during normal operations.

**Cons**
- Requires periodic maintenance (`RenumberPriorityAsync`) when gaps are exhausted.
- Needs careful handling of numeric precision if using floating point.
- Edge cases require rescaling or renumbering.
- Complexity increases with maintenance logic.

---

### 5) DB-level batch updates

**Entity interface**
```csharp
public interface IOrderableEntity
{
    long OrderIndex { get; set; }
}
```

Uses SQL CASE statements for batch updates:
```sql
UPDATE Products 
SET OrderIndex = CASE 
WHEN Id = 3 THEN 1 
WHEN Id = 2 THEN 2 
WHEN Id = 1 THEN 3 
WHEN Id = 4 THEN 4 
END 
WHERE Id IN (3, 2, 1, 4);
```

**Pros**
- Single atomic SQL operation — fastest approach.
- No temporary values or intermediate states.
- Excellent performance for bulk reordering.
- Natural transaction boundaries.

**Cons**
- Requires raw SQL or advanced ORM features.
- Less portable across different database systems.
- Harder to unit test and mock compared to entity-based approaches.
- Dynamic SQL generation complexity for variable-length reorder lists.