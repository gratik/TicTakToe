using System;
using System.Collections.Generic;
using System.Linq;

namespace TicTakToe.App.Core.Models;

/// <summary>
/// Represents a Tic-Tac-Toe board of arbitrary size.
/// </summary>
public sealed class Board
{
    private readonly Player[] _cells;
    private readonly int[][] _winningLines;

    /// <summary>Initialises an empty board with the default 3×3 configuration.</summary>
    public Board() : this(BoardConfiguration.Default)
    {
    }

    /// <summary>Initialises an empty board using the provided configuration.</summary>
    public Board(BoardConfiguration configuration)
    {
        Size = configuration.Size;
        WinLength = configuration.WinLength;
        _cells = new Player[Size * Size];
        _winningLines = GenerateWinningLines();
    }

    private Board(Player[] cells, int size, int winLength, int[][] winningLines)
    {
        Size = size;
        WinLength = winLength;
        _cells = (Player[])cells.Clone();
        _winningLines = winningLines;
    }

    /// <summary>Gets the board dimension.</summary>
    public int Size { get; }

    /// <summary>Gets the number of aligned marks needed to win.</summary>
    public int WinLength { get; }

    /// <summary>Gets the total number of cells on the board.</summary>
    public int CellCount => _cells.Length;

    /// <summary>Gets the underlying configuration.</summary>
    public BoardConfiguration Configuration => new(Size, WinLength);

    /// <summary>Gets the <see cref="Player"/> occupying the cell at <paramref name="index"/>.</summary>
    public Player this[int index] => _cells[index];

    /// <summary>Returns <c>true</c> when <paramref name="index"/> is unoccupied and within range.</summary>
    public bool IsValidMove(int index) =>
        index >= 0 && index < _cells.Length && _cells[index] == Player.None;

    /// <summary>
    /// Places <paramref name="player"/> on the cell at <paramref name="index"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the cell is already occupied.</exception>
    public void MakeMove(int index, Player player)
    {
        if (!IsValidMove(index))
            throw new InvalidOperationException($"Cell {index} is not a valid move.");
        _cells[index] = player;
    }

    /// <summary>Returns all unoccupied cell indices.</summary>
    public IReadOnlyList<int> GetAvailableMoves()
    {
        var moves = new List<int>(CellCount);
        for (int i = 0; i < _cells.Length; i++)
            if (_cells[i] == Player.None)
                moves.Add(i);
        return moves;
    }

    /// <summary>Evaluates the current state of the board and returns the <see cref="GameResult"/>.</summary>
    public GameResult CheckResult()
    {
        foreach (var line in _winningLines)
        {
            var first = _cells[line[0]];
            if (first == Player.None)
                continue;

            bool winner = true;
            foreach (var index in line)
            {
                if (_cells[index] != first)
                {
                    winner = false;
                    break;
                }
            }

            if (winner)
                return first == Player.X ? GameResult.XWins : GameResult.OWins;
        }

        return _cells.Any(c => c == Player.None) ? GameResult.InProgress : GameResult.Draw;
    }

    /// <summary>
    /// Returns the winning line indices if a player has won, or <c>null</c> if no winner yet.
    /// </summary>
    public int[]? GetWinningLine()
    {
        foreach (var line in _winningLines)
        {
            var first = _cells[line[0]];
            if (first == Player.None)
                continue;

            bool winner = true;
            foreach (var index in line)
            {
                if (_cells[index] != first)
                {
                    winner = false;
                    break;
                }
            }

            if (winner)
                return line;
        }

        return null;
    }

    /// <summary>Creates a deep copy of this board.</summary>
    public Board Clone() => new(_cells, Size, WinLength, _winningLines);

    /// <summary>Exposes a read-only view of all cells.</summary>
    public IReadOnlyList<Player> Cells => Array.AsReadOnly(_cells);

    private int[][] GenerateWinningLines()
    {
        var lines = new List<int[]>();

        // Horizontal lines
        for (int row = 0; row < Size; row++)
        {
            for (int start = 0; start <= Size - WinLength; start++)
            {
                var line = new int[WinLength];
                for (int offset = 0; offset < WinLength; offset++)
                    line[offset] = row * Size + start + offset;
                lines.Add(line);
            }
        }

        // Vertical lines
        for (int col = 0; col < Size; col++)
        {
            for (int startRow = 0; startRow <= Size - WinLength; startRow++)
            {
                var line = new int[WinLength];
                for (int offset = 0; offset < WinLength; offset++)
                    line[offset] = (startRow + offset) * Size + col;
                lines.Add(line);
            }
        }

        // Diagonal (down-right)
        for (int row = 0; row <= Size - WinLength; row++)
        {
            for (int col = 0; col <= Size - WinLength; col++)
            {
                var line = new int[WinLength];
                for (int offset = 0; offset < WinLength; offset++)
                    line[offset] = (row + offset) * Size + col + offset;
                lines.Add(line);
            }
        }

        // Diagonal (down-left)
        for (int row = 0; row <= Size - WinLength; row++)
        {
            for (int col = WinLength - 1; col < Size; col++)
            {
                var line = new int[WinLength];
                for (int offset = 0; offset < WinLength; offset++)
                    line[offset] = (row + offset) * Size + col - offset;
                lines.Add(line);
            }
        }

        return lines.ToArray();
    }
}
