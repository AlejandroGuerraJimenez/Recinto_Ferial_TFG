namespace Fairground.Model.Attractions.BalloonThrow
{
    public sealed class DefaultBalloonThrowEndCondition : IBalloonThrowEndCondition
    {
        public BalloonThrowPhase Evaluate(
            BalloonThrowRules rules,
            int balloonsPopped,
            int throwsRemaining,
            int balloonsRemaining)
        {
            if (rules.IsWin(balloonsPopped))
                return BalloonThrowPhase.Won;

            if (rules.IsLose(throwsRemaining, balloonsRemaining))
                return BalloonThrowPhase.Lost;

            return BalloonThrowPhase.Playing;
        }
    }
}
