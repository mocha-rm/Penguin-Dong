using GameScene.Rule;
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
        [Inject] IObjectResolver _container;

        BLOC _bloc;
        PlayerFacade _player;


        public void Initialize()
        {
            _bloc = _container.Resolve<BLOC>();
            _player = _container.Resolve<PlayerFacade>();
        }

        public void Dispose()
        {
            
        }

        public void FixedTick()
        {
            if (_bloc.GameModel.GameStateProperty.Value == GameState.Playing)
            {
                _player.MoveAction();
            }
        }

        public void SetdirectionChange()
        {
            _player.DirectionControlAction();
        }

        public bool IsPlayerInvul()
        {
            return _player.Model.Isinvul.Value;
        }
    }
}
