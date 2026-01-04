# Tiny Wilds (working title)

Lil Gator-style, top-down 3D cozy adventure with tool-driven puzzles and simple real-time combat

## 1. Vision

A calm, wholesome adventure about a kid balancing everyday life (school, family, town) with a magical “secret” quest to stop chaos spreading. Magic realism and dreamlike moments, grounded in kid everyday reality.

Tone rules:

- Cozy, safe, friendly
- Mischievous chaos; not scary
- Short, kid-friendly dialog; humor welcome
- Family-friendly; no gore; no horror

## 2. Target audience

- Kids 8 to 12
- Family-friendly

## 3. Inspirations

- Zelda: exploration, tool gating, readable combat
- Lil Gator / Alba: playful exploration, charm-forward world
- Animal Crossing / Stardew: cozy town vibe and approachable tasks
- Earthbound: kid-centric story tone and humor

## 4. Product goals

- Dense zones that feel “open” without being huge
- Tool-driven puzzle progression (Blacklight Lantern, then Lasso, then Flute)
- Simple real-time combat that is non-scary and quick (Light Attack + Dodge)
- Easy-to-add content: NPCs, micro-quests, collectibles, puzzle set pieces
- Save-anywhere; stop and resume exactly where you were

## 5. Non-goals for the proof-of-concept

- No complex RPG builds or grinding
- No procedural generation
- No timers as fail states
- No scary enemies or dark imagery
- No advanced base building (clubhouse later)
- No travel to mountains, ocean, island (blocked in PoC)

## 6. Setting

Calming biomes:

- Meadows, forest, ponds, hills, caves

World fantasy:

- Small town plus nearby nature zones
- “Chaos” is a spell that makes things goofy and unruly

## 7. High-level story (Hero’s Journey outline)

- Start: Fae wakes up at home (ordinary life)
- Chaos begins showing up around town and school
- Constraint: school responsibilities; must solve “how to get out” via story puzzles
- Allies: other kids join the party later (Sue, Jordan)
- Twist: the bad guy is actually a kid friend
- Final twist: it was all a dream when mom wakes Fae up

PoC narrative rule:

- Only establish the premise and the first “lantern mystery.” Do not attempt major twists in PoC.

## 8. Main character

Fae:

- Age 10; loves art, music, animals, nature
- Brave, curious, kind-hearted, smart; inventor/builder vibe
- Backpack with journal and tools

Animal companion:

- Maddie the kitten follows initially

## 9. Core gameplay loop

1. Explore a zone
2. Talk to NPCs; discover a problem or clue
3. Solve a puzzle or complete a micro-quest
4. Earn rewards (gems, items, story progress)
5. Unlock new interactions, areas, tools, and story beats

## 10. Player verbs

Always:

- Move (walk/run)
- Interact (talk, pick up, use)
- Journal (clues and quest steps)

Often:

- Use tool (Lantern, later Lasso, later Flute)
- Combat (simple real-time)

PoC must include:

- Movement + interact
- Journal v0
- Blacklight Lantern v0
- One combat encounter type (Chaos Raccoon)
- Arcade claw machine v0

## 11. Camera and controls

Camera:

- Perspective
- Tilt: 55 to 65 degrees downward
- Soft follow with slight look-ahead
- Avoid tall occluders; prefer low fences and short trees

Input:

- Touch-first with optional keyboard and mouse
- Combat buttons: Attack, Dodge
- Tool button: Blacklight Lantern toggles on/off or hold-to-scan (final choice made during implementation)
- Flute opens animal selection menu (later milestone)

## 12. World and locations

Long-term world includes multiple towns and a central gated Enchanted Forest. PoC is intentionally small.

### 12.1 PoC playable scope

- Hero’s House (start; includes bed respawn anchor)
- School (interior; includes entrance respawn anchor)
- Park/Playground (outdoor)
- Arcade (interior; includes entrance respawn anchor)

### 12.2 PoC navigation requirements

- One obvious main road connecting Home, School, Park, Arcade
- One simple loop path to reduce backtracking
- Clear blocked paths to future areas (mountains, ocean, enchanted forest)

## 13. School (story-driven gating)

School is not a schedule simulation; it appears as story scenes and puzzle sequences.

School puzzle patterns:

- Find items (hall pass, art supplies, note pages)
- Dialog routing (talk to the right adult with the right phrasing)
- Invisible ink clue discovery (lantern)
- Hidden door discovery (lantern)

PoC must include:

- At least 2 blacklight notes in school
- At least 1 hidden door in school

## 14. Tool progression

Tool progression is the main gating system:

1. Blacklight Lantern (PoC scope)
2. Lasso (later)
3. Flute (later)

### 14.1 Blacklight Lantern (PoC scope)

What it does:

- Reveals hidden notes (invisible ink)
- Reveals hidden doors (UV symbols activate doorways)

Lantern interactions:

- Hidden note: scanning reveals a UV doodle and triggers a short UI popup (text + simple icon)
- Hidden door: scanning reveals a UV symbol; once revealed, the door becomes interactable

PoC must include:

- 6 to 10 hidden notes total across PoC map
- 2 hidden doors total (School plus Park or Arcade)

## 15. Animal companions

Rules:

- One animal follows you at a time
- Maddie the kitten follows you initially
- Active animal automatically helps in fights
- Bonding and multiple animals come later; PoC only needs Maddie as a follower and combat helper

## 16. Combat (simple live, non-scary)

Combat is real-time, not turn-based.

Control scheme:

- Light attack
- Dodge roll with brief invulnerability

Combat constraints:

- Short encounters; no grind
- Player never “dies”; player gets “tired” and respawns
- No loss of items or currency on respawn for PoC

Respawn rules:

- If inside a named interior location (School, Arcade), respawn at that location’s entrance
- If outside (town and park), respawn in bed at home

PoC must include:

- 1 enemy family: Chaos Raccoon
- 1 to 2 encounters placed outdoors (park or outside arcade)

## 17. Enemies (PoC)

Chaos Raccoon:

- Cute silhouette; playful animations; no scary face
- Teaches dodge timing quickly

Suggested behavior:

- Approach, telegraph swipe, swipe, dash past player, reposition

## 18. Arcade mini-game (PoC)

Arcade exists for fun and prizes; it is not progression gating.

PoC includes:

- Claw machine only
- Prize pool: gems and candy bars

Candy bars:

- Consumable; restore energy (exact values set during implementation)

## 19. Progression system (story flags)

Use story flags as gating; explicit and simple.

Examples:

- Tool.Lantern.Unlocked
- School.UVNotesFound
- School.HiddenDoorOpened
- Park.HiddenDoorOpened
- Arcade.ClawMachine.Introduced
- Combat.Raccoon.FirstDefeated

## 20. Save system (evolves with features)

Goal: save-anywhere; resume exactly where you were.

Approach:

- Start with a minimal safe payload
- Add fields only when related features ship
- Backward-compatible defaults for missing fields

PoC minimum save payload:

- Scene or zone id
- Player position and facing
- Story flags
- Inventory: gems, candy bars, tools unlocked
- Player energy state
- Respawn anchors (home bed id; entrance ids for interiors)
- Enemy encounter state is optional in PoC (it is acceptable for enemies to respawn)

## 21. Art direction (production rules)

- Chunky proportions, toy-like silhouettes
- Flat or lightly painted materials; minimal texture detail
- Bright but calm palette; limited accents
- Props can feel kid-made (cardboard, tape, doodles, stickers)
- Bouncy animation; strong idles

## 22. Tech stack

- Unity LTS (pin exact version)
- URP
- Input System
- Cinemachine
- Keep dependencies minimal

## 23. Proof-of-concept definition of done

A player can:

- Start at Fae’s house
- Walk to school; discover blacklight notes; open at least one hidden door
- Walk to park; complete at least one lantern interaction and one raccoon encounter
- Enter arcade; play claw machine; win gems or candy bars
- Use candy bar to restore energy
- Get “tired” in combat; respawn correctly (interior entrance or home bed)
- Save anywhere; quit; reload; resume in the same spot with the same inventory and flags
