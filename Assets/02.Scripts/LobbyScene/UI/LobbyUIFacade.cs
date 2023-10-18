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
        #region Login
        /*Button _googleLoginBtn;
        Button _guestLoginBtn;*/
        #endregion

        Button _startBtn;

        Button _soundBtn;
        Button _vibrationBtn;


        CompositeDisposable _disposables;



        public void RegistBehavior(IContainerBuilder builder)
        {
            _startBtn = gameObject.GetHierachyPath<Button>(Hierarchy.GameStartButton);
            _soundBtn = gameObject.GetHierachyPath<Button>(Hierarchy.SoundControlButton);
            _vibrationBtn = gameObject.GetHierachyPath<Button>(Hierarchy.VibrationControlButton);
        }

        public override void Initialize()
        {
            _disposables = new CompositeDisposable();

            var loader = _container.Resolve<SceneService>();

            _startBtn.OnClickAsObservable().Subscribe(_ =>
            {
                loader.LoadScene(SceneName.GameScene).Forget();
            }).AddTo(_disposables);

            var pref = _container.Resolve<Preferences>();

            _soundBtn.OnClickAsObservable().Subscribe(_ =>
            {
                pref.SoundControl(_soundBtn);
            }).AddTo(_disposables);

            _vibrationBtn.OnClickAsObservable().Subscribe(_ =>
            {
                pref.VibrateControl(_vibrationBtn);
            }).AddTo(_disposables);
        }

        public override void Dispose()
        {
            _startBtn.onClick.RemoveAllListeners();
            _soundBtn.onClick.RemoveAllListeners();
            _vibrationBtn.onClick.RemoveAllListeners();

            _disposables?.Dispose();
            _disposables = null;
        }



        public static class Hierarchy
        {
            public static readonly string GameStartButton = "Background/Start/GameStart";

            public static readonly string SoundControlButton = "Background/Preferences/Sound";
            public static readonly string VibrationControlButton = "Background/Preferences/Vibration";
        }
    }
}

