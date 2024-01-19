using System.Collections;
using System.Collections.Generic;
using TestScene;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using UniRx;

using System;

public class RoguelikeController : IInitializable, IDisposable
{
    //communicate with upper class like model

    //Roguelike System with VContainer..

    //controller for send information about item using moments
    //update there status in realtime
    //and clear it if game is done   

    [Inject] IObjectResolver _container;


    RoguelikeFacade _rogueFacade = null;


    public void Initialize()
    {
        try
        {
            _rogueFacade = _container.Resolve<RoguelikeFacade>();
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log("RoguelikeController Init Error");
        }
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }


    public void Buy()
    {
        
    }
}
