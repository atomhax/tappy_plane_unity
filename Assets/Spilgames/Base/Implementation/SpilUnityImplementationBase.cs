using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine;
using SpilGames.Unity;
using SpilGames.Unity.Base.Implementations.Tracking;
using SpilGames.Unity.Json;
using SpilGames.Unity.Base.SDK;
using SpilGames.Unity.Helpers.AssetBundles;
using SpilGames.Unity.Helpers.DailyBonus;
using SpilGames.Unity.Helpers.EventParams;
using SpilGames.Unity.Helpers.IAPPackages;
using SpilGames.Unity.Helpers.Promotions;
using SpilGames.Unity.Helpers.GameData;
using SpilGames.Unity.Helpers.PlayerData;

namespace SpilGames.Unity.Base.Implementations{
    public abstract class SpilUnityImplementationBase{
        public static string PluginName = "Unity";
        public static string PluginVersion = "3.1.0";

        public static string AndroidVersion = "3.1.0";
        public static string iOSVersion = "3.1.0";
	    
        /// <summary>
        /// Contains the game data: items, currencies, bundles (collections of items/currencies for softcurrency/hardcurrency transactions and gifting), Shop and Gacha boxes.
        /// For player-specific data such as owned items or currencies use the PlayerData object.
		/// Same as Spil.GameData
        /// </summary>
        public SpilGameDataHelper GameData { get { return Spil.GameData; } }

        /// <summary>
        /// Contains data specific to the user: Owned items, Currencies and Gacha boxes.
        /// Provides methods for adding/deleting/buying items and currencies and opening gacha boxes.
        /// For saving/loading player data such as game progress, use the "Gamestate" related methods and events in Spil.Instance. 
		/// Same as Spil.PlayerData
        /// </summary>
        public PlayerDataHelper PlayerData { get { return Spil.PlayerData; } }

	    private DailyBonusHelper DailyBonusHelper;
	    
        #region Events

        #region Standard Spil events
        
        /// <summary>
        /// Sends the "milestoneAchieved" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="name"></param>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.MilestoneAchieved(...).AddX().Track() instead.")]
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
            
            SendCustomEventInternal("milestoneAchieved", dict);
        }

        /// <summary>
        /// Sends the "levelStart" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="levelName">Level name.</param>
        /// <param name="difficulty">The difficulty of the started level.</param>
        /// <param name="customCreated">If set to <c>true</c> custom created.</param>
        /// <param name="creatorId">Creator identifier.</param>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.LevelStart(...).AddX().Track() instead.")]
        public void TrackLevelStartEvent(string levelName, string difficulty = null, int score = 0, bool customCreated = false,
            string creatorId = null, List<String> activeBooster = null) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("level", levelName);

            if (difficulty != null) {
                dict.Add("difficulty", difficulty);
            }

            if (score != 0) {
                dict.Add("score", score);
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

            SendCustomEventInternal("levelStart", dict);
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
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.LevelComplete(...).AddX().Track() instead.")]
        public void TrackLevelCompleteEvent(string levelName, string difficulty = null, float score = 0, int stars = 0,
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
            
            SendCustomEventInternal("levelComplete", dict);
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
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.LevelFailed(...).AddX().Track() instead.")]
        public void TrackLevelFailedEvent(string levelName, string difficulty = null, float score = 0,
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
            
            SendCustomEventInternal("levelFailed", dict);
        }

        /// <summary>
        /// Sends the "levelUp" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="level">Level.</param>
        /// <param name="objectId">Object identifier.</param>
        /// <param name="skillId">Skill identifier.</param>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.LevelUp(...).AddX().Track() instead.")]
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
            
            SendCustomEventInternal("levelUp", dict);
        }

        /// <summary>
        /// Sends the "equip" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="equippedItem">Equipped item.</param>
        /// <param name="equippedTo">Equipped to.</param>
        /// <param name="unequippedFrom">The character/object which unequipped the item.</param>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.Equip(...).AddX().Track() instead.")]
        public void TrackEquipEvent(string equippedItem, string equippedTo = null, string unequippedFrom = null) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("equippedItem", equippedItem);

            if (equippedTo != null) {
                dict.Add("equippedTo", equippedTo);
            }

            if (unequippedFrom != null) {
                dict.Add("unequippedFrom", unequippedFrom);
            }

            SendCustomEventInternal("equip", dict);
        }

        /// <summary>
        /// Sends the "equip" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="upgradeId">Upgrade identifier.</param>
        /// <param name="level">Level.</param>
        /// <param name="reason">Reason.</param>
        /// <param name="iteration">Iteration.</param>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.Upgrade(...).AddX().Track() instead.")]
        public void TrackUpgradeEvent(string upgradeId, string level, string reason = null, int iteration = 0, string achievement = null, string key = null) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("upgradeId", upgradeId);
            dict.Add("level", level);

            if (reason != null) {
                dict.Add("reason", reason);
            }

            if (iteration != 0) {
                dict.Add("iteration", iteration);
            }
            
            if (achievement != null) {
                dict.Add("achievement", achievement);
            }

            if (key != null) {
                dict.Add("key", key);
            }

            SendCustomEventInternal("upgrade", dict);
        }

        /// <summary>
        /// Sends the "levelCreate" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="levelId">Level identifier.</param>
        /// <param name="creatorId">Creator identifier.</param>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.LevelCreate(...).AddX().Track() instead.")]
        public void TrackLevelCreateEvent(string levelId, string level, string creatorId) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("levelId", levelId);
            dict.Add("level", level);
            dict.Add("creatorId", creatorId);

            SendCustomEventInternal("levelCreate", dict);
        }

        /// <summary>
        /// Sends the "levelDownload" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="levelId">Level identifier.</param>
        /// <param name="creatorId">Creator identifier.</param>
        /// <param name="rating">Rating.</param>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.LevelDownload(...).AddX().Track() instead.")]
        public void TrackLevelDownloadEvent(string levelId, string creatorId, float rating = 0) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("levelId", levelId);
            dict.Add("creatorId", creatorId);

            if (rating != 0) {
                dict.Add("rating", rating);
            }

            SendCustomEventInternal("levelDownload", dict);
        }

        /// <summary>
        /// Sends the "levelRate" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="levelId">Level identifier.</param>
        /// <param name="creatorId">Creator identifier.</param>
        /// <param name="rating">Rating.</param>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.LevelRate(...).AddX().Track() instead.")]
        public void TrackLevelRateEvent(string levelId, string creatorId, float rating = 0) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("levelId", levelId);
            dict.Add("creatorId", creatorId);

            if (rating != 0) {
                dict.Add("rating", rating);
            }

            SendCustomEventInternal("levelRate", dict);
        }

        /// <summary>
        /// Sends the "endlessModeStart" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.EndlessModeStart(...).AddX().Track() instead.")]
        public void TrackEndlessModeStartEvent() {
            SendCustomEventInternal("endlessModeStart");
        }

        /// <summary>
        /// Sends the "endlessModeEnd" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="distance">Distance.</param>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.EndlessModeEnd(...).AddX().Track() instead.")]
        public void TrackEndlessModeEndEvent(int distance) {
            SendCustomEventInternal("endlessModeEnd", new Dictionary<string, object>() {
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
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.PlayerDies(...).AddX().Track() instead.")]
        public void TrackPlayerDiesEvent(string levelName) {
            SendCustomEventInternal("playerDies", new Dictionary<string, object>() {
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
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.WalletInventoryEvent(...).AddX().Track() instead.")]
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

            SendCustomEventInternal("updatePlayerData", dictionary);
        }

        /// <summary>
        /// Sends the "iapPurchased" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="skuId">The product identifier of the item that was purchased</param>
        /// <param name="transactionId ">The transaction identifier of the item that was purchased (also called orderId)</param>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.IAPPurchased(...).AddX().Track() instead.")]
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

            SendCustomEventInternal("iapPurchased", dictionary);
        }

        /// <summary>
        /// Sends the "iapRestored" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="skuId">The product identifier of the item that was purchased</param>
        /// <param name="originalTransactionId ">For a transaction that restores a previous transaction, the transaction identifier of the original transaction. Otherwise, identical to the transaction identifier</param>
        /// <param name="originalPurchaseDate">For a transaction that restores a previous transaction, the date of the original transaction. Please use a proper DateTime format (RFC3339), for instance: "2016-08-30T11:54:48.5247936+02:00". If you have a DateTime object you can use: DateTimeObject.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz")</param>                
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.IAPRestored(...).AddX().Track() instead.")]
        public void TrackIAPRestoredEvent(string skuId, string originalTransactionId, string originalPurchaseDate, string reason = null) {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            
            dictionary.Add("skuId", skuId);
            dictionary.Add("originalTransactionId", originalTransactionId);
            dictionary.Add("originalPurchaseDate", originalPurchaseDate);
            
            if (reason != null) {
                dictionary.Add("reason", reason);
            }
            
            SendCustomEventInternal("iapRestored", dictionary);
        }

        /// <summary>
        /// Sends the "iapFailed" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="error">Error description or error code</param>
        /// <param name="skuId">The product identifier of the item that was purchased</param>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.IAPFailed(...).AddX().Track() instead.")]
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
            
            SendCustomEventInternal("iapFailed", dictionary);
        }

        /// <summary>
        /// Sends the "tutorialComplete" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.TutorialComplete(...).AddX().Track() instead.")]
        public void TrackTutorialCompleteEvent() {
            SendCustomEventInternal("tutorialComplete");
        }

        /// <summary>
        /// Sends the "tutorialSkipped" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.TutorialSkipped(...).AddX().Track() instead.")]
        public void TrackTutorialSkippedEvent() {
            SendCustomEventInternal("tutorialSkipped");
        }

        /// <summary>
        /// Sends the "tutorialSkipped" event to the native Spil SDK which will send a request to the back-end.
        /// Should be called after the user completes registration via email, Facebook, Google Plus or other available option. Registration option is assumed.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="platform">A string like ‘facebook’ or ’email’</param>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.Register(...).AddX().Track() instead.")]
        public void TrackRegisterEvent(string platform) {
            SendCustomEventInternal("register", new Dictionary<string, object>() {
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
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.Share(...).AddX().Track() instead.")]
        public void TrackShareEvent(string platform, string location = null, string reason = null) {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("platform", platform);

            if (location != null) {
                dictionary.Add("location", location);
            }     
            
            if (reason != null) {
                dictionary.Add("reason", reason);
            }
            
            SendCustomEventInternal("share", dictionary);
        }

        /// <summary>
        /// Sends the "invite" event to the native Spil SDK which will send a request to the back-end.
        /// Should be called every time the user invites another user. Respective function in-game is assumed.
        /// See https://github.com/spilgames/spil_event_unity_plugin for more information on events.
        /// </summary>
        /// <param name="platform">A string like ‘facebook’ or ’email’</param>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.Invite(...).AddX().Track() instead.")]
        public void TrackInviteEvent(string platform, string location = null) {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("platform", platform);

            if (location != null) {
                dictionary.Add("location", location);
            }
            
            SendCustomEventInternal("invite", dictionary);
        }

        /// <summary>
        /// Sends the "levelAppeared" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="level">The name/id of the level that appeared.</param>
        /// <param name="difficulty">The difficulty of the level that appeared</param>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.LevelAppeared(...).AddX().Track() instead.")]
        public void TrackLevelAppeared(string level, string difficulty = null) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("level", level);

            if (difficulty != null) {
                dict.Add("difficulty", difficulty);
            }

            SendCustomEventInternal("levelAppeared", dict);
        }

        /// <summary>
        /// Sends the "levelDiscarded" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="level">The name/id of the level that appeared.</param>
        /// <param name="difficulty">The difficulty of the level that appeared</param>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.LevelDiscarded(...).AddX().Track() instead.")]
        public void TrackLevelDiscarded(string level, string difficulty = null) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("level", level);

            if (difficulty != null) {
                dict.Add("difficulty", difficulty);
            }

            SendCustomEventInternal("levelDiscarded", dict);
        }

        /// <summary>
        /// Sends the "errorShown" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="reason">The reason for the error to be shown.</param>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.ErrorShown(...).AddX().Track() instead.")]
        public void TrackErrorShown(string reason) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("reason", reason);

            SendCustomEventInternal("errorShown", dict);
        }

        /// <summary>
        /// Sends the "timeElapLoad" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="timeElap">The time elapsed between the starting point and the pointInGame in seconds.</param>
        /// <param name="pointInGame">The point in game which is reached.</param>
        /// <param name="startPoint">The point in game which we start to measure time.</param>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.TimeElapLoad(...).AddX().Track() instead.")]
        public void TrackTimeElapLoad(int timeElap, string pointInGame, string startPoint = null) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("timeElap", timeElap);
            dict.Add("pointInGame", pointInGame);

            if (startPoint != null) {
                dict.Add("startPoint", startPoint);
            }

            SendCustomEventInternal("timeElapLoad", dict);
        }

        /// <summary>
        /// Sends the "timeoutDetected" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="timeElap">The time elapsed between the starting point and the pointInGame in seconds.</param>
        /// <param name="pointInGame">The point in game which is reached.</param>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.TimeoutDetected(...).AddX().Track() instead.")]
        public void TrackTimeoutDetected(int timeElap, string pointInGame) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("timeElap", timeElap);
            dict.Add("pointInGame", pointInGame);

            SendCustomEventInternal("timeoutDetected", dict);
        }

        /// <summary>
        /// Sends the "objectStateChanged" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="changedObject">The object which changed it's state.</param>
        /// <param name="status">The new status the object is in.</param>
        /// <param name="reason">The reason for the state change.</param>
        /// <param name="changedProperties">The property/properties which have changed.</param>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.ObjectStateChanged(...).AddX().Track() instead.")]
        public void TrackObjectStateChanged(string changedObject, string status, string reason,
            string changedProperties = null, string optionConditions = null, string situation = null, string allChoiceResults = null, string allSelectedChoices = null, string involvedParties = null) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("changedObject", changedObject);
            dict.Add("status", status);
            dict.Add("reason", reason);

            if (changedProperties != null) {
                dict.Add("changedProperties", changedProperties);
            }
            
            if (optionConditions != null) {
                dict.Add("optionConditions", optionConditions);
            }
            
            if (situation != null) {
                dict.Add("situation", situation);
            }
            
            if (allChoiceResults != null) {
                dict.Add("allChoiceResults", allChoiceResults);
            }
            
            if (allSelectedChoices != null) {
                dict.Add("allSelectedChoices", allSelectedChoices);
            }
            
            if (involvedParties != null) {
                dict.Add("involvedParties", involvedParties);
            }

            SendCustomEventInternal("objectStateChanged", dict);
        }

        /// <summary>
        /// Triggered when the user clicks a specific ui element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="type"></param>
        /// <param name="screenName"></param>
        /// <param name="location"></param>
        /// <param name="grade"></param>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.UIElementClicked(...).AddX().Track() instead.")]
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

            SendCustomEventInternal("uiElementClicked", dict);
        }

        /// <summary>
        /// Triggered when a user sends a gift
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="location"></param>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.SendGift(...).AddX().Track() instead.")]
        public void TrackSendGift(string platform, string location = null) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("platform", platform);
           
            if (location != null) {
                dict.Add("location", location);
            }

            SendCustomEventInternal("sendGift", dict);
        }

        /// <summary>
        /// Triggered when the level timer finishes
        /// </summary>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.LevelTimeOut(...).AddX().Track() instead.")]
        public void TrackLevelTimeOut() {
            SendCustomEventInternal("levelTimeOut");
        }

        /// <summary>
        /// Triggered whenever user selects an choice for a dialog,or runs out of time and default answer is selected	
        /// </summary>
        /// <param name="name"></param>
        /// <param name="hasToken"></param>
        /// <param name="isPremiumChoice"></param>
        /// <param name="isQuizz"></param>
        /// <param name="isForced"></param>
        /// <param name="isTimed"></param>
        /// <param name="time"></param>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.DialogueChosen(...).AddX().Track() instead.")]
        public void TrackDialogChosen(string name, string choice, bool hasToken, bool isPremiumChoice, bool isQuizz, bool isForced, bool isTimed, int time = 0) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("name", name);
            dict.Add("choice", choice);
            dict.Add("hasToken", hasToken);
            dict.Add("isPremiumChoice", isPremiumChoice);
            dict.Add("isQuizz", isQuizz);
            dict.Add("isForced", isForced);
            dict.Add("isTimed", isTimed);
           
            if (time != 0) {
                dict.Add("time", time);
            }

            SendCustomEventInternal("dialogueChosen", dict);
        }

        /// <summary>
        /// Triggered when the user is adding a friend in the game.
        /// </summary>
        /// <param name="friend"></param>
        /// <param name="platform"></param>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.FriendAdded(...).AddX().Track() instead.")]
        public void TrackFriendAdded(string friend, string platform = null) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("friend", friend);
           
            if (platform != null) {
                dict.Add("platform", platform);
            }

            SendCustomEventInternal("friendAdded", dict);
        }

        /// <summary>
        /// Triggered for special game object interactions
        /// </summary>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.GameObjectInteraction(...).AddX().Track() instead.")]
        public void TrackGameObjectInteraction() {
            SendCustomEventInternal("gameObjectInteraction");
        }

        /// <summary>
        /// Triggered at the end of a match and indicates the result of it
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="itemType"></param>
        /// <param name="label"></param>
        /// <param name="matchId"></param>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.GameResult(...).AddX().Track() instead.")]
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
            
            SendCustomEventInternal("gameResult", dict);
        }

        /// <summary>
        /// Triggered when a user is crafting an item.
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="itemType"></param>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.ItemCrafted(...).AddX().Track() instead.")]
        public void TrackItemCrafted(string itemId, string itemType = null) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("itemId", itemId);
           
            if (itemType != null) {
                dict.Add("itemType", itemType);
            }

            SendCustomEventInternal("itemCrafted", dict);
        }
        
        /// <summary>
        /// Triggered when a user creates an item.
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="itemType"></param>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.ItemCreated(...).AddX().Track() instead.")]
        public void TrackItemCreated(string itemId, string itemType = null) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("itemId", itemId);
           
            if (itemType != null) {
                dict.Add("itemType", itemType);
            }

            SendCustomEventInternal("itemCreated", dict);
        }
        
        /// <summary>
        /// Triggered when the user is updating an item (e.g. equipping a character with an item).
        /// </summary>
        /// <param name="content"></param>
        /// <param name="itemId"></param>
        /// <param name="itemType"></param>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.ItemUpdated(...).AddX().Track() instead.")]
        public void TrackItemUpdated(string content, string itemId, string itemType = null) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("content", content);
            dict.Add("itemId", itemId);
           
            if (itemType != null) {
                dict.Add("itemType", itemType);
            }

            SendCustomEventInternal("itemUpdated", dict);
        }

        /// <summary>
        /// Triggered when the user is updating his deck, e.g. by adding and removing cards from it.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="itemId"></param>
        /// <param name="itemType"></param>
        /// <param name="label"></param>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.DeckUpdated(...).AddX().Track() instead.")]
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
            
            SendCustomEventInternal("deckUpdated", dict);
        }
        
        /// <summary>
        /// Triggered when a player-vs-player match ends 
        /// </summary>
        /// <param name="matchId"></param>
        /// <param name="itemId"></param>
        /// <param name="itemType"></param>
        /// <param name="label"></param>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.MatchComplete(...).AddX().Track() instead.")]
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
            
            SendCustomEventInternal("matchComplete", dict);
        }
        
        /// <summary>
        /// Triggered when a user loses a player-vs-player match
        /// </summary>
        /// <param name="matchId"></param>
        /// <param name="itemId"></param>
        /// <param name="itemType"></param>
        /// <param name="label"></param>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.MatchLost(...).AddX().Track() instead.")]
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
            
            SendCustomEventInternal("matchLost", dict);
        }
        
        /// <summary>
        /// Triggered when a player-vs-player match ends with a tie
        /// </summary>
        /// <param name="matchId"></param>
        /// <param name="itemId"></param>
        /// <param name="itemType"></param>
        /// <param name="label"></param>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.MatchTie(...).AddX().Track() instead.")]
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
            
            SendCustomEventInternal("matchTie", dict);
        }
        
        /// <summary>
        /// Triggered when a user wins player-vs-player match 
        /// </summary>
        /// <param name="matchId"></param>
        /// <param name="itemId"></param>
        /// <param name="itemType"></param>
        /// <param name="label"></param>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.MatchWon(...).AddX().Track() instead.")]
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
            
            SendCustomEventInternal("matchWon", dict);
        }

        /// <summary>
        /// Triggered when the user either moves or changes the state of an in-game object
        /// </summary>
        /// <param name="name"></param>
        /// <param name="reason"></param>
        /// <param name="label"></param>
        /// <param name="delta"></param>
        /// <param name="energy"></param>
        /// <param name="kind"></param>
        /// <param name="location"></param>
        /// <param name="rarity"></param>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.PawnMoved(...).AddX().Track() instead.")]
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
            
            SendCustomEventInternal("pawnMoved", dict);
        }
        
        /// <summary>
        /// Triggered when the user interacts with the leaderboard/league
        /// </summary>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.PlayerLeagueChanged(...).AddX().Track() instead.")]
        public void TrackPlayerLeagueChanged() {
            SendCustomEventInternal("playerLeagueChanged");
        }

        /// <summary>
        /// Event used for tracking the state of a timed action in game, e.g. in Operate Now when the user assigns a staff member to the break room to regenerate energy,the event is fired at start and end of the regeneration
        /// </summary>
        /// <param name="timedAction"></param>
        /// <param name="label"></param>
        /// <param name="timedObject"></param>
        /// <param name="timeToFinish"></param>
        /// <param name="effectMultiplier"></param>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.TimedAction(...).AddX().Track() instead.")]
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
            
            SendCustomEventInternal("timedAction", dict);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.TransitionToGame(...).AddX().Track() instead.")]
        public void TrackTransitionToGame(string type) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("type", type);
            
            SendCustomEventInternal("transitionToGame", dict);
        }
        
        /// <summary>
        /// 
        /// </summary>
        [Obsolete("This tracking method has been deprecated and will be removed in a future version. Please use SpilTracking.TransitionToMenu(...).AddX().Track() instead.")]
        public void TrackTransitionToMenu() {
            SendCustomEventInternal("transitionToMenu");
        }
        
        #endregion

	            
        #region Game config

        /// <summary>
        /// Method that returns the SLOT Config as a (user-defined) object. See the example in JSONHelper.cs for
        /// more information on how to turn JSON strings (such as the SLOT game config) into classes.
        /// This method currently does not catch and handle any exceptions and will not present the developer with
        /// tips for fixing his/her JSON or class. Developers will have to make due with the standard
        /// exceptions for debugging their JSON and classes.
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
        /// <returns>A packageshelper object containing packages, or empty if there are none. Returns null if no packages are present, which should only happen if the server has never been successfully queried for packages.</returns>
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
        /// This event indicates that the IAP packages data is available and can be used.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/sdk-iap-packages-promotions/
        /// </summary>
		public event PackagesAvailable OnPackagesAvailable;
        
		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void firePackagesAvailable() {
			SpilLogging.Log("firePackagesAvailable");
			if(OnPackagesAvailable != null) {
				OnPackagesAvailable();
			}
		}

        public delegate void PackagesNotAvailable();
 
        /// <summary>
        /// This event indicates that the IAP packages data is not available and cannot be used.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/sdk-iap-packages-promotions/
        /// </summary>
		public event PackagesNotAvailable OnPackagesNotAvailable;
        
		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void firePackagesNotAvailable() {
			SpilLogging.Log("firePackagesNotAvailable");
			if(OnPackagesNotAvailable != null) {
				OnPackagesNotAvailable();
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
        /// This event indicates that the data for promotions for IAP and in-game items is available and can be used.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/promotions/
        /// </summary>
		public event PromotionsAvailable OnPromotionsAvailable;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void firePromotionsAvailable() {
			SpilLogging.Log("firePromotionsAvailable");

			if (Spil.GameData != null) {
				Spil.GameData.SpilGameDataHandler();
			}

			if(OnPromotionsAvailable != null) {
				OnPromotionsAvailable();
			}
		}

        public delegate void PromotionsNotAvailable();

        /// <summary>
        /// This event indicates that the data for promotions for IAP and in-game items is not available and cannot be used.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/promotions/
        /// </summary>
		public event PromotionsNotAvailable OnPromotionsNotAvailable;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void firePromotionsNotAvailable() {
			SpilLogging.Log("firePromotionsNotAvailable");
			if(OnPromotionsAvailable != null) {
				OnPromotionsNotAvailable();
			}
		}

        public delegate void PromotionAmountBought(int promotionId, int currentAmount, bool maxAmountReached);

        /// <summary>
        /// This event indicates that a bundle/package was bought with a promotion attached to it.
        /// Returns the promotion id, the current amount bought for the promotion by the user and if the maximum amount has been reached.
        /// The promotion visuals should be removed if maxAmountReached is true.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/promotions/
        /// </summary>
		public event PromotionAmountBought OnPromotionAmountBought;
        
		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// Developers can subscribe to events defined in Spil.Instance.
		/// </summary>
		public void firePromotionAmountBought(string data) {
			SpilLogging.Log("firePromotionAmountBought with data: " + data);

			JSONObject promotionInfo = new JSONObject(data);
			int promotionId = (int)promotionInfo.GetField("promotionId").i;
			int currentAmount = (int)promotionInfo.GetField("amountPurchased").i;
			bool maxAmountReached = promotionInfo.GetField("maxAmountReached").b;

			if(OnPromotionAmountBought != null) {
				OnPromotionAmountBought(promotionId, currentAmount, maxAmountReached);
			}
		}

        #endregion

        #region AssetBundles

        public AssetBundlesHelper GetAssetBundles() {
            string assetBundlesString = GetAllAssetBundles();

            if (assetBundlesString == null)
                return null;

            List<SpilAssetBundle> assetBundlesList = JsonHelper.getObjectFromJson<List<SpilAssetBundle>>(assetBundlesString);
            AssetBundlesHelper helper = new AssetBundlesHelper(assetBundlesList);
            return helper;
        }

        public delegate void AssetBundlesAvailable();

        /// <summary>
        /// This event indicates that the asset bundles configuration data is available and can be used.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/implementing-asset-bundles-configurations/
        /// </summary>
		public event AssetBundlesAvailable OnAssetBundlesAvailable;
        
		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireAssetBundlesAvailable() {
			SpilLogging.Log("fireAssetBundlesAvailable");
			if(OnAssetBundlesAvailable != null) {
				OnAssetBundlesAvailable();
			}
		}

        public delegate void AssetBundlesNotAvailable();

        /// <summary>
        /// This event indicates that the asset bundles configuration data is not available and cannot be used.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/implementing-asset-bundles-configurations/
        /// </summary>
		public event AssetBundlesNotAvailable OnAssetBundlesNotAvailable;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireAssetBundlesNotAvailable() {
			SpilLogging.Log("fireAssetBundlesNotAvailable");
			if(OnAssetBundlesNotAvailable != null) {
				OnAssetBundlesNotAvailable();
			}
		}

        #endregion
	    
        #region Advertisement events

        public delegate void RewardEvent(PushNotificationRewardResponse rewardResponse);

        /// <summary>
        /// This is fired by the Unity Spil SDK after it receives a push notification with reward data.
        /// The developer can subscribe to this event, assign the reward and update the UI.
        /// This event should only be used for push notification rewards. For ads reward data is passed as a parameter of the AdFinished event.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/spil-sdk-push-notifications/
        /// </summary>
		public event RewardEvent OnReward;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// Developers can subscribe to events defined in Spil.Instance.
		/// </summary>
		public void fireOnRewardEvent(PushNotificationRewardResponse rewardResponse) {
			if(OnReward != null) {
				OnReward(rewardResponse);
			}
		}

        public delegate void AdAvailableEvent(enumAdType adType);

        /// <summary>
        /// This event indicates that an ad is available and ready to be shown.
        /// The developer can subscribe to this event and call "Spil.Instance.PlayVideo();" or "Spil.Instance.PlayMoreApps()"
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/spil-sdk-advertisement-2/
        /// </summary>
		public event AdAvailableEvent OnAdAvailable;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireAdAvailable(string type) {
			SpilLogging.Log("Ad " + type + " ready!");
			enumAdType adType = enumAdType.Unknown;
			if (type.ToLower().Trim().Equals("rewardvideo")) {
				adType = enumAdType.RewardVideo;
			}
			else if (type.ToLower().Trim().Equals("interstitial")) {
				adType = enumAdType.Interstitial;
			}
			else if (type.ToLower().Trim().Equals("moreapps")) {
				adType = enumAdType.MoreApps;
			}
			if (adType == enumAdType.Unknown) {
				SpilLogging.Log("AdAvailable event fired but type is unknown. Type: " + type);
			}
			if(OnAdAvailable != null) {
				OnAdAvailable(adType);
			}
		}

        public delegate void AdNotAvailableEvent(enumAdType adType);

        /// <summary>
        /// This event indicates that an ad was requested but no ad is available, most likely due to a lack of add-fill by the ad networks or because there is no internet connection.
        /// The developer can subscribe to this event and hide any UI elements related to showing ads.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/spil-sdk-advertisement-2/
        /// </summary>
		public event AdNotAvailableEvent OnAdNotAvailable;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireAdNotAvailable(string type) {
			SpilLogging.Log("Ad " + type + " is not available");
			enumAdType adType = enumAdType.Unknown;
			if (type.ToLower().Trim().Equals("rewardvideo")) {
				adType = enumAdType.RewardVideo;
			}
			else if (type.ToLower().Trim().Equals("interstitial")) {
				adType = enumAdType.Interstitial;
			}
			else if (type.ToLower().Trim().Equals("moreapps")) {
				adType = enumAdType.MoreApps;
			}
			if (adType == enumAdType.Unknown) {
				SpilLogging.Log("AdNotAvailable event fired but type is unknown. Type: " + type);
			}
			if(OnAdNotAvailable != null) {
				OnAdNotAvailable(adType);
			}
		}

        public delegate void OpenParentalGateEvent();

        /// <summary>
        /// 
        /// </summary>
		public event OpenParentalGateEvent OnOpenParentalGate;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireOpenParentalGate() {
			SpilLogging.Log("OpenParentalGate");
			if(OnOpenParentalGate != null) {
				OnOpenParentalGate();
			}
		}

        public delegate void AdStartedEvent();

        /// <summary>
        /// This event indicates that an ad has started playing.
        /// The developer can subscribe to this event and for instance disable the in-game sound.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/spil-sdk-advertisement-2/
        /// </summary>
		public event AdStartedEvent OnAdStarted;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireAdStart() {
			SpilLogging.Log("Ad started");
			if(OnAdStarted != null) {
				OnAdStarted();
			}
		}

        public delegate void AdFinishedEvent(SpilAdFinishedResponse response);

        /// <summary>
        /// This event indicates that an ad has finished playing.
        /// The developer can subscribe to this event and for instance re-enable the in-game sound.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/spil-sdk-advertisement-2/
        /// </summary>
		public event AdFinishedEvent OnAdFinished;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireAdFinished(string response) {
			SpilLogging.Log("Ad finished! Response = " + response);
			SpilAdFinishedResponse responseObject = JsonHelper.getObjectFromJson<SpilAdFinishedResponse>(response);
			if(OnAdFinished != null) {
				OnAdFinished(responseObject);
			}
		}

		#endregion

        public delegate void ConfigUpdatedEvent();

        /// <summary>
        /// This event indicates that the game config has been updated and can be used.
        /// Returns the data from the defaultGameConfig.json if there is no internet connection, returns the game config from the SLOT back-end if there is an internet connection.
        /// Any game config data retrieved from the server is cached and is returned instead of the defaultGameConfig.json if there is no internet connection.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/spil-sdk-game-config/
        /// </summary>
		public event ConfigUpdatedEvent OnConfigUpdated;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireConfigUpdated() {
			SpilLogging.Log("Config updated!");
			if(OnConfigUpdated != null){
				OnConfigUpdated();
			}
		}

        public delegate void ConfigError(SpilErrorMessage errorMessage);

        /// <summary>
        /// This event indicates that the game config could not be loaded, this could be because there is an error in the json or because there is an internet connection and the server returned an error.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/spil-sdk-game-config/
        /// </summary>
		public event ConfigError OnConfigError;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireConfigError(string error) {
			SpilLogging.Log("Config Error with message: " + error);
			SpilErrorMessage errorMessage = JsonHelper.getObjectFromJson<SpilErrorMessage>(error);
			if(OnConfigError != null){
				OnConfigError(errorMessage);
			}
		}

        #endregion

        #region Spil Game Data

        public delegate void SpilGameDataAvailable();

        /// <summary>
        /// This event indicates that the game data has been loaded and can be used.
        /// Returns the data from the defaultGameData.json if there is no internet connection, returns the game data from the SLOT back-end if there is an internet connection.
        /// Any game data retrieved from the server is cached and is returned instead of the defaultGameData.json if there is no internet connection.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/spil-sdk-wallet-shop-inventory/
        /// </summary>
		public event SpilGameDataAvailable OnSpilGameDataAvailable;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireSpilGameDataAvailable() {
			if (Spil.GameData != null) {
				Spil.GameData.SpilGameDataHandler();
			}

			SpilLogging.Log("Spil Game Data is available");
			if(OnSpilGameDataAvailable != null){
				OnSpilGameDataAvailable();
			}
		}

        public delegate void SpilGameDataError(SpilErrorMessage errorMessage);

        /// <summary>
        /// This event indicates that the game data could not be loaded, this could be because there is an error in the data or because there is an internet connection and the server returned an error.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/spil-sdk-wallet-shop-inventory/
        /// </summary>
		public event SpilGameDataError OnSpilGameDataError;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireSpilGameDataError(string reason) {
			SpilLogging.Log("Spil Game Data error with reason = " + reason);
			SpilErrorMessage errorMessage = JsonHelper.getObjectFromJson<SpilErrorMessage>(reason);
			if(OnSpilGameDataError != null){
				OnSpilGameDataError(errorMessage);
			}
		}

        #endregion

        #region Player Data

        public delegate void PlayerDataUpdated(string reason, PlayerDataUpdatedData updatedData);

        /// <summary>
        /// This event indicates that the player data has been updated and can be used.
        /// Returns the data from the defaultPlayerData.json if there is no internet connection, returns the player data from the SLOT back-end if there is an internet connection.
        /// Any player data retrieved from the server is cached and is returned instead of the defaultPlayerData.json if there is no internet connection.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/spil-sdk-wallet-shop-inventory/
        /// </summary>
		public event PlayerDataUpdated OnPlayerDataUpdated;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void firePlayerDataUpdated(string data) {
			SpilLogging.Log("Player Data has been updated with data: " + data);

			PlayerDataUpdatedData playerDataUpdatedData = JsonHelper.getObjectFromJson<PlayerDataUpdatedData>(data);

			if (Spil.PlayerData != null) {
				Spil.PlayerData.PlayerDataUpdatedHandler();
			}

			if(OnPlayerDataUpdated != null){
				OnPlayerDataUpdated(playerDataUpdatedData.reason, playerDataUpdatedData);
			}
		}

        public delegate void PlayerDataEmptyGacha();

        /// <summary>
        /// This event indicates that a user received nothing from a gacha box.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/spil-sdk-wallet-shop-inventory/
        /// </summary>
		public event PlayerDataEmptyGacha OnPlayerDataEmptyGacha;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void firePlayerDataEmptyGacha() {
			SpilLogging.Log("Received nothing from gacha box!");
			if(OnPlayerDataEmptyGacha != null) {
				OnPlayerDataEmptyGacha();
			}
		}

        public delegate void GameStateUpdated(string access);

        /// <summary>
        /// This event indicates that the game state has been updated and can be used.
        /// Returns the locally cached data if there is no internet connection, returns the gamestate data from the SLOT back-end if there is an internet connection.
        /// Any gamestate data retrieved from the server is cached locally.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/spil-sdk-game-state/
        /// </summary>
		public event GameStateUpdated OnGameStateUpdated;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireGameStateUpdated(string access) {
			SpilLogging.Log("Game State Data updated, access = " + access);
			if(OnGameStateUpdated != null) {
				OnGameStateUpdated(access);
			}
		}

        public delegate void OtherUsersGameStateDataLoaded(OtherUsersGameStateData data);

        /// <summary>
        /// This event indicates that the other user's game state that was requested has been retrieved and can be used.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/spil-sdk-game-state/
        /// </summary>
		public event OtherUsersGameStateDataLoaded OnOtherUsersGameStateDataLoaded;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireOtherUsersGameStateLoaded(string message) {
			SpilLogging.Log("Other users game state data loaded, message = " + message);
			OtherUsersGameStateData data = JsonHelper.getObjectFromJson<OtherUsersGameStateData>(message);
			if(OnOtherUsersGameStateDataLoaded != null) {
				OnOtherUsersGameStateDataLoaded(data);
			}
		}

        public delegate void SplashScreenOpen();

        /// <summary>
        /// This event indicates that a web-view containing a splash-screen was opened.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/spil-sdk-splash-daily-bonus-screen/
        /// </summary>
		public event SplashScreenOpen OnSplashScreenOpen;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireSplashScreenOpen() {
			SpilLogging.Log("Web open");
			if(OnSplashScreenOpen != null) {
				OnSplashScreenOpen();
			}
		}

        public delegate void SplashScreenNotAvailable();

        /// <summary>
        /// This event indicates that a splash screen was not available and cannot be shown.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/spil-sdk-splash-daily-bonus-screen/
        /// </summary>
		public event SplashScreenNotAvailable OnSplashScreenNotAvailable;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireSplashScreenNotAvailable() {
			SpilLogging.Log("splash screen not available");
			if(OnSplashScreenNotAvailable != null) {
				OnSplashScreenNotAvailable();
			}
		}

        public delegate void SplashScreenClosed();

        /// <summary>
        /// This event indicates that a web-view containing a splash-screen has closed.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/spil-sdk-splash-daily-bonus-screen/
        /// </summary>
		public event SplashScreenClosed OnSplashScreenClosed;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireSplashScreenClosed() {
			SpilLogging.Log("Web closed");
			if(OnSplashScreenClosed != null) {
				OnSplashScreenClosed();
			}
		}

        public delegate void SplashScreenOpenShop();

        /// <summary>
        /// This event indicates that a web-view containing a splash-screen has sent a callback indicating that the user should be redirected to the in-game shop.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/spil-sdk-splash-daily-bonus-screen/
        /// </summary>
		public event SplashScreenOpenShop OnSplashScreenOpenShop;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireSplashScreenOpenShop() {
			SpilLogging.Log("Open Game Shop");
			if(OnSplashScreenOpenShop != null) {
				OnSplashScreenOpenShop();
			}
		}

        public delegate void SplashScreenData(string payload);

        /// <summary>
        /// This event indicates that a web-view containing a splash-screen has sent a callback containing data that should be read and acted on by the developer.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/spil-sdk-splash-daily-bonus-screen/
        /// </summary>
		public event SplashScreenData OnSplashScreenData;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireSplashScreenData(string payload) {
			SpilLogging.Log("Splash Screen Data: " + payload);
			if(OnSplashScreenData != null) {
				OnSplashScreenData(payload);
			}
		}

        public delegate void SplashScreenError(SpilErrorMessage errorMessage);

        /// <summary>
        /// This event indicates that a web-view containing a splash-screen has thrown an error.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/spil-sdk-splash-daily-bonus-screen/
        /// </summary>
		public event SplashScreenError OnSplashScreenError;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireSplashScreenError(string error) {
			SpilLogging.Log("Web Error with reason = " + error);
			SpilErrorMessage errorMessage = JsonHelper.getObjectFromJson<SpilErrorMessage>(error);
			if(OnSplashScreenError != null){
				OnSplashScreenError(errorMessage);
			}
		}

        public delegate void DailyBonusOpen();

        /// <summary>
        /// This event indicates that a web-view containing a daily bonus screen was opened.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/spil-sdk-splash-daily-bonus-screen/
        /// </summary>
		public event DailyBonusOpen OnDailyBonusOpen;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireDailyBonusOpen() {
			SpilLogging.Log("Web open");
			if(OnDailyBonusOpen != null){
				OnDailyBonusOpen();
			}
		}

	    public delegate void DailyBonusAvailable();

	    /// <summary>
	    /// This event indicates that a daily bonus screen was available.
	    /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/spil-sdk-splash-daily-bonus-screen/
	    /// </summary>
	    public event DailyBonusAvailable OnDailyBonusAvailable;

	    /// <summary>
	    /// This method is meant for internal use only, it should not be used by developers.
	    /// </summary>
	    public void fireDailyBonusAvailable(string type) {
		    SpilLogging.Log("Daily bonus available: " + type);

		    DailyBonusHelper = Spil.MonoInstance.gameObject.AddComponent<DailyBonusHelper>();
		    DailyBonusHelper.DailyBonus = GetDailyBonusConfig();
		    
		    if (type != null && type.Equals("assetBundle")) {
			    Spil.MonoInstance.StartCoroutine(DailyBonusHelper.DownloadDailyBonusAssets());
		    }
		    
		    if(OnDailyBonusAvailable != null) {
			    OnDailyBonusAvailable();
		    }
	    }
	    
        public delegate void DailyBonusNotAvailable();

        /// <summary>
        /// This event indicates that a daily bonus screen was not available and cannot be shown.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/spil-sdk-splash-daily-bonus-screen/
        /// </summary>
		public event DailyBonusNotAvailable OnDailyBonusNotAvailable;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireDailyBonusNotAvailable() {
			SpilLogging.Log("Daily bonus not available");
			if(OnDailyBonusNotAvailable != null) {
				OnDailyBonusNotAvailable();
			}
		}

        public delegate void DailyBonusClosed();

        /// <summary>
        /// This event indicates that a web-view containing a daily bonus screen has closed.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/spil-sdk-splash-daily-bonus-screen/
        /// </summary>
		public event DailyBonusClosed OnDailyBonusClosed;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireDailyBonusClosed() {
			SpilLogging.Log("Web closed");
			if(OnDailyBonusClosed != null) {
				OnDailyBonusClosed();
			}
		}

        public delegate void DailyBonusError(SpilErrorMessage errorMessage);

        /// <summary>
        /// This event indicates that a web-view containing a daily bonus screen has thrown an error.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/spil-sdk-splash-daily-bonus-screen/
        /// </summary>
		public event DailyBonusError OnDailyBonusError;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireDailyBonusError(string error) {
			SpilLogging.Log("Web Error with reason = " + error);
			SpilErrorMessage errorMessage = JsonHelper.getObjectFromJson<SpilErrorMessage>(error);
			if(OnDailyBonusError != null) {
				OnDailyBonusError(errorMessage);
			}
		}

        public delegate void DailyBonusReward(string rewardList);

        /// <summary>
        /// This event indicates that a web-view containing a daily bonus screen has sent a callback containing reward data, the developer can read the data and give the reward.
        /// This is only necessary when using external rewards (items/currencies not managed via the SpilSDK wallet/inventory).
        /// For items and currencies managed via the SpilSDK wallet/inventory the reward is given automatically so this event is not needed.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/spil-sdk-splash-daily-bonus-screen/
        /// </summary>
		public event DailyBonusReward OnDailyBonusReward;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireDailyBonusReward(string receivedReward) {
			SpilLogging.Log("Received reward = " + receivedReward);
			if(OnDailyBonusReward != null){
				OnDailyBonusReward(receivedReward);
			}
		}

        public delegate void DeepLinkReceived(string url, JSONObject payload);

        /// <summary>
        /// This event indicates that a deeplink was received. The developer can subscribe to this event, read the deeplink url and payload and react to the deeplink.
        /// </summary>
		public event DeepLinkReceived OnDeepLinkReceived;

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// </summary>
        public void fireDeepLinkReceived(string deepLink) {
            SpilLogging.Log("Received deeplink = " + deepLink);

            JSONObject deeplinkJSON = new JSONObject(deepLink);
            string url = deeplinkJSON.GetField("url").str;
            JSONObject payload = deeplinkJSON.GetField("payload");

            if (OnDeepLinkReceived != null) {
                OnDeepLinkReceived(url, payload);
            }
        }

        public delegate void RewardTokenReceived(string token, List<RewardObject> reward, string rewardType);

        /// <summary>
        /// This event indicates that a reward token was received from the back-end. The developer can subscribe to this event and use the token to claim the reward.
        /// 
        /// </summary>
		public event RewardTokenReceived OnRewardTokenReceived;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireRewardTokenReceived(string response) {
			SpilLogging.Log("Received reward = " + response);
			RewardResponse rewardResponse = JsonHelper.getObjectFromJson<RewardResponse>(response);
			if(OnRewardTokenReceived != null) {
				OnRewardTokenReceived(rewardResponse.token, rewardResponse.reward,
				rewardResponse.rewardType);
			}
		}

        public delegate void RewardTokenClaimed(List<RewardObject> reward, string rewardType);

        /// <summary>
        /// This event indicates that a reward token was successfully claimed via the SLOT back-end. Each token can only be claimed once per user.
        /// </summary>
		public event RewardTokenClaimed OnRewardTokenClaimed;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireRewardTokenClaimed(string response) {
			SpilLogging.Log("Claimed reward = " + response);
			RewardResponse rewardResponse = JsonHelper.getObjectFromJson<RewardResponse>(response);
			if(OnRewardTokenClaimed != null) {
				OnRewardTokenClaimed(rewardResponse.reward, rewardResponse.rewardType);
			}
		}

        public delegate void RewardTokenClaimFailed(string rewardType, SpilErrorMessage error);

        /// <summary>
        /// This event indicates that a reward token could not be claimed via the SLOT back-end. This can happen when the server returned an error or if the token was already claimed.
        /// </summary>
		public event RewardTokenClaimFailed OnRewardTokenClaimFailed;

		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireRewardTokenClaimFailed(string response) {
			SpilLogging.Log("Claim failed for = " + response);
			RewardResponse rewardResponse = JsonHelper.getObjectFromJson<RewardResponse>(response);
			if(OnRewardTokenClaimFailed !=  null) {
				OnRewardTokenClaimFailed(rewardResponse.rewardType, rewardResponse.error);
			}
		}

        #region Image loading

        //Get the image URL of the item.
        /// <summary>
        /// This method is a convenience method for the developers to easily load a locally stored image file into a Texture2d. Loaded images will be passed back to the developer via the OnImageLoaded event.
        /// </summary>
        public void LoadImage(MonoBehaviour gameObject, string localPath, int width = 512, int height = 512,
            TextureFormat textureFormat = TextureFormat.RGB24, bool mipMap = false) {
            gameObject.StartCoroutine(getImageFromURL(localPath, width, height, textureFormat, mipMap));
        }

        private IEnumerator getImageFromURL(string localPath, int width = 512, int height = 512,
            TextureFormat textureFormat = TextureFormat.RGB24, bool mipMap = false) {
            Texture2D tex = new Texture2D(width, height, textureFormat, mipMap);
            //try to load images in this way.it should takes exactly 48MB per texture, 
            SpilLogging.Log("Loading image texture from path: " + localPath);
            WWW www = new WWW(localPath);
            yield return www;
            
            www.LoadImageIntoTexture(tex);
            www.Dispose();
            www = null;

            fireImageLoaded(tex, localPath);
        }

        public delegate void ImageLoaded(Texture2D image, string localPath);

        /// <summary>
        /// This event indicates that an image has been loaded from local storage, it contains the Texture2D object for the image.
        /// </summary>
		public event ImageLoaded OnImageLoaded;

        /// <summary>
        /// Not intended for use by developers.
        /// </summary>
        private static void fireImageLoaded(Texture2D image, string localPath) {
			SpilLogging.Log("fireImageLoaded");

            if (Spil.Instance.OnImageLoaded != null) {
                Spil.Instance.OnImageLoaded(image, localPath);
            }
        }

        public delegate void ImageLoadSuccess(string localPath, ImageContext imageContext);

        /// <summary>
        /// This event indicates that an image has been downloaded and saved to local storage and can be used.
        /// </summary>
		public event ImageLoadSuccess OnImageLoadSuccess;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireImageLoadSuccess(string response) {
			SpilLogging.Log("fireImageLoadSuccess " + response);

			JSONObject responseJSON = new JSONObject(response);

			string localPath = responseJSON.GetField("localPath").str;

			ImageContext imageContextObj =
				JsonHelper.getObjectFromJson<ImageContext>(responseJSON.GetField("imageContext").Print(false));
			if(OnImageLoadSuccess != null) {
				OnImageLoadSuccess(localPath, imageContextObj);
			}
		}

        public delegate void ImageLoadFailed(ImageContext imageContext, SpilErrorMessage errorMessage);

        /// <summary>
        /// This event indicates that an image failed to download or could not be saved to local storage.
        /// </summary>
		public event ImageLoadFailed OnImageLoadFailed;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireImageLoadFailed(string response) {
			SpilLogging.Log("fireImageLoadFailed error: " + response);

			JSONObject responseJSON = new JSONObject(response);

			ImageContext imageContextObj =
				JsonHelper.getObjectFromJson<ImageContext>(responseJSON.GetField("imageContext").Print(false));
			SpilErrorMessage errorMessage =
				JsonHelper.getObjectFromJson<SpilErrorMessage>(responseJSON.GetField("errorCode").Print(false));
			if(OnImageLoadFailed != null) {
				OnImageLoadFailed(imageContextObj, errorMessage);
			}
		}

        public delegate void ImagePreloadingCompleted();

        /// <summary>
        /// This event indicates that pre-loading of item- and bundle-images has finished.
        /// </summary>
		public event ImagePreloadingCompleted OnImagePreloadingCompleted;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireImagePreloadingCompleted() {
			SpilLogging.Log("fireImagePreloadingCompleted");
			if(OnImagePreloadingCompleted != null) {
				OnImagePreloadingCompleted();
			}
		}

        #endregion

        #region IAP validation

        public delegate void IAPValid(string data);

        /// <summary>
        /// This event indicates that the IAP was successfully validated by the SLOT back-end.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/sdk-iap-packages-promotions/
        /// </summary>
		public event IAPValid OnIAPValid;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireIAPValid(string data) {
			SpilLogging.Log("fireIAPValid with data: " + data);
			if(OnIAPValid != null) {
				OnIAPValid(data);
			}
		}

        public delegate void IAPInvalid(string message);

        /// <summary>
        /// This event indicates that IAP validation for the IAP by the SLOT back-end failed.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/sdk-iap-packages-promotions/
        /// </summary>
		public event IAPInvalid OnIAPInvalid;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireIAPInvalid(string message) {
			SpilLogging.Log("fireIAPInvalid with data: " + message);
			if(OnIAPValid != null) {
				OnIAPInvalid(message);
			}
		}

        public delegate void IAPRequestPurchase(string skuId);

        /// <summary>
        /// This event indicates that an IAP request was fired from the SDK. The developer should take care of the purchase via their chosen IAP library.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/sdk-iap-packages-promotions/
        /// </summary>
		public event IAPRequestPurchase OnIAPRequestPurchase;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireIAPRequestPurchase(string skuId) {
			SpilLogging.Log("fireIAPRequestPurchase with sku: " + skuId);
			if(OnIAPRequestPurchase != null) {
				OnIAPRequestPurchase(skuId);
			}
		}

        public delegate void IAPServerError(SpilErrorMessage errorMessage);

        /// <summary>
        /// This event indicates that the SLOT back-end could not validate the IAP due to a server error.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/sdk-iap-packages-promotions/
        /// </summary>
		public event IAPServerError OnIAPServerError;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireIAPServerError(string error) {
			SpilLogging.Log("fireIAPServerError with data: " + error);
			SpilErrorMessage errorMessage = JsonHelper.getObjectFromJson<SpilErrorMessage>(error);
			if(OnIAPServerError != null) {
				OnIAPServerError(errorMessage);
			}
		}

        #endregion

        #region Server Time

        public delegate void ServerTimeRequestSuccess(long time);

        /// <summary>
        /// This event indicates that the server time request was successfull, it returns the server time as a timestamp.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/spil-sdk-anti-cheating/
        /// </summary>
		public event ServerTimeRequestSuccess OnServerTimeRequestSuccess;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireServerTimeRequestSuccess(string time) {
			SpilLogging.Log("fireServerTimeRequestSuccess with data: " + time);
			long longTime = long.Parse(time);
			if(OnServerTimeRequestSuccess != null) {
				OnServerTimeRequestSuccess(longTime);
			}
		}

        public delegate void ServerTimeRequestFailed(SpilErrorMessage errorMessage);

        /// <summary>
        /// This event indicates that the server time request has failed and the server time could not be retrieved.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/spil-sdk-anti-cheating/
        /// </summary>
		public event ServerTimeRequestFailed OnServerTimeRequestFailed;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireServerTimeRequestFailed(string error) {
			SpilLogging.Log("fireServerTimeRequestFailed with data: " + error);
			SpilErrorMessage errorMessage = JsonHelper.getObjectFromJson<SpilErrorMessage>(error);
			if(OnServerTimeRequestFailed != null) {
				OnServerTimeRequestFailed(errorMessage);
			}
		}

        #endregion

        #region Live Event

        public delegate void LiveEventAvailable();

        /// <summary>
        /// This event indicates that live event data is available and can be used.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/spil-sdk-live-events/
        /// </summary>
		public event LiveEventAvailable OnLiveEventAvailable;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireLiveEventAvailable() {
			SpilLogging.Log("fireLiveEventAvailable");
			if(OnLiveEventAvailable != null) {
				OnLiveEventAvailable();
			}
		}

        public delegate void LiveEventStageOpen();

        /// <summary>
        /// This event indicates that a web-view containing a live event stage has been opened.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/spil-sdk-live-events/
        /// </summary>
		public event LiveEventStageOpen OnLiveEventStageOpen;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireLiveEventStageOpen() {
			SpilLogging.Log("fireLiveEventStageOpen");
			if(OnLiveEventStageOpen != null) {
				OnLiveEventStageOpen();
			}
		}

        public delegate void LiveEventStageClosed();

        /// <summary>
        /// This event indicates that a web-view containing a live-event stage has been closed.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/spil-sdk-live-events/
        /// </summary>
		public event LiveEventStageClosed OnLiveEventStageClosed;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireLiveEventStageClosed() {
			SpilLogging.Log("fireLiveEventStageClosed");
			if(OnLiveEventStageClosed != null) {
				OnLiveEventStageClosed();
			}
		}

        public delegate void LiveEventNotAvailable();

        /// <summary>
        /// This event indicates that live event data is not available and cannot be used.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/spil-sdk-live-events/
        /// </summary>
		public event LiveEventNotAvailable OnLiveEventNotAvailable;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireLiveEventNotAvailable() {
			SpilLogging.Log("fireLiveEventNotAvailable");
			if(OnLiveEventNotAvailable != null) {
				OnLiveEventNotAvailable();
			}
		}

        public delegate void LiveEventError(SpilErrorMessage errorMessage);

        /// <summary>
        /// This event indicates that a web-view containing a live event screen threw an error.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/spil-sdk-live-events/
        /// </summary>
		public event LiveEventError OnLiveEventError;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireLiveEventError(string error) {
			SpilLogging.Log("fireLiveEventError with data: " + error);
			SpilErrorMessage errorMessage = JsonHelper.getObjectFromJson<SpilErrorMessage>(error);
			if(OnLiveEventError != null) {
				OnLiveEventError(errorMessage);
			}
		}

        public delegate void LiveEventUsedExternalItems(string items);

        /// <summary>
        /// This event indicates which items/currencies were used (consumed) while a web-view containg a live event was being shown.
        /// The developer can remove the items/currencies from the player's possessions.
        /// This is only required for external items/currencies (items/currencies not managed via the SpilSDK wallet/inventory).
        /// For items and currencies managed via the SpilSDK wallet/inventory the used items/currencies are subtracted automatically so this event is not needed.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/spil-sdk-live-events/
        /// </summary>
		public event LiveEventUsedExternalItems OnLiveEventUsedExternalItems;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireLiveEventUsedExternalItems(string items) {
			SpilLogging.Log("Used items = " + items);
			if(OnLiveEventUsedExternalItems != null) {
				OnLiveEventUsedExternalItems(items);
			}
		}

        public delegate void LiveEventReward(string rewardList);

        /// <summary>
        /// This event indicates that a web-view containing a live-event gave a reward to the user, this event contains the reward data.
        /// The developer can subscribe to this event and provide the reward to the user.
        /// This is only required for external items/currencies (items/currencies not managed via the SpilSDK wallet/inventory).
        /// For items and currencies managed via the SpilSDK wallet/inventory the reward is given automatically so this event is not needed.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/spil-sdk-live-events/
        /// </summary>
		public event LiveEventReward OnLiveEventReward;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireLiveEventReward(string receivedReward) {
			SpilLogging.Log("Received reward = " + receivedReward);
			if(OnLiveEventReward != null) {
				OnLiveEventReward(receivedReward);
			}
		}

        public delegate void LiveEventMetRequirements(bool metRequirements);

        /// <summary>
        /// This event indicates that the user met the requirements for a live event and will receive the live event reward.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/spil-sdk-live-events/
        /// </summary>
		public event LiveEventMetRequirements OnLiveEventMetRequirements;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireLiveEventMetRequirements(string sMetRequirements) {
			SpilLogging.Log("LiveEventMetRequirements with data = " + sMetRequirements);
			bool metRequirements = Convert.ToBoolean(sMetRequirements);
			if(OnLiveEventMetRequirements != null) {
				OnLiveEventMetRequirements(metRequirements);
			}
		}

        public delegate void LiveEventCompleted();

        /// <summary>
        /// This event indicates that a Live Event was completed by the user.
        /// The developer can subscribe to this event and hide any live-event related UI.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/spil-sdk-live-events/
        /// </summary>
		public event LiveEventCompleted OnLiveEventCompleted;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireLiveEventCompleted() {
			SpilLogging.Log("fireLiveEventCompleted");
			if(OnLiveEventCompleted != null) {
				OnLiveEventCompleted();
			}
		}

        #endregion

        #region Tiered Events

        public delegate void TieredEventsAvailable();

        /// <summary>
        /// This event indicates that tiered events data is available and can be used.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/tiered-events/
        /// </summary>
		public event TieredEventsAvailable OnTieredEventsAvailable;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireTieredEventsAvailable() {
			SpilLogging.Log("fireTieredEventsAvailable");
			if(OnTieredEventsAvailable != null) {
				OnTieredEventsAvailable();
			}
		}

        public delegate void TieredEventNotAvailable();

        /// <summary>
        /// This event indicates that tiered events data is not available and cannot be used.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/tiered-events/
        /// </summary>
		public event TieredEventNotAvailable OnTieredEventsNotAvailable;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireTieredEventsNotAvailable() {
			SpilLogging.Log("fireTieredEventsNotAvailable");
			if(OnTieredEventsNotAvailable != null) {
				OnTieredEventsNotAvailable();
			}
		}

        public delegate void TieredEventUpdated(TieredEventProgress tieredProgress);

        /// <summary>
        /// This event indicates that tiered event progress has been updated.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/tiered-events/
        /// </summary>
		public event TieredEventUpdated OnTieredEventUpdated;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireTieredEventUpdated(string data) {
			SpilLogging.Log("fireTieredEventUpdated with data = " + data);
			TieredEventProgress tieredProgress = JsonHelper.getObjectFromJson<TieredEventProgress>(data);
			if(OnTieredEventUpdated != null) {
				OnTieredEventUpdated(tieredProgress);
			}
		}

        public delegate void TieredEventsError(SpilErrorMessage error);

        /// <summary>
        /// This event indicates that an error occurred while updating tiered event progress or trying to claim a tier reward. Most likely there is an error in the tiered event's SLOT configuration or the SLOT back-end returned an error.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/tiered-events/
        /// </summary>
		public event TieredEventsError OnTieredEventsError;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireTieredEventsError(string data) {
			SpilLogging.Log("Tiered Events Error with reason = " + data);
			SpilErrorMessage errorMessage = JsonHelper.getObjectFromJson<SpilErrorMessage>(data);
			if(OnTieredEventsError != null) {
				OnTieredEventsError(errorMessage);
			}
		}

        public delegate void TieredEventProgressOpen();

        /// <summary>
        /// This event indicates that a web-view containing the tiered event progress screen has opened.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/tiered-events/
        /// </summary>
		public event TieredEventProgressOpen OnTieredEventProgressOpen;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireTieredEventProgressOpen() {
			SpilLogging.Log("fireTieredEventStageOpen");
			if(OnTieredEventProgressOpen != null) {
				OnTieredEventProgressOpen();
			}
		}
			
        public delegate void TieredEventProgressClosed();

        /// <summary>
        /// This event indicates that the web-view containing the tiered event progress screen has closed.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/tiered-events/
        /// </summary>
		public event TieredEventProgressClosed OnTieredEventProgressClosed;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireTieredEventProgressClosed() {
			SpilLogging.Log("fireTieredEventStageClosed");
			if(OnTieredEventProgressClosed != null) {
				OnTieredEventProgressClosed();
			}
		}

        #endregion

        #region Social Login

        public delegate void LoginSuccessful(bool resetData, string socialProvider, string socialId, bool isGuest);

        /// <summary>
        /// This event indicates that the social login was successful.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/working-social-login-feature/
        /// </summary>
		public event LoginSuccessful OnLoginSuccessful;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireLoginSuccessful(string message) {
			SpilLogging.Log("fireLoginSuccessful with message: " + message);

			JSONObject loginJSON = new JSONObject(message);
			bool resetData = loginJSON.GetField("resetData").b;
			string socialProvider = loginJSON.HasField("socialProvider") ? loginJSON.GetField("socialProvider").str : null;
			string socialId = loginJSON.HasField("socialId") ? loginJSON.GetField("socialId").str : null;
			bool isGuest = loginJSON.GetField("isGuest").b;
			if(OnLoginSuccessful != null) {
				OnLoginSuccessful(resetData, socialProvider, socialId, isGuest);
			}
		}

        public delegate void LoginFailed(SpilErrorMessage errorMessage);

        /// <summary>
        /// This event indicates that the social login failed.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/working-social-login-feature/
        /// </summary>
		public event LoginFailed OnLoginFailed;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireLoginFailed(string error) {
			SpilLogging.Log("fireLoginFailed with data: " + error);
			SpilErrorMessage errorMessage = JsonHelper.getObjectFromJson<SpilErrorMessage>(error);
			if(OnLoginFailed != null) {
				OnLoginFailed(errorMessage);
			}
		}

        public delegate void RequestLogin();

        /// <summary>
        /// This event indicates that social login has to be requested in order to log in.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/working-social-login-feature/
        /// </summary>
		public event RequestLogin OnRequestLogin;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireRequestLogin() {
			SpilLogging.Log("fireRequestLogin");
			if(OnRequestLogin != null) {
				OnRequestLogin();
			}
		}

        public delegate void LogoutSuccessful();

        /// <summary>
        /// This event indicates that social logout was successful.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/working-social-login-feature/
        /// </summary>
		public event LogoutSuccessful OnLogoutSuccessful;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireLogoutSuccessful() {
			SpilLogging.Log("fireLogoutSuccessful");
			if(OnLogoutSuccessful != null) {
				OnLogoutSuccessful();
			}
		}

        public delegate void LogoutFailed(SpilErrorMessage errorMessage);

        /// <summary>
        /// This event indicates that social logout failed.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/working-social-login-feature/
        /// </summary>
		public event LogoutFailed OnLogoutFailed;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// Developers can subscribe to events defined in Spil.Instance.
		/// </summary>
		public void fireLogoutFailed(string error) {
			SpilLogging.Log("fireLogoutFailed with data: " + error);
			SpilErrorMessage errorMessage = JsonHelper.getObjectFromJson<SpilErrorMessage>(error);
			if(OnLogoutFailed != null) {
				OnLogoutFailed(errorMessage);
			}
		}

        public delegate void AuthenticationError(SpilErrorMessage errorMessage);

        /// <summary>
        /// This event indicates that an authentication error occured. This can happen if the session of a logged in user is no longer valid, either because the session token has expired or due to a global logout on a different device.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/working-social-login-feature/
        /// </summary>
		public event AuthenticationError OnAuthenticationError;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireAuthenticationError(string error) {
			SpilLogging.Log("fireAuthenticationError with data: " + error);
			SpilErrorMessage errorMessage = JsonHelper.getObjectFromJson<SpilErrorMessage>(error);
			if(OnAuthenticationError != null) {
				OnAuthenticationError(errorMessage);
			}
		}

		public delegate void UserDataMergeConflict(MergeConflictData localDataMergeData, MergeConflictData remoteDataMergeData);

        /// <summary>
        /// This event indicates there was a merge conflict, local data and conflicted remote data are supplied so that the developer can choose how to resolve the merge conflict.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/working-social-login-feature/
        /// </summary>
		public event UserDataMergeConflict OnUserDataMergeConflict;

		/// This method is meant for internal use only, it should not be used by developers.
		/// Developers can subscribe to events defined in Spil.Instance.
		/// </summary>
		public void fireUserDataMergeConflict(string data) {
			SpilLogging.Log("fireUserDataMergeConflict");
			MergeConflict mergeConflict = JsonHelper.getObjectFromJson<MergeConflict>(data);
			if(OnUserDataMergeConflict != null) {
				OnUserDataMergeConflict(mergeConflict.localData, mergeConflict.remoteData);
			}
		}

		public delegate void UserDataMergeSuccessful();

        /// <summary>
        /// This event indicates that the user data merge was successfull and the merge conflict has been resolved.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/working-social-login-feature/
        /// </summary>
		public event UserDataMergeSuccessful OnUserDataMergeSuccessful;

		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireUserDataMergeSuccessful() {
			SpilLogging.Log("fireUserDataMergeSuccessful");

			if (Spil.PlayerData != null) {
				Spil.PlayerData.UpdatePlayerData();
			}
			if(OnUserDataMergeSuccessful != null) {
				OnUserDataMergeSuccessful();
			}
		}

		public delegate void UserDataMergeFailed(string mergeData, string mergeType);

        /// <summary>
        /// This event indicates that the user data merge has failed, the mergeData and mergeType parameters can be used to retry.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/working-social-login-feature/
        /// </summary>
		public event UserDataMergeFailed OnUserDataMergeFailed;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireUserDataMergeFailed(string data) {
			SpilLogging.Log("fireUserDataMergeFailed");

			JSONObject jsonData = new JSONObject(data);
			string mergeData = jsonData.HasField("mergeData") ? jsonData.GetField("mergeData").str : null;
			string mergeType = jsonData.HasField("mergeType") ? jsonData.GetField("mergeType").str : null;
			if(OnUserDataMergeFailed != null){
				OnUserDataMergeFailed(mergeData, mergeType);
			}
		}

		public delegate void UserDataHandleMerge(string mergeType);

        /// <summary>
        /// This event indicates that the user data merge has to be handled for a specific merge type: “remote”, “local”, or “merge.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/working-social-login-feature/
        /// </summary>
		public event UserDataHandleMerge OnUserDataHandleMerge;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireUserDataHandleMerge(string mergeType) {
			SpilLogging.Log("fireUserDataHandleMerge");
			if(OnUserDataHandleMerge != null) {
				OnUserDataHandleMerge(mergeType);
			}
		}

		public delegate void UserDataSyncError();

        /// <summary>
        /// This event indicates that there was a userdata synchronisation error. This can happen when a second device is being used to play the same game using the same Facebook ID.
        /// Call the RequestUserData() method to re-synchronize the locally stored user data. It is recommended that game play is suspended at this point, and does not resume until the user data has been successfully synchronized again.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/working-social-login-feature/
        /// </summary>
		public event UserDataSyncError OnUserDataSyncError;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireUserDataSyncError() {
			SpilLogging.Log("fireUserDataSyncError");
			if(OnUserDataSyncError != null) {
				OnUserDataSyncError();
			}
		}

		public delegate void UserDataLockError();

        /// <summary>
        /// This event indicates that there was a userdata lock error. This happens when a user plays on two devices at the same time. The first device applies a lock to prevent other devices from sending data while an update is in progress.
        /// It is recommended that the game waits and tries to resend the changes later.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/working-social-login-feature/
        /// </summary>
		public event UserDataLockError OnUserDataLockError;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireUserDataLockError() {
			SpilLogging.Log("fireUserDataLockError");
			if(OnUserDataLockError != null) {
				OnUserDataLockError();
			}
		}

		public delegate void UserDataError(SpilErrorMessage errorMessage);

        /// <summary>
        /// This event indicates that there was a general user data error and provides information about the error.
        /// A synchronization error can occur when user data is edited via SLOT. This indicates that the data on the SLOT back-end differs from the local data.
        /// Just as with a UserDataSyncError, solve the merge conflict by calling RequestUserData() to re-synchronize the locally stored user data. It is recommended that game play is suspended at this point, and does not resume until the user data has been successfully synchronized again.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/working-social-login-feature/
        /// </summary>
		public event UserDataError OnUserDataError;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireUserDataError(string sErrorMessage) {
			SpilLogging.Log("fireUserDataError with data: " + sErrorMessage);
			SpilErrorMessage errorMessage = JsonHelper.getObjectFromJson<SpilErrorMessage>(sErrorMessage);
			if(OnUserDataError != null){
				OnUserDataError(errorMessage);
			}
		}

		public delegate void UserDataAvailable();

        /// <summary>
        /// This event indicates that the user data is available and can be used.
        /// See also: http://www.spilgames.com/integration/unity/spil-sdk-features/working-social-login-feature/
        /// </summary>
		public event UserDataAvailable OnUserDataAvailable;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void fireUserDataAvailable() {
			SpilLogging.Log("fireUserDataAvailable");
			Spil.PlayerData.UpdatePlayerData();
			if(OnUserDataAvailable != null) {
				OnUserDataAvailable();
			}
		}

        #endregion

        #region Privacy Policy

        public delegate void PrivacyPolicyStatus(bool accepted);

        /// <summary>
        /// This event indicates whether the Privacy Policy was accepted by the user and is fired on each app start and/or after the privacy policy popup has been accepted.
        /// See also: http://www.spilgames.com/integration/unity/getting-started/gdpr-privacy-policy/
        /// </summary>
		public event PrivacyPolicyStatus OnPrivacyPolicyStatus;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void firePrivacyPolicyStatus(string sAccepted) {
			SpilLogging.Log("firePrivacyPolicyStatus");

			bool accepted = Convert.ToBoolean(sAccepted);

			if (accepted) {
				if (Spil.UseUnityPrefab) {
					Spil.Instance.SpilInit(false);
				} else {
					Spil.Instance.SpilInit(true);
				}
			}
			
			if(OnPrivacyPolicyStatus != null){
				OnPrivacyPolicyStatus(accepted);
			}
		}

        #endregion

#if UNITY_ANDROID

        #region Permission

        public delegate void PermissionResponse(
            SpilAndroidUnityImplementation.PermissionResponseObject permissionResponse);

        /// <summary>
        /// This event indicates the status of the Android permission request.
        /// Android only.
        /// </summary>
		public event PermissionResponse OnPermissionResponse;

		/// <summary>
		/// This method is meant for internal use only, it should not be used by developers.
		/// </summary>
		public void firePermissionResponse(string message) {
			SpilLogging.Log("Permission response with message: " + message);
			SpilAndroidUnityImplementation.PermissionResponseObject permissionResponse =
				JsonHelper.getObjectFromJson<SpilAndroidUnityImplementation.PermissionResponseObject>(message);
			if(OnPermissionResponse != null) {
				OnPermissionResponse(permissionResponse);
			}
		}

        #endregion

#endif

        #endregion

        #region Abstract Methods

        /// <summary>
        /// DO NOT USE THIS METHOD!
        /// This event is only used internally by the SDK
        /// Please use the designated methods or the new SpilTracking API
        /// </summary>
        internal abstract void SendCustomEventInternal(string eventName, Dictionary<string, object> eventParams = null);

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
                Spil.Instance.firePrivacyPolicyStatus("true");
            }  
        }

        public abstract void ShowPrivacyPolicySettings();

        public void TrackPrivacyPolicyChanged(bool withPersonalisedAds, bool withPersonalisedContent, string location, bool fromStartScreen) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("personalizedAds", withPersonalisedAds);
            dict.Add("personalizedContents", withPersonalisedContent);
            dict.Add("location", location);

            if (fromStartScreen) {
                dict.Add("acceptGDPR", true);
            }
            
            SendCustomEventInternal("privacyChanged", dict);
        }
        
        public abstract void SavePrivValue(int priv);

        public abstract int GetPrivValue();
        
        public abstract void SetPluginInformation(string PluginName, string PluginVersion);

        #endregion

        #region Config

	    public abstract void RequestGameConfig();
	    
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
	    /// Requests the daily bonus screen.
	    /// </summary>
	    public void ShowDailyBonus() {
		    if (DailyBonusHelper.DailyBonus.Type != null && DailyBonusHelper.DailyBonus.Type.Equals("assetBundle")) {
			    DailyBonusHelper.ShowDailyBonus();
		    } else {
			    ShowDailyBonusNative();
		    }
	    }

	    protected abstract void ShowDailyBonusNative();

	    public DailyBonusHelper GetDailyBonusHelper() {
		    return DailyBonusHelper;
	    }
	    
	    public abstract DailyBonus GetDailyBonusConfig();

	    public abstract void CollectDailyBonus();
	    
        /// <summary>
        /// Requests the splashscreen.
        /// </summary>
        public abstract void RequestSplashScreen(string type);

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

	    public abstract void SetCurrencyLimit(int currencyId, int limit);

	    public abstract void SetItemLimit(int itemId, int limit);
	    
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

        public abstract void UserLogin(string socialId, string socialProvider, string socialToken, Dictionary<string,object> socialValidationData = null);

        public abstract void UserLogout(bool global);

        public abstract void UserPlayAsGuest();

        public abstract void ShowUnauthorizedDialog(string title, string message, string loginText, 
            string playAsGuestText);

        public abstract bool IsLoggedIn();

        #endregion

        public abstract void ResetData();

        public abstract void ShowNativeDialog(string title, string message, string buttonText);

        #region AssetBundles

        public abstract void RequestAssetBundles();
        
        public abstract string GetAllAssetBundles();

        #endregion

        #endregion
    }
}

public class SpilLogging {
	public static void Log(string message) {
		Spil spil = GameObject.FindObjectOfType<Spil>();
		if (spil != null && spil.SpilLoggingEnabled) {
			Debug.Log("SpilSDK: " + message);
		}
	}

	public static void Error(string message) {
		Spil spil = GameObject.FindObjectOfType<Spil>();
		if (spil != null && spil.SpilLoggingEnabled) {
			Debug.LogError("SpilSDK: " + message);
		}
	}

	public static void Assert(bool condition, string message) {
		Spil spil = GameObject.FindObjectOfType<Spil>();
		if (spil != null && spil.SpilLoggingEnabled) {
			Debug.Assert(condition, "SpilSDK: " + message);
		}
	}
}