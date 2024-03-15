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
        ISubscriber<RoguelikeRefreshEvent> _rogueRefreshSub;

        IDisposable SubscribeRoguelikeRefreshEvent()
        {
            _rogueRefreshSub = _container.Resolve<ISubscriber<RoguelikeRefreshEvent>>();
            return _rogueRefreshSub.Subscribe(_ =>
            {
                //Refresh then what ?
                if (_roguelikeController.GetRefreshStatus() == true)
                {
                    _roguelikeController.SetRefreshStatus();
                }

                _model.Coin.Value -= _roguelikeController.RefreshCost;
                _roguelikeController.Refresh();
            });
        }
    }
}


namespace GameScene.Message
{
    class RoguelikeRefreshEvent
    {

    }
}



