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
    }
}