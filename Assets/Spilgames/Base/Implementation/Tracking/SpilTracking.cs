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
            public BaseLevelComplete AddScore(double score) {
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
            public BaseLevelFailed AddScore(double score) {
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
            
            /// <param name="sourceId">ToDo</param>
            public BaseLevelUp AddSourceId(string sourceId) {
                parameters.Add("sourceId", sourceId);
                return this;
            }
            
            /// <param name="sourceUniqueId">ToDo</param>
            public BaseLevelUp AddSourceUniqueId(string sourceUniqueId) {
                parameters.Add("sourceUniqueId", sourceUniqueId);
                return this;
            }
            
            /// <param name="objectUniqueId">ToDo</param>
            public BaseLevelUp AddObjectUniqueId(string objectUniqueId) {
                parameters.Add("objectUniqueId", objectUniqueId);
                return this;
            }
            
            /// <param name="objectUniqueIdType">ToDo</param>
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
            
            /// <param name="key">ToDo</param>
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
            public BaseLevelDownload AddRating(double rating) {
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
            public BaseLevelRate AddRating(double rating) {
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

            public BaseWalletInventoryEvent AddWallet(List<TrackingCurrency> currencyList) {
                Dictionary<string, object> wallet = new Dictionary<string, object>();
                wallet.Add("currencies", new JSONObject(JsonHelper.getJSONFromObject(currencyList)));
                wallet.Add("offset", 0);
                parameters.Add("wallet", wallet);
                return this;
            }

            public BaseWalletInventoryEvent AddInventory(List<TrackingItem> itemsList) {
                Dictionary<string, object> inventory = new Dictionary<string, object>();
                inventory.Add("items", new JSONObject(JsonHelper.getJSONFromObject(itemsList)));
                inventory.Add("offset", 0);
                parameters.Add("inventory", inventory);
                return this;
            }
            
            public BaseWalletInventoryEvent AddReasonDetails(string reasonDetails) {
                parameters.Add("reasonDetails", reasonDetails);
                return this;
            }
            
            public BaseWalletInventoryEvent AddTransactionId(string transactionId) {
                parameters.Add("transactionId", transactionId);
                return this;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reason"></param>
        /// <param name="location"></param>
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
            
            public BaseIAPPurchased AddToken(string token) {
                if (!token.Equals("")) {
                    parameters.Add("token", token); 
                }
                
                return this;
            }
            
            public BaseIAPPurchased AddReason(string reason) {
                parameters.Add("reason", reason);
                return this;
            }
            
            public BaseIAPPurchased AddLocation(string location) {
                parameters.Add("location", location);
                return this;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="skuId"></param>
        /// <param name="transactionId"></param>
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
            
            public BaseIAPRestored AddReason(string reason) {
                parameters.Add("reason", reason);
                return this;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="skuId"></param>
        /// <param name="originalTransactionId"></param>
        /// <param name="originalPurchaseDate"></param>
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
            
            public BaseIAPFailed AddReason(string reason) {
                parameters.Add("reason", reason);
                return this;
            }
            
            public BaseIAPFailed AddLocation(string location) {
                parameters.Add("location", location);
                return this;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="skuId"></param>
        /// <param name="errorDescription"></param>
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
        /// 
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
        /// 
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
        /// 
        /// </summary>
        /// <param name="platform"></param>
        /// <returns></returns>
        public static BaseRegister Register(string platform) {
            return new BaseRegister(platform);
        }
        
        public class BaseShare : BaseTracking {
            public BaseShare(string platform) {
                eventName = "share";
                parameters.Add("platform", platform);
            }
            
            public BaseShare AddReason(string reason) {
                parameters.Add("reason", reason);
                return this;
            }
            
            public BaseShare AddLocation(string location) {
                parameters.Add("location", location);
                return this;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="platform"></param>
        /// <returns></returns>
        public static BaseShare Share(string platform) {
            return new BaseShare(platform);
        }
        
        public class BaseInvite : BaseTracking {
            public BaseInvite(string platform) {
                eventName = "invite";
                parameters.Add("platform", platform);
            }        
            
            public BaseInvite AddLocation(string location) {
                parameters.Add("location", location);
                return this;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="platform"></param>
        /// <returns></returns>
        public static BaseInvite Invite(string platform) {
            return new BaseInvite(platform);
        }
        
        public class BaseLevelAppeared : BaseTracking {
            public BaseLevelAppeared(string level) {
                eventName = "levelAppeared";
                parameters.Add("level", level);
            }        
            
            public BaseLevelAppeared AddDifficulty(string difficulty) {
                parameters.Add("difficulty", difficulty);
                return this;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static BaseLevelAppeared LevelAppeared(string level) {
            return new BaseLevelAppeared(level);
        }
        
        public class BaseLevelDiscarded : BaseTracking {
            public BaseLevelDiscarded(string level) {
                eventName = "levelDiscarded";
                parameters.Add("level", level);
            }        
            
            public BaseLevelDiscarded AddDifficulty(string difficulty) {
                parameters.Add("difficulty", difficulty);
                return this;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
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
        /// 
        /// </summary>
        /// <param name="reason"></param>
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
            
            public BaseTimeElapLoad AddStartPoint(double startPoint) {
                parameters.Add("startPoint", startPoint);
                return this;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeElap"></param>
        /// <param name="pointInGame"></param>
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
        /// 
        /// </summary>
        /// <param name="timeElap"></param>
        /// <param name="pointInGame"></param>
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
            
            public BaseObjectStateChanged AddChangedProperties(string changedProperties) {
                parameters.Add("changedProperties", changedProperties);
                return this;
            }
            
            public BaseObjectStateChanged AddOptionConditions(string optionConditions) {
                parameters.Add("optionConditions", optionConditions);
                return this;
            }
            
            public BaseObjectStateChanged AddSituation(string situation) {
                parameters.Add("situation", situation);
                return this;
            }
            
            public BaseObjectStateChanged AddAllChoiceResults(string allChoiceResults) {
                parameters.Add("allChoiceResults", allChoiceResults);
                return this;
            }
            
            public BaseObjectStateChanged AddAllSelectedChoices(string allSelectedChoices) {
                parameters.Add("allSelectedChoices", allSelectedChoices);
                return this;
            }
            
            public BaseObjectStateChanged AddInvolvedParties(string involvedParties) {
                parameters.Add("involvedParties", involvedParties);
                return this;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="changedObject"></param>
        /// <param name="status"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public static BaseObjectStateChanged ObjectStateChanged(string changedObject, string status, string reason) {
            return new BaseObjectStateChanged(changedObject, changedObject, reason);
        }
        
        public class BaseUIElementClicked : BaseTracking {
            public BaseUIElementClicked(string element) {
                eventName = "uiElementClicked";
                parameters.Add("element", element);
            }        
            
            public BaseUIElementClicked AddType(string type) {
                parameters.Add("type", type);
                return this;
            }
            
            public BaseUIElementClicked AddScreenName(string screenName) {
                parameters.Add("screenName", screenName);
                return this;
            }
            
            public BaseUIElementClicked AddLocation(string location) {
                parameters.Add("location", location);
                return this;
            }
            
            public BaseUIElementClicked AddGrade(int grade) {
                parameters.Add("grade", grade);
                return this;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static BaseUIElementClicked UIElementClicked(string element) {
            return new BaseUIElementClicked(element);
        }
        
        public class BaseSendGift : BaseTracking {
            public BaseSendGift(string platform) {
                eventName = "sendGift";
                parameters.Add("platform", platform);
            }        
            
            public BaseSendGift AddLocation(string location) {
                parameters.Add("location", location);
                return this;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="platform"></param>
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
        /// 
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
            
            public BaseDialogueChosen AddTime(int time) {
                parameters.Add("time", time);
                return this;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="choice"></param>
        /// <param name="hasToken"></param>
        /// <param name="isPremiumChoice"></param>
        /// <param name="isQuizz"></param>
        /// <param name="isForced"></param>
        /// <param name="isTimed"></param>
        /// <returns></returns>
        public static BaseDialogueChosen DialogueChosen(string name, string choice, bool hasToken, bool isPremiumChoice, bool isQuizz, bool isForced, bool isTimed) {
            return new BaseDialogueChosen(name, choice, hasToken, isPremiumChoice, isQuizz, isForced, isTimed);
        }
        
        public class BaseFriendAdded : BaseTracking {
            public BaseFriendAdded(string friend) {
                eventName = "friendAdded";
                parameters.Add("friend", friend);
            }        
            
            public BaseFriendAdded AddPlatform(string platform) {
                parameters.Add("platform", platform);
                return this;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="friend"></param>
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
        /// 
        /// </summary>
        /// <returns></returns>
        public static BaseGameObjectInteraction GameObjectInteraction() {
            return new BaseGameObjectInteraction();
        }

        public class BaseGameResult : BaseTracking {
            public BaseGameResult() {
                eventName = "gameResult";
            }
            
            public BaseGameResult AddItemId(string itemId) {
                parameters.Add("itemId", itemId);
                return this;
            }
            
            public BaseGameResult AddItemType(string itemType) {
                parameters.Add("itemType", itemType);
                return this;
            }
            
            public BaseGameResult AddLabel(string label) {
                parameters.Add("label", label);
                return this;
            }
            
            public BaseGameResult AddMatchId(string matchId) {
                parameters.Add("matchId", matchId);
                return this;
            }
        }
        
        /// <summary>
        /// 
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
            
            public BaseItemCrafted AddItemType(string itemType) {
                parameters.Add("itemType", itemType);
                return this;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static BaseItemCrafted ItemCrafted(string itemId) {
            return new BaseItemCrafted(itemId);
        }
        
        public class BaseItemCreated : BaseTracking {
            public BaseItemCreated(string itemId) {
                eventName = "itemCreated";
                parameters.Add("itemId", itemId);
            }        
            
            public BaseItemCreated AddItemType(string itemType) {
                parameters.Add("itemType", itemType);
                return this;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
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
            
            public BaseItemUpdated AddItemType(string itemType) {
                parameters.Add("itemType", itemType);
                return this;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="itemId"></param>
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
            
            public BaseDeckUpdated AddItemType(string itemType) {
                parameters.Add("itemType", itemType);
                return this;
            }
            
            public BaseDeckUpdated AddLabel(string label) {
                parameters.Add("label", label);
                return this;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static BaseDeckUpdated DeckUpdated(string content, string itemId) {
            return new BaseDeckUpdated(content, itemId);
        }
        
        public class BaseMatchComplete : BaseTracking {
            public BaseMatchComplete() {
                eventName = "matchComplete";
            }
            
            public BaseMatchComplete AddMatchId(string matchId) {
                parameters.Add("matchId", matchId);
                return this;
            }
            
            public BaseMatchComplete AddItemId(string itemId) {
                parameters.Add("itemId", itemId);
                return this;
            }
            
            public BaseMatchComplete AddItemType(string itemType) {
                parameters.Add("itemType", itemType);
                return this;
            }
            
            public BaseMatchComplete AddLabel(string label) {
                parameters.Add("label", label);
                return this;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static BaseMatchComplete MatchComplete() {
            return new BaseMatchComplete();
        }
        
        public class BaseMatchLost : BaseTracking {
            public BaseMatchLost() {
                eventName = "matchLost";
            }
            
            public BaseMatchLost AddMatchId(string matchId) {
                parameters.Add("matchId", matchId);
                return this;
            }
            
            public BaseMatchLost AddItemId(string itemId) {
                parameters.Add("itemId", itemId);
                return this;
            }
            
            public BaseMatchLost AddItemType(string itemType) {
                parameters.Add("itemType", itemType);
                return this;
            }
            
            public BaseMatchLost AddLabel(string label) {
                parameters.Add("label", label);
                return this;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static BaseMatchLost MatchLost() {
            return new BaseMatchLost();
        }
        
        public class BaseMatchTie : BaseTracking {
            public BaseMatchTie() {
                eventName = "matchTie";
            }
            
            public BaseMatchTie AddMatchId(string matchId) {
                parameters.Add("matchId", matchId);
                return this;
            }
            
            public BaseMatchTie AddItemId(string itemId) {
                parameters.Add("itemId", itemId);
                return this;
            }
            
            public BaseMatchTie AddItemType(string itemType) {
                parameters.Add("itemType", itemType);
                return this;
            }
            
            public BaseMatchTie AddLabel(string label) {
                parameters.Add("label", label);
                return this;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static BaseMatchTie MatchTie() {
            return new BaseMatchTie();
        }
        
        public class BaseMatchWon : BaseTracking {
            public BaseMatchWon() {
                eventName = "matchWon";
            }
            
            public BaseMatchWon AddMatchId(string matchId) {
                parameters.Add("matchId", matchId);
                return this;
            }
            
            public BaseMatchWon AddItemId(string itemId) {
                parameters.Add("itemId", itemId);
                return this;
            }
            
            public BaseMatchWon AddItemType(string itemType) {
                parameters.Add("itemType", itemType);
                return this;
            }
            
            public BaseMatchWon AddLabel(string label) {
                parameters.Add("label", label);
                return this;
            }
        }
        
        /// <summary>
        /// 
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
            
            public BasePawnMoved AddReason(string reason) {
                parameters.Add("reason", reason);
                return this;
            }
            
            public BasePawnMoved AddDelta(string delta) {
                parameters.Add("delta", delta);
                return this;
            }
            
            public BasePawnMoved AddEnergy(string energy) {
                parameters.Add("energy", energy);
                return this;
            }
            
            public BasePawnMoved AddKind(string kind) {
                parameters.Add("kind", kind);
                return this;
            }
            
            public BasePawnMoved AddLocation(string location) {
                parameters.Add("location", location);
                return this;
            }
            
            public BasePawnMoved AddRarity(string rarity) {
                parameters.Add("rarity", rarity);
                return this;
            }
            
            public BasePawnMoved AddLabel(string label) {
                parameters.Add("label", label);
                return this;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
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
        /// 
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
            
            public BaseTimedAction AddLabel(string label) {
                parameters.Add("label", label);
                return this;
            }
            
            public BaseTimedAction AddTimedObject(string timedObject) {
                parameters.Add("timedObject", timedObject);
                return this;
            }
            
            public BaseTimedAction AddTimeToFinish(int timeToFinish) {
                parameters.Add("timeToFinish", timeToFinish);
                return this;
            }
            
            public BaseTimedAction AddEffectMultiplier(float effectMultiplier) {
                parameters.Add("effectMultiplier", effectMultiplier);
                return this;
            }
        } 
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timedAction"></param>
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
        /// 
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
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static BaseTransitionToGame TransitionToGame(string type) {
            return new BaseTransitionToGame(type);
        }
    }
}