# PoC content map
Version: v1  
Purpose: Concrete placement list for the PoC loop (notes, hidden doors, anchors, encounters, arcade).  
Scope: MainTown, SchoolInterior, ArcadeInterior.

This file is meant to be a literal checklist for building scenes. It assumes the revised architecture (Bootstrap + persistent GameState + ScriptableObject databases + stable GUID ids).

---

## 1. Scenes and classification
### 1.1 Content scenes
- **MainTown** (Exterior)
- **SchoolInterior** (Interior)
- **ArcadeInterior** (Interior)

### 1.2 Spawn and respawn anchors
Respawn rules:
- If tired while **inside an interior**; respawn at that interior’s entrance anchor.
- If tired while **outside**; respawn at **Home Bed**.

Anchors to place in scenes (each must have a `PersistentId` component with the exact id below):

- **Home Bed Anchor** (MainTown)  
  - AnchorId: `55452987-7a93-43ac-ad9b-5adfa125a88a`  
  - Placement: inside Fae’s house, next to the bed (or at the front door if you are not building an interior).

- **School Entrance Anchor** (SchoolInterior)  
  - AnchorId: `926d2964-498c-4252-a029-ccb1a22ca900`  
  - Placement: just inside the school front doors, facing into the hallway.

- **Arcade Entrance Anchor** (ArcadeInterior)  
  - AnchorId: `477d95b1-733b-4170-a63a-f00e6c9bdd9c`  
  - Placement: just inside the arcade entry, facing toward the claw machine.

Optional (only if you want the school hidden door to load a sub-room with its own spawn):
- **School Secret Room Anchor** (SchoolInterior)  
  - AnchorId: `20d784ea-40fd-4b04-8037-89a6347f8671`  
  - Placement: inside the secret room area behind the hidden closet door.

---

## 2. Intended PoC player path (no quest system)
This is the “soft quest” driven by notes.

1) Start at **Home Bed** in MainTown.
2) Use **Blacklight Lantern** to discover the first note near home.
3) Follow the road to **SchoolInterior**; discover 2–3 notes; reveal and open the **School Hidden Door**; collect a candy bar reward in the secret room.
4) Exit to **Park** area in MainTown; trigger **Chaos Raccoon** encounter; take damage; eat candy bar to restore energy.
5) Reveal the **Park Hedge Gate** hidden door as a shortcut.
6) Enter **ArcadeInterior**; play **Claw Machine**; win gems and/or candy bars.
7) Save; reload; everything persists (position, notes, doors, prizes).

---

## 3. Blacklight Lantern content rules (PoC)
- Notes and door symbols exist as normal decals/meshes but are invisible until scanned.
- Reveal requires holding scan for a short time (recommended 0.3–0.7s).
- After revealing a door, the door becomes interactable and stays revealed permanently (saved).

Icon keys used in note popups and journal list:
- `icon_home`
- `icon_school`
- `icon_star`
- `icon_paw`
- `icon_arrow`
- `icon_arcade`
- `icon_raccoon`
- `icon_gem`

---

## 4. Notes (8 total)
All notes must exist as:
- A scene object with a `PersistentId` matching `noteId`
- A `NoteReveal` component referencing a `NoteDefinition` in `NoteDatabase`

Recommended: keep note body text to 1–2 lines.

### NOTE 1
- noteId: `05d763c1-0a81-46fc-9e99-4acb426c0b25`
- Scene: MainTown
- Suggested placement: Fae’s bedroom wall poster OR journal page on desk (right next to the bed)
- Title: “Glow Time”
- Body: “If you can’t see it, shine the light. Start with the road to school.”
- Doodle: small flashlight + arrow
- iconKey: `icon_home`

### NOTE 2
- noteId: `3b59327c-3932-4c40-8ce5-15ea485a8898`
- Scene: MainTown
- Suggested placement: sidewalk chalk near the main road out of the home yard
- Title: “Follow Me”
- Body: “The real clue trail starts at school. Look for the star.”
- Doodle: dotted path + star
- iconKey: `icon_arrow`

### NOTE 3
- noteId: `f41b557b-e6a7-46f2-b225-e0b39b94f71f`
- Scene: SchoolInterior
- Suggested placement: hallway bulletin board poster (near entrance)
- Title: “Not All Posters”
- Body: “Some notes are fake. The glowing one points the right way.”
- Doodle: poster with a big X and a small checkmark
- iconKey: `icon_school`

### NOTE 4
- noteId: `00284283-34b5-4e17-bc1a-dbb97877d7ed`
- Scene: SchoolInterior
- Suggested placement: outside the art room door frame OR on a nearby wall
- Title: “Star Corner”
- Body: “The secret door has a star in the corner. Shine the frame.”
- Doodle: door rectangle with a star in top-right corner
- iconKey: `icon_star`

### NOTE 5
- noteId: `7568df78-1f0d-4ba6-84d2-ac434c1a651b`
- Scene: SchoolInterior (inside the hidden room behind the closet door)
- Suggested placement: on a small table next to the reward
- Title: “Snack Stash”
- Body: “Candy helps when things get wild. The park gets weird.”
- Doodle: candy bar + little raccoon face
- iconKey: `icon_raccoon`
- Reward nearby (not part of the note itself): 1 candy bar pickup

### NOTE 6
- noteId: `773a4316-f5a7-4566-9093-fec4194de64d`
- Scene: MainTown (Park area)
- Suggested placement: playground sign or trash can sticker near where the raccoon encounter triggers
- Title: “Trash Bandit”
- Body: “Raccoons dash past you. Dodge when they swipe!”
- Doodle: raccoon + curved dash arrow
- iconKey: `icon_raccoon`

### NOTE 7
- noteId: `5b8be8fb-8042-4e74-afa8-7bdcd2f0f042`
- Scene: MainTown (Park area, near hedge gate)
- Suggested placement: hedge wall near the hidden gate
- Title: “Paw Gate”
- Body: “Paw prints mean a secret path. Shine the hedge.”
- Doodle: 3 paw prints leading into leaves
- iconKey: `icon_paw`

### NOTE 8
- noteId: `47523377-7034-48b4-b0dd-320f36cac78a`
- Scene: ArcadeInterior
- Suggested placement: poster next to claw machine (visible only under blacklight)
- Title: “Prize Machine”
- Body: “Win gems or candy. The center drop is the best!”
- Doodle: claw + gem + candy
- iconKey: `icon_arcade`

---

## 5. Hidden doors (2 total)

### 5.1 School hidden door: “Star Closet”
- doorId: `97d2e256-8e98-45c2-83ae-417685f196b8`
- Scene: SchoolInterior
- Placement: a plain-looking closet door or locker panel in the hallway (close to NOTE 4)
- UV symbol: a **star in the top-right corner** of the door frame
- Reveal requirement: blacklight scan completes on the door’s `HiddenDoorReveal` component
- On reveal complete:
  - Mark `revealedDoors` contains `doorId`
  - Enable interaction prompt: “Open (Secret)”
- Door behavior (recommended for PoC): **in-scene** open to a small secret room behind it
  - The room should contain:
    - 1 candy bar pickup (teaches energy restore loop)
    - Optional small gem pickup (5 gems) as extra reward
- Story flags (optional but useful):
  - `School.HiddenDoor.Revealed`
  - `School.HiddenDoor.Opened`

### 5.2 Park hidden door: “Paw Hedge Gate”
- doorId: `326e80cb-d53c-42ed-a2da-405e4ce2ec56`
- Scene: MainTown (Park)
- Placement: hedge wall segment that visually looks like set dressing
- UV symbol: **three paw prints** leading into a leaf outline
- Reveal requirement: blacklight scan completes on the hedge gate reveal area
- Door behavior:
  - Opens a shortcut passage that reduces walking time from Park to Arcade
  - Recommended implementation:
    - Gate swings open, or leaves part like curtains
    - Nav path becomes passable
- Story flags (optional):
  - `Park.HedgeGate.Revealed`
  - `Park.HedgeGate.Opened`

---

## 6. Combat encounter placement
### 6.1 Park raccoon encounter (single encounter for PoC)
- encounterId: `a54ac5a3-b1b9-440e-9350-ac46a2140144`
- Scene: MainTown (Park)
- Placement: a trigger volume near NOTE 6 (trash can / playground sign)
- Behavior:
  - On player enter:
    - spawn 1 Chaos Raccoon at a spawn point in view
    - lock out re-triggering until raccoon is defeated OR player exits trigger for 10 seconds
- Reward:
  - No reward required (arcade handles prizes), but optional: 1 gem drop for positive feedback
- Save:
  - Encounter state persistence is optional for PoC
  - It is acceptable for the raccoon to respawn after reload

---

## 7. Arcade claw machine
### 7.1 Claw machine interactable
- minigameId: `ae7869a9-efce-43a1-a89c-95113e7e9cd8`
- Scene: ArcadeInterior
- Placement: clearly visible from entrance anchor
- Interaction prompt: “Play Claw Machine”
- Minigame type: timing-based (marker oscillates; tap drop)
- Prize pool (recommended PoC defaults):
  - GemsSmall: +5 (weight 50)
  - GemsMedium: +15 (weight 25)
  - GemsLarge: +50 (weight 5)
  - CandyBar1: +1 (weight 15)
  - CandyBar2: +2 (weight 5)

Notes:
- This table should live in a `PrizeTable` ScriptableObject so you can tune without code.

---

## 8. Blockers for future content (set dressing only)
Place playful blockers in MainTown so the map feels bigger without building it.

- **To Mountains**: sign “Field trip later!” + orange cones
- **To Ocean/Dock**: sign “Bridge under construction” + wooden barrier
- **To Enchanted Forest**: sparkly hedge barrier + sign “Too sparkly right now…”

No gameplay behind these blockers in PoC.

---

## 9. Implementation notes for agents
- Every Note, Door, and Anchor must have a `PersistentId` with the exact id from this file.
- `NoteDatabase` and `DoorDatabase` entries must use the same ids.
- Discovery state:
  - Notes: add noteId to `GameState.discoveredNotes`
  - Doors: add doorId to `GameState.revealedDoors`
- Save and load must persist `discoveredNotes` and `revealedDoors`.

If you need to adjust placements during blockout:
- Do not change ids; move the objects instead.
