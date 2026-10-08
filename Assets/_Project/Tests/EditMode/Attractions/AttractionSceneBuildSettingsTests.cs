using System.Linq;
using Fairground.Core.Attractions;
using NUnit.Framework;
using UnityEditor;

namespace Fairground.Tests.EditMode.Attractions
{
    public sealed class AttractionSceneBuildSettingsTests
    {
        [Test]
        public void BalloonThrowScene_IsEnabledInBuildSettings()
        {
            AssertSceneEnabled(AttractionId.BalloonThrow);
        }

        [Test]
        public void PunchingBallScene_IsEnabledInBuildSettings()
        {
            AssertSceneEnabled(AttractionId.PunchingBall);
        }

        [Test]
        public void BasketballHoopScene_IsEnabledInBuildSettings()
        {
            AssertSceneEnabled(AttractionId.BasketballHoop);
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
