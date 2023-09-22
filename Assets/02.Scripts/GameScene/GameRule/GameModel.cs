using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;


namespace GameScene.Rule
{
    public class GameModel : IGameModel
    {
        public ReactiveProperty<int> Score;
        public ReactiveProperty<int> Coin;
        public ReactiveProperty<int> Life;
        public ReactiveProperty<int> Level;
        public ReactiveProperty<GameState> GameState;


        public IReadOnlyReactiveProperty<int> ScoreProperty { get => Score; }
        public IReadOnlyReactiveProperty<int> CoinProperty { get => Coin; }
        public IReadOnlyReactiveProperty<int> LifeProperty { get => Life; }
        public IReadOnlyReactiveProperty<int> LevelProperty { get => Level; }
        public IReadOnlyReactiveProperty<GameState> GameStateProperty { get => GameState; }

        public void Dispose()
        {
            Score?.Dispose();
            Coin?.Dispose();
            Life?.Dispose();
            Level?.Dispose();
            GameState?.Dispose();
        }
    }


    public interface IGameModel
    {
        public IReadOnlyReactiveProperty<int> ScoreProperty { get; }
        public IReadOnlyReactiveProperty<int> CoinProperty { get; }
        public IReadOnlyReactiveProperty<int> LifeProperty { get; }
        public IReadOnlyReactiveProperty<int> LevelProperty { get; }
        public IReadOnlyReactiveProperty<GameState> GameStateProperty { get; }
    }
}


