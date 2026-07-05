# Architecture Notes

## Current High-Level Architecture

Pendulum Grid Match now has a small pure gameplay domain layer plus a Unity scene adapter layer.

The domain layer lives in `Assets/Game/Scripts/Domain` and is compiled by the `Pendulum.Runtime` assembly definition. It has `noEngineReferences` enabled, so it cannot depend on UnityEngine, MonoBehaviours, GameObjects, transforms, colliders, resources, or scene objects.

The existing Unity scene layer remains intentionally lightweight and compatible with the current scene setup. It still owns physics, trigger views, visual effects, UI, object spawning, and screen transitions.

## Dependency Direction

```text
Unity scene objects and controllers
        |
        v
Pendulum.Domain pure rules
```

The domain layer does not know about Unity. Unity scripts map scene state into `BoardModel`, call the domain rules, then apply the returned result back to scene objects.

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
- `GameplayController` keeps scene-level game state, screen transitions, score display events, and compatibility score methods.
- `PendulumManager`, `PendulumEngine`, `SpawnCircleController`, and `CircleObject` remain Unity physics and spawning components.

## Gameplay Resolution Flow

1. `GameplayController.StartGame` resets score, switches to the gameplay screen, and resets the board adapter.
2. `PendulumManager` spawns and releases circles through the existing Unity physics flow.
3. `TriggerInfo` components track which circle currently occupies each trigger cell.
4. `TriggerGridChecker.CheckGrid` creates a `BoardModel` snapshot from the trigger cells.
5. `GameSession.ResolveBoard` runs `MatchDetector`, uses `ScoreCalculator`, and determines whether a full board without matches is game over.
6. If matches are returned, `TriggerGridChecker` applies Unity effects and destroys the matched circle objects.
7. If no matches exist and the board is full, `GameplayController.EndGame` shows the stats screen and clears remaining circles.
8. `CheckingLimitingTrigger` still handles the height-limit loss condition through Unity trigger timing.

## Current Dependency Issues

- `GameSingleton` still hides several scene dependencies.
- `TriggerGridChecker` is slimmer than before, but it still applies multiple scene side effects after domain resolution.
- Circle colors are still generated inside `CircleObject`, which keeps random data generation in a Unity component.
- The board is still updated from trigger snapshots after a delay rather than from a cleaner settled-cell event.
- Height-limit loss logic remains Unity-side and is not represented in the pure domain layer.

## Pass 3 Target

The next pass should focus on Unity-side cleanup without replacing the whole scene:

- Move score/effect/screen side effects behind smaller presenters or controller methods.
- Replace delayed `Invoke` grid checks with a more explicit settle/check event if it can be done safely.
- Reduce `GameSingleton` usage by assigning explicit scene references where practical.
- Consider moving random color selection behind a small provider so it can be tested or controlled.
- Add focused PlayMode or scene smoke tests only if they can be kept stable.
