using Fairground.Core.Attractions;
using Fairground.Model.Attractions;
using Fairground.Model.Attractions.PunchingBall;
using Fairground.ViewModel.Attractions;

namespace Fairground.ViewModel.Attractions.PunchingBall
{
    /// <summary>
    /// Publishes punch-match state. The session owns the counters.
    /// </summary>
    public sealed class PunchingBallViewModel : BoothMatchViewModel
    {
        readonly PunchingBallSession _session;

        public PunchingBallViewModel(PunchingBallRules rules = null)
        {
            _session = new PunchingBallSession(rules ?? new PunchingBallRules());
            PublishState();
        }

        public PunchingBallViewModel(
            int startingPunches,
            int winScoreThreshold,
            int maxScore,
            float maxImpactSpeed)
            : this(new PunchingBallRules(startingPunches, winScoreThreshold, maxScore, maxImpactSpeed))
        {
        }

        public AttractionId AttractionId => AttractionId.PunchingBall;

        public MatchPhase Phase => _session.Phase;
        public int Score => _session.Score;
        public int BestScore => _session.BestScore;
        public int LastPunchPower => _session.LastPunchPower;
        public int PunchesRemaining => _session.PunchesRemaining;
        public int WinScoreThreshold => _session.Rules.WinScoreThreshold;
        public int MaxScore => _session.Rules.MaxScore;
        public bool IsPlaying => Phase == MatchPhase.Playing;
        public bool CanPunch => IsPlaying && PunchesRemaining > 0;
        public string StatusText => Describe(Phase);
        public override string[] HudLines { get; protected set; }

        public override void StartGame()
        {
            _session.Start();
            Notify();
        }

        public bool TryRegisterPunch(float impactMagnitude) => Apply(() => _session.TryRegisterPunch(impactMagnitude));

        public override void Restart()
        {
            _session.Restart();
            Notify();
        }

        protected override void PublishState()
        {
            HudLines = new[]
            {
                $"Mejor: {BestScore}",
                $"Golpes: {PunchesRemaining}",
                $"Último: {LastPunchPower}",
                StatusText,
            };
        }

        string Describe(MatchPhase phase)
        {
            switch (phase)
            {
                case MatchPhase.Playing: return $"¡Supera {WinScoreThreshold}!";
                case MatchPhase.Won: return "¡Fuerza!";
                case MatchPhase.Lost: return "¡Muy flojo!";
                default: return "Listo";
            }
        }
    }
}
