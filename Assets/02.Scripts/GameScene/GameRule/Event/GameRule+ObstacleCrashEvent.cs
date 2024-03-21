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

        float _damage = 20.0f;

        IDisposable SubscribeObstacleCrashEvent()
        {
            _colObstacleSub = _container.Resolve<ISubscriber<ObstacleCrashEvent>>();
            return _colObstacleSub.Subscribe(data =>
            {
                _audioService.Play(AudioService.AudioResources.Hitted, AudioService.SoundType.SFX);
                _model.Life.Value -= _damage - _model.Abilities["SkinUpgrade"];
                _playerController.SetPlayerInvulnerable();
                _uiController.LifeUIAction(_damage - _model.Abilities["SkinUpgrade"], _model.Abilities["Heart"]);
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

