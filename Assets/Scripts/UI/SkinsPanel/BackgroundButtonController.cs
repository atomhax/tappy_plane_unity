using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SpilGames.Unity;
using SpilGames.Unity.Helpers;

public class BackgroundButtonController : MonoBehaviour {

	public SkinSelectPanelController skinSelectPanelController;

	public Image backgroundImage, lockImage, playerImage;

	public int position;

	public int itemID;

	public bool defaultSkin = false;

	public bool owned = false;

	public Color selectedColor, unselectedColor;

	void OnEnable(){
		SetupButton ();
	}

	public void SetupButton(){
		if (position == 3) {
			if (skinSelectPanelController.gameController.backgroundRuin == null) {
				gameObject.SetActive(false);
				return;
			}

			playerImage.sprite = skinSelectPanelController.gameController.backgroundRuin;
		}
		
		if (position == 4) {
			if (skinSelectPanelController.gameController.backgroundTown == null) {
				gameObject.SetActive(false);
				return;
			}

			playerImage.sprite = skinSelectPanelController.gameController.backgroundTown;
		}
		
		if (PlayerPrefs.GetInt ("Background", 0) == position) {
			backgroundImage.color = selectedColor;
		} else {
			backgroundImage.color = unselectedColor;
		}

		if(defaultSkin){
			lockImage.enabled = false;
			return;
		}
		if(Spil.PlayerData.InventoryHasItem(itemID)){
			lockImage.enabled = false;
			owned = true;
		}
			
	}

	public void ButtonClicked(){
		if (owned) {
			PlayerPrefs.SetInt ("Background", position);
			skinSelectPanelController.UpdateButtons ();
		}
	}

}
