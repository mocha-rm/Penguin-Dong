using UnityEngine;
using System.Collections;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T _instance;
	private static bool appIsClosing = false;

	public static T Instance
	{
		get
		{
			if (appIsClosing)
			{
				return null;
			}
			if (_instance == null)
			{
				_instance = (T) FindObjectOfType(typeof(T));
				if (_instance == null)
				{
					GameObject newSingleton = new GameObject();
					_instance = newSingleton.AddComponent<T>();
					newSingleton.name = typeof(T).ToString();
				}
                DontDestroyOnLoad(_instance);
            }
			return _instance;
		}
	}

	public virtual void Start()
	{
		T[] allInstances = FindObjectsOfType(typeof(T)) as T[];
		
		if(allInstances.Length > 1)
		{
			foreach(T instanceToCheck in allInstances)
			{
				if(instanceToCheck != Instance)
				{
					// Destroy it now
					Destroy(instanceToCheck.gameObject);
				}
			}
		}
		DontDestroyOnLoad((T)FindObjectOfType(typeof(T)));
	}

	protected virtual void ClearAll()
    {
		if (_instance == this)
		{
			_instance = null;
		}

	}

	void OnDestroy()
    {
		ClearAll();
	}

    void OnApplicationQuit()
	{
		appIsClosing = true;
	}
}
