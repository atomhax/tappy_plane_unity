using System.Collections.Generic;
using SpilGames.Unity.Base.SDK;
using SpilGames.Unity.Helpers.Promotions;

namespace SpilGames.Unity.Helpers.AssetBundles {
    public class AssetBundlesHelper {
        public List<AssetBundle> AssetBundles;

        public AssetBundlesHelper(List<SpilAssetBundle> assetBundlesData) {
            AssetBundles = new List<AssetBundle>();

            if (assetBundlesData != null) {
                foreach (SpilAssetBundle assetBundle in assetBundlesData) {
                    AssetBundles.Add(new AssetBundle(assetBundle.name, assetBundle.type, assetBundle.endDate, assetBundle.url));
                }
            }
        }
        
        public AssetBundle GetAssetBundle(string name) {
            foreach (AssetBundle assetBundle in AssetBundles) {
                if (assetBundle.Name.Equals(name)) {
                    return assetBundle;
                }
            }

            return null;
        }

        public List<AssetBundle> GetAssetBundlesOfType(string type) {
            List<AssetBundle> assetBundlesOfType = new List<AssetBundle>();
            
            foreach (AssetBundle assetBundle in AssetBundles) {
                if (assetBundle.Type.Equals(type)) {
                    assetBundlesOfType.Add(assetBundle);
                }
            }

            return assetBundlesOfType;
        }
        
        public bool HasAssetBundle(string name) {
            foreach (AssetBundle assetBundle in AssetBundles) {
                if (assetBundle.Name.Equals(name) && assetBundle.IsValid()) {
                    return true;
                }
            }

            return false;
        }
    }
}