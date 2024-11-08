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

            try
            {
                _bloc = _container.Resolve<BLOC>();
                _player = _container.Resolve<PlayerFacade>();

            }
            catch(Exception e)
            {
                Debug.Log(e.Message);
                Debug.Log("PlayerController Init Error");
            }
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

        public void IdleAction()
        {
            _player.IdleAction();
        }

        

        public void SetdirectionChange()
        {
            _player.DirectionControlAction();
        }

        public void SetPlayerInvulnerable()
        {
            _player.IsInvulnerable();
        }

        public void SetPlayerInvulnerable(bool isActivate)
        {
            _player.InvulStatus_Roguelike(isActivate);
        }

        public bool IsPlayerInvul()
        {
            return _player.Model.Isinvul.Value;
        }

        //Shield Section

        public bool IsPlayerHasShield()
        {
            return _player.Model.IsShieldHave.Value;
        }

        public void ShieldActivate(int count)
        {
            _player.ShieldActivate(count);
        }

        public void ShieldDamage()
        {
            _player.ShieldDamage();
        }
    }
}
