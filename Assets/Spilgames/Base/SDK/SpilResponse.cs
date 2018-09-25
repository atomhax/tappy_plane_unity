using System;
using System.Collections.Generic;
using UnityEngine;
using SpilGames.Unity.Json;

namespace SpilGames.Unity.Base.SDK {
    public class SpilResponse {
        public string name;
        public string type;
        public string action;
        public string message;

        // The data field is not included here since it's object type varies between the different
        // types of responses. Use the type field to identify the specific type of SpilResponse,
        // then deserialize the JSON string again using that type. TODO: Make that prettier?
    }

    public class SpilErrorMessage {

        public static readonly SpilErrorMessage LoadFailed = new SpilErrorMessage(1, "LoadFailed", "Data container is empty!");
        public static readonly SpilErrorMessage ItemNotFound = new SpilErrorMessage(2, "ItemNotFound", "Item does not exist!");
        public static readonly SpilErrorMessage CurrencyNotFound = new SpilErrorMessage(3, "CurrencyNotFound", "Currency does not exist!");
        public static readonly SpilErrorMessage BundleNotFound = new SpilErrorMessage(4, "BundleNotFound", "Bundle does not exist!");
        public static readonly SpilErrorMessage WalletNotFound = new SpilErrorMessage(5, "WalletNotFound", "No wallet data stored!");
        public static readonly SpilErrorMessage InventoryNotFound = new SpilErrorMessage(6, "InventoryNotFound", "No inventory data stored!");
        public static readonly SpilErrorMessage NotEnoughCurrency = new SpilErrorMessage(7, "NotEnoughCurrency", "Not enough balance for currency!");
        public static readonly SpilErrorMessage ItemAmountToLow = new SpilErrorMessage(8, "ItemAmountToLow", "Could not remove item as amount is too low!");
        public static readonly SpilErrorMessage CurrencyOperation = new SpilErrorMessage(9, "CurrencyOperation", "Error updating wallet!");
        public static readonly SpilErrorMessage ItemOperation = new SpilErrorMessage(10, "ItemOperation", "Error updating item to player inventory!");
        public static readonly SpilErrorMessage BundleOperation = new SpilErrorMessage(11, "BundleOperation", "Error adding bundle to player inventory!");
        public static readonly SpilErrorMessage PublicGameStateOperation = new SpilErrorMessage(12, "UserIdMissing", "Error adding public game state data! A custom user id must be set in order to save public game state data");
        public static readonly SpilErrorMessage GameStateOtherUsersServerError = new SpilErrorMessage(13, "OtherUsersGameStateError", "Error when loading provided user id's game states from the server");
        public static readonly SpilErrorMessage DailyBonusServerError = new SpilErrorMessage(14, "DailyBonusServerError", "Error processing the reward from daily bonus");
        public static readonly SpilErrorMessage DailyBonusLoadError = new SpilErrorMessage(15, "DailyBonusLoadError", "Error loading the daily bonus page");
        public static readonly SpilErrorMessage SplashScreenLoadError = new SpilErrorMessage(16, "SplashScreenLoadError", "Error loading the splash screen");
        public static readonly SpilErrorMessage ImageLoadFailedError = new SpilErrorMessage(17, "ImageLoadFailedError", "Error loading image");
        public static readonly SpilErrorMessage RewardServerError = new SpilErrorMessage(18, "RewardServerError", "");
        public static readonly SpilErrorMessage ConfigServerError = new SpilErrorMessage(19, "ConfigServerError", "Error retrieving config");
        public static readonly SpilErrorMessage GameDataServerError = new SpilErrorMessage(20, "GameDataServerError", "Error retrieving game data");
        public static readonly SpilErrorMessage GameStateServerError = new SpilErrorMessage(21, "GameStateServerError", "Error communicating with the server for game state");
        public static readonly SpilErrorMessage IAPServerError = new SpilErrorMessage(22, "IAPServerError", "Error communication with the server for IAP");
        public static readonly SpilErrorMessage PlayerDataServerError = new SpilErrorMessage(23, "PlayerDataServerError", "Error retrieving player data from the server");
        public static readonly SpilErrorMessage ServerTimeRequestError = new SpilErrorMessage(24, "ServerTimeRequestError", "Error retrieving server time");
        public static readonly SpilErrorMessage LiveEventServerError = new SpilErrorMessage(25, "LiveEventServerError", "Error retrieving live event data");
        public static readonly SpilErrorMessage LiveEventInvalidNextStage = new SpilErrorMessage(26, "LiveEventInvalid", "Error validating live event next stage");
        public static readonly SpilErrorMessage LiveEventMissingNextStage = new SpilErrorMessage(27, "LiveEventMissingNextStage", "Error opening next stage due to missing data");
        public static readonly SpilErrorMessage LiveEventLoadError = new SpilErrorMessage(28, "LiveEventLoadError", "Error loading the live event page");
        public static readonly SpilErrorMessage GachaNotFound = new SpilErrorMessage(29, "GachaNotFound", "Gacha does not exist!");
        public static readonly SpilErrorMessage GachaOperation = new SpilErrorMessage(30, "GachaOperation", "Error opening gacha!");
        public static readonly SpilErrorMessage NotEnoughGachaBoxes = new SpilErrorMessage(31, "NotEnoughGachaBoxes", "Not enough gacha boxes in the inventory!");
        public static readonly SpilErrorMessage InvalidSpilTokenError = new SpilErrorMessage(32, "InvalidSpilTokenError", "Spil Token is invalid! Please login again!");
        public static readonly SpilErrorMessage RequiresLoginError = new SpilErrorMessage(33, "RequriesLoginError", "Event requires user login!");
        public static readonly SpilErrorMessage InvalidSocialTokenError = new SpilErrorMessage(34, "InvalidSocialTokenError", "The provided social token could not be verified with the backend");
        public static readonly SpilErrorMessage UserAlreadyLinkedError = new SpilErrorMessage(35, "UserAlreadyLinkedError", "User already has a social provider account");
        public static readonly SpilErrorMessage SocialIdAlreadyLinkedError = new SpilErrorMessage(36, "SocialIdAlreadyLinkedError", "The social id is already linked to another user!");
        public static readonly SpilErrorMessage SocialLoginServerError = new SpilErrorMessage(37, "SocialLoginServerError", "Error communicating with the server!");
        public static readonly SpilErrorMessage UserDataServerError = new SpilErrorMessage(38, "UserDataServerError", "Error retrieving user data from server (gameState and playerData)!");
        public static readonly SpilErrorMessage ConfigResetError = new SpilErrorMessage(39, "ConfigResetError", "Error while resetting game config. This may be caused by loss of internet connection.");
        public static readonly SpilErrorMessage GameDataResetError = new SpilErrorMessage(40, "GameDataResetError", "Error while resetting game data. This may be caused by loss of internet connection.");
        public static readonly SpilErrorMessage UserDataResetError = new SpilErrorMessage(41, "UserDataResetError", "Error while resetting user data. This may be caused by loss of internet connection.");
        public static readonly SpilErrorMessage TieredEventShowProgressError = new SpilErrorMessage(42, "TieredEventShowProgressError", "Unable to show tiered event progress.");
        public static readonly SpilErrorMessage TieredEventUpdateProgressError = new SpilErrorMessage(43, "TieredEventUpdateProgressError", "Unable to update the tiered event progress.");
        public static readonly SpilErrorMessage TieredEventClaimTierError = new SpilErrorMessage(44, "TieredEventClaimTierError", "Unable to claim the tier reward.");

        public int id;
        public string name;
        public string message;

        public SpilErrorMessage() {
            
        }
        
        public SpilErrorMessage(int id, string name, string message) {
            this.id = id;
            this.name = name;
            this.message = message;
        }

        public string ToJson() {
            return JsonHelper.getJSONFromObject(this);
        }
    }
}