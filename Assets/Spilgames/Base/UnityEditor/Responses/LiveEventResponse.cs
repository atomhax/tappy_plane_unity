using System;
using System.Collections.Generic;
using SpilGames.Unity.Base.Implementations;
using SpilGames.Unity.Base.SDK;
using SpilGames.Unity.Helpers.GameData;
using SpilGames.Unity.Helpers.PlayerData;
using SpilGames.Unity.Json;
using UnityEngine;

#if UNITY_EDITOR


namespace SpilGames.Unity.Base.UnityEditor.Responses {
    public class LiveEventResponse : ResponseEvent {
        enum StageType {
            START,
            PROGRESS,
            INFO
        }

        private static Spil.LiveEventRewardTypeEnum rewardType = Spil.MonoInstance.LiveEventRewardType;
        public static LiveEventOverview liveEventOverview = new LiveEventOverview();

        public static void ProcessLiveEventResponse(ResponseEvent response) {
            if (response.data != null) {
                if (response.action.ToLower().Trim().Equals("request")) {
                    ProcessRequestLiveEvent(response.data);
                }
                else if (response.action.ToLower().Trim().Equals("nextstage")) {
                    ProcessAdvanceToNextStage(response.data);
                }
                else if (response.action.ToLower().Trim().Equals("applyitems")) {
                    ProcessApplyItems(response.data);
                }
                else if (response.action.ToLower().Trim().Equals("notavailable")) {
                    SpilUnityImplementationBase.fireLiveEventNotAvailable();
                }
            }
        }

        public static void AdvanceToNextStage() {
            if (liveEventOverview.fromStartStage) {
                if (liveEventOverview.currentStage != null) {
                    OpenStageView(StageType.PROGRESS, liveEventOverview.currentStage);
                    liveEventOverview.fromStartStage = false;
                }
                else {
                    SpilLogging.Error("Error validating live event next stage!");
                    CloseStageView();
                }
            }
            else {
                SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent>();
                spilEvent.eventName = "liveEventNextStage";

                spilEvent.customData.AddField("liveEventId", liveEventOverview.liveEventId);

                spilEvent.Send();
            }
        }

        public static void ApplyItems(JSONObject items) {
            SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent>();
            spilEvent.eventName = "liveEventApplyItems";

            spilEvent.customData.AddField("liveEventId", liveEventOverview.liveEventId);

            if (items != null) {
                spilEvent.customData.AddField("items", items);

                for (int i = 0; i < items.Count; i++) {
                    int id = (int) items.list[i].GetField("id").n;
                    int amount = (int) items.list[i].GetField("amount").n;
                    Spil.Instance.SubtractItemFromInventory(id, amount, PlayerDataUpdateReasons.LiveEvent, null);
                }
            }
            else {
                spilEvent.customData.AddField("items", "");
            }

            spilEvent.Send();
        }

        private static void ProcessRequestLiveEvent(JSONObject response) {
            if (response != null && response.Count > 0) {
                if (response.HasField("config")) {
                    liveEventOverview.liveEventConfig = response.GetField("config");
                }

                if (response.HasField("currentStage")) {
                    liveEventOverview.currentStage = response.GetField("currentStage");

                    JSONObject rewards = new JSONObject(JSONObject.Type.ARRAY);
                    rewards.Add(liveEventOverview.currentStage.GetField("rewards"));
                    liveEventOverview.currentStage.AddField("rewards", rewards);

                    liveEventOverview.fromStartStage = true;
                }

                if (response.HasField("eventItems")) {
                    liveEventOverview.eventItems = response.GetField("eventItems");
                }

                if (response.HasField("liveEventId")) {
                    liveEventOverview.liveEventId = (int) response.GetField("liveEventId").n;
                }

                if (response.HasField("startDate")) {
                    liveEventOverview.startDate = (long) response.GetField("startDate").n;
                }

                if (response.HasField("endDate")) {
                    liveEventOverview.endDate = (long) response.GetField("endDate").n;
                }

                if (response.HasField("stage")) {
                    JSONObject startStage = response.GetField("stage");

                    JSONObject rewards = new JSONObject(JSONObject.Type.ARRAY);
                    rewards.Add(startStage.GetField("rewards"));

                    startStage.AddField("rewards", rewards);

                    liveEventOverview.startStage = startStage;
                    
                    SpilUnityImplementationBase.fireLiveEventAvailable();                    
                }
            }
            else {
                SpilLogging.Error("Error retrieving live event data!");
            }
        }

        private static void ProcessAdvanceToNextStage(JSONObject response) {
            bool valid = response.GetField("valid").b;

            if (!valid) {
                SpilLogging.Error("Error validating live event next stage!");
                CloseStageView();
                return;
            }

            liveEventOverview.currentStage = null;
            if (response.HasField("nextStage")) {
                liveEventOverview.currentStage = response.GetField("nextStage");

                JSONObject rewards = new JSONObject(JSONObject.Type.ARRAY);
                rewards.Add(liveEventOverview.currentStage.GetField("rewards"));
                liveEventOverview.currentStage.AddField("rewards", rewards);
            }
            else if (response.HasField("noMoreStages")) {
                SpilUnityImplementationBase.fireLiveEventCompleted();
                CloseStageView();
                return;
            }

            if (liveEventOverview.currentStage != null) {
                OpenStageView(StageType.PROGRESS, liveEventOverview.currentStage);
            }
            else {
                SpilLogging.Error("Error opening next stage due to missing data!");
            }
        }

        private static void ProcessApplyItems(JSONObject response) {
            bool valid = response.GetField("valid").b;

            if (!valid) {
                SpilLogging.Error("Error validating live event next stage!");
                CloseStageView();
                return;
            }

            bool metRequirements = response.GetField("metRequirements").b;

            if (!metRequirements) {
                SpilUnityImplementationBase.fireLiveEventMetRequirements(false);
                return;
            }
            SpilUnityImplementationBase.fireLiveEventMetRequirements(true);

            if (rewardType == Spil.LiveEventRewardTypeEnum.EXTERNAL) {
                List<Reward> rewards = new List<Reward>();

                Reward reward = new Reward();
                reward.externalId = Spil.LiveEventExternalRewardId;
                reward.amount = Spil.LiveEventRewardAmount;

                rewards.Add(reward);

                string rewardsJSON = JsonHelper.getJSONFromObject(rewards);

                JSONObject json = new JSONObject();
                json.AddField("data", rewardsJSON);

                SpilUnityImplementationBase.fireLiveEventReward(json.Print());
            }
            else {
                int id = Spil.LiveEventRewardId;
                int amount = Spil.LiveEventRewardAmount;

                if (id == 0 || amount == 0) {
                    SpilLogging.Error("Daily Bonus Rewards not configured for Editor!");
                }

                if (rewardType == Spil.LiveEventRewardTypeEnum.CURRENCY) {
                    SpilUnityEditorImplementation.pData.WalletOperation("add", id, amount, PlayerDataUpdateReasons.LiveEvent, null, "LiveEvent", null);
                }
                else if (rewardType == Spil.LiveEventRewardTypeEnum.ITEM) {
                    SpilUnityEditorImplementation.pData.InventoryOperation("add", id, amount, PlayerDataUpdateReasons.LiveEvent, null, "LiveEvent", null);
                }
            }

            JSONObject nextStage = response.GetField("nextStage");
            nextStage.AddField("givenReward", response.GetField("reward"));

            JSONObject nextStageRewards = new JSONObject(JSONObject.Type.ARRAY);
            nextStageRewards.Add(liveEventOverview.currentStage.GetField("rewards"));
            liveEventOverview.currentStage.AddField("rewards", nextStageRewards);

            liveEventOverview.currentStage = nextStage;
            OpenStageView(StageType.INFO, nextStage);
        }

        public static void OpenLiveEvent() {
            if (liveEventOverview.startStage != null) {
                OpenStageView(StageType.START, liveEventOverview.startStage);
            } else {
                SpilUnityImplementationBase.fireLiveEventNotAvailable();
            }
            
        }
        
        private static void OpenStageView(StageType stageType, JSONObject stage) {
            SpilLogging.Log(stage.Print());

            switch (stageType) {
                case StageType.START:
                    GameObject overlayObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    WebOverlay overlay = overlayObject.AddComponent<WebOverlay>();
                    overlay.overlayType = "start";
                    overlay.stageData = stage;
                    break;
                case StageType.PROGRESS:

                    break;
                case StageType.INFO:

                    break;
            }
        }

        private static void CloseStageView() {
        }

        public static long GetLiveEventStartDate() {
            return liveEventOverview != null ? liveEventOverview.startDate : 0;
        }

        public static long GetLiveEventEndDate() {
            return liveEventOverview != null ? liveEventOverview.endDate : 0;
        }

        public static string GetLiveEventConfig() {
            return liveEventOverview != null ? liveEventOverview.liveEventConfig.Print() : null;
        }

        public class WebOverlay : MonoBehaviour {
            public string overlayType;
            public JSONObject stageData;

            void OnGUI() {
                if (overlayType.Equals("start")) {
                    GUI.Label(new Rect(10, 10, (Screen.width - 20), (Screen.height - 20) / 2), stageData.Print());
                    
                    if (GUI.Button(new Rect(10, Screen.height / 2, (Screen.width - 20), (Screen.height - 20) / 2), "Continue")) {
                        AdvanceToNextStage();
                    }
                }
                else if (overlayType.Equals("progress")) {
                    if (GUI.Button(new Rect(10, 10, (Screen.width - 20), (Screen.height - 20) / 2), "Close")) {
                    }

                    if (GUI.Button(new Rect(10, Screen.height / 2, (Screen.width - 20), (Screen.height - 20) / 2), "Collect Reward")) {
                    }
                }
                else if (overlayType.Equals("info")) {
                }
            }
        }

        public class LiveEventOverview {
            public int liveEventId;
            public JSONObject startStage = null;
            public JSONObject currentStage = null;
            public JSONObject liveEventConfig = new JSONObject();
            public JSONObject eventItems = new JSONObject(JSONObject.Type.ARRAY);
            public long startDate;
            public long endDate;
            public bool fromStartStage;
        }

        public class Reward {
            public int id;
            public string externalId;
            public string type;
            public int amount;
        }
    }
}

#endif