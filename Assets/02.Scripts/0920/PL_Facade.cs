
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

using UniRx;
using UniRx.Triggers;

public class PL_Facade : BaseFacade, IRegistMonobehavior
{

    public void RegistBehavior(IContainerBuilder builder)
    {

    }

    public override void Initialize()
    {

    }
    public override void Dispose()
    {

    }


    public void CrashedAction()
    {
        Debug.Log("Player Crash Action");
    }
}
