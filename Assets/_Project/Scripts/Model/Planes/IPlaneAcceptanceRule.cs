namespace Fairground.Model
{
    public interface IPlaneAcceptanceRule
    {
        bool TryAccept(in DetectedPlane plane, out int priority);
    }
}
