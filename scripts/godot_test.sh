#!/usr/bin/env bash
set -euo pipefail

SUITE="${1:-all}"
PROJECT_PATH="${GODOT_PROJECT_PATH:-GodotProject}"
GODOT_EXE="${GODOT_PATH:-godot}"

if [[ ! -f "${PROJECT_PATH}/project.godot" ]]; then
  echo "Godot project not found at '${PROJECT_PATH}/project.godot'"
  echo "(No tests to run yet.)"
  exit 0
fi

if ! command -v "${GODOT_EXE}" >/dev/null 2>&1 && [[ ! -x "${GODOT_EXE}" ]]; then
  echo "Godot executable not found: '${GODOT_EXE}'"
  echo "Set GODOT_PATH to your Godot executable (or ensure 'godot' is in PATH)."
  exit 1
fi

# If GUT (Godot Unit Test) is installed, run it headless.
if [[ -f "${PROJECT_PATH}/addons/gut/gut_cmdln.gd" ]]; then
  echo "Running GUT tests (${SUITE})..."
  "${GODOT_EXE}" --headless --quit --path "${PROJECT_PATH}" -s res://addons/gut/gut_cmdln.gd -gdir=res://tests
  exit 0
fi

# Otherwise, do a minimal smoke boot (opens the project headless and quits).
# This is a placeholder until unit tests are added.
echo "No test runner configured (GUT not found). Running headless smoke boot..."
"${GODOT_EXE}" --headless --quit --path "${PROJECT_PATH}"
