using System.IO;
using Fairground.Core.Attractions;
using Fairground.View.Attractions.BasketballHoop;
using Fairground.View.Attractions.BasketballHoop.Factories;
using Fairground.View.Attractions.Rendering;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fairground.Editor.Attractions.BasketballHoop
{
    /// <summary>
    /// Editor Builder: bakes static booth prefab into the BasketballHoop scene.
    /// </summary>
    public static class BasketballHoopSceneBuilder
    {
        const string XrOriginPrefabPath =
            "Assets/Samples/XR Interaction Toolkit/3.6.1/Starter Assets/Prefabs/XR Origin (XR Rig).prefab";

        const string BoothPrefabFolder = "Assets/_Project/Prefabs/Attractions/BasketballHoop";
        const string BoothPrefabPath = BoothPrefabFolder + "/BasketballHoopBoothEnvironment.prefab";
        const string MaterialsFolder = "Assets/_Project/Art/Materials/Attractions/BasketballHoop";
        const string PrimitiveLitPath = "Assets/_Project/Resources/AttractionPrimitiveLit.mat";

        static readonly (string Child, string MaterialFile)[] BakedMaterials =
        {
            ("Ground", "BoothGround.mat"),
            ("Playfield", "BoothPlayfield.mat"),
            ("LeftRail", "BoothRail.mat"),
            ("RightRail", "BoothRail.mat"),
            ("BackStop", "BoothBackStop.mat"),
            ("Marquee", "BoothMarquee.mat"),
        };

        [MenuItem("Fairground/Attractions/Build Basketball Hoop Scene")]
        public static void BuildFromMenu()
        {
            Build();
            EditorUtility.DisplayDialog(
                "Basketball Hoop",
                "Scene rebuilt with baked booth environment prefab.",
                "OK");
        }

        public static void BuildFromCommandLine()
        {
            Build();
            EditorApplication.Exit(0);
        }

        [InitializeOnLoadMethod]
        static void EnsureBoothPrefabOnLoad()
        {
            EditorApplication.delayCall += () =>
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode)
                    return;

                if (AssetDatabase.LoadAssetAtPath<GameObject>(BoothPrefabPath) != null)
                    return;

                try
                {
                    EnsureFolders();
                    ApplyPrimitiveTemplate();
                    BakeBoothEnvironmentPrefab();
                    Debug.Log($"[Fairground] Auto-baked missing booth prefab: {BoothPrefabPath}");
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"[Fairground] Booth prefab auto-bake skipped: {ex.Message}");
                }
            };
        }

        public static void Build()
        {
            EnsureFolders();
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            ApplyPrimitiveTemplate();
            GameObject boothPrefab = BakeBoothEnvironmentPrefab();
            Transform bakedRoot = PlaceBoothInScene(boothPrefab);
            CreateInstaller(LoadXrOriginPrefab(), boothPrefab, bakedRoot);
            SaveAndRegister(scene);
        }

        static void EnsureFolders()
        {
            Directory.CreateDirectory(ToAbsolute(AttractionScenes.AttractionsFolder));
            Directory.CreateDirectory(ToAbsolute(BoothPrefabFolder));
            Directory.CreateDirectory(ToAbsolute(MaterialsFolder));
            AssetDatabase.Refresh();
        }

        static void ApplyPrimitiveTemplate()
        {
            var template = AssetDatabase.LoadAssetAtPath<Material>(PrimitiveLitPath);
            if (template != null)
                PrimitiveMaterialApplier.SetTemplate(template);
        }

        static GameObject BakeBoothEnvironmentPrefab()
        {
            var factory = new BoothPropFactory();
            var environment = factory.CreateEnvironment();
            ApplyBakedMaterials(environment);

            PrefabUtility.SaveAsPrefabAsset(environment, BoothPrefabPath);
            Object.DestroyImmediate(environment);

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(BoothPrefabPath);
            if (prefab == null)
                throw new IOException($"Failed to save booth prefab at {BoothPrefabPath}");

            AssetDatabase.SaveAssets();
            return prefab;
        }

        static void ApplyBakedMaterials(GameObject environment)
        {
            foreach (var (child, materialFile) in BakedMaterials)
                AssignSharedMaterial(environment.transform.Find(child), $"{MaterialsFolder}/{materialFile}");
        }

        static void AssignSharedMaterial(Transform target, string materialPath)
        {
            if (target == null)
                return;

            var renderer = target.GetComponent<Renderer>();
            var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            if (renderer != null && material != null)
                renderer.sharedMaterial = material;
        }

        static Transform PlaceBoothInScene(GameObject boothPrefab)
        {
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(boothPrefab);
            instance.name = BoothPropFactory.EnvironmentRootName;
            return instance.transform;
        }

        static GameObject LoadXrOriginPrefab()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(XrOriginPrefabPath);
            if (prefab == null)
                throw new FileNotFoundException($"Missing XR Origin prefab at {XrOriginPrefabPath}");

            return prefab;
        }

        static void CreateInstaller(GameObject xrOriginPrefab, GameObject boothPrefab, Transform bakedRoot)
        {
            var go = new GameObject("BasketballHoopInstaller");
            var installer = go.AddComponent<BasketballHoopSceneInstaller>();
            ApplyInstallerDefaults(installer, xrOriginPrefab, boothPrefab, bakedRoot);

            var lightFactory = new BoothPropFactory();
            lightFactory.EnsureLighting();
        }

        static void ApplyInstallerDefaults(
            BasketballHoopSceneInstaller installer,
            GameObject xrOriginPrefab,
            GameObject boothPrefab,
            Transform bakedRoot)
        {
            var so = new SerializedObject(installer);
            so.FindProperty("xrOriginPrefab").objectReferenceValue = xrOriginPrefab;
            so.FindProperty("boothEnvironmentPrefab").objectReferenceValue = boothPrefab;
            so.FindProperty("bakedBoothEnvironment").objectReferenceValue = bakedRoot;
            WriteMatchDefaults(so);
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        static void WriteMatchDefaults(SerializedObject so)
        {
            so.FindProperty("pointsPerBasket").intValue = 2;
            so.FindProperty("startingBalls").intValue = 10;
            so.FindProperty("winScoreThreshold").intValue = 5;
            so.FindProperty("buildOnAwake").boolValue = true;
        }

        static void SaveAndRegister(Scene scene)
        {
            string path = AttractionScenes.GetScenePath(AttractionId.BasketballHoop);
            EditorSceneManager.SaveScene(scene, path);
            AddSceneToBuildSettings(path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[Fairground] Basketball Hoop scene saved with baked booth: {path}");
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
