using System.Collections.Generic;
using SpilGames.Unity.Base.Implementations;
using SpilGames.Unity.Helpers.PlayerData;
using SpilGames.Unity.Json;

#if UNITY_EDITOR
//ToDo
namespace SpilGames.Unity.Base.UnityEditor.Responses {
    public class RewardResponse : ResponseEvent{
        private static Spil.TokenRewardTypeEnum rewardType = Spil.MonoInstance.TokenRewardType;
        
        public static void ProcessRewardResponse(ResponseEvent response) {
            if (rewardType == Spil.TokenRewardTypeEnum.EXTERNAL) {
                List<Reward> rewards = new List<Reward>();

                Reward reward = new Reward();
                reward.externalId = Spil.TokenExternalRewardId;
                reward.amount = Spil.TokenRewardAmount;

                rewards.Add(reward);

                string rewardsJSON = JsonHelper.getJSONFromObject(rewards);

                JSONObject json = new JSONObject();
                json.AddField("data", rewardsJSON);

                //ToDo
//                SpilUnityImplementationBase.fireRewardTokenClaimed();
                
            }
            else {
                int id = Spil.DailyBonusId;
                int amount = Spil.DailyBonusAmount;

                if (id == 0 || amount == 0) {
                    SpilLogging.Error("Daily Bonus Rewards not configured for Editor!");
                }

                if (rewardType == Spil.TokenRewardTypeEnum.CURRENCY) {
                    SpilUnityEditorImplementation.pData.WalletOperation("add", id, amount, PlayerDataUpdateReasons.DailyBonus, null, "DailyBonus", null);
                }
                else if (rewardType == Spil.TokenRewardTypeEnum.ITEM) {
                    SpilUnityEditorImplementation.pData.InventoryOperation("add", id, amount, PlayerDataUpdateReasons.DailyBonus, null, "DailyBonus", null);
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