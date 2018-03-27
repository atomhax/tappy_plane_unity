namespace SpilGames.Unity.Helpers.GameData {
    /// <summary>
    /// This is the business object that the developer can use to work with an Item.
    /// </summary>
    public class GachaContent {
        /// <summary>
        /// The item Id
        /// </summary>
        public int Id {
            get { return id; }
        }

        private int id;

        /// <summary>
        /// The content type (can be CURRENCY, ITEM, GACHA, BUNDLE, NONE)
        /// </summary>
        public string Type {
            get { return type; }
        }

        private string type;

        /// <summary>
        /// The weight which is used to calculate the probablity of this object being received by the user.
        /// </summary>
        public int Amount {
            get { return amount; }
        }

        private int amount;

        /// <summary>
        /// The weight which is used to calculate the probablity of this object being received by the user.
        /// </summary>
        public int Weight {
            get { return weight; }
        }

        private int weight;

        /// <summary>
        /// The position on which the gacha should be displayed.
        /// </summary>
        public int Position {
            get { return position; }
        }

        private int position;
        
        /// <summary>
        /// The image url for the customised 
        /// </summary>
        public string ImageUrl {
            get { return imageUrl; }
        }

        private string imageUrl;
        
        public GachaContent(int id, string type, int amount, int weight, int position, string imageUrl) {
            this.id = id;
            this.type = type;
            this.amount = amount;
            this.weight = weight;
            this.position = position;
            this.imageUrl = imageUrl;
        }
    }
}