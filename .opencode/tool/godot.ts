import { tool } from "@opencode-ai/plugin"

/*
  Godot helper tool scaffold.

  Expected env vars:
  - GODOT_PATH: path to the Godot executable (e.g., godot4)
  - GODOT_PROJECT_PATH: folder containing project.godot (default: GodotProject)

  This tool is intentionally thin and delegates to ./scripts/*.sh so teams can
  evolve the build/test/export pipeline without rewriting tool code.
*/

const godotPath = process.env.GODOT_PATH
const projectPath = process.env.GODOT_PROJECT_PATH ?? "GodotProject"

export const info = tool({
  description: "Print Godot tool configuration expected by this repo",
  args: {},
  async execute() {
    return [
      "Godot tool config:",
      `GODOT_PATH=${godotPath ?? "<unset>"}`,
      `GODOT_PROJECT_PATH=${projectPath}`,
      "",
      "Set GODOT_PATH to your Godot executable.",
      "Example macOS (Homebrew): /opt/homebrew/bin/godot",
    ].join("\n")
  },
})

export const test = tool({
  description: "Run automated tests via ./scripts/godot_test.sh (headless)",
  args: {
    suite: tool.schema.enum(["unit", "all"]).default("all"),
  },
  async execute(args) {
    const result = await Bun.$`bash ./scripts/godot_test.sh ${args.suite}`.text()
    return result
  },
})

export const exportBuild = tool({
  description: "Export a build via ./scripts/godot_export.sh",
  args: {
    target: tool.schema.enum(["macos", "ios"]).default("macos"),
  },
  async execute(args) {
    const result = await Bun.$`bash ./scripts/godot_export.sh ${args.target}`.text()
    return result
  },
})
