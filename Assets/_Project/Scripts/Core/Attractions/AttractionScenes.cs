using System;
using System.Collections.Generic;

namespace Fairground.Core.Attractions
{
    /// <summary>
    /// Catalog of attraction VR scene names/paths for future LoadScene wiring.
    /// </summary>
    public static class AttractionScenes
    {
        public const string FairgroundSceneFolder = "Assets/_Project/Scenes";
        public const string Fairground = "Fairground";
        public const string AttractionsFolder = "Assets/_Project/Scenes/Attractions";
        public const string BalloonThrow = "BalloonThrow";
        public const string DuckFishing = "DuckFishing";
        public const string PunchingBall = "PunchingBall";
        public const string FerrisWheel = "FerrisWheel";

        static readonly Dictionary<AttractionId, string> NamesById = new Dictionary<AttractionId, string>
        {
            { AttractionId.BalloonThrow, BalloonThrow },
            { AttractionId.DuckFishing, DuckFishing },
            { AttractionId.PunchingBall, PunchingBall },
            { AttractionId.FerrisWheel, FerrisWheel },
        };

        static readonly Dictionary<string, AttractionId> IdsByName = new Dictionary<string, AttractionId>
        {
            { BalloonThrow, AttractionId.BalloonThrow },
            { DuckFishing, AttractionId.DuckFishing },
            { PunchingBall, AttractionId.PunchingBall },
            { FerrisWheel, AttractionId.FerrisWheel },
        };

        public static string GetSceneName(AttractionId attractionId)
        {
            if (NamesById.TryGetValue(attractionId, out string name))
                return name;

            throw new ArgumentOutOfRangeException(nameof(attractionId), attractionId, "Unknown attraction.");
        }

        public static string GetScenePath(AttractionId attractionId)
        {
            return $"{AttractionsFolder}/{GetSceneName(attractionId)}.unity";
        }

        public static bool TryGetAttractionId(string sceneName, out AttractionId attractionId)
        {
            return IdsByName.TryGetValue(sceneName, out attractionId);
        }
    }
}
