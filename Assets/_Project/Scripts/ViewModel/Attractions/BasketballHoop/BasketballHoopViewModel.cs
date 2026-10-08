using System;
using Fairground.Core.Attractions;
using Fairground.Model.Attractions.BasketballHoop;

namespace Fairground.ViewModel.Attractions.BasketballHoop
{
    /// <summary>
    /// MVVM presentation state. Observer via <see cref="StateChanged"/>.
    /// </summary>
    public sealed class BasketballHoopViewModel
    {
        readonly BasketballHoopSession _session;
        readonly IBasketballHoopStatusPresenter _statusPresenter;

        public BasketballHoopViewModel(
            BasketballHoopRules rules = null,
            IBasketballHoopStatusPresenter statusPresenter = null)
        {
            _session = new BasketballHoopSession(rules ?? new BasketballHoopRules());
            _statusPresenter = statusPresenter ?? new DefaultBasketballHoopStatusPresenter();
        }

        public BasketballHoopViewModel(
            int pointsPerBasket,
            int startingBalls,
            int winScoreThreshold,
            IBasketballHoopStatusPresenter statusPresenter = null)
            : this(new BasketballHoopRules(pointsPerBasket, startingBalls, winScoreThreshold), statusPresenter)
        {
        }

        public AttractionId AttractionId => AttractionId.BasketballHoop;
        public string SceneName => AttractionScenes.BasketballHoop;
        public event Action StateChanged;

        public BasketballHoopPhase Phase => _session.Phase;
        public int Score => _session.Score;
        public int BallsRemaining => _session.BallsRemaining;
        public bool IsPlaying => Phase == BasketballHoopPhase.Playing;
        public bool CanThrow => IsPlaying && BallsRemaining > 0;
        public string StatusText => _statusPresenter.Present(Phase);

        public void StartGame()
        {
            _session.Start();
            RaiseStateChanged();
        }

        public bool NotifyBallThrown() => Apply(_session.TryRegisterThrow);

        public bool NotifyBasket() => Apply(_session.TryRegisterBasket);

        public bool NotifyThrowResolved() => Apply(_session.ResolveThrow);

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
