using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveSystem
{
	private const string PLAYERPREFS_KEY = "PlayerData";

	private static PlayerData currentData;

	#region Save and Get Score

	//Return the best score saved
	public static int GetSavedBestScore()
	{
		if (currentData == null)
			currentData = LoadPlayerData();

		return currentData.bestScore;
	}

	//Save new best score
	public static void SaveBestScore(int _value)
	{
		currentData.bestScore = _value;
		SavePlayerData();
	}
	#endregion

	#region Sound Functions
	//Get saved volume
	public static float GetSavedSoundVolume(VolumeParameter _whichOne)
	{
		float _value = 1;

		if (currentData == null)
			currentData = LoadPlayerData();

		switch (_whichOne)
		{
			case VolumeParameter.music:
				_value = currentData.musicVolume;
				break;
			case VolumeParameter.sfx:
				_value = currentData.sfxVolume;
				break;
		}

		return _value;
	}

	//Register volume in the current opened player data
	public static void RegisterNewVolume(VolumeParameter _whichOne, float _value)
	{
		switch (_whichOne)
		{
			case VolumeParameter.music:
				currentData.musicVolume = _value;
				break;
			case VolumeParameter.sfx:
				currentData.sfxVolume = _value;
				break;
		}
	}
	#endregion

	#region Save and Load Player Data

	//Called by other scripts to save current data
	public static void SavePlayerData()
	{
		SavePlayerData(currentData);
	}

	//Save the data
	private static void SavePlayerData(PlayerData newData)
	{
		PlayerPrefs.SetString(PLAYERPREFS_KEY, JsonUtility.ToJson(newData));
		PlayerPrefs.Save();

		Debug.Log("New data Saved!");
	}

	//Load data
	private static PlayerData LoadPlayerData()
	{
		PlayerData loadedData;

		if (PlayerPrefs.HasKey(PLAYERPREFS_KEY))
		{
			string json = PlayerPrefs.GetString(PLAYERPREFS_KEY);

			if (json != string.Empty)
				loadedData = JsonUtility.FromJson<PlayerData>(json);
			else
				loadedData = new PlayerData();
		}
		else
			loadedData = new PlayerData();

		Debug.Log("Data Loaded!");
		return loadedData;
	}

	#endregion
}

public enum VolumeParameter
{
	sfx = 0,
	music = 1
}

public class PlayerData
{
	public int bestScore = 0;
	public float sfxVolume = 1;
	public float musicVolume = 1;
}
