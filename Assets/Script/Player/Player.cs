using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour {
	public float maxJumpHeight;
	public float minJumpHeight;
	float accelerationTimeAirborne = .05f;
	float accelerationTimeGrounded = .0f;
	public float timeToJumpApex;
	float moveSpeed = 6;
	public bool enableWallSlide;
	public bool enableDoubleJump;
	public float wallSlideSpeed = 0.5f;
	public float wallJumpImpulse = 1.0f;
	bool isWallSliding;

	float maxJumpVelocity;
	float minJumpVelocity;
	Vector3 velocity;
	float velocityXSmoothing;
	bool isGrounded;
	bool canJump;
	bool canDoubleJump;

	public GameObject pauseMenuObject;
	Controller2D controller;
	public PlayerInput input;

	float deltaTime;
	float fps;

	void Start() {
		controller = GetComponent<Controller2D>();
		input = GetComponent<PlayerInput>();
		Physics2D.gravity = new Vector2(0, -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2));
		maxJumpVelocity = Mathf.Abs(Physics2D.gravity.y) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs (Physics2D.gravity.y) * minJumpHeight);
	}

	void Update() {
		HandlePlayerInput();
		RefreshDebugInfo();
		
		if(Time.timeScale != 0) {
			deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
		}
		fps = 1.0f / deltaTime;
	}

	private void FixedUpdate() {
		CalculateVelocity();
		CheckGround();

		controller.Move(velocity * Time.deltaTime, input.moveAxis);

		if (controller.collisions.above || controller.collisions.below) {
			velocity.y = 0;
		}
	}

	void HandlePlayerInput() {
		//Game commands
		if(input.listenGame) {
			if(input.jumpInput) {
				//Platform drop
				if(controller.collisions.onOneWayPlatform && input.moveAxis.y == -1) {
					controller.collisions.fallingThroughPlatform = true;
					return;
				}

				//Jump
				if (canJump) {
					canJump = false;
					velocity.y = maxJumpVelocity;
				}

				//Double jump
				if(enableDoubleJump && canDoubleJump && !isGrounded && !canJump) {
					canDoubleJump = false;
					velocity.y = maxJumpVelocity * 0.75f;
				}
			}

			if(input.jumpInputUp){
				if (velocity.y > minJumpVelocity) {
					velocity.y = minJumpVelocity;
				}
			}
		}

		//Menu commands
		if(input.listenMenu) {
			if(input.pauseInput && Time.timeScale > 0) {
				Time.timeScale = 0;
				pauseMenuObject.gameObject.SetActive(true);
				input.listenGame = false;
				return;
			}

			if(input.pauseInput && Time.timeScale == 0) {
				Time.timeScale = 1;
				pauseMenuObject.gameObject.SetActive(false);
				input.listenGame = true;
				return;
			}
		}
	}

	void CheckGround()
	{
		isGrounded = (controller.collisions.below) ? true : false;

		canJump = (isGrounded) ? true : false;

		if(isGrounded && velocity.y <= 0) {
			canDoubleJump = true;
		}
	}

	void CalculateVelocity() {
		float targetVelocityX = input.moveAxis.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);
		velocity.y += Physics2D.gravity.y * Time.deltaTime;
	}

	void RefreshDebugInfo() {
		GameObject debugInfo = GameObject.Find("DebugInfo");
		Text debugInfoTxt = debugInfo.GetComponent<Text>();
		debugInfoTxt.text = "      Debug Info \n" +
							"FPS: " + Mathf.Ceil (fps).ToString() + "\n" +
							"Position: " + "X: " + Math.Round(transform.position.x, 2) + " Y: " + Math.Round(transform.position.y, 2) + "\n" +
							"Velocity: " + "X: " + Math.Round(velocity.x, 2) + " Y: " + Math.Round(velocity.y, 2) + "\n" +
							"Can Jump: " + canJump + "\n\n" +

							"      Collisions " + "\n" +
							"Above: " + controller.collisions.above + "\n" +
							"Below: " + controller.collisions.below + "\n" +
							"Left: " + controller.collisions.left + "\n" +
							"Right: " + controller.collisions.right + "\n" +
							"On Slope: " + controller.collisions.slope + "\n" +





							"";
	}
}
