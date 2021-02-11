using UnityEngine;
using System.Collections;

public class Controller2D : RaycastController {

	public float maxSlopeAngle = 80;

	public CollisionInfo collisions;
	[HideInInspector]
	public Vector2 playerInput;
	public Rigidbody2D rb2d;

	public override void Start() {
		base.Start ();
		collisions.faceDir = 1;
		rb2d = GetComponent<Rigidbody2D>();
	}

	public void Move(Vector2 moveAmount, bool standingOnPlatform) {
		Move (moveAmount, Vector2.zero, standingOnPlatform);
	}

	public void Move(Vector2 moveAmount, Vector2 input, bool standingOnPlatform = false) {
		UpdateRaycastOrigins ();

		collisions.Reset ();
		collisions.moveAmountOld = moveAmount;
		playerInput = input;

		if (moveAmount.x != 0) {
			collisions.faceDir = (int)Mathf.Sign(moveAmount.x);
		}

		SlopeCollisions(ref moveAmount);
		HorizontalCollisions (ref moveAmount);
		if (moveAmount.y != 0) {
			VerticalCollisions (ref moveAmount);
		}

		rb2d.MovePosition (rb2d.position + moveAmount);

		if (standingOnPlatform) {
			collisions.below = true;
		}
	}

	void HorizontalCollisions(ref Vector2 moveAmount) {
		float directionX = collisions.faceDir;
		float rayLength = Mathf.Abs (moveAmount.x) + skinWidth;

		if (Mathf.Abs(moveAmount.x) < skinWidth) {
			rayLength = 2*skinWidth;
		}

		for (int i = 0; i < horizontalRayCount; i ++) {
			Vector2 rayOrigin = (directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.right * directionX,Color.red);

			if (hit) {
				if(collisions.slope && i == Mathf.Round(horizontalRayCount/4)) {
					Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.yellow);
				}
				if(collisions.slope && i != Mathf.Round(horizontalRayCount/4) )
				{
					continue;
				}
				if (hit.collider.tag == "OneWayPlatform") {
					continue;
				}

				if (hit.distance == 0) {
					continue;
				}

				moveAmount.x = (hit.distance - skinWidth) * directionX;
				rayLength = hit.distance;

				collisions.left = directionX == -1;
				collisions.right = directionX == 1;
			}
		}
	}

	void VerticalCollisions(ref Vector2 moveAmount) {
		float directionY = Mathf.Sign (moveAmount.y);
		float rayLength = Mathf.Abs (moveAmount.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i ++) {

			Vector2 rayOrigin = (directionY == -1)?raycastOrigins.bottomLeft:raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.up * directionY,Color.red);

			if (hit) {
				if (hit.collider.tag == "OneWayPlatform") {
					collisions.onOneWayPlatform = true;
					if (directionY == 1 || hit.distance == 0) {
						continue;
					}
					if (collisions.fallingThroughPlatform) {
						Invoke("ResetFallingThroughPlatform",.25f);
						continue;
					}
				}
				if(collisions.slope && i != Mathf.Round(verticalRayCount/2))
				{
					continue;
				}

				moveAmount.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;

				collisions.below = directionY == -1;
				collisions.above = directionY == 1;
			}
		}
	}

	void SlopeCollisions(ref Vector2 moveAmount) {
		float directionY = Mathf.Sign (moveAmount.y);
		Vector2 rayOrigin = new Vector2(thisCollider.bounds.min.x + thisCollider.bounds.size.x/2, thisCollider.bounds.min.y + thisCollider.bounds.size.y/2);

		float tempSkinWidth = skinWidth;

		if(Mathf.Abs(moveAmount.y) < skinWidth) {
			tempSkinWidth = 0.5f;
		}

		print(Mathf.Abs (moveAmount.y));

		float rayLength = thisCollider.bounds.size.y/2 + tempSkinWidth;

		RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, slopeCollisionMask);

		if(hit && !collisions.right && !collisions.left && !collisions.above) {
			Debug.DrawLine(rayOrigin, hit.point, Color.yellow);

			moveAmount = Vector2.Perpendicular(hit.normal) * -moveAmount.x;
			moveAmount.y -= (hit.distance - thisCollider.bounds.size.y/2);
		
			collisions.slope = true;
			collisions.above = directionY == 1;
			collisions.below = directionY == -1;
		}
	}

	void ResetFallingThroughPlatform() {
		collisions.fallingThroughPlatform = false;
	}

	public struct CollisionInfo {
		public bool above, below;
		public bool left, right;
		public bool slope;

		public Vector2 moveAmountOld;
		public int faceDir;
		public bool onOneWayPlatform, fallingThroughPlatform;

		public void Reset() {
			above = below = false;
			left = right = false;
			slope = false;
			onOneWayPlatform = false;
		}
	}

}
