# Refactoring Notes

## Pass 1 Cleanup

Pass 1 focused on repository presentation and safe cleanup:

- Removed tracked JetBrains `.idea` metadata from version control.
- Updated `.gitignore` for Unity, IDE, generated project, and build artifact files.
- Rewrote `README.md` as a portfolio-facing project page.
- Removed unprofessional source comments.
- Added initial architecture and refactoring documentation.

## Pass 2 Refactor

Pass 2 extracted the core board rules into testable pure C#:

- Added `Pendulum.Domain` assembly for the pure domain layer.
- Added `BoardModel`, `BoardCell`, `BoardPosition`, and `CellColor`.
- Added `MatchDetector`, `MatchLine`, and `MatchLineType`.
- Added `ScoreCalculator`, `GameSession`, and `BoardResolutionResult`.
- Added `Pendulum.Tests.EditMode` assembly and EditMode tests for domain rules.
- Changed `TriggerGridChecker` to snapshot Unity trigger state and delegate match/scoring/game-over decisions to `GameSession`.
- Removed `TriggerGridChecker`'s async polling loop and old inline row/column/diagonal matching logic.
- Kept Unity scene objects, prefabs, and gameplay flow compatible with the existing project.
- Kept `ColorPoints` as the Unity-facing scoring config and added conversion into domain score data.

## Pass 3 Integration Polish

Pass 3 focused on Unity integration and final repository quality:

- Added explicit `GameState` values for menu, playing, resolving, and game over.
- Changed gameplay, trigger, UI, and pendulum classes to receive dependencies through `Construct(...)` methods.
- Isolated `GameSingleton` as a compatibility bootstrapper instead of using it directly from gameplay classes.
- Added bootstrap validation with clear `Debug.LogError` messages for missing scene references.
- Centralized Unity-to-domain color mapping in `CircleColorMapper`.
- Added `SpawnCircleController.ClearSpawnedCircles` so end-game cleanup no longer needs a scene-wide circle search.
- Added GitHub Actions CI for license-free pure domain tests.
- Updated README and architecture documentation for final portfolio presentation.
- Added runtime cell visuals and a light cell-center magnet to make match placement clearer.
- Added a main-menu gameplay hint and simple English/Russian UI localization toggle backed by a ScriptableObject table.
- Improved board check scheduling by reacting to trigger occupancy changes instead of relying only on a single delayed check.
- Improved trigger cell occupancy by tracking candidate circles per cell and selecting the best settled occupant for board snapshots.
- Changed match resolution into a coroutine that waits across Unity frame/physics boundaries before collapsing remaining circles.
- Removed ScriptableObject constructor initialization from `ColorPoints` and moved default setup into Unity-safe validation hooks.

## Intentionally Not Changed Yet

- The scene was not rewired into a full presenter/controller architecture.
- Existing prefabs, scenes, sprites, materials, and third-party visual effects were not removed.
- `GameSingleton` was not eliminated, but direct gameplay usage was removed and it is now isolated.
- Circle spawning and pendulum physics were not redesigned.
- Height-limit loss remains handled by `CheckingLimitingTrigger`.
- No PlayMode tests were added because the remaining scene-side behavior depends on Unity timing and current scene wiring.

## Known Risks

- Manual Unity validation is still needed when scene references are assigned or changed.
- `TriggerGridChecker` still coordinates score, effects, physics wake-up, destruction, and collapse after the domain result.
- The delayed grid check still uses Unity `Invoke`; timing should be checked in the scene.
- `Resources.Load` remains as a compatibility fallback for `ColorPoints` when bootstrap references are not assigned.
- Multiple simultaneous matches are now resolved from one board snapshot and scored together. This is cleaner and tested, but it should be manually compared against the intended feel.
- CI currently verifies only pure domain logic outside Unity; a Unity test job still requires repository license secrets.
- Cell magnet values are intentionally conservative, but they should be tuned in Play Mode if balls feel too sticky or not sticky enough.

## Recommended Future Steps

1. Open the scene and verify start, drop, match, full-board loss, height-limit loss, restart, and menu return.
2. Assign all bootstrapper references directly in the scene, including `ColorPoints`, then remove fallback lookup code.
3. Split `TriggerGridChecker` result application into smaller presenter methods or collaborators if the project grows.
4. Consider a settled-cell event or short coroutine flow instead of delayed `Invoke` if the physics timing needs tighter control.
5. Add a small PlayMode smoke test only after scene references are stable.
6. Review package dependencies and remove unused optional packages after Unity confirms they are not required.
