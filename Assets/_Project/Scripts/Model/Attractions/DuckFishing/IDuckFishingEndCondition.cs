namespace Fairground.Model.Attractions.DuckFishing
{
    /// <summary>
    /// Strategy: decides whether a match ends as win or lose.
    /// </summary>
    public interface IDuckFishingEndCondition
    {
        DuckFishingPhase Evaluate(
            DuckFishingRules rules,
            int ducksCaught,
            int attemptsRemaining,
            int dipsInFlight);
    }
}
