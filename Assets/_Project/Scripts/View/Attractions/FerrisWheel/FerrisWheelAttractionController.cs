using Fairground.Core.Attractions;
using Fairground.Model.Attractions.FerrisWheel;
using Fairground.ViewModel;
using Fairground.ViewModel.Attractions.FerrisWheel;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fairground.View.Attractions.FerrisWheel
{
    /// <summary>
    /// View facade: owns the ride ViewModel and pushes its angle into the rig.
    /// </summary>
    public sealed class FerrisWheelAttractionController : MonoBehaviour
    {
        [SerializeField] float degreesPerSecond = FerrisWheelRules.DefaultDegreesPerSecond;
        [SerializeField] FerrisWheelRigView rig;

        FerrisWheelViewModel _viewModel;
        AttractionInput _input;
        FairgroundSession _session;

        public AttractionId AttractionId => AttractionId.FerrisWheel;
        public FerrisWheelViewModel ViewModel => _viewModel;
        public AttractionInput Input => _input;

        public void Configure(FerrisWheelRigView wheelRig, float speed, int cabins)
        {
            rig = wheelRig;
            degreesPerSecond = speed;
            int built = rig != null ? rig.CabinCount : 0;
            if (built < 1)
            {
                Debug.LogError("[FerrisWheel] The ride has no cabins.");
                return;
            }

            if (cabins != built)
            {
                Debug.LogError(
                    "[FerrisWheel] Cabin count " + cabins + " does not match the wheel (" + built +
                    "). Cabins come from " + nameof(LondonEyeLayout) + ".");
            }
        }

        public void Receive(FairgroundSession session) => _session = session;

        void Awake()
        {
            _input = new AttractionInput(restart: false, play: false);
            _input.Enable();
        }

        void Start()
        {
            if (_viewModel != null)
                return;

            Bootstrap();
        }

        void Update()
        {
            if (_viewModel == null)
                return;

            _viewModel.Advance(Time.deltaTime);
            rig?.ApplyAngle(_viewModel.AngleDegrees);

            if (_input != null && _input.ReturnToFairgroundPressed)
                ReturnToFairground();
        }

        void OnDestroy()
        {
            _input?.Dispose();
            _input = null;
        }

        void Bootstrap()
        {
            if (rig == null)
                rig = FindAnyObjectByType<FerrisWheelRigView>();

            _viewModel = new FerrisWheelViewModel(degreesPerSecond);
            _viewModel.StartRide();
            rig?.ApplyAngle(_viewModel.AngleDegrees);
        }

        public void ReturnToFairground()
        {
            _session?.LeaveAttraction();
            SceneManager.LoadScene(AttractionScenes.Fairground);
        }
    }
}
