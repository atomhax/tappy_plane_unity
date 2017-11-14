#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using SpilGames.Unity.Base.Implementations;
using SpilGames.Unity.Base.SDK;
using SpilGames.Unity.Json;
using UnityEngine;

namespace SpilGames.Unity.Base.UnityEditor.Managers {
    public class UserDataManager {
        public static List<UserDataVersion> userDataVersions = new List<UserDataVersion>();
        private static List<UserDataVersion> remoteUserDataVersions;
        private static UserDataMeta userDataMeta;

        public static void ProcessRequestUserData(WalletData receivedWallet, InventoryData receivedInventory, bool externalChange, string gameStateData, List<UserDataVersion> receivedUserDataVersions, UserDataMeta receivedUserDataMeta) {
            SpilUnityEditorImplementation.pData.CalculatePlayerDataResponse(receivedWallet, receivedInventory);
            GameStateManager.ProcessMyGameStateResponse(new JSONObject(gameStateData));

            if (receivedUserDataVersions != null) {
                userDataVersions = receivedUserDataVersions;
            }

            UpdateUserDataMeta();
            if (receivedUserDataMeta != null) {
                userDataMeta.serverTime = receivedUserDataMeta.serverTime;
            }

            SpilUnityImplementationBase.fireUserDataAvailable();
        }

        public static void ProcessMergeConflict(WalletData receivedWallet, InventoryData receivedInventory, string receivedGameState, List<UserDataVersion> receivedUserDataVersions, UserDataMeta receivedMetaData) {
            JSONObject localData = new JSONObject();
            JSONObject remoteData = new JSONObject();

            WalletData localWallet = SpilUnityEditorImplementation.pData.Wallet;
            InventoryData localInventory = SpilUnityEditorImplementation.pData.Inventory;
            string localPrivateGameState = GameStateManager.PrivateGameStateData;
            string localPublicGameState = GameStateManager.PublicGameStateData;
            UserDataMeta localDataMeta = userDataMeta;

            //Local
            JSONObject localPlayerData = new JSONObject();

            JSONObject walletJSON = new JSONObject(JsonHelper.getJSONFromObject(localWallet));
            localPlayerData.AddField("wallet", walletJSON);

            JSONObject inventoryJSON = new JSONObject(JsonHelper.getJSONFromObject(localInventory));
            localPlayerData.AddField("inventory", inventoryJSON);

            localData.AddField("playerData", localPlayerData);

            JSONObject privateGameStateJSON = new JSONObject();
            privateGameStateJSON.AddField("access", "private");
            privateGameStateJSON.AddField("data", localPrivateGameState);

            JSONObject publicGameStateJSON = null;
            if (localPublicGameState != null) {
                publicGameStateJSON = new JSONObject();
                publicGameStateJSON.AddField("access", "public");
                publicGameStateJSON.AddField("data", localPublicGameState);
            }

            JSONObject localGameStateArray = new JSONObject(JSONObject.Type.ARRAY);
            localGameStateArray.Add(privateGameStateJSON);
            if (publicGameStateJSON != null) {
                localGameStateArray.Add(publicGameStateJSON);
            }

            JSONObject localGameState = new JSONObject();
            localGameState.AddField("gameStates", localGameStateArray);

            localData.AddField("gameState", localGameState);

            localData.AddField("metaData", new JSONObject(JsonHelper.getJSONFromObject(localDataMeta)));

            //Remote
            JSONObject remotePlayerData = new JSONObject();

            for (int i = 0; i < receivedWallet.currencies.Count; i++) {
                for (int j = 0; j < SpilUnityEditorImplementation.gData.currencies.Count; j++) {
                    if (receivedWallet.currencies[i].id == SpilUnityEditorImplementation.gData.currencies[j].id) {
                        receivedWallet.currencies[i].name = SpilUnityEditorImplementation.gData.currencies[j].name;
                        receivedWallet.currencies[i].type = SpilUnityEditorImplementation.gData.currencies[j].type;
                        receivedWallet.currencies[i].displayName = SpilUnityEditorImplementation.gData.currencies[j].displayName;
                        receivedWallet.currencies[i].displayDescription = SpilUnityEditorImplementation.gData.currencies[j].displayDescription;
                        receivedWallet.currencies[i].imageUrl = SpilUnityEditorImplementation.gData.currencies[j].imageUrl;
                        receivedWallet.currencies[i].initialValue = SpilUnityEditorImplementation.gData.currencies[j].initialValue;
                    }
                }
            }

            remotePlayerData.AddField("wallet", new JSONObject(JsonHelper.getJSONFromObject(receivedWallet)));

            for (int i = 0; i < receivedInventory.items.Count; i++) {
                for (int j = 0; j < SpilUnityEditorImplementation.gData.items.Count; j++) {
                    if (receivedInventory.items[i].id == SpilUnityEditorImplementation.gData.items[j].id) {
                        receivedInventory.items[i].name = SpilUnityEditorImplementation.gData.items[j].name;
                        receivedInventory.items[i].type = SpilUnityEditorImplementation.gData.items[j].type;
                        receivedInventory.items[i].displayName = SpilUnityEditorImplementation.gData.items[j].displayName;
                        receivedInventory.items[i].displayDescription = SpilUnityEditorImplementation.gData.items[j].displayDescription;
                        receivedInventory.items[i].imageUrl = SpilUnityEditorImplementation.gData.items[j].imageUrl;
                        receivedInventory.items[i].initialValue = SpilUnityEditorImplementation.gData.items[j].initialValue;
                        receivedInventory.items[i].isGacha = SpilUnityEditorImplementation.gData.items[j].isGacha;
                        receivedInventory.items[i].content = SpilUnityEditorImplementation.gData.items[j].content;
                    }
                }
            }

            remotePlayerData.AddField("inventory", new JSONObject(JsonHelper.getJSONFromObject(receivedInventory)));

            remoteData.AddField("playerData", remotePlayerData);

            remoteData.AddField("gameState", new JSONObject(receivedGameState));

            remoteData.AddField("metaData", new JSONObject(JsonHelper.getJSONFromObject(receivedMetaData)));

            remoteUserDataVersions = receivedUserDataVersions;
            
            JSONObject mergeData = new JSONObject();
            mergeData.AddField("localData", localData);
            mergeData.AddField("remoteData", remoteData);

            SpilUnityImplementationBase.fireUserDataMergeConflict(mergeData.Print());
        }

        public static void ProcessSyncError() {
            SpilUnityImplementationBase.fireUserDataSyncError();
        }

        public static void ProcessLockError() {
            SpilUnityImplementationBase.fireUserDataLockError();
        }

        public static void ProcessDroppedResponse(string message) {
            SpilLogging.Log("Update event dropped. Message: " + message);
        }

        public static void MergeUserData(string mergeData, string mergeType) {
            JSONObject mergeDataJSON = new JSONObject(mergeData);

            JSONObject mergePlayerData = mergeDataJSON.GetField("playerData");
            JSONObject mergeGameStateJSON = mergeDataJSON.GetField("gameState");

            JSONObject mergeWallet = mergePlayerData.GetField("wallet");
            JSONObject mergeCurrencies = new JSONObject(JSONObject.Type.ARRAY);

            for (int i = 0; i < mergeWallet.GetField("currencies").Count; i++) {
                JSONObject currency = new JSONObject();
                currency.AddField("id", mergeWallet.GetField("currencies")[i].GetField("id"));
                currency.AddField("name", mergeWallet.GetField("currencies")[i].GetField("name"));
                currency.AddField("type", mergeWallet.GetField("currencies")[i].GetField("type"));
                currency.AddField("currentBalance", mergeWallet.GetField("currencies")[i].GetField("currentBalance"));
                currency.AddField("delta", mergeWallet.GetField("currencies")[i].GetField("delta"));
                mergeCurrencies.Add(currency);
            }
            mergePlayerData.RemoveField("currencies");
            mergePlayerData.AddField("currencies", mergeCurrencies);

            JSONObject mergeInventory = mergePlayerData.GetField("inventory");
            JSONObject mergeItems = new JSONObject(JSONObject.Type.ARRAY);
            
            for (int i = 0; i < mergeInventory.GetField("items").Count; i++) {
                JSONObject item = new JSONObject();
                item.AddField("id", mergeInventory.GetField("items")[i].GetField("id"));
                item.AddField("name", mergeInventory.GetField("items")[i].GetField("name"));
                item.AddField("type", mergeInventory.GetField("items")[i].GetField("type"));
                item.AddField("amount", mergeInventory.GetField("items")[i].GetField("amount"));
                item.AddField("delta", mergeInventory.GetField("items")[i].GetField("delta"));
                item.AddField("value", mergeInventory.GetField("items")[i].GetField("value"));
                mergeItems.Add(item);
            }
            mergePlayerData.RemoveField("items");
            mergePlayerData.AddField("items", mergeItems);

            if (!mergeType.Equals("local")) {
                WalletData wallet = JsonHelper.getObjectFromJson<WalletData>(mergeDataJSON.GetField("playerData").GetField("wallet").Print());
                InventoryData inventory = JsonHelper.getObjectFromJson<InventoryData>(mergeDataJSON.GetField("playerData").GetField("inventory").Print());

                SpilUnityEditorImplementation.pData.Wallet = wallet;
                SpilUnityEditorImplementation.pData.Inventory = inventory;

                JSONObject gameStateData = mergeGameStateJSON.GetField("gameStates");

                foreach (JSONObject gameState in gameStateData.list) {
                    string access = null;
                    if (gameState.HasField("access")) {
                        access = gameState.GetField("access").str;
                    }

                    string data = null;
                    if (gameState.HasField("data")) {
                        data = gameState.GetField("data").str;
                    }

                    if (access == null || data == null) continue;
                    if (access.Equals("private")) {
                        GameStateManager.PrivateGameStateData = data;
                        SpilUnityImplementationBase.fireGameStateUpdated("private");
                    } else if (access.Equals("public")) {
                        GameStateManager.PublicGameStateData = data;
                        SpilUnityImplementationBase.fireGameStateUpdated("public");
                    }
                }

                UpdateUserDataMeta();
            }

            SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent>();
            spilEvent.eventName = "mergeUserData";

            spilEvent.customData.AddField("playerData", mergeDataJSON.GetField("playerData"));
            spilEvent.customData.AddField("gameState", mergeDataJSON.GetField("gameState"));
            
            JSONObject deviceVersionsJSON = new JSONObject();
            deviceVersionsJSON.AddField("local", GenerateUserDataVersionsJSON(userDataVersions));

            if (remoteUserDataVersions != null) {
                deviceVersionsJSON.AddField("remote", GenerateUserDataVersionsJSON(remoteUserDataVersions));
            } else {
                deviceVersionsJSON.AddField("remote", GenerateUserDataVersionsJSON(new List<UserDataVersion>()));
            }

            spilEvent.customData.AddField("deviceVersions", deviceVersionsJSON);

            spilEvent.customData.AddField("mergeType", mergeType);

            spilEvent.Send();
        }

        public static void ProcessMergeUserData(List<UserDataVersion> receivedUserDataVersions) {
            userDataVersions = receivedUserDataVersions;

            remoteUserDataVersions = null;
            
            Spil.Instance.RequestLiveEvent();
            SpilUnityImplementationBase.fireUserDataMergeSuccessful();
        }

        public static void UpdateUserDataMeta() {
            if (userDataMeta == null) {
                userDataMeta = new UserDataMeta();
            }

            if (userDataMeta.deviceModel == null) {
                userDataMeta.deviceModel = "Unity Editor";
            }

            if (userDataMeta.timezoneOffset == 0) {
                userDataMeta.timezoneOffset = (int) (DateTime.UtcNow - DateTime.Now).TotalMinutes;
            }

            if (userDataMeta.appVersion == null) {
                userDataMeta.appVersion = Application.version;
            }

            userDataMeta.clientTime = DateTime.Now.Millisecond;
        }

        public static void UpdateUserDataVersions() {
            bool check = false;

            for (int i = 0; i < userDataVersions.Count; i++) {
                if (userDataVersions[i].deviceId.Equals(SystemInfo.deviceUniqueIdentifier)) {
                    check = true;
                }
            }

            if (!check) {
                UserDataVersion userDataVersion = new UserDataVersion();
                userDataVersion.deviceId = SystemInfo.deviceUniqueIdentifier;
                userDataVersion.version = 0;

                userDataVersions.Add(userDataVersion);
            }

            for (int i = 0; i < userDataVersions.Count; i++) {
                if (userDataVersions[i].deviceId.Equals(SystemInfo.deviceUniqueIdentifier)) {
                    userDataVersions[i].version++;
                }
            }
        }

        public static JSONObject GenerateUserDataVersionsJSON(List<UserDataVersion> userDataVersions) {
            if (userDataVersions.Count == 0) {
                UserDataVersion localUserDataVersion = new UserDataVersion();
                localUserDataVersion.deviceId = SystemInfo.deviceUniqueIdentifier;
                localUserDataVersion.version = 0;

                userDataVersions.Add(localUserDataVersion);
            }

            JSONObject userDataVersionsJSON = new JSONObject();

            foreach (UserDataVersion userDataVersion in userDataVersions) {
                userDataVersionsJSON.AddField(userDataVersion.deviceId, userDataVersion.version);
            }

            return userDataVersionsJSON;
        }

        public static void ShowSyncErrorDialog(string title, string message, string mergeButton) {
            GameObject overlayObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            overlayObject.GetComponent<MeshRenderer>().enabled = false;
            UserDataOverlay overlay = overlayObject.AddComponent<UserDataOverlay>();

            overlay.title = title;
            overlay.message = message;
            overlay.pozitiveButton = mergeButton;
            overlay.overlayType = "syncError";
        }

        public static void ShowMergeFailedDialog(string title, string message, string retryButton, string mergeData, string mergeType) {
            GameObject overlayObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            overlayObject.GetComponent<MeshRenderer>().enabled = false;
            UserDataOverlay overlay = overlayObject.AddComponent<UserDataOverlay>();

            overlay.title = title;
            overlay.message = message;
            overlay.pozitiveButton = retryButton;
            overlay.mergeData = mergeData;
            overlay.mergeType = mergeType;
            overlay.overlayType = "mergeFailed";
        }

        public static void ShowMergeConflictDialog(string title, string message, string localButton, string remoteButton, string mergeButton) {
            GameObject overlayObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            overlayObject.GetComponent<MeshRenderer>().enabled = false;
            UserDataOverlay overlay = overlayObject.AddComponent<UserDataOverlay>();

            overlay.title = title;
            overlay.message = message;
            overlay.pozitiveButton = localButton;
            overlay.negativeButton = remoteButton;
            overlay.neutralButton = mergeButton;
            overlay.overlayType = "mergeConflict";
        }

        public class UserDataOverlay : MonoBehaviour {
            public string title;
            public string message;
            public string pozitiveButton;
            public string negativeButton;
            public string neutralButton;

            public string mergeData;
            public string mergeType;

            public string overlayType;

            private GUIStyle textFieldGuiStyle;

            private void Start() {
                textFieldGuiStyle = CreateGuiStyleTextField();
            }

            void OnGUI() {
                switch (overlayType) {
                    case "syncError":
                        ShowSyncError();
                        break;
                    case "mergeFailed":
                        ShowMergeFailed();
                        break;
                    case "mergeConflict":
                        ShowMergeConflict();
                        break;
                }
            }

            private void ShowSyncError() {
                GUI.Label(new Rect(10, 10, (Screen.width - 20), (Screen.height - 20) / 3), title, textFieldGuiStyle);

                GUI.Label(new Rect(10, 10 + Screen.height / 3, (Screen.width - 20), (Screen.height - 20) / 3), message, textFieldGuiStyle);

                if (GUI.Button(new Rect(10, 10 + 2 * Screen.height / 3, (Screen.width - 20), (Screen.height - 20) / 3), pozitiveButton)) {
                    Spil.Instance.RequestUserData();
                    GameObject.Destroy(this.gameObject);
                }
            }

            private void ShowMergeFailed() {
                GUI.Label(new Rect(10, 10, (Screen.width - 20), (Screen.height - 20) / 3), title, textFieldGuiStyle);

                GUI.Label(new Rect(10, 10 + Screen.height / 3, (Screen.width - 20), (Screen.height - 20) / 3), message, textFieldGuiStyle);

                if (GUI.Button(new Rect(10, 10 + 2 * Screen.height / 3, (Screen.width - 20), (Screen.height - 20) / 3), pozitiveButton)) {
                    Spil.Instance.MergeUserData(mergeData, mergeType);
                    GameObject.Destroy(this.gameObject);
                }
            }

            private void ShowMergeConflict() {
                GUI.Label(new Rect(10, 10, (Screen.width - 20), (Screen.height - 20) / 3), title, textFieldGuiStyle);

                GUI.Label(new Rect(10, 10 + Screen.height / 3, (Screen.width - 20), (Screen.height - 20) / 3), message, textFieldGuiStyle);

                if (GUI.Button(new Rect(10, 10 + 2 * Screen.height / 3, (Screen.width - 20) / 3, (Screen.height - 20) / 3), pozitiveButton)) {
                    SpilUnityImplementationBase.fireUserDataHandleMerge("local");
                    GameObject.Destroy(this.gameObject);
                }

                if (GUI.Button(new Rect(10 + (Screen.width - 20) / 3, 10 + 2 * Screen.height / 3, (Screen.width - 20) / 3, (Screen.height - 20) / 3), negativeButton)) {
                    SpilUnityImplementationBase.fireUserDataHandleMerge("remote");
                    GameObject.Destroy(this.gameObject);
                }

                if (neutralButton != null) {
                    if (GUI.Button(new Rect(10 + 2 * (Screen.width - 20) / 3, 10 + 2 * Screen.height / 3, (Screen.width - 20) / 3, (Screen.height - 20) / 3), neutralButton)) {
                        SpilUnityImplementationBase.fireUserDataHandleMerge("merge");
                        GameObject.Destroy(this.gameObject);
                    }
                }
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
                textFieldGuiStyle.alignment = TextAnchor.MiddleCenter;

                return textFieldGuiStyle;
            }
        }
    }

    public class UserDataResponse : ResponseEvent {
        public static void ProcessUserDataResponse(ResponseEvent responseEvent) {
            WalletData receivedWallet = null;
            InventoryData receivedInventory = null;
            string gameStateData = null;
            List<UserDataVersion> userDataVersions = null;
            UserDataMeta userDataMeta = null;
            bool externalChange = false;

            if (responseEvent.data.HasField("playerData")) {
                JSONObject playerDataJSON = responseEvent.data.GetField("playerData");
                if (playerDataJSON.HasField("wallet")) {
                    JSONObject walletJSON = playerDataJSON.GetField("wallet");

                    receivedWallet = new WalletData();

                    if (walletJSON.HasField("currencies")) {
                        receivedWallet.currencies = new List<PlayerCurrencyData>();

                        JSONObject currenciesJSON = walletJSON.GetField("currencies");

                        for (int i = 0; i < currenciesJSON.Count; i++) {
                            PlayerCurrencyData currency = new PlayerCurrencyData();

                            currency.id = (int) currenciesJSON.list[i].GetField("id").n;
                            currency.currentBalance = (int) currenciesJSON.list[i].GetField("currentBalance").n;
                            currency.delta = (int) currenciesJSON.list[i].GetField("delta").n;

                            receivedWallet.currencies.Add(currency);
                        }
                    }

                    receivedWallet.offset = (int) walletJSON.GetField("offset").n;
                    receivedWallet.logic = walletJSON.GetField("logic").str;
                }

                if (playerDataJSON.HasField("inventory")) {
                    JSONObject inventoryJSON = playerDataJSON.GetField("inventory");

                    receivedInventory = new InventoryData();

                    if (inventoryJSON.HasField("items")) {
                        receivedInventory.items = new List<PlayerItemData>();

                        JSONObject itemsJSON = inventoryJSON.GetField("items");

                        for (int i = 0; i < itemsJSON.Count; i++) {
                            PlayerItemData item = new PlayerItemData();

                            item.id = (int) itemsJSON.list[i].GetField("id").n;
                            item.amount = (int) itemsJSON.list[i].GetField("amount").n;
                            item.delta = (int) itemsJSON.list[i].GetField("delta").n;

                            receivedInventory.items.Add(item);
                        }
                    }

                    receivedInventory.offset = (int) inventoryJSON.GetField("offset").n;
                    receivedInventory.logic = inventoryJSON.GetField("logic").str;
                }
            }

            if (responseEvent.data.HasField("gameState")) {
                gameStateData = responseEvent.data.GetField("gameState").Print();
            }

            if (responseEvent.data.HasField("externalChange")) {
                externalChange = responseEvent.data.GetField("externalChange").b;
            }

            if (responseEvent.data.HasField("deviceVersions")) {
                JSONObject deviceVersionsJSON = responseEvent.data.GetField("deviceVersions");
                userDataVersions = new List<UserDataVersion>();

                if (deviceVersionsJSON.Count > 0) {
                    for (int i = 0; i < deviceVersionsJSON.Count; i++) {
                        UserDataVersion userDataVersion = new UserDataVersion();
                        userDataVersion.deviceId = deviceVersionsJSON.keys[i];
                        userDataVersion.version = (int) deviceVersionsJSON.list[i].i;

                        userDataVersions.Add(userDataVersion);
                    }
                }
            }

            if (responseEvent.data.HasField("metaData")) {
                JSONObject metaDataJSON = responseEvent.data.GetField("metaData");

                userDataMeta = new UserDataMeta();

                if (metaDataJSON.HasField("serverTime")) {
                    userDataMeta.serverTime = metaDataJSON.GetField("serverTime").i;
                }

                if (metaDataJSON.HasField("clientTime")) {
                    userDataMeta.clientTime = metaDataJSON.GetField("clientTime").i;
                }

                if (metaDataJSON.HasField("timezoneOffset")) {
                    userDataMeta.timezoneOffset = (int) metaDataJSON.GetField("timezoneOffset").i;
                }

                if (metaDataJSON.HasField("deviceModel")) {
                    userDataMeta.deviceModel = metaDataJSON.GetField("deviceModel").str;
                }

                if (metaDataJSON.HasField("appVersion")) {
                    userDataMeta.appVersion = metaDataJSON.GetField("appVersion").str;
                }
            }

            if (responseEvent.action.ToLower().Trim().Equals("request")) {
                UserDataManager.ProcessRequestUserData(receivedWallet, receivedInventory, externalChange, gameStateData, userDataVersions, userDataMeta);
            } else if (responseEvent.action.ToLower().Trim().Equals("mergeconflict")) {
                UserDataManager.ProcessMergeConflict(receivedWallet, receivedInventory, gameStateData, userDataVersions, userDataMeta);
            } else if (responseEvent.action.ToLower().Trim().Equals("merge")) {
                UserDataManager.ProcessMergeUserData(userDataVersions);
            } else if (responseEvent.action.ToLower().Trim().Equals("lockerror")) {
                UserDataManager.ProcessLockError();
            } else if (responseEvent.action.ToLower().Trim().Equals("syncerror")) {
                UserDataManager.ProcessSyncError();
            }
        }
    }

    public class UserDataVersion {
        public string deviceId;
        public int version;
    }

    public class UserDataMeta {
        public long serverTime;
        public long clientTime;
        public int timezoneOffset;
        public string deviceModel;
        public string appVersion;
    }
}
#endif