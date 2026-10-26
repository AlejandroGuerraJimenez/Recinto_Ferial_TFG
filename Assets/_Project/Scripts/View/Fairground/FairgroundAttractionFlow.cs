using Fairground.Core.Attractions;
using Fairground.Model;
using Fairground.View.Attractions;
using Fairground.ViewModel;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fairground.View
{
    /// <summary>
    /// Loads an attraction scene after the user confirms a horizontal surface.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Fairground/Fairground Attraction Flow")]
    public sealed class FairgroundAttractionFlow : MonoBehaviour
    {
        [SerializeField] PlaneSelectionView planeSelection;
        [SerializeField] AttractionId attractionId = AttractionId.BalloonThrow;

        readonly FairgroundParcelPads _parcels = new FairgroundParcelPads();

        void OnEnable()
        {
            AttractionViewpoint.Clear();
            if (planeSelection != null)
                planeSelection.OnPlaneSelected += HandlePlaneSelected;
        }

        void OnDisable()
        {
            if (planeSelection != null)
                planeSelection.OnPlaneSelected -= HandlePlaneSelected;
            _parcels.Hide();
        }

        void LateUpdate()
        {
            if (planeSelection == null)
                return;

            _parcels.Show(planeSelection.Proposal);
        }

        void HandlePlaneSelected(DetectedPlane plane)
        {
            var chosen = AimedParcel();
            RememberViewpoint();
            FairgroundSession.Remember(plane, chosen);
            SceneManager.LoadScene(AttractionScenes.GetSceneName(chosen));
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
