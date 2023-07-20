using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using UniRx;



namespace GameScene.UI
{
    public class InGameUIController : IInitializable, IDisposable
    {
        [Inject] IObjectResolver _container;
        BloC _bloc;

        NormalUIFacade _ingameUI;
        //EndUIFacade

        CompositeDisposable _disposables;


        public void Initialize()
        {
            _disposables = new CompositeDisposable();
            _bloc = _container.Resolve<BloC>();

            _ingameUI = _container.Resolve<NormalUIFacade>();
        }


        public void PopGameOverPanel()
        {
            Debug.Log($"Game is Over..\n");

        }

        public void Dispose()
        {
            _disposables?.Dispose();
            _disposables?.Clear();
            _disposables = null;
        }
    }
}