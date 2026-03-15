# TicTakToe Improvement Plan

Date: 2026-03-15

## Goals

- Improve reliability of async orchestration and AI scheduling.
- Enforce game-rule invariants in the core engine.
- Harden stats persistence against invalid browser data.
- Improve test stability and close edge-case coverage gaps.
- Tighten model encapsulation and accessibility semantics.

## Phase 1: Home Orchestration Hardening ✅ DONE

Target files:

- src/TicTakToe.App/Components/Pages/Home.razor

Changes made:

- Replaced `System.Threading.Timer` scheduling with `CancellationTokenSource` + `Task.Delay` (`ExecuteAfterDelayAsync`). Cancellation is clean and immediate on disposal or mode change.
- Added `_resultPersisted` flag — `PersistResultAsync` is guarded and runs exactly once per completed game, even if `GameStateChanged` fires multiple times.
- Added try/catch in the `async void OnGameStateChanged` handler so exceptions never propagate into the Blazor circuit.
- `ScheduleCvcRestart` resets `_resultPersisted = false` before each auto-restart so CvC stats accumulate correctly across chained games.

## Phase 2: Engine Invariant Enforcement ✅ DONE

Target files:

- src/TicTakToe.App/Core/Services/GameEngine.cs

Changes made:

- `HumanMove` returns early (no-op) when `Mode == GameMode.CvC`.
- `TriggerAiMove` returns early (no-op) when `Mode == GameMode.PvP`.

## Phase 3: Stats Deserialization Resilience ✅ DONE

Target files:

- src/TicTakToe.App/Infrastructure/LocalStorageStatsService.cs

Changes made:

- `GetStatsAsync` wraps `JsonSerializer.Deserialize` in a `catch (JsonException)` block.
- On failure, removes the corrupt `localStorage` key (self-heal) and returns `GameStats.Empty`.

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

## Phase 5: Board Encapsulation ✅ DONE

Target files:

- src/TicTakToe.App/Core/Models/Board.cs

Changes made:

- `Cells` returns `Array.AsReadOnly(_cells)` (`ReadOnlyCollection<Player>`) instead of the raw array, preventing cast-and-mutate bypasses.
- `GetAvailableMoves()` replaced LINQ `Enumerable.Range(...).Where(...).ToList()` with a pre-sized `List<int>(9)` loop, avoiding IEnumerable allocations.

## Phase 6: Accessibility Refinement ✅ DONE

Target files:

- src/TicTakToe.App/Components/Game/GameCell.razor

Changes made:

- `aria-label` replaced with a computed `AriaLabel` property that outputs `"Row R, column C, empty|marked X|marked O"`.
- `GameBoard` already had `role="grid"` and `aria-label="Tic-Tac-Toe board"` — no changes needed.

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
