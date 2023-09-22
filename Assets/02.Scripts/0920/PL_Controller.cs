using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;


public class PL_Controller : IInitializable
{
    [Inject] IObjectResolver _container;

    PL_Facade _pl;

    public void Initialize()
    {
        _pl = _container.Resolve<PL_Facade>();
    }

    public void CrashEvent()
    {
        _pl.CrashedAction();
    }
}
