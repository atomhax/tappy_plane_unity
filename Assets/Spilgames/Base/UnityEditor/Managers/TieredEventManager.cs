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
        const string EventUpdateTierProgress = "updateTieredEventProgress";
        const string EventClaimTierReward = "claimTierReward";
        const string EventShowTierProgress = "showTieredEventProgress";

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

        public static TieredEventsOverview tieredEventsOverview = new TieredEventsOverview();

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

            SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent>();
            spilEvent.eventName = EventUpdateTierProgress;

            spilEvent.customData.AddField(TieredEventId, tieredEventId);
            spilEvent.customData.AddField(TierEventName, tieredEventsOverview.tieredEvents[tieredEventId].name);
            spilEvent.customData.AddField(TierName, tierName);
            spilEvent.customData.AddField(EntityId, entityId);
            spilEvent.customData.AddField(EntityType, entityType);
            spilEvent.customData.AddField(EntityAmount, entityAmount);
            
            spilEvent.Send();
        }

        public static void ProcessUpdateTierProgress(JSONObject responseData) {
            if (responseData != null && responseData.Count > 0) {
                TieredEventProgress progress = JsonHelper.getObjectFromJson<TieredEventProgress>(responseData.ToString());
                tieredEventsOverview.progress.Remove(progress.tieredEventId);
                tieredEventsOverview.progress.Add(progress.tieredEventId, progress);

                SpilUnityImplementationBase.fireTieredEventUpdated(JsonHelper.getJSONFromObject(progress));
            }
            else {
                SpilErrorMessage error = new SpilErrorMessage(42, "TieredEventShowProgressError", "Unable to show tiered event progress.");
                SpilUnityImplementationBase.fireTieredEventsError(JsonHelper.getJSONFromObject(error));
            }
            
        }

        public static void ClaimTierReward(int tieredEventId, int tierId) {
            TieredEvent tieredEvent = tieredEventsOverview.tieredEvents[tieredEventId];

            if(tieredEvent == null) {
                SpilErrorMessage error = new SpilErrorMessage(44, "TieredEventClaimTierError", "Unable to claim the tier reward.");
                SpilUnityImplementationBase.fireTieredEventsError(JsonHelper.getJSONFromObject(error));
                return;
            }

            string tierName = "";
            foreach (TieredEventTier tier in tieredEventsOverview.tieredEvents[tieredEventId].tiers) {
                if (tier.id == tierId) {
                    tierName = tier.name;
                    break;
                }
            }

            SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent>();
            spilEvent.eventName = EventClaimTierReward;

            spilEvent.customData.AddField(TieredEventId, tieredEventId);
            spilEvent.customData.AddField(TierEventName, tieredEventsOverview.tieredEvents[tieredEventId].name);
            spilEvent.customData.AddField(TierName, tierName);
            
            spilEvent.Send();
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
        }

        public static void ShowTieredEventProgress(int tieredEventId) {
            TieredEvent tieredEvent = tieredEventsOverview.tieredEvents[tieredEventId];

            if(tieredEvent == null) {
                SpilErrorMessage error = new SpilErrorMessage(42, "TieredEventShowProgressError", "Unable to show tiered event progress.");
                SpilUnityImplementationBase.fireTieredEventsError(JsonHelper.getJSONFromObject(error));
                return;
            }

            SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent>();
            spilEvent.eventName = EventClaimTierReward;

            spilEvent.customData.AddField(TieredEventId, tieredEventId);
            spilEvent.customData.AddField(TierEventName, tieredEventsOverview.tieredEvents[tieredEventId].name);
            
            spilEvent.Send();
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

            SpilUnityImplementationBase.fireTieredEventProgressOpen();
        }

        public void ClosePrefab() {
            CloseStageView();
        }

        public static void CloseStageView() {
            Destroy(TieredEventProgress);
            SpilUnityImplementationBase.fireTieredEventProgressClosed();
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
                    case "error":
                        SpilErrorMessage errorMessage = new SpilErrorMessage();
                        if (response.name.Contains("claimTierReward")) {
                            errorMessage = new SpilErrorMessage(44, "TieredEventClaimTierError", "Unable to claim the tier reward.");
                        } else if(response.name.Contains("showTieredEventProgress")) {
                            errorMessage = new SpilErrorMessage(42, "TieredEventShowProgressError", "Unable to show tiered event progress.");
                        } else if (response.name.Contains("updateTIeredEventProgress")) {
                            errorMessage = new SpilErrorMessage(43, "TieredEventUpdateProgressError", "Unable to update the tiered event progress.");
                        }
                        
                        SpilUnityImplementationBase.fireTieredEventsError(JsonHelper.getJSONFromObject(errorMessage));
                        break;
                }
            }
        }
    }
}

#endif