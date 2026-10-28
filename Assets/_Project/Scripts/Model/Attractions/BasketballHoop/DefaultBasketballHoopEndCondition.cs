using Fairground.Model.Attractions;

namespace Fairground.Model.Attractions.BasketballHoop
{
    public sealed class DefaultBasketballHoopEndCondition : IBasketballHoopEndCondition
    {
        public MatchPhase Evaluate(
            BasketballHoopRules rules,
            int score,
            int ballsRemaining,
            int ballsInFlight)
        {
            if (rules.IsWin(score))
                return MatchPhase.Won;

            if (rules.IsLose(ballsRemaining, ballsInFlight, score))
                return MatchPhase.Lost;

            return MatchPhase.Playing;
        }
    }
}
