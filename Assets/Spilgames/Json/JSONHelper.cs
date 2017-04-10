using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace SpilGames.Unity.Json
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
		public static T getObjectFromJson<T> (string jsonString) where T : new()
		{
			try {
				return JsonConvert.DeserializeObject<T> (jsonString);
			} catch (System.MissingMethodException ex) {
				Debug.Log ("Something has gone wrong while loading NewtonSoft.json. Most likely the file \"link.xml\" is missing from the /Assets directory of your Unity project. Please copy the \"link.xml\" file included with the Unity SpilSDK download to the /Assets folder of your project.\n" + ex.Message);
				throw new System.Exception ();
			} catch (System.Exception ex) {
				Debug.Log ("Something has gone wrong while loading NewtonSoft.json or parsing the supplied JSON. Exception: " + ex.Message);
				throw new System.Exception ();
			}
		}

		/// <summary>
		/// Turns an object into a JSON string so it can be easily stored or sent to another application.
		/// </summary>
		/// <param name="_object"></param>
		/// <returns></returns>
		public static string getJSONFromObject (object _object)
		{
			try {
				return JsonConvert.SerializeObject (_object);
			} catch (System.MissingMethodException ex) {
				Debug.Log ("Something has gone wrong while loading NewtonSoft.json. Most likely the file \"link.xml\" is missing from the /Assets directory of your Unity project. Please copy the \"link.xml\" file included with the Unity SpilSDK download to the /Assets folder of your project.\n" + ex.Message);
				throw new System.Exception ();
			} catch (System.Exception ex) {
				Debug.Log ("Something has gone wrong while loading NewtonSoft.json or parsing the supplied JSON. Exception: " + ex.Message);
				throw new System.Exception ();
			}
		}
	}
}