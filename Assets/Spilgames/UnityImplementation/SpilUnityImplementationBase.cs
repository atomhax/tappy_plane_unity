﻿using System.Collections.Generic;
using System;
using UnityEngine;
using SpilGames.Unity.Helpers;
using SpilGames.Unity.Utils;

namespace SpilGames.Unity.Implementations
{ 
    public abstract class SpilUnityImplementationBase
    {
	public static string PluginName = "Unity";
	public static string PluginVersion = "2.2.2";

	public abstract void SetPluginInformation(string PluginName, string PluginVersion);

        #region Misc.

            /// <summary>
            /// This method is marked as internal and should not be exposed to developers.
            /// The Spil Unity SDK is not packaged as a seperate assembly yet so unfortunately this method is currently visible.
            /// Internal method names start with a lower case so you can easily recognise and avoid them.
            /// </summary>
			internal abstract void SpilInit();  

        #endregion

        #region Game config

            /// <summary>
            /// Method that returns the SLOT Config as a (user-defined) object. See the example in JSONHelper.cs for
            /// more information on how to turn JSON strings (such as the SLOT game config) into classes.
            /// This method currently does not catch and handle any exceptions and will not present the developer with
            /// handy tips for fixing his/her JSON or class. Developers will have to make due with the standard
            /// exceptions for now for debugging their JSON and classes. This may be improved in the future.
            /// </summary>
            /// <typeparam name="T">The user-defined class that mirrors the shape of the data in the JSON</typeparam>
            /// <returns></returns>
            public T GetConfig<T>() where T : new()
            {
                // TODO: Handle exceptions gracefully
                return JsonHelper.getObjectFromJson<T>(GetConfigAll());
            }

            public abstract string GetConfigAll();

            public abstract string GetConfigValue(string key);

        #endregion

        #region Packages and promotions

            // This is not essential so could be removed but might be handy for some developers so we left it in.
            public abstract void UpdatePackagesAndPromotions();

            /// <summary>
            /// Fetch packages and promotions locally. Packages and promotions are requested from the server when the app starts and are cached.
            /// </summary>
            /// <returns>A packageshelper object filled with packages and promotions, will be empty if there are none. Returns null if no packages or promotions are present, which should only happen if the server has never been successfully queried for packages and promotions.</returns>
            public PackagesHelper GetPackagesAndPromotions()
            {
                PackagesHelper helper = null;
                string packagesString = GetAllPackages();
                if(packagesString != null)
                {
                    List<PackageData> packagesList = JsonHelper.getObjectFromJson<List<PackageData>>(packagesString);
                    helper = new PackagesHelper(packagesList);
                }
                return helper;
            }

            protected abstract string GetAllPackages();

            //Method that returns a package based on key
            protected abstract string GetPackage(string key);

			/// <summary>
			/// This method is marked as internal and should not be exposed to developers.
			/// The Spil Unity SDK is not packaged as a seperate assembly yet so this method is currently visible, this will be fixed in the future.
			/// Internal method names start with a lower case so you can easily recognise and avoid them.
			/// </summary>
            internal abstract string getPromotion(string key);

        #endregion

        #region Events         

            #region Standard Spil events

		[Obsolete]
                /// <summary>
                /// Sends the "milestoneAchieved" event to the native Spil SDK which will send a request to the back-end.
                /// See https://github.com/spilgames/spil_event_unity_plugin for more information on events.
                /// </summary>
                /// <param name="name"></param>
                public void SendmilestoneAchievedEvent(string name)
                {
                    SendCustomEvent("milestoneAchieved", new Dictionary<string, string>() { { "name", name } });
                }

		/// <summary>
                /// Sends the "milestoneAchieved" event to the native Spil SDK which will send a request to the back-end.
		/// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
                /// </summary>
                /// <param name="name"></param>
                public void TrackMilestoneAchievedEvent(string name)
                {
                    SendCustomEvent("milestoneAchieved", new Dictionary<string, string>() { { "name", name } });
                }

                [Obsolete]
                /// <summary>
                /// Sends the "levelStart" event to the native Spil SDK which will send a request to the back-end.
                /// See https://github.com/spilgames/spil_event_unity_plugin for more information on events.
                /// </summary>
                /// <param name="levelName"></param>
                public void SendlevelStartEvent(string levelName)
                {
                    SendCustomEvent("levelStart", new Dictionary<string, string>() { { "level", levelName } });
                }

		/// <summary>
                /// Sends the "levelStart" event to the native Spil SDK which will send a request to the back-end.
		/// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
                /// </summary>
                /// <param name="levelName"></param>
                public void TrackLevelStartEvent(string levelName)
                {
                    SendCustomEvent("levelStart", new Dictionary<string, string>() { { "level", levelName } });
                }

                [Obsolete]
                /// <summary>
                /// Sends the "levelComplete" event to the native Spil SDK which will send a request to the back-end.
                /// See https://github.com/spilgames/spil_event_unity_plugin for more information on events.
                /// </summary>
                /// <param name="levelName"></param>
                public void SendlevelCompleteEvent(string levelName)
                {
                    SendCustomEvent("levelComplete", new Dictionary<string, string>() { { "level", levelName } });
                }

		/// <summary>
                /// Sends the "levelComplete" event to the native Spil SDK which will send a request to the back-end.
		/// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
                /// </summary>
                /// <param name="levelName"></param>
                public void TrackLevelCompleteEvent(string levelName)
                {
                    SendCustomEvent("levelComplete", new Dictionary<string, string>() { { "level", levelName } });
                }

                [Obsolete]
                /// <summary>
                /// Sends the "levelFailed" event to the native Spil SDK which will send a request to the back-end.
                /// See https://github.com/spilgames/spil_event_unity_plugin for more information on events.
                /// </summary>
                /// <param name="levelName"></param>
                public void SendlevelFailedEvent(string levelName)
                {
                    SendCustomEvent("levelFailed", new Dictionary<string, string>() { { "level", levelName } });
                }

		/// <summary>
                /// Sends the "levelFailed" event to the native Spil SDK which will send a request to the back-end.
		/// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
                /// </summary>
                /// <param name="levelName"></param>
                public void TrackLevelFailedEvent(string levelName)
                {
                    SendCustomEvent("levelFailed", new Dictionary<string, string>() { { "level", levelName } });
                }

                [Obsolete]
                /// <summary>
                /// Sends the "playerDies" event to the native Spil SDK which will send a request to the back-end.
                /// See https://github.com/spilgames/spil_event_unity_plugin for more information on events.
                /// </summary>
                /// <param name="levelName"></param>
                public void SendplayerDiesEvent(string levelName)
                {
                    SendCustomEvent("playerDies", new Dictionary<string, string>() { { "level", levelName } });
                }

		/// <summary>
                /// Sends the "playerDies" event to the native Spil SDK which will send a request to the back-end.
		/// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
                /// </summary>
                /// <param name="levelName"></param>
                public void TrackPlayerDiesEvent(string levelName)
                {
                    SendCustomEvent("playerDies", new Dictionary<string, string>() { { "level", levelName } });
                }

                /// <summary>
                /// Sends the "requestRewardVideo" event to the native Spil SDK which will send a request to the back-end.
                /// When a response has been received from the back-end the SDK will fire either an "AdAvailable" or and "AdNotAvailable"
                /// event to which the developer can subscribe and for instance call PlayVideo(); or PlayMoreApps();
		/// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
                /// </summary>
                public void SendRequestRewardVideoEvent(string rewardType = null)
                {
                    SendCustomEvent("requestRewardVideo", rewardType == null ? null : new Dictionary<string, string>() { { "rewardType", rewardType } });
                }

                // Deprecated this old method in favor of SendRequestRewardVideoEvent() for uniformity
                /// <summary>
                /// Sends the "requestRewardVideo" event to the native Spil SDK which will send a request to the back-end.
                /// When a response has been received from the back-end the SDK will fire either an "AdAvailable" or and "AdNotAvailable"
                /// event to which the developer can subscribe and for instance call PlayVideo(); or PlayMoreApps();
                /// </summary>
                [Obsolete("ShowRewardedVideo() is deprecated, please use SendRequestRewardVideoEvent() instead. This method will be removed in the next version.")]                
                public void ShowRewardedVideo()
                {
                    SendCustomEvent("requestRewardVideo");
                }

                [Obsolete("Use the new TrackWalletInventoryEvent() instead!")]
                /// <summary>
                /// Sends the "walletUpdate" event to the native Spil SDK which will send a request to the back-end.
                /// See https://github.com/spilgames/spil_event_unity_plugin for more information on events.
                /// </summary>
                /// <param name="name"></param>
                /// <param name="walletValue">The new wallet value after subtracting the item value. E.g coins</param>
                /// <param name="itemValue">The value of the item consumed. E.g. coins. (note: This property can also be negative, for example if a user spends coins, the itemValue can be -100)</param>
                /// <param name="source">(int) - 0 == premium</param>
                /// <param name="item">item id or sku</param>
                /// <param name="category">(int) - 0 = Consumable, 1 = Booster, 2 = Permanent</param>
		public void SendwalletUpdateEvent(string walletValue, string itemValue, string source, string item, string category)
                {
                    SendCustomEvent("walletUpdate", new Dictionary<string, string>() { { "walletValue", walletValue }, { "itemValue", itemValue }, { "source", source }, { "item", item }});
                }

		/// <summary>
                /// Sends the "updatePlayerData" event to the native Spil SDK which will send a request to the back-end.
		/// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
                /// </summary>
		/// <param name="reason">The reason for which the event occured. You can also use the PlayerDataUpdateReasons class to pass one of the default reasons</param>
                /// <param name="location">The location where the event occured (ex.: Shop Screen, End of the level)</param>
                /// <param name="currencyList">A list of TrackingCurrency objects that defines all the currencies that have been changed with this event. This parameter can also be omited if no currencies have been updated</param>
		/// <param name="itemsList">A list of TrackingItems objects that defines all the items that have been changed with this event. This parameter can also be omited if no items have been updated</param>
		public void TrackWalletInventoryEvent(string reason, string location, List<TrackingCurrency> currencyList = null, List<TrackingItem> itemsList = null)
                {
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    if(currencyList != null)
                    {
			string currencyListJSON = JsonHelper.getJSONFromObject(currencyList);
			string wallet = "{\"currencies\":" + currencyListJSON + ", \"offset\":0}";
			dictionary.Add("wallet", wallet);
                    }

		    if(itemsList != null)
                    {
			string itemsListJSON = JsonHelper.getJSONFromObject(itemsList);
			string inventory = "{\"items\":" + itemsListJSON + ", \"offset\":0}";
			dictionary.Add("inventory", inventory);
                    }

                    dictionary.Add("reason", reason);
                    dictionary.Add("location", location);
                    dictionary.Add("trackingOnly", "true");

		    SendCustomEvent("updatePlayerData", dictionary);

                }

                [Obsolete]
                /// <summary>
                /// Sends the "iapPurchased" event to the native Spil SDK which will send a request to the back-end.
                /// See https://github.com/spilgames/spil_event_unity_plugin for more information on events.
                /// </summary>
                /// <param name="skuId">The product identifier of the item that was purchased</param>
                /// <param name="transactionId ">The transaction identifier of the item that was purchased (also called orderId)</param>
                public void SendiapPurchasedEvent(string skuId, string transactionId)
                {
			SendCustomEvent("iapPurchased", new Dictionary<string, string>() { { "skuId", skuId }, { "transactionId", transactionId }, { "purchaseDate", DateTime.Now.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz") } });
                }

		/// <summary>
                /// Sends the "iapPurchased" event to the native Spil SDK which will send a request to the back-end.
		/// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
                /// </summary>
                /// <param name="skuId">The product identifier of the item that was purchased</param>
                /// <param name="transactionId ">The transaction identifier of the item that was purchased (also called orderId)</param>
                public void TrackIAPPurchasedEvent(string skuId, string transactionId)
                {
					SendCustomEvent("iapPurchased", new Dictionary<string, string>() { { "skuId", skuId }, { "transactionId", transactionId }, { "purchaseDate", DateTime.Now.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz")  }});
                }

                [Obsolete]
                /// <summary>
                /// Sends the "iapRestored" event to the native Spil SDK which will send a request to the back-end.
                /// See https://github.com/spilgames/spil_event_unity_plugin for more information on events.
                /// </summary>
                /// <param name="skuId">The product identifier of the item that was purchased</param>
                /// <param name="originalTransactionId ">For a transaction that restores a previous transaction, the transaction identifier of the original transaction. Otherwise, identical to the transaction identifier</param>
				/// <param name="originalPurchaseDate">For a transaction that restores a previous transaction, the date of the original transaction. Please use a proper DateTime format (RFC3339), for instance: "2016-08-30T11:54:48.5247936+02:00". If you have a DateTime object you can use: DateTimeObject.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz")</param>                
				public void SendiapRestoredEvent(string skuId, string originalTransactionId, string originalPurchaseDate)
                {
                    SendCustomEvent("iapRestored", new Dictionary<string, string>() { { "skuId", skuId }, { "originalTransactionId", originalTransactionId }, { "originalPurchaseDate", originalPurchaseDate } });
                }

		/// <summary>
                /// Sends the "iapRestored" event to the native Spil SDK which will send a request to the back-end.
		/// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
                /// </summary>
                /// <param name="skuId">The product identifier of the item that was purchased</param>
                /// <param name="originalTransactionId ">For a transaction that restores a previous transaction, the transaction identifier of the original transaction. Otherwise, identical to the transaction identifier</param>
		/// <param name="originalPurchaseDate">For a transaction that restores a previous transaction, the date of the original transaction. Please use a proper DateTime format (RFC3339), for instance: "2016-08-30T11:54:48.5247936+02:00". If you have a DateTime object you can use: DateTimeObject.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz")</param>                
                public void TrackIAPRestoredEvent(string skuId, string originalTransactionId, string originalPurchaseDate)
                {
                    SendCustomEvent("iapRestored", new Dictionary<string, string>() { { "skuId", skuId }, { "originalTransactionId", originalTransactionId }, { "originalPurchaseDate", originalPurchaseDate } });
                }

                [Obsolete]
                /// <summary>
                /// Sends the "iapFailed" event to the native Spil SDK which will send a request to the back-end.
                /// See https://github.com/spilgames/spil_event_unity_plugin for more information on events.
                /// </summary>
                /// <param name="error">Error description or error code</param>
                /// <param name="skuId">The product identifier of the item that was purchased</param>
                public void SendiapFailedEvent(string error, string skuId)
                {
                    SendCustomEvent("iapFailed", new Dictionary<string, string>() { { "error", error }, { "skuId", skuId } });
                }

		/// <summary>
                /// Sends the "iapFailed" event to the native Spil SDK which will send a request to the back-end.
		/// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
                /// </summary>
                /// <param name="error">Error description or error code</param>
                /// <param name="skuId">The product identifier of the item that was purchased</param>
                public void TrackIAPFailedEvent(string error, string skuId)
                {
                    SendCustomEvent("iapFailed", new Dictionary<string, string>() { { "error", error }, { "skuId", skuId } });
                }

                [Obsolete]
                /// <summary>
                /// Sends the "tutorialComplete" event to the native Spil SDK which will send a request to the back-end.
                /// See https://github.com/spilgames/spil_event_unity_plugin for more information on events.
                /// </summary>
                public void SendtutorialCompleteEvent()
                {
                    SendCustomEvent("tutorialComplete");
                }

		/// <summary>
                /// Sends the "tutorialComplete" event to the native Spil SDK which will send a request to the back-end.
		/// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
                /// </summary>
                public void TrackTutorialCompleteEvent()
                {
                    SendCustomEvent("tutorialComplete");
                }

                [Obsolete]
                /// <summary>
                /// Sends the "tutorialSkipped" event to the native Spil SDK which will send a request to the back-end.
                /// See https://github.com/spilgames/spil_event_unity_plugin for more information on events.
                /// </summary>
                public void SendtutorialSkippedEvent()
                {
                    SendCustomEvent("tutorialSkipped");
                }

		/// <summary>
                /// Sends the "tutorialSkipped" event to the native Spil SDK which will send a request to the back-end.
		/// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
                /// </summary>
                public void TrackTutorialSkippedEvent()
                {
                    SendCustomEvent("tutorialSkipped");
                }

                [Obsolete]
                /// <summary>
                /// Sends the "tutorialSkipped" event to the native Spil SDK which will send a request to the back-end.
                /// Should be called after the user completes registration via email, Facebook, Google Plus or other available option. Registration option is assumed.
                /// See https://github.com/spilgames/spil_event_unity_plugin for more information on events.
                /// </summary>
                /// <param name="platform">A string like ‘facebook’ or ’email’</param>
                public void SendregisterEvent(string platform)
                {
                    SendCustomEvent("register", new Dictionary<string, string>() { { "platform", platform } });
                }

		/// <summary>
                /// Sends the "tutorialSkipped" event to the native Spil SDK which will send a request to the back-end.
                /// Should be called after the user completes registration via email, Facebook, Google Plus or other available option. Registration option is assumed.
		/// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
                /// </summary>
                /// <param name="platform">A string like ‘facebook’ or ’email’</param>
                public void TrackRegisterEvent(string platform)
                {
                    SendCustomEvent("register", new Dictionary<string, string>() { { "platform", platform } });
                }

                [Obsolete]
                /// <summary>
                /// Sends the "share" event to the native Spil SDK which will send a request to the back-end.
                /// Should be called every time the user shares content on their social media accounts. Social media integration is assumed.
                /// See https://github.com/spilgames/spil_event_unity_plugin for more information on events.
                /// </summary>
                /// <param name="platform">A string like ‘facebook’ or ’email’</param>
                public void SendshareEvent(string platform)
                {
                    SendCustomEvent("share", new Dictionary<string, string>() { { "platform", platform } });
                }

		/// <summary>
                /// Sends the "share" event to the native Spil SDK which will send a request to the back-end.
                /// Should be called every time the user shares content on their social media accounts. Social media integration is assumed.
		/// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
                /// </summary>
                /// <param name="platform">A string like ‘facebook’ or ’email’</param>
                public void TrackShareEvent(string platform)
                {
                    SendCustomEvent("share", new Dictionary<string, string>() { { "platform", platform } });
                }

                [Obsolete]
                /// <summary>
                /// Sends the "invite" event to the native Spil SDK which will send a request to the back-end.
                /// Should be called every time the user invites another user. Respective function in-game is assumed.
                /// See https://github.com/spilgames/spil_event_unity_plugin for more information on events.
                /// </summary>
                /// <param name="platform">A string like ‘facebook’ or ’email’</param>
                public void SendinviteEvent(string platform)
                {
                    SendCustomEvent("invite", new Dictionary<string, string>() { { "platform", platform } });
                }

		/// <summary>
                /// Sends the "invite" event to the native Spil SDK which will send a request to the back-end.
                /// Should be called every time the user invites another user. Respective function in-game is assumed.
                /// See https://github.com/spilgames/spil_event_unity_plugin for more information on events.
                /// </summary>
                /// <param name="platform">A string like ‘facebook’ or ’email’</param>
                public void TrackInviteEvent(string platform)
                {
                    SendCustomEvent("invite", new Dictionary<string, string>() { { "platform", platform } });
                }				
				
            #endregion

            [Obsolete("This method is obsolete and has been replaced by SendCustomEvent(string eventName). Please update any references, this method will be removed in the next version.")]
            public void TrackEvent(string key) { SendCustomEvent(key); }

            /// <summary>
            /// Sends an event to the native Spil SDK which will send a request to the back-end.
            /// See https://github.com/spilgames/spil_event_unity_plugin for more information on events.
            /// This method was previously called "TrackEvent"
            /// </summary>
            public void SendCustomEvent(string eventName) { SendCustomEvent(eventName, null); }

            [Obsolete("This method is obsolete and has been replaced by SendCustomEvent(string eventName, Dictionary<string, string> eventParams). Please update any references, this method will be removed in the next version.")]
            public void TrackEvent(string eventName, Dictionary<string, string> eventParams) { SendCustomEvent(eventName, eventParams); }

            /// <summary>
            /// Sends an event to the native Spil SDK which will send a request to the back-end.
            /// Track an event with params 
            /// </summary>
            /// <param name="eventName"></param>
            /// <param name="eventParams"></param>
            public abstract void SendCustomEvent(string eventName, Dictionary<string, string> eventParams);

            #region Advertisement events

                /// <summary>
                /// When Fyber has shown a reward video and the user goes back to the game to receive his/her reward Fyber can
                /// automatically show a toast message with information about the reward, for instance "You've received 50 coins". 
                /// This is disabled by default to allow the developer to create a reward notification for the user.
                /// Developers can call SetShowToastOnVideoReward(true) to enable Fyber's automatic toast message.
                /// </summary>
			    public abstract void SetShowToastOnVideoReward(bool value);

                /// <summary>
                /// This is fired by the native Spil SDK after it receives a response from the back-end.
                /// This method is exposed only for use by the native Spil SDK and should not be used by the developer!
                /// </summary>
                /// <param name="response"></param>
                public static void OnResponseReceived(string response)
                {
                    Debug.Log("SpilSDK-Unity OnResponseReceived " + response);

                    // Rewards are now received via the AdFinished event (in Android). Leave this in for now for iOS and legacy users (?)

                    SpilResponse spilResponse = JsonHelper.getObjectFromJson<SpilResponse>(response);

					if (spilResponse.type.ToLower().Trim().Equals("notificationreward"))
                    {
						PushNotificationRewardResponse rewardResponseData = JsonHelper.getObjectFromJson<PushNotificationRewardResponse>(response);
                        fireOnRewardEvent(rewardResponseData);
                    }
                }

				public delegate void RewardEvent(PushNotificationRewardResponse rewardResponse);
                /// <summary>
                /// This is fired by the Unity Spil SDK after it receives a "Reward" response from the back-end.
                /// The developer can subscribe to this event to assign the reward and update the UI.
                /// This event should no longer be used, reward data is now passed as a parameter of the AdFinished event so please use that instead.
                /// </summary>
                public event RewardEvent OnReward;
                /// <summary>
                /// This is fired by the Unity Spil SDK after it receives a "Reward" response from the back-end.
                /// </summary>
				private static void fireOnRewardEvent(PushNotificationRewardResponse rewardResponse) {  if (Spil.Instance.OnReward != null) { Spil.Instance.OnReward(rewardResponse); } }

                public delegate void AdAvailableEvent(enumAdType adType);
                /// <summary>
                /// This is fired by the native Spil SDK after it receives an "AdAvailable" response from the back-end.
                /// The developer can subscribe to this event and for instance call "Spil.Instance.PlayVideo();" or "Spil.Instance.PlayMoreApps()"
                /// </summary>
                public event AdAvailableEvent OnAdAvailable;
                /// <summary>
                /// This is called by the native Spil SDK and will fire an OnAdAvailable event to which the developer 
                /// can subscribe and for instance call "Spil.Instance.PlayVideo();" or "Spil.Instance.PlayMoreApps()"
                /// This method is exposed only for use by the native Spil SDK and should not be used by the developer!
                /// </summary>
                /// <param name="type"></param>
                public static void fireAdAvailableEvent(string type)
                {
			        Debug.Log ("SpilSDK-Unity Ad " + type + " ready!");

                    enumAdType adType = enumAdType.Unknown;
                    if(type.ToLower().Trim().Equals("rewardvideo"))
                    {
                         adType = enumAdType.RewardVideo;
                    }
                    else if(type.ToLower().Trim().Equals("interstitial"))
                    {
                        adType = enumAdType.Interstitial;
                    }
                    else if(type.ToLower().Trim().Equals("moreapps"))
                    {
                        adType = enumAdType.MoreApps;
                    }
                    if(adType == enumAdType.Unknown)
                    {
                        Debug.Log ("SpilSDK-Unity AdAvailable event fired but type is unknown. Type: " + type);
                    }
                    if (Spil.Instance.OnAdAvailable != null) { Spil.Instance.OnAdAvailable(adType); }
	            }

                public delegate void AdNotAvailableEvent(enumAdType adType);
                /// <summary>
                /// This is fired by the native Spil SDK after it receives an "AdNotAvailable" response from the back-end.
                /// The developer can subscribe to this event and for instance NOT call "Spil.Instance.PlayVideo();" or "Spil.Instance.PlayMoreApps()"
                /// but do something else instead.
                /// </summary>
                public event AdNotAvailableEvent OnAdNotAvailable;
                /// <summary>
                /// This is called by the native Spil SDK and will fire an OnAdNotAvailable event to which the developer 
                /// can subscribe and for instance NOT call "Spil.Instance.PlayVideo();" or "Spil.Instance.PlayMoreApps()" but do something else instead.
                /// This method is exposed only for use by the native Spil SDK and should not be used by the developer!
                /// </summary>
                /// <param name="type"></param>
                public static void fireAdNotAvailableEvent(string type)
                {		
		            Debug.Log ("SpilSDK-Unity Ad " + type + " is not available");

                    enumAdType adType = enumAdType.Unknown;
                    if(type.ToLower().Trim().Equals("rewardvideo"))
                    {
                         adType = enumAdType.RewardVideo;
                    }
                    else if(type.ToLower().Trim().Equals("interstitial"))
                    {
                        adType = enumAdType.Interstitial;
                    }
                    else if(type.ToLower().Trim().Equals("moreapps"))
                    {
                        adType = enumAdType.MoreApps;
                    }
                    if(adType == enumAdType.Unknown)
                    {
                        Debug.Log ("SpilSDK-Unity AdNotAvailable event fired but type is unknown. Type: " + type);
                    }
                    if (Spil.Instance.OnAdNotAvailable != null) { Spil.Instance.OnAdNotAvailable(adType); }
	            }            

                public delegate void AdStartedEvent();
                /// <summary>
                /// This is fired by the native Spil SDK after a video is started.
                /// The developer can subscribe to this event and for instance disable the in-game sound.
                /// </summary>
                public event AdStartedEvent OnAdStarted;
                /// <summary>
                /// This is called by the native Spil SDK and will fire an OnAdStarted event to which the developer 
                /// can subscribe and for instance disable the in-game sound.
                /// This method is exposed only for use by the native Spil SDK and should not be used by the developer!
                /// </summary>
                public static void fireAdStartedEvent()
                {
                    Debug.Log ("SpilSDK-Unity Ad started");
                    if (Spil.Instance.OnAdStarted != null) { Spil.Instance.OnAdStarted(); }
	            }

                public delegate void AdFinishedEvent(SpilAdFinishedResponse response);
                /// <summary>
                /// This is fired by the native Spil SDK after a video has finished playing.
                /// The developer can subscribe to this event and for instance re-enable the in-game sound.
                /// </summary>
                public event AdFinishedEvent OnAdFinished;
                /// <summary>
                /// This is called by the native Spil SDK and will fire an OnAdFinished event to which the developer 
                /// can subscribe and for instance re-enable the in-game sound.
                /// This method is exposed only for use by the native Spil SDK and should not be used by the developer!
                /// </summary>
                public static void fireAdFinishedEvent(string response)
                {
                    Debug.Log ("SpilSDK-Unity Ad finished! Response = " + response);
                    SpilAdFinishedResponse responseObject = JsonHelper.getObjectFromJson<SpilAdFinishedResponse>(response);
                    if (Spil.Instance.OnAdFinished != null) { Spil.Instance.OnAdFinished(responseObject); }
	            }

                public delegate void ConfigUpdatedEvent();
                /// <summary>
                /// This is fired by the native Spil SDK when the config was updated.
                /// The developer can subscribe to this event and for instance re-enable the in-game sound.
                /// </summary>
                public event ConfigUpdatedEvent OnConfigUpdated;
                /// <summary>
                /// This is called by the native Spil SDK and will fire an ConfigUpdated event to which the developer 
                /// can subscribe, it will only be called when the config values are different from the previous loaded config.
                /// </summary>
                public static void fireConfigUpdatedEvent()
                {
                    Debug.Log ("SpilSDK-Unity Config updated!");
                    if (Spil.Instance.OnConfigUpdated != null) { Spil.Instance.OnConfigUpdated(); }
                }

            #endregion
        
        #endregion

        // TODO: Find a more logical place to put this...

        /// <summary>
        /// Method that requests the "more apps" activity
        /// </summary>
        public abstract void RequestMoreApps();

        public abstract void PlayVideo();

        public abstract void PlayMoreApps();

		public abstract void TestRequestAd(string providerName, string adType, bool parentalGate);
		
        /// <summary>
        /// Retrieves the Spil User Id so that developers can show this in-game for users.
        /// If users contact Spil customer service they can supply this Id so that 
        /// customer support can help them properly. Please make this Id available for users
        /// in one of your game's screens.
        /// </summary>
		public abstract string GetSpilUserId();

		/// <summary>
		/// Updates the player data from the server.
		/// </summary>
		public abstract void UpdatePlayerData ();

		/// <summary>
		/// Sets the user identifier.
		/// </summary>
		/// <param name="providerId">Provider identifier.</param>
		/// <param name="userId">User identifier.</param>
		public abstract void SetUserId (string providerId, string userId);

		/// <summary>
		/// Gets the user identifier.
		/// </summary>
		/// <returns>The user identifier.</returns>
		public abstract string GetUserId ();

		/// <summary>
		/// Gets the user provider.
		/// </summary>
		/// <returns>The user provider native.</returns>
		public abstract string GetUserProvider();

		/// <summary>
		/// Sets the state of the private game.
		/// </summary>
		/// <param name="privateData">Private data.</param>
		public abstract void SetPrivateGameState(string privateData);

		/// <summary>
		/// Gets the state of the private game.
		/// </summary>
		/// <returns>The private game state.</returns>
		public abstract string GetPrivateGameState ();

		/// <summary>
		/// Sets the public game state.
		/// </summary>
		/// <param name="publicData">Public data.</param>
		public abstract void SetPublicGameState (string publicData);

		/// <summary>
		/// Gets the public game state.
		/// </summary>
		/// <returns>The public game state.</returns>
		public abstract string GetPublicGameState ();

		/// <summary>
		/// Gets the public game state of other users.
		/// </summary>
		/// <param name="provider">Provider.</param>
		/// <param name="userIdsJsonArray">User identifiers json array.</param>
		public abstract void GetOtherUsersGameState(string provider, string userIdsJsonArray);

		/// <summary>
		/// Requests the daily bonus screen.
		/// </summary>
		public abstract void RequestDailyBonus();

		/// <summary>
		/// Requests the splashscreen.
		/// </summary>
		public abstract void RequestSplashScreen ();

		#region Spil Game Objects

		public delegate void SpilGameDataAvailable();
		/// <summary>
		/// This is fired by the native Spil SDK after game data has been received from the server.
		/// The developer can subscribe to this event and then request the Spil Game Data.
		/// </summary>
		public event SpilGameDataAvailable OnSpilGameDataAvailable;

		public static void fireSpilGameDataAvailable()
		{
			Spil.GameData.SpilGameDataHandler ();

			Debug.Log ("SpilSDK-Unity Spil Game Data is available");
			
			if (Spil.Instance.OnSpilGameDataAvailable != null) { Spil.Instance.OnSpilGameDataAvailable(); }
		}
		
		public delegate void SpilGameDataError(SpilErrorMessage errorMessage);
		/// <summary>
		/// This is fired by the native Spil SDK after game data has failed to be retrieved.
		/// The developer can subscribe to this event and check the reason.
		/// </summary>
		public event SpilGameDataError OnSpilGameDataError;
		
		public static void fireSpilGameDataError(string reason)
		{
			Debug.Log ("SpilSDK-Unity Spil Game Data error with reason = " + reason);
			
			SpilErrorMessage errorMessage = JsonHelper.getObjectFromJson<SpilErrorMessage>(reason);
			if (Spil.Instance.OnSpilGameDataError != null) { Spil.Instance.OnSpilGameDataError(errorMessage); }
		}
		
		public abstract string GetSpilGameDataFromSdk();
		
//		public SpilGameDataHelper GetSpilGameData()
//		{
//			SpilGameDataHelper helper = null;
//			string spilGameDataString = GetSpilGameDataFromSdk();
//			Debug.Log ("Spil Game Data: " + spilGameDataString);
//			if(spilGameDataString != null)
//			{
//				SpilGameData spilGameData = JsonHelper.getObjectFromJson<SpilGameData>(spilGameDataString);
//				helper = new SpilGameDataHelper(spilGameData.currencies, spilGameData.items, spilGameData.bundles, spilGameData.shop, spilGameData.promotions);
//			}
//			return helper;
//		}

		#endregion
		
		#region Player Data

		public delegate void PlayerDataAvailable();
		/// <summary>
		/// This is fired by the native Spil SDK after player data has been received from the server.
		/// The developer can subscribe to this event and then request the Player Data (Wallet & Inventory).
		/// </summary>
		public event PlayerDataAvailable OnPlayerDataAvailable;
		
		public static void firePlayerDataAvailable()
		{
			Debug.Log ("SpilSDK-Unity Player Data is available");
			
			if (Spil.Instance.OnPlayerDataAvailable != null) { Spil.Instance.OnPlayerDataAvailable(); }
			
		}
		
        public delegate void PlayerDataUpdated(string reason, PlayerDataUpdatedData updatedData);
		/// <summary>
		/// This is fired by the native Spil SDK after player data has been updated.
		/// The developer can subscribe to this event and then request the Player Data (Wallet & Inventory).
		/// </summary>
		public event PlayerDataUpdated OnPlayerDataUpdated;
		
        public static void firePlayerDataUpdated(string data)
		{
            PlayerDataUpdatedData playerDataUpdatedData = JsonHelper.getObjectFromJson<PlayerDataUpdatedData>(data);

			Spil.PlayerData.PlayerDataUpdatedHandler ();

			Debug.Log ("SpilSDK-Unity Player Data has been updated");

            if (Spil.Instance.OnPlayerDataUpdated != null) { Spil.Instance.OnPlayerDataUpdated(playerDataUpdatedData.reason, playerDataUpdatedData); }
			
		}
		
		public delegate void PlayerDataError(SpilErrorMessage errorMessage);
		/// <summary>
		/// This is fired by the native Spil SDK after player data has failed to be retrieved.
		/// The developer can subscribe to this event and check the reason.
		/// </summary>
		public event PlayerDataError OnPlayerDataError;
		
		public static void firePlayerDataError(string reason)
		{
			Debug.Log ("SpilSDK-Unity Player Data error with reason = " + reason);
			
			SpilErrorMessage errorMessage = JsonHelper.getObjectFromJson<SpilErrorMessage>(reason);

			if (Spil.Instance.OnPlayerDataError != null) { Spil.Instance.OnPlayerDataError(errorMessage); } 	
		}

		public delegate void GameStateUpdated(String access);
		/// <summary>
		/// This is fired by the native Spil SDK after game state was updated.
		/// The developer can subscribe to this event and check the reason.
		/// </summary>
		public event GameStateUpdated OnGameStateUpdated;

		public static void fireGameStateUpdated(String access)
		{
			Debug.Log ("SpilSDK-Unity Game State Data updated, access = " + access);

			if (Spil.Instance.OnGameStateUpdated != null) { Spil.Instance.OnGameStateUpdated(access); } 	
		}

		public delegate void OtherUsersGameStateDataLoaded(OtherUsersGameStateData data);
		/// <summary>
		/// This is fired by the native Spil SDK after the game state data of other users was loaded.
		/// The developer can subscribe to this event and check the reason.
		/// </summary>
		public event OtherUsersGameStateDataLoaded OnOtherUsersGameStateDataLoaded;

		public static void fireOtherUsersGameStateLoaded(String message)
		{
			Debug.Log ("SpilSDK-Unity Other users game state data loaded, message = " + message);

			OtherUsersGameStateData data = JsonHelper.getObjectFromJson<OtherUsersGameStateData>(message);

			if (Spil.Instance.OnOtherUsersGameStateDataLoaded != null) { Spil.Instance.OnOtherUsersGameStateDataLoaded(data); } 	
		}

		public delegate void GameStateError(SpilErrorMessage errorMessage);
		/// <summary>
		/// This is fired by the native Spil SDK when the game state has failed to be retrieved.
		/// The developer can subscribe to this event and check the reason.
		/// </summary>
		public event GameStateError OnGameStateError;

		public static void fireGameStateError(string reason)
		{
			Debug.Log ("SpilSDK-Unity Game State Data error with reason = " + reason);

			SpilErrorMessage errorMessage = JsonHelper.getObjectFromJson<SpilErrorMessage>(reason);

			if (Spil.Instance.OnGameStateError != null) { Spil.Instance.OnGameStateError(errorMessage); } 	
		}

		public delegate void OpenGameShop();
		/// <summary>
		/// This is fired by the native Spil SDK when the game shop should be opened.
		/// The developer can subscribe to this event and open there own shop implementation.
		/// </summary>
		public event OpenGameShop OnOpenGameShop;

		public static void fireOpenGameShop()
		{
			Debug.Log ("SpilSDK-Unity Open Game Shop");

			if (Spil.Instance.OnOpenGameShop != null) { Spil.Instance.OnOpenGameShop(); } 	
		}

		public delegate void ReceiveWebReward();
		/// <summary>
		/// This is fired by the native Spil SDK when the reward is received from the web view.
		/// The developer can subscribe to this event and provide the reward to the user.
		/// </summary>
		public event ReceiveWebReward OnReceiveWebReward;

		public static void fireReceiveReward(String reward)
		{
			Debug.Log ("SpilSDK-Unity Received reward = " + reward);

			if (Spil.Instance.OnReceiveWebReward != null) { Spil.Instance.OnReceiveWebReward(); } 	
		}

		public delegate void WebError(SpilErrorMessage errorMessage);
		/// <summary>
		/// This is fired by the native Spil SDK when the web view encounters an error.
		/// The developer can subscribe to this event and inspect the error.
		/// </summary>
		public event WebError OnWebError;

		public static void fireWebError(string reason)
		{
			Debug.Log ("SpilSDK-Unity Web Error with reason = " + reason);

			SpilErrorMessage errorMessage = JsonHelper.getObjectFromJson<SpilErrorMessage>(reason);

			if (Spil.Instance.OnWebError != null) { Spil.Instance.OnWebError(errorMessage); } 	
		}

		public static void fireWebOpen() {
			Debug.Log ("SpilSDK-Unity Web open");

			if (Spil.Instance.OnWebOpen != null) { Spil.Instance.OnWebOpen(); } 
		}

		public delegate void WebOpen();
		/// <summary>
		/// This is fired by the native Spil SDK when the web view opened.
		/// </summary>
		public event WebOpen OnWebOpen;

		public static void fireWebClosed() {
			Debug.Log ("SpilSDK-Unity Web closed");

			if (Spil.Instance.OnWebClosed != null) { Spil.Instance.OnWebClosed(); } 
		}

		public delegate void WebClosed();
		/// <summary>
		/// This is fired by the native Spil SDK when the web view closes.
		/// </summary>
		public event WebClosed OnWebClosed;

		public abstract string GetWalletFromSdk();
		
		public abstract string GetInvetoryFromSdk();
		
//		public PlayerDataHelper GetPlayerData()
//		{
//			PlayerDataHelper helper = null;
//			string walletString = GetWalletFromSdk();
//			string inventoryString = GetInvetoryFromSdk();
//			if(walletString != null && inventoryString != null)
//			{
//				WalletData walletData = JsonHelper.getObjectFromJson<WalletData>(walletString);
//				InventoryData inventoryData = JsonHelper.getObjectFromJson<InventoryData>(inventoryString);
//				
//				helper = new PlayerDataHelper(walletData, inventoryData);
//				
//			}
//			return helper;
//		}
		
		public abstract void AddCurrencyToWallet(int currencyId, int amount, string reason);
		
		public abstract void SubtractCurrencyFromWallet(int currencyId, int amount, string reason);
		
		public abstract void AddItemToInventory(int itemId, int amount, string reason);
		
		public abstract void SubtractItemFromInventory(int itemId, int amount, string reason);
		
		public abstract void ConsumeBundle(int bundleId, string reason);
		
		#endregion

        #region Customer support

        public abstract void ShowHelpCenter();
        public abstract void ShowContactCenter();
        public abstract void ShowHelpCenterWebview();

        #endregion
    }
}