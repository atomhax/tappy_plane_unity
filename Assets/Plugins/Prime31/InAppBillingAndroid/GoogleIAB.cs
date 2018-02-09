using UnityEngine;
using System.Collections;



#if UNITY_ANDROID

namespace Prime31
{
	public class GoogleIAB
	{
		private static AndroidJavaObject _plugin;
	
		static GoogleIAB()
		{
			if( Application.platform != RuntimePlatform.Android )
				return;
	
			// find the plugin instance
			using( var pluginClass = new AndroidJavaClass( "com.prime31.GoogleIABPlugin" ) )
				_plugin = pluginClass.CallStatic<AndroidJavaObject>( "instance" );
		}
	
	
		/// <summary>
		/// Toggles high detail logging on/off
		/// </summary>
		public static void enableLogging( bool shouldEnable )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;
	
			if( shouldEnable )
				Debug.LogWarning( "YOU HAVE ENABLED HIGH DETAIL LOGS. DO NOT DISTRIBUTE THE GENERATED APK PUBLICLY. IT WILL DUMP SENSITIVE INFORMATION TO THE CONSOLE!" );
			_plugin.Call( "enableLogging", shouldEnable );
		}
	
	
		/// <summary>
		/// Toggles automatic signature verification on/off
		/// </summary>
		public static void setAutoVerifySignatures( bool shouldVerify )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;
			
			_plugin.Call( "setAutoVerifySignatures", shouldVerify );
		}
	
	
		/// <summary>
		/// Initializes the billing system. allowTestProducts will default to the value of Debug.isDebugBuild.
		/// </summary>
		public static void init( string publicKey, bool? allowTestProducts = null )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;
			
			// if we didnt have anything passed in default to true for debug builds
			if( !allowTestProducts.HasValue )
				allowTestProducts = Debug.isDebugBuild;
	
			_plugin.Call( "init", publicKey, allowTestProducts.Value );
		}
	
	
		/// <summary>
		/// Unbinds and shuts down the billing service
		/// </summary>
		public static void unbindService()
		{
			if( Application.platform != RuntimePlatform.Android )
				return;
	
			_plugin.Call( "unbindService" );
		}
	
	
		/// <summary>
		/// Returns whether subscriptions are supported on the current device
		/// </summary>
		public static bool areSubscriptionsSupported()
		{
			if( Application.platform != RuntimePlatform.Android )
				return false;
	
			return _plugin.Call<bool>( "areSubscriptionsSupported" );
		}
	
	
		/// <summary>
		/// Sends a request to get all completed purchases and product information as setup in the Play dashboard about the provided skus
		/// </summary>
		public static void queryInventory( string[] skus )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;
	
			_plugin.Call( "queryInventory", new object[] { skus } );
	
			//var method = AndroidJNI.GetMethodID( _plugin.GetRawClass(), "queryInventory", "([Ljava/lang/String;)V" );
			//AndroidJNI.CallVoidMethod( _plugin.GetRawObject(), method, AndroidJNIHelper.CreateJNIArgArray( new object[] { skus } ) );
		}
	
	
		/// <summary>
		/// Sends our a request to purchase the product
		/// </summary>
		public static void purchaseProduct( string sku )
		{
			purchaseProduct( sku, string.Empty );
		}
	
		public static void purchaseProduct( string sku, string developerPayload )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;
	
			_plugin.Call( "purchaseProduct", sku, developerPayload );
		}
	
	
		/// <summary>
		/// Sends out a request to consume the product
		/// </summary>
		public static void consumeProduct( string sku )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;
	
			_plugin.Call( "consumeProduct", sku );
		}
	
	
		/// <summary>
		/// Sends out a request to consume all of the provided products
		/// </summary>
		public static void consumeProducts( string[] skus )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;
	
			_plugin.Call( "consumeProducts", new object[] { skus } );
		}
	
	}

}
#endif
