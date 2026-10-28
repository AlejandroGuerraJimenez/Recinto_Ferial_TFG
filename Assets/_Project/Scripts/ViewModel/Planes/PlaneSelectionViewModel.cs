using System;
using Fairground.Core.Planes;
using Fairground.Model;

namespace Fairground.ViewModel
{
    /// <summary>
    /// Selection commands. Each command is handled by the current state object.
    /// </summary>
    public sealed class PlaneSelectionViewModel
    {
        readonly PlaneSelectionContext _context;

        public PlaneSelectionViewModel()
        {
            _context = new PlaneSelectionContext(AwaitingPlaneSelection.Instance);
            _context.StateChanged += NotifyState;
            _context.PlaneSelected += NotifySelected;
        }

        public PlaneSelectionState State => _context.State.Kind;

        public DetectedPlane? SelectedPlane => _context.Selected;

        public event Action<PlaneSelectionState> StateChanged;

        public event Action<DetectedPlane> PlaneSelected;

        public bool SelectPlane(DetectedPlane plane) => _context.State.Select(_context, plane);

        public bool TryRank(DetectedPlane plane, out PlaneRank rank) => PlaneCandidateFilter.TryRank(plane, out rank);

        public bool IsSelectable(DetectedPlane plane) => TryRank(plane, out _);

        public void RestartSelection() => _context.State.Restart(_context);

        void NotifyState(PlaneSelectionState state) => StateChanged?.Invoke(state);

        void NotifySelected(DetectedPlane plane) => PlaneSelected?.Invoke(plane);
    }
}
