# Wilds of Cloverhollow (Unity) agent rules

## Source of truth

- `spec.md` is the product spec. Do not implement anything that contradicts it.
- If you change behavior or add a new system, update `spec.md` in the same PR.

## Planning and tracking (Linear + GitHub)

- Linear is the source of truth for phases and planned work.
- All work must map to a Linear issue in the current phase.
- Bugs are tracked as GitHub Issues and synced into Linear via Linear's GitHub Issues Sync. :contentReference[oaicite:0]{index=0}
  - File a bug as a GitHub Issue.
  - Ensure it appears in Linear (sync), then schedule it into the right phase.
  - Do not track bugs only in Linear without a GitHub Issue.

## Mandatory workflow for all file modifications (CRITICAL)

**Every file modification requires the full issue/branch/PR workflow. No exceptions.**

Before modifying ANY file in the repository, agents MUST:

1. **Create a GitHub Issue first**
   - Describe what will be changed and why.
   - Use appropriate labels (e.g., `enhancement`, `bug`, `chore`, `docs`).
   - If a Linear issue already exists, reference it; otherwise create the GitHub Issue first.

2. **Create a feature branch**
   - Branch from `main`.
   - Name format: `<type>/gh-<issue-number>-<short-description>` (e.g., `feat/gh-42-add-lantern`, `fix/gh-99-combat-crash`, `chore/gh-7-update-docs`).
   - Use Worktrunk if available: `wt switch -c feat/gh-42-add-lantern`.

3. **Make changes on the branch**
   - Commit with clear messages referencing the issue (e.g., `Add lantern scanning (#42)`).
   - Follow TDD: write tests first when applicable.

4. **Open a Pull Request**
   - PR title must include the issue reference (e.g., `feat: Add lantern scanning (#42)`).
   - Link the PR to the GitHub Issue.
   - Wait for CI/review if configured.

5. **Rebase and merge to main**
   - Rebase from `main` before merging to ensure a linear history: `git fetch origin && git rebase origin/main`.
   - Always use **squash merge** to keep `main` history clean: `gh pr merge <pr-number> --squash --delete-branch`.
   - Delete the branch after merge.

**Why this matters**: Untracked changes on `main` break history, make rollbacks hard, and bypass review. All work must be traceable.

## One issue = one branch = one PR

- Create a feature branch for every Linear item (feature, chore, refactor that is explicitly planned).
- Create a bugfix branch for every bug issue.
- Do not mix multiple issues in one PR unless they are inseparable and explicitly approved.

### Naming and linking

- Branch name must include the Linear issue key (or GitHub issue number for pure-bug work), for example:
  - `feat/TW-123-blacklight-notes`
  - `fix/gh-456-shop-crash`
- PR title must include the same identifier so Linear links the PR back to the issue. :contentReference[oaicite:1]{index=1}

## Worktrees (worktrunk)

We use Worktrunk to keep one worktree per issue so parallel work never collides. :contentReference[oaicite:2]{index=2}

Core commands (examples):

- Create and switch to a new worktree/branch:
  - `wt switch -c feat/TW-123-blacklight-notes`
- List worktrees:
  - `wt list`
- Remove a finished worktree:
  - `wt remove feat/TW-123-blacklight-notes`

If you are using Opencode locally, run it inside the issue’s worktree.

## Build workflow (TDD required)

Default approach is test-driven design.

For each issue:

1. Restate the acceptance criteria (from Linear + `spec.md`) in your own words in the PR description.
2. Write tests first for the intended behavior.
3. Implement the minimum code to pass tests.
4. Refactor only after green tests, and only within the scope of the issue.

### What to test in Unity

- Prefer pure C# classes for logic so they are easy to unit test.
- Use Unity EditMode tests for pure logic and fast checks.
- Use PlayMode tests only when the behavior requires scenes, GameObjects, input, or timing.
- Avoid broad “integration soup” tests. Write small, targeted tests that match the acceptance criteria.

## PR process (Opencode review via GitHub Actions)

- Open a PR for every branch.
- The PR will be reviewed by Opencode via a GitHub Action.
- Address review feedback using this rule set:

### Merge policy

Merge the phase “as is” unless there is a serious problem.

A “serious problem” is one of:

- Failing tests or broken build.
- Breaking change to player flow, save model, or public APIs without explicit approval.
- Behavior contradicts `spec.md` or introduces unplanned scope.
- Crash, data loss, security issue, or major performance regression.
- New third-party dependency or Unity package without explicit approval.

If a serious problem exists, fix it in the same PR before merging.

### Handling non-blocking review comments

Do not fix minor issues immediately.

For each non-blocking Opencode comment:

- Create a follow-up GitHub Issue that captures the comment and desired change.
- Link the follow-up issue to the PR and to the related Linear issue.
- Apply an appropriate label (for example: `follow-up` or `tech-debt`).
- Then proceed with merging the current PR.

## Working agreements

- Prefer small, reviewable diffs.
- No refactors unless they remove active pain or unblock the current phase.
- When in doubt, choose the simplest implementation that supports M1 (vertical slice).
- Do not change the Unity editor version unless explicitly requested.

## Repo layout expectations

- Unity project lives at: `./UnityProject` (adjust if different)
- Docs live at: `./docs`
- OpenCode project agents live at: `./.opencode/agent`
- OpenCode custom tools live at: `./.opencode/tool`

## Coding standards (C#)

- Naming: PascalCase for types and public members; camelCase for locals and private fields.
- Prefer composition over inheritance.
- Avoid singletons; if needed, isolate behind a small interface.
- Keep MonoBehaviours small; move pure logic into plain C# classes.
- Keep changes scoped to the issue. Avoid drive-by edits.

## Testing and validation

After code changes:

- Run automated tests (EditMode and PlayMode) if they exist for the touched area.
- Do a quick Unity play sanity check:
  - Load the main scene.
  - Verify the changed feature works.
  - Interact with at least one NPC if applicable.

## Build and run

- If scripts exist in `./scripts`, use them (`unity_test.sh`, `unity_build.sh`).
- If scripts do not exist, create them rather than embedding long instructions in chat.

## Asset and content rules

- Style Kit defined in `./docs/style-kit`
- No high-frequency textures for v1; use flat colors or simple painted textures.
- Keep props chunky and readable from a top-down camera.
- NPC text must be short and kid-friendly.

## How to use subagents

- Use `@game-designer` for quest ideas and progression structure.
- Use `@unity-engineer` for architecture and implementation.
- Use `@tech-artist` for lighting, materials, shaders, and asset pipeline.
- Use `@qa-playtest` to produce test checklists and edge cases before merging.

## Notes

- Subagents are defined in `.opencode/agent/*.md`
- Custom tools are defined in `.opencode/tool/*.ts`
- Be sure to always cleanup any temp files created while testing and commit work when finished.
