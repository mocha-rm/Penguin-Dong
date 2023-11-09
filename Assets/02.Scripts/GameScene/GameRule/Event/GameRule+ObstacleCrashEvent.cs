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

        int lifeImg_order = 2;

        IDisposable SubscribeObstacleCrashEvent()
        {
            _colObstacleSub = _container.Resolve<ISubscriber<ObstacleCrashEvent>>();
            return _colObstacleSub.Subscribe(data =>
            {
                _audioService.Play(AudioService.AudioResources.Hitted, AudioService.SoundType.SFX);
                _model.Life.Value -= 1;
                _playerController.SetPlayerInvulnerable();
                _uiController.LifeUIAction(lifeImg_order--);
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

