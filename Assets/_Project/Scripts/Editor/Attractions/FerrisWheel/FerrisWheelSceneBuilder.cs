using System.IO;
using Fairground.Core.Attractions;
using Fairground.View.Attractions.FerrisWheel;
using Fairground.View.Attractions.Rendering;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fairground.Editor.Attractions.FerrisWheel
{
    /// <summary>
    /// Editor builder: empty FerrisWheel scene whose installer starts the London Eye ride at play time.
    /// </summary>
    public static class FerrisWheelSceneBuilder
    {
        const string XrOriginPrefabPath =
            "Assets/Samples/XR Interaction Toolkit/3.6.1/Starter Assets/Prefabs/XR Origin (XR Rig).prefab";

        const string PrimitiveLitPath = "Assets/_Project/Resources/AttractionPrimitiveLit.mat";

        [MenuItem("Fairground/Attractions/Build Ferris Wheel Scene")]
        public static void BuildFromMenu()
        {
            Build();
            EditorUtility.DisplayDialog(
                "Ferris Wheel",
                "London Eye ride is ready. Press Play to spin the capsule. B returns to the fairground.",
                "OK");
        }

        public static void BuildFromCommandLine()
        {
            Build();
            EditorApplication.Exit(0);
        }

        public static void Build()
        {
            Directory.CreateDirectory(ToAbsolute(AttractionScenes.AttractionsFolder));
            AssetDatabase.Refresh();

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            ApplyPrimitiveTemplate();
            CreateInstaller(LoadXrOriginPrefab(), LoadPrimitiveTemplate());
            SaveAndRegister(scene);
        }

        static void ApplyPrimitiveTemplate()
        {
            var template = LoadPrimitiveTemplate();
            if (template != null)
                PrimitiveMaterialApplier.SetTemplate(template);
        }

        static Material LoadPrimitiveTemplate()
        {
            return AssetDatabase.LoadAssetAtPath<Material>(PrimitiveLitPath);
        }

        static GameObject LoadXrOriginPrefab()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(XrOriginPrefabPath);
            if (prefab == null)
                throw new FileNotFoundException($"Missing XR Origin prefab at {XrOriginPrefabPath}");

            return prefab;
        }

        static void CreateInstaller(GameObject xrOriginPrefab, Material primitiveTemplate)
        {
            var go = new GameObject("FerrisWheelInstaller");
            var installer = go.AddComponent<FerrisWheelSceneInstaller>();
            var so = new SerializedObject(installer);
            so.FindProperty("xrOriginPrefab").objectReferenceValue = xrOriginPrefab;
            so.FindProperty("primitiveLitTemplate").objectReferenceValue = primitiveTemplate;
            so.FindProperty("degreesPerSecond").floatValue = FerrisWheelBalance.DegreesPerSecond;
            so.FindProperty("cabinCount").intValue = LondonEyeLayout.CabinCount;
            so.FindProperty("buildOnAwake").boolValue = true;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        static void SaveAndRegister(Scene scene)
        {
            string path = AttractionScenes.GetScenePath(AttractionId.FerrisWheel);
            EditorSceneManager.SaveScene(scene, path);
            AddSceneToBuildSettings(path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[Fairground] Ferris Wheel scene saved: {path}");
        }

        static void AddSceneToBuildSettings(string scenePath)
        {
            if (TryEnableExisting(scenePath))
                return;

            AppendScene(scenePath);
        }

        static bool TryEnableExisting(string scenePath)
        {
            var scenes = EditorBuildSettings.scenes;
            for (int i = 0; i < scenes.Length; i++)
            {
                if (scenes[i].path != scenePath)
                    continue;

                scenes[i].enabled = true;
                EditorBuildSettings.scenes = scenes;
                return true;
            }

            return false;
        }

        static void AppendScene(string scenePath)
        {
            var scenes = EditorBuildSettings.scenes;
            var list = new EditorBuildSettingsScene[scenes.Length + 1];
            scenes.CopyTo(list, 0);
            list[scenes.Length] = new EditorBuildSettingsScene(scenePath, true);
            EditorBuildSettings.scenes = list;
        }

        static string ToAbsolute(string assetsPath)
        {
            return assetsPath.Replace("Assets/", Application.dataPath + "/");
        }
    }
}
