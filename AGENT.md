# Unity Brotato-like Project Instructions

This repository is a Unity project for a Brotato-like game.

The assistant must treat this as a Unity gameplay project, not a generic C# project.

## Main Goal

Help implement, debug, review, and refactor gameplay systems while keeping the codebase maintainable, data-driven, and easy to expand.

The project is inspired by Brotato.

The core gameplay loop is:

* Wave combat
* Enemy spawning
* Material pickup
* End-of-wave shop
* Buying items and weapons
* Refreshing and locking shop entries
* Combining weapons
* Applying stat modifiers
* Starting the next wave

Do not confuse this project with Vampire Survivors.

Do not assume level-up three-choice rewards unless the existing project code explicitly implements that mechanic.

## File Reading Rules

Prioritize reading C# gameplay code.

Read these first:

* `Assets/**/*.cs`
* `Assets/Scripts/**/*.cs`
* `Assets/**/Scripts/**/*.cs`

Usually avoid reading these unless explicitly needed:

* `*.meta`
* `Library/**`
* `Temp/**`
* `Logs/**`
* `obj/**`
* `bin/**`
* `Build/**`
* `Builds/**`
* `UserSettings/**`
* `.vs/**`
* `.git/**`

Do not inspect Unity package internals unless the bug clearly depends on package code.

Avoid reading these unless the task is specifically about scenes, prefabs, serialized references, or Unity asset wiring:

* `*.unity`
* `*.prefab`
* `*.asset`
* `*.controller`
* `*.anim`
* `*.mat`

Only inspect `ProjectSettings/**` when the task is about:

* Input settings
* Tags or layers
* Physics settings
* Render settings
* Package setup
* Build configuration

When asked to understand the project, scan source code first. Do not waste context on Unity-generated files.

## Before Editing Code

Before making any code change:

1. Identify the gameplay system involved.
2. Search for relevant symbols, classes, methods, and call sites.
3. Read the smallest useful set of files.
4. List the files actually read.
5. Explain the current behavior based on the code.
6. Propose the smallest safe change.
7. Then edit.

Do not edit before understanding the relevant call chain.

If the request is ambiguous, state the assumption and continue with the most reasonable interpretation.

## Accuracy Rules

Do not invent:

* Brotato mechanics
* Unity APIs
* Package APIs
* Project-specific class names
* Project-specific components
* Existing files
* Existing serialized fields

If something is not confirmed by project code, mark it as an inference.

Use these labels when useful:

* `[Confirmed from code]`
* `[Inference]`
* `[Recommendation]`
* `[Uncertain]`

When discussing Unity APIs or package behavior, prefer verified knowledge. If unsure, say so and search official documentation when web access is available.

## Unity Architecture Rules

Prefer composition over deep inheritance.

Prefer data-driven design for static gameplay data.

Use ScriptableObject for static configuration when appropriate, such as:

* Weapons
* Items
* Enemies
* Characters
* Shop entries
* Wave configs
* Stat definitions

Keep runtime mutable state separate from static data assets.

Do not mutate ScriptableObject config assets at runtime unless the project already intentionally uses that pattern.

Avoid unnecessary singletons.

Avoid `FindObjectOfType`, `GameObject.Find`, and repeated scene-wide searches in gameplay hot paths.

Avoid putting unrelated responsibilities into:

* `PlayerController`
* `GameManager`
* `UIManager`
* `EnemyManager`

Large manager classes should not absorb every new feature.

Separate these systems when possible:

* Player movement
* Player stats
* Weapon runtime behavior
* Item effects
* Shop logic
* Wave spawning
* Enemy AI
* Pickup logic
* UI display
* Save data

UI should display gameplay state. UI should not own core gameplay rules.

## Brotato-like System Design Rules

Stats should be modifier-based.

Avoid directly scattering stat changes like:

* `damage += 5`
* `attackSpeed *= 1.2f`
* `player.maxHp += item.hp`

Prefer explicit stat modifiers or effect objects.

Weapons should be data-driven where practical.

A weapon should generally separate:

* Static weapon data
* Runtime weapon state
* Attack behavior
* Targeting behavior
* Projectile or hitbox behavior

Items should apply effects through clear item effect logic.

Shop logic should be separated from combat logic.

Wave spawning should be separated from enemy movement and enemy combat behavior.

Pickup logic should be separated from player stat logic.

Weapon combining should be explicit and testable.

Rarity, price, tags, and shop weights should not be hardcoded randomly across unrelated files.

## Performance Rules

Keep `Update` methods lightweight.

Avoid unnecessary allocations in combat hot paths.

Use object pooling when relevant for:

* Projectiles
* Enemies
* Damage numbers
* Pickups
* Floating text
* Repeated visual effects

Avoid LINQ in hot gameplay loops unless the project already accepts the overhead.

Avoid frequent `GetComponent` calls inside hot loops. Cache references when reasonable.

## Coding Style

Keep code compatible with Unity C#.

Preserve existing public APIs unless changing them is necessary.

Respect the existing project style.

Keep C# comments when they already exist.

Add comments only when they explain non-obvious gameplay or architecture decisions.

Do not generate large rewrites unless explicitly requested.

Prefer small, reviewable changes.

## Testing and Validation

After editing, run available compile, test, or validation commands when possible.

If Unity Editor access is unavailable, explain what should be checked manually in Unity.

For gameplay changes, mention the minimum Play Mode validation needed.

For serialized fields, mention whether the user may need to assign references in the Inspector.

If there are compile errors, fix them before finishing when possible.

## Response Expectations

Be direct and technical.

When analyzing code, mention concrete file names and symbols.

When editing code, summarize:

* What changed
* Why it changed
* Which files changed
* How to test it

If the task reveals architectural debt, point it out clearly.

Do not pretend the project has systems that were not found in the code.

## Do Not

Do not read irrelevant `.meta` files.

Do not inspect Unity-generated folders unless necessary.

Do not invent project structure.

Do not invent APIs.

Do not create hidden global state casually.

Do not mix UI, shop, combat, save data, and wave logic into one giant manager.

Do not treat this as a Vampire Survivors clone unless the user explicitly asks for that.

Do not assume level-up three-choice rewards.

Do not make broad refactors when a small fix is enough.
