using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CameraScript : MonoBehaviour {
	static public CameraScript instance;
	//time of level until it ends
	float 			levelTime = 30;
	//text for number of kills and time
	Text			timeText;
	Text			zeroScoreText;
	Text			oneScoreText;
	Text			twoScoreText;
	Text			threeScoreText;
	//number of kills for each player
	int				playerZeroScore = 0;
	int				playerOneScore = 0;
	int				playerTwoScore = 0;
	int				playerThreeScore = 0;
	//this will be set to true only once
	bool			timeUp = false;
	//the person who is the champion at the end of the round
	//public because it can be changed by a kill at the end
	public int 		champion;
	
	void Awake(){
		instance = this;
		GameObject timeGO = GameObject.Find("TimeLeftText"); 
		if (!timeGO) return;
		timeText = timeGO.GetComponent<Text>();
		timeText.text = "Time left: " + (levelTime - Time.timeSinceLevelLoad).ToString("F2");
		instantiateKillText();
	}
	
	void FixedUpdate(){
		if (Time.timeSinceLevelLoad > levelTime){
			afterTimeFixedUpdate();
			return;
		}
		float timeLeft = (levelTime - Time.timeSinceLevelLoad);
		timeText.text = "Time left: " + timeLeft.ToString("F2");
	}
	
	//instantiates the number of kills text for the game screen
	void instantiateKillText(){
		GameObject zeroGO = GameObject.Find("ZeroScoreText"); 
		if (!zeroGO) return;
		zeroScoreText = zeroGO.GetComponent<Text>();
		zeroScoreText.text = "Player 0: " + playerZeroScore.ToString();
		GameObject oneGO = GameObject.Find("OneScoreText"); 
		if (!oneGO) return;
		oneScoreText = oneGO.GetComponent<Text>();
		oneScoreText.text = "Player 1: " + playerOneScore.ToString();
		GameObject twoGO = GameObject.Find("TwoScoreText"); 
		if (!twoGO) return;
		twoScoreText = twoGO.GetComponent<Text>();
		twoScoreText.text = "Player 2: " + playerTwoScore.ToString();
		GameObject threeGo = GameObject.Find("ThreeScoreText"); 
		if (!threeGo) return;
		threeScoreText = threeGo.GetComponent<Text>();
		threeScoreText.text = "Player 3: " + playerThreeScore.ToString();
	}
	
	//adds int score to the score of the player (playerNum)
	public void addScore(int playerNum, int score){
		if (playerNum == 0){
			playerZeroScore += score;
			zeroScoreText.text = "Player 0: " + playerZeroScore.ToString();
		}
		else if (playerNum == 1){
			playerOneScore += score;
			oneScoreText.text = "Player 1: " + playerOneScore.ToString();
		}
		else if (playerNum == 2){
			playerTwoScore += score;
			twoScoreText.text = "Player 2: " + playerTwoScore.ToString();
		}
		else if (playerNum == 3){
			playerThreeScore += score;
			threeScoreText.text = "Player 3: " + playerThreeScore.ToString();
		}
	}
	
	//the fixed update that is called when someone is superhuman
	void afterTimeFixedUpdate(){
		if (!timeUp){
			LevelMovementScript.stopMoving = true;
			timeUp = true;
			pickChampion();
		}
	}
	
	//picks the champion, if it is a tie it randomly picks a champion
	void pickChampion(){
		bool tied = false;
		List<int> winners = new List<int>();
		int championNum = 0;
		int maxScore = playerZeroScore;
		winners.Add(0);
		if (playerOneScore > maxScore){
			championNum = 1;
			maxScore = playerOneScore;
			winners.Clear();
			winners.Add(1);
			tied = false;
		}
		else if (playerOneScore == maxScore){
			winners.Add(1);
			tied = true;
		}
		if (playerTwoScore > maxScore){
			championNum = 2;
			maxScore = playerTwoScore;
			winners.Clear();
			winners.Add(2);
			tied = false;
		}
		else if (playerTwoScore == maxScore){
			winners.Add(2);
			tied = true;
		}
		if (playerThreeScore > maxScore){
			tied = false;
			championNum = 3;
			maxScore = playerThreeScore;
		}
		else if (playerThreeScore == maxScore){
			winners.Add(3);
			tied = true;
		}
		if (tied){
			int index = (int) Random.Range(0, winners.Count);
			championNum = winners[index];
			winners.Clear();
		}
		champion = championNum;
		timeText.text = "Champion is Player" + champion.ToString() + "!!!";
	}
}
