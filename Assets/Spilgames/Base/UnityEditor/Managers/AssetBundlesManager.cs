﻿#if UNITY_EDITOR || UNITY_WEBGL
using System.Collections.Generic;
using SpilGames.Unity.Base.Implementations;
using SpilGames.Unity.Base.SDK;
using SpilGames.Unity.Helpers.IAPPackages;
using SpilGames.Unity.Json;

namespace SpilGames.Unity.Base.UnityEditor.Managers {
    public class AssetBundlesManager {
        public static List<SpilAssetBundle> AssetBundlesData = new List<SpilAssetBundle>();
        
        public static string GetAssetBundles() {
            return JsonHelper.getJSONFromObject(AssetBundlesData);
        }
    }
    
    public class AssetBundlesResponse : Response {
        public static void ProcessAssetBundlesResponse(ResponseEvent response) {
            if (response.data == null) {
				Spil.Instance.fireAssetBundlesNotAvailable();
                return;
            }

            if (response.action.Equals("request")) {
                if (response.data.HasField("assetBundles")) {
                    JSONObject assetBundlesJSON = response.data.GetField("assetBundles");

                    if (assetBundlesJSON.list.Count == 0) {
						Spil.Instance.fireAssetBundlesNotAvailable();
                        return;
                    }
                    
                    for (int i = 0; i < assetBundlesJSON.Count; i++) {
                        JSONObject assetBundle = assetBundlesJSON.list[i];
                        SpilAssetBundle assetBundleData = new SpilAssetBundle();

                        if (assetBundle.HasField("name")) {
                            assetBundleData.name = assetBundle.GetField("name").str;
                        }

                        if (assetBundle.HasField("type")) {
                            assetBundleData.type = assetBundle.GetField("type").str;
                        }

                        if (assetBundle.HasField("endDate")) {
                            assetBundleData.endDate = assetBundle.GetField("endDate").i;
                        }

                        if (assetBundle.HasField("url")) {
                            assetBundleData.url = assetBundle.GetField("url").str;
                        }

                        if (assetBundle.HasField("hash")) {
                            assetBundleData.hash = assetBundle.GetField("hash").str;
                        }

                        if (assetBundle.HasField("version")) {
                            assetBundleData.version = (int) assetBundle.GetField("version").i;
                        }
                        
                        AssetBundlesManager.AssetBundlesData.Add(assetBundleData);
                    }
					Spil.Instance.fireAssetBundlesAvailable();
                }
            }
        }
    }
}

#endif