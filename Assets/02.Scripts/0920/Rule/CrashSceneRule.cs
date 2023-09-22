using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VContainer;
using VContainer.Unity;

using MessagePipe;
using System;

public partial class CrashSceneRule : IInitializable, IDisposable
{
    [Inject] IObjectResolver _container;

    //Controller
    PL_Controller _plController;
    OJ_Controller _ojController;

    //Sub
    IDisposable _disposable;
    

    public void Initialize()
    {
        _plController = _container.Resolve<PL_Controller>();
        _ojController = _container.Resolve<OJ_Controller>();

        var bag = DisposableBag.CreateBuilder();
        SubscribeCrashEvent().AddTo(bag);

        _disposable = bag.Build();
    }
    public void Dispose()
    {
        _disposable?.Dispose();
    }
}
