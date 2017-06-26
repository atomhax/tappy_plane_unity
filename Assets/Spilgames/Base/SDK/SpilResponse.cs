using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpilGames.Unity.Base.SDK {
    public class SpilResponse {
        public string name;
        public string type;
        public string action;
        public string message;

        // The data field is not included here since it's object type varies between the different
        // types of responses. Use the type field to identify the specific type of SpilResponse,
        // then deserialize the JSON string again using that type. TODO: Make that prettier?
    }

    public class SpilErrorMessage {
        public int id;
        public string name;
        public string message;
    }
}