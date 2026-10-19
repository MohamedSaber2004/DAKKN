# Two-Step Checkout Stepper Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use subagent-driven-development (recommended) or executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Transform the order checkout wizard in DAKKN from a 4-step process into a clean, modern 2-step stepper, integrating the governorate selection dropdown inside the delivery details step.

**Architecture:** Update Razor views `Shop/Checkout.cshtml` and `Customer/Checkout.cshtml` to feature a 2-step wizard. Step 1 handles cart item review with real-time AJAX price recalculation; Step 2 presents the recipient details alongside an integrated governorate `<select>` dropdown and direct order submission. Update localization keys in `messages.ar.json` and `messages.en.json`.

**Tech Stack:** ASP.NET Core MVC, Razor Views, Tailwind CSS, JavaScript (Vanilla fetch API), JSON Localization.

---

### Task 1: Update Localization Resources for 2-Step Flow

**Files:**
- Modify: `DAKKN.Application/Localization/Resources/messages.ar.json:2020-2048`
- Modify: `DAKKN.Application/Localization/Resources/messages.en.json:2220-2245`

- [x] **Step 1: Update Arabic messages file (`messages.ar.json`)**
Replace existing 4-step stepper messages with 2-step keys:
```json
  "stepper_step1_cart_title": "سلة المشتريات",
  "stepper_step1_cart_desc": "مراجعة المنتجات والكميات",
  "stepper_step1_cart_heading": "سلة المشتريات والمنتجات المطلوبة",
  "stepper_step1_cart_subheading": "يمكنك تعديل الكميات أو حذف المنتجات قبل المتابعة لبيانات الشحن",
  "stepper_step2_title": "بيانات التوصيل والشحن",
  "stepper_step2_desc": "البيانات والمحافظة وتأكيد الطلب",
  "stepper_step2_heading": "بيانات التوصيل والمستلم",
  "stepper_step2_subheading": "اختر المحافظة وأدخل بيانات المستلم لتأكيد شحن طلبك",
  "stepper_select_gov_ph": "اختر محافظة التوصيل...",
  "stepper_next": "التالي",
  "stepper_next_delivery": "المتابعة لبيانات التوصيل",
  "stepper_prev": "السابق",
  "stepper_prev_cart": "السابق: سلة المشتريات",
  "stepper_select_gov_err": "يرجى اختيار المحافظة أولاً للمتابعة",
  "stepper_fill_required_err": "يرجى ملء جميع الحقول المطلوبة بشكل صحيح",
  "stepper_phone_invalid_err": "يرجى إدخال رقم هاتف مصري صحيح يبدأ بـ 01 (11 رقماً)",
  "stepper_edit": "تعديل",
  "stepper_selected_gov_info": "التوصيل المتوقع خلال 24 - 48 ساعة عمل",
  "stepper_cod_badge": "الدفع نقداً عند الاستلام",
```

- [x] **Step 2: Update English messages file (`messages.en.json`)**
Mirror the updated keys in English:
```json
  "stepper_step1_cart_title": "Cart Items",
  "stepper_step1_cart_desc": "Review items & quantities",
  "stepper_step1_cart_heading": "Your Cart Items & Order Details",
  "stepper_step1_cart_subheading": "You can adjust quantities or remove items before proceeding to shipping",
  "stepper_step2_title": "Delivery & Shipping",
  "stepper_step2_desc": "Details, governorate & submit",
  "stepper_step2_heading": "Recipient & Delivery Details",
  "stepper_step2_subheading": "Select governorate and enter recipient details to confirm order",
  "stepper_select_gov_ph": "Select delivery governorate...",
  "stepper_next": "Next",
  "stepper_next_delivery": "Continue to Delivery",
  "stepper_prev": "Previous",
  "stepper_prev_cart": "Previous: Cart Items",
  "stepper_select_gov_err": "Please select a governorate to continue",
  "stepper_fill_required_err": "Please fill in all required fields correctly",
  "stepper_phone_invalid_err": "Please enter a valid Egyptian phone number (e.g. 01012345678)",
  "stepper_edit": "Edit",
  "stepper_selected_gov_info": "Estimated delivery within 24 - 48 business hours",
  "stepper_cod_badge": "Cash on Delivery",
```

- [x] **Step 3: Commit localization changes**
```bash
git add DAKKN.Application/Localization/Resources/messages.*.json
git commit -m "feat(i18n): update checkout stepper localization keys for 2-step flow"
```

---

### Task 2: Refactor Shop Checkout View to 2-Step Flow with Integrated Dropdown

**Files:**
- Modify: `DAKKN.MVC/Views/Shop/Checkout.cshtml`

- [x] **Step 1: Update Stepper Progress Bar to 2 steps (0% and 100%)**
Change the stepper header to render only 2 nodes (`step-nav-1` and `step-nav-2`).
`updateStepperNav(activeStep)` sets progress width to `0%` for Step 1 and `100%` for Step 2.

- [x] **Step 2: Keep Step 1 (Cart Items) and adjust navigation button**
Adjust the next button in `step-pane-1` to say `@Localizer["stepper_next_delivery"]` and call `validateStep1AndProceed()`, which smoothly moves to Step 2.

- [x] **Step 3: Update Step 2 to integrate Governorate `<select>` with delivery details**
In `step-pane-2`:
- Header: `@Localizer["stepper_step2_heading"]`
- Add governorate `<select id="shipping-gov-select" name="ShippingGovernorateId" required>` styled with custom Tailwind select arrow, displaying `@govName - @(gov.ShippingPrice > 0 ? $"{gov.ShippingPrice:N0} {Localizer[\"Currency_EGP\"]}" : Localizer["shipping_free"])`.
- Customer Name, Egyptian Phone number, Shipping Address, and Order Notes fields.
- Cash On Delivery trust card.
- Action buttons: "Previous: Cart Items" (`goToStep(1)`) and "Submit Order" (`btn-submit-order`).
- Remove obsolete `step-pane-3` and `step-pane-4`.

- [x] **Step 4: Update JavaScript logic in `Shop/Checkout.cshtml`**
- Adjust `goToStep(stepNumber)` for 2 steps (1 and 2).
- Add `shipping-gov-select` change listener to update `selectedGovId`, set `currentShippingPrice`, call `/api/v1/cart/shipping` PUT, and recalculate totals.
- Adjust `validateStep2(showErrors)` to validate both governorate selection and customer form inputs (Name, Phone, Address).
- Update form submit listener to ensure validation passes before showing spinner and disabling submit button.

- [x] **Step 5: Commit Shop Checkout View changes**
```bash
git add DAKKN.MVC/Views/Shop/Checkout.cshtml
git commit -m "feat(shop): refactor checkout to 2-step stepper with integrated governorate dropdown"
```

---

### Task 3: Refactor Customer Checkout View to 2-Step Flow with Integrated Dropdown

**Files:**
- Modify: `DAKKN.MVC/Views/Customer/Checkout.cshtml`

- [x] **Step 1: Mirror 2-step stepper markup in `Customer/Checkout.cshtml`**
Apply identical 2-step stepper progress bar, Step 1 cart review pane, and Step 2 integrated governorate dropdown + delivery form pane using Customer theme classes (`Layout = "_LayoutCustomer"`).

- [x] **Step 2: Mirror JavaScript logic in `Customer/Checkout.cshtml`**
Ensure identical client-side validation, governorate change handler, AJAX sync, and submit spinner behavior.

- [x] **Step 3: Commit Customer Checkout View changes**
```bash
git add DAKKN.MVC/Views/Customer/Checkout.cshtml
git commit -m "feat(customer): refactor checkout to 2-step stepper with integrated governorate dropdown"
```

---

### Task 4: Build Verification and Test Execution

**Files:**
- Test: Run .NET build and execute existing tests to ensure no regressions.

- [x] **Step 1: Build the solution**
Run: `dotnet build DAKKN.slnx`
Expected: Build succeeded with 0 errors.

- [x] **Step 2: Run tests**
Run: `dotnet test DAKKN.Tests/DAKKN.Tests.csproj --filter "FullyQualifiedName!~Performance"`
Expected: All tests pass.

- [x] **Step 3: Commit any final adjustments**
```bash
git commit -m "chore: verify build and tests pass for 2-step checkout"
```
