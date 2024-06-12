using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using MessagePipe;

namespace GameScene.Rule
{
    using Message;
    using System;


    public partial class GameRule
    {
        ISubscriber<RoguelikeSkipEvent> _rogueSkipSub;

        IDisposable SubscribeRoguelikeSkipEvent()
        {
            _rogueSkipSub = _container.Resolve<ISubscriber<RoguelikeSkipEvent>>();
            return _rogueSkipSub.Subscribe(_ =>
            {
                //Skip Event
                if (_model.GameState.Value != GameState.GameOver)
                {
                    _model.GameState.Value = GameState.Playing;
                    _roguelikeController.DeactivateRoguelike();
                    _playerController.SetPlayerInvulnerable(false);
                    _audioService.RandomBGMPlay();
                    CoreTask(levelGuage).Forget();
                }
            });
        }
    }
}

namespace GameScene.Message
{
    class RoguelikeSkipEvent
    {

    }
}

