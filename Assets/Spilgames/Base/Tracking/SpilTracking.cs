using System.Collections.Generic;
using SpilGames.Unity;
using SpilGames.Unity.Json;

namespace Spilgames.Base.Tracking {
    public class SpilTracking {
        public abstract class BaseTracking {
            protected string eventName;
            protected Dictionary<string, object> parameters = new Dictionary<string, object>();

            public void Track() {
                Spil.Instance.SendCustomEvent(eventName, parameters);
            }
        }

        public class BaseMilestoneAchieved : BaseTracking {
            public BaseMilestoneAchieved(string name) {
                eventName = "milestoneAchieved";
                parameters.Add("name", name);
            }

            /// <param name="description"></param>
            public BaseMilestoneAchieved AddMilestoneDescription(string description) {
                parameters.Add("description", description);
                return this;
            }
            
            /// <param name="score"></param>
            public BaseMilestoneAchieved AddScore(float score) {
                parameters.Add("score", score);
                return this;
            }
            
            /// <param name="location"></param>
            public BaseMilestoneAchieved AddLocation(string location) {
                parameters.Add("location", location);
                return this;
            }
            
            /// <param name="iteration"></param>
            public BaseMilestoneAchieved AddIteration(string iteration) {
                parameters.Add("iteration", iteration);
                return this;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public static BaseMilestoneAchieved MilestoneAchieved(string name) {
            return new BaseMilestoneAchieved(name);
        }
        
        public class BaseLevelStart : BaseTracking {
            public BaseLevelStart(string level) {
                eventName = "levelStart";
                parameters.Add("level", level);
            }

            /// <param name="difficulty"></param>
            public BaseLevelStart AddDifficulty(string difficulty) {
                parameters.Add("difficulty", difficulty);
                return this;
            }
            
            /// <param name="customCreated"></param>
            public BaseLevelStart AddCustomCreated(float customCreated) {
                parameters.Add("customCreated", customCreated);
                return this;
            }
            
            /// <param name="creatorId"></param>
            public BaseLevelStart AddCreatorId(string creatorId) {
                parameters.Add("creatorId", creatorId);
                return this;
            }
            
            /// <param name="activeBooster"></param>
            public BaseLevelStart AddActiveBooster(List<string> activeBooster) {
                parameters.Add("activeBooster", new JSONObject(JsonHelper.getJSONFromObject(activeBooster)));
                return this;
            }            
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        public static BaseLevelStart LevelStart(string level) {
            return new BaseLevelStart(level);
        }
    }
}