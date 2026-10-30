using Fairground.Core.Attractions;
using Fairground.View.Attractions.BalloonThrow;
using UnityEditor;

namespace Fairground.Editor.Attractions.BalloonThrow
{
    /// <summary>
    /// Menu for the balloon booth. The shared builder bakes the scene. Balance stays in code.
    /// </summary>
    public static class BalloonThrowSceneBuilder
    {
        static readonly BoothSceneRecipe Recipe = new BoothSceneRecipe
        {
            Attraction = AttractionId.BalloonThrow,
            Title = "Balloon Throw",
            Layout = BalloonThrowBoothLayout.Props,
            BakedMaterials = BalloonThrowBoothLayout.MaterialBindings(),
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
