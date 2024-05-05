using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using UnityEngine.UI;

using UniRx;


namespace LobbyScene.UI
{
    public class LobbyUIFacade : BaseFacade, IRegistMonobehavior
    {
        Button _startBtn;

        Button _soundBtn;
        Button _vibrationBtn;
        Button _rankingloadBtn;

        #region Login
        LoginService _loginService;

        Button _guestBtn;
        Button _googleBtn;
        #endregion

        AudioService _audioService;


        CompositeDisposable _disposables;



        public void RegistBehavior(IContainerBuilder builder)
        {
            _startBtn = gameObject.GetHierachyPath<Button>(Hierarchy.GameStartButton);
            _soundBtn = gameObject.GetHierachyPath<Button>(Hierarchy.SoundControlButton);
            _vibrationBtn = gameObject.GetHierachyPath<Button>(Hierarchy.VibrationControlButton);
            _rankingloadBtn = gameObject.GetHierachyPath<Button>(Hierarchy.RankingLoadButton);

            _guestBtn = gameObject.GetHierachyPath<Button>(Hierarchy.GuestLoginButton);
            _googleBtn = gameObject.GetHierachyPath<Button>(Hierarchy.GoogleLoginButton);
        }


        public override void Initialize()
        {
            _audioService = _container.Resolve<AudioService>();

            _loginService = _container.Resolve<LoginService>();

            _disposables = new CompositeDisposable();

            var loader = _container.Resolve<SceneService>();

            var preferences = _container.Resolve<Preferences>();

            preferences.CheckSoundStatus(_soundBtn);
            preferences.CheckVibrationStatus(_vibrationBtn);


            _guestBtn.OnClickAsObservable().Subscribe(_ =>
            {
                _loginService.OnClickGuestLogin();
            }).AddTo(_disposables);


            _googleBtn.OnClickAsObservable().Subscribe(_ =>
            {
                //Add Later
            }).AddTo(_disposables);


            _startBtn.OnClickAsObservable().Subscribe(_ =>
            {
                _audioService.Play(AudioService.AudioResources.Button, AudioService.SoundType.SFX);
                loader.LoadScene(SceneName.GameScene).Forget();
            }).AddTo(_disposables);

            _startBtn.transform.parent.gameObject.SetActive(false);

            var pref = _container.Resolve<Preferences>();

            _soundBtn.OnClickAsObservable().Subscribe(_ =>
            {
                pref.SoundControl(_soundBtn);
                _audioService.Play(AudioService.AudioResources.Button, AudioService.SoundType.SFX);
            }).AddTo(_disposables);

            _vibrationBtn.OnClickAsObservable().Subscribe(_ =>
            {
                pref.VibrateControl(_vibrationBtn);
                _audioService.Play(AudioService.AudioResources.Button, AudioService.SoundType.SFX);
            }).AddTo(_disposables);

            _rankingloadBtn.OnClickAsObservable().Subscribe(_ =>
            {
                _audioService.Play(AudioService.AudioResources.Button, AudioService.SoundType.SFX);
                //Ranking Board Open
            }).AddTo(_disposables);


            Observable.EveryUpdate().Where(_ => _loginService.PLAYFABID.Length > 0)
                .Subscribe(_ =>
                {
                    _guestBtn.transform.parent.gameObject.SetActive(false);
                    _startBtn.transform.parent.gameObject.SetActive(true);
                }).AddTo(_disposables);
        }

        public override void Dispose()
        {
            _startBtn.onClick.RemoveAllListeners();
            _soundBtn.onClick.RemoveAllListeners();
            _vibrationBtn.onClick.RemoveAllListeners();
            _rankingloadBtn.onClick.RemoveAllListeners();
            _guestBtn.onClick.RemoveAllListeners();
            _googleBtn.onClick.RemoveAllListeners();

            _disposables?.Dispose();
            _disposables = null;
        }



        public static class Hierarchy
        {
            public static readonly string GameStartButton = "Background/Start/GameStart";

            public static readonly string SoundControlButton = "Background/Preferences/Sound";
            public static readonly string VibrationControlButton = "Background/Preferences/Vibration";
            public static readonly string RankingLoadButton = "Background/Preferences/RankingLoad";

            public static readonly string GuestLoginButton = "Background/Login/GuestLogin";
            public static readonly string GoogleLoginButton = "Background/Login/GoogleLogin";
        }
    }
}

