# Proof-of-Concept build plan

This plan is designed to be handed to OpenCode subagents. It is implementation-ready and avoids needing a full quest chain. It prioritizes a playable loop and the minimum systems required to prove the concept.

---

## 0. End-state deliverables

The PoC is complete when the repo contains:

Core loop

- A Unity project that boots into MainTown
- Player movement and interaction
- Journal UI v0 that shows discovered blacklight notes
- Blacklight lantern that reveals notes and hidden doors
- Combat v0 with Chaos Raccoon (light attack + dodge)
- Maddie follower and simple combat assist
- Arcade interior with a claw machine mini-game
- Prize pool: gems and candy bars
- Energy model, candy consumption, “tired” state
- Respawn rules: interior entrance if inside; bed at home if outside
- Save-anywhere; reload resumes exact position and state

Explicit non-deliverables for PoC

- Lasso and flute gameplay (only stubs if needed)
- Multiple towns, mountains, ocean, island, enchanted forest gameplay
- Party members
- Clubhouse building

---

## 1. Project structure expectations

### 1.1 Unity folder layout

Create and adhere to this:

- Assets/
  - \_Project/
    - Scenes/
    - Prefabs/
    - Scripts/
      - Core/
      - Player/
      - Camera/
      - Interaction/
      - Tools/
      - Combat/
      - AI/
      - UI/
      - Save/
      - Minigames/
      - World/
    - ScriptableObjects/
    - ArtPlaceholders/
    - AudioPlaceholders/

### 1.2 Scenes (PoC)

- MainTown (outdoor strip connecting Home, School entry, Park, Arcade entry)
- SchoolInterior
- ArcadeInterior

Notes

- Keep interiors as separate scenes for respawn clarity and simpler lighting.
- Keep Park as part of MainTown for PoC; do not over-split scenes early.

### 1.3 Prefabs (PoC)

- PlayerFae (movement, interactor, tool controller, combat)
- CompanionMaddie (follow + assist)
- CameraRig (Cinemachine)
- InteractableNote (blacklight note)
- HiddenDoor (blacklight door)
- RaccoonEnemy
- EncounterTrigger (spawns raccoon)
- SaveAnchorHomeBed
- SaveAnchorEntrance (per interior)
- UI_HUD
- UI_Journal
- UI_ClawMachine

---

## 2. System architecture (what exists and what owns what)

### 2.1 Systems and responsibilities

Core

- GameBootstrap
  - Spawns persistent managers; loads initial scene
- SceneDirector
  - Scene transitions; knows interior versus exterior classification
  - Provides spawn and entrance anchor lookup by id

Player

- PlayerController
  - Movement and facing
- Interactor
  - Detects interactables and triggers Interact
- ToolController
  - Blacklight lantern state (toggle or hold-to-scan)
  - Stubs for lasso and flute (disabled in PoC)
- CombatController
  - Light attack combo; dodge roll; hit events

World

- EncounterSystem
  - Trigger and spawn logic for raccoon encounters
- RespawnSystem
  - Applies respawn rules on “tired”
- EnergySystem
  - Manages energy; candy consumption; tired trigger
- InventorySystem
  - Gems and candy bars; add and remove operations

UI

- HUDController
  - Energy display; gems; candy count; prompts
- JournalController
  - Stores discovered notes; provides list to Journal UI
- ClawMachineUI
  - Plays mini-game; shows prize; grants reward

Persistence

- SaveSystem
  - Minimal, safe JSON payload; backward-compatible defaults

### 2.2 PoC code constraints

- Prefer composition over inheritance.
- MonoBehaviours stay thin; move logic to plain C# classes when practical.
- Keep dependencies minimal: URP, Input System, Cinemachine only.
- Any “global manager” must be created once and persist through scene loads.

---

## 3. Build order (to avoid rework)

Implement in this sequence:

1. Bootstrap, camera, movement
2. Interaction framework
3. Save and load v0 plus respawn anchors (early)
4. Energy model (simple)
5. Blacklight lantern, notes, hidden doors, journal
6. Combat v0 plus raccoon AI and encounter trigger
7. Maddie follower and assist
8. Arcade scene plus claw machine rewards
9. Polish pass plus regression checklist

---

## 4. Detailed work items for OpenCode agents

### 4.1 Unity bootstrap, scenes, camera, movement

Owner: @unity-engineer

Tasks

1. Create Unity project with URP, Input System, Cinemachine.
2. Create MainTown blockout using the sketch as reference:
   - Home area with bed anchor
   - Main road to School exterior entry
   - Park area on the same strip
   - Arcade exterior entry
   - Blocked paths to future areas with playful signage
3. CameraRig:
   - Cinemachine virtual camera
   - Tilt 55 to 65 degrees
   - Soft follow and look-ahead
4. PlayerFae:
   - Top-down movement with acceleration and deceleration
   - Facing based on movement direction
   - Choose one movement tech and stick to it: CharacterController or Rigidbody
5. Add HUD shell (empty is fine initially) to validate UI scaling.

Acceptance criteria

- Player moves smoothly with keyboard and touch.
- Camera remains stable and readable.
- MainTown loads as the default entry.

---

### 4.2 Interaction system

Owner: @unity-engineer

Goal
A single “interact” verb that supports notes, doors, arcade, and later NPCs.

Tasks

1. Define IInteractable:
   - string GetPromptText()
   - bool CanInteract(GameObject interactor)
   - void Interact(GameObject interactor)
2. Interactor on player:
   - Finds nearest interactable in range (sphere overlap) or in front
   - Shows prompt when available
   - Calls Interact on input
3. Prompt UI:
   - Simple; either bottom-of-screen or floating above target

Acceptance criteria

- Prompt appears when near an interactable.
- Interact triggers the correct behavior.
- No direct coupling to specific interactable classes.

---

### 4.3 Save and load v0 plus respawn anchors

Owner: @unity-engineer

Why now
Everything later needs persistence and safe recovery.

Tasks

1. Define SaveData v0:
   - sceneId
   - playerPosition (x,y,z)
   - playerFacing (yaw or vector)
   - storyFlags (set of strings)
   - inventory: gems, candyBars, toolsUnlocked (lantern bool)
   - energy: current, max
   - respawn anchors: homeBedId, entranceIdByScene
2. SaveSystem
   - Save() and Load()
   - JSON on disk; single slot is fine
   - Backward-compatible defaults when fields are missing
3. Respawn anchors
   - HomeBed anchor in Home area
   - Entrance anchor in SchoolInterior and ArcadeInterior
4. Respawn rules implementation
   - If tired while inside SchoolInterior or ArcadeInterior; respawn at that scene entrance anchor
   - If tired while in MainTown; respawn at home bed
   - No loss of gems or candy on respawn in PoC
5. Add debug shortcuts
   - Save, Load, Teleport-to-Home for testing

Acceptance criteria

- Save anywhere, reload, resume exact position and scene.
- Tired triggers respawn to correct anchor.
- Save remains valid as new fields are added later.

---

### 4.4 Readability defaults for URP and lantern visuals

Owner: @tech-artist

Tasks

1. Outdoor lighting recipe
   - One directional sun; soft shadows
   - Warm ambient
2. Interior lighting recipe
   - Simple area lights; readable corners; no harsh contrast
3. Lantern reveal visuals
   - Select one UV glow color and use it consistently
   - Notes and door symbols must be readable at camera distance
   - Decide if reveal persists briefly after scanning (recommended: yes)

Acceptance criteria

- Blacklight notes and door symbols are legible during play.
- Scene readability is stable with minimal post-processing.

---

### 4.5 Blacklight lantern, notes, hidden doors, journal

Owners: @unity-engineer and @game-designer

Mechanic definition (PoC)

- Lantern scan mode reveals UV content in a cone or radius in front of player.
- Notes and doors react to scanning:
  - Notes: show UV decal and popup; save as discovered; add to journal
  - Doors: show UV symbol; mark revealed; becomes interactable

Tasks

1. ToolController
   - Choose: toggle lantern or hold-to-scan
   - Expose scan distance and angle
2. Reveal receiver component
   - Handles OnScanEnter and OnScanStay
   - One-time discovery for notes and doors
3. InteractableNote
   - noteId, title, bodyText, iconKey
   - On discover: popup and journal entry
4. HiddenDoor
   - Starts non-interactable
   - On reveal: becomes interactable
   - PoC door behavior should be one of:
     - Opens shortcut passage in-scene, or
     - Loads a tiny secret room scene
5. Journal
   - Stores discovered notes
   - UI shows list of entries (title and short text)

Content targets for PoC

- 6 to 10 notes across all scenes
- 2 hidden doors total:
  - School hidden door (storage closet to secret room or shortcut)
  - Park hedge gate or arcade back door

Acceptance criteria

- Lantern consistently reveals notes and doors.
- Discovery persists across save and load.
- Journal reflects discovered notes.

---

### 4.6 Energy and candy bars

Owner: @unity-engineer

Tasks

1. EnergySystem
   - maxEnergy and currentEnergy
   - TakeDamage reduces energy
   - ConsumeCandy restores energy with clamp
2. Inventory for candy bars
   - Start with a small count or none
   - Add via claw machine prizes
3. UI
   - Show energy
   - Show candy count

Acceptance criteria

- Getting hit reduces energy.
- Candy restores energy.
- Energy reaching zero triggers tired and respawn.

---

### 4.7 Combat v0 and Chaos Raccoon

Owners: @unity-engineer and @game-designer

Player combat requirements

- Light attack combo (2 to 3 hits)
- Dodge roll with i-frames and cooldown
- Targeting: simplest acceptable approach (nearest enemy within radius, or directional)

Raccoon requirements

- Cute and readable
- Teaches dodge timing
- State machine:
  - Patrol or idle
  - Chase
  - Telegraph
  - Swipe
  - Dash past
  - Recover

Tasks

1. CombatController
   - Attack input and hitbox window
   - Dodge movement burst; invulnerability window; cooldown
2. Damage model
   - Enemy HP
   - Enemy defeat behavior: puff and despawn
3. RaccoonEnemy AI
   - Executes swipe then dash past
   - Telegraph uses body lean and audio cue
4. EncounterTrigger
   - Spawns raccoon in Park area
   - PoC can respawn enemies on reload; persistence is optional for PoC
5. Expose tuning constants
   - PlayerAttackDamage, PlayerAttackRate
   - DodgeDistance, DodgeIFrames, DodgeCooldown
   - RaccoonHP, RaccoonDamage, TelegraphTime, DashSpeed

Acceptance criteria

- Combat feels understandable and fair.
- Dodge is required and works reliably.
- Getting tired respawns correctly without losing prizes.

---

### 4.8 Maddie follower and assist

Owner: @unity-engineer

PoC requirements

- Maddie follows behind Fae.
- Maddie assists automatically in combat on a cooldown.

Tasks

1. Follow behavior
   - Maintain an offset behind player
   - Smooth motion; avoid jitter; avoid blocking the player
2. Assist behavior
   - If enemy in range and cooldown ready; apply small damage or brief stun
   - Simple leap or dash animation; puff VFX
3. Persistence
   - Maddie always active in PoC; no flute menu yet

Acceptance criteria

- Maddie stays with player and does not interfere with movement.
- Assist is noticeable but not overpowering.

---

### 4.9 Arcade and claw machine mini-game

Owners: @unity-engineer and @game-designer

PoC requirements

- ArcadeInterior scene
- Claw machine interactable opens UI mini-game
- Prize pool: gems and candy bars
- Rewards saved and persist

Tasks

1. ArcadeInterior blockout and claw machine interactable
2. Claw machine UI
   - Play button
   - Result display
3. Prize pool table and probabilities
   - Gems: small, medium, large
   - Candy bars: 1 or 2
4. Reward grant integration
   - Adds to inventory; updates HUD; persists via SaveSystem

Acceptance criteria

- Player can win prizes consistently.
- Prizes appear in inventory immediately.
- Save and load preserves prizes.

---

### 4.10 QA regression checklist

Owner: @qa-playtest

Minimum checklist

- New game spawns at home.
- Walk to school; reveal two notes; open hidden door; journal persists after save and load.
- Walk to park; trigger raccoon; take damage; consume candy; verify energy.
- Get tired outdoors; respawn at home bed.
- Enter SchoolInterior; get tired; respawn at school entrance.
- Enter arcade; play claw machine; receive gems or candy; save; reload; verify counts.

---

## 5. Map application for PoC

Use your sketch as the top-level layout, but implement PoC as a compact vertical strip.

Placement guidance

- Home at the bottom of the strip; bed anchor inside or adjacent
- School in the middle; hidden door inside
- Park between school and arcade or adjacent; raccoon encounter zone; second hidden door
- Arcade near the town center portion; claw machine inside

Gates to future content

- Mountains: “field trip later”
- Ocean and dock: “bridge under construction”
- Enchanted forest: “sparkly barrier” with a clear later promise

---

## 6. Asset prompt pack (for your external asset pipeline)

Use this style anchor in every prompt:
“Lil Gator-style; chunky toy-like shapes; low poly; flat colors or lightly painted texture; bright calm palette; soft lighting; friendly and cozy; top-down readability; rounded edges; playful kid-made vibe (cardboard, tape, stickers); no scary elements.”

Blacklight notes (10)
Prompt:
“Create a set of invisible-ink blacklight note decals for a kid mystery game; each note is a simple doodle plus short text that only appears under a UV lantern; playful handwritten style; readable from top-down; include simple icons (school bell, paw print, star, arrow). Provide 10 variants.”

Suggested note themes:

1. Follow the paw prints to the playground.
2. The real door has a star on the corner.
3. Not all posters tell the truth.
4. Look where the chalk is brightest.
5. Raccoons love snacks behind the arcade.
6. Two arrows mean turn back.
7. The nurse knows the quiet paths.
8. A hidden door is still a door.
9. If you see three dots, you’re close.
10. The glow shows what’s real.

Hidden door symbols (6)
Prompt:
“Design 6 simple UV glyph symbols that look like kid doodles; each symbol indicates a hidden door; bold lines, playful shapes; consistent style; readable at small size; not occult.”

Chaos raccoon
Prompt:
“Cute non-scary Chaos Raccoon enemy; toy-like proportions; expressive eyes; mischievous smile; attack set is swipe then dash past; show idle, telegraph, swipe, dash poses; low poly; flat colors; no sharp teeth; top-down readability.”

Claw machine and prizes
Prompt:
“Small-town arcade claw machine; chunky toy-like shape; bright calm colors; sticker decals; clear glass box with prizes; prizes are colorful gem shapes and candy bars; top-down readability.”

---

## 7. Risk controls

- Do not build a full quest system for PoC.
- Do not build a full inventory UI; use counts and a small panel.
- Do not add more enemies until raccoon feels right.
- Do not implement lasso or flute; keep them as stubs.

---

## 8. PoC walkthrough script

1. Spawn at home.
2. Walk to school exterior; enter SchoolInterior.
3. Use lantern; reveal two notes; open hidden door; journal updates.
4. Exit; walk to park.
5. Trigger raccoon encounter; use attack and dodge; take at least one hit.
6. Eat a candy bar; energy increases.
7. Get tired outdoors; respawn at home bed.
8. Walk to arcade; enter; play claw machine; win gems or candy.
9. Save; quit; load; resume same position with same prizes and flags.

---

## 9. OpenCode work item split

Create work items matching sections 4.1 through 4.10. Keep each work item small and verifiable.

Recommended commit granularity

- Movement and camera
- Interaction
- Save and respawn
- Lantern and journal
- Combat and raccoon
- Companion
- Arcade
- Polish and QA checklist
