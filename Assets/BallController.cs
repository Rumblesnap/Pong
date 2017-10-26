using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {

	public GameObject gameManager = null;
	public GameObject player1 = null;
	public GameObject player2 = null;

	public float speed = 6.0f; //ball's movement speed

	public Vector3 startPos; //starting position after randomly selecting spot on y-axis
	public Vector3 startPosInitial; //starting position before randomly selecting spot on y-axis

	public Vector3 currentPos; //current position of ball
	public Vector3 maxPos = new Vector3 (8.0f, 8.0f, 0.0f);
	public Vector3 minPos = new Vector3 (-8.0f, -8.0f, 0.0f);

	public bool? moveLeft; //keeps track of which directions ball is currently moving
	public bool? moveRight; //keeping them nullable makes it a smoother transition every time the ball gets reset
	public bool? moveUp;
	public bool? moveDown;

	public bool atWall = false; //is ball touching wall
	public bool atGoal = false; //is ball touching goal
	public bool atPlayer = false; //is ball touching player

	public bool p1 = false; //flags to determine which player is being hit by ball
	public bool p2 = false;

	public int collideLimit = 0; //used to limit how many times a collision between two objects is detected to once
	public int hitLevel = -1; //determines where the ball hit the paddle. 0 = bottom, 1 = mid, 2 = top, -1 = default (aka not yet hit)

	// Use this for initialization
	void Start () {

		//store starting position of ball for reset after each goal
		startPosInitial = transform.position;
		startPoint();
		Invoke ("startMotion", 2.0f);
	}
	
	// Update is called once per frame
	void Update () {

		//movement instruction
		if (moveLeft == true)
			transform.Translate (Vector3.left * Time.deltaTime * speed);
		if (moveRight == true)
			transform.Translate (Vector3.right * Time.deltaTime * speed);
		if (moveUp == true)
			transform.Translate (Vector3.up * Time.deltaTime * speed);
		if (moveDown == true)
			transform.Translate (Vector3.down * Time.deltaTime * speed);


		//reverse UD direction when ball hits wall
		if (atWall == true && collideLimit == 1) {
			if (moveUp == true) {
				moveUp = false;
				moveDown = true;
			} else {
				moveDown = false;
				moveUp = true;
			}
			collideLimit--;
		}

		//reset ball when ball hits goal
		if (atGoal == true && collideLimit == 1) {
			resetBall ();
		}

		//reverse LR direction when ball hits player
		if (atPlayer == true && collideLimit == 1) {
			if (moveLeft == true) {
				moveLeft = false;
				moveRight = true;
			} else {
				moveRight = false;
				moveLeft = true;
			}

			//if the ball is moving up and hits the bottom of the player
			if (moveUp == true && hitLevel == 0) {
				//if the player is moving down when the ball hits, the ball gains some speed and changes direction
				if ((p1 == true && player1.GetComponent<PlayerController> ().moveDown == true)
				|| (p2 == true && player2.GetComponent<PlayerController> ().moveDown == true)) {
					speed += 1;
					moveUp = false;
					moveDown = true;
				}
				//if player is moving up, ball will continue upward but lose some speed
				if ((p1 == true && player1.GetComponent<PlayerController> ().moveUp == true)
				|| (p2 == true && player2.GetComponent<PlayerController> ().moveUp == true)) {
					speed -= 2;
					moveUp = true;
					moveDown = false;
				}
			//if the ball is moving up and hits the top of the player
			if (moveUp == true && hitLevel == 2) {
				//if the player is moving up when the ball hits, the ball gains some speed
				if ((p1 == true && player1.GetComponent<PlayerController> ().moveUp == true)
				|| (p2 == true && player2.GetComponent<PlayerController> ().moveUp == true)) {
					speed += 2;
					moveUp = true;
					moveDown = false;
				}
				//if the player is moving down, the ball will change direction and lose speed.
				if ((p1 == true && player1.GetComponent<PlayerController> ().moveDown == true)
				|| (p2 == true && player2.GetComponent<PlayerController> ().moveDown == true)) {
					speed -= 1;
					moveUp = false;
					moveDown = true;
				}
			}

			//if ball hits center, ball bounces straight off perpendicular to player
			if (hitLevel == 1) {
				moveUp = false;
				moveDown = false;
				speed = 5;
			}

			//if ball is moving down and hits the top part of the player
			if (moveDown == true && hitLevel == 2) {
				//if the player is moving up, the ball gains some speed and changes direction
				if ((p1 == true && player1.GetComponent<PlayerController> ().moveUp == true)
				|| (p2 == true && player2.GetComponent<PlayerController> ().moveUp == true)) {
					speed += 2;
					moveUp = true;
					moveDown = false;
				}
			    //if player is moving down, ball will continue downward but lose some speed
				}
				if ((p1 == true && player1.GetComponent<PlayerController> ().moveDown == true)
				|| (p2 == true && player2.GetComponent<PlayerController> ().moveDown == true)) {
					speed -= 2;
					moveUp = false;
					moveDown = true;
				}
			}
			//if ball is moving down and hits the bottom part of the player
			if (moveDown == true && hitLevel == 0) {
				//if the player is moving down when the ball hits down low, the ball gains some speed
				if ((p1 == true && player1.GetComponent<PlayerController> ().moveDown == true)
				|| (p2 == true && player2.GetComponent<PlayerController> ().moveDown == true)) {
					speed += 2;
					moveUp = false;
					moveDown = true;
				}
				//if player is moving up, ball will continue downward but lose some speed
				if ((p1 == true && player1.GetComponent<PlayerController> ().moveUp == true)
				|| (p2 == true && player2.GetComponent<PlayerController> ().moveUp == true)) {
					speed -= 2;
					moveUp = true;
					moveDown = false;
				}
			}

			//if ball is moving perpendicular to players and hits top, ball moves up. If it hits bottom, ball moves down.
			if (moveDown == false && moveUp == false && hitLevel == 2) {
				moveUp = true;
			}
			if (moveDown == false && moveUp == false && hitLevel == 0) {
				moveDown = true;
			}

			//reset data values to reflect no longer processing a hit once ball has been properly
			hitLevel = -1;
			collideLimit--;
		}

		//reset ball if collision detection somehow gets mishandled and value is out of scope
		if (collideLimit > 1 || collideLimit < 0) {
			resetBall ();
		}

		//reset collideLimit to 0 if for some reason it is flagged when ball is not actively colliding
		if (collideLimit == 1 && atWall == false && atPlayer == false && atGoal == false) {
			collideLimit--;
		}

		//keep speed above 4 to keep game from being too dull
		if (speed < 4) {
			speed = 4;
		}

		//essentially always checking whenever not processing collision, collideLimit is zero anytime ball is not actively colliding with another object
		if (collideLimit == 0) {
			setPosition ();
			//if current position exceeds maximum position (x and y axis) that fits on screen, ball resets
			if (currentPos.x > maxPos.x || currentPos.y > maxPos.y || currentPos.x < minPos.x || currentPos.y < minPos.y)
				resetBall ();
		}

		//preventing ball from getting stuck moving vertically
		if ((moveLeft == true && moveRight == true) || (moveLeft == false && moveRight == false)) {
			resetBall ();
		}

		//reset hitLevel after hitting player
		if (hitLevel != -1 && !atWall && !atPlayer && !atGoal)
			hitLevel = -1;
	}

	//checking for collision with goal zones, boundary walls, or players
	void OnCollisionEnter (Collision other) {
	
		//add +1 to player score and set goal contact flag when ball hits both goals
		if (other.gameObject.name == "Goal1") {
			collideLimit++;
			if (collideLimit == 1) {
				gameManager.GetComponent<GameManager> ().player2Score += 1;
				gameManager.GetComponent<GameManager> ().setText ();
				atGoal = true;
			}
		}
		if (other.gameObject.name == "Goal2") {
			collideLimit++;
			if (collideLimit == 1) {
				gameManager.GetComponent<GameManager> ().player1Score += 1;
				gameManager.GetComponent<GameManager> ().setText ();
				atGoal = true;
			}
		}

		//set wall contact flag when ball hits wall
		if (other.gameObject.tag == "Wall") {
			collideLimit++;
			if (collideLimit == 1) {
				atWall = true;
			}
		}

		//set player contact flag when ball hits either player
		if (other.gameObject.tag == "Player") {
			collideLimit++;
			if (collideLimit == 1) {
				atPlayer = true;
				//flag specifically which player ball collided with, to be stored & used for hitLevel processes
				if (other.gameObject.name == "Player 1") {
					p1 = true;
					p2 = false;
				} else {
					p2 = true;
					p1 = false;
				}
			}
		}
	}

	//reset all collision flags when leaving collision
	void OnCollisionExit (Collision other) {

		if (other.gameObject.name == "Goal1")
			atGoal = false;
		if (other.gameObject.name == "Goal2")
			atGoal = false;
		if (other.gameObject.tag == "Wall")
			atWall = false;
		if (other.gameObject.tag == "Player") {
			atPlayer = false;
			p1 = false;
			p2 = false;
		}
	}

	//triggers checking for what part of the player the ball hits
	void OnTriggerEnter (Collider other) {
		if (other.gameObject.name == "Bottom")
			hitLevel = 0;
		if (other.gameObject.name == "Middle")
			hitLevel = 1;
		if (other.gameObject.name == "Top")
			hitLevel = 2;
	}

	//places ball at a random place on the y-axis in center of game field
	void startPoint () {
		float startPt = Random.Range (-4.0f, 6.0f);
		Vector3 temp = new Vector3 (0, startPt, 0);
		transform.position += temp;
	}

	//store current position to be compared with maximum and minimum x and y positions to prevent ball from escaping game field
	void setPosition () {
		currentPos = transform.position;
	}

	//generates initial direction of ball movement via a random number modulus'd down to determine the values of all movement-related boolean variables
	void startMotion () {

		int starterLR = Random.Range (0, 100);
		int starterUD = Random.Range (0, 100);
		starterLR %= 2;
		starterUD %= 2;

		if (starterLR == 0)
			moveRight = true;
		else
			moveLeft = true;

		if (starterUD == 0)
			moveUp = true;
		else
			moveDown = true;
	}
		
	//reset the ball's position, direction, speed, etc.
	void resetBall () {
		moveUp = null;
		moveDown = null;
		moveLeft = null;
		moveRight = null;
		speed = 6.0f;
		transform.position = startPosInitial;
		startPoint ();
		collideLimit = 0;
		hitLevel = -1;
		Invoke ("startMotion", 2.0f);
	}
}
