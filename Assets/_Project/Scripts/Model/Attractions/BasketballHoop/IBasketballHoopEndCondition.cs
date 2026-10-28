using Fairground.Model.Attractions;

namespace Fairground.Model.Attractions.BasketballHoop
{
    /// <summary>
    /// Strategy: decides whether a match ends as win or lose.
    /// </summary>
    public interface IBasketballHoopEndCondition
    {
        MatchPhase Evaluate(
            BasketballHoopRules rules,
            int score,
            int ballsRemaining,
            int ballsInFlight);
    }
}
