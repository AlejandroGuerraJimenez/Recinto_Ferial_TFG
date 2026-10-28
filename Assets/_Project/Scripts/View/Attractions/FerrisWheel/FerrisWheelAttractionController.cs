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
        [SerializeField] int cabinCount = FerrisWheelRules.DefaultCabinCount;
        [SerializeField] FerrisWheelRigView rig;

        FerrisWheelViewModel _viewModel;
        AttractionInput _input;

        public AttractionId AttractionId => AttractionId.FerrisWheel;
        public FerrisWheelViewModel ViewModel => _viewModel;
        public AttractionInput Input => _input;

        public void Configure(FerrisWheelRigView wheelRig, float speed, int cabins)
        {
            rig = wheelRig;
            degreesPerSecond = speed;
            cabinCount = cabins;
        }

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

            _viewModel = new FerrisWheelViewModel(degreesPerSecond, cabinCount);
            _viewModel.StartRide();
            rig?.ApplyAngle(_viewModel.AngleDegrees);
        }

        public void ReturnToFairground()
        {
            FairgroundSession.LeaveAttraction();
            SceneManager.LoadScene(AttractionScenes.Fairground);
        }
    }
}
