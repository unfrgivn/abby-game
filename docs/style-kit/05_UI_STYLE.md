# UI Style Guide: Sticker & Scrapbook

> **Vision:** The UI should feel like Fae’s personal journal or scrapbook. It is handmade, playful, and tactile. It uses "stickers" for icons, "tape" for headers, and "doodles" for emphasis.

---

## 1. Core Metaphor: The Scrapbook

-   **Materials:** Paper textures, cardboard backing, masking tape, stickers, marker doodles.
-   **Edges:** Slightly rough or torn edges on panels. No perfect digital rectangles.
-   **Depth:** Slight drop shadows to show layering (paper on top of paper).
-   **Motion:** Panels "slide" or "flop" in like turning a page.

---

## 2. Iconography: The Sticker Set

All icons should look like die-cut stickers.

-   **Outline:** Thick white outline (2-4px) around every icon.
-   **Finish:** Subtle gloss highlight on the top half to suggest a vinyl sticker.
-   **Shadow:** Soft drop shadow (offset y: 2px, blur: 4px, alpha: 20%) to lift it off the "paper".
-   **Examples:**
    -   *Heart Container:* A red heart sticker with a white border.
    -   *Coin/Gem:* A sparkly gem sticker.
    -   *Button Prompts:* A circular sticker with the button letter drawn in marker.

---

## 3. Typography

-   **Headers:** `Fredoka One` or similar rounded, chunky font.
    -   Color: `Fae Purple` (#6A5ACD) or `Dark Grey` (#333333).
-   **Body Text:** `Nunito` or `Quicksand` (Rounded sans-serif).
    -   Color: `Ink Black` (#202020).
    -   Readability is key, but keep it friendly.
-   **Handwritten Notes:** A legible handwriting font for "Fae's notes" on the UI.
    -   Color: `Marker Blue` (#0055AA) or `Pencil Gray` (#505050).

---

## 4. HUD Elements

### Health (Energy)
-   **Style:** Stickers on a bar.
-   **Full:** Bright red heart sticker.
-   **Empty:** A faded, "peeled off" sticker outline or a greyed-out backing.

### Action Prompts
-   **Context:** Appear near the object in world space.
-   **Visual:** A "speech bubble" doodle with the button prompt sticker inside.
-   **Animation:** Bob up and down gently (Sine wave, freq: 2, amp: 5px).

### Dialog Box
-   **Background:** A crumpled paper texture or index card.
-   **Portrait:** A polaroid photo frame of the speaker clipped to the box.
-   **Nameplate:** A piece of masking tape with the name written on it.

---

## 5. Menus & Inventory

### The Journal (Pause Menu)
-   **Visual:** Literally opens a book on screen.
-   **Tabs:** Colored bookmarks sticking out the side.
-   **Selection:** A thick marker circle drawn around the selected item.

### Inventory Grid
-   **Slots:** Drawn pencil squares on paper.
-   **Items:** Stickers placed in the squares.
-   **Empty Slot:** Scribble texture or faint doodle.

---

## 6. Colors (UI Specific)

| Usage | Color Name | Hex | Notes |
| :--- | :--- | :--- | :--- |
| **Primary Base** | `Paper White` | `#FDFBF7` | Warm, slightly textured background |
| **Primary Text** | `Ink Black` | `#2D2D2D` | Softened black for readability |
| **Highlight/Select** | `Highlighter Yellow` | `#FFEB3B` | Marker highlight behind text |
| **Accent 1** | `Tape Teal` | `#40E0D0` | Masking tape elements |
| **Accent 2** | `Sticker Pink` | `#FF69B4` | Notification badges |
| **Error/Alert** | `Marker Red` | `#FF4500` | "Wrong" buzzers or low health |

---

## 7. Do's and Don'ts

**DO:**
-   Use white outlines on interactive elements.
-   Rotate elements slightly (-2 to +2 degrees) so they don't look perfectly aligned (computer-generated).
-   Use "tape" visuals to attach panels to the screen edges.

**DON'T:**
-   Use sharp 90-degree corners.
-   Use Sci-Fi or high-tech glows/gradients.
-   Use standard system fonts (Arial, Times).
-   Make the UI look "dirty" or "grunge" (keep it clean-crafty, not trash-crafty).
