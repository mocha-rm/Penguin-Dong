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

    private Dictionary<string, int> _rankDic;

    IDisposable _disposable;



    public void Initialize()
    {
        _rankDic = new Dictionary<string, int>();
    }


    public void Dispose()
    {
        _disposable?.Dispose();
        _disposable = null;
    }


    //Read from Leaderboard

    public void RequestLeaderboard(GameObject obj)
    {
        PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest
        {

            StatisticName = "BestScore",
            StartPosition = 0,
            MaxResultsCount = 10,

        }, result => SetRanking(result, obj), FailureCallback);
    }

    
    private void SetRanking(GetLeaderboardResult result, GameObject obj)
    {
        var curBoard = result.Leaderboard;

        foreach (PlayerLeaderboardEntry player in curBoard)
        {
            string playFabId = player.PlayFabId;
            int score = player.StatValue;

            _rankDic[playFabId] = score;
        }
        obj.SetActive(false);
    }

    public Dictionary<string, int> GetRankInfo() => _rankDic;




    //Get My Rank Status in Realtime;;;

    public void GetMyRanking(string myPlayFabId, Action<int> onSuccess, Action<string> onFailure)
    {
        var request = new GetLeaderboardAroundPlayerRequest
        {
            PlayFabId = myPlayFabId,
            StatisticName = "BestScore", // 이 부분은 리더보드 통계 이름으로 대체하세요.
            MaxResultsCount = 1 // 나의 순위만 가져오려면 1로 설정
        };

        PlayFabClientAPI.GetLeaderboardAroundPlayer(request,
            result => OnGetLeaderboardSuccess(result, onSuccess),
            error => OnGetLeaderboardFailure(error, onFailure));
    }

    private void OnGetLeaderboardSuccess(GetLeaderboardAroundPlayerResult result, Action<int> onSuccess)
    {
        var myEntry = result.Leaderboard[0]; // 항상 한 개의 항목만 가져올 것이므로 첫 번째 항목이 본인의 정보
        int myPosition = myEntry.Position;
        Debug.Log("My Position: " + myPosition);
        onSuccess?.Invoke(myPosition); // 콜백 호출
    }

    private void OnGetLeaderboardFailure(PlayFabError error, Action<string> onFailure)
    {
        string errorMessage = error.GenerateErrorReport();
        Debug.LogError("Failed to get leaderboard: " + errorMessage);
        onFailure?.Invoke(errorMessage); // 실패 콜백 호출
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
