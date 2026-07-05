# Architecture Notes

## Current High-Level Architecture

Pendulum Grid Match is currently organized around scene-level MonoBehaviours. The main scene contains controllers for gameplay state, pendulum spawning, grid checking, effects, and screens. These controllers communicate primarily through `GameSingleton`, which resolves important components from the scene.

The code is compact and readable, but gameplay rules, Unity physics state, visual effects, and UI transitions are still closely coupled.

## Main Gameplay Flow

1. `GameplayController.StartGame` resets score, marks the game as active, and switches to the gameplay screen.
2. `PendulumManager` spawns a new `CircleObject` when gameplay is active.
3. `PendulumEngine` moves the pendulum by setting angular velocity on a `Rigidbody2D`.
4. On player input, `PendulumManager` stops grid checking, releases the current circle from its hinge joint, schedules the next spawn, and restarts grid checking.
5. `TriggerGridBuilder` creates a 3x3 grid of trigger objects at runtime.
6. `TriggerInfo` stores which circle currently occupies each trigger cell.
7. `TriggerGridChecker` periodically checks rows, columns, diagonals, and full-grid state.
8. On a match, `TriggerGridChecker` adds score, plays the explosion effect, wakes related rigidbodies, and destroys matched circles.
9. `CheckingLimitingTrigger` ends the game if circles remain above the grid limit too long.
10. `GameplayController.EndGame` switches to the stats screen and clears remaining circles.

## Current Dependency Issues

- `GameSingleton` hides dependencies and makes object relationships harder to test outside a live scene.
- `TriggerGridChecker` combines rule evaluation, score changes, effects, physics wake-up, and object destruction.
- `GameplayController` owns state transitions, score mutation, resource loading, and cleanup.
- Match detection depends on `TriggerInfo` and live `CircleObject` references instead of a pure grid model.
- Circle colors are randomly assigned inside `CircleObject`, which couples data generation to a Unity component.
- The input path uses `Input.anyKeyDown`, which is simple but broad and not isolated behind an input abstraction.

These are acceptable for the original small assignment, but they limit automated testing and make future behavior changes riskier.

## Why Separate Pure Rules From Unity Presentation

The most valuable next step is to move rules such as match detection, scoring, and game-over checks into pure C# classes. Those classes could take simple data structures as input and return explicit results such as matched cells, score awards, or game-over reasons.

That separation would improve the project because:

- Edit Mode tests could validate rule behavior without loading a scene.
- Physics and UI side effects would become consumers of rule results rather than part of the rule calculation.
- Match detection edge cases would be easier to cover.
- The scene layer could focus on presentation, spawning, effects, and Unity object lifetime.
- Future gameplay changes would have a smaller blast radius.

## Planned Target Architecture For Pass 2

The next refactoring pass should keep the playable scene intact while introducing a small rules layer.

Planned shape:

- `GameRules` or similar pure service for grid evaluation.
- Plain data types for grid cells, circle colors, match results, and game-over results.
- `ScoreRules` or a tested scoring method that maps matched colors to score values.
- A slimmer `TriggerGridChecker` that reads Unity trigger state, calls the rules layer, then applies returned effects to scene objects.
- A slimmer `GameplayController` that coordinates state changes instead of owning every detail directly.
- Focused Edit Mode tests for rows, columns, diagonals, no-match full grid, and scoring.

Pass 2 should avoid replacing every controller at once. The goal should be to carve out testable gameplay rules while preserving the existing scene and prefab wiring.
