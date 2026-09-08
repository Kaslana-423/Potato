# POTATO Project Agent Guide

## 1. Project purpose

This repository is a Unity 2D top-down wave-survival prototype inspired by *Brotato*.

The current milestone is not to reproduce all of *Brotato*. The goal is to maintain a small, stable and playable vertical slice with a complete loop:

`Title -> Save Slot -> Main Menu -> Character Select -> Wave Combat -> Level/Crate Rewards -> Shop -> Next Wave -> Result`

Prioritize playability, correctness and maintainability over content count.

## 2. Current technical baseline

- Unity version: `2022.3.62f2c1`
- UI: uGUI and TextMeshPro
- Input: legacy Input Manager
- Main scenes:
  - `Assets/Scenes/MainMenu.unity`
  - `Assets/Scenes/SampleScene.unity`
- Current playable character: `薄荷`
- Current initial weapon: `木棍`
- Current run length: 20 waves
- Save data is stored under `Application.persistentDataPath`
- `weapons.xlsx` and `items.xlsx` are imported reference-data sources

## 3. Scope rule

Only functionality that already works or is explicitly requested by the user belongs to the current version.

Do not treat imported definitions, descriptions, spreadsheet rows, placeholder assets or locked configurations as implemented content.

When reporting project progress, distinguish these states:

- `Implemented`: usable in a normal run and verified in Play Mode.
- `Partial`: code exists but behavior, integration or resources are incomplete.
- `Reference only`: imported data with no complete runtime implementation.
- `Out of scope`: deliberately excluded from the current milestone.

Never use the total number of imported definitions as evidence of playable content.

## 4. In scope

- Existing title, save-slot, main-menu, character-selection and settings flow
- One complete playable character: `薄荷`
- Existing working weapon templates: slash, thrust and ranged
- Weapons and items whose full effects are already connected to runtime combat
- Player movement, automatic targeting, health, damage, death and working combat stats
- Existing wave timing, spawning, enemy pooling, contact damage and knockback
- Materials, experience, fruit and crate drops
- Post-wave level rewards and crate rewards
- Shop purchasing, refresh, locking, merging and detail display where already functional
- Pause, save/resume and win/loss results
- Existing 20-wave flow unless the user explicitly changes it
- Bug fixes, integration fixes, validation and small refactors required by the current playable slice

## 5. Out of scope

Do not implement or expand any of the following unless the user explicitly requests it:

- Full reproduction of *Brotato* weapons, items, enemies or DLC content
- Imported weapon or item effects that have no complete runtime implementation
- Explosion, burning, ricochet, penetration, chain lightning, turrets or other unfinished effect families
- Imported enemies without an independent working prefab and complete behavior
- The 34 imported DLC-category enemy definitions
- New elite or boss content added merely to match imported data
- Unconnected player stats
- New metaprogression or character-unlock systems
- Completing `斗士`, `游侠` or `幸运星`
- Formal art production for unfinished characters
- Multiplayer, online services, achievements, localization or platform SDK integration
- Large framework rewrites justified only by hypothetical future content

Out-of-scope content must not appear in active shops, reward pools, enemy pools, character selection or player-facing progress counts.

## 6. Imported-data policy

The large weapon, item and enemy datasets originate from *Brotato* and exist as development references. They are not a development backlog.

- Do not implement missing systems simply because an imported row references them.
- Do not silently enable imported entries.
- Do not present imported counts as original project content.
- Do not delete source spreadsheets or reference data unless the user explicitly asks for deletion.
- Prefer filtering reference-only entries out of runtime catalogs and generated active content.
- Any entry enabled in a build must have valid assets, valid IDs, complete behavior and a verified acquisition path.
- Before a public release, flag imported names, descriptions, balance values and other source-game data for replacement or explicit review.

## 7. Runtime-content acceptance gate

A weapon, item, enemy or character is considered implemented only when all applicable checks pass.

### Weapon or item

- It can be obtained through the intended runtime flow.
- Its description matches its real behavior.
- Every referenced stat or effect is connected to combat calculations.
- Buying, equipping, merging, removing, saving and loading do not desynchronize state.
- Missing assets or IDs fail validation clearly instead of producing a silent placeholder.
- It has been verified in Play Mode, not only inspected in a spreadsheet or ScriptableObject.

### Enemy

- It owns a valid prefab and definition.
- Spawn, movement, damage, knockback, death, pooling and drops work.
- Pool reuse resets all mutable state and event subscriptions.
- It appears only in intended waves.
- It has been fought in Play Mode.

### Character

- Character ID, portrait, initial weapon and modifiers are valid.
- Selection, new-run creation, save/resume and result display work.
- Its modifiers change real runtime behavior.
- It is intentionally visible and intentionally unlocked.

Entries that fail this gate remain disabled and must be described as `Partial` or `Reference only`.

## 8. Architecture boundaries

### Game flow

`EnemySpawner` currently coordinates too many concerns. Do not add unrelated shop, reward, save or UI responsibilities to it.

For small fixes, preserve current behavior. When a requested feature genuinely requires separation, prefer these responsibilities:

- Run phase transitions
- Wave timing and completion
- Enemy spawn planning
- Post-wave reward sequencing
- Shop presentation

Do not perform a broad refactor unless a concrete task is blocked by the existing coupling.

### Weapon state

Avoid two writable sources of truth between `PlayerWeaponEquipment`, `WeaponBag`, shop state and save data.

One runtime loadout model should be authoritative. Scene weapon objects and UI should reflect that model. Any change involving purchase, merge, removal or load must verify both presentation and combat behavior.

### Data loading

- Stable IDs are authoritative; display names are not IDs.
- Reject duplicate IDs during validation or generation.
- Handle missing IDs explicitly when loading saves.
- Do not make every reference spreadsheet row runtime-active by default.
- Avoid adding new `Resources.LoadAll` scans without checking initialization cost and duplicate discovery.

### Save compatibility

Changes to saved structures must consider existing slot data and run saves.

- Preserve existing saves when practical.
- Add explicit versioning or migration when changing serialized schemas.
- Never rename stable content IDs casually.
- Verify resume behavior separately for combat, post-wave rewards and shop phases.
- Do not claim exact combat restoration unless remaining wave state, spawn progress and required runtime state are actually restored.

## 9. Coding rules

- Preserve existing user changes and unrelated dirty-worktree files.
- Inspect relevant code, prefabs and scene bindings before editing.
- Make the smallest change that completely solves the requested problem.
- Do not create generic managers, service locators or abstractions without a current consumer.
- Do not add a new singleton when ownership can remain local or be passed explicitly.
- Avoid per-frame allocations and repeated scene-wide searches in `Update` or `FixedUpdate`.
- Pooling code must reset state completely on reuse.
- Keep Unity C# comments when they explain intent, lifecycle constraints, serialization requirements or teammate-facing setup.
- Do not add comments that merely translate the next line of code.
- Keep generated files separate from handwritten runtime logic.
- Do not hand-edit generated files when the source generator or spreadsheet is authoritative.

## 10. UI rules

- Preserve the current paper, tape, pin and hand-drawn menu language.
- Use `Canvas Scaler` and intentional `RectTransform` anchors; do not solve layout with one reference resolution's absolute positions.
- Keyboard and mouse navigation must lead to the same selection and page state.
- A disabled action must be visibly disabled and non-executable.
- `Esc` should return exactly one navigation level unless the current screen explicitly defines another behavior.
- Do not expose out-of-scope characters or reference-only content through UI placeholders.

## 11. Required workflow for every task

1. Restate the concrete requested outcome in one sentence.
2. Inspect only the files and Unity objects relevant to that outcome.
3. Identify whether affected content is `Implemented`, `Partial`, `Reference only` or `Out of scope`.
4. Explain any destructive operation before performing it.
5. Implement the minimum complete change.
6. Run available compile, static or data-validation checks.
7. If Unity Play Mode cannot be run, state exactly what still requires manual verification.
8. Report changed files, verified behavior and remaining risks.

Do not expand the task into unrelated cleanup.

## 12. Definition of done

A task is done only when:

- The requested behavior is implemented, not merely scaffolded.
- The project has no new compile errors.
- Relevant scene and prefab references are assigned.
- Runtime pools and catalogs contain only intentionally active content.
- Save/load compatibility has been considered where applicable.
- The normal player path has been tested or a precise manual test procedure is provided.
- No out-of-scope imported feature was added as collateral work.

## 13. Agent response format

Keep reports compact and evidence-based:

1. `Result`: what now works.
2. `Changed`: files and systems touched.
3. `Verified`: checks actually run.
4. `Manual check`: only steps that still require Unity Editor or human inspection.
5. `Risk`: concrete remaining issue, if any.

Mark uncertain conclusions as hypotheses. Do not report reference data, placeholder configuration or untested code as completed gameplay.
