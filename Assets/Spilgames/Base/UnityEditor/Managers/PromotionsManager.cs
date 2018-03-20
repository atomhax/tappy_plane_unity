#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpilGames.Unity.Base.Implementations;
using SpilGames.Unity.Base.SDK;
using SpilGames.Unity.Helpers.IAPPackages;
using SpilGames.Unity.Helpers.Promotions;
using SpilGames.Unity.Json;

namespace SpilGames.Unity.Base.UnityEditor.Managers {
    public class PromotionsManager {
        public static List<SpilPromotionData> PromotionData = new List<SpilPromotionData>();

        public static void ProcessBoughtPromotionResponse(SpilPromotionData boughtPromotion) {
            SpilPromotionData promotion = null;
            foreach (SpilPromotionData promotionData in PromotionData) {
                if (promotionData.id == boughtPromotion.id) {                  
                    promotionData.amountPurchased = boughtPromotion.amountPurchased;
                    promotion = promotionData;
                    break;
                }
            }

            bool maxReached = false;
            if (promotion != null && (promotion.amountPurchased == promotion.maxPurchase || promotion.amountPurchased > promotion.maxPurchase)) {
                PromotionData.Remove(promotion);
                maxReached = true;
            }

            JSONObject data = new JSONObject();
            data.AddField("promotionId", boughtPromotion.id);
            data.AddField("amountPurchased", boughtPromotion.amountPurchased);
            data.AddField("maxAmountReached", maxReached);
            
            SpilUnityImplementationBase.firePromotionAmountBought(data.Print());
        }
        
        public static void ShowPromotionScreen(int promotionId) {
            SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent>();
            spilEvent.eventName = "showPromotionScreen";

            spilEvent.customData.AddField("id", promotionId);
            
            spilEvent.Send();
        }

        public static void SendBoughtPromotion(int promotionId) {
            string promotionName = "";

            foreach (SpilPromotionData promotionData in PromotionData) {
                if (promotionData.id == promotionId) {
                    promotionName = promotionData.name;
                    break;
                }
            }
            
            SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent>();
            spilEvent.eventName = "boughtPromotion";

            spilEvent.customData.AddField("id", promotionId);
            spilEvent.customData.AddField("name", promotionName);
            
            spilEvent.Send();
        }

        public static string GetAllPromotions() {
            return JsonHelper.getJSONFromObject(PromotionData);
        }

        public static string GetBundlePromotion(int bundleId) {
            foreach (SpilPromotionData promotionData in PromotionData) {
                foreach (SpilPromotionAffectedEntity affectedEntity in promotionData.affectedEntities) {
                    if (affectedEntity.type.Equals("BUNDLE") && affectedEntity.id == bundleId) {
                        return JsonHelper.getJSONFromObject(promotionData);
                    }
                }
            }

            return null;
        }

        public static string GetPackagePromotion(string packageId) {
            foreach (SpilPromotionData promotionData in PromotionData) {
                foreach (SpilPromotionAffectedEntity affectedEntity in promotionData.affectedEntities) {
                    if (affectedEntity.type.Equals("PACKAGE")) {
                        Package package = Spil.Instance.GetPackages().GetPackageByPackageId(packageId);
                        if (package != null && package.PackageId.Equals(packageId)) {
                            return JsonHelper.getJSONFromObject(promotionData);
                        }
                    }
                }
            }

            return null;
        }

        public static bool HasBundlePromotion(int bundleId) {
            foreach (SpilPromotionData promotionData in PromotionData) {
                foreach (SpilPromotionAffectedEntity affectedEntity in promotionData.affectedEntities) {
                    if (affectedEntity.type.Equals("BUNDLE") && affectedEntity.id == bundleId) {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool HasPackagePromotion(int id) {
            foreach (SpilPromotionData promotionData in PromotionData) {
                foreach (SpilPromotionAffectedEntity affectedEntity in promotionData.affectedEntities) {
                    if (affectedEntity.type.Equals("PACKAGE")) {
                        Package package = Spil.Instance.GetPackages().GetPackageById(id);
                        if (package != null && package.Id == id) {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
    
    public class PromotionsResponse : Response {
        public static void ProcessPromotionResponse(ResponseEvent response) {
            if (response.data == null) {
                SpilUnityImplementationBase.firePromotionsNotAvailable();
                return;
            }

            if (response.action.Equals("request")) {
                if (response.data.HasField("promotions")) {
                    JSONObject promotionsJSON = response.data.GetField("promotions");
                    for (int i = 0; i < promotionsJSON.Count; i++) {
                        JSONObject promotion = promotionsJSON.list[i];
                        SpilPromotionData promotionData = new SpilPromotionData();

                        if (promotion.HasField("id")) {
                            promotionData.id = (int) promotion.GetField("id").i;
                        }

                        if (promotion.HasField("name")) {
                            promotionData.name = promotion.GetField("name").str;
                        }

                        if (promotion.HasField("amountPurchased")) {
                            promotionData.amountPurchased = (int) promotion.GetField("amountPurchased").i;
                        }

                        promotionData.maxPurchase = 0;
                        if (promotion.HasField("maxPurchase")) {
                            promotionData.maxPurchase = (int) promotion.GetField("maxPurchase").i;
                        }

                        if (promotion.HasField("label")) {
                            promotionData.label = promotion.GetField("label").str;
                        }

                        if (promotion.HasField("startDate")) {
                            promotionData.startDate = promotion.GetField("startDate").i;
                        }

                        if (promotion.HasField("endDate")) {
                            promotionData.endDate = promotion.GetField("endDate").i;
                        }
                        
                        promotionData.affectedEntities = new List<SpilPromotionAffectedEntity>();
                        if (promotion.HasField("affectedEntities")) {
                            List<SpilPromotionAffectedEntity> affectedEntities = new List<SpilPromotionAffectedEntity>();
                            JSONObject affectedEntitiesJSON = promotion.GetField("affectedEntities");
                            for (int j = 0; j < affectedEntitiesJSON.Count; j++) {
                                SpilPromotionAffectedEntity affectedEntity = JsonHelper.getObjectFromJson<SpilPromotionAffectedEntity>(affectedEntitiesJSON.list[j].Print());
                                affectedEntities.Add(affectedEntity);
                            }

                            promotionData.affectedEntities = affectedEntities;
                        }
                        
                        promotionData.affectedEntities = new List<SpilPromotionAffectedEntity>();
                        if (promotion.HasField("affectedEntities")) {
                            List<SpilPromotionAffectedEntity> affectedEntities = new List<SpilPromotionAffectedEntity>();
                            JSONObject affectedEntitiesJSON = promotion.GetField("affectedEntities");
                            for (int j = 0; j < affectedEntitiesJSON.Count; j++) {
                                SpilPromotionAffectedEntity affectedEntity = JsonHelper.getObjectFromJson<SpilPromotionAffectedEntity>(affectedEntitiesJSON.list[j].Print());
                                affectedEntities.Add(affectedEntity);
                            }

                            promotionData.affectedEntities = affectedEntities;
                        }
                        
                        promotionData.extraEntities = new List<SpilPromotionExtraEntity>();
                        if (promotion.HasField("extraEntities")) {
                            List<SpilPromotionExtraEntity> extraEntities = new List<SpilPromotionExtraEntity>();
                            JSONObject extraEntitiesJSON = promotion.GetField("extraEntities");
                            for (int j = 0; j < extraEntitiesJSON.Count; j++) {
                                SpilPromotionExtraEntity extraEntity = JsonHelper.getObjectFromJson<SpilPromotionExtraEntity>(extraEntitiesJSON.list[j].Print());
                                extraEntities.Add(extraEntity);
                            }

                            promotionData.extraEntities = extraEntities;
                        }
                        
                        promotionData.priceOverride = new List<SpilPromotionPriceOverride>();
                        if (promotion.HasField("priceOverride")) {
                            List<SpilPromotionPriceOverride> priceOverride = new List<SpilPromotionPriceOverride>();
                            JSONObject priceOverrideJSON = promotion.GetField("priceOverride");
                            for (int j = 0; j < priceOverrideJSON.Count; j++) {
                                SpilPromotionPriceOverride price = JsonHelper.getObjectFromJson<SpilPromotionPriceOverride>(priceOverrideJSON.list[j].Print());
                                priceOverride.Add(price);
                            }

                            promotionData.priceOverride = priceOverride;
                        }
                        
                        promotionData.gameAssets = new List<SpilPromotionGameAsset>();
                        if (promotion.HasField("gameAssets")) {
                            List<SpilPromotionGameAsset> gameAssets = new List<SpilPromotionGameAsset>();
                            JSONObject gameAssetsJSON = promotion.GetField("gameAssets");
                            for (int j = 0; j < gameAssetsJSON.Count; j++) {
                                SpilPromotionGameAsset gameAsset = JsonHelper.getObjectFromJson<SpilPromotionGameAsset>(gameAssetsJSON.list[j].Print());
                                gameAssets.Add(gameAsset);
                            }

                            promotionData.gameAssets = gameAssets;
                        }
                        
                        PromotionsManager.PromotionData.Add(promotionData);
                    }
                    SpilUnityImplementationBase.firePromotionsAvailable();
                }
            } else if (response.action.Equals("update")) {
               
                SpilPromotionData boughtPromotion = new SpilPromotionData();

                if (response.data.HasField("id")) {
                    boughtPromotion.id = (int) response.data.GetField("id").i;
                }

                if (response.data.HasField("amountPurchased")) {
                    boughtPromotion.amountPurchased = (int) response.data.GetField("amountPurchased").i;
                }
                
                if (response.data.HasField("maxPurchase")) {
                    boughtPromotion.amountPurchased = (int) response.data.GetField("maxPurchase").i;
                }
                
                PromotionsManager.ProcessBoughtPromotionResponse(boughtPromotion);
            }
        }

    }
}
#endif