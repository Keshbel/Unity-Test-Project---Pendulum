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

- Added `Pendulum.Runtime` assembly for the pure domain layer.
- Added `BoardModel`, `BoardCell`, `BoardPosition`, and `CellColor`.
- Added `MatchDetector`, `MatchLine`, and `MatchLineType`.
- Added `ScoreCalculator`, `GameSession`, and `BoardResolutionResult`.
- Added `Pendulum.Tests.EditMode` assembly and EditMode tests for domain rules.
- Changed `TriggerGridChecker` to snapshot Unity trigger state and delegate match/scoring/game-over decisions to `GameSession`.
- Removed `TriggerGridChecker`'s async polling loop and old inline row/column/diagonal matching logic.
- Kept Unity scene objects, prefabs, and gameplay flow compatible with the existing project.
- Kept `ColorPoints` as the Unity-facing scoring config and added conversion into domain score data.

## Intentionally Not Changed Yet

- The scene was not rewired into a full presenter/controller architecture.
- Existing prefabs, scenes, sprites, materials, and third-party visual effects were not removed.
- `GameSingleton` was not eliminated.
- Circle spawning, pendulum physics, and trigger occupancy behavior were not redesigned.
- Height-limit loss remains handled by `CheckingLimitingTrigger`.
- No PlayMode tests were added in this pass.

## Known Risks

- Manual Unity validation is still needed because the editor was not available through `unity-cli` during this pass.
- `TriggerGridChecker` still applies score, effects, physics wake-up, and destruction after the domain result.
- The delayed grid check now uses Unity `Invoke` instead of `async void` polling; timing should be checked in the scene.
- `Resources.Load` remains as a compatibility fallback for `ColorPoints` when the serialized reference is missing.
- Multiple simultaneous matches are now resolved from one board snapshot and scored together. This is cleaner and tested, but it should be manually compared against the intended feel.

## Recommended Pass 3 Steps

1. Open the scene and verify start, drop, match, full-board loss, height-limit loss, restart, and menu return.
2. Assign `ColorPoints` directly in the scene if it is currently relying on the `Resources.Load` fallback.
3. Split `TriggerGridChecker` result application into small presenter methods or collaborators.
4. Replace `GameSingleton` calls with explicit serialized references where practical.
5. Consider a settled-cell event or short coroutine flow instead of delayed `Invoke` if the physics timing needs tighter control.
6. Add a small PlayMode smoke test only after scene references are stable.
