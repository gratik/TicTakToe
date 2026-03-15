# TicTakToe Improvement Plan

Date: 2026-03-15

## Goals

- Improve reliability of async orchestration and AI scheduling.
- Enforce game-rule invariants in the core engine.
- Harden stats persistence against invalid browser data.
- Improve test stability and close edge-case coverage gaps.
- Tighten model encapsulation and accessibility semantics.

## Phase 1: Home Orchestration Hardening

Target files:

- src/TicTakToe.App/Components/Pages/Home.razor

Changes:

- Replace async void state-change flow with a guarded async pipeline.
- Add exception handling around state-change work.
- Replace System.Threading.Timer scheduling with cancellation-based async scheduling.
- Add idempotency guard so result persistence runs once per completed game.

Success criteria:

- No overlapping AI/game-loop callbacks during mode changes or fast state transitions.
- No duplicate stats writes for the same finished game.
- Disposal cleanly cancels scheduled work.

## Phase 2: Engine Invariant Enforcement

Target files:

- src/TicTakToe.App/Core/Services/GameEngine.cs

Changes:

- Enforce mode and turn invariants in engine methods.
- No-op safely when callers request invalid move paths.

Success criteria:

- HumanMove blocked in CvC.
- TriggerAiMove blocked in PvP.
- Valid existing gameplay behavior remains unchanged.

## Phase 3: Stats Deserialization Resilience

Target files:

- src/TicTakToe.App/Infrastructure/LocalStorageStatsService.cs

Changes:

- Catch JSON parse failures and return empty stats fallback.
- Optionally clear invalid localStorage payload for self-healing behavior.

Success criteria:

- Invalid/corrupt localStorage data does not throw into app flow.
- Normal read/write behavior remains unchanged.

## Phase 4: Test Reliability and Coverage ✅ DONE

Target files:

- tests/TicTakToe.Tests/Components/GameCellTests.cs (new)
- tests/TicTakToe.Tests/Components/GameControlsTests.cs (new)
- tests/TicTakToe.Tests/Components/GameStatusTests.cs (new)
- tests/TicTakToe.Tests/Components/HomeTests.cs (new)
- tests/TicTakToe.Tests/Components/ThemeToggleTests.cs (new)
- tests/TicTakToe.Tests/Core/Strategies/MinimaxStrategyTests.cs
- tests/TicTakToe.Tests/Infrastructure/LocalStorageStatsServiceTests.cs

Changes made:

- Added bUnit component tests for GameCell, GameControls, GameStatus, ThemeToggle.
- Added HomeTests covering all PersistResultAsync branches (PvP/PvC/CvC × win/loss/draw), human move, mode/difficulty change, and Dispose.
- Added MinimaxStrategy throw-on-full-board test (line 19 branch).
- Added LocalStorageStatsService null-deserialization branch test.

Results:

- Test count: 73 → 133
- Line coverage: 78.72% → 82.90% overall; 94.71% for game-specific code

## Phase 5: Board Encapsulation

Target files:

- src/TicTakToe.App/Core/Models/Board.cs

Changes:

- Prevent direct external access to mutable board backing storage.
- Review available-move allocation behavior and optimize only if clean and justified.

Success criteria:

- External mutation risk reduced/eliminated.
- Existing tests continue to pass.

## Phase 6: Accessibility Refinement

Target files:

- src/TicTakToe.App/Components/Game/GameCell.razor
- src/TicTakToe.App/Components/Game/GameBoard.razor

Changes:

- Improve aria-label values to include row/column and current cell state.
- Keep board-level role/label semantics coherent.

Success criteria:

- Screen reader announcements are clear and human-meaningful.

## Verification

Commands:

- dotnet test
- dotnet build

Manual checks:

- PvP: normal turn flow and result rendering.
- PvC: AI triggers only when expected and stops at game end.
- CvC: continuous play, speed behavior, and restart behavior are stable.
- Switching modes during pending AI work does not create ghost moves.

## Delivery Notes

- Keep edits minimal and localized.
- Preserve existing public API shape unless a change is required for correctness.
- Add concise comments only where logic is non-obvious.
