using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

using GameScene.Player;
using GameScene.UI;
using GameScene.Icicle;
using GameScene.Rule;


namespace GameScene
{
    public class BloC : IInitializable
    {
        public IGameModelObserver GameModelObservable;

        [Inject] IObjectResolver _container;

        public void Initialize()
        {
            GameModelObservable = _container.Resolve<GameRule>().Model;
        }
    }
}
