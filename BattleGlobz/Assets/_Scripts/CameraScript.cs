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
	Text			zeroKillsText;
	Text			oneKillsText;
	Text			twoKillsText;
	Text			threeKillsText;
	//number of kills for each player
	int				playerZeroKills = 0;
	int				playerOneKills = 0;
	int				playerTwoKills = 0;
	int				playerThreeKills = 0;
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
		GameObject zeroGO = GameObject.Find("ZeroKillsText"); 
		if (!zeroGO) return;
		zeroKillsText = zeroGO.GetComponent<Text>();
		zeroKillsText.text = "Player 0: " + playerZeroKills.ToString();
		GameObject oneGO = GameObject.Find("OneKillsText"); 
		if (!oneGO) return;
		oneKillsText = oneGO.GetComponent<Text>();
		oneKillsText.text = "Player 1: " + playerOneKills.ToString();
		GameObject twoGO = GameObject.Find("TwoKillsText"); 
		if (!twoGO) return;
		twoKillsText = twoGO.GetComponent<Text>();
		twoKillsText.text = "Player 2: " + playerTwoKills.ToString();
		GameObject threeGo = GameObject.Find("ThreeKillsText"); 
		if (!threeGo) return;
		threeKillsText = threeGo.GetComponent<Text>();
		threeKillsText.text = "Player 3: " + playerThreeKills.ToString();
	}
	
	//adds a kill to the player's kill num
	public void addKill(int playerNum){
		if (playerNum == 0){
			playerZeroKills++;
			zeroKillsText.text = "Player 0: " + playerZeroKills.ToString();
		}
		else if (playerNum == 1){
			playerOneKills++;
			oneKillsText.text = "Player 1: " + playerOneKills.ToString();
		}
		else if (playerNum == 2){
			playerTwoKills++;
			twoKillsText.text = "Player 2: " + playerTwoKills.ToString();
		}
		else if (playerNum == 3){
			playerThreeKills++;
			threeKillsText.text = "Player 3: " + playerThreeKills.ToString();
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
		int maxKills = playerZeroKills;
		winners.Add(0);
		if (playerOneKills > maxKills){
			championNum = 1;
			maxKills = playerOneKills;
			winners.Clear();
			winners.Add(1);
			tied = false;
		}
		else if (playerOneKills == maxKills){
			winners.Add(1);
			tied = true;
		}
		if (playerTwoKills > maxKills){
			championNum = 2;
			maxKills = playerTwoKills;
			winners.Clear();
			winners.Add(2);
			tied = false;
		}
		else if (playerTwoKills == maxKills){
			winners.Add(2);
			tied = true;
		}
		if (playerThreeKills > maxKills){
			tied = false;
			championNum = 3;
			maxKills = playerThreeKills;
		}
		else if (playerThreeKills == maxKills){
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
