using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Singleton<Player>
{
    public delegate void PlayerEvents();
    public event PlayerEvents StartGame_Event;
    public event PlayerEvents PlayerDied_Event;
	public event PlayerEvents PlayerScore_Event;

	[Header("Attributes")]
	[SerializeField] private bool imortal;
	[SerializeField] private float jumpForce = 50;
	[SerializeField] private float maxYVel = 10;

	[Header("Components")]
	[SerializeField] private AudioSource sfxSource;

    private Rigidbody2D rb;
	private Animator animator;
	private Vector2 initialPos;
	
    private GameController gameController;

	#region Initialization
	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		initialPos = transform.position;
	}

	void Start()
    {
		SetStartPlayer();
	}

	private void OnEnable()
	{
		if(gameController == null)
			gameController = GameController.Instance;

		gameController.GameReset_Event += SetStartPlayer;
	}

	private void OnDisable()
	{
		gameController.GameReset_Event -= SetStartPlayer;
	}
	#endregion

	#region Player setups
	//Setup the player when the game goes to menu
	private void SetStartPlayer()
	{
		transform.position = initialPos;
		transform.eulerAngles = Vector2.zero;
		rb.isKinematic = true;

		int playerColor = Random.Range(0, 3);
		animator.SetInteger("PlayerColor", playerColor);
		animator.speed = 1;
	}

	//Setup the player when he dies
	private void SetDiedPlayer()
	{
		PlayerDied_Event?.Invoke();
		rb.velocity = Vector2.zero;
		rb.isKinematic = true;
		animator.speed = 0;
	}
	#endregion

	#region Updates
	void Update()
    {
		//If the player can start the game, then setup the it
        if(gameController.gameState == gameStates.menu && gameController.canStartGame)
		{
			if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonUp(0))
			{
                StartGame_Event?.Invoke();
				rb.isKinematic = false;
				Jump();
			}
		}else if(gameController.gameState == gameStates.game)
		{
			//Jump
			if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
				Jump();
		}
    }

	private void FixedUpdate()
	{
		//Controls the max velocity of the player and its rotation
		if (gameController.gameState == gameStates.game)
		{
			float maxVelTime = maxYVel * Time.fixedDeltaTime;

			if (rb.velocity.y < -maxVelTime)
				rb.velocity = new Vector2(0, -maxVelTime);

			rb.MoveRotation(rb.velocity.y * 10);
		}
	}
	#endregion

	//Jump
	private void Jump()
	{
		//Apply Force to the Rigidbody
		rb.velocity = Vector2.zero;
		rb.AddForce(Vector2.up * jumpForce);

		//Play sfx
		sfxSource.pitch = Random.Range(.8f, 1.2f);
		sfxSource.Play();

	}

	#region Triggers
	private void OnTriggerEnter2D(Collider2D collision)
	{
		//Verify if the player touches a pipe or the ground
		if (!imortal && collision.CompareTag("Ground"))
		{
			SetDiedPlayer();
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		//Verify if the player exit the score trigger from the pipes.
		if (collision.CompareTag("Score"))
		{
			PlayerScore_Event?.Invoke();
		}
	}
	#endregion
}
