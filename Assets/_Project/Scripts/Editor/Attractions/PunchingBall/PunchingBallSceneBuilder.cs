using Fairground.Core.Attractions;
using Fairground.View.Attractions.PunchingBall;
using UnityEditor;

namespace Fairground.Editor.Attractions.PunchingBall
{
    /// <summary>
    /// Menu and match defaults for the punch booth. The shared builder bakes the scene.
    /// </summary>
    public static class PunchingBallSceneBuilder
    {
        static readonly BoothSceneRecipe Recipe = new BoothSceneRecipe
        {
            Attraction = AttractionId.PunchingBall,
            Title = "Punching Ball",
            Layout = PunchingBallBoothLayout.Props,
            BakedMaterials = new (string, string)[]
            {
                ("Ground", "BoothGround.mat"),
                ("BoothBack", "BoothBack.mat"),
            },
            InstallerType = typeof(PunchingBallSceneInstaller),
            WriteMatchDefaults = so =>
            {
                so.FindProperty("startingPunches").intValue = 5;
                so.FindProperty("winScoreThreshold").intValue = 700;
                so.FindProperty("maxScore").intValue = 999;
                so.FindProperty("maxImpactSpeed").floatValue = 8f;
            },
        };

        [MenuItem("Fairground/Attractions/Build Punching Ball Scene")]
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
