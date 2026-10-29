using System.Collections.Generic;
using Fairground.Core.Attractions;
using Fairground.View.Attractions.BalloonThrow;
using Fairground.View.Attractions.BasketballHoop;
using Fairground.View.Attractions.DuckFishing;
using Fairground.View.Attractions.FerrisWheel;
using Fairground.View.Attractions.PunchingBall;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// Every playable attraction. A new booth adds one line here and its own folder.
    /// </summary>
    public static class AttractionModules
    {
        static readonly IAttractionModule[] Registered =
        {
            new BalloonThrowModule(),
            new DuckFishingModule(),
            new PunchingBallModule(),
            new FerrisWheelModule(),
            new BasketballHoopModule(),
        };

        static readonly Dictionary<AttractionId, IAttractionModule> ById;

        static AttractionModules()
        {
            ById = new Dictionary<AttractionId, IAttractionModule>(Registered.Length);
            for (var i = 0; i < Registered.Length; i++)
                ById.Add(Registered[i].Id, Registered[i]);
        }

        public static bool TryGet(AttractionId attractionId, out IAttractionModule module)
        {
            return ById.TryGetValue(attractionId, out module);
        }
    }
}
