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

    public int RefreshCost { get; private set; }
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

        RefreshCost = 100;
    }

    public void Dispose()
    {
        RefreshCost = 0;
    }


    public void ActiveRoguelike()
    {
        //Open Action
        _rogueFacade.OpenAction();
    }

    public void Refresh()
    {
        RefreshCost *= 2;
        _rogueFacade.SetItems();
    }

    public bool GetRefreshStatus()
    {
        return _rogueFacade.Model.IsRefresh.Value;
    }

    public void SetRefreshStatus()
    {
        _rogueFacade.Model.ReturnRefreshStatus();
    }


    public int DisburseCoin()
    {
        return _rogueFacade.GetCoinInfo();
    }

    public float ApplyItemValue()
    {
        return _rogueFacade.GetItemValue();
    }
}
