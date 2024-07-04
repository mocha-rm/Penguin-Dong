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
using System.Threading.Tasks;

public class RankingService : IInitializable, IDisposable
{
    [Inject] IObjectResolver _container;

    IDisposable _disposable;



    public void Initialize()
    {
        
    }


    public void Dispose()
    {
        _disposable?.Dispose();
        _disposable = null;
    }


    //Read from Leaderboard

    public void RequestLeaderboard()
    {
        PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest
        {

            StatisticName = "BestScore",
            StartPosition = 0,
            MaxResultsCount = 10,

        }, result => SetRanking(result), FailureCallback);
    }


    private void SetRanking(GetLeaderboardResult result)
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



    //Write to Leaderboard

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
