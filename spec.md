
# Tiny Wilds (working title)
Lil Gator-style, top-down 3D cozy adventure in Unity

## 1. Product goals
- A chill exploration game for an 8-year-old to co-create.
- Short play loop: walk around, meet characters, do tiny quests, collect cute stuff.
- Strong readability from a top-down camera.
- Content should be easy to add: props, NPCs, quests, collectibles.

## 2. Non-goals (for the first release)
- Combat.
- Procedural generation.
- Complex physics puzzles.
- Online multiplayer.
- Full “open world” scale. We will fake it with a small, dense map.

## 3. Target platforms
- Primary: macOS (keyboard and mouse).
- Secondary: iPad (touch).
- Optional later: iPhone (only if UI and readability hold up).

## 4. Core gameplay loop
1) Explore a small “park world”
2) Find NPCs with simple needs
3) Complete micro-quests (2 to 5 minutes)
4) Earn rewards (stickers, badges, costume bits)
5) Unlock small world changes (new props, new NPCs, new zones)

## 5. Player verbs
- Move (walk/run)
- Interact (talk, pick up, use)
- Hop (optional, low stakes traversal)
- Tool (one tool only for v1; choose one)
  - Camera tool (take photos of critters)
  - Magnet hand (pull “stickers” / trinkets)
  - Net (catch butterflies, no failure)

## 6. World structure
World is a dense playground, not a wilderness simulation.
- Zone A: Meadow Park (start zone)
- Zone B: Beach Cove (unlocks after 3 to 5 quests)
- Zone C: Clubhouse Hill (unlocks after a “big” quest chain)

Each zone has:
- 1 landmark visible from afar
- 2 to 3 quest NPCs
- 1 collectible theme
- 3 to 6 “photo subjects” or “spotting targets”

## 7. Quests (v1 patterns)
Keep quests templated.
- Fetch: find 3 items
- Spotting: photo or “observe” 2 critters
- Delivery: take X to NPC Y
- Decorate: place 3 stickers/props on a board
- Build: place 3 cardboard pieces to make a ramp

No fail states. No timers. No “wrong answers.”

## 8. Progression and rewards
- Reward types:
  - Stickers (collectible currency)
  - Badges (quest completion stamps)
  - Costume bits (hat, backpack, toy sword)
- Gating:
  - New zone unlocks after N badges
  - New NPCs appear after zone unlocks
- Save:
  - Autosave after quest completion and zone transitions

## 9. Art direction (Lil Gator, top-down 3D)
- Chunky proportions, toy-like silhouettes
- Flat or lightly painted materials, minimal texture detail
- Bright but calm palette; limited accents
- World props feel “kid-made”: cardboard, tape, doodles, stickers
- Animation is bouncy; idles matter

## 10. Camera
- Perspective camera
- Tilt: 55 to 65 degrees downward
- Soft follow and slight look-ahead
- Avoid tall occluders; prefer low fences and short trees

## 11. UX and UI
- One big interact button on touch
- Simple quest tracker (one active quest at a time for v1)
- Map is optional for v1; prefer landmark signage and obvious paths
- Sticker book screen:
  - Shows badges earned
  - Shows collectibles count per zone

## 12. Audio
- Ambient loops per zone
- Friendly UI sounds
- Simple “reward pop” sound
- Minimal dialog; rely on short text bubbles

## 13. Tech stack (Unity)
- Unity (LTS preferred; pin exact version once chosen)
- URP
- Input System (supports touch + keyboard/mouse)
- Cinemachine (camera follow)
- Addressables optional later (do not start with it)

## 14. Architecture (v1)
Keep it boring and modular.
- Player:
  - PlayerController (movement + facing)
  - Interactor (ray/overlap for interact targets)
- Interactables:
  - IInteractable interface
  - Simple components: Talkable, Pickup, Useable
- Quest system:
  - QuestDefinition (data)
  - QuestRuntimeState (saveable)
  - QuestManager (single active quest)
- World state:
  - Simple flags (badge counts, zone unlocked flags)
- Save system:
  - JSON file per profile (single slot is fine)

## 15. Milestones
M0: Project boot
- Unity project created, URP configured, input system set
- Top-down camera follow working

M1: Vertical slice (Meadow Park)
- Player movement + interact
- 2 NPCs
- 1 collectible type
- 1 quest chain (3 micro-quests)
- Autosave
- Simple sticker book screen

M2: Content scaling
- Quest templates implemented (fetch, spot, delivery, decorate)
- Add Beach Cove zone

M3: Polish
- Animation pass, VFX puffs, UI bounce
- Basic performance pass
- Build pipeline

## 16. Quality bar (Definition of Done for a feature)
- Works on macOS and iPad controls
- No hard locks
- Saves and loads correctly
- Readable from camera distance
- Minimal settings, minimal dependencies
