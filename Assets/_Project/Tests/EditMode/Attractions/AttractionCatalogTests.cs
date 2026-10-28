using System;
using Fairground.Core.Attractions;
using NUnit.Framework;

namespace Fairground.Tests.EditMode.Attractions
{
    public sealed class AttractionCatalogTests
    {
        [Test]
        public void EveryAttractionId_HasOneDescriptor()
        {
            var ids = (AttractionId[])Enum.GetValues(typeof(AttractionId));
            var playable = 0;
            for (var i = 0; i < ids.Length; i++)
            {
                var id = ids[i];
                if (id == AttractionId.None)
                {
                    Assert.IsFalse(AttractionCatalog.TryGet(id, out _));
                    continue;
                }

                playable++;
                Assert.IsTrue(AttractionCatalog.TryGet(id, out var attraction), id.ToString());
                Assert.AreEqual(id, attraction.Id);
                Assert.AreEqual(attraction.SceneName, AttractionCatalog.GetSceneName(id));
                Assert.AreEqual(attraction.DisplayName, AttractionCatalog.DisplayName(id));
            }

            Assert.AreEqual(playable, AttractionCatalog.Playable.Length);
        }

        [Test]
        public void Playable_RecordsIdNameAndSceneOnce()
        {
            AssertDescriptor(AttractionCatalog.BalloonThrow, AttractionId.BalloonThrow, "Globos", "BalloonThrow");
            AssertDescriptor(AttractionCatalog.DuckFishing, AttractionId.DuckFishing, "Patos", "DuckFishing");
            AssertDescriptor(AttractionCatalog.PunchingBall, AttractionId.PunchingBall, "Puñetazo", "PunchingBall");
            AssertDescriptor(AttractionCatalog.FerrisWheel, AttractionId.FerrisWheel, "Noria", "FerrisWheel");
            AssertDescriptor(AttractionCatalog.BasketballHoop, AttractionId.BasketballHoop, "Canasta", "BasketballHoop");
        }

        [Test]
        public void UnknownId_HasNoSceneAndUsesParcelFallback()
        {
            Assert.AreEqual("Parcela", AttractionCatalog.DisplayName(AttractionId.None));
            Assert.IsFalse(AttractionCatalog.TryGetBySceneName("Missing", out _));
            Assert.Throws<ArgumentOutOfRangeException>(() => AttractionCatalog.GetSceneName(AttractionId.None));
        }

        static void AssertDescriptor(
            AttractionDescriptor attraction,
            AttractionId id,
            string displayName,
            string sceneName)
        {
            Assert.AreEqual(id, attraction.Id);
            Assert.AreEqual(displayName, attraction.DisplayName);
            Assert.AreEqual(sceneName, attraction.SceneName);
            Assert.IsTrue(AttractionCatalog.TryGet(id, out var byId));
            Assert.AreSame(attraction, byId);
            Assert.IsTrue(AttractionCatalog.TryGetBySceneName(sceneName, out var byScene));
            Assert.AreSame(attraction, byScene);
            Assert.AreEqual(
                AttractionScenes.AttractionsFolder + "/" + sceneName + ".unity",
                AttractionScenes.GetScenePath(id));
        }
    }
}
