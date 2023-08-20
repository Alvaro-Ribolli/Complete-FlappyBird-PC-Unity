using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] private Animator animator;

	private GameController gameController;

	private void OnEnable()
	{
		if (gameController == null)
			gameController = GameController.Instance;

		gameController.GameReset_Event += OpenMenu;
		gameController.StartGame_Event += GameStarted;
	}

	private void OnDisable()
	{
		gameController.GameReset_Event -= OpenMenu;
		gameController.StartGame_Event -= GameStarted;
	}

	public void OpenMenu()
	{
		animator.SetTrigger("openMenu");
	}

	public void GameStarted()
	{
		animator.SetTrigger("startGame");
	}
}
