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

        readonly FairgroundParcelPads _parcels = new FairgroundParcelPads();
        DetectedPlane? _locked;
        FairgroundVisitState _visit;

        void OnEnable()
        {
            if (planeSelection == null)
                return;

            planeSelection.PlaneSelected += HandlePlaneSelected;
            planeSelection.LockedConfirm += HandleEnter;
            planeSelection.OnRestarted += HandleRestarted;
            RestoreParcelMenu();
        }

        void OnDisable()
        {
            if (planeSelection != null)
            {
                planeSelection.PlaneSelected -= HandlePlaneSelected;
                planeSelection.LockedConfirm -= HandleEnter;
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
            EnsureVisit().Remember(plane);
            FairgroundVisit.Deliver(_visit);
        }

        void HandleEnter()
        {
            AttractionId aimed = AttractionId.None;
            bool hit = TryAimedParcel(out aimed);
            if (!ParcelEntry.TryChoose(_locked.HasValue && hit, aimed, out AttractionId chosen))
                return;

            var visit = EnsureVisit();
            visit.Remember(_locked.Value, chosen);
            FairgroundVisit.Deliver(visit);
            AttractionExit.Enter(AttractionScenes.GetSceneName(chosen));
        }

        void RestoreParcelMenu()
        {
            if (!FairgroundVisit.TryReceive(out var visit) || !visit.SelectedPlane.HasValue)
                return;

            _visit = visit;
            planeSelection.TryRestore(visit.SelectedPlane.Value);
        }

        void HandleRestarted()
        {
            _locked = null;
            _parcels.Release();
        }

        bool TryAimedParcel(out AttractionId attractionId)
        {
            attractionId = AttractionId.None;
            return planeSelection != null
                && planeSelection.TryGetAim(out var ray)
                && _parcels.TryPick(ray, out attractionId);
        }

        FairgroundVisitState EnsureVisit()
        {
            if (_visit == null && !FairgroundVisit.TryReceive(out _visit))
                _visit = new FairgroundVisitState();

            return _visit;
        }
    }
}
