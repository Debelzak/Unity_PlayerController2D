using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Player))]
public class PlayerInput : MonoBehaviour {
	Player player;
	public bool listenGame;
	public bool listenMenu;

	public Vector2 moveAxis;
	public bool jumpInput;
	public bool jumpInputUp;
	public bool pauseInput;

	void Start () {
		player = GetComponent<Player> ();
		listenGame = true;
		listenMenu = true;
	}

	void Update () {
		moveAxis = Vector2.zero;
		moveAxis = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		jumpInput = false;
		jumpInputUp = false;
		pauseInput = false;

		if (Input.GetKeyDown (KeyCode.Space)) {
			jumpInput = true;
		}
		if (Input.GetKeyUp (KeyCode.Space)) {
			jumpInputUp = true;
		}
		if (Input.GetKeyDown (KeyCode.Escape)) {
			pauseInput = true;
		}
	}
}
