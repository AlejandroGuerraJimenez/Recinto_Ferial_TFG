using Fairground.Model.Attractions;

namespace Fairground.Model.Attractions.PunchingBall
{
    /// <summary>
    /// Strategy: decides whether a match ends as win or lose.
    /// </summary>
    public interface IPunchingBallEndCondition
    {
        MatchPhase Evaluate(
            PunchingBallRules rules,
            int bestScore,
            int punchesRemaining);
    }
}
