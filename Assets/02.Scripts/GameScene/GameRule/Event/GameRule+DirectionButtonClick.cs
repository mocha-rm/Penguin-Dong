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
                //플레이어 턴 동작?
                _playerController.SetdirectionChange();
                Debug.Log("Turn");
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

