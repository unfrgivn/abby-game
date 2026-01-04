
import { tool } from "@opencode-ai/plugin"

/*
  This is a scaffold.
  You will likely need to set UNITY_PATH (path to Unity executable) and UNITY_PROJECT_PATH.
  The execute bodies can be wired to whatever scripts you create under ./scripts.
*/

const unityPath = process.env.UNITY_PATH
const projectPath = process.env.UNITY_PROJECT_PATH ?? "UnityProject"

export const info = tool({
  description: "Print Unity tool configuration expected by this repo",
  args: {},
  async execute() {
    return [
      "Unity tool config:",
      `UNITY_PATH=${unityPath ?? "<unset>"}`,
      `UNITY_PROJECT_PATH=${projectPath}`,
      "",
      "Set UNITY_PATH to your Unity executable path.",
      "Example macOS (typical): /Applications/Unity/Hub/Editor/<version>/Unity.app/Contents/MacOS/Unity",
    ].join("\n")
  },
})

export const test = tool({
  description: "Run Unity tests in batchmode via ./scripts/unity_test.sh",
  args: {
    mode: tool.schema.enum(["editmode", "playmode"]).default("playmode"),
  },
  async execute(args) {
    const result = await Bun.$`bash ./scripts/unity_test.sh ${args.mode}`.text()
    return result
  },
})

export const build = tool({
  description: "Run a Unity build via ./scripts/unity_build.sh",
  args: {
    target: tool.schema.enum(["macos", "ios"]).default("macos"),
  },
  async execute(args) {
    const result = await Bun.$`bash ./scripts/unity_build.sh ${args.target}`.text()
    return result
  },
})
