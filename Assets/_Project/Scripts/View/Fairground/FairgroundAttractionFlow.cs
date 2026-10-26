using Fairground.Core.Attractions;
using Fairground.Model;
using Fairground.View.Attractions;
using Fairground.ViewModel;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        void OnEnable()
        {
            AttractionViewpoint.Clear();
            if (planeSelection == null)
                return;

            planeSelection.OnPlaneSelected += HandlePlaneSelected;
            planeSelection.OnLockedConfirm += HandleEnter;
            planeSelection.OnRestarted += HandleRestarted;
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
            FairgroundSession.Remember(plane);
        }

        void HandleEnter()
        {
            if (!_locked.HasValue)
                return;

            var chosen = AimedParcel();
            RememberViewpoint();
            FairgroundSession.Remember(_locked.Value, chosen);
            SceneManager.LoadScene(AttractionScenes.GetSceneName(chosen));
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

        static void RememberViewpoint()
        {
            var camera = Camera.main;
            if (camera == null)
                return;

            AttractionViewpoint.Remember(camera.transform.position, camera.transform.eulerAngles.y);
        }
    }
}
