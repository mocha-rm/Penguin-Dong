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
        bool isGameOver;
        float levelGuage = 0f;

        [Inject] IObjectResolver _container;

        IPublisher<GameOverEvent> _gameOverPub;
        double _reducedTime = 0;

        AudioService _audioService;
        DBService _dbService;
        LoginService _loginService;

        //Controller
        PlayerController _playerController;
        ObstacleController _obstacleController;
        RoguelikeController _roguelikeController;
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
            _roguelikeController = _container.Resolve<RoguelikeController>();
            _uiController = _container.Resolve<InGameUIController>();
            _gameOverPub = _container.Resolve<IPublisher<GameOverEvent>>();
            _audioService = _container.Resolve<AudioService>();
            _loginService = _container.Resolve<LoginService>();
            _dbService = _container.Resolve<DBService>();

            _model.SetAbilities();

            isGameOver = false;

            GameRunning().Forget();

            _model.Life.AsObservable()
                .Where(_ => _model.Life.Value <= 0f).Where(_ => !isGameOver)
                .Subscribe(_ =>
                {
                    isGameOver = true;

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

            SubscribeScoreUpEvent().AddTo(bag);

            SubscribeRoguelikePayEvent().AddTo(bag);

            SubscribeRoguelikeRefreshEvent().AddTo(bag);

            SubscribeRoguelikeSkipEvent().AddTo(bag);

            _disposable = bag.Build();
        }

        private async UniTaskVoid GameRunning()
        {
            _model.Level.Value = Constants.DefaultLevel;
            
            await UniTask.WaitUntil(() => _model.GameState.Value == GameState.Playing);

            CoreTask(levelGuage).Forget();
        }

        private async UniTaskVoid CoreTask(float levelGuage)
        {
            while (_model.GameState.Value == GameState.Playing)
            {
                await UniTask.Delay(TimeSpan.FromMilliseconds((Constants.DefaultTime - _reducedTime) * 1000));

                levelGuage += (Constants.DefaultLevelAmount/ _model.LevelProperty.Value) * _model.Abilities[AbilityNames.Exp.ToString()];

                if (levelGuage >= 1.0f)
                {
                    _model.Level.Value += 1;
                    levelGuage -= 1.0f;

                    if (_reducedTime < 0.8f)
                    {
                        _reducedTime += 0.1f;
                    }

                    _audioService.Stop(AudioService.SoundType.BGM);
                    _audioService.SetRandomObjectClip();

                    _obstacleController.GetRandomColor();

                    OpenRoguelike();
                }

                _uiController.LevelUIAction(levelGuage, _model.Level.Value);

                if (_model.GameState.Value == GameState.GameOver || _model.GameState.Value == GameState.Waiting)
                {
                    break;
                }

                _obstacleController.SpawnObstacles(1);
                _audioService.RandomObjectPlay();
                //_audioService.Play(AudioService.AudioResources.Fire_Shoot);
            }
        }



        public void Dispose()
        {
            _disposable?.Dispose();
            _model?.Dispose();
            _model = null;
        }

        #region Roguelike

        private void OpenRoguelike()
        {
            if (_model.GameState.Value != GameState.GameOver)
            {
                _model.GameState.Value = GameState.Waiting;
                _roguelikeController.ActivateRoguelike();
                _playerController.SetPlayerInvulnerable(true);
                 //Level Up Particle and sound?
            }
        }

        private void SetAbilityDic()
        {
            Utility.CustomLog.Log(_roguelikeController.GetItem().Name);
            Utility.CustomLog.Log(_roguelikeController.GetItem().Value);
            _model.Abilities[_roguelikeController.GetItem().Name] = _roguelikeController.GetItem().Value;

            //만약에 벨류가 일회용이면 벨류 만큼 있다가 다시 0으로 초기화 하기
            if (_roguelikeController.GetItem().Disposable)
            {
                DisposableAbilityActions(_roguelikeController.GetItem().Name);
                ResetAbilityValue(_roguelikeController.GetItem()).Forget();
            } 
        }
         
        private async UniTaskVoid ResetAbilityValue(Item item)
        {
            await UniTask.Delay(TimeSpan.FromMilliseconds(item.Value));
            _model.Abilities[item.Name] = 0;
            Utility.CustomLog.Log($"{item.Name} : {_model.Abilities[item.Name]}");
        }

        private void DisposableAbilityActions(string name)
        {
            switch (name)
            {
                case "Lotto":
                    _roguelikeController.LotteryAction();
                    break;

                case "Heal":
                    _uiController.HealUIAction(_roguelikeController.GetItem().Value);
                    _model.Life.Value += (Constants.DefaultHP + _model.Abilities[AbilityNames.Heart.ToString()]) * 0.1f;
                    break;

                case "Shield":
                    _playerController.ShieldActivate((int)_roguelikeController.GetItem().Value);
                    break;
            }
        }
        #endregion


        private GameModel CreateModel()
        {
            return new GameModel()
            {
                Score = new ReactiveProperty<int>(0),
                Life = new ReactiveProperty<float>(Constants.DefaultHP),
                Coin = new ReactiveProperty<int>(0),
                Level = new ReactiveProperty<int>(Constants.DefaultLevel),
                GameState = new ReactiveProperty<GameState>(GameState.Waiting),
            };
        }



        public static class Constants
        {
            public static float DefaultHP = 100.0f;
            public static int DefaultLevel = 1;

            public static double DefaultTime = 1.0;
            public static float DefaultLevelAmount = 0.1f;
        }
    }
}

