using System.Collections.Generic;
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
            BakedMaterials = MaterialBindings(),
        };

        static (string, string)[] MaterialBindings()
        {
            var bindings = new List<(string, string)>
            {
                ("Ground", "BoothGround.mat"),
                ("BoothBack", "BoothBack.mat"),
                ("Sign", "BoothSign.mat"),
                ("PostLeft", "BoothBack.mat"),
                ("PostRight", "BoothBack.mat"),
                ("Awning", "BoothBack.mat"),
                ("Pond/Basin", "BoothBasin.mat"),
                ("Pond/PoolBed", "BoothPoolBed.mat"),
                ("Pond/Water", "BoothWater.mat"),
            };

            for (int i = 0; i < DuckFishingBoothLayout.RimCount; i++)
            {
                string material = i % 2 == 0 ? "BoothRimRed.mat" : "BoothRimCream.mat";
                bindings.Add(("Pond/Rim_" + i, material));
            }

            return bindings.ToArray();
        }

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
