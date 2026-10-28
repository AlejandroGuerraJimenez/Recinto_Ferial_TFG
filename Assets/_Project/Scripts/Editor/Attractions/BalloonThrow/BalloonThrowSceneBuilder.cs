using System.IO;
using Fairground.Core.Attractions;
using Fairground.View.Attractions.BalloonThrow;
using Fairground.View.Attractions;
using Fairground.View.Attractions.Rendering;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fairground.Editor.Attractions.BalloonThrow
{
    /// <summary>
    /// Editor Builder: bakes static booth prefab into the BalloonThrow scene.
    /// Runtime installer only wires XR + gameplay (no CreatePrimitive for ground/wall).
    /// </summary>
    public static class BalloonThrowSceneBuilder
    {
        const string XrOriginPrefabPath =
            "Assets/Samples/XR Interaction Toolkit/3.6.1/Starter Assets/Prefabs/XR Origin (XR Rig).prefab";

        const string BoothPrefabFolder = "Assets/_Project/Prefabs/Attractions/BalloonThrow";
        const string BoothPrefabPath = BoothPrefabFolder + "/BalloonThrowBoothEnvironment.prefab";
        const string PrimitiveLitPath = "Assets/_Project/Resources/AttractionPrimitiveLit.mat";
        const string GroundMaterialPath = "Assets/_Project/Art/Materials/Attractions/BalloonThrow/BoothGround.mat";
        const string BackMaterialPath = "Assets/_Project/Art/Materials/Attractions/BalloonThrow/BoothBack.mat";

        [MenuItem("Fairground/Attractions/Build Balloon Throw Scene")]
        public static void BuildFromMenu()
        {
            Build();
            EditorUtility.DisplayDialog(
                "Balloon Throw",
                "Scene rebuilt with baked booth environment prefab.",
                "OK");
        }

        public static void BuildFromCommandLine()
        {
            Build();
            EditorApplication.Exit(0);
        }

        /// <summary>
        /// Ensures the booth prefab exists after script reload (Editor open with project lock).
        /// Full scene rebuild remains a menu / batchmode action.
        /// </summary>
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
            Directory.CreateDirectory(ToAbsolute("Assets/_Project/Art/Materials/Attractions/BalloonThrow"));
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
            var factory = new BoothPropFactory(BalloonThrowBoothLayout.Props());
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
            AssignSharedMaterial(environment.transform.Find("Ground"), GroundMaterialPath);
            AssignSharedMaterial(environment.transform.Find("BoothBack"), BackMaterialPath);
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
            var go = new GameObject("BalloonThrowInstaller");
            var installer = go.AddComponent<BalloonThrowSceneInstaller>();
            ApplyInstallerDefaults(installer, xrOriginPrefab, boothPrefab, bakedRoot);

            var lightFactory = new BoothPropFactory(BalloonThrowBoothLayout.Props());
            lightFactory.EnsureLighting();
        }

        static void ApplyInstallerDefaults(
            BalloonThrowSceneInstaller installer,
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
            so.FindProperty("balloonCount").intValue = 6;
            so.FindProperty("startingThrows").intValue = 8;
            so.FindProperty("buildOnAwake").boolValue = true;
        }

        static void SaveAndRegister(Scene scene)
        {
            string path = AttractionScenes.GetScenePath(AttractionId.BalloonThrow);
            EditorSceneManager.SaveScene(scene, path);
            AddSceneToBuildSettings(path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[Fairground] Balloon Throw scene saved with baked booth: {path}");
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
