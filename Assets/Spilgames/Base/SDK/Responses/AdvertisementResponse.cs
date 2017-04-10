using UnityEngine;
using System.Collections;

namespace SpilGames.Unity.Base.SDK
{

	public enum enumAdType
	{
		RewardVideo = 0,
		Interstitial = 1,
		MoreApps = 2,
		Unknown = 3
	}

	public class SpilAdFinishedResponse
	{
		/// <summary>
		/// Possible values: interstitial / rewardVideo / moreApps
		/// </summary>
		public string type;

		public enumAdType GetTypeAsEnum ()
		{
			enumAdType adType = enumAdType.Unknown;
			if (type.ToLower ().Trim ().Equals ("rewardvideo")) {
				adType = enumAdType.RewardVideo;
			} else if (type.ToLower ().Trim ().Equals ("interstitial")) {
				adType = enumAdType.Interstitial;
			} else if (type.ToLower ().Trim ().Equals ("moreapps")) {
				adType = enumAdType.MoreApps;
			}
			if (adType == enumAdType.Unknown) {
				Debug.Log ("SpilSDK-Unity AdNotAvailable event fired but type is unknown. Type: " + type);
			}
			return adType;
		}

		/// <summary>
		/// Possible values: close / dismiss
		/// </summary>
		public string reason;

		/// <summary>
		/// reward / empty
		/// </summary>
		public RewardData reward;

		/// <summary>
		/// DFP / Fyber / Chartboost
		/// </summary>
		public string network;
	}

}

