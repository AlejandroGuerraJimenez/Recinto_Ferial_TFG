using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Fairground.Editor.Attractions.FerrisWheel
{
    /// <summary>
    /// Copies the gitignored ion token into a Resources TextAsset so a Quest build can read it.
    /// The copy stays gitignored.
    /// </summary>
    public sealed class CesiumIonTokenBuildCopy : IPreprocessBuildWithReport
    {
        public const string SourceAssetPath = "Assets/_Project/Settings/cesium-ion-token.txt";
        public const string ResourcesAssetPath = "Assets/_Project/Settings/Resources/cesium-ion-token.txt";

        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            if (!Copy())
            {
                throw new BuildFailedException(
                    "Quest and other players cannot read " + SourceAssetPath +
                    ". Add the Cesium ion token there before building.");
            }
        }

        public static bool Copy()
        {
            string source = Path.GetFullPath(SourceAssetPath);
            if (!File.Exists(source))
            {
                Debug.LogError("[FerrisWheel] Missing " + SourceAssetPath + ". London will not load on the headset.");
                return false;
            }

            string destination = Path.GetFullPath(ResourcesAssetPath);
            Directory.CreateDirectory(Path.GetDirectoryName(destination));
            File.Copy(source, destination, true);
            AssetDatabase.ImportAsset(ResourcesAssetPath);
            return true;
        }
    }
}
