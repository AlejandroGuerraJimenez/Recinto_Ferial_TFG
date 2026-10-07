using Fairground.Model;

namespace Fairground.ViewModel
{
    /// <summary>
    /// Presentation session shared across scenes. The view writes it; attractions read it.
    /// </summary>
    public static class FairgroundSession
    {
        public static DetectedPlane? SelectedPlane { get; private set; }

        public static void Remember(DetectedPlane plane) => SelectedPlane = plane;

        public static void Clear() => SelectedPlane = null;
    }
}
