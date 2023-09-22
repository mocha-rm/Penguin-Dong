using System;
using UnityEngine;

using VContainer;
using VContainer.Unity;

using MessagePipe;
using UniRx;
using Cysharp.Threading.Tasks;
using GameScene.Player;
using GameScene.UI;
using GameScene.Obstacle;

namespace GameScene.Rule
{
    public enum GameState { Waiting, Playing, GameOver }

    public partial class GameRule : IInitializable, IDisposable
    {
        public IGameModel Model { get { return _model; } }

        GameModel _model;

        [Inject] IObjectResolver _container;


        //Controller
        PlayerController _playerController;
        ObstacleController _obstacleController;
        InGameUIController _uiController;

        //Sub
        IDisposable _disposable;


        [Inject]
        void Construct() //Initialize 함수보다 호출순서가 빠름 System적으로 맨 처음 있어야 될 것들은 Construct 함수에서 호출 해주면 될 것 같다
        {
            _model = CreateModel();
        }


        public void Initialize()
        {
            _playerController = _container.Resolve<PlayerController>();
            _obstacleController = _container.Resolve<ObstacleController>();
            _uiController = _container.Resolve<InGameUIController>();


            GameRunning().Forget();

            //ISubScriber
            var bag = DisposableBag.CreateBuilder();

            SubscribePlayerTurnEvent().AddTo(bag);

            SubscribeCountDownCompleteEvent().AddTo(bag);

            SubscribeGameOverEvent().AddTo(bag);

            SubscribeObstacleCrashEvent().AddTo(bag);

            SubscribeSceneLoadEvent().AddTo(bag);

            _disposable = bag.Build();
        }

        private async UniTaskVoid GameRunning()
        {
            await UniTask.WaitUntil(() => _model.GameState.Value == GameState.Playing);

            while (true)
            {
                int level = _model.Score.Value / 100 + 1;
                int duration = Mathf.FloorToInt(Constants.Duration * Mathf.Pow(Constants.DurationRatePerLevel, _model.Score.Value));

                await UniTask.Delay(Mathf.Max(Constants.MinDuration, duration)); //TODO: Change when level up timing later

                if (_model.GameState.Value == GameState.GameOver)
                {
                    break;
                }

                _obstacleController.SpawnObstacles(1);
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
                GameState = new ReactiveProperty<GameState>(GameState.Waiting),
            };
        }

        public static class Constants
        {
            public static int Duration = 1000;
            public static int MinDuration = 200;
            public static float DurationRatePerLevel = 0.9f;
        }
    }
}

