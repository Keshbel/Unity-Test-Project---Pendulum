# Pendulum Grid Match

A compact Unity gameplay architecture sample focused on physics interaction, match detection, score rules, and testable code.

## Gameplay Overview

Pendulum Grid Match is a small 2D Unity project where colored circles are released from a swinging pendulum into a 3x3 trigger grid. The player drops the current circle, waits for it to settle into the grid, and scores when a horizontal, vertical, or diagonal line contains matching colors.

The game ends when the grid is full without a valid match or when circles remain above the grid height limit for too long.

## Key Technical Highlights

- Physics-driven pendulum movement and circle drops using Unity 2D rigidbodies and joints.
- Runtime-built trigger grid for detecting occupied cells.
- Color-based scoring configured through a `ScriptableObject` and resolved through a pure `ScoreCalculator`.
- Pure C# board, match, scoring, and game-session rules covered by EditMode tests.
- Simple screen flow for menu, gameplay, and results states.
- Small controller classes that make the current responsibilities easy to inspect before deeper refactoring.

## Architecture Overview

The current project is intentionally compact and still close to its original test-assignment form, but core rules now live outside MonoBehaviour-heavy scene code.

- `Assets/Game/Scripts/Domain` contains the pure board model, match detector, score calculator, and game-session resolution classes.
- `TriggerGridChecker` snapshots Unity trigger state into the domain board model, then applies the resolved result to scene objects.
- `GameplayController` owns scene-level game state, score display events, screen transitions, and end-game cleanup.
- `PendulumManager`, `PendulumEngine`, and `SpawnCircleController` handle pendulum movement, input, spawning, and circle release.
- `TriggerGridBuilder`, `TriggerInfo`, and `CheckingLimitingTrigger` remain Unity trigger adapters.
- `CircleObject` wraps circle color assignment, joint release, rigidbody wake-up, and destruction.
- `ScreenManager`, `MenuView`, `StatsView`, and animation scripts drive the simple UI flow.
- `GameSingleton` provides project-wide access to major scene controllers.

The architecture is still not presented as final. A future pass should continue reducing scene-level coupling and global lookups.

## Project Structure

```text
Assets/Game/
  Materials/       Runtime materials used by the game.
  Prefabs/         Circle prefab and related gameplay prefabs.
  Resources/       Color score data loaded by the gameplay controller.
  Scenes/          Main playable scene.
  Scripts/         Domain rules plus Unity gameplay, UI, trigger, and animation scripts.
  Tests/EditMode/  Pure gameplay rule tests.
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

EditMode tests now cover the pure board, match, scoring, and game-session rules. They do not depend on scenes, physics, GameObjects, or real time.

Run them from:

- Unity Editor: `Window > General > Test Runner`
- Command line: Unity batchmode test execution for the `Pendulum.Tests.EditMode` assembly

## Build Information

No release build is committed to the repository. Portfolio builds should be attached through GitHub Releases rather than stored in source control.

The project currently targets Unity 6 and uses the package set defined in `Packages/manifest.json`. Before publishing a build, verify the active target platform, player settings, and scene list in Unity's Build Profiles or Build Settings.

## Original Constraint

This project started as a roughly 10-hour Unity test assignment. The current repository is being prepared as a portfolio-ready sample through incremental cleanup and refactoring passes.

Pass 1 focused on repository presentation, documentation, and low-risk source cleanup. Pass 2 extracts core gameplay rules into a testable pure C# domain layer while keeping the scene flow recognizable.

## Future Improvements

- Replace global singleton lookups with explicit serialized references where practical.
- Reduce Unity-side result application responsibilities in `TriggerGridChecker`.
- Add scene-level smoke coverage after references and timing are stable.
- Review package dependencies and remove unused optional packages after confirming project requirements.
- Prepare release builds and attach them to GitHub Releases.

## Honest Limitations

- Unity presentation and object lifetime are still coupled to MonoBehaviours, scene objects, and physics callbacks.
- The project has EditMode tests for pure rules, but no PlayMode scene tests yet.
- Match resolution is pure, but visual side effects are still applied from `TriggerGridChecker`.
- Some dependencies are broader than the project likely needs and should be reviewed in a later cleanup pass.
- The project still reflects its test-assignment origin and is not presented as a finished production architecture.
