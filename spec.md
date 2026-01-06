# Wilds of Cloverhollow

Cozy, kid-friendly top-down 3D adventure with tool-driven secrets, a scrapbook journal, and **turn-based Sticker battles** (EarthBound / classic Final Fantasy cadence) triggered by **visible overworld encounters**.

## 1. Vision

A calm, wholesome adventure about a kid balancing everyday life (school, family, town) with a magical “secret” quest to calm chaos spreading. Magic realism and dreamlike moments, grounded in kid everyday reality.

Tone rules:
- Cozy, safe, friendly
- Mischievous chaos; not scary
- Short, kid-friendly dialog; humor welcome
- Family-friendly; no gore; no horror

## 2. Target audience

- Kids 8 to 12
- Family-friendly

## 3. Inspirations

- EarthBound: kid-centric story tone, humor, quality-of-life battle ideas
- Final Fantasy (classic): turn-based pacing, clear menus, party identity
- Cassette Beasts: modern overworld + turn-based battle structure and collectible move identity
- Zelda: exploration, tool gating, readable overworld navigation
- Lil Gator / Alba: playful exploration, charm-forward world

## 4. Product goals

- Dense zones that feel “open” without being huge
- Tool-driven puzzle progression (Blacklight Lantern, then Lasso, then Flute)
- **Turn-based battles** that are fast, non-scary, and mobile-friendly
- **Sticker moves** as a lightweight build system (equip a small loadout)
- No random encounters; enemies are visible in the overworld
- Easy-to-add content: NPCs, micro-quests, collectibles, sticker drops
- Save-anywhere; stop and resume exactly where you were

## 5. Non-goals for the proof-of-concept (PoC)

- No complex RPG grinding or long battle chains
- No fusion/combinatorics systems (keep Sticker system simple in PoC)
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
- Twist (later): the bad guy is actually a kid friend
- Final twist (later): it was all a dream when mom wakes Fae up

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
3. Use tools (Lantern in PoC) to reveal notes/doors
4. Trigger a visible encounter → short turn-based battle
5. Earn rewards (gems, stickers, story progress)
6. Unlock new interactions, areas, tools, and story beats

## 10. Player verbs

Always:
- Move (walk/run)
- Interact (talk, pick up)
- Journal (clues and quest steps)

Often:
- Use tool (Blacklight Lantern)
- Battle commands (Stickers / Items / Defend / Run)

PoC must include:
- Movement + interact
- Journal v0
- Blacklight Lantern v0
- One battle encounter type (Chaos Raccoon) implemented as **turn-based**
- Sticker Book v0 (inventory + equip 4 stickers)
- Arcade claw machine v0

## 11. Camera and controls

Camera:
- Fixed top-down camera at 60° tilt (classic SNES readability)
- No player-controlled camera rotation
- Camera follows player smoothly with slight lag
- “Living diorama” perspective; readable and simple

Input (macOS + iOS):
- Touch-first with optional keyboard/mouse and controller support
- Overworld:
  - Move: left virtual joystick (touch) or WASD
  - Interact: large action button
  - Lantern toggle/hold: large tool button
  - Journal: dedicated button
- Battle:
  - Large command buttons
  - Sticker grid buttons sized to avoid mis-taps
  - Confirm/Back always available

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
- Dialog routing (talk to the right adult)
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

## 15. Stickers (moves) and Sticker Book

Stickers are collectible move “cards” stored in Fae’s scrapbook. Stickers are the primary battle actions.

### 15.1 Sticker definitions

Each sticker has:
- `id` (stable string)
- `name`
- `description` (1 sentence)
- `type`: Attack / Support / Utility
- `targeting`: Single enemy / All enemies / Self / Ally
- `power` (simple integer)
- `cooldown_turns` (0–3 in PoC)

PoC simplifications:
- No sticker durability/breaking
- No fusion/combo stickers
- No elemental chart (unless required for one tutorial beat)

### 15.2 Sticker inventory + loadout

- Player can own many stickers.
- Player equips **4 stickers** into an active battle loadout.
- Battle UI shows only the equipped stickers.
- Stickers on cooldown are disabled with clear UI.

PoC sticker set (minimum):
- Starter (granted on new game):
  - Bonk (Attack)
  - Glitter Bandage (Support)
  - Pocket Sand (Utility)
- First battle reward:
  - Raccoon Dash (Attack/Utility)

### 15.3 Rewards

- Winning a PoC encounter grants:
  - Candy gems (currency)
  - A sticker reward (first win only)

## 16. Battles (turn-based, non-scary)

Battles are turn-based, fast, and readable.

### 16.1 Encounter rules

- Enemies are visible in the overworld.
- Touching/engaging an enemy triggers a battle transition.
- Optional (later): back-attack / advantage based on engagement direction.

### 16.2 Battle commands

Player commands:
- **Stickers** (use equipped sticker)
- **Items** (simple healing item in PoC)
- **Defend** (reduced damage for one turn)
- **Run** (allowed; returns to overworld near the encounter)

### 16.3 Turn order

- Deterministic turn order based on `speed`.
- PoC can use a simple alternating model if speed is not implemented yet, but the API must support speed later.

### 16.4 Win / loss framing

- Win: enemy is “calmed” and flees; rewards granted.
- Loss: Fae gets “tired” and respawns (no death).

Respawn rules (PoC):
- If inside a named interior location (School, Arcade), respawn at that location’s entrance
- If outside (town and park), respawn in bed at home

### 16.5 PoC enemy

- 1 enemy family: Chaos Raccoon
- 1 to 2 encounters placed outdoors (Park and/or outside Arcade)

## 17. Saving

- Save anywhere.
- Loading restores:
  - Player location + current scene
  - Discovered notes and unlocked doors
  - Sticker inventory + equipped loadout
  - Currency count
  - Respawn anchor

PoC must never:
- Corrupt save data
- Soft-lock player (always able to return to overworld)

