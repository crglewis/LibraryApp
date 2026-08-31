# Handoff: Library Web App UI Theme

## Overview
A calm, scholarly UI theme for a public-library web app: catalog browse/search, book detail with reviews, reading lists/favorites, and a librarian admin dashboard.

## About the Design Files
The bundled `Library Theme.dc.html` is a **design reference built in HTML** (a Design Component prototype), not production code to copy directly. It demonstrates layout, styling, states, and interactions using inline styles and a lightweight custom templating runtime specific to this design tool. The task is to **recreate this design in the target codebase's existing environment** (React, Vue, etc., or the best-fit framework if none exists yet) using its own component patterns, state management, and styling approach — port the visual design and behavior, not the file's markup/runtime mechanics.

## Fidelity
**High-fidelity.** Colors, typography, spacing, and interaction states shown are final — recreate pixel-close using the target codebase's own component/styling system.

## Screens / Views

### 1. Catalog (Browse & Search)
- **Purpose**: Search and browse books by genre/shelf.
- **Layout**: Sticky top nav (logo left, 4-tab nav right) → page container (max-width 1200px, centered, padding 40px) → title/subtitle → full-width search input → wrapping row of genre filter chips → results count → responsive grid (`repeat(auto-fill, minmax(200px,1fr))`, gap 24px balanced density).
- **Components**:
  - Search input: full width, padding 13px 16px, 1px border, 8px radius, 15px Inter.
  - Genre chip: pill, padding 8px 16px, 20px radius; active = filled brick bg + white text; inactive = transparent bg, 1px border, muted text.
  - Book card: card bg, 1px border, 10px radius; striped placeholder cover (200px tall, diagonal repeating-linear-gradient tint, "COVER" monospace label centered); body padding 16px with title (Lora 600 16px, 2-line clamp), author (13px muted), star rating + review count row, heart favorite toggle (♡/♥, click stops row navigation), availability text (green "Available" or muted "On loan · back {date}").
  - Clicking a card navigates to Book Detail for that book.

### 2. Book Detail
- **Purpose**: View full book info, place hold/check out, read and post reviews.
- **Layout**: "← Back to Catalog" text link → 2-column grid (280px cover | flexible info), gap 40px → full-width Reviews section below (max-width 640px).
- **Components**:
  - Cover: 280×400px striped placeholder, 10px radius.
  - Genre pill badge (12px, tinted olive), title (Lora 600 36px), author (16px muted), star+review-count row, availability pill + action button ("Check Out" if available, "Place Hold" otherwise) — button: brick bg, white text, 8px radius, 11px/22px padding.
  - Description paragraph (15px, line-height 1.65, max-width 560px).
  - Meta grid: Publisher / Published / Pages / ISBN as label-value pairs.
  - Reviews list: cards with reviewer name + date row, star rating, review text.
  - "Write a review" card: 5-star clickable picker, textarea, "Post Review" button (same brick style as action button); submitting appends a review to the list and clears the form.

### 3. My Lists (Reading Lists / Favorites)
- **Purpose**: View saved reading lists and favorited books.
- **Layout**: Title → grid of list cards (auto-fill, minmax 220px) + a dashed "+ Create new list" card → "Favorites" section heading → same book-card grid as Catalog, filtered to favorited books; empty state shows italic muted helper text.
- **Components**: List card shows a 3-cover overlapping stack (44×60px each, -14px overlap, tinted placeholders), list name (Lora 600 17px), book count (13px muted).

### 4. Librarian Dashboard (Admin)
- **Purpose**: Operational overview for library staff.
- **Layout**: Title → 4-column stat card row → 2-column layout (Recent Activity table 2fr | Inventory Alerts list 1fr), gap 24px.
- **Components**:
  - Stat card: big number (Lora 700 28px, brick color), uppercase label (12px muted).
  - Activity table: Patron / Item / Date / Status columns; status shown as colored pill (Checked Out = neutral gray, Returned = green, Overdue = brick/red).
  - Alert card: left 3px brick accent border, title (14px bold) + note (13px muted).

## Interactions & Behavior
- Top nav: 4 tabs (Catalog, Book Detail, My Lists, Dashboard) — click switches the active screen; active tab gets a 2px brick bottom border + bold dark text.
- Genre chips: click sets active filter (single-select, "All" default); filters the book grid live.
- Search input: live text filter against title and author (case-insensitive substring).
- Book card click: navigates to Book Detail for that book and sets it as selected.
- Heart icon click: toggles favorite state for that book; uses `stopPropagation` so it doesn't also trigger card navigation.
- Star picker (review form): click a star to set the draft rating (1–5); filled stars up to the chosen value.
- "Post Review" click: appends `{name:'You', date:'Just now', rating, text}` to that book's review list, clears the textarea and resets rating to 5; no-op if text is empty.
- No hover/focus states beyond default browser behavior were explicitly styled — add standard hover/focus affordances (e.g., subtle darken/lift on cards and buttons, focus rings on inputs) per the target codebase's conventions.
- No responsive/mobile breakpoints were designed — grids use `auto-fill`/`minmax` so they reflow naturally, but the admin 2-column and detail 2-column layouts are desktop-oriented and will need explicit mobile stacking rules.

## State Management
- `screen`: 'catalog' | 'detail' | 'lists' | 'admin' — current view.
- `query`: string — catalog search text.
- `genre`: string — active genre filter ('All' default).
- `selectedId`: string — id of the book shown on the Detail screen.
- `favorites`: map of book id → boolean.
- `reviews`: map of book id → array of `{name, date, rating, text}`, seeded with 2 reviews for one book; new reviews append here.
- `reviewText` / `reviewRating`: draft state for the review form, reset after submit.
- Derived/computed per render: filtered book list (genre + query), favorite book list, selected book + its reviews, star-glyph strings, tinted placeholder styles, activity/stat/alert data (static demo data in this prototype — replace with real API data).

## Design Tokens

**Colors (OKLCH)**
- Background: `oklch(0.97 0.01 60)` — warm off-white
- Card/surface background: `oklch(0.995 0.004 60)`
- Ink (primary text): `oklch(0.24 0.02 50)`
- Ink soft (secondary text): `oklch(0.45 0.02 50)` and `oklch(0.55 0.02 50)` (muted/tertiary)
- Border: `oklch(0.88 0.015 60)`
- Brick (primary accent — CTAs, active states, links): `oklch(0.45 0.1 30)`
- Olive (secondary accent — genre tags): `oklch(0.55 0.09 95)` / darker `oklch(0.42 0.08 95)`
- Success/available green: `oklch(0.5 0.1 145)`
- Overdue/error: brick `oklch(0.45 0.1 30)` at reduced opacity for pill backgrounds

**Typography**
- Headings: Lora (serif), weights 500/600/700
- Body/UI: Inter (sans), weights 400–700
- Scale used: 34–36px page titles, 22–28px section/stat headings, 16–17px card titles, 13–15px body/meta, 11–12px labels/badges

**Radius**: 8px (inputs/buttons), 10px (cards), 20px (pills/chips)

**Spacing**: page padding 40px (balanced density; theme also supports compact 28px/16px-gap and spacious 52px/32px-gap variants via a density setting), card grid gap 24px, card internal padding 16px

## Assets
No real imagery — book covers use a generated diagonal-stripe placeholder pattern (CSS `repeating-linear-gradient`, 3 tint variants cycling by book index) with a centered "COVER" monospace label. Replace with real cover art in production; keep the same aspect ratios (card cover ~200px tall in grids, 280×400px on Detail).

## Files
- `Library Theme.dc.html` — the full prototype (all 4 screens, styles, and interaction logic in one file).
