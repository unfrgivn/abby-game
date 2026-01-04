#!/usr/bin/env bash
set -euo pipefail

TARGET="${1:-macos}"
UNITY_PATH="${UNITY_PATH:-}"
PROJECT_PATH="${UNITY_PROJECT_PATH:-UnityProject}"

if [[ -z "$UNITY_PATH" ]]; then
  echo "UNITY_PATH is not set"
  exit 1
fi

echo "Building for ${TARGET}..."
# Wire this to a BuildScript method you create in Unity, for example:
# -executeMethod BuildScripts.BuildMacOS
# Keep this stub minimal until the build pipeline exists.

"$UNITY_PATH" \
  -batchmode -nographics -quit \
  -projectPath "$PROJECT_PATH" \
  -logFile -
