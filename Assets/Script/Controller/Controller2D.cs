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
		HandleVerticalCollisions (ref moveAmount);
		HandleHorizontalCollisions (ref moveAmount);
	}
	
	void HandleVerticalCollisions(ref Vector2 moveAmount) {
		Vector2 movingDirection = Vector2.zero;
		movingDirection.x = (moveAmount.x < 0) ? -1 : (moveAmount.x > 0) ? 1 : 0;
		movingDirection.y = (moveAmount.y < 0) ? -1 : (moveAmount.y > 0) ? 1 : 0;

		float rayLength = skinWidth*2;

		Vector2 rayOrigin = (movingDirection.y == -1) ? new Vector2(thisCollider.bounds.min.x + thisCollider.bounds.size.x/2, thisCollider.bounds.min.y + skinWidth)
							: new Vector2(thisCollider.bounds.min.x + thisCollider.bounds.size.x/2, thisCollider.bounds.max.y - skinWidth);

		RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * movingDirection.y, rayLength, collisionMask);

		Debug.DrawRay(rayOrigin, Vector2.up * movingDirection, Color.red);

		if (hit) {
			float terrainAngle = Vector2.Angle(Vector2.up, hit.normal);

			moveAmount.y = (hit.distance - skinWidth) * movingDirection.y;

			if(terrainAngle != 0) {
				Vector2 rayTopOrigin = new Vector2(thisCollider.bounds.min.x + thisCollider.bounds.size.x/2, thisCollider.bounds.max.y - skinWidth);
				RaycastHit2D hitTop = Physics2D.Raycast(rayTopOrigin, Vector2.up, rayLength, collisionMask);

				if(hitTop) {
					collisions.above = true;
					moveAmount.x = 0;
				}
			}

			collisions.below = movingDirection.y == -1;
			collisions.above = movingDirection.y == 1;
		}
	}

	void HandleHorizontalCollisions (ref Vector2 moveAmount) {
		return;
	}

	public struct CollisionInfo {
		public bool above, below;
		public bool left, right;

		public void Reset() {
			above = below = false;
			left = right = false;
		}
	}
}
