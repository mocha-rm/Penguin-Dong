using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using UniRx;

public class AudioService : IInitializable, IDisposable
{
    [Inject] IObjectResolver _container;

    public void Dispose()
    {
        
    }

    public void Initialize()
    {
        
    }
}
