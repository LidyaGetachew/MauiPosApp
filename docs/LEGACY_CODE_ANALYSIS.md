# Legacy Xamarin.Forms Code Bug Analysis (Section 4 & Section 7)

## 1. Identified Bugs & Outdated Patterns

### **Bug 1: Unit Discrepancy in `DiscountPercent` Calculation**
In `GetTotal()`, the math is executed as:
```csharp
total = total - (total * DiscountPercent);
```
If a developer or user passes `15` to represent a 15% discount (`ApplyDiscount(15)`), the calculation yields `total - (total * 15) = total * (-14)`, which results in a massive negative total (-1400%). 
**Fix:** The percentage must be converted to a fraction by dividing by 100 (`DiscountPercent / 100.0`), or stored as a fractional rate (e.g. `0.15`), and validated between `0` and `100` (or `0.0` and `1.0`).

---

### **Bug 2: Floating-Point Math for Currency**
The legacy class uses `double` for item prices, discount percent, and total. Floating-point arithmetic (`double`/`float`) causes binary rounding errors (e.g., `$0.10 + $0.20 = $0.30000000000000004`).
**Fix:** Use `decimal` for all financial figures, prices, tax calculations, and currency totals.

---

### **Bug 3: Broken MVVM Data Binding & Collection Notifications**
1. `Items` is defined as `List<CartItem>` instead of `ObservableCollection<CartItem>`. When items are added or removed from `Items`, no collection change notification is raised, so XAML UI bindings (`CollectionView`/`ListView`) will not update.
2. Neither `Items` nor `DiscountPercent` call `OnPropertyChanged()`, meaning property changes will not reflect in the UI.
3. `GetTotal()` is a method rather than a property or bound expression. XAML data binding cannot bind directly to methods like `GetTotal()`.

---

### **Bug 4: Missing Boundary & State Validation**
1. `ApplyDiscount(double percent)` allows negative percentages or values exceeding 100% (e.g., `ApplyDiscount(-50)` increases price by 5000%, `ApplyDiscount(150)` results in negative balance).
2. If `ApplyDiscount` is called repeatedly or with invalid values, there is no validation or reset logic.

---

## 2. Ported & Fixed .NET MAUI Solution (`FixedCartViewModel`)

The ported class in `src/MauiPosApp/Legacy/FixedCartViewModel.cs` uses `CommunityToolkit.Mvvm`:
- Replaces `double` with `decimal`.
- Replaces `List<CartItem>` with `ObservableCollection<CartItem>`.
- Converts percentage `DiscountPercent` to fraction (`/ 100m`) and clamps range between `0` and `100`.
- Exposes `Total` as a bound property that raises property change notifications whenever `Items` or `DiscountPercent` change.
