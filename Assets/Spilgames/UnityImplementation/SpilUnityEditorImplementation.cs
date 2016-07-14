#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json;
using SpilGames.Unity.Utils;
using SpilGames.Unity.Helpers;

namespace SpilGames.Unity.Implementations
{

	public class SpilUnityEditorImplementation : SpilUnityImplementationBase {

			#region Inherited members

			#region Game config

			/// <summary>
			/// Returns the game config as a json string.
			/// This is not essential for developers so could be made private (getConfig T () uses it so it cannot be removed entirely) but might be handy for some developers so we left it in.
			/// </summary>
			/// <returns></returns>     
			public override string GetConfigAll()
			{
				string config = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/defaultGameConfig.json");
				return config;
			}

			/// <summary>
			/// Method that returns a configuration value from the game config based on key 
			/// </summary>
			/// <param name="key"></param>
			/// <returns></returns>
			public override string GetConfigValue(string key)
			{
				return "Not Avalible in editorMode";
			}

			#endregion

			#region Packages and promotions

			/// <summary>
			/// Method that requests packages and promotions from the server.
			/// This is called automatically by the Spil SDK when the game starts.
			/// This is not essential so could be removed but might be handy for some developers so we left it in.
			/// </summary>
			public override void UpdatePackagesAndPromotions()
			{
				
			}

            protected override string GetAllPackages()
            {
                return "Not Avalible in editorMode";
            }

            protected override string GetPackage(string key)
            {
                return "Not Avalible in editorMode";
            }

        	/// <summary>
			/// This method is marked as internal and should not be exposed to developers.
			/// The Spil Unity SDK is not packaged as a seperate assembly yet so this method is currently visible, this will be fixed in the future.
			/// Internal method names start with a lower case so you can easily recognise and avoid them.
			/// </summary>
            internal override string getPromotion(string key)
            {
                return "Not Avalible in editorMode";
            }

			#endregion

			/// <summary>
			/// This method is marked as internal and should not be exposed to developers.
			/// The Spil Unity SDK is not packaged as a seperate assembly yet so this method is currently visible, this will be fixed in the future.
			/// Internal method names start with a lower case so you can easily recognise and avoid them.
			/// </summary>
			internal override void SpilInit()
			{
				
			}

			/// <summary>
			/// Sends an event to the native Spil SDK which will send a request to the back-end.
			/// Custom events may be tracked as follows:
			/// To track an simple custom event, simply call Spil.Instance.SendCustomEvent(String eventName); from anywhere in your code.
			/// To pass more information with the event create a &lt;String, String&gt; Dictionary and pass that as the second parameter like so:
			/// Dictionary&lt;String, String&gt; eventParams = new Dictionary&lt;String,String&gt;();
			/// eventParams.Add(“level”,levelName);
			/// Spil.Instance.SendCustomEvent(“PlayerDeath”, eventParams);
			/// See https://github.com/spilgames/spil_event_unity_plugin for more information on events.
			/// This method was previously called "TrackEvent".
			/// </summary>
			/// <param name="eventName"></param>
			/// <param name="dict"></param>
			public override void SendCustomEvent(string eventName, Dictionary<string, string> dict)
			{
				Debug.Log("SpilSDK-Unity SendCustomEvent " + eventName);
			}

			/// <summary>
			/// This can be called to show a video, for instance after calling "SendrequestRewardVideoEvent()"
			/// and receiving an "AdAvailable" event the developer could call this method from the event handler.
			/// When calling this method "SendrequestRewardVideoEvent()" must first have been called to request and cache a video.
			/// If no video is available then nothing will happen.
			/// </summary>
			public override void PlayVideo()
			{
				
			}

			/// <summary>
			/// This can be called to show the "more apps" activity, for instance after calling "RequestMoreApps()"
			/// and receiving an "AdAvailable" event the developer could call this method from the event handler.
			/// When calling this method "RequestMoreApps()" must first have been called to request and cache a video.
			/// If no video is available then nothing will happen.
			/// </summary>
			public override void PlayMoreApps()
			{
				
			}

            /// <summary>
            /// Sends the "requestAd" event with the "moreApps" parameter to the native Spil SDK which will send a request to the back-end.
            /// When a response has been received from the back-end the SDK will fire either an "AdAvailable" or and "AdNotAvailable"
            /// event to which the developer can subscribe and for instance call PlayVideo(); or PlayMoreApps();
            /// </summary>
			public override void RequestMoreApps()
			{
				
			}

			/// <summary>
			/// When Fyber has shown a reward video and the user goes back to the game to receive his/her reward Fyber can
			/// automatically show a toast message with information about the reward, for instance "You've received 50 coins". 
			/// This is disabled by default to allow the developer to create a reward notification for the user.
			/// Developers can call SetShowToastOnVideoReward(true) to enable Fyber's automatic toast message.
			/// </summary>
			public override void SetShowToastOnVideoReward(bool value)
            {
               
            }

			#endregion
			
			#region Spil Game Objects
			
			public override string GetSpilGameDataFromSdk ()
			{
				string gameData = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/defaultGameData.json");
				return gameData;
			}
			
			#endregion
			
			#region Player Data
			
			public override string GetWalletFromSdk()
			{
				string playerData = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/defaultPlayerData.json");
				TempUserInfo temp = JsonHelper.getObjectFromJson<TempUserInfo> (playerData);
				string wallet = JsonHelper.getJSONFromObject(temp.wallet);
				return wallet;
			}
			
			public override string GetInvetoryFromSdk()
			{
				string playerData = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/defaultPlayerData.json");
				TempUserInfo temp = JsonHelper.getObjectFromJson<TempUserInfo> (playerData);
				string inventory = JsonHelper.getJSONFromObject(temp.inventory);
				return inventory;
			}
			
			public override void AddCurrencyToWallet (int currencyId, int amount, string reason)
			{
				CallNativeMethod("addCurrencyToWallet", new object[]{ currencyId, amount, reason });
			}
			
			public override void SubtractCurrencyFromWallet (int currencyId, int amount, string reason)
			{
				CallNativeMethod("subtractCurrencyFromWallet", new object[]{ currencyId, amount, reason });
			}
			
			public override void AddItemToInventory (int itemId, int amount, string reason)
			{
				CallNativeMethod("addItemToInventory", new object[]{ itemId, amount, reason });
			}
			
			public override void SubtractItemFromInventory (int itemId, int amount, string reason)
			{
				CallNativeMethod("subtractItemFromInventory", new object[]{ itemId, amount, reason });
			}
			
			public override void ConsumeBundle (int bundleId, string reason)
			{
				CallNativeMethod("consumeBundle", new object[]{ bundleId, reason });
			}
			
			#endregion

			#region Non inherited members (Android only members)

			#region DFP / Fyber / Chartboost

			/// <summary>
			/// Method that initiaties DFP Ads (to be used only for testing purposes).
			/// This is not essential for developers so could be hidden but it might be handy for some developers so we left it in.
			/// </summary>
			/// <param name="adUnitId"></param>
			public void TestStartDFP(string adUnitId)
			{
				
			}

			/// <summary>
			/// Method that initiaties Fyber Ads (to be used only for testing purposes).
			/// This is not essential for developers so could be hidden but it might be handy for some developers so we left it in.
			/// </summary>
			/// <param name="appId"></param>
			/// <param name="token"></param>
			public void TestStartFyber(string appId, string token)
			{
				
			}

			/// <summary>
			/// Method that shows Chartboost more apps (to be used only for testing purposes).
			/// This is not essential for developers so could be hidden but it might be handy for some developers so we left it in.
			/// </summary>
			/// <param name="appId"></param>
			/// <param name="appSignature"></param>
			public void TestStartChartBoost(string appId, string appSignature)
			{
				
			}

			/// <summary>
			/// Method that requests ads (to be used only for testing purposes).
			/// This is not essential for developers so could be hidden but it might be handy for some developers so we left it in.
			/// Use SendrequestRewardVideoEvent() if you want to request an ad!
			/// </summary>
			/// <param name="provider"></param>
			/// <param name="adType"></param>
			/// <param name="parentalGate"></param>
			public override void TestRequestAd(string provider, string adType, bool parentalGate)
			{
				
			}

			#endregion

			#region Push notifications

			/// <summary>
			/// Enables push notification for Android.
			/// Be sure to fill in the correct Project_ID in Spil.cs
			/// </summary>
			public void EnablePushNotifications()
			{
				
			}

			/// <summary>
			/// Disables push notification for Android.
			/// </summary>
			public void DisablePushNotifications()
			{
				
			}

			/// <summary>
            /// Disables the automatic register for push notifications for iOS.
            /// Should be called before the SpilInit() call!
            /// </summary>
            public void DisableAutomaticRegisterForPushNotifications()
            {
            
            }

            /// <summary>
            /// Registers for push notifications for iOS.
            /// Can be used then the automatic registration was disabled using: DisableAutomaticRegisterForPushNotifications();
            /// </summary>
            public void RegisterForPushNotifications()
            {
            }

			#endregion

			// If Android, register device
			private void RegisterDevice(string projectID)
			{
				
			}

			/// <summary>
			/// Retrieves the Spil User Id so that developers can show this in-game for users.
			/// If users contact Spil customer service they can supply this Id so that 
			/// customer support can help them properly. Please make this Id available for users
			/// in one of your game's screens.
			/// </summary>
			public override string GetSpilUID()
			{
				return "1337";
			}

			/// <summary>
			/// Used only by the Test app to check that calls were successfully made.
			/// This method is marked as "internal" and should not be exposed to developers.
			/// The Spil Unity SDK is not packaged as a seperate assembly yet so this method is currently visible, this will be fixed in the future.
			/// Internal method names start with a lower case so you can easily recognise and avoid them.
			/// </summary>
			internal string clearLog()
			{
				return "";
			}

			/// <summary>
			/// Used only by the Test app to check that calls were successfully made.
			/// This method is marked as internal and should not be exposed to developers.
			/// The Spil Unity SDK is not packaged as a seperate assembly yet so this method is currently visible, this will be fixed in the future.
			/// Internal method names start with a lower case so you can easily recognise and avoid them.
			/// </summary>
			internal string getLog()
			{
				return "";
			}

			private string CallNativeMethod(string methodName)
			{
				return "";
			}

			/// <summary>
			/// Calls a native method with the given method name. When using param1 T does not need to be supplied, the type of the object passed as param1 will be used automatically.
			/// </summary>
			/// <typeparam name="T"></typeparam>
			/// <param name="methodName"></param>
			/// <param name="param1"></param>
			/// <param name="useParam1"></param>
			/// <returns></returns>
			private string CallNativeMethod<T>(string methodName, T param1 = null, bool useParam1 = false) where T: class
			{
				string value = null;
				return value;
			}

        #endregion
    }

	public class TempUserInfo{
		public WalletData wallet;
		public InventoryData inventory;
	}

	}
#endif
