using System;
using Fairground.Core.Attractions;
using Fairground.Model.Attractions.DuckFishing;

namespace Fairground.ViewModel.Attractions.DuckFishing
{
    /// <summary>
    /// MVVM presentation state. Observer via <see cref="StateChanged"/>.
    /// </summary>
    public sealed class DuckFishingViewModel
    {
        readonly DuckFishingSession _session;
        readonly IDuckFishingStatusPresenter _statusPresenter;

        public DuckFishingViewModel(
            DuckFishingRules rules = null,
            IDuckFishingStatusPresenter statusPresenter = null)
        {
            _session = new DuckFishingSession(rules ?? new DuckFishingRules());
            _statusPresenter = statusPresenter ?? new DefaultDuckFishingStatusPresenter();
        }

        public DuckFishingViewModel(
            int pointsPerDuck,
            int startingAttempts,
            int duckCount,
            int ducksToWin,
            IDuckFishingStatusPresenter statusPresenter = null)
            : this(new DuckFishingRules(pointsPerDuck, startingAttempts, duckCount, ducksToWin), statusPresenter)
        {
        }

        public AttractionId AttractionId => AttractionId.DuckFishing;
        public string SceneName => AttractionScenes.DuckFishing;
        public event Action StateChanged;

        public DuckFishingPhase Phase => _session.Phase;
        public int Score => _session.Score;
        public int AttemptsRemaining => _session.AttemptsRemaining;
        public int DucksRemaining => _session.DucksRemaining;
        public int DucksCaught => _session.DucksCaught;
        public int DuckCount => _session.Rules.DuckCount;
        public int DucksToWin => _session.Rules.DucksToWin;
        public bool IsPlaying => Phase == DuckFishingPhase.Playing;
        public bool CanDip => IsPlaying && AttemptsRemaining > 0;
        public string StatusText => _statusPresenter.Present(Phase, DucksToWin);

        public void StartGame()
        {
            _session.Start();
            RaiseStateChanged();
        }

        public bool NotifyDipStarted() => Apply(_session.TryRegisterDip);

        public bool NotifyDuckCaught() => Apply(_session.TryRegisterCatch);

        public bool NotifyDipResolved() => Apply(_session.ResolveDip);

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
