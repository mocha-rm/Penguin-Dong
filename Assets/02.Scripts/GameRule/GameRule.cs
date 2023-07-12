using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VContainer;
using VContainer.Unity;

using MessagePipe;
using UniRx;
using Cysharp.Threading.Tasks;



public enum GameState { Playing, GameOver }
public partial class GameRule : IInitializable, IDisposable
{
    [Inject] IObjectResolver _container;

    GameModel _model;

    //Controller
    PlayerController _playerController;
    InGameUIController _uiController;

    //Sub
    IDisposable _disposable;



    public void Initialize()
    {
        _playerController = _container.Resolve<PlayerController>();
        _uiController = _container.Resolve<InGameUIController>();

        if (_model == null)
        {
            _model = CreateModel();

        }
        GameRunning().Forget();

        

        var bag = DisposableBag.CreateBuilder();

        SubscribePlayerTurnEvent().AddTo(bag);

        _disposable = bag.Build();
    }

    private async UniTaskVoid GameRunning() //게임시작과 동시에 계속 도는 것 들?
    {
        int score = 0;

        while(true)
        {
            score++;

            await UniTask.Delay(TimeSpan.FromMilliseconds(100));

            if(_model.GameState.Value == GameState.GameOver)
            {
                Debug.Log(score.ToString());
                break;
            }
        }
    }
    


    public void Dispose()
    {
        _disposable?.Dispose();
        _model?.Dispose();
        _model = null;
    }


    private GameModel CreateModel()
    {
        return new GameModel()
        {
            Score = new ReactiveProperty<int>(0),
            GameState = new ReactiveProperty<GameState>(GameState.Playing),
        };
    }

    public static class Constants { }
}
