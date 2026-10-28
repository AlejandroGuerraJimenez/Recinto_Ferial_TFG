namespace Fairground.Core.Attractions
{
    /// <summary>
    /// Scene folders and load paths. Attraction scene names come from <see cref="AttractionCatalog"/>.
    /// </summary>
    public static class AttractionScenes
    {
        public const string FairgroundSceneFolder = "Assets/_Project/Scenes";
        public const string Fairground = "Fairground";
        public const string AttractionsFolder = "Assets/_Project/Scenes/Attractions";

        public static string GetSceneName(AttractionId attractionId)
        {
            return AttractionCatalog.GetSceneName(attractionId);
        }

        public static string GetScenePath(AttractionId attractionId)
        {
            return AttractionsFolder + "/" + GetSceneName(attractionId) + ".unity";
        }

        public static bool TryGetAttractionId(string sceneName, out AttractionId attractionId)
        {
            if (!AttractionCatalog.TryGetBySceneName(sceneName, out var attraction))
            {
                attractionId = AttractionId.None;
                return false;
            }

            attractionId = attraction.Id;
            return true;
        }
    }
}
