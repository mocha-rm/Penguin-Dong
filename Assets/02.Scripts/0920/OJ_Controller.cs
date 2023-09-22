using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class OJ_Controller : IInitializable
{
    [Inject] IObjectResolver _container;

    OJ_Facade _obj;

    public void Initialize()
    {
        _obj = _container.Resolve<OJ_Facade>();
    }

    public void CrashEvent()
    {
        _obj.CrashedAction();
    }
}
