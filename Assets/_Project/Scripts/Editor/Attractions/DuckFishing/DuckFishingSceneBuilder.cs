using Fairground.Model.Attractions.DuckFishing;
using Fairground.Core.Attractions;
using Fairground.View.Attractions.DuckFishing;
using UnityEditor;

namespace Fairground.Editor.Attractions.DuckFishing
{
    /// <summary>
    /// Menu and match defaults for the duck booth. The shared builder bakes the scene.
    /// </summary>
    public static class DuckFishingSceneBuilder
    {
        static readonly BoothSceneRecipe Recipe = new BoothSceneRecipe
        {
            Attraction = AttractionId.DuckFishing,
            Title = "Duck Fishing",
            Layout = DuckFishingBoothLayout.Props,
            BakedMaterials = new (string, string)[]
            {
                ("Ground", "BoothGround.mat"),
                ("BoothBack", "BoothBack.mat"),
                ("Pond/Basin", "BoothBasin.mat"),
                ("Pond/Water", "BoothWater.mat"),
            },
            InstallerType = typeof(DuckFishingSceneInstaller),
            WriteMatchDefaults = so =>
            {
                so.FindProperty("duckCount").intValue = DuckFishingRules.DefaultDuckCount;
                so.FindProperty("ducksToWin").intValue = DuckFishingRules.DefaultDucksToWin;
                so.FindProperty("startingAttempts").intValue = DuckFishingRules.DefaultStartingAttempts;
                so.FindProperty("pointsPerDuck").intValue = DuckFishingRules.DefaultPointsPerDuck;
            },
        };

        [MenuItem("Fairground/Attractions/Build Duck Fishing Scene")]
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
