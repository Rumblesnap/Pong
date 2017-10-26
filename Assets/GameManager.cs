using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public int player1Score;
	public int player2Score;
	public Text player1ScoreText;
	public Text player2ScoreText;

	// Use this for initialization
	void Start () {

		//initialize player scores both at 0 at start
		player1Score = 0;
		player2Score = 0;
		setText ();
	}

	// Update is called once per frame
	void Update () {
		
	}

	public void setText () {
		player1ScoreText.text = player1Score.ToString ();
		player2ScoreText.text = player2Score.ToString ();
	}

}
