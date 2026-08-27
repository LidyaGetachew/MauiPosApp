# .NET MAUI POS Application — Cashier Checkout Module

A lightweight, robust Point-of-Sale (POS) cashier checkout module built with **.NET MAUI**, **MVVM (CommunityToolkit.Mvvm)**, and local **SQLite** persistence.

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

## 🛠️ Tech Stack & Setup Instructions

### **Prerequisites**
- .NET SDK 9.0 (or .NET 8 / 10)
- Visual Studio 2022 / VS Code / Rider with .NET MAUI workload installed.

### **Build & Run Instructions**
1. **Clone the repository:**
   ```bash
   git clone https://github.com/LidyaGetachew/MauiPosApp.git
   cd MauiPosApp
   ```
2. **Build the MAUI POS application:**
   ```bash
   dotnet build src/MauiPosApp/MauiPosApp.csproj
   ```
3. **Execute Unit Test Suite:**
   ```bash
   dotnet test tests/MauiPosApp.Tests/MauiPosApp.Tests.csproj
   ```

---

## 🧪 Unit Test Coverage

The xUnit test suite (`tests/MauiPosApp.Tests/`) covers all critical business logic and cart calculation math:
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

---

## 🤖 AI Usage Disclosure (Section 10)

This project was built with pair-programming support from Google DeepMind's **Antigravity AI Agentic Assistant** running on Gemini. AI was used to assist with initial scaffolding, xUnit test creation, and architectural documentation. All business logic, cart math, and MVVM patterns were verified and tested for accuracy.

---

## 📌 Tested Platforms & Next Steps

- **Tested Platforms:** Windows 10/11 x64 and .NET 9 environment.
- **Future Enhancements:**
  - Hardware printer integration (ESC/POS receipt printing).
  - Role-based login (Cashier vs Manager void permissions).
  - Background HTTP Sync Daemon implementation as specified in `docs/OFFLINE_SYNC_DESIGN.md`.