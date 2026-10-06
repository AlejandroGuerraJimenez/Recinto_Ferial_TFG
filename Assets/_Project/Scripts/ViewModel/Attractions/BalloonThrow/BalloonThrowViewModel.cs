using System;
using Fairground.Core.Attractions;
using Fairground.Model.Attractions.BalloonThrow;

namespace Fairground.ViewModel.Attractions.BalloonThrow
{
    /// <summary>
    /// MVVM presentation state. Observer via <see cref="StateChanged"/>.
    /// </summary>
    public sealed class BalloonThrowViewModel
    {
        readonly BalloonThrowSession _session;
        readonly IBalloonThrowStatusPresenter _statusPresenter;

        public BalloonThrowViewModel(
            BalloonThrowRules rules = null,
            IBalloonThrowStatusPresenter statusPresenter = null)
        {
            _session = new BalloonThrowSession(rules ?? new BalloonThrowRules());
            _statusPresenter = statusPresenter ?? new DefaultBalloonThrowStatusPresenter();
        }

        public AttractionId AttractionId => AttractionId.BalloonThrow;
        public string SceneName => AttractionScenes.BalloonThrow;
        public event Action StateChanged;

        public BalloonThrowPhase Phase => _session.Phase;
        public int Score => _session.Score;
        public int ThrowsRemaining => _session.ThrowsRemaining;
        public int BalloonsRemaining => _session.BalloonsRemaining;
        public int BalloonsPopped => _session.BalloonsPopped;
        public bool IsPlaying => Phase == BalloonThrowPhase.Playing;
        public string StatusText => _statusPresenter.Present(Phase);

        public void StartGame()
        {
            _session.Start();
            RaiseStateChanged();
        }

        public bool NotifyBallThrown() => Apply(_session.TryRegisterThrow);

        public bool NotifyBalloonHit() => Apply(_session.TryRegisterBalloonHit);

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
