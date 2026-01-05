
# Setup

This repo is intended to be opened and built with Unity (URP) and OpenCode.

## 1. Prereqs
- Unity Hub installed
- A pinned Unity LTS editor installed (choose one and keep it fixed)
- Git
- macOS is the primary dev environment; iPad/iOS builds are optional later

## 2. Choose and pin Unity version
Pick a Unity LTS version and record it in two places:
1) UnityProject/ProjectSettings/ProjectVersion.txt (Unity writes this)
2) docs/setup.md (this file)

Recommended: use an LTS line and do not upgrade during M0/M1.

Fill in here once chosen:
- Unity version: 6000.3.2f1

## 3. Project path conventions
This scaffold assumes:
- Unity project folder: ./UnityProject

If your Unity project folder is different, update:
- AGENTS.md (Repo layout expectations)
- scripts/unity_test.sh and scripts/unity_build.sh (PROJECT_PATH default)
- .opencode/tool/unity.ts (projectPath default)

## 4. Environment variables
These are used by the scripts and optional OpenCode tools:

- UNITY_PATH
  Path to the Unity executable

  Typical macOS pattern:
  /Applications/Unity/Hub/Editor/<version>/Unity.app/Contents/MacOS/Unity

- UNITY_PROJECT_PATH (optional)
  Defaults to UnityProject

## 5. Running tests (batchmode)
From repo root:
- Playmode tests:
  ./scripts/unity_test.sh playmode
- Editmode tests:
  ./scripts/unity_test.sh editmode

If you have no tests yet, these commands will still validate the project can open in batchmode.

## 6. Builds (stub)
The build script is intentionally a stub until you add a BuildScripts class in Unity.

From repo root:
- macOS:
  ./scripts/unity_build.sh macos
- iOS (later):
  ./scripts/unity_build.sh ios

When you are ready:
- Create a C# editor script with build methods
- Wire scripts/unity_build.sh to call -executeMethod

## 7. First-time Unity project boot (M0)
Minimum checklist:
- Open the Unity project
- Install/confirm URP
- Enable Input System package and set Active Input Handling appropriately
- Add Cinemachine package
- Create a Main scene with:
  - Ground plane and a few props
  - Player capsule placeholder
  - Top-down camera follow

## 8. Repo hygiene
- Do not commit Library/ or Temp/ folders.
- Commit ProjectSettings/ and Packages/ so the project is reproducible.
- Avoid adding big binary assets early; keep placeholders small.
