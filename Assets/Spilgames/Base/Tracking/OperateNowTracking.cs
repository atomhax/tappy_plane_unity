namespace Spilgames.Base.Tracking {
    public class OperateNowTracking : SpilTracking {
        public new class BaseLevelStart : SpilTracking.BaseLevelStart {
            public BaseLevelStart(string level, string hero) : base(level) {
                parameters.Add("hero", hero);
            }

            public BaseLevelStart AddId(string levelId) {
                parameters.Add("levelId", levelId);
                return this;
            }
        }
        
        public static BaseLevelStart LevelStart(string level, string hero) {
            return new BaseLevelStart(level, hero);
        }
    }
}