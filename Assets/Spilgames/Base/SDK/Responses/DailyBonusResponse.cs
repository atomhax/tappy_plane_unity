using System.Collections.Generic;

namespace SpilGames.Unity.Base.SDK {
    public class SpilDailyBonus {
        public string url;
        public string type;
        public List<SpilDayConfig> days = new List<SpilDayConfig>(); 

        public class SpilDayConfig {
            public int day;
            public string status;
            public List<SpilCollectible> collectibles = new List<SpilCollectible>();

            public class SpilCollectible {
                public int id;
                public string type;
                public int amount;
            }
        }
    }
}