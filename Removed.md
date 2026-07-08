# Removed Code — Payment-Related Features

This document records all payment-related code that was removed from the GreenCarWash project.

---

## Deleted Files

### `Enums/PaymentMethod.cs` (Entire file deleted)

```csharp
namespace GreenCarWash.Api.Enums
{
    public enum PaymentMethod
    {
        Cash = 0,
        Card = 1,
        UPI = 2
    }
}
```

### `Enums/PaymentStatus.cs` (Entire file deleted)

```csharp
namespace GreenCarWash.Api.Enums
{
    public enum PaymentStatus
    {
        Pending = 0,
        Completed = 1,
        Failed = 2
    }
}
```

---

## Modified Files

### `Models/Order.cs`

Removed the following properties:

```csharp
public PaymentMethod? PaymentMethod{get;set;} = Enums.PaymentMethod.Cash;
public PaymentStatus? PaymentStatus{get;set;} = Enums.PaymentStatus.Pending;
public DateTime? PaymentTime{get;set;}
```

---

### `DTO/ResponseDtos/OrderResponseDto.cs`

Removed the following commented-out properties:

```csharp
//public string PaymentMethod{get;set;} = string.Empty;
//public string PaymentStatus{get;set;} = string.Empty;
//public DateTime CreatedAt{get;set;}
```

---

### `Services/AdminService.cs` — `GetOrdersAsync()`

Removed DTO mapping lines:

```csharp
PaymentMethod = o.PaymentMethod?.ToString() ?? "",
PaymentStatus = o.PaymentMethod?.ToString() ?? "",
CreatedAt = o.CreatedAt
```

---

### `Services/BookingService.cs`

#### `BookAsync()` — Return DTO mapping

Removed:

```csharp
PaymentMethod = order.PaymentMethod?.ToString() ?? "",
PaymentStatus = order.PaymentStatus?.ToString() ?? "",
CreatedAt = order.CreatedAt
```

#### `MapToOrderResponseDto()` — Private helper method

Removed:

```csharp
PaymentMethod = o.PaymentMethod?.ToString() ?? "",
PaymentStatus = o.PaymentStatus?.ToString() ?? "",
CreatedAt = o.CreatedAt
```

---

### `Services/WasherService.cs`

#### `CompleteOrderAsync()` — Payment status logic

Removed:

```csharp
var order = await _orderRepo.GetByIdAsync(orderId);
if (order != null)
{
    order.PaymentStatus = PaymentStatus.Completed;
    order.PaymentTime = DateTime.UtcNow;
    await _orderRepo.UpdateAsync(order);
}
```

#### `GetMyOrdersAsync()` — DTO mapping

Removed:

```csharp
PaymentMethod = o.PaymentMethod?.ToString() ?? "",
PaymentStatus = o.PaymentStatus?.ToString() ?? "",
CreatedAt = o.CreatedAt
```

---

## Removed — AddOnItemDto & Placeholder Add-on Logic

### `DTO/RequestDtos/AddOnItemDto.cs` (Entire file deleted)

```csharp
namespace GreenCarWash.Api.DTOs.RequestDtos
{
    public class AddOnItemDto
    {
        [Required]
        public int AddOnId{get;set;}
    }
}
```

**Reason:** This DTO was unused by any request. It was only referenced in `BookingService` for deserialization, where it caused a **bug** — `AddOnsJson` is serialized as `List<int>` (e.g. `[1, 3]`) but was deserialized as `List<AddOnItemDto>` (expects `[{"addOnId": 1}]`), causing all add-on IDs to silently return as `0`.

---

### `Services/BookingService.cs` — Changes (not removed, refactored)

#### `MapToOrderResponseDto()` — Removed placeholder add-on block

The following code was removed from the mapper. It created empty placeholder DTOs with no name and price = 0, which were then overwritten in the caller:

```csharp
var addOnLines = new List<OrderAddOnDto>();
try
{
    var items = JsonSerializer.Deserialize<List<AddOnItemDto>>(o.AddOnsJson ?? "[]");
    if(items != null)
    {
        foreach(var item in items)
        {
            addOnLines.Add(new OrderAddOnDto
            {
                AddOnName = "",
                Price = 0
            });
        }
    }
}
catch
{}
```

The mapper now returns an empty `AddOn` list by default (from the DTO's initializer).

#### `GetOrderByIdAsync()` — Fixed deserialization bug

Changed from (buggy):

```csharp
var addOnIds = JsonSerializer.Deserialize<List<AddOnItemDto>>(order.AddOnsJson ?? "[]")
    ?.Select(a => a.AddOnId).ToList() ?? new List<int>();
```

Changed to (fixed):

```csharp
var addOnIds = JsonSerializer.Deserialize<List<int>>(order.AddOnsJson ?? "[]") ?? new List<int>();
```

Add-ons are now populated directly in the caller using a clean LINQ select:

```csharp
responseDto.AddOn = addOnsData.Select(a => new OrderAddOnDto
{
    AddOnName = a.Name,
    Price = a.Price
}).ToList();
```

---

## Removed — Order CreatedAt

### `Models/Order.cs`

Removed property:

```csharp
public DateTime CreatedAt{get;set;} = DateTime.UtcNow;
```

### `Services/BookingService.cs` — `BookAsync()`

Removed assignment:

```csharp
CreatedAt = DateTime.UtcNow
```

**Reason:** `CreatedAt` was not exposed in any API response (it was previously removed from `OrderResponseDto`). It only existed in the database for auditing purposes, which was not needed.

**Database:** Migration `RemoveOrderCreatedAt` applied — dropped the `CreatedAt` column from the `Orders` table.

---

> **Note:** Migration files (`Migrations/`) are left untouched as they represent historical database schema changes.
