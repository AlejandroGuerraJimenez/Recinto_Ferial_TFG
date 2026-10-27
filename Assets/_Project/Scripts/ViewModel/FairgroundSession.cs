using Fairground.Core.Attractions;
using Fairground.Model;

namespace Fairground.ViewModel
{
    /// <summary>
    /// Presentation session shared across scenes. Fairground writes; attractions read.
    /// </summary>
    public static class FairgroundSession
    {
        public static DetectedPlane? SelectedPlane { get; private set; }

        public static AttractionId PendingAttraction { get; private set; } = AttractionId.None;

        public static void Remember(DetectedPlane plane) => Remember(plane, AttractionId.None);

        public static void Remember(DetectedPlane plane, AttractionId attractionId)
        {
            SelectedPlane = plane;
            PendingAttraction = attractionId;
        }

        /// <summary>
        /// Drops the attraction that was entered and keeps the chosen floor,
        /// so the fairground can show the parcel menu again.
        /// </summary>
        public static void LeaveAttraction() => PendingAttraction = AttractionId.None;

        public static void Clear()
        {
            SelectedPlane = null;
            PendingAttraction = AttractionId.None;
        }
    }
}
