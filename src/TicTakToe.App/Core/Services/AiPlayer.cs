using TicTakToe.App.Core.Models;
using TicTakToe.App.Core.Services.Interfaces;
using TicTakToe.App.Core.Services.Strategies;

namespace TicTakToe.App.Core.Services;

/// <summary>
/// Routes move selection to the correct <see cref="IAiStrategy"/> based on difficulty.
/// </summary>
public sealed class AiPlayer : IAiPlayer
{
    private readonly IReadOnlyDictionary<Difficulty, IAiStrategy> _strategies;

    /// <summary>
    /// Initialises a new <see cref="AiPlayer"/> with the provided strategies.
    /// </summary>
    /// <param name="strategies">All registered <see cref="IAiStrategy"/> implementations.</param>
    /// <exception cref="ArgumentException">Thrown if any difficulty level is missing a strategy.</exception>
    public AiPlayer(IEnumerable<IAiStrategy> strategies)
    {
        _strategies = strategies.ToDictionary(s => s.Difficulty);
        foreach (Difficulty d in Enum.GetValues<Difficulty>())
        {
            if (!_strategies.ContainsKey(d))
                throw new ArgumentException($"No strategy registered for difficulty '{d}'.");
        }
    }

    /// <inheritdoc/>
    public int ChooseMove(Board board, Player player, Difficulty difficulty)
    {
        if (!_strategies.TryGetValue(difficulty, out var strategy))
            throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, "Unknown difficulty.");

        return strategy.ChooseMove(board, player);
    }
}
