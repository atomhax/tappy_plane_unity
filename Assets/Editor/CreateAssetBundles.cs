using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor {
    public class CreateAssetBundles {
        [MenuItem("Assets/Build AssetBundles")]
        static void BuildAllAssetBundles() {
            // Cleanup iOS, Android & WebGL
            string androidAssetBundleDirectory = "Assets/AssetBundles/Android";
            if (Directory.Exists(androidAssetBundleDirectory)) {
                Directory.Delete(androidAssetBundleDirectory, true);
            }
            string iosAssetBundleDirectory = "Assets/AssetBundles/iOS";
            if (Directory.Exists(iosAssetBundleDirectory)) {
                Directory.Delete(iosAssetBundleDirectory, true);
            }
            string webAssetBundleDirectory = "Assets/AssetBundles/WebGL";
            if (Directory.Exists(webAssetBundleDirectory)) {
                Directory.Delete(webAssetBundleDirectory, true);
            }
            
            // Build iOS
            Debug.Log("Starting iOS asset bundle building.");
            if (!Directory.Exists(iosAssetBundleDirectory)) {
                Directory.CreateDirectory(iosAssetBundleDirectory);
            }
            BuildPipeline.BuildAssetBundles(iosAssetBundleDirectory, BuildAssetBundleOptions.ForceRebuildAssetBundle, BuildTarget.iOS);
            Debug.Log("Finished iOS asset bundle building.");
            
            // Build Android
            Debug.Log("Starting Android asset bundle building.");
            if (!Directory.Exists(androidAssetBundleDirectory)) {
                Directory.CreateDirectory(androidAssetBundleDirectory);
            }
            BuildPipeline.BuildAssetBundles(androidAssetBundleDirectory, BuildAssetBundleOptions.ForceRebuildAssetBundle, BuildTarget.Android);
            Debug.Log("Finished Android asset bundle building.");
            
            // Build WebGL
            Debug.Log("Starting WebGL asset bundle building.");
            if (!Directory.Exists(webAssetBundleDirectory)) {
                Directory.CreateDirectory(webAssetBundleDirectory);
            }
            BuildPipeline.BuildAssetBundles(webAssetBundleDirectory, BuildAssetBundleOptions.ForceRebuildAssetBundle, BuildTarget.WebGL);
            Debug.Log("Finished WebGL asset bundle building.");
        }
    }
}