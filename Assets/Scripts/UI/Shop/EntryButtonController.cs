using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SpilGames.Unity;
using SpilGames.Unity.Helpers.GameData;

public class EntryButtonController : MonoBehaviour {
	Entry entry;
	
	Bundle bundle;

	public Text buttonLabel;

	public GameObject saleImage;

	TabController parentTabController;

	public void SetupButton(Entry entryValue, TabController parent) {
		entry = entryValue;
		parentTabController = parent;
		bundle = Spil.GameData.GetBundle (entry.BundleId);
		buttonLabel.text = entry.Label;

		Promotion promotion = Spil.GameData.GetPromotion(entry.BundleId);
		if (promotion != null) {
			saleImage.SetActive(true);
		}
	}

	public void EntryButtonPressed(){
		parentTabController.bundleDisplayPanel.SetupBundleDisplayPanel (bundle, entry);
	}



}
