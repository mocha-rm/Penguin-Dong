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
        ISubscriber<RoguelikePayEvent> _roguePaySub;

        IDisposable SubscribeRoguelikePayEvent()
        {
            _roguePaySub = _container.Resolve<ISubscriber<RoguelikePayEvent>>();
            return _roguePaySub.Subscribe(_ =>
            {
                if(_model.GameState.Value != GameState.GameOver)
                {
                    _model.GameState.Value = GameState.Playing;
                    _model.Coin.Value -= _roguelikeController.GetItemCost();
                    _roguelikeController.DeactivateRoguelike();
                    _playerController.SetPlayerInvulnerable(false);
                    CoreTask(levelGuage).Forget();
                }
            });
        }
    }
}

namespace GameScene.Message
{
    class RoguelikePayEvent
    {
        
    }
}

