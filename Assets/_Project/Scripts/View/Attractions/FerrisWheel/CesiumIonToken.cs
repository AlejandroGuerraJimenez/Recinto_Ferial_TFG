using System;
using System.IO;
using UnityEngine;

namespace Fairground.View.Attractions.FerrisWheel
{
    /// <summary>
    /// Reads a Cesium ion token from the environment or a gitignored project file.
    /// The token itself is never written to the repository.
    /// </summary>
    public static class CesiumIonToken
    {
        public const string EnvironmentVariable = "CESIUM_ION_TOKEN";
        public const string ProjectAssetPath = "Assets/_Project/Settings/cesium-ion-token.txt";

        public static bool TryRead(out string token)
        {
            token = ReadEnvironment();
            if (!string.IsNullOrEmpty(token))
                return true;

            // The project file is on disk in the editor. A Quest build cannot read it from the APK.
            token = ReadProjectFile();
            if (!string.IsNullOrEmpty(token))
                return true;

            token = ReadResources();
            return !string.IsNullOrEmpty(token);
        }

        static string ReadEnvironment()
        {
            string value = Environment.GetEnvironmentVariable(EnvironmentVariable);
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        static string ReadProjectFile()
        {
            string path = Path.Combine(Application.dataPath, "_Project", "Settings", "cesium-ion-token.txt");
            if (!File.Exists(path))
                return null;

            try
            {
                string value = File.ReadAllText(path).Trim();
                return string.IsNullOrEmpty(value) ? null : value;
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                Debug.LogError("[FerrisWheel] Could not read " + ProjectAssetPath + ": " + exception.Message);
                return null;
            }
        }

        /// <summary>
        /// TextAsset baked for the player. The Quest APK cannot see the gitignored file on disk.
        /// </summary>
        static string ReadResources()
        {
            TextAsset asset = Resources.Load<TextAsset>("cesium-ion-token");
            if (asset == null)
                return null;

            string value = asset.text != null ? asset.text.Trim() : null;
            return string.IsNullOrEmpty(value) ? null : value;
        }
    }
}
