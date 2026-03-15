namespace TicTakToe.App.Core.Models;

/// <summary>
/// Represents the 3×3 Tic-Tac-Toe board as a flat nine-element array.
/// Index layout:
/// <code>
/// 0 | 1 | 2
/// ---------
/// 3 | 4 | 5
/// ---------
/// 6 | 7 | 8
/// </code>
/// </summary>
public sealed class Board
{
    private static readonly int[][] WinLines =
    [
        [0, 1, 2], [3, 4, 5], [6, 7, 8], // rows
        [0, 3, 6], [1, 4, 7], [2, 5, 8], // columns
        [0, 4, 8], [2, 4, 6]             // diagonals
    ];

    private readonly Player[] _cells;

    /// <summary>Initialises an empty board.</summary>
    public Board() => _cells = new Player[9];

    private Board(Player[] cells) => _cells = (Player[])cells.Clone();

    /// <summary>Gets the <see cref="Player"/> occupying the cell at <paramref name="index"/>.</summary>
    /// <param name="index">Cell index (0–8).</param>
    public Player this[int index] => _cells[index];

    /// <summary>Returns <c>true</c> when <paramref name="index"/> is unoccupied and within range.</summary>
    public bool IsValidMove(int index) =>
        index >= 0 && index < 9 && _cells[index] == Player.None;

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
    public IReadOnlyList<int> GetAvailableMoves() =>
        Enumerable.Range(0, 9).Where(i => _cells[i] == Player.None).ToList();

    /// <summary>Evaluates the current state of the board and returns the <see cref="GameResult"/>.</summary>
    public GameResult CheckResult()
    {
        foreach (var line in WinLines)
        {
            var a = _cells[line[0]];
            if (a != Player.None && a == _cells[line[1]] && a == _cells[line[2]])
                return a == Player.X ? GameResult.XWins : GameResult.OWins;
        }
        return _cells.Any(c => c == Player.None) ? GameResult.InProgress : GameResult.Draw;
    }

    /// <summary>
    /// Returns the winning line indices if a player has won, or <c>null</c> if no winner yet.
    /// </summary>
    public int[]? GetWinningLine()
    {
        foreach (var line in WinLines)
        {
            var a = _cells[line[0]];
            if (a != Player.None && a == _cells[line[1]] && a == _cells[line[2]])
                return line;
        }
        return null;
    }

    /// <summary>Creates a deep copy of this board.</summary>
    public Board Clone() => new(_cells);

    /// <summary>Exposes a read-only view of all nine cells.</summary>
    public IReadOnlyList<Player> Cells => _cells;
}
