using Fairground.Core.Attractions;
using Fairground.View.Attractions.DuckFishing;
using UnityEditor;

namespace Fairground.Editor.Attractions.DuckFishing
{
    /// <summary>
    /// Menu for the duck booth. The shared builder bakes the scene. Balance stays in code.
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
