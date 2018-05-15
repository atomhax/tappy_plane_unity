using System.Collections.Generic;
using SpilGames.Unity.Base.SDK;
using UnityEngine;

namespace SpilGames.Unity.Helpers.Promotions {
    public class Promotion {
        public int Id {
            get { return id; }
        }

        private int id;

        public string Name {
            get { return name; }
        }

        private string name;
        
        public int AmountPurchased {
            get { return amountPurchased; }
            set { amountPurchased = value; }
        }

        private int amountPurchased;
        
        public int MaxPurchase {
            get { return maxPurchase; }
        }

        private int maxPurchase;
        
        public string Label {
            get { return label; }
        }

        private string label;

        public long StartDate {
            get { return startDate; }
        }

        private long startDate;
        
        public long EndDate {
            get { return endDate; }
        }

        private long endDate;

        public List<AffectedEntity> AffectedEntities;

        public List<ExtraEntity> ExtraEntities;

        public List<PriceOverride> PriceOverride;

        public List<GameAsset> GameAssets;

        public Promotion(int id, string name, int amountPurchased, int maxPurchase, string label, long startDate, long endDate, List<SpilPromotionAffectedEntity> affectedEntities, List<SpilPromotionExtraEntity> extraEntities, List<SpilPromotionPriceOverride> priceOverrides, List<SpilPromotionGameAsset> gameAssets) {
            this.id = id;
            this.name = name;
            this.amountPurchased = amountPurchased;
            this.maxPurchase = maxPurchase;
            this.label = label;
            this.startDate = startDate;
            this.endDate = endDate;
            
            AffectedEntities = new List<AffectedEntity>();
            foreach (SpilPromotionAffectedEntity affectedEntity in affectedEntities) {
                AffectedEntities.Add(new AffectedEntity(affectedEntity.id, affectedEntity.type));
            }
            
            ExtraEntities = new List<ExtraEntity>();
            foreach (SpilPromotionExtraEntity extraEntity in extraEntities) {
                ExtraEntities.Add(new ExtraEntity(extraEntity.id, extraEntity.type, extraEntity.amount));
            }
            
            PriceOverride = new List<PriceOverride>();
            foreach (SpilPromotionPriceOverride priceOverride in priceOverrides) {
                PriceOverride.Add(new PriceOverride(priceOverride.id, priceOverride.type, priceOverride.amount));
            }
            
            GameAssets = new List<GameAsset>();
            foreach (SpilPromotionGameAsset gameAsset in gameAssets) {
                GameAssets.Add(new GameAsset(gameAsset.name, gameAsset.locale, gameAsset.position, gameAsset.type, gameAsset.value));
            }
        }

        public bool IsValid() {
            return endDate > System.DateTime.Now.Millisecond && (amountPurchased < maxPurchase || maxPurchase == 0);
        }
    }

    public class AffectedEntity {
        public int Id {
            get { return id; }
        }

        private int id;

        public string Type {
            get { return type; }
        }

        private string type;

        public AffectedEntity(int id, string type) {
            this.id = id;
            this.type = type;
        }
    }

    public class ExtraEntity {
        public int Id {
            get { return id; }
        }

        private int id;

        public string Type {
            get { return type; }
        }

        private string type;

        public int Amount {
            get { return amount; }
        }

        private int amount;
        
        public ExtraEntity(int id, string type, int amount) {
            this.id = id;
            this.type = type;
            this.amount = amount;
        }
    }

    public class PriceOverride {
        public int Id {
            get { return id; }
        }

        private int id;

        public string Type {
            get { return type; }
        }

        private string type;

        public int Amount {
            get { return amount; }
        }

        private int amount;
        
        public PriceOverride(int id, string type, int amount) {
            this.id = id;
            this.type = type;
            this.amount = amount;
        }
    }

    public class GameAsset {
        public string Name {
            get { return name; }
        }

        private string name;
        
        public string Locale {
            get { return locale; }
        }

        private string locale;
        
        public string Position {
            get { return position; }
        }

        private string position;
        
        public string Type {
            get { return type; }
        }

        private string type;
        
        public string Value {
            get { return value; }
        }

        private string value;
        
        public GameAsset(string name, string locale, string position, string type, string value) {
            this.name = name;
            this.locale = locale;
            this.position = position;
            this.type = type;
            this.value = value;
        }
    }
}