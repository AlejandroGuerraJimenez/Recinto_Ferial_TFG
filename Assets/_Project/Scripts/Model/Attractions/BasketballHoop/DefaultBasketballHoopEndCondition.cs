namespace Fairground.Model.Attractions.BasketballHoop
{
    public sealed class DefaultBasketballHoopEndCondition : IBasketballHoopEndCondition
    {
        public BasketballHoopPhase Evaluate(
            BasketballHoopRules rules,
            int score,
            int ballsRemaining,
            int ballsInFlight)
        {
            if (rules.IsWin(score))
                return BasketballHoopPhase.Won;

            if (rules.IsLose(ballsRemaining, ballsInFlight, score))
                return BasketballHoopPhase.Lost;

            return BasketballHoopPhase.Playing;
        }
    }
}
