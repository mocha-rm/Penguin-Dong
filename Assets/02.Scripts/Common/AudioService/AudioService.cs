using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using UniRx;
using MoreMountains.Feedbacks;
using static UnityEngine.RuleTile.TilingRuleOutput;
using static AudioService;


public class AudioService : IInitializable, IDisposable
{
    public enum SoundType
    {
        BGM,
        SFX,
        OBJ,
        End //for counting
    }

    public enum AudioResources //If resources more added you have to edit code again.. so this is not a good idea..
    {
        #region BGM
        //ASMR Background
        

        #endregion

        #region SFX
        Button,
        Count,
        GO,
        Hitted,
        Shield, //Shield Hitted Sound

        //Object Sounds
        
        #endregion
    }

    [Inject] IObjectResolver _container;

    GameObject _soundObject;

    AudioSource[] _audioSources = new AudioSource[(int)SoundType.End];
    float[] _volumes = new float[(int)SoundType.End];

    Dictionary<string, AudioClip> _sfxDic = new Dictionary<string, AudioClip>();

    List<AudioClip> _bgmClips = new List<AudioClip>();
    List<AudioClip> _objClips = new List<AudioClip>();

    IDisposable _disposable;



    public void Initialize()
    {
        LoadSoundResource();


        if (_soundObject == null)
        {
            _soundObject = new GameObject() { name = "AudioService" };
            UnityEngine.Object.DontDestroyOnLoad(_soundObject);

            string[] soundNames = System.Enum.GetNames(typeof(SoundType));
            for (int i = 0; i < soundNames.Length - 1; i++)
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

        _audioSources[(int)SoundType.OBJ].volume = 0.1f;
        SetRandomObjectClip();
    }

    private void LoadSoundResource()
    {
        AudioClip[] bgm = Resources.LoadAll<AudioClip>("Sounds/BGM");
        AudioClip[] sfx = Resources.LoadAll<AudioClip>("Sounds/SFX");
        AudioClip[] obj = Resources.LoadAll<AudioClip>("Sounds/Object");

        for (int i = 0; i < bgm.Length; i++)
        {
            _bgmClips.Add(bgm[i]);
        }

        for (int i = 0; i < sfx.Length; i++)
        {
            _sfxDic[sfx[i].name] = sfx[i];
        }

        for (int i = 0; i < obj.Length; i++)
        {
            _objClips.Add(obj[i]);
        }
    }


    public void SetVolume(SoundType type, float volume)
    {
        if (type >= SoundType.End)
        {
            Debug.LogWarning($"{type} is Over MaxCount");
            return;
        }

        _volumes[(int)type] = volume;
        _audioSources[(int)type].volume = volume;
    }

    public void Stop(SoundType type)
    {
        if (type < SoundType.End)
        {
            _audioSources[(int)type].Stop();
        }
        else
        {
            Debug.LogError($"{type} is Over MaxCount");
        }
    }


    public void Play(AudioResources resourceName)
    {
        //SFX
        _audioSources[(int)SoundType.SFX].clip = _sfxDic[resourceName.ToString()];
        _audioSources[(int)SoundType.SFX].Play();
    }


    //Call when Level Up
    public void RandomBGMPlay()
    {
        //BGM
        AudioClip bgm = _bgmClips[UnityEngine.Random.Range(0, _bgmClips.Count)];
        _audioSources[(int)SoundType.BGM].clip = bgm;
        _audioSources[(int)SoundType.BGM].Play();
    }

    public void SetRandomObjectClip()
    {
        //OBJ
        AudioClip obj = _objClips[UnityEngine.Random.Range(0, _objClips.Count)];
        _audioSources[(int)SoundType.OBJ].clip = obj;
    }

    public void RandomObjectPlay() => _audioSources[(int)SoundType.OBJ].PlayOneShot(_audioSources[(int)SoundType.OBJ].clip);


    public void Dispose()
    {
        foreach (AudioSource audioSource in _audioSources)
        {
            if (audioSource != null)
            {
                audioSource.Stop();
                audioSource.clip = null;
            }
        }

        _audioSources = null;

        _disposable?.Dispose();
        _disposable = null;
    }


    //Seperate Stereo

    private float panRange = 10f;

    public void StereoSetting(Vector3 pos)
    {
        // 오브젝트의 x 위치를 기준으로 panStereo 값을 설정
        float xPos = pos.x;

        // panRange를 기준으로 -1에서 1 사이로 pan 값을 설정
        float panValue = Mathf.Clamp(xPos / panRange, -1f, 1f);

        _audioSources[(int)SoundType.OBJ].panStereo = panValue;
    }
}
