using Fairground.Core.Attractions;
using Fairground.Core.Planes;
using Fairground.View.Attractions;
using Fairground.ViewModel;
using UnityEngine;

namespace Fairground.View
{
    /// <summary>
    /// Keeps parcels on the confirmed flat surface, then loads the attraction the user aims at.
    /// </summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(50)]
    [AddComponentMenu("Fairground/Fairground Attraction Flow")]
    public sealed class FairgroundAttractionFlow : MonoBehaviour
    {
        [SerializeField] PlaneSelectionView planeSelection;
        [SerializeField] AttractionId attractionId = AttractionId.BalloonThrow;

        readonly FairgroundParcelPads _parcels = new FairgroundParcelPads();
        DetectedPlane? _locked;
        FairgroundSession _session;

        void OnEnable()
        {
            AttractionViewpoint.Clear();
            if (planeSelection == null)
                return;

            planeSelection.OnPlaneSelected += HandlePlaneSelected;
            planeSelection.OnLockedConfirm += HandleEnter;
            planeSelection.OnRestarted += HandleRestarted;
            RestoreParcelMenu();
        }

        void OnDisable()
        {
            if (planeSelection != null)
            {
                planeSelection.OnPlaneSelected -= HandlePlaneSelected;
                planeSelection.OnLockedConfirm -= HandleEnter;
                planeSelection.OnRestarted -= HandleRestarted;
            }

            _parcels.Hide();
        }

        void LateUpdate()
        {
            if (planeSelection == null)
                return;

            Ray? aim = null;
            if (planeSelection.TryGetAim(out var ray))
                aim = ray;

            _parcels.Show(planeSelection.Proposal, aim);
        }

        void HandlePlaneSelected(DetectedPlane plane)
        {
            _locked = plane;
            _parcels.PinTo(planeSelection.LockedSurface != null ? planeSelection.LockedSurface : planeSelection.Proposal);
            RememberViewpoint();
            CurrentSession().Remember(plane);
        }

        void HandleEnter()
        {
            if (!_locked.HasValue)
                return;

            var chosen = AimedParcel();
            RememberViewpoint();
            var session = CurrentSession();
            session.Remember(_locked.Value, chosen);
            FairgroundVisit.Deliver(session);
            AttractionExit.Enter(AttractionScenes.GetSceneName(chosen));
        }

        void RestoreParcelMenu()
        {
            if (!FairgroundVisit.TryReceive(out var session) || !session.SelectedPlane.HasValue)
                return;

            _session = session;
            planeSelection.TryRestore(session.SelectedPlane.Value);
        }

        void HandleRestarted()
        {
            _locked = null;
            _parcels.Release();
        }

        AttractionId AimedParcel()
        {
            if (planeSelection != null && planeSelection.TryGetAim(out var ray) && _parcels.TryPick(ray, out var aimed))
                return aimed;

            return attractionId;
        }

        FairgroundSession CurrentSession()
        {
            if (_session == null && !FairgroundVisit.TryReceive(out _session))
                _session = new FairgroundSession();

            FairgroundVisit.Deliver(_session);
            return _session;
        }

        static void RememberViewpoint()
        {
            var camera = Camera.main;
            if (camera == null)
                return;

            AttractionViewpoint.Remember(camera.transform.position, camera.transform.eulerAngles.y);
        }
    }
}
