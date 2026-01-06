---
name: godot
description: Godot Engine game development. Use for Godot projects, GDScript/C#, scenes (.tscn), Resources, signals, UI, rendering, and macOS/iOS export.
---

# Godot Engine Development Skill

Assistance with Godot 4 game development, covering scenes/nodes, GDScript architecture, UI flows, data-driven Resources, rendering/performance, and platform exports (macOS + iOS).

## When to Use This Skill

Trigger this skill when working with:
- Godot Engine projects (2D/3D)
- GDScript (preferred) or C# in Godot
- Scene composition (.tscn), Node hierarchies, and signals
- Resources (.tres/.res) for data-driven definitions (items, stickers, enemies)
- UI (Control nodes), input, and focus/touch handling
- Performance profiling, rendering settings, and asset import
- Export pipelines (macOS app bundles, iOS Xcode projects)

## Project Conventions (Wilds of Cloverhollow)

- Scenes are small and composable; prefer reusable child scenes.
- Keep gameplay rules out of UI Nodes.
- Combat rules should be deterministic and testable.
- Use Resources for content definitions:
  - `StickerDef`, `EnemyDef`, `EncounterDef`
- Use autoload singletons sparingly:
  - `GameState` (save/load + persistent inventory)
  - `SceneRouter` (scene transitions)

## Quick Reference

### Node lifecycle

```gdscript
extends Node

func _ready() -> void:
    pass

func _process(delta: float) -> void:
    pass

func _physics_process(delta: float) -> void:
    pass
```

### Signals

```gdscript
signal sticker_used(sticker_id: String)

func use_sticker(id: String) -> void:
    sticker_used.emit(id)
```

### Resources for data

```gdscript
extends Resource
class_name StickerDef

@export var id: String
@export var display_name: String
@export var description: String
@export var power: int
@export var cooldown_turns: int
```

### Saving (minimal)

```gdscript
var save_path := "user://save.json"

func save_json(data: Dictionary) -> void:
    var f := FileAccess.open(save_path, FileAccess.WRITE)
    f.store_string(JSON.stringify(data))

func load_json() -> Dictionary:
    if not FileAccess.file_exists(save_path):
        return {}
    var f := FileAccess.open(save_path, FileAccess.READ)
    return JSON.parse_string(f.get_as_text())
```
