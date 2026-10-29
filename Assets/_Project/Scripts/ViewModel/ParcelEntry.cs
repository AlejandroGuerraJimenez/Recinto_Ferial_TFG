using Fairground.Core.Attractions;

namespace Fairground.ViewModel
{
    /// <summary>
    /// Whether a locked floor and an aimed parcel are enough to enter.
    /// A miss does not choose a fallback attraction.
    /// </summary>
    public static class ParcelEntry
    {
        public static bool TryChoose(bool planeLockedAndHit, AttractionId aimed, out AttractionId chosen)
        {
            if (!planeLockedAndHit || aimed == AttractionId.None)
            {
                chosen = AttractionId.None;
                return false;
            }

            chosen = aimed;
            return true;
        }
    }
}
