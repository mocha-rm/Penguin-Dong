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

    IDisposable _disposable;

    public bool IsUserLoaded { get; private set; }

    //DATA
    public string NickName { get; private set; }
    public int TotalCoin { get; private set; } = 5000;
    public int BestScore { get; private set; }
    private const string TOTAL_USER_KEY = "TotalUsers";



    public void Initialize()
    {
        _container.Resolve<LoginService>();
        IsUserLoaded = false;
    }

    

    public void Dispose()
    {
        //게임 종료시 최종 데이터 저장
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


    public void GetUserData(string playfabID)
    {
        var request = new GetUserDataRequest() { PlayFabId = playfabID};
        PlayFabClientAPI.GetUserData(request, (result) =>
        {
            foreach (var eachData in result.Data)
            {
                string key = eachData.Key;

                if (key.Contains("NickName"))
                {
                    Debug.Log($"NicName : {eachData.Value.Value}");
                    NickName = eachData.Value.Value;
                }

                if (key.Contains("TotalCoin"))
                {
                    Debug.Log($"TotalCoin : {eachData.Value.Value}");
                    string totalCoinString = eachData.Value.Value;
                    if (int.TryParse(totalCoinString, out int totalCoinValue))
                    {
                        TotalCoin = totalCoinValue;
                    }
                    else
                    {
                        Debug.LogError("Parsing Fail");
                    }
                }

                if (key.Contains("BestScore"))
                {
                    Debug.Log($"BestScore : {eachData.Value.Value}");
                    string bestScoreString = eachData.Value.Value;
                    if (int.TryParse(bestScoreString, out int bestScoreValue))
                    {
                        BestScore = bestScoreValue;
                    }
                    else
                    {
                        Debug.LogError("Parsing Fail");
                    }
                }
            }

            IsUserLoaded = true;
        }, DisplayPlayfabError);
    }

    public void GetStatisticsAndUpdateNickName()
    {
        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest(),
            OnGetStatistics,
            error => Debug.LogError(error.GenerateErrorReport())
        );
    }

    private void OnGetStatistics(GetPlayerStatisticsResult result)
    {
        Debug.Log("Received the following Statistics:");
        int totalUsers = 0;
        foreach (var eachStat in result.Statistics)
        {
            if (eachStat.StatisticName == TOTAL_USER_KEY)
            {
                totalUsers = eachStat.Value;
                Debug.Log("Statistic (" + eachStat.StatisticName + "): " + eachStat.Value);
                break;
            }
        }

        // Increment total users count
        totalUsers++;
        string newNickName = "Guest" + totalUsers;
        NickName = newNickName;
        SetPlayerData("NickName", newNickName);

        // Update the total user count statistic in PlayFab
        var updateRequest = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = TOTAL_USER_KEY,
                    Value = totalUsers
                }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(updateRequest,
            success => { Debug.Log("Total user count updated successfully."); },
            error => { Debug.LogError("Failed to update total user count: " + error.GenerateErrorReport()); }
        );

        IsUserLoaded = true;
    }
}
