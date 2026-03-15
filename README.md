# Tic-Tac-Toe

A production-quality, interactive Tic-Tac-Toe game built with **C# / Blazor Web App (.NET 10)** featuring three game modes, three AI difficulty levels, persistent statistics, and a modern light/dark theme.

---

## Table of Contents

- [Features](#features)
- [Screenshots](#screenshots)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Game Modes](#game-modes)
- [AI Difficulties](#ai-difficulties)
- [Statistics](#statistics)
- [Theming](#theming)
- [Testing](#testing)
- [Technology Stack](#technology-stack)

---

## Features

| Feature | Details |
|---|---|
| **Three game modes** | Player vs Player, Player vs Computer, Computer vs Computer |
| **Three AI difficulties** | Easy (random), Medium (win/block heuristic), Hard (minimax + alpha-beta pruning) |
| **CvC auto-play** | Continuous self-play that progressively accelerates until games become a blur |
| **Persistent stats** | Win / loss / draw counts per mode, stored in browser `localStorage` |
| **Light / dark theme** | Toggle with instant persistence; applied before first render to prevent flash |
| **Fully tested** | 73 xUnit tests covering domain logic, services, AI strategies, and Blazor components |
| **Clean architecture** | Domain layer has zero Blazor/web dependencies — fully testable in isolation |

---

## Architecture

The application follows a layered, clean-architecture approach:

```
┌──────────────────────────────────────────────┐
│              Blazor Components (UI)           │
│  Home.razor · GameBoard · GameControls ···   │
│           @rendermode InteractiveServer       │
├──────────────────────────────────────────────┤
│             Core / Application Layer          │
│  IGameEngine · IAiPlayer · IStatsService     │
│  GameEngine  · AiPlayer  · (interfaces)      │
├──────────────────────────────────────────────┤
│              Domain Layer (pure C#)           │
│  Board · Player · GameResult · GameStats     │
│  IAiStrategy: Random · Weighted · Minimax    │
├──────────────────────────────────────────────┤
│               Infrastructure                  │
│  LocalStorageStatsService (JS interop)       │
└──────────────────────────────────────────────┘
```

### Key Design Decisions

- **Strategy pattern** for AI: `IAiStrategy` is implemented by `RandomStrategy`, `WeightedStrategy`, and `MinimaxStrategy`. `AiPlayer` selects the right strategy at runtime based on the chosen `Difficulty`.
- **Event-driven game loop**: `IGameEngine` raises `GameStateChanged` after every move. `Home.razor` subscribes and drives all UI updates.
- **Scoped DI**: All game services are registered as `Scoped` — one instance per browser session/circuit.
- **SSR-safe theming**: `MainLayout` is rendered in the static SSR shell and is not interactive. Dark mode is implemented in pure JavaScript to work around this Blazor Web App constraint.
- **Prerender-safe JS interop**: All `localStorage` reads are deferred to `OnAfterRenderAsync(firstRender: true)` to avoid exceptions during server-side prerender.

---

## Project Structure

```
TicTakToe/
├── README.md
├── TicTakToe.slnx                        # Solution (Visual Studio / Rider)
├── .github/
│   └── copilot-instructions.md           # AI assistant conventions
├── src/
│   └── TicTakToe.App/
│       ├── Program.cs                    # DI registrations, middleware
│       ├── Core/
│       │   ├── Models/
│       │   │   ├── Board.cs              # 9-cell board, win detection, cloning
│       │   │   ├── Player.cs             # Enum: None, X, O
│       │   │   ├── GameMode.cs           # Enum: PvP, PvC, CvC
│       │   │   ├── GameResult.cs         # Enum: InProgress, XWins, OWins, Draw
│       │   │   ├── Difficulty.cs         # Enum: Easy, Medium, Hard
│       │   │   └── GameStats.cs          # Immutable record: Wins, Losses, Draws
│       │   └── Services/
│       │       ├── Interfaces/
│       │       │   ├── IGameEngine.cs
│       │       │   ├── IAiPlayer.cs
│       │       │   └── IStatsService.cs
│       │       ├── Strategies/
│       │       │   ├── IAiStrategy.cs
│       │       │   ├── RandomStrategy.cs  # Easy: picks a random empty cell
│       │       │   ├── WeightedStrategy.cs# Medium: wins if possible, blocks if not
│       │       │   └── MinimaxStrategy.cs # Hard: minimax + alpha-beta pruning
│       │       ├── AiPlayer.cs           # Selects strategy by difficulty
│       │       └── GameEngine.cs         # Turn management, result detection, events
│       ├── Infrastructure/
│       │   └── LocalStorageStatsService.cs # IStatsService via JS interop
│       ├── Components/
│       │   ├── Layout/
│       │   │   ├── MainLayout.razor      # App shell (SSR, no code-behind)
│       │   │   └── ThemeToggle.razor     # Pure-JS dark mode button
│       │   ├── Game/
│       │   │   ├── GameBoard.razor       # 3×3 interactive grid
│       │   │   ├── GameCell.razor        # Individual cell (X / O / empty)
│       │   │   ├── GameStatus.razor      # Current turn or result banner
│       │   │   └── GameControls.razor    # Mode/difficulty selectors + New Game
│       │   ├── Stats/
│       │   │   └── StatsPanel.razor      # Per-mode win/loss/draw display
│       │   └── Pages/
│       │       └── Home.razor            # Page orchestrator
│       └── wwwroot/
│           ├── app.css                   # CSS custom properties, light/dark tokens
│           └── js/
│               └── localStorage.js       # Theme toggle, storage helpers, IIFE restore
└── tests/
    └── TicTakToe.Tests/
        ├── Core/
        │   ├── BoardTests.cs
        │   ├── GameEngineTests.cs
        │   ├── GameStatsTests.cs
        │   ├── AiPlayerTests.cs
        │   └── Strategies/
        │       ├── RandomStrategyTests.cs
        │       ├── WeightedStrategyTests.cs
        │       └── MinimaxStrategyTests.cs
        ├── Infrastructure/
        │   └── LocalStorageStatsServiceTests.cs
        └── Components/
            ├── GameBoardTests.cs
            └── StatsPanelTests.cs
```

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

### Run the app

```bash
# Clone the repository
git clone <repo-url>
cd TicTakToe

# Start the development server
dotnet run --project src/TicTakToe.App

# Open in browser
open http://localhost:5267
```

### Run the tests

```bash
dotnet test
```

### Build only

```bash
dotnet build
```

---

## Game Modes

### Player vs Player (PvP)
Two human players take turns on the same device. Wins and draws are tracked.

### Player vs Computer (PvC)
The human plays as **X** and always goes first. The computer plays as **O** at the selected difficulty. Stats track the human's perspective: wins, losses, and draws.

### Computer vs Computer (CvC)
Both X and O are controlled by the AI. Games play continuously without any user input, and the speed **progressively accelerates** with each game:

| Game # | Move delay | Between-game pause |
|---|---|---|
| 1 | 600 ms | ~1.8 s |
| 3 | ~340 ms | ~1 s |
| 6 | ~107 ms | ~320 ms |
| 9+ | **30 ms** | ~120 ms (blur) |

Clicking **New Game** resets the speed back to 600 ms. Switching to another mode stops the loop immediately.

---

## AI Difficulties

### Easy — `RandomStrategy`
Picks a random empty cell on every turn. Makes no attempt to win or block.

### Medium — `WeightedStrategy`
Uses a simple heuristic:
1. Take a winning move if one exists
2. Block the opponent's winning move if one exists
3. Otherwise pick randomly

### Hard — `MinimaxStrategy`
Full [minimax](https://en.wikipedia.org/wiki/Minimax) search with [alpha-beta pruning](https://en.wikipedia.org/wiki/Alpha%E2%80%93beta_pruning). The Hard AI **never loses** — it always plays the game-theoretically optimal move. Against itself, every game ends in a draw.

Scoring prefers faster wins and slower losses:
- Win at depth `d` → score `10 - d`
- Loss at depth `d` → score `d - 10`

---

## Statistics

Stats are stored in the browser's `localStorage` under mode-specific keys:

| Key | Mode |
|---|---|
| `ttt_stats_PvP` | Player vs Player |
| `ttt_stats_PvC` | Player vs Computer |
| `ttt_stats_CvC` | Computer vs Computer |

Each entry is a JSON object: `{ "wins": N, "losses": N, "draws": N }`.

Stats can be reset per-mode via the **Reset** button in the Stats panel.

---

## Theming

The app supports **light** and **dark** themes using CSS custom properties:

```css
:root {
  --bg-primary: #f8f9fa;
  --text-primary: #212529;
  /* ... */
}

[data-theme="dark"] {
  --bg-primary: #0d1117;
  --text-primary: #e6edf3;
  /* ... */
}
```

The `data-theme` attribute is set on `<html>` by JavaScript so that `:root` CSS variables cascade globally. The current theme is persisted to `localStorage` under the key `ttt_theme` and restored synchronously via an IIFE in `localStorage.js` (loaded in `<head>`) before Blazor renders — eliminating any flash of the wrong theme.

---

## Testing

The project has **73 tests** across three categories:

| Category | Framework | Coverage |
|---|---|---|
| Domain / Core | xUnit | `Board`, `GameEngine`, `AiPlayer`, `GameStats`, all three AI strategies |
| Infrastructure | xUnit + Moq | `LocalStorageStatsService` JS interop calls |
| Components | bUnit | `GameBoard` rendering and interaction, `StatsPanel` display |

```bash
dotnet test                          # run all tests
dotnet test --logger "console;verbosity=detailed"  # verbose output
```

### Test highlights

- **`MinimaxStrategyTests`**: Exhaustively proves the Hard AI never loses from any game state (plays every possible first move as X and verifies O draws or wins).
- **`GameEngineTests`**: Covers win detection for all 8 winning lines, draw detection, turn alternation, and the `GameStateChanged` event.
- **`GameBoardTests`** (bUnit): Verifies correct cell rendering, disabled state when game is over, and click callbacks.

---

## Technology Stack

| Layer | Technology |
|---|---|
| Language | C# 13 |
| Runtime | .NET 10 |
| UI framework | Blazor Web App (Interactive Server) |
| CSS | Custom properties (no external UI framework) |
| AI | Minimax + alpha-beta pruning |
| Storage | Browser `localStorage` via JS interop |
| Unit testing | xUnit |
| Component testing | bUnit |
| Mocking | Moq |
| IDE support | Visual Studio / Rider / VS Code |
