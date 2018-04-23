using System.Collections.Generic;
using SpilGames.Unity.Helpers.PlayerData;
using SpilGames.Unity.Json;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Linq;
using SpilGames.Unity.Base.Implementations;
using SpilGames.Unity.Base.SDK;
using UnityEngine.UI;

#if UNITY_EDITOR
namespace SpilGames.Unity.Base.UnityEditor.Managers {
    public class TieredEventManager : MonoBehaviour {
        // Event names
        const string EventRequestTieredEvent = "requestTieredEvents";
        const string EventUpdateTierProgress = "updateTierProgress";
        const string EventClaimTierReward = "claimTierReward";
        const string EventShowTierProgress = "showTierProgress";

        // Event params
        const string TieredEventId = "tieredEventId";
        const string TieredEvents = "tieredEvents";
        const string TierEventName = "tierEventName";
        const string EntityId = "entityId";
        const string EntityType = "entityType";
        const string EntityAmount = "entityAmount";
        const string TierName = "tierName";
        const string Tiers = "tiers";
        const string Id = "id";
        const string Rewards = "rewards";
        const string Currency = "CURRENCY";
        const string Item = "ITEM";
        const string Progress = "progress";

        static TieredEventsOverview tieredEventsOverview = new TieredEventsOverview();

        public static void RequestTieredEvents() {
            SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent>();
            spilEvent.eventName = "requestTieredEvents";
            spilEvent.Send();
        }

        public static void ProcessRequestTieredEvents(JSONObject response) {
            if (response != null && response.Count > 0) {
                TieredEventsResponseData tieredEvents = JsonHelper.getObjectFromJson<TieredEventsResponseData>(response.ToString());
                tieredEventsOverview.tieredEvents.Clear();
                tieredEventsOverview.progress.Clear();
                foreach (TieredEvent tieredEvent in tieredEvents.tieredEvents) {
                    tieredEventsOverview.tieredEvents.Add(tieredEvent.id, tieredEvent);
                }
                foreach (TieredEventProgress tieredProgress in tieredEvents.progress) {
                    tieredEventsOverview.progress.Add(tieredProgress.tieredEventId, tieredProgress);
                }
                SpilUnityImplementationBase.fireTieredEventsAvailable();
                
                // For testing
                UpdateTierProgress(tieredEventsOverview.tieredEvents.First().Key, tieredEventsOverview.tieredEvents.First().Value.tiers.First().id, 0, "Derp", 0);
            } else {
                SpilLogging.Error("Error retrieving tiered event data!");
                SpilUnityImplementationBase.fireTieredEventsNotAvailable();
            }
        }

        public static void UpdateTierProgress(int tieredEventId, int tierId, int entityId, string entityType, int entityAmount) {
            string tierName = "";
            foreach (TieredEventTier tier in tieredEventsOverview.tieredEvents[tieredEventId].tiers) {
                if (tier.id == tierId) {
                    tierName = tier.name;
                    break;
                }
            }

            Spil.Instance.SendCustomEvent(
                EventUpdateTierProgress,
                new Dictionary<string, object>() {
                    { TieredEventId, tieredEventId },
                    { TierEventName, tieredEventsOverview.tieredEvents[tieredEventId].name },
                    { EntityId, entityId },
                    { EntityType, entityType },
                    { EntityAmount, entityAmount },
                    { TierName, tierName }
                }
            );
        }

        public static void ProcessUpdateTierProgress(JSONObject responseData) {
            TieredEventProgress progress = JsonHelper.getObjectFromJson<TieredEventProgress>(responseData.ToString());
            tieredEventsOverview.progress.Remove(progress.tieredEventId);
            tieredEventsOverview.progress.Add(progress.tieredEventId, progress);

            SpilUnityImplementationBase.fireTieredEventUpdated(progress);
            
            // For testing
            ClaimTierReward(progress.tieredEventId, progress.currentTierId);
        }

        public static void ClaimTierReward(int tieredEventId, int tierId) {
            TieredEvent tieredEvent = tieredEventsOverview.tieredEvents[tieredEventId];

            if(tieredEvent == null) {
                return;
            }

            string tierName = "";
            foreach (TieredEventTier tier in tieredEventsOverview.tieredEvents[tieredEventId].tiers) {
                if (tier.id == tierId) {
                    tierName = tier.name;
                    break;
                }
            }

            Spil.Instance.SendCustomEvent(
                EventClaimTierReward,
                new Dictionary<string, object>() {
                    { TieredEventId, tieredEventId },
                    { TierEventName, tieredEventsOverview.tieredEvents[tieredEventId].name },
                    { TierName, tierName }                    
                }
            );
        }

        public static void ProcessClaimTierReward(JSONObject responseData) {
            ClaimRewardData rewardDataResponse = JsonHelper.getObjectFromJson<ClaimRewardData>(responseData.ToString());

            foreach(TieredEventTierRewardData rewardDataTier in rewardDataResponse.tiers) {
                string tierName = tieredEventsOverview.tieredEvents.First(a => a.Value.id == rewardDataResponse.tieredEventId).Value.name;
                foreach(TieredEventReward reward in rewardDataTier.rewards) {
                    if (reward.type.Equals(Currency)) {
                        Spil.Instance.AddCurrencyToWallet(reward.id, reward.amount, PlayerDataUpdateReasons.TierReward, tierName, "sdk", null);
                    }
                    else if (reward.type.Equals(Item)) {
                        Spil.Instance.AddItemToInventory(reward.id, reward.amount, PlayerDataUpdateReasons.TierReward, tierName, "sdk", null);
                    }
                }
            }

            SpilUnityImplementationBase.fireTieredEventRewardClaimed(rewardDataResponse.tieredEventId, rewardDataResponse);

            // For testing
            ShowTieredEventProgress(rewardDataResponse.tieredEventId);
        }

        public static void ShowTieredEventProgress(int tieredEventId) {
            TieredEvent tieredEvent = tieredEventsOverview.tieredEvents[tieredEventId];

            if(tieredEvent == null) {
                return;
            }

            Spil.Instance.SendCustomEvent(
                EventShowTierProgress, 
                new Dictionary<string, object>() {
                    { TieredEventId, tieredEventId },
                    { TierEventName, tieredEventsOverview.tieredEvents[tieredEventId].name }
                }
            );
        }
        
        public static void ProcessShowTieredEventProgress(JSONObject responseData) {
            ShowProgressResponse showProgressResponse = JsonHelper.getObjectFromJson<ShowProgressResponse>(responseData.ToString());
            // Show splash screen and inject data.
            OpenTieredEventProgressView(showProgressResponse);
        }

        public static GameObject TieredEventProgress;

        static void OpenTieredEventProgressView(ShowProgressResponse showProgressResponse) {
            CloseStageView();

            TieredEventProgress = (GameObject)Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Spilgames/Editor/Prefabs/TieredEventInfo.prefab"));
            Text labelText = TieredEventProgress.transform.Find("TieredEventInfoText").gameObject.GetComponent<Text>();
            labelText.text = JsonHelper.getJSONFromObject(showProgressResponse);
            TieredEventProgress.SetActive(true);

            SpilUnityImplementationBase.fireTieredEventStageOpen();
        }

        public void ClosePrefab() {
            CloseStageView();
        }

        public static void CloseStageView() {
            Destroy(TieredEventProgress);
            SpilUnityImplementationBase.fireTieredEventStageClosed();
        }

        public static TieredEventsOverview GetTieredEventsOverview() {
            return tieredEventsOverview;
        }
    }

    public class TieredEventResponse : ResponseEvent {
        public static void ProcessTieredEventResponse(ResponseEvent response) {
            if (response.data != null) {
                switch (response.action.ToLower().Trim()) {
                    case "request":
                        TieredEventManager.ProcessRequestTieredEvents(response.data);
                    break;
                    case "update":
                        TieredEventManager.ProcessUpdateTierProgress(response.data);
                    break;
                    case  "claim":
                        TieredEventManager.ProcessClaimTierReward(response.data);
                    break;
                    case "show":
                        TieredEventManager.ProcessShowTieredEventProgress(response.data);
                    break;
                }
            }
        }
    }
}

#endif