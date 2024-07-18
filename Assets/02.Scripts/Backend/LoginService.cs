using PlayFab;
using PlayFab.ClientModels;
using PlayFab.AuthenticationModels;
using PlayFab.Json;
using PlayFab.ProfilesModels;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;
using Cysharp.Threading;
using System.Threading;
using Cysharp.Threading.Tasks;

using EntityKey = PlayFab.ProfilesModels.EntityKey;

public enum LoginStatus
{
    FIRST,
    USED,
    NONE
}



public class LoginService : IInitializable, IDisposable
{
    [Inject] IObjectResolver _container;

    private string savePath;

    DBService _dbService;

    static string customId = "";
    static string playfabId = "";
    public string PLAYFABID { get => playfabId; } 

    private string entityId;
    private string entityType;

    public bool IsLoginSuccess { get => isLoginSuccess; }
    private bool isLoginSuccess;

    IDisposable _disposable;



    public void Initialize()
    {
        _dbService = _container.Resolve<DBService>();

        isLoginSuccess = false;

        savePath = Application.persistentDataPath + "/guest_id.txt";

        if (File.Exists(savePath))
        {
            customId = File.ReadAllText(savePath);
            LoginGuestId();
        }       
    }

    public void Dispose()
    {
        _disposable?.Dispose();
        _disposable = null;
    }



    #region GuestLogin

    public void OnClickGuestLogin() //게스트 로그인 버튼
    {
        if (string.IsNullOrEmpty(customId))
            CreateGuestId();
        else
            LoginGuestId();
    }


    private void CreateGuestId() //저장된 아이디가 없을 경우 새로 생성
    {
        customId = GetRandomPassword(16);

        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
        {
            CustomId = customId,
            CreateAccount = true
        }, result =>
        {
            UpdatePlayerData().Forget();
            OnLoginSuccess(result);
            File.WriteAllText(savePath, customId);
        }, error =>
        {           
            Debug.LogError("Login Fail - Guest");
        });

    }


    private string GetRandomPassword(int _totLen) //랜덤한 16자리 id 생성
    {
        string input = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var chars = Enumerable.Range(0, _totLen)
            .Select(x => input[UnityEngine.Random.Range(0, input.Length)]);
        return new string(chars.ToArray());
    }


    private void LoginGuestId() //게스트 로그인
    {
        Debug.Log("Guest Login");

        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
        {
            CustomId = customId,
            CreateAccount = false
        }, result =>
        {
            Debug.Log("Normal Login - Guest");
            _dbService.GetUserData(playfabId);
            OnLoginSuccess(result);
        }, error =>
        {
            Debug.LogError("Login Fail - Guest");
        });
    }

    #endregion

    public void OnLoginSuccess(LoginResult result) //로그인 결과
    {
        Debug.Log("Playfab Login Success");
        playfabId = result.PlayFabId;
        entityId = result.EntityToken.Entity.Id;
        entityType = result.EntityToken.Entity.Type;
        isLoginSuccess = true;
    }



    private async UniTaskVoid UpdatePlayerData()
    {
        _dbService.UpdateUsernameAndCount();
        await UniTask.Delay(System.TimeSpan.FromMilliseconds(500));
  
        _dbService.SetPlayerData("BestScore", "0");
        await UniTask.WaitUntil(() => _dbService.NickName != null);
        //await UniTask.Delay(System.TimeSpan.FromMilliseconds(500));

        SetDisplayName(_dbService.NickName);
    }



    //Display Name
    void SetDisplayName(string displayName)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = displayName
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdateSuccess, OnDisplayNameUpdateFailure);
    }

    void OnDisplayNameUpdateSuccess(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("DisplayName updated to: " + result.DisplayName);
    }

    void OnDisplayNameUpdateFailure(PlayFabError error)
    {
        Debug.LogError("DisplayName update failed: " + error.GenerateErrorReport());
    }
}