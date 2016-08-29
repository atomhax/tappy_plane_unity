using UnityEngine;
using System.Collections;

public class PublicGameState {


	public int HighScore;

	public void setHighScore(int highScore){
		this.HighScore = highScore;
	}

	public int getHighScore(){
		return HighScore;
	}

}
