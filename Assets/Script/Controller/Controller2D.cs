﻿using UnityEngine;
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

	public void Move(Vector2 moveAmount) {
		UpdateRaycastOrigins ();

		collisions.Reset ();
		collisions.moveAmountOld = moveAmount;

		if (moveAmount.x != 0) {
			collisions.faceDir = (int)Mathf.Sign(moveAmount.x);
		}

		HorizontalCollisions (ref moveAmount);
		SlopeCollisions(ref moveAmount);
		if (moveAmount.y != 0) {
			VerticalCollisions (ref moveAmount);
		}

		rb2d.MovePosition (rb2d.position + moveAmount);
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

				if(directionY == -1)
				collisions.below = true;
				if(directionY == 1)
				collisions.above = true;
			}
		}
	}

	void SlopeCollisions(ref Vector2 moveAmount) {
		collisions.slope = false;
		float directionY = Mathf.Sign (moveAmount.y);

		Vector2 rayOrigin = new Vector2(thisCollider.bounds.min.x + thisCollider.bounds.size.x/2, thisCollider.bounds.min.y + thisCollider.bounds.size.y/2);

		float tempSkinWidth = Mathf.Abs(moveAmount.y);

		if(Mathf.Abs(moveAmount.y) < skinWidth) {
			tempSkinWidth = 0.5f;
		}

		float rayLength = thisCollider.bounds.size.y/2 + tempSkinWidth;

		RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, slopeCollisionMask);

		if(hit) {
			Debug.DrawLine(rayOrigin, hit.point, Color.yellow);

			if(directionY < 0) {
				moveAmount = Vector2.Perpendicular(hit.normal) * -moveAmount.x;

				collisions.slope = true;
				collisions.slopeClimbing = (moveAmount.y > 0) ? true : false;
				collisions.slopeDescending = (moveAmount.y < 0) ? true : false;
			}

			moveAmount.y += (hit.distance - thisCollider.bounds.size.y/2) * directionY;

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
		public bool slopeClimbing;
		public bool slopeDescending;

		public Vector2 moveAmountOld;
		public int faceDir;
		public bool onOneWayPlatform, fallingThroughPlatform;

		public void Reset() {
			above = below = false;
			left = right = false;
			onOneWayPlatform = false;
			slopeClimbing = false;
			slopeDescending = false;
		}
	}

}
