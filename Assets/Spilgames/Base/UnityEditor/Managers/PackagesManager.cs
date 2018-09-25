#if UNITY_EDITOR || UNITY_WEBGL
using UnityEngine;
using System.Collections;
using SpilGames.Unity.Helpers;
using System.Collections.Generic;
using SpilGames.Unity.Base.Implementations;
using SpilGames.Unity.Base.SDK;
using SpilGames.Unity.Json;

namespace SpilGames.Unity.Base.UnityEditor.Managers {
    public class PackagesManager {
        public static List<PackageData> GamePackagesData;

        public static string getPackage(string packageId) {
            for (int i = 0; i < GamePackagesData.Count; i++) {
                if (GamePackagesData[i].packageId.Equals(packageId)) {
                    return JsonHelper.getJSONFromObject(GamePackagesData[i]);
                }
            }

            return null;
        }
    }
    
    public class PackagesResponse : Response {
        public static void ProcessPackagesResponse(ResponseEvent response) {
            if (response.data == null) {
				Spil.Instance.firePackagesNotAvailable();
                return;
            }
            
            if (response.data.HasField("packages")) {
                JSONObject json = new JSONObject();
                json.AddField("data", response.data.GetField("packages").Print(false));
                PackagesManager.GamePackagesData = JsonHelper.getObjectFromJson<List<PackageData>>(json.GetField("data").str);
                
                
            }
        }
    }
}
#endif