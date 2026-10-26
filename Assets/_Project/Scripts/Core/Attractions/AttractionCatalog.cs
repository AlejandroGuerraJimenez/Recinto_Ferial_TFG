namespace Fairground.Core.Attractions
{
    /// <summary>
    /// Playable attractions shown as parcels on the chosen flat surface.
    /// </summary>
    public static class AttractionCatalog
    {
        public static readonly AttractionId[] Playable =
        {
            AttractionId.BalloonThrow,
            AttractionId.DuckFishing,
            AttractionId.PunchingBall,
            AttractionId.FerrisWheel,
            AttractionId.BasketballHoop,
        };

        public static string DisplayName(AttractionId attractionId)
        {
            switch (attractionId)
            {
                case AttractionId.BalloonThrow: return "Globos";
                case AttractionId.DuckFishing: return "Patos";
                case AttractionId.PunchingBall: return "Punching";
                case AttractionId.FerrisWheel: return "Noria";
                case AttractionId.BasketballHoop: return "Canasta";
                default: return "Parcela";
            }
        }
    }
}
