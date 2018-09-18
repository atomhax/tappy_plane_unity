using System.Collections.Generic;
using SpilGames.Unity.Base.SDK;

namespace SpilGames.Unity.Helpers.DailyBonus {
    public class DailyBonus {
        public string Url {
            get { return url; }
        }

        private string url;
        
        public string Type {
            get { return type; }
        }

        private string type;
        
        public List<DayConfig> Days;

        public DailyBonus(string url, string type, List<SpilDailyBonus.SpilDayConfig> spilDaysConfig) {
            this.url = url;
            this.type = type;
            
            Days = new List<DayConfig>();
            foreach (SpilDailyBonus.SpilDayConfig dayConfig in spilDaysConfig) {
                Days.Add(new DayConfig(dayConfig.day, dayConfig.status, dayConfig.collectibles));
            }
        }

        public class DayConfig {
            public int Day {
                get { return day; }
            }

            private int day;

            public string Status {
                get { return status; }
            }

            private string status;
            
            public List<Collectible> Collectibles;

            public DayConfig(int day, string status, List<SpilDailyBonus.SpilDayConfig.SpilCollectible> spilCollectibles) {
                this.day = day;
                this.status = status;
                
                Collectibles = new List<Collectible>();
                foreach (SpilDailyBonus.SpilDayConfig.SpilCollectible collectible in spilCollectibles) {
                    Collectibles.Add(new Collectible(collectible.id, collectible.type, collectible.amount));
                }
            }

            public class Collectible {
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

                public Collectible(int id, string type, int amount) {
                    this.id = id;
                    this.type = type;
                    this.amount = amount;
                }
            }
        }
    }
}