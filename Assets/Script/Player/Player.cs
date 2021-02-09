using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {
	public float maxJumpHeight;
	public float minJumpHeight;
	public float timeToJumpApex;
	float moveSpeed = 6;
	public bool enableWallSlide;
	public bool enableDoubleJump;
	public float wallSlideSpeed = 0.5f;
	public float wallJumpImpulse = 1.0f;
	bool isWallSliding;

	float maxJumpVelocity;
	float minJumpVelocity;
	[SerializeField]Vector3 velocity;
	bool isGrounded;
	bool canJump;
	bool canDoubleJump;

	Controller2D controller;
	public PlayerInput input;

	void Start() {
		controller = GetComponent<Controller2D>();
		input = GetComponent<PlayerInput>();
		Physics2D.gravity = new Vector2(0, -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2));
		maxJumpVelocity = Mathf.Abs(Physics2D.gravity.y) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs (Physics2D.gravity.y) * minJumpHeight);
	}

	void Update() {
		HandlePlayerInput();
		CalculateVelocity();
		CheckGround();

		controller.Move(velocity * Time.deltaTime);

		if (controller.collisions.above || controller.collisions.below) {
			velocity.y = 0;
		}
	}

	void HandlePlayerInput() {
		if(input.jumpInput) {
			//Jump
			if (canJump) {
				canJump = false;
				Jump();
			}

			//Double jump
			if(enableDoubleJump && canDoubleJump && !isGrounded && !canJump) {
				canDoubleJump = false;
				Jump();
			}
		}

		if(input.jumpInputUp){
			if (velocity.y > minJumpVelocity) {
				velocity.y = minJumpVelocity;
			}
		}
	}

	void CheckGround()
	{
		Vector2 point = new Vector2 (controller.thisCollider.bounds.min.x + (controller.thisCollider.size.x / 2), controller.thisCollider.bounds.min.y);
		isGrounded = (controller.collisions.below || Physics2D.OverlapCircle(point, 0.4f, controller.collisionMask)) ? true : false;

		canJump = (isGrounded && !controller.collisions.above) ? true : false;

		if(isGrounded && velocity.y <= 0) {
			canDoubleJump = true;
		}
	}

	void CalculateVelocity() {
		velocity.x = input.moveAxis.x * moveSpeed;
		velocity.y += Physics2D.gravity.y * Time.deltaTime;
	}

	void Jump() {
		velocity.y = maxJumpVelocity;
	}
}
