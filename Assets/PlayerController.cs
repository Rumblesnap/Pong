using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {


	public bool atTopWall = false; //flags if player touching top wall
	public bool atBottomWall = false; //flags if player touching bottom wall

	public KeyCode upInput = KeyCode.UpArrow; //input configuration
	public KeyCode downInput = KeyCode.DownArrow;

	public float speed = 5.0f; //movement speed

	public bool moveUp = false;	//flags if player moving up
	public bool moveDown = false; //flags if player moving down

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		//Player moves up and down if not in contact with wall
		//Cannot move up if touching top wall, cannot move down if touching bottom wall
		if (atTopWall == false) {
			if (Input.GetKey (upInput)) {
				transform.Translate (Vector3.up * Time.deltaTime * speed);
				moveUp = true;
				moveDown = false;
			} else {
				moveUp = false;
			}
		}
		if (atBottomWall == false) {
			if (Input.GetKey (downInput)) {
				transform.Translate (Vector3.down * Time.deltaTime * speed);
				moveUp = false;
				moveDown = true;
			} else {
				moveDown = false;
			}
		}		 
	}

	void OnCollisionEnter (Collision other) {

		//flags called depending on which wall is touched
		if (other.gameObject.name == "TopWall")
			atTopWall = true;
		if (other.gameObject.name == "BottomWall")
			atBottomWall = true;
	}

	void OnCollisionExit (Collision other) {

		//flags removed when contact with wall lost
		if (other.gameObject.name == "TopWall")
			atTopWall = false;
		if (other.gameObject.name == "BottomWall")
			atBottomWall = false;
	}
}
