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
    public string NickName { get; private set; } = null;
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

    public void UpdateUsernameAndCount()
    {
        GetCurrentUserCountAndSave();
    }

    private void GetCurrentUserCountAndSave()
    {
        PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest
        {
            StatisticName = TOTAL_USER_KEY,
            StartPosition = 0,
            MaxResultsCount = 1
        }, result => OnLeaderboardDataReceived(result), FailureCallback);
    }

    private void OnLeaderboardDataReceived(GetLeaderboardResult result)
    {
        if (result.Leaderboard.Count == 0)
        {
            // No data exists for this leaderboard, save initial user count.
            SaveUser(0, false);
        }
        else
        {
            // Data exists, extract the current user count.
            int userCount = result.Leaderboard[0].StatValue;
            SaveUser(userCount, true);
        }
    }


    public void SaveUser(int userCount, bool hasData)
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate> {
            new StatisticUpdate {
                StatisticName = TOTAL_USER_KEY,
                Value = hasData? (userCount + 1) : 1
            }
        }
        }, result => OnStatisticsUpdated(result, hasData ? (userCount + 1) : 1), FailureCallback);
    }

    private void OnStatisticsUpdated(UpdatePlayerStatisticsResult updateResult, int newUserCount)
    {
        SetNickname(newUserCount);
        IsUserLoaded = true;
        Debug.Log("Successfully submitted user count");
    }
    private void SetNickname(int userCount)
    {
        NickName = $"Guest{userCount}";
        SetPlayerData("NickName", NickName);
    }


    private void FailureCallback(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your API call. Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }
}
