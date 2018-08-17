﻿using System;
using System.Collections.Generic;

namespace SpilGames.Unity.Helpers.PlayerData.Perk {
    public class PerkItem {
        public string name;
        public List<PerkPriceReduction> priceReductions = new List<PerkPriceReduction>();
        public List<PerkAddition> additions = new List<PerkAddition>();
        public PerkItem(string name) {
            this.name = name;
        }
    }

    public class PerkPriceReduction {
        public int currencyId;
        public int discountValue;

        public PerkPriceReduction(int currencyId, int discountValue) {
            this.currencyId = currencyId;
            this.discountValue = discountValue;
        }
    }
    
    public class PerkAddition {
        public int id;
        public string type;
        public int additionalValue;

        public enum PerkAdditionType {
            CURRENCY,
            ITEM
        }

        public PerkAddition(int id, PerkAdditionType type, int additionalValue) {
            this.id = id;
            this.type = type.ToString();
            this.additionalValue = additionalValue;
        }
    }
    
    
}