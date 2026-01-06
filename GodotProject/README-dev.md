# Wilds of Cloverhollow - Developer Guide

## Prerequisites

- **Godot 4.3** (stable) - [Download](https://godotengine.org/download)
- Git

## Quick Start

1. Open Godot 4.3
2. Click "Import" and navigate to `GodotProject/`
3. Select `project.godot` and open
4. Press F5 or click the Play button to run

## Project Structure

```
GodotProject/
├── assets/          # Textures, models, audio, fonts
├── data/            # Resource definitions (.tres)
│   ├── stickers/    # StickerDef resources
│   ├── enemies/     # EnemyDef resources
│   ├── encounters/  # EncounterDef resources
│   ├── notes/       # NoteDef resources (blacklight)
│   ├── doors/       # DoorDef resources (hidden doors)
│   └── tables/      # PrizeTable resources
├── scenes/          # Scene files (.tscn)
│   ├── bootstrap/   # Main.tscn entry point
│   ├── world/       # Overworld scenes
│   ├── battle/      # Battle system scenes
│   └── ui/          # UI scenes
├── scripts/         # GDScript files (.gd)
│   ├── bootstrap/   # Entry point scripts
│   ├── core/        # Autoloads (GameState, SaveSystem, SceneRouter)
│   ├── overworld/   # Player, interactables, lantern
│   ├── battle/      # BattleState, BattleDirector, AI
│   └── ui/          # UI controllers
└── tests/           # Unit tests (GUT framework)
```

## Input Map

| Action            | Key       | Description                    |
|-------------------|-----------|--------------------------------|
| move_up           | W         | Move player up                 |
| move_down         | S         | Move player down               |
| move_left         | A         | Move player left               |
| move_right        | D         | Move player right              |
| interact          | E         | Interact with objects/NPCs     |
| lantern           | Q         | Toggle blacklight lantern      |
| journal           | J         | Open journal                   |
| open_sticker_book | I         | Open sticker book              |
| ui_accept         | Enter/Space | Confirm selection            |
| ui_cancel         | Escape    | Cancel/back                    |
| ui_pause          | Tab       | Pause menu                     |

## Running Tests

From the repo root:

```bash
./scripts/godot_test.sh all
```

## Exporting Builds

Requires export presets configured in Godot (Project → Export).

```bash
# macOS
./scripts/godot_export.sh macos

# iOS (requires Xcode + Apple Developer account)
./scripts/godot_export.sh ios
```

## Godot Version

This project uses **Godot 4.3**. See `GODOT_VERSION.txt` for the exact version.

Do not upgrade Godot version during M0/M1 milestones without team discussion.

## Architecture Notes

- **Main.tscn** is the bootstrap scene that never unloads
- Content scenes load via **SceneRouter** autoload
- Battle is a dedicated scene: **BattleScene.tscn**
- All persistent data uses stable IDs for save/load
- Battle rules live in pure script classes (no Node dependencies) for testability

## Related Documentation

- `spec.md` - Product specification
- `docs/poc-plan.md` - PoC build plan
- `docs/poc-content-map.md` - Content requirements
- `docs/poc-regression-checklist.md` - QA checklist
