# Refactoring Notes

## Pass 1 Cleanup

This pass focused on low-risk repository presentation and documentation work:

- Removed tracked JetBrains `.idea` metadata from version control.
- Updated `.gitignore` with common Unity, IDE, generated project file, and build artifact rules.
- Rewrote `README.md` as a portfolio-facing project page.
- Added architecture documentation in `docs/architecture.md`.
- Added staged refactoring notes in this file.
- Removed unprofessional source comments and replaced only useful context with professional wording.
- Cleaned several inspector tooltips so they display readable English instead of garbled text.

## Intentionally Not Changed Yet

- Gameplay architecture was not rewritten.
- Match detection logic was not redesigned.
- Scene, prefab, material, sprite, package, and project setting GUIDs were not changed.
- Third-party visual effect assets were left in place.
- Package dependencies were not removed, even where they may deserve later review.
- No new gameplay features were added.
- No tests were added in this pass.

## Known Risks

- The project still relies on `GameSingleton` for cross-scene-controller access.
- Rule checks are coupled to MonoBehaviour state and physics trigger occupancy.
- `TriggerGridChecker` has multiple responsibilities and should be treated carefully in future edits.
- The current async checking loop should be reviewed before deeper gameplay changes.
- Broad package dependencies may increase project size or import time.
- Manual Unity validation is still needed after cleanup because this pass did not run the editor.

## Recommended Next Refactoring Steps

1. Add baseline manual verification notes for the current scene behavior.
2. Extract match detection into pure C# data and rule classes.
3. Add Edit Mode tests for horizontal, vertical, diagonal, and no-match scenarios.
4. Extract score calculation so color values can be tested independently of UI and effects.
5. Reduce `TriggerGridChecker` to scene-state collection plus rule-result application.
6. Replace or contain global singleton access where explicit scene references are practical.
7. Review package dependencies and remove unused packages only after Unity confirms they are not required.
