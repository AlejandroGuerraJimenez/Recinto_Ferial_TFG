namespace Fairground.Model.Attractions.PunchingBall
{
    public sealed class DefaultPunchingBallEndCondition : IPunchingBallEndCondition
    {
        public PunchingBallPhase Evaluate(
            PunchingBallRules rules,
            int bestScore,
            int punchesRemaining)
        {
            if (rules.IsWin(bestScore))
                return PunchingBallPhase.Won;

            if (rules.IsLose(punchesRemaining, bestScore))
                return PunchingBallPhase.Lost;

            return PunchingBallPhase.Playing;
        }
    }
}
