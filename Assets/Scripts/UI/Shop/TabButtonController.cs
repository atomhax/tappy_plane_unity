using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SpilGames.Unity;
public class TabButtonController : MonoBehaviour {

	public Text buttonLabelText;

	public GameObject saleImage;

	public int tabNumber = 0;

	ShopPanelController shopUIController;

	public void TabButtonClicked(){
		shopUIController.tabsPanel.gameObject.SetActive (true);
		shopUIController.shopTabs [tabNumber].SetActive (true);
	}

	public void SetupButton(string tabName, int tabNumber, bool hasActivePromotion, ShopPanelController parentController){
		buttonLabelText.text = tabName;
		this.tabNumber = tabNumber;
		shopUIController = parentController;

		if (hasActivePromotion) {
			saleImage.SetActive(true);
		}
	}

}
