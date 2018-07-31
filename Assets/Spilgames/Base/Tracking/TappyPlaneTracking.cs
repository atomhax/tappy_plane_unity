using System;
using SpilGames.Unity.Base.Implementations.Tracking;

namespace Spilgames.Base.Tracking {
    public class TappyPlaneTracking : SpilTracking {
        public string FileName = "TappyPlaneTracking";
        public string Version = "1.0.0";
        
        public new class BaseEquip : SpilTracking.BaseEquip {
            public BaseEquip(string equippedItem, string test) : base(equippedItem) {
                parameters.Add("test", test);
            }
        }

        public new static BaseEquip Equip(string equippedItem) {
            throw new NotSupportedException("Please use the method specified in the game specific tracking file.");
        }
        
        public static BaseEquip Equip(string equippedItem, string test) {
            return new BaseEquip(equippedItem, test);
        }

        public class Parameters {
            public string test = "test";
        }
    }
}