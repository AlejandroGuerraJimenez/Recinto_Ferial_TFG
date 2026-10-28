using Fairground.Model.Attractions;

namespace Fairground.Model.Attractions.DuckFishing
{
    /// <summary>
    /// Strategy: decides whether a match ends as win or lose.
    /// </summary>
    public interface IDuckFishingEndCondition
    {
        MatchPhase Evaluate(
            DuckFishingRules rules,
            int ducksCaught,
            int attemptsRemaining,
            int dipsInFlight);
    }
}
