using System.IO;
using Fairground.Core.Attractions;
using Fairground.View.Attractions.BalloonThrow;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fairground.Editor.Attractions.BalloonThrow
{
    /// <summary>
    /// Editor Builder entry: creates installer-driven BalloonThrow scene.
    /// </summary>
    public static class BalloonThrowSceneBuilder
    {
        const string XrOriginPrefabPath =
            "Assets/Samples/XR Interaction Toolkit/3.6.1/Starter Assets/Prefabs/XR Origin (XR Rig).prefab";

        [MenuItem("Fairground/Attractions/Build Balloon Throw Scene")]
        public static void BuildFromMenu()
        {
            Build();
            EditorUtility.DisplayDialog("Balloon Throw", "Scene rebuilt with runtime installer.", "OK");
        }

        public static void BuildFromCommandLine()
        {
            Build();
            EditorApplication.Exit(0);
        }

        public static void Build()
        {
            EnsureFolders();
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            CreateInstaller(LoadXrOriginPrefab());
            SaveAndRegister(scene);
        }

        static void EnsureFolders()
        {
            Directory.CreateDirectory(ToAbsolute(AttractionScenes.AttractionsFolder));
            Directory.CreateDirectory(ToAbsolute("Assets/_Project/Prefabs/Attractions/BalloonThrow"));
            Directory.CreateDirectory(ToAbsolute("Assets/_Project/Art/Materials/Attractions/BalloonThrow"));
            AssetDatabase.Refresh();
        }

        static GameObject LoadXrOriginPrefab()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(XrOriginPrefabPath);
            if (prefab == null)
                throw new FileNotFoundException($"Missing XR Origin prefab at {XrOriginPrefabPath}");

            return prefab;
        }

        static void CreateInstaller(GameObject xrOriginPrefab)
        {
            var go = new GameObject("BalloonThrowInstaller");
            var installer = go.AddComponent<BalloonThrowSceneInstaller>();
            ApplyInstallerDefaults(installer, xrOriginPrefab);
        }

        static void ApplyInstallerDefaults(BalloonThrowSceneInstaller installer, GameObject xrOriginPrefab)
        {
            var so = new SerializedObject(installer);
            so.FindProperty("xrOriginPrefab").objectReferenceValue = xrOriginPrefab;
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
            Debug.Log($"[Fairground] Balloon Throw scene saved: {path}");
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
