using UnityEngine;
using System.Collections;

namespace SpilGames.Unity.Base.SDK
{
	// Example reward JSON string (contains RewardResponse object): "{ \"name\":\"reward\",\"action\":\"receive\",\"type\":\"notificationreward\",\"data\":{ \"eventData\":{ \"currencyName\":\"Coins\",\"currencyId\":\"coins\",\"reward\":50} } }";


	public class RewardResponse : SpilResponse
	{
		public RewardEventData data;
	}

	public class RewardEventData
	{
		public RewardData eventData;
	}

	public class RewardData
	{
		public string currencyName;
		public string currencyId;
		public int reward;
	}

	// Pushnotifications reward classes

	public class PushNotificationRewardResponse : SpilResponse
	{
		public PushRewardEventData data;
	}

	public class PushRewardEventData
	{
		public NotificationRewardData eventData;
	}

	public class NotificationRewardData
	{
		public string currencyName;
		public int currencyId;
		public int reward;
	}

}

