using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class GameModel : IGameModelObserver
{
    public ReactiveProperty<int> Score;
    public ReactiveProperty<GameState> GameState;


    public IReadOnlyReactiveProperty<int> ScoreProperty { get => Score; }

    public IReadOnlyReactiveProperty<GameState> GameStateProperty { get => GameState; }

    public void Dispose()
    {
        Score?.Dispose();
        GameState?.Dispose();
    }
}


public interface IGameModelObserver
{
    public IReadOnlyReactiveProperty<int> ScoreProperty { get; }

    public IReadOnlyReactiveProperty<GameState> GameStateProperty { get; }
}
   

