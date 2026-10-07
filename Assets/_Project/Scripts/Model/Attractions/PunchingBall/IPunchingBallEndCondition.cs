namespace Fairground.Model.Attractions.PunchingBall
{
    /// <summary>
    /// Strategy: decides whether a match ends as win or lose.
    /// </summary>
    public interface IPunchingBallEndCondition
    {
        PunchingBallPhase Evaluate(
            PunchingBallRules rules,
            int bestScore,
            int punchesRemaining);
    }
}
