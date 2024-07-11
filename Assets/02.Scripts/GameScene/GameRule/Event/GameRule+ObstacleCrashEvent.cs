using System;
using VContainer;
using MessagePipe;
using UnityEngine.TextCore.Text;
using GameScene.Player;
using GameScene.Message;
using UnityEngine;
using Unity.VisualScripting;

namespace GameScene.Rule
{
    public partial class GameRule
    {
        ISubscriber<ObstacleCrashEvent> _colObstacleSub;

        float _damage = 100.0f;

        IDisposable SubscribeObstacleCrashEvent()
        {
            _colObstacleSub = _container.Resolve<ISubscriber<ObstacleCrashEvent>>();
            return _colObstacleSub.Subscribe(data =>
            {
                if (_playerController.IsPlayerHasShield())
                {
                    _playerController.ShieldDamage();
                    _audioService.Play(AudioService.AudioResources.Shield);
                    
                }
                else
                {
                    _audioService.Play(AudioService.AudioResources.Hitted);
                    _model.Life.Value -= _damage - _model.Abilities[AbilityNames.SkinUpgrade.ToString()];
                    _uiController.LifeUIAction(_damage - _model.Abilities[AbilityNames.SkinUpgrade.ToString()], _model.Abilities[AbilityNames.Heart.ToString()]);
                    _playerController.SetPlayerInvulnerable();
                }
            });
        }
    }
}


namespace GameScene.Message
{
    public class ObstacleCrashEvent
    {

    }
}

