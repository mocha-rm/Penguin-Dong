using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using UniRx;

using MessagePipe;
using System;

public partial class CrashSceneRule
{
    ISubscriber<CrashEvent> _crashEvent;

    IDisposable SubscribeCrashEvent()
    {
        _crashEvent = _container.Resolve<ISubscriber<CrashEvent>>();

        return _crashEvent.Subscribe(data =>
        {
            _plController.CrashEvent();
            _ojController.CrashEvent();
        });
    }
}

public class CrashEvent
{
    public CrashEvent()
    {
        
    }
}
