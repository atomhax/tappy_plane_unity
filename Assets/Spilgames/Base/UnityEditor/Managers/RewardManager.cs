using SpilGames.Unity.Base.Implementations;
using SpilGames.Unity.Helpers.PlayerData;

#if UNITY_EDITOR
namespace SpilGames.Unity.Base.UnityEditor.Managers {
    public class RewardManager {
        public static Spil.RewardFeatureTypeEnum rewardFeatureType = Spil.MonoInstance.RewardFeatureType;
        public static Spil.TokenRewardTypeEnum rewardType = Spil.MonoInstance.TokenRewardType;

        public static string DeepLink = "deeplink";
        public static string PushNotification = "pushnotification";
    }
    
    public class RewardResponse : ResponseEvent {
        public static void ProcessRewardResponse(ResponseEvent response) {
            string reason = "";

            if (response.data.HasField("success") && !response.data.GetField("success").b) {
                string rewardType = response.data.GetField("rewardType").str;
                
                JSONObject errorMessage = new JSONObject();
                errorMessage.AddField("id", 18);
                errorMessage.AddField("name", "RewardServerError");
                errorMessage.AddField("message", response.data.GetField("error").str);
                
                JSONObject jsonObject = new JSONObject();
                jsonObject.AddField("rewardType", rewardType);
                jsonObject.AddField("error", errorMessage);
                
                Spil.Instance.fireRewardTokenClaimFailed(jsonObject.Print());
                return;
            }
            
            if (response.eventName.Equals("dailybonus")) {
                reason = PlayerDataUpdateReasons.DailyBonus;
            } else if (RewardManager.rewardFeatureType.ToString().ToLower().Trim().Equals(RewardManager.DeepLink)) {
                reason = PlayerDataUpdateReasons.Deeplink;
            } else if (RewardManager.rewardFeatureType.ToString().ToLower().Trim().Equals(RewardManager.PushNotification)) {
                reason = PlayerDataUpdateReasons.PushNotification;
            }

            if (RewardManager.rewardType == Spil.TokenRewardTypeEnum.EXTERNAL) {
                JSONObject rewards = new JSONObject(JSONObject.Type.ARRAY);

                JSONObject reward = new JSONObject();
                reward.AddField("externalId", Spil.TokenExternalRewardId);
                reward.AddField("amount", Spil.TokenRewardAmount);
                reward.AddField("type", RewardManager.rewardType.ToString());

                rewards.Add(reward);

                JSONObject json = new JSONObject();
                json.AddField("reward", rewards);
                json.AddField("rewardType", RewardManager.rewardFeatureType.ToString());

				Spil.Instance.fireRewardTokenClaimed(json.Print());
            } else {
                int id = 0;
                int amount = 0;
                
                if (response.eventName.Equals("dailybonus") && OverlayManager.spilDailyBonusConfig.type.Equals("assetBundle")) {
                    JSONObject collectibles = response.data.GetField("collectibles");
                    for (int i = 0; i < collectibles.Count; i++) {
                        id = (int) collectibles[i].GetField("id").i;
                        amount = (int) collectibles[i].GetField("amount").i;
                        
                        if (collectibles[i].GetField("type").str == "CURRENCY") {
                            SpilUnityEditorImplementation.pData.WalletOperation("add", id, amount, reason, null, reason, null);
                        } else if (collectibles[i].GetField("type").str == "ITEM") {
                            SpilUnityEditorImplementation.pData.InventoryOperation("add", id, amount, reason, null, reason, null);
                        }
                    }
                } else {
                    id = Spil.TokenRewardId;
                    amount = Spil.TokenRewardAmount;
                    
                    if ((id == 0 || amount == 0) ) {
                        SpilLogging.Error("Token Rewards not configured for Editor!");
                    }
                    
                    if (RewardManager.rewardType == Spil.TokenRewardTypeEnum.CURRENCY) {
                        SpilUnityEditorImplementation.pData.WalletOperation("add", id, amount, reason, null, reason, null);
                    } else if (RewardManager.rewardType == Spil.TokenRewardTypeEnum.ITEM) {
                        SpilUnityEditorImplementation.pData.InventoryOperation("add", id, amount, reason, null, reason, null);
                    }
                }
            }
            Spil.Instance.RequestDailyBonus();
        }
    }
}

#endif