using Fairground.Model.Attractions;

namespace Fairground.Model.Attractions.BalloonThrow
{
    /// <summary>
    /// Strategy: decides whether a match ends as win or lose.
    /// </summary>
    public interface IBalloonThrowEndCondition
    {
        MatchPhase Evaluate(
            BalloonThrowRules rules,
            int balloonsPopped,
            int throwsRemaining,
            int balloonsRemaining,
            int throwsInFlight);
    }
}
