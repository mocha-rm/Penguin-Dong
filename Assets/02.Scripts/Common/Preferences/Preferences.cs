using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

using VibrationUtility;




public class Preferences : VContainer.Unity.IInitializable, IDisposable
{
    [Inject] IObjectResolver _container;

    AudioService _audioService;

    IDisposable _disposable;



    public void Initialize()
    {
        PlayerPrefs.SetInt(Constants.SoundPrefKey, 1);
        PlayerPrefs.SetInt(Constants.VibrationPrefKey, 1);
        _audioService = _container.Resolve<AudioService>();
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
            _audioService.SetVolume(AudioService.SoundType.BGM, 0f);
            _audioService.SetVolume(AudioService.SoundType.SFX, 0f);
        }
        else
        {
            PlayerPrefs.SetInt(Constants.SoundPrefKey, 1);
            btn.image.color = Color.white;
            _audioService.SetVolume(AudioService.SoundType.BGM, 1f);
            _audioService.SetVolume(AudioService.SoundType.SFX, 1f);
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

    public void CheckSoundStatus(Button btn)
    {
        if(PlayerPrefs.GetInt(Constants.SoundPrefKey) == 0)
        {
            btn.image.color = new Color(1f, 1f, 1f, 0.7f);
        }
        else
        {
            btn.image.color = Color.white;
        }
    }

    public void CheckVibrationStatus(Button btn)
    {
        if (PlayerPrefs.GetInt(Constants.VibrationPrefKey) == 0)
        {
            btn.image.color = new Color(1f, 1f, 1f, 0.3f);
        }
        else
        {
            btn.image.color = Color.white;
        }
    }




    public static class Constants
    {
        public static readonly string SoundPrefKey = "Sound";
        public static readonly string VibrationPrefKey = "Vibration";
    }
}
