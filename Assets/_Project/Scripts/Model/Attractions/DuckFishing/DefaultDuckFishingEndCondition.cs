using Fairground.Model.Attractions;

namespace Fairground.Model.Attractions.DuckFishing
{
    public sealed class DefaultDuckFishingEndCondition : IDuckFishingEndCondition
    {
        public MatchPhase Evaluate(
            DuckFishingRules rules,
            int ducksCaught,
            int attemptsRemaining,
            int dipsInFlight)
        {
            if (rules.IsWin(ducksCaught))
                return MatchPhase.Won;

            if (rules.IsLose(attemptsRemaining, ducksCaught, dipsInFlight))
                return MatchPhase.Lost;

            return MatchPhase.Playing;
        }
    }
}
