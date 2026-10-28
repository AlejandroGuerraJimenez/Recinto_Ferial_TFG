using Fairground.Model.Attractions;

namespace Fairground.Model.Attractions.PunchingBall
{
    public sealed class DefaultPunchingBallEndCondition : IPunchingBallEndCondition
    {
        public MatchPhase Evaluate(
            PunchingBallRules rules,
            int bestScore,
            int punchesRemaining)
        {
            if (rules.IsWin(bestScore))
                return MatchPhase.Won;

            if (rules.IsLose(punchesRemaining, bestScore))
                return MatchPhase.Lost;

            return MatchPhase.Playing;
        }
    }
}
