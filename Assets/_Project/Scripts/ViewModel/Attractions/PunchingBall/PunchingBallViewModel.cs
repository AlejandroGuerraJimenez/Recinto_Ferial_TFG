using System;
using Fairground.Core.Attractions;
using Fairground.Model.Attractions.PunchingBall;

namespace Fairground.ViewModel.Attractions.PunchingBall
{
    /// <summary>
    /// MVVM presentation state. Observer via <see cref="StateChanged"/>.
    /// </summary>
    public sealed class PunchingBallViewModel
    {
        readonly PunchingBallSession _session;
        readonly IPunchingBallStatusPresenter _statusPresenter;

        public PunchingBallViewModel(
            PunchingBallRules rules = null,
            IPunchingBallStatusPresenter statusPresenter = null)
        {
            _session = new PunchingBallSession(rules ?? new PunchingBallRules());
            _statusPresenter = statusPresenter ?? new DefaultPunchingBallStatusPresenter();
        }

        public PunchingBallViewModel(
            int startingPunches,
            int winScoreThreshold,
            int maxScore,
            float maxImpactSpeed,
            IPunchingBallStatusPresenter statusPresenter = null)
            : this(
                new PunchingBallRules(startingPunches, winScoreThreshold, maxScore, maxImpactSpeed),
                statusPresenter)
        {
        }

        public AttractionId AttractionId => AttractionId.PunchingBall;
        public string SceneName => AttractionScenes.PunchingBall;
        public event Action StateChanged;

        public PunchingBallPhase Phase => _session.Phase;
        public int Score => _session.Score;
        public int BestScore => _session.BestScore;
        public int LastPunchPower => _session.LastPunchPower;
        public int PunchesRemaining => _session.PunchesRemaining;
        public int WinScoreThreshold => _session.Rules.WinScoreThreshold;
        public int MaxScore => _session.Rules.MaxScore;
        public bool IsPlaying => Phase == PunchingBallPhase.Playing;
        public bool CanPunch => IsPlaying && PunchesRemaining > 0;
        public string StatusText => _statusPresenter.Present(Phase, WinScoreThreshold);

        public void StartGame()
        {
            _session.Start();
            RaiseStateChanged();
        }

        public bool NotifyPunch(float impactMagnitude) => Apply(() => _session.TryRegisterPunch(impactMagnitude));

        public void Restart() => StartGame();

        bool Apply(Func<bool> action)
        {
            if (!action())
                return false;

            RaiseStateChanged();
            return true;
        }

        void RaiseStateChanged() => StateChanged?.Invoke();
    }
}
