using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VContainer;
using MessagePipe;
using UnityEngine;

namespace GameScene.Rule
{
    using Message;

    public partial class GameRule
    {
        ISubscriber<DirectionButtonClick> _playerTurnSub;

        IDisposable SubscribePlayerTurnEvent()
        {
            _playerTurnSub = _container.Resolve<ISubscriber<DirectionButtonClick>>();
            return _playerTurnSub.Subscribe(dta =>
            {
                if(_model.GameState.Value == GameState.Playing)
                {
                    _playerController.SetdirectionChange();
                }
            });
        }
    }

}

namespace GameScene.Message
{
    public class DirectionButtonClick
    {
        enum ButtonSpec { }
    }
}

