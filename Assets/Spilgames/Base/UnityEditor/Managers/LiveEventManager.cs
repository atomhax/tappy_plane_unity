using System;
using System.Collections.Generic;
using SpilGames.Unity.Base.Implementations;
using SpilGames.Unity.Base.SDK;
using SpilGames.Unity.Helpers.GameData;
using SpilGames.Unity.Helpers.PlayerData;
using SpilGames.Unity.Json;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR


namespace SpilGames.Unity.Base.UnityEditor.Managers {
    public class LiveEventManager : MonoBehaviour{
        public enum StageType {
            START,
            PROGRESS,
            INFO
        }
        
        public static StageType StageTypeValue;
        public static JSONObject stageData;
        public static JSONObject currentStage;

        public static JSONObject applyItems;

        public static string id = "";
        public static string amount = "";
        
        public static GameObject LiveEventStart;
        public static GameObject LiveEventProgress;
        public static GameObject LiveEventInfo;

        public Text InfoText;
        public InputField IdField;
        public InputField AmountField;

        private static Spil.LiveEventRewardTypeEnum rewardType;
        void Awake() {
            rewardType = Spil.MonoInstance.LiveEventRewardType; 
        }
        
        void Update() {
            switch (StageTypeValue) {
                    case StageType.START:
                        break;
                    case StageType.PROGRESS:
                        InfoText.text = "\nProgress Page \n Requirements: " + currentStage.GetField("requirements").Print() + " \n \n Progress: " + (currentStage.HasField("progress") ? currentStage.GetField("progress").Print() : "") + "\n \n Applied Items: " + applyItems.Print();
                        id = IdField.text;
                        amount = AmountField.text;
                        break;
                    case StageType.INFO:
                        break;
            }
        }

        public static LiveEventOverview liveEventOverview = new LiveEventOverview();
        
        public void AdvanceToNextStage() {
            if (liveEventOverview.currentStage != null && liveEventOverview.alwaysShowStartStage) {
                OpenStageView(StageType.PROGRESS, liveEventOverview.currentStage);
            }
            else {
                SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent>();
                spilEvent.eventName = "liveEventNextStage";

                spilEvent.customData.AddField("liveEventId", liveEventOverview.liveEventId);

                spilEvent.Send();
            }
        }

        public void ApplyItems(JSONObject items) {
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

        public static void ProcessRequestLiveEvent(JSONObject response) {
            if (response != null && response.Count > 0) {
                if (response.HasField("config")) {
                    liveEventOverview.liveEventConfig = response.GetField("config");
                }

                if (response.HasField("currentStage")) {
                    liveEventOverview.currentStage = response.GetField("currentStage");

                    JSONObject rewards = new JSONObject(JSONObject.Type.ARRAY);
                    rewards.Add(liveEventOverview.currentStage.GetField("rewards"));
                    liveEventOverview.currentStage.AddField("rewards", rewards);
                }

                if (response.HasField("alwaysShowStartStage")) {
                    liveEventOverview.alwaysShowStartStage = response.GetField("alwaysShowStartStage").b;
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

        public static void ProcessAdvanceToNextStage(JSONObject response) {
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

        public static void ProcessApplyItems(JSONObject response) {
            bool valid = response.GetField("valid").b;

            if (!valid) {
                SpilLogging.Error("Error validating live event next stage!");
                CloseStageView();
                return;
            }

            bool metRequirements = response.GetField("metRequirements").b;

            if (!metRequirements) {
                SpilUnityImplementationBase.fireLiveEventMetRequirements(false);
                if (response.HasField("progress")) {
                    liveEventOverview.currentStage.RemoveField("progress");
                    liveEventOverview.currentStage.AddField("progress", response.GetField("progress"));
                }
                CloseStageView();
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
            if (liveEventOverview.alwaysShowStartStage) {
                if (liveEventOverview.startStage != null) {
                    OpenStageView(StageType.START, liveEventOverview.startStage);
                }
                else {
                    SpilUnityImplementationBase.fireLiveEventNotAvailable();
                }
            }
            else {
                if (liveEventOverview.currentStage != null) {
                    OpenStageView(StageType.PROGRESS, liveEventOverview.currentStage);
                }
                else if (liveEventOverview.startStage != null) {
                    OpenStageView(StageType.START, liveEventOverview.startStage);
                }
                else {
                    SpilUnityImplementationBase.fireLiveEventNotAvailable();
                }
            }
        }

        private static void OpenStageView(StageType stageType, JSONObject stage) {
            SpilLogging.Log(stage.Print());

            CloseStageView();

            StageTypeValue = stageType;
            switch (stageType) {
                case StageType.START:
                    LiveEventStart = (GameObject) Instantiate(Resources.Load("Spilgames/Editor/LiveEventStart"));
                    LiveEventStart.SetActive(true);
                   
                    stageData = stage;
                    break;
                case StageType.PROGRESS:
                    LiveEventProgress = (GameObject) Instantiate(Resources.Load("Spilgames/Editor/LiveEventProgress"));
                    LiveEventProgress.SetActive(true);
                    
                    applyItems = new JSONObject(JSONObject.Type.ARRAY);
                    currentStage = stage;
                    break;
                case StageType.INFO:
                    LiveEventInfo = (GameObject) Instantiate(Resources.Load("Spilgames/Editor/LiveEventInfo"));
                    LiveEventInfo.SetActive(true);
                    
                    currentStage = stage;
                    break;
            }
        }

        private static void CloseStageView() {
            stageData = null;
            currentStage = null;
            id = "";
            amount = "";
            applyItems = null;
            
            switch (StageTypeValue) {
                case StageType.START:
                    Destroy(LiveEventStart);
                    break;
                case StageType.PROGRESS:
                    Destroy(LiveEventProgress);
                    break;
                case StageType.INFO:
                    Destroy(LiveEventInfo);
                    break;
            }
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

        public void AddItemToList() {
            AddItemToList(id, amount);
            id = "";
            amount = "";
            IdField.text = "";
            AmountField.text = "";
        }
        
        public void AddItemToList(string id, string amount) {
            if (id.Equals("") || amount.Equals("")) return;
            int idValue = Convert.ToInt32(id);
            int amountValue = Convert.ToInt32(amount);
            for (int i = 0; i < applyItems.Count; i++) {
                JSONObject jsonObject = applyItems.list[i];
                if (jsonObject.GetField("id").n != idValue) continue;
                int oldAmount = (int) jsonObject.GetField("amount").n;
                int newAmount = oldAmount + amountValue;

                jsonObject.RemoveField("amount");
                jsonObject.AddField("amount", newAmount);

                return;
            }

            JSONObject item = new JSONObject();
            item.AddField("id", idValue);
            item.AddField("amount", amountValue);

            applyItems.Add(item);
        }

        public void ApplyItems() {
            ApplyItems(applyItems);
        }

        public void CloseStage() {
            CloseStageView();
        }        
        public class LiveEventOverview {
            public int liveEventId;
            public JSONObject startStage = null;
            public JSONObject currentStage = null;
            public JSONObject liveEventConfig = new JSONObject();
            public JSONObject eventItems = new JSONObject(JSONObject.Type.ARRAY);
            public long startDate;
            public long endDate;
            public bool alwaysShowStartStage = false;
        }

        public class Reward {
            public int id;
            public string externalId;
            public string type;
            public int amount;
        }
    }
    
    public class LiveEventResponse : ResponseEvent {
        public static void ProcessLiveEventResponse(ResponseEvent response) {
            if (response.data != null) {
                if (response.action.ToLower().Trim().Equals("request")) {
                    LiveEventManager.ProcessRequestLiveEvent(response.data);
                } else if (response.action.ToLower().Trim().Equals("nextstage")) {
                    LiveEventManager.ProcessAdvanceToNextStage(response.data);
                } else if (response.action.ToLower().Trim().Equals("applyitems")) {
                    LiveEventManager.ProcessApplyItems(response.data);
                } else if (response.action.ToLower().Trim().Equals("notavailable")) {
                    SpilUnityImplementationBase.fireLiveEventNotAvailable();
                }
            }
        }
    }
}

#endif