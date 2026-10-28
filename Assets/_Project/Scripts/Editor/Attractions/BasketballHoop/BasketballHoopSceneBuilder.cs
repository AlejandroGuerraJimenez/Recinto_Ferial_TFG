using Fairground.Core.Attractions;
using Fairground.View.Attractions.BasketballHoop;
using UnityEditor;

namespace Fairground.Editor.Attractions.BasketballHoop
{
    /// <summary>
    /// Menu and match defaults for the basketball booth. The shared builder bakes the scene.
    /// </summary>
    public static class BasketballHoopSceneBuilder
    {
        static readonly BoothSceneRecipe Recipe = new BoothSceneRecipe
        {
            Attraction = AttractionId.BasketballHoop,
            Title = "Basketball Hoop",
            Layout = BasketballHoopBoothLayout.Props,
            BakedMaterials = new (string, string)[]
            {
                ("Ground", "BoothGround.mat"),
                ("Playfield", "BoothPlayfield.mat"),
                ("LeftRail", "BoothRail.mat"),
                ("RightRail", "BoothRail.mat"),
                ("BackStop", "BoothBackStop.mat"),
                ("Marquee", "BoothMarquee.mat"),
            },
            InstallerType = typeof(BasketballHoopSceneInstaller),
            WriteMatchDefaults = so =>
            {
                so.FindProperty("pointsPerBasket").intValue = 2;
                so.FindProperty("startingBalls").intValue = 10;
                so.FindProperty("winScoreThreshold").intValue = 5;
            },
        };

        [MenuItem("Fairground/Attractions/Build Basketball Hoop Scene")]
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
