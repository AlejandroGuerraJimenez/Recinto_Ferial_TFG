namespace Fairground.Model.Attractions.BasketballHoop
{
    /// <summary>
    /// Strategy: decides whether a match ends as win or lose.
    /// </summary>
    public interface IBasketballHoopEndCondition
    {
        BasketballHoopPhase Evaluate(
            BasketballHoopRules rules,
            int score,
            int ballsRemaining,
            int ballsInFlight);
    }
}
