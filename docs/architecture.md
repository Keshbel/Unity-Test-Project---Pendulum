# Architecture Notes

## Final High-Level Architecture

Pendulum Grid Match has a small pure gameplay domain layer, a Unity scene adapter layer, and a thin legacy bootstrapper for the current scene.

The domain layer lives in `Assets/Game/Scripts/Domain` and is compiled by the `Pendulum.Runtime` assembly definition. It has `noEngineReferences` enabled, so it cannot depend on UnityEngine, MonoBehaviours, GameObjects, transforms, colliders, resources, or scene objects.

The existing Unity scene layer remains intentionally lightweight and compatible with the current scene setup. It owns physics, trigger views, visual effects, UI, object spawning, and screen transitions.

`GameSingleton` no longer acts as a service locator for gameplay classes. It is isolated as a bootstrapper that resolves the current scene references once, calls `Construct(...)` on Unity-facing classes, and reports missing required references.

## Dependency Direction

```text
Unity scene objects and controllers
        |
        v
Pendulum.Domain pure rules
```

The domain layer does not know about Unity. Unity scripts map scene state into `BoardModel`, call the domain rules, then apply the returned result back to scene objects.

Normal gameplay classes now depend on explicit constructed references instead of calling `GameSingleton.Instance` directly. The only remaining `FindObjectOfType` calls are isolated inside the bootstrapper fallback for the existing scene.

## Pure Gameplay Classes

- `CellColor` is the domain color enum used by board rules.
- `BoardPosition` identifies a logical board coordinate.
- `BoardCell` represents a color at a board position.
- `BoardModel` stores the current logical grid state.
- `MatchLine` and `MatchLineType` describe resolved row, column, and diagonal matches.
- `MatchDetector` finds horizontal, vertical, main diagonal, and anti-diagonal matches.
- `ScoreCalculator` calculates score from matched colors and injected score values.
- `BoardResolutionResult` reports matches, score awarded, and game-over state.
- `GameSession` coordinates board replacement, match detection, scoring, and full-board game-over checks.

These classes are covered by EditMode tests and do not require a scene, physics, GameObjects, or real time.

## Unity Adapter Classes

- `TriggerGridBuilder` still builds the runtime trigger grid in the scene.
- `TriggerInfo` still observes Unity trigger enter/exit events and exposes the occupying circle color.
- `TriggerGridChecker` now snapshots `TriggerInfo` colors into `BoardModel`, delegates match/scoring/game-over decisions to `GameSession`, and applies the result by adding score, playing effects, waking rigidbodies, and destroying matched circles.
- `ColorPoints` remains a `ScriptableObject` configuration asset, but now converts its values into domain score data.
- `CircleColorMapper` centralizes conversion from Unity-facing `CircleColor` values into domain `CellColor` values.
- `GameplayController` keeps explicit `GameState`, screen transitions, score display events, and compatibility score methods.
- `GameSingleton` remains only as the legacy composition bootstrapper for the current scene.

## Game State Flow

The Unity layer uses a small explicit state model:

- `Menu`
- `Playing`
- `Resolving`
- `GameOver`

`GameplayController` owns state transitions. `PendulumManager` accepts input only in `Playing`. `TriggerGridChecker` moves briefly into `Resolving` while applying a resolved match, then returns to `Playing`. Full-board loss and height-limit loss enter `GameOver`.

## Gameplay Resolution Flow

1. `GameSingleton` wires scene references into gameplay, UI, pendulum, trigger, and animation classes.
2. `GameplayController.StartGame` resets score, clears existing spawned circles, switches to `Playing`, and resets the board adapter.
3. `PendulumManager` spawns and releases circles through the existing Unity physics flow.
4. `TriggerInfo` components track which circle currently occupies each trigger cell.
5. `TriggerGridChecker.CheckGrid` creates a `BoardModel` snapshot from the trigger cells.
6. `GameSession.ResolveBoard` runs `MatchDetector`, uses `ScoreCalculator`, and determines whether a full board without matches is game over.
7. If matches are returned, `TriggerGridChecker` applies Unity effects and destroys the matched circle objects.
8. If no matches exist and the board is full, `GameplayController.EndGame` shows the stats screen and clears spawned circles.
9. `CheckingLimitingTrigger` handles the height-limit loss condition through Unity trigger timing.

## Testing Strategy

- EditMode tests cover pure domain logic: row, column, diagonal, empty-cell, full-board, scoring, and multi-match behavior.
- The CI workflow runs EditMode tests because they are deterministic and do not depend on physics timing or scene loading.
- PlayMode tests are intentionally deferred until scene references are fully assigned and stable.

## CI Strategy

`.github/workflows/ci.yml` uses GameCI's Unity test runner to execute EditMode tests on pushes and pull requests. Repository Unity license secrets must be configured before CI can pass on GitHub.

## Current Dependency Issues

- `GameSingleton` still uses fallback scene lookups if references are not assigned in the inspector.
- `TriggerGridChecker` is slimmer than before, but it still applies multiple scene side effects after domain resolution.
- Circle colors are still generated inside `CircleObject`, which keeps random data generation in a Unity component.
- The board is still updated from trigger snapshots after a delay rather than from a cleaner settled-cell event.
- Height-limit loss logic remains Unity-side and is not represented in the pure domain layer.

## Remaining Improvements

Future cleanup should stay focused and avoid replacing the whole scene:

- Assign all bootstrapper references directly in the scene and remove fallback lookup code.
- Move score/effect/screen side effects behind smaller presenters or controller methods if the project grows.
- Replace delayed `Invoke` grid checks with a more explicit settle/check event if it can be done safely.
- Consider moving random color selection behind a small provider so it can be tested or controlled.
- Add focused PlayMode or scene smoke tests only if they can be kept stable.
