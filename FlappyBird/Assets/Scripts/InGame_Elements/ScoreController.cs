using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class ScoreController : MonoBehaviour
{
	[Header("InGame Score")]
	[SerializeField] private TextMeshProUGUI scoreText;
	[SerializeField] private TextMeshProUGUI playerScoreText;

	[Header("Scoreboard")]
	[SerializeField] private TextMeshProUGUI runScoreText;
	[SerializeField] private TextMeshProUGUI bestScoreText;
	[SerializeField] private Image newBestImage;
	[SerializeField] private Button playButton;

	[Header("Other Components")]
	[SerializeField] private AudioSource scoreSFXSource;

	private int score = 0;
	private int bestScore = 0;
	private bool isBestScore = false;
	private float timeInScore = 0;

	private Player player;
	private GameController gamecontroller;
	private Animator anim;

	public delegate void ScoreboardEvents();
	public event ScoreboardEvents EndScoreboard_Event;

	#region Unity Functions
	private void Awake()
	{
		anim = GetComponent<Animator>();
	}

	private void Start()
	{
		bestScore = SaveSystem.GetSavedBestScore();
		playerScoreText.alpha = 0;
	}

	private void OnEnable()
	{
		if (player == null)
			player = Player.Instance;

		if (gamecontroller == null)
			gamecontroller = GameController.Instance;

		player.PlayerScore_Event += AddScore;
		player.StartGame_Event += ResetScore;
		player.PlayerDied_Event += EndGame;
		gamecontroller.GameReset_Event += DisableButton;

	}

	private void OnDisable()
	{
		if (player != null)
		{
			player.PlayerScore_Event -= AddScore;
			player.StartGame_Event -= ResetScore;
			player.PlayerDied_Event -= EndGame;
		}

		gamecontroller.GameReset_Event += DisableButton;
	}

	private void Update()
	{
		//Controls a delay the player can exit the scoreboard with the keyboard
		if(gamecontroller.gameState == gameStates.score)
		{
			timeInScore += Time.deltaTime;

			if (timeInScore > 1 && Input.GetKeyDown(KeyCode.Space))
				Button_Play();
		}
	}
	#endregion

	//Add score to the player
	private void AddScore()
	{
		score++;
		UpdateScoreCanvas();

		//Small text feedback in the player's position indicating he got a new point
		Vector3 position = Camera.main.WorldToScreenPoint(player.transform.position);
		Vector3 textPos = (Vector2) position + (Vector2.right * 20) + (Vector2.up * 20);
		playerScoreText.rectTransform.position = textPos;
		playerScoreText.alpha = 1;
		playerScoreText.transform.DOMoveY(textPos.y +  50, .5f).SetEase(Ease.OutSine);
		DOTween.To(() => 1, value => playerScoreText.alpha = value, 0f, .2f).SetEase(Ease.InSine).SetDelay(.3f);

		//Point sfx
		scoreSFXSource.pitch = Random.Range(.9f, 1.1f);
		scoreSFXSource.Play();
	}

	//When the player died, the event calls this function
	private void EndGame()
	{
		timeInScore = 0;

		//Set scoreboard
		if (score > bestScore)
		{
			isBestScore = true;
			bestScore = score;
			newBestImage.DOFade(1, .5f).SetDelay(.25f).SetEase(Ease.OutSine);

			SaveSystem.SaveBestScore(bestScore);
		}

		runScoreText.text = score.ToString("00");
		bestScoreText.text = bestScore.ToString("00");

		anim.SetTrigger("Scoreboard");
		playButton.interactable = true;
		playButton.Select();
	}

	//Disable the play button of the scoreboard
	private void DisableButton()
	{
		playButton.interactable = false;
	}

	//When the player goes to menu, reset the scoreboard
	private void ResetScore()
	{
		score = 0;
		UpdateScoreCanvas();
		anim.SetTrigger("RevealScore");
		playButton.interactable = false;
	}

	//Update the in game score canvas
	private void UpdateScoreCanvas()
	{
		scoreText.text = score.ToString("00");
	}

	//The play button of the scoreboard.
	public void Button_Play()
	{
		if (GameController.Instance != null && GameController.Instance.gameState == gameStates.score || GameController.Instance == null)
		{
			if (isBestScore)
			{
				isBestScore = false;
				newBestImage.DOFade(0, .5f).SetEase(Ease.InSine);
			}

			anim.SetTrigger("HideScoreboard");
			EndScoreboard_Event?.Invoke();
		}
	}
}
