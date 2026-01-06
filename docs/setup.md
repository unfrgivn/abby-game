# Setup

This repo is intended to be opened and built with **Godot 4** and OpenCode.

## 1) Prereqs
- Godot 4.x installed (pin a specific version for the project)
- Git
- macOS is the primary dev environment
- For iOS export (later): Xcode + an Apple Developer account + Godot export templates

## 2) Choose and pin Godot version
Pick a Godot 4.x version and record it in two places:
1) `docs/setup.md` (this file)
2) `GodotProject/GODOT_VERSION.txt` (create this file once the Godot project exists)

Recommended: choose a stable 4.x release and do not upgrade during M0/M1.

Fill in here once chosen:
- Godot version: 4.x.x

## 3) Project path conventions
This scaffold assumes:
- Godot project folder: `./GodotProject`
- The file `GodotProject/project.godot` exists once the project is created

If your Godot project folder is different, update:
- `AGENTS.md` (repo layout expectations)
- `scripts/godot_test.sh` and `scripts/godot_export.sh`
- `.opencode/tool/godot.ts`

## 4) Environment variables
Used by the scripts and optional OpenCode tools:

- `GODOT_PATH`
  - Path to the Godot executable (or just `godot` if it is on PATH)

- `GODOT_PROJECT_PATH` (optional)
  - Defaults to `GodotProject`
  - Must point at the folder that contains `project.godot`

## 5) Running tests (headless)
From repo root:

- Run all tests / smoke boot:
  - `./scripts/godot_test.sh all`

Notes:
- If the project does not exist yet, the script exits without error.
- If you add a unit test framework (recommended: GUT), document the install steps here and ensure tests live under `GodotProject/tests/`.

## 6) Exports (stub)
Exports require that you configure **Export Presets** inside the Godot project:
- In Godot: `Project -> Export...`
- Add presets for:
  - `macOS` (preset name must be exactly `macOS`)
  - `iOS` (preset name must be exactly `iOS`)

From repo root:
- macOS:
  - `./scripts/godot_export.sh macos`
- iOS (later):
  - `./scripts/godot_export.sh ios`

## 7) First-time Godot project boot (M0)
Minimum checklist:
- Create/open the Godot project at `GodotProject/`
- Set the main scene (Project Settings -> Application -> Run)
- Configure InputMap for:
  - Move (WASD + virtual joystick)
  - Interact
  - Lantern
  - Journal
  - Battle: confirm/select/back
- Create a placeholder world scene with:
  - Simple ground mesh + a few props
  - Player placeholder
  - Fixed-tilt Camera3D (60° tilt) with smooth follow

## 8) Repo hygiene
- Do not commit Godot generated caches (`.godot/` / `.import/`).
- Commit project assets and scenes so the project is reproducible.
- Keep early placeholders small; avoid large binaries until the pipeline is stable.
