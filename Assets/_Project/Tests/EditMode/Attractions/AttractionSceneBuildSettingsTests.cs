using System.Linq;
using Fairground.Core.Attractions;
using NUnit.Framework;
using UnityEditor;

namespace Fairground.Tests.EditMode.Attractions
{
    public sealed class AttractionSceneBuildSettingsTests
    {
        [Test]
        public void EveryPlayableAttraction_IsEnabledInBuildSettings()
        {
            var playable = AttractionCatalog.Playable;
            Assert.Greater(playable.Length, 0);

            for (var i = 0; i < playable.Length; i++)
                AssertSceneEnabled(playable[i].Id);
        }

        static void AssertSceneEnabled(AttractionId attractionId)
        {
            string path = AttractionScenes.GetScenePath(attractionId);
            var scene = EditorBuildSettings.scenes.FirstOrDefault(s => s.path == path);

            Assert.IsNotNull(scene, $"Scene missing from Build Settings: {path}");
            Assert.IsTrue(scene.enabled, $"Scene disabled in Build Settings: {path}");
            Assert.IsTrue(System.IO.File.Exists(path), $"Scene asset file missing: {path}");
        }
    }
}
