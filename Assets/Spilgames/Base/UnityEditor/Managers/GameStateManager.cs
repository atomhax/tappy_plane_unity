#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using SpilGames.Unity;
using System;
using System.Collections.Generic;
using NUnit.Framework.Internal;
using SpilGames.Unity.Base.Implementations;
using SpilGames.Unity.Json;
using SpilGames.Unity.Base.SDK;

namespace SpilGames.Unity.Base.UnityEditor.Managers {
    public class GameStateManager {
        public static string PublicGameStateData;
        public static string PrivateGameStateData;

        public static void ProcessMyGameStateResponse(JSONObject jsonData) {
            if (!jsonData.GetField("gameStates")) return;
            JSONObject gameStateData = jsonData.GetField("gameStates");

            foreach (JSONObject gameState in gameStateData.list) {
                string access = null;
                if (gameState.HasField("access")) {
                    access = gameState.GetField("access").str;
                }

                string data = null;
                if (gameState.HasField("data")) {
                    data = gameState.GetField("data").str;
                }

                if (access == null || data == null) continue;
                if (access.Equals("private")) {
                    if (data.Contains("\"")) {
                        data = data.Replace("\"", "\\\"").Replace("\\\\", "\\");
                    }
                    
                    PrivateGameStateData = data;
                    SpilUnityImplementationBase.fireGameStateUpdated("private");
                } else if (access.Equals("public")) {
                    if (data.Contains("\"")) {
                        data = data.Replace("\"", "\\\"").Replace("\\\\", "\\");
                    }
                    PublicGameStateData = data;
                    SpilUnityImplementationBase.fireGameStateUpdated("public");
                }
            }
        }

        public static void ProcessOtherUsersGameStateResponse(JSONObject jsonData) {
            JSONObject json = new JSONObject();

            String provider = null;
            if (jsonData.HasField("provider")) {
                provider = jsonData.GetField("provider").str;
                json.AddField("provider", provider);
            }

            JSONObject gameStates = null;
            if (jsonData.HasField("gameStates")) {
                gameStates = jsonData.GetField("gameStates");
                json.AddField("data", gameStates);
            }

            SpilUnityImplementationBase.fireOtherUsersGameStateLoaded(json.Print(false));
        }

        public static void SetPrivateGameState(String gameState) {
            UserDataManager.UpdateUserDataVersions();
            UserDataManager.UpdateUserDataMeta();

            SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent>();
            spilEvent.eventName = "updateGameState";
             
            if (gameState.Contains("\"")) {
                gameState = gameState.Replace("\"", "\\\"").Replace("\\\\", "\\");
            }
            
            PrivateGameStateData = gameState;

            JSONObject gameStateJSONArray = new JSONObject(JSONObject.Type.ARRAY);
            JSONObject gameStateJSON = new JSONObject();
                
            gameStateJSON.AddField("data", gameState);
            gameStateJSON.AddField("access", "private");
            gameStateJSONArray.Add(gameStateJSON);
                
            spilEvent.customData.AddField("gameStates", gameStateJSONArray);
            spilEvent.customData.AddField("deviceVersions", UserDataManager.GenerateUserDataVersionsJSON(UserDataManager.userDataVersions));
            
            spilEvent.Send();
        }

        public static void SetPublicGameState(String gameState) {
            if (Response.externalId != null && Response.provider != null) {
                UserDataManager.UpdateUserDataVersions();
                UserDataManager.UpdateUserDataMeta();              

                SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent>();
                spilEvent.eventName = "updateGameState";

                if (gameState.Contains("\"")) {
                    gameState = gameState.Replace("\"", "\\\"").Replace("\\\\", "\\");
                }
                
                PublicGameStateData = gameState;

                JSONObject gameStateJSONArray = new JSONObject(JSONObject.Type.ARRAY);
                JSONObject gameStateJSON = new JSONObject();
                
                gameStateJSON.AddField("data", gameState);
                gameStateJSON.AddField("access", "public");
                gameStateJSONArray.Add(gameStateJSON);
                
                spilEvent.customData.AddField("gameStates", gameStateJSONArray);
                spilEvent.customData.AddField("deviceVersions", UserDataManager.GenerateUserDataVersionsJSON(UserDataManager.userDataVersions));
                
                spilEvent.Send();
            } else {
                SpilErrorMessage error = new SpilErrorMessage();
                error.id = 12;
                error.name = "UserIdMissing";
                error.message =
                    "Error adding public game state data! A custom user id must be set in order to save public game state data";
                SpilUnityImplementationBase.fireUserDataError(JsonUtility.ToJson(error));
            }
        }
    }

    public class GameStateResponse : Response {
        public static void ProcessGameStateResponse(ResponseEvent response) {
            if (response.action == null) return;
            if (response.action.ToLower().Trim().Equals("update")) {
                GameStateManager.ProcessMyGameStateResponse(response.data);
            } else if (response.action.ToLower().Trim().Equals("otherusers")) {
                GameStateManager.ProcessOtherUsersGameStateResponse(response.data);
            } else if (response.action.ToLower().Trim().Equals("syncerror")) {
                UserDataManager.ProcessSyncError();
            } else if (response.action.ToLower().Trim().Equals("dropped")) {
                UserDataManager.ProcessDroppedResponse("Event dropped!");
            } else if (response.action.ToLower().Trim().Equals("lockerror")) {
                UserDataManager.ProcessLockError();
            }
        }
    }
}
#endif