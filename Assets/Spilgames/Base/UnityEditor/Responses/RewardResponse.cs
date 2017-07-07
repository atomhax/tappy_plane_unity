using System.Collections.Generic;
using SpilGames.Unity.Base.Implementations;
using SpilGames.Unity.Helpers.PlayerData;
using SpilGames.Unity.Json;

#if UNITY_EDITOR
//ToDo
namespace SpilGames.Unity.Base.UnityEditor.Responses {
    public class RewardResponse : ResponseEvent{
        private static Spil.TokenRewardTypeEnum rewardType = Spil.MonoInstance.TokenRewardType;
        
        private static string DeepLink = "deeplink";
        private static string PushNotification = "pushnotification";
        
        public static void ProcessRewardResponse(ResponseEvent response) {
            
            string reason = "";

            if (rewardType.ToString().ToLower().Trim().Equals(DeepLink)) {
                reason = PlayerDataUpdateReasons.Deeplink;
            } else if (rewardType.ToString().ToLower().Trim().Equals(PushNotification)) {
                reason = PlayerDataUpdateReasons.PushNotification;
            }
            
            if (rewardType == Spil.TokenRewardTypeEnum.EXTERNAL) {
                List<Reward> rewards = new List<Reward>();

                Reward reward = new Reward();
                reward.externalId = Spil.TokenExternalRewardId;
                reward.amount = Spil.TokenRewardAmount;

                rewards.Add(reward);

                string rewardsJSON = JsonHelper.getJSONFromObject(rewards);

                JSONObject json = new JSONObject();
                json.AddField("reward", rewardsJSON);
                json.AddField("rewardType", rewardType.ToString());

                SpilUnityImplementationBase.fireRewardTokenClaimed(json.Print());
                
            }
            else {
                int id = Spil.TokenRewardId;
                int amount = Spil.TokenRewardAmount;

                if (id == 0 || amount == 0) {
                    SpilLogging.Error("Token Rewards not configured for Editor!");
                }

                if (rewardType == Spil.TokenRewardTypeEnum.CURRENCY) {
                    SpilUnityEditorImplementation.pData.WalletOperation("add", id, amount, reason, null, "DeepLink", null);
                }
                else if (rewardType == Spil.TokenRewardTypeEnum.ITEM) {
                    SpilUnityEditorImplementation.pData.InventoryOperation("add", id, amount, reason, null, "DailyBonus", null);
                }
            }
        }
    }
    
    public class Reward{
        public int id;
        public string externalId;
        public string type;
        public int amount;
    }
    
}

#endif