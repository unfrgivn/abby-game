# Proof-of-Concept build plan (revised v2)

This version tightens architecture and reliability. It keeps the PoC scope the same, but makes the implementation cleaner to extend into the full game.

---

## What I changed and why (high level)

### 1) Reduce “manager sprawl”

The original plan names many systems as separate global objects. That tends to turn into a web of references fast.  
Revision: keep a small set of persistent services; push feature logic into local components and plain C# classes; communicate via events.

### 2) Make persistence safe and future-proof

The original plan saves “story flags + position” but does not force stable IDs or atomic writes. That will break as soon as you rename objects/scenes or crash during save.  
Revision: every save-relevant object gets a stable GUID; saves are versioned; writes are atomic; load falls back to safe spawn if anything is missing.

### 3) Make blacklight and interaction data-driven

Hard-coding note text, prize weights, and enemy tuning inside prefabs or scripts slows iteration.  
Revision: ScriptableObject databases for notes, doors, prize tables, and tuning constants.

### 4) Make scanning and interaction performant

Naive per-frame physics and allocations cause GC spikes on mobile.  
Revision: use NonAlloc physics queries; throttle scans; use layer masks; pool VFX.

### 5) Make the claw machine feel like a game, not a random button

Pure RNG is fine for a placeholder but it feels flat quickly.  
Revision: keep it simple but add one skill step (timing-based drop); still cheap to implement.

### 6) Add developer tools early

A PoC lives and dies by iteration speed.  
Revision: a debug overlay with save/load, teleport, grant items, and spawn raccoon is part of the plan.

---

## 0. End-state deliverables

The PoC is complete when:

Core loop

- Game boots into Cloverhollow
- Player moves and interacts
- Journal UI shows discovered blacklight notes
- Blacklight lantern reveals notes and hidden doors
- Combat v0: Chaos Raccoon with Light Attack + Dodge
- Maddie follows and provides simple combat assist
- Arcade interior with claw machine mini-game
- Prize pool: gems and candy bars
- Energy model; candy consumption; tired state
- Respawn rules:
  - Inside School or Arcade; respawn at that interior entrance
  - Outside; respawn in bed at home
- Save-anywhere; reload resumes exact position, inventory, and discovered content

Non-deliverables for PoC

- Lasso and flute gameplay (only stubs)
- Multiple towns, mountains, ocean, island, enchanted forest gameplay
- Party members
- Clubhouse building

---

## 1. Locked decisions for PoC

These decisions prevent churn.

### 1.1 Scenes and loading

- Content scenes:
  - Cloverhollow
  - SchoolInterior
  - ArcadeInterior
- A persistent “Bootstrap” scene loads first and never unloads.
  - Contains: UIRoot, InputRouter, SaveSystem, GameState, Audio (optional), DebugOverlay.
- Content scenes load additively; Bootstrap keeps UI stable across transitions.

### 1.2 Input System

- Use Unity Input System with two action maps:
  - Gameplay: Move, Interact, Attack, Dodge, Lantern, Journal, Pause
  - UI: Navigate, Submit, Cancel (supports touch and mouse)
- Touch:
  - PoC can ship with simple on-screen buttons for Attack, Dodge, Interact, Lantern, Journal.
  - Movement can be a virtual joystick or tap-to-move later; for PoC pick joystick to avoid pathing work.

### 1.3 Saving and IDs

- Everything that needs to persist has a stable GUID:
  - Notes (noteId)
  - Hidden doors (doorId)
  - Respawn anchors (anchorId)
- GUIDs are assigned in-editor and never change unless the object is replaced.
- Save files are versioned and written atomically.

### 1.4 Combat

- Player: Light Attack combo (2–3 hits) + Dodge roll with i-frames and cooldown.
- Enemy: Chaos Raccoon only.

### 1.5 PoC pacing

- No quest system; the blacklight note trail is the “soft quest.”
- One hidden room or shortcut behind the school hidden door; it should contain a small reward (gem or candy) so the tool feels meaningful.

---

## 2. Project structure (Unity)

Keep your folder structure; add a dedicated data/config area so tuning is not buried in prefabs.

- Assets/
  - \_Project/
    - Scenes/
    - Prefabs/
    - Scripts/
      - Bootstrap/
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
      - Debug/
    - ScriptableObjects/
      - Config/
      - Content/
      - Tuning/
    - ArtPlaceholders/
    - AudioPlaceholders/

---

## 3. Data-driven content (ScriptableObjects)

This is the biggest leverage change. It keeps code stable while you iterate.

### 3.1 Config

- GameConfig (ScriptableObject)
  - Scene names and which scenes are “interiors”
  - Default start scene and start anchor
  - Mapping: sceneName → entranceAnchorId
  - HomeBedAnchorId

### 3.2 Content databases

- NoteDatabase
  - List of NoteDefinition:
    - noteId (GUID string)
    - title
    - bodyText
    - iconKey (optional)
    - recommendedPlacement (string; “School hallway poster”, etc.)
- DoorDatabase
  - List of DoorDefinition:
    - doorId (GUID string)
    - type: ShortcutInScene | LoadSecretRoom
    - targetScene (optional)
    - targetAnchorId (optional)
- PrizeTable
  - Weighted rewards:
    - GemsSmall, GemsMedium, GemsLarge
    - CandyBar1, CandyBar2

### 3.3 Tuning

- PlayerTuning
  - Move speed, accel, decel
  - Attack timings, hitbox sizes, damage
  - Dodge distance, i-frames, cooldown
  - Max energy and restore amounts
- RaccoonTuning
  - HP
  - Telegraph time
  - Swipe damage, range, windup
  - Dash speed, dash distance, recover time
- LanternTuning
  - Scan range
  - Scan angle
  - Reveal time (instant or 0.5s “charge”)
  - Reveal persistence time (recommended 10–20 seconds)

---

## 4. Runtime architecture (clean and extendable)

### 4.1 Persistent layer (Bootstrap)

Bootstrap contains:

- GameState (plain C# + MonoBehaviour wrapper)
  - Holds: story flags, discovered note ids, revealed door ids, inventory counts, energy, current scene context
- SaveSystem (service)
- SceneDirector (service)
- InputRouter (service)
- UIRoot (canvas + panel controllers)
- DebugOverlay (dev only; can be toggled)

Rule:

- Persistent layer knows how to load scenes and save state.
- Content scenes do not create global managers.

### 4.2 Content layer (loaded scenes)

Each content scene contains:

- SceneContext (MonoBehaviour):
  - sceneName
  - isInterior bool
  - references to local anchors, optional local audio snapshot
- Respawn anchors (home bed or entrance)
- Interactables (notes, hidden doors, claw machine trigger)
- Encounter triggers (park zone)

Rule:

- Content objects read and write state through GameState and services.
- Content objects do not own persistence logic.

### 4.3 Communication

Use event-based communication; no direct cross-references when possible.
Examples:

- NoteDiscovered(noteId)
- DoorRevealed(doorId)
- InventoryChanged
- EnergyChanged
- TiredTriggered(context)
- PrizeGranted(reward)

Implementation options:

- C# events on GameState
- ScriptableObject event channels (fine if you prefer inspector wiring)

For PoC, C# events are simpler.

---

## 5. Performance and reliability guardrails

These should be enforced from day one.

### 5.1 Physics query rules

- Use layer masks:
  - Interactable
  - BlacklightReveal
  - Enemy
- Use NonAlloc queries where repeated:
  - Physics.OverlapSphereNonAlloc for interaction detection
  - Physics.OverlapSphereNonAlloc for lantern scanning
- Throttle expensive scans:
  - Interaction selection can run at 10 Hz instead of every frame.
  - Lantern scanning can run at 15–20 Hz; it still feels instant.

### 5.2 Object lifecycle rules

- Do not Instantiate/Destroy in combat loops.
- Pool small VFX (puff) and damage number popups if you add them.
- Raccoon spawns are few; pooling is optional but recommended if you respawn often during tests.

### 5.3 Save safety rules

- Save file has:
  - version
  - timestamp
  - payload
- Save writes are atomic:
  - write to temp file
  - flush
  - replace the main file
  - keep a backup file (previous save) if possible
- Load is defensive:
  - if corrupted, fall back to safe defaults and spawn at home bed

---

## 6. Revised build order

This sequence reduces rework and improves iteration speed.

1. Bootstrap scene + InputRouter + UIRoot skeleton + DebugOverlay
2. Cloverhollow blockout + Player movement + Camera
3. Interaction system (prompt + interact verb)
4. GameState + SaveSystem + SceneDirector + anchors + respawn
5. Lantern scanning + notes + journal
6. Combat controller + raccoon AI + encounter trigger
7. Maddie follower + assist
8. Arcade interior + claw machine (timing-based) + prizes
9. Polish; QA checklist; perf check on iPad target settings

---

## 7. Work items for OpenCode agents (revised)

### 7.1 Foundation: Bootstrap + UI root + input

Owner: @unity-engineer

Tasks

1. Create Bootstrap scene:
   - PersistentRoot object marked DontDestroyOnLoad
   - UIRoot canvas (HUD + prompt + journal panel + minigame panel placeholders)
   - InputRouter reads Input Actions and drives player or UI
2. Add DebugOverlay:
   - Toggle key (example: F1 on desktop) and a simple on-screen button on touch builds
   - Buttons:
     - Save, Load
     - Teleport Home
     - Grant Candy +1
     - Grant Gems +10
     - Toggle Lantern Unlocked
     - Spawn Raccoon (near player)
3. Create Input Actions asset with two maps: Gameplay and UI.

Acceptance criteria

- Bootstrap loads first; then loads Cloverhollow additively.
- DebugOverlay works in play mode.
- Input can drive UI panels (open/close) and gameplay actions (even if gameplay actions are stubbed).

---

### 7.2 Cloverhollow blockout + camera + movement

Owner: @unity-engineer

Tasks

1. Create Cloverhollow blockout using the sketch as reference:
   - Home area with bed anchor
   - Road to School entrance
   - Park area
   - Road to Arcade entrance
   - Blocked future paths with playful signage
2. CameraRig using Cinemachine:
   - Tilt 55–65 degrees
   - Soft follow + look-ahead
3. PlayerController:
   - Choose CharacterController for PoC (recommended for fewer physics surprises)
   - Smooth accel/decel; keep movement on ground plane
   - Facing based on movement direction

Acceptance criteria

- Movement feels stable and predictable.
- Camera never clips into ground; player stays centered.
- Runs at stable frame rate with placeholder assets.

---

### 7.3 Interaction system (with reliable targeting)

Owner: @unity-engineer

Revisions

- Instead of “nearest by overlap every frame,” pick best candidate by distance and angle with throttling.

Tasks

1. IInteractable interface stays.
2. Interactor:
   - Every 0.1 seconds:
     - OverlapSphereNonAlloc on Interactable layer
     - Score each candidate by distance and facing angle
   - Store “current target”
   - Show prompt for current target
3. Interact input:
   - Buffered for a short time window (0.1–0.2s) so taps feel responsive.

Acceptance criteria

- Prompt does not flicker when multiple interactables are nearby.
- Interact selects the expected object consistently.

---

### 7.4 GameState + save/load + scene transitions + respawn

Owner: @unity-engineer

Revisions

- Add stable GUIDs and versioned atomic save.

Tasks

1. PersistentId component:
   - Holds a GUID string
   - Editor-only utility: “Assign GUID if missing”
2. GameState (service):
   - storyFlags: HashSet<string>
   - discoveredNotes: HashSet<string>
   - revealedDoors: HashSet<string>
   - inventory: gems, candyBars
   - energy: current, max
   - currentSceneName
3. SaveData:
   - version int
   - sceneName
   - playerPosition, playerFacing
   - storyFlags list
   - discoveredNotes list
   - revealedDoors list
   - inventory
   - energy
4. SaveSystem:
   - Atomic writes
   - Backup previous save
5. SceneDirector:
   - LoadScene(sceneName, anchorId)
   - Handles fade in/out (simple canvas fade)
   - Maintains interior/exterior flag from GameConfig or SceneContext
6. RespawnSystem:
   - On tired:
     - if interior; load same interior and spawn at entrance anchor
     - else; load Cloverhollow and spawn at home bed anchor
   - Restore energy to a safe value (example: 50%)

Acceptance criteria

- Renaming scene objects does not break persistence because GUIDs are stable.
- Corrupt save falls back to home bed safely.
- Scene transitions do not duplicate UI because UIRoot is persistent.

---

### 7.5 Lantern scanning + notes + hidden doors + journal

Owners: @unity-engineer and @game-designer

Revisions

- Use a reveal interface and data-driven note content.
- Reveal requires a short scan time (recommended 0.3–0.7s) to prevent accidental discovery.

Tasks

1. IBlacklightRevealable:
   - string GetRevealId()
   - void OnRevealStart()
   - void OnRevealComplete()
2. BlacklightScanner:
   - Runs at 15–20 Hz when lantern is active
   - OverlapSphereNonAlloc on BlacklightReveal layer
   - For each candidate:
     - angle check
     - accumulate reveal progress
   - On complete:
     - mark discovered in GameState
3. NoteReveal:
   - Linked to NoteDefinition (by noteId)
   - On reveal complete:
     - show popup (title + 1–2 lines)
     - add to journal list
4. HiddenDoorReveal:
   - On reveal complete:
     - mark revealed
     - enable IInteractable on the door
5. Journal UI:
   - List view of discovered notes
   - Highlight “new” note until opened once

Content targets

- 8 notes is a good PoC target (not 6, not 10); it is enough to feel like a trail.
- 2 hidden doors:
  - School hidden door: leads to a tiny secret room with a reward
  - Park hedge gate shortcut: opens a path that saves walking time

Acceptance criteria

- Reveals persist across save/load.
- Reveal is readable and satisfying (popup + sound cue).
- Hidden doors never become interactable until revealed.

---

### 7.6 Energy + candy bars

Owner: @unity-engineer

Revisions

- Integrate energy changes with events so HUD updates automatically.

Tasks

1. EnergySystem in GameState:
   - currentEnergy, maxEnergy
   - TakeDamage, RestoreEnergy
2. Candy consumption:
   - Use candy from HUD quick button or inventory panel
   - Disallow use if candyBars == 0
3. HUD:
   - Energy bar/hearts
   - Candy count + use button
   - Gems count

Acceptance criteria

- Candy always restores the same amount and clamps to max.
- Tired triggers only once (no double respawn).

---

### 7.7 Combat controller + Chaos Raccoon AI + encounter

Owners: @unity-engineer and @game-designer

Revisions

- Use a small combat state machine to avoid input conflicts.
- Use IDamageable to unify player and enemy damage.

Tasks

1. IDamageable:
   - void ApplyDamage(int amount, Vector3 sourcePosition)
2. PlayerCombat:
   - States: Idle, Attacking, Dodging, Hurt, Tired
   - Attack combo windows; prevent attack during dodge; allow dodge cancel only if you want it (choose one)
   - Dodge:
     - fixed distance
     - invulnerable frames
     - cooldown
3. Hit detection:
   - Use a trigger hitbox spawned/enabled during attack frames
   - Track already-hit targets per swing to avoid multi-hit spam
4. Raccoon AI:
   - States: Patrol, Chase, Telegraph, Swipe, DashPast, Recover
   - Telegraph time is tuneable and must be obvious
5. Encounter trigger:
   - One park encounter zone that spawns a raccoon when entered
   - Optional: cooldown before respawn if player leaves and re-enters quickly

Acceptance criteria

- The first encounter teaches dodge.
- Getting hit feels fair; telegraph is obvious.
- No jitter or physics explosions.

---

### 7.8 Maddie follower + assist

Owner: @unity-engineer

Revisions

- Make Maddie purely cosmetic outside combat and only assist when an enemy is engaged to avoid constant polling.

Tasks

1. Follow:
   - simple spring follow or “arrive” steering
2. Assist:
   - subscribe to “EnemyEngaged” event
   - on cooldown; do a small dash; apply small damage
3. Visual feedback:
   - small puff VFX and a friendly sound

Acceptance criteria

- Maddie never blocks the player.
- Assist feels like a bonus; it should not solo the raccoon.

---

### 7.9 Arcade + claw machine mini-game (timing-based)

Owners: @unity-engineer and @game-designer

Revisions

- Still simple; but include one skill input.

Mechanic

- A target marker oscillates left-right.
- Player taps “Drop.”
- Distance from center determines reward tier.
- Reward tier maps to PrizeTable weighted results.

Tasks

1. ArcadeInterior scene + interactable claw machine
2. ClawMachine UI panel
   - oscillating marker
   - drop button
   - reward reveal
3. PrizeTable integration
4. Reward grant updates inventory and saves

Acceptance criteria

- Player influence matters.
- Rewards still vary within tier so it stays interesting.

---

### 7.10 QA and automated checks

Owner: @qa-playtest

Revisions

- Add at least two automated checks; it saves time later.

Manual regression checklist

- Same as original checklist; keep it.

Automated checks (minimal)

- Unit test: SaveData serialize/deserialize round trip
- PlayMode smoke test:
  - load Bootstrap
  - load Cloverhollow
  - assert player spawns at home anchor

Acceptance criteria

- Tests run in batch mode and catch obvious breakages early.

---

## 8. PoC walkthrough script (final)

1. Spawn at home bed.
2. Toggle lantern and reveal a note near home or on the road.
3. Enter school; reveal two notes; reveal and open a hidden door; collect a small reward.
4. Exit to park; trigger raccoon; dodge at least one swipe; defeat raccoon.
5. Eat a candy bar after taking damage.
6. Get tired outdoors; respawn at home bed.
7. Enter arcade; play claw machine; win gems or candy.
8. Save; quit; load; resume at exact position with the same inventory and discovered notes.

---

## 9. Updated risk controls

- Do not add a quest system; use notes as the trail.
- Do not add more enemies; ship raccoon only.
- Do not add more currencies; gems and candy only for PoC.
- Do not add lasso/flute; only stubs.
- Do not create cross-scene references; everything goes through GameState and services.
- Keep per-frame allocations near zero; watch the profiler early.
