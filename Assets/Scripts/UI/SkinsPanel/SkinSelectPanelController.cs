using UnityEngine;
using System.Collections;

public class SkinSelectPanelController : MonoBehaviour {

	public BackgroundButtonController[] backgroundButtons;

	public PlaneSkinButtonController[] skinButtons;

	public void UpdateButtons(){
		for (int i = 0; i < backgroundButtons.Length; i++) {
			backgroundButtons [i].SetupButton ();
		}
		for (int i = 0; i < skinButtons.Length; i++) {
			skinButtons [i].SetupButton ();
		}
	}



}
