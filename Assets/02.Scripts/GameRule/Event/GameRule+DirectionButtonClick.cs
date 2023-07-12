using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using MessagePipe;




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

public class DirectionButtonClick
{
    enum ButtonSpec { }
}
