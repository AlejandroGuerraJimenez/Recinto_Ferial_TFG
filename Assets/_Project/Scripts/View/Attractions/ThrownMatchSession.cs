using System;
using Fairground.ViewModel;
using Fairground.ViewModel.Attractions;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// Throw, resolve, HUD, and restart shared by booths that spawn a prop.
    /// The booth supplies the scoring verb and, when it must, a target rebuild.
    /// </summary>
    public sealed class ThrownMatchSession : IDisposable
    {
        readonly ThrownPropTracker _props = new ThrownPropTracker();
        readonly Action _restart;

        BoothMatchViewModel _viewModel;
        BallSpawnerView _spawner;
        Func<bool> _registerThrow;
        Func<bool> _resolveThrow;
        Func<bool> _canThrow;
        AttractionMatchControls _controls;

        public ThrownMatchSession(Action restart)
        {
            _restart = restart ?? throw new ArgumentNullException(nameof(restart));
        }

        public AttractionInput Input => _controls != null ? _controls.Input : null;

        public void Bind()
        {
            if (_controls != null)
                return;

            _controls = new AttractionMatchControls(_restart);
            _props.Thrown += OnThrown;
            _props.Resolved += OnResolved;
        }

        public void Tick() => _controls?.Tick();

        public void Use(FairgroundVisitState visit) => _controls?.Use(visit);

        public void Open(
            BoothMatchViewModel viewModel,
            BallSpawnerView spawner,
            BoothHudView hud,
            Func<string[]> lines,
            Func<bool> registerThrow,
            Func<bool> resolveThrow,
            Func<bool> canThrow)
        {
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            _spawner = spawner ?? throw new ArgumentNullException(nameof(spawner));
            _registerThrow = registerThrow ?? throw new ArgumentNullException(nameof(registerThrow));
            _resolveThrow = resolveThrow ?? throw new ArgumentNullException(nameof(resolveThrow));
            _canThrow = canThrow ?? throw new ArgumentNullException(nameof(canThrow));
            BoothHudBinding.Attach(hud, viewModel, lines);
            _props.Attach(spawner);
            viewModel.StartMatch();
            RefreshSpawner();
        }

        /// <summary>
        /// Applies a scoring command and refreshes the spawner when the match accepted it.
        /// </summary>
        public void ApplyAndRefresh(Func<bool> action)
        {
            if (_viewModel == null || action == null)
                return;

            if (action())
                RefreshSpawner();
        }

        public void Restart(Action rebuildTargets)
        {
            if (_viewModel == null)
                return;

            _props.Detach();
            _props.DestroyAll(_spawner);
            _viewModel.Restart();
            rebuildTargets?.Invoke();
            _props.Attach(_spawner);
            RefreshSpawner();
            _spawner?.TrySpawnImmediate();
        }

        public void Dispose()
        {
            _props.Thrown -= OnThrown;
            _props.Resolved -= OnResolved;
            _props.Detach();
            _controls?.Dispose();
            _controls = null;
        }

        void OnThrown() => ApplyAndRefresh(_registerThrow);

        void OnResolved() => ApplyAndRefresh(_resolveThrow);

        void RefreshSpawner()
        {
            if (_spawner == null || _canThrow == null)
                return;

            _spawner.SetSpawningEnabled(_canThrow());
        }
    }
}
