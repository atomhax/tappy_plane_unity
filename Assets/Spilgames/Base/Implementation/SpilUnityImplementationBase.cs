using System.Collections.Generic;
using System;
using UnityEngine;
using SpilGames.Unity.Helpers.GameData;
using System.Collections;
using SpilGames.Unity.Json;
using SpilGames.Unity.Base.SDK;
using SpilGames.Unity.Helpers.IAPPackages;
using SpilGames.Unity.Helpers.Promotions;
using UnityEngine.Analytics;
using SpilGames.Unity.Helpers.EventParams;

namespace SpilGames.Unity.Base.Implementations{
    public abstract class SpilUnityImplementationBase{
        public static string PluginName = "Unity";
        public static string PluginVersion = "2.10.0";

        public static string AndroidVersion = "2.10.0";
        public static string iOSVersion = "2.10.0";

        #region Game config

        /// <summary>
        /// Method that returns the SLOT Config as a (user-defined) object. See the example in JSONHelper.cs for
        /// more information on how to turn JSON strings (such as the SLOT game config) into classes.
        /// This method currently does not catch and handle any exceptions and will not present the developer with
        /// handy tips for fixing his/her JSON or class. Developers will have to make due with the standard
        /// exceptions for now for debugging their JSON and classes. This may be improved in the future.
        /// </summary>
        /// <typeparam name="T">The user-defined class that mirrors the shape of the data in the JSON</typeparam>
        /// <returns></returns>
        public T GetConfig<T>() where T : new() {
            return JsonHelper.getObjectFromJson<T>(GetConfigAll());
        }

        #endregion

        #region Packages

        /// <summary>
        /// Fetch packages locally. Packages are requested from the server when the app starts and are cached.
        /// </summary>
        /// <returns>A packageshelper object filled with packages, will be empty if there are none. Returns null if no packages are present, which should only happen if the server has never been successfully queried for packages.</returns>
        public PackagesHelper GetPackages() {
            string packagesString = GetAllPackages();

            if (packagesString == null)
                return null;

            List<PackageData> packagesList = JsonHelper.getObjectFromJson<List<PackageData>>(packagesString);
            PackagesHelper helper = new PackagesHelper(packagesList);
            return helper;
        }

        public delegate void PackagesAvailable();

        /// <summary>
        /// This event indicates that the IAP packages can be retrieved.
        /// </summary>
        public event PackagesAvailable OnPackagesAvailable;

        public static void firePackagesAvailable() {
            Debug.Log("SpilSDK-Unity firePackagesAvailable");

            if (Spil.Instance.OnPackagesAvailable != null) {
                Spil.Instance.OnPackagesAvailable();
            }
        }
        
        public delegate void PackagesNotAvailable();

        /// <summary>
        /// This event indicates that the IAP packages are not available in the SDK.
        /// </summary>
        public event PackagesNotAvailable OnPackagesNotAvailable;

        public static void firePackagesNotAvailable() {
            Debug.Log("SpilSDK-Unity firePackagesNotAvailable");

            if (Spil.Instance.OnPackagesNotAvailable != null) {
                Spil.Instance.OnPackagesNotAvailable();
            }
        }
        
        #endregion

        #region Promotions

        public PromotionsHelper GetPromotions() {
            string promotionsString = GetAllPromotions();

            if (promotionsString == null)
                return null;

            List<SpilPromotionData> promotionsList = JsonHelper.getObjectFromJson<List<SpilPromotionData>>(promotionsString);
            PromotionsHelper helper = new PromotionsHelper(promotionsList);
            return helper;
        }

        public delegate void PromotionsAvailable();

        /// <summary>
        /// This event indicates that the promotions for IAP and In-Game items can be retrieved.
        /// </summary>
        public event PromotionsAvailable OnPromotionsAvailable;

        public static void firePromotionsAvailable() {
            Debug.Log("SpilSDK-Unity firePromotionsAvailable");

            if (Spil.GameData != null) {
                Spil.GameData.SpilGameDataHandler();
            }
            
            if (Spil.Instance.OnPromotionsAvailable != null) {
                Spil.Instance.OnPromotionsAvailable();
            }
        }
        
        public delegate void PromotionsNotAvailable();

        /// <summary>
        /// This event indicates that the promotions for IAP and In-Game items are not available in the SDK.
        /// </summary>
        public event PromotionsNotAvailable OnPromotionsNotAvailable;

        public static void firePromotionsNotAvailable() {
            Debug.Log("SpilSDK-Unity firePromotionsNotAvailable");

            if (Spil.Instance.OnPromotionsNotAvailable != null) {
                Spil.Instance.OnPromotionsNotAvailable();
            }
        }


        public delegate void PromotionAmountBought(int promotionId, int currentAmount, bool maxAmountReached);
        
        /// <summary>
        /// This event informs the updated status of a promotion when something is bought.
        /// </summary>
        public event PromotionAmountBought OnPromotionAmountBought;
        
        public static void firePromotionAmountBought(string data) {
            Debug.Log("SpilSDK-Unity firePromotionAmountBought with data: " + data);

            JSONObject promotionInfo = new JSONObject(data);
            int promotionId = (int) promotionInfo.GetField("promotionId").i;
            int currentAmount = (int) promotionInfo.GetField("amountPurchased").i;
            bool maxAmountReached = promotionInfo.GetField("maxAmountReached").b;
            
            if (Spil.Instance.OnPromotionAmountBought != null) {
                Spil.Instance.OnPromotionAmountBought(promotionId, currentAmount, maxAmountReached);
            }
        }
        
        #endregion
        
        #region Events

        #region Standard Spil events

        /// <summary>
        /// Sends the "milestoneAchieved" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="name"></param>
        public void TrackMilestoneAchievedEvent(string name, string milestoneDescription = null, float score = -1, string location = null, int iteration = 0) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            
            dict.Add("name", name);
            
            if (milestoneDescription != null) {
                dict.Add("milestoneDescription", milestoneDescription);
            }
            
            if (score != -1) {
                dict.Add("score", score);
            }
            
            if (location != null) {
                dict.Add("location", location);
            }
            
            if (iteration != 0) {
                dict.Add("iteration", iteration);
            }
            
            SendCustomEvent("milestoneAchieved", dict);
        }

        /// <summary>
        /// Sends the "levelStart" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="levelName">Level name.</param>
        /// <param name="difficulty">The difficulty of the started level.</param>
        /// <param name="customCreated">If set to <c>true</c> custom created.</param>
        /// <param name="creatorId">Creator identifier.</param>
        public void TrackLevelStartEvent(string levelName, string difficulty = null, bool customCreated = false,
            string creatorId = null, List<String> activeBooster = null) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("level", levelName);

            if (difficulty != null) {
                dict.Add("difficulty", difficulty);
            }

            if (customCreated) {
                dict.Add("customCreated", customCreated);
            }

            if (creatorId != null) {
                dict.Add("creatorId", creatorId);
            }

            if (activeBooster != null) {
                dict.Add("activeBooster", new JSONObject(JsonHelper.getJSONFromObject(activeBooster)));
            }

            SendCustomEvent("levelStart", dict);
        }

        /// <summary>
        /// Sends the "levelComplete" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="levelName">Level name.</param>
        /// <param name="difficulty">The difficulty of the started level.</param>
        /// <param name="score">Score.</param>
        /// <param name="stars">Stars.</param>
        /// <param name="speed">The speed in which the level was completed.</param>
        /// <param name="moves">The number of moves which it took the user to complete the level.</param>
        /// <param name="turns">Turns.</param>
        /// <param name="customCreated">If set to <c>true</c> custom created.</param>
        /// <param name="creatorId">Creator identifier.</param>
        /// <param name="objectUsed">A list of objects used.</param>
        public void TrackLevelCompleteEvent(string levelName, string difficulty = null, double score = 0, int stars = 0,
            string speed = null, int moves = 0, int turns = 0, bool customCreated = false, string creatorId = null, List<LevelCompleteObjectUsed> objectUsed = null, string levelId = null,
            string achievement = null, float avgCombos = 0, int movesLeft = -1, string rating = null, int timeLeft = -1) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("level", levelName);

            if (levelId != null) {
                dict.Add("levelId", levelId);
            }
            
            if (difficulty != null) {
                dict.Add("difficulty", difficulty);
            }

            if (score != 0) {
                dict.Add("score", score);
            }

            if (stars != 0) {
                dict.Add("stars", stars);
            }

            if (speed != null) {
                dict.Add("speed", speed);
            }

            if (moves != 0) {
                dict.Add("moves", moves);
            }

            if (turns != 0) {
                dict.Add("turns", turns);
            }

            if (customCreated) {
                dict.Add("customCreated", customCreated);
            }

            if (creatorId != null) {
                dict.Add("creatorId", creatorId);
            }

            if (objectUsed != null) {
                dict.Add("objectUsed", new JSONObject(JsonHelper.getJSONFromObject(objectUsed)));
            }

            if (achievement != null) {
                dict.Add("achievement", achievement);
            }

            if (avgCombos != 0) {
                dict.Add("avgCombos", avgCombos);
            }

            if (movesLeft != -1) {
                dict.Add("movesLeft", movesLeft);
            }
            
            if (rating != null) {
                dict.Add("rating", rating);
            }
            
            if (timeLeft != -1) {
                dict.Add("timeLeft", timeLeft);
            }
            
            SendCustomEvent("levelComplete", dict);
        }

        /// <summary>
        /// Sends the "levelFailed" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="levelName">Level name.</param>
        /// <param name="difficulty"></param>
        /// <param name="score">Score.</param>
        /// <param name="speed">The speed in which the level was failed.</param>
        /// <param name="moves">he number of moves which it took the user before he failed.</param>
        /// <param name="stars">Stars.</param>
        /// <param name="turns">Turns.</param>
        /// <param name="reason">The reason of failure</param>
        /// <param name="customCreated">If set to <c>true</c> custom created.</param>
        /// <param name="creatorId">Creator identifier.</param>
        public void TrackLevelFailedEvent(string levelName, string difficulty = null, double score = 0,
            string speed = null, int moves = 0, int stars = 0, int turns = 0, string reason = null,
            bool customCreated = false, string creatorId = null, List<LevelCompleteObjectUsed> objectUsed = null,
            string achievement = null, float avgCombos = 0, int movesLeft = -1, int timeLeft = -1) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("level", levelName);

            if (difficulty != null) {
                dict.Add("difficulty", difficulty);
            }

            if (score != 0) {
                dict.Add("score", score);
            }

            if (stars != 0) {
                dict.Add("stars", stars);
            }

            if (speed != null) {
                dict.Add("speed", speed);
            }

            if (moves != 0) {
                dict.Add("moves", moves);
            }

            if (turns != 0) {
                dict.Add("turns", turns);
            }

            if (reason != null) {
                dict.Add("reason", reason);
            }

            if (customCreated) {
                dict.Add("customCreated", customCreated);
            }

            if (creatorId != null) {
                dict.Add("creatorId", creatorId);
            }

            if (objectUsed != null) {
                dict.Add("objectUsed", new JSONObject(JsonHelper.getJSONFromObject(objectUsed)));
            }

            if (achievement != null) {
                dict.Add("achievement", achievement);
            }
            
            if (avgCombos != 0) {
                dict.Add("avgCombos", avgCombos);
            }
            
            if (timeLeft != -1) {
                dict.Add("timeLeft", timeLeft);
            }
            
            if (movesLeft != -1) {
                dict.Add("movesLeft", movesLeft);
            }
            
            SendCustomEvent("levelFailed", dict);
        }

        /// <summary>
        /// Sends the "levelUp" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="level">Level.</param>
        /// <param name="objectId">Object identifier.</param>
        /// <param name="skillId">Skill identifier.</param>
        public void TrackLevelUpEvent(string level, string objectId, string skillId = null, string sourceId = null, string sourceUniqueId = null, string objectUniqueId = null, string objectUniqueIdType = null) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("level", level);
            dict.Add("objectId", objectId);

            if (skillId != null) {
                dict.Add("skillId", skillId);
            }

            if (sourceId != null) {
                dict.Add("sourceId", sourceId);
            }
            
            if (sourceUniqueId != null) {
                dict.Add("sourceUniqueId", sourceUniqueId);
            }
            
            if (objectUniqueId != null) {
                dict.Add("objectUniqueId", objectUniqueId);
            }
            
            if (objectUniqueIdType != null) {
                dict.Add("objectUniqueIdType", objectUniqueIdType);
            }
            
            SendCustomEvent("levelUp", dict);
        }

        /// <summary>
        /// Sends the "equip" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="equippedItem">Equipped item.</param>
        /// <param name="equippedTo">Equipped to.</param>
        /// <param name="unequippedFrom">The character/object which unequipped the item.</param>
        public void TrackEquipEvent(string equippedItem, string equippedTo = null, string unequippedFrom = null) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("equippedItem", equippedItem);

            if (equippedTo != null) {
                dict.Add("equippedTo", equippedTo);
            }

            if (unequippedFrom != null) {
                dict.Add("unequippedFrom", unequippedFrom);
            }

            SendCustomEvent("equip", dict);
        }

        /// <summary>
        /// Sends the "equip" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="upgradeId">Upgrade identifier.</param>
        /// <param name="level">Level.</param>
        /// <param name="reason">Reason.</param>
        /// <param name="iteration">Iteration.</param>
        public void TrackUpgradeEvent(string upgradeId, string level, string reason = null, int iteration = 0) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("upgradeId", upgradeId);
            dict.Add("level", level);

            if (reason != null) {
                dict.Add("reason", reason);
            }

            if (iteration != 0) {
                dict.Add("iteration", iteration);
            }

            SendCustomEvent("upgrade", dict);
        }

        /// <summary>
        /// Sends the "levelCreate" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="levelId">Level identifier.</param>
        /// <param name="creatorId">Creator identifier.</param>
        public void TrackLevelCreateEvent(string levelId, string level, string creatorId) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("levelId", levelId);
            dict.Add("level", level);
            dict.Add("creatorId", creatorId);

            SendCustomEvent("levelCreate", dict);
        }

        /// <summary>
        /// Sends the "levelDownload" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="levelId">Level identifier.</param>
        /// <param name="creatorId">Creator identifier.</param>
        /// <param name="rating">Rating.</param>
        public void TrackLevelDownloadEvent(string levelId, string creatorId, double rating = 0) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("levelId", levelId);
            dict.Add("creatorId", creatorId);

            if (rating != 0) {
                dict.Add("rating", rating);
            }

            SendCustomEvent("levelDownload", dict);
        }

        /// <summary>
        /// Sends the "levelRate" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="levelId">Level identifier.</param>
        /// <param name="creatorId">Creator identifier.</param>
        /// <param name="rating">Rating.</param>
        public void TrackLevelRateEvent(string levelId, string creatorId, double rating = 0) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("levelId", levelId);
            dict.Add("creatorId", creatorId);

            if (rating != 0) {
                dict.Add("rating", rating);
            }

            SendCustomEvent("levelRate", dict);
        }

        /// <summary>
        /// Sends the "endlessModeStart" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        public void TrackEndlessModeStartEvent() {
            SendCustomEvent("endlessModeStart");
        }

        /// <summary>
        /// Sends the "endlessModeEnd" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="distance">Distance.</param>
        public void TrackEndlessModeEndEvent(int distance) {
            SendCustomEvent("endlessModeEnd", new Dictionary<string, object>() {
                {
                    "distance",
                    distance
                }
            });
        }

        /// <summary>
        /// Sends the "playerDies" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="levelName"></param>
        public void TrackPlayerDiesEvent(string levelName) {
            SendCustomEvent("playerDies", new Dictionary<string, object>() {
                {
                    "level",
                    levelName
                }
            });
        }

        /// <summary>
        /// Sends the "updatePlayerData" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="reason">The reason for which the event occured. You can also use the PlayerDataUpdateReasons class to pass one of the default reasons</param>
        /// <param name="location">The location where the event occured (ex.: Shop Screen, End of the level)</param>
        /// <param name="reasonDetails">Additional parameter used to describe the details of why the event happened</param>
        /// <param name="transactionId">The transactionId if the update was due to an IAP</param>
        /// <param name="currencyList">A list of TrackingCurrency objects that defines all the currencies that have been changed with this event. This parameter can also be omited if no currencies have been updated</param>
        /// <param name="itemsList">A list of TrackingItems objects that defines all the items that have been changed with this event. This parameter can also be omited if no items have been updated</param>
        public void TrackWalletInventoryEvent(string reason, string location,
            List<TrackingCurrency> currencyList = null, List<TrackingItem> itemsList = null,
            string reasonDetails = null, string transactionId = null) {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            if (currencyList != null) {
                Dictionary<string, object> wallet = new Dictionary<string, object>();
                wallet.Add("currencies", new JSONObject(JsonHelper.getJSONFromObject(currencyList)));
                wallet.Add("offset", 0);
                dictionary.Add("wallet", wallet);
            }

            if (itemsList != null) {
                Dictionary<string, object> inventory = new Dictionary<string, object>();
                inventory.Add("items", new JSONObject(JsonHelper.getJSONFromObject(itemsList)));
                inventory.Add("offset", 0);
                dictionary.Add("inventory", inventory);
            }

            dictionary.Add("reason", reason);
            dictionary.Add("location", location);
            dictionary.Add("trackingOnly", true);

            if (reasonDetails != null) {
                dictionary.Add("reasonDetails", reasonDetails);
            }

            if (transactionId != null) {
                dictionary.Add("transactionId", transactionId);
            }

            SendCustomEvent("updatePlayerData", dictionary);
        }

        /// <summary>
        /// Sends the "iapPurchased" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="skuId">The product identifier of the item that was purchased</param>
        /// <param name="transactionId ">The transaction identifier of the item that was purchased (also called orderId)</param>
        public void TrackIAPPurchasedEvent(string skuId, string transactionId, string token = "", string reason = null, string location = null) {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("skuId", skuId);
            dictionary.Add("transactionId", transactionId);
            dictionary.Add("purchaseDate", DateTime.Now.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"));

#if UNITY_ANDROID
            if (token != null && !token.Equals("")) {
                dictionary.Add("token", token);
            }
#endif

            if (reason != null) {
                dictionary.Add("reason", reason);
            }

            if (location != null) {
                dictionary.Add("location", location);
            }

            SendCustomEvent("iapPurchased", dictionary);
        }

        /// <summary>
        /// Sends the "iapRestored" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="skuId">The product identifier of the item that was purchased</param>
        /// <param name="originalTransactionId ">For a transaction that restores a previous transaction, the transaction identifier of the original transaction. Otherwise, identical to the transaction identifier</param>
        /// <param name="originalPurchaseDate">For a transaction that restores a previous transaction, the date of the original transaction. Please use a proper DateTime format (RFC3339), for instance: "2016-08-30T11:54:48.5247936+02:00". If you have a DateTime object you can use: DateTimeObject.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz")</param>                
        public void TrackIAPRestoredEvent(string skuId, string originalTransactionId, string originalPurchaseDate, string reason = null) {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            
            dictionary.Add("skuId", skuId);
            dictionary.Add("originalTransactionId", originalTransactionId);
            dictionary.Add("originalPurchaseDate", originalPurchaseDate);
            
            if (reason != null) {
                dictionary.Add("reason", reason);
            }
            
            SendCustomEvent("iapRestored", dictionary);
        }

        /// <summary>
        /// Sends the "iapFailed" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="error">Error description or error code</param>
        /// <param name="skuId">The product identifier of the item that was purchased</param>
        public void TrackIAPFailedEvent(string error, string skuId, string location = null, string reason = null) {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            
            dictionary.Add("errorDescription", error);
            dictionary.Add("skuId", skuId);
            dictionary.Add("purchaseDate", DateTime.Now.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"));

            if (location != null) {
                dictionary.Add("location", location);
            }
            
            if (reason != null) {
                dictionary.Add("reason", reason);
            }
            
            SendCustomEvent("iapFailed", dictionary);
        }

        /// <summary>
        /// Sends the "tutorialComplete" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        public void TrackTutorialCompleteEvent() {
            SendCustomEvent("tutorialComplete");
        }

        /// <summary>
        /// Sends the "tutorialSkipped" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        public void TrackTutorialSkippedEvent() {
            SendCustomEvent("tutorialSkipped");
        }

        /// <summary>
        /// Sends the "tutorialSkipped" event to the native Spil SDK which will send a request to the back-end.
        /// Should be called after the user completes registration via email, Facebook, Google Plus or other available option. Registration option is assumed.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="platform">A string like ‘facebook’ or ’email’</param>
        public void TrackRegisterEvent(string platform) {
            SendCustomEvent("register", new Dictionary<string, object>() {
                {
                    "platform",
                    platform
                }
            });
        }

        /// <summary>
        /// Sends the "share" event to the native Spil SDK which will send a request to the back-end.
        /// Should be called every time the user shares content on their social media accounts. Social media integration is assumed.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="platform">A string like ‘facebook’ or ’email’</param>
        public void TrackShareEvent(string platform) {
            SendCustomEvent("share", new Dictionary<string, object>() {
                {
                    "platform",
                    platform
                }
            });
        }

        /// <summary>
        /// Sends the "invite" event to the native Spil SDK which will send a request to the back-end.
        /// Should be called every time the user invites another user. Respective function in-game is assumed.
        /// See https://github.com/spilgames/spil_event_unity_plugin for more information on events.
        /// </summary>
        /// <param name="platform">A string like ‘facebook’ or ’email’</param>
        public void TrackInviteEvent(string platform, string location = null) {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("platform", platform);

            if (location != null) {
                dictionary.Add("location", location);
            }
            
            SendCustomEvent("invite", dictionary);
        }

        /// <summary>
        /// Sends the "levelAppeared" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="level">The name/id of the level that appeared.</param>
        /// <param name="difficulty">The difficulty of the level that appeared</param>
        public void TrackLevelAppeared(string level, string difficulty = null) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("level", level);

            if (difficulty != null) {
                dict.Add("difficulty", difficulty);
            }

            SendCustomEvent("levelAppeared", dict);
        }

        /// <summary>
        /// Sends the "levelDiscarded" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="level">The name/id of the level that appeared.</param>
        /// <param name="difficulty">The difficulty of the level that appeared</param>
        public void TrackLevelDiscarded(string level, string difficulty = null) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("level", level);

            if (difficulty != null) {
                dict.Add("difficulty", difficulty);
            }

            SendCustomEvent("levelDiscarded", dict);
        }

        /// <summary>
        /// Sends the "errorShown" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="reason">The reason for the error to be shown.</param>
        public void TrackErrorShown(string reason) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("reason", reason);

            SendCustomEvent("errorShown", dict);
        }

        /// <summary>
        /// Sends the "timeElapLoad" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="timeElap">The time elapsed between the starting point and the pointInGame in seconds.</param>
        /// <param name="pointInGame">The point in game which is reached.</param>
        /// <param name="startPoint">The point in game which we start to measure time.</param>
        public void TrackTimeElapLoad(int timeElap, string pointInGame, string startPoint = null) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("timeElap", timeElap);
            dict.Add("pointInGame", pointInGame);

            if (startPoint != null) {
                dict.Add("startPoint", startPoint);
            }

            SendCustomEvent("timeElapLoad", dict);
        }

        /// <summary>
        /// Sends the "timeoutDetected" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="timeElap">The time elapsed between the starting point and the pointInGame in seconds.</param>
        /// <param name="pointInGame">The point in game which is reached.</param>
        public void TrackTimeoutDetected(int timeElap, string pointInGame) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("timeElap", timeElap);
            dict.Add("pointInGame", pointInGame);

            SendCustomEvent("timeoutDetected", dict);
        }

        /// <summary>
        /// Sends the "objectStateChanged" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="changedObject">The object which changed it's state.</param>
        /// <param name="status">The new status the object is in.</param>
        /// <param name="reason">The reason for the state change.</param>
        /// <param name="changedProperties">The property/properties which have changed.</param>
        public void TrackObjectStateChanged(string changedObject, string status, string reason = null,
            string changedProperties = null) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("changedObject", changedObject);
            dict.Add("status", status);

            if (reason != null) {
                dict.Add("reason", reason);
            }

            if (changedProperties != null) {
                dict.Add("changedProperties", changedProperties);
            }

            SendCustomEvent("objectStateChanged", dict);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="type"></param>
        /// <param name="screenName"></param>
        /// <param name="location"></param>
        /// <param name="grade"></param>
        public void TrackUIElementClicked(string element, string type = null, string screenName = null, string location = null, int grade = 0) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("element", element);

            if (type != null) {
                dict.Add("type", type);
            }

            if (screenName != null) {
                dict.Add("screenName", screenName);
            }
            
            if (location != null) {
                dict.Add("location", location);
            }
            
            if (grade != 0) {
                dict.Add("grade", grade);
            }

            SendCustomEvent("uiElementClicked", dict);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="location"></param>
        public void TrackSendGift(string platform, string location = null) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("platform", platform);
           
            if (location != null) {
                dict.Add("location", location);
            }

            SendCustomEvent("sendGift", dict);
        }

        /// <summary>
        /// 
        /// </summary>
        public void TrackLevelTimeOut() {
            SendCustomEvent("levelTimeOut");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="hasToken"></param>
        /// <param name="isPremiumChoice"></param>
        /// <param name="isQuizz"></param>
        /// <param name="isForced"></param>
        /// <param name="isTimed"></param>
        /// <param name="time"></param>
        public void TrackDialogChosen(string name, bool hasToken, bool isPremiumChoice, bool isQuizz, bool isForced, bool isTimed, int time = 0) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("name", name);
            dict.Add("hasToken", hasToken);
            dict.Add("isPremiumChoice", isPremiumChoice);
            dict.Add("isQuizz", isQuizz);
            dict.Add("isForced", isForced);
            dict.Add("isTimed", isTimed);
           
            if (time != 0) {
                dict.Add("time", time);
            }

            SendCustomEvent("dialogueChosen", dict);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="friend"></param>
        /// <param name="platform"></param>
        public void TrackFriendAdded(string friend, string platform = null) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("friend", friend);
           
            if (platform != null) {
                dict.Add("platform", platform);
            }

            SendCustomEvent("friendAdded", dict);
        }

        /// <summary>
        /// 
        /// </summary>
        public void TrackGameObjectInteraction() {
            SendCustomEvent("gameObjectInteraction");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="itemType"></param>
        /// <param name="label"></param>
        /// <param name="matchId"></param>
        public void TrackGameResult(string itemId = null, string itemType = null, string label = null, string matchId = null) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
           
            if (itemId != null) {
                dict.Add("itemId", itemId);
            }

            if (itemType != null) {
                dict.Add("itemType", itemType);
            }
            
            if (label != null) {
                dict.Add("label", label);
            }
            
            if (matchId != null) {
                dict.Add("matchId", matchId);
            }
            
            SendCustomEvent("gameResult", dict);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="itemType"></param>
        public void TrackItemCrafted(string itemId, string itemType = null) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("itemId", itemId);
           
            if (itemType != null) {
                dict.Add("itemType", itemType);
            }

            SendCustomEvent("itemCrafted", dict);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="itemType"></param>
        public void TrackItemCreated(string itemId, string itemType = null) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("itemId", itemId);
           
            if (itemType != null) {
                dict.Add("itemType", itemType);
            }

            SendCustomEvent("itemCreated", dict);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="itemId"></param>
        /// <param name="itemType"></param>
        public void TrackItemUpdated(string content, string itemId, string itemType = null) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("content", content);
            dict.Add("itemId", itemId);
           
            if (itemType != null) {
                dict.Add("itemType", itemType);
            }

            SendCustomEvent("itemUpdated", dict);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="itemId"></param>
        /// <param name="itemType"></param>
        /// <param name="label"></param>
        public void TrackDeckUpdated(string content, string itemId, string itemType = null, string label = null) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("content", content);
            dict.Add("itemId", itemId);
           
            if (itemType != null) {
                dict.Add("itemType", itemType);
            }

            if (label != null) {
                dict.Add("label", label);
            }
            
            SendCustomEvent("deckUpdated", dict);
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void TrackIAPPayingUser() {
            SendCustomEvent("iapPayingUser");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matchId"></param>
        /// <param name="itemId"></param>
        /// <param name="itemType"></param>
        /// <param name="label"></param>
        public void TrackMatchComplete(string matchId = null, string itemId = null, string itemType = null, string label = null) {
            Dictionary<string, object> dict = new Dictionary<string, object>();

            if (matchId != null) {
                dict.Add("matchId", matchId);
            }

            if (itemId != null) {
                dict.Add("itemId", itemId);
            }
           
            if (itemType != null) {
                dict.Add("itemType", itemType);
            }

            if (label != null) {
                dict.Add("label", label);
            }
            
            SendCustomEvent("matchComplete", dict);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="matchId"></param>
        /// <param name="itemId"></param>
        /// <param name="itemType"></param>
        /// <param name="label"></param>
        public void TrackMatchLost(string matchId = null, string itemId = null, string itemType = null, string label = null) {
            Dictionary<string, object> dict = new Dictionary<string, object>();

            if (matchId != null) {
                dict.Add("matchId", matchId);
            }

            if (itemId != null) {
                dict.Add("itemId", itemId);
            }
           
            if (itemType != null) {
                dict.Add("itemType", itemType);
            }

            if (label != null) {
                dict.Add("label", label);
            }
            
            SendCustomEvent("matchLost", dict);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="matchId"></param>
        /// <param name="itemId"></param>
        /// <param name="itemType"></param>
        /// <param name="label"></param>
        public void TrackMatchTie(string matchId = null, string itemId = null, string itemType = null, string label = null) {
            Dictionary<string, object> dict = new Dictionary<string, object>();

            if (matchId != null) {
                dict.Add("matchId", matchId);
            }

            if (itemId != null) {
                dict.Add("itemId", itemId);
            }
           
            if (itemType != null) {
                dict.Add("itemType", itemType);
            }

            if (label != null) {
                dict.Add("label", label);
            }
            
            SendCustomEvent("matchTie", dict);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="matchId"></param>
        /// <param name="itemId"></param>
        /// <param name="itemType"></param>
        /// <param name="label"></param>
        public void TrackMatchWon(string matchId = null, string itemId = null, string itemType = null, string label = null) {
            Dictionary<string, object> dict = new Dictionary<string, object>();

            if (matchId != null) {
                dict.Add("matchId", matchId);
            }

            if (itemId != null) {
                dict.Add("itemId", itemId);
            }
           
            if (itemType != null) {
                dict.Add("itemType", itemType);
            }

            if (label != null) {
                dict.Add("label", label);
            }
            
            SendCustomEvent("matchWon", dict);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="reason"></param>
        /// <param name="label"></param>
        /// <param name="delta"></param>
        /// <param name="energy"></param>
        /// <param name="kind"></param>
        /// <param name="location"></param>
        /// <param name="rarity"></param>
        public void TrackPawnMoved(string name, string reason = null, string label = null, string delta = null, string energy = null, string kind = null, string location = null, string rarity = null) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("name", name);
            
            if (reason != null) {
                dict.Add("reason", reason);
            }

            if (label != null) {
                dict.Add("label", label);
            }
           
            if (delta != null) {
                dict.Add("delta", delta);
            }

            if (energy != null) {
                dict.Add("energy", energy);
            }
            
            if (kind != null) {
                dict.Add("kind", kind);
            }
           
            if (location != null) {
                dict.Add("location", location);
            }

            if (rarity != null) {
                dict.Add("rarity", rarity);
            }
            
            SendCustomEvent("pawnMoved", dict);
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void TrackPlayerLeagueChanged() {
            SendCustomEvent("playerLeagueChanged");
        }

        /// <summary>
        /// 
        /// </summary>
        public void TrackPrepareWebPayments() {
            SendCustomEvent("prepareWebPayments");
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void TrackSignUpWithFacebook() {
            SendCustomEvent("signUpWithFacebook");
        }

        public void TrackSpinnedFortune(string spinResult, string objectId = null) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("spinResult", spinResult);
            
            if (objectId != null) {
                dict.Add("objectId", objectId);
            }
            
            SendCustomEvent("spinnedFortune", dict);
        }

        public void TrackTimedAction(string timedAction, string label = null, string timedObject = null, int timeToFinish = 0, float effectMultiplier = 0) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("timedAction", timedAction);
            
            if (label != null) {
                dict.Add("label", label);
            }
            
            if (timedObject != null) {
                dict.Add("timedObject", timedObject);
            }
            
            if (timeToFinish != 0) {
                dict.Add("timeToFinish", timeToFinish);
            }
            
            if (effectMultiplier != 0) {
                dict.Add("effectMultiplier", effectMultiplier);
            }
            
            SendCustomEvent("timedAction", dict);
        }

        public void TrackTransitionToGame(string type) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("type", type);
            
            SendCustomEvent("transitionToGame", dict);
        }
        
        public void TrackTransitionToMenu() {
            SendCustomEvent("transitionToMenu");
        }
        
        #endregion

        #region Advertisement events

        /// <summary>
        /// This is fired by the native Spil SDK after it receives a response from the back-end.
        /// This method is exposed only for use by the native Spil SDK and should not be used by the developer!
        /// </summary>
        /// <param name="response"></param>
        public static void OnResponseReceived(string response) {
            Debug.Log("SpilSDK-Unity OnResponseReceived " + response);

            SpilResponse spilResponse = JsonHelper.getObjectFromJson<SpilResponse>(response);

            if (!spilResponse.type.ToLower().Trim().Equals("notificationreward")) return;
            PushNotificationRewardResponse rewardResponseData =
                JsonHelper.getObjectFromJson<PushNotificationRewardResponse>(response);
            fireOnRewardEvent(rewardResponseData);
        }

        public delegate void RewardEvent(PushNotificationRewardResponse rewardResponse);

        /// <summary>
        /// This is fired by the Unity Spil SDK after it receives a "Reward" response from the back-end.
        /// The developer can subscribe to this event to assign the reward and update the UI.
        /// This event should no longer be used, reward data is now passed as a parameter of the AdFinished event so please use that instead.
        /// </summary>
        public event RewardEvent OnReward;

        /// <summary>
        /// This is fired by the Unity Spil SDK after it receives a "Reward" response from the back-end.
        /// </summary>
        private static void fireOnRewardEvent(PushNotificationRewardResponse rewardResponse) {
            if (Spil.Instance.OnReward != null) {
                Spil.Instance.OnReward(rewardResponse);
            }
        }

        public delegate void AdAvailableEvent(enumAdType adType);

        /// <summary>
        /// This is fired by the native Spil SDK after it receives an "AdAvailable" response from the back-end.
        /// The developer can subscribe to this event and for instance call "Spil.Instance.PlayVideo();" or "Spil.Instance.PlayMoreApps()"
        /// </summary>
        public event AdAvailableEvent OnAdAvailable;

        /// <summary>
        /// This is called by the native Spil SDK and will fire an OnAdAvailable event to which the developer 
        /// can subscribe and for instance call "Spil.Instance.PlayVideo();" or "Spil.Instance.PlayMoreApps()"
        /// This method is exposed only for use by the native Spil SDK and should not be used by the developer!
        /// </summary>
        /// <param name="type"></param>
        public static void fireAdAvailableEvent(string type) {
            Debug.Log("SpilSDK-Unity Ad " + type + " ready!");

            enumAdType adType = enumAdType.Unknown;
            if (type.ToLower().Trim().Equals("rewardvideo")) {
                adType = enumAdType.RewardVideo;
            } else if (type.ToLower().Trim().Equals("interstitial")) {
                adType = enumAdType.Interstitial;
            } else if (type.ToLower().Trim().Equals("moreapps")) {
                adType = enumAdType.MoreApps;
            }
            if (adType == enumAdType.Unknown) {
                Debug.Log("SpilSDK-Unity AdAvailable event fired but type is unknown. Type: " + type);
            }
            if (Spil.Instance.OnAdAvailable != null) {
                Spil.Instance.OnAdAvailable(adType);
            }
        }

        public delegate void AdNotAvailableEvent(enumAdType adType);

        /// <summary>
        /// This is fired by the native Spil SDK after it receives an "AdNotAvailable" response from the back-end.
        /// The developer can subscribe to this event and for instance NOT call "Spil.Instance.PlayVideo();" or "Spil.Instance.PlayMoreApps()"
        /// but do something else instead.
        /// </summary>
        public event AdNotAvailableEvent OnAdNotAvailable;

        /// <summary>
        /// This is called by the native Spil SDK and will fire an OnAdNotAvailable event to which the developer 
        /// can subscribe and for instance NOT call "Spil.Instance.PlayVideo();" or "Spil.Instance.PlayMoreApps()" but do something else instead.
        /// This method is exposed only for use by the native Spil SDK and should not be used by the developer!
        /// </summary>
        /// <param name="type"></param>
        public static void fireAdNotAvailableEvent(string type) {
            Debug.Log("SpilSDK-Unity Ad " + type + " is not available");

            enumAdType adType = enumAdType.Unknown;
            if (type.ToLower().Trim().Equals("rewardvideo")) {
                adType = enumAdType.RewardVideo;
            } else if (type.ToLower().Trim().Equals("interstitial")) {
                adType = enumAdType.Interstitial;
            } else if (type.ToLower().Trim().Equals("moreapps")) {
                adType = enumAdType.MoreApps;
            }
            if (adType == enumAdType.Unknown) {
                Debug.Log("SpilSDK-Unity AdNotAvailable event fired but type is unknown. Type: " + type);
            }
            if (Spil.Instance.OnAdNotAvailable != null) {
                Spil.Instance.OnAdNotAvailable(adType);
            }
        }

        public delegate void OpenParentalGateEvent();

        /// <summary>
        /// This is fired by the native Spil SDK after it receives an "OpenParentalGate" response from the back-end.
        /// The developer can subscribe to this event and for instance NOT call "Spil.Instance.PlayVideo();" or "Spil.Instance.PlayMoreApps()"
        /// but do something else instead.
        /// </summary>
        public event OpenParentalGateEvent OnOpenParentalGate;

        /// <summary>
        /// This is called by the native Spil SDK and will fire an OpenParentalGate event to which the developer 
        /// can subscribe and for instance NOT call "Spil.Instance.PlayVideo();" or "Spil.Instance.PlayMoreApps()" but do something else instead.
        /// This method is exposed only for use by the native Spil SDK and should not be used by the developer!
        /// </summary>
        /// <param name="type"></param>
        public static void fireOpenParentalGateEvent() {
            Debug.Log("SpilSDK-Unity OpenParentalGate");

            if (Spil.Instance.OnOpenParentalGate != null) {
                Spil.Instance.OnOpenParentalGate();
            }
        }

        public delegate void AdStartedEvent();

        /// <summary>
        /// This is fired by the native Spil SDK after a video is started.
        /// The developer can subscribe to this event and for instance disable the in-game sound.
        /// </summary>
        public event AdStartedEvent OnAdStarted;

        /// <summary>
        /// This is called by the native Spil SDK and will fire an OnAdStarted event to which the developer 
        /// can subscribe and for instance disable the in-game sound.
        /// This method is exposed only for use by the native Spil SDK and should not be used by the developer!
        /// </summary>
        public static void fireAdStartedEvent() {
            Debug.Log("SpilSDK-Unity Ad started");
            if (Spil.Instance.OnAdStarted != null) {
                Spil.Instance.OnAdStarted();
            }
        }

        public delegate void AdFinishedEvent(SpilAdFinishedResponse response);

        /// <summary>
        /// This is fired by the native Spil SDK after a video has finished playing.
        /// The developer can subscribe to this event and for instance re-enable the in-game sound.
        /// </summary>
        public event AdFinishedEvent OnAdFinished;

        /// <summary>
        /// This is called by the native Spil SDK and will fire an OnAdFinished event to which the developer 
        /// can subscribe and for instance re-enable the in-game sound.
        /// This method is exposed only for use by the native Spil SDK and should not be used by the developer!
        /// </summary>
        public static void fireAdFinishedEvent(string response) {
            Debug.Log("SpilSDK-Unity Ad finished! Response = " + response);
            SpilAdFinishedResponse responseObject = JsonHelper.getObjectFromJson<SpilAdFinishedResponse>(response);
            if (Spil.Instance.OnAdFinished != null) {
                Spil.Instance.OnAdFinished(responseObject);
            }
        }

        #endregion

        public delegate void ConfigUpdatedEvent();

        /// <summary>
        /// This is fired by the native Spil SDK when the config was updated.
        /// The developer can subscribe to this event and for instance re-enable the in-game sound.
        /// </summary>
        public event ConfigUpdatedEvent OnConfigUpdated;

        /// <summary>
        /// This is called by the native Spil SDK and will fire an ConfigUpdated event to which the developer 
        /// can subscribe, it will only be called when the config values are different from the previous loaded config.
        /// </summary>
        public static void fireConfigUpdatedEvent() {
            Debug.Log("SpilSDK-Unity Config updated!");
            if (Spil.Instance.OnConfigUpdated != null) {
                Spil.Instance.OnConfigUpdated();
            }
        }

        public delegate void ConfigError(SpilErrorMessage errorMessage);

        /// <summary>
        /// This is fired by the native Spil SDK when the config was updated.
        /// The developer can subscribe to this event and for instance re-enable the in-game sound.
        /// </summary>
        public event ConfigError OnConfigError;

        /// <summary>
        /// This is called by the native Spil SDK and will fire an ConfigUpdated event to which the developer 
        /// can subscribe, it will only be called when the config values are different from the previous loaded config.
        /// </summary>
        public static void fireConfigError(string error) {
            Debug.Log("SpilSDK-Unity Config Error with message: " + error);

            SpilErrorMessage errorMessage = JsonHelper.getObjectFromJson<SpilErrorMessage>(error);
            if (Spil.Instance.OnConfigError != null) {
                Spil.Instance.OnConfigError(errorMessage);
            }
        }

        #endregion

        #region Spil Game Data

        public delegate void SpilGameDataAvailable();

        /// <summary>
        /// This is fired by the native Spil SDK after game data has been received from the server.
        /// The developer can subscribe to this event and then request the Spil Game Data.
        /// </summary>
        public event SpilGameDataAvailable OnSpilGameDataAvailable;

        public static void fireSpilGameDataAvailable() {
            if (Spil.GameData != null) {
                Spil.GameData.SpilGameDataHandler();
            }

            Debug.Log("SpilSDK-Unity Spil Game Data is available");

            if (Spil.Instance.OnSpilGameDataAvailable != null) {
                Spil.Instance.OnSpilGameDataAvailable();
            }
        }

        public delegate void SpilGameDataError(SpilErrorMessage errorMessage);

        /// <summary>
        /// This is fired by the native Spil SDK after game data has failed to be retrieved.
        /// The developer can subscribe to this event and check the reason.
        /// </summary>
        public event SpilGameDataError OnSpilGameDataError;

        public static void fireSpilGameDataError(string reason) {
            Debug.Log("SpilSDK-Unity Spil Game Data error with reason = " + reason);

            SpilErrorMessage errorMessage = JsonHelper.getObjectFromJson<SpilErrorMessage>(reason);
            if (Spil.Instance.OnSpilGameDataError != null) {
                Spil.Instance.OnSpilGameDataError(errorMessage);
            }
        }

        #endregion

        #region Player Data

        public delegate void PlayerDataUpdated(string reason, PlayerDataUpdatedData updatedData);

        /// <summary>
        /// This is fired by the native Spil SDK after player data has been updated.
        /// The developer can subscribe to this event and then request the Player Data (Wallet & Inventory).
        /// </summary>
        public event PlayerDataUpdated OnPlayerDataUpdated;

        public static void firePlayerDataUpdated(string data) {
            Debug.Log("SpilSDK-Unity Player Data has been updated with data: " + data);

            PlayerDataUpdatedData playerDataUpdatedData = JsonHelper.getObjectFromJson<PlayerDataUpdatedData>(data);

            if (Spil.PlayerData != null) {
                Spil.PlayerData.PlayerDataUpdatedHandler();
            }


            if (Spil.Instance.OnPlayerDataUpdated != null) {
                Spil.Instance.OnPlayerDataUpdated(playerDataUpdatedData.reason, playerDataUpdatedData);
            }
        }

        public delegate void PlayerDataEmptyGacha();

        /// <summary>
        /// This is fired by the native Spil SDK after player data has been received from the server.
        /// The developer can subscribe to this event and then request the Player Data (Wallet & Inventory).
        /// </summary>
        public event PlayerDataEmptyGacha OnPlayerDataEmptyGacha;

        public static void firePlayerDataEmptyGacha() {
            Debug.Log("SpilSDK-Unity Received nothing from gacha box!");

            if (Spil.Instance.OnPlayerDataEmptyGacha != null) {
                Spil.Instance.OnPlayerDataEmptyGacha();
            }
        }

        public delegate void GameStateUpdated(string access);

        /// <summary>
        /// This is fired by the native Spil SDK after game state was updated.
        /// The developer can subscribe to this event and check the reason.
        /// </summary>
        public event GameStateUpdated OnGameStateUpdated;

        public static void fireGameStateUpdated(string access) {
            Debug.Log("SpilSDK-Unity Game State Data updated, access = " + access);

            if (Spil.Instance.OnGameStateUpdated != null) {
                Spil.Instance.OnGameStateUpdated(access);
            }
        }

        public delegate void OtherUsersGameStateDataLoaded(OtherUsersGameStateData data);

        /// <summary>
        /// This is fired by the native Spil SDK after the game state data of other users was loaded.
        /// The developer can subscribe to this event and check the reason.
        /// </summary>
        public event OtherUsersGameStateDataLoaded OnOtherUsersGameStateDataLoaded;

        public static void fireOtherUsersGameStateLoaded(string message) {
            Debug.Log("SpilSDK-Unity Other users game state data loaded, message = " + message);

            OtherUsersGameStateData data = JsonHelper.getObjectFromJson<OtherUsersGameStateData>(message);

            if (Spil.Instance.OnOtherUsersGameStateDataLoaded != null) {
                Spil.Instance.OnOtherUsersGameStateDataLoaded(data);
            }
        }

        public delegate void SplashScreenOpen();

        /// <summary>
        /// This is fired by the native Spil SDK when the web view opened.
        /// </summary>
        public event SplashScreenOpen OnSplashScreenOpen;

        public static void fireSplashScreenOpen() {
            Debug.Log("SpilSDK-Unity Web open");

            if (Spil.Instance.OnSplashScreenOpen != null) {
                Spil.Instance.OnSplashScreenOpen();
            }
        }

        public delegate void SplashScreenNotAvailable();

        /// <summary>
        /// This is fired by the native Spil SDK when the splash screen is not available.
        /// </summary>
        public event SplashScreenNotAvailable OnSplashScreenNotAvailable;

        public static void fireSplashScreenNotAvailable() {
            Debug.Log("SpilSDK-Unity splash screen not available");

            if (Spil.Instance.OnSplashScreenNotAvailable != null) {
                Spil.Instance.OnSplashScreenNotAvailable();
            }
        }

        public delegate void SplashScreenClosed();

        /// <summary>
        /// This is fired by the native Spil SDK when the web view closes.
        /// </summary>
        public event SplashScreenClosed OnSplashScreenClosed;

        public static void fireSplashScreenClosed() {
            Debug.Log("SpilSDK-Unity Web closed");

            if (Spil.Instance.OnSplashScreenClosed != null) {
                Spil.Instance.OnSplashScreenClosed();
            }
        }

        public delegate void SplashScreenOpenShop();

        /// <summary>
        /// This is fired by the native Spil SDK when the game shop should be opened.
        /// The developer can subscribe to this event and open there own shop implementation.
        /// </summary>
        public event SplashScreenOpenShop OnSplashScreenOpenShop;

        public static void fireSplashScreenOpenShop() {
            Debug.Log("SpilSDK-Unity Open Game Shop");

            if (Spil.Instance.OnSplashScreenOpenShop != null) {
                Spil.Instance.OnSplashScreenOpenShop();
            }
        }

        public delegate void SplashScreenData(string payload);

        /// <summary>
        /// This is fired by the native Spil SDK in order to pass relevant information from the Splash Screen.
        /// The developer can subscribe to this event and process the JSON string.
        /// </summary>
        public event SplashScreenData OnSplashScreenData;

        public static void fireSplashScreenData(string payload) {
            Debug.Log("SpilSDK-Unity Splash Screen Data: " + payload);

            if (Spil.Instance.OnSplashScreenData != null) {
                Spil.Instance.OnSplashScreenData(payload);
            }
        }

        public delegate void SplashScreenError(SpilErrorMessage errorMessage);

        /// <summary>
        /// This is fired by the native Spil SDK when the web view encounters an error.
        /// The developer can subscribe to this event and inspect the error.
        /// </summary>
        public event SplashScreenError OnSplashScreenError;

        public static void fireSplashScreenError(string reason) {
            Debug.Log("SpilSDK-Unity Web Error with reason = " + reason);

            SpilErrorMessage errorMessage = JsonHelper.getObjectFromJson<SpilErrorMessage>(reason);

            if (Spil.Instance.OnSplashScreenError != null) {
                Spil.Instance.OnSplashScreenError(errorMessage);
            }
        }

        public delegate void DailyBonusOpen();

        /// <summary>
        /// This is fired by the native Spil SDK when the web view opened.
        /// </summary>
        public event DailyBonusOpen OnDailyBonusOpen;

        public static void fireDailyBonusOpen() {
            Debug.Log("SpilSDK-Unity Web open");

            if (Spil.Instance.OnDailyBonusOpen != null) {
                Spil.Instance.OnDailyBonusOpen();
            }
        }

        public delegate void DailyBonusNotAvailable();

        /// <summary>
        /// This is fired by the native Spil SDK when the dailybonus screen is not available.
        /// </summary>
        public event DailyBonusNotAvailable OnDailyBonusNotAvailable;

        public static void fireDailyBonusNotAvailable() {
            Debug.Log("SpilSDK-Unity Daily bonus not available");

            if (Spil.Instance.OnDailyBonusNotAvailable != null) {
                Spil.Instance.OnDailyBonusNotAvailable();
            }
        }

        public delegate void DailyBonusClosed();

        /// <summary>
        /// This is fired by the native Spil SDK when the web view closes.
        /// </summary>
        public event DailyBonusClosed OnDailyBonusClosed;

        public static void fireDailyBonusClosed() {
            Debug.Log("SpilSDK-Unity Web closed");

            if (Spil.Instance.OnDailyBonusClosed != null) {
                Spil.Instance.OnDailyBonusClosed();
            }
        }

        public delegate void DailyBonusError(SpilErrorMessage errorMessage);

        /// <summary>
        /// This is fired by the native Spil SDK when the web view encounters an error.
        /// The developer can subscribe to this event and inspect the error.
        /// </summary>
        public event DailyBonusError OnDailyBonusError;

        public static void fireDailyBonusError(string reason) {
            Debug.Log("SpilSDK-Unity Web Error with reason = " + reason);

            SpilErrorMessage errorMessage = JsonHelper.getObjectFromJson<SpilErrorMessage>(reason);

            if (Spil.Instance.OnDailyBonusError != null) {
                Spil.Instance.OnDailyBonusError(errorMessage);
            }
        }

        public delegate void DailyBonusReward(string rewardList);

        /// <summary>
        /// This is fired by the native Spil SDK when the reward is received from the web view.
        /// The developer can subscribe to this event and provide the reward to the user.
        /// </summary>
        public event DailyBonusReward OnDailyBonusReward;

        public static void fireDailyBonusReward(string reward) {
            Debug.Log("SpilSDK-Unity Received reward = " + reward);

            if (Spil.Instance.OnDailyBonusReward != null) {
                Spil.Instance.OnDailyBonusReward(reward);
            }
        }

        public delegate void RewardTokenReceived(string token, List<RewardObject> reward, string rewardType);

        /// <summary>
        /// This is fired by the native Spil SDK when the reward is received.
        /// The developer can subscribe to this event and provide the reward to the user.
        /// </summary>
        public event RewardTokenReceived OnRewardTokenReceived;

        public static void fireRewardTokenReceived(string response) {
            Debug.Log("SpilSDK-Unity Received reward = " + response);

            RewardResponse rewardResponse = JsonHelper.getObjectFromJson<RewardResponse>(response);

            if (Spil.Instance.OnRewardTokenReceived != null) {
                Spil.Instance.OnRewardTokenReceived(rewardResponse.token, rewardResponse.reward,
                    rewardResponse.rewardType);
            }
        }

        public delegate void RewardTokenClaimed(List<RewardObject> reward, string rewardType);

        /// <summary>
        /// This is fired by the native Spil SDK when the reward has been claimed successfully from SLOT.
        /// The developer can subscribe to this event and provide the reward to the user.
        /// </summary>
        public event RewardTokenClaimed OnRewardTokenClaimed;

        public static void fireRewardTokenClaimed(string response) {
            Debug.Log("SpilSDK-Unity Claimed reward = " + response);

            RewardResponse rewardResponse = JsonHelper.getObjectFromJson<RewardResponse>(response);

            if (Spil.Instance.OnRewardTokenClaimed != null) {
                Spil.Instance.OnRewardTokenClaimed(rewardResponse.reward, rewardResponse.rewardType);
            }
        }

        public delegate void RewardTokenClaimFailed(string rewardType, SpilErrorMessage error);

        /// <summary>
        /// This is fired by the native Spil SDK when the reward claiming has failed in SLOT.
        /// The developer can subscribe to this event and provide the reward to the user.
        /// </summary>
        public event RewardTokenClaimFailed OnRewardTokenClaimFailed;

        public static void fireRewardTokenClaimFailed(string response) {
            Debug.Log("SpilSDK-Unity Claim failed for = " + response);

            RewardResponse rewardResponse = JsonHelper.getObjectFromJson<RewardResponse>(response);

            if (Spil.Instance.OnRewardTokenClaimFailed != null) {
                Spil.Instance.OnRewardTokenClaimFailed(rewardResponse.rewardType, rewardResponse.error);
            }
        }

        #region Image loading

        //Get the image URL of the item.
        /// <summary>
        /// This method is a convenience method for the developers to easily load a locally stored image file into a Texture2d. Loaded images will be passed back to the developer via the OnImageLoaded event.
        /// </summary>
        public void LoadImage(MonoBehaviour gameObject, string localPath, int width = 4096, int height = 4096,
            TextureFormat textureFormat = TextureFormat.RGB24, bool mipMap = false) {
            gameObject.StartCoroutine(getImageFromURL(localPath, width, height, textureFormat, mipMap));
        }

        private IEnumerator getImageFromURL(string localPath, int width = 4096, int height = 4096,
            TextureFormat textureFormat = TextureFormat.RGB24, bool mipMap = false) {
            Texture2D tex = new Texture2D(width, height, textureFormat, mipMap);
            //try to load images in this way.it should takes exactly 48MB per texture, 
            Debug.Log("Loading image texture from path: " + localPath);
            WWW www = new WWW(localPath);
            yield return www;

            // calling this function with StartCoroutine solves the problem
            //Debug.Log("Why on earth is this never called?");

            www.LoadImageIntoTexture(tex);
            www.Dispose();
            www = null;

            fireImageLoaded(tex, localPath);
        }

        public delegate void ImageLoaded(Texture2D image, string localPath);

        /// <summary>
        /// This event is called when an image has been loaded from Disk and contains the Texture2D object for the image.
        /// </summary>
        public event ImageLoaded OnImageLoaded;

        public static void fireImageLoaded(Texture2D image, string localPath) {
            Debug.Log("SpilSDK-Unity fireImageLoaded");

            if (Spil.Instance.OnImageLoaded != null) {
                Spil.Instance.OnImageLoaded(image, localPath);
            }
        }

        public delegate void ImageLoadSuccess(string localPath, ImageContext imageContext);

        /// <summary>
        /// This event is called when an image has been loaded.
        /// </summary>
        public event ImageLoadSuccess OnImageLoadSuccess;

        public static void fireImageLoadSuccess(string response) {
            Debug.Log("SpilSDK-Unity fireImageLoadSuccess " + response);

            JSONObject responseJSON = new JSONObject(response);

            string localPath = responseJSON.GetField("localPath").str;

            ImageContext imageContextObj =
                JsonHelper.getObjectFromJson<ImageContext>(responseJSON.GetField("imageContext").Print(false));

            if (Spil.Instance.OnImageLoadSuccess != null) {
                Spil.Instance.OnImageLoadSuccess(localPath, imageContextObj);
            }
        }

        public delegate void ImageLoadFailed(ImageContext imageContext, SpilErrorMessage errorMessage);

        /// <summary>
        /// This event is called when an image has failed to load.
        /// </summary>
        public event ImageLoadFailed OnImageLoadFailed;

        public static void fireImageLoadFailed(string response) {
            Debug.Log("SpilSDK-Unity fireImageLoadFailed error: " + response);

            JSONObject responseJSON = new JSONObject(response);

            ImageContext imageContextObj =
                JsonHelper.getObjectFromJson<ImageContext>(responseJSON.GetField("imageContext").Print(false));
            SpilErrorMessage errorMessage =
                JsonHelper.getObjectFromJson<SpilErrorMessage>(responseJSON.GetField("errorCode").Print(false));

            if (Spil.Instance.OnImageLoadFailed != null) {
                Spil.Instance.OnImageLoadFailed(imageContextObj, errorMessage);
            }
        }

        public delegate void ImagePreloadingCompleted();

        /// <summary>
        /// This event indicates that the operation for preloading item and bundle images has been completed.
        /// </summary>
        public event ImagePreloadingCompleted OnImagePreloadingCompleted;

        public static void fireImagePreloadingCompleted() {
            Debug.Log("SpilSDK-Unity fireImagePreloadingCompleted");

            if (Spil.Instance.OnImagePreloadingCompleted != null) {
                Spil.Instance.OnImagePreloadingCompleted();
            }
        }

        #endregion

        #region IAP validation

        public delegate void IAPValid(string data);

        /// <summary>
        /// This event indicates that the IAP has been validated with the SLOT backend.
        /// </summary>
        public event IAPValid OnIAPValid;

        public static void fireIAPValid(string data) {
            Debug.Log("SpilSDK-Unity fireIAPValid with data: " + data);

            if (Spil.Instance.OnIAPValid != null) {
                Spil.Instance.OnIAPValid(data);
            }
        }

        public delegate void IAPInvalid(string message);

        /// <summary>
        /// This event indicates that the IAP has been validated with the SLOT backend.
        /// </summary>
        public event IAPInvalid OnIAPInvalid;

        public static void fireIAPInvalid(string message) {
            Debug.Log("SpilSDK-Unity fireIAPInvalid with data: " + message);

            if (Spil.Instance.OnIAPInvalid != null) {
                Spil.Instance.OnIAPInvalid(message);
            }
        }

        public delegate void IAPRequestPurchase(string skuId);

        /// <summary>
        /// This event indicates that an IAP request was fired from the SDK.
        /// </summary>
        public event IAPRequestPurchase OnIAPRequestPurchase;

        public static void fireIAPRequestPurchase(string skuId) {
            Debug.Log("SpilSDK-Unity fireIAPRequestPurchase with sku: " + skuId);

            if (Spil.Instance.OnIAPRequestPurchase != null) {
                Spil.Instance.OnIAPRequestPurchase(skuId);
            }
        }

        public delegate void IAPServerError(SpilErrorMessage errorMessage);

        /// <summary>
        /// This event indicates that the IAP has been validated with the SLOT backend.
        /// </summary>
        public event IAPServerError OnIAPServerError;

        public static void fireIAPServerError(string error) {
            Debug.Log("SpilSDK-Unity fireIAPServerError with data: " + error);

            SpilErrorMessage errorMessage = JsonHelper.getObjectFromJson<SpilErrorMessage>(error);
            if (Spil.Instance.OnIAPInvalid != null) {
                Spil.Instance.OnIAPServerError(errorMessage);
            }
        }

        #endregion

        #region Server Time

        public delegate void ServerTimeRequestSuccess(long time);

        /// <summary>
        /// This event indicates that the IAP has been validated with the SLOT backend.
        /// </summary>
        public event ServerTimeRequestSuccess OnServerTimeRequestSuccess;

        public static void fireServerTimeRequestSuccess(string timeString) {
            Debug.Log("SpilSDK-Unity fireServerTimeRequestSuccess with data: " + timeString);

            long time = long.Parse(timeString);

            if (Spil.Instance.OnServerTimeRequestSuccess != null) {
                Spil.Instance.OnServerTimeRequestSuccess(time);
            }
        }

        public delegate void ServerTimeRequestFailed(SpilErrorMessage errorMessage);

        /// <summary>
        /// This event indicates that the IAP has been validated with the SLOT backend.
        /// </summary>
        public event ServerTimeRequestFailed OnServerTimeRequestFailed;

        public static void fireServerTimeRequestFailed(string error) {
            Debug.Log("SpilSDK-Unity fireServerTimeRequestFailed with data: " + error);

            SpilErrorMessage errorMessage = JsonHelper.getObjectFromJson<SpilErrorMessage>(error);
            if (Spil.Instance.OnServerTimeRequestFailed != null) {
                Spil.Instance.OnServerTimeRequestFailed(errorMessage);
            }
        }

        #endregion

        #region Live Event

        public delegate void LiveEventAvailable();

        /// <summary>
        /// This event indicates that the Live Event Stage has been opened.
        /// </summary>
        public event LiveEventAvailable OnLiveEventAvailable;

        public static void fireLiveEventAvailable() {
            Debug.Log("SpilSDK-Unity fireLiveEventAvailable");

            if (Spil.Instance.OnLiveEventAvailable != null) {
                Spil.Instance.OnLiveEventAvailable();
            }
        }

        public delegate void LiveEventStageOpen();

        /// <summary>
        /// This event indicates that the Live Event Stage has been opened.
        /// </summary>
        public event LiveEventStageOpen OnLiveEventStageOpen;

        public static void fireLiveEventStageOpen() {
            Debug.Log("SpilSDK-Unity fireLiveEventStageOpen");

            if (Spil.Instance.OnLiveEventStageOpen != null) {
                Spil.Instance.OnLiveEventStageOpen();
            }
        }

        public delegate void LiveEventStageClosed();

        /// <summary>
        /// This event indicates that the Live Event Stage has been closed.
        /// </summary>
        public event LiveEventStageClosed OnLiveEventStageClosed;

        public static void fireLiveEventStageClosed() {
            Debug.Log("SpilSDK-Unity fireLiveEventStageClosed");

            if (Spil.Instance.OnLiveEventStageClosed != null) {
                Spil.Instance.OnLiveEventStageClosed();
            }
        }

        public delegate void LiveEventNotAvailable();

        /// <summary>
        /// This event indicates that the Live Event is not available.
        /// </summary>
        public event LiveEventNotAvailable OnLiveEventNotAvailable;

        public static void fireLiveEventNotAvailable() {
            Debug.Log("SpilSDK-Unity fireLiveEventNotAvailable");

            if (Spil.Instance.OnLiveEventNotAvailable != null) {
                Spil.Instance.OnLiveEventNotAvailable();
            }
        }

        public delegate void LiveEventError(SpilErrorMessage errorMessage);

        /// <summary>
        /// This event indicates that the Live Event encountered an error.
        /// </summary>
        public event LiveEventError OnLiveEventError;

        public static void fireLiveEventError(string error) {
            Debug.Log("SpilSDK-Unity fireLiveEventError with data: " + error);

            SpilErrorMessage errorMessage = JsonHelper.getObjectFromJson<SpilErrorMessage>(error);
            if (Spil.Instance.OnLiveEventError != null) {
                Spil.Instance.OnLiveEventError(errorMessage);
            }
        }

        public delegate void LiveEventUsedExternalItems(string items);

        /// <summary>
        /// This event indicates the items used in the live event.
        /// The developer can subscribe to this event and provide the reward to the user.
        /// </summary>
        public event LiveEventUsedExternalItems OnLiveEventUsedExternalItems;

        public static void fireLiveEventUsedExternalItems(string items) {
            Debug.Log("SpilSDK-Unity Used items = " + items);

            if (Spil.Instance.OnLiveEventUsedExternalItems != null) {
                Spil.Instance.OnLiveEventUsedExternalItems(items);
            }
        }

        public delegate void LiveEventReward(string rewardList);

        /// <summary>
        /// This event indicates the reward given for the Live Event.
        /// The developer can subscribe to this event and provide the reward to the user.
        /// </summary>
        public event LiveEventReward OnLiveEventReward;

        public static void fireLiveEventReward(string reward) {
            Debug.Log("SpilSDK-Unity Received reward = " + reward);

            if (Spil.Instance.OnLiveEventReward != null) {
                Spil.Instance.OnLiveEventReward(reward);
            }
        }

        public delegate void LiveEventMetRequirements(bool metRequirements);

        /// <summary>
        /// This event indicates if the user met the requirements to receive the reward.
        /// </summary>
        public event LiveEventMetRequirements OnLiveEventMetRequirements;

        public static void fireLiveEventMetRequirements(bool metRequirements) {
            Debug.Log("SpilSDK-Unity LiveEventMetRequirements with data = " + metRequirements);

            if (Spil.Instance.OnLiveEventMetRequirements != null) {
                Spil.Instance.OnLiveEventMetRequirements(metRequirements);
            }
        }

        public delegate void LiveEventCompleted();

        /// <summary>
        /// This event indicates that the Live Event is not available.
        /// </summary>
        public event LiveEventCompleted OnLiveEventCompleted;

        public static void fireLiveEventCompleted() {
            Debug.Log("SpilSDK-Unity fireLiveEventCompleted");

            if (Spil.Instance.OnLiveEventCompleted != null) {
                Spil.Instance.OnLiveEventCompleted();
            }
        }

        #endregion

        #region Tiered Events

        public delegate void TieredEventsAvailable();

        /// <summary>
        /// This event indicates that tiered events are available.
        /// </summary>
        public event TieredEventsAvailable OnTieredEventsAvailable;

        public static void fireTieredEventsAvailable() {
            Debug.Log("SpilSDK-Unity fireTieredEventsAvailable");

            if (Spil.Instance.OnTieredEventsAvailable != null) {
                Spil.Instance.OnTieredEventsAvailable();
            }
        }

        public delegate void TieredEventNotAvailable();

        /// <summary>
        /// This event indicates that tiered events are not available.
        /// </summary>
        public event TieredEventNotAvailable OnTieredEventsNotAvailable;

        public static void fireTieredEventsNotAvailable() {
            Debug.Log("SpilSDK-Unity fireTieredEventsNotAvailable");

            if (Spil.Instance.OnTieredEventsNotAvailable != null) {
                Spil.Instance.OnTieredEventsNotAvailable();
            }
        }

        public delegate void TieredEventUpdated(TieredEventProgress tieredProgress);

        /// <summary>
        /// This event indicates that tiered event progress has been updated.
        /// </summary>
        public event TieredEventUpdated OnTieredEventUpdated;

        public static void fireTieredEventUpdated(string data) {
            Debug.Log("SpilSDK-Unity fireTieredEventUpdated with data = " + data);

            TieredEventProgress tieredProgress = JsonHelper.getObjectFromJson<TieredEventProgress>(data);
            
            if (Spil.Instance.OnTieredEventUpdated != null) {
                Spil.Instance.OnTieredEventUpdated(tieredProgress);
            }
        }

        public delegate void TieredEventsError(SpilErrorMessage error);

        /// <summary>
        /// This event indicates that something went wrong with the TieredEventsError request.
        /// </summary>
        public event TieredEventsError OnTieredEventsError;

        public static void fireTieredEventsError(string reason) {
            Debug.Log("SpilSDK-Unity Tiered Events Error with reason = " + reason);

            SpilErrorMessage errorMessage = JsonHelper.getObjectFromJson<SpilErrorMessage>(reason);

            if (Spil.Instance.OnTieredEventsError != null) {
                Spil.Instance.OnTieredEventsError(errorMessage);
            }
        }

        public delegate void TieredEventProgressOpen();

        /// <summary>
        /// This event indicates that the Tiered Event progress has been opened.
        /// </summary>
        public event TieredEventProgressOpen OnTieredEventProgressOpen;

        public static void fireTieredEventProgressOpen() {
            Debug.Log("SpilSDK-Unity fireTieredEventStageOpen");

            if (Spil.Instance.OnTieredEventProgressOpen != null) {
                Spil.Instance.OnTieredEventProgressOpen();
            }
        }

        public delegate void TieredEventProgressClosed();

        /// <summary>
        /// This event indicates that the Tiered Event progress has been closed.
        /// </summary>
        public event TieredEventProgressClosed OnTieredEventProgressClosed;

        public static void fireTieredEventProgressClosed() {
            Debug.Log("SpilSDK-Unity fireTieredEventStageClosed");

            if (Spil.Instance.OnTieredEventProgressClosed != null) {
                Spil.Instance.OnTieredEventProgressClosed();
            }
        }

        #endregion

        #region Social Login

        public delegate void LoginSuccessful(bool resetData, string socialProvider, string socialId, bool isGuest);

        /// <summary>
        /// This event indicates that the Social Login was successful.
        /// </summary>
        public event LoginSuccessful OnLoginSuccessful;

        public static void fireLoginSuccessful(string message) {
            Debug.Log("SpilSDK-Unity fireLoginSuccessful with message: " + message);

            JSONObject loginJSON = new JSONObject(message);
            bool resetData = loginJSON.GetField("resetData").b;
			string socialProvider = loginJSON.HasField("socialProvider") ? loginJSON.GetField("socialProvider").str : null;
			string socialId = loginJSON.HasField ("socialId") ? loginJSON.GetField ("socialId").str : null;
            bool isGuest = loginJSON.GetField("isGuest").b;
            if (Spil.Instance.OnLoginSuccessful != null) {
                Spil.Instance.OnLoginSuccessful(resetData, socialProvider, socialId, isGuest);
            }
        }

        public delegate void LoginFailed(SpilErrorMessage errorMessage);

        /// <summary>
        /// This event indicates that the Social Login failed.
        /// </summary>
        public event LoginFailed OnLoginFailed;

        public static void fireLoginFailed(string error) {
            Debug.Log("SpilSDK-Unity fireLoginFailed with data: " + error);

            SpilErrorMessage errorMessage = JsonHelper.getObjectFromJson<SpilErrorMessage>(error);
            if (Spil.Instance.OnLoginFailed != null) {
                Spil.Instance.OnLoginFailed(errorMessage);
            }
        }

        public delegate void RequestLogin();

        /// <summary>
        /// This event indicates that the Social login should be requested again.
        /// </summary>
        public event RequestLogin OnRequestLogin;

        public static void fireRequestLogin() {
            Debug.Log("SpilSDK-Unity fireRequestLogin");

            if (Spil.Instance.OnRequestLogin != null) {
                Spil.Instance.OnRequestLogin();
            }
        }

        public delegate void LogoutSuccessful();

        /// <summary>
        /// This event indicates that the Social Logout was successful.
        /// </summary>
        public event LogoutSuccessful OnLogoutSuccessful;

        public static void fireLogoutSuccessful() {
            Debug.Log("SpilSDK-Unity fireLogoutSuccessful");

            if (Spil.Instance.OnLogoutSuccessful != null) {
                Spil.Instance.OnLogoutSuccessful();
            }
        }

        public delegate void LogoutFailed(SpilErrorMessage errorMessage);

        /// <summary>
        /// This event indicates that the Social Logout failed.
        /// </summary>
        public event LogoutFailed OnLogoutFailed;

        public static void fireLogoutFailed(string error) {
            Debug.Log("SpilSDK-Unity fireLogoutFailed with data: " + error);

            SpilErrorMessage errorMessage = JsonHelper.getObjectFromJson<SpilErrorMessage>(error);
            if (Spil.Instance.OnLogoutFailed != null) {
                Spil.Instance.OnLogoutFailed(errorMessage);
            }
        }

        public delegate void AuthenticationError(SpilErrorMessage errorMessage);

        /// <summary>
        /// This event indicates that the Social Logout failed.
        /// </summary>
        public event AuthenticationError OnAuthenticationError;

        public static void fireAuthenticationError(string error) {
            Debug.Log("SpilSDK-Unity fireAuthenticationError with data: " + error);

            SpilErrorMessage errorMessage = JsonHelper.getObjectFromJson<SpilErrorMessage>(error);
            if (Spil.Instance.OnAuthenticationError != null) {
                Spil.Instance.OnAuthenticationError(errorMessage);
            }
        }

		public delegate void UserDataMergeConflict(MergeConflictData localDataMergeData, MergeConflictData remoteDataMergeData);

		/// <summary>
		/// This event indicates there was a merge conflict, the current local data and the conflicted remote data are passed.
		/// </summary>
		public event UserDataMergeConflict OnUserDataMergeConflict;

		public static void fireUserDataMergeConflict(string data) {
			Debug.Log("SpilSDK-Unity fireUserDataMergeConflict");

			MergeConflict mergeConflict = JsonHelper.getObjectFromJson<MergeConflict>(data);

			if (Spil.Instance.OnUserDataMergeConflict != null) {
				Spil.Instance.OnUserDataMergeConflict(mergeConflict.localData, mergeConflict.remoteData);
			}
		}

		public delegate void UserDataMergeSuccessful();

		/// <summary>
		/// This event indicates that the data was merged successfully.
		/// </summary>
		public event UserDataMergeSuccessful OnUserDataMergeSuccessful;

		public static void fireUserDataMergeSuccessful() {
			Debug.Log("SpilSDK-Unity fireUserDataMergeSuccessful");

		    if (Spil.PlayerData != null) {
		        Spil.PlayerData.UpdatePlayerData();
		    }
		    
			if (Spil.Instance.OnUserDataMergeSuccessful != null) {
				Spil.Instance.OnUserDataMergeSuccessful();
			}
		}

		public delegate void UserDataMergeFailed(string mergeData, string mergeType);

		/// <summary>
		/// This event indicates that the user data merge failed, 
		/// the mergeData and mergeType params can be used to retry.
		/// </summary>
		public event UserDataMergeFailed OnUserDataMergeFailed;

		public static void fireUserDataMergeFailed(string data) {
			Debug.Log("SpilSDK-Unity fireUserDataMergeFailed");

		    JSONObject jsonData = new JSONObject(data);
		    string mergeData = jsonData.HasField ("mergeData") ? jsonData.GetField ("mergeData").str : null;
		    string mergeType = jsonData.HasField ("mergeType") ? jsonData.GetField ("mergeType").str : null;
		    
			if (Spil.Instance.OnUserDataMergeFailed != null) {
				Spil.Instance.OnUserDataMergeFailed(mergeData, mergeType);
			}
		}

		public delegate void UserDataHandleMerge(string mergeType);

		/// <summary>
		/// This event indicates that the user data merge has to be handled for the specified merge type.
		/// </summary>
		public event UserDataHandleMerge OnUserDataHandleMerge;

		public static void fireUserDataHandleMerge(string mergeType) {
			Debug.Log("SpilSDK-Unity fireUserDataHandleMerge");

			if (Spil.Instance.OnUserDataHandleMerge != null) {
				Spil.Instance.OnUserDataHandleMerge(mergeType);
			}
		}

		public delegate void UserDataSyncError();

		/// <summary>
		/// This event indicates that there was a sync error.
		/// </summary>
		public event UserDataSyncError OnUserDataSyncError;

		public static void fireUserDataSyncError() {
			Debug.Log("SpilSDK-Unity fireUserDataSyncError");

			if (Spil.Instance.OnUserDataSyncError != null) {
				Spil.Instance.OnUserDataSyncError();
			}
		}

		public delegate void UserDataLockError();

		/// <summary>
		/// This event indicates that there was a lock error.
		/// </summary>
		public event UserDataLockError OnUserDataLockError;

		public static void fireUserDataLockError() {
			Debug.Log("SpilSDK-Unity fireUserDataLockError");

			if (Spil.Instance.OnUserDataLockError != null) {
				Spil.Instance.OnUserDataLockError();
			}
		}

		public delegate void UserDataError(SpilErrorMessage errorMessage);

		/// <summary>
		/// This event indicates that there was a general user data error.
		/// </summary>
		public event UserDataError OnUserDataError;

		public static void fireUserDataError(string error) {
			Debug.Log("SpilSDK-Unity fireUserDataError with data: " + error);

			SpilErrorMessage errorMessage = JsonHelper.getObjectFromJson<SpilErrorMessage>(error);
			if (Spil.Instance.OnUserDataError != null) {
				Spil.Instance.OnUserDataError(errorMessage);
			}
		}

		public delegate void UserDataAvailable();

		/// <summary>
		/// This event indicates that the user data is available.
		/// </summary>
		public event UserDataAvailable OnUserDataAvailable;

		public static void fireUserDataAvailable() {
			Debug.Log("SpilSDK-Unity fireUserDataAvailable");

		    Spil.PlayerData.UpdatePlayerData();
		    
			if (Spil.Instance.OnUserDataAvailable != null) {
				Spil.Instance.OnUserDataAvailable();
			}
		}

        #endregion

        #region Privacy Policy

        public delegate void PrivacyPolicyStatus(bool accepted);

        public event PrivacyPolicyStatus OnPrivacyPolicyStatus;

        public static void firePrivacyPolicyStatus(bool accepted) {
            Debug.Log("SpilSDK-Unity firePrivacyPolicyStatus");

            if (accepted) {
                if (Spil.UseUnityPrefab) {
                    Spil.Instance.SpilInit(false);
                } else {
                    Spil.Instance.SpilInit(true);
                }
            }

            if (Spil.Instance.OnPrivacyPolicyStatus != null) {
                Spil.Instance.OnPrivacyPolicyStatus(accepted);
            }
        }

        #endregion
        
#if UNITY_ANDROID

        #region Permission

        public delegate void PermissionResponse(
            SpilAndroidUnityImplementation.PermissionResponseObject permissionResponse);

        public event PermissionResponse OnPermissionResponse;

        public static void firePermissionResponse(string message) {
            Debug.Log("SpilSDK-Unity Permission response with message: " + message);

            SpilAndroidUnityImplementation.PermissionResponseObject permissionResponse =
                JsonHelper.getObjectFromJson<SpilAndroidUnityImplementation.PermissionResponseObject>(message);
            if (Spil.Instance.OnPermissionResponse != null) {
                Spil.Instance.OnPermissionResponse(permissionResponse);
            }
        }

        #endregion

#endif

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Sends an event to the native Spil SDK which will send a request to the back-end.
        /// Track an event with params 
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="eventParams"></param>
        protected abstract void SendCustomEvent(string eventName, Dictionary<string, object> eventParams = null);

        #region Init

        /// <summary>
        /// This method is marked as internal and should not be exposed to developers.
        /// The Spil Unity SDK is not packaged as a seperate assembly yet so unfortunately this method is currently visible.
        /// Internal method names start with a lower case so you can easily recognise and avoid them.
        /// </summary>
        internal abstract void SpilInit(bool withPrivacyPolicy);

        internal abstract void CheckPrivacyPolicy();

        public void CheckPrivacyPolicyUnity() {
            #if UNITY_EDITOR
            int isPrivacyPolicyAccepted = PlayerPrefs.GetInt(GetSpilUserId() + "-unityPrivacyPolicyStatus", 0);
            #else
            int isPrivacyPolicyAccepted = PlayerPrefs.GetInt("unityPrivacyPolicyStatus", 0);
            #endif
            
            if (isPrivacyPolicyAccepted == 0) {
                PrivacyPolicyHelper.PrivacyPolicyObject = (GameObject) GameObject.Instantiate(Resources.Load("Spilgames/PrivacyPolicy/PrivacyPolicyUnity" + Spil.MonoInstance.PrefabOrientation));
                PrivacyPolicyHelper.PrivacyPolicyObject.SetActive(true);
                
                PrivacyPolicyHelper.Instance.ShowMainScreen(0);
            } else {
                firePrivacyPolicyStatus(true);
            }
             
        }

        public abstract void ShowPrivacyPolicySettings();

        internal void ShowAdsScreen() {
            PrivacyPolicyHelper.PrivacyPolicyObject = (GameObject) GameObject.Instantiate(Resources.Load("Spilgames/PrivacyPolicy/PrivacyPolicyUnity" + Spil.MonoInstance.PrefabOrientation));
            PrivacyPolicyHelper.PrivacyPolicyObject.SetActive(true);
            
            PrivacyPolicyHelper.Instance.ShowAdsScreen(2);
        }

        public void TrackPrivacyPolicyChanged(bool withPersonalisedAds, bool withPersonalisedContent, string location, bool fromStartScreen) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("personalizedAds", withPersonalisedAds);
            dict.Add("personalizedContents", withPersonalisedContent);
            dict.Add("location", location);

            if (fromStartScreen) {
                dict.Add("acceptGDPR", true);
            }
            
            SendCustomEvent("privacyChanged", dict);
        }
        
        public abstract void SavePrivValue(int priv);

        public abstract int GetPrivValue();
        
        public abstract void SetPluginInformation(string PluginName, string PluginVersion);

        #endregion

        #region Config

        public abstract string GetConfigAll();

        public abstract string GetConfigValue(string key);

        #endregion

        #region Advertisement

        public abstract void RequestRewardVideo(string location = null, string rewardType = null);

        public abstract void RequestMoreApps();

        public abstract void PlayVideo(string location = null, string rewardType = null);

        public abstract void PlayMoreApps();

        public abstract void TestRequestAd(string providerName, string adType, bool parentalGate);

        /// <summary>
        /// Call to inform the SDK that the parental gate was (not) passes
        /// </summary>
        public abstract void ClosedParentalGate(bool pass);

        #endregion

        #region Packages

        protected abstract string GetAllPackages();

        //Method that returns a package based on key
        protected abstract string GetPackage(string key);

        // This is not essential so could be removed but might be handy for some developers so we left it in.
        public abstract void RequestPackages();

        #endregion

        #region User Id

        /// <summary>
        /// Retrieves the Spil User Id so that developers can show this in-game for users.
        /// If users contact Spil customer service they can supply this Id so that 
        /// customer support can help them properly. Please make this Id available for users
        /// in one of your game's screens.
        /// </summary>
        public abstract string GetSpilUserId();

        /// <summary>
        /// Sets the user identifier.
        /// </summary>
        /// <param name="providerId">Provider identifier.</param>
        /// <param name="userId">User identifier.</param>
        public abstract void SetUserId(string providerId, string userId);

        /// <summary>
        /// Gets the user identifier.
        /// </summary>
        /// <returns>The user identifier.</returns>
        public abstract string GetUserId();

        /// <summary>
        /// Gets the device identifier.
        /// </summary>
        /// <returns>The user identifier.</returns>
        public abstract string GetDeviceId();
        
        /// <summary>
        /// Sets the custom bundle identifier.
        /// Use this when the bundle id used to connect to our backend differs from the one used to build.
        /// </summary>
        /// <param name="bundleId">Bundle identifier.</param>
        public abstract void SetCustomBundleId(string bundleId);

        /// <summary>
        /// Gets the user provider.
        /// </summary>
        /// <returns>The user provider native.</returns>
        public abstract string GetUserProvider();

        #endregion

        #region Daily Bonus and Splash Screen

        /// <summary>
        /// Requests the daily bonus screen.
        /// </summary>
        public abstract void RequestDailyBonus();

        /// <summary>
        /// Requests the splashscreen.
        /// </summary>
        public abstract void RequestSplashScreen(string type = null);

        #endregion

        #region Image Processing

        /// <summary>
        /// Used to get the image from the cache, based on the url provided.
        /// </summary>
        public abstract string GetImagePath(string url);

        /// <summary>
        /// Used to get the image from the cache, based on the url provided.ImageContext will be imageType = custom when it's not provided as parameter
        /// </summary>
        public abstract void RequestImage(string url, int id, string imageType);

        /// <summary>
        /// Clears the cache, useful in case when a lot of items have been updated.
        /// </summary>
        public abstract void ClearDiskCache();

        /// <summary>
        /// This method loops through all the items and bundles and adds urls to images (if any) to a download queue if those images have not yet been download and saved to local storage.
        /// </summary>
        public abstract void PreloadItemAndBundleImages();

        #endregion

        #region Game Data, Player Data and Game State

        public abstract string GetSpilGameDataFromSdk();

        public abstract void RequestPromotions();

        public abstract string GetAllPromotions();

        public abstract string GetBundlePromotion(int bundleId);

        public abstract string GetPackagePromotion(string packageId);

        public abstract void ShowPromotionScreen(int promotionId);
        
        /// <summary>
        /// Request the users data from SLOT.
        /// </summary>
        public abstract void RequestUserData();

        public abstract void MergeUserData(string mergeData, string mergeType);

        public abstract void ShowMergeConflictDialog(string title, string message, string localButtonText, string remoteButtonText, string mergeButtonText = null);

        public abstract void ShowSyncErrorDialog(string title, string message, string startMergeButtonText);

		public abstract void ShowMergeFailedDialog(string title, string message, string retryButtonText, string mergeData, string mergeType);
        
        public abstract string GetWalletFromSdk();

        public abstract string GetInvetoryFromSdk();

        public abstract void AddCurrencyToWallet(int currencyId, int amount, string reason, string location,
            string reasonDetails = null, string transactionId = null);

        public abstract void SubtractCurrencyFromWallet(int currencyId, int amount, string reason, string location,
            string reasonDetails = null, string transactionId = null);

        public abstract void AddItemToInventory(int itemId, int amount, string reason, string location,
            string reasonDetails = null, string transactionId = null);

        public abstract void SubtractItemFromInventory(int itemId, int amount, string reason, string location,
            string reasonDetails = null, string transactionId = null);

        public abstract void BuyBundle(int bundleId, string reason, string location, string reasonDetails = null,
            string transactionId = null);

        public abstract void OpenGacha(int gachaId, string reason, string location, string reasonDetails = null);
        
        /// <summary>
        /// Sets the state of the private game.
        /// </summary>
        /// <param name="privateData">Private data.</param>
        public abstract void SetPrivateGameState(string privateData);

        /// <summary>
        /// Gets the state of the private game.
        /// </summary>
        /// <returns>The private game state.</returns>
        public abstract string GetPrivateGameState();

        /// <summary>
        /// Sets the public game state.
        /// </summary>
        /// <param name="publicData">Public data.</param>
        public abstract void SetPublicGameState(string publicData);

        /// <summary>
        /// Gets the public game state.
        /// </summary>
        /// <returns>The public game state.</returns>
        public abstract string GetPublicGameState();

        /// <summary>
        /// Gets the public game state of other users.
        /// </summary>
        /// <param name="provider">Provider.</param>
        /// <param name="userIdsJsonArray">User identifiers json array.</param>
        public abstract void GetOtherUsersGameState(string provider, string userIdsJsonArray);
        
        public abstract void ResetPlayerData();

        public abstract void ResetInventory();

        public abstract void ResetWallet();

        /// <summary>
        /// Updates the player data from the server.
        /// </summary>
        public abstract void UpdatePlayerData();

        #endregion

        #region Customer Support

        public abstract void ShowHelpCenterWebview(string url);

        #endregion

        #region Reward

        public abstract void ClaimToken(string token, string rewardType);

        #endregion

        #region Server Time

        public abstract void RequestServerTime();

        #endregion

        #region Live Event

        public abstract void RequestLiveEvent();

        public abstract void OpenLiveEvent();

        public abstract string GetLiveEventConfig();

        public abstract long GetLiveEventStartDate();

        public abstract long GetLiveEventEndDate();

        #endregion

        #region Tiered events

        public abstract void RequestTieredEvents();

        public abstract List<TieredEvent> GetAllTieredEvents();

        public abstract TieredEventProgress GetTieredEventProgress(int tieredEventId);
        
        public abstract void ShowTieredEventProgress(int tieredEventId);

        #endregion

        #region Social Login

        public abstract void UserLogin(string socialId, string socialProvider, string socialToken);

        public abstract void UserLogout(bool global);

        public abstract void UserPlayAsGuest();

        public abstract void ShowUnauthorizedDialog(string title, string message, string loginText,
            string playAsGuestText);

        public abstract bool IsLoggedIn();

        #endregion

        public abstract void ResetData();

        public abstract void ShowNativeDialog(string title, string message, string buttonText);

        #endregion
    }
}