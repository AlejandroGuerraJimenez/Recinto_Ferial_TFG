using Fairground.Core.Attractions;
using Fairground.View.Attractions.BalloonThrow.Factories;
using Fairground.ViewModel;
using Fairground.ViewModel.Attractions.BalloonThrow;
using UnityEngine;

namespace Fairground.View.Attractions.BalloonThrow
{
    /// <summary>
    /// Hosts the balloon match: targets, the shared throw session, and the HUD.
    /// </summary>
    public sealed class BalloonThrowAttractionHost : MonoBehaviour
    {
        [SerializeField] BalloonTargetView[] balloons;
        [SerializeField] BallSpawnerView ballSpawner;
        [SerializeField] BoothHudView hud;

        BalloonThrowViewModel _viewModel;
        ThrownMatchSession _match;
        readonly BalloonTargetFactory _balloonFactory = new BalloonTargetFactory();
        Transform _balloonRoot;
        FairgroundVisitState _visit;

        public AttractionId AttractionId => AttractionId.BalloonThrow;
        public BalloonThrowViewModel ViewModel => _viewModel;
        public AttractionInput Input => _match != null ? _match.Input : null;

        public void Configure(
            BalloonTargetView[] balloonTargets,
            BallSpawnerView spawner,
            BoothHudView hudView,
            BalloonThrowViewModel viewModel)
        {
            balloons = BoothWiring.RequireAny(balloonTargets, "Balloon throw", "balloons");
            ballSpawner = BoothWiring.Require(spawner, "Balloon throw", "a ball spawner");
            hud = BoothWiring.Require(hudView, "Balloon throw", "a HUD");
            _viewModel = BoothWiring.Require(viewModel, "Balloon throw", "a match");
            OpenMatch();
        }

        public void Receive(FairgroundVisitState visit)
        {
            _visit = visit;
            _match?.Use(visit);
        }

        void Awake()
        {
            _match = new ThrownMatchSession(RestartMatch);
            _match.Bind();
        }

        void Update() => _match.Tick();

        void OnDestroy()
        {
            UnsubscribeBalloons();
            _match?.Dispose();
        }

        void OpenMatch()
        {
            CaptureTargetRoot();
            SubscribeBalloons();
            _match.Open(
                _viewModel,
                ballSpawner,
                hud,
                () => BalloonThrowHud.Lines(_viewModel),
                _viewModel.TryRegisterThrow,
                _viewModel.TryResolveThrow,
                () => _viewModel.CanThrow);
        }

        void SubscribeBalloons()
        {
            if (balloons == null)
                return;

            foreach (var balloon in balloons)
                if (balloon != null)
                    balloon.Popped += OnBalloonPopped;
        }

        void UnsubscribeBalloons()
        {
            if (balloons == null)
                return;

            foreach (var balloon in balloons)
                if (balloon != null)
                    balloon.Popped -= OnBalloonPopped;
        }

        void OnBalloonPopped(BalloonTargetView balloon) => _match.ApplyAndRefresh(_viewModel.TryRegisterPop);

        public void RestartMatch()
        {
            if (_viewModel == null)
                return;

            UnsubscribeBalloons();
            _match.Restart(RebuildBalloons);
            SubscribeBalloons();
        }

        void RebuildBalloons()
        {
            if (CanRearm(balloons, _viewModel.BalloonCount))
            {
                for (var i = 0; i < balloons.Length; i++)
                    balloons[i].Rearm();

                return;
            }

            if (_balloonRoot != null)
                Destroy(_balloonRoot.gameObject);

            balloons = _balloonFactory.CreateRow(_viewModel.BalloonCount);
            CaptureTargetRoot();
            AttractionPlacement.TryApply(_visit, _balloonRoot);
        }

        static bool CanRearm(BalloonTargetView[] targets, int count)
        {
            if (targets == null || targets.Length != count)
                return false;

            for (var i = 0; i < targets.Length; i++)
            {
                if (targets[i] == null)
                    return false;
            }

            return true;
        }

        void CaptureTargetRoot()
        {
            if (balloons == null || balloons.Length == 0 || balloons[0] == null)
                return;

            _balloonRoot = balloons[0].transform.parent;
        }
    }
}
