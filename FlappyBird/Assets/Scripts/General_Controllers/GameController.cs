using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController>
{
	[SerializeField] private Player player;
	[SerializeField] private ScoreController score;
	[SerializeField] private PauseController pause;

	private gameStates lastGameState;
	public gameStates gameState {get; private set; }
	public bool canStartGame { get; private set; }

	public delegate void GameEvents();
	public event GameEvents StartGame_Event;
	public event GameEvents EndGame_Event;
	public event GameEvents GameReset_Event;

	#region Unity Functions
	private void Start()
	{
		SwitchGameState(gameStates.menu);
		StartCoroutine(DelayToStartGame());
	}

	private void OnEnable()
	{
		player.StartGame_Event += StartGame;
		player.PlayerDied_Event += EndGame;

		score.EndScoreboard_Event += OpenMenu;

		pause.GamePaused_Event += PauseGame;
		pause.GameUnpaused_Event += UnpauseGame;
	}

	private void OnDisable()
	{
		player.StartGame_Event -= StartGame;
		player.PlayerDied_Event -= EndGame;

		score.EndScoreboard_Event -= OpenMenu;

		pause.GamePaused_Event -= PauseGame;
		pause.GameUnpaused_Event -= UnpauseGame;
	}

	#endregion

	private void SwitchGameState(gameStates _newState)
	{
		lastGameState = gameState;
		gameState = _newState;
	}
	
	private void OpenMenu()
	{
		GameReset_Event?.Invoke();
		SwitchGameState(gameStates.menu);
		StartCoroutine(DelayToStartGame());
	}

	#region Start and End Game Functions
	//Controls the time the player can start the game.
	//This prevents the player to start the game immediatly when game loads, after score or after pauses.
	private IEnumerator DelayToStartGame()
	{
		yield return new WaitForSeconds(.5f);
		canStartGame = true;
	}

	private void StartGame()
	{
		SwitchGameState(gameStates.game);
		canStartGame = false;
		StartGame_Event?.Invoke();
	}

	private void EndGame()
	{
		SwitchGameState(gameStates.score);
		EndGame_Event?.Invoke();
	}
	#endregion

	#region Pause Functions
	private void PauseGame()
	{
		SwitchGameState(gameStates.pause);
		Time.timeScale = 0;
	}

	private void UnpauseGame()
	{
		if (lastGameState == gameStates.menu)
		{
			canStartGame = false;
			StartCoroutine(DelayToStartGame());
		}
		Time.timeScale = 1;
		SwitchGameState(lastGameState);
	}
	#endregion
}

public enum gameStates
{
	none = 0,
	menu = 1,
	game = 2,
	score = 3,
	pause = 4
}
