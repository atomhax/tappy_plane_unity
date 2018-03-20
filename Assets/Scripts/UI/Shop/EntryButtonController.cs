using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SpilGames.Unity;
using SpilGames.Unity.Helpers.GameData;
using SpilGames.Unity.Helpers.Promotions;

public class EntryButtonController : MonoBehaviour {
	Entry entry;
	
	Bundle bundle;

	public Text buttonLabel;

	public GameObject saleImage;

	TabController parentTabController;

	public void SetupButton(Entry entryValue, TabController parent) {
		entry = entryValue;
		parentTabController = parent;
		bundle = Spil.GameData.GetBundle (entry.Id);
		buttonLabel.text = entry.Label;

		if (Spil.Instance.GetPromotions().HasActiveEntryPromotion(entryValue)) {
			saleImage.SetActive(true);
		}
	}

	public void EntryButtonPressed(){
		parentTabController.bundleDisplayPanel.SetupBundleDisplayPanel (bundle, entry);
	}



}
