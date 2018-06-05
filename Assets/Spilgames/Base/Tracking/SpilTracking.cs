using System;
using System.Collections.Generic;
using SpilGames.Unity;
using SpilGames.Unity.Helpers.EventParams;
using SpilGames.Unity.Helpers.GameData;
using SpilGames.Unity.Json;

namespace SpilGames.Unity.Base.Implementations.Tracking {
    public class SpilTracking {
        public abstract class BaseTracking{
            protected string eventName;
            protected Dictionary<string, object> parameters = new Dictionary<string, object>();

            /// <summary>
            /// Method that should be used only in extraordinary circumstances.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public BaseTracking AddCustomParameter(string key, object value) {
                parameters.Add(key, value);
                return this;
            }
            
            /// <summary>
            /// Method is used to track the event that has been built.
            /// Should always be used as the last method in the API chain.
            /// </summary>
            public void Track() {
                Spil.Instance.SendCustomEventInternal(eventName, parameters);
            }         
        }

        public class BaseCustomEvent : BaseTracking {
            public BaseCustomEvent(string eventName) {
                this.eventName = eventName;
            }
        }
        
        /// <summary>
        /// Event that should only be used in extraordinary circumstances.
        /// </summary>
        /// <param name="eventName">The name of the event</param>
        /// <returns></returns>
        public static BaseCustomEvent CustomEvent(string eventName) {
            return new BaseCustomEvent(eventName);
        }
        
        public class BaseMilestoneAchieved : BaseTracking {
            public BaseMilestoneAchieved(string name) {
                eventName = "milestoneAchieved";
                parameters.Add("name", name);
            }

            /// <param name="description">Description of the milestone.</param>
            public BaseMilestoneAchieved AddMilestoneDescription(string description) {
                parameters.Add("description", description);
                return this;
            }
            
            /// <param name="score">The current score of the user when he achieved the milestone.</param>
            public BaseMilestoneAchieved AddScore(float score) {
                parameters.Add("score", score);
                return this;
            }
            
            /// <param name="location">The location where the milestone was achieved.</param>
            public BaseMilestoneAchieved AddLocation(string location) {
                parameters.Add("location", location);
                return this;
            }
            
            /// <param name="iteration">The number of times the milestone was achieved.</param>
            public BaseMilestoneAchieved AddIteration(int iteration) {
                parameters.Add("iteration", iteration);
                return this;
            }
        }
        
        /// <summary>
        /// Sends the "milestoneAchieved" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="name"></param>
        public static BaseMilestoneAchieved MilestoneAchieved(string name) {
            return new BaseMilestoneAchieved(name);
        }
        
        public class BaseLevelStart : BaseTracking {
            public BaseLevelStart(string level) {
                eventName = "levelStart";
                parameters.Add("level", level);
            }

            public BaseLevelStart AddLevelId(string levelId) {
                parameters.Add("levelId", levelId);
                return this;
            }
            
            /// <param name="difficulty">The difficulty of the started level.</param>
            public BaseLevelStart AddDifficulty(string difficulty) {
                parameters.Add("difficulty", difficulty);
                return this;
            }
            
            /// <param name="customCreated">If set to <c>true</c> custom created.</param>
            public BaseLevelStart AddCustomCreated(float customCreated) {
                parameters.Add("customCreated", customCreated);
                return this;
            }
            
            /// <param name="creatorId">Creator identifier.</param>
            public BaseLevelStart AddCreatorId(string creatorId) {
                parameters.Add("creatorId", creatorId);
                return this;
            }
            
            /// <param name="activeBooster">List of boosters which the user started the level.</param>
            public BaseLevelStart AddActiveBooster(List<string> activeBooster) {
                parameters.Add("activeBooster", new JSONObject(JsonHelper.getJSONFromObject(activeBooster)));
                return this;
            }            
        }
        
        /// <summary>
        /// Sends the "levelStart" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="level">The level name.</param>
        public static BaseLevelStart LevelStart(string level) {
            return new BaseLevelStart(level);
        }

        public class BaseLevelComplete : BaseTracking {
            public BaseLevelComplete(string level) {
                eventName = "levelComplete";
                parameters.Add("level", level);
            }

            public BaseLevelComplete AddLevelId(string levelId) {
                parameters.Add("levelId", levelId);
                return this;
            }

            /// <param name="difficulty">The difficulty of the started level.</param>
            public BaseLevelComplete AddDifficulty(string difficulty) {
                parameters.Add("difficulty", difficulty);
                return this;
            }
            
            /// <param name="score">The score achieved a the end of the level.</param>
            public BaseLevelComplete AddScore(float score) {
                parameters.Add("score", score);
                return this;
            }
            
            /// <param name="stars">The number of stars received at the end of the level.</param>
            public BaseLevelComplete AddStars(int stars) {
                parameters.Add("stars", stars);
                return this;
            }
            
            /// <param name="speed">The speed at which the level was completed.</param>
            public BaseLevelComplete AddSpeed(string speed) {
                parameters.Add("speed", speed);
                return this;
            }
            
            /// <param name="moves">The number of moves made to complete the level.</param>
            public BaseLevelComplete AddMoves(int moves) {
                parameters.Add("moves", moves);
                return this;
            }
            
            /// <param name="turns">The number of turns made to complete the level.</param>
            public BaseLevelComplete AddTurns(int turns) {
                parameters.Add("turns", turns);
                return this;
            }       
            
            /// <param name="customCreated">If the completed level was custome created.</param>
            public BaseLevelComplete AddCustomCreated(bool customCreated) {
                parameters.Add("customCreated", customCreated);
                return this;
            }
            
            /// <param name="creatorId">The id of the creator of the level.</param>
            public BaseLevelComplete AddCreatorId(string creatorId) {
                parameters.Add("creatorId", creatorId);
                return this;
            }
            
            /// <param name="objectUsed">A list of objects used for the level.</param>
            public BaseLevelComplete AddObjectUsed(List<LevelCompleteObjectUsed> objectUsed) {
                parameters.Add("objectUsed", new JSONObject(JsonHelper.getJSONFromObject(objectUsed)));
                return this;
            }
            
            /// <param name="achievement">The achievemt received for completing the level.</param>
            public BaseLevelComplete AddAchievement(string achievement) {
                parameters.Add("achievement", achievement);
                return this;
            }
            
            /// <param name="avgCombos">The average number of combos used for completing the level.</param>
            public BaseLevelComplete AddAvgCombos(float avgCombos) {
                parameters.Add("difficulty", avgCombos);
                return this;
            }
            
            /// <param name="movesLeft">The amount of moves left after completing the level.</param>
            public BaseLevelComplete AddMovesLeft(int movesLeft) {
                parameters.Add("movesLeft", movesLeft);
                return this;
            }
            
            /// <param name="rating">The rating gained for completing the level.</param>
            public BaseLevelComplete AddRating(string rating) {
                parameters.Add("rating", rating);
                return this;
            }
            
            /// <param name="timeLeft">The amount of time left after completing the level.</param>
            public BaseLevelComplete AddTimeLeft(int timeLeft) {
                parameters.Add("timeLeft", timeLeft);
                return this;
            }
        }
        
        /// <summary>
        /// Sends the "levelComplete" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="level">The level name.</param>
        public static BaseLevelComplete LevelComplete(string level) {
            return new BaseLevelComplete(level);
        }
        
        public class BaseLevelFailed : BaseTracking {
            public BaseLevelFailed(string level) {
                eventName = "levelFailed";
                parameters.Add("level", level);
            }

            public BaseLevelFailed AddLevelId(string levelId) {
                parameters.Add("levelId", levelId);
                return this;
            }
            
            /// <param name="difficulty">The difficulty of the started level.</param>
            public BaseLevelFailed AddDifficulty(string difficulty) {
                parameters.Add("difficulty", difficulty);
                return this;
            }
            
            /// <param name="score">The score achieved a the end of the level.</param>
            public BaseLevelFailed AddScore(float score) {
                parameters.Add("score", score);
                return this;
            }
            
            /// <param name="stars">The number of stars received at the end of the level.</param>
            public BaseLevelFailed AddStars(int stars) {
                parameters.Add("stars", stars);
                return this;
            }
            
            /// <param name="speed">The speed at which the level was completed.</param>
            public BaseLevelFailed AddSpeed(string speed) {
                parameters.Add("speed", speed);
                return this;
            }
            
            /// <param name="moves">The number of moves made to complete the level.</param>
            public BaseLevelFailed AddMoves(int moves) {
                parameters.Add("moves", moves);
                return this;
            }
            
            /// <param name="turns">The number of turns made to complete the level.</param>
            public BaseLevelFailed AddTurns(int turns) {
                parameters.Add("turns", turns);
                return this;
            }       
            
            /// <param name="customCreated">If the completed level was custome created.</param>
            public BaseLevelFailed AddCustomCreated(bool customCreated) {
                parameters.Add("customCreated", customCreated);
                return this;
            }
            
            /// <param name="creatorId">The id of the creator of the level.</param>
            public BaseLevelFailed AddCreatorId(string creatorId) {
                parameters.Add("creatorId", creatorId);
                return this;
            }
            
            /// <param name="objectUsed">A list of objects used for the level.</param>
            public BaseLevelFailed AddObjectUsed(List<LevelCompleteObjectUsed> objectUsed) {
                parameters.Add("objectUsed", new JSONObject(JsonHelper.getJSONFromObject(objectUsed)));
                return this;
            }
            
            /// <param name="achievement">The achievemt received for completing the level.</param>
            public BaseLevelFailed AddAchievement(string achievement) {
                parameters.Add("achievement", achievement);
                return this;
            }
            
            /// <param name="avgCombos">The average number of combos used for completing the level.</param>
            public BaseLevelFailed AddAvgCombos(float avgCombos) {
                parameters.Add("difficulty", avgCombos);
                return this;
            }
            
            /// <param name="movesLeft">The amount of moves left after completing the level.</param>
            public BaseLevelFailed AddMovesLeft(int movesLeft) {
                parameters.Add("movesLeft", movesLeft);
                return this;
            }
            
            /// <param name="rating">The rating gained for completing the level.</param>
            public BaseLevelFailed AddRating(string rating) {
                parameters.Add("rating", rating);
                return this;
            }
            
            /// <param name="timeLeft">The amount of time left after completing the level.</param>
            public BaseLevelFailed AddTimeLeft(int timeLeft) {
                parameters.Add("timeLeft", timeLeft);
                return this;
            }
        }
        
        /// <summary>
        /// Sends the "levelFailed" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="level">The level name.</param>
        public static BaseLevelFailed LevelFailed(string level) {
            return new BaseLevelFailed(level);
        }

        public class BaseLevelUp : BaseTracking {
            public BaseLevelUp(string level, string objectId) {
                eventName = "levelUp";
                parameters.Add("level", level);
                parameters.Add("objectId", objectId);
            }
            
            /// <param name="skillId">The identifier of the skill that has leveled up.</param>
            public BaseLevelUp AddSkillId(string skillId) {
                parameters.Add("skillId", skillId);
                return this;
            }
            
            /// <param name="sourceId"></param>
            public BaseLevelUp AddSourceId(string sourceId) {
                parameters.Add("sourceId", sourceId);
                return this;
            }
            
            /// <param name="sourceUniqueId"></param>
            public BaseLevelUp AddSourceUniqueId(string sourceUniqueId) {
                parameters.Add("sourceUniqueId", sourceUniqueId);
                return this;
            }
            
            /// <param name="objectUniqueId"></param>
            public BaseLevelUp AddObjectUniqueId(string objectUniqueId) {
                parameters.Add("objectUniqueId", objectUniqueId);
                return this;
            }
            
            /// <param name="objectUniqueIdType"></param>
            public BaseLevelUp AddObjectUniqueIdType(string objectUniqueIdType) {
                parameters.Add("objectUniqueIdType", objectUniqueIdType);
                return this;
            }
        }
        
        /// <summary>
        /// Sends the "levelUp" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="level">The level that has been reached by the player.</param>
        /// <param name="objectId">The id of the leveled up object.</param>
        /// <returns></returns>
        public static BaseLevelUp LevelUp(string level, string objectId) {
            return new BaseLevelUp(level, objectId);
        }

        public class BaseEquip : BaseTracking {
            public BaseEquip(string equippedItem) {
                eventName = "equip";
                parameters.Add("equippedItem", equippedItem);
            }
            
            /// <param name="equippedTo">The position on which the item was equiped to.</param>
            public BaseEquip AddEquippedTo(string equippedTo) {
                parameters.Add("equippedTo", equippedTo);
                return this;
            }
            
            /// <param name="unequippedFrom">The position from which the item was unequipped from.</param>
            public BaseEquip AddUnequippedFrom(string unequippedFrom) {
                parameters.Add("unequippedFrom", unequippedFrom);
                return this;
            }
        }
        
        /// <summary>
        /// Sends the "equip" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="equippedItem">The item that was equiped.</param>
        /// <returns></returns>
        public static BaseEquip Equip(string equippedItem) {
            return new BaseEquip(equippedItem);
        }

        public class BaseUpgrade : BaseTracking {
            public BaseUpgrade(string upgradeId, string level) {
                eventName = "upgrade";
                parameters.Add("upgradeId", upgradeId);
                parameters.Add("level", level);
            }
            
            /// <param name="reason">The reason for which the item was upgraded.</param>
            public BaseUpgrade AddReason(string reason) {
                parameters.Add("reason", reason);
                return this;
            }
            
            /// <param name="iteration">The number of iterations for the upgraded item.</param>
            public BaseUpgrade AddIteration(int iteration) {
                parameters.Add("iteration", iteration);
                return this;
            }
            
            /// <param name="achievement">The achievement gained for upgrading the item.</param>
            public BaseUpgrade AddAchievement(string achievement) {
                parameters.Add("achievement", achievement);
                return this;
            }
            
            /// <param name="key"></param>
            public BaseUpgrade AddKey(string key) {
                parameters.Add("key", key);
                return this;
            }
        }
        
        /// <summary>
        /// Sends the "equip" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="upgradeId">The id of the item that has been upgraded.</param>
        /// <param name="level">The level of the item that was upgraded</param>
        /// <returns></returns>
        public static BaseUpgrade Upgrade(string upgradeId, string level) {
            return new BaseUpgrade(upgradeId, level);
        }

        public class BaseLevelCreate : BaseTracking {
            public BaseLevelCreate(string levelId, string level, string creatorId) {
                eventName = "levelCreate";
                parameters.Add("levelId", levelId);
                parameters.Add("level", level);
                parameters.Add("creatorId", creatorId);
            }
        }
        
        /// <summary>
        /// Sends the "levelCreate" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="levelId">The id of the created level.</param>
        /// <param name="level">The name of the level crearted.</param>
        /// <param name="creatorId">The id of the entity which created the level.</param>
        /// <returns></returns>
        public static BaseLevelCreate LevelCreate(string levelId, string level, string creatorId) {
            return new BaseLevelCreate(levelId, level, creatorId);
        }

        public class BaseLevelDownload : BaseTracking {
            public BaseLevelDownload(string levelId, string creatorId) {
                eventName = "levelDownload";
                parameters.Add("levelId", levelId);
                parameters.Add("creatorId", creatorId);
            }
            
            /// <param name="rating">The rating of the downloaded level.</param>
            public BaseLevelDownload AddRating(float rating) {
                parameters.Add("rating", rating);
                return this;
            }
        }
        
        /// <summary>
        /// Sends the "levelDownload" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="levelId">The id of the downloaded level.</param>
        /// <param name="creatorId">The id of the entity which created the level.</param>
        /// <returns></returns>
        public static BaseLevelDownload LevelDownload(string levelId, string creatorId) {
            return new BaseLevelDownload(levelId, creatorId);
        }
        
        public class BaseLevelRate : BaseTracking {
            public BaseLevelRate(string levelId, string creatorId) {
                eventName = "levelRate";
                parameters.Add("levelId", levelId);
                parameters.Add("creatorId", creatorId);
            }
            
            /// <param name="rating">The rating given to the level.</param>
            public BaseLevelRate AddRating(float rating) {
                parameters.Add("rating", rating);
                return this;
            }
        }
        
        /// <summary>
        /// Sends the "levelRate" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="levelId">The id of the rated level.</param>
        /// <param name="creatorId">The id of the entity which created the level.</param>
        /// <returns></returns>
        public static BaseLevelRate LevelRate(string levelId, string creatorId) {
            return new BaseLevelRate(levelId, creatorId);
        }

        public class BaseEndlessModeStart : BaseTracking {
            public BaseEndlessModeStart() {
                eventName = "endlessModeStart";
            }
        }
        
        /// <summary>
        /// Sends the "endlessModeStart" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <returns></returns>
        public static BaseEndlessModeStart EndlessModeStart() {
            return new BaseEndlessModeStart();
        }
        
        public class BaseEndlessModeEnd : BaseTracking {
            public BaseEndlessModeEnd(int distance) {
                eventName = "endlessModeEnd";
                parameters.Add("distance", distance);
            }
        }
        
        /// <summary>
        /// Sends the "endlessModeEnd" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="distance">The distance that was covered by the user.</param>
        /// <returns></returns>
        public static BaseEndlessModeEnd EndlessModeEnd(int distance) {
            return new BaseEndlessModeEnd(distance);
        }
        
        public class BasePlayerDies : BaseTracking {
            public BasePlayerDies(string level) {
                eventName = "playerDies";
                parameters.Add("level", level);
            }
        }
        
        /// <summary>
        /// Sends the "playerDies" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="level">The level in which the player died.</param>
        /// <returns></returns>
        public static BasePlayerDies PlayerDies(string level) {
            return new BasePlayerDies(level);
        }

        public class BaseWalletInventoryEvent : BaseTracking {
            public BaseWalletInventoryEvent(string reason, string location) {
                eventName = "updatePlayerData";
                parameters.Add("reason", reason);
                parameters.Add("location", location);
                parameters.Add("trackingOnly", true);
            }


            /// <param name="currencyList">A list of TrackingCurrency objects that defines all the currencies that have been changed with this event. This parameter can also be omited if no currencies have been updated</param>
            public BaseWalletInventoryEvent AddWallet(List<TrackingCurrency> currencyList) {
                Dictionary<string, object> wallet = new Dictionary<string, object>();
                wallet.Add("currencies", new JSONObject(JsonHelper.getJSONFromObject(currencyList)));
                wallet.Add("offset", 0);
                parameters.Add("wallet", wallet);
                return this;
            }

            /// <param name="itemsList">A list of TrackingItems objects that defines all the items that have been changed with this event. This parameter can also be omited if no items have been updated</param>
            public BaseWalletInventoryEvent AddInventory(List<TrackingItem> itemsList) {
                Dictionary<string, object> inventory = new Dictionary<string, object>();
                inventory.Add("items", new JSONObject(JsonHelper.getJSONFromObject(itemsList)));
                inventory.Add("offset", 0);
                parameters.Add("inventory", inventory);
                return this;
            }
            
            /// <param name="reasonDetails">Additional parameter used to describe the details of why the event happened</param>
            public BaseWalletInventoryEvent AddReasonDetails(string reasonDetails) {
                parameters.Add("reasonDetails", reasonDetails);
                return this;
            }
            
            /// <param name="transactionId">The transactionId if the update was due to an IAP</param>
            public BaseWalletInventoryEvent AddTransactionId(string transactionId) {
                parameters.Add("transactionId", transactionId);
                return this;
            }
        }
        
        /// <summary>
        /// Sends the "updatePlayerData" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// DO NOT USE THIS EVENT IF YOU IMPLEMENTED WALLET AND INVENTORY FEATURES!
        /// </summary>
        /// <param name="reason">The reason for which the event occured. You can also use the PlayerDataUpdateReasons class to pass one of the default reasons</param>
        /// <param name="location">The location where the event occured (ex.: Shop Screen, End of the level)</param>
        /// <returns></returns>
        public static BaseWalletInventoryEvent WalletInventoryEvent(string reason, string location) {
            return new BaseWalletInventoryEvent(reason, location);
        }

        public class BaseIAPPurchased : BaseTracking {
            public BaseIAPPurchased(string skuId, string transactionId) {
                eventName = "iapPurchased";
                parameters.Add("skuId", skuId);
                parameters.Add("transactionId", transactionId);
                parameters.Add("purchaseDate", DateTime.Now.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"));
            }
            
            /// <param name="token">The transaction token returned from the purchase. Only use this for ANDROID!</param>
            public BaseIAPPurchased AddToken(string token) {
                if (!token.Equals("")) {
                    parameters.Add("token", token); 
                }
                
                return this;
            }
            
            /// <param name="reason">The reason for which the IAP purchase was done.</param>
            public BaseIAPPurchased AddReason(string reason) {
                parameters.Add("reason", reason);
                return this;
            }
            
            /// <param name="location">The location where the IAP was done.</param>
            public BaseIAPPurchased AddLocation(string location) {
                parameters.Add("location", location);
                return this;
            }
        }
        
        /// <summary>
        /// Sends the "iapPurchased" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="skuId">The product identifier of the item that was purchased</param>
        /// <param name="transactionId">The transaction identifier of the item that was purchased (also called orderId)</param>
        /// <returns></returns>
        public static BaseIAPPurchased IAPPurchased(string skuId, string transactionId) {
            return new BaseIAPPurchased(skuId, transactionId);
        }

        public class BaseIAPRestored : BaseTracking {
            public BaseIAPRestored(string skuId, string originalTransactionId, string originalPurchaseDate) {
                eventName = "iapRestored";
                parameters.Add("skuId", skuId);
                parameters.Add("originalTransactionId", originalTransactionId);
                parameters.Add("originalPurchaseDate", originalPurchaseDate);
            }
            
            /// <param name="reason">The reason for which the IAP restore was done.</param>
            public BaseIAPRestored AddReason(string reason) {
                parameters.Add("reason", reason);
                return this;
            }
        }
        
        /// <summary>
        /// Sends the "iapRestored" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="skuId">The product identifier of the item that was purchased</param>
        /// <param name="originalTransactionId">For a transaction that restores a previous transaction, the transaction identifier of the original transaction. Otherwise, identical to the transaction identifier</param>
        /// <param name="originalPurchaseDate">For a transaction that restores a previous transaction, the date of the original transaction. Please use a proper DateTime format (RFC3339), for instance: "2016-08-30T11:54:48.5247936+02:00". If you have a DateTime object you can use: DateTimeObject.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz")</param>
        /// <returns></returns>
        public static BaseIAPRestored IAPRestored(string skuId, string originalTransactionId, string originalPurchaseDate) {
            return new BaseIAPRestored(skuId, originalTransactionId, originalPurchaseDate);
        }
      
        public class BaseIAPFailed : BaseTracking {
            public BaseIAPFailed(string skuId, string errorDescription) {
                eventName = "iapFailed";
                parameters.Add("skuId", skuId);
                parameters.Add("errorDescription", errorDescription);
                parameters.Add("purchaseDate", DateTime.Now.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"));
            }
            
            /// <param name="reason">The reason for which the IAP failed.</param>
            public BaseIAPFailed AddReason(string reason) {
                parameters.Add("reason", reason);
                return this;
            }
            
            /// <param name="location">The location where the IAP failed.</param>
            public BaseIAPFailed AddLocation(string location) {
                parameters.Add("location", location);
                return this;
            }
        }
        
        /// <summary>
        /// Sends the "iapFailed" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="skuId">The product identifier of the item that was purchased</param>
        /// <param name="errorDescription">Error description or error code</param>
        /// <returns></returns>
        public static BaseIAPFailed IAPFailed(string skuId, string errorDescription) {
            return new BaseIAPFailed(skuId, errorDescription);
        }
        
        public class BaseTutorialComplete : BaseTracking {
            public BaseTutorialComplete() {
                eventName = "tutorialComplete";
            }
        }
        
        /// <summary>
        /// Sends the "tutorialComplete" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <returns></returns>
        public static BaseTutorialComplete TutorialComplete() {
            return new BaseTutorialComplete();
        }
        
        public class BaseTutorialSkipped : BaseTracking {
            public BaseTutorialSkipped() {
                eventName = "tutorialSkipped";
            }
        }
        
        /// <summary>
        /// Sends the "tutorialSkipped" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <returns></returns>
        public static BaseTutorialSkipped TutorialSkipped() {
            return new BaseTutorialSkipped();
        }
        
        public class BaseRegister : BaseTracking {
            public BaseRegister(string platform) {
                eventName = "register";
                parameters.Add("platform", platform);
            }
        }
        
        /// <summary>
        /// Sends the "tutorialSkipped" event to the native Spil SDK which will send a request to the back-end.
        /// Should be called after the user completes registration via email, Facebook, Google Plus or other available option. Registration option is assumed.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="platform">A string like ‘facebook’ or ’email’</param>
        /// <returns></returns>
        public static BaseRegister Register(string platform) {
            return new BaseRegister(platform);
        }
        
        public class BaseShare : BaseTracking {
            public BaseShare(string platform) {
                eventName = "share";
                parameters.Add("platform", platform);
            }
            
            /// <param name="reason">The reason for which share was done.</param>
            public BaseShare AddReason(string reason) {
                parameters.Add("reason", reason);
                return this;
            }
            
            /// <param name="location">The location where the share action occured.</param>
            public BaseShare AddLocation(string location) {
                parameters.Add("location", location);
                return this;
            }
        }
        
        /// <summary>
        /// Sends the "share" event to the native Spil SDK which will send a request to the back-end.
        /// Should be called every time the user shares content on their social media accounts. Social media integration is assumed.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="platform">A string like ‘facebook’ or ’email’</param>
        /// <returns></returns>
        public static BaseShare Share(string platform) {
            return new BaseShare(platform);
        }
        
        public class BaseInvite : BaseTracking {
            public BaseInvite(string platform) {
                eventName = "invite";
                parameters.Add("platform", platform);
            }        
            
            /// <param name="location">The location where the invite action occured.</param>
            public BaseInvite AddLocation(string location) {
                parameters.Add("location", location);
                return this;
            }
        }
        
        /// <summary>
        /// Sends the "invite" event to the native Spil SDK which will send a request to the back-end.
        /// Should be called every time the user invites another user. Respective function in-game is assumed.
        /// See https://github.com/spilgames/spil_event_unity_plugin for more information on events.
        /// </summary>
        /// <param name="platform">A string like ‘facebook’ or ’email’</param>
        /// <returns></returns>
        public static BaseInvite Invite(string platform) {
            return new BaseInvite(platform);
        }
        
        public class BaseLevelAppeared : BaseTracking {
            public BaseLevelAppeared(string level) {
                eventName = "levelAppeared";
                parameters.Add("level", level);
            }        
            
            /// <param name="difficulty">The difficulty of the level that appeared.</param>
            public BaseLevelAppeared AddDifficulty(string difficulty) {
                parameters.Add("difficulty", difficulty);
                return this;
            }
        }
        
        /// <summary>
        /// Sends the "levelAppeared" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="level">The name/id of the level that appeared.</param>
        /// <returns></returns>
        public static BaseLevelAppeared LevelAppeared(string level) {
            return new BaseLevelAppeared(level);
        }
        
        public class BaseLevelDiscarded : BaseTracking {
            public BaseLevelDiscarded(string level) {
                eventName = "levelDiscarded";
                parameters.Add("level", level);
            }        
            
            /// <param name="difficulty">The difficulty of the level that was discarded.</param>
            public BaseLevelDiscarded AddDifficulty(string difficulty) {
                parameters.Add("difficulty", difficulty);
                return this;
            }
        }
        
        /// <summary>
        /// Sends the "levelDiscarded" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="level">The name/id of the level that appeared.</param>
        /// <returns></returns>
        public static BaseLevelDiscarded LevelDiscarded(string level) {
            return new BaseLevelDiscarded(level);
        }
        
        public class BaseErrorShown : BaseTracking {
            public BaseErrorShown(string reason) {
                eventName = "errorShown";
                parameters.Add("reason", reason);
            }        
        }
        
        /// <summary>
        /// Sends the "errorShown" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="reason">The reason for the error to be shown.</param>
        /// <returns></returns>
        public static BaseErrorShown ErrorShown(string reason) {
            return new BaseErrorShown(reason);
        }
        
        public class BaseTimeElapLoad : BaseTracking {
            public BaseTimeElapLoad(int timeElap, string pointInGame) {
                eventName = "timeElapLoad";
                parameters.Add("timeElap", timeElap);
                parameters.Add("pointInGame", pointInGame);
            }
            
            /// <param name="startPoint">The point in game which we start to measure time.</param>
            public BaseTimeElapLoad AddStartPoint(double startPoint) {
                parameters.Add("startPoint", startPoint);
                return this;
            }
        }
        
        /// <summary>
        /// Sends the "timeElapLoad" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="timeElap">The time elapsed between the starting point and the pointInGame in seconds.</param>
        /// <param name="pointInGame">The point in game which is reached.</param>
        /// <returns></returns>
        public static BaseTimeElapLoad TimeElapLoad(int timeElap, string pointInGame) {
            return new BaseTimeElapLoad(timeElap, pointInGame);
        }
        
        public class BaseTimeoutDetected : BaseTracking {
            public BaseTimeoutDetected(int timeElap, string pointInGame) {
                eventName = "timeoutDetected";
                parameters.Add("timeElap", timeElap);
                parameters.Add("pointInGame", pointInGame);
            }
        }
        
        /// <summary>
        /// Sends the "timeoutDetected" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="timeElap">The time elapsed between the starting point and the pointInGame in seconds.</param>
        /// <param name="pointInGame">The point in game which is reached.</param>
        /// <returns></returns>
        public static BaseTimeoutDetected TimeoutDetected(int timeElap, string pointInGame) {
            return new BaseTimeoutDetected(timeElap, pointInGame);
        }

        public class BaseObjectStateChanged : BaseTracking {
            public BaseObjectStateChanged(string changedObject, string status, string reason) {
                eventName = "objectStateChanged";
                parameters.Add("changedObject", changedObject);
                parameters.Add("status", status);
                parameters.Add("reason", reason);
            }
            
            /// <param name="changedProperties">The properties that were changed when the object changed</param>
            public BaseObjectStateChanged AddChangedProperties(string changedProperties) {
                parameters.Add("changedProperties", changedProperties);
                return this;
            }
            
            /// <param name="optionConditions"></param>
            public BaseObjectStateChanged AddOptionConditions(string optionConditions) {
                parameters.Add("optionConditions", optionConditions);
                return this;
            }
            
            /// <param name="situation"></param>
            public BaseObjectStateChanged AddSituation(string situation) {
                parameters.Add("situation", situation);
                return this;
            }
            
            /// <param name="allChoiceResults"></param>
            public BaseObjectStateChanged AddAllChoiceResults(string allChoiceResults) {
                parameters.Add("allChoiceResults", allChoiceResults);
                return this;
            }
            
            /// <param name="allSelectedChoices"></param>
            public BaseObjectStateChanged AddAllSelectedChoices(string allSelectedChoices) {
                parameters.Add("allSelectedChoices", allSelectedChoices);
                return this;
            }
            
            /// <param name="involvedParties"></param>
            public BaseObjectStateChanged AddInvolvedParties(string involvedParties) {
                parameters.Add("involvedParties", involvedParties);
                return this;
            }
        }
        
        /// <summary>
        /// Sends the "objectStateChanged" event to the native Spil SDK which will send a request to the back-end.
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        /// <param name="changedObject">The object which changed it's state.</param>
        /// <param name="status">The new status the object is in.</param>
        /// <param name="reason">The reason for the state change.</param>
        /// <returns></returns>
        public static BaseObjectStateChanged ObjectStateChanged(string changedObject, string status, string reason) {
            return new BaseObjectStateChanged(changedObject, changedObject, reason);
        }
        
        public class BaseUIElementClicked : BaseTracking {
            public BaseUIElementClicked(string element) {
                eventName = "uiElementClicked";
                parameters.Add("element", element);
            }        
            
            /// <param name="type">The type of element that was clicked (eg.: Button, Icon, etc.)</param>
            public BaseUIElementClicked AddType(string type) {
                parameters.Add("type", type);
                return this;
            }
            
            /// <param name="screenName">The screen name where the click occurred.</param>
            public BaseUIElementClicked AddScreenName(string screenName) {
                parameters.Add("screenName", screenName);
                return this;
            }
            
            /// <param name="location">The location within the screen where the click occurred.</param>
            public BaseUIElementClicked AddLocation(string location) {
                parameters.Add("location", location);
                return this;
            }
            
            /// <param name="grade"></param>
            public BaseUIElementClicked AddGrade(int grade) {
                parameters.Add("grade", grade);
                return this;
            }
        }
        
        /// <summary>
        /// Triggered when the user clicks a specific ui element
        /// </summary>
        /// <param name="element">The name of the element that was clicked in the game</param>
        /// <returns></returns>
        public static BaseUIElementClicked UIElementClicked(string element) {
            return new BaseUIElementClicked(element);
        }
        
        public class BaseSendGift : BaseTracking {
            public BaseSendGift(string platform) {
                eventName = "sendGift";
                parameters.Add("platform", platform);
            }        
            
            /// <param name="location">The location where the gift action occured.</param>
            public BaseSendGift AddLocation(string location) {
                parameters.Add("location", location);
                return this;
            }
        }
        
        /// <summary>
        /// Triggered when a user sends a gift
        /// </summary>
        /// <param name="platform">A string like ‘facebook’ or ’email’</param>
        /// <returns></returns>
        public static BaseSendGift SendGift(string platform) {
            return new BaseSendGift(platform);
        }
        
        public class BaseLevelTimeOut : BaseTracking {
            public BaseLevelTimeOut() {
                eventName = "levelTimeOut";
            }
        }
        
        /// <summary>
        /// Triggered when the level timer finishes
        /// </summary>
        /// <returns></returns>
        public static BaseLevelTimeOut LevelTimeOut() {
            return new BaseLevelTimeOut();
        }

        public class BaseDialogueChosen : BaseTracking {
            public BaseDialogueChosen(string name, string choice, bool hasToken, bool isPremiumChoice, bool isQuizz, bool isForced, bool isTimed) {
                eventName = "dialogueChosen";
                parameters.Add("name", name);
                parameters.Add("choice", choice);
                parameters.Add("hasToken", hasToken);
                parameters.Add("isPremiumChoice", isPremiumChoice);
                parameters.Add("isQuizz", isQuizz);
                parameters.Add("isForced", isForced);
                parameters.Add("isTimed", isTimed);
            }  
            
            /// <param name="time">The time it took for the choice to be made.</param>
            public BaseDialogueChosen AddTime(int time) {
                parameters.Add("time", time);
                return this;
            }
        }
        
        /// <summary>
        /// Triggered whenever user selects an choice for a dialog,or runs out of time and default answer is selected	
        /// </summary>
        /// <param name="name">The name of the dialogue.</param>
        /// <param name="choice">The choice that the play has made for this dialogue.</param>
        /// <param name="hasToken">Specifies if the dialogue choice had a token.</param>
        /// <param name="isPremiumChoice">Defines if the choice was a premium one.</param>
        /// <param name="isQuizz"></param>
        /// <param name="isForced"></param>
        /// <param name="isTimed">Specifices if the dialogue choice was timed.</param>
        /// <returns></returns>
        public static BaseDialogueChosen DialogueChosen(string name, string choice, bool hasToken, bool isPremiumChoice, bool isQuizz, bool isForced, bool isTimed) {
            return new BaseDialogueChosen(name, choice, hasToken, isPremiumChoice, isQuizz, isForced, isTimed);
        }
        
        public class BaseFriendAdded : BaseTracking {
            public BaseFriendAdded(string friend) {
                eventName = "friendAdded";
                parameters.Add("friend", friend);
            }        
            
            /// <param name="platform">A string like ‘facebook’ or ’email’</param>
            public BaseFriendAdded AddPlatform(string platform) {
                parameters.Add("platform", platform);
                return this;
            }
        }
        
        /// <summary>
        /// Triggered when the user is adding a friend in the game.
        /// </summary>
        /// <param name="friend">The id of the friend that was added.</param>
        /// <returns></returns>
        public static BaseSendGift FriendAdded(string friend) {
            return new BaseSendGift(friend);
        }
        
        public class BaseGameObjectInteraction : BaseTracking {
            public BaseGameObjectInteraction() {
                eventName = "gameObjectInteraction";
            }
        }
        
        /// <summary>
        /// Triggered for special game object interactions
        /// </summary>
        /// <returns></returns>
        public static BaseGameObjectInteraction GameObjectInteraction() {
            return new BaseGameObjectInteraction();
        }

        public class BaseGameResult : BaseTracking {
            public BaseGameResult() {
                eventName = "gameResult";
            }
            
            /// <param name="itemId"></param>
            public BaseGameResult AddItemId(string itemId) {
                parameters.Add("itemId", itemId);
                return this;
            }
            
            /// <param name="itemType"></param>
            public BaseGameResult AddItemType(string itemType) {
                parameters.Add("itemType", itemType);
                return this;
            }
            
            /// <param name="label"></param>
            public BaseGameResult AddLabel(string label) {
                parameters.Add("label", label);
                return this;
            }
            
            /// <param name="matchId"></param>
            public BaseGameResult AddMatchId(string matchId) {
                parameters.Add("matchId", matchId);
                return this;
            }
        }
        
        /// <summary>
        /// Triggered at the end of a match and indicates the result of it
        /// </summary>
        /// <returns></returns>
        public static BaseGameResult GameResult() {
            return new BaseGameResult();
        }
        
        public class BaseItemCrafted : BaseTracking {
            public BaseItemCrafted(string itemId) {
                eventName = "itemCrafted";
                parameters.Add("itemId", itemId);
            }        
            
            /// <param name="itemType">The type of item that was crafted.</param>
            public BaseItemCrafted AddItemType(string itemType) {
                parameters.Add("itemType", itemType);
                return this;
            }
        }
        
        /// <summary>
        /// Triggered when a user is crafting an item.
        /// </summary>
        /// <param name="itemId">The id of the item that was crafted.</param>
        /// <returns></returns>
        public static BaseItemCrafted ItemCrafted(string itemId) {
            return new BaseItemCrafted(itemId);
        }
        
        public class BaseItemCreated : BaseTracking {
            public BaseItemCreated(string itemId) {
                eventName = "itemCreated";
                parameters.Add("itemId", itemId);
            }        
            
            /// <param name="itemType">The type of item that was created.</param>
            public BaseItemCreated AddItemType(string itemType) {
                parameters.Add("itemType", itemType);
                return this;
            }
        }
        
        /// <summary>
        /// Triggered when a user creates an item.
        /// </summary>
        /// <param name="itemId">The id of the item that was created.</param>
        /// <returns></returns>
        public static BaseItemCreated ItemCreated(string itemId) {
            return new BaseItemCreated(itemId);
        }

        public class BaseItemUpdated : BaseTracking {
            public BaseItemUpdated(string content, string itemId) {
                eventName = "itemUpdated";
                parameters.Add("content", content);
                parameters.Add("itemId", itemId);
            }
            
            /// <param name="itemType">The type of item that was updated.</param>
            public BaseItemUpdated AddItemType(string itemType) {
                parameters.Add("itemType", itemType);
                return this;
            }
        }
        
        /// <summary>
        /// Triggered when the user is updating an item (e.g. equipping a character with an item).
        /// </summary>
        /// <param name="content">The content in which the item was updated.</param>
        /// <param name="itemId">The id of the item that was updated.</param>
        /// <returns></returns>
        public static BaseItemUpdated ItemUpdated(string content, string itemId) {
            return new BaseItemUpdated(content, itemId);
        }
        
        public class BaseDeckUpdated : BaseTracking {
            public BaseDeckUpdated(string content, string itemId) {
                eventName = "deckUpdated";
                parameters.Add("content", content);
                parameters.Add("itemId", itemId);
            }
            
            /// <param name="itemType">The type of deck that was updated.</param>
            public BaseDeckUpdated AddItemType(string itemType) {
                parameters.Add("itemType", itemType);
                return this;
            }
            
            /// <param name="label">The label given to the updated deck.</param>
            public BaseDeckUpdated AddLabel(string label) {
                parameters.Add("label", label);
                return this;
            }
        }
        
        /// <summary>
        /// Triggered when the user is updating his deck, e.g. by adding and removing cards from it.
        /// </summary>
        /// <param name="content">The content in which the deck was updated.</param>
        /// <param name="itemId">The id of the deck that was updated.</param>
        /// <returns></returns>
        public static BaseDeckUpdated DeckUpdated(string content, string itemId) {
            return new BaseDeckUpdated(content, itemId);
        }
        
        public class BaseMatchComplete : BaseTracking {
            public BaseMatchComplete() {
                eventName = "matchComplete";
            }
            
            /// <param name="matchId">The id of the match completed.</param>
            public BaseMatchComplete AddMatchId(string matchId) {
                parameters.Add("matchId", matchId);
                return this;
            }
            
            /// <param name="itemId"></param>
            public BaseMatchComplete AddItemId(string itemId) {
                parameters.Add("itemId", itemId);
                return this;
            }
            
            /// <param name="itemType"></param>
            public BaseMatchComplete AddItemType(string itemType) {
                parameters.Add("itemType", itemType);
                return this;
            }
            
            /// <param name="label"></param>
            public BaseMatchComplete AddLabel(string label) {
                parameters.Add("label", label);
                return this;
            }
        }
        
        /// <summary>
        /// Triggered when a player-vs-player match ends 
        /// </summary>
        /// <returns></returns>
        public static BaseMatchComplete MatchComplete() {
            return new BaseMatchComplete();
        }
        
        public class BaseMatchLost : BaseTracking {
            public BaseMatchLost() {
                eventName = "matchLost";
            }
            
            /// <param name="matchId">The id of the match lost.</param>
            public BaseMatchLost AddMatchId(string matchId) {
                parameters.Add("matchId", matchId);
                return this;
            }
            
            /// <param name="itemId"></param>
            public BaseMatchLost AddItemId(string itemId) {
                parameters.Add("itemId", itemId);
                return this;
            }
            
            /// <param name="itemType"></param>
            public BaseMatchLost AddItemType(string itemType) {
                parameters.Add("itemType", itemType);
                return this;
            }
            
            /// <param name="label"></param>
            public BaseMatchLost AddLabel(string label) {
                parameters.Add("label", label);
                return this;
            }
        }
        
        /// <summary>
        /// Triggered when a user loses a player-vs-player match
        /// </summary>
        /// <returns></returns>
        public static BaseMatchLost MatchLost() {
            return new BaseMatchLost();
        }
        
        public class BaseMatchTie : BaseTracking {
            public BaseMatchTie() {
                eventName = "matchTie";
            }
            
            /// <param name="matchId">The id of the match tied.</param>
            public BaseMatchTie AddMatchId(string matchId) {
                parameters.Add("matchId", matchId);
                return this;
            }
            
            /// <param name="itemId"></param>
            public BaseMatchTie AddItemId(string itemId) {
                parameters.Add("itemId", itemId);
                return this;
            }
            
            /// <param name="itemType"></param>
            public BaseMatchTie AddItemType(string itemType) {
                parameters.Add("itemType", itemType);
                return this;
            }
            
            /// <param name="label"></param>
            public BaseMatchTie AddLabel(string label) {
                parameters.Add("label", label);
                return this;
            }
        }
        
        /// <summary>
        /// Triggered when a player-vs-player match ends with a tie
        /// </summary>
        /// <returns></returns>
        public static BaseMatchTie MatchTie() {
            return new BaseMatchTie();
        }
        
        public class BaseMatchWon : BaseTracking {
            public BaseMatchWon() {
                eventName = "matchWon";
            }
            
            /// <param name="matchId">The id of the match won.</param>
            public BaseMatchWon AddMatchId(string matchId) {
                parameters.Add("matchId", matchId);
                return this;
            }
            
            /// <param name="itemId"></param>
            public BaseMatchWon AddItemId(string itemId) {
                parameters.Add("itemId", itemId);
                return this;
            }
            
            /// <param name="itemType"></param>
            public BaseMatchWon AddItemType(string itemType) {
                parameters.Add("itemType", itemType);
                return this;
            }
            
            /// <param name="label"></param>
            public BaseMatchWon AddLabel(string label) {
                parameters.Add("label", label);
                return this;
            }
        }
        
        /// <summary>
        /// Triggered when a user wins player-vs-player match 
        /// </summary>
        /// <returns></returns>
        public static BaseMatchWon MatchWon() {
            return new BaseMatchWon();
        }

        public class BasePawnMoved : BaseTracking {
            public BasePawnMoved(string name) {
                eventName = "pawnMoved";
                parameters.Add("name", name);
            }
            
            /// <param name="reason">The reason why the pawn/object was moved.</param>
            public BasePawnMoved AddReason(string reason) {
                parameters.Add("reason", reason);
                return this;
            }
            
            /// <param name="delta">The differance caused by the movement.</param>
            public BasePawnMoved AddDelta(string delta) {
                parameters.Add("delta", delta);
                return this;
            }
            
            /// <param name="energy">The amount of energy required for the movement.</param>
            public BasePawnMoved AddEnergy(string energy) {
                parameters.Add("energy", energy);
                return this;
            }
            
            /// <param name="kind">The type of pawn that was moved.</param>
            public BasePawnMoved AddKind(string kind) {
                parameters.Add("kind", kind);
                return this;
            }
            
            /// <param name="location">The location where the pawn was moved.</param>
            public BasePawnMoved AddLocation(string location) {
                parameters.Add("location", location);
                return this;
            }
            
            /// <param name="rarity">The rarity of the moved pawn.</param>
            public BasePawnMoved AddRarity(string rarity) {
                parameters.Add("rarity", rarity);
                return this;
            }
            
            /// <param name="label">The label attached to the pawn.</param>
            public BasePawnMoved AddLabel(string label) {
                parameters.Add("label", label);
                return this;
            }
        }
        
        /// <summary>
        /// Triggered when the user either moves or changes the state of an in-game object
        /// </summary>
        /// <param name="name">The name of the object moved.</param>
        /// <returns></returns>
        public static BasePawnMoved PawnMoved(string name) {
            return new BasePawnMoved(name);
        }
        
        public class BasePlayerLeagueChanged : BaseTracking {
            public BasePlayerLeagueChanged() {
                eventName = "playerLeagueChanged";
            }
        }
        
        /// <summary>
        /// Triggered when the user interacts with the leaderboard/league
        /// </summary>
        /// <returns></returns>
        public static BasePlayerLeagueChanged PlayerLeagueChanged() {
            return new BasePlayerLeagueChanged();
        }
        
        public class BaseTimedAction : BaseTracking {
            public BaseTimedAction(string timedAction) {
                eventName = "pawnMoved";
                parameters.Add("timedAction", timedAction);
            }
            
            /// <param name="label">The label attached to the action.</param>
            public BaseTimedAction AddLabel(string label) {
                parameters.Add("label", label);
                return this;
            }
            
            /// <param name="timedObject">The object associated with the action.</param>
            public BaseTimedAction AddTimedObject(string timedObject) {
                parameters.Add("timedObject", timedObject);
                return this;
            }
            
            /// <param name="timeToFinish">The amount of time required to finish the action.</param>
            public BaseTimedAction AddTimeToFinish(int timeToFinish) {
                parameters.Add("timeToFinish", timeToFinish);
                return this;
            }
            
            /// <param name="effectMultiplier">The multiplier influencing the action.</param>
            public BaseTimedAction AddEffectMultiplier(float effectMultiplier) {
                parameters.Add("effectMultiplier", effectMultiplier);
                return this;
            }
        } 
        
        /// <summary>
        /// Event used for tracking the state of a timed action in game, e.g. in Operate Now when the user assigns a staff member to the break room to regenerate energy,the event is fired at start and end of the regeneration
        /// </summary>
        /// <param name="timedAction">The name of the action.</param>
        /// <returns></returns>
        public static BasePawnMoved TimedAction(string timedAction) {
            return new BasePawnMoved(timedAction);
        }
        
        public class BaseTransitionToMenu : BaseTracking {
            public BaseTransitionToMenu() {
                eventName = "transitionToMenu";
            }
        }
        
        /// <summary>
        /// Triggered when the player moved to the main menu.
        /// </summary>
        /// <returns></returns>
        public static BaseTransitionToMenu TransitionToMenu() {
            return new BaseTransitionToMenu();
        }
        
        public class BaseTransitionToGame : BaseTracking {
            public BaseTransitionToGame(string type) {
                eventName = "transitionToGame";
                parameters.Add("type", type);
            }        
        }
        
        /// <summary>
        /// Triggered when the player entered game mode.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static BaseTransitionToGame TransitionToGame(string type) {
            return new BaseTransitionToGame(type);
        }
    }
}