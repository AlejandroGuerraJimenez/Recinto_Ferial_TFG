namespace Fairground.View
{
    public sealed class PlanePointer
    {
        public PlaneSurfaceVisual Selected { get; set; }

        public void ClearIf(PlaneSurfaceVisual visual)
        {
            if (Selected == visual)
                Selected = null;
        }
    }
}
