using Fairground.Model.Attractions.BalloonThrow;
using Fairground.Core.Attractions;
using Fairground.View.Attractions.BalloonThrow;
using UnityEditor;

namespace Fairground.Editor.Attractions.BalloonThrow
{
    /// <summary>
    /// Menu and match defaults for the balloon booth. The shared builder bakes the scene.
    /// </summary>
    public static class BalloonThrowSceneBuilder
    {
        static readonly BoothSceneRecipe Recipe = new BoothSceneRecipe
        {
            Attraction = AttractionId.BalloonThrow,
            Title = "Balloon Throw",
            Layout = BalloonThrowBoothLayout.Props,
            BakedMaterials = new (string, string)[]
            {
                ("Ground", "BoothGround.mat"),
                ("BoothBack", "BoothBack.mat"),
            },
            InstallerType = typeof(BalloonThrowSceneInstaller),
            WriteMatchDefaults = so =>
            {
                so.FindProperty("balloonCount").intValue = BalloonThrowRules.DefaultBalloonCount;
                so.FindProperty("pointsPerBalloon").intValue = BalloonThrowRules.DefaultPointsPerBalloon;
                so.FindProperty("startingThrows").intValue = BalloonThrowRules.DefaultStartingThrows;
            },
        };

        [MenuItem("Fairground/Attractions/Build Balloon Throw Scene")]
        public static void BuildFromMenu()
        {
            BoothSceneBuilder.BuildFromMenu(Recipe);
        }

        public static void BuildFromCommandLine()
        {
            BoothSceneBuilder.BuildFromCommandLine(Recipe);
        }

        [InitializeOnLoadMethod]
        static void EnsureBoothPrefabOnLoad()
        {
            BoothSceneBuilder.EnsurePrefab(Recipe);
        }

        public static void Build()
        {
            BoothSceneBuilder.Build(Recipe);
        }
    }
}
