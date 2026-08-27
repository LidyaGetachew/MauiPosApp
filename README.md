# .NET MAUI POS Application — Cashier Checkout Module

A robust Point-of-Sale (POS) cashier checkout application built with **.NET MAUI**, **MVVM (CommunityToolkit.Mvvm)**, and local **SQLite** persistence.

---

## 🚀 Architectural Highlights & Features

- **Product Catalog Screen:** Browse products seeded from local SQLite database (Espresso, Croissant, Iced Tea, Blueberry Muffin, Bottled Water, Turkey Sandwich), filtered by category, and searchable by name or SKU.
- **Order / Cart Screen:** Real-time quantity adjustments, line item removals, empty-cart state handling, and live calculation of subtotal, **8.5% sales tax**, promo discounts, and grand totals.
- **Checkout Screen:** Payment method selection (Cash vs Card), cash tendered calculation with live change-due computation, SQLite order persistence, cart clearing, and receipt generation.
- **Order History Screen:** Complete record of past completed orders pulled from local SQLite database ordered newest first.
- **Legacy Code Bug Analysis & Modernization:** In-depth written analysis (`docs/LEGACY_CODE_ANALYSIS.md`) and modernized implementation (`FixedCartViewModel`) of the legacy Xamarin.Forms discount snippet.
- **Bonus Features:**
  - 📷 **Barcode Scan Simulation:** Mock barcode scan button that adds products by SKU.
  - 🏷️ **Discount & Promo Engine:** Supports percentage and flat dollar discounts with validation (can't discount below $0).
  - 🧾 **Receipt Text Generation & Export:** Generates clean, formatted text receipts for completed orders.
  - 🌐 **Offline-First Cloud Sync Design Document:** Complete architecture spec (`docs/OFFLINE_SYNC_DESIGN.md`).

---

## 📁 Project Directory Structure

```text
MauiPosApp/
├── src/
│   └── MauiPosApp/
│       ├── Models/              # Product, CartItem, Order, OrderItem SQLite Entities
│       ├── Services/            # CartService (Math Engine), DatabaseService (SQLite)
│       ├── ViewModels/          # ProductsViewModel, CartViewModel, CheckoutViewModel, OrderHistoryViewModel
│       ├── Views/               # ProductsPage, CartPage, CheckoutPage, OrderHistoryPage (XAML)
│       ├── Legacy/              # FixedCartViewModel (Modernized Xamarin Fix)
│       ├── AppShell.xaml        # TabBar Navigation Shell
│       └── MauiProgram.cs       # DI Container Service Registrations
├── tests/
│   └── MauiPosApp.Tests/        # 12 xUnit Unit Tests for Cart Math & Legacy Fixes
└── docs/
    ├── LEGACY_CODE_ANALYSIS.md  # Detailed Analysis of Xamarin Snippet Bugs
    └── OFFLINE_SYNC_DESIGN.md   # Offline-First Outbox Pattern Architecture Spec
```

---

## 🛠️ Setup & Execution Instructions

### **Prerequisites**
- .NET SDK 9.0 (or .NET 8 / 10)
- Any C# IDE (Visual Studio 2022, VS Code, or Rider)

### **Build & Run Commands**
1. **Clone the repository:**
   ```bash
   git clone https://github.com/LidyaGetachew/MauiPosApp.git
   cd MauiPosApp
   ```
2. **Build the solution:**
   ```bash
   dotnet build src/MauiPosApp/MauiPosApp.csproj
   ```
3. **Run the 100% Passing Unit Test Suite (12 Tests):**
   ```bash
   dotnet test
   ```
4. **Run the Application Engine:**
   ```bash
   dotnet run --project src/MauiPosApp/MauiPosApp.csproj
   ```

---

## 🧪 Unit Test Coverage (12/12 Passed)

The xUnit test suite (`tests/MauiPosApp.Tests/`) verifies all business rules:
1. `EmptyCart_SubtotalTaxAndTotal_ShouldBeZero`
2. `AddToCart_SingleProduct_CalculatesCorrectSubtotalAndTax`
3. `AddToCart_MultipleProducts_Calculates8Point5PercentSalesTax`
4. `UpdateQuantity_ZeroQuantity_RemovesItemFromCart`
5. `RemoveFromCart_ExistingProduct_RemovesItem`
6. `ApplyPercentageDiscount_ValidPercentage_AppliesCorrectDiscount`
7. `ApplyPercentageDiscount_InvalidPercentage_ReturnsFalseAndIgnores`
8. `ApplyFlatDiscount_ValidAmount_AppliesCorrectDiscount`
9. `DiscountAmount_ExceedingSubtotal_ClampsDiscountToSubtotal`
10. `ClearCart_ResetsItemsAndTotals`
11. `GetTotal_WithPercentageDiscount_FixesUnitBug` (Legacy Fix)
12. `ApplyDiscount_InvalidPercentage_ThrowsException` (Legacy Fix)

---

## 🐛 Legacy Xamarin.Forms Bug Summary (Section 4 & 7)

The legacy snippet suffered from four major issues:
1. **Unit Error in `DiscountPercent`:** Calculated `total - (total * DiscountPercent)`. Passing `15` resulted in `-1400%` total. Fixed by dividing by `100m` and clamping.
2. **`double` Precision Issue:** Used `double` for currency causing floating-point rounding errors. Fixed by using `decimal`.
3. **Broken MVVM Data Binding:** Used `List<CartItem>` instead of `ObservableCollection<CartItem>` and lacked `INotifyPropertyChanged` notifications.
4. **Missing Validation:** Allowed negative percentages or values exceeding 100%.

*Detailed documentation available in [`docs/LEGACY_CODE_ANALYSIS.md`](docs/LEGACY_CODE_ANALYSIS.md).*