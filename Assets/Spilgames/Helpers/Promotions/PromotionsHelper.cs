using System;
using System.Collections.Generic;
using SpilGames.Unity.Base.SDK;
using SpilGames.Unity.Helpers.GameData;
using SpilGames.Unity.Helpers.IAPPackages;

namespace SpilGames.Unity.Helpers.Promotions {
    public class PromotionsHelper {
        public List<Promotion> Promotions;

        public PromotionsHelper(List<SpilPromotionData> promotionData) {
            Promotions = new List<Promotion>();

            if (promotionData != null) {
                foreach (SpilPromotionData promotion in promotionData) {
                    Promotions.Add(new Promotion(promotion.id, promotion.name, promotion.amountPurchased, promotion.maxPurchase, promotion.label, promotion.startDate, promotion.endDate, promotion.affectedEntities, promotion.extraEntities, promotion.priceOverride, promotion.gameAssets));
                }
            }
        }

        public Promotion GetBundlePromotion(int bundleId) {
            foreach (Promotion promotion in Promotions) {
                foreach (AffectedEntity affectedEntity in promotion.AffectedEntities) {
                    if (affectedEntity.Id == bundleId && affectedEntity.Type.Equals("BUNDLE")) {
                        return promotion;
                    }
                }
            }

            return null;
        }

        public Promotion GetPackagePromotion(string packageId) {
            Package package = Spil.Instance.GetPackages().GetPackageById(packageId);

            if (package == null) {
                return null;
            }

            foreach (Promotion promotion in Promotions) {
                foreach (AffectedEntity affectedEntity in promotion.AffectedEntities) {
                    if (affectedEntity.Id == package.Id && affectedEntity.Type.Equals("PACKAGE")) {
                        return promotion;
                    }
                }
            }

            return null;
        }
        
        public bool HasBundlePromotion(int bundleId) {
            foreach (Promotion promotion in Promotions) {
                foreach (AffectedEntity affectedEntity in promotion.AffectedEntities) {
                    if (affectedEntity.Id == bundleId) {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool HasPackagePromotion(String packageId) {
            Package package = Spil.Instance.GetPackages().GetPackageById(packageId);

            if (package == null) {
                return false;
            }

            foreach (Promotion promotion in Promotions) {
                foreach (AffectedEntity affectedEntity in promotion.AffectedEntities) {
                    if (affectedEntity.Id == package.Id) {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}