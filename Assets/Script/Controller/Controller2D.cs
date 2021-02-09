using UnityEngine;
using System.Collections;

public class Controller2D : MonoBehaviour {
	public float skinWidth = 0.15f;
	public float maxClimbAngle = 60;
	public float maxDescendAngle = 60;

	public Rigidbody2D rigidBody;
	public BoxCollider2D thisCollider;
	public LayerMask collisionMask;

	public CollisionInfo collisions;
	
	void Start() {
		rigidBody = GetComponent<Rigidbody2D>();
		thisCollider = GetComponent<BoxCollider2D>();
	}

	public void Move(Vector2 moveAmount) {
		collisions.Reset ();

		HandleCollisions (ref moveAmount);

		rigidBody.MovePosition(rigidBody.position + moveAmount);
	}

	void HandleCollisions(ref Vector2 moveAmount) {
		if(moveAmount.x != 0) {
			HorizontalCollisions (ref moveAmount);
		}
		if(moveAmount.y != 0) {
			VerticalCollisions (ref moveAmount);
		}
	}
	
	void VerticalCollisions(ref Vector2 moveAmount) {
		Vector2 movingDirection = Vector2.zero;
		movingDirection.x = (moveAmount.x < 0) ? -1 : (moveAmount.x > 0) ? 1 : 0;
		movingDirection.y = (moveAmount.y < 0) ? -1 : (moveAmount.y > 0) ? 1 : 0;

		float rayLength = skinWidth*2;

		Vector2 rayOrigin = (movingDirection.y == -1) ? new Vector2(thisCollider.bounds.min.x + thisCollider.bounds.size.x/2, thisCollider.bounds.min.y + skinWidth)
							: new Vector2(thisCollider.bounds.min.x + thisCollider.bounds.size.x/2, thisCollider.bounds.max.y - skinWidth);

		RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * movingDirection.y, rayLength, collisionMask);

		Debug.DrawRay(rayOrigin, Vector2.up * movingDirection, Color.red);

		if (hit) {
			collisions.terrainAngle = Vector2.Angle(Vector2.up, hit.normal);

			if(hit.collider.tag == "OneWayPlatform") {
				if(movingDirection.y > 0) {
					return;
				}
			}

			moveAmount.y = (hit.distance - skinWidth) * movingDirection.y;

			collisions.below = movingDirection.y == -1;
			collisions.above = movingDirection.y == 1;

			if(collisions.terrainAngle != 0) {
				Vector2 rayTopOrigin = new Vector2(thisCollider.bounds.min.x + thisCollider.bounds.size.x/2, thisCollider.bounds.max.y - skinWidth);
				RaycastHit2D hitTop = Physics2D.Raycast(rayTopOrigin, Vector2.up, rayLength, collisionMask);

				if(hitTop) {
					collisions.above = true;

					collisions.left = hit.normal.x > 0;
					collisions.right = hit.normal.x < 0;

					if( (collisions.right && moveAmount.x > 0) || (collisions.left && moveAmount.x < 0)) {
						moveAmount = Vector2.zero;
					}
				}
			}
		}
	}

	void HorizontalCollisions (ref Vector2 moveAmount) {
		Vector2 movingDirection = Vector2.zero;
		movingDirection.x = (moveAmount.x < 0) ? -1 : (moveAmount.x > 0) ? 1 : 0;
		movingDirection.y = (moveAmount.y < 0) ? -1 : (moveAmount.y > 0) ? 1 : 0;

		float rayLength = skinWidth * 2;

		Vector2 rayOrigin = (movingDirection.x < 0) ? new Vector2(thisCollider.bounds.min.x + skinWidth, thisCollider.bounds.min.y + thisCollider.bounds.size.y/2)
							: new Vector2(thisCollider.bounds.max.x - skinWidth, thisCollider.bounds.min.y + thisCollider.bounds.size.y/2);
		RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * movingDirection.x, rayLength, collisionMask);

		Debug.DrawRay(rayOrigin, Vector2.right * movingDirection, Color.red);

		if(hit) {
			if(Mathf.Abs(hit.normal.x) != 1) {
				return;
			}
			if(hit.collider.tag == "OneWayPlatform") {
				return;
			}

			moveAmount.x = (hit.distance - skinWidth) * movingDirection.x;
			collisions.right = movingDirection.x == -1;
			collisions.left = movingDirection.x == 1;
		}

		if(collisions.left && movingDirection.x < 0) {
			moveAmount.x = 0;
		}

		if(collisions.right && movingDirection.x > 0) {
			moveAmount.x = 0;
		}

	}

	public struct CollisionInfo {
		public bool above, below;
		public bool left, right;
		public float terrainAngle;
		public float wallAngle;

		public void Reset() {
			above = below = false;
			left = right = false;
			terrainAngle = 0;
		}
	}
}
