namespace Fairground.Model.Attractions.DuckFishing
{
    public sealed class DefaultDuckFishingEndCondition : IDuckFishingEndCondition
    {
        public DuckFishingPhase Evaluate(
            DuckFishingRules rules,
            int ducksCaught,
            int attemptsRemaining,
            int dipsInFlight)
        {
            if (rules.IsWin(ducksCaught))
                return DuckFishingPhase.Won;

            if (rules.IsLose(attemptsRemaining, ducksCaught, dipsInFlight))
                return DuckFishingPhase.Lost;

            return DuckFishingPhase.Playing;
        }
    }
}
