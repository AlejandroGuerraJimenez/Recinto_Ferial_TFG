using System;
using System.Collections.Generic;

namespace Fairground.Core.Attractions
{
    /// <summary>
    /// Playable attractions. Each entry is one <see cref="AttractionDescriptor"/>
    /// (id, visible name, scene). Lookups are indexed from that list.
    /// The id list stays closed because each scene is fixed in the player build.
    /// A new scoring booth adds an <see cref="AttractionId"/>, a row in <see cref="Playable"/>,
    /// rules that build an attempt match, a view model that names those counters,
    /// and the view of its props. It does not add a session type.
    /// Punch and the wheel keep their own model.
    /// </summary>
    public static class AttractionCatalog
    {
        public static readonly AttractionDescriptor BalloonThrow =
            new AttractionDescriptor(AttractionId.BalloonThrow, "Globos", "BalloonThrow");

        public static readonly AttractionDescriptor DuckFishing =
            new AttractionDescriptor(AttractionId.DuckFishing, "Patos", "DuckFishing");

        public static readonly AttractionDescriptor PunchingBall =
            new AttractionDescriptor(AttractionId.PunchingBall, "Puñetazo", "PunchingBall");

        public static readonly AttractionDescriptor FerrisWheel =
            new AttractionDescriptor(AttractionId.FerrisWheel, "Noria", "FerrisWheel");

        public static readonly AttractionDescriptor BasketballHoop =
            new AttractionDescriptor(AttractionId.BasketballHoop, "Canasta", "BasketballHoop");

        public static readonly AttractionDescriptor[] Playable =
        {
            BalloonThrow,
            DuckFishing,
            PunchingBall,
            FerrisWheel,
            BasketballHoop,
        };

        static readonly Dictionary<AttractionId, AttractionDescriptor> ById;
        static readonly Dictionary<string, AttractionDescriptor> BySceneName;

        static AttractionCatalog()
        {
            ById = new Dictionary<AttractionId, AttractionDescriptor>(Playable.Length);
            BySceneName = new Dictionary<string, AttractionDescriptor>(Playable.Length);
            for (var i = 0; i < Playable.Length; i++)
            {
                var attraction = Playable[i];
                ById.Add(attraction.Id, attraction);
                BySceneName.Add(attraction.SceneName, attraction);
            }
        }

        public static bool TryGet(AttractionId attractionId, out AttractionDescriptor attraction)
        {
            return ById.TryGetValue(attractionId, out attraction);
        }

        public static bool TryGetBySceneName(string sceneName, out AttractionDescriptor attraction)
        {
            return BySceneName.TryGetValue(sceneName, out attraction);
        }

        public static string DisplayName(AttractionId attractionId)
        {
            if (TryGet(attractionId, out var attraction))
                return attraction.DisplayName;

            return "Parcela";
        }

        public static string GetSceneName(AttractionId attractionId)
        {
            if (TryGet(attractionId, out var attraction))
                return attraction.SceneName;

            throw new ArgumentOutOfRangeException(nameof(attractionId), attractionId, "Unknown attraction.");
        }
    }
}
