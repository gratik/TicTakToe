using System;

namespace TicTakToe.App.Core.Models;

/// <summary>Describes the shape and winning line length for a Tic-Tac-Toe board.</summary>
public sealed record BoardConfiguration
{
    public static BoardConfiguration Default => new(3, 3);
    public static BoardConfiguration FourByFour => new(4, 4);
    public static BoardConfiguration FiveByFive => new(5, 5);

    public int Size { get; init; }
    public int WinLength { get; init; }

    public int CellCount => Size * Size;

    public BoardConfiguration(int size, int winLength)
    {
        if (size < 3)
            throw new ArgumentOutOfRangeException(nameof(size), "Board size must be at least 3.");
        if (winLength < 3 || winLength > size)
            throw new ArgumentOutOfRangeException(nameof(winLength), "Win length must be between 3 and the board size.");

        Size = size;
        WinLength = winLength;
    }
}
