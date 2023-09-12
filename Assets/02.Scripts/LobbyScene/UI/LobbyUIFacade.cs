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
    public class LobbyUIFacade : IInitializable, IDisposable
    {
        [Inject]
        IObjectResolver _container;

        RectTransform _canvasRoot;

        Button _singleGame;

        Button _quickGame;



        public void Initialize()
        {
            _canvasRoot = _container.Resolve<RectTransform>();
            var buttons = _canvasRoot.GetComponentsInChildren<Button>();

            var loader = _container.Resolve<SceneService>();

            _singleGame = buttons[0];
            _quickGame = buttons[1];

            _singleGame.OnClickAsObservable().Subscribe(_ =>
            {
                loader.LoadScene(SceneName.GameScene).Forget();
            });

            _quickGame.OnClickAsObservable().Subscribe(_ =>
            {
                loader.LoadScene(SceneName.GameScene).Forget();
            });
        }

        public void Dispose()
        {
        }
    }
}

