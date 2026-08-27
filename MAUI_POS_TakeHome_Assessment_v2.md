# **TAKE-HOME TECHNICAL ASSESSMENT** 

**.NET MAUI POS Application — Cashier Checkout Module** 

**Time allotted:** 24 hours from the time you receive this document 

**Role:** .NET MAUI Developer — new POS product build + legacy Xamarin maintenance **Format:** Individual, open-book, take-home 

## **1. Context** 

We're hiring a .NET MAUI developer to build a new Point-of-Sale (POS) product and to maintain/modernize an existing Xamarin.Forms codebase alongside it. This assessment is designed to reflect that reality: most of it is a fresh MAUI build, and one small section asks you to work with legacy code, the way you would on the job. 

We care far more about how you think, structure, and test your code than about a polished UI. A working, wellarchitected app with a plain UI will always score higher than a beautiful UI sitting on fragile logic. 

## **2. The Scenario** 

A small café chain needs a lightweight cashier-facing checkout screen for their tablets. Build a minimal but correct POS module in .NET MAUI that lets a cashier browse products, build an order, and complete checkout. 

## **3. Core Requirements (must-have)** 

### **3.1 Product catalog screen** 

- Display a list/grid of products (name, price, category) seeded from local data — see the sample catalog in Section 6. 

- Support a simple search or category filter. 

### **3.2 Order / cart screen** 

- Add a product to the current order by tapping it; support increasing/decreasing quantity and removing a line item. 

- **Calculate correctly:** subtotal, an 8.5% sales tax, and a grand total. Recalculate live as the cart changes. 

- Handle the empty-cart state without crashing or showing garbage values. 

### **3.3 Checkout screen** 

- Let the cashier choose a payment method (Cash or Card is enough — no real payment integration needed, this can be a mock/stub). 

- On confirm, persist the completed order (with its line items) to local storage and clear the cart. 

- Show a simple order confirmation / receipt view with the order number, items, and totals. 

### **3.4 Order history screen** 

- A simple list of past completed orders, pulled from local storage, newest first. 

### **3.5 Technical requirements** 

- **Framework:** .NET MAUI (current stable release). Do not use Xamarin.Forms for the new build. 

- **Architecture:** MVVM. We recommend CommunityToolkit.Mvvm, but any clean MVVM approach is fine. 

- **Persistence:** A real local database (SQLite via sqlite-net-pcl, or EF Core with the SQLite provider). No hardcoded in-memory-only data for orders. 

- **Platforms:** Must build and run on at least Android or Windows. Bonus if it also runs on the other. 

- **Tests:** At least one unit test project covering the cart/total calculation logic (the highest-risk business logic in a POS app). 

- **Source control:** A git repository with incremental commits that show your process, not a single final commit. 

## **4. Legacy Code Task (required, small)** 

Included with this assessment is a short Xamarin.Forms snippet (Section 7) from a fictional legacy POS screen with a subtle bug and some outdated patterns. 

1. Identify and explain the bug(s) in a short written note (a few sentences is enough). 

2. Port the snippet to .NET MAUI, fixing the bug(s) as part of the port. 

_This section exists specifically to see how you read and reason about existing code, not just greenfield code — please don't skip it even if it feels small._ 

## **5. Bonus Features (optional — pick as many or as few as you like)** 

These are genuinely optional. A complete, well-tested core app with zero bonus items will always outscore a shaky app with three half-finished bonus items. Pick bonus work only if your core build is solid and you have time left. 

- **Barcode scan simulation —** a mock "scan" button that adds a product by SKU, structured so it could be swapped for a real camera scanner later. 

- **Discount / promo codes —** apply a percentage or flat discount to an order with basic validation (e.g. can't discount below $0). 

- **Role-based access —** a simple Cashier vs. Manager login where only Manager can void a completed order or apply a discount over a threshold. 

- **Receipt export —** generate a PDF or shareable text receipt for a completed order. 

- **Dark/light theme support —** respects system theme or offers a toggle. 

- **Second platform —** confirmed working build on both Android and Windows (or iOS/macOS if you have the tooling). 

- **CI pipeline —** a basic GitHub Actions workflow that restores, builds, and runs your unit tests on push. 

- **Offline-first note —** a short written explanation (design doc, not code) of how you'd extend this app to sync orders to a cloud backend when connectivity returns. 

## **6. Sample Seed Data** 

Use this (or something equivalent/expanded) to seed the product catalog on first launch: 

|**SKU**|**Name**|**Price**|**Category**|
|---|---|---|---|
|SKU-001|Espresso|$2.50|Beverages|
|SKU-002|Croissant|$3.25|Bakery|
|SKU-003|Iced Tea|$3.00|Beverages|



|**SKU**|**Name**|**Price**|**Category**|
|---|---|---|---|
|SKU-004|Blueberry Mufn|$3.75|Bakery|
|SKU-005|Botled Water|$1.50|Beverages|
|SKU-006|Turkey Sandwich|$6.95|Food|



## **7. Legacy Xamarin.Forms Snippet** 

From a fictional "ApplyDiscount" feature on an existing Xamarin.Forms POS screen: 

public class CartViewModel : INotifyPropertyChanged { public List<CartItem> Items { get; set; } = new List<CartItem>(); public double DiscountPercent { get; set; } public double GetTotal() { double total = 0; foreach (var item in Items) total += item.Price * item.Quantity; total = total - (total * DiscountPercent); return total; } public void ApplyDiscount(double percent) { DiscountPercent = percent; } } 

_(Hint: think about what happens with the units of DiscountPercent, what happens if ApplyDiscount is called more than once, and what MVVM issue this class has as written.)_ 

## **8. What to Submit** 

1. A link to a git repository (private GitHub/GitLab repo with abel@minervadigital.se added as a collaborator, or a zip of the repo including .git history). 

2. **A runnable build:** if you target Android, include an installable APK (a Release build is preferred; debug-signed is fine) as a GitHub Release asset or a download link — please don’t commit the binary into the repo. If you target Windows only, a zipped self-contained build is fine instead. State the minimum OS version it was tested against. 

3. A README with: setup/build instructions, which platform(s) you tested on, any assumptions you made, and what you'd do next with more time. 

4. Your short written note on the legacy bug (Section 4). 

5. Optional but appreciated: a 2–5 minute screen recording walking through the app. 

## **9. Evaluation Rubric** 

|**Area**|**What we're looking for**|**Weight**|
|---|---|---|
|**Correctness & business logic**|Cart math, tax, discounts, and totals are accurate in every<br>edge case (empty cart, qty 0, negatve discount, etc.)|25%|
|**Architecture & MVVM**|Clean separaton of View / ViewModel / Model / Services.<br>No business logic in code-behind. Dependency injecton used<br>sensibly.|20%|
|**MAUI/XAML craf**|Responsive layout, sensible use of Shell navigaton, data<br>binding done correctly (not brute-forced), reasonable UI<br>polish.|15%|
|**Data persistence**|SQLite (or equivalent) correctly models products, orders, and<br>order line items; data survives an app restart.|15%|
|**Code quality & git hygiene**|Readable code, meaningful naming, incremental commits<br>with clear messages (not one giant commit at the deadline).|10%|
|**Error handling & edge cases**|Graceful handling of bad input, empty states, and failures —<br>no unhandled crashes during the demo walkthrough.|10%|
|**Bonus features atempted**|Extra points for any bonus item completed well — see bonus<br>secton.|+5% each,<br>capped at<br>+15%|



## **10. Ground Rules** 

- Open book: official docs, Stack Overflow, and AI coding assistants are all fine to use. 

- **Disclose AI use:** if you used an AI assistant significantly, say so briefly in the README. This isn't disqualifying — we just want to talk about your process in the follow-up interview, and you should be able to explain every line you submit. 

- Do not use another person to complete this on your behalf. 

- If you get stuck on something non-essential, leave a comment explaining what you'd do with more time and move on — partial, well-reasoned progress beats a blocked submission. 

- If 24 hours genuinely isn't enough due to a scheduling conflict, tell us before the deadline — we're generally flexible on timing, not on effort. 

## **11. Questions** 

If anything in this brief is ambiguous, make a reasonable assumption, document it in your README, and keep moving — how you handle ambiguity is part of what we're evaluating. If something seems genuinely broken or contradictory, reach out to us directly. 

_Good luck — we're looking forward to seeing how you think._ 

