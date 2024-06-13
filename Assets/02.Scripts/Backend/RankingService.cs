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


public class RankingService : IInitializable, IDisposable
{
    [Inject] IObjectResolver _container;

    IDisposable _disposable;



    public void Initialize()
    {
        //GetLeaderboarder("BestScore", SetRanking); // call when push leaderboard button at lobbyscene
    }


    public void Dispose()
    {
        _disposable?.Dispose();
        _disposable = null;
    }


    private void DisplayPlayfabError(PlayFabError error) => Debug.LogError("error : " + error.GenerateErrorReport());

    public void GetLeaderboarder(string name, Action<GetLeaderboardResult> callBack)
    {
        var requestLeaderboard = new GetLeaderboardRequest
        {
            StartPosition = 0,
            StatisticName = name,
            MaxResultsCount = 100,

            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowLocations = true,
                ShowDisplayName = true,
                ShowStatistics = true
            }
        };

        PlayFabClientAPI.GetLeaderboard(requestLeaderboard, callBack, DisplayPlayfabError);
    }


    public void SetRanking(GetLeaderboardResult result)
    {
        var curBoard = result.Leaderboard;

        foreach (PlayerLeaderboardEntry player in curBoard)
        {
            string displayName = player.DisplayName;
            string playFabId = player.PlayFabId;
            int score = player.StatValue;


            Debug.Log("RankingBoard Debug Meesages");
            Debug.Log(displayName);
            Debug.Log(playFabId);
            Debug.Log(score);
        }
    }



    public void SubmitScore(int playerScore) //call when gameoverEvent if get a bestscore
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate> {
            new StatisticUpdate {
                StatisticName = "BestScore",
                Value = playerScore
            }
        }
        }, result => OnStatisticsUpdated(result), FailureCallback);
    }

    private void OnStatisticsUpdated(UpdatePlayerStatisticsResult updateResult)
    {
        Debug.Log("Successfully submitted high score");
    }

    private void FailureCallback(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your API call. Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }
}
