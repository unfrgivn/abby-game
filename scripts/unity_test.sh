#!/usr/bin/env bash
set -euo pipefail

MODE="${1:-playmode}"
UNITY_PATH="${UNITY_PATH:-}"
PROJECT_PATH="${UNITY_PROJECT_PATH:-UnityProject}"

if [[ -z "$UNITY_PATH" ]]; then
  echo "UNITY_PATH is not set"
  exit 1
fi

echo "Running Unity ${MODE} tests..."
"$UNITY_PATH" \
  -batchmode -nographics -quit \
  -projectPath "$PROJECT_PATH" \
  -runTests \
  -testPlatform "$MODE" \
  -logFile -
