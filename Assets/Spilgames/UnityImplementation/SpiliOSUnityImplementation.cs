using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;
using SpilGames.Unity.Utils;
using System.Collections;

namespace SpilGames.Unity.Implementations
{
    #if UNITY_IPHONE
    public class SpiliOSUnityImplementation : SpilUnityImplementationBase
    {
        #region Inherited members

            #region Game config
	
                /// <summary>
                /// Returns the game config as a json string.
                /// This is not essential for developers so could be made private (getConfig<T>() uses it so it cannot be removed entirely) but might be handy for some developers so we left it in.
                /// </summary>
                /// <returns></returns>
    	        public override string GetConfigAll()
                {
		            return getConfigNative();
	            }
	
                /// <summary>
                /// Method that returns a configuration value from the game config based on key 
                /// </summary>
                /// <param name="key"></param>
                /// <returns></returns>
	            public override string GetConfigValue(string key)
                {
		            return getConfigValueNative(key);
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
                    requestPackagesNative();
                }

                // Method that returns the all packages
                protected override string GetAllPackages()
                {
                    return getAllPackagesNative();
                }

                // Method that returns a package based on key
                protected override string GetPackage(string key)
                {
                    return getPackageNative(key);
                }

			    /// <summary>
			    /// This method is marked as internal and should not be exposed to developers.
			    /// The Spil Unity SDK is not packaged as a seperate assembly yet so this method is currently visible, this will be fixed in the future.
			    /// Internal method names start with a lower case so you can easily recognise and avoid them.
			    /// </summary>
                internal override string getPromotion(string key)
                {
                    return getPromotionNative(key);
                }

                [DllImport("__Internal")]
	            private static extern void requestPackagesNative();

                [DllImport("__Internal")]
	            private static extern string getAllPackagesNative();

                [DllImport("__Internal")]
	            private static extern string getPackageNative(string keyName);

                [DllImport("__Internal")]
	            private static extern string getPromotionNative(string keyName);

            #endregion

            /// <summary>
            /// This method is marked as internal and should not be exposed to developers.
            /// The Spil Unity SDK is not packaged as a seperate assembly yet so this method is currently visible, this will be fixed in the future.
            /// Internal method names start with a lower case so you can easily recognise and avoid them.
            /// </summary>
            internal override void SpilInit()
            {
		        JSONObject options = new JSONObject();
		        options.AddField ("isUnity",true);
		        initEventTrackerWithOptions(options.ToString());
		        applicationDidBecomeActive();
		        RegisterForIosPushNotifications();
		        CheckForRemoteNotifications();
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
	        public override void SendCustomEvent(string eventName, Dictionary<string,string> dict)
            {
		        if (dict != null)
                {
			        //creat a json object using the JSONobject library
			        JSONObject jsonString = new JSONObject(JSONObject.Type.STRING);
			        foreach(var item in dict)
                    {
				        jsonString.AddField(item.Key,item.Value);
			        }
			        trackEventWithParamsNative (eventName, jsonString.ToString());
		        } else {
			        trackEventNative(eventName);
		        }
	        }

            /// <summary>
            /// This can be called to show a video, for instance after calling "SendrequestRewardVideoEvent()"
            /// and receiving an "AdAvailable" event the developer could call this method from the event handler.
            /// When calling this method "SendrequestRewardVideoEvent()" must first have been called to request and cache a video.
            /// If no video is available then nothing will happen.
            /// </summary>
	        public override void PlayVideo()
            {
                playRewardVideoNative();
	        }

            [DllImport("__Internal")]
	        private static extern void playRewardVideoNative();




			public override void ShowToastOnVideoReward(bool show)
			{
				showToastOnVideoReward(show);
			}
			
			[DllImport("__Internal")]
			private static extern void showToastOnVideoReward(bool show);

			

            /// <summary>
            /// This can be called to show the "more apps" activity, for instance after calling "RequestMoreApps()"
            /// and receiving an "AdAvailable" event the developer could call this method from the event handler.
            /// When calling this method "RequestMoreApps()" must first have been called to request and cache a video.
            /// If no video is available then nothing will happen.
            /// </summary>
	        public override void PlayMoreApps()
            {
                showMoreAppsNative();
	        }

            [DllImport("__Internal")]
	        private static extern void showMoreAppsNative();

            /// <summary>
            /// Sends the "requestAd" event with the "moreApps" parameter to the native Spil SDK which will send a request to the back-end.
            /// When a response has been received from the back-end the SDK will fire either an "AdAvailable" or and "AdNotAvailable"
            /// event to which the developer can subscribe and for instance call PlayVideo(); or PlayMoreApps();
            /// </summary>
            public override void RequestMoreApps()
            {
                devRequestAdNative("Chartboost", "moreApps", false);
            }

            [DllImport("__Internal")]
	        private static extern void devRequestAdNative(string providerName, string adTypeName, bool parentalGate);

            /// <summary>
            /// Retrieves the Spil User Id so that developers can show this in-game for users.
            /// If users contact Spil customer service they can supply this Id so that 
            /// customer support can help them properly. Please make this Id available for users
            /// in one of your game's screens.
            /// </summary>
	        public override string GetSpilUID()
            {
                return getSpilUIDNative();
            }

            [DllImport("__Internal")]
	        private static extern string getSpilUIDNative();

			#region Spil Game Objects

				public override string GetSpilGameDataFromSdk ()
				{
					return getSpilGameDataNative();
				}

				[DllImport("__Internal")]
				private static extern string getSpilGameDataNative();
			
			#endregion

			#region Player Data

				public override string GetWalletFromSdk()
				{
					return getWalletNative();
				}

				[DllImport("__Internal")]
				private static extern string getWalletNative();

				public override string GetInvetoryFromSdk()
				{
					return getInventoryNative();
				}

				[DllImport("__Internal")]
				private static extern string getInventoryNative();

				public override void AddCurrencyToWallet (int currencyId, int amount, string reason)
				{
					addCurrencyToWallet(currencyId, amount, reason);
				}

				[DllImport("__Internal")]
				private static extern void addCurrencyToWallet(int currencyId, int amount, string reason);

				public override void SubtractCurrencyFromWallet (int currencyId, int amount, string reason)
				{
					subtractCurrencyFromWallet(currencyId, amount, reason);
				}

				[DllImport("__Internal")]
				private static extern void subtractCurrencyFromWallet(int currencyId, int amount, string reason);

				public override void AddItemToInventory (int itemId, int amount, string reason)
				{
					addItemToInventory(itemId, amount, reason);
				}

				[DllImport("__Internal")]
				private static extern void addItemToInventory (int itemId, int amount, string reason);

				public override void SubtractItemFromInventory (int itemId, int amount, string reason)
				{
					subtractItemFromInventory(itemId, amount, reason);
				}

				[DllImport("__Internal")]
				private static extern void subtractItemFromInventory (int itemId, int amount, string reason);

				public override void ConsumeBundle (int bundleId, string reason)
				{
					consumeBundle(bundleId, reason);
				}

				[DllImport("__Internal")]
				private static extern void consumeBundle (int bundleId, string reason);

			#endregion

        #endregion

        #region Non inherited members (iOS only members)

            #region Game config

    	        [DllImport("__Internal")]
	            private static extern string getConfigNative();

	            [DllImport("__Internal")]
	            private static extern string getConfigValueNative(string keyName);

            #endregion

            #region Push notifications

	            [DllImport("__Internal")]
	            private static extern void setPushNotificationKey(string key);

	            [DllImport("__Internal")]
	            private static extern void handlePushNotification(string notificationStringParams);

	            //register for ios push notifications
	            private void RegisterForIosPushNotifications()
                {
		            Debug.Log ("UNITY: REGISTERING FOR PUSH NOTIFICATIONS");
                    #if UNITY_IPHONE
                    #if UNITY_5
		            UnityEngine.iOS.NotificationServices.RegisterForNotifications(
                        UnityEngine.iOS.NotificationType.Alert |
                        UnityEngine.iOS.NotificationType.Badge |
                        UnityEngine.iOS.NotificationType.Sound,
                        true
                    );
                    #else
		            NotificationServices.RegisterForRemoteNotificationTypes (
                        RemoteNotificationType.Alert|
		                RemoteNotificationType.Badge|
		                RemoteNotificationType.Sound
                    );
                    #endif
                    #endif
	            }

	            private void CheckForRemoteNotifications()
                {
                    #if UNITY_IPHONE
                    #if UNITY_5
                    if (UnityEngine.iOS.NotificationServices.remoteNotificationCount > 0)
                    {			
			            foreach(UnityEngine.iOS.RemoteNotification notification in 	UnityEngine.iOS.NotificationServices.remoteNotifications)
                        {
                    #else
                    if (UnityEngine.NotificationServices.remoteNotificationCount > 0)
                    {			
			            foreach(UnityEngine.RemoteNotification notification in 	UnityEngine.NotificationServices.remoteNotifications)
                        {
                    #endif
				            foreach(var key in notification.userInfo.Keys)
                            {
					            if(notification.userInfo[key].GetType() == typeof(Hashtable))
                                {
						            Hashtable userInfo = (Hashtable) notification.userInfo[key];
						            JSONObject notificationPayload = new JSONObject();
						            foreach(var pKey in userInfo.Keys)
                                    {
							            if(userInfo[pKey].GetType() == typeof(string))
                                        {
								            string keyStr = pKey.ToString();
								            string value = userInfo[pKey].ToString();
								            notificationPayload.AddField(keyStr,value);
							            }
							            if(userInfo[pKey].GetType() == typeof(Hashtable))
                                        {
								            JSONObject innerJson = new JSONObject();
								            Hashtable innerTable = (Hashtable)userInfo[pKey];
								            foreach(var iKey in innerTable.Keys)
                                            {
									            string iKeyStr = iKey.ToString();
									            if(innerTable[iKey].GetType() == typeof(Hashtable))
                                                {
										            Hashtable innerTableB = (Hashtable)innerTable[iKey];
										            JSONObject innerJsonB = new JSONObject();
										            foreach(var bKey in innerTableB.Keys)
                                                    {
											            innerJsonB.AddField(bKey.ToString(),innerTableB[bKey].ToString());
										            }
										            innerJson.AddField(iKeyStr,innerJsonB);
									            }
									            if(innerTable[iKey].GetType() == typeof(string))
                                                {
										            string iValue = innerTable[iKey].ToString();
										            innerJson.AddField(iKeyStr,iValue);
									            }
								            }
								            string keyStr = pKey.ToString();
								            notificationPayload.AddField(keyStr,innerJson);
							            }
						            }
						            handlePushNotification(notificationPayload.ToString());
					            }
				            }
			            }
                        #if UNITY_5
                            UnityEngine.iOS.NotificationServices.ClearRemoteNotifications();
                        #else
                            UnityEngine.NotificationServices.ClearRemoteNotifications();
                        #endif
                    } else {
			            Debug.Log("NO REMOTE NOTIFICATIONS FOUND");
		            }
                    #endif
	            }

                //is the IOS notification service token sent
	            private bool tokenSent;
        	
                /// This method is marked as internal and should not be exposed to developers.
                /// The Spil Unity SDK is not packaged as a seperate assembly yet so this method is currently visible, this will be fixed in the future.
                /// Internal method names start with a lower case so you can easily recognise and avoid them.
	            internal void SendNotificationTokenToSpil()
                {
		            if (!tokenSent)
                    {
                        #if UNITY_IPHONE
                        #if UNITY_5
                            byte[] token = UnityEngine.iOS.NotificationServices.deviceToken;
                        #else
                            byte[] token = UnityEngine.NotificationServices.deviceToken;
                        #endif
			            if (token != null)
                        {
				            // send token to a provider
				            string tokenToBeSent = System.BitConverter.ToString (token).Replace ("-", "");
				            Dictionary<string,string> param = new Dictionary<string, string> ();
				            param.Add ("regId", tokenToBeSent);
				            setPushNotificationKey (tokenToBeSent);
				            tokenSent = true;
			            }
                        #endif
		            }
	            }

            #endregion

            #region App lifecycle handlers

	            [DllImport("__Internal")]
	            private static extern void applicationDidEnterBackground();

	            [DllImport("__Internal")]
	            private static extern void applicationDidBecomeActive();
	
    	        private void OnApplicationPause(bool pauseStatus)
                {
		            if(!pauseStatus)
                    {
			            applicationDidBecomeActive();
			            CheckForRemoteNotifications();
		            } else {
			            applicationDidEnterBackground();
		            }
	            }

            #endregion

	        [DllImport("__Internal")]
	        private static extern void initEventTrackerWithOptions(string options);	

	        [DllImport("__Internal")]
	        private static extern void trackEventNative(string eventName);

	        [DllImport("__Internal")]
	        private static extern void trackEventWithParamsNative(string eventName, string jsonStringParams);

        #endregion
    }        
    #endif
}