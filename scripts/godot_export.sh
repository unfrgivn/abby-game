#!/usr/bin/env bash
set -euo pipefail

TARGET="${1:-macos}"
PROJECT_PATH="${GODOT_PROJECT_PATH:-GodotProject}"
GODOT_EXE="${GODOT_PATH:-godot}"

if [[ ! -f "${PROJECT_PATH}/project.godot" ]]; then
  echo "Godot project not found at '${PROJECT_PATH}/project.godot'"
  echo "Set GODOT_PROJECT_PATH to the folder that contains project.godot."
  exit 1
fi

if ! command -v "${GODOT_EXE}" >/dev/null 2>&1 && [[ ! -x "${GODOT_EXE}" ]]; then
  echo "Godot executable not found: '${GODOT_EXE}'"
  echo "Set GODOT_PATH to your Godot executable (or ensure 'godot' is in PATH)."
  exit 1
fi

OUT_DIR="builds/${TARGET}"
mkdir -p "${OUT_DIR}"

case "${TARGET}" in
  macos)
    OUT_PATH="${OUT_DIR}/WildsOfCloverhollow.app"
    echo "Exporting macOS build to ${OUT_PATH}..."
    "${GODOT_EXE}" --headless --quit --path "${PROJECT_PATH}" --export-release "macOS" "${OUT_PATH}"
    ;;
  ios)
    OUT_PATH="${OUT_DIR}/WildsOfCloverhollow"
    echo "Exporting iOS Xcode project to ${OUT_PATH}..."
    echo "Note: iOS export requires an iOS preset configured in the Godot project."
    "${GODOT_EXE}" --headless --quit --path "${PROJECT_PATH}" --export-release "iOS" "${OUT_PATH}"
    ;;
  *)
    echo "Unknown export target: ${TARGET} (expected: macos|ios)"
    exit 1
    ;;
esac
