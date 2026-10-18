# Design Document: 4-Step Checkout Stepper with Integrated Cart Navigation

**Date:** 2026-10-18  
**Topic:** Checkout Stepper & Cart Integration  
**Status:** Approved  

---

## 1. Problem Statement & User Goal
Previously, the checkout page ([Checkout.cshtml](file:///e:/DAKKN/DAKKN.MVC/Views/Customer/Checkout.cshtml) and [Shop/Checkout.cshtml](file:///e:/DAKKN/DAKKN.MVC/Views/Shop/Checkout.cshtml)) used a 3-step stepper (Governorate -> Delivery Info -> Review). The cart items were locked in a read-only sidebar with no ability to edit quantities, delete items, or step back to the cart from Step 1. Users who wanted to adjust their cart items had to abandon the checkout flow and navigate back to `/Cart`.

**User Request:**
Enhance the user experience by enabling users to step back to their order items/cart directly within the stepper steps.

---

## 2. Proposed Architecture & Stepper Flow
We transform the checkout process into a seamless 4-step wizard:

```
[ Step 1: Cart Items ] <---> [ Step 2: Shipping ] <---> [ Step 3: Delivery Info ] <---> [ Step 4: Review & Confirm ]
```

### 2.1 Stepper Steps Detail

#### Step 1: Order Items in Cart (`سلة المشتريات والمنتجات`)
- **Components:**
  - Item listing cards with product image, product title (localized Arabic/English), unit price, subtotal, and quantity controls (`-` / `+`).
  - Item removal button (`delete` icon).
  - Empty cart notification if all items are removed (shows empty state illustration and "Browse Products" action).
- **Interactivity & AJAX:**
  - Invokes existing API: `PUT /api/v1/cart/update-quantity` with `{ productId, quantity }`.
  - Invokes existing API: `DELETE /api/v1/cart/remove/{productId}`.
  - Dynamically recalculates subtotal, shipping cost, and grand total in the sidebar and the review pane in real time without full page reload.
  - Synchronizes cart header badge count.
- **Navigation Controls:**
  - Left / Start: "Continue Shopping" link (navigates to `/Products` with back arrow).
  - Right / End: "Next: Shipping & Governorate" button.

#### Step 2: Shipping & Governorate (`المحافظة والشحن`)
- **Components:**
  - Governorate search bar with live filtering.
  - Governorate selectable card grid with shipping fees.
  - Selected governorate banner preview.
- **Navigation Controls:**
  - Left / Start: **"Previous: Order Items" (`السابق: سلة المشتريات`)** — steps back to Step 1.
  - Right / End: "Next: Delivery Info" button (validates that a governorate has been selected before proceeding).

#### Step 3: Recipient & Delivery Details (`بيانات المستلم والتوصيل`)
- **Components:**
  - Full Name input (required).
  - Egyptian phone number input (`+20` prefix, regex validation `^01[0125][0-9]{8}$`, required).
  - Shipping address textarea (required).
  - Order notes textarea (optional).
- **Navigation Controls:**
  - Left / Start: **"Previous: Shipping & Governorate" (`السابق: المحافظة والشحن`)** — steps back to Step 2.
  - Right / End: "Next: Review & Confirm" button (validates required fields before proceeding).

#### Step 4: Review & Confirm (`مراجعة وتأكيد الطلب`)
- **Components:**
  - **Review Card 1: Cart Items Summary** with an **"Edit" (`تعديل`)** button that takes the user directly back to **Step 1**.
  - **Review Card 2: Shipping & Governorate** with an **"Edit"** button taking the user back to **Step 2**.
  - **Review Card 3: Recipient & Address** with an **"Edit"** button taking the user back to **Step 3**.
  - **Review Card 4: Payment Method** (Cash On Delivery badge & security notice).
- **Navigation Controls:**
  - Left / Start: **"Previous: Delivery Info" (`السابق: بيانات التوصيل`)** — steps back to Step 3.
  - Right / End: "Place Order" (`تأكيد الطلب`) submit button with loading spinner and double-submit prevention.

---

## 3. UI/UX & Visual Direction
- **Design System:** Follows DAKKN's existing Tailwind CSS design tokens (`primary`, `surface`, `border`, `text-primary`, `text-secondary`, dark mode support).
- **Progress Bar Tracker:**
  - 4 progress nodes connected by an animated track.
  - Progress percentages:
    - Step 1: `0%`
    - Step 2: `33%`
    - Step 3: `66%`
    - Step 4: `100%`
  - Completed steps show a green checkmark badge and allow direct one-click navigation back to that step.
  - Active step scales smoothly (`scale-105`) with primary ring highlight.
- **Icons:**
  - Step 1: `shopping_bag`
  - Step 2: `local_shipping`
  - Step 3: `badge`
  - Step 4: `fact_check`

---

## 4. State Coverage Matrix
| State | Behavior |
|-------|----------|
| **Initial load with items** | Renders Step 1 with items active; progress bar at 0%. |
| **Initial load empty** | Shows full empty state with "Browse Products" button. |
| **Quantity decrease to 1** | Quantity button disabled or prevents decreasing below 1. |
| **Item removal** | Animates item out (`fade/scale`), sends `DELETE` request, recalculates totals, and updates sidebar. If last item removed, triggers empty state view. |
| **Step 1 -> Step 2 transition** | Checks cart has at least 1 item; moves smoothly to Step 2. |
| **Step 2 -> Step 3 transition** | Validates governorate is selected; if not, shakes container and highlights error. |
| **Step 3 -> Step 4 transition** | Validates Name, Phone format, Address; displays inline errors if invalid. |
| **Step-back from Step 2, 3, or 4** | Instant animated transition back to previous pane; smooth scroll to top of checkout container. |
| **Edit button clicked from Step 4** | Jumps directly to targeted step pane (Step 1, 2, or 3) and updates progress bar. |
| **Order Submission** | Disables submit button, shows spinner, submits `POST /Customer/PlaceOrder` or `POST /Shop/PlaceOrder`. |

---

## 5. Localization
New and updated localization keys in `DAKKN.Application/Localization/Resources/messages.{ar,en}.json`:
- `stepper_step1_cart_title`: "سلة المشتريات" / "Cart Items"
- `stepper_step1_cart_desc`: "مراجعة المنتجات والكميات" / "Review items & quantities"
- `stepper_step1_cart_heading`: "سلة المشتريات والمنتجات المطلوبة" / "Your Cart Items & Order Details"
- `stepper_step1_cart_subheading`: "يمكنك تعديل الكميات أو حذف المنتجات قبل المتابعة لبيانات الشحن" / "You can adjust quantities or remove items before proceeding to shipping"
- `stepper_step2_title`: "المحافظة والشحن" / "Governorate & Shipping"
- `stepper_step2_desc`: "تحديد المحافظة وحساب التكلفة" / "Select governorate & calculate shipping"
- `stepper_step2_heading`: "اختر محافظة الشحن والتوصيل" / "Select Shipping Governorate"
- `stepper_step2_subheading`: "اختر محافظتك من القائمة لعرض تكلفة الشحن بدقة وبشكل فوري" / "Choose your governorate to view exact shipping rates"
- `stepper_step3_title`: "بيانات التوصيل" / "Delivery Information"
- `stepper_step3_desc`: "اسم المستلم ورقم الهاتف والعنوان" / "Recipient name, phone, and address"
- `stepper_step3_heading`: "بيانات المستلم وعنوان التوصيل" / "Recipient & Delivery Details"
- `stepper_step3_subheading`: "يرجى التأكد من كتابة العنوان ورقم الهاتف بدقة لسهولة الوصول إليك" / "Please ensure address and phone number are accurate"
- `stepper_step4_title`: "مراجعة وتأكيد" / "Review & Confirm"
- `stepper_step4_desc`: "ملخص الطلب والدفع عند الاستلام" / "Order summary & Cash on Delivery"
- `stepper_step4_heading`: "مراجعة تفاصيل الطلب والتأكيد" / "Review Order Details & Confirm"
- `stepper_step4_subheading`: "راجع بيانات الشحن والتوصيل والمنتجات قبل تأكيد الطلب النهائي" / "Review shipping, recipient, and items before final order placement"
- `stepper_prev_cart`: "السابق: سلة المشتريات" / "Previous: Cart Items"
- `stepper_prev_shipping`: "السابق: المحافظة والشحن" / "Previous: Shipping"
- `stepper_prev_delivery`: "السابق: بيانات التوصيل" / "Previous: Delivery Info"

---

## 6. Files to Update
1. [DAKKN.Application/Localization/Resources/messages.ar.json](file:///e:/DAKKN/DAKKN.Application/Localization/Resources/messages.ar.json)
2. [DAKKN.Application/Localization/Resources/messages.en.json](file:///e:/DAKKN/DAKKN.Application/Localization/Resources/messages.en.json)
3. [DAKKN.MVC/Views/Customer/Checkout.cshtml](file:///e:/DAKKN/DAKKN.MVC/Views/Customer/Checkout.cshtml)
4. [DAKKN.MVC/Views/Shop/Checkout.cshtml](file:///e:/DAKKN/DAKKN.MVC/Views/Shop/Checkout.cshtml)
