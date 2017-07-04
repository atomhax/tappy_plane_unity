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
        public static GameObject overlayObject;
        public static WebOverlay overlay;

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
                } else if (response.action.ToLower().Trim().Equals("nextstage")) {
                    ProcessAdvanceToNextStage(response.data);
                } else if (response.action.ToLower().Trim().Equals("applyitems")) {
                    ProcessApplyItems(response.data);
                } else if (response.action.ToLower().Trim().Equals("notavailable")) {
                    SpilUnityImplementationBase.fireLiveEventNotAvailable();
                }
            }
        }

        public static void AdvanceToNextStage() {
            if (liveEventOverview.currentStage != null && liveEventOverview.alwaysShowStartStage) {
                OpenStageView(StageType.PROGRESS, liveEventOverview.currentStage);
            } else {
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
            } else {
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
            } else {
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
            } else if (response.HasField("noMoreStages")) {
                SpilUnityImplementationBase.fireLiveEventCompleted();
                CloseStageView();
                return;
            }

            if (liveEventOverview.currentStage != null) {
                OpenStageView(StageType.PROGRESS, liveEventOverview.currentStage);
            } else {
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
            } else {
                int id = Spil.LiveEventRewardId;
                int amount = Spil.LiveEventRewardAmount;

                if (id == 0 || amount == 0) {
                    SpilLogging.Error("Daily Bonus Rewards not configured for Editor!");
                }

                if (rewardType == Spil.LiveEventRewardTypeEnum.CURRENCY) {
                    SpilUnityEditorImplementation.pData.WalletOperation("add", id, amount, PlayerDataUpdateReasons.LiveEvent, null, "LiveEvent", null);
                } else if (rewardType == Spil.LiveEventRewardTypeEnum.ITEM) {
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
                } else {
                    SpilUnityImplementationBase.fireLiveEventNotAvailable();
                }
            } else {
                if (liveEventOverview.currentStage != null) {
                    OpenStageView(StageType.PROGRESS, liveEventOverview.currentStage);
                } else if (liveEventOverview.startStage != null) {
                    OpenStageView(StageType.START, liveEventOverview.startStage);
                } else {
                    SpilUnityImplementationBase.fireLiveEventNotAvailable();
                }
            }
        }

        private static void OpenStageView(StageType stageType, JSONObject stage) {
            SpilLogging.Log(stage.Print());

            CloseStageView();

            switch (stageType) {
                case StageType.START:
                    overlayObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    overlay = overlayObject.AddComponent<WebOverlay>();
                    overlay.overlayType = "start";
                    overlay.stageData = stage;
                    break;
                case StageType.PROGRESS:
                    overlayObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    overlay = overlayObject.AddComponent<WebOverlay>();
                    overlay.overlayType = "progress";
                    overlay.currentStage = stage;
                    break;
                case StageType.INFO:
                    overlayObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    overlay = overlayObject.AddComponent<WebOverlay>();
                    overlay.overlayType = "info";
                    overlay.currentStage = stage;
                    break;
            }
        }

        private static void CloseStageView() {
            if (overlayObject == null || overlay == null) return;
            Destroy(overlayObject);
            overlayObject = null;
            overlay = null;
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
            public JSONObject currentStage;

            public JSONObject applyItems = new JSONObject(JSONObject.Type.ARRAY);

            public string id = "";
            public string amount = "";

            private GUIStyle infoGuiStyle;
            private GUIStyle itemsGuiStyle;
            private GUIStyle textFieldGuiStyle;

            private void Start() {
                infoGuiStyle = CreateGuiStyleForText(false);
                itemsGuiStyle = CreateGuiStyleForText(true);

                textFieldGuiStyle = CreateGuiStyleTextField();
            }

            void OnGUI() {
                if (overlayType.Equals("start")) {
                    GUI.Label(new Rect(10, 10, (Screen.width - 20), (Screen.height - 20) / 2), "\nStart Page", infoGuiStyle);

                    if (GUI.Button(new Rect(10, Screen.height / 2, (Screen.width - 20), (Screen.height - 20) / 2), "Continue")) {
                        AdvanceToNextStage();
                    }
                } else if (overlayType.Equals("progress")) {
                    GUI.Label(new Rect(10, 10, (Screen.width - 20), (Screen.height - 20) / 2),
                        "\nProgress Page \n Requirements: " + currentStage.GetField("requirements").Print() + " \n \n Progress: " + (currentStage.HasField("progress") ? currentStage.GetField("progress").Print() : "") + "\n \n Applied Items: " + applyItems.Print(), infoGuiStyle);

                    GUI.Label(new Rect(10, (Screen.height / 2 + 10), 200, (Screen.height - 20) / 12 - 10), "Id", itemsGuiStyle);
                    id = GUI.TextField(new Rect(220, (Screen.height / 2 + 10), Screen.width - 230, (Screen.height - 20) / 12 - 10), id, textFieldGuiStyle);

                    GUI.Label(new Rect(10, (Screen.height / 2 + Screen.height / 12 + 10), 200, (Screen.height - 20) / 12 - 10), "Amount", itemsGuiStyle);
                    amount = GUI.TextField(new Rect(220, (Screen.height / 2 + Screen.height / 12 + 10), Screen.width - 230, (Screen.height - 20) / 12 - 10), amount, textFieldGuiStyle);

                    if (GUI.Button(new Rect(10, (Screen.height / 2 + Screen.height / 6), (Screen.width - 20), (Screen.height - 20) / 12 - 10), "Add Item")) {
                        AddItemToList(id, amount);
                    }

                    if (GUI.Button(new Rect(10, (Screen.height / 2 + Screen.height / 4), (Screen.width - 20), (Screen.height - 20) / 4), "Apply Items")) {
                        ApplyItems(applyItems);
                    }
                } else if (overlayType.Equals("info")) {
                    GUI.Label(new Rect(10, 10, (Screen.width - 20), (Screen.height - 20) / 2), "\nInfo/Claim Reward Page", infoGuiStyle);

                    if (GUI.Button(new Rect(10, Screen.height / 2, (Screen.width - 20), (Screen.height - 20) / 2), "Continue")) {
                        AdvanceToNextStage();
                    }
                }
            }

            private GUIStyle CreateGuiStyleForText(bool alignMiddle) {
                GUIStyle infoGuiStyle = new GUIStyle();
                GUIStyleState guiStyleState = new GUIStyleState();
                guiStyleState.textColor = Color.white;
                Color color = new Color(0f, 0f, 0f, 0.39f);
                Texture2D blackBackground = new Texture2D(Screen.width - 230, (Screen.height - 20) / 12 - 10);

                var fillColorArray = blackBackground.GetPixels();

                for (var i = 0; i < fillColorArray.Length; ++i) {
                    fillColorArray[i] = color;
                }

                blackBackground.SetPixels(fillColorArray);

                blackBackground.Apply();

                guiStyleState.background = blackBackground;
                infoGuiStyle.normal = guiStyleState;
                infoGuiStyle.wordWrap = true;
                infoGuiStyle.fontSize = 24;
                infoGuiStyle.padding = new RectOffset(10, 10, 10, 10);

                if (alignMiddle) {
                    infoGuiStyle.alignment = TextAnchor.MiddleLeft;
                }

                return infoGuiStyle;
            }

            private GUIStyle CreateGuiStyleTextField() {
                GUIStyle textFieldGuiStyle = new GUIStyle();
                GUIStyleState guiStyleState = new GUIStyleState();
                guiStyleState.textColor = Color.white;
                Color color = new Color(0f, 0f, 0f, 0.39f);
                Texture2D blackBackground = new Texture2D((Screen.width - 20), (Screen.height - 20) / 2);

                var fillColorArray = blackBackground.GetPixels();

                for (var i = 0; i < fillColorArray.Length; ++i) {
                    fillColorArray[i] = color;
                }

                blackBackground.SetPixels(fillColorArray);

                blackBackground.Apply();

                guiStyleState.background = blackBackground;
                textFieldGuiStyle.normal = guiStyleState;
                textFieldGuiStyle.fontSize = 24;
                textFieldGuiStyle.padding = new RectOffset(10, 10, 10, 10);
                textFieldGuiStyle.alignment = TextAnchor.MiddleLeft;

                return textFieldGuiStyle;
            }

            private void AddItemToList(string id, string amount) {
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
}

#endif