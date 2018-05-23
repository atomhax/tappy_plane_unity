﻿using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor {
    public class CreateAssetBundles {
        [MenuItem("Assets/Build AssetBundles")]
        static void BuildAllAssetBundles() {
            string assetBundleDirectory = "Assets/AssetBundles";
            if (!Directory.Exists(assetBundleDirectory)) {
                Directory.CreateDirectory(assetBundleDirectory);
            }

            BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.ForceRebuildAssetBundle, EditorUserBuildSettings.activeBuildTarget);
        }
    }
}