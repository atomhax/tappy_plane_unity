using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace SpilGames.Unity.Helpers.DailyBonus {
    public class DailyBonusHelper : MonoBehaviour{
        public static DailyBonus DailyBonus;
        
        public static GameObject SpilDailyBonus;
        public static GameObject TempSpilDailyBonus;

        public IEnumerator DownloadDailyBonusAssets() {
            if (!Caching.IsVersionCached(DailyBonus.Url, Hash128.Parse(DailyBonus.Url))){
                SpilLogging.Log("[DB] Asset bundle downloading started: " + DailyBonus.Url);
                UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(DailyBonus.Url, Hash128.Parse(DailyBonus.Url), 0);
                yield return request.SendWebRequest();

                SpilLogging.Log("[DB] Asset bundle downloading succeeded: " + request.error);
                
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
                bundle.LoadAllAssets();
                SpilDailyBonus = bundle.LoadAsset<GameObject>("DailyBonus");
                
                SpilLogging.Log("[DB] SpilDailyBonus found: " + (SpilDailyBonus != null));
            }   
        }

        public void ShowDailyBonus() {
            TempSpilDailyBonus = Instantiate(SpilDailyBonus);
            
            for (int i = 0; i < DailyBonus.Days.Count; i++) {
                GameObject dayEntry = GameObject.Find("DayEntry" + i);
                Image[] dayEntryImages = dayEntry.GetComponentsInChildren<Image>(true);

                if (DailyBonus.Days[i].Status.Equals("collectible")) {
                    dayEntry.GetComponentInChildren<Text>().text = "Today";
                    
                    foreach (Image image in dayEntryImages) {
                        if (image.name.Equals("DayImageCurrent")) {
                            image.gameObject.SetActive(true);
                        }
                    }
                } else if (DailyBonus.Days[i].Status.Equals("inactive")){
                    dayEntry.GetComponentInChildren<Text>().text = "Day " + DailyBonus.Days[i].Day;
                    foreach (Image image in dayEntryImages) {
                        if (image.name.Equals("DayImageNotCollected")) {
                            image.gameObject.SetActive(true);
                        }
                    }
                } else if (DailyBonus.Days[i].Status.Equals("collected")){
                    dayEntry.GetComponentInChildren<Text>().text = "Day " + DailyBonus.Days[i].Day;
                    foreach (Image image in dayEntryImages) {
                        if (image.name.Equals("DayImageCollected")) {
                            image.gameObject.SetActive(true);
                        }
                    }
                }
            }
            
            Spil.Instance.fireDailyBonusOpen();
            SpilDailyBonus.SetActive(true);
        }
        
        public void CloseDailyBonus() {
            Spil.Instance.fireDailyBonusClosed();

            Destroy(TempSpilDailyBonus);
        }

        public void CollectDailyBonusReward() {
            SpilLogging.Log("Collect");
            
            Spil.Instance.CollectDailyBonus();
            
            Destroy(TempSpilDailyBonus);
        }
    }
}