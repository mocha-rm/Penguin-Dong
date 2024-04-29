using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using UniRx;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using PlayFab.ProfilesModels;


public class DBService : IInitializable, IDisposable
{
    [Inject] IObjectResolver _container;

    LoginService _loginService;

    IDisposable _disposable;



    public void Initialize()
    {
        _container.Resolve<LoginService>();
    }

    public void Dispose()
    {
        _disposable?.Dispose();
        _disposable = null;
    }


    private void DisplayPlayfabError(PlayFabError error) => Debug.LogError("error : " + error.GenerateErrorReport());

    public void SetUserData(Dictionary<string, string> data)
    {
        var request = new UpdateUserDataRequest() { Data = data, Permission = UserDataPermission.Public };
        try
        {
            PlayFabClientAPI.UpdateUserData(request, (result) =>
            {
                Debug.Log("Update Player Data!");

            }, DisplayPlayfabError);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    public void SetPlayerData(string str1, string str2)
    {
        Dictionary<string, string> data = new Dictionary<string, string>
        {
            { str1, str2 }
        };

        SetUserData(data);
    }


    public void GetUserData()
    {
        var request = new GetUserDataRequest() { PlayFabId = _loginService .PLAYFABID};
        PlayFabClientAPI.GetUserData(request, (result) =>
        {
            foreach (var eachData in result.Data)
            {
                string key = eachData.Key;

                if (key.Contains("TotalCoin"))
                {
                    
                }

                if (key.Contains("BsetScore"))
                {

                }

            }

        }, DisplayPlayfabError);
    }
}
