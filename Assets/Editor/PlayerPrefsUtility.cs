using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerPrefsUtility : EditorWindow
{
    [MenuItem("Tools/PlayerPrefs Utility")]
    public static void ShowWindos()
    {
        GetWindow<PlayerPrefsUtility>("PlayerPrefs Utility");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Reset PlayerPrefs"))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("PlayerPrefs has been reset.");
        }
    }
}
