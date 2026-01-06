# Proof-of-Concept build plan (Godot + Turn-based Stickers)

This plan defines the minimum architecture and delivery sequence for the PoC described in `spec.md`.

Design target:
- Cozy top-down 3D exploration (lantern, notes, hidden doors)
- **Visible overworld encounters** that transition into **turn-based Sticker battles**
- Runs on **macOS + iOS**

---

## 0) End-state deliverables (PoC “done”)

Core loop
- Game boots into Cloverhollow (Home → Road → School → Park → Arcade)
- Player moves and interacts
- Journal UI shows discovered blacklight notes
- Blacklight Lantern reveals notes and hidden doors
- **Battle v0: Chaos Raccoon** using turn-based commands
- **Sticker Book v0:** inventory + equip 4 stickers
- Arcade interior with claw machine mini-game
- Prize pool: gems and candy bars
- Tired/respawn rules:
  - If inside School or Arcade: respawn at that interior entrance
  - If outside: respawn in bed at home
- Save-anywhere; reload resumes exact position, inventory, and discovered content

Non-deliverables for PoC
- Lasso and flute gameplay (only stubs)
- Multiple towns, mountains, ocean, island, enchanted forest gameplay
- Party members beyond Maddie follower
- Clubhouse building
- Complex RPG systems (fusion, deep status charts)

---

## 1) Locked decisions for PoC

These decisions prevent churn.

### 1.1 Scene model (Godot)

- `Bootstrap.tscn` is the entry scene and never unloads.
- Content scenes load as full scene changes via `SceneRouter` (simpler than additive for early Godot projects).
- Battle is a dedicated scene: `BattleScene.tscn`.

**Rule:** content scenes do not create global managers.

### 1.2 Autoload singletons (keep minimal)

Autoloads:
- `GameState` — persistent player state (inventory, flags, last anchor)
- `SaveSystem` — versioned, atomic save/load
- `SceneRouter` — loads scenes and resolves respawn anchors
- `AudioBus` (optional) — music/SFX routing

### 1.3 Input

Use InputMap actions:
- `move_up/down/left/right`
- `interact`
- `lantern`
- `journal`
- `ui_accept`, `ui_cancel` (battle menus)

Touch
- On-screen joystick (movement)
- Large “Action / Lantern / Journal” buttons

### 1.4 Persistence and stable IDs

Everything that persists must have a stable ID:
- Notes: `note_id`
- Doors: `door_id`
- Respawn anchors: `anchor_id`
- Encounters: `encounter_id`

Save file requirements:
- Versioned schema
- Atomic writes (write temp then rename)
- Load fallback: if scene/anchor missing, spawn at HomeBed anchor

### 1.5 Battles

- Turn-based command menu:
  - Stickers / Items / Defend / Run
- Equipped sticker loadout = 4 stickers
- PoC enemy family: Chaos Raccoon

### 1.6 PoC pacing

- No explicit quest system; the blacklight note trail is the “soft quest.”
- Hidden doors should reward the player quickly (gems or a sticker).

---

## 2) Project structure (recommended)

Inside `GodotProject/`:
- `scenes/`
  - `bootstrap/Bootstrap.tscn`
  - `world/` (Cloverhollow, School, Arcade)
  - `battle/BattleScene.tscn`
  - `ui/` (Journal, StickerBook, HUD)
- `scripts/`
  - `core/` (GameState, SaveSystem, SceneRouter)
  - `overworld/` (Player, Interactables, Lantern)
  - `battle/` (BattleState, BattleDirector, AI)
  - `ui/`
- `data/`
  - `stickers/` (StickerDef .tres)
  - `enemies/` (EnemyDef .tres)
  - `encounters/` (EncounterDef .tres)
  - `notes/` (NoteDef .tres)
  - `doors/` (DoorDef .tres)
  - `tables/` (PrizeTable .tres)
- `assets/` (art/audio placeholders)

---

## 3) Data-driven content (Resources)

Use `Resource` classes so designers can iterate without code changes.

### 3.1 StickerDef
Fields:
- `id`, `name`, `description`
- `type`, `targeting`
- `power`, `cooldown_turns`

### 3.2 EnemyDef
Fields:
- `id`, `name`
- `max_hp`, `speed`, `attack_power`
- `ai_pattern` (simple enum for PoC)

### 3.3 EncounterDef
Fields:
- `id`
- `enemy_ids` (list)
- `first_win_sticker_reward_id`
- `gems_reward`

### 3.4 NoteDef / DoorDef
- `id`, `title`, `body`
- Door has `target_scene` and/or `target_anchor_id`

---

## 4) Runtime architecture

### 4.1 Overworld

Key nodes/components:
- `PlayerController` (movement + interact)
- `Interactable` interface-like pattern (base script with `interact()`)
- `LanternScanner` (reveals notes/doors)
- `JournalController` (UI + discovered notes)

### 4.2 Battle

**Separation of concerns is mandatory.**

- `BattleState` (pure logic, no nodes):
  - HP, cooldowns, turn order, victory/defeat
  - Resolves commands into outcomes

- `BattleDirector` (node):
  - Bridges Overworld Encounter → BattleState → Battle UI

- `BattleHUD` (Control):
  - Command menu + sticker grid + message log

### 4.3 Scene transitions

- Overworld encounter calls: `SceneRouter.start_battle(encounter_id)`
- Battle outcome returns to prior scene and position (or respawn)

---

## 5) Build sequence (recommended PR order)

### PR 1 — Data foundations
- Implement Resource definitions and loaders
- Implement GameState skeleton (inventory + flags)
- Implement SaveSystem with versioned JSON and atomic write

### PR 2 — Sticker Book UI
- Inventory display
- Equip 4-slot loadout
- Persistence save/load

### PR 3 — Battle sandbox
- BattleScene with HUD
- BattleState supports:
  - turn order
  - sticker cooldowns
  - damage/heal
  - victory/defeat

### PR 4 — Overworld encounter trigger
- Place a Chaos Raccoon encounter in the Park
- Trigger battle and return
- First win grants sticker

### PR 5 — PoC polish
- Lantern notes + hidden doors tuned
- Arcade claw machine integrated
- Regression checklist pass

---

## 6) Developer tools (must-have)

Add a minimal Debug Panel (dev builds only):
- Teleport to Home/School/Park/Arcade
- Grant gems
- Grant sticker by ID
- Force-start battle by encounter ID
- Save/Load buttons
