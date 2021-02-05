using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Player))]
public class PlayerInput : MonoBehaviour {
	Player player;
	public Vector2 moveAxis;
	public bool jumpInput;
	public bool jumpInputUp;

	void Start () {
		player = GetComponent<Player> ();
	}

	void Update () {
		moveAxis = Vector2.zero;
		moveAxis = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		jumpInput = false;
		jumpInputUp = false;

		if (Input.GetKeyDown (KeyCode.Space)) {
			jumpInput = true;
		}
		if (Input.GetKeyUp (KeyCode.Space)) {
			jumpInputUp = true;
		}
	}
}
