# Copilot Instructions

## Project Overview

Tic-Tac-Toe game built with **C# and Blazor Web App (.NET 10, Server interactivity)**.  
Three game modes (PvP, PvC, CvC), three AI difficulties, localStorage-persisted stats, light/dark theme.

## Build, Run & Test

```bash
# Restore
dotnet restore

# Build
dotnet build

# Run the Blazor app
dotnet run --project src/TicTakToe.App

# Run all tests
dotnet test

# Run a single test by name
dotnet test --filter "FullyQualifiedName~<TestMethodName>"

# Run tests in a specific class
dotnet test --filter "ClassName~BoardTests"
```

## Solution Structure

```
src/TicTakToe.App/
├── Core/                       Pure C# — no Blazor dependencies (fully testable)
│   ├── Models/                 Board, Player, GameResult, GameMode, Difficulty, GameStats
│   └── Services/
│       ├── Interfaces/         IGameEngine, IAiPlayer, IStatsService
│       ├── Strategies/         IAiStrategy, RandomStrategy, WeightedStrategy, MinimaxStrategy
│       ├── AiPlayer.cs         Routes to correct strategy by Difficulty
│       └── GameEngine.cs       Orchestrates turns; raises GameStateChanged event
├── Infrastructure/
│   └── LocalStorageStatsService.cs   IStatsService backed by browser localStorage via JS interop
├── Components/
│   ├── Layout/                 MainLayout, ThemeToggle
│   ├── Game/                   GameBoard, GameCell, GameStatus, GameControls
│   └── Stats/                  StatsPanel
├── Pages/
│   └── Home.razor              Single-page app shell; subscribes to GameStateChanged
└── wwwroot/
    ├── css/app.css             CSS custom properties drive all theming
    └── js/localStorage.js      JS interop helpers (tttStorage.getItem/setItem/removeItem)

tests/TicTakToe.Tests/          xUnit + Moq; mirrors src structure
```

## Architecture

- **Core/** has zero Blazor dependencies — all game logic is pure C# and testable without a browser.
- **Strategy pattern** for AI: `IAiStrategy` → `RandomStrategy` (Easy), `WeightedStrategy` (Medium), `MinimaxStrategy` with alpha-beta pruning (Hard). `AiPlayer` selects the right strategy by `Difficulty`.
- **`GameEngine`** raises `GameStateChanged` event. Components (Home.razor) subscribe and call `StateHasChanged()` — never poll. `HumanMove` is a no-op in CvC mode; `TriggerAiMove` is a no-op in PvP mode.
- **CvC auto-play** uses `CancellationTokenSource` + `Task.Delay` (`ExecuteAfterDelayAsync`) with a 600ms starting delay, scheduled recursively after each AI move. Cancels cleanly on mode change or disposal.
- **Stats** are keyed by `GameMode`, serialised to JSON, stored in localStorage under keys `ttt_stats_{mode}`. Corrupt/malformed entries are caught, self-healed (removed), and fall back to `GameStats.Empty`.
- **Theming**: `data-theme="light|dark"` on the `<div>` in `MainLayout.razor` toggles CSS custom properties defined in `app.css`. No JS needed for the toggle.

## Key Conventions

- Each `.razor` component has a paired `.razor.css` scoped stylesheet. Shared variables live in `app.css` only.
- Components receive state via `[Parameter]` and raise `EventCallback` — they do not inject `IGameEngine` directly (except `Home.razor` which is the orchestrator).
- `StatsPanel` exposes a `RefreshAsync()` method; `Home.razor` holds a `@ref` and calls it after each game ends.
- `Home.razor` uses a `_resultPersisted` bool flag reset on every `StartNewGame` — `PersistResultAsync` is idempotent and runs exactly once per completed game even if `GameStateChanged` fires multiple times.
- `Board` is a mutable class; always `Clone()` before passing to AI strategies (they must not mutate the live board). `Board.Cells` returns `Array.AsReadOnly(_cells)` — do not cast to `Player[]`.
- `GameStats` is an immutable record; mutation returns new instances via `WithWin()` / `WithLoss()` / `WithDraw()`.
- `GameCell` `aria-label` reads `"Row R, column C, empty|marked X|marked O"` — keep this format if editing the component.
- All public types carry XML doc comments (`/// <summary>`).
- Services are registered as `Scoped` (Blazor Server lifetime matches the SignalR circuit).

## Testing

- `BoardTests` — covers all 8 win lines, draw, invalid moves, cloning
- `GameEngineTests` — turn flow, win/draw detection, GameStateChanged event, CvC/PvP mode invariants
- `MinimaxStrategyTests` — exhaustively proves Hard AI never loses as X or O; throws on full board
- `LocalStorageStatsServiceTests` — mocks `IJSRuntime` with Moq; covers null-deserialization and malformed-JSON self-heal
- `GameCellTests` — symbol rendering, CSS classes, click callback, aria-label row/col/state format (bUnit)
- `GameControlsTests` — mode/difficulty selects, callbacks, PvP disable logic (bUnit)
- `GameStatusTests` — all StatusText/StatusIcon/StatusClass branches (bUnit)
- `StatsPanelTests` — display, reset callback, active-mode highlight (bUnit)
- `ThemeToggleTests` — button render, aria-label, theme icons (bUnit)
- `HomeTests` — init, human move, mode/difficulty changes, all PersistResultAsync branches, idempotency guard, Dispose (bUnit + Moq)
