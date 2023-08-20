using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using DG.Tweening;

public class PauseController : Singleton<PauseController>
{
	[Header("Component")]
	[SerializeField] private AudioMixer soundMixer;
	[SerializeField] private Image pauseImage;
	[SerializeField] private CanvasGroup pauseGroup;
	[SerializeField] private Slider sfxSlider;
	[SerializeField] private Slider musicSlider;

	[Header("Images")]
	[SerializeField] private Sprite pauseButtonSprite;
	[SerializeField] private Sprite playButtonSprite;

	private bool paused;
	private Vector3 buttonDefaultPos;
	private const string SFX_VOLUME_PARAMETER = "SfxVolume";
	private const string MUSIC_VOLUME_PARAMETER = "MusicVolume";

	private Tweener pauseButtonTween;

	private GameController gamecontroller;

	public delegate void PauseEvents();
	public event PauseEvents GamePaused_Event;
	public event PauseEvents GameUnpaused_Event;

	#region Unity Functions
	private void Start()
	{
		buttonDefaultPos = pauseImage.transform.position;

		//Load previous value of the volume
		float sfxVolume = SaveSystem.GetSavedSoundVolume(VolumeParameter.sfx);
		sfxSlider.value = sfxVolume;
		
		float musicVolume = SaveSystem.GetSavedSoundVolume(VolumeParameter.music);
		musicSlider.value = musicVolume;

		//Update slider values according the loaded volume
		SFXSliderValue(sfxVolume);
		MusicSliderValue(musicVolume);
	}

	private void OnEnable()
	{
		if (gamecontroller == null)
			gamecontroller = GameController.Instance;

		gamecontroller.EndGame_Event += GameEnded;
		gamecontroller.GameReset_Event += GameRestarted;
	}

	private void OnDisable()
	{
		gamecontroller.EndGame_Event -= GameEnded;
		gamecontroller.GameReset_Event -= GameRestarted;
	}

	private void Update()
	{
		if(gamecontroller.gameState != gameStates.score)
		{
			if (Input.GetKeyDown(KeyCode.P))
				PauseButton();
		}
	}
	#endregion

	#region Events Functions
	//When the player dies, the pause button hides.
	private void GameEnded()
	{
		pauseButtonTween = pauseImage.transform.DOMoveX(buttonDefaultPos.x - 100, .2f).SetEase(Ease.InSine);
	}

	//When back to menu, the button come back again
	private void GameRestarted()
	{
		if (pauseButtonTween.IsActive())
			pauseButtonTween.Kill();

		pauseButtonTween = pauseImage.transform.DOMoveX(buttonDefaultPos.x, .2f);
	}
	#endregion

	//Function called by the input or the canvas button
	public void PauseButton()
	{
		//Only pauses if the game is not in the score state
		if (gamecontroller.gameState != gameStates.score)
		{
			paused = !paused;

			if (paused)
			{
				//Show the pause menu
				pauseImage.sprite = playButtonSprite;
				pauseGroup.interactable = true;
				pauseGroup.DOFade(1, .2f).SetEase(Ease.OutSine).SetUpdate(UpdateType.Normal, true);
				GamePaused_Event?.Invoke();
			}
			else
			{
				//Hide the pause menu
				//Save the volumes
				pauseImage.sprite = pauseButtonSprite;
				pauseGroup.interactable = false;
				pauseGroup.DOFade(0, .2f).SetEase(Ease.InSine).SetUpdate(UpdateType.Normal, true);
				SaveSystem.RegisterNewVolume(VolumeParameter.music, musicSlider.value);
				SaveSystem.RegisterNewVolume(VolumeParameter.sfx, sfxSlider.value);
				SaveSystem.SavePlayerData();
				GameUnpaused_Event?.Invoke();
			}
		}
	}

	#region Sliders Function
	//Called by the slider component to update the volume value
	public void SFXSliderValue(float _value)
	{
		soundMixer.SetFloat(SFX_VOLUME_PARAMETER, ConvertSoundValue(_value));
	}

	//Called by the slider component to update the volume value
	public void MusicSliderValue(float _value)
	{
		soundMixer.SetFloat(MUSIC_VOLUME_PARAMETER, ConvertSoundValue(_value));
	}

	//Convert the slider value to the mixer value
	private float ConvertSoundValue(float _value)
	{
		//Invert the slider value
		//Ex: if the slider is 0.3, then turn it to 0.7
		float newValue = (_value - 1) * -1;

		//Multiply the value to transform in Dbs and apply to the mixer.
		newValue = newValue * -40;
		return newValue;
	}
	#endregion
}
