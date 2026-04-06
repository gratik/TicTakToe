# Tic-Tac-Toe

A modern interactive Tic-Tac-Toe app built with **C# / Blazor Web App (.NET 10)**. This project includes multiple game modes, AI strategies, board-size options, persistent browser stats, and a rich UI with theme, audio, and visualization controls.

---

## Table of Contents

- [Features](#features)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Game Modes](#game-modes)
- [AI Difficulties](#ai-difficulties)
- [Board Sizes](#board-sizes)
- [User Experience Features](#user-experience-features)
- [Statistics](#statistics)
- [Testing](#testing)
- [Technology Stack](#technology-stack)

---

## Features

| Feature | Details |
|---|---|
| **Game modes** | Player vs Player, Player vs Computer, Computer vs Computer |
| **AI difficulties** | Easy, Medium, Hard |
| **Board sizes** | 3×3, 4×4, 5×5 with matching win-length rules |
| **AI visualization** | Heatmap and step-through move evaluation overlays |
| **Persistent stats** | Win/loss/draw counts saved per mode in `localStorage` |
| **Customization** | Player names, marker colors, accent color, mute toggle |
| **Theme switching** | Light/dark mode via pure JS theme toggle |
| **CvC auto-play** | Accelerating AI vs AI replay with camera cues |
| **Clean architecture** | Pure C# core logic with Blazor UI and testable infrastructure |

---

## Architecture

The application uses a layered architecture with clean separation between UI, core logic, and infrastructure.

- **Blazor UI**: `Home.razor` orchestrates the game and subscribes to `GameStateChanged`. Components are parameter-driven and raise events back to the page.
- **Core services**: `GameEngine` manages turn flow, result detection, and game state. `AiPlayer` routes difficulty selection to the appropriate strategy.
- **Domain models**: `Board`, `Player`, `GameMode`, `GameResult`, `Difficulty`, `GameStats`, and `BoardConfiguration` are pure C# types with no Blazor dependencies.
- **Infrastructure**: `LocalStorageStatsService` persists stats using browser `localStorage` and JS interop.

Key components:

- `Program.cs` registers `IGameEngine`, `IAiPlayer`, and `IStatsService` as scoped services.
- `ThemeToggle.razor` uses JavaScript helpers to switch themes outside Blazor render cycles.
- `Home.razor` supports AI hints, visualization overlays, CvC scheduling, player preferences, and animated result flow.

---

## Project Structure

```
TicTakToe/
├── README.md
├── TicTakToe.slnx
├── .github/
│   └── copilot-instructions.md
├── src/
│   └── TicTakToe.App/
│       ├── Program.cs
│       ├── Core/
│       │   ├── Models/
│       │   │   ├── AiMoveEvaluation.cs
│       │   │   ├── Board.cs
│       │   │   ├── BoardConfiguration.cs
│       │   │   ├── Difficulty.cs
│       │   │   ├── GameMode.cs
│       │   │   ├── GameResult.cs
│       │   │   ├── GameStats.cs
│       │   │   └── Player.cs
│       │   └── Services/
│       │       ├── Interfaces/
│       │       │   ├── IGameEngine.cs
│       │       │   ├── IAiPlayer.cs
│       │       │   └── IStatsService.cs
│       │       ├── Strategies/
│       │       │   ├── IAiStrategy.cs
│       │       │   ├── MinimaxStrategy.cs
│       │       │   ├── RandomStrategy.cs
│       │       │   └── WeightedStrategy.cs
│       │       ├── AiPlayer.cs
│       │       ├── CameraOverlayManager.cs
│       │       ├── CvcScheduler.cs
│       │       └── GameEngine.cs
│       ├── Infrastructure/
│       │   └── LocalStorageStatsService.cs
│       ├── Components/
│       │   ├── Layout/
│       │   │   ├── MainLayout.razor
│       │   │   └── ThemeToggle.razor
│       │   ├── Game/
│       │   │   ├── GameBoard.razor
│       │   │   ├── GameCell.razor
│       │   │   ├── GameControls.razor
│       │   │   └── GameStatus.razor
│       │   └── Pages/
│       │       └── Home.razor
│       └── wwwroot/
│           ├── app.css
│           └── js/
│               └── localStorage.js
└── tests/
    └── TicTakToe.Tests/
        ├── Components/
        ├── Core/
        └── Infrastructure/
```

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

### Run the app

```bash
git clone <repo-url>
cd TicTakToe
dotnet run --project src/TicTakToe.App
open http://localhost:5267
```

### Run tests

```bash
dotnet test
```

---

## Game Modes

### Player vs Player (PvP)
Two human players alternate turns on the same device.

### Player vs Computer (PvC)
The human plays as **X** and goes first. The computer plays as **O** at the selected difficulty. A hint button is available for move suggestions.

### Computer vs Computer (CvC)
Both sides are controlled by AI. Games replay continuously and accelerate over time with camera-style cue overlays and a result banner.

---

## AI Difficulties

- **Easy** — random empty-cell move selection.
- **Medium** — heuristic strategy that wins when possible and blocks opponent threats.
- **Hard** — minimax with alpha-beta pruning for optimal play.

---

## Board Sizes

Supported board sizes:

- `3×3` with a 3-in-a-row win condition
- `4×4` with a 4-in-a-row win condition
- `5×5` with a 5-in-a-row win condition

---

## User Experience Features

- Light/dark theme toggle
- Accent color picker saved to `localStorage`
- Mute/unmute sound control
- Player name and color customization for X and O
- AI visualization overlays (heatmap or step mode)
- Hint support in PvC mode
- Animated result banners, shake effects, and camera cue overlays

---

## Statistics

Game statistics are persisted per game mode in browser `localStorage` using keys such as `ttt_stats_PvP`, `ttt_stats_PvC`, and `ttt_stats_CvC`. The stats service caches values in memory and recovers gracefully from malformed stored data.

---

## Testing

The repository includes unit and component tests covering core game logic, AI strategies, stats persistence, and Blazor components.

---

## Technology Stack

- .NET 10
- Blazor Interactive Server Components
- C# 12
- xUnit / bUnit / Moq
- Browser `localStorage` via `IJSRuntime`
