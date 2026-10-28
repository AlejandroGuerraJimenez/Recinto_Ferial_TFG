using System;
using System.IO;
using Fairground.Core.Attractions;
using Fairground.View.Attractions;
using Fairground.View.Attractions.Rendering;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fairground.Editor.Attractions
{
    /// <summary>
    /// What one booth scene adds on top of the shared editor bake: layout, materials, and match defaults.
    /// </summary>
    public sealed class BoothSceneRecipe
    {
        public AttractionId Attraction;
        public string Title;
        public Func<BoothLayout> Layout;
        public (string ChildPath, string MaterialFile)[] BakedMaterials = Array.Empty<(string, string)>();
        public Type InstallerType;
        public Action<SerializedObject> WriteMatchDefaults;

        public string PrefabFolder => "Assets/_Project/Prefabs/Attractions/" + Attraction;

        public string PrefabPath => PrefabFolder + "/" + Attraction + "BoothEnvironment.prefab";

        public string MaterialsFolder => "Assets/_Project/Art/Materials/Attractions/" + Attraction;
    }

    /// <summary>
    /// Bakes a booth prefab into its scene and wires the installer. Each attraction only fills a <see cref="BoothSceneRecipe"/>.
    /// </summary>
    public static class BoothSceneBuilder
    {
        const string XrOriginPrefabPath =
            "Assets/Samples/XR Interaction Toolkit/3.6.1/Starter Assets/Prefabs/XR Origin (XR Rig).prefab";

        const string PrimitiveLitPath = "Assets/_Project/Resources/AttractionPrimitiveLit.mat";

        public static void BuildFromMenu(BoothSceneRecipe recipe)
        {
            Build(recipe);
            EditorUtility.DisplayDialog(
                recipe.Title,
                "Scene rebuilt with baked booth environment prefab.",
                "OK");
        }

        public static void BuildFromCommandLine(BoothSceneRecipe recipe)
        {
            Build(recipe);
            EditorApplication.Exit(0);
        }

        /// <summary>
        /// Ensures the booth prefab exists after script reload. Full scene rebuild stays a menu or batch action.
        /// </summary>
        public static void EnsurePrefab(BoothSceneRecipe recipe)
        {
            EditorApplication.delayCall += () =>
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode)
                    return;

                if (AssetDatabase.LoadAssetAtPath<GameObject>(recipe.PrefabPath) != null)
                    return;

                try
                {
                    EnsureFolders(recipe);
                    ApplyPrimitiveTemplate();
                    BakeBoothEnvironmentPrefab(recipe);
                    Debug.Log($"[Fairground] Auto-baked missing booth prefab: {recipe.PrefabPath}");
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[Fairground] Booth prefab auto-bake skipped: {ex.Message}");
                }
            };
        }

        public static void Build(BoothSceneRecipe recipe)
        {
            if (recipe == null)
                throw new ArgumentNullException(nameof(recipe));

            EnsureFolders(recipe);
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            ApplyPrimitiveTemplate();
            GameObject boothPrefab = BakeBoothEnvironmentPrefab(recipe);
            Transform bakedRoot = PlaceBoothInScene(boothPrefab);
            CreateInstaller(recipe, LoadXrOriginPrefab(), boothPrefab, bakedRoot);
            SaveAndRegister(recipe, scene);
        }

        static void EnsureFolders(BoothSceneRecipe recipe)
        {
            Directory.CreateDirectory(ToAbsolute(AttractionScenes.AttractionsFolder));
            Directory.CreateDirectory(ToAbsolute(recipe.PrefabFolder));
            Directory.CreateDirectory(ToAbsolute(recipe.MaterialsFolder));
            AssetDatabase.Refresh();
        }

        static void ApplyPrimitiveTemplate()
        {
            var template = AssetDatabase.LoadAssetAtPath<Material>(PrimitiveLitPath);
            if (template != null)
                PrimitiveMaterialApplier.SetTemplate(template);
        }

        static GameObject BakeBoothEnvironmentPrefab(BoothSceneRecipe recipe)
        {
            var factory = new BoothPropFactory(recipe.Layout());
            var environment = factory.CreateEnvironment();
            ApplyBakedMaterials(recipe, environment);

            PrefabUtility.SaveAsPrefabAsset(environment, recipe.PrefabPath);
            UnityEngine.Object.DestroyImmediate(environment);

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(recipe.PrefabPath);
            if (prefab == null)
                throw new IOException($"Failed to save booth prefab at {recipe.PrefabPath}");

            AssetDatabase.SaveAssets();
            return prefab;
        }

        static void ApplyBakedMaterials(BoothSceneRecipe recipe, GameObject environment)
        {
            var materials = recipe.BakedMaterials ?? Array.Empty<(string, string)>();
            for (int i = 0; i < materials.Length; i++)
            {
                var (childPath, materialFile) = materials[i];
                AssignSharedMaterial(
                    environment.transform.Find(childPath),
                    recipe.MaterialsFolder + "/" + materialFile);
            }
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

        static void CreateInstaller(
            BoothSceneRecipe recipe,
            GameObject xrOriginPrefab,
            GameObject boothPrefab,
            Transform bakedRoot)
        {
            var go = new GameObject(recipe.Attraction + "Installer");
            var installer = go.AddComponent(recipe.InstallerType);
            var so = new SerializedObject(installer);
            so.FindProperty("xrOriginPrefab").objectReferenceValue = xrOriginPrefab;
            so.FindProperty("boothEnvironmentPrefab").objectReferenceValue = boothPrefab;
            so.FindProperty("bakedBoothEnvironment").objectReferenceValue = bakedRoot;
            so.FindProperty("buildOnAwake").boolValue = true;
            recipe.WriteMatchDefaults?.Invoke(so);
            so.ApplyModifiedPropertiesWithoutUndo();

            var lightFactory = new BoothPropFactory(recipe.Layout());
            lightFactory.EnsureLighting();
        }

        static void SaveAndRegister(BoothSceneRecipe recipe, Scene scene)
        {
            string path = AttractionScenes.GetScenePath(recipe.Attraction);
            EditorSceneManager.SaveScene(scene, path);
            AddSceneToBuildSettings(path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[Fairground] {recipe.Title} scene saved with baked booth: {path}");
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
