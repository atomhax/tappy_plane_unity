using SpilGames.Unity.Base.Implementations;
using SpilGames.Unity.Helpers.PlayerData;

#if UNITY_EDITOR
namespace SpilGames.Unity.Base.UnityEditor.Responses {
    public class RewardResponse : ResponseEvent {
        private static Spil.RewardFeatureTypeEnum rewardFeatureType = Spil.MonoInstance.RewardFeatureType;
        private static Spil.TokenRewardTypeEnum rewardType = Spil.MonoInstance.TokenRewardType;

        private static string DeepLink = "deeplink";
        private static string PushNotification = "pushnotification";

        public static void ProcessRewardResponse(ResponseEvent response) {
            string reason = "";

            if (rewardFeatureType.ToString().ToLower().Trim().Equals(DeepLink)) {
                reason = PlayerDataUpdateReasons.Deeplink;
            } else if (rewardFeatureType.ToString().ToLower().Trim().Equals(PushNotification)) {
                reason = PlayerDataUpdateReasons.PushNotification;
            }

            if (rewardType == Spil.TokenRewardTypeEnum.EXTERNAL) {
                JSONObject rewards = new JSONObject(JSONObject.Type.ARRAY);

                JSONObject reward = new JSONObject();
                reward.AddField("externalId", Spil.TokenExternalRewardId);
                reward.AddField("amount", Spil.TokenRewardAmount);
                reward.AddField("type", rewardType.ToString());

                rewards.Add(reward);

                JSONObject json = new JSONObject();
                json.AddField("reward", rewards);
                json.AddField("rewardType", rewardFeatureType.ToString());

                SpilUnityImplementationBase.fireRewardTokenClaimed(json.Print());
            } else {
                int id = Spil.TokenRewardId;
                int amount = Spil.TokenRewardAmount;

                if (id == 0 || amount == 0) {
                    SpilLogging.Error("Token Rewards not configured for Editor!");
                }

                if (rewardType == Spil.TokenRewardTypeEnum.CURRENCY) {
                    SpilUnityEditorImplementation.pData.WalletOperation("add", id, amount, reason, null, "DeepLink", null);
                } else if (rewardType == Spil.TokenRewardTypeEnum.ITEM) {
                    SpilUnityEditorImplementation.pData.InventoryOperation("add", id, amount, reason, null, "DeepLink", null);
                }
            }
        }
    }
}

#endif