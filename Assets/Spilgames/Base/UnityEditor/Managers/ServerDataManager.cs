﻿#if UNITY_EDITOR || UNITY_WEBGL
using SpilGames.Unity.Base.Implementations;
using SpilGames.Unity.Base.SDK;
using UnityEngine;

namespace SpilGames.Unity.Base.UnityEditor.Managers {
    public class ServerTimeManager {
        
    }

    public class ServerDataResponse : Response {
        public static void ProcessServerTimeResponse(ResponseEvent response) {
            if (response.data == null) return;

            long serverTime = 0;

            if (response.data.HasField("serverTime")) {
                serverTime = response.data.GetField("serverTime").i;
            } else {
				Spil.Instance.fireServerTimeRequestFailed(JsonUtility.ToJson(new SpilErrorMessage(24, "ServerTimeRequestError", "Error retrieving server time")));
            }

            if (response.action.Equals("serverTime")) {
				Spil.Instance.fireServerTimeRequestSuccess(serverTime.ToString());
            }
        }
    }
}
#endif