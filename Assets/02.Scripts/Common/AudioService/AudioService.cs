using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using UniRx;
using MoreMountains.Feedbacks;


public class AudioService : IInitializable, IDisposable
{
    public enum SoundType
    {
        BGM,
        SFX,
        End //for counting
    }

    [Inject] IObjectResolver _container;

    GameObject _soundObject;

    AudioSource[] _audioSources = new AudioSource[(int)SoundType.End];
    float[] _volumes = new float[(int)SoundType.End];

    IDisposable _disposable;


    public void Initialize()
    {
        if (_soundObject == null)
        {
            _soundObject = new GameObject() { name = "AudioService" };
            UnityEngine.Object.DontDestroyOnLoad(_soundObject);

            string[] soundNames = System.Enum.GetNames(typeof(SoundType));
            for(int i = 0; i < soundNames.Length - 1; i++)
            {
                GameObject obj = new GameObject { name = soundNames[i] };
                _audioSources[i] = obj.AddComponent<AudioSource>();

                _audioSources[i].volume = _volumes[i] = 1.0f;
                obj.transform.parent = _soundObject.transform;
            }
        }
        else
        {
            Debug.LogError($"Audio Service Make Twice!! Please Check");
        }
    }

    
    public void SetVolume(SoundType type, float volume)
    {
        if(type >= SoundType.End)
        {
            Debug.LogWarning($"{type} is Over MaxCount");
            return;
        }

        _volumes[(int)type] = volume;
        _audioSources[(int)type].volume = volume;
    }

    public void Stop(SoundType type)
    {
        if(type < SoundType.End)
        {
            _audioSources[(int)type].Stop();
        }
        else
        {
            Debug.LogError($"{type} is Over MaxCount");
        }
    }


    public void Play()
    {

    }

    public void Dispose()
    {
        foreach(AudioSource audioSource in _audioSources)
        {
            if(audioSource != null)
            {
                audioSource.Stop();
                audioSource.clip = null;
            }
        }

        _audioSources = null;

        _disposable?.Dispose();
        _disposable = null;
    }
}
