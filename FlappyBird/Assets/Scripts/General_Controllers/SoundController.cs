using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundController : MonoBehaviour
{
	[Header("Sound Components")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioSource musicSource;
	[SerializeField] private AudioSource deathSource;

	[Header("Sounds")]
	[SerializeField] private AudioClip musicIntroduction;
	[SerializeField] private AudioClip musicLoop;

	private Coroutine musicLoopCoroutine;
	private GameController gameController;

	#region Unity Functions
	private void Start()
	{
		GameReseted();
	}

	private void OnEnable()
	{
		if (gameController == null)
			gameController = GameController.Instance;

		gameController.EndGame_Event += EndGame;
		gameController.GameReset_Event += GameReseted;
	}

	private void OnDisable()
	{
		gameController.EndGame_Event -= EndGame;
		gameController.GameReset_Event -= GameReseted;
	}
	#endregion

	#region music Control Functions
	//The music has an introduction. So this method was created to play this intro and than start the music loop.
	private IEnumerator MusicLoopController()
	{
		yield return null;

		while (musicSource.isPlaying)
			yield return null;

		musicSource.clip = musicLoop;
		musicSource.Play();
		musicSource.loop = true;
	}

	//Sets the music to menu setup.
	private void GameReseted()
	{
		musicSource.clip = musicIntroduction;
		musicSource.Play();
		musicLoopCoroutine = StartCoroutine(MusicLoopController());
		musicSource.loop = false;
	}

	//Play the game over sound and stop the music.
	private void EndGame()
	{
		if (musicLoopCoroutine != null)
			StopCoroutine(musicLoopCoroutine);

		deathSource.Play();
		musicSource.Stop();
	}
	#endregion
}
