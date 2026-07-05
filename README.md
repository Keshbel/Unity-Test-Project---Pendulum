# Pendulum Grid Match

[![CI](../../actions/workflows/ci.yml/badge.svg)](../../actions/workflows/ci.yml)

A compact Unity gameplay architecture sample focused on physics interaction, match detection, score rules, and testable code.

## Gameplay

Pendulum Grid Match is a small 2D game where colored balls are released from a swinging pendulum into a 3x3 grid. The player scores by forming a straight horizontal, vertical, or diagonal line from balls of the same color.

The game ends when the grid is full without a valid match or when balls remain above the grid limit for too long.

## Technical Highlights

- Unity 2D pendulum interaction using rigidbodies and joints.
- Runtime trigger grid with visual cell markers and light center magnet behavior.
- Pure C# domain layer for board state, match detection, scoring, collapse, and game-over rules.
- EditMode/domain tests for deterministic gameplay rules.
- ScriptableObject score and localization data.
- Explicit game states for menu, play, resolving, and game over.
- Small Unity adapter layer around scene objects, UI, effects, spawning, and physics.

## Project Structure

```text
Assets/Game/
  Resources/       Score and localization data.
  Scenes/          Main playable scene.
  Scripts/Domain/  Pure C# gameplay rules.
  Scripts/         Unity runtime adapters, UI, triggers, pendulum, and effects.
  Tests/EditMode/  Domain rule tests.
  Prefabs/         Gameplay prefabs.

Packages/          Unity package manifest and lock file.
ProjectSettings/   Unity project settings.
docs/              Architecture and refactoring notes.
```

## Open And Run

1. Install Unity `6000.3.11f1` or a compatible Unity 6 version.
2. Open the repository folder from Unity Hub.
3. Open `Assets/Game/Scenes/Game.unity`.
4. Press Play.
5. Start the game from the menu and press/click to release the current ball.

## Tests

The domain tests cover board matches, scoring, collapse, and game-over logic without relying on Unity physics or scene objects.

Run tests from Unity Test Runner:

```text
Window > General > Test Runner
```

The GitHub CI workflow also runs the pure domain tests through a temporary NUnit project with `dotnet test`. This keeps the default CI path license-free. It does not replace a manual Unity scene check before publishing a build.

## Releases

Playable builds should be distributed through GitHub Releases.

The repository includes a release build workflow at `.github/workflows/release-build.yml`. It builds Windows and Android artifacts with GameCI and attaches them to a GitHub Release.

Required repository secrets for the release workflow:

- `UNITY_LICENSE`
- `UNITY_EMAIL`
- `UNITY_PASSWORD`

## Background

This project started as a short Unity test assignment and was later cleaned up into a portfolio-oriented gameplay architecture sample. The current codebase keeps the original gameplay idea while separating core rules from Unity scene behavior.
