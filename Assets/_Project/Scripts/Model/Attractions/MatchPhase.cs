namespace Fairground.Model.Attractions
{
    /// <summary>
    /// Shared booth match phase. The wheel keeps its own ride phase.
    /// </summary>
    public enum MatchPhase
    {
        NotStarted = 0,
        Playing = 1,
        Won = 2,
        Lost = 3,
    }
}
