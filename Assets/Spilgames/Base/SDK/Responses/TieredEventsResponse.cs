using System.Collections.Generic;

namespace SpilGames.Unity.Base.SDK {
    public class TieredEvent {
        public int id;
        public string name;
        public string type;
        public long startDate;
        public long endDate;
        public string imageUrl;
        public List<TieredEventTier> tiers;
    }

    public class TieredEventTier {
        public int id;
        public string name;
        public int entityId;
        public string entityType;
        public int entityTierStart;
        public int entityTierEnd;
        public string imageUrl;
        public List<TieredEventReward> rewards;
    }

    public class TieredEventProgress {
        public int tieredEventId;
        public int currentTierId;
        public int previousAmount;
        public int currentAmount;
        public List<int> completedTiers;
        public List<int> claimableTiers;
    }

    public class ShowProgressResponse : TieredEventProgress {
        public string url;
        public int tieredEventId;
        public int currentAmount;
        public int currentTierId;
        public List<int> completedTiers;
        public List<int> claimableTiers;
    }

    public class TieredEventReward {
        public int id;
        public string imageUrl;
        public string type;
        public int amount;
    }

    public class TieredEventsOverview {
        public Dictionary<int, TieredEvent> tieredEvents = new Dictionary<int, TieredEvent>();
        public Dictionary<int, TieredEventProgress> progress = new Dictionary<int, TieredEventProgress>();
    }

    public class TieredEventsResponseData {
        public List<TieredEvent> tieredEvents;
        public List<TieredEventProgress> progress;
    }

    public class TieredEventTierRewardData {
        public int id;
        public List<TieredEventReward> rewards;
    }

    public class ClaimRewardData : TieredEventProgress {
        public List<TieredEventTierRewardData> tiers;
    }
}