using System.Collections.Generic;
using Newtonsoft.Json;

namespace SpilGames.Unity.Helpers
{ 
    /// <summary>
    /// Use this class to turn your JSON strings into objects and vice versa.
    /// Check the JSONHelper.cs file in the Helpers directory for an example of 
    /// how to do this with your game config!
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// If you have a JSON string then you can design a class that mirrors the structure of the JSON.
        /// You can then call this method and pass the JSON string as a parameter and your class as T.
        /// Check the JSONHelper.cs file in the Helpers directory for an example of how to do this with your
        /// game config!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns>A new instance (object) of your class containing. all the values from the JSON data</returns>
        public static T getObjectFromJson<T>(string jsonString) where T : new()
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        /// <summary>
        /// Turns an object into a JSON string so it can be easily stored or sent to another application.
        /// </summary>
        /// <param name="_object"></param>
        /// <returns></returns>
        public static string getJSONFromObject(object _object)
        {
            return JsonConvert.SerializeObject(_object);
        }
    }

    #region Config example

        /* This is an example of a game config with all kinds of variables that can be tweaked to 
        * balance a game without having to update it. 
        * If you are making your own JSON files, be sure to use http://jsonlint.com/ to validate them!
        {
            "Enemies":[
                {
                    "Name":"Zombie",
                    "HitPoints":"10",
                    "Damage":"10",
                    "AttackSpeed":"10",
                    "WalkSpeed":"1",
                    "CoinDropMinimum":"1",
                    "CoinDropMaximum":"2",
                    "LootLevel":"1",
                    "DropChance":"50"
                },
                {
                    "Name":"Skeleton",
                    "HitPoints":"20",
                    "Damage":"20",
                    "AttackSpeed":"20",
                    "WalkSpeed":"2",
                    "CoinDropMinimum":"2",
                    "CoinDropMaximum":"3",
                    "LootLevel":"2",
                    "DropChance":"25"
                },
                {
                    "Name":"Demon",
                    "HitPoints":"30",
                    "Damage":"30",
                    "AttackSpeed":"30",
                    "WalkSpeed":"3",
                    "CoinDropMinimum":"3",
                    "CoinDropMaximum":"4",
                    "LootLevel":"3",
                    "DropChance":"100"
                }
            ],
            "Levels":[
                {
                    "LevelNr":"1",
                    "TimeLimit":"60",
                    "LevelCompleteReward":"10",
                    "Enemies":[
                        {
                            "Name":"Zombie",
                            "AmountSpawnedMinimum":"2",
                            "AmountSpawnedMaximum":"3",
                            "SpawnChance":"50"
                        },
                        {
                            "Name":"Skeleton",
                            "AmountSpawnedMinimum":"1",
                            "AmountSpawnedMaximum":"2",
                            "SpawnChance":"35"
                        },
                        {
                            "Name":"Demon",
                            "AmountSpawnedMinimum":"1",
                            "AmountSpawnedMaximum":"1",
                            "SpawnChance":"15"
                        }
                    ],
                    "LootContainerSpawnChance":"50",
                    "LootContainerMinimumLevel":"1",
                    "LootContainerMaximumLevel":"2"
                }
            ]
        }
        */

        // This is a class that mirrors the shape of the data in the JSON file.
        // It can be used with the "GetConfigAsObject<T>()" method in Spil.cs to 
        // retrieve SLOT game configs as objects like this: 
        // GameConfig myConfig = GetConfigAsObject<GameConfig>();
        //
        // If you have your own JSON string you can turn it into an object in exactly the
        // same way using "JsonHelper.getObjectFromJson<T>(string jsonString)" like this:
        // MyCustomObject object = JsonHelper.getObjectFromJson<MyCustomObject>(string jsonString);
        //
        public class GameConfig
        {
            public List<Enemy> Enemies;
            public List<Level> Levels;

            // You can move these nested classes out of the GameConfig class if you like, I put them here so
            // you can clearly see how this class mirrors the JSON example above.
            public class Enemy
            {
                public string Name;
                public int HitPoints;
                public int Damage;
                public int AttackSpeed;
                public int WalkSpeed;
                public int CoinDropMinimum;
                public int CoinDropMaximum;
                public int LootLevel;
                public int DropChance;
            }

            public class Level
            {
                public int LevelNr;
                public int TimeLimit;
                public int LevelCompleteReward;

                public List<EnemyLevelConfig> Enemies;

                public class EnemyLevelConfig
                {
                    public string Name;
                    public int AmountSpawnedMinimum;
                    public int AmountSpawnedMaximum;
                    public int SpawnChance;
                }

                public int LootContainerSpawnChance;
                public int LootContainerMinimumLevel;
                public int LootContainerMaximumLevel;
            }
        }

    #endregion
}