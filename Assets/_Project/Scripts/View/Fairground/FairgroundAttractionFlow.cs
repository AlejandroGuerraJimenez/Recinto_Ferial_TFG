using Fairground.Core.Attractions;
using Fairground.Model;
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

        void OnEnable()
        {
            if (planeSelection != null)
                planeSelection.OnPlaneSelected += HandlePlaneSelected;
        }

        void OnDisable()
        {
            if (planeSelection != null)
                planeSelection.OnPlaneSelected -= HandlePlaneSelected;
        }

        void HandlePlaneSelected(DetectedPlane plane)
        {
            FairgroundSession.Remember(plane);
            SceneManager.LoadScene(AttractionScenes.GetSceneName(attractionId));
        }
    }
}
