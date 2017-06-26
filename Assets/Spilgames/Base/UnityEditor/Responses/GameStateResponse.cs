#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using SpilGames.Unity;
using System;
using System.Collections.Generic;
using SpilGames.Unity.Base.Implementations;
using SpilGames.Unity.Json;
using SpilGames.Unity.Base.SDK;

namespace SpilGames.Unity.Base.UnityEditor.Responses {
    public class GameStateResponse : Response {
        public static string PublicGameStateData;

        public static string PrivateGameStateData;

        public static void ProcessGameStateResponse(ResponseEvent response) {
            if (response.action == null) return;
            if (response.action.ToLower().Trim().Equals("update")) {
                ProcessMyGameStateResponse(response.data);
            }
            else if (response.action.ToLower().Trim().Equals("otherusers")) {
                ProcessOtherUsersGameStateResponse(response.data);
            }
        }

        private static void ProcessMyGameStateResponse(JSONObject jsonData) {
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
                    PrivateGameStateData = data;
                    SpilUnityImplementationBase.fireGameStateUpdated("private");
                }
                else if (access.Equals("public")) {
                    PublicGameStateData = data;
                    SpilUnityImplementationBase.fireGameStateUpdated("public");
                }
            }
        }

        private static void ProcessOtherUsersGameStateResponse(JSONObject jsonData) {
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
            PrivateGameStateData = gameState;

            SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent>();
            spilEvent.eventName = "updateGameState";

            gameState = gameState.Replace("\"", "\\\"");

            spilEvent.customData.AddField("data", gameState);
            spilEvent.customData.AddField("access", "private");

            spilEvent.Send();
        }

        public static void SetPublicGameState(String gameState) {
            if (Response.externalId != null && Response.provider != null) {
                PublicGameStateData = gameState;

                SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent>();
                spilEvent.eventName = "updateGameState";

                gameState = gameState.Replace("\"", "\\\"");

                spilEvent.customData.AddField("data", gameState);
                spilEvent.customData.AddField("access", "public");

                spilEvent.Send();
            }
            else {
                SpilErrorMessage error = new SpilErrorMessage();
                error.id = 12;
                error.name = "UserIdMissing";
                error.message =
                    "Error adding public game state data! A custom user id must be set in order to save public game state data";
                SpilUnityImplementationBase.fireGameStateError(JsonUtility.ToJson(error));
            }
        }
    }
}
#endif