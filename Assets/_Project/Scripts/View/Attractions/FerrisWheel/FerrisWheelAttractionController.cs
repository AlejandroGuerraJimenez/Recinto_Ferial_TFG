using Fairground.Core.Attractions;
using Fairground.ViewModel;
using Fairground.ViewModel.Attractions.FerrisWheel;
using UnityEngine;

namespace Fairground.View.Attractions.FerrisWheel
{
    /// <summary>
    /// View facade: owns the ride ViewModel and pushes its angle into the rig.
    /// </summary>
    public sealed class FerrisWheelAttractionController : MonoBehaviour
    {
        [SerializeField] FerrisWheelRigView rig;

        FerrisWheelViewModel _viewModel;
        AttractionInput _input;
        FairgroundSession _session;

        public AttractionId AttractionId => AttractionId.FerrisWheel;
        public FerrisWheelViewModel ViewModel => _viewModel;
        public AttractionInput Input => _input;

        public void Configure(FerrisWheelRigView wheelRig, int cabins, FerrisWheelViewModel match)
        {
            rig = wheelRig;
            _viewModel = match;
            int built = rig != null ? rig.CabinCount : 0;
            if (built < 1)
                Debug.LogError("[FerrisWheel] The ride has no cabins.");
            else if (cabins != built)
                Debug.LogError(
                    "[FerrisWheel] Cabin count " + cabins + " does not match the wheel (" + built +
                    "). Cabins come from " + nameof(LondonEyeLayout) + ".");

            if (_viewModel == null)
                return;

            _viewModel.StartRide();
            rig?.ApplyAngle(_viewModel.AngleDegrees);
        }

        public void Receive(FairgroundSession session) => _session = session;

        void Awake()
        {
            _input = new AttractionInput(restart: false, play: false);
            _input.Enable();
        }

        void Update()
        {
            if (_viewModel == null)
                return;

            _viewModel.Advance(Time.deltaTime);
            rig?.ApplyAngle(_viewModel.AngleDegrees);

            if (_input != null && _input.ReturnToFairgroundPressed)
                AttractionExit.ReturnToFairground(_session);
        }

        void OnDestroy()
        {
            _input?.Dispose();
            _input = null;
        }
    }
}
