using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;


public enum AbilityNames
{
    Exp,
    Freeze,
    Heal,
    Heart,
    Lotto,
    Shield,
    Shoes,
    SkinUpgrade,
    NONE
}

namespace GameScene.Rule
{
    public class GameModel : IGameModel //게임시작 할 때 생성 (1회용)
    {
        public Dictionary<string, float> Abilities;

        public ReactiveProperty<int> Score;
        public ReactiveProperty<int> Coin;
        public ReactiveProperty<float> Life;
        public ReactiveProperty<int> Level;
        public ReactiveProperty<GameState> GameState;


        public IReadOnlyReactiveProperty<int> ScoreProperty { get => Score; }
        public IReadOnlyReactiveProperty<int> CoinProperty { get => Coin; }
        public IReadOnlyReactiveProperty<float> LifeProperty { get => Life; }
        public IReadOnlyReactiveProperty<int> LevelProperty { get => Level; }
        public IReadOnlyReactiveProperty<GameState> GameStateProperty { get => GameState; }
        public Dictionary<string, float> AbilitiesProperty { get => Abilities; }

        public void Dispose()
        {
            Score?.Dispose();
            Coin?.Dispose();
            Life?.Dispose();
            Level?.Dispose();
            GameState?.Dispose();
            Abilities?.Clear();
        }

        public void SetAbilities()
        {
            Abilities = new Dictionary<string, float>()
            {
                {AbilityNames.Exp.ToString(), 1.0f },
                {AbilityNames.Freeze.ToString(), 0f },
                {AbilityNames.Heal.ToString(), 0f },
                {AbilityNames.Heart.ToString(), 0f },
                {AbilityNames.Lotto.ToString(), 0f },
                {AbilityNames.Shield.ToString(), 0f },
                {AbilityNames.Shoes.ToString(), 0f },
                {AbilityNames.SkinUpgrade.ToString(), 0f}
            };
        }
    }


    public interface IGameModel
    {
        public IReadOnlyReactiveProperty<int> ScoreProperty { get; }
        public IReadOnlyReactiveProperty<int> CoinProperty { get; }
        public IReadOnlyReactiveProperty<float> LifeProperty { get; }
        public IReadOnlyReactiveProperty<int> LevelProperty { get; }
        public IReadOnlyReactiveProperty<GameState> GameStateProperty { get; }
        public Dictionary<string, float> AbilitiesProperty { get; }
    }
}


