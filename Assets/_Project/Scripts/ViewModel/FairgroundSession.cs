using Fairground.Core.Attractions;
using Fairground.Model;

namespace Fairground.ViewModel
{
    /// <summary>
    /// One visit: the floor the player confirmed and the attraction they are entering.
    /// The fairground creates it and the attraction installer receives that same instance.
    /// </summary>
    public sealed class FairgroundSession
    {
        public DetectedPlane? SelectedPlane { get; private set; }

        public AttractionId PendingAttraction { get; private set; } = AttractionId.None;

        public void Remember(DetectedPlane plane) => Remember(plane, AttractionId.None);

        public void Remember(DetectedPlane plane, AttractionId attractionId)
        {
            SelectedPlane = plane;
            PendingAttraction = attractionId;
        }

        /// <summary>
        /// Drops the attraction that was entered and keeps the chosen floor,
        /// so the fairground can show the parcel menu again.
        /// </summary>
        public void LeaveAttraction() => PendingAttraction = AttractionId.None;

        public void Clear()
        {
            SelectedPlane = null;
            PendingAttraction = AttractionId.None;
        }
    }
}
