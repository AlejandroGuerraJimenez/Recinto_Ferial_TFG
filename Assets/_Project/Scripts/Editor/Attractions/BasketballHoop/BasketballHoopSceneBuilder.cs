using Fairground.Core.Attractions;
using Fairground.View.Attractions.BasketballHoop;
using UnityEditor;

namespace Fairground.Editor.Attractions.BasketballHoop
{
    /// <summary>
    /// Menu for the basketball booth. The shared builder bakes the scene. Balance stays in code.
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
                ("FrontPostLeft", "BoothRail.mat"),
                ("FrontPostRight", "BoothRail.mat"),
                ("FrontBar", "BoothRail.mat"),
                ("BackPostLeft", "BoothRail.mat"),
                ("BackPostRight", "BoothRail.mat"),
                ("LaneStripe", "BoothMarquee.mat"),
                ("Bulb_0", "BoothMarquee.mat"),
                ("Bulb_1", "BoothMarquee.mat"),
                ("Bulb_2", "BoothMarquee.mat"),
                ("Bulb_3", "BoothMarquee.mat"),
                ("Bulb_4", "BoothMarquee.mat"),
                ("Bulb_5", "BoothMarquee.mat"),
                ("Bulb_6", "BoothMarquee.mat"),
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
