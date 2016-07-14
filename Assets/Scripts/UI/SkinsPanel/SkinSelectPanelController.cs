using UnityEngine;
using System.Collections;

public class SkinSelectPanelController : MonoBehaviour {

	public BackgroundButtonController[] backgroundButtons;

	public PlaneSkinButtonController[] skinButtons;

	public GameController gameController;

	public void UpdateButtons(){
		for (int i = 0; i < backgroundButtons.Length; i++) {
			backgroundButtons [i].SetupButton ();
		}
		for (int i = 0; i < skinButtons.Length; i++) {
			skinButtons [i].SetupButton ();
		}
	}

	public void CloseSkinsPanel(){
		gameController.UpdateSkins ();
		gameObject.SetActive (false);
	}

}
