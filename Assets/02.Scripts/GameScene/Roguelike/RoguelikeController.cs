using System.Collections;
using System.Collections.Generic;
using TestScene;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using UniRx;
using GameScene;

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
        
    }


    public void ActivateRoguelike()
    {
        //Open Action
        _rogueFacade.SyncCurrentCoinforUI();
        _rogueFacade.InitializeRefreshFee();
        _rogueFacade.SetRefreshCostValue();
        _rogueFacade.OpenAction();
        _rogueFacade.SetItems();
    }

    public void DeactivateRoguelike()
    {
        //Close Action
        _rogueFacade.CloseAction();
    }

    public void Refresh()
    {
        _rogueFacade.SetItems();
        _rogueFacade.InceaseRefreshFee();
        _rogueFacade.SetRefreshCostValue();
        _rogueFacade.SyncCurrentCoinforUI();
    }

    public bool GetRefreshStatus()
    {
        return _rogueFacade.Model.IsRefresh.Value;
    }

    public void SetRefreshStatus()
    {
        _rogueFacade.Model.ReturnRefreshStatus();
    }

    public int GetCoin()
    {
        return _rogueFacade.useCoin;
    }

    public int GetRefreshFee()
    {
        return _rogueFacade.refreshFee;
    }

    public Item GetItem()
    {
        return _rogueFacade.GetPickedItem();
    }

    public void LotteryAction()
    {
        _rogueFacade.LotteryAction();
    }

    public void AddCoin(int add)
    {
        _rogueFacade.AddCoinValue(add);
        Utility.CustomLog.Log(_rogueFacade.useCoin.ToString());
    }

    public void MinusCoin(int minus)
    {
        _rogueFacade.MinusCoinValue(minus);
        Utility.CustomLog.Log(_rogueFacade.useCoin.ToString());
    }
}
