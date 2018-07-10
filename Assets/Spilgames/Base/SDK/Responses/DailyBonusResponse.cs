using System.Collections.Generic;

namespace SpilGames.Unity.Base.SDK {
    public class SpilDailyBonus {
        public string url;
        public string cycleType;
        public int is_consecutive;
        public string halfBeforeHalfAfter;
        public List<Day> days = new List<Day>(); 

        public class Day {
            public int day;
            public string status;
            public List<Collectible> collectibles = new List<Collectible>();

            public class Collectible {
                public int id;
                public string type;
                public int amount;
            }
        }
    }
}