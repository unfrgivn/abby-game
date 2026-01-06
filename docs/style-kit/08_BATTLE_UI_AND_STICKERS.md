# 08 Battle UI & Sticker System (Scrapbook Combat)

> **Vision:** Battles should feel like flipping to a new page in Fae’s scrapbook. Commands are **stickers** you tap. Feedback is tactile: stickers pop, wobble, and “stamp” onto the page.

This document extends `05_UI_STYLE.md` for **turn-based Sticker battles**.

---

## 1. Visual targets from concept art (use these as references)

Use the existing concept art in this repo as “north stars”:

- Sticker HUD style reference:
  - `docs/style-kit/examples/concepts/ui/ui_kit_sticker_style_for_a_cozy_.png`
- Sticker collectible sheet reference (gloss + die-cut):
  - `docs/style-kit/examples/concepts/collectibles/sticker_collectibles.png`
- Gameplay lighting / softness reference:
  - `docs/style-kit/examples/concepts/environments/topdown_gameplay_screenshot_in_l.png`

Design rule: battle UI should look like it belongs on the same scrapbook paper as the Journal.

---

## 2. Battle screen layout (mobile-first)

### 2.1 Recommended composition

- **Background:** warm paper texture (`Paper White`) with subtle vignette.
- **Enemy area (top 40%):**
  - Enemy “sticker portrait” (large, centered or slightly right).
  - Enemy name on masking tape label.
  - Enemy HP bar (simple, readable).
- **Message strip (middle 10%):**
  - 1–2 line log: “Fae used Bonk!”
  - Tap to advance when needed.
- **Player area (bottom 50%):**
  - Player portrait (polaroid or sticker) + HP (Energy hearts/bar).
  - Primary command row: **Stickers / Items / Defend / Run**.
  - Sticker grid (equipped stickers) appears when Stickers is selected.

### 2.2 Touch sizing

- Minimum tappable size: **48×48 dp**.
- Minimum spacing between buttons: **8 dp**.
- Keep the most common action (Stickers) in the lower-left or centered, depending on handedness testing.

---

## 3. Sticker design rules (icons + cards)

### 3.1 Sticker silhouette

- **Die-cut edge** (no perfect rectangles).
- **White border** (3–4px) + **thin brown stroke** (1px) outside.
- **Soft shadow** (paper-lift effect).
- Optional: tiny “peel” corner on rare stickers.

### 3.2 Sticker tiers

- **Common:** flat color + subtle gloss.
- **Rare:** slightly stronger gloss + small sparkle accent.
- **Sparkly:** holo/iridescent overlay (use sparingly; never strobe).

### 3.3 Sticker states

- **Available:** full color, normal shadow.
- **Selected:** marker-circle highlight *behind* the sticker OR gentle scale-up (1.06x) + bounce.
- **On cooldown:** desaturate to ~60% + small “clock” doodle + turn counter.
- **Disabled (no target / invalid):** reduce alpha to 40% and show a small “X” doodle.

---

## 4. Battle motion + feedback

### 4.1 Transitions

- Enter battle: page flip + tape snap sound.
- Open sticker grid: stickers slide up as if pulled from a pocket.

### 4.2 Action feedback

- When a sticker is used:
  - Sticker pops to center briefly (0.2–0.35s).
  - A “stamp” animation plays (scale down → up with overshoot).
  - Small confetti puff for damage, sparkle puff for healing.

### 4.3 Damage/heal readability

- Use **big, simple numbers** with a short float.
- Keep contrast high: dark outline + light fill.
- Avoid rapid screen shake (kid-friendly).

---

## 5. Battle UI typography

- **Command labels:** bold and rounded (`Fredoka One`).
- **Log text:** friendly rounded sans (`Nunito`/`Quicksand`).
- **Sticker names:** short; 1–2 words preferred.

---

## 6. Sound design cues

- Sticker select: soft paper “tap”.
- Confirm action: rubber stamp “thump”.
- Heal: twinkle.
- Defeat enemy: playful “poof” + tiny raccoon squeak.

---

## 7. Implementation notes (Godot)

UI framework:
- Build battle UI using **Control** nodes.
- Use a shared `Theme` for consistent typography, margins, and button states.
- Prefer 9-slice (`NinePatchRect`) for paper/tape panels.

Animation:
- Use `Tween` or `AnimationPlayer` for sticker pops, bounces, and page-flip transitions.
- Keep animation curves “toy-like”: ease-out + small overshoot.

Performance notes:
- Keep transparency layers minimal on mobile.
- Prefer one shared paper texture background, not multiple full-screen overlays.
- Avoid heavy full-screen blur/DOF; get the look from art direction.

---

## 8. “Do / Don’t” for battle UI

**DO**
- Keep actions to 4 primary choices.
- Make the sticker grid visually fun but not busy.
- Keep text short and readable.

**DON’T**
- Put essential buttons near iOS home indicator edges.
- Use high-contrast flashing effects.
- Make the battle UI look like a sci-fi RPG.
