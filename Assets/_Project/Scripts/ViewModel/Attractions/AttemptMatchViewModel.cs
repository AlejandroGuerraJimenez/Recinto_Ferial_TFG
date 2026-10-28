using System;
using Fairground.Model.Attractions;

namespace Fairground.ViewModel.Attractions
{
    /// <summary>
    /// Scoring booth that spends attempts. The booth adds its own verbs.
    /// </summary>
    public abstract class AttemptMatchViewModel : BoothMatchViewModel
    {
        protected AttemptMatchViewModel(AttemptMatchRules rules)
        {
            Match = new AttemptMatch(rules ?? throw new ArgumentNullException(nameof(rules)));
        }

        protected AttemptMatch Match { get; }

        public MatchPhase Phase => Match.Phase;
        public int Score => Match.Score;
        public int AttemptsRemaining => Match.AttemptsRemaining;
        public int Successes => Match.Successes;
        public int TargetsRemaining => Match.TargetsRemaining;
        public bool IsPlaying => Phase == MatchPhase.Playing;
        public bool CanSpendAttempt => IsPlaying && AttemptsRemaining > 0;

        public override void StartMatch()
        {
            Match.Start();
            Notify();
        }

        public override void Restart()
        {
            Match.Restart();
            Notify();
        }

        protected bool TrySpendAttempt() => Apply(Match.TrySpendAttempt);

        protected bool TryRegisterSuccess() => Apply(Match.TryRegisterSuccess);

        protected bool TryResolveAttempt() => Apply(Match.ResolveAttempt);
    }
}
