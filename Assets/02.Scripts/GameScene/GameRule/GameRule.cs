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
using GameScene.Message;

namespace GameScene.Rule
{
    public enum GameState { Waiting, Playing, GameOver }

    public partial class GameRule : IInitializable, IDisposable
    {
        public IGameModel Model { get { return _model; } }

        GameModel _model;

        [Inject] IObjectResolver _container;

        IPublisher<GameOverEvent> _gameOverPub;
        double _reducedTime = 0;

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
            _gameOverPub = _container.Resolve<IPublisher<GameOverEvent>>();

            GameRunning().Forget();

            _model.Life.AsObservable()
                .Where(_ => _model.Life.Value <= 0)
                .Subscribe(_ =>
                {
                    _gameOverPub.Publish(new GameOverEvent()
                    {

                    });
                });


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
            _model.Level.Value = Constants.DefaultLevel;
            float levelGuage = 0f;

            await UniTask.WaitUntil(() => _model.GameState.Value == GameState.Playing);

            while (true)
            {
                await UniTask.Delay(TimeSpan.FromMilliseconds((Constants.DefaultTime - _reducedTime) * 1000));

                levelGuage += 0.1f;

                if(levelGuage >= 1.0f)
                {
                    _model.Level.Value += 1;
                    levelGuage -= 1.0f;

                    if(_reducedTime < 0.8f)
                    {
                        _reducedTime += 0.2f;
                    }
                }

                _uiController.LevelUIAction(levelGuage, _model.Level.Value);
                
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
                Life = new ReactiveProperty<int>(Constants.DefaultLifeCount),
                Level = new ReactiveProperty<int>(Constants.DefaultLevel),
                GameState = new ReactiveProperty<GameState>(GameState.Waiting),
            };
        }



        public static class Constants
        {
            public static int DefaultLifeCount = 3;
            public static int DefaultLevel = 1;

            public static double DefaultTime = 1.0;
        }
    }
}

