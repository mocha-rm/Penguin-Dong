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
                if(_roguelikeController.GetRefreshStatus() == true)
                {
                    _roguelikeController.SetRefreshStatus();
                }
                else
                {
                    _model.Coin.Value -= _roguelikeController.DisburseCoin();
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

