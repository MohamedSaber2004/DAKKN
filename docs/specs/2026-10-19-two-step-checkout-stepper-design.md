# Two-Step Checkout Stepper with Integrated Governorate Dropdown Specification

## 1. Overview & Objective
Transform the multi-step checkout wizard in DAKKN (present in both `DAKKN.MVC/Views/Shop/Checkout.cshtml` and `DAKKN.MVC/Views/Customer/Checkout.cshtml`) from a 4-step process into an efficient **2-step** flow:
- **Step 1:** Order Items Review & Adjustment (سلة المشتريات والكميات).
- **Step 2:** Recipient & Shipping Information with Integrated Governorate Selection Dropdown (بيانات التوصيل والشحن) and final Order Submission.

---

## 2. Architecture & Flow

```
[ Step 1: Cart Items & Quantities ] <=======> [ Step 2: Delivery, Governorate Dropdown & Order Submit ]
```

### 2.1 Stepper Progress Bar (2 Nodes)
- Node 1: `shopping_bag` icon, Title: `سلة المشتريات`, Subtitle: `مراجعة المنتجات والكميات`.
- Node 2: `local_shipping` icon, Title: `بيانات التوصيل والشحن`, Subtitle: `البيانات والمحافظة وتأكيد الطلب`.
- Progress tracking:
  - Step 1: `0%`
  - Step 2: `100%`
- Clicking Node 1 while on Step 2 immediately returns the user to Step 1.

### 2.2 Step 1: Order Items & Quantities (`step-pane-1`)
- **Components:**
  - Cart item cards displaying product image, localized name, unit price, and item total.
  - Interactive stepper buttons (`-` / `+`) and item delete button.
  - AJAX updates: invokes existing `/api/v1/cart/update-quantity` and `/api/v1/cart/remove/{productId}`.
  - Dynamic recalculation of subtotal, total items, and grand total in the sidebar.
  - Empty cart fallback state if all items are removed.
- **Actions:**
  - Back / Cancel: "Continue Shopping" link to `/Products`.
  - Next: "المتابعة لبيانات التوصيل والشحن" button with forward icon.
- **Validation:**
  - Ensures at least one item remains in the cart before allowing transition to Step 2.

### 2.3 Step 2: Delivery Info, Governorate Dropdown & Place Order (`step-pane-2`)
- **Components:**
  1. **Governorate Selection Dropdown (`<select id="shipping-gov-select" name="ShippingGovernorateId">`):**
     - Modern styled dropdown with Egyptian flag/pin iconography.
     - Each option formatted as: `[Governorate Name] — [Shipping Price EGP / Free]`.
     - `change` event listener immediately updates `selectedGovId`, sets `currentShippingPrice`, calls backend `/api/v1/cart/shipping` PUT in background, and recalculates the sticky sidebar total.
  2. **Full Name (`CustomerName`):** Text input, required.
  3. **Phone Number (`CustomerPhone`):** Egyptian phone input with `+20` prefix, required (regex `^01[0125][0-9]{8}$`).
  4. **Detailed Address (`ShippingAddress`):** Textarea, required (min length 5).
  5. **Order Notes (`Notes`):** Textarea, optional.
  6. **Payment Trust Banner:** Cash On Delivery (COD) badge with verified secure badge.
- **Actions:**
  - Previous: "السابق: سلة المشتريات" button (returns to Step 1).
  - Submit: "تأكيد الطلب" (`btn-submit-order`) with lock icon, double-submit prevention, and loading spinner.
- **Validation:**
  - Validates governorate selection, name, phone, and address before allowing form submission.
  - Shakes container and displays inline error messages if validation fails.

### 2.4 Sticky Order Summary Sidebar
- Remains visible across both steps.
- Displays item breakdown, subtotal, real-time shipping fee (updated when governorate dropdown changes), and total amount.
- Quick edit link in sidebar header redirects back to Step 1.

---

## 3. Localization Keys
Updates in `messages.ar.json` and `messages.en.json`:
- `stepper_step1_cart_title`: "سلة المشتريات" / "Cart Items"
- `stepper_step1_cart_desc`: "مراجعة المنتجات والكميات" / "Review items & quantities"
- `stepper_step2_title`: "بيانات التوصيل والشحن" / "Delivery & Shipping"
- `stepper_step2_desc`: "البيانات والمحافظة وتأكيد الطلب" / "Details, governorate & submit"
- `stepper_step2_heading`: "بيانات التوصيل والمستلم" / "Recipient & Delivery Details"
- `stepper_step2_subheading`: "اختر المحافظة وأدخل بيانات المستلم لتأكيد شحن طلبك" / "Select governorate and enter recipient details to confirm order"
- `stepper_select_gov_ph`: "اختر محافظة التوصيل..." / "Select delivery governorate..."
- `stepper_prev_cart`: "السابق: سلة المشتريات" / "Previous: Cart Items"
- `stepper_next_delivery`: "المتابعة لبيانات التوصيل" / "Continue to Delivery"

---

## 4. Target Files
1. `DAKKN.Application/Localization/Resources/messages.ar.json`
2. `DAKKN.Application/Localization/Resources/messages.en.json`
3. `DAKKN.MVC/Views/Shop/Checkout.cshtml`
4. `DAKKN.MVC/Views/Customer/Checkout.cshtml`
