using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour {
	[Header("Movement")]
	public float moveSpeed = 6;
	public float accelerationTimeAirborne = .05f;
	public float accelerationTimeGrounded = .0f;
	[Range(1, 100)] public float climbSlopeSpeed = 100;
	[Range(100, 200)] public float descendSlopeSpeed = 100;

	[Header("Jump")]
	public float minJumpHeight;
	public float maxJumpHeight;
	public float timeToJumpApex = 1;

	[Header("Double Jump")]
	public bool enableDoubleJump;
	[Range(1, 100)] public int doubleJumpSpeed = 100;

	[Header("Wall slide")]
	public bool enableWallSlide;
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

		controller.Move(velocity * Time.deltaTime);

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
					velocity.y = maxJumpVelocity * ( (float)doubleJumpSpeed / 100);
				}
			}
			//Jump cancel
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

		//velocity climbing slope
		if(controller.collisions.slopeClimbing) {
			velocity.x *= (float)climbSlopeSpeed / 100;
		}

		//velocity descending slope
		if(controller.collisions.slopeDescending) {
			velocity.x *= (float)descendSlopeSpeed / 100;
		}

		velocity.y += Physics2D.gravity.y * Time.deltaTime;
	}

	void RefreshDebugInfo() {
		GameObject debugInfo = GameObject.Find("DebugInfo");
		Text debugInfoTxt = debugInfo.GetComponent<Text>();
		debugInfoTxt.text = "      Debug Info \n" +
							"FPS: " + Mathf.Ceil (fps).ToString() + "\n" +
							"Position: " + "\n" +
							"- X " + Math.Round((double)transform.position.x, 2) + "\n" +
							"- Y " + Math.Round((double)transform.position.y, 2) + "\n" +
							"Velocity: " + "\n" +
							"- X " + Math.Round((double)velocity.x, 2) + "\n" +
							"- Y " + Math.Round((double)velocity.y, 2) + "\n" +
							"\n" +
							"      Collisions " + "\n" +
							"Above: " + controller.collisions.above + "\n" +
							"Below: " + controller.collisions.below + "\n" +
							"Left: " + controller.collisions.left + "\n" +
							"Right: " + controller.collisions.right + "\n" +
							"One Way Platform: " + controller.collisions.onOneWayPlatform + "\n" +
							"On Slope: " + controller.collisions.slope + "\n" +
							" - Climbing: " + controller.collisions.slopeClimbing + "\n" +
							" - Descending: " + controller.collisions.slopeDescending + "\n" +
							"\n" +
							"      Features " + "\n" +
							"Double Jump: " + enableDoubleJump + "\n" +
							"Wall Slide: " + enableWallSlide + "\n" +

							"";
	}
}
