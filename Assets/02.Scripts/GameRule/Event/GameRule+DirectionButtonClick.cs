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
            //�÷��̾� �� ����?
            _playerController.SetdirectionChange();
            Debug.Log("Turn");
        });
    }
}

public class DirectionButtonClick
{
    enum ButtonSpec { }
}
