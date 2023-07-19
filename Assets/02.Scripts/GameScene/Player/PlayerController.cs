using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;


namespace GameScene.Player
{
    public class PlayerController : IInitializable, IDisposable, IFixedTickable
    {
        //_player�� ��Ʈ�� �ϴ� ��ü

        [Inject] IObjectResolver _container;

        PlayerFacade _player;


        public void Initialize()
        {
            _player = _container.Resolve<PlayerFacade>();
        }

        public void Dispose()
        {

        }

        public void FixedTick()
        {
            if (Application.isPlaying)
            {
                _player.MoveAction();
            }
        }

        public void SetdirectionChange()
        {
            _player.DirectionControlAction();
        }
    }
}
