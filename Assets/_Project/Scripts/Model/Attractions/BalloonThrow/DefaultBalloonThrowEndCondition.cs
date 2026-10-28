using Fairground.Model.Attractions;

namespace Fairground.Model.Attractions.BalloonThrow
{
    public sealed class DefaultBalloonThrowEndCondition : IBalloonThrowEndCondition
    {
        public MatchPhase Evaluate(
            BalloonThrowRules rules,
            int balloonsPopped,
            int throwsRemaining,
            int balloonsRemaining,
            int throwsInFlight)
        {
            if (rules.IsWin(balloonsPopped))
                return MatchPhase.Won;

            if (rules.IsLose(throwsRemaining, balloonsRemaining, throwsInFlight))
                return MatchPhase.Lost;

            return MatchPhase.Playing;
        }
    }
}
