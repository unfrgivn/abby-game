# Wilds of Cloverhollow (Godot) agent rules

## Source of truth

- `spec.md` is the product spec. Do not implement anything that contradicts it.
- If you change behavior or add a new system, update `spec.md` in the same PR.

## Repo layout expectations

- Godot project folder: `./GodotProject` (must contain `project.godot`)
- Documentation: `./docs` (including `docs/style-kit`)
- Automation scripts: `./scripts`

If the Godot project folder changes, update:
- `docs/setup.md`
- `scripts/godot_test.sh` and `scripts/godot_export.sh`
- `.opencode/tool/godot.ts`

## Planning and tracking (Linear + GitHub)

- Linear is the source of truth for phases and planned work.
- All work must map to a Linear issue in the current phase.
- Bugs are tracked as GitHub Issues and synced into Linear via Linear's GitHub Issues Sync.

## Mandatory workflow for all file modifications (CRITICAL)

**Every file modification requires the full issue/branch/PR workflow. No exceptions.**

Before modifying ANY file in the repository, agents MUST:

1. **Create a GitHub Issue first**
   - Describe what will be changed and why.
   - Use appropriate labels (e.g., `enhancement`, `bug`, `chore`, `docs`).
   - If a Linear issue already exists, reference it; otherwise create the GitHub Issue first.

2. **Create a worktree and feature branch (REQUIRED)**
   - **BLOCKING REQUIREMENT:** You MUST check for Worktrunk (`wt`) before creating any branch.
   - Run `wt list` to verify availability.
   - If `wt` exists, you **MUST** use `wt switch -c <branch-name>`. **DO NOT** use `git checkout -b`.
   - Branch from `main`.
   - Name format: `<type>/gh-<issue-number>-<short-description>` (e.g., `feat/gh-42-sticker-book`, `fix/gh-99-save-crash`).
   - **Work in the new worktree directory**, not the main checkout.

3. **Make changes on the branch**
   - Commit with clear messages referencing the issue (e.g., `Add StickerBook UI (#42)`).
   - Follow TDD: write tests first when applicable.

4. **Open a Pull Request**
   - PR title should be descriptive but should **not** include the issue number.
   - Link the PR to the GitHub Issue in the PR description (e.g., `Closes #42`).

5. **Wait for CI checks to pass**

6. **Rebase and merge to main**
   - Rebase from `main` before merging: `git fetch origin && git rebase origin/main`.
   - Use **squash merge**: `gh pr merge <pr-number> --squash --delete-branch`.

7. **Clean up worktree**
   - Remove the local worktree: `wt remove <branch-name>`.

## Forbidden Actions

- **NEVER** use `git checkout -b` or `git switch -c` if the `wt` command is available.
- **NEVER** start coding without verifying you are in the correct worktree directory.

## One issue = one branch = one PR

- Create a feature branch for every Linear item.
- Create a bugfix branch for every GitHub Issue.
- Do not mix multiple issues in one PR unless explicitly approved.

## Build workflow (TDD required)

Default approach is test-driven design.

For each issue:
1. Restate acceptance criteria (from Linear + `spec.md`) in the PR description.
2. Write tests first for the intended behavior.
3. Implement the minimum code to pass tests.
4. Refactor only after green tests.

### What to test in Godot

- Prefer pure script classes (no scene dependencies) for:
  - Battle rules (turn order, damage, cooldowns)
  - Sticker inventory/loadout logic
  - Save/load serialization

- Use a headless test runner when available:
  - Recommended: **GUT** (Godot Unit Test). If used, document setup in `docs/setup.md`.
  - Tests should live under `GodotProject/tests/`.

- Scene-level checks should be minimal and targeted:
  - BattleScene can start from an EncounterDef and return to overworld
  - Save/load does not break scene transitions

### Commands

- Run tests (headless):
  - `./scripts/godot_test.sh all`

- Export builds (requires export presets configured in Godot):
  - macOS: `./scripts/godot_export.sh macos`
  - iOS: `./scripts/godot_export.sh ios`

## PR process (OpenCode review via GitHub Actions)

- Open a PR for every branch.
- The PR will be reviewed by OpenCode via a GitHub Action.

### Merge policy

Only block for serious problems:
- Failing tests or broken build
- Breaking changes to player flow, save model, or public APIs
- Behavior contradicts `spec.md` or introduces unplanned scope
- Crash, data loss, security issue, or major performance regression
- New third-party dependency without approval
