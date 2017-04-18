using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SpilGames.Unity.Base.SDK
{
	// Example reward JSON string (contains RewardResponse object): "{ \"name\":\"reward\",\"action\":\"receive\",\"type\":\"notificationreward\",\"data\":{ \"eventData\":{ \"currencyName\":\"Coins\",\"currencyId\":\"coins\",\"reward\":50} } }";

	public class RewardResponse
	{
		public string token;
		public List<RewardObject> reward;
		public string rewardType;
		public SpilErrorMessage error;
	}

	public class RewardObject
	{
		public int id;
		public string externalId;
		public string type;
		public int amount;
	}

	public class AdvertisementRewardResponse : SpilResponse
	{
		public AdvertisementRewardEventData data;
	}

	public class AdvertisementRewardEventData
	{
		public AdvertisementRewardData eventData;
	}

	public class AdvertisementRewardData
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

