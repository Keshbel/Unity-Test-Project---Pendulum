# Pendulum Grid Match

A compact Unity gameplay architecture sample focused on physics interaction, match detection, score rules, and testable code.

## Gameplay Overview

Pendulum Grid Match is a small 2D Unity project where colored circles are released from a swinging pendulum into a 3x3 trigger grid. The player drops the current circle, waits for it to settle into the grid, and scores when a horizontal, vertical, or diagonal line contains matching colors.

The game ends when the grid is full without a valid match or when circles remain above the grid height limit for too long.

## Key Technical Highlights

- Physics-driven pendulum movement and circle drops using Unity 2D rigidbodies and joints.
- Runtime-built trigger grid for detecting occupied cells.
- Color-based scoring configured through a `ScriptableObject`.
- Simple screen flow for menu, gameplay, and results states.
- Small controller classes that make the current responsibilities easy to inspect before deeper refactoring.

## Architecture Overview

The current project is intentionally compact and still close to its original test-assignment form.

- `GameplayController` owns game state, score changes, screen transitions, and end-game cleanup.
- `PendulumManager`, `PendulumEngine`, and `SpawnCircleController` handle pendulum movement, input, spawning, and circle release.
- `TriggerGridBuilder`, `TriggerGridChecker`, `TriggerInfo`, and `CheckingLimitingTrigger` create and evaluate the grid.
- `CircleObject` wraps circle color assignment, joint release, rigidbody wake-up, and destruction.
- `ScreenManager`, `MenuView`, `StatsView`, and animation scripts drive the simple UI flow.
- `GameSingleton` provides project-wide access to major scene controllers.

This pass documents the current state rather than claiming the architecture is complete. A future pass should separate pure gameplay rules from Unity presentation and scene-level services.

## Project Structure

```text
Assets/Game/
  Materials/       Runtime materials used by the game.
  Prefabs/         Circle prefab and related gameplay prefabs.
  Resources/       Color score data loaded by the gameplay controller.
  Scenes/          Main playable scene.
  Scripts/         Gameplay, UI, screen, trigger, and animation scripts.
  Sprites/         Project-specific visual assets.

Assets/JMO Assets/ Third-party visual effect assets used by the project.
Packages/          Unity package manifest and lock file.
ProjectSettings/   Unity project settings required to open the project.
docs/              Architecture and refactoring notes.
```

## How To Open The Project

1. Install Unity `6000.3.11f1` or a compatible Unity 6 editor version.
2. Open this repository folder from Unity Hub.
3. Let Unity restore packages from `Packages/manifest.json`.
4. Open `Assets/Game/Scenes/Game.unity`.

## How To Run The Game

1. Open `Assets/Game/Scenes/Game.unity`.
2. Press Play in the Unity Editor.
3. Use the start button to enter gameplay.
4. Press or tap to release the current pendulum circle.

## Tests

The Unity Test Framework package is present in the project manifest, but this cleanup pass does not add new tests. Tests are planned for the next refactoring pass, especially around match detection, score calculation, and end-game rules after those rules are separated from MonoBehaviour-driven presentation code.

When tests are added, run them from:

- Unity Editor: `Window > General > Test Runner`
- Command line: Unity batchmode test execution for Edit Mode and Play Mode test assemblies

## Build Information

No release build is committed to the repository. Portfolio builds should be attached through GitHub Releases rather than stored in source control.

The project currently targets Unity 6 and uses the package set defined in `Packages/manifest.json`. Before publishing a build, verify the active target platform, player settings, and scene list in Unity's Build Profiles or Build Settings.

## Original Constraint

This project started as a roughly 10-hour Unity test assignment. The current repository is being prepared as a portfolio-ready sample through incremental cleanup and refactoring passes.

Pass 1 focuses on repository presentation, documentation, and low-risk source cleanup only. Gameplay architecture and behavior are intentionally left mostly unchanged until later passes.

## Future Improvements

- Extract match detection, scoring, and end-game conditions into pure C# classes with Edit Mode tests.
- Replace global singleton lookups with explicit serialized references or a lightweight composition root.
- Reduce responsibilities in `GameplayController` and `TriggerGridChecker`.
- Add automated tests for row, column, diagonal, full-grid, and height-limit scenarios.
- Review package dependencies and remove unused optional packages after confirming project requirements.
- Prepare release builds and attach them to GitHub Releases.

## Honest Limitations

- The current gameplay logic is still coupled to MonoBehaviours, scene objects, and Unity physics callbacks.
- There are no committed automated tests yet.
- Match detection and visual side effects currently live in the same checking flow.
- Some dependencies are broader than the project likely needs and should be reviewed in a later cleanup pass.
- The project still reflects its test-assignment origin and is not presented as a finished production architecture.
