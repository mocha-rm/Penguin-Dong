//using GooglePlayGames;
//using GooglePlayGames.BasicApi;
//using GooglePlayGames.BasicApi.Events;
//using GooglePlayGames.BasicApi.Nearby;
//using GooglePlayGames.BasicApi.SavedGame;
//using GooglePlayGames.Android;
//using GooglePlayGames.Editor;
//using GooglePlayGames.OurUtils;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.AuthenticationModels;
using PlayFab.Json;
using PlayFab.ProfilesModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;
using EntityKey = PlayFab.ProfilesModels.EntityKey;



public class LoginService : IInitializable, IDisposable
{
    [Inject] IObjectResolver _container;

    //GameObject _start;

    //Button _guest;
    //Button _google; 



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
        isLoginSuccess = false;

        //_guest.OnClickAsObservable().Subscribe(_ =>
        //{
        //    OnClickGuestLogin();
        //});

        //_google.OnClickAsObservable().Subscribe(_ =>
        //{
        //    //Google Login
        //    //GoogleActivate();
        //});
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
            PlayerPrefs.SetInt("FIRSTLOGIN", 1);
            PlayerPrefs.SetString("GUESTID", customId);
            OnLoginSuccess(result);
        }, error =>
        {
            PlayerPrefs.SetInt("FIRSTLOGIN", 0);
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

        customId = PlayerPrefs.GetString("GUESTID");

        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
        {
            CustomId = customId,
            CreateAccount = false
        }, result =>
        {
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
    }
}