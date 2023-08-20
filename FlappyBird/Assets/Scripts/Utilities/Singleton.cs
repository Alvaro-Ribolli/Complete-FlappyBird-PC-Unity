using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T: Component
{
	//This is a generic Singleton script.
	//This singleton is not prepared to DontDestroyOnLoad

    private static T _instance;
    public static T Instance
	{
		get
		{
			//When some script calls for the singleton, and the instance is null
			if(_instance == null)
			{
				//Try to find an object with this singleton
				var objs = FindObjectsByType<T>(FindObjectsSortMode.InstanceID);
				if(objs.Length > 0)
					_instance = objs[0];
				if(objs.Length > 1)
				{
					Debug.LogError("There are more than one " + typeof(T).Name + " in the scene");
				}

				//If no object was find, than create one
				if(_instance == null)
				{
					GameObject obj = new GameObject();
					obj.hideFlags = HideFlags.HideAndDontSave;
					_instance = obj.AddComponent<T>();
				}

			}

			return _instance;
		}
	}


}
