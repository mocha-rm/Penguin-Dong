using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

using VibrationUtility;




public class Preferences : IInitializable, IDisposable
{
    [Inject] IObjectResolver _container;

    IDisposable _disposable;



    public void Initialize()
    {
        PlayerPrefs.SetInt(Constants.SoundPrefKey, 1);
        PlayerPrefs.SetInt(Constants.VibrationPrefKey, 1);
    }

    public void Dispose()
    {
        _disposable?.Dispose();
        _disposable = null;
    }

    public void SoundControl(Button btn)
    {
        if(PlayerPrefs.GetInt(Constants.SoundPrefKey) == 1)
        {
            PlayerPrefs.SetInt(Constants.SoundPrefKey, 0);
            btn.image.color = new Color(1f, 1f, 1f, 0.7f);
        }
        else
        {
            PlayerPrefs.SetInt(Constants.SoundPrefKey, 1);
            btn.image.color = Color.white;
        }
    }

    public void VibrateControl(Button btn)
    {
        if (PlayerPrefs.GetInt(Constants.VibrationPrefKey) == 1)
        {
            PlayerPrefs.SetInt(Constants.VibrationPrefKey, 0);
            btn.image.color = new Color(1f, 1f, 1f, 0.3f);
        }
        else
        {
            PlayerPrefs.SetInt(Constants.VibrationPrefKey, 1);
            btn.image.color = Color.white;
            VibrationUtil.Vibrate(VibrationType.Rigid, 1f);
        }
    }

    public void DebugStatus()
    {
        Debug.Log($"Sound is {PlayerPrefs.GetInt(Constants.SoundPrefKey, 1)}");
        Debug.Log($"Vibration is {PlayerPrefs.GetInt(Constants.VibrationPrefKey, 1)}");
    }



    public static class Constants
    {
        public static readonly string SoundPrefKey = "Sound";
        public static readonly string VibrationPrefKey = "Vibration";
    }
}
