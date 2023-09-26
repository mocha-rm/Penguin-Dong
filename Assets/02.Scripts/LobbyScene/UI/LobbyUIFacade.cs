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




        public void RegistBehavior(IContainerBuilder builder)
        {
            _startBtn = gameObject.GetHierachyPath<Button>(Hierarchy.GameStartButton);
        }

        public override void Initialize()
        {
            var loader = _container.Resolve<SceneService>();

            _startBtn.OnClickAsObservable().Subscribe(_ =>
            {
                loader.LoadScene(SceneName.GameScene).Forget();
            });
        }

        public override void Dispose()
        {

        }



        public static class Constants
        {

        }

        public static class Hierarchy
        {
            public static readonly string GameStartButton = "Background/Start/GameStart";
        }
    }
}

