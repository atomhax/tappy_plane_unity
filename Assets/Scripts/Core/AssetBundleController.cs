using System.Collections;
using System.IO;
using SpilGames.Unity.Json;
using UnityEngine;
using UnityEngine.Networking;

public class AssetBundleController : MonoBehaviour{
    public static IEnumerator DownloadGoldenPlaneBundle(GameController gameController, SpilGames.Unity.Helpers.AssetBundles.AssetBundle assetBundleConfig) {
        UnityWebRequest request = UnityWebRequest.GetAssetBundle(assetBundleConfig.Url, assetBundleConfig.Hash, 0);
        yield return request.SendWebRequest();

        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);

        if (bundle != null) {  
            bundle.LoadAllAssets<Sprite>();
            RuntimeAnimatorController goldPlaneController = bundle.LoadAsset<RuntimeAnimatorController>("PlayerGold");
            gameController.goldPlaneController = goldPlaneController;
            gameController.player.playerSkinAnimators[3] = goldPlaneController;
            
            gameController.UpdateSkins();
        }

    }

    public static IEnumerator DownloadBackgroundBundle(GameController gameController, SpilGames.Unity.Helpers.AssetBundles.AssetBundle assetBundleConfig) {
        UnityWebRequest request;
        if (assetBundleConfig.Version > 0) {
            request = UnityWebRequest.GetAssetBundle(assetBundleConfig.Url, assetBundleConfig.Version, 0);
        } else {
            request = UnityWebRequest.GetAssetBundle(assetBundleConfig.Url);
        }
        
        yield return request.SendWebRequest();

        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);

        if (bundle != null) {
            if (assetBundleConfig.Name.Contains("ruin")) {
                Sprite sprite = bundle.LoadAsset<Sprite>("colored_ruins");
                gameController.backgroundRuin = sprite;
                gameController.backgroundSprites[3] = sprite;
            } else if (assetBundleConfig.Name.Contains("town")) {
                Sprite sprite = bundle.LoadAsset<Sprite>("colored_town");
                gameController.backgroundTown = sprite;
                gameController.backgroundSprites[4] = sprite;
            }
            
            gameController.UpdateSkins();
        }
    }
}