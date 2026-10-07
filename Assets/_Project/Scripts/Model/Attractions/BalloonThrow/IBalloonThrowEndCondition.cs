namespace Fairground.Model.Attractions.BalloonThrow
{
    /// <summary>
    /// Strategy: decides whether a match ends as win or lose.
    /// </summary>
    public interface IBalloonThrowEndCondition
    {
        BalloonThrowPhase Evaluate(
            BalloonThrowRules rules,
            int balloonsPopped,
            int throwsRemaining,
            int balloonsRemaining,
            int throwsInFlight);
    }
}
